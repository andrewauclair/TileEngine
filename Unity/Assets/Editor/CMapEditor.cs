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

	private string[] m_aStrTilesets = new string[0];

	private Vector2 m_v2PrevPos = new Vector2(0, 0);
	private int m_nTileSize = 32;
	private int m_nSelectedTile = 0;
	private int m_nSelectedTileset = 0;

	private bool m_fEnabled = false;
	private bool m_fShiftDown = false;

	[MenuItem(CEditorTools.msc_strToolsName + "/Map Editor")]
	private static void Init()
	{
		CMapEditor t_mapEditor = (CMapEditor)EditorWindow.GetWindow(typeof(CMapEditor));//<CMapEditor>("Map Editor", true, typeof(SceneView));
		//t_mapEditor.position = new Rect(250.0f, 200.0f, 1000.0f, 750.0f);
	}

	public CMapEditor()
	{
		Debug.Log("CMapEditor Constructor");
		InitData();
		//if (PlayerPrefs.HasKey("MapEditorTextureID"))
		//{
		//    m_tex2dTileset = (Texture2D)EditorUtility.InstanceIDToObject(PlayerPrefs.GetInt("MapEditorTextureID"));
		//}
		try { SceneView.onSceneGUIDelegate -= SceneGUI; }
		catch { }
		SceneView.onSceneGUIDelegate += SceneGUI;
	}

	public void InitData()
	{
		Debug.Log("show map editor");

		//m_lstTex2dMap.Clear();

		//for (int t_x = 0; t_x < 1; ++t_x)
		//{
		//    m_lstTex2dMap.Add(new List<Texture2D>());
		//    m_lstTileNums.Add(new List<int>());

		//    for (int t_y = 0; t_y < 1; ++t_y)
		//    {
		//        m_lstTex2dMap[t_x].Add(new Texture2D(m_nTileSize * 30, m_nTileSize * 30, TextureFormat.RGBA32, false));
		//        m_lstTileNums[t_x].Add(0);

		//        for (int t_i = 0; t_i < m_nTileSize * 30; ++t_i)
		//        {
		//            for (int t_j = 0; t_j < m_nTileSize * 30; ++t_j)
		//            {
		//                m_lstTex2dMap[t_x][t_y].SetPixel(t_i, t_j, new Color(t_x / 30.0f, t_y / 30.0f, 0, 1.0f));
		//            }
		//        }

		//        m_lstTex2dMap[t_x][t_y].Apply();
		//    }
		//}
	}
	void Close()
	{
		Debug.Log("Close Map Editor");
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

		m_nSelectedTileset = EditorGUILayout.Popup("Tileset", m_nSelectedTileset, m_aStrTilesets);

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

		//m_v2ScrollPos = EditorGUILayout.BeginScrollView(m_v2ScrollPos, true, true, GUILayout.Height(750.0f), GUILayout.Width(750.0f));//, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

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
		//EditorGUILayout.EndScrollView();
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
	// Save the active map whenever it is changed
	private void vSaveMap()
	{
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
		vCheckMapInput(t_fMouseDown, t_fMouseUp, t_Event.mousePosition);
	}
	private void vCheckTileInput(bool p_fMouseDown, bool p_fMouseUp, Vector2 p_v2MousePos)
	{
	}
	private void vCheckMapInput(bool p_fMouseDown, bool p_fMouseUp, Vector2 p_v2MousePos)
	{
		for (int t_i = 0; t_i < m_lstTex2dMap.Count; ++t_i)
		{
		}
	}
	private Vector2 v2GridPos(Vector2 p_v2MousePos, SceneView p_sceneView)
	{
		Camera t_camera = p_sceneView.camera;
		Vector2 t_v2Pos = t_camera.ScreenToWorldPoint(new Vector3(p_v2MousePos.x, t_camera.pixelHeight - p_v2MousePos.y, 0));
		Debug.Log("screen to world: " + t_v2Pos);
		//t_v2Pos.z = 0;
		//t_v2Pos.y *= -1;

		t_v2Pos.x = Mathf.RoundToInt(t_v2Pos.x);
		t_v2Pos.y = Mathf.RoundToInt(t_v2Pos.y);

		return t_v2Pos;
	}
	private void vGenerateCube(Vector2 p_v2Pos)
	{
		float t_rChunkSize = (float)CChunkEditorGen.msc_nChunkSize;

		Vector2 t_v2Chunk = new Vector2(Mathf.RoundToInt(p_v2Pos.x / t_rChunkSize), Mathf.RoundToInt(p_v2Pos.y / t_rChunkSize));

		CChunkEditorGen.Instance.vAddTile(new CTile(), p_v2Pos);
	}
	#endregion
}
