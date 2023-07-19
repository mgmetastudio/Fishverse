using UnityEngine;
using UnityEngine.Rendering;

public class ModelTransparencyController : MonoBehaviour
{
    public LayerMask waterLayer;
    public Renderer[] modelRenderers;
    public float fadeSpeed = 1f; // Adjust the fading speed as needed

    private bool isTouchingWater;
    private Material[][] originalMaterials;

    private void Start()
    {
        originalMaterials = new Material[modelRenderers.Length][];

        for (int i = 0; i < modelRenderers.Length; i++)
        {
            Renderer renderer = modelRenderers[i];
            originalMaterials[i] = new Material[renderer.materials.Length];

            for (int j = 0; j < renderer.materials.Length; j++)
            {
                originalMaterials[i][j] = renderer.materials[j];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (waterLayer == (waterLayer | (1 << other.gameObject.layer)))
        {
            isTouchingWater = true;
            Debug.Log("Model is touching the water.");
            UpdateSurfaceType();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (waterLayer == (waterLayer | (1 << other.gameObject.layer)))
        {
            isTouchingWater = false;
            Debug.Log("Model is no longer touching the water.");
            UpdateSurfaceType();
        }
    }

    private void UpdateSurfaceType()
    {
        string surfaceType = isTouchingWater ? "Transparent" : "Opaque";

        for (int i = 0; i < modelRenderers.Length; i++)
        {
            Renderer renderer = modelRenderers[i];

            Material[] materials = renderer.materials;
            for (int j = 0; j < materials.Length; j++)
            {
                Material material = materials[j];
                material.SetOverrideTag("_Surface", surfaceType);
                material.SetInt("_BlendOp", (int)BlendOp.Add);
                material.SetInt("_SrcBlend", (int)BlendMode.One);
                material.SetInt("_DstBlend", (int)BlendMode.Zero);
                material.SetInt("_ZWrite", isTouchingWater ? 0 : 1);
                material.EnableKeyword("_SURFACE_TYPE_" + surfaceType);
            }

            renderer.materials = materials;
        }
    }

    public void RestoreTransparency()
    {
        for (int i = 0; i < modelRenderers.Length; i++)
        {
            Renderer renderer = modelRenderers[i];
            renderer.materials = originalMaterials[i];
        }

        isTouchingWater = false;
        UpdateSurfaceType();
    }
}
