using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionScript : MonoBehaviour
{
    private playerScript _PlayerScript;
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _PlayerScript = GameObject.Find("Player").GetComponent<playerScript>();
        if (_PlayerScript == null)
        {
            Debug.LogError("Player Script is NULL! (This error is in the Explosion Script!)");
        }
        _PlayerScript.playExplosionSFX(_audioSource, this.gameObject);
    }

}
