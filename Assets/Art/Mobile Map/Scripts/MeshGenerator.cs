using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    public Texture2D heightMap;
    public Terrain terrain;
    public int resolution;
    public float heightMultiplier;
    public float threshold;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    [Button]
    public void GenerateMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        int xSize = resolution;
        int zSize = resolution;

        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float xCoord = (float)x / xSize;
                float zCoord = (float)z / zSize;

                float xTerrain = xCoord * terrainWidth;
                float zTerrain = zCoord * terrainLength;

                float terrainHeight = terrain.SampleHeight(new Vector3(xTerrain, 0, zTerrain));
                float y = terrainHeight;
                //Debug.Log($"Y = {y}");

                float grayscale = heightMap.GetPixelBilinear(xCoord, zCoord).grayscale;
                if (grayscale > threshold)
                {
                    y += grayscale * heightMultiplier;
                }

                vertices[i] = new Vector3(xTerrain, y, zTerrain);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}
