﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Singleton-styled class for starting and stopping audio clips
/// <para>Lazily creates new AudioSource channels for SFX and music playback</para>
/// </summary>

//TODO: add more AudioMixer integration, e.g. snapshot blending?!?!??? Its annoying that we cant cache it

public class AudioManager : MonoBehaviour
{
    [SerializeField, Tooltip("Display the debug window for the AudioManager")] 
    private bool showDebug;

    private int SourcesCount => _channels.Values.Count;
    private int TotalChannelCount => _channels.Values.Sum(channels => channels.Count);
    private int ActiveChannelCount => _channels.Values.Sum(channels => channels.Sum(channel => channel.isPlaying ? 1 : 0));
    
    public static AudioManager Instance()
    {
        if (_instance == null)
        {
            _instance = new GameObject(
                    "Audio Manager", 
                    typeof(AudioManager))
                .GetComponent<AudioManager>();
        }
    
        return _instance;
    }
    private static AudioManager _instance = null;
    private GameObject _globalTarget = null;
    private Dictionary<GameObject, HashSet<AudioSource>> _channels;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;
        _globalTarget = new GameObject("Global Target");
        _globalTarget.transform.parent = transform;
        _channels = new Dictionary<GameObject, HashSet<AudioSource>>();
    }

    /// <summary>
    /// Start playing a selected sound, using its loopable property to determine playback method.
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="target">What GameObject this sound should originate from</param>
    public void PlaySound(Sound sound, GameObject target)
    {
        if (sound.IsLooping)
        {
            PlayLoopable(sound, target);
        }
        else
        {
            PlayOneShot(sound, target);
        }
    }
    public void PlaySound(Sound sound)
    {
        PlaySound(sound, _globalTarget);
    }

    /// <summary>
    /// Start playing a non-loopable sound
    /// </summary>
    /// <param name="sound">The sound that will be played</param>
    /// <param name="target">The GameObject to start playing this sound</param>
    private void PlayOneShot(Sound sound, GameObject target)
    {
         // SFX are fine all living on the same channel - if a channel is already 
        // playing this sound, just take it over and interrupt that sound.
        AudioSource channel = GetCurrentlyPlaying(target, sound.GetClip, true);   
        
        if (!target.Equals(_globalTarget))
        {
            channel.spatialBlend = 1;
        }
        
        // Apply the sound's modulation and initial parameters.
        StartCoroutine(sound.ApplyToChannel(channel));
        
        // One-shots cannot loop, but that's OK for SFX.
        channel.PlayOneShot(sound.GetClip);
    }

    /// <summary>
    /// Start playing a loopable sound
    /// </summary>
    /// <param name="sound">The sound that will be played</param>
    /// <param name="target">The GameObject to start playing this sound</param>
    private void PlayLoopable(Sound sound, GameObject target)
    {
        // Loopable sounds probably won't sound good interrupting each other,
        // so for now they each get their own channel.
        AudioSource channel = GetOpenChannel(target);
        
        if (!target.Equals(_globalTarget))
        {
            channel.spatialBlend = 1;
        }
        
        // Apply modulation and initialize audio source.
        StartCoroutine(sound.ApplyToChannel(channel));
        channel.Play();
    }

    /// <summary>
    /// Stop playing a loopable sound
    /// </summary>
    /// <param name="sound">The sound that will be stopped, if its playing</param>
    /// <param name="target">The GameObject to stop playing this sound</param>
    public void StopSound(Sound sound, GameObject target)
    {
        // Try and find the requested sound, if its playing.
        AudioSource source = GetCurrentlyPlaying(target, sound.GetClip);
        
        if (source != null) 
            source.Stop();
    }
    public void StopSound(Sound sound)
    {
        StopSound(sound, _globalTarget);
    }

    /// <summary>
    /// Updates the mixer's volume
    /// </summary>
    /// <param name="mixer">The mixer to apply the volume to</param>
    /// <param name="volume">The desired mixer volume (0 being mute, 1 being full)</param>
    public static void SetVolume(AudioMixer mixer, float volume)
    {
        var trueVolume = Mathf.Clamp(volume, 0.0001f, 1f);
        mixer.SetFloat("MasterVolume", Mathf.Log10(trueVolume) * 20);
    }

    /// <summary>
    /// Disposes of all non-global GameObjects being tracked.
    /// <remarks>This should be called before ever changing scenes; when we get
    ///  an actual scene management system, attach this method to an event it calls</remarks>
    /// </summary>
    public void Cleanup()
    {
        foreach (var target in _channels.Keys.Reverse())
        {
            if (target != _globalTarget) _channels.Remove(target);
        }
    }

    /// <summary>
    /// Find an audio channel that isn't currently playing anything
    /// </summary>
    /// <returns>An audio channel that isn't being used</returns>
    private AudioSource GetOpenChannel(GameObject target)
    {
        AudioSource result;
        try
        {
            var openChannels =
                from channel in _channels[target]
                where !channel.isPlaying
                select channel;
            
            result = openChannels.First();
        }
        catch (Exception)
        {
            // If no open channels exist, make a new one and return it
            result = target.AddComponent<AudioSource>();

            if (!_channels.ContainsKey(target))
            {
                _channels.Add(target, new HashSet<AudioSource>());
            }
            _channels[target].Add(result);
        }
        return result;
    }

    /// <summary>
    /// Find an audio channel that is playing a certain clip
    /// </summary>
    /// <param name="target">The GameObject to check for this clip</param>
    /// <param name="clip">The clip to search for</param>
    /// <param name="getIfNone">Make a new channel if the target is missing</param>
    /// <returns></returns>
    private AudioSource GetCurrentlyPlaying(GameObject target, AudioClip clip, bool getIfNone = false)
    {
        AudioSource result = null;
        try
        {
            var openChannels =
                from channel in _channels[target]
                where channel.clip.Equals(clip)
                select channel;

            result = openChannels.First();
        }
        catch (Exception)
        {
            if (getIfNone)
                result = GetOpenChannel(target);
        }
        
        return result;
    }
    
    #region gui / editor code

    private Rect _windowRect = new Rect(Screen.width - 300, 0, 250, 75);
    private readonly List<bool> _showSounds = new List<bool>();

    private void OnGUI()
    {
        if (!showDebug) return;
        _windowRect = GUILayout.Window(0, _windowRect, HandleWindow, "AudioManager");
    }

    private void HandleWindow(int id)
    {
        while (_showSounds.Count < _channels.Count)
        {
            _showSounds.Add(false);
        }
        
        GUILayout.Label("Sources: " + SourcesCount);
        GUILayout.Label("Active channels: " + ActiveChannelCount + " / " + TotalChannelCount);
        for (var targetIndex = 0; targetIndex < _channels.Count; targetIndex++)
        {
            var target = _channels.ElementAt(targetIndex);
            _showSounds[targetIndex] = GUILayout.Toggle(_showSounds[targetIndex], target.Key.name);
            
            if (!_showSounds[targetIndex]) continue;
            for (var channelIndex = 0; channelIndex < target.Value.Count; channelIndex++)
            {
                GUILayout.Label($"Channel {channelIndex + 1}: {GetChannelInfo(target.Value.ElementAt(channelIndex))}");
            }
        }

        if (!_showSounds.Contains(true))
        {
            _windowRect.width = 250;
            _windowRect.height = 75 + (25 * _showSounds.Count);
        }

        GUI.DragWindow();
    }

    private static string GetChannelInfo(AudioSource source)
    {
        StringBuilder result = new StringBuilder();
        result.AppendLine(source.clip.name);
        result.AppendLine("\tPitch: " + source.pitch);
        result.AppendLine("\tVolume: " + source.volume);
        result.AppendLine("\tPan: " + source.panStereo);
        return result.ToString();
    }

    #endregion
}