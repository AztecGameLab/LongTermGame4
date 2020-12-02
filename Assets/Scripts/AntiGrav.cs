using UnityEngine;

public class AntiGrav : Interactable
{
    private float riseTime = 2.5f;
    private float fallTime = 0.9f;

    private bool _ending;
    private float _timeSpentRising, _timeSpentFalling;
    private Rigidbody _rb;
    private AudioManager _audioManager;
    private MultiSoundInstance _gravitySound;
    private InteractHoldable _holdable;
    private RigidbodyConstraints _previousConstraints;
    
    private bool IsHoldable => _holdable != null;
    
    private void FixedUpdate()
    {
        if (_gravitySound.State == MultiSoundState.Intro && _timeSpentRising < riseTime)
        {
            // apply slow-rise while in intro state
            _rb.velocity = new Vector3(0, 0.25f, 0);
            _timeSpentRising += Time.fixedDeltaTime;
        }
        else if (_gravitySound.State == MultiSoundState.Outro && _timeSpentFalling < fallTime)
        {
            // apply slow-fall while in outro state
            _rb.velocity = new Vector3(0, -0.25f, 0);
            _timeSpentFalling += Time.fixedDeltaTime;
        }
        else if (_timeSpentRising >= riseTime && _timeSpentFalling == 0)
        {
            // apply reversed gravity while in loop
            _rb.velocity += new Vector3(0, 0.25f, 0); 
        }
    }

    public void Activate(MultiSoundInstance gravitySound)
    {
        // get component references
        _rb = GetComponent<Rigidbody>();
        _holdable = GetComponent<InteractHoldable>();
        _audioManager = AudioManager.Instance();
        
        // finish preparing script for activation
        _ending = false;
        _timeSpentRising = 0;
        _timeSpentFalling = 0;
        CameraFX.instance.AddTrauma(0.25f);
        _gravitySound = gravitySound;
        _audioManager.PlaySound(_gravitySound, gameObject);
        
        if(IsHoldable && _holdable.IsHeld) _holdable.ToggleHolding();
        
        // initialize floating rigidbody settings
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
        _previousConstraints = _rb.constraints;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Deactivate()
    {
        // Reduce velocity of floating boxes that are shot (more controllable when shot)
        var velo = _rb.velocity;
        velo *= 0.25f;
        velo.y = -1;
        _rb.velocity = velo;
        _rb.constraints = _previousConstraints;
        
        // Make sure the box is in the correct state to be ended (Invokes incorrectly otherwise)
        if (_ending || _gravitySound.State == MultiSoundState.Outro) return;
        _ending = true;
        CameraFX.instance.AddTrauma(0.25f);
        _audioManager.StopSound(_gravitySound, gameObject);

        // Destroy this script after outro has finished playing
        Invoke(nameof(DestroyScript), (float) _gravitySound.OutroDuration + 0.1f - fallTime);
    }

    private void DestroyScript()
    {
        _audioManager.Dispose(gameObject);
        _rb.useGravity = true;
        
        Destroy(this);
    }

    protected override void OnInteract(Transform userTransform)
    {
        if (IsHoldable && _gravitySound.State != MultiSoundState.Inactive)
        {
            _holdable.SetCanceled(true);
        }
    }
}
