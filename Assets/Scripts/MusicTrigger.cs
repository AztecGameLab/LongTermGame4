using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicTrigger : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private Sound music = default;
    [SerializeField] private bool stopPrevious = false;

    private static SoundInstance _music;
    private static SoundInstance _currentPlaying;
    private AudioManager _audioManager;
    
    private void Awake()
    {
        _music = music == null ? null : music.GenerateInstance();
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stopPrevious) _audioManager.StopSound(_currentPlaying);
        PlayMusic();
    }

    private void PlayMusic()
    {
        _audioManager.PlaySound(_music);
        _currentPlaying = _music;
    }
}
