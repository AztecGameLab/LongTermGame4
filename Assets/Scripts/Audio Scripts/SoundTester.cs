using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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

    private void OnGUI()
    {
        if (!showGUI) return;
        
        foreach (var sound in sounds2D)
        {
            if (!GUILayout.Button(sound.name)) continue;
            
            if (!sound.IsPlaying)
            {
                _audioManager.PlaySound(sound);
            }
            else if (sound.IsPlaying && sound.IsLooping)
            {
                _audioManager.StopSound(sound);
            }
        }
        
        foreach (var sound in sounds3D)
        {
            if (!GUILayout.Button(sound.name)) continue;
            
            if (!sound.IsPlaying)
            {
                _audioManager.PlaySound(sound, gameObject);
            }
            else if (sound.IsPlaying && sound.IsLooping)
            {
                _audioManager.StopSound(sound, gameObject);
            }
        }
    }
}