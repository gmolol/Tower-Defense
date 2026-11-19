using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{

    public enum unitType
    {
        Hill,
        Hybrid,
        Ground,
        Farm

    };


    [Header("Unit Type")]
    [SerializeField] public unitType type;

    [Header("Unit Info")]
    public string unitName;
    public GameObject unitModel;
    

    [Header("Stat")]
    public float damage;
    public float range;
    public float spa;
    public int placement;
    
    //public string unitType;

    //TODO: UPGRADES COST, STAT CHANGES(through classes and hashmap), UPDATE VISUAL
}
