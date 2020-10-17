using UnityEngine;
using UnityEngine.Audio;

// used for quickly testing out sounds and their modulations

public class SoundTester : MonoBehaviour
{
    [SerializeField, Tooltip("A list of sounds to display for testing")] 
    private Sound[] sounds = new Sound[0];

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
        
        foreach (var sound in sounds)
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
    }
}