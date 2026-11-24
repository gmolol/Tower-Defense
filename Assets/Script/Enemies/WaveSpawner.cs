using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// =====================================================
// WaveSpawner
// Spawns waves of enemies on the path
// Each wave will have more enemies and health
// 
// Math:
// enemies = startEnemies + extraEnemiesPerWave * (wave - 1)
// health multiplier = 1 + healthGrowth * (wave - 1)
// =====================================================

public class WaveSpawner : MonoBehaviour
{
    [Header("Map Generator")]
    public PathGenerator pathGenerator;

    [Header("Enemy Types")]
    public List<EnemyData> enemyTypes = new List<EnemyData>();

    [Header("Wave Timing")]
    public float timeBetweenWaves = 5f;     // break before each wave (seconds)
    public float timeBetweenSpawns = 1f;    // gap between each enemy (seconds)

    [Header("Difficulty Scaling")]
    public int startEnemies = 5;            // enemies in wave 1
    public int extraEnemiesPerWave = 1;     // enemies added each wave
    public float healthGrowth = 0.1f;       // +10% health each wave

    [Header("Wave Info (just for checking)")]
    public int currentWave = 0;
    public int enemiesLeftToSpawn = 0;
    public int enemiesAlive = 0;
    public float healthMultiplier = 1f;

    float timer = 0f;
    bool onBreak = false;


    void Update()
    {
        // count how many enemies are still on the map
        enemiesAlive = FindObjectsByType<Enemy>().Length;

        // count the timer down every frame
        timer -= Time.deltaTime;

        //  1. spawning: still enemies to send out
        if (enemiesLeftToSpawn > 0)
        {
            if (timer <= 0f)
            {
                spawnEnemy(pickEnemy());
                enemiesLeftToSpawn--;
                timer = timeBetweenSpawns;
            }
        }
        //  2. wave cleared: start the break
        else if (enemiesAlive == 0 && !onBreak)
        {
            if (currentWave > 0)
            {
                Debug.Log($"Wave {currentWave} cleared!");
            }
            onBreak = true;
            timer = timeBetweenWaves;
        }
        //  3. break over: start the next wave
        else if (onBreak && timer <= 0f)
        {
            startWave();
        }

        // TEST: press K to deal 5 damage to every enemy (towers can't attack yet)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            foreach (Enemy e in FindObjectsByType<Enemy>())
            {
                e.takeDamage(5);
            }
        }
    }


    // Wave Helper
    void startWave()
    {
        onBreak = false;
        currentWave++;

        enemiesLeftToSpawn = getEnemyCount(currentWave);
        healthMultiplier = getHealthMultiplier(currentWave);
        timer = 0f;     // first enemy spawns right away

        Debug.Log($"Wave {currentWave} started: {enemiesLeftToSpawn} enemies, health x{healthMultiplier:F1}");
    }


    // Difficulty Math
    // wave 1 = 5, wave 10 = 14, wave 50 = 54
    int getEnemyCount(int wave)
    {
        return startEnemies + extraEnemiesPerWave * (wave - 1);
    }

    // wave 1 = x1.0, wave 10 = x1.9, wave 50 = x5.9
    float getHealthMultiplier(int wave)
    {
        return 1f + healthGrowth * (wave - 1);
    }



    // Spawn Helper
    // pick a random enemy type that is allowed this wave (checks firstWave on the card)
    EnemyData pickEnemy()
    {
        List<EnemyData> allowed = new List<EnemyData>();
        foreach (EnemyData ed in enemyTypes)
        {
            if (ed.firstWave <= currentWave)
            {
                allowed.Add(ed);
            }
        }

        if (allowed.Count == 0)
        {
            Debug.LogWarning("No enemy types allowed this wave. Check First Wave on your EnemyData cards.");
            return null;
        }

        return allowed[Random.Range(0, allowed.Count)];
    }

    // create the enemy at the start of the path and give it its card + path
    void spawnEnemy(EnemyData ed)
    {
        if (ed == null) return;

        List<Vector3> path = pathGenerator.GetWorldPath();
        GameObject enemy = Instantiate(ed.enemyModel, path[0], Quaternion.identity);
        enemy.GetComponent<Enemy>().initializeEnemy(ed, path, healthMultiplier);
    }
}