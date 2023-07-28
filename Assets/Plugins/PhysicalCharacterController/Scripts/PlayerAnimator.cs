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
    private bool isBlending = false; // Flag to track if blending is already in progress
    private Coroutine blendCoroutine;

    public float maxDistance = 0.1f; // The maximum distance to check for ground
    public LayerMask groundLayer;


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


        if (_anim.GetFloat(speedX) == 0 && _anim.GetFloat(speedY) == 0)
        {
            // Start the random blending only if it's not already in progress
            if (!isBlending)
            {
                // Change the blend value to 0
                _anim.SetFloat("BlendIdle", 0f);
                blendCoroutine = StartCoroutine(RandomBlendCoroutine());
                isBlending = true;
            }
        }
        else
        {
            // Stop the blending coroutine if the character starts moving
            if (isBlending && blendCoroutine != null)
            {
                StopCoroutine(blendCoroutine);
                isBlending = false;
                _anim.SetFloat("BlendIdle", 0f); // Reset BlendIdle to 0
            }
        }
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


    private IEnumerator RandomBlendCoroutine()
    {
        while (true)
        {
            // Set the blend value to a random value between 1 and 2
            _anim.SetFloat("BlendIdle", 1f);

            // Smoothly transition from 1 to 0
            float elapsedTime = 0f;
            float blendDuration = 1.0f;
            float startBlendValue = 1f;
            while (elapsedTime < blendDuration)
            {
                float currentBlendValue = Mathf.Lerp(startBlendValue, 0f, elapsedTime / blendDuration);
                _anim.SetFloat("BlendIdle", currentBlendValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the blend value is set to 0 exactly
            _anim.SetFloat("BlendIdle", 0f);

            // Wait for a random time between 3 and 5 seconds
            float waitTime1 = Random.Range(10f, 12f);
            yield return new WaitForSeconds(waitTime1);

            // Set the blend value back to 1
            _anim.SetFloat("BlendIdle", 0f);

            // Smoothly transition from 0 to 1
            elapsedTime = 0f;
            blendDuration = 1.0f;
            startBlendValue = 0f;
            while (elapsedTime < blendDuration)
            {
                float currentBlendValue = Mathf.Lerp(startBlendValue, 1f, elapsedTime / blendDuration);
                _anim.SetFloat("BlendIdle", currentBlendValue);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the blend value is set to 1 exactly
            _anim.SetFloat("BlendIdle", 1f);

            // Wait for another random time between 3 and 5 seconds before starting the loop again
            float waitTime2 = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime2);
        }
    }

}

