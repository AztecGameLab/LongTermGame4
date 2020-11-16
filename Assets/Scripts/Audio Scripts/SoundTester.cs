using UnityEngine;

// used for quickly testing out sounds and their modulations

public class SoundTester : MonoBehaviour
{
    [SerializeField, Tooltip("A list of sounds to display for testing, will originate from this script's GameObject")] 
    private Sound[] sounds3D = new Sound[0];
    private SoundInstance[] _soundsInstances3D;
    
    [SerializeField, Tooltip("A list of sounds to display for testing, will not be spatial")] 
    private Sound[] sounds2D = new Sound[0];
    private SoundInstance[] _soundsInstances2D;
    
    [SerializeField]
    private bool showGUI = true;
    
    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = AudioManager.Instance();
        _soundsInstances3D = new SoundInstance[sounds3D.Length];
        _soundsInstances2D = new SoundInstance[sounds2D.Length];
        for (int i = 0; i < sounds3D.Length; i++)
        {
            _soundsInstances3D[i] = sounds3D[i].GenerateInstance();
        }
        for (int i = 0; i < sounds2D.Length; i++)
        {
            _soundsInstances2D[i] = sounds2D[i].GenerateInstance();
        }
    }

    private void OnGUI()
    {
        if (!showGUI) return;

        foreach (var sound in _soundsInstances2D)
        {
            if (!GUILayout.Button(sound.Name + ": 2D")) continue;
            
            if (sound.IsInactive)
            {
                _audioManager.PlaySound(sound);
            }
            else if (!sound.IsInactive && sound.IsLooping)
            {
                _audioManager.StopSound(sound);
            }
        }
        
        foreach (var sound in _soundsInstances3D)
        {
            if (!GUILayout.Button(sound.Name + ": 3D")) continue;
            
            if (sound.IsInactive)
            {
                _audioManager.PlaySound(sound, gameObject);
            }
            else if (!sound.IsInactive && sound.IsLooping)
            {
                _audioManager.StopSound(sound, gameObject);
            }
        }
    }
}