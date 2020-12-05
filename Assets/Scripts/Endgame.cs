using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    public RawImage image;
    public Sound crickets;
    public AudioMixer mixer;
    
    public void EndGame()
    {
        StartCoroutine(end());
    }

    IEnumerator end()
    {
        float t = 0;
        float time = 2;
        
        mixer.GetFloat("MasterVolume", out float raw);
        
        while(t < time)
        {
            t += Time.deltaTime;
            image.color = new Color(0, 0, 0, t / time);
            Debug.Log(1 - t / time);
            AudioManager.SetMusicVolume(mixer, 1 - t / time);
            yield return null;
        }
        AudioManager.Instance().StopSound(MusicTrigger._currentPlaying);
        AudioManager.Instance().DisposeAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayCricketSound()
    {
        Debug.Log("cricket");
        AudioManager.Instance().PlaySound(crickets.GenerateInstance(), gameObject);
    }
}
