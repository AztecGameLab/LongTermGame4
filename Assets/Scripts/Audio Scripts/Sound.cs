using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Custom/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] private AudioClip clip = default;
    [SerializeField] private float volume = 1f;

    public AudioClip Clip => clip;
    public float Volume => volume;

    public void ApplyToSource(AudioSource source)
    {
        source.clip = Clip;
        source.volume = Volume;
    }
}
