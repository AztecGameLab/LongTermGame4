using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerInteract.OnInteract += CheckForInteraction;
    }

    private void OnDisable()
    {
        PlayerInteract.OnInteract -= CheckForInteraction;
    }

    private void CheckForInteraction(Transform target, Transform user)
    {
        if (target != transform)
        {
            OnMissedInteract();
        }
        else
        {
            OnInteract(user);
        }
    }

    protected abstract void OnInteract(Transform userTransform);
    protected virtual void OnMissedInteract() { /* Do nothing */ }
}