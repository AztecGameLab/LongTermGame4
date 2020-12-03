using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class Lava : MonoBehaviour
{
    [Header("Lava Settings")]
    [SerializeField] private Sound lavaAmbientSound = default;
    
    [Header("Lava Pop Settings")]
    [SerializeField] private Sound lavaPopSound = default;
    [SerializeField] private float popFrequencySeconds = default;
    [SerializeField, Range(0, 1)] private float popRandomOffsetSeconds = 0;
    [SerializeField] private GameObject lavaPopPrefab = default;
    
    private AudioManager _audioManager;
    private SoundInstance _lavaAmbientSound;
    private Mesh _mesh;
    private Renderer _renderer;

    private float _timeSincePop = 0f;
    private float _nextPop;

    private void Awake()
    {
        _lavaAmbientSound = lavaAmbientSound.GenerateInstance();
        _mesh = GetComponent<MeshFilter>().mesh;
        _renderer = GetComponent<Renderer>();
        _nextPop = popFrequencySeconds + Random.Range(-popRandomOffsetSeconds, popRandomOffsetSeconds);
    }

    private void Start()
    {
        _audioManager = AudioManager.Instance();
        _audioManager.PlaySound(_lavaAmbientSound, gameObject);
    }

    private void Update()
    {
        _timeSincePop += Time.deltaTime;
        if (_timeSincePop < _nextPop) return;

        var offsetX = Random.Range(-_renderer.bounds.extents.x, _renderer.bounds.extents.x);
        var offsetZ = Random.Range(-_renderer.bounds.extents.z, _renderer.bounds.extents.z);
        Vector3 randomPositionOnMesh = transform.position + new Vector3(offsetX, 0, offsetZ);

        var lavaPopInstance = Instantiate(lavaPopPrefab, transform).GetComponent<LavaPop>();
        lavaPopInstance.Initialize(randomPositionOnMesh, _audioManager, lavaPopSound);

        _timeSincePop = 0;
    }
}
