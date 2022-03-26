using UnityEngine;

namespace Colocation.Utilities
{
	public static class ImageUtilities
	{
		public static Texture2D DuplicateTexture(Texture2D source)
		{
			var renderTex = RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			var previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			var readableText = new Texture2D(source.width, source.height) { minimumMipmapLevel = 0 };
			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			return readableText;
		}
	}
}
