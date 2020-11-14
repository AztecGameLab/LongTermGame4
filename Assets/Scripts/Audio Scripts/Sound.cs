using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

/// <summary>
/// Playable audio wrapper, can be passed into <see cref="AudioManager"/> for playback.
/// </summary>

//TODO: find better way to define default values / getters / setters, maybe move away from delegates?

[CreateAssetMenu(fileName = "New Sound", menuName = "Audio Custom/Sound", order = 1)]
public class Sound : ScriptableObject
{
    [Header("Sound Settings")]
    
    [SerializeField, Tooltip("The audio files to be played.")] 
    protected AudioClip[] clip = new AudioClip[1];
    
    [SerializeField, Tooltip("Whether or not this sound loops when its finished")]
    private bool looping = false;

    [SerializeField, Tooltip("The MixerGroup this sound will be assigned to while it's playing")] 
    protected AudioMixerGroup mixerGroup = default;

    [Header("Modulated Values")] 
    
    [SerializeField, Tooltip("Values that will override certain aspects of the original AudioClip")] 
    protected ModulatedFloat[] modulatedValues = default;

    protected Settings _settings;
    
    protected AudioClip GetClip() 
    {
        return clip[Random.Range(0, clip.Length)];
    }
    public bool IsLooping => looping;
    public bool IsInactive { get; set; } = true;

    private static int _nextId = 0;
    [HideInInspector] public int id;

    private void OnEnable()
    {
        IsInactive = true;
        id = _nextId;
        _nextId++;
        _settings = new Settings(modulatedValues, mixerGroup);
    }

    /// <summary>
    /// Applies a sound's modulations to an AudioSource
    /// </summary>
    /// <remarks>This should be called before the sound is played</remarks>
    /// <param name="mainSource">The main AudioSource used for playback</param>
    /// <param name="schedulingSource">The AudioSource used for scheduling new clips</param>
    public virtual IEnumerator PlayOnSource([NotNull] AudioSource mainSource, [NotNull] AudioSource schedulingSource)
    {
        // Preparing state for playback
        _settings.ApplyValues(mainSource);
        if (IsLooping)
        {
            mainSource.clip = GetClip();
            mainSource.Play();
        }
        else
        {
            mainSource.PlayOneShot(GetClip());
        }
        IsInactive = false;
        var startTime = Time.time;
        
        // Updating state while playing back
        while (!IsInactive) {
            var elapsedTime = Time.time - startTime;
            
            // Iterate through the modulated values, and call their modulate method
            foreach (var val in modulatedValues)
            {
                SoundValue value = val.soundValue;
                float target = val.modulator.Modulate(val.value, elapsedTime);
                
                SetValue(value, target);
            }

            _settings.ApplyValues(mainSource);
            IsInactive |= !mainSource.isPlaying;
            yield return new WaitForEndOfFrame();
        }

        // Ending playback
        mainSource.Stop();
    }

    /// <summary>
    /// Linearly interpolates a setting of this Sound.
    /// </summary>
    /// <param name="value">Which aspect of the sound should be changed</param>
    /// <param name="from">The initial value</param>
    /// <param name="to">The final value</param>
    /// <param name="completion">A float from 0 to 1 representing completion of the interpolation</param>
    public void Lerp(SoundValue value, float from, float to, float completion)
    {
        SetValue(value,Mathf.Lerp(from, to, completion));
    }

    /// <summary>
    /// Returns the value associated with a setting on this Sound.
    /// </summary>
    /// <param name="value">The setting to get the value of</param>
    /// <returns>The value of the specified setting</returns>
    public float GetValue(SoundValue value)
    {
        return _settings.OriginalValues[(int) value];
    }
    
    /// <summary>
    /// Sets the value of a setting on this Sound.
    /// </summary>
    /// <param name="value">The value to modify</param>
    /// <param name="target">The new target for this value</param>
    public void SetValue(SoundValue value, float target)
    {
        _settings.OriginalValues[(int) value] = target;
    }

    public void SetSpacialBlend(float target)
    {
        _settings.SpacialBlend = target;
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"{name} ({(IsInactive ? "Inactive" : "Active")})");
        
        for (var i = 0; i < _settings.OriginalValues.Length; i++)
        {
            var setting = _settings.OriginalValues[i];
            builder.Append( "\n" + Enum.GetNames(typeof(SoundValue))[i] + " : "+ setting);
        }

        return builder.ToString();
    }

    /// <summary>
    /// Exposes a float to the inspector, associates it with a modulator.
    /// </summary>
    [Serializable]
    protected class ModulatedFloat
    {
        public SoundValue soundValue = default;
        public float value = 1f;
        public Modulator modulator = default;
    }
    
    /// <summary>
    /// Different settings that can be modulated and applied to a <see cref="Sound"/>
    /// </summary>
    protected class Settings
    {
        public readonly float[] OriginalValues;
        public float SpacialBlend;
        
        private readonly AudioMixerGroup _mixerGroup;
        
        public Settings(IEnumerable<ModulatedFloat> modulatedFloats, AudioMixerGroup mixerGroup)
        {
            OriginalValues = new float[Enum.GetNames(typeof(SoundValue)).Length];
            _mixerGroup = mixerGroup;
            SpacialBlend = 0f;
            
            OriginalValues[(int) SoundValue.Volume] = 1;
            OriginalValues[(int) SoundValue.Pitch] = 1;
            OriginalValues[(int) SoundValue.Pan] = 0;
            OriginalValues[(int) SoundValue.MaxRange] = 500;
            OriginalValues[(int) SoundValue.MinRange] = 1;

            foreach (var modulated in modulatedFloats)
            {
                OriginalValues[(int) modulated.soundValue] = modulated.value;
            }
        }
        
        public void ApplyValues(AudioSource source)
        {
            source.outputAudioMixerGroup = _mixerGroup;
            source.spatialBlend = SpacialBlend;
            
            source.volume = OriginalValues[(int) SoundValue.Volume];
            source.pitch = OriginalValues[(int) SoundValue.Pitch];
            source.panStereo = OriginalValues[(int) SoundValue.Pan];
            source.maxDistance = OriginalValues[(int) SoundValue.MaxRange];
            source.minDistance = OriginalValues[(int) SoundValue.MinRange];
        }
    }
}