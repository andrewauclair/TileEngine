using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CChunk : MonoBehaviour
{
    #region Static Data
    #endregion

    #region Public Data
	public Vector2 v2Position { get; set; }
	public int nLayer { get; set; }
    #endregion

    #region Private Data
	private MeshRenderer m_meshRenderer = null;
	private MeshFilter m_meshFilter = null;
	private Mesh m_mesh = null;

	[SerializeField]
	private List<int> m_lstData = new List<int>();

	private bool m_fMeshGenerated = false;
    #endregion

    #region Unity Methods
    void Awake()
    {
		m_meshRenderer = GetComponent<MeshRenderer>();
		m_meshFilter = GetComponent<MeshFilter>();
    }
    void Start()
    {
		CWorldManager.Instance.m_lstChunks.Add(this);
	}
	void Update()
    {
    }
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		int t_nChunkSize = CChunkEditorGen.msc_nChunkSize;

		Gizmos.color = Color.white;
		
		float t_rHalfChunk = t_nChunkSize / 2.0f;

		Vector3 t_v3TopLeft = new Vector3(transform.position.x - t_rHalfChunk, transform.position.y + t_rHalfChunk, 0f);
		Vector3 t_v3TopRight = new Vector3(transform.position.x + t_rHalfChunk, transform.position.y + t_rHalfChunk, 0f);
		Vector3 t_v3BottomLeft = new Vector3(transform.position.x - t_rHalfChunk, transform.position.y - t_rHalfChunk, 0f);
		Vector3 t_v3BottomRight = new Vector3(transform.position.x + t_rHalfChunk, transform.position.y - t_rHalfChunk, 0f);

		Gizmos.DrawLine(t_v3TopLeft, t_v3TopRight);
		Gizmos.DrawLine(t_v3TopRight, t_v3BottomRight);
		Gizmos.DrawLine(t_v3BottomRight, t_v3BottomLeft);
		Gizmos.DrawLine(t_v3BottomLeft, t_v3TopLeft);
	}
#endif
    #endregion

    #region Public Methods
	public void vSetData(List<int> p_lstData)
	{
		m_meshRenderer = GetComponent<MeshRenderer>();
		m_meshFilter = GetComponent<MeshFilter>();
		m_lstData = new List<int>(p_lstData);
	}
	public void vGenerateMesh()
	{
		if (m_fMeshGenerated || m_lstData == null || fIsEmpty())
		{
			return;
		}

		if (m_meshRenderer == null)
		{
			m_meshRenderer = GetComponent<MeshRenderer>();
		}

		if (m_meshFilter == null)
		{
			m_meshFilter = GetComponent<MeshFilter>();
		}

		int t_xSize = 11;// CWorldManager.Instance.ChunkWidth;
		int t_ySize = 11;// CWorldManager.Instance.ChunkHeight;
		int t_nTileSize = 1;

		int t_cTiles = t_xSize * t_ySize;
		int t_cTris = t_cTiles * 2;
		
		m_mesh = new Mesh();

		Vector3[] t_aVertices = new Vector3[t_cTiles * 4];
		Vector2[] t_aUVs = new Vector2[t_cTiles * 4];
		int[] t_aTriangles = new int[t_cTris * 3];

		int t_iVert = 0;
		int t_iTri = 0;
		int t_iUV = 0;
		
		for (int t_y = 0; t_y < t_ySize; ++t_y)
		{
			for (int t_x = 0; t_x < t_xSize; ++t_x)
			{
				if (m_lstData[(t_y * t_xSize) + t_x] == -1)
				{
					continue;
				}
				// calculate the center of this tile
				float t_rx = -((t_xSize * t_nTileSize) / 2.0f) + (t_x * t_nTileSize) + (t_nTileSize / 2.0f);
				float t_ry = -((t_ySize * t_nTileSize) / 2.0f) + (t_y * t_nTileSize) + (t_nTileSize / 2.0f);

				// create the 4 vertices for this tile
				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0f);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0f);
				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0f);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0f);

#if UNITY_EDITOR
				CTile t_Tile = CChunkEditorGen.Instance.lstTiles[m_lstData[(t_y * t_xSize) + t_x]];

				t_aUVs[t_iUV++] = t_Tile.UV4;
				t_aUVs[t_iUV++] = t_Tile.UV2;
				t_aUVs[t_iUV++] = t_Tile.UV3;
				t_aUVs[t_iUV++] = t_Tile.UV1;
#endif
				// generate the 2 triangles for this tile
				t_aTriangles[t_iTri++] = t_iVert - 4; // 0
				t_aTriangles[t_iTri++] = t_iVert - 3; // 1
				t_aTriangles[t_iTri++] = t_iVert - 2; // 2
				t_aTriangles[t_iTri++] = t_iVert - 2; // 2
				t_aTriangles[t_iTri++] = t_iVert - 3; // 1
				t_aTriangles[t_iTri++] = t_iVert - 1; // 3
			}
		}

		// set the mesh data, calculate the normals and set the mesh and material
		m_mesh.vertices = t_aVertices;
		m_mesh.triangles = t_aTriangles;
		m_mesh.uv = t_aUVs;

		m_mesh.RecalculateNormals();
		
		m_meshFilter.mesh = m_mesh;
#if UNITY_EDITOR
		m_meshRenderer.material = CChunkEditorGen.Instance.AtlasMat;
#endif
		m_fMeshGenerated = true;
	}
	public void vDisposeMesh()
	{
		if (!m_fMeshGenerated)
		{
			return;
		}

		m_fMeshGenerated = false;

		DestroyImmediate(m_mesh);
		m_mesh = null;
	}
	public void vSetTile(Vector2 p_v2Pos, CTile p_Tile)
	{
#if UNITY_EDITOR
		m_lstData[(int)((p_v2Pos.y * CChunkEditorGen.msc_nChunkSize) + p_v2Pos.x)] = p_Tile.Tile;
#endif
		vDisposeMesh();
		vGenerateMesh();
	}
	public bool fIsEmpty()
	{
		for (int t_i = 0; t_i < m_lstData.Count; ++t_i)
		{
			if (m_lstData[t_i] != -1)
			{
				return false;
			}
		}
		return true;
	}
    #endregion

    #region Private Methods
    #endregion
}
