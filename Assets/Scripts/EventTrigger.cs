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

    private int numTriggeringTrigger;
    private int numTriggeringCollider;

    private void Start()
    {
        numTriggeringCollider = 0;
        numTriggeringTrigger = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger && tagsToTrigger.Contains(other.tag))
        {
            if (numTriggeringTrigger == 0)
            {
                triggerEnter.Invoke();
            }
            numTriggeringTrigger++;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (useTrigger && tagsToTrigger.Contains(other.tag))
        {
            numTriggeringTrigger--;
            if (numTriggeringTrigger == 0)
            {
                triggerExit.Invoke();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (useCollision && tagsToTrigger.Contains(collision.gameObject.tag))
        {
            if (numTriggeringCollider == 0)
            {
                collisionStart.Invoke();
            }
            numTriggeringCollider++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (useCollision && tagsToTrigger.Contains(collision.gameObject.tag))
        {
            numTriggeringCollider--;
            if (numTriggeringCollider == 0)
            {
                collisionEnd.Invoke();
            }
        }
    }
}
