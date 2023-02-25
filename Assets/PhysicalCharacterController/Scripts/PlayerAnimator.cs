using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] float animSmooth = 2f;
    // public CharacterController cc { get; private set; }
    [SerializeField] CharacterMovement character;
    // public Rigidbody rb { get; private set; }
    public Animator anim { get; private set; }

    readonly string speedX = "SpeedX";
    readonly string speedY = "SpeedY";

    void Start()
    {
        character = GetComponent<CharacterMovement>();
        // cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        var state = character.GetState();
        Vector3 dir = state.velocity.WithY(0);
        dir = transform.InverseTransformDirection(dir);
        // print(dir);
        anim.SetFloat(speedX, Mathf.Lerp(anim.GetFloat(speedX), dir.x, animSmooth * Time.deltaTime));
        anim.SetFloat(speedY, Mathf.Lerp(anim.GetFloat(speedY), dir.z, animSmooth * Time.deltaTime));

        anim.SetBool("OnGround", state.hitGround);
        
    }
}
