using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{

    public GameObject ArrowPrefab;
    public Image ShotPowerFill;

    public float minForce = 0f;
    public float maxForce = 50f;
    public float timeToGetToMaxForce = 2.5f;

    public float shootingStrength = 0f;

    public float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !PlayerManager.instance.stopShooting)
        {
            if (GrapplingArrow.CurrentArrow != null)
            {
                GrapplingArrow.CurrentArrow = null;
            }
        }
        else if (Input.GetMouseButton(0) && !PlayerManager.instance.stopShooting)
        {
            timer += Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, timeToGetToMaxForce);
        }
        else if (Input.GetMouseButtonUp(0) && !PlayerManager.instance.stopShooting)
        {
            GameObject arrowObject = Instantiate(ArrowPrefab) as GameObject;
            arrowObject.transform.position = transform.position + transform.forward;
            arrowObject.transform.forward = transform.forward;
            arrowObject.GetComponent<Rigidbody>().AddForce(transform.forward * shootingStrength, ForceMode.Impulse);

            timer = 0;
            shootingStrength = minForce;
        }

        ShotPowerFill.fillAmount = timer / timeToGetToMaxForce;
        shootingStrength = Mathf.Lerp(minForce, maxForce, timer / timeToGetToMaxForce);

    }
}