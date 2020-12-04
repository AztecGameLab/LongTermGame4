using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndgameScreen : MonoBehaviour
{
    public Image image;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(end());
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(2);
        float t = 0;
        float time = 2;
        while (t < time)
        {
            t += Time.deltaTime;
            image.color = new Color(0, 0, 0, 1 - t / time);
            yield return null;
        }
    }
}
