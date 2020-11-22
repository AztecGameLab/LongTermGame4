using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu]
public class HoldableSettings : ScriptableObject
{
    [Header("Audio Settings")]
    [SerializeField, Tooltip("The sound that is played when this object collides with something.")] 
    [CanBeNull] private Sound hitSound = null;
    
    [Header("Hold Position Settings")]
    [SerializeField, Tooltip("How far this object should be held from the player.")] 
    private float holdDistance = 2f;
    [SerializeField, Tooltip("A way to adjust the precise position at which this object is held (good for weird pivots)")] 
    private Vector3 holdPositionOffset = Vector3.zero;
    [SerializeField, Tooltip("The distance between this object and a target where the velocity will be zero.")] 
    private float deadZone = 0.1f;
    [SerializeField, Tooltip("How far the object can be underneath the player before it is dropped.\n1 = directly beneath player")]
    [Range(0, 1)]
    private float dropDistance = 0.95f;
    
    [Header("Speed Settings")]
    [SerializeField, Tooltip("How fast this object moves to follow the player's direction.")] 
    private float moveSpeed = 10f;
    [SerializeField, Tooltip("How fast this object will rotate itself")]
    private float rotateSpeed = 5f;
    [SerializeField, Tooltip("A way to cap the speed that this object can have when thrown")]
    private float maxThrowSpeed = 10f;
    
    [CanBeNull]
    public Sound HitSound => hitSound;
    
    public float HoldDistance => holdDistance;
    public Vector3 HoldPositionOffset => holdPositionOffset;
    public float DeadZone => deadZone;
    public float DropDistance => dropDistance;
    public float MoveSpeed => moveSpeed;
    public float RotateSpeed => rotateSpeed;
    public float MaxThrowSpeed => maxThrowSpeed;
}
