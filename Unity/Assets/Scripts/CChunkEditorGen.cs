using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class CChunkEditorGen : MonoBehaviour
{
    #region Static Data
	public static CChunkEditorGen Instance
	{
		get
		{
			if (m_Instance)
			{
				return m_Instance;
			}

			m_Instance = null;
			m_Instance = FindObjectOfType<CChunkEditorGen>();

			if (m_Instance)
			{
				return m_Instance;
			}

			Debug.LogError("No instance of CChunkEditorGen found.");
			return null;
		}
	}

	// TEMP: fixed chunk sizes for ease of set up for now
	public static int msc_nChunkSize = 11;

	protected static CChunkEditorGen m_Instance = null;
    #endregion

    #region Public Data
	[HideInInspector]
	public Material AtlasMat;

	public List<CLayer> lstLayers = new List<CLayer>();

	[HideInInspector]
	public List<CTile> lstTiles = new List<CTile>();
    #endregion

    #region Private Data
	private List<GameObject> m_lstLayers = new List<GameObject>();
	private List<CChunk> m_lstChunks = new List<CChunk>();
	private List<int> m_aTestData = new List<int>();
    #endregion

    #region Unity Methods
    void OnEnable()
    {
		m_Instance = GetComponent<CChunkEditorGen>();

		m_aTestData.Clear();

		for (int t_x = 0; t_x < msc_nChunkSize; ++t_x)
		{
			for (int t_y = 0; t_y < msc_nChunkSize; ++t_y)
			{
				m_aTestData.Add(-1);
			}
		}

		m_lstLayers.Clear();

		for (int t_i = 0; t_i < lstLayers.Count; ++t_i)
		{
			GameObject t_goLayer = GameObject.Find(lstLayers[t_i].m_strName);

			if (t_goLayer == null)
			{
				t_goLayer = new GameObject();
				t_goLayer.name = lstLayers[t_i].m_strName;
				t_goLayer.transform.position = Vector3.zero;
				t_goLayer.transform.parent = gameObject.transform;
			}

			m_lstLayers.Add(t_goLayer);
		}

		// fill our list of chunks with any chunks that might be children of this object
		for (int t_i = 0; t_i < transform.childCount; ++t_i)
		{
			CChunk t_chunk = transform.GetChild(t_i).GetComponent<CChunk>();

			if (t_chunk != null)
			{
				m_lstChunks.Add(t_chunk);
			}
		}

		// add the layers we want to always have
		if (lstLayers.Count == 0)
		{
			CLayer t_bgLayer = new CLayer();
			t_bgLayer.m_strName = "Background";
			t_bgLayer.m_fEditable = false;
			t_bgLayer.m_fMoveable = false;
			t_bgLayer.m_fTilesAllowed = true;

			CLayer t_playerLayer = new CLayer();
			t_playerLayer.m_strName = "Player";
			t_playerLayer.m_fEditable = false;
			t_playerLayer.m_fMoveable = true;
			t_playerLayer.m_fTilesAllowed = false;

			lstLayers.Add(t_bgLayer);
			lstLayers.Add(t_playerLayer);
		}
    }
	void Update()
    {
    }
    #endregion

    #region Public Methods
	public void vAddTile(CTile p_Tile, Vector3 p_v3Pos, int p_nLayer)
	{
		Vector2 t_v2Chunk = new Vector2(Mathf.RoundToInt(p_v3Pos.x / (float)msc_nChunkSize), Mathf.RoundToInt(p_v3Pos.y / (float)msc_nChunkSize));

		// find the layer or create it
		GameObject t_goLayer = null;

		for (int t_i = 0; t_i < m_lstLayers.Count; ++t_i)
		{
			if (m_lstLayers[t_i].name == lstLayers[p_nLayer].m_strName)
			{
				t_goLayer = m_lstLayers[t_i];
				break;
			}
		}

		if (t_goLayer == null)
		{
			t_goLayer = new GameObject();
			t_goLayer.name = lstLayers[p_nLayer].m_strName;
			t_goLayer.transform.position = Vector3.zero;
			t_goLayer.transform.parent = gameObject.transform;

			m_lstLayers.Add(t_goLayer);
		}

		// Find the correct chunk that this tile belongs to
		CChunk t_chunk = null;

		for (int t_i = 0; t_i < m_lstChunks.Count; ++t_i)
		{
			if (m_lstChunks[t_i].v2Position == t_v2Chunk && m_lstChunks[t_i].nLayer == p_nLayer)
			{
				t_chunk = m_lstChunks[t_i];
				break;
			}
		}

		// If we didn't find the chunk then this is the first tile for it and we need to create the chunk
		if (t_chunk == null && p_Tile.Tile != -1)
		{
			GameObject t_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			t_obj.transform.position = new Vector3(t_v2Chunk.x * msc_nChunkSize, t_v2Chunk.y * msc_nChunkSize, p_nLayer * -.5f);
			t_obj.transform.parent = t_goLayer.transform;
			t_obj.name = "Chunk (" + t_v2Chunk.x + ", " + t_v2Chunk.y + ") : " + lstLayers[p_nLayer];

			t_chunk = t_obj.AddComponent<CChunk>();
			t_chunk.vSetData(m_aTestData);
			
			t_chunk.vGenerateMesh();
			t_chunk.v2Position = t_v2Chunk;
			t_chunk.nLayer = p_nLayer;

			m_lstChunks.Add(t_chunk);
		}
		else if (t_chunk == null && p_Tile.Tile == -1)
		{
			// we're deleting a tile from an empty chunk, just leave
			return;
		}

		

		// Tile 0 of this chunk
		Vector2 t_v2TileZero = new Vector2((t_v2Chunk.x * msc_nChunkSize) - Mathf.Floor(msc_nChunkSize / 2.0f), (t_v2Chunk.y * msc_nChunkSize) + Mathf.Floor(msc_nChunkSize / 2.0f));
		
		Vector2 t_v2Diff = (Vector2)p_v3Pos - t_v2TileZero;
		
		t_v2Diff.y = Mathf.Abs(msc_nChunkSize - 1 - ( t_v2TileZero.y - p_v3Pos.y ));
		t_v2Diff.x = Mathf.Abs(t_v2Diff.x);
		
		//int t_nTile = Mathf.RoundToInt((t_v2Diff.y * msc_nChunkSize) + t_v2Diff.x);
		
		t_chunk.vSetTile(t_v2Diff, p_Tile);

		if (t_chunk.fIsEmpty())
		{
			m_lstChunks.Remove(t_chunk);
			DestroyImmediate(t_chunk.gameObject);
		}
	}
	public void vSetTileset(string p_strTileset)
	{
		Debug.Log("load tileset: " + p_strTileset);
		
		AtlasMat = Resources.LoadAssetAtPath("Assets/Art/Tilesets/Materials/" + p_strTileset + ".mat", typeof(Material)) as Material;
		
		TextAsset t_Bytes = Resources.Load("Tilesets/" + AtlasMat.name) as TextAsset;

		// load json file of uvs
		if (t_Bytes == null)
		{
			Debug.Log("File Tilesets/'" + AtlasMat.name + "' not found");
		}
		else
		{
			CByteStreamReader t_Reader = new CByteStreamReader(t_Bytes.bytes);
			JSONObject t_obj = new JSONObject(t_Reader.strRead());

			lstTiles = new List<CTile>();
			for (int t_i = 0; t_i < t_obj.Count; ++t_i)
			{
				lstTiles.Add(new CTile(t_obj[t_i]));
			}
		}
	}
	public void vSetLayerActive(int p_nLayer, bool p_fActive)
	{
		m_lstLayers[p_nLayer].SetActive(p_fActive);
	}
    #endregion

    #region Private Methods
    #endregion
}
#endif