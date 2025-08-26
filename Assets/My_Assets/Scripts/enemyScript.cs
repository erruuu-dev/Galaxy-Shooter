using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyScript : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float _enemySpeed = 4f;
    [SerializeField] private float _enemyDestroyedSpeed = 0.2f;
    [SerializeField] private float _enemyDestroyDelay = 2.30f;
    [SerializeField] private bool _isEnemyDestroyed = false;

    [Header("Enemy Laser Settings")]
    [SerializeField] private GameObject _enemyLaserPrefab;
    [SerializeField] private GameObject _enemyLaserContainer;
    [SerializeField] private float _enemyLaserFireRate = 3f;
    [SerializeField] private float _enemyCanFire = -1f;

    [Header("Enemy Components Settings")]
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private AudioSource _audioSource;

    //[SerializeField] private GameObject _laserTrailFX;
    private playerScript _PlayerScript;


    void Start()
    {
        _enemyLaserContainer = GameObject.FindGameObjectWithTag("Manager").transform.GetChild(2).gameObject;
        _PlayerScript = GameObject.Find("Player").GetComponent<playerScript>();
       if (_PlayerScript == null )
        {
            Debug.Log("Player Script not found! This code is in enemyScript");
        }
        _enemyAnimator = GetComponent<Animator>();

    }
    void Update()
    {
        enemyMove();
        respawnEnemy();
        disableBoxCollider();
        enemyFireLaser();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {           
        if (collision.tag == "Player")
        {
            _PlayerScript.playExplosionSFX(_audioSource, this.gameObject);
            _PlayerScript.damage();
            enemyCollisionBehaviour(this.gameObject, _enemyDestroyDelay);
        }
        if (collision.tag == "Projectile")
        {
            _PlayerScript.playExplosionSFX(_audioSource, this.gameObject);
            enemyCollisionBehaviour(this.gameObject, _enemyDestroyDelay);
        }
    }
    private void enemyMove()
    {
       transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
    }
    void respawnEnemy()
    {
        
        if (transform.position.y <= -5.60f)
        {
            float xRand = Random.Range(-8f, 8f);

            transform.position = new Vector3(xRand, 7.6f, 0);
        }
    }
    void disableBoxCollider()
    {
        if (_isEnemyDestroyed)
        {
           GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    void enemyCollisionBehaviour(GameObject destroyObj, float destroyTime)
    {
        _isEnemyDestroyed = true;
        _enemyAnimator.SetTrigger("EnemyDeath");
        _enemySpeed = _enemyDestroyedSpeed;
        _PlayerScript.addScore(10);
        Destroy(destroyObj, destroyTime);
    }

    void enemyFireLaser()
    {
        if (Time.time > _enemyCanFire && !_isEnemyDestroyed)
        {
            _enemyLaserFireRate = Random.Range(1f, 3f);
            _enemyCanFire = Time.time + _enemyLaserFireRate;
            Transform enemyFirePoint = transform.GetChild(0);
            GameObject enemyLaser = Instantiate(_enemyLaserPrefab, enemyFirePoint.position, Quaternion.identity, _enemyLaserContainer.transform);
            laserScript[] eLaser = enemyLaser.GetComponents<laserScript>();

            for(int i = 0; i < eLaser.Length; i++)
            {
                eLaser[i].setEnemyLaser();
            }
        }
    }


}
