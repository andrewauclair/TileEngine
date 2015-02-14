using UnityEditor;
using UnityEngine;
using System;
using System.IO;

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

				JSONObject t_JSON = new JSONObject();
				
				for (int t_y = t_nTilesHeight - 1; t_y >= 0; --t_y)
				{
					for (int t_x = 0; t_x < t_nTilesWidth; ++t_x)
					{
						STile t_tile = new STile();

						int t_nTile = (t_y * t_nTilesWidth) + t_x;

						t_tile.m_nTile = t_nTile;

						t_tile.m_uv1 = new Vector2( (t_x * t_rUVWidth) + t_rUVWidth, t_y * t_rUVHeight);
						t_tile.m_uv2 = new Vector2((t_x * t_rUVWidth) + t_rUVWidth, (t_y * t_rUVHeight) + t_rUVHeight);
						t_tile.m_uv3 = new Vector2(t_x * t_rUVWidth, t_y * t_rUVHeight);
						t_tile.m_uv4 = new Vector2(t_x * t_rUVWidth, (t_y * t_rUVHeight) + t_rUVHeight);

						t_JSON.Add(t_tile);

						Debug.Log("uv1: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" + 
								  "uv2: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight) + "\n" +
								  "uv3: " + (t_x * t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" +
								  "uv4: " + (t_x * t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight));
					}
				}

				Debug.Log(t_JSON.ToString());

				FileStream t_File = File.Create(Application.dataPath + "/test.json");
				CByteStreamWriter t_Writer = new CByteStreamWriter();

				t_Writer.vWriteStr(t_JSON.ToString(true));

				t_File.Write(t_Writer.ToArray(), 0, t_Writer.nArrayLength());
				t_File.Close();

				System.IO.File.WriteAllText(Application.dataPath + "/test_readable.json", t_JSON.ToString(true));

				Debug.Log("Saved file to: " + Application.dataPath + "/test.json");
			}
		}
	}
}
