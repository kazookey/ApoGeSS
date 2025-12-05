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
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;
    public int minEnemiesPerWave = 3;
    public int maxEnemiesPerWave = 8;
    public float timeBetweenWaves = 5f; // delay between waves


    
    private Coroutine spawnCoroutine;
   
    // --- GIZMOS ---
    void OnDrawGizmos()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return;

        Gizmos.color = Color.cyan;
        foreach (var lane in spawnPoints)
        {
            if (lane == null) continue;

            // Draw vertical line showing min-max Y spawn range
            Vector3 bottom = new Vector3(lane.position.x, minSpawnY, lane.position.z);
            Vector3 top = new Vector3(lane.position.x, maxSpawnY, lane.position.z);
            Gizmos.DrawLine(bottom, top);

            // Draw spheres at min and max Y
            Gizmos.DrawSphere(bottom, 0.3f);
            Gizmos.DrawSphere(top, 0.3f);

            // Draw a small sphere at the lane base
            Gizmos.DrawSphere(lane.position, 0.2f);
        }
    }
    
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
            // Determine random number of enemies for this wave
            int enemiesThisWave = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);

            for (int i = 0; i < enemiesThisWave; i++)
            {
                // Pick a random lane
                Transform lane = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
                // Pick a random Y position within range
                Vector3 spawnPosition = new Vector3(lane.position.x, Random.Range(minSpawnY, maxSpawnY), lane.position.z);

                // Spawn the enemy
                Instantiate(GetWeightedEnemy(), spawnPosition, Quaternion.identity);

                // Wait for a random time before spawning next enemy in the wave
                float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
                yield return new WaitForSeconds(waitTime);
            }

            // Wait before starting the next wave
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }



    public void StopSpawning()
        {
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }

        public void DespawnAllEnemies()
        {
            foreach (var enemy in FindObjectsOfType<Enemy>())
            {
                Destroy(enemy.gameObject);
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
