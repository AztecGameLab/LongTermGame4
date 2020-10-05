using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTriggeredButton : MonoBehaviour{

   [SerializeField] GameObject triggeredObject;
   bool isTriggered = false;

    private void OnTriggerEnter(Collider other){

        if (other.CompareTag("arrow")){
            //Modify the triggered object's state depending on the mechanics used
            //Example used here is changing the material on the indicator block
            triggeredObject.GetComponent<Renderer>().material.color = Color.green;
        }

    }

}
