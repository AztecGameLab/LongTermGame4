using UnityEngine;

public class StickyArrowScript : MonoBehaviour
{
    private ParticleSystem _arrowHitEffect;
    private SoundInstance _arrowHitSound;
    private AudioManager _audioManager;
    
    Rigidbody arrowRB;
    Quaternion arrowRotation;
    private Collider _collider;

    private void Start()
    {
        _audioManager = AudioManager.Instance();
        arrowRB = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ArrowReflector script = collision.gameObject.GetComponent<ArrowReflector>();

        //To keep players from stacking arrows oddly, and to test if the arrow should bounce, should not stick to player
        if (!collision.gameObject.CompareTag("arrow") && script == null &&  !collision.gameObject.CompareTag("Player"))
        {
            var contact = collision.GetContact(0);

            //changing collision detection mode to avoid warning from unity
            arrowRB.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            arrowRB.isKinematic = true;
            _collider.enabled = false;
            //Make sure the arrow is pointing in the right dirrection using last known rotation before collision.
            gameObject.transform.rotation = arrowRotation;
            //Object sticks to where it first made contact, sinks in just enough to be embedded. 
            gameObject.transform.position = contact.point + transform.forward * -.4f;
            //Checks if object is a movable object, will set as parrent as to move with object. 
            if (collision.rigidbody != null)
            {
                //Still a bug here, in that the arrow will rotate after having it's parent set. Still researching ways around this
                //This only works with objects that have a scale value of (1,1,1)
                gameObject.transform.parent = collision.gameObject.transform;
                //To avoid objects being trigured repeatedly by the arrow you can use the following
                //If you want the arrows to still interact physically with the world.
                Destroy(arrowRB);
                //could use the following if you don't mind arrows phasing through solid objects
            }

            var targetTerrain = collision.transform.GetComponent<Terrain>();
            if (targetTerrain != null)
            {
                _arrowHitSound = targetTerrain.terrainType.ArrowHitSound.GenerateInstance();
                _arrowHitEffect = targetTerrain.terrainType.ArrowHitEffect;
            }
            else
            {
                _arrowHitSound = PlayerManager.instance.s_playerMovement.Terrain.ArrowHitSound.GenerateInstance();
                _arrowHitEffect = PlayerManager.instance.s_playerMovement.Terrain.ArrowHitEffect;
            }
            _audioManager.PlaySound(_arrowHitSound, gameObject);
            // create the particle effect
            var effect = 
                Instantiate(_arrowHitEffect, contact.point, Quaternion.FromToRotation(Vector3.forward, -transform.forward));
            // destroy it once it finishes playing
            var particleSystemDuration = effect.GetComponent<ParticleSystem>().main.duration;
            Destroy(effect, particleSystemDuration);
            Invoke(nameof(DisposeAudio), particleSystemDuration);
        }
    }

    private void DisposeAudio()
    {
        _audioManager.Dispose(gameObject);
    }
    
    private void LateUpdate()
    {
        //grab the last rotation before collision's rotation
        arrowRotation = gameObject.transform.rotation;
    }
}