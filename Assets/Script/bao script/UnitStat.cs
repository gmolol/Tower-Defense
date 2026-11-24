using UnityEngine;

public class UnitStat : MonoBehaviour
{

    [Header("Stat")]
    public string unitName;
    public float damage;
    public float range;
    public float spa;
    public int placement;

    public string type;

    


    public void initializeUnit(UnitData ud) 
    {
        unitName = ud.name;
        damage = ud.damage; range = ud.range; spa = ud.spa;
        placement = ud.placement;
        type = ud.type.ToString(); ;
        
    }

    public (string n, float dmg, float speed, float rng, int placement, string t) getStat()
    {
        if (type == "Ground")
        {
            type = "GRND";
        }
        else if (type == "Hybrid") 
        {
            type = "HYBD";
        }

        return (unitName, damage, spa, range, placement, type);
    }
}
