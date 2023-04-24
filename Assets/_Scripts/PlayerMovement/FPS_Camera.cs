using UnityEngine;

public class FPS_Camera : MonoBehaviour
{
    public float touchSensitivity = 10f;
    public Transform playerBody;

    private float rotation_x;
    private float rotation_y;
    private float input_x;
    private float input_y;
    private float input_raw_x;
    private float input_raw_y;
    private float rotationSensitivity;

    public FixedTouchField touchArea;

    public bool can_move = true;
    public bool use_y_rotation = false;

    [Space(8)]
    [Header("Clamp Angle")]
    public float min_x = -90f;
    public float max_x = 90;
    public float min_y = -90f;
    public float max_y = 90;

    void Update()
    {
        if (can_move)
        {
            HandleInput();
            HandleRotation();
        }
    }

    void HandleInput()
    {
        if (touchArea)
        {
            input_raw_x = touchArea.TouchDist.x;
            input_raw_y = touchArea.TouchDist.y;
            rotationSensitivity = touchSensitivity;
        }

        input_x = input_raw_x * rotationSensitivity * Time.deltaTime;
        input_y = input_raw_y * rotationSensitivity * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (use_y_rotation)
        {
            rotation_x -= input_y;
            rotation_x = Mathf.Clamp(rotation_x, min_x, max_x);
            rotation_y += input_x;
            rotation_y = Mathf.Clamp(rotation_y, min_y, max_y);
            transform.localRotation = Quaternion.Euler(rotation_x, rotation_y, 0f);
        }

        else
        {
            rotation_x -= input_y;
            rotation_x = Mathf.Clamp(rotation_x, min_x, max_x);
            transform.localRotation = Quaternion.Euler(rotation_x, 0f, 0f);
        }

        if(playerBody)
            playerBody.Rotate(Vector3.up * input_x);
    }
}
