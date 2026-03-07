using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;
    private void Awake()
    {
        instance = this;
    }

    public List<WaveInfo> waves;
    private int MaxWave;
    private int currentWave;

    public float maxXSpawn, maxYSpawn;
    private float spawnTimeCounter;

    private int num;

    //生成测试
    public List<GameObject> generatedEnemiesList;
    

    // Start is called before the first frame update
    void Start()
    {   
        //40,30
        maxXSpawn = 29; maxYSpawn = 18;
        currentWave = 0;
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimeCounter -= Time.deltaTime;
        if (spawnTimeCounter <= 0) 
        {
            spawnTimeCounter = waves[currentWave].timeBetweenSpawn;
            num = Random.Range(0, waves[currentWave].EnemiesList.Count);
            GameObject enemy = Instantiate(waves[currentWave].EnemiesList[num].enemy, SeleceSpawnPoint(), Quaternion.Euler(0, 0, 0));
            waves[currentWave].EnemiesList[num].currentEnemiesAmount++;
            //生成测试
            generatedEnemiesList.Add(enemy);
            Debug.Log("已生成敌人：" + waves[currentWave].EnemiesList[num].enemy.name + "第" + waves[currentWave].EnemiesList[num].currentEnemiesAmount + "个");
        }

        for (int i = 0; i < waves[currentWave].EnemiesList.Count; i++) 
        {
            EnemiesInfoToSpawn enemy = waves[currentWave].EnemiesList[i];
            if (enemy.currentEnemiesAmount >= enemy.maxEnemiesAmount) 
            {
                waves[currentWave].EnemiesList.RemoveAt(i);
            }
        }

        //如果当前阶段敌人待生成敌人列表为空，则跳到下个阶段（如果是最后一阶段暂未处理）
        if (waves[currentWave].EnemiesList.Count == 0 && generatedEnemiesList.Count <= 0) 
        {
            currentWave++;
            Debug.Log("进入下个阶段");
        }
    }
    //生成测试
    public void RemoveInList(GameObject theEnemy) 
    {
        generatedEnemiesList.Remove(theEnemy);
    } 

    public Vector2 SeleceSpawnPoint()
    {
        Vector2 spawnPoint;

        bool spawnVerticalEdge = Random.Range(0f, 1f) > .5f;
        if (spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(-maxYSpawn, maxYSpawn);
            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.x = maxXSpawn;
            }
            else
            {
                spawnPoint.x = -maxXSpawn;
            }
        }
        else 
        {
            spawnPoint.x = Random.Range(-maxXSpawn, maxXSpawn);
            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.y = maxYSpawn;
            }
            else
            {
                spawnPoint.y = -maxYSpawn;
            }
        }

        return spawnPoint;
    }
}


//波浪系统
[System.Serializable]
public class WaveInfo 
{
    public List<EnemiesInfoToSpawn> EnemiesList;
    public bool waveIsEnging;//当前波次是否已结束
    public float timeBetweenSpawn = 1f;//生成敌人的时间间隔;
}

//
[System.Serializable]
public class EnemiesInfoToSpawn 
{
    public GameObject enemy;
    public int maxEnemiesAmount;
    public int currentEnemiesAmount = 0;
} 
