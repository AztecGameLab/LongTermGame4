using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    public GameObject TutorialBackground;
    public GameObject Player;
    public bool TutorialCompleted = false;
    public int TutorialXMin;
    public int TutorialXMax;
    public int TutorialZMin;
    public int TutorialZMax;
    
    void Start()
    {
        //Find the player
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //If the tutorial hasn't been completed and the player is within the tutorial bounds
        if (!TutorialCompleted &&
            Player.transform.position.x > TutorialXMin && Player.transform.position.x < TutorialXMax &&
            Player.transform.position.z > TutorialZMin && Player.transform.position.z < TutorialZMax)
        {
            //Activate the tutorial
            TutorialBackground.SetActive(true);
        }
        //Otherwise if the player has completed this tutorial or they are outside of the tutorial bounds
        else
        {
            //Deactivate the tutorial
            TutorialBackground.SetActive(false);
        }
    }
}
