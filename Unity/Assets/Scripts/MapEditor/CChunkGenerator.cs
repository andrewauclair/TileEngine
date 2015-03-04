using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CChunkGenerator : MonoBehaviour
{
	#region Static Data
	public static CChunkGenerator Instance = null;
	public static int msc_nChunkSize = 11;
	#endregion

	#region Public Data
	[HideInInspector]
	public Material AtlasMat = null;

	[HideInInspector]
	public List<CTile> lstTiles = new List<CTile>();
	[HideInInspector]
	public List<CLayer> lstLayers = new List<CLayer>();
	#endregion

	#region Private Data
	private List<GameObject> m_lstGOLayers = new List<GameObject>();
	private List<CChunk> m_lstChunks = new List<CChunk>();
	private List<int> m_aTestData = new List<int>();
	private string m_strCurrentScene = "";
	private GameObject m_goWorld = null;
	#endregion

	#region Unity Methods
	void OnEnable()
	{
		Instance = this;

		m_goWorld = GameObject.FindGameObjectWithTag("World");

#if UNITY_EDITOR
		m_strCurrentScene = UnityEditor.EditorApplication.currentScene;
#endif

		CChunkData t_chunkData = FindObjectOfType<CChunkData>();
		if (t_chunkData != null)
		{
			lstLayers = new List<CLayer>(t_chunkData.m_lstLayers.ToArray());
		}
		m_aTestData.Clear();

		for (int t_x = 0; t_x < msc_nChunkSize; ++t_x)
		{
			for (int t_y = 0; t_y < msc_nChunkSize; ++t_y)
			{
				m_aTestData.Add(-1);
			}
		}

		m_lstGOLayers.Clear();

		for (int t_i = 0; t_i < lstLayers.Count; ++t_i)
		{
			GameObject t_goLayer = GameObject.Find(lstLayers[t_i].m_strName);

			if (t_goLayer == null)
			{
				t_goLayer = new GameObject();
				t_goLayer.name = lstLayers[t_i].m_strName;
				t_goLayer.transform.position = Vector3.zero;
				t_goLayer.transform.parent = m_goWorld.transform;
			}

			m_lstGOLayers.Add(t_goLayer);
		}

		CChunk[] t_aChunks = m_goWorld.transform.GetComponentsInChildren<CChunk>();
		m_lstChunks = new List<CChunk>(t_aChunks);
	}
	void Update()
	{
#if UNITY_EDITOR
		if (UnityEditor.EditorApplication.currentScene != m_strCurrentScene)
		{
			OnEnable();
		}
#endif
	}
	#endregion

	#region Public Methods
	public void vAddTile(CTile p_Tile, Vector3 p_v3Pos, int p_nLayer)
	{
		Vector2 t_v2Chunk = new Vector2(Mathf.RoundToInt(p_v3Pos.x / (float)msc_nChunkSize), Mathf.RoundToInt(p_v3Pos.y / (float)msc_nChunkSize));

		GameObject t_goLayer = null;

		for (int t_i = 0; t_i < m_lstGOLayers.Count; ++t_i)
		{
			if (m_lstGOLayers[t_i].name == lstLayers[p_nLayer].m_strName)
			{
				t_goLayer = m_lstGOLayers[t_i];
				break;
			}
		}

		// Find the correct chunk that this tile belongs to
		CChunk t_chunk = findChunk(t_v2Chunk, p_nLayer);

		// If we didn't find the chunk then this is the first tile for it and we need to create the chunk
		if (t_chunk == null && p_Tile.Tile != -1)
		{
			t_chunk = generateChunk(t_goLayer, t_v2Chunk, p_nLayer);
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

		t_v2Diff.y = Mathf.Abs(msc_nChunkSize - 1 - (t_v2TileZero.y - p_v3Pos.y));
		t_v2Diff.x = Mathf.Abs(t_v2Diff.x);

		t_chunk.vSetTile(t_v2Diff, p_Tile);

		if (t_chunk.fIsEmpty())
		{
			m_lstChunks.Remove(t_chunk);
			DestroyImmediate(t_chunk.m_mesh);
			DestroyImmediate(t_chunk.gameObject);
		}
		else if (lstLayers[p_nLayer].m_strName != "Collision")
		{
			vUpdateChunk(t_chunk);
		}
		else
		{
			vUpdateCollisions(t_chunk);
		}
	}
	public void vSetTileset(string p_strTileset)
	{
		Debug.Log("load tileset: " + p_strTileset);

#if UNITY_EDITOR
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
#endif
	}

	public void vSetLayerActive(int p_nLayer, bool p_fActive)
	{
		m_lstGOLayers[p_nLayer].SetActive(p_fActive);
	}
	#endregion

	#region Private Methods
	private CChunk findChunk(Vector2 p_v2Pos, int p_nLayer)
	{
		CChunk t_chunk = null;

		for (int t_i = 0; t_i < m_lstChunks.Count; ++t_i)
		{
			if (m_lstChunks[t_i].v2Position == p_v2Pos && m_lstChunks[t_i].nLayer == p_nLayer)
			{
				t_chunk = m_lstChunks[t_i];
				break;
			}
		}

		return t_chunk;
	}
	private CChunk generateChunk(GameObject p_goLayer, Vector2 p_v2Pos, int p_nLayer)
	{
		GameObject t_obj = new GameObject();

		t_obj.AddComponent<MeshFilter>();
		t_obj.AddComponent<MeshRenderer>();

		t_obj.transform.position = new Vector3(p_v2Pos.x * msc_nChunkSize, p_v2Pos.y * msc_nChunkSize, p_nLayer * -.5f);
		t_obj.transform.parent = p_goLayer.transform;
		t_obj.name = "Chunk (" + p_v2Pos.x + ", " + p_v2Pos.y + ") : " + lstLayers[p_nLayer].m_strName;

		CChunk t_chunk = t_obj.AddComponent<CChunk>();
		t_chunk.vSetData(m_aTestData);

		t_chunk.v2Position = p_v2Pos;
		t_chunk.nLayer = p_nLayer;

		t_chunk.m_mesh = new Mesh();

		return t_chunk;
	}
	private void vUpdateChunk(CChunk p_chunk)
	{
		int t_cTiles = msc_nChunkSize * msc_nChunkSize;
		int t_cTris = t_cTiles * 2;

		DestroyImmediate(p_chunk.m_mesh);
		p_chunk.m_mesh = null;
		p_chunk.m_mesh = new Mesh();

		List<int> t_lstData = p_chunk.lstData();
		List<Vector3> t_lstVertices = new List<Vector3>();
		List<Vector2> t_lstUVs = new List<Vector2>();
		List<int> t_lstTriangles = new List<int>();

		int t_iVert = 0;
		int t_iTri = 0;
		int t_iUV = 0;

		for (int t_y = 0; t_y < msc_nChunkSize; ++t_y)
		{
			for (int t_x = 0; t_x < msc_nChunkSize; ++t_x)
			{
				if (t_lstData[(t_y * msc_nChunkSize) + t_x] == -1)
				{
					continue;
				}
				// calculate the center of this tile
				float t_rx = -msc_nChunkSize / 2.0f + t_x + .5f;
				float t_ry = -msc_nChunkSize / 2.0f + t_y + .5f;

				CTile t_tile = lstTiles[t_lstData[(t_y * msc_nChunkSize) + t_x]];
				if (t_tile.Tile == 32)
				{
					vAutoTile(p_chunk, t_x, t_y, t_lstVertices, t_lstUVs, t_lstTriangles);
				}
				else
				{
					vGenerateTile(t_rx, t_ry, 1f, t_tile, t_lstVertices, t_lstUVs, t_lstTriangles);
				}
			}
		}

		// set the mesh data, calculate the normals and set the mesh and material
		p_chunk.m_mesh.vertices = t_lstVertices.ToArray();
		p_chunk.m_mesh.triangles = t_lstTriangles.ToArray();
		p_chunk.m_mesh.uv = t_lstUVs.ToArray();

		p_chunk.m_mesh.RecalculateNormals();

		MeshFilter t_meshFilter = p_chunk.GetComponent<MeshFilter>();
		MeshRenderer t_meshRenderer = p_chunk.GetComponent<MeshRenderer>();

		t_meshFilter.mesh = p_chunk.m_mesh;
		t_meshRenderer.material = AtlasMat;
	}
	private void vUpdateCollisions(CChunk p_chunk)
	{
		BoxCollider[] t_aColliders = p_chunk.gameObject.GetComponentsInChildren<BoxCollider>();

		for (int t_i = 0; t_i < t_aColliders.Length; ++t_i)
		{
			DestroyImmediate(t_aColliders[t_i].gameObject);
		}
		List<int> t_lstData = p_chunk.lstData();

		for (int t_y = 0; t_y < msc_nChunkSize; ++t_y)
		{
			for (int t_x = 0; t_x < msc_nChunkSize; ++t_x)
			{
				if (t_lstData[(t_y * msc_nChunkSize) + t_x] == -1)
				{
					continue;
				}
				// calculate the center of this tile
				float t_rx = -msc_nChunkSize / 2.0f + t_x + .5f;
				float t_ry = -msc_nChunkSize / 2.0f + t_y + .5f;

				GameObject t_goCollider = new GameObject();
				t_goCollider.transform.position = new Vector3(t_rx, t_ry, 0f);
				t_goCollider.AddComponent<BoxCollider>();
				t_goCollider.transform.parent = p_chunk.gameObject.transform;
			}
		}
	}
	private void vGenerateTile(float p_x, float p_y, float p_rSize, CTile p_tile, List<Vector3> p_lstVertices, List<Vector2> p_lstUVs, List<int> p_lstTriangles)
	{
		float t_rRad = p_rSize / 2.0f;

		// create the 4 vertices for this tile
		p_lstVertices.Add(new Vector3(p_x - t_rRad, p_y - t_rRad, 0f));
		p_lstVertices.Add(new Vector3(p_x + t_rRad, p_y + t_rRad, 0f));
		p_lstVertices.Add(new Vector3(p_x + t_rRad, p_y - t_rRad, 0f));
		p_lstVertices.Add(new Vector3(p_x - t_rRad, p_y + t_rRad, 0f));

		p_lstUVs.Add(p_tile.UV1);
		p_lstUVs.Add(p_tile.UV2);
		p_lstUVs.Add(p_tile.UV3);
		p_lstUVs.Add(p_tile.UV4);

		int t_cVerts = p_lstVertices.Count;

		// generate the 2 triangles for this tile
		p_lstTriangles.Add(t_cVerts - 4); // 0
		p_lstTriangles.Add(t_cVerts - 3); // 1
		p_lstTriangles.Add(t_cVerts - 2); // 2
		p_lstTriangles.Add(t_cVerts - 3); // 1
		p_lstTriangles.Add(t_cVerts - 4); // 0
		p_lstTriangles.Add(t_cVerts - 1); // 3
	}
	private void vAutoTile(CChunk p_chunk, int p_x, int p_y, List<Vector3> p_lstVertices, List<Vector2> p_lstUVs, List<int> p_lstTriangles)
	{
		// Corners - 0
		
		// 00011100
		// 00000111
		// 11000001
		// 01110000

		// inside
		// 11111111 - 4

		// edge
		// 00010101 - 1
		// 01000101 - 2
		// 01010001 - 1
		// 01010100 - 2

		// inside edge - 3
		// 01111111
		// 11011111
		// 11110111
		// 11111101

		// 40, 41
		// 48, 49
		CTile t_tileTL = lstTiles[40];
		CTile t_tileTR = lstTiles[41];
		CTile t_tileBL = lstTiles[48];
		CTile t_tileBR = lstTiles[49];
		float t_rx = -msc_nChunkSize / 2.0f + p_x + .5f;
		float t_ry = -msc_nChunkSize / 2.0f + p_y + .5f;

		float t_xL = t_rx - .25f;
		float t_yT = t_ry + .25f;
		float t_yB = t_ry - .25f;
		float t_xR = t_rx + .25f;

		// sides
		bool t_fTop = p_chunk.lstData()[(p_y - 1) * msc_nChunkSize + p_x] != -1;
		bool t_fBottom = p_chunk.lstData()[(p_y + 1) * msc_nChunkSize + p_x] != -1;
		bool t_fLeft = p_chunk.lstData()[p_y * msc_nChunkSize + p_x - 1] != -1;
		bool t_fRight = p_chunk.lstData()[p_y * msc_nChunkSize + p_x + 1] != -1;

		// corners
		bool t_fTopLeft = p_chunk.lstData()[(p_y - 1) * msc_nChunkSize + p_x - 1] != -1;
		bool t_fTopRight = p_chunk.lstData()[(p_y - 1) * msc_nChunkSize + p_x + 1] != -1;
		bool t_fBottomLeft = p_chunk.lstData()[(p_y + 1) * msc_nChunkSize + p_x - 1] != -1;
		bool t_fBottomRight = p_chunk.lstData()[(p_y + 1) * msc_nChunkSize + p_x + 1] != -1;

		// top left
		int t_nTL = 0;
		int t_nTR = 0;
		int t_nBL = 0;
		int t_nBR = 0;

		if (t_fTopLeft) t_nTL++;
		if (t_fTopRight) t_nTR++;
		if (t_fBottomLeft) t_nBL++;
		if (t_fBottomRight) t_nBR++;

		if (t_fTop)
		{
			t_nTL++;
			t_nTR++;
		}

		if (t_fBottom)
		{
			t_nBL++;
			t_nBR++;
		}

		if (t_fLeft)
		{
			t_nTL++;
			t_nBL++;
		}

		if (t_fRight)
		{
			t_nTR++;
			t_nBR++;
		}

		switch (t_nTL)
		{
			case 0:
				{
					//vGenerateTile(t_xL, t_yT, .5f, t_tileTL.tileTopLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			case 3:
				{
					vGenerateTile(t_xL, t_yT, .5f, t_tileTL.tileBottomRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			default:
				{
					if (t_fTop || t_fBottom)
					{
						//vGenerateTile(t_xL, t_yT, .5f, t_tileTL.tileBottomLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
					else
					{
						//vGenerateTile(t_xL, t_yT, .5f, t_tileTL.tileTopRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
				}break;
		}

		switch (t_nTR)
		{
			case 0:
				{
					//vGenerateTile(t_xR, t_yT, .5f, t_tileTR.tileTopRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			case 3:
				{
					vGenerateTile(t_xR, t_yT, .5f, t_tileTR.tileBottomLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			default:
				{
					if (t_fTop || t_fBottom)
					{
						//vGenerateTile(t_xR, t_yT, .5f, t_tileTR.tileBottomRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
					else
					{
						//vGenerateTile(t_xR, t_yT, .5f, t_tileTR.tileTopLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
				}break;
		}

		switch (t_nBL)
		{
			case 0:
				{
					//vGenerateTile(t_xL, t_yB, .5f, t_tileBL.tileBottomLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			case 3:
				{
					vGenerateTile(t_xL, t_yB, .5f, t_tileBL.tileTopRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			default:
				{
					if (t_fTop || t_fBottom)
					{
						//vGenerateTile(t_xL, t_yB, .5f, t_tileBL.tileTopRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
					else
					{
						//vGenerateTile(t_xL, t_yB, .5f, t_tileBL.tileTopRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
					}
				}break;
		}

		switch (t_nBR)
		{
			case 0:
				{
					vGenerateTile(t_xR, t_yB, .5f, t_tileBR.tileBottomRight(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
			case 3:
				{
					vGenerateTile(t_xR, t_yB, .5f, t_tileBR.tileTopLeft(), p_lstVertices, p_lstUVs, p_lstTriangles);
				}break;
		}
	}
	#endregion
}
