using UnityEngine;

public class Object_Speed : MonoBehaviour
{
    public float current_speed;
    private Vector3 last_position;

    private void Start()
    {
        last_position = transform.position;
    }

    void LateUpdate()
    {
        current_speed = Vector3.Distance(transform.position, last_position) / Time.deltaTime;
        last_position = transform.position;
    }
}
