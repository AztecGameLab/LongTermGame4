using System;
using System.Collections;
using System.Collections.Generic;
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

    // A method that applied a value to an audio source
    private delegate void ApplyValue(float value, AudioSource source);
    private ApplyValue[] _applicators;

    private void OnEnable()
    {
        _applicators = new ApplyValue[]
        {
            ApplyVolume,
            ApplyPitch,
            ApplyPan,
            ApplyMaxRange3D,
            ApplyMinRange3D
        };
    }

    /// <summary>
    /// Applies a sound's parameters to an AudioSource (should be called before playing)
    /// </summary>
    /// <param name="source">The AudioSource to apply changes to</param>
    public IEnumerator ApplyToChannel(AudioSource source)
    {
        source.clip = Clip;
        source.loop = Looping;
        source.outputAudioMixerGroup = mixerGroup;
        var startTime = Time.time;
        
        do
        {
            var elapsedTime = Time.time - startTime;
            
            foreach (var val in modulatedValues)
            {
                _applicators[(int) val.setting](val.modulator.Modulate(val.value, elapsedTime), source);
            }

            yield return new WaitForEndOfFrame();
        } while (source.isPlaying);
    }

    private static void ApplyVolume(float val, AudioSource src) { src.volume = val; }
    private static void ApplyPitch(float val, AudioSource src) { src.pitch = val; }
    private static void ApplyPan(float val, AudioSource src) { src.panStereo = val; }
    private static void ApplyMaxRange3D(float val, AudioSource src) { src.maxDistance = val; }
    private static void ApplyMinRange3D(float val, AudioSource src) { src.minDistance = val; }
    
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