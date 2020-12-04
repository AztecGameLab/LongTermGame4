using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float footstepTime = 1f;
    [SerializeField] private TerrainType defaultTerrain = default;
    private float _timeSinceFootstep;
    private SoundInstance _footstepSound;
    private SoundInstance _jumpSound;
    private SoundInstance _jumpLandSound;
    private AudioManager _audioManager;
    private TerrainType _currentTerrain;

    public TerrainType Terrain => _currentTerrain;
    
    public float moveSpeed = 5f;
    public float rotateVerticleSpeed = 5f;
    public float rotateHorizontalSpeed = 5f;
    public float jumpSpeed = 8f;
    public Rigidbody body;
    public Camera camera;

    private bool ground = true;
    private bool groundPrev = true;

    public bool IsActive => inputX != 0 || inputZ != 0;
    float inputX;
    float inputZ;

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = AudioManager.Instance();
        _currentTerrain = defaultTerrain;
        _footstepSound = Terrain.WalkSound.GenerateInstance();
        _jumpSound = Terrain.JumpSound.GenerateInstance();
        _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector3(0, body.velocity.y, 0);//This is kains very dumb line but it works so shhhhh

        // //Looking
        // Vector3 cameraAngle = camera.transform.rotation.eulerAngles;
        // float rotateVertical = Input.GetAxis("Mouse Y") * rotateVerticleSpeed;
        // float angleRotateX = cameraAngle.x - rotateVertical;
        // if (angleRotateX > 180)
        // {
        //     angleRotateX = angleRotateX - 360;
        // }
        // angleRotateX = Mathf.Clamp(angleRotateX, -60, 60);
        // rotateVertical = cameraAngle.x - angleRotateX;
        // camera.transform.Rotate(-rotateVertical, 0f, 0f);

        // float rotateHorizontal = Input.GetAxis("Mouse X") * rotateHorizontalSpeed;
        // transform.Rotate(0f, rotateHorizontal, 0f);

        //Movement Input
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (ground)
            {
                body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
                
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
                {
                    // if the terrain changed, update the sound and terrain variables
                    UpdateTerrain(hit.transform.GetComponent<Terrain>());
                
                    _audioManager.PlaySound(_jumpSound);
                }
            }
        }
        //Debug.Log(ground);

        // plays footstep sound if we exceed the footstep time
        if (_timeSinceFootstep > footstepTime && ground && IsActive)
        {
            _timeSinceFootstep = 0f;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
            {
                // if the terrain changed, update the sound and terrain variables
                UpdateTerrain(hit.transform.GetComponent<Terrain>());
                
                _audioManager.PlaySound(_footstepSound);
            }
        }
        else if (IsActive)
        {
            _timeSinceFootstep += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        //Movement
        body.position += (transform.forward * inputZ + transform.right * inputX) * (moveSpeed * Time.deltaTime);

        //ground check
        groundPrev = ground;
        
        RaycastHit hit;
        ground = Physics.SphereCast(transform.position, 0.35f, Vector3.down, out hit, 0.1f);
        
        if (ground && groundPrev == false)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
            {
                // if the terrain changed, update the sound and terrain variables
                UpdateTerrain(hit.transform.GetComponent<Terrain>());
                
                _audioManager.PlaySound(_jumpLandSound);
            }
        }
    }

    // void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         ground = true;
    //     }
    // }

    // void OnCollisionExit(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         ground = false;
    //     }
    // }

    private void UpdateTerrain(Terrain newTerrain)
    {
        if (newTerrain != null && newTerrain.terrainType != Terrain)
        {
            _currentTerrain = newTerrain.terrainType;
            _footstepSound = Terrain.WalkSound.GenerateInstance();
            _jumpSound = Terrain.JumpSound.GenerateInstance();
            _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();
        }
        else if (newTerrain == null)
        {
            _currentTerrain = defaultTerrain;
            _footstepSound = Terrain.WalkSound.GenerateInstance();
            _jumpSound = Terrain.JumpSound.GenerateInstance();
            _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();
        }
    }
}
