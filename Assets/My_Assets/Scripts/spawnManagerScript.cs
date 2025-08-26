//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//public class spawnManagerScript : MonoBehaviour
//{
//    [Header("Start Spawn Settings")]
//    [SerializeField] private float _startSpawnDelay = 2f;

//    [Header("Enemy Prefab Settings")]
//    [SerializeField] private GameObject _enemyPrefab;
//    [SerializeField] private GameObject _enemyContainer;

//    [Header("Power Up Spawn Settings")]
//    [SerializeField] private float _spawntTimeMin = 3f;
//    [SerializeField] private float _spawntTimeMax = 7f;
//    [SerializeField] private GameObject[] _powerUpPrefab;
//    [SerializeField] private GameObject _powerUpContainer;

//    [Header("Enemy Spawn Settings")]
//    [SerializeField] private float _enemySpawnWait = 5f;
//    private bool _stopSpawning = false;

//    public void startSpawning()
//    {
//        StartCoroutine(spawnEnemyRoutine());
//        StartCoroutine(spawnPowerupRoutine());
//    }

//    IEnumerator spawnEnemyRoutine()
//    {
//        yield return new WaitForSeconds(_startSpawnDelay);
//        while (!_stopSpawning)
//        {
//            GameObject newEnemy = Instantiate(_enemyPrefab, randomSpawnPosition(), Quaternion.identity);
//            newEnemy.transform.parent = _enemyContainer.transform;
//            yield return new WaitForSeconds(_enemySpawnWait);
//        }
//    }
//    IEnumerator spawnPowerupRoutine()
//    {
//        yield return new WaitForSeconds(_startSpawnDelay);
//        while (!_stopSpawning)
//        {
//            float randomSpawnTime = Random.Range(_spawntTimeMin, _spawntTimeMax);
//            int randPowerUp = Random.Range(0, 3);
//            yield return new WaitForSeconds(randomSpawnTime);

//            if (_stopSpawning)
//            {
//                yield break;
//            }

//            GameObject newPowerup = Instantiate(_powerUpPrefab[randPowerUp], randomSpawnPosition(), Quaternion.identity);
//            Debug.Log(_powerUpPrefab[randPowerUp].name + " has spawned!");
//            newPowerup.transform.parent = _powerUpContainer.transform;
//        }
//    }

//    public void onPlayerDeath()
//    {
//        _stopSpawning = true;
//        Debug.Log("Enemy Stopped Spawning.");
//    }
//    private Vector3 randomSpawnPosition()
//    {
//        float xRand = Random.Range(-8f, 8f);
//        return new Vector3(xRand, 7.6f, 0);
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManagerScript : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private float _timeBetweenWaves = 5f;
    [SerializeField] private List<Wave> _waves;  // List to store different waves.

    [Header("Enemy Prefab Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    [Header("Power Up Spawn Settings")]
    [SerializeField] private float _spawnTimeMin = 3f;
    [SerializeField] private float _spawnTimeMax = 7f;
    [SerializeField] private GameObject[] _powerUpPrefabs;
    [SerializeField] private GameObject _powerUpContainer;

    private bool _stopSpawning = false;
    private int _currentWaveIndex = 0;

    public void startSpawning()
    {
        StartCoroutine(SpawnWavesRoutine());
    }

    IEnumerator SpawnWavesRoutine()
    {
        yield return new WaitForSeconds(_timeBetweenWaves); // Initial delay before the first wave starts

        while (!_stopSpawning && _currentWaveIndex < _waves.Count)
        {
            Wave currentWave = _waves[_currentWaveIndex];
            Debug.Log($"Starting Wave {_currentWaveIndex + 1} with {currentWave.enemyCount} enemies and {currentWave.powerUpCount} power-ups.");

            // Start spawning enemies and power-ups for the current wave
            StartCoroutine(SpawnPowerupRoutine(currentWave));
            yield return StartCoroutine(SpawnEnemyRoutine(currentWave));

            // Once enemies are done, stop spawning powerups
            StopCoroutine(SpawnPowerupRoutine(currentWave));

            // Wait between waves
            _currentWaveIndex++;
            if (_currentWaveIndex < _waves.Count)
            {
                yield return new WaitForSeconds(_timeBetweenWaves);
            }
        }

        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnEnemyRoutine(Wave wave)
    {
        yield return new WaitForSeconds(wave.startSpawnDelay);
        for (int i = 0; i < wave.enemyCount; i++)
        {
            if (_stopSpawning)
                yield break;

            GameObject newEnemy = Instantiate(_enemyPrefab, RandomSpawnPosition(), Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(wave.enemySpawnInterval);
        }
    }

    IEnumerator SpawnPowerupRoutine(Wave wave)
    {
        for (int i = 0; i < wave.powerUpCount; i++)
        {
            if (_stopSpawning)
                yield break;

            float randomSpawnTime = Random.Range(_spawnTimeMin, _spawnTimeMax);
            yield return new WaitForSeconds(randomSpawnTime);

            int randomIndex = Random.Range(0, _powerUpPrefabs.Length);
            GameObject newPowerUp = Instantiate(_powerUpPrefabs[randomIndex], RandomSpawnPosition(), Quaternion.identity);
            newPowerUp.transform.parent = _powerUpContainer.transform;
            Debug.Log(_powerUpPrefabs[randomIndex].name + " power-up has spawned!");
        }
    }

    public void onPlayerDeath()
    {
        _stopSpawning = true;
        Debug.Log("Spawning stopped due to player death.");
    }

    private Vector3 RandomSpawnPosition()
    {
        float xRand = Random.Range(-8f, 8f);
        return new Vector3(xRand, 7.6f, 0);
    }

    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 5;             // Number of enemies in this wave
        public float startSpawnDelay = 2f;     // Delay before starting to spawn enemies in this wave
        public float enemySpawnInterval = 1f;  // Time between each enemy spawn
        public int powerUpCount = 3;           // Number of power-ups to spawn in this wave
    }
}

