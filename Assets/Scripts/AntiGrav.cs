using System;
using UnityEngine;

public class AntiGrav : Interactable
{
    private float riseTime = 2.5f;
    private float fallTime = 2.5f;

    private bool _ending;
    private float _timeSpentRising, _timeSpentFalling;
    private Rigidbody _rb;
    private AudioManager _audioManager;
    private MultiSoundInstance _gravitySound;
    private InteractHoldable _holdable;
    
    void Start()
    {
        _timeSpentRising = 0;
        _holdable = GetComponent<InteractHoldable>();
        _rb = GetComponent<Rigidbody>();
        _audioManager = AudioManager.Instance();
    }

    private void FixedUpdate()
    {
        if (_gravitySound.State == MultiSoundState.Intro && _timeSpentRising < riseTime)
        {
            _rb.velocity = new Vector3(0, 0.25f, 0);
            _timeSpentRising += Time.fixedDeltaTime;
        }
        else if (_gravitySound.State == MultiSoundState.Outro && _timeSpentFalling < fallTime)
        {
            _rb.velocity = new Vector3(0, -0.25f, 0);
            _timeSpentFalling += Time.fixedDeltaTime;
        }
        else if (_gravitySound.State != MultiSoundState.Outro)
        {
            _rb.velocity += new Vector3(0, 0.25f, 0); 
        }
    }

    public void Activate(MultiSoundInstance gravitySound)
    {
        _ending = false;
        _timeSpentRising = 0;
        _timeSpentFalling = 0;
        CameraFX.instance.AddTrauma(0.5f);
        _rb = GetComponent<Rigidbody>();
        _audioManager = AudioManager.Instance();
        _gravitySound = gravitySound;
        _audioManager.PlaySound(_gravitySound, gameObject);
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Deactivate()
    {
        var velo = _rb.velocity;
        velo *= 0.25f;
        velo.y = -1;
        _rb.velocity = velo;
        
        if (_ending || _gravitySound.State != MultiSoundState.Looping) return;
        _ending = true;
        Debug.Log("deactivate");
        CameraFX.instance.AddTrauma(1f);
        _audioManager.StopSound(_gravitySound, gameObject);
        
        // Cleanup stuff after outro has finished playing
        Invoke(nameof(ResetState), (float) _gravitySound.OutroDuration + 0.1f);
    }

    private void ResetState()
    {
        _audioManager.Dispose(gameObject);
        _rb.useGravity = true;
        
        Destroy(this);
    }

    protected override void OnInteract(Transform userTransform)
    {
        if (_holdable != null && _gravitySound.State != MultiSoundState.Inactive)
        {
            _holdable.SetCanceled(true);
        }
    }
}
