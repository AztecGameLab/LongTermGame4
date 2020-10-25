using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArrow : StickyArrowScript
{
    PlayerManager player = PlayerManager.instance;
    bool arrowUsed;
    RaycastHit hitUp, hitLeft, hitRight;

   
    private void Update()
    {
        //order of these two ifs matter
        //Destroying the object in the same frame update causes the teleport to fail.
        if (arrowUsed)
            Destroy(gameObject);
        if (Input.GetKeyDown("t"))
        {
            arrowUsed = true;
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hitUp);
            if (hitUp.distance > 1.1f || hitUp.collider == null)
                player.transform.position = transform.position;
        }
            
    }

   
}
