using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    static Vector3 LocalPlayerPosition;
    static Vector3 LocalPlayerRotation;
    static float LocalPlayerCameraRotation;
    public bool isEntrance;

    private void Start()
    {
        if (!isEntrance)
            return;

        PlayerManager.instance.transform.position = transform.TransformPoint(LocalPlayerPosition);
        PlayerManager.instance.transform.forward = transform.TransformDirection(LocalPlayerRotation);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEntrance || other.tag != "Player")
            return;

        LocalPlayerPosition = transform.InverseTransformPoint(PlayerManager.instance.transform.position);
        LocalPlayerRotation = transform.InverseTransformDirection(PlayerManager.instance.transform.forward);


        AudioManager.Instance().DisposeAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
