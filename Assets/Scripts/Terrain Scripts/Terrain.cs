using UnityEngine;

/// <summary>
/// Associates a GameObject with a type of terrain, which provides information on hit sounds, walk sounds, ect.
/// </summary>
public class Terrain : MonoBehaviour
{
    /// <summary>
    /// The type of terrain associated with this GameObject.
    /// </summary>
    [Tooltip("The type of terrain associated with this GameObject.")]
    public TerrainType terrainType;
}