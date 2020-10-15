using UnityEngine;

namespace Audio.Modulators
{
    /// <summary>
    /// Shifts a float by a configurable random amount
    /// </summary>
    [CreateAssetMenu(fileName = "Random Modulator", menuName = "Custom/Random Modulator")]
    public class Random : Modulator
    {
        [SerializeField, Tooltip("Hi")] private float randomMin = default;
        [SerializeField] private float randomMax = default;
    
        public override float Modulate(float value, float time)
        {
            return value + UnityEngine.Random.Range(randomMin, randomMax);
        }
    }
}