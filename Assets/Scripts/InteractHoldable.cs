using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractHoldable : Interactable
{
    [SerializeField, Tooltip("The settings that will applied to this object.")]
    private HoldableSettings settings = default;
    
    public bool IsHeld => _isHeld;
    
    private bool _isHeld = false;
    private Vector3 _restingPosition = Vector3.zero;
    private Transform _playerTransform = null;
    private Rigidbody _rigidbody = null;
    private float _curDistance;
    private AudioManager _manager;
    private SoundInstance _hitSound;
    
    private bool _wantsToToggle = false;
    private int _isCanceled = 0;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _manager = AudioManager.Instance();
        _hitSound = settings.HitSound != null ? settings.HitSound.GenerateInstance() : null;
    }
    
    protected override void OnInteract(Transform userTransform)
    {
        _playerTransform = userTransform;
    
        SetHolding(!_isHeld);
    }
    
    protected override void OnMissedInteract()
    {
        SetHolding(false);
    }
    
    private void FixedUpdate()
    {
        if (!_isHeld) return;
     
        TurnTowardsOwner();
        MoveTowardsRestingPosition();
    }
    
    private void LateUpdate()
    {
        if (_isCanceled > 0)
        {
            _wantsToToggle = false;
            _isCanceled = 0;
        }
    
        if (_wantsToToggle) ToggleHolding();
    }
    
    private void CheckForDisconnect(ContactPoint contact)
    {
        // If the player moves too far away from this object, they will drop it
        var distanceToPlayer = (transform.position - _playerTransform.position).magnitude;
        if (distanceToPlayer > settings.HoldDistance * 2f) SetHolding(false);
    
        // No prop-climbing here! Sorry, hl2 players
        if (contact.otherCollider.CompareTag("Player"))
        {
            SetHolding(false);
        }
    }
    
    private void TurnTowardsOwner()
    {
        // Make it so this object faces the player, but doesn't tilt up or down
        var newRotation = -1 * _playerTransform.forward;
        newRotation.y = 0;
        
        // Smoothly move towards this target rotation
        transform.forward = Vector3.MoveTowards(transform.forward, newRotation, Time.deltaTime * settings.RotateSpeed);
    }
    
    private void MoveTowardsRestingPosition()
    {
        // Checks to make sure the box isn't directly under the player
        var directionToPlayer = (transform.position - _playerTransform.position).normalized;
        if (Vector3.Dot(directionToPlayer, Vector3.down) > settings.DropDistance)
        {
            _rigidbody.velocity = Vector3.zero;
            SetHolding(false);
            return;
        }
    
        // Calculate a point a certain distance in front of the player to aim for.
        _restingPosition = _playerTransform.position + (_playerTransform.forward * settings.HoldDistance);
        _restingPosition += settings.HoldPositionOffset;
        
        // Calculate how close this object is to the point we are aiming at.
        _curDistance = Vector3.Distance(_restingPosition, transform.position);
    
        // If we are in the dead-zone, kill the velocity to stop micro-movements.
        if (_curDistance <= settings.DeadZone)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        else
        {
            // Find the direction towards the target position, and scale it by how distant the target it.
            // Multiplying by curDistance makes it move faster for longer distances, and reach a smooth stop for close ones.
            var towardsTarget = (_restingPosition - transform.position).normalized * _curDistance;
            _rigidbody.velocity = towardsTarget * settings.MoveSpeed;
        }
    }
    
    private void SetHolding(bool holding)
    {
        if (holding != _isHeld)
        {
            _wantsToToggle = true;
        }
    }
    
    public void SetCanceled(bool canceled)
    {
        _isCanceled += canceled ? 1 : -1;
    }
    
    public void ToggleHolding()
    {
        var isHeld = _isHeld;
        _wantsToToggle = false;
        
        // If we are holding something, all of these should be true; otherwise, false
        _isHeld = !isHeld;
        _rigidbody.freezeRotation = !isHeld;
        _rigidbody.useGravity = isHeld;
    
        if (isHeld)
        {
            // Updates the velocity of the object before it is thrown to comply with the defined max
            var velocity = _rigidbody.velocity;
            _rigidbody.velocity = velocity.normalized * Mathf.Min(velocity.magnitude, settings.MaxThrowSpeed);
        }
    }
    
    private void PlayHitSound()
    {
        if (settings.HitSound == null) return;
        
        _hitSound.SetValue(SoundValue.Volume, 0.15f * Mathf.Min(_rigidbody.velocity.magnitude / 5, 1));
        _manager.PlaySound(_hitSound, gameObject);
    }
    
    private void OnCollisionEnter()
    {
        PlayHitSound();    
    }
    
    private void OnCollisionStay(Collision other)
    {
        if (_isHeld) CheckForDisconnect(other.GetContact(0));
    }
}