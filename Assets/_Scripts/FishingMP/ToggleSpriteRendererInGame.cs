using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ToggleSpriteRendererInGame : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = true;
    }
}