using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>Returns RectTransform</summary>
	public static RectTransform GetRectTransform(this Component component)
	{
		return component.GetComponent<RectTransform>();
	}

    /// <summary>Sets offset from left</summary>
	/// <param name="left">Offset</param>
	public static RectTransform SetLeftPosition(this RectTransform rectTransform, float left)
	{
		rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
		return rectTransform;
	}

	/// <summary>Sets offset from right</summary>
	/// <param name="right">Offset</param>
	public static RectTransform SetRightPosition(this RectTransform rectTransform, float right)
	{
		rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
		return rectTransform;
	}

	/// <summary>Sets offset from top</summary>
	/// <param name="top">Offset</param>
	public static RectTransform SetTopPosition(this RectTransform rectTransform, float top)
	{
		rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
		return rectTransform;
	}

	/// <summary>Sets offset from bottom</summary>
	/// <param name="bottom">Offset</param>
	public static RectTransform SetBottomPosition(this RectTransform rectTransform, float bottom)
	{
		rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
		return rectTransform;
	}

    public static void SetDefaultScale(this RectTransform trans)
    {
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
    {
        trans.pivot = aVec;
        trans.anchorMin = aVec;
        trans.anchorMax = aVec;
    }

    public static Vector2 GetSize(this RectTransform trans)
    {
        return trans.rect.size;
    }

    public static float GetWidth(this RectTransform trans)
    {
        return trans.rect.width;
    }

    public static float GetHeight(this RectTransform trans)
    {
        return trans.rect.height;
    }

    public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }

    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }

    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
    {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSize(this RectTransform trans, Vector2 newSize)
    {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }

    public static void SetWidth(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(newSize, trans.rect.size.y));
    }

    public static void SetHeight(this RectTransform trans, float newSize)
    {
        SetSize(trans, new Vector2(trans.rect.size.x, newSize));
    }

}