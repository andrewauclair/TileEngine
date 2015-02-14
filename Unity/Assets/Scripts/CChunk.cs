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
    #endregion

    #region Private Data
	private MeshRenderer m_meshRenderer = null;
	private MeshFilter m_meshFilter = null;
	private Mesh m_mesh = null;

	private List<int> m_aData = null;

	private bool m_fMeshGenerated = false;

	private int m_nPixelSize = 32;
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

		int t_xSize = CWorldManager.Instance.ChunkWidth;
		int t_ySize = CWorldManager.Instance.ChunkHeight;
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

		int t_iVert = 0;
		int t_iTri = 0;
		int t_iUV = 0;
		int t_nTextureWidth = CWorldManager.Instance.AtlasMat.mainTexture.width;
		int t_nTextureHeight = CWorldManager.Instance.AtlasMat.mainTexture.height;

		for (int t_y = 0; t_y < t_ySize; ++t_y)
		{
			for (int t_x = 0; t_x < t_xSize; ++t_x)
			{
				// calculate the center of this tile
				float t_rx = -((t_xSize * t_nTileSize) / 2.0f) + (t_x * t_nTileSize) + (t_nTileSize / 2.0f);
				float t_ry = -((t_ySize * t_nTileSize) / 2.0f) + (t_y * t_nTileSize) + (t_nTileSize / 2.0f);

				// create the 4 vertices for this tile
				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0); 
				t_aVertices[t_iVert++] = new Vector3(t_rx - (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry - (t_nTileSize / 2.0f), 0);
				t_aVertices[t_iVert++] = new Vector3(t_rx + (t_nTileSize / 2.0f), t_ry + (t_nTileSize / 2.0f), 0);

				// TEMP: generate UV's to stagger the grass for now
				int t_it = (t_y + t_x) % 3;
				float t_rT = t_it * .125f;

				switch (m_aData[(t_y * t_xSize) + t_x])
				{
					case 0:
						{
							t_aUVs[t_iUV++] = new Vector2(t_rT + .125f, .75f); 
							t_aUVs[t_iUV++] = new Vector2(t_rT + .125f, 1); 
							t_aUVs[t_iUV++] = new Vector2(t_rT, .75f); 
							t_aUVs[t_iUV++] = new Vector2(t_rT, 1);
						}break;
					case 1:
						{
							t_aUVs[t_iUV++] = new Vector2(0, 0);
							t_aUVs[t_iUV++] = new Vector2(0, m_nPixelSize / (float)t_nTextureHeight);
							t_aUVs[t_iUV++] = new Vector2(m_nPixelSize / (float)t_nTextureWidth, 0);
							t_aUVs[t_iUV++] = new Vector2(m_nPixelSize / (float)t_nTextureWidth, m_nPixelSize / (float)t_nTextureHeight);
						}break;
				}
				
				// generate the 2 triangles for this tile
				t_aTriangles[t_iTri++] = t_iVert - 4;
				t_aTriangles[t_iTri++] = t_iVert - 3;
				t_aTriangles[t_iTri++] = t_iVert - 2;
				t_aTriangles[t_iTri++] = t_iVert - 2;
				t_aTriangles[t_iTri++] = t_iVert - 3;
				t_aTriangles[t_iTri++] = t_iVert - 1;
			}
		}

		// set the mesh data, calculate the normals and set the mesh and material
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
