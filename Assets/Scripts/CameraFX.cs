using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFX : MonoBehaviour
{

    [SerializeField] private float maxYaw = 5f;
    [SerializeField] private float maxPitch = 5f;
    [SerializeField] private float maxRoll = 5f;

    private readonly Camera cam;
    private Vector3 targetEulerAngles;

    private float Trauma { get; set; }
    private float Shake => Trauma * Trauma;

    public CameraFX(Camera camera)
    {
        cam = camera;
        targetEulerAngles = Vector3.zero;
    }

    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        targetEulerAngles.x = maxPitch * Shake * Mathf.PerlinNoise(0, Time.time);
        targetEulerAngles.y = maxYaw * Shake * Mathf.PerlinNoise(0, Time.time);
        targetEulerAngles.z = maxRoll * Shake * Mathf.PerlinNoise(0, Time.time);

        cam.transform.eulerAngles = targetEulerAngles;
    }

    public void AddTrauma(float amount)
    {
        Trauma += amount;
    }
}
