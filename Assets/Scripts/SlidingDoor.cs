using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public List<GameObject> doors;
    public List<Animator> animators;

    private void Start()
    {
        foreach (GameObject door in doors)
            animators.Add(door.GetComponent<Animator>());
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Open();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Close();
        }
    }




    public void Open()
    {
        Slide(true);
    }

    public void Close()
    {
        Slide(false);
    }

    private void Slide(bool state)
    {
        foreach (Animator animator in animators)
        {
            animator.SetBool("slide", state);
            Debug.Log("Set slide to " + state);
        }
    }


}
