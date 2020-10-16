using System;
using System.Media;
using UnityEngine;

public class TestSounds : MonoBehaviour
{
    [SerializeField] private Sound arrowHit = default;
    [SerializeField] private Sound arrowShootIce = default;
    [SerializeField] private Sound arrowShootFire = default;
    [SerializeField] private Sound music = default;

    [SerializeField] private float spamTime = -1;
    [SerializeField] private Sound spamSound = default;
    
    private AudioManager _audioManager;
    private bool _playingMusic;
    private float _lastSpammed;
    private bool CanSpam => Time.time - _lastSpammed > spamTime;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
        _playingMusic = false;
        _lastSpammed = Time.time;
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

        if (_playingMusic == GUILayout.Toggle(_playingMusic, "music")) return;
        _playingMusic = !_playingMusic;

        if (_playingMusic)
        {
            _audioManager.PlayMusic(music);
        }
        else
        {
            _audioManager.StopMusic(music);
        }
    }
}