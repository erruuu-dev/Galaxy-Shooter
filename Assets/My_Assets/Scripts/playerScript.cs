using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class playerScript : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _score;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _fireRate = 0.25f;
    [SerializeField] private float _destroyDelay = 3.5f;
    [SerializeField] private GameObject _explosionFX;


    [Header("Laser Settings")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _laserPoint;
    [SerializeField] private GameObject _muzzleFireFX;
    [SerializeField] private float _muzzleOffset = 1f;
    private float _canFire;

    [Header("Triple Shot Settings")]
    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleLaserTime = 5f;
    [SerializeField] private float _tripleShotYOffset = 1f;
    [SerializeField] private GameObject _TripleMuzzleFireFX;
    [SerializeField] private GameObject _tripleLaserPrefab;

    [Header("Speed Boost Settings")]
    private bool _isSpeedBoostActive = false;
    private float _originalMoveSpeed;
    private float _boostedMoveSpeed;
    [SerializeField] private int _speedBoostMultiplier = 2;
    [SerializeField] private float _speedBoostTime = 5f;

    [Header("Shield Settings")]
    public bool isShieldActive = false;
    [SerializeField] private GameObject _shieldVisual;
    [SerializeField] private int _shieldStrength = 0;
    [SerializeField] private int _maxShieldStrength = 3;    
    
    [Header("Spawn Manager Settings")]
    [SerializeField] private spawnManagerScript _spawnManager;
    [SerializeField] private GameObject _laserContainer;

    [Header("Game Manager Settings")]
    [SerializeField] private gameManagerScript _gameManager;

    [Header("Audio SFX Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _laserSFX;
    [SerializeField] private AudioClip _explosionSFX;
    [SerializeField] private AudioClip _collectPowerUpsSFX;

    public Coroutine _tripleShotCoroutine;
    public Coroutine _speedBoostCoroutine;
    public Transform trailTransRef;
    private BoxCollider2D _playerBoxCollider;
    private CircleCollider2D _playerCircleCollider;

    void Awake()
    {
        getReference();
        _audioSource = GetComponent<AudioSource>();
        _playerBoxCollider = GetComponent<BoxCollider2D>();
        _playerCircleCollider = GetComponent<CircleCollider2D>();
        _originalMoveSpeed = _moveSpeed;
        _boostedMoveSpeed = _moveSpeed * _speedBoostMultiplier;
    }
    void Start()
    {
        setPos();
        _playerBoxCollider.enabled = true;
        _playerCircleCollider.enabled = false;
    }
    void Update()
    {
        movement();
        playerBound();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            attack();
        }


        if (Input.GetKeyDown(KeyCode.Q))
        {
            _isTripleShotActive = true;
        }        
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isTripleShotActive = false;
        }
    }
    void getReference()
    {
        if (_laserPrefab != null)
        {
            _laserPoint = GameObject.Find("Laser_Point");
        }
        else
        {
            Debug.LogError("Laser Point is Null!");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<spawnManagerScript>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is Null!");
        }

        _gameManager = GameObject.Find("Game_Manager").GetComponent<gameManagerScript>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager is Null!");
        }
    }
    private void setPos()
    {
        transform.position = new Vector3(0, -2.5f, 0);
    }
    private void movement()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical"); 

        if (_isSpeedBoostActive)
        {
            _moveSpeed = _boostedMoveSpeed;
        }
        else
        {
            _moveSpeed = _originalMoveSpeed;
        }

        Vector3 newMovement = new Vector3(xMove, yMove,0) * _moveSpeed * Time.deltaTime;
        transform.Translate(newMovement);
    }
    private void playerBound()
    {
        Vector3 pos = transform.position;
        //Up and Down Bounds
        pos.y = Mathf.Clamp(pos.y, -3.88f, 0f);
        transform.position = pos; 

        //Side Bounds
        if (transform.position.x < -11.10f)
        {
            transform.position = new Vector3(11.10f,pos.y,0);
        }
        else if(transform.position.x > 11.10f)
        {
            transform.position = new Vector3(-11.10f,pos.y,0);
        }
    }
    private void attack()
    {
        _canFire = Time.time + _fireRate;
        GameObject laserRef;
        if (_isTripleShotActive) 
        {
            Vector3 tripleShotYOffset = new Vector3(0, _tripleShotYOffset, 0);
            laserRef = Instantiate(_tripleLaserPrefab, _laserPoint.transform.position - tripleShotYOffset, Quaternion.identity);
            GameObject muzzleFX = Instantiate(_TripleMuzzleFireFX, _laserPoint.transform.position, Quaternion.identity, _laserPoint.transform);
            Destroy(muzzleFX, 0.9f);
            //laserRef.GetComponent<ParticleSystem>().startDelay = ;
            //laserRef = Instantiate(_tripleLaserPrefab, _laserPoint.transform.position, Quaternion.identity);
        }
        else
        {
            laserRef = Instantiate(_laserPrefab, _laserPoint.transform.position, Quaternion.identity);
            Vector3 muzzleFXOffset = new Vector3(0, _muzzleOffset, 0);
            GameObject muzzleFX = Instantiate(_muzzleFireFX, _laserPoint.transform.position - muzzleFXOffset, Quaternion.Euler(new Vector3(-90f, 0, 0)), _laserPoint.transform);
            Destroy(muzzleFX, 0.9f);
        }

        if (laserRef != null && _laserContainer != null)
        {
            laserRef.transform.parent = _laserContainer.transform;
        }
        trailTransRef = laserRef.transform;

        _audioSource.clip = _laserSFX;
        _audioSource.Play();
    }
    public void damage()
    {
        if (isShieldActive)
        {
            _shieldStrength -= 1; 
            Debug.Log("Shield took damage! Remaining shield strength: " + _shieldStrength);
            if (_shieldStrength == 0)
            {
                isShieldActive = false;
                _playerCircleCollider.enabled = false;
                _playerBoxCollider.enabled = true;
                _shieldVisual.SetActive(false);
                Debug.Log("Shield is broken!");
            }
            return;
        }
        _lives -= 1;
        damageVisuals();

    }
    void damageVisuals()
    {
        GameObject _rightEngineDamage = transform.GetChild(3).gameObject;
        GameObject _leftEngineDamage = transform.GetChild(4).gameObject;

        if (_lives == 1)
        {
            _rightEngineDamage.SetActive(true);
        }
        else if (_lives == 0)
        {
            _leftEngineDamage.SetActive(true);
        }

        if (_lives <= -1)
        {
            _spawnManager.onPlayerDeath();
            _gameManager.setGameOver();
            Debug.Log("Player is Dead! Game Over!");
            _audioSource.clip = _explosionSFX;
            _audioSource.Play();
            GameObject explosionObj = Instantiate(_explosionFX, transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
            Destroy(this.gameObject, _destroyDelay);
        }

    }
    public void activatePowerup(Action powerup, IEnumerator routing, ref Coroutine coroutine)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        powerup();
        coroutine = StartCoroutine(routing);
    }    
    public IEnumerator TripleShotPowerDownRouting()
    {
        yield return new WaitForSeconds(_tripleLaserTime);
        _isTripleShotActive = false;
        Debug.Log("Triple Shot Ran out!");
    }
    public IEnumerator SpeedBoostPowerDownRouting()
    {
        yield return new WaitForSeconds(_speedBoostTime);
        _isSpeedBoostActive = false;
        Debug.Log("Speed Boost Ran out!");
    }
    public void onPowerUpTrue(int x)
    {
        switch (x)
        {
            case 0:
                _isTripleShotActive = true;
                Debug.Log("Triple Shot Activated!");
                break;
            case 1:
                _isSpeedBoostActive = true;
                Debug.Log("Speed Boost Activated!");
                break;
            default:
                Debug.Log("No Power Up Returned");
                break;
        }
    }
    public void shieldActive()
    {
        isShieldActive = true;
        _shieldVisual.SetActive(true);

        //Change collider when shield is active
        _playerBoxCollider.enabled = false;
        _playerCircleCollider.enabled = true;

        if (_shieldStrength == _maxShieldStrength)
        {
            Debug.Log("Max Shield Strength Acquired!");
            return;
        }
        _shieldStrength += 1;
    }
    public void addScore(int points)
    {
        _score += points;
    }
    public int showScore()
    {
        int scoreTemp = _score;
        return scoreTemp;
    }
    public int showLives()
    {
        int livesTemp = _lives;
        return livesTemp;
    }
    //private void playMuzzleFlash(GameObject fx, float offset)
    //{
    //    Vector3 muzzleFXOffset = new Vector3(0, offset, 0);
    //    GameObject muzzleFX = Instantiate(fx, _laserPoint.transform.position - muzzleFXOffset, Quaternion.Euler(new Vector3(-90f, 0, 0)), _laserPoint.transform);
    //    //GameObject muzzleFX = Instantiate(_muzzleFireFX, _laserPoint.transform.position, Quaternion.Euler(new Vector3(-90f, 0, 0)), _laserPoint.transform);
    //    Destroy(muzzleFX, 0.9f);
    //}
    public void playExplosionSFX(AudioSource audioSource, GameObject gameObj)
    {
        audioSource = gameObj.AddComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.PlayOneShot(_explosionSFX);
        }
        else
        {
            Debug.LogError("Audio Source is NULL! (This error is in the Player Script!)");
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == ("Powerups"))
        {
           _audioSource.PlayOneShot(_collectPowerUpsSFX);
        }        
        if (collision.tag == ("EnemyLaser"))
        {
            damage();
            Destroy(collision.gameObject);
        }
    }
}

