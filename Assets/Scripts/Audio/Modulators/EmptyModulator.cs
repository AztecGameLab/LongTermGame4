using UnityEngine;

[CreateAssetMenu(fileName = "Empty Modulator", menuName = "Custom/Empty Modulator")]
public class EmptyModulator : Modulator
{
    public override float Modulate(float value, float time)
    {
        return value;
    }
}