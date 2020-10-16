using UnityEngine;

[CreateAssetMenu(fileName = "New LFO Modulator", menuName = "Custom/LFO Modulator")]
public class LFO : Modulator
{
    [Header("LFO Modulation Settings")]
    
    [SerializeField, Tooltip("The value that the LFO will oscillate between")]
    private float extreme = 1f;

    [SerializeField, Tooltip("How frequently this LFO will oscillate")]
    private float frequency = 1f;

    [SerializeField, Tooltip("Vertical shift the LFO up or down")]
    private float verticalShift = 1f;
    
    public override float Modulate(float value, float time)
    {
        return value + (extreme / 2) * Mathf.Sin(frequency * time) + (verticalShift - 0.5f);
    }
}