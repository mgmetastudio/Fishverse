using UnityEngine;

public class MiniMap_Scanner : MonoBehaviour
{
    public float rotation_speed = 40f;
    private RectTransform rect_transform;
    private Vector3 new_rotation = Vector3.zero;
    
    private void Start()
    {
        rect_transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        new_rotation.z = rotation_speed * Time.deltaTime;
        rect_transform.Rotate(new_rotation);
    }
}
