using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class laserScript : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float _laserSpeed = 8f;
    [SerializeField] private bool _isEnemyLaser = false;
    [SerializeField] private Transform _trailPrefab;
    [SerializeField] private GameObject _laserExplosionPrefab;
    [SerializeField] private Vector3 _explosionOffset;
    private Transform _trailInstance;
    private ParticleSystem _trailParticleSystem;


    private void Awake()
    {
        if (_trailPrefab != null)
        {
            _trailInstance = Instantiate(_trailPrefab, transform.position, Quaternion.identity);
            _trailInstance.SetParent(transform); // Set the instantiated trail as a child of this laser

            _trailParticleSystem = _trailInstance.GetComponent<ParticleSystem>();
            if (_trailParticleSystem == null )
            {
                Debug.Log("Trail Particle is NULL! Error is in the Laser Script");
            }
        }
    }
    void Update()
    {
        if (_isEnemyLaser)
        {
            laserBehavior(Vector3.down, -9.0f, "EnemyLaser");
        }
        else
        {
            laserBehavior(Vector3.up, 17.0f, "Projectile");
        }
    }

    void  laserBehavior(Vector3 laserDir, float yPosBound, string laserTag)
    {
        transform.Translate(laserDir * _laserSpeed * Time.deltaTime);
        if (_isEnemyLaser && transform.position.y <= yPosBound)
        {
            destroyLaser(laserTag);
        }

        if (!_isEnemyLaser && transform.position.y >= yPosBound)
        {
            destroyLaser(laserTag);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (_trailInstance != null)
            {
                // Detach the trail so it can complete its effect
                Transform getParent = GameObject.FindGameObjectWithTag("Manager").transform.GetChild(2);
                _trailInstance.parent = getParent;
                setROTZero();
                GameObject explosionFX = Instantiate(_laserExplosionPrefab, transform.position + _explosionOffset, Quaternion.identity, getParent);
                Destroy(_trailInstance.gameObject, 3f); // Destroy trail after a delay to let it finish playing
                Destroy(explosionFX, 3f);
            }

            Destroy(this.gameObject); // Destroy the laser
        }
    }

    void setROTZero()
    {
        var trailEmmision = _trailParticleSystem.emission;
        trailEmmision.rateOverTime = 0;
    }

    void destroyLaser(string laserTag)
    {
        if (transform.parent != null && transform.parent.CompareTag(laserTag))
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void setEnemyLaser()
    {   
        _isEnemyLaser = true;
    }
}
