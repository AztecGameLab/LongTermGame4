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
    [SerializeField] private bool usePhysicsPull = false;

    public static GrapplingArrow CurrentArrow;
    
    private PlayerManager _player;
    private Rigidbody _playerRb;
    private Collider _collider;
    private LineRenderer _line;
    private Vector3[] _linePositions;
    private bool _canceled = false;
    
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

        _collider = GetComponent<Collider>();
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
        if (_canceled) DestroyThis();
        UpdateLinePosition();
        
        // If the player tries to shoot while arrow is active, destroy this
        if (Input.GetMouseButton(0))
        {
            _audioManager.PlaySound(_snapSound);
            _canceled = true;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null || !collision.rigidbody.GetComponent<IsGrappable>())
        {
            // We hit something that cannot be used to grapple, so this script is no longer needed
            _audioManager.PlaySound(_fallSound, gameObject);
            DestroyThis();
        }
        else if (!collision.gameObject.CompareTag("arrow"))
        {
            // We hit something that can be grappled, determine what object is pulled
            _audioManager.PlaySound(_hitSound, gameObject);
            transform.parent = collision.transform;
            Destroy(GetComponent<Rigidbody>());
            _collider.enabled = false;
            
            StartCoroutine(collision.rigidbody.mass < massThreshold
                ? usePhysicsPull ? PullToPlayerPhysics(collision.rigidbody) : PullToPlayer(collision.rigidbody)
                : PullPlayerToObject());
        }
    }

    private IEnumerator PullToPlayer(Rigidbody rb)
    {
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        
        while (!_canceled && Vector3.Distance(rb.position, _player.transform.position) > pullRadiusThreshold) //Test if we want to stop pulling, if not, continue with lerp
        {
            rb.transform.position = Vector3.Lerp(rb.transform.position, _player.transform.position, moveSpeed * 2 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        
        rb.useGravity = true;
        DestroyThis();
    }

    private IEnumerator PullPlayerToObject()
    {
        _playerRb.useGravity = false;
        
        while (!_canceled && Vector3.Distance(_player.transform.position, transform.position) > pullRadiusThreshold)//Test if we want to stop pulling, if not, continue with lerp
        {
            _playerRb.velocity = Vector3.zero;
            _player.transform.position = Vector3.Lerp(_player.transform.position, transform.position, moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        _playerRb.useGravity = true;
        DestroyThis();
    }
    
    private IEnumerator PullToPlayerPhysics(Rigidbody rb)
    {
        rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, 3);
        
        var vectorToPlayer = _player.transform.position - transform.position;
        while (!_canceled && vectorToPlayer.magnitude > pullRadiusThreshold)
        {
            vectorToPlayer = _player.transform.position - rb.position;
            rb.AddForceAtPosition(vectorToPlayer, transform.position);
            rb.velocity = rb.velocity.normalized * Mathf.Max(rb.velocity.magnitude, 3);
            yield return new WaitForEndOfFrame();
        }

        rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, 3);

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
