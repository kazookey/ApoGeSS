using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyEntry
{
    public GameObject prefab;
    public int weight = 1;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Enemy Types")]
    public List<EnemyEntry> enemyPool = new List<EnemyEntry>();

    [Header("Spawn Lanes")]
    public Transform[] spawnPoints;  
    [Header("Wave Settings")]
    public int startingEnemies = 3;
    public float spawnDelay = 1f;
    public float waveDelay = 2f;

    private int currentWave = 0;
    public static int aliveEnemies = 0;

    void Awake()
    {
        Instance = this;
    }

    public void BeginWaves()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        while (true)
        {
            currentWave++;
            int enemiesThisWave = startingEnemies + (currentWave - 1);

            Debug.Log("Starting Wave " + currentWave);
            aliveEnemies = enemiesThisWave;

            
            for (int i = 0; i < enemiesThisWave; i++)
            {
                Transform lane = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(GetWeightedEnemy(), lane.position, Quaternion.identity);

                yield return new WaitForSeconds(spawnDelay);
            }

            while (aliveEnemies > 0)
                yield return null;

            Debug.Log("Wave " + currentWave + " cleared!");

            
            yield return new WaitForSeconds(waveDelay);
        }
    }

    public static void EnemyKilled()
    {
        aliveEnemies--;
    }

    GameObject GetWeightedEnemy()
    {
        int totalWeight = 0;

        foreach (var e in enemyPool)
            totalWeight += e.weight;

        int roll = Random.Range(0, totalWeight);
        int sum = 0;

        foreach (var e in enemyPool)
        {
            sum += e.weight;
            if (roll < sum)
                return e.prefab;
        }

        return enemyPool[0].prefab;
    }
}
