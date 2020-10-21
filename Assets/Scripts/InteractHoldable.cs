using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractHoldable : Interactable
{
    [SerializeField] private Sound hitSound;
    
    [Header("Hold Position Settings")]
    [SerializeField, Tooltip("How far this object should be held from the player.")] 
    private float holdDistance = 2f;
    [SerializeField, Tooltip("A way to adjust the precise position at which this object is held (good for weird pivots)")] 
    private Vector3 holdPositionOffset = Vector3.zero;
    [SerializeField, Tooltip("The distance between this object and a target where the velocity will be zero.")] 
    private float deadZone = 0.1f;
    
    [Header("Speed Settings")]
    [SerializeField, Tooltip("How fast this object moves to follow the player's direction.")] 
    private float moveSpeed = 10f;
    [SerializeField, Tooltip("How fast this object will rotate itself")]
    private float rotateSpeed = 5f;
    [SerializeField, Tooltip("A way to cap the speed that this object can have when thrown")]
    private float maxThrowSpeed = 10f;
    [SerializeField, Tooltip("The speed at which this object will be forcefully dropped")] 
    private float maxDistance = 10f;
    
    private bool _isHeld = false;
    private Vector3 _restingPosition = Vector3.zero;
    private Transform _playerTransform = null;
    private Rigidbody _rigidbody = null;
    private float _curDistance;
    private AudioManager _manager;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _manager = AudioManager.Instance();
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
        CheckForDisconnect();
    }

    private void CheckForDisconnect()
    {
        // Better way to check for disconnecting than checking distance?
        // I think ray-casting to player would be best, but too expensive. 
        if (_curDistance > maxDistance)
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
        transform.forward = Vector3.MoveTowards(transform.forward, newRotation, Time.deltaTime * rotateSpeed);
    }

    private void MoveTowardsRestingPosition()
    {
        // Calculate a point a certain distance in front of the player to aim for.
        _restingPosition = _playerTransform.position + (_playerTransform.forward * holdDistance);
        _restingPosition += holdPositionOffset;
        
        // Calculate how close this object is to the point we are aiming at.
        _curDistance = Vector3.Distance(_restingPosition, transform.position);

        // If we are in the dead-zone, kill the velocity to stop micro-movements.
        if (_curDistance <= deadZone)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        else
        {
            // Find the direction towards the target position, and scale it by how distant the target it.
            // Multiplying by curDistance makes it move faster for longer distances, and reach a smooth stop for close ones.
            var towardsTarget = (_restingPosition - transform.position).normalized * _curDistance;
            _rigidbody.velocity = towardsTarget * moveSpeed;
        }
    }
    
    private void SetHolding(bool holding)
    {
        // If we are holding something, all of these should be true; otherwise, false
        _isHeld = holding;
        _rigidbody.freezeRotation = holding;
        _rigidbody.useGravity = !holding;

        if (!holding)
        {
            // Updates the velocity of the object before it is thrown to comply with the defined max
            var velocity = _rigidbody.velocity;
            _rigidbody.velocity = velocity.normalized * Mathf.Min(velocity.magnitude, maxThrowSpeed);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        hitSound.SetSetting(SoundSetting.Volume, 0.15f * Mathf.Min(_rigidbody.velocity.magnitude / 5, 1));
        _manager.PlaySound(hitSound, gameObject);
        
        // No prop-climbing here! Sorry, hl2 players
        if (other.gameObject.CompareTag("Player"))
        {
            SetHolding(false);
        }
    }
}