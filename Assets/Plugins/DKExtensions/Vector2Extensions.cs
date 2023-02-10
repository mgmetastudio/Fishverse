using UnityEngine;

public static class Vector2Extensions
{
    ///<summary>
    ///Converts Vector2 to Vector3 where x = v2.x, y = 0, z = v2.y
    ///</summary>
    public static Vector3 VectorYtoZ(this Vector2 vector2)
    {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    ///<summary>
    ///Return random value between vector2.x and vector2.y
    ///</summary>
    public static float GetRandom(this Vector2 vector2)
    {
        return Random.Range(vector2.x, vector2.y);
    }

    ///<summary>
    ///Return random value between vector2.x and vector2.y
    ///</summary>
    public static int GetRandom(this Vector2Int vector2)
    {
        return Random.Range(vector2.x, vector2.y);
    }

    ///<summary>
    ///Return lerped value between vector2.x and vector2.y by t
    ///</summary>
    public static float Lerp(this Vector2 vector2, float t)
    {
        return Mathf.Lerp(vector2.x, vector2.y, t);
    }

    /// <summary>Return absolute vector</summary>
	public static Vector2 Abs(this Vector2 vector)
	{
		var absVector = new Vector2();
		absVector.x = Mathf.Abs(vector.x);
		absVector.y = Mathf.Abs(vector.y);
		return absVector;
	}

	/// <summary>Floor to int</summary>
	public static Vector2Int FloorToInt(this Vector2 vector)
	{
		var v2i = new Vector2Int();
		v2i.x = Mathf.FloorToInt(vector.x);
		v2i.y = Mathf.FloorToInt(vector.y);
		return v2i;
	}
	
	/// <summary>Ceil to int</summary>
	public static Vector2Int CeilToInt(this Vector2 vector)
	{
		var v2i = new Vector2Int();
		v2i.x = Mathf.CeilToInt(vector.x);
		v2i.y = Mathf.CeilToInt(vector.y);
		return v2i;
	}
	
	/// <summary>Round vector to int</summary>
	public static Vector2Int RoundToInt(this Vector2 vector)
	{
		var v2i = new Vector2Int();
		v2i.x = Mathf.RoundToInt(vector.x);
		v2i.y = Mathf.RoundToInt(vector.y);
		return v2i;
	}

	/// <summary>Returns same vector with changed y value</summary>
    public static Vector2 WithX(this Vector2 vector, float x)
    {
        return new Vector2(x, vector.y);
    }

    /// <summary>Returns same vector with changed y value</summary>
    public static Vector2 WithY(this Vector2 vector, float y)
    {
        return new Vector2(vector.x, y);
    }
}
