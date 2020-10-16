﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField, Tooltip("The panning of the playback")]
    private ModulatedFloat pan = default;

    public AudioClip Clip => clip;
    private bool Looping => looping;
    private bool _isPlaying = false;

    private void OnEnable()
    {
        Modulator none = CreateInstance<EmptyModulator>();

        if (volume.modulator == null) volume.modulator = none;
        if (pitch.modulator == null) pitch.modulator = none;
        if (pan.modulator == null) pan.modulator = none;
    }

    // Apply modulation when accessing modulated values
    private float GetVolume(float time)
    {
        return volume.modulator.Modulate(volume.value, time);
    }
    private float GetPitch(float time)
    {
        return pitch.modulator.Modulate(pitch.value, time);
    }
    private float GetPan(float time)
    {
        return pan.modulator.Modulate(pan.value, time);
    }

    /// <summary>
    /// Applies a sound's parameters to an AudioSource (should be called before playing)
    /// </summary>
    /// <param name="source">The AudioSource to apply changes to</param>
    public IEnumerator ApplyToSource(AudioSource source)
    {
        _isPlaying = true;
        do
        {
            var time = Time.time;
            
            source.clip = Clip;
            source.loop = Looping;
            source.volume = GetVolume(time);
            source.pitch = GetPitch(time);
            source.panStereo = GetPan(time);

            yield return new WaitForEndOfFrame();
        } while (source.isPlaying);

        _isPlaying = false;
    }
    
    /// <summary>
    /// Exposes a float to the inspector, associates it with a modulator.
    /// </summary>
    [Serializable]
    private class ModulatedFloat
    {
        public float value = 1f;
        public Modulator modulator = default;
    }

    public override string ToString()
    {
        var time = Time.time;
        var result = $"{clip.name}\n\tVolume: {GetVolume(time)}\n\tPitch: {GetPitch(time)}\n\tPan: {GetPan(time)}";
        return result;
    }
}