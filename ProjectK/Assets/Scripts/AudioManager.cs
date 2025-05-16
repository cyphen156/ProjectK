using System.Collections.Generic;
using System;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip fireClip;

    private void Start()
    {
        Gun.OnFire += FireSound;
    }

    private void FireSound(Vector3 inFirePosition)
    {
        AudioSource.PlayClipAtPoint(fireClip, inFirePosition);
    }

}
