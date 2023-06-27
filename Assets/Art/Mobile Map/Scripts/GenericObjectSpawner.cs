#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenericObjectSpawner : MonoBehaviour
{
    [SerializeField] private Terrain _terrain;
    [SerializeField] private SpawnData[] _spawnDatas;

    private static readonly int TextureRotation = Shader.PropertyToID("_TextureRotation");
    private static readonly int TilingBase = Shader.PropertyToID("_TilingBase");

    [Button("Spawn")]
    private void SetupPixelsToSpawn()
    {
        Debug.Log("Start SETUP");

        foreach (var spawnData in _spawnDatas)
        {
            if (spawnData.Enabled)
                spawnData.SetupTempSpawnDatas();
        }

        Debug.Log("Complete SETUP");

        Spawn();
    }

    private void Spawn()
    {
        Bounds bounds = _terrain.terrainData.bounds;

        Material terrainMaterial = _terrain.materialTemplate;

        var rotation = terrainMaterial.GetFloat(TextureRotation);
        Vector2 tilingBase = terrainMaterial.GetVector(TilingBase);

        foreach (var spawnData in _spawnDatas)
        {
            if (!spawnData.Enabled)
                continue;

            List<Transform> childs = new List<Transform>();

            foreach (Transform child in spawnData.Parent) childs.Add(child);


            for (int i = 0; i < childs.Count; i++)
            {
                if (childs[i] == null)
                    continue;

                DestroyImmediate(childs[i].gameObject);
            }
        }

        for (var index = 0; index < _spawnDatas.Length; index++)
        {
            var spawnData = _spawnDatas[index];
            if (!spawnData.Enabled) continue;

            spawnData.Parent.position = new Vector3(_terrain.transform.position.x + bounds.center.x, 0,
                _terrain.transform.position.z + bounds.center.z);
            spawnData.Parent.localScale = new Vector3(tilingBase.x, 1, tilingBase.y);
            spawnData.Parent.rotation = Quaternion.Euler(0, rotation, 0);

            foreach (TempSpawnData data in spawnData.PixelsToSpawn)
            {
                var instance =
                    PrefabUtility.InstantiatePrefab(spawnData.Prefabs.GetRandom(), spawnData.Parent) as GameObject;

                var x = Mathf.Lerp(_terrain.transform.position.x + bounds.min.x,
                    _terrain.transform.position.x + bounds.max.x, (float)data.PixelPos.x / spawnData.Tex.width);
                var z = Mathf.Lerp(_terrain.transform.position.z + bounds.min.z,
                    _terrain.transform.position.z + bounds.max.z, (float)data.PixelPos.y / spawnData.Tex.height);

                z = _terrain.transform.position.z + bounds.max.z - z;

                instance.transform.position = new Vector3(x, _terrain.SampleHeight(new Vector3(x, 0, z)), z);


                instance.transform.localScale = spawnData.InstanceSpawnData.GetRandomScale();
                instance.transform.rotation = Quaternion.Euler(spawnData.InstanceSpawnData.GetRandomRotation());
            }
        }
    }
}

[Serializable]
public class SpawnData
{
    public bool Enabled;
    public Transform Parent;
    [Space]
    public List<GameObject> Prefabs;
    public InstanceSpawnData InstanceSpawnData;
    [Space]
    public Texture2D Tex;

    [SerializeField] private int _objectsCount = 200;
    [SerializeField] private int _pixelsToMarkAroundObject = 20;
    [SerializeField] private int _sectorCount = 4;
    [SerializeField] private float _maxColorValue;
    [Range(0, 1)][SerializeField] private float _minColorValue = 0;
    [Range(0, 10)][SerializeField] private float _distributionPower = 1;

    [HideInInspector] public List<TempSpawnData> PixelsToSpawn = new();
    private List<TempSpawnData> _tempSpawnDatas = new();

    public void SetupTempSpawnDatas()
    {
        _tempSpawnDatas.Clear();
        PixelsToSpawn.Clear();


        float currentColorValue = 0;

        var pixels = Tex.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color color = pixels[i];

            if (currentColorValue < color.r)
                currentColorValue = color.r;
        }

        for (int x = 0; x < Tex.width; x++)
        {
            for (int y = 0; y < Tex.height; y++)
            {
                _tempSpawnDatas.Add(new TempSpawnData(new Pixel(x, y)));
            }
        }

        // for (int x = 0; x < _tex.width; x++)
        // {
        //     for (int y = 0; y < _tex.height; y++)
        //     {
        //         _tempSpawnDatas.Add(new TempSpawnData(new Pixel(x,y), _pixelsToMarkAroundObject));
        //     }
        // }

        // foreach (var spawnData in _tempSpawnDatas)
        // {
        //     for (int i = 0; i < spawnData.ImpactedPixels.Length; i++)
        //     {
        //         spawnData.ImpactedPixels = _tempSpawnDatas.FindAll(data => spawnData.Area.Contains(data.PixelPos)).ToArray();
        //     }
        // }

        _maxColorValue = currentColorValue;

        SetupPixelsToSpawn();
    }

    private void SetupPixelsToSpawn()
    {
        for (int i = 0; i < _objectsCount; i++)
        {
            PixelsToSpawn.Add(GetRandomPos());
            ClearList();
        }
    }

    private TempSpawnData GetRandomPos()
    {
        TempSpawnData tempSpawnData = _tempSpawnDatas.GetRandom();

        tempSpawnData.IsChecked = true;

        Color pixel = Tex.GetPixel(tempSpawnData.PixelPos.x, tempSpawnData.PixelPos.y);

        if (CanSpawn(pixel.r))
        {
            MarkPixels(tempSpawnData.PixelPos);
            return tempSpawnData;
        }

        return GetRandomPos();
    }

    private void ClearList()
    {
        _tempSpawnDatas.RemoveAll(data => data.IsChecked);
        _tempSpawnDatas.RemoveAll(data => data.IsPossibleToSpawn == false);
    }

    private bool CanSpawn(float color)
    {
        float normalizedColor = color / _maxColorValue;
        float poweredColor = Mathf.Pow(normalizedColor, _distributionPower);
        int r = Random.Range((int)(_minColorValue * 100), (int)(_maxColorValue * 100));
        bool canSpawn = r <= poweredColor * 100;
        return canSpawn;
    }

    private void MarkPixels(Pixel pos)
    {
        for (int x = pos.x - _pixelsToMarkAroundObject; x < pos.x + _pixelsToMarkAroundObject; x++)
        {
            for (int y = pos.y - _pixelsToMarkAroundObject; y < pos.y + _pixelsToMarkAroundObject; y++)
            {
                var data = _tempSpawnDatas.FirstOrDefault(spawnData => spawnData.PixelPos == new Pixel(x, y));
                if (data != null)
                {
                    data.IsPossibleToSpawn = false;
                }
            }
        }
    }
}

[Serializable]
public class TempSpawnData
{
    public Pixel PixelPos;
    public bool IsPossibleToSpawn = true;
    public bool IsChecked;
    public TempSpawnData[] ImpactedPixels;
    public Pixel[] Area;

    public TempSpawnData(Pixel pixelPos)
    {
        PixelPos = pixelPos;
    }

    public TempSpawnData(Pixel pixelPos, int markedPixelsCount)
    {
        PixelPos = pixelPos;
        var pixelsCount = markedPixelsCount * 2 + 1;
        ImpactedPixels = new TempSpawnData[pixelsCount * pixelsCount];

        Area = new Pixel[pixelsCount * pixelsCount];

        int i = 0;

        for (int x = pixelPos.x - markedPixelsCount; x < pixelPos.x + markedPixelsCount; x++)
        {
            for (int y = pixelPos.y - markedPixelsCount; y < pixelPos.y + markedPixelsCount; y++)
            {
                var pixel = new Pixel(x, y);

                if (pixel == pixelPos)
                    continue;

                Area[i] = pixel;
                i++;
            }
        }
    }
}

[Serializable]
public struct Pixel
{
    public int x;
    public int y;

    public Pixel(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Pixel pixel1, Pixel pixel2)
    {
        return (pixel1.x == pixel2.x && pixel1.y == pixel2.y);
    }

    public static bool operator !=(Pixel pixel1, Pixel pixel2)
    {
        return (pixel1.x != pixel2.x && pixel1.y != pixel2.y);
    }
}

[Serializable]
public struct InstanceSpawnData
{
    public float MinScale;
    public float MaxScale;

    public Vector3 MinRotation;
    public Vector3 MaxRotation;

    public Vector3 GetRandomRotation() => new(Random.Range(MinRotation.x, MaxRotation.x),
        Random.Range(MinRotation.y, MaxRotation.y), Random.Range(MinRotation.z, MaxRotation.z));

    public Vector3 GetRandomScale()
    {
        var scale = Random.Range(MinScale, MaxScale);
        return Vector3.one * scale;
    }

}
#endif