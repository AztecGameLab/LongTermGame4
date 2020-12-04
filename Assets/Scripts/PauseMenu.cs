using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    public GameObject PauseGUI;
    ReloadScene reloadScene;
    // Start is called before the first frame update
    void Start()
    {
        reloadScene = GameObject.FindObjectOfType<ReloadScene>();
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            //If we press escape enable/disable the
            Pause();
        }
    }

    public void Pause(){
        
        paused = !paused;
        Cursor.lockState = (paused ? CursorLockMode.None : CursorLockMode.Locked);
        Time.timeScale = 1 * (paused ? 0 : 1);
        PauseGUI.SetActive(paused);
    }

    public void Restart(){
        Time.timeScale = 1;
        reloadScene.ReloadTheScene();
    }

    public void Quit(){
        SceneManager.LoadScene("MainMenu");
    }
}
