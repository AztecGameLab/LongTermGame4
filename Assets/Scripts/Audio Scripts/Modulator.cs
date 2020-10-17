using UnityEngine;

/// <summary>
/// Performs some operation on a float, usually changing it over time.
/// </summary>

public abstract class Modulator : ScriptableObject
{
    // A nice feature for audio people to leave comments about the intended use of modulators, ect.
    [SerializeField, TextArea]
    private string notes;
    
    /// <summary>
    /// Performs some operation on a float, usually changing it over time.
    /// </summary>
    /// <param name="value">The value that should be changed.</param>
    /// <param name="time">The current time.</param>
    /// <returns>The value that was passed in, after being modulated in some way.</returns>
    public abstract float Modulate(float value, float time);
}