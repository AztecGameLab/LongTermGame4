using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Playable audio wrapper, can be passed into <see cref="AudioManager"/> for playback.
/// </summary>
[CreateAssetMenu(fileName = "New Sound", menuName = "Custom/Sound")]
public class Sound : ScriptableObject
{
    [Header("Sound Settings")]
    
    [SerializeField, Tooltip("The audio file to be played")] 
    private AudioClip clip = default;
    
    [SerializeField, Tooltip("Whether or not this sound loops when its finished")]
    private bool looping = false;

    [SerializeField] 
    private AudioMixerGroup mixerGroup = default;

    [Header("Modulated Values")] 
    
    [SerializeField] 
    private ModulatedFloat[] modulatedValues = default;

    public AudioClip Clip => clip;
    private bool Looping => looping;

    // Apply a value to this audio source
    private delegate void ApplyValue(float value);
    private ApplyValue[] _applicators;

    // Get a value from this audio source
    private delegate float GetValue();
    private GetValue[] _getters;
    
    private AudioSource _source;
    private bool _isPlaying = false;

    private void OnEnable()
    {
        _applicators = new ApplyValue[]
        {
            (val) => _source.volume = val,
            (val) => _source.pitch = val,
            (val) => _source.panStereo = val,
            (val) => _source.maxDistance = val,
            (val) => _source.minDistance = val,
        };

        _getters = new GetValue[]
        {
            () => _source.volume,
            () => _source.pitch,
            () => _source.panStereo,
            () => _source.maxDistance,
            () => _source.minDistance
        };
    }

    /// <summary>
    /// Applies a sound's parameters to an AudioSource (should be called before playing)
    /// </summary>
    /// <param name="source">The AudioSource to apply changes to</param>
    public IEnumerator ApplyToChannel(AudioSource source)
    {
        _source = source;
        _isPlaying = true;
        
        _source.clip = Clip;
        _source.loop = Looping;
        _source.outputAudioMixerGroup = mixerGroup;
        var startTime = Time.time;
        
        do
        {
            var elapsedTime = Time.time - startTime;
            
            foreach (var val in modulatedValues)
            {
                var index = (int) val.setting;
                _applicators[index](val.modulator.Modulate(val.value, elapsedTime));
            }

            yield return new WaitForEndOfFrame();
        } while (_source.isPlaying);

        _isPlaying = false;
    }
    
    public void Lerp(SoundSetting setting, float from, float to, float completion)
    {
        if (_source == null) return;
        
        _applicators[(int) setting](Mathf.Lerp(from, to, completion));
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
}