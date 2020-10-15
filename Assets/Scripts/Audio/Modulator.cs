using System;
using Audio.Modulators;
using UnityEngine;

namespace Audio
{
    /// <summary>
    /// Performs some operation on a float, usually changing it over time.
    /// </summary>
    public abstract class Modulator : ScriptableObject
    {
        /// <summary>
        /// Performs some operation on a float, usually changing it over time.
        /// </summary>
        /// <param name="value">The value that should be changed.</param>
        /// <param name="time">The current time.</param>
        /// <returns>The value that was passed in, after being modulated in some way.</returns>
        public abstract float Modulate(float value, float time);
    }
}