using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public struct Wave
    {
        public string name;
        public Transform[] enemys;
        public int count;
        public float rate;
    }

    public enum SpawnState
    {
        Spawning,
        Waiting,
        Selecting,
        Counting
    };


    public Wave[] waves;
    public Transform[] spawnPoints;


    private int nextwave = 0;
    public static SpawnState currentSpawnState = SpawnState.Counting;


    public float timeBetwenWaves = 5f;
    private float waveCountdown;

    private float searchCountdown = 1f;

    private void Start()
    {
        waveCountdown = timeBetwenWaves;
    }

    private void Update()
    {
        print("State: " + currentSpawnState);
        if (currentSpawnState == SpawnState.Waiting)
        {
            if (!EnemyIsAlive())
            {
                StartCoroutine(WaveCompleted());
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0)
        {
            if (currentSpawnState != SpawnState.Spawning)
            {
                StartCoroutine(SpawnWave(waves[nextwave]));
            }
        }
        else
        {
            if (currentSpawnState == SpawnState.Counting)
            {
                waveCountdown -= Time.deltaTime;
            }
        }
    }

    private IEnumerator WaveCompleted()
    {
        currentSpawnState = SpawnState.Selecting;

        if (nextwave + 1 > waves.Length - 1)
        {
            nextwave = 0;
        }
        else
        {
            nextwave++;
        }

        waveCountdown = timeBetwenWaves;
        GameManager.onselectStart?.Invoke();
        yield return new WaitUntil(() => currentSpawnState == SpawnState.Counting);
        GameManager.onselectEnd?.Invoke();
    }

    IEnumerator SpawnWave(Wave newWave)
    {
        currentSpawnState = SpawnState.Spawning;

        for (int i = 0; i < newWave.count; i++)
        {
            SpawnEnemy(GetRandomEnenmy(newWave));
            yield return new WaitForSeconds(newWave.rate);
        }

        currentSpawnState = SpawnState.Waiting;

        yield break;

    }
    public Transform GetRandomEnenmy(Wave newWave)
    {
        return newWave.enemys[Random.Range(0, newWave.enemys.Length)];
    }

    void SpawnEnemy(Transform newEnemy)
    {
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(newEnemy, sp.position, sp.rotation);
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0)
        {
            searchCountdown = 1f;
            if (FindObjectOfType<Enemy>() == null)
            {
                return false;
            }
        }
        return true;
    }

}

