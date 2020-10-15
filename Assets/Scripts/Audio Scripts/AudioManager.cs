using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Singleton-styled class for playing 2D audio clips.
/// </summary>

public class AudioManager : MonoBehaviour
{
    private const int MaxMusicChannels = 2;
    private int AvailableChannelCount => _musicChannels.Length - _activeChannels.Count;

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

    [SerializeField] private bool showDebug;
    
    private AudioSource _sfxChannel;
    private Dictionary<Sound, AudioSource> _activeChannels;
    private AudioSource[] _musicChannels;
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;
        _sfxChannel = gameObject.AddComponent<AudioSource>();
        _activeChannels = new Dictionary<Sound, AudioSource>();
        _musicChannels = new AudioSource[MaxMusicChannels];

        for (var i = 0; i < _musicChannels.Length; i++)
        {
            Debug.Log(i);
            _musicChannels[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayMusic(Sound sound)
    {
        AudioSource source = GetOpenMusicChannel();
        sound.ApplyToSource(source);
        
        source.Play();
        _activeChannels.Add(sound, source);
    }

    public void StopMusic(Sound sound)
    {
        AudioSource source = _activeChannels[sound];
        source.Stop();

        _activeChannels.Remove(sound);
    }

    public void PlayOneShot(Sound sound)
    {
        _sfxChannel.PlayOneShot(sound.Clip);
    }

    private AudioSource GetOpenMusicChannel()
    {
        if (AvailableChannelCount <= 0)
        {
            Debug.LogWarning("No more music channels can be opened.");
            Debug.Break();
        }

        var openChannels =
            from channel in _musicChannels
            where channel.isPlaying == false
            select channel;
        
        return openChannels.First();
    }

    private Rect _windowRect = new Rect(Screen.width - 200, 0, 150, 100);
    private bool _showingAllSounds = true;
    
    private void OnGUI()
    {
        if (!showDebug) return;
        
        _windowRect = GUILayout.Window(0, _windowRect, HandleWindow, "AudioManager");        
    }

    private void HandleWindow(int id)
    {
        
        GUILayout.Label("Available sound channels: " + AvailableChannelCount);

        _showingAllSounds = GUILayout.Toggle(_showingAllSounds, "Active sounds: " + _activeChannels.Count);
        
        if (_showingAllSounds)
        {
            foreach (var sound in _activeChannels.Keys)
            {
                GUILayout.Label("    " + sound.name);
            }
        }
        
        GUI.DragWindow();
    }
}
