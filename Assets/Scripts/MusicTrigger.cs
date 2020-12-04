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
    
    private SoundInstance _music;
    private static SoundInstance _currentPlaying;
    private AudioManager _audioManager;
    
    private void Awake()
    {
        _music = music == null ? null : music.GenerateInstance();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance();
        if (playOnStart) PlayMusic();
    }

    private void OnTriggerEnter(Collider other)
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

    private void PlayMusic()
    {
        if (music != null && _currentPlaying != _music) _audioManager.PlaySound(_music);
        _currentPlaying = _music;
    }

    private IEnumerator StopPrevious(SoundInstance music)
    {
        var elapsed = 0f;
        var initialVolume = music.GetValue(SoundValue.Volume);
        
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
