using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private Sound music = default;
    [SerializeField] private bool stopPrevious = false;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private float fadeTime = 5;
    
    private SoundInstance _stopped;
    private SoundInstance _music;
    public static SoundInstance _currentPlaying;
    private AudioManager _audioManager;
    
    private void Awake()
    {
        _music = music == null ? null : music.GenerateInstance();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance();
        if (playOnStart)
        {
            if (stopPrevious)
            {
                StartCoroutine(StopPrevious(_currentPlaying));
            }
            else
            {
                PlayMusic();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
    
        if (stopPrevious)
        {
            StartCoroutine(StopPrevious(_currentPlaying));
        }
        else
        {
            PlayMusic();
        }
    }

    private void OnDestroy()
    {
        if (_stopped != null && !_stopped.IsInactive) _audioManager.StopSound(_stopped);
    }

    private void PlayMusic()
    {
        if (music != null)
        {
            if (_currentPlaying != null && _currentPlaying.Name != _music.Name)
            {
                // if there is music playing, but its different... change it
                _audioManager.PlaySound(_music);
                _currentPlaying = _music;    
            }
            else if (_currentPlaying == null)
            {
                // if there is no music playing... change it
                _audioManager.PlaySound(_music);
                _currentPlaying = _music;    
            }
            
            // if there is music playing, but its the same, don't do anything
        }
    }

    private IEnumerator StopPrevious(SoundInstance music)
    {
        var elapsed = 0f;
        var initialVolume = music.GetValue(SoundValue.Volume);
        _stopped = music;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            music.Lerp(SoundValue.Volume, initialVolume, 0, elapsed / fadeTime);
            yield return new WaitForEndOfFrame();
        }
        
        _audioManager.StopSound(music);
        PlayMusic();
    }
}
