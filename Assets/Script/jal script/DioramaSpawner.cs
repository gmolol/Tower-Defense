using System.Collections.Generic;
using UnityEngine;


public class DioramaSpawner : MonoBehaviour
{
    public PathGenerator pathGenerator;
    public List<EnemyData> enemyTypes = new List<EnemyData>();
    public GameObject towerPrefab;
    public Transform riftCore;          // optional: moved to the end of the path
    public int towerCount = 4;
    public float interval = 1.5f;

    List<Vector3> path;
    float timer;

    void Update()
    {
        if (path == null)
        {
            if (pathGenerator.path.Count < 2) return; // bao start() gen
            path = pathGenerator.GetWorldPath();
            if (riftCore != null) riftCore.position = path[path.Count - 1];
            PlaceTowers();
        }

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = interval;

        EnemyData ed = enemyTypes[Random.Range(0, enemyTypes.Count)];
        GameObject enemy = Instantiate(ed.enemyModel, path[0], Quaternion.identity);
        enemy.GetComponent<Enemy>().initializeEnemy(ed, path);
    }

    // Puts towers one grid cell (2 units) beside random path points, never on the road or each other
    void PlaceTowers()
    {
        Bounds ground = pathGenerator.map.GetComponent<MeshRenderer>().bounds;
        Vector3[] sides = { Vector3.left * 2f, Vector3.right * 2f, Vector3.forward * 2f, Vector3.back * 2f };
        var used = new List<Vector3>(path);

        for (int placed = 0, tries = 0; placed < towerCount && tries < 200; tries++)
        {
            Vector3 spot = path[Random.Range(1, path.Count - 1)] + sides[Random.Range(0, 4)];

            bool onMap = Mathf.Abs(spot.x - ground.center.x) <= ground.extents.x
                      && Mathf.Abs(spot.z - ground.center.z) <= ground.extents.z;
            if (!onMap) continue;
            if (used.Exists(p => (p - spot).sqrMagnitude < 0.1f)) continue; // road or another tower

            Instantiate(towerPrefab, spot + Vector3.down * 0.5f, Quaternion.identity, transform); // path points sit 0.5 above the ground
            used.Add(spot);
            placed++;
        }
    }
}