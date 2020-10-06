using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraFX.instance.AddTrauma(0.33f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CameraFX.instance.AddTrauma(0.65f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CameraFX.instance.AddTrauma(1f);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Slow time");
            Time.timeScale = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Regular Time");
            Time.timeScale = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Freeze Trauma");
            CameraFX.instance.SetFrozen(!CameraFX.instance.IsFrozen, 5f);
        }
    }
}
