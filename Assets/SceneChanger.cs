using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    static Vector3 LocalPlayerPosition;
    static Vector3 LocalPlayerRotation;
    static float LocalPlayerCameraRotation;
    public string nextSceneName;
    public bool isEntrance;

    private void Start()
    {
        if (!isEntrance)
            return;

        PlayerManager.instance.transform.position = transform.TransformPoint(LocalPlayerPosition);
        PlayerManager.instance.transform.forward = transform.TransformDirection(LocalPlayerRotation);
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isEntrance || other.tag != "Player")
            return;

        LocalPlayerPosition = transform.InverseTransformPoint(PlayerManager.instance.transform.position);
        LocalPlayerRotation = transform.InverseTransformDirection(PlayerManager.instance.transform.forward);



        if (nextSceneName != "")
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        else
            print("SceneChanger needs next scene name");
    }
}
