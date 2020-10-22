using UnityEngine;
using UnityEngine.Events;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField]
    private string[] tagsToTrigger;

    [SerializeField]
    private bool useTrigger;

    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;

    [SerializeField]
    private bool useCollision;
    public UnityEvent collisionStart;
    public UnityEvent collisionEnd;


    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger && tagsToTrigger.Contains(other.tag))
        {
            triggerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (useTrigger && tagsToTrigger.Contains(other.tag))
        {
            triggerExit.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (useCollision && tagsToTrigger.Contains(collision.gameObject.tag))
        {
            collisionStart.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (useCollision && tagsToTrigger.Contains(collision.gameObject.tag))
        {
            collisionEnd.Invoke();
        }
    }
}
