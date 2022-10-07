using UnityEngine;

public class Boat_Movement : MonoBehaviour
{
    public float speed = 15f;
    public float lerp_speed_forward = 2f;
    public float lerp_speed_side = 2f; 
    public float rotation_speed = 10f;
    public float gravity = -10f;
    public bool useKeyboardInput = false;

    private float sideInput;
    private float forwardInput;
    private Vector3 movement;

    private float side_input_lerp;
    private float forward_input_lerp;

    [HideInInspector]
    public CharacterController controller;
    private Vector3 velocity;

    public FixedJoystick moveJoystick;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleGravity();
    }

    public bool IsMoving()
    {
        if (forwardInput != 0 || sideInput != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void HandleRotation()
    {
        if (forwardInput != 0)
        {
            transform.Rotate(Vector3.up * side_input_lerp * Time.deltaTime * rotation_speed * forwardInput);
        }
    }

    void HandleInput()
    {
        if (useKeyboardInput == false)
        {
            if (moveJoystick)
            {
                sideInput = moveJoystick.Horizontal;
                forwardInput = moveJoystick.Vertical;

                side_input_lerp = Mathf.Lerp(side_input_lerp, sideInput, Time.deltaTime * lerp_speed_side);
                forward_input_lerp = Mathf.Lerp(forward_input_lerp, forwardInput, Time.deltaTime * lerp_speed_forward);
                return;
            }
        }

        sideInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        side_input_lerp = Mathf.Lerp(side_input_lerp, sideInput, Time.deltaTime * lerp_speed_side);
        forward_input_lerp = Mathf.Lerp(forward_input_lerp, forwardInput, Time.deltaTime * lerp_speed_forward);
    }

    void HandleMovement()
    {
        movement = transform.forward * forward_input_lerp;
        controller.Move(movement * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
