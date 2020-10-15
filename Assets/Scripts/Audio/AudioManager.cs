using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Singleton-styled class for playing 2D audio clips.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // How many channels should be created for playing loopable music.
    
    // More channels is not bad, but playing multiple long tracks at the same time
    // takes up a lot of memory, best practice to keep them to a minimum.
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

    [SerializeField, Tooltip("Display the debug window for the AudioManager")] 
    private bool showDebug;

    // The source used for playing sound effects, aka one-shots
    private AudioSource _sfxChannel;
    // The sources used for playing looping sound effects, such as music or ambience
    private AudioSource[] _musicChannels;
    private Dictionary<Sound, AudioSource> _activeChannels;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        showDebug = Debug.isDebugBuild;

        // Initialize AudioSources
        _sfxChannel = gameObject.AddComponent<AudioSource>();
        _activeChannels = new Dictionary<Sound, AudioSource>();
        _musicChannels = new AudioSource[MaxMusicChannels];

        for (var i = 0; i < _musicChannels.Length; i++)
        {
            _musicChannels[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// Start playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be played</param>
    public void PlayMusic(Sound sound)
    {
        AudioSource source = GetOpenMusicChannel();
        sound.ApplyToSource(source);
        source.Play();

        _activeChannels.Add(sound, source);
    }

    /// <summary>
    /// Stop playing a loopable 2D sound.
    /// </summary>
    /// <param name="sound">The sound that will be stopped, if its playing</param>
    public void StopMusic(Sound sound)
    {
        AudioSource source = _activeChannels[sound];
        source.Stop();

        _activeChannels.Remove(sound);
    }

    /// <summary>
    /// Play a non-looping 2D sound effect
    /// </summary>
    /// <param name="sound"></param>
    public void PlayOneShot(Sound sound)
    {
        sound.ApplyToSource(_sfxChannel);
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