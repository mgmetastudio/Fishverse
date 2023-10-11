using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    public Terrain terrain;
    [Header("Audio Source")]
    public AudioSource audioSource; // Reference to the AudioSource component.

    [Header("Footstep Library")]
    public FootStepsGroup footstepLibrary; // Reference to your FootStepsGroup class.

    [Header("Ground Layer")]
    public LayerMask groundLayer; // Layer mask to determine the ground.
    [Header("Water Layer")]
    public LayerMask waterLayer;
    [Header("Footstep Interval")]
    public float footstepInterval = 0.5f; // Time interval between footstep sounds.
    private float lastFootstepTime; // Time when the last footstep sound was played.

    private void Start()
    {
        lastFootstepTime = Time.time;
    }

    private void Update()
    {
        if (IsGrounded() && Time.time - lastFootstepTime >= footstepInterval)
        {
            lastFootstepTime = Time.time;
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundLayer);
    }

    public void PlayFootstepSound()
    {
        if (footstepLibrary != null && audioSource != null)
        {
            string currentGroundTag = DetermineGroundTag();
            FootStepsGroup.AudioGroup audioGroup = footstepLibrary.GetGroupFor(currentGroundTag);

            if (audioGroup != null)
            {
                AudioClip footstepClip = audioGroup.GetRandomClip();
                audioSource.PlayOneShot(footstepClip);
            }
            else
            {
                Debug.LogWarning("No AudioGroup found for tag: " + currentGroundTag);
            }
        }
    }

    private string DetermineGroundTag()
    {
        Vector3 spherePosition = transform.position + new Vector3(0, -0.2f, 0);
        float sphereRadius = 0.3f;

        // Overlap the sphere with the water layer.
        Collider[] colliders = Physics.OverlapSphere(spherePosition, sphereRadius, waterLayer);

        // If there are any colliders overlapping the sphere and the tag is "Water", return the tag of the first collider.
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Water"))
            {
                return "Water";
            }
        }

        // Default tag.
        return "Generic";
    }

}
