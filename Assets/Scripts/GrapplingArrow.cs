using System.Collections;
using UnityEngine;

public class GrapplingArrow : MonoBehaviour
{
    [Header("Grapple Sound Settings")]
    [SerializeField] private Sound launchSound = default;
    [SerializeField] private Sound hitSound = default;
    [SerializeField] private Sound snapSound = default;
    [SerializeField] private Sound fallSound = default;
    
    private SoundInstance _launchSound;
    private SoundInstance _hitSound;
    private SoundInstance _snapSound;
    private SoundInstance _fallSound;
    private AudioManager _audioManager;

    [Header("Grapple Pull Settings")]
    [SerializeField] private Material arrowRopeMaterial = default;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float massThreshold = 4;
    [SerializeField] private float pullRadiusThreshold = 4;

    public static GrapplingArrow CurrentArrow;
    
    private bool _stopPull = false;
    private PlayerManager _player;
    private Rigidbody _playerRb;
    private LineRenderer _line;
    private Vector3[] _linePositions;

    private void Awake()
    {
        // initialize sounds
        _launchSound = launchSound.GenerateInstance();
        _hitSound = hitSound.GenerateInstance();
        _snapSound = snapSound.GenerateInstance();
        _fallSound = fallSound.GenerateInstance();
        
        // initialize line renderer
        _line = gameObject.AddComponent<LineRenderer>();
        _line.material = arrowRopeMaterial == null ? new Material(Shader.Find("Sprites/Default")) : arrowRopeMaterial;
        _line.textureMode = LineTextureMode.Tile;
        _line.widthMultiplier = 0.05f;
        _line.positionCount = 2;
        _linePositions = new Vector3[2];
        
        CurrentArrow = this;
    }

    private void Start()
    {
        // cache singleton references
        _audioManager = AudioManager.Instance();
        _player = PlayerManager.instance;
        
        _audioManager.PlaySound(_launchSound);
        _playerRb = _player.GetComponent<Rigidbody>();
        UpdateLinePosition();
    }

    private void Update()
    {
        UpdateLinePosition();
        
        // If the player tries to shoot while arrow is active, destroy this
        if (Input.GetMouseButton(0))
        {
            _audioManager.PlaySound(_snapSound);
            DestroyThis();
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        //if the collided object DOES have a rigid body, then the grapple will work on it
        if (collision.rigidbody == null || !collision.rigidbody.GetComponent<IsGrappable>()) //If object is not grappable, destroy line renderer on arrow and return
        {
            _audioManager.PlaySound(_fallSound, gameObject);
            DestroyThis();
        }
        else if (!collision.gameObject.CompareTag("arrow"))//To keep players from stacking arrows oddly and from grabbing other arrows
        {
            transform.parent = collision.transform; 
            Destroy(GetComponent<Rigidbody>());
            StartCoroutine(MoveObject(collision));
        }
    }

    private IEnumerator MoveObject(Collision collision)
    {
        _audioManager.PlaySound(_hitSound, gameObject);
        //If the object is above a certain mass, the object will pull the player. Else, the player pulls the object
        if (collision.rigidbody.mass < massThreshold)
        {
            ///////////////////////////////////////////
            // CASE: OBJECT is pulled towards player //
            ///////////////////////////////////////////
            
            collision.rigidbody.velocity = Vector3.zero;
            collision.rigidbody.useGravity = false;
            while (CurrentArrow == this && !_stopPull && Vector3.Distance(collision.transform.position, _player.transform.position) > pullRadiusThreshold) //Test if we want to stop pulling, if not, continue with lerp
            {
                collision.transform.position = Vector3.Lerp(collision.transform.position, _player.transform.position, moveSpeed * 2 * Time.deltaTime);
                yield return null;
            }
            
            collision.rigidbody.useGravity = true;
            DestroyThis();
        }
        
        ///////////////////////////////////////////
        // CASE: PLAYER is pulled towards object //
        ///////////////////////////////////////////
        
        //When player is being pulled, player movement must temporarily be disabled to function properly
        _playerRb.useGravity = false;
        
        while (CurrentArrow == this && !_stopPull && Vector3.Distance(_player.transform.position, transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            _playerRb.velocity = Vector3.zero;
            _player.transform.position = Vector3.Lerp(_player.transform.position, this.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        _playerRb.useGravity = true;
        DestroyThis();
    }

    private void DestroyThis()
    {
        CurrentArrow = null;
        
        _audioManager.Dispose(gameObject, 2);

        Destroy(_line);
        Destroy(this);
    }

    private void UpdateLinePosition()
    {
        _linePositions[0] = _player.transform.position;
        _linePositions[1] = transform.position;
        _line.SetPositions(_linePositions);
    }
}
