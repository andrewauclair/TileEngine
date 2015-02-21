using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class CChunkEditorGen : MonoBehaviour
{
    #region Static Data
	public static CChunkEditorGen Instance = null;

	// TEMP: fixed chunk sizes for ease of set up for now
	public static int msc_nChunkSize = 11;
    #endregion

    #region Public Data
	public Material AtlasMat = null;
	[HideInInspector]
	public List<CTile> lstTiles = new List<CTile>();
    #endregion

    #region Private Data
	private List<CChunk> m_lstChunks = new List<CChunk>();
	private List<int> m_aTestData = new List<int>();
    #endregion

    #region Unity Methods
    void OnEnable()
    {
		Instance = this;

		// load json file of uvs
		TextAsset t_Bytes = Resources.Load("Tilesets/" + AtlasMat.name) as TextAsset;

		if (t_Bytes == null)
		{
			Debug.Log("File Tilesets/'" + AtlasMat.name + "' not found");
		}
		else
		{
			CByteStreamReader t_Reader = new CByteStreamReader(t_Bytes.bytes);
			JSONObject t_obj = new JSONObject(t_Reader.strRead());

			for (int t_i = 0; t_i < t_obj.Count; ++t_i)
			{
				lstTiles.Add(new CTile(t_obj[t_i]));
			}
		}
		
		m_aTestData.Clear();

		for (int t_x = 0; t_x < msc_nChunkSize; ++t_x)
		{
			for (int t_y = 0; t_y < msc_nChunkSize; ++t_y)
			{
				m_aTestData.Add(-1);
			}
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
    }
	void Update()
    {
    }
    #endregion

    #region Public Methods
	public void vAddTile(CTile p_Tile, Vector3 p_v3Pos)
	{
		Vector2 t_v2Chunk = new Vector2(Mathf.RoundToInt(p_v3Pos.x / (float)msc_nChunkSize), Mathf.RoundToInt(p_v3Pos.y / (float)msc_nChunkSize));


		// Find the correct chunk that this tile belongs to
		CChunk t_chunk = null;

		for (int t_i = 0; t_i < m_lstChunks.Count; ++t_i)
		{
			if (m_lstChunks[t_i].v2Position == t_v2Chunk)
			{
				t_chunk = m_lstChunks[t_i];
				break;
			}
		}

		// If we didn't find the chunk then this is the first tile for it and we need to create the chunk
		if (t_chunk == null)
		{
			GameObject t_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			t_obj.transform.position = t_v2Chunk * msc_nChunkSize;
			t_obj.transform.parent = gameObject.transform;

			t_chunk = t_obj.AddComponent<CChunk>();
			t_chunk.vSetData(m_aTestData);
			t_chunk.vGenerateMesh();
			t_chunk.v2Position = t_v2Chunk;

			m_lstChunks.Add(t_chunk);
		}

		Vector2 t_v2TilePos = Vector2.zero;

		// Tile 0 of this chunk
		Vector2 t_v2TileZero = new Vector2((t_v2Chunk.x * msc_nChunkSize) - Mathf.Floor(msc_nChunkSize / 2.0f), (t_v2Chunk.y * msc_nChunkSize) + Mathf.Floor(msc_nChunkSize / 2.0f));
		
		Vector2 t_v2Diff = (Vector2)p_v3Pos - t_v2TileZero;
		
		t_v2Diff.y = Mathf.Abs(msc_nChunkSize - 1 - ( t_v2TileZero.y - p_v3Pos.y ));
		t_v2Diff.x = Mathf.Abs(t_v2Diff.x);
		
		int t_nTile = Mathf.RoundToInt((t_v2Diff.y * msc_nChunkSize) + t_v2Diff.x);
		
		t_chunk.vSetTile(t_v2Diff, p_Tile);
	}
    #endregion

    #region Private Methods
    #endregion
}
#endif