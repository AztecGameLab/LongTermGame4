using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseSensitivity : MonoBehaviour
{
    
    public void SetSensitivity(float x)
    {
        PlayerManager.instance.s_looking.SetSensitivity(Mathf.Lerp(10,1000, x));
    }
    
}
