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

    private void Awake()
    {
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
        if(!s_playerMovement)
            Debug.LogError("PlayerMovement component missing");

        s_looking = GetComponentInChildren<Looking>();
        if(!s_looking)
            Debug.LogError("Looking component missing");

        s_playerShooting = GetComponentInChildren<PlayerShooting>();
        if(!s_playerShooting)
            Debug.LogError("PlayerShooting component missing");
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
