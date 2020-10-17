using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Playable audio wrapper, can be passed into <see cref="AudioManager"/> for playback.
/// </summary>

//TODO: find better way to define default values / getters / setters, maybe move away from delegates?

[CreateAssetMenu(fileName = "New Sound", menuName = "Custom/Sound")]
public class Sound : ScriptableObject
{
    [Header("Sound Settings")]
    
    [SerializeField, Tooltip("The audio file to be played")] 
    private AudioClip clip = default;
    
    [SerializeField, Tooltip("Whether or not this sound loops when its finished")]
    private bool looping = false;

    [SerializeField, Tooltip("The MixerGroup this sound will be assigned to while it's playing")] 
    private AudioMixerGroup mixerGroup = default;

    [Header("Modulated Values")] 
    
    [SerializeField, Tooltip("Values that will override certain aspects of the original AudioClip")] 
    private ModulatedFloat[] modulatedValues = default;

    public AudioClip Clip => clip;
    private bool Looping => looping;

    /// <summary>
    /// Applies a value to this sound's AudioSource
    /// </summary>
    /// <param name="value">The value that should be applied</param>
    private delegate void ApplyValue(float value);
    private ApplyValue[] _setters;

    /// <summary>
    /// Gets a value from this sound's AudioSource
    /// </summary>
    private delegate float GetValue();
    private GetValue[] _getters;

    /// <summary>
    /// The AudioSource currently playing this sound
    /// </summary>
    [CanBeNull] private AudioSource _source = null;

    private void OnEnable()
    {
        InitializeGetters();
        InitializeSetters();
    }

    /// <summary>
    /// Applies a sound's modulations to an AudioSource
    /// </summary>
    /// <remarks>This should be called before the sound is played</remarks>
    /// <param name="source">The AudioSource to apply changes to</param>
    public IEnumerator ApplyToChannel([NotNull] AudioSource source)
    {
        _source = source;
        
        // Apply the non-modulating values
        ApplyDefaultValues(source);

        var startTime = Time.time;

        do
        {
            var elapsedTime = Time.time - startTime;
            
            // Iterate through the modulated values, and call their modulate method
            foreach (var val in modulatedValues)
            {
                var index = (int) val.setting;
                _setters[index](val.modulator.Modulate(val.value, elapsedTime));
            }
            yield return new WaitForEndOfFrame();
            
        } while (source.isPlaying);

        // Set source to null because this source may be re-allocated for another Sound.
        // This ensures we don't step on the next sound's toes with our modulation.
        _source = null;
    }

    /// <summary>
    /// Applies specified default values to the AudioSource
    /// </summary>
    /// <param name="source">The source to apply the values to</param>
    private void ApplyDefaultValues(AudioSource source)
    {
        source.clip = Clip;
        source.loop = Looping;
        source.outputAudioMixerGroup = mixerGroup;
        
        source.volume = 1f;
        source.pitch = 1f;
        source.panStereo = 0f;
        source.maxDistance = 1f;
        source.minDistance = 1f;
    }

    /// <summary>
    /// Linearly interpolates a setting of this Sound.
    /// </summary>
    /// <param name="setting">Which aspect of the sound should be changed</param>
    /// <param name="from">The initial value of the setting</param>
    /// <param name="to">The final value of the setting</param>
    /// <param name="completion">A float from 0 to 1 representing completion of the interpolation</param>
    public void Lerp(SoundSetting setting, float from, float to, float completion)
    {
        _setters[(int) setting](Mathf.Lerp(from, to, completion));
    }

    /// <summary>
    /// Returns the value associated with a setting on this Sound.
    /// </summary>
    /// <param name="setting">The setting to get the value of</param>
    /// <returns>The value of the specified setting</returns>
    public float GetSetting(SoundSetting setting)
    {
        return _getters[(int) setting]();
    }

    /// <summary>
    /// Exposes a float to the inspector, associates it with a modulator.
    /// </summary>
    [Serializable]
    private class ModulatedFloat
    {
        public SoundSetting setting = default;
        public float value = 1f;
        public Modulator modulator = default;
    }
    
    #region initializers

    private void InitializeGetters()
    {
        _getters = new GetValue[]
        {
            () => 
            { 
                if (_source != null) return _source.volume;
                return 0; 
            },
            () =>
            {
                if (_source != null) return _source.pitch;
                return 0;
            },
            () =>
            {
                if (_source != null) return _source.panStereo;
                return 0;
            },
            () =>
            {
                if (_source != null) return _source.maxDistance;
                return 0;
            },
            () =>
            {
                if (_source != null) return _source.minDistance;
                return 0;
            }
        };
    }
    
    private void InitializeSetters()
    {
        _setters = new ApplyValue[]
        {
            val => { if (_source != null) _source.volume = val; },
            val => { if (_source != null) _source.pitch = val; },
            val => { if (_source != null) _source.panStereo = val; },
            val => { if (_source != null) _source.maxDistance = val; },
            val => { if (_source != null) _source.minDistance = val; }
        };
    }
    
    #endregion
}