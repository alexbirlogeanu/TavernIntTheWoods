using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

[System.Serializable]
public class WaveDescription
{
    public GameObject EnemyTemplate;
    public uint NumberOfEnemiesPerWave;
}

public class Wave
{
    public GameObject EnemyType;
    public int NumberOfEnemies;
    public bool Started = false;

    public class WaveInfo
    {

    }
    //private:
    private List<GameObject> Enemies = new List<GameObject>();


    public Wave(WaveDescription desc)
    {
        EnemyType = desc.EnemyTemplate;
        NumberOfEnemies = (int)desc.NumberOfEnemiesPerWave;
        Enemies.Capacity = NumberOfEnemies;
    }

    public void AddEnemy(GameObject enemy)
    {
        Enemies.Add(enemy);
        Started = true;
    }

    public bool IsFinished()
    {
        if (!Started)
            return false;

        Enemies.RemoveAll(e => e == null);
        return Enemies.Count == 0;
    }

    public EnemyStatus GetWaveEnemyStatus()
    {
        EnemyStatusComponent statusComp = EnemyType.GetComponent<EnemyStatusComponent>();
        Assert.IsNotNull(statusComp);
        return statusComp.Status;
    }
};


public class GameState
{
    public static int EnemiesEscaped = 0;
    public static int EnemiesEscapedTreshold = 20;
    public static int Gold = 0;
    public static int CurrentWaveIndex = 0;
    public static Wave CurrentWave = null;
    public static int MaxWaves = 0;
}

public class GameLoopScript : MonoBehaviour {
    public GameObject SpawnPoint = null;
    public List<WaveDescription> EnemyWaves = new List<WaveDescription>();
    public float TimeBetweenWaves = 2f;
    public int StartingGold = 100;
   
    //UI Components
    public GameObject MainHUD = null;
    public GameObject PauseMenu = null;
    public GameObject GameOverMenu = null;
    public GameObject VictoryMenu = null;

    public GameObject TowerPlacer = null;
    public UITimer WaveTimer = null;
    private int CurrentWaveIndex = -1;
    private bool IsPaused = false;

    // Use this for initialization
    void Start ()
    {
        Assert.raiseExceptions = true;
        Assert.IsNotNull(SpawnPoint);
        Assert.IsNotNull(MainHUD, "GameLoopScript.Start: MainHUD is NULL");
        Assert.IsNotNull(PauseMenu, "GameLoopScript.Start: PauseMenu is NULL");
        Assert.IsNotNull(TowerPlacer, "GameLoopScript.Start: TowerPlacer is NULL");
        Assert.IsNotNull(GameOverMenu, "GameLoopScript.Start: GameOverMenu is NULL");
        Assert.IsNotNull(WaveTimer, "GameLoopScript.Start: WaveTimer is NULL");
        Assert.IsNotNull(VictoryMenu, "GameLoopScript.Start: VictoryMenu is NULL");

        Init();
    }

    // Update is called once per frame
    void Update ()
    {
        if (GameState.EnemiesEscaped >= GameState.EnemiesEscapedTreshold)
        {
            GameOver();
            return;
        }

        if (GameState.CurrentWaveIndex >= EnemyWaves.Count)
        {
            return;
        }

        ProcessInput();

        Wave currentWave = GameState.CurrentWave;
        if (currentWave == null || currentWave.IsFinished())
        {
            if (++CurrentWaveIndex >= EnemyWaves.Count)
            {
                Victory();
                return;
            }
            GameState.CurrentWave = new Wave(EnemyWaves[CurrentWaveIndex]);
            StartWaveTimer();
            StartCoroutine(SpawnNextWave());
            GameState.CurrentWaveIndex = CurrentWaveIndex;
        }
    }


    private void ProcessInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!IsPaused);
        }

    }

    public void Pause(bool pause)
    {
        Time.timeScale = (pause)? 0f : 1f;
        MainHUD.SetActive(!pause);
        PauseMenu.SetActive(pause);
        PlaceTowerScript placeScript =  TowerPlacer.GetComponent<PlaceTowerScript>();
        Assert.IsNotNull(placeScript, "GameLoopScript.Pause: Couldn't pause the TowerPlacer");

        if (placeScript != null)
            placeScript.Pause(pause);

        IsPaused = pause;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    private void StartWaveTimer()
    {
        WaveTimer.gameObject.SetActive(true);
        WaveTimer.TimeToWait = TimeBetweenWaves;
    }

    private IEnumerator SpawnNextWave()
    {
        yield return new WaitForSeconds(TimeBetweenWaves);
        SpawnEnemyScript spawnEnemies = SpawnPoint.GetComponent<SpawnEnemyScript>();
        Assert.IsNotNull(spawnEnemies, "GameLoop.SpawnNextWave: Couldnt find SpawnEnemyScriptComponent!");
        spawnEnemies.RequestSpawnWave();
    }

    private void Init()
    {
        GameState.MaxWaves = EnemyWaves.Count;
        WaveTimer.gameObject.SetActive(false);
        MainHUD.SetActive(true);
        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(false);
        VictoryMenu.SetActive(false);

        Pause(false);

        //reinit game state
        GameState.EnemiesEscaped = 0;
        GameState.Gold = StartingGold;
        GameState.CurrentWave = null;
    }

    private void GameOver()
    {
        Pause(true);
        GameOverMenu.SetActive(true);
        PauseMenu.SetActive(false);
    }


    private void Victory()
    {
        Pause(true);
        PauseMenu.SetActive(false);
        VictoryMenu.SetActive(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }
}
