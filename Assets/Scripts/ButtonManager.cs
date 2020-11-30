using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Sound lightClick = default;
    [SerializeField] private Sound heavyClick = default;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.Instance();
    }

    public void LoadSceneCredits()
    {
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
        SceneManager.LoadScene("Play");
    }

    public void LoadSceneMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayLightClick()
    {
        _audioManager.PlaySound(lightClick);
    }

    public void PlayHeavyClip()
    {
        _audioManager.PlaySound(heavyClick);
    }

}
