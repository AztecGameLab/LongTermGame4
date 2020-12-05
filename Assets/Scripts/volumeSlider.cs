using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class volumeSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public void setVolume(float volume)
    {
        AudioManager.SetMasterVolume(mixer, volume);
    }
}
