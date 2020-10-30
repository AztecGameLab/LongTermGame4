using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArrow : StickyArrowScript
{
    PlayerManager player = PlayerManager.instance;
    bool arrowUsed;
    RaycastHit hitUp, hitDown, hitLeft, hitRight;
    KeyCode teleportKeybind = KeyCode.T; //This could be changed in the future if we allow rebinding of keys. 


    private void Update()
    {
        //order of these two ifs matter
        //Destroying the object in the same frame update sometimes causes the teleport to fail.
        if (arrowUsed)
            Destroy(gameObject);

        if (Input.GetKeyDown(teleportKeybind))
            TeleportPlayer();
    }
    public void TeleportPlayer()
    {
        arrowUsed = true;
        Physics.Raycast(transform.position, Vector3.up, out hitUp);
        Physics.Raycast(transform.position, Vector3.down, out hitDown);
        Physics.Raycast(transform.position, Vector3.right, out hitRight);
        Physics.Raycast(transform.position, Vector3.left, out hitLeft);

        //will not teleport player if there is no ground beneath them, to the left and right
        if (hitDown.collider != null &&
           (hitLeft.distance + hitRight.distance > 1.1 || (hitLeft.collider == null && hitRight.collider == null) || hitLeft.distance > 1.1f || hitRight.distance > 1.1f) )
        {
            //check if there is enough vertical distance for player to exist in the space. And limiting the possibility to boundry break a level
            if ((hitUp.distance > 1 && hitDown.distance + hitUp.distance > 2f) || hitUp.collider == null)
                player.transform.position = transform.position;

        }
    }
}
