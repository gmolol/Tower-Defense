using UnityEngine;

// Menu tower: turns toward the nearest enemy and shoots it, with a point-light muzzle flash
public class DioramaTower : MonoBehaviour
{
    public Transform head;
    public Light muzzleFlash;           // point light, starts disabled
    public float range = 5f;
    public float fireInterval = 0.8f;
    public float damage = 1f;
    public float flashTime = 0.05f;

    float cooldown;

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (muzzleFlash.enabled && cooldown < fireInterval - flashTime) muzzleFlash.enabled = false;

        Enemy target = FindNearest();
        if (target == null) return;

        Vector3 dir = target.transform.position - head.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        head.rotation = Quaternion.Slerp(head.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);

        if (cooldown <= 0f)
        {
            cooldown = fireInterval;
            muzzleFlash.enabled = true;
            target.takeDamage(damage);
        }
    }

    Enemy FindNearest()
    {
        Enemy best = null;
        float bestDist = range * range;
        foreach (var e in FindObjectsByType<Enemy>(FindObjectsSortMode.None))
        {
            float d = (e.transform.position - transform.position).sqrMagnitude;
            if (d < bestDist) { bestDist = d; best = e; }
        }
        return best;
    }
}