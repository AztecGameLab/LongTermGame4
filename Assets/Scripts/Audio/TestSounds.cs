using System;
using System.Media;
using UnityEngine;

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
    private bool _playing3DMusic;
    private bool _playing2DMusic;
    private float _lastSpammed;
    private bool CanSpam => Time.time - _lastSpammed > spamTime;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
        _playing3DMusic = false;
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
        
        if (GUILayout.Button("3D arrow-hit"))
        {
            _audioManager.PlayOneShot(arrowHit, gameObject);
        }

        if (_playing3DMusic != GUILayout.Toggle(_playing3DMusic, "3d music"))
        {
            _playing3DMusic = !_playing3DMusic;

            if (_playing3DMusic)
            {
                _audioManager.PlayLoopable(spacialMusic, gameObject);
            }
            else
            {
                _audioManager.StopLoopable(spacialMusic, gameObject);
            }            
        }
        if (_playing2DMusic != GUILayout.Toggle(_playing2DMusic, "2d music"))
        {
            _playing2DMusic = !_playing2DMusic;

            if (_playing2DMusic)
            {
                _audioManager.PlayLoopable(music);
            }
            else
            {
                _audioManager.StopLoopable(music);
            }            
        }
    }
}