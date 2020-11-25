using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public GameObject TutorialBackground;

    public void Start()
    {
        TutorialBackground.SetActive(false);
    }

    public void ShowTutorial()
    {
        TutorialBackground.SetActive(true);
    }

    public void HideTutorial()
    {
        //Deactivate the tutorial
        TutorialBackground.SetActive(false);
    }
    public void CompleteTutorial()
    {
        HideTutorial();
        this.enabled = false;
    }
    
}
