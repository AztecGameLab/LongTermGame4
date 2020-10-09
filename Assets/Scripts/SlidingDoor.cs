using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public GameObject Door;
    public Vector3 OpenDirection = Vector3.up;
    public float TimeToOpen = 1;

    private Vector3 ClosedPosition;
    private Vector3 OpenPosition;

    private void Start()
    {
        // Start wherever the door
        ClosedPosition = Door.transform.position;
        // Move a distance that is proportional to the dimension of the door we are moving along (aka move up the height of the door with a y transform of 1)
        OpenPosition = Door.transform.position + Vector3.Scale(Door.transform.localScale, OpenDirection);
    }


    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(OpenClose(true));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(OpenClose(false));
    }


    private IEnumerator OpenClose(bool open)
    {
        // Start where ever the door currently is
        Vector3 StartPos = Door.transform.position;
        Vector3 FinalPos;

        // Go to where we're supposed to
        if (open)
            FinalPos = OpenPosition;
        else
            FinalPos = ClosedPosition;


        float ElapsedTime = 0;
        // Take time proportional to the distance we have to go
        float TotalTime = (FinalPos - StartPos).magnitude/(OpenPosition - ClosedPosition).magnitude * TimeToOpen;


        while (ElapsedTime < TotalTime)
        {
            // Move the door by using Lerp
            Door.transform.position = Vector3.Lerp(StartPos, FinalPos, ElapsedTime / TotalTime);
            ElapsedTime += Time.deltaTime;
            yield return null;
        }
    }


}
