using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LorePickup : Interactable
{
    [TextArea(5, 20)]
    public string text;
    protected override void OnInteract(Transform userTransform)
    {
        print("yppp");
        PlayerManager.instance.DisplayLore(text);
    }

}
