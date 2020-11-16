using UnityEngine;
using Cursor = UnityEngine.Cursor;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.visible = true;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && sounds2D.Length > 0) Run2DSound(_soundsInstances2D[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2) && sounds2D.Length > 1) Run2DSound(_soundsInstances2D[1]);
        if (Input.GetKeyDown(KeyCode.Alpha3) && sounds3D.Length > 0) Run3DSound(_soundsInstances3D[0], gameObject);
        if (Input.GetKeyDown(KeyCode.Alpha4) && sounds3D.Length > 1) Run3DSound(_soundsInstances3D[1], gameObject);
    }

    private void OnGUI()
    {
        if (!showGUI) return;

        foreach (var sound in _soundsInstances2D)
        {
            if (GUILayout.Button(sound.Name + ": 2D")) Run2DSound(sound);
        }

        foreach (var sound in _soundsInstances3D)
        {
            if (GUILayout.Button(sound.Name + ": 3D")) Run3DSound(sound, gameObject);
        }
    }

    private void Run2DSound(SoundInstance sound)
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
    
    private void Run3DSound(SoundInstance sound, GameObject target)
    {
        if (sound.IsInactive)
        {
            _audioManager.PlaySound(sound, target);
        }
        else if (!sound.IsInactive && sound.IsLooping)
        {
            _audioManager.StopSound(sound, target);
        }
    }
}