using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SkinnedMeshRendererExtensions
{
    /// <summary>
    /// Animates blend shape to endValue 
    /// </summary>
    public static async Task AnimateBlendShape(this SkinnedMeshRenderer rend, int id, float beginValue = 0, float endValue = 100, float time = 1f)
    {
        float elapsed = 0;
        while (elapsed < time)
        {
            await UniTask.NextFrame();

            elapsed += Time.deltaTime;
            rend.SetBlendShapeWeight(id, Mathf.Lerp(beginValue, endValue, Mathf.InverseLerp(0, time, elapsed)));
        }
        rend.SetBlendShapeWeight(id, endValue);
    }

    /// <summary>
    /// Animates blend shape to endValue and then back
    /// </summary>
    public static async Task AnimateBlendShapePunch(this SkinnedMeshRenderer rend, int id, float beginValue = 0, float endValue = 100, float time = 1f)
    {
        time *= .5f;
        await AnimateBlendShape(rend, id, beginValue, endValue, time);
        await AnimateBlendShape(rend, id, endValue, beginValue, time);
    }
}
