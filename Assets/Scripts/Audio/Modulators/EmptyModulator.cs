using UnityEngine;

/// <summary>
/// A null object for Modulators
/// </summary>

[CreateAssetMenu(fileName = "Empty Modulator", menuName = "Custom/Empty Modulator")]
public class EmptyModulator : Modulator
{
    public override float Modulate(float value, float time)
    {
        return value;
    }
}