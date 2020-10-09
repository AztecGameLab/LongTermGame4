using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFX : MonoBehaviour
{
    public static CameraFX instance;

    // TODO: eventually move these values to a CameraSettings scriptable object
    [Header("Camera Shake Values")]
    [SerializeField] private float maxYaw = 5f;
    [SerializeField] private float maxPitch = 5f;
    [SerializeField] private float maxRoll = 5f;
    [SerializeField] private float decaySpeed = 1f;
    [SerializeField] private float shakeSpeed = 10f;

    [Header("GameObject References")]
    [SerializeField] private Transform cameraDirection = null;
    [SerializeField] private new Camera camera = null;

    private Vector3 targetCameraAngle;          // What we want the camera rotation to look like
    private float freezeTime;                   // How much time is left before the camera unfreezes

    private float Trauma { get; set; }          // How much camera shake we should apply
    private float Shake => Trauma * Trauma;     // The actual value that determines camera rotation
    public bool IsFrozen { get; private set; }  // Whether or not the Trauma should decrease over time

    private void Awake()
    {
        instance = this;

        IsFrozen = false;
        targetCameraAngle = Vector3.zero;
        freezeTime = 0f;
    }

    private void Update()
    {
        UpdateTrauma();
        ApplyCameraShake();
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
        if (IsFrozen)
        {
            // If we are frozen without a specified time, freeze indefinately
            if (freezeTime == -1)
            {
                return;
            }

            // Countdown every second, and unfreeze at zero
            if (freezeTime > 0)
            {
                freezeTime = Mathf.Max(freezeTime - Time.deltaTime, 0f);
            }
            else
            {
                IsFrozen = false;
            }

            return;
        }

        // If we are not frozen, decrease Trauma
        Trauma = Mathf.Clamp(Trauma - (Time.deltaTime * decaySpeed), 0f, 1f);
    }

    public void AddTrauma(float amount)
    {
        Trauma += amount;
    }

    public void SetFrozen(bool isFrozen, float durationSeconds = -1f)
    {
        IsFrozen = isFrozen;
        freezeTime = IsFrozen ? durationSeconds : 0f;
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
        string text = string.Format("Trauma: {0}\nShake: {1}\nFreeze Time: {2}", (int) (Trauma * 100), (int) (Shake * 100), freezeTime);
        GUI.Label(rect, text, style);
    }
#endif
}
