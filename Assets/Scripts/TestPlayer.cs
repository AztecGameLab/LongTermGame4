using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    [SerializeField] private Transform cameraDirection = null;
    [SerializeField] private new Camera camera = null;

    private CameraFX camFX;

    void Awake()
    {
        camFX = gameObject.AddComponent<CameraFX>().Initialize(cameraDirection, camera);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            camFX.AddTrauma(0.33f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            camFX.AddTrauma(0.65f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            camFX.AddTrauma(1f);
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
            camFX.IsFrozen = !camFX.IsFrozen;
        }
    }
}
