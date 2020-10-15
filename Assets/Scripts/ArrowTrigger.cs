using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ArrowTrigger : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("arrow"))
        {
            EventManager.current.ArrowTriggerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("arrow"))
        {
            EventManager.current.ArrowTriggerExit();
        }
    }
}
