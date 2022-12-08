using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniGame_Marker : MonoBehaviour
{
    private Camera main_camera;
    public Transform world_target;
    public Vector3 offset;
    public Image img;
    public TMP_Text text_distance;
    public int distance_text_offset = -15;

    private void Start()
    {
        main_camera = Camera.main;
    }

    void Update()
    {
        float min_x = img.GetPixelAdjustedRect().width / 2;
        float max_x = Screen.width - min_x;
        float min_y = img.GetPixelAdjustedRect().height / 2;
        float max_y = Screen.height - min_y;
        
        Vector3 screen_pos = main_camera.WorldToScreenPoint(world_target.position + offset);

        //Check if object is visible
        Vector3 vpPos = main_camera.WorldToViewportPoint(world_target.position);

        if (vpPos.x >= 0.05f && vpPos.x <= 0.95f && vpPos.y >= 0.05f && vpPos.y <= 0.95f && vpPos.z > 0.05f)
        {
            img.enabled = true;
            text_distance.enabled = true;
        }
        else
        {
            img.enabled = false;
            text_distance.enabled = false;
        }

        screen_pos.x = Mathf.Clamp(screen_pos.x, min_x, max_x);
        screen_pos.y = Mathf.Clamp(screen_pos.y, min_y, max_y);

        if (transform.position != screen_pos)
        {
            transform.position = screen_pos;
        }

        text_distance.text = (Mathf.Clamp((int)Vector3.Distance(world_target.position, main_camera.transform.position) + distance_text_offset, 0, 9999)).ToString() + "m";
    }
}
