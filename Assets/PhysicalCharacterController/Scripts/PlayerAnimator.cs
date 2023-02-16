using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] float animSmooth = 2f;
    public CharacterController cc { get; private set; }
    public Animator anim { get; private set; }

    readonly string speedX = "SpeedX";
    readonly string speedY = "SpeedY";

    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 dir = cc.velocity.normalized;
        anim.SetFloat(speedX, Mathf.Lerp(anim.GetFloat(speedX), dir.x, animSmooth * Time.deltaTime));
        anim.SetFloat(speedY, Mathf.Lerp(anim.GetFloat(speedY), dir.z, animSmooth * Time.deltaTime));
    }
}
