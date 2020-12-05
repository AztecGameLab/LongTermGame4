using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSound : MonoBehaviour
{
    [SerializeField] private Sound sound = default;
    public void PlaySound()
    {
        AudioManager.Instance().PlaySound(sound.GenerateInstance(), gameObject);
    }
}
