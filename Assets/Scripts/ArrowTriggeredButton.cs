using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTriggeredButton : MonoBehaviour{

   [SerializeField] GameObject triggeredObject;


    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("arrow")){
            //Modify the triggered object's state depending on the mechanics used
            triggeredObject.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    private void OnTriggerExit(Collider other){
        if (other.CompareTag("arrow")){
            //Modify the triggered object's state depending on the mechanics used
            triggeredObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}

//May need to revisit script depending on how arrow recall functionally works
