using UnityEngine;

public class TestSounds : MonoBehaviour
{
    [SerializeField] private Sound arrowHit = default;
    [SerializeField] private Sound arrowShootIce = default;
    [SerializeField] private Sound arrowShootFire = default;
    [SerializeField] private Sound music = default;

    private AudioManager _audioManager;
    private bool _playingMusic;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
        _playingMusic = false;
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