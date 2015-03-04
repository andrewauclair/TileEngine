using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CMapEditor : EditorWindow
{
	#region Static Data
	public static CMapEditor Instance = null;
	#endregion

	#region Private Data
	private Texture2D m_tex2dTileset = null;
	private Texture2D m_tex2dHighlight = null;

	private string[] m_aStrTilesets = new string[0];
	private CLayer[] m_aLayers = new CLayer[0];
	private string[] m_aStrLayers = new string[0];

	private Vector2 m_v2PrevPos = Vector2.zero;
	private Vector2 m_v2ScrollPos = Vector2.zero;
	private Vector2 m_v2TexturePos = Vector2.zero;

	private int m_nTileSize = 32;
	private int m_nSelectedTile = 0;
	private int m_nSelectedTileset = 0;
	private int m_nSelectedLayer = 0;
	private int m_nLayerMask = -1;

	private bool m_fEnabled = false;
	private bool m_fShiftDown = false;
	private bool m_fCompiling = false;
	private bool m_fIsPlaying = false;

	private string m_strCurrentScene = "";

	private GameObject m_goPreview = null;
	private GameObject m_goGenerator = null;
	#endregion

	[MenuItem(CEditorTools.msc_strToolsName + "/Map Editor")]
	private static void Init()
	{
		EditorWindow.GetWindow(typeof(CMapEditor));
	}

	public CMapEditor()
	{
		Instance = this;

		SceneView.onSceneGUIDelegate += SceneGUI;
		wantsMouseMove = true;
	}
	void Awake()
	{
		m_strCurrentScene = EditorApplication.currentScene;

		vCreateChunkGenerator();

		vRefreshTilesetList();
		vRefreshLayerList();
		vGenerateTilesetTexture();

		CChunkGenerator.Instance.vSetTileset(m_aStrTilesets[0]);

		vUpdatePreviewMesh();

		// We default to not editing the map, so hide the preview
		m_goPreview.gameObject.SetActive(false);
	}
	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= SceneGUI;

		if (m_goPreview != null)
		{
			DestroyImmediate(m_goPreview);
			m_goPreview = null;
		}

		if (m_goGenerator != null)
		{
			DestroyImmediate(m_goGenerator);
			m_goGenerator = null;
		}

		Tools.current = Tool.None;
		Tools.visibleLayers = -1;
	}
	void OnApplicationQuit()
	{
		OnDestroy();
	}
	void Update()
	{
		vRefreshTilesetList();
		vRefreshLayerList();

		if (EditorApplication.isCompiling)
		{
			m_fCompiling = true;
		}

		if (EditorApplication.isPlaying)
		{
			m_fIsPlaying = true;
		}

		if (!EditorApplication.isCompiling && m_fCompiling)
		{
			Debug.Log("Finished Recompiling");
			m_fCompiling = false;
			vCreateChunkGenerator();
			m_goGenerator.GetComponent<CChunkGenerator>().vSetTileset(m_aStrTilesets[m_nSelectedTileset]);
		}

		if (!EditorApplication.isPlaying && !EditorApplication.isPaused && m_fIsPlaying)
		{
			Debug.Log("Finished playing");
			Repaint();
			m_fIsPlaying = false;
			vCreateChunkGenerator();
			m_goGenerator.GetComponent<CChunkGenerator>().vSetTileset(m_aStrTilesets[m_nSelectedTileset]);
		}

		if (UnityEditor.EditorApplication.currentScene != m_strCurrentScene)
		{
			Debug.Log("Switched Scenes");
			m_strCurrentScene = EditorApplication.currentScene;
			vCreateChunkGenerator();
			m_goGenerator.GetComponent<CChunkGenerator>().vSetTileset(m_aStrTilesets[m_nSelectedTileset]);
		}
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

		if (t_fEnabled != m_fEnabled)
		{
			m_goPreview.gameObject.SetActive(t_fEnabled);
			Selection.objects = new UnityEngine.Object[0];
			Tools.current = t_fEnabled ? Tool.View : Tool.None;
			Tools.visibleLayers = t_fEnabled ? 1 : -1;
		}

		m_fEnabled = t_fEnabled;

		int t_nPrevTileset = m_nSelectedTileset;
		int t_nPrevMask = m_nLayerMask;

		m_nSelectedTileset = EditorGUILayout.Popup("Tileset", m_nSelectedTileset, m_aStrTilesets);
		m_nLayerMask = EditorGUILayout.MaskField("Display Layers", m_nLayerMask, m_aStrLayers);
		m_nSelectedLayer = EditorGUILayout.Popup("Edit Layer", m_nSelectedLayer, m_aStrLayers);

		if (m_nSelectedTileset != t_nPrevTileset)
		{
			m_nSelectedTile = 0;
			
			// load the material for this tileset
			CChunkGenerator.Instance.vSetTileset(m_aStrTilesets[m_nSelectedTileset]);

			// update the tileset texture
			vGenerateTilesetTexture();
			vUpdatePreviewMesh();
		}

		if (m_tex2dTileset != null && position.width + m_nTileSize < m_tex2dTileset.width)
		{
			// update the tileset texture
			vUpdateTilesetTexture();
		}
		else if (m_tex2dTileset != null && position.width - m_nTileSize > m_tex2dTileset.width)
		{
			vUpdateTilesetTexture();
		}

		if (m_nLayerMask != t_nPrevMask)
		{
			for (int t_i = 0; t_i < m_aStrLayers.Length; ++t_i)
			{
				CChunkGenerator.Instance.vSetLayerActive(t_i, ((1 << t_i) & m_nLayerMask) > 0);
			}
		}

		m_v2ScrollPos = EditorGUILayout.BeginScrollView(m_v2ScrollPos, GUILayout.ExpandHeight(true));
		EditorGUILayout.BeginVertical();
		
		if (m_tex2dTileset != null)
		{
			Rect t_rect = GUILayoutUtility.GetRect(m_tex2dTileset.width, m_tex2dTileset.height);
			t_rect.width = m_tex2dTileset.width;
			t_rect.height = m_tex2dTileset.height;

			EditorGUI.DrawTextureTransparent(t_rect, m_tex2dTileset);
		}

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();

		Rect t_rectLast = GUILayoutUtility.GetLastRect();
		m_v2TexturePos = new Vector2(t_rectLast.x, t_rectLast.y);
		vCheckInput();
	}
	public void SceneGUI(SceneView p_sceneView)
	{
		p_sceneView.Repaint();
		
		if (!m_fEnabled)
		{
			return;
		}

		Tools.current = Tool.View;

		Event t_Event = Event.current;

		Vector2 t_v2Pos = v2GridPos(t_Event.mousePosition, p_sceneView.camera);

		if (t_Event.type == EventType.MouseMove)
		{
			m_goPreview.transform.position = new Vector3(t_v2Pos.x, t_v2Pos.y, (-.5f * m_nSelectedLayer) - .25f);
		}

		int t_nControlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

		// Drag to add more of the current tile
		if (t_Event.type == EventType.MouseDrag && t_Event.button == 0 && !m_fShiftDown)
		{
			if (!m_fShiftDown)
			{
				if (t_v2Pos != m_v2PrevPos)
				{
					vAddTile(t_v2Pos);
				}

				m_v2PrevPos = t_v2Pos;

				t_Event.Use();
			}
		}
		// Drag to delete more tiles
		if (t_Event.type == EventType.MouseDrag && t_Event.button == 1 && !m_fShiftDown)
		{
			if (t_v2Pos != m_v2PrevPos)
			{
				vRemoveTile(t_v2Pos);
			}

			m_v2PrevPos = t_v2Pos;

			t_Event.Use();
		}
		// Stop taking control of the GUI input
		if (t_Event.rawType == EventType.MouseUp && GUIUtility.hotControl != 0 && (t_Event.button == 0 || t_Event.button == 1))
		{
			m_goPreview.SetActive(true);

			if (!m_fShiftDown)
			{
				GUIUtility.hotControl = 0;
			}
		}
		// Add a tile in this position
		if (t_Event.type == EventType.MouseDown && t_Event.button == 0)
		{
			m_fShiftDown = t_Event.shift;

			if (!t_Event.shift)
			{
				GUIUtility.hotControl = t_nControlID;
				vAddTile(t_v2Pos);
				m_v2PrevPos = t_v2Pos;
			}
		}
		// Delete a tile in this position
		if (t_Event.type == EventType.MouseDown && t_Event.button == 1)
		{
			m_goPreview.SetActive(false);

			m_fShiftDown = t_Event.shift;

			if (!t_Event.shift)
			{
				GUIUtility.hotControl = t_nControlID;
				vRemoveTile(t_v2Pos);
				m_v2PrevPos = t_v2Pos;
			}
		}
	}
	#region Private Methods
	// Find all the .bytes files for tilesets in Resources/Tilesets
	private void vRefreshTilesetList()
	{
		string[] t_aStrTilesets = Directory.GetFiles(Application.dataPath + "/Resources/Tilesets", "*.bytes");
		//string[] t_aStrTextures = Directory.GetFiles(Application.dataPath + "/Art/Tilesets", "*.png");

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
	private void vRefreshLayerList()
	{
		CLayer[] t_aLayers = CChunkGenerator.Instance.lstLayers.ToArray();

		if (t_aLayers.Length != m_aLayers.Length)
		{
			Array.Resize<CLayer>(ref m_aLayers, t_aLayers.Length);
			Array.Resize<string>(ref m_aStrLayers, t_aLayers.Length);
			Array.Copy(t_aLayers, m_aLayers, t_aLayers.Length);

			for (int t_i = 0; t_i < m_aStrLayers.Length; ++t_i)
			{
				m_aStrLayers[t_i] = t_aLayers[t_i].m_strName;
			}
		}
	}
	private void vGenerateTilesetTexture()
	{
		// Find the texture that goes along with the tileset
		byte[] t_aBytes = File.ReadAllBytes(Application.dataPath + "/Art/Tilesets/" + m_aStrTilesets[m_nSelectedTileset] + ".png");
		Texture2D t_texture = new Texture2D(1,1);

		if (t_texture.LoadImage(t_aBytes))
		{
			if (m_tex2dTileset != null)
			{
				DestroyImmediate(m_tex2dTileset);
				m_tex2dTileset = null;
			}

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

			if (p_v2MousePos.x - m_v2TexturePos.x > m_tex2dTileset.width ||
				p_v2MousePos.y - m_v2TexturePos.y > m_tex2dTileset.height)
			{
				return;
			}
			m_nSelectedTile = p_nTile;
			Debug.Log("selected tile: " + p_nTile);
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
	private void vAddTile(Vector2 p_v2Pos)
	{
		CTile t_tile = new CTile();
		t_tile.Tile = m_nSelectedTile;
		CChunkGenerator.Instance.vAddTile(t_tile, p_v2Pos, m_nSelectedLayer);
	}
	private void vRemoveTile(Vector2 p_v2Pos)
	{
		CTile t_tile = new CTile();
		t_tile.Tile = -1;
		CChunkGenerator.Instance.vAddTile(t_tile, p_v2Pos, m_nSelectedLayer);
	}
	private void vUpdatePreviewMesh()
	{
		if (m_goPreview == null)
		{
			m_goPreview = GameObject.CreatePrimitive(PrimitiveType.Quad);
			m_goPreview.name = "CMapEditor - m_goPreview";
			m_goPreview.GetComponent<Renderer>().material = CChunkGenerator.Instance.AtlasMat;

			m_goPreview.AddComponent<CPreviewHighlight>();

			// We don't want the user to select this object, so hide it
			m_goPreview.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
		}

		Vector2[] t_aUVs = new Vector2[4];

		CTile t_Tile = CChunkGenerator.Instance.lstTiles[m_nSelectedTile];
		MeshFilter t_meshFilter = m_goPreview.GetComponent<MeshFilter>();

		t_aUVs[0] = t_Tile.UV1;
		t_aUVs[1] = t_Tile.UV2;
		t_aUVs[2] = t_Tile.UV3;
		t_aUVs[3] = t_Tile.UV4;

		t_meshFilter.sharedMesh.uv = t_aUVs;
		t_meshFilter.sharedMesh.RecalculateNormals();

		m_goPreview.GetComponent<Renderer>().material = CChunkGenerator.Instance.AtlasMat;
	}
	private void vCreateChunkGenerator()
	{
		if (m_goGenerator != null)
		{
			DestroyImmediate(m_goGenerator);
			m_goGenerator = null;
		}

		m_goGenerator = new GameObject();
		m_goGenerator.name = "CChunkGenerator";

		m_goGenerator.hideFlags = HideFlags.HideAndDontSave;

		m_goGenerator.AddComponent<CChunkGenerator>();
	}
	private void vUpdateTilesetTexture()
	{
		vGenerateTilesetTexture();

		int t_nTexWidth = m_tex2dTileset.width;
		int t_nTexHeight = m_tex2dTileset.height;
		
		int t_nTiles = (t_nTexWidth / m_nTileSize) * (t_nTexHeight / m_nTileSize);

		int t_nEditorWidth = Mathf.RoundToInt(position.width);
		
		int t_nTileWidth = Mathf.FloorToInt(t_nEditorWidth / m_nTileSize) - 1;
		int t_nTileHeight = Mathf.CeilToInt(t_nTiles / (float)t_nTileWidth);

		if (t_nTileWidth * m_nTileSize > m_tex2dTileset.width)
		{
			return;
		}
		Texture2D t_texNew = new Texture2D(t_nTileWidth * m_nTileSize, t_nTileHeight * m_nTileSize);

		int t_xOld = 0;
		int t_yOld = t_nTexHeight - m_nTileSize;
		int t_xNew = 0;
		int t_yNew = (t_nTileHeight * m_nTileSize) - m_nTileSize;

		// copy the tile data from the old texture to the new one
		for (int t_iTile = 0; t_iTile < t_nTiles; ++t_iTile)
		{
			Color[] t_aColors = m_tex2dTileset.GetPixels(t_xOld, t_yOld, m_nTileSize, m_nTileSize);

			t_texNew.SetPixels(t_xNew, t_yNew, m_nTileSize, m_nTileSize, t_aColors);

			t_xOld += m_nTileSize;
			t_xNew += m_nTileSize;

			if (t_xOld >= t_nTexWidth)
			{
				t_yOld -= m_nTileSize;
				t_xOld = 0;
			}

			if (t_xNew >= t_nTileWidth * m_nTileSize)
			{
				t_yNew -= m_nTileSize;
				t_xNew = 0;
			}
		}

		t_texNew.Apply();
		
		DestroyImmediate(m_tex2dTileset);
		m_tex2dTileset = null;

		// set to new texture
		m_tex2dTileset = t_texNew;
		m_tex2dTileset.hideFlags = HideFlags.HideAndDontSave;
	}
	#endregion
}
