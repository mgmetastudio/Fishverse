using UnityEngine;

public static class TextureExtensions
{
    /// <summary>Resizes Texture</summary>
	/// <param name="newWidth">New width</param>
	/// <param name="newHeight">New height</param>
	public static Texture2D TextureResize(this Texture2D sourceTexture, int newWidth, int newHeight)
    {
        // Crop
        var startWidth = sourceTexture.width;
        var startHeight = sourceTexture.height;

        var aspectRatio = ((float)newWidth) / newHeight;

        var preferedWidth = 0;
        var preferedHeight = 0;

        var xOffset = 0;
        var yOffset = 0;

        if ((aspectRatio * startHeight) < startWidth)
        {
            preferedWidth = (int)(startHeight * aspectRatio);
            preferedHeight = startHeight;
            xOffset = (startWidth - preferedWidth) / 2;
        }
        else
        {
            preferedHeight = (int)(startWidth / aspectRatio);
            preferedWidth = startWidth;
            yOffset = (startHeight - preferedHeight) / 2;
        }

        var pixels = sourceTexture.GetPixels(xOffset, yOffset, preferedWidth, preferedHeight, 0);
        var _tex = new Texture2D(preferedWidth, preferedHeight);
        _tex.SetPixels(pixels);
        _tex.Apply();

        // Resize
        _tex.filterMode = FilterMode.Point;

        var renderTexture = RenderTexture.GetTemporary(newWidth, newHeight);
        renderTexture.filterMode = FilterMode.Point;

        RenderTexture.active = renderTexture;
        Graphics.Blit(_tex, renderTexture);

        var newTexture = new Texture2D(newWidth, newHeight);
        newTexture.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        newTexture.Apply();

        RenderTexture.active = null;
        return newTexture;
    }

    /// <summary>Returns readable texture of the sprite</summary>
    public static Texture2D CreateReadableTexture(this Sprite sprite)
    {
        var source = sprite.texture;
        var renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear
        );
        Graphics.Blit(source, renderTex);

        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;

        var readableTex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        readableTex.ReadPixels(sprite.rect, 0, 0);
        readableTex.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableTex;
    }

    public static Texture2D ToTexture2D(this RenderTexture rendTex)
    {
        Texture2D tex = new Texture2D(rendTex.width, rendTex.height, TextureFormat.RGBAFloat, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = rendTex;

        tex.ReadPixels(new Rect(0, 0, rendTex.width, rendTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }
}
