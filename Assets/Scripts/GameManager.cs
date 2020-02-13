using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage
{
    diskRaise,
    gracePeriod,
    roundInProgress,
    roundOver
}

public class GameManager : MonoBehaviour
{
    public static bool DebugMode = true;
    public GameStage GameFlow;
    public List<GameObject> Spawners;
    public GameObject Disk;
    public GameObject Player;
    public GameObject Enemy;

    public int GracePeriodLength = 30;
    int GracePeriodFrames;

    bool firstWave = false;
    public int EnenmyLimit = 10; //Amount of Enemies that can be spawned in at a time
    public int EnenmyInRound = 20; //Amount of Enemies that will be spawned during the round
    public static int EnenmiesLeft; //Amount of Enemies that still have to be killed
    public int EnenmiesLeftToSpawn; //Amount of Enemies that still have to be spawned
    public int framesBetweenEnemies = 240; //Number of frames in between enemy spawns
    public int EnemySpawnFrames; //Number of frames before the next enemy can spawn
    public int EnemiesInFirstSpawn = 5; //Number of enemies in the first spawn
    static List<GameObject> enemies;

    // Start is called before the first frame update
    void Start()
    {
        GameFlow = GameStage.diskRaise;
        GracePeriodFrames = GracePeriodLength * 90;
        EnenmiesLeft = EnenmyInRound;
        EnenmiesLeftToSpawn = EnenmyInRound;
        enemies = new List<GameObject>(EnenmyLimit);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameFlow)
        {
            case GameStage.diskRaise:

                Disk.transform.position = Vector3.MoveTowards(Disk.transform.position, new Vector3(0,-0.5f,0), Time.deltaTime/2);
                if (Disk.transform.position == new Vector3(0, -0.5f, 0))
                {
                    GameFlow = GameStage.gracePeriod;
                }
                break;
            case GameStage.gracePeriod:
                if (!UIManager.roundInfoPanel.activeInHierarchy && (!UIManager.inspecting && !UIManager.SkillMenuActive))
                {
                    UIManager.roundInfoPanel.SetActive(true);
                }
                GracePeriodFrames--;
                if (GracePeriodFrames % 90 == 0 && GracePeriodFrames > 0)
                {
                    GracePeriodLength--;
                    if (UIManager.roundInfoPanel.activeInHierarchy && (!UIManager.inspecting && !UIManager.SkillMenuActive))
                    {
                        UIManager.updateRoundCountdown(GracePeriodLength);
                    }
                }
                else if (GracePeriodFrames == 0)
                {
                    UIManager.updateRoundEnemies(EnenmiesLeft);
                    firstWave = true;
                    GameFlow = GameStage.roundInProgress;
                }
                break;
            case GameStage.roundInProgress:
                if (EnenmiesLeft == 0) GameFlow = GameStage.roundOver;

                if (firstWave)
                {
                    for (int i = 0; i < EnemiesInFirstSpawn; i++)
                    {
                        SpawnEnemy();
                    }
                    EnemySpawnFrames = framesBetweenEnemies;
                }
                else if (EnenmiesLeft > 0 && enemies.Count < EnenmyLimit)
                {
                    if (EnenmiesLeftToSpawn != 0)
                    {
                        if (EnemySpawnFrames != 0)
                        {
                            EnemySpawnFrames--;
                        }
                        else
                        {
                            SpawnEnemy();
                            EnemySpawnFrames = framesBetweenEnemies;
                        }
                    }
                }

                if (firstWave) firstWave = false;
                break;
            case GameStage.roundOver:
                UIManager.updateRoundResult("You Win!");
                break;
        }
    }

    GameObject FindBestSpawner()
    {
        GameObject bestSpawner = null;
        float bestSpawnerDistanceFromPlayer = 0;
        foreach(GameObject go in Spawners)
        {
            float distance = Vector3.Distance(go.transform.position, Player.transform.position);
            if (bestSpawner == null)
            {
                if(distance > 5f)
                {
                    bestSpawnerDistanceFromPlayer = distance;
                    bestSpawner = go;
                }
            }
            else if (distance < bestSpawnerDistanceFromPlayer && distance > 5f)
            {
                if (!go.GetComponent<Spawner>().recentlyUsed)
                {
                    go.GetComponent<Spawner>().recentlyUsed = true;
                    bestSpawner.GetComponent<Spawner>().recentlyUsed = false;
                    bestSpawnerDistanceFromPlayer = distance;
                    bestSpawner = go;
                }
            }
        }

        return bestSpawner;
    }

    void SpawnEnemy()
    {
        GameObject spawnPoint = FindBestSpawner();
        GameObject tempEnemy = Instantiate<GameObject>(Enemy);
        tempEnemy.transform.position = new Vector3(spawnPoint.transform.position.x, 0.89f, spawnPoint.transform.position.z);
        enemies.Add(tempEnemy);
        EnenmiesLeftToSpawn--;
    }

    public static void RemoveEnemy(GameObject enemyToBeRemoved)
    {
        enemies.Remove(enemyToBeRemoved);
        Destroy(enemyToBeRemoved);
        EnenmiesLeft--;
        UIManager.updateRoundEnemies(EnenmiesLeft);
        if (EnenmiesLeft == 0)
        {
            foreach (GameObject go in enemies)
            {
                Destroy(go);//need to limit spawns when getting close to end
            }
        }
    }
}


