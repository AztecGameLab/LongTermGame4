using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Sound whooshSound = default;
    [SerializeField] private Sound lightClick = default;
    [SerializeField] private Sound heavyClick = default;
    [SerializeField] private Sound mainMusic = default;

    private SoundInstance _whooshSound;
    private SoundInstance _lightClick;
    private SoundInstance _heavyClick;
    private AudioManager _audioManager;

    private static bool _playingMusic = false;
    private static SoundInstance _mainMusic;
    
    private void Start()
    {
        // Initialize sounds
        _whooshSound = whooshSound.GenerateInstance();
        _lightClick = lightClick.GenerateInstance();
        _heavyClick = heavyClick.GenerateInstance();
        _audioManager = AudioManager.Instance();

        if (!_playingMusic)
        {
            _mainMusic = mainMusic.GenerateInstance();
            _audioManager.PlaySound(_mainMusic);
            _playingMusic = true;
        }
    }

    // Public Scene Changing Methods

    public void LoadSceneCredits()
    {
        PlayWhoosh();
        SceneManager.LoadScene("MainMenuCredits");
    }
    
    public void LoadSceneExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        //set the PlayMode too stop
#else
        Application.Quit();
#endif
    }
    
    public void LoadScenePlay()
    {
        PlayWhoosh();
        _audioManager.StopSound(_mainMusic);
        _playingMusic = false;
        SceneManager.LoadScene(2);
    }

    public void LoadSceneMainMenu()
    {
        PlayWhoosh();
        SceneManager.LoadScene("MainMenu");
    }

    // Public Sound-Playing Methods
    
    public void PlayLightClick()
    {
        _audioManager.PlaySound(_lightClick);
    }

    public void PlayHeavyClick()
    {
        _audioManager.PlaySound(_heavyClick);
    }

    public void PlayWhoosh()
    {
        _audioManager.PlaySound(_whooshSound);
    }
}
