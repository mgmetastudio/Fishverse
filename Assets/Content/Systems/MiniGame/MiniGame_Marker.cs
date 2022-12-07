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

        if(Vector3.Dot((world_target.position - main_camera.transform.position), main_camera.transform.forward) < 0)
        {
            //Target is behind camera
            if (screen_pos.x < Screen.width / 2)
            {
                screen_pos.x = max_x;
            }
            else
            {
                screen_pos.x = min_x;
            }
        }

        screen_pos.x = Mathf.Clamp(screen_pos.x, min_x, max_x);
        screen_pos.y = Mathf.Clamp(screen_pos.y, min_y, max_y);

        if (transform.position != screen_pos)
        {
            transform.position = screen_pos;
        }

        text_distance.text = ((int)Vector3.Distance(world_target.position, main_camera.transform.position)).ToString() + "m";
    }
}
