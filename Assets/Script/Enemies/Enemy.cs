using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq.Expressions;

// =====================================================
// Enemy: goes on every enemy prefab
// Copies stats from an EnemyData card, walks the path
// from PathGenerator.GetWorldPath(), takes damage, and dies
// Towers will call takeDamage() once they can attack
// =====================================================

public class Enemy : MonoBehaviour
{
    [Header("Stat")]
    public string enemyName;
    public float health;
    public float speed;
    public int damageToBase;
    public int reward;

    [Header("Path")]
    List<Vector3> waypoints;
    int targetIndex = 0;

    // healthMultiplier comes from WaveSpawner (wave 1 = 1.0, wave 2 = 1.1, ...)
    public void initializeEnemy(EnemyData ed, List<Vector3> path, float healthMultiplier)
    {
        enemyName = ed.enemyName;
        health = ed.health * healthMultiplier;
        speed = ed.speed;
        damageToBase = ed.damageToBase;;
        reward = ed.reward;

        waypoints = path;
        targetIndex = 0;
        transform.position = waypoints[0];
    }

    void Update()
    {
        if (waypoints == null) return;

        // reaches base if walked pass the last point
        if (targetIndex >= waypoints.Count)
        {
            reachEnd();
            return;
        }

        // move towards the next point
        Vector3 target = waypoints[targetIndex];
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // if reached aim for next one
        if (transform.position == target)
        {
            targetIndex++;
        }
    }

    // towers will call this later
    public void takeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            die();
        }
    }

    void die()
    {
        Debug.Log(enemyName + " died");
        Destroy(gameObject);
    }

    void reachEnd()
    {
        Debug.Log(enemyName + " reached the base");
        Destroy(gameObject);
    }
}
