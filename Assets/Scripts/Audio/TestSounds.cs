using System;
using System.Media;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TestSounds : MonoBehaviour
{
    [SerializeField] private Sound arrowHit = default;
    [SerializeField] private Sound arrowShootIce = default;
    [SerializeField] private Sound arrowShootFire = default;
    [SerializeField] private Sound spacialMusic = default;
    [SerializeField] private Sound music = default;
    
    [SerializeField] private float spamTime = -1;
    [SerializeField] private Sound spamSound = default;
    
    private AudioManager _audioManager;
    private bool _playingMusicA = false;
    private bool _playingMusicB = false;
    private float _lastSpammed;
    private float _fadeCompletion;
    private bool _fadingOut;
    private bool CanSpam => Time.time - _lastSpammed > spamTime;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
        _playingMusicA = false;
        _lastSpammed = Time.time;
        _fadeCompletion = 1;
        _fadingOut = false;
    }

    private void Update()
    {
        if (spamTime > 0 && CanSpam)
        {
            _lastSpammed = Time.time;
            _audioManager.PlayOneShot(spamSound);
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("arrow-hit"))
        {
            _audioManager.PlayOneShot(arrowHit);
        }

        if (GUILayout.Button("arrow-shoot-ice"))
        {
            _audioManager.PlayOneShot(arrowShootIce);
        }
    
        if (GUILayout.Button("arrow-shoot-fire"))
        {
            _audioManager.PlayOneShot(arrowShootFire);
        }
        
        if (GUILayout.Button("3D arrow-hit"))
        {
            _audioManager.PlayOneShot(arrowHit, gameObject);
        }

        if (_playingMusicA != GUILayout.Toggle(_playingMusicA, "music A"))
        {
            _playingMusicA = !_playingMusicA;

            if (_playingMusicA)
            {
                _audioManager.PlayLoopable(spacialMusic);
            }
            else
            {
                _audioManager.StopLoopable(spacialMusic);
            }            
        }

        if (_playingMusicB != GUILayout.Toggle(_playingMusicB, "music B"))
        {
            _playingMusicB = !_playingMusicB;

            if (_playingMusicB)
            {
                _audioManager.PlayLoopable(music);
            }
            else
            {
                _audioManager.StopLoopable(music);
            }            
        }
        
        _fadingOut = GUILayout.Toggle(_fadingOut, "fade out");
        
        if(_fadingOut)
        {
            _fadeCompletion = Mathf.Clamp01(_fadeCompletion - (Time.deltaTime * 2f));
        }
        else
        {
            _fadeCompletion = Mathf.Clamp01(_fadeCompletion + (Time.deltaTime * 2f));
        }

        spacialMusic.Lerp(SoundSetting.Volume, 0, 1, _fadeCompletion);
        spacialMusic.Lerp(SoundSetting.Pitch, 0.5f, 1, _fadeCompletion);
        music.Lerp(SoundSetting.Volume, 0, 1, 1 - _fadeCompletion);
        music.Lerp(SoundSetting.Pitch, 0.5f, 1, 1- _fadeCompletion);
    }
}