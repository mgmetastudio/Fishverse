using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] float animSmooth = 2f;
    public Animator anim { get; private set; }
    CharacterMovement character;

    readonly string speedX = "SpeedX";
    readonly string speedY = "SpeedY";

    void Start()
    {
        character = GetComponent<CharacterMovement>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        var state = character.GetState();
        Vector3 dir = state.velocity.WithY(0);
        dir = transform.InverseTransformDirection(dir);
        
        anim.SetFloat(speedX, Mathf.Lerp(anim.GetFloat(speedX), dir.x, animSmooth * Time.deltaTime));
        anim.SetFloat(speedY, Mathf.Lerp(anim.GetFloat(speedY), dir.z, animSmooth * Time.deltaTime));

        anim.SetBool("OnGround", state.hitGround);
    }

    public void FishingCast()
    {
        anim.SetTrigger("FishingCast");
    }
}
