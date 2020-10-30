using System;
using UnityEngine;

// used for quickly testing out sounds and their modulations

public class SoundTester : MonoBehaviour
{
    [SerializeField, Tooltip("A list of sounds to display for testing, will originate from this script's GameObject")] 
    private Sound[] sounds3D = new Sound[0];
    
    [SerializeField, Tooltip("A list of sounds to display for testing, will not be spatial")] 
    private Sound[] sounds2D = new Sound[0];

    [SerializeField]
    private bool showGUI = true;
    
    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && sounds2D.Length > 0) RunSound(sounds2D[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2) && sounds2D.Length > 1) RunSound(sounds2D[1]);
        if (Input.GetKeyDown(KeyCode.Alpha3) && sounds2D.Length > 2) RunSound(sounds2D[2]);
        if (Input.GetKeyDown(KeyCode.Alpha4) && sounds2D.Length > 3) RunSound(sounds2D[3]);
        if (Input.GetKeyDown(KeyCode.Alpha5) && sounds2D.Length > 4) RunSound(sounds2D[4]);
    }

    private void OnGUI()
    {
        if (!showGUI) return;

        foreach (var sound in sounds2D)
        {
            if (GUILayout.Button(sound.name + ": 2D")) RunSound(sound);
        }

        foreach (var sound in sounds3D)
        {
            if (GUILayout.Button(sound.name + ": 3D")) RunSound(sound, gameObject);
        }
    }

    private void RunSound(Sound sound, GameObject target = null)
    {
        if (sound.IsInactive)
        {
            _audioManager.PlaySound(sound);
        }
        else if (!sound.IsInactive && sound.IsLooping)
        {
            _audioManager.StopSound(sound);
        }
    }
}