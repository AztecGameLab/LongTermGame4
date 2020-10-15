using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ArrowTrigger : MonoBehaviour
{
    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arrow"))
        {
            triggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("arrow"))
        {
            triggerExit.Invoke();
        }
    }
}
