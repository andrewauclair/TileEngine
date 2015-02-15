using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

	public float MoveTime = 1f;

	public Animator MainCharAnimator = null;

	[HideInInspector]
	public List<CTile> lstTiles = new List<CTile>();
    #endregion

    #region Private Data
	List<CChunk> m_aChunks = new List<CChunk>();
	
	private bool m_fLeftPressed = false;
	private bool m_fRightPressed = false;
	private bool m_fUpPressed = false;
	private bool m_fDownPressed = false;
	private bool m_fLeftInput = false;
	private bool m_fRightInput = false;
	private bool m_fUpInput = false;
	private bool m_fDownInput = false;
	private bool m_fMoving = false;

	private Vector3 m_v3Start = Vector3.zero;
	private Vector3 m_v3Target = Vector3.zero;
	private Vector3 m_v3Current = Vector3.zero;
	private float m_rTime = 0.0f;
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

		// load json file of uvs
		TextAsset t_Bytes = Resources.Load("JSON/" + AtlasMat.name) as TextAsset;

		if (t_Bytes == null)
		{
			Debug.Log("File JSON/'" + AtlasMat.name + "' not found");
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
		List<int> t_aTestData = new List<int>();

		for (int t_i = 0; t_i < ChunkHeight * ChunkWidth; t_i += 2)
		{
			t_aTestData.Add(0);
			t_aTestData.Add(1);
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
		m_fLeftPressed = m_fLeftInput || Input.GetKey(KeyCode.LeftArrow);
		m_fRightPressed = m_fRightInput || Input.GetKey(KeyCode.RightArrow);
		m_fUpPressed = m_fUpInput || Input.GetKey(KeyCode.UpArrow);
		m_fDownPressed = m_fDownInput || Input.GetKey(KeyCode.DownArrow);
		
		if (m_fLeftPressed && !m_fMoving)
		{
			m_fMoving = true;
			MainCharAnimator.SetInteger("Direction", 1);
			MainCharAnimator.SetBool("Idle", false);
			m_v3Start = transform.position;
			m_v3Target = new Vector3(m_v3Start.x + TileSize, m_v3Start.y, 0f);
			m_v3Current = m_v3Start;
		}
		if (m_fRightPressed && !m_fMoving)
		{
			Debug.Log("right pressed");
			m_fMoving = true;
			MainCharAnimator.SetInteger("Direction", 3);
			MainCharAnimator.SetBool("Idle", false);
			m_v3Start = transform.position;
			m_v3Target = new Vector3(m_v3Start.x - TileSize, m_v3Start.y, 0f);
			Debug.Log("Start: " + m_v3Start + "\nEnd: " + m_v3Target);
			m_v3Current = m_v3Start;
		}
		if (m_fUpPressed && !m_fMoving)
		{
			m_fMoving = true;
			MainCharAnimator.SetInteger("Direction", 2);
			MainCharAnimator.SetBool("Idle", false);
			m_v3Start = transform.position;
			m_v3Target = new Vector3(m_v3Start.x, m_v3Start.y - TileSize, 0f);
			m_v3Current = m_v3Start;
		}
		if (m_fDownPressed && !m_fMoving)
		{
			m_fMoving = true;
			MainCharAnimator.SetInteger("Direction", 0);
			MainCharAnimator.SetBool("Idle", false);
			m_v3Start = transform.position;
			m_v3Target = new Vector3(m_v3Start.x, m_v3Start.y + TileSize, 0f);
			m_v3Current = m_v3Start;
		}

		if (m_fMoving)
		{
			Vector3 t_v3Offset = Vector3.zero;

			m_rTime += Time.deltaTime;
			
			Vector3 t_v3Pos = Vector3.Lerp(m_v3Start, m_v3Target, Mathf.Min(m_rTime / MoveTime, 1.0f));

			t_v3Offset = t_v3Pos - m_v3Current;
			m_v3Current = t_v3Pos;

			vMoveChunks(t_v3Offset);

			if (m_rTime >= MoveTime)
			{
				Debug.Log("Finished moving");
				m_fMoving = false;
				m_rTime = 0f;
				MainCharAnimator.SetBool("Idle", true);
			}
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
					m_fLeftInput = true;
				}break;
			case 1:
				{
					m_fRightInput = true;
				}break;
			case 2:
				{
					m_fUpInput = true;
				}break;
			case 3:
				{
					m_fDownInput = true;
				}break;
		}
	}
	public void vArrowUp(int p_key)
	{
		switch (p_key)
		{
			case 0:
				{
					m_fLeftInput = false;
				} break;
			case 1:
				{
					m_fRightInput = false;
				} break;
			case 2:
				{
					m_fUpInput = false;
				} break;
			case 3:
				{
					m_fDownInput = false;
				} break;
		}
	}
	#endregion
}
