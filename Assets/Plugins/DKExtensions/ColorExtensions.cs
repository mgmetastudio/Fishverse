using UnityEngine;

public static class ColorExtensions
{
    /// <summary>
    ///Returns color with alpha
    ///</summary>
	/// <param name="alpha">alpha</param>
	public static Color WithAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }

    /// <summary>
    ///Returns html string/hex value of the color
    ///</summary>
	public static string Hex(this Color color) => ColorUtility.ToHtmlStringRGBA(color);
}
