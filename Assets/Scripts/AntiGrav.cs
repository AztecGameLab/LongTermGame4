using UnityEngine;

public class AntiGrav : Interactable
{
    private Rigidbody _rb;
    private AudioManager _audioManager;
    private MultiSound _gravitySound;
    private InteractHoldable _holdable;
    
    void Start()
    {
        _holdable = GetComponent<InteractHoldable>();
        _rb = GetComponent<Rigidbody>();
        _audioManager = AudioManager.Instance();
    }

    private void FixedUpdate()
    {
        switch (_gravitySound.State)
        {
            case MultiSound.MultiSoundState.Intro:
                _rb.velocity = new Vector3(0, 0.25f, 0);
                break;
            case MultiSound.MultiSoundState.Looping:
                _rb.AddForce(new Vector3(0, 5, 0), ForceMode.Acceleration);
                break;
            case MultiSound.MultiSoundState.Outro:
                _rb.velocity = new Vector3(0, -0.25f, 0);
                break;
        }
    }
    
    public void Activate(MultiSound gravitySound)
    {
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
        CameraFX.instance.AddTrauma(1f);
        _audioManager.StopSound(_gravitySound, gameObject);
        var velo = _rb.velocity;
        velo *= 0.25f;
        velo.y = -1;
        _rb.velocity = velo;
        
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
        if (_holdable != null && _gravitySound.State != MultiSound.MultiSoundState.Inactive)
        {
            _holdable.SetCanceled(true);
        }
    }
}
