using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CMapEditor : EditorWindow
{
	#region Static Data
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

	[MenuItem(CEditorTools.msc_strToolsName + "/Map Editor")]
	private static void Init()
	{
		CMapEditor t_mapEditor = (CMapEditor)EditorWindow.GetWindow(typeof(CMapEditor));
	}

	public CMapEditor()
	{
		SceneView.onSceneGUIDelegate += SceneGUI;

		vRefreshTilesetList();
		vGenerateTilesetTexture();
	}
	public void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= SceneGUI;
	}
	void Update()
	{
		vRefreshTilesetList();
	}
	void OnGUI()
	{
		if (!SceneView.lastActiveSceneView.in2DMode)
		{
			EditorGUILayout.LabelField("Please put the scene camera in 2D mode.");
			return;
		}

		bool t_fEnabled = EditorGUILayout.Toggle("Editing", m_fEnabled);

		if (t_fEnabled != m_fEnabled && t_fEnabled)
		{
			Selection.objects = new UnityEngine.Object[0];
		}

		m_fEnabled = t_fEnabled;

		

		//EditorGUILayout.BeginHorizontal(GUILayout.Width(250.0f), GUILayout.Height(750.0f));

		//EditorGUILayout.BeginHorizontal();

		//EditorGUILayout.BeginVertical();

		//List<int> t_aInts = new List<int>();
		//List<string> t_aNames = new List<string>();
		//for (int t_i = 0; t_i < 60; ++t_i)
		//{
		//    t_aInts.Add(t_i);
		//    t_aNames.Add(t_i.ToString());
		//}

		int t_nPrevTileset = m_nSelectedTileset;
		int t_nPrevTileSize = m_nTileSize;

		m_nSelectedTileset = EditorGUILayout.Popup("Tileset", m_nSelectedTileset, m_aStrTilesets);
		m_nTileSize = EditorGUILayout.IntPopup("Tile Size", m_nTileSize, ms_aTileSizeDisplays, ms_aTileSizeValues);


		if (m_nSelectedTileset != t_nPrevTileset)
		{
			// update the tileset texture
		}

		if (m_nTileSize != t_nPrevTileSize)
		{
			// update the tileset texture
		}

		if (m_tex2dTileset != null && position.width + m_nTileSize < m_tex2dTileset.width)
		{
			// update the tileset texture
		}

		//EditorGUILayout.IntPopup("test: ", 0, t_aNames.ToArray(), t_aInts.ToArray());

		//int t_nID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;
		//m_tex2dTileset = (Texture2D)EditorGUILayout.ObjectField(m_tex2dTileset, typeof(Texture2D), true);
		//int t_nNewID = m_tex2dTileset != null ? m_tex2dTileset.GetInstanceID() : -1;

		
		//if (GUILayout.Button("Save"))
		//{
		//    if (m_tex2dTileset != null)
		//    {
		//        JSONObject t_json = new JSONObject();

		//        for (int t_j = m_lstTileNums.Count - 1; t_j >= 0; --t_j)
		//        {
		//            for (int t_i = 0; t_i < m_lstTileNums[t_j].Count; ++t_i)
		//            {
		//                Debug.Log("tile: (" + t_i + "," + t_j + ": " + m_lstTileNums[t_i][t_j]);
		//                t_json.Add(m_lstTileNums[t_i][t_j]);
		//            }
		//        }

		//        FileStream t_File = File.Create(Application.dataPath + "/Resources/JSON/testmap.bytes");
		//        CByteStreamWriter t_Writer = new CByteStreamWriter();

		//        t_Writer.vWriteStr(t_json.ToString(true));

		//        t_File.Write(t_Writer.ToArray(), 0, t_Writer.nArrayLength());
		//        t_File.Close();

		//        AssetDatabase.Refresh();
		//    }
		//}

		//if (t_nID != t_nNewID)
		//{
		//    Debug.Log("new texture");
		//    PlayerPrefs.SetInt("MapEditorTextureID", t_nNewID);

		//    int t_nTilesWidth = m_tex2dTileset.width / m_nTileSize;
		//    int t_nTilesHeight = m_tex2dTileset.height / m_nTileSize;

		//    for (int t_y = t_nTilesHeight - 1; t_y >= 0; --t_y)
		//    {
		//        for (int t_x = 0; t_x < t_nTilesWidth; ++t_x)
		//        {
		//            int t_nTile = (t_y * t_nTilesWidth) + t_x;

		//            Texture2D t_tex = new Texture2D(m_nTileSize, m_nTileSize, m_tex2dTileset.format, false);
		//            t_tex.SetPixels(m_tex2dTileset.GetPixels(t_x * m_nTileSize, t_y * m_nTileSize, m_nTileSize, m_nTileSize));
		//            t_tex.Apply();
		//            m_lstTex2dTiles.Add(t_tex);
		//        }
		//    }

		//    // check for tileset json file
		//}

		//EditorGUILayout.BeginScrollView(Vector2.zero);//, GUILayout.VerticalScrollbar(0f, 4f, 0f, 4f));

		//int t_nGridWidth = 4;
		//int t_nRow = 0;
		//int t_nCol = 0;

		//for (int t_i = 0; t_i < m_lstTex2dTiles.Count; ++t_i)
		//{
		//    EditorGUI.DrawTextureTransparent(new Rect(t_nCol * m_nTileSize, t_nRow * m_nTileSize, m_nTileSize, m_nTileSize), m_lstTex2dTiles[t_i]);

		//    if (GUI.Button(new Rect(t_nCol * m_nTileSize, t_nRow * m_nTileSize, m_nTileSize, m_nTileSize), "", GUIStyle.none))
		//    {
		//        m_nSelectedTile = t_i;
		//    }

		//    t_nCol++;

		//    if (t_nCol >= t_nGridWidth - 1)
		//    {
		//        t_nCol = 0;
		//        t_nRow++;
		//    }
		//}

		//EditorGUILayout.BeginHorizontal();

		m_v2ScrollPos = EditorGUILayout.BeginScrollView(m_v2ScrollPos);

		if (m_tex2dTileset != null)
		{
			EditorGUI.DrawTextureTransparent(new Rect(0, 0, m_tex2dTileset.width, m_tex2dTileset.height), m_tex2dTileset);
		}

		//for (int t_i = 0; t_i < m_lstTex2dMap.Count; ++t_i)
		//{
		//    for (int t_j = 0; t_j < m_lstTex2dMap[t_i].Count; ++t_j)
		//    {
		//        EditorGUI.DrawTextureTransparent(new Rect(t_i * m_nTileSize, t_j * m_nTileSize, m_nTileSize * 30, m_nTileSize * 30), m_lstTex2dMap[t_i][t_j]);

		//        //if (GUI.Button(new Rect(250 + t_i * m_nTileSize, 50 + t_j * m_nTileSize, m_nTileSize, m_nTileSize), "", GUIStyle.none))
		//        //{
		//        //    if (m_tex2dTileset != null)
		//        //    {
		//        //        m_lstTileNums[t_i][t_j] = m_nSelectedTile;
		//        //        m_lstTex2dMap[t_i][t_j].SetPixels(m_lstTex2dTiles[m_nSelectedTile].GetPixels());
		//        //        m_lstTex2dMap[t_i][t_j].Apply();
		//        //    }
		//        //}
		//    }
		//}
		EditorGUILayout.EndScrollView(); 
		
		Rect t_rectLast = GUILayoutUtility.GetLastRect();
		m_v2TexturePos = new Vector2(t_rectLast.x, t_rectLast.y);
		vCheckInput();
		//m_nTileSize = EditorGUILayout.IntField("Pixel Size", m_nTileSize);//, GUILayout.MaxWidth(80f));
		
		//EditorGUILayout.EndHorizontal();
	}
	public void SceneGUI(SceneView p_sceneView)
	{
		if (!m_fEnabled)
		{
			return;
		}
		
		Event t_Event = Event.current;

		int t_nControlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);

		if (t_Event.type == EventType.MouseDrag || t_Event.type == EventType.mouseDrag)
		{
			if (!m_fShiftDown)
			{
				Vector2 t_v2Pos = v2GridPos(t_Event.mousePosition, p_sceneView);

				if (t_v2Pos != m_v2PrevPos)
				{
					vGenerateCube(t_v2Pos);
				}

				m_v2PrevPos = t_v2Pos;

				t_Event.Use();
			}
		}
		if (t_Event.rawType == EventType.MouseUp && GUIUtility.hotControl != 0)
		{
			if (!m_fShiftDown)
			{
				GUIUtility.hotControl = 0;
			}
		}
		if (t_Event.type == EventType.MouseDown || t_Event.type == EventType.mouseDown)
		{
			m_fShiftDown = t_Event.shift;

			if (!t_Event.shift)
			{
				GUIUtility.hotControl = t_nControlID;
				// Clamp mouse position to grid position
				Vector2 t_v2Pos = v2GridPos(t_Event.mousePosition, p_sceneView);

				if (t_v2Pos != m_v2PrevPos)
				{
					vGenerateCube(t_v2Pos);
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
		}
	}
	private Vector2 v2GridPos(Vector2 p_v2MousePos, SceneView p_sceneView)
	{
		// REF: http://forum.unity3d.com/threads/screenpointtoray-with-event-mouseposition.119584/
		Camera t_camera = p_sceneView.camera;
		Vector2 t_v2Pos = t_camera.ScreenToWorldPoint(new Vector3(p_v2MousePos.x, t_camera.pixelHeight - p_v2MousePos.y, 0));
		
		t_v2Pos.x = Mathf.RoundToInt(t_v2Pos.x);
		t_v2Pos.y = Mathf.RoundToInt(t_v2Pos.y);

		return t_v2Pos;
	}
	private void vGenerateCube(Vector2 p_v2Pos)
	{
		float t_rChunkSize = (float)CChunkEditorGen.msc_nChunkSize;

		Vector2 t_v2Chunk = new Vector2(Mathf.RoundToInt(p_v2Pos.x / t_rChunkSize), Mathf.RoundToInt(p_v2Pos.y / t_rChunkSize));

		CTile t_tile = new CTile();
		t_tile.Tile = m_nSelectedTile;
		Debug.Log("selected tile: " + m_nSelectedTile);

		CChunkEditorGen.Instance.vAddTile(t_tile, p_v2Pos);
	}
	#endregion
}
