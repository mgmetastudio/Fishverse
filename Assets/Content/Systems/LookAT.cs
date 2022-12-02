using UnityEngine;

public class LookAT : MonoBehaviour
{
    public Vector3 offset;
    public Transform target;

    void Update()
    {
        transform.LookAt(target);
        transform.rotation *= Quaternion.Euler(offset);
    }
}
