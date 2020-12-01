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
    [SerializeField] private Material arrowRopeMaterial;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private float massThreshold = 4;
    [SerializeField] private float pullRadiusThreshold = 4;

    public static GrapplingArrow CurrentArrow;
    
    private bool _collided = false;
    private bool _stopPull = false;
    private PlayerManager _player;
    private Rigidbody _playerRb;
    private LineRenderer _line;

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
        
        CurrentArrow = this;
    }

    private void Start()
    {
        // cache singleton references
        _audioManager = AudioManager.Instance();
        _player = PlayerManager.instance;
        
        _audioManager.PlaySound(_launchSound);
        _playerRb = _player.GetComponent<Rigidbody>();
        _line.SetPositions(new[] { _player.transform.position, transform.position });
    }

    private void Update()
    {
        _line.SetPositions(new[] { _player.transform.position, transform.position });
        
        if (Input.GetMouseButton(0))
        {
            _audioManager.PlaySound(_snapSound);
            DestroyThis();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(_collided)//so it can only collide once
            return;
        
        //if the collided object DOES have a rigid body, then the grapple will work on it

        if (collision.rigidbody == null || !collision.rigidbody.GetComponent<IsGrappable>()) //If object is not grappable, destroy line renderer on arrow and return
        {
            _audioManager.PlaySound(_fallSound, gameObject);
            Invoke(nameof(DestroyThis), 1f);
        }
        else if (!collision.gameObject.CompareTag("arrow"))//To keep players from stacking arrows oddly and from grabbing other arrows
        {
            _collided = true;
            transform.parent = collision.transform; 
            Destroy(GetComponent<Rigidbody>());
            StartCoroutine(MoveObject(collision));
        }
        
        //Debug.Log(collision.rigidbody.mass);
    }

    private IEnumerator MoveObject(Collision collision)
    {
        _audioManager.PlaySound(_hitSound, gameObject);
        LineRenderer line = GetComponent<LineRenderer>();
        var points = new Vector3[2];
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
                points[0] = _player.transform.position;
                points[1] = transform.position;
                if(line)
                    line.SetPositions(points); // update line vertices

                yield return null;
            }
            
            //Set bool variables back to default
            _stopPull = false;
            collision.rigidbody.useGravity = true;
            Destroy(line);
            Invoke(nameof(DisposeAudio), 2f);
            yield break;
        }
        
        ///////////////////////////////////////////
        // CASE: PLAYER is pulled towards object //
        ///////////////////////////////////////////
        
        //When player is being pulled, player movement must temporarily be disabled to function properly
        _playerRb.useGravity = false;
        
        while (CurrentArrow == this && !_stopPull && Vector3.Distance(_player.transform.position, this.transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            _player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _player.transform.position = Vector3.Lerp(_player.transform.position, this.transform.position, moveSpeed * Time.deltaTime);
            points[0] = _player.transform.position;
            points[1] = transform.position;
            if(line)
                line.SetPositions(points); // update line vertices
            yield return null;
        }

        _playerRb.useGravity = true;
        //Set bool variables back to default
        _stopPull = false;
        Destroy(line);
        //Debug.Log(isPulling);

        CurrentArrow = null;
        Invoke(nameof(DisposeAudio), 2f);
    }

    private void DestroyThis()
    {
        DisposeAudio();
        Destroy(_line);
        Destroy(this);
    }

    private void DisposeAudio()
    {
        _audioManager.Dispose(gameObject);
    }
}
