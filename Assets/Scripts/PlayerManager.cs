using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [HideInInspector]
    public PlayerInteract s_playerInteract;

    private AudioManager audioManager;
    public Sound pullBow;
    public Sound shootArrow;
    public Sound music;

    private SoundInstance pullBowInstance;
    private SoundInstance shootArrowInstance;
    private SoundInstance musicInstance;

    Animator bowAnimator;

    public GameObject[] DevArrowSelection;
    public GameObject loreDisplay;

    private static string arrowTypeMessage;
    private bool regArrow = true;
    private bool grappleArrow = false;
    private bool gravArrow = false;

    public TMP_Text currentArrowName;
    public bool stopShooting;
    public void EnableShooting() => stopShooting = false;


    private void Awake()
    {
        audioManager = AudioManager.Instance();
        pullBowInstance = pullBow.GenerateInstance();
        shootArrowInstance = shootArrow.GenerateInstance();
        musicInstance = music.GenerateInstance();

        bowAnimator = GetComponentInChildren<Animator>();

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

        s_playerInteract = GetComponentInChildren<PlayerInteract>();
        if (!s_playerInteract)
            Debug.LogError("PlayerInteract component missing");
    }

    void Start()
    {
        setRegArrow();
        audioManager.PlaySound(musicInstance);
    }

    void Update()
    {
        DevArrows();
        GetInput();


    }

    void OnGUI()
    {
        // if (regArrow)
        //     arrowTypeMessage = GUI.TextField(new Rect((Screen.width / 2 - 40), (Screen.height - 22), 88, 22), "Regular Arrow");
        // else if (grappleArrow)
        //     arrowTypeMessage = GUI.TextField(new Rect((Screen.width / 2 - 40), (Screen.height - 22), 88, 22), "Grapple Arrow");
        // else
        //     arrowTypeMessage = GUI.TextField(new Rect((Screen.width / 2 - 40), (Screen.height - 22), 88, 22), "Gravity Arrow");
    }

    void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!loreDisplay.activeSelf)
                s_playerInteract.TryInteract();
            else
                HideLore();
        }

        if (loreDisplay.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0) && !stopShooting)
        {
            if (!bowAnimator)
                bowAnimator = GetComponentInChildren<Animator>();

            audioManager.PlaySound(pullBowInstance);
            bowAnimator.SetTrigger("Draw");
        }


        if (Input.GetMouseButtonUp(0) && !stopShooting)
        {
            if (!bowAnimator)
                bowAnimator = GetComponentInChildren<Animator>();

            audioManager.StopSound(pullBowInstance);
            audioManager.PlaySound(shootArrowInstance);
            bowAnimator.SetTrigger("Shoot");
        }
    }


    public void DisplayLore(string text, Sprite background = null)
    {
        loreDisplay.SetActive(true);
        loreDisplay.GetComponentInChildren<TextMeshProUGUI>().SetText(text);
        if (background)
            loreDisplay.GetComponent<Image>().sprite = background;

        s_playerMovement.enabled = false;
        s_looking.enabled = false;
        s_playerShooting.enabled = false;
    }

    public void HideLore()
    {
        loreDisplay.SetActive(false);

        s_playerMovement.enabled = true;
        s_looking.enabled = true;
        s_playerShooting.enabled = true;
    }


    void setRegArrow()
    {
        currentArrowName.text = "Regular Arrow";
        currentArrowName.color = new Color(0.77f, 0.34f, 0.53f);
        regArrow = true;
        grappleArrow = false;
        gravArrow = false;
    }
    void setGrappleArrow()
    {
        currentArrowName.text = "Grapple Arrow";
        currentArrowName.color = new Color(0.93f, 0.67f, 0.34f);
        regArrow = false;
        grappleArrow = true;
        gravArrow = false;
    }
    void setGravArrow()
    {
        currentArrowName.text = "Anti-Grav Arrow";
        currentArrowName.color = new Color(0.15f, 0.19f, 0.89f);
        regArrow = false;
        grappleArrow = false;
        gravArrow = true;
    }


    void DevArrows()
    {
        if (loreDisplay.activeSelf)
            return;

        // Right-click to get next arrow
        if (Input.GetMouseButtonDown(1))
        {
            if (s_playerShooting.ArrowPrefab == DevArrowSelection[1])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[2];
                setGrappleArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[2])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[3];
                setGravArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[3])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[1];
                setRegArrow();
            }
        }

        // ScrollWheel for next and prev
        if (Input.mouseScrollDelta.y > 0)
        {
            if (s_playerShooting.ArrowPrefab == DevArrowSelection[1])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[3];
                setGravArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[2])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[1];
                setRegArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[3])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[2];
                setGrappleArrow();
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (s_playerShooting.ArrowPrefab == DevArrowSelection[1])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[2];
                setGrappleArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[2])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[3];
                setGravArrow();
            }
            else if (s_playerShooting.ArrowPrefab == DevArrowSelection[3])
            {
                s_playerShooting.ArrowPrefab = DevArrowSelection[1];
                setRegArrow();
            }
        }

        // // Num-key arrow selection
        // /*if (Input.GetKeyDown(KeyCode.Alpha0))
        //       s_playerShooting.ArrowPrefab = DevArrowSelection[0];
        // */
        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     s_playerShooting.ArrowPrefab = DevArrowSelection[1];
        //     setRegArrow();
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     s_playerShooting.ArrowPrefab = DevArrowSelection[2];
        //     setGrappleArrow();
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     s_playerShooting.ArrowPrefab = DevArrowSelection[3];
        //     setGravArrow();
        // }
        /*if (Input.GetKeyDown(KeyCode.Alpha4))
              s_playerShooting.ArrowPrefab = DevArrowSelection[4];
          if (Input.GetKeyDown(KeyCode.Alpha5))
              s_playerShooting.ArrowPrefab = DevArrowSelection[5];
          if (Input.GetKeyDown(KeyCode.Alpha6))
              s_playerShooting.ArrowPrefab = DevArrowSelection[6];
          if (Input.GetKeyDown(KeyCode.Alpha7))
              s_playerShooting.ArrowPrefab = DevArrowSelection[7];
          if (Input.GetKeyDown(KeyCode.Alpha8))
              s_playerShooting.ArrowPrefab = DevArrowSelection[8];
          if (Input.GetKeyDown(KeyCode.Alpha9))
              s_playerShooting.ArrowPrefab = DevArrowSelection[9];
        */
    }
}
