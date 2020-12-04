using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class Lava : MonoBehaviour
{
    [Header("Lava Settings")]
    [SerializeField] private Sound lavaAmbientSound = default;
    [SerializeField] private float lavaAmbientStrength = default;
    
    [Header("Lava Pop Settings")]
    [SerializeField] private Sound lavaPopSound = default;
    [SerializeField] private float popFrequencySeconds = 5;
    [SerializeField, Range(0, 1)] private float popRandomOffsetSeconds = 1;
    [SerializeField] private GameObject lavaPopPrefab = default;
    [SerializeField] private Collider lavaCollider = default;
    
    private AudioManager _audioManager;
    private SoundInstance _lavaAmbientSound;
    private Renderer _renderer;
    private Transform _player;
    
    private float _timeSincePop = 0f;
    private float _nextPop;

    private void Awake()
    {
        _lavaAmbientSound = lavaAmbientSound.GenerateInstance();
        _renderer = GetComponent<Renderer>();
        _nextPop = popFrequencySeconds + Random.Range(-popRandomOffsetSeconds, popRandomOffsetSeconds);
    }

    private void Start()
    {
        _player = PlayerManager.instance.transform;
        _audioManager = AudioManager.Instance();
        _audioManager.PlaySound(_lavaAmbientSound);
    }

    private void OnDestroy()
    {
        _audioManager.StopSound(_lavaAmbientSound);
    }

    private void Update()
    {
        AdjustLavaVolume();

        if (popFrequencySeconds < 0) return;
        _timeSincePop += Time.deltaTime;
        if (_timeSincePop < _nextPop) return;

        var offsetX = Random.Range(-_renderer.bounds.extents.x, _renderer.bounds.extents.x);
        var offsetZ = Random.Range(-_renderer.bounds.extents.z, _renderer.bounds.extents.z);
        Vector3 randomPositionOnMesh = transform.position + new Vector3(offsetX, 0, offsetZ);

        var lavaPopInstance = Instantiate(lavaPopPrefab, transform).GetComponent<LavaPop>();
        lavaPopInstance.Initialize(randomPositionOnMesh, _audioManager, lavaPopSound);

        _timeSincePop = 0;
    }

    private void AdjustLavaVolume()
    {
        Vector3 closestPointToPlayer = Physics.ClosestPoint(_player.position, lavaCollider, lavaCollider.transform.position,
            lavaCollider.transform.rotation);
        float distFromPlayer = Vector3.Distance(closestPointToPlayer, _player.position);
        _lavaAmbientSound.SetValue(SoundValue.Volume, Mathf.Clamp01(lavaAmbientStrength / distFromPlayer));
    }
}
