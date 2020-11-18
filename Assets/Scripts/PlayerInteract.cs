using JetBrains.Annotations;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private float maxDistance = 1f;

    public delegate void Interact([CanBeNull] Transform target, [NotNull] Transform player);
    public static event Interact OnInteract;

    public void TryInteract()
    {
        // Raycast to check if we are looking at something within the interactable range
        var playerTransform = transform;
        Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hit, maxDistance);

        OnInteract?.Invoke(hit.transform, transform);
    }
}