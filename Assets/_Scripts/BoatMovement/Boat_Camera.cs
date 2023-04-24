using UnityEngine;

public class Boat_Camera : MonoBehaviour
{
    public float smoothing;
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.position, target.position + offset, smoothing * Time.deltaTime);
    }
}
