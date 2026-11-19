using UnityEngine;

public class UnitStat : MonoBehaviour
{

    [Header("Stat")]
    public string name;
    public float damage;
    public float range;
    public float spa;
    public int placement;



    public GameObject display;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initializeUnit(UnitData ud) 
    {
        this.name = ud.name;
        damage = ud.damage; range = ud.range; spa = ud.spa;
        this.placement = ud.placement;

        
    }

    public (string n, float dmg, float speed, float rng, int placement) getStat()
    {
        return (name, damage, spa, range, placement);
    }
}
