using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor2 : MonoBehaviour
{
    public GameObject Door;
    public Vector3 OpenDirection;
    private bool IsOpen = false;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsOpen)
            {
                Open();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (IsOpen)
            {
                Close();
            }
        }
    }




    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(OpenDirection, 2f));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(-OpenDirection, 2f));
    }


    IEnumerator Slide(Vector3 direction, float time)
    {
        Vector3 startPos = Door.transform.position;
        Vector3 finalPos = Door.transform.position + Vector3.Scale(Door.transform.localScale, direction);

        float elapsedTime = 0;
        while (elapsedTime < time)
        {
            Door.transform.position = Vector3.Lerp(startPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        IsOpen = !IsOpen;

    }


}
