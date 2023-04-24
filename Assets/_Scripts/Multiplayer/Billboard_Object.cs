using UnityEngine;

public class Billboard_Object : MonoBehaviour
{
    private Camera main_camera;
    public bool use_rect_transform = false;
    public RectTransform rect_transform;

    private void Start()
    {
        main_camera = Camera.main;

        if (use_rect_transform)
        {
            rect_transform = GetComponent<RectTransform>();
        }
    }

    private void LateUpdate()
    {
        if (use_rect_transform)
        {
            rect_transform.rotation = Quaternion.Euler(0f, main_camera.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, main_camera.transform.rotation.eulerAngles.y, 0f);
        }
    }
}
