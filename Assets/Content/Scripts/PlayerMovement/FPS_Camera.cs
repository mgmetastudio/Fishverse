using UnityEngine;

public class FPS_Camera : MonoBehaviour
{
    public float touchSensitivity = 10f;
    public Transform playerBody;

    private float rotation_x;
    private float input_x;
    private float input_y;
    private float input_raw_x;
    private float input_raw_y;
    private float rotationSensitivity;

    public FPS_Movement fpsMovement;
    public FixedTouchField touchArea;

    public bool can_move = true;

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
        rotation_x -= input_y;
        rotation_x = Mathf.Clamp(rotation_x, -90f, 90f);
        transform.localRotation = Quaternion.Euler(rotation_x, 0f, 0f);
        playerBody.Rotate(Vector3.up * input_x);
    }
}
