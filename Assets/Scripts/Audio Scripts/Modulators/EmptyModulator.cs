using UnityEngine;

/// <summary>
/// A null object for Modulators
/// <remarks>Commented out asset menu attribute, because we don't really need to make multiple of these</remarks>
/// </summary>

//[CreateAssetMenu(fileName = "Empty Modulator", menuName = "Audio Custom/Modulator")]
public class EmptyModulator : Modulator
{
    public override float Modulate(float value, float time)
    {
        return value;
    }
}