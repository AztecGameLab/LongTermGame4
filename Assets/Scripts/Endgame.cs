using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    public RawImage image;
    public void EndGame()
    {
        StartCoroutine(end());
    }

    IEnumerator end()
    {
        float t = 0;
        float time = 2;
        while(t < time)
        {
            t += Time.deltaTime;
            image.color = new Color(0, 0, 0, t / time);
            yield return null;
        }
        AudioManager.Instance().DisposeAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
