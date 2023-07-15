using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class OrganizeImages : MonoBehaviour
{
    private const int MaxImagesPerFolder = 50;

    [MenuItem("Assets/Organize Images")]
    public static void Organize()
    {
        // Get the selected prefab
        GameObject prefab = Selection.activeGameObject;

        if (prefab == null)
        {
            Debug.LogError("No prefab selected.");
            return;
        }

        // Open a folder selection dialog
        string rootFolderPath = EditorUtility.SaveFolderPanel("Select Root Folder", "", "");

        // If no folder was selected, return
        if (string.IsNullOrEmpty(rootFolderPath))
        {
            return;
        }

        // Convert the absolute path to a relative path
        rootFolderPath = "Assets" + rootFolderPath.Substring(Application.dataPath.Length);

        // Get all Image components in the prefab
        Image[] images = prefab.GetComponentsInChildren<Image>(true);

        // A dictionary to store the images grouped by their parent GameObject
        Dictionary<string, List<Texture>> imageGroups = new Dictionary<string, List<Texture>>();

        // Iterate over all Image components
        foreach (Image image in images)
        {
            // If the sprite is not null
            if (image.sprite != null)
            {
                // Get the parent GameObject of the Image component
                string parentName = image.transform.parent.name;

                // If the parent GameObject is not in the dictionary, add it
                if (!imageGroups.ContainsKey(parentName))
                {
                    imageGroups[parentName] = new List<Texture>();
                }

                // Add the sprite to the list of the parent GameObject
                imageGroups[parentName].Add(image.sprite.texture);
            }
        }

        // Sort the image groups by the number of images in descending order
        var sortedImageGroups = imageGroups.OrderByDescending(group => group.Value.Count).ToList();

        // A list to store the current group of images
        List<Texture> currentGroup = new List<Texture>();

        // The name of the current folder
        string currentFolderName = "";

        // Create a main folder named after the selected prefab
        string mainFolderPath = rootFolderPath + "/" + prefab.name;

        if (!AssetDatabase.IsValidFolder(mainFolderPath))
        {
            AssetDatabase.CreateFolder(rootFolderPath, prefab.name);
        }

        // Iterate over all image groups
        foreach (KeyValuePair<string, List<Texture>> imageGroup in sortedImageGroups)
        {
            // Add the images to the current group
            currentGroup.AddRange(imageGroup.Value);

            // If the current group contains enough images
            if (currentGroup.Count >= MaxImagesPerFolder)
            {
                // Create a sub-folder for the group
                string folderPath = mainFolderPath + "/" + currentFolderName;

                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    AssetDatabase.CreateFolder(mainFolderPath, currentFolderName);
                }

                // Move the images to the sub-folder
                foreach (Texture image in currentGroup)
                {
                    string path = AssetDatabase.GetAssetPath(image);
                    string name = Path.GetFileName(path);
                    AssetDatabase.MoveAsset(path, folderPath + "/" + name);
                }

                // Start a new group
                currentGroup.Clear();
                currentFolderName = imageGroup.Key;
            }
        }

        // If there are any remaining images
        if (currentGroup.Count > 0)
        {
            // Create a sub-folder for the remaining images
            string folderPath = mainFolderPath + "/" + currentFolderName;

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(mainFolderPath, currentFolderName);
            }

            // Move the remaining images to the sub-folder
            foreach (Texture image in currentGroup)
            {
                string path = AssetDatabase.GetAssetPath(image);
                string name = Path.GetFileName(path);
                AssetDatabase.MoveAsset(path, folderPath + "/" + name);
            }
        }

        // Refresh the AssetDatabase to apply the changes
        AssetDatabase.Refresh();
    }
}
