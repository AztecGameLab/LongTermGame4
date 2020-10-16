using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Singleton-styled class for playing 2D audio clips.
/// </summary>
public class AudioManager : MonoBehaviour
{
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

    [SerializeField, Tooltip("Display the debug window for the AudioManager")] 
    private bool showDebug;

    private HashSet<AudioSource> _channels;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;

        // Initialize AudioSources
        _channels = new HashSet<AudioSource>();
    }

    /// <summary>
    /// Start playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be played</param>
    public void PlayMusic(Sound sound)
    {
        AudioSource source = GetOpenChannel();
        StartCoroutine(sound.ApplyToSource(source));
        source.Play();
    }

    /// <summary>
    /// Stop playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be stopped, if its playing</param>
    public void StopMusic(Sound sound)
    {
        AudioSource source = GetCurrentlyPlaying(sound.Clip);
        if (source != null) 
            source.Stop();
    }

    /// <summary>
    /// Play a non-looping 2D sound effect.
    /// </summary>
    /// <param name="sound"></param>
    public void PlayOneShot(Sound sound)
    {
        AudioSource source = GetCurrentlyPlaying(sound.Clip, true);
        StartCoroutine(sound.ApplyToSource(source));
        
        source.PlayOneShot(sound.Clip);
    }

    // Find a channel that's not currently playing music
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
            result = gameObject.AddComponent<AudioSource>();
            _channels.Add(result);
        }

        return result;
    }

    // Find a channel playing the specified clip
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

    // IMGUI Variables
    private Rect _windowRect = new Rect(Screen.width - 300, 0, 250, 75);
    private bool _showingAllSounds = false;

    // Drawing the IMGUI window
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
}