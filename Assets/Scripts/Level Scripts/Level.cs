using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    [SerializeField] private string sceneName;
    
    private 
    
    void Start()
    {
        SceneManager.LoadScene(sceneName);
    }

    public struct PlayerData
    {
        private Vector3 position;
        private Quaternion rotation;
    }
}
