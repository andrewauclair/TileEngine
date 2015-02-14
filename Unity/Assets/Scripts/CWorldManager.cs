using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CWorldManager : MonoBehaviour
{
	[SerializeField]
	public enum Arrows
	{
		Left,
		Right,
		Up,
		Down
	}

    #region Static Data
	public static CWorldManager Instance = null;
    #endregion

    #region Public Data
	[Tooltip("Chunk size to load in a single mesh, screen size or greater recommended")]
	public int ChunkWidth = 0;
	public int ChunkHeight = 0;
	public int TileSize = 0;
	public Material AtlasMat = null;

	public float MoveSpeed = 10f;

	public Animator MainCharAnimator = null;
    #endregion

    #region Private Data
	List<CChunk> m_aChunks = new List<CChunk>();
	private bool m_fLeftPressed = false;
	private bool m_fRightPressed = false;
	private bool m_fUpPressed = false;
	private bool m_fDownPressed = false;

	private Vector3 m_v3Target = Vector3.zero;
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

		for (int t_i = 0; t_i < 6; ++t_i)
		{
			GameObject t_obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			t_obj.transform.position = new Vector3(-(TileSize / 2.0f) + (ChunkWidth * t_i), -(TileSize / 2.0f), 0f);
			t_obj.transform.parent = gameObject.transform;

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
		Vector3 t_v3Offset = Vector3.zero;

		if (Input.GetKey(KeyCode.LeftArrow) || m_fLeftPressed)
		{
			t_v3Offset.x += MoveSpeed * Time.deltaTime;
			MainCharAnimator.SetInteger("Direction", 1);
		}
		if (Input.GetKey(KeyCode.RightArrow) || m_fRightPressed)
		{
			t_v3Offset.x -= MoveSpeed * Time.deltaTime;
			MainCharAnimator.SetInteger("Direction", 3);
		}
		if (Input.GetKey(KeyCode.UpArrow) || m_fUpPressed)
		{
			t_v3Offset.y -= MoveSpeed * Time.deltaTime;
			MainCharAnimator.SetInteger("Direction", 2);
		}
		if (Input.GetKey(KeyCode.DownArrow) || m_fDownPressed)
		{
			t_v3Offset.y += MoveSpeed * Time.deltaTime;
			MainCharAnimator.SetInteger("Direction", 0);
		}

		if (t_v3Offset != Vector3.zero)
		{
			MainCharAnimator.SetBool("Idle", false);
			vMoveChunks(t_v3Offset);
		}
		else
		{
			MainCharAnimator.SetBool("Idle", true);
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
	private void vMoveChunks(Vector3 p_v3Offset)
	{
		for (int t_i = 0; t_i < m_aChunks.Count; ++t_i)
		{
			m_aChunks[t_i].transform.position += p_v3Offset;
		}
	}
	#endregion

	#region Response Methods
	public void vArrowDown(int p_key)
	{
		switch (p_key)
		{
			case 0:
				{
					m_fLeftPressed = true;
				}break;
			case 1:
				{
					m_fRightPressed = true;
				}break;
			case 2:
				{
					m_fUpPressed = true;
				}break;
			case 3:
				{
					m_fDownPressed = true;
				}break;
		}
	}
	public void vArrowUp(int p_key)
	{
		switch (p_key)
		{
			case 0:
				{
					m_fLeftPressed = false;
				} break;
			case 1:
				{
					m_fRightPressed = false;
				} break;
			case 2:
				{
					m_fUpPressed = false;
				} break;
			case 3:
				{
					m_fDownPressed = false;
				} break;
		}
	}
	#endregion
}
