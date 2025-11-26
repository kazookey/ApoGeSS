using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public GameObject enemyPrefab;
    public Transform spawnPoint;

    public int startingEnemies = 3;
    public float spawnDelay = 1f;
    public float waveDelay = 2f;

    int currentWave = 0;
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
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
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
}
