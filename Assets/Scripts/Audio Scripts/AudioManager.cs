using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
    private Dictionary<GameObject, HashSet<Channel>> _channels;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;
        _globalTarget = new GameObject("Global Target");
        _globalTarget.transform.parent = transform;
        _channels = new Dictionary<GameObject, HashSet<Channel>>();
    }

    /// <summary>
    /// Start playing a selected sound, using its loopable property to determine playback method.
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="target">What GameObject this sound should originate from</param>
    public void PlaySound(SoundInstance sound, GameObject target)
    {
        Channel channel;
        
        if (sound.IsLooping)
        {
            channel = GetOpenChannel(target);
        }
        else
        {
            channel = GetCurrentlyPlaying(target, sound) ?? GetOpenChannel(target);
        }
        
        // Make the sound 3D if its not targeting the global target.
        sound.SetValue(SoundValue.SpacialBlend, target.Equals(_globalTarget) ? 0f : Mathf.Min(1f, sound.GetValue(SoundValue.SpacialBlend)));

        StartCoroutine(channel.Play(sound));
    }
    public void PlaySound(SoundInstance sound)
    {
        PlaySound(sound, _globalTarget);
    }

    /// <summary>
    /// Stop playing a loopable sound
    /// </summary>
    /// <param name="sound">The sound that will be stopped, if its playing</param>
    /// <param name="target">The GameObject to stop playing this sound</param>
    public void StopSound(SoundInstance sound, GameObject target)
    {
        Channel channel = GetCurrentlyPlaying(target, sound);

        if (channel != null)
            StartCoroutine(channel.Stop());
    }
    public void StopSound(SoundInstance sound)
    {
        StopSound(sound, _globalTarget);
    }

    /// <summary>
    /// Updates the mixer's volume
    /// </summary>
    /// <param name="mixer">The mixer to apply the volume to</param>
    /// <param name="volume">The desired mixer volume (0 being mute, 1 being full)</param>
    public static void SetMasterVolume(AudioMixer mixer, float volume)
    {
        var trueVolume = Mathf.Clamp(volume, 0.0001f, 1f);
        mixer.SetFloat("MasterVolume", Mathf.Log10(trueVolume) * 20);
    }

    /// <summary>
    /// Disposes of all non-global GameObjects being tracked.
    /// <remarks>This should be called before ever changing scenes; when we get
    ///  an actual scene management system, attach this method to an event it calls</remarks>
    /// </summary>
    public void DisposeAll()
    {
        foreach (var target in _channels.Keys.Reverse())
        {
            if (target == _globalTarget) continue;
            
            foreach (var channel in _channels[target])
            {
                StartCoroutine(channel.Stop());
            }
            _channels.Remove(target);
        }
    }
    
    /// <summary>
    /// Removes a GameObject from the AudioManager system.
    /// <remarks>This should be called before the object is destroyed so the AudioManager knows to stop updating it</remarks>
    /// </summary>
    public void Dispose(GameObject target)
    {
        if (!_channels.ContainsKey(target)) return;
        
        foreach (var channel in _channels[target])
        {
            StartCoroutine(channel.Stop());
        }
        _channels.Remove(target);
    }
    public void Dispose(GameObject target, float time)
    {
        StartCoroutine(HelperDispose(target, time));
    }
    private IEnumerator HelperDispose(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        Dispose(target);
    }
    
    /// <summary>
    /// Find an audio channel that isn't currently playing anything
    /// </summary>
    /// <returns>An audio channel that isn't being used</returns>
    private Channel GetOpenChannel(GameObject target)
    {
        Channel result;
        try
        {
            var openChannels =
                from channel in _channels[target]
                where channel.IsAvailable
                select channel;
            
            result = openChannels.First();
        }
        catch (Exception)
        {
            // If no open channels exist, make a new one and return it
            result = new Channel(target.AddComponent<AudioSource>(), target.AddComponent<AudioSource>());

            if (!_channels.ContainsKey(target))
                _channels.Add(target, new HashSet<Channel>());

            _channels[target].Add(result);
        }
        return result;
    }

    /// <summary>
    /// Find an audio channel that is attached to a Sound
    /// </summary>
    /// <param name="target">The GameObject to check for this sound</param>
    /// <param name="sound">The sound to search for</param>
    /// <returns></returns>
    [CanBeNull]
    private Channel GetCurrentlyPlaying(GameObject target, SoundInstance sound)
    {
        Channel result = null;
        try
        {
            var channels =
                from channel in _channels[target]
                where channel.HasSound(sound)
                select channel;

            result = channels.First();                
        }
        catch (KeyNotFoundException) { /* We are not tracking the target */ }
        catch (InvalidOperationException) { /* The target is tracked, but doesn't have the sound */ }

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
        for (var targetIndex = 0; targetIndex < _channels.Count; targetIndex++)
        {
            var target = _channels.ElementAt(targetIndex);
            _showSounds[targetIndex] = GUILayout.Toggle(_showSounds[targetIndex], target.Key.name);
            
            if (!_showSounds[targetIndex]) continue;
            for (var channelIndex = 0; channelIndex < target.Value.Count; channelIndex++)
            {
                GUILayout.Label($"Channel {channelIndex + 1}: {target.Value.ElementAt(channelIndex)}");
            }
        }

        if (!_showSounds.Contains(true))
        {
            _windowRect.width = 250;
            _windowRect.height = 75 + (25 * _showSounds.Count);
        }

        GUI.DragWindow();
    }
    
    #endregion
}