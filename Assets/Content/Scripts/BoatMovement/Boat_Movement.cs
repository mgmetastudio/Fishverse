using UnityEngine;

public class Boat_Movement : MonoBehaviour
{
    public float speed = 15f;
    public float rotation_speed = 10f;
    public float gravity = -10f;
    public bool useKeyboardInput = false;
    private float sideInput;
    private float forwardInput;
    private Vector3 movement;
    [HideInInspector]
    public CharacterController controller;
    private Vector3 velocity;

    public FixedJoystick moveJoystick;
    public FixedJoystick rotationJoystick;

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
            transform.Rotate(Vector3.up * sideInput * Time.deltaTime * rotation_speed * forwardInput);
        }
    }

    void HandleInput()
    {
        if (useKeyboardInput == false)
        {
            if (moveJoystick)
            {
                sideInput = rotationJoystick.Horizontal;
                forwardInput = moveJoystick.Vertical;
                return;
            }
        }

        sideInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
    }

    void HandleMovement()
    {
        movement = transform.forward * forwardInput;
        controller.Move(movement * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
