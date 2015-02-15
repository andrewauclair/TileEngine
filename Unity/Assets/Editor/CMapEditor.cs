using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CMapEditor : EditorWindow
{
	private Texture2D m_tex2dTileset = null;

	private List<Texture2D> m_lstTex2dTiles = new List<Texture2D>();

	private int m_nTileSize = 32;

	void Show()
	{
	}
	void OnGUI()
	{
		EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(150.0f));

		EditorGUILayout.BeginVertical();

		int t_nID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;
		m_tex2dTileset = (Texture2D)EditorGUILayout.ObjectField(m_tex2dTileset, typeof(Texture2D), true);
		int t_nNewID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;

		m_nTileSize = EditorGUILayout.IntField("Pixel Size", m_nTileSize);//, GUILayout.MaxWidth(80f));
		
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

		EditorGUILayout.BeginScrollView(Vector2.zero);

		int t_nGridWidth = 4;
		int t_nRow = 0;
		int t_nCol = 0;

		for (int t_i = 0; t_i < m_lstTex2dTiles.Count; ++t_i)
		{
			EditorGUI.DrawTextureTransparent(new Rect(t_nCol * m_nTileSize, t_nRow * m_nTileSize, m_nTileSize, m_nTileSize), m_lstTex2dTiles[t_i]);
			t_nCol++;

			if (t_nCol >= t_nGridWidth - 1)
			{
				t_nCol = 0;
				t_nRow++;
			}
		}

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical();

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndHorizontal();
	}
}
