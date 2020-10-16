using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Singleton-styled class for starting and stopping 2D audio clips.
/// <para>Lazily creates new AudioSource channels for SFX and music playback</para>
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField, Tooltip("Display the debug window for the AudioManager")] 
    private bool showDebug;
    
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
    private HashSet<AudioSource> _channels;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;
        _channels = new HashSet<AudioSource>();
    }

    /// <summary>
    /// Start playing a non-loopable 2D sound.
    /// </summary>
    /// <param name="sound"></param>
    public void PlayOneShot(Sound sound)
    {
        // SFX are fine all living on the same channel - if a channel is already 
        // playing this sound, just take it over and interrupt that sound.
        AudioSource channel = GetCurrentlyPlaying(sound.Clip, true);
        
        // Apply the sound's modulation and initial parameters.
        StartCoroutine(sound.ApplyToChannel(channel));
        
        // One-shots cannot loop, but that's OK for SFX.
        channel.PlayOneShot(sound.Clip);
    }
    
    /// <summary>
    /// Start playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be played</param>
    public void PlayLoopable(Sound sound)
    {
        // Loopable sounds probably won't sound good interrupting each other,
        // so for now they each get their own channel.
        AudioSource source = GetOpenChannel();
        
        // Apply modulation and initialize audio source.
        StartCoroutine(sound.ApplyToChannel(source));
        source.Play();
    }

    /// <summary>
    /// Stop playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be stopped, if its playing</param>
    public void StopLoopable(Sound sound)
    {
        // Try and find the requested sound, if its playing.
        AudioSource source = GetCurrentlyPlaying(sound.Clip);
        
        if (source != null) 
            // TODO: add more ways to stop loopable music, e.g. fade out or cross-fade, or other cool transitions
            source.Stop();
    }

    /// <summary>
    /// Find an audio channel that isn't currently playing anything
    /// </summary>
    /// <returns>An audio channel that isn't being used</returns>
    private AudioSource GetOpenChannel()
    {
        AudioSource result;
        try
        {
            var openChannels =
                from channel in _channels
                where !channel.isPlaying
                select channel;
            
            result = openChannels.First();
        }
        catch (InvalidOperationException)
        {
            // If no open channels exist, make a new one and return it
            result = gameObject.AddComponent<AudioSource>();
            _channels.Add(result);
        }

        return result;
    }

    /// <summary>
    /// Find an audio channel that is playing a certain clip
    /// </summary>
    /// <param name="target">The clip to search for</param>
    /// <param name="getIfNone">Make a new channel if the target is missing</param>
    /// <returns></returns>
    private AudioSource GetCurrentlyPlaying(AudioClip target, bool getIfNone = false)
    {
        AudioSource result = null;
        try
        {
            var openChannels =
                from channel in _channels
                where channel.clip.Equals(target)
                select channel;

            result = openChannels.First();
        }
        catch (InvalidOperationException)
        {
            if (getIfNone)
                result = GetOpenChannel();
        }
        
        return result;
    }

    #region gui / editor code

    private Rect _windowRect = new Rect(Screen.width - 300, 0, 250, 75);
    private bool _showingAllSounds = false;

    private void OnGUI()
    {
        if (!showDebug) return;
        _windowRect = GUILayout.Window(0, _windowRect, HandleWindow, "AudioManager");
    }

    private void HandleWindow(int id)
    {
        GUILayout.Label("Current sound channels: " + _channels.Count);

        _showingAllSounds = GUILayout.Toggle(_showingAllSounds, "Show sounds");
    
        if (_showingAllSounds)
        {
            for (var i = 0; i < _channels.Count; i++)
            {
                GUILayout.Label($"Channel {i + 1}: {GetChannelInfo(_channels.ElementAt(i))}");
            }
        }
        else
        {
            _windowRect.width = 250;
            _windowRect.height = 75;
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