using UnityEngine;

public class DestroyTimed : MonoBehaviour
{
    public float destroy_time = 2f;

    private void Start()
    {
        Destroy(gameObject, destroy_time);
    }
}
