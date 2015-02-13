using UnityEditor;
using UnityEngine;

public class CEditorTools : MonoBehaviour
{
    public const string msc_strToolsName = "Tools";

    [MenuItem(msc_strToolsName + "/Clear Prefs")]
    private static void vClearPrefs()
    {
        Debug.Log("Clearing Player Prefs");
        PlayerPrefs.DeleteAll();
    }

	[MenuItem(msc_strToolsName + "/Generate tile sheet json")]
	private static void vGenerateTilesheetJSON()
	{
		if (Selection.objects.Length == 0)
		{
			Debug.LogWarning("No tile sheets have been selected.");
		}

		for (int t_i = 0; t_i < Selection.objects.Length; ++t_i)
		{
			Texture2D t_texture = Selection.objects[t_i] as Texture2D;

			if (t_texture != null)
			{
				Debug.Log("Found texture");

				// assume tile of 32px
				int t_nWidth = t_texture.width;
				int t_nHeight = t_texture.height;

				int t_nTilesWidth = t_nWidth / 32;
				int t_nTilesHeight = t_nHeight / 32;

				float t_rUVWidth = 32 / (float)t_nWidth;
				float t_rUVHeight = 32 / (float)t_nHeight;

				for (int t_y = t_nTilesHeight - 1; t_y >= 0; --t_y)
				{
					for (int t_x = 0; t_x < t_nTilesWidth; ++t_x)
					{
						int t_nTile = (t_y * t_nTilesWidth) + t_x;

						Debug.Log("uv1: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" + 
								  "uv2: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight) + "\n" +
								  "uv3: " + (t_x * t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" +
								  "uv4: " + (t_x * t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight));
					}
				}
			}
		}
	}
}
