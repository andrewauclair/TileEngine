using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class CChunk : MonoBehaviour
{
    #region Static Data
    #endregion

    #region Public Data
    #endregion

    #region Private Data
	private MeshRenderer m_meshRenderer = null;
	private MeshFilter m_meshFilter = null;
	private Mesh m_mesh = null;

	private List<int> m_aData = null;

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
	}
	void Update()
    {
    }
    #endregion

    #region Public Methods
	public void vSetData(List<int> p_aData)
	{
		m_aData = p_aData;
	}
	public void vGenerateMesh()
	{
		if (m_fMeshGenerated || m_aData == null)
		{
			return;
		}

		Mesh t_mesh = m_meshFilter.mesh;
		string t_strTris = "Tris: ";
		for (int t_j = 0; t_j < t_mesh.triangles.Length; ++t_j)
		{
			t_strTris += t_mesh.triangles[t_j] + ", ";
		}

		Debug.Log(t_strTris);

		string t_strVerts = "Verts: ";
		for (int t_j = 0; t_j < t_mesh.vertices.Length; ++t_j)
		{
			t_strVerts += t_mesh.vertices[t_j].ToString() + ", ";
		}

		Debug.Log(t_strVerts);

		int t_xSize = CWorldManager.Instance.ChunkSize;
		int t_ySize = CWorldManager.Instance.ChunkSize;
		int t_nTileSize = CWorldManager.Instance.TileSize;

		int t_cTiles = t_xSize * t_ySize;
		int t_cTris = t_cTiles * 2;
		int t_cVerts = (t_xSize + 1) * (t_ySize + 1);

		m_mesh = new Mesh();

		Vector3[] t_aVertices = new Vector3[t_cTiles * 4];
		Vector3[] t_aNormals = new Vector3[t_cTiles * 4];
		Vector2[] t_aUVs = new Vector2[t_cTiles * 4];
		int[] t_aTriangles = new int[t_cTris * 3];

		int t_i = 0;

		for (float t_y = -(t_ySize / 2.0f); t_y <= t_ySize / 2.0f; ++t_y)
		{
			for (float t_x = -(t_xSize / 2.0f); t_x <= t_xSize / 2.0f; ++t_x)
			{
				//t_aVertices[t_i] = new Vector3(t_x *t_nTileSize, t_y * t_nTileSize, 0);

				//t_aUVs[t_i++] = new Vector2((t_x + (t_xSize / 2.0f)) / (float)t_xSize, (t_y + (t_ySize / 2.0f)) / (float)t_ySize);
			}
		}

		int t_iVert = 0;
		int t_iTri = 0;
		int t_iUV = 0;

		for (int t_y = 0; t_y < t_ySize; ++t_y)
		{
			for (int t_x = 0; t_x < t_xSize; ++t_x)
			{
				float t_rx = -((t_xSize * t_nTileSize) / 2.0f) + (t_x * t_nTileSize) + (t_nTileSize / 2.0f);
				float t_ry = -((t_ySize * t_nTileSize) / 2.0f) + (t_y * t_nTileSize) + (t_nTileSize / 2.0f);

				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0); 
				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0);

				switch (m_aData[(t_y * t_xSize) + t_x])
				{
					case 0:
						{
							t_aUVs[t_iUV++] = new Vector2(0, 0);
							t_aUVs[t_iUV++] = new Vector2(0, .5f);
							t_aUVs[t_iUV++] = new Vector2(1, 0);
							t_aUVs[t_iUV++] = new Vector2(1, .5f);
						}break;
					case 1:
						{
							t_aUVs[t_iUV++] = new Vector2(0, .5f);
							t_aUVs[t_iUV++] = new Vector2(0, 1);
							t_aUVs[t_iUV++] = new Vector2(1, .5f);
							t_aUVs[t_iUV++] = new Vector2(1, 1);
						}break;
				}
				

				t_aTriangles[t_iTri++] = t_iVert - 4;
				t_aTriangles[t_iTri++] = t_iVert - 3;
				t_aTriangles[t_iTri++] = t_iVert - 2;
				t_aTriangles[t_iTri++] = t_iVert - 2;
				t_aTriangles[t_iTri++] = t_iVert - 3;
				t_aTriangles[t_iTri++] = t_iVert - 1;

				//t_aTriangles[t_iTri++] = (t_y * (t_xSize + 1)) + t_x;
				//t_aTriangles[t_iTri++] = ((t_y + 1) * (t_xSize + 1) )+ t_x;
				//t_aTriangles[t_iTri++] = (t_y * (t_xSize + 1)) + t_x + 1;

				//t_aTriangles[t_iTri++] = ((t_y + 1) * (t_xSize + 1)) + t_x;
				//t_aTriangles[t_iTri++] = ((t_y + 1) * (t_xSize + 1)) + t_x + 1;
				//t_aTriangles[t_iTri++] = (t_y * (t_xSize + 1)) + t_x + 1;
			}
		}

		m_mesh.vertices = t_aVertices;
		m_mesh.triangles = t_aTriangles;
		m_mesh.uv = t_aUVs;

		m_mesh.RecalculateNormals();
		
		m_meshFilter.mesh = m_mesh;
		m_meshRenderer.material = CWorldManager.Instance.AtlasMat;

		m_fMeshGenerated = true;
	}
	public void vDisposeMesh()
	{
		if (!m_fMeshGenerated)
		{
			return;
		}

		m_mesh.Clear();
		m_mesh = null;
	}
    #endregion

    #region Private Methods
    #endregion
}
