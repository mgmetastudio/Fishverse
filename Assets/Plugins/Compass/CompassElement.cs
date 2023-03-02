using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CompassElement : MonoBehaviour
{
    [Tooltip("Attach your desired icon. This will be displayed on the compass")] public Sprite icon;

    [HideInInspector] public Image image;

    [HideInInspector] public int id;

    [SerializeField, Tooltip("Adds the compass element to the compass on start")] private bool addOnStart;

    public Vector2 GetVector2Pos() { return new Vector2(transform.position.x, transform.position.z); }

    private async void Start()
    {
        await UniTask.WaitForSeconds(.5f);
        if (addOnStart) Add();
    }

    public void Add() => Compass.Instance.AddCompassElement(this);

    public void Remove() => Compass.Instance.RemoveCompassElement(this);
}
