using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

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
	public static CWorldManager Instance
	{
		get
		{
			if (m_Instance)
			{
				return m_Instance;
			}

			m_Instance = null;
			m_Instance = FindObjectOfType<CWorldManager>();

			if (m_Instance)
			{
				return m_Instance;
			}

			Debug.LogError("No instance of CWorldManager found.");
			return null;
		}
	}
	protected static CWorldManager m_Instance = null;
    #endregion

    #region Public Data
    #endregion

    #region Private Data
	[HideInInspector]
	public List<CChunk> m_lstChunks = new List<CChunk>();
	
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
	private CCharacter m_character = null;
    #endregion

    #region Unity Methods
    void Awake()
    {
		m_Instance = GetComponent<CWorldManager>();

		GameObject t_goCharacter = GameObject.FindGameObjectWithTag("Player");
		m_character = t_goCharacter.GetComponent<CCharacter>();

		CChunk[] t_aChunks = transform.GetComponentsInChildren<CChunk>();
		m_lstChunks = new List<CChunk>(t_aChunks);
    }
    void Start()
    {
		GameObject t_camera = GameObject.FindWithTag("MainCamera");
		t_camera.hideFlags |= HideFlags.NotEditable;
	}
	void Update()
    {
		m_fLeftPressed = m_fLeftInput || Input.GetKey(KeyCode.LeftArrow);
		m_fRightPressed = m_fRightInput || Input.GetKey(KeyCode.RightArrow);
		m_fUpPressed = m_fUpInput || Input.GetKey(KeyCode.UpArrow);
		m_fDownPressed = m_fDownInput || Input.GetKey(KeyCode.DownArrow);
		
		if (m_fLeftPressed && !m_fMoving)
		{
			m_character.Animator.SetInteger("Direction", 1);
			if (!Physics.Raycast(transform.position, Vector3.left, .75f))
			{
				m_fMoving = true;
				m_character.Animator.SetBool("Idle", false);
				m_v3Start = transform.position;
				m_v3Target = new Vector3(m_v3Start.x + 1, m_v3Start.y, 0f);
				m_v3Current = m_v3Start;
			}
		}
		if (m_fRightPressed && !m_fMoving)
		{
			m_character.Animator.SetInteger("Direction", 3);
			if (!Physics.Raycast(transform.position, Vector3.right, .75f))
			{
				m_fMoving = true;
				m_character.Animator.SetBool("Idle", false);
				m_v3Start = transform.position;
				m_v3Target = new Vector3(m_v3Start.x - 1, m_v3Start.y, 0f);
				m_v3Current = m_v3Start;
			}
		}
		if (m_fUpPressed && !m_fMoving)
		{
			m_character.Animator.SetInteger("Direction", 2);
			if (!Physics.Raycast(transform.position, Vector3.up, .75f))
			{
				m_fMoving = true;
				m_character.Animator.SetBool("Idle", false);
				m_v3Start = transform.position;
				m_v3Target = new Vector3(m_v3Start.x, m_v3Start.y - 1, 0f);
				m_v3Current = m_v3Start;
			}
		}
		if (m_fDownPressed && !m_fMoving)
		{
			m_character.Animator.SetInteger("Direction", 0);
			if (!Physics.Raycast(transform.position, Vector3.down, .75f))
			{
				m_fMoving = true;
				m_character.Animator.SetBool("Idle", false);
				m_v3Start = transform.position;
				m_v3Target = new Vector3(m_v3Start.x, m_v3Start.y + 1, 0f);
				m_v3Current = m_v3Start;
			}
		}

		if (m_fMoving)
		{
			Vector3 t_v3Offset = Vector3.zero;

			m_rTime += Time.smoothDeltaTime;
			
			Vector3 t_v3Pos = Vector3.Lerp(m_v3Start, m_v3Target, Mathf.Min(m_rTime / m_character.rMoveTime, 1.0f));

			t_v3Offset = t_v3Pos - m_v3Current;
			m_v3Current = t_v3Pos;

			vMoveChunks(t_v3Offset);

			if (m_rTime >= m_character.rMoveTime)
			{
				m_fMoving = false;
				m_rTime = 0f;
				m_character.Animator.SetBool("Idle", true);
			}
		}
    }
	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;

		Vector3 t_v3TopLeft = new Vector3(-.5f, .5f, 0f);
		Vector3 t_v3TopRight = new Vector3(.5f, .5f, 0f);
		Vector3 t_v3BottomLeft = new Vector3(-.5f, -.5f, 0f);
		Vector3 t_v3BottomRight = new Vector3(.5f, -.5f, 0f);

		Gizmos.DrawLine(t_v3TopLeft, t_v3TopRight);
		Gizmos.DrawLine(t_v3TopRight, t_v3BottomRight);
		Gizmos.DrawLine(t_v3BottomRight, t_v3BottomLeft);
		Gizmos.DrawLine(t_v3BottomLeft, t_v3TopLeft);
	}
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
	private void vMoveChunks(Vector3 p_v3Offset)
	{
		for (int t_i = 0; t_i < m_lstChunks.Count; ++t_i)
		{
			m_lstChunks[t_i].transform.position += p_v3Offset;
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
