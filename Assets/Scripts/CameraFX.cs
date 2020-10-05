using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFX : MonoBehaviour
{
    // TODO: eventually move these values to a CameraSettings scriptable object
    [Header("Camera Shake Values")]
    [SerializeField] private float maxYaw = 5f;
    [SerializeField] private float maxPitch = 5f;
    [SerializeField] private float maxRoll = 5f;
    [SerializeField] private float decaySpeed = 1f;
    [SerializeField] private float shakeSpeed = 10f;

    private Transform cameraDirection;
    private new Camera camera;
    private Vector3 targetCameraAngle;

    private float Trauma { get; set; }          // How much camera shake we should apply
    private float Shake => Trauma * Trauma;     // The actual value that determines camera rotation
    public bool IsFrozen { get; set; }          // Whether or not the Trauma should decrease over time

    public CameraFX Initialize(Transform cameraDir, Camera cam)
    {
        cameraDirection = cameraDir;
        camera = cam;
        IsFrozen = false;
        targetCameraAngle = Vector3.zero;

        return this;
    }

    public void AddTrauma(float amount)
    {
        Trauma += amount;
    }

    private void Update()
    {
        try
        {
            UpdateTrauma();
            ApplyCameraShake();
        }
        catch (UnassignedReferenceException)
        {
            Debug.LogError("CameraFX was not initialized correctly. \nIs there a valid camera being passed in?");
            Debug.Break();
        }
    }

    private void ApplyCameraShake()
    {
        // generate random rotation values based off perlin noise, with respect to our max values and shake amount
        targetCameraAngle.x = maxPitch * Shake * (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f);
        targetCameraAngle.y = maxYaw * Shake * (Mathf.PerlinNoise(1, Time.time * shakeSpeed) - 0.5f);
        targetCameraAngle.z = maxRoll * Shake * (Mathf.PerlinNoise(2, Time.time * shakeSpeed) - 0.5f);

        camera.transform.position = cameraDirection.transform.position;
        camera.transform.eulerAngles = cameraDirection.transform.eulerAngles + targetCameraAngle;
    }

    private void UpdateTrauma()
    {
        if (IsFrozen) return;
        Trauma = Mathf.Clamp(Trauma - (Time.deltaTime * decaySpeed), 0f, 1f);
    }

#if UNITY_EDITOR
    // purely for debugging purposes: in the future, create a global "debugging" variable that 
    // controls the visibility of stuff like this?
    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        string text = string.Format("Trauma: {0}\nShake: {1}", (int) (Trauma * 100), (int) (Shake * 100));
        GUI.Label(rect, text, style);
    }
#endif
}
