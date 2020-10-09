using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{

    // Update is called once per frame
    public void LoadSceneCredits()
    {
        //Insert Credits Implementation here
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

}
