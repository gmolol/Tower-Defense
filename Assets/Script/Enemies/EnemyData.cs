using UnityEngine;

// =====================================================
// EnemyData: stat card for one enemy type (Raptor, Triceratops, ...)
// Create one: right-click > Create > Scriptable Objects > EnemyData
// =====================================================
[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public enum enemyType
    {
        Normal,
        Fast,
        Tank,
        Boss
    };

    [Header("Enemy Type")]
    public enemyType type;

    [Header("Enemy Info")]
    public string enemyName;
    public GameObject enemyModel;

    [Header("Stat")]
    public float health;
    public float speed;
    public int damageToBase;
    public int reward;

    [Header("Waves")]
    public int firstWave;
}
