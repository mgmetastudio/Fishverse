using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class FindDependencies : MonoBehaviour
{
    [MenuItem("Assets/Find Texture Dependencies")]
    public static void FindTextures()
    {
        // Get the selected prefab
        GameObject prefab = Selection.activeGameObject;

        if (prefab == null)
        {
            Debug.LogError("No prefab selected.");
            return;
        }

        // Get all SpriteRenderer and Image components in the prefab
        SpriteRenderer[] spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
        Image[] images = prefab.GetComponentsInChildren<Image>(true);

        // A list to store all the found sprites
        List<Object> sprites = new List<Object>();

        // Iterate over all SpriteRenderer components
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            // If the sprite is not null
            if (spriteRenderer.sprite != null)
            {
                // Add the sprite to the list
                sprites.Add(spriteRenderer.sprite.texture);
            }
        }

        // Iterate over all Image components
        foreach (Image image in images)
        {
            // If the sprite is not null
            if (image.sprite != null)
            {
                // Add the sprite to the list
                sprites.Add(image.sprite.texture);
            }
        }

        // Select all found sprites in the Project tab
        Selection.objects = sprites.ToArray();
    }
}
