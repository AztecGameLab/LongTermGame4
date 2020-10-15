using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Sound", menuName = "Custom/Sound")]
    public class Sound : ScriptableObject
    {
        [SerializeField] private AudioClip clip = default;
        [SerializeField] private bool looping = false;
        [SerializeField] private ModulatedFloat volume = default;
        [SerializeField] private ModulatedFloat pitch = default;

        public AudioClip Clip => clip;
        private bool Looping => looping;
        private float Volume => 
            volume.modulator != null ? volume.modulator.Modulate(volume.value, Time.time) : volume.value;
        private float Pitch => 
            pitch.modulator != null ? pitch.modulator.Modulate(pitch.value, Time.time) : pitch.value;

        public void ApplyToSource(AudioSource source)
        {
            Debug.Log(Volume);
            
            source.clip = Clip;
            source.loop = Looping;
            source.volume = Volume;
            source.pitch = Pitch;
        }
        
        [Serializable]
        private class ModulatedFloat
        {
            public float value = 1f;
            [CanBeNull] public Modulator modulator = default;
        }
    }
}
