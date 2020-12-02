using UnityEngine;

public class LavaPop : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private SoundInstance _popSound;
    private AudioManager _audioManager;
        
    public void Initialize(Vector3 position, AudioManager manager, Sound popSound)
    {
        _audioManager = manager;
        _particleSystem = GetComponent<ParticleSystem>();
        _popSound = popSound.GenerateInstance();
        transform.position = position;

        _particleSystem.Play();
        _audioManager.PlaySound(_popSound, gameObject);
        Invoke(nameof(EndPop), _particleSystem.main.duration);
    }

    private void EndPop()
    {
        _audioManager.Dispose(gameObject);
        _particleSystem.Stop();
            
        Destroy(gameObject);
    }
}