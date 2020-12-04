using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

/// <summary>
/// Playable audio wrapper, can be passed into <see cref="AudioManager"/> for playback.
/// </summary>
[CreateAssetMenu(fileName = "New Sound", menuName = "Audio Custom/Sound", order = 1)]
public class Sound : ScriptableObject
{
    [Header("Sound Settings")]
    
    [Tooltip("The audio files to be played.")] 
    public AudioClip[] clip = new AudioClip[1];
    
    [Tooltip("Whether or not this sound loops when its finished")]
    public bool looping = false;

    [Tooltip("The MixerGroup this sound will be assigned to while it's playing")] 
    public AudioMixerGroup mixerGroup = default;

    [Header("Modulated Values")] 
    
    [Tooltip("Values that will override certain aspects of the original AudioClip")] 
    public ModulatedFloat[] modulatedValues = default;
    
    public SoundInstance GenerateInstance()
    {
        return new SoundInstance(this);
    }
}

public class SoundInstance
{
    protected readonly InstanceSettings _settings;
    private readonly string _name;

    public bool IsInactive { get; set; }
    public string Name => _name;
    public bool IsLooping => _settings.Looping;
    public float Length => _settings.Clips[0].length;
    
    public SoundInstance(Sound sound)
    {
        _name = sound.name;
        _settings = new InstanceSettings(sound);
        IsInactive = true;
    }
    
    /// <summary>
    /// Applies a sound's modulations to an AudioSource
    /// </summary>
    /// <remarks>This should be called before the sound is played</remarks>
    /// <param name="mainSource">The main AudioSource used for playback</param>
    /// <param name="schedulingSource">The AudioSource used for scheduling new clips</param>
    public virtual IEnumerator PlayOnSource([NotNull] AudioSource mainSource, [NotNull] AudioSource schedulingSource)
    {
        // Apply values to the AudioSource and start playing the sound
        _settings.ApplyToSource(mainSource);
        _settings.PlayOnSource(mainSource);
        
        IsInactive = false;
        var startTime = Time.time;
    
        // Updating state while playing back
        while (!IsInactive) {
            var elapsedTime = Time.time - startTime;
        
            // Iterate through the modulated values, and call their modulate method
            foreach (var val in _settings.ModulatedValues)
            {
                SoundValue value = val.soundValue;
                float target = val.modulator.Modulate(val.value, elapsedTime);
            
                SetValue(value, target);
            }

            _settings.ApplyToSource(mainSource);
            IsInactive |= !mainSource.isPlaying;
            yield return new WaitForEndOfFrame();
        }

        // Ending playback
        if (mainSource != null) mainSource.Stop();
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
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append($"{_name} ({(IsInactive ? "Inactive" : "Active")})");
    
        for (var i = 0; i < _settings.OriginalValues.Length; i++)
        {
            var setting = _settings.OriginalValues[i];
            builder.Append( "\n" + Enum.GetNames(typeof(SoundValue))[i] + " : "+ setting);
        }

        return builder.ToString();
    }
    
    /// <summary>
    /// Different settings that can be modulated and applied to a <see cref="Sound"/>
    /// </summary>
    protected class InstanceSettings
    {
        public readonly float[] OriginalValues;
        public readonly ModulatedFloat[] ModulatedValues;
        public readonly AudioClip[] Clips;
        
        public readonly bool Looping;
        private readonly AudioMixerGroup _mixerGroup;
        
        public InstanceSettings(Sound sound)
        {
            OriginalValues = new float[Enum.GetNames(typeof(SoundValue)).Length];
            ModulatedValues = sound.modulatedValues;
            Clips = sound.clip;
            
            sound.modulatedValues.CopyTo(ModulatedValues, 0);
            
            _mixerGroup = sound.mixerGroup;
            Looping = sound.looping;
            
            OriginalValues[(int) SoundValue.Volume] = 1;
            OriginalValues[(int) SoundValue.Pitch] = 1;
            OriginalValues[(int) SoundValue.Pan] = 0;
            OriginalValues[(int) SoundValue.MaxRange] = 500;
            OriginalValues[(int) SoundValue.MinRange] = 1;
            OriginalValues[(int) SoundValue.SpacialBlend] = 1;
            
            foreach (var modulated in ModulatedValues)
            {
                OriginalValues[(int) modulated.soundValue] = modulated.value;
            }
        }
        
        public void ApplyToSource(AudioSource source, AudioClip clip = null)
        {
            source.clip = clip == null ? Clips[Random.Range(0, Clips.Length)] : clip;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.outputAudioMixerGroup = _mixerGroup;
            source.loop = Looping;

            source.volume = OriginalValues[(int) SoundValue.Volume];
            source.pitch = OriginalValues[(int) SoundValue.Pitch];
            source.panStereo = OriginalValues[(int) SoundValue.Pan];
            source.maxDistance = OriginalValues[(int) SoundValue.MaxRange];
            source.minDistance = OriginalValues[(int) SoundValue.MinRange];
            source.spatialBlend = OriginalValues[(int) SoundValue.SpacialBlend];
        }

        public void PlayOnSource(AudioSource source)
        {
            if (Looping)
            {
                source.Play();
            }
            else
            {
                source.PlayOneShot(Clips[Random.Range(0, Clips.Length)]);
            }
        }
    }
}

/// <summary>
/// Exposes a float to the inspector, associates it with a modulator.
/// </summary>
[Serializable]
public class ModulatedFloat
{
    public SoundValue soundValue = default;
    public float value = 1f;
    public Modulator modulator = default;
}