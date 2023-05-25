using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyCharacterMovement;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] float animSmooth = 2f;
    [SerializeField] float coughtWait = 1f;

    Animator _anim;
    CharacterMovement _movement;
    Character _character;

    readonly string speedX = "SpeedX";
    readonly string speedY = "SpeedY";

    void Start()
    {
        _movement = GetComponent<CharacterMovement>();
        _character = GetComponent<Character>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        var state = _movement.GetState();
        Vector3 dir = state.velocity.WithY(0);
        dir = transform.InverseTransformDirection(dir);

        _anim.SetFloat(speedX, Mathf.Lerp(_anim.GetFloat(speedX), dir.x, animSmooth * Time.deltaTime));
        _anim.SetFloat(speedY, Mathf.Lerp(_anim.GetFloat(speedY), dir.z, animSmooth * Time.deltaTime));

        _anim.SetBool("OnGround", state.hitGround);

        _anim.SetBool("Swim", _character.IsSwimming());
    }

    public void FishingCast()
    {
        _anim.SetTrigger("FishingCast");
    }

    public void FishCatch()
    {
        _anim.Play("FishingExit");

        // await UniTask.WaitForSeconds(coughtWait);

        // _anim.Play("Fish_Caught");
    }
}
