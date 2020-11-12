using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingObject : MonoBehaviour
{
    [SerializeField]
    private bool startInvisible;
    private Renderer ren;
    // Start is called before the first frame update
    void Start()
    {
        ren = gameObject.GetComponent<Renderer>();

        if (startInvisible)
        {
            ren.enabled = false;
        }
    }

    public void Disappear()
    {
        ren.enabled = false;
    }
    public void Appear()
    {
        ren.enabled = true;
    }

}
