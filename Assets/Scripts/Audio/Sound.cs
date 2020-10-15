using System;
using JetBrains.Annotations;
using UnityEngine;

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
    
    [Header("Modulated Values")]
    
    [SerializeField, Tooltip("Volume of the playback (0 is silent, 1 is full volume)")] 
    private ModulatedFloat volume = default;
    
    [SerializeField, Tooltip("Pitch of the playback (1 is default pitch)")] 
    private ModulatedFloat pitch = default;

    public AudioClip Clip => clip;
    private bool Looping => looping;
    
    // Apply modulation when accessing modulated values
    private float Volume => 
        volume.modulator != null ? volume.modulator.Modulate(volume.value, Time.time) : volume.value;
    private float Pitch => 
        pitch.modulator != null ? pitch.modulator.Modulate(pitch.value, Time.time) : pitch.value;

    /// <summary>
    /// Applies a sound's parameters to an AudioSource (should be called before playing)
    /// </summary>
    /// <param name="source">The AudioSource to apply changes to</param>
    public void ApplyToSource(AudioSource source)
    {
        source.clip = Clip;
        source.loop = Looping;
        source.volume = Volume;
        source.pitch = Pitch;
    }
    
    /// <summary>
    /// Exposes a float to the inspector, associates it with a modulator.
    /// </summary>
    [Serializable]
    private class ModulatedFloat
    {
        public float value = 1f;
        [CanBeNull] public Modulator modulator = default;
    }
}