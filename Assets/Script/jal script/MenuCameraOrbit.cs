using UnityEngine;

// Slowly circles the camera around the diorama
public class MenuCameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 18f;
    public float height = 12f;
    public float speed = 6f;         // degrees per second
    public float bobAmount = 0.5f;   // gentle up/down drift
    public float bobSpeed = 0.3f;

    float angle;

    // Unscaled time so the camera keeps moving while a menu panel pauses the game
    void LateUpdate()
    {
        angle += speed * Time.unscaledDeltaTime;
        float rad = angle * Mathf.Deg2Rad;
        float y = height + Mathf.Sin(Time.unscaledTime * bobSpeed) * bobAmount;

        transform.position = target.position + new Vector3(Mathf.Cos(rad) * distance, y, Mathf.Sin(rad) * distance);
        transform.LookAt(target);
    }
}
