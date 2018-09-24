using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnEnemyScript : MonoBehaviour {
    private Wave WaveToSpawn = null;
    private bool NewRequest = false;
    private uint NumberOfEnemiesSpawned;
    //private Timer timer = null;
   

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (NewRequest)
            StartSpawning();
    }

    public void RequestSpawnWave()
    {
        Assert.IsNull(WaveToSpawn, "SpawnEnemyScript.RequestSpawnWave Spawning is not finished!!");
        NewRequest = true;
        WaveToSpawn = GameState.CurrentWave;
    }

    IEnumerator SpawnEnemy()
    {
        while (NumberOfEnemiesSpawned < WaveToSpawn.NumberOfEnemies)
        {
            WaveToSpawn.AddEnemy(Instantiate(WaveToSpawn.EnemyType, transform.position, transform.rotation));
            ++NumberOfEnemiesSpawned;

            yield return new WaitForSeconds(0.5f);
        }
        FinishSpawning();
        yield return null;
    }

    private void StartSpawning()
    {
        NumberOfEnemiesSpawned = 0;
        StartCoroutine(SpawnEnemy());
        NewRequest = false;
    }


    private void FinishSpawning()
    {
        WaveToSpawn = null;
    }
}
