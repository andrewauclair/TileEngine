using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class CEditorTools : MonoBehaviour
{
    public const string msc_strToolsName = "Tools";

    [MenuItem(msc_strToolsName + "/Clear Prefs")]
    private static void vClearPrefs()
    {
        Debug.Log("Clearing Player Prefs");
        PlayerPrefs.DeleteAll();
    }

	[MenuItem(msc_strToolsName + "/Map Editor")]
	private static void vOpenMapEditor()
	{
		EditorWindow t_window = EditorWindow.GetWindow(typeof(CMapEditor));
		
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

				List<CTile> t_lstTiles = new List<CTile>();

				for (int t_y = t_nTilesHeight - 1; t_y >= 0; --t_y)
				{
					for (int t_x = 0; t_x < t_nTilesWidth; ++t_x)
					{
						CTile t_tile = new CTile();

						int t_nTile = ((t_nTilesHeight - 1 - t_y) * t_nTilesWidth) + t_x;

						t_tile.Tile = t_nTile;

						t_tile.UV1 = new Vector2( (t_x * t_rUVWidth) + t_rUVWidth, t_y * t_rUVHeight);
						t_tile.UV2 = new Vector2((t_x * t_rUVWidth) + t_rUVWidth, (t_y * t_rUVHeight) + t_rUVHeight);
						t_tile.UV3 = new Vector2(t_x * t_rUVWidth, t_y * t_rUVHeight);
						t_tile.UV4 = new Vector2(t_x * t_rUVWidth, (t_y * t_rUVHeight) + t_rUVHeight);

						Debug.Log("uv1: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" + 
								  "uv2: " + ((t_x * t_rUVWidth) + t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight) + "\n" +
								  "uv3: " + (t_x * t_rUVWidth) + ", " + (t_y * t_rUVHeight) + "\n" +
								  "uv4: " + (t_x * t_rUVWidth) + ", " + ((t_y * t_rUVHeight) + t_rUVHeight));

						t_lstTiles.Add(t_tile);
					}
				}

				t_lstTiles.Sort((CTile p_t1, CTile p_t2) =>
					{
						return p_t1.Tile.CompareTo(p_t2.Tile);
					});

				foreach (CTile t_Tile in t_lstTiles)
				{
					t_JSON.Add(t_Tile.ToJSON());
				}

				Debug.Log(t_JSON.ToString());

				FileStream t_File = File.Create(Application.dataPath + "/" + t_texture.name + ".json");
				CByteStreamWriter t_Writer = new CByteStreamWriter();

				t_Writer.vWriteStr(t_JSON.ToString(true));

				t_File.Write(t_Writer.ToArray(), 0, t_Writer.nArrayLength());
				t_File.Close();

				System.IO.File.WriteAllText(Application.dataPath + "/" + t_texture.name + "_readable.json", t_JSON.ToString(true));

				Debug.Log("Saved file to: " + Application.dataPath + "/test.json");
			}
		}
	}
}
