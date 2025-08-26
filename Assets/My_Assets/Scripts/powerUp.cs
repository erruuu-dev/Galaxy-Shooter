using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUp : MonoBehaviour
{
    [Header("Power Up Settings")]
    [SerializeField] private float _powerUpSpeed = 3f;
    [SerializeField] private int _powerUpID;

    void Update()
    {
        powerUpBehavior();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerScript PlayerScript = collision.GetComponent<playerScript>(); 
            if (PlayerScript != null)
            {
                switch (_powerUpID)
                {
                    case 0:
                        PlayerScript.activatePowerup(() => PlayerScript.onPowerUpTrue(0), PlayerScript.TripleShotPowerDownRouting(), ref PlayerScript._tripleShotCoroutine);
                        break;
                    case 1:
                        PlayerScript.activatePowerup(() => PlayerScript.onPowerUpTrue(1), PlayerScript.SpeedBoostPowerDownRouting(), ref PlayerScript._speedBoostCoroutine);
                        break;
                    case 2:
                        PlayerScript.shieldActive();
                        break;
                    default:
                        Debug.Log("Default!");
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }

    private void powerUpBehavior()
    {
        transform.Translate(Vector3.down * _powerUpSpeed * Time.deltaTime);

        if (transform.position.y <= -5.60f)
        {
            Destroy(this.gameObject);
            Debug.Log(this.name + " is detroyed. Is outside the screen.");
        }
    }
}
