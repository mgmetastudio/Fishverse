using UnityEngine;

public class FPS_Movement : MonoBehaviour
{
    public float speed = 15f;
    public float gravity = -10f;
    public bool useKeyboardInput = false;
    private float sideInput;
    private float forwardInput;
    private Vector3 movement;
    private CharacterController controller;
    private Vector3 velocity;

    public FixedJoystick moveJoystick;
    public bool can_move = true;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (can_move)
        {
            controller.enabled = true;
            HandleInput();
            HandleMovement();
            HandleGravity();
            controller.enabled = false;
        }
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

    void HandleInput()
    {
        if(useKeyboardInput == false)
        {
            if(moveJoystick)
            {
                sideInput = moveJoystick.Horizontal;
                forwardInput = moveJoystick.Vertical;
                return;
            }
        }

        sideInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");
    }

    void HandleMovement()
    {
        movement = transform.right * sideInput + transform.forward * forwardInput;
        controller.Move(movement * speed * Time.deltaTime);
    }

    void HandleGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /*void OnControllerColliderHit(ControllerColliderHit hit)
    {
        print(hit.gameObject.name);
    }*/
}