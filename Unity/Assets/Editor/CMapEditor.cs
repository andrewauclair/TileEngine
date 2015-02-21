﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CMapEditor : EditorWindow
{
	#region Static Data
	// Tileset size options, defaults to 32
	private static int[] ms_aTileSizeValues = { 16, 32, 64 };
	private static string[] ms_aTileSizeDisplays = { "16", "32", "64" };
	#endregion

	private Texture2D m_tex2dTileset = null;

	private List<List<int>> m_lstTileNums = new List<List<int>>();

	private string[] m_aStrTilesets = new string[0];
	
	private Vector2 m_v2PrevPos = Vector2.zero;
	private Vector2 m_v2ScrollPos = Vector2.zero;
	private Vector2 m_v2TexturePos = Vector2.zero;

	private int m_nTileSize = 32;
	private int m_nSelectedTile = 0;
	private int m_nSelectedTileset = 0;

	private bool m_fEnabled = false;
	private bool m_fShiftDown = false;

	private GameObject m_goPreview = null;

	[MenuItem(CEditorTools.msc_strToolsName + "/Map Editor")]
	private static void Init()
	{
		CMapEditor t_mapEditor = (CMapEditor)EditorWindow.GetWindow(typeof(CMapEditor));
	}

	public CMapEditor()
	{
		SceneView.onSceneGUIDelegate += SceneGUI;
		wantsMouseMove = true;

		if (m_goPreview == null)
		{
			m_goPreview = GameObject.CreatePrimitive(PrimitiveType.Quad);
			m_goPreview.name = "CMapEditor - m_goPreview";
			m_goPreview.renderer.material = CChunkEditorGen.Instance.AtlasMat;

			vUpdatePreviewMesh();
			
			// We don't want the user to select this object, so hide it
			m_goPreview.hideFlags = HideFlags.HideAndDontSave;

			// We default to not editing the map, so hide the preview
			m_goPreview.gameObject.SetActive(false);
		}

		vRefreshTilesetList();
		vGenerateTilesetTexture();
	}
	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= SceneGUI;

		if (m_goPreview != null)
		{
			DestroyImmediate(m_goPreview);
			m_goPreview = null;
		}
	}
	void Update()
	{
		vRefreshTilesetList();
	}
	void OnGUI()
	{
		// Wait for the scene view camera to exist
		if (SceneView.lastActiveSceneView == null)
		{
			return;
		}

		if (!SceneView.lastActiveSceneView.in2DMode)
		{
			EditorGUILayout.LabelField("Please put the scene camera in 2D mode.");
			return;
		}

		bool t_fEnabled = EditorGUILayout.Toggle("Editing", m_fEnabled);

		if (t_fEnabled != m_fEnabled && t_fEnabled)
		{
			m_goPreview.gameObject.SetActive(true);
			Selection.objects = new UnityEngine.Object[0];
		}
		else if (t_fEnabled != m_fEnabled && !t_fEnabled)
		{
			m_goPreview.gameObject.SetActive(false);
		}

		m_fEnabled = t_fEnabled;

		int t_nPrevTileset = m_nSelectedTileset;
		int t_nPrevTileSize = m_nTileSize;

		m_nSelectedTileset = EditorGUILayout.Popup("Tileset", m_nSelectedTileset, m_aStrTilesets);
		m_nTileSize = EditorGUILayout.IntPopup("Tile Size", m_nTileSize, ms_aTileSizeDisplays, ms_aTileSizeValues);


		if (m_nSelectedTileset != t_nPrevTileset)
		{
			// update the tileset texture
			vUpdatePreviewMesh();
		}

		if (m_nTileSize != t_nPrevTileSize)
		{
			// update the tileset texture
			vUpdatePreviewMesh();
		}

		if (m_tex2dTileset != null && position.width + m_nTileSize < m_tex2dTileset.width)
		{
			// update the tileset texture
			vUpdatePreviewMesh();
		}

		m_v2ScrollPos = EditorGUILayout.BeginScrollView(m_v2ScrollPos);

		if (m_tex2dTileset != null)
		{
			EditorGUI.DrawTextureTransparent(new Rect(0, 0, m_tex2dTileset.width, m_tex2dTileset.height), m_tex2dTileset);
		}

		EditorGUILayout.EndScrollView(); 
		
		Rect t_rectLast = GUILayoutUtility.GetLastRect();
		m_v2TexturePos = new Vector2(t_rectLast.x, t_rectLast.y);
		vCheckInput();
	}
	public void SceneGUI(SceneView p_sceneView)
	{
		if (!m_fEnabled)
		{
			return;
		}

		p_sceneView.Repaint();
		
		Tools.current = Tool.View;

		Event t_Event = Event.current;

		if (t_Event.type == EventType.keyDown || t_Event.type == EventType.KeyDown)
		{
			Debug.Log("key down: " + t_Event.keyCode);
		}

		Vector2 t_v2Pos = v2GridPos(t_Event.mousePosition, p_sceneView.camera);

		if (t_Event.type == EventType.MouseMove || t_Event.type == EventType.mouseMove)
		{
			m_goPreview.transform.position = new Vector3(t_v2Pos.x, t_v2Pos.y, 0);
		}

		int t_nControlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

		// Drag to add more of the current tile
		if ((t_Event.type == EventType.MouseDrag || t_Event.type == EventType.mouseDrag) && t_Event.button == 0 && !m_fShiftDown)
		{
			if (!m_fShiftDown)
			{
				if (t_v2Pos != m_v2PrevPos)
				{
					vGenerateCube(t_v2Pos);
				}

				m_v2PrevPos = t_v2Pos;

				t_Event.Use();
			}
		}
		// Drag to delete more tiles
		if ((t_Event.type == EventType.MouseDrag || t_Event.type == EventType.mouseDrag) && t_Event.button == 1 && !m_fShiftDown)
		{
			if (t_v2Pos != m_v2PrevPos)
			{
				vDeleteCube(t_v2Pos);
			}

			m_v2PrevPos = t_v2Pos;

			t_Event.Use();
		}
		// Stop taking control of the GUI input
		if (t_Event.rawType == EventType.MouseUp && GUIUtility.hotControl != 0)
		{
			if (!m_fShiftDown)
			{
				GUIUtility.hotControl = 0;
			}
		}
		// Add a tile in this position
		if ((t_Event.type == EventType.MouseDown || t_Event.type == EventType.mouseDown) && t_Event.button == 0)
		{
			m_fShiftDown = t_Event.shift;

			if (!t_Event.shift)
			{
				GUIUtility.hotControl = t_nControlID;
				// Clamp mouse position to grid position
				if (t_v2Pos != m_v2PrevPos)
				{
					vGenerateCube(t_v2Pos);
				}

				m_v2PrevPos = t_v2Pos;
			}
		}
		// Delete a tile in this position
		if ((t_Event.type == EventType.MouseDown || t_Event.type == EventType.mouseDown) && t_Event.button == 1)
		{
			m_fShiftDown = t_Event.shift;
			Debug.Log("Delete");
			if (!t_Event.shift)
			{
				GUIUtility.hotControl = t_nControlID;

				if (t_v2Pos != m_v2PrevPos)
				{
					vDeleteCube(t_v2Pos);
				}

				m_v2PrevPos = t_v2Pos;
			}
		}
	}
	#region Private Methods
	// Find all the .bytes files for tilesets in Resources/Tilesets
	private void vRefreshTilesetList()
	{
		string[] t_aStrTilesets = Directory.GetFiles(Application.dataPath + "/Resources/Tilesets", "*.bytes");
		string[] t_aStrTextures = Directory.GetFiles(Application.dataPath + "/Art/Tilesets", "*.png");

		if (t_aStrTilesets.Length != m_aStrTilesets.Length)
		{
			for (int t_i = 0; t_i < t_aStrTilesets.Length; ++t_i)
			{
				string[] t_aSplit = t_aStrTilesets[t_i].Split('/', '\\');
				t_aStrTilesets[t_i] = t_aSplit[t_aSplit.Length - 1].Split('.')[0];
			}

			Array.Resize<string>(ref m_aStrTilesets, t_aStrTilesets.Length);
			Array.Copy(t_aStrTilesets, m_aStrTilesets, t_aStrTilesets.Length);
		}
	}
	private void vGenerateTilesetTexture()
	{
		// Find the texture that goes along with the tileset
		byte[] t_aBytes = File.ReadAllBytes(Application.dataPath + "/Art/Tilesets/" + m_aStrTilesets[m_nSelectedTileset] + ".png");
		Texture2D t_texture = new Texture2D(1,1);

		if (t_texture.LoadImage(t_aBytes))
		{
			m_tex2dTileset = t_texture;
			m_tex2dTileset.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	private void vCheckInput()
	{
		Event t_Event = Event.current;
		bool t_fMouseDown = false;
		bool t_fMouseUp = false;

		if (t_Event.type == EventType.MouseDown)
		{
			t_fMouseDown = true;
		}
		else if (t_Event.type == EventType.MouseUp)
		{
			t_fMouseUp = true;
		}

		vCheckTileInput(t_fMouseDown, t_fMouseUp, t_Event.mousePosition);
	}
	private void vCheckTileInput(bool p_fMouseDown, bool p_fMouseUp, Vector2 p_v2MousePos)
	{
		if (p_fMouseDown)
		{
			int p_xTile = Mathf.FloorToInt((p_v2MousePos.x - m_v2TexturePos.x) / (float)m_nTileSize);
			int p_yTile = Mathf.FloorToInt((p_v2MousePos.y - m_v2TexturePos.y) / (float)m_nTileSize);

			int p_nTile = (p_yTile * (m_tex2dTileset.width / m_nTileSize)) + p_xTile;
			m_nSelectedTile = p_nTile;
			vUpdatePreviewMesh();
		}
	}
	private Vector2 v2GridPos(Vector2 p_v2MousePos, Camera p_camera)
	{
		// REF: http://forum.unity3d.com/threads/screenpointtoray-with-event-mouseposition.119584/
		Vector2 t_v2Pos = p_camera.ScreenToWorldPoint(new Vector3(p_v2MousePos.x, p_camera.pixelHeight - p_v2MousePos.y, 0));
		
		t_v2Pos.x = Mathf.RoundToInt(t_v2Pos.x);
		t_v2Pos.y = Mathf.RoundToInt(t_v2Pos.y);

		return t_v2Pos;
	}
	private void vGenerateCube(Vector2 p_v2Pos)
	{
		CTile t_tile = new CTile();
		t_tile.Tile = m_nSelectedTile;
		CChunkEditorGen.Instance.vAddTile(t_tile, p_v2Pos);
	}
	private void vDeleteCube(Vector2 p_v2Pos)
	{
		CTile t_tile = new CTile();
		t_tile.Tile = -1;
		CChunkEditorGen.Instance.vAddTile(t_tile, p_v2Pos);
	}
	private void vUpdatePreviewMesh()
	{
		Vector3[] t_aVertices = new Vector3[4];
		Vector2[] t_aUVs = new Vector2[4];
		int[] t_aTriangles = new int[6];

		t_aVertices[0] = new Vector3(-.5f, .5f, 0);
		t_aVertices[1] = new Vector3(.5f, .5f, 0);
		t_aVertices[2] = new Vector3(-.5f, -.5f, 0);
		t_aVertices[3] = new Vector3(.5f, -.5f, 0);

		CTile t_Tile = CChunkEditorGen.Instance.lstTiles[m_nSelectedTile];
		MeshFilter t_meshFilter = m_goPreview.GetComponent<MeshFilter>();

		t_aUVs[0] = t_Tile.UV3;
		t_aUVs[1] = t_Tile.UV2;
		t_aUVs[2] = t_Tile.UV1;
		t_aUVs[3] = t_Tile.UV4;

		t_aTriangles[0] = 0;
		t_aTriangles[1] = 1;
		t_aTriangles[2] = 2;
		t_aTriangles[3] = 2;
		t_aTriangles[4] = 1;
		t_aTriangles[5] = 3;

		t_meshFilter.sharedMesh.uv = t_aUVs;
		t_meshFilter.sharedMesh.RecalculateNormals();
	}
	#endregion
}
