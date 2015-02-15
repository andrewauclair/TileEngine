using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CMapEditor : EditorWindow
{
	private Texture2D m_tex2dTileset = null;

	private List<Texture2D> m_lstTex2dTiles = new List<Texture2D>();
	private List<List<Texture2D>> m_lstTex2dMap = new List<List<Texture2D>>();
	private List<List<int>> m_lstTileNums = new List<List<int>>();

	private int m_nTileSize = 32;
	private int m_nSelectedTile = 0;
	
	public void InitData()
	{
		Debug.Log("show map editor");

		m_lstTex2dMap.Clear();

		for (int t_x = 0; t_x < 10	; ++t_x)
		{
			m_lstTex2dMap.Add(new List<Texture2D>());
			m_lstTileNums.Add(new List<int>());

			for (int t_y = 0; t_y < 10; ++t_y)
			{
				m_lstTex2dMap[t_x].Add(new Texture2D(m_nTileSize, m_nTileSize, TextureFormat.RGBA32, false));
				m_lstTileNums[t_x].Add(0);

				for (int t_i = 0; t_i < m_nTileSize; ++t_i)
				{
					for (int t_j = 0; t_j < m_nTileSize; ++t_j)
					{
						m_lstTex2dMap[t_x][t_y].SetPixel(t_i, t_j, new Color(0,0,0,0));
					}
				}

				m_lstTex2dMap[t_x][t_y].Apply();
			}
		}
	}
	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(200.0f));

		EditorGUILayout.BeginVertical();

		int t_nID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;
		m_tex2dTileset = (Texture2D)EditorGUILayout.ObjectField(m_tex2dTileset, typeof(Texture2D), true);
		int t_nNewID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;

		m_nTileSize = EditorGUILayout.IntField("Pixel Size", m_nTileSize);//, GUILayout.MaxWidth(80f));
		
		if (GUILayout.Button("Save"))
		{
			if (m_tex2dTileset != null)
			{
				JSONObject t_json = new JSONObject();

				for (int t_j = m_lstTileNums.Count - 1; t_j >= 0; --t_j)
				{
					for (int t_i = 0; t_i < m_lstTileNums[t_j].Count; ++t_i)
					{
						Debug.Log("tile: (" + t_i + "," + t_j + ": " + m_lstTileNums[t_i][t_j]);
						t_json.Add(m_lstTileNums[t_i][t_j]);
					}
				}

				FileStream t_File = File.Create(Application.dataPath + "/Resources/JSON/testmap.bytes");
				CByteStreamWriter t_Writer = new CByteStreamWriter();

				t_Writer.vWriteStr(t_json.ToString(true));

				t_File.Write(t_Writer.ToArray(), 0, t_Writer.nArrayLength());
				t_File.Close();
			}
		}

		if (t_nID != t_nNewID)
		{
			Debug.Log("new texture");

			int t_nTilesWidth = m_tex2dTileset.width / m_nTileSize;
			int t_nTilesHeight = m_tex2dTileset.height / m_nTileSize;

			for (int t_y = t_nTilesHeight - 1; t_y >= 0; --t_y)
			{
				for (int t_x = 0; t_x < t_nTilesWidth; ++t_x)
				{
					int t_nTile = (t_y * t_nTilesWidth) + t_x;

					Texture2D t_tex = new Texture2D(m_nTileSize, m_nTileSize, m_tex2dTileset.format, false);
					t_tex.SetPixels(m_tex2dTileset.GetPixels(t_x * m_nTileSize, t_y * m_nTileSize, m_nTileSize, m_nTileSize));
					t_tex.Apply();
					m_lstTex2dTiles.Add(t_tex);
				}
			}

			// check for tileset json file
		}

		EditorGUILayout.BeginScrollView(Vector2.zero);//, GUILayout.VerticalScrollbar(0f, 4f, 0f, 4f));

		int t_nGridWidth = 4;
		int t_nRow = 0;
		int t_nCol = 0;

		for (int t_i = 0; t_i < m_lstTex2dTiles.Count; ++t_i)
		{
			EditorGUI.DrawTextureTransparent(new Rect(t_nCol * m_nTileSize, t_nRow * m_nTileSize, m_nTileSize, m_nTileSize), m_lstTex2dTiles[t_i]);

			if (GUI.Button(new Rect(t_nCol * m_nTileSize, t_nRow * m_nTileSize, m_nTileSize, m_nTileSize), "", GUIStyle.none))
			{
				m_nSelectedTile = t_i;
			}

			t_nCol++;

			if (t_nCol >= t_nGridWidth - 1)
			{
				t_nCol = 0;
				t_nRow++;
			}
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();


		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();

		for (int t_i = 0; t_i < m_lstTex2dMap.Count; ++t_i)
		{
			for (int t_j = 0; t_j < m_lstTex2dMap[t_i].Count; ++t_j)
			{
				EditorGUI.DrawTextureTransparent(new Rect(250 + t_i * m_nTileSize, 50 + t_j * m_nTileSize, m_nTileSize, m_nTileSize), m_lstTex2dMap[t_i][t_j]);

				if (GUI.Button(new Rect(250 + t_i * m_nTileSize, 50 + t_j * m_nTileSize, m_nTileSize, m_nTileSize), "", GUIStyle.none))
				{
					if (m_tex2dTileset != null)
					{
						m_lstTileNums[t_i][t_j] = m_nSelectedTile;
						m_lstTex2dMap[t_i][t_j].SetPixels(m_lstTex2dTiles[m_nSelectedTile].GetPixels());
						m_lstTex2dMap[t_i][t_j].Apply();
					}
				}
			}
		}

		EditorGUILayout.EndHorizontal();
	}
}
