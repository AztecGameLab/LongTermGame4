using UnityEngine;

[CreateAssetMenu]
public class TerrainType : ScriptableObject
{
    [SerializeField] private Sound walkSound = default;
    [SerializeField] private Sound arrowHitSound = default;
    [SerializeField] private ParticleSystem arrowHitEffect = default;

    public Sound WalkSound => walkSound;
    public Sound ArrowHitSound => arrowHitSound;
    public ParticleSystem ArrowHitEffect => arrowHitEffect;
}
