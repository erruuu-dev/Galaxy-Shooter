using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class asteroidScript : MonoBehaviour
{
    [Header("Asteroid Setting")]
    [SerializeField] private int _health = 5;
    [SerializeField] private float _rotateSpeed = 2f;
    [SerializeField] private float _degrees = 90f;

    [Header("Effects Settings")]
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private float _destroyExplosionDelay = 3f;
    [SerializeField] private float _destroyAsteroidDelay = .5f;

    private spawnManagerScript _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<spawnManagerScript>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager Script is NULL! (This error is in Asteroid Script. )");
        }        
    }

    void Update()
    {
        rotateAsteroid();
    }

    void rotateAsteroid()
    {
        Vector3 rotateAngle = new Vector3(0, 0, _degrees);
        transform.Rotate(rotateAngle * _rotateSpeed * Time.deltaTime);
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == ("Projectile"))
        {
            Destroy(collision.gameObject);
            _health -= 1;
            Debug.Log("Asteroid Damaged! Remaining Life: " + _health);
            if(_health == 0)
            {
                GameObject explosionObj = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
                Destroy(this.gameObject, _destroyAsteroidDelay);
                Destroy(explosionObj, _destroyExplosionDelay);
                _spawnManager.startSpawning();
            }
        }
    }


}
