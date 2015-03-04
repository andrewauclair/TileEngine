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
	[HideInInspector]
	public Vector2 v2Position = Vector2.zero;

	[HideInInspector]
	public int nLayer = 0;

	[HideInInspector][SerializeField]
	public Mesh m_mesh = null;
    #endregion

    #region Private Data

	[SerializeField][HideInInspector]
	private List<int> m_lstData = new List<int>();
    #endregion

    #region Unity Methods
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		int t_nChunkSize = CChunkGenerator.msc_nChunkSize;

		Gizmos.color = Color.white;
		
		float t_rHalfChunk = t_nChunkSize / 2.0f;
		float t_x = transform.position.x;
		float t_y = transform.position.y;

		Vector3 t_v3TopLeft = new Vector3(t_x - t_rHalfChunk, t_y + t_rHalfChunk, 0f);
		Vector3 t_v3TopRight = new Vector3(t_x + t_rHalfChunk, t_y + t_rHalfChunk, 0f);
		Vector3 t_v3BottomLeft = new Vector3(t_x - t_rHalfChunk, t_y - t_rHalfChunk, 0f);
		Vector3 t_v3BottomRight = new Vector3(t_x + t_rHalfChunk, t_y - t_rHalfChunk, 0f);

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
		m_lstData = new List<int>(p_lstData);
	}
	public void vSetTile(Vector2 p_v2Pos, CTile p_Tile)
	{
#if UNITY_EDITOR
		m_lstData[(int)((p_v2Pos.y * CChunkGenerator.msc_nChunkSize) + p_v2Pos.x)] = p_Tile.Tile;
#endif
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
	public List<int> lstData()
	{
		return m_lstData;
	}
    #endregion

    #region Private Methods
    #endregion
}
