using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager instance { get { return _instance; } }
    public float Health = 100;

    [HideInInspector]
    public PlayerMovement s_playerMovement;
    [HideInInspector]
    public Looking s_looking;
    [HideInInspector]
    public PlayerShooting s_playerShooting;

    private AudioManager audioManager;
    public Sound pullBow;
    public Sound shootArrow;
    public Sound music;

    public GameObject[] DevArrowSelection;

    private void Awake()
    {
        audioManager = AudioManager.Instance();

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Multiple Players found. Destroying extra players");
        }
        else
        {
            _instance = this;
        }

        s_playerMovement = GetComponentInChildren<PlayerMovement>();
        if (!s_playerMovement)
            Debug.LogError("PlayerMovement component missing");

        s_looking = GetComponentInChildren<Looking>();
        if (!s_looking)
            Debug.LogError("Looking component missing");

        s_playerShooting = GetComponentInChildren<PlayerShooting>();
        if (!s_playerShooting)
            Debug.LogError("PlayerShooting component missing");
    }

    void Start()
    {
        audioManager.PlaySound(music);
    }

    void Update()
    {
        DevArrows();

        if (Input.GetMouseButtonDown(0))
            PrimaryActionDown();

        if (Input.GetMouseButton(0))
            PrimaryActionHold();

        if (Input.GetMouseButtonUp(0))
            PrimaryActionUp();
    }

    void PrimaryActionDown()
    {
        audioManager.PlaySound(pullBow);
    }

    void PrimaryActionHold()
    {

    }

    void PrimaryActionUp()
    {
        audioManager.StopSound(pullBow);
        audioManager.PlaySound(shootArrow);
    }

    void DevArrows()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
            s_playerShooting.arrowPrefab = DevArrowSelection[0];
        if(Input.GetKeyDown(KeyCode.Alpha1))
            s_playerShooting.arrowPrefab = DevArrowSelection[1];
        if(Input.GetKeyDown(KeyCode.Alpha2))
            s_playerShooting.arrowPrefab = DevArrowSelection[2];
        if(Input.GetKeyDown(KeyCode.Alpha3))
            s_playerShooting.arrowPrefab = DevArrowSelection[3];
        if(Input.GetKeyDown(KeyCode.Alpha4))
            s_playerShooting.arrowPrefab = DevArrowSelection[4];
        if(Input.GetKeyDown(KeyCode.Alpha5))
            s_playerShooting.arrowPrefab = DevArrowSelection[5];
        if(Input.GetKeyDown(KeyCode.Alpha6))
            s_playerShooting.arrowPrefab = DevArrowSelection[6];
        if(Input.GetKeyDown(KeyCode.Alpha7))
            s_playerShooting.arrowPrefab = DevArrowSelection[7];
        if(Input.GetKeyDown(KeyCode.Alpha8))
            s_playerShooting.arrowPrefab = DevArrowSelection[8];
        if(Input.GetKeyDown(KeyCode.Alpha9))
            s_playerShooting.arrowPrefab = DevArrowSelection[9];
            
    }
}
