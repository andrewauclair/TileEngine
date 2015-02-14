using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CWorldManager : MonoBehaviour
{
    #region Static Data
	public static CWorldManager Instance = null;
    #endregion

    #region Public Data
	[Tooltip("Chunk size to load in a single mesh, screen size or greater recommended")]
	public int ChunkWidth = 0;
	public int ChunkHeight = 0;
	public int TileSize = 0;
	public Material AtlasMat = null;
    #endregion

    #region Private Data
	List<CChunk> m_aChunks = new List<CChunk>();
    #endregion

    #region Unity Methods
    void Awake()
    {
		Instance = this;

		if (ChunkWidth <= 0 || ChunkHeight <= 0)
		{
			Debug.LogError("Chunk size is <= 0");
			Debug.Break();
		}

		List<int> t_aTestData = new List<int>();

		for (int t_i = 0; t_i < ChunkHeight * ChunkWidth; t_i += 2)
		{
			t_aTestData.Add(0);
			t_aTestData.Add(0);
		}

		for (int t_i = 0; t_i < 1; ++t_i)
		{
			GameObject t_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			t_obj.transform.position = new Vector3(-(TileSize / 2.0f), -(TileSize / 2.0f), 0f);

			CChunk t_chunk = t_obj.AddComponent<CChunk>();
			t_chunk.vSetData(t_aTestData);
			t_chunk.vGenerateMesh();
			m_aChunks.Add(t_chunk);
		}
    }
    void Start()
    {	
	}
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevelAsync("Test2");
		}
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
	private void vGenerateChunk()
	{

	}
	private void vDeleteChunk()
	{
	}
    #endregion
}
