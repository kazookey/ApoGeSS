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
    [Header("Spawn Range")]
    public float minSpawnY = -5f;
    public float maxSpawnY = 5f;
    [Header("Wave Settings")]
    public float spawnDelay = 1f; 
    
    private Coroutine spawnCoroutine;
   
    void Awake()
    {
        Instance = this;
    }

    public void BeginWaves()
    {
        spawnCoroutine = StartCoroutine(ContinuousSpawn());
    }
    IEnumerator ContinuousSpawn()
    {
        while (true)
        {
            Transform lane = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Vector3 spawnPosition = new Vector3(lane.position.x, Random.Range(minSpawnY, maxSpawnY), lane.position.z);
            Instantiate(GetWeightedEnemy(), spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }
       public static void EnemyKilled()
    {
       //for scoring/results
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
