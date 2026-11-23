using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.VisualScripting;

// =====================================================
// WaveSpawner: creates enemies at the start of the path
// Currently: press Space to spawn enemyTypes[0] (testing only)
// Next: automatic waves with difficulty scaling
// =====================================================

public class WaveSpawner : MonoBehaviour
{
    [Header("Map Generator")]
    public PathGenerator pathGenerator;

    [Header("Enemy Types")]
    public List<EnemyData> enemyTypes = new List<EnemyData>();

    void Update()
    {
        // test: press spacebar to spawn an enemy
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spawnEnemy(enemyTypes[0]);
        }
    }

    void spawnEnemy(EnemyData ed)
    {
        List<Vector3> path = pathGenerator.GetWorldPath();

        GameObject enemy = Instantiate(ed.enemyModel, path[0], Quaternion.identity);
        enemy.GetComponent<Enemy>().initializeEnemy(ed, path);
    }
}
