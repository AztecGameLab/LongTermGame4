using UnityEngine;

/// <summary>
/// Shifts a float by a configurable random amount
/// </summary>
[CreateAssetMenu(fileName = "New Random Modulator", menuName = "Custom/Random Modulator")]
public class Random : Modulator
{
    [Header("Random Modulation Settings")]
    
    [SerializeField, Tooltip("How low this value can be modulated")] 
    private float randomMin = default;
    
    [SerializeField, Tooltip("How high this value can be modulated")] 
    private float randomMax = default;

    /// <summary>
    /// Shift a float by a configurable random amount.
    /// </summary>
    /// <param name="value">The value to be modulated</param>
    /// <param name="time">The current time</param>
    /// <returns>The value after it has been modified</returns>
    public override float Modulate(float value, float time)
    {
        return value + UnityEngine.Random.Range(randomMin, randomMax);
    }
}