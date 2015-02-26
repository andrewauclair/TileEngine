using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CChunkData : MonoBehaviour
{

	[HideInInspector]
	public List<CLayer> m_lstLayers = new List<CLayer>();

	void OnEnable()
	{
		// add the layers we want to always have
		if (m_lstLayers.Count == 0)
		{
			CLayer t_bgLayer = new CLayer();
			t_bgLayer.m_strName = "Background";
			t_bgLayer.m_fEditable = false;
			t_bgLayer.m_fMoveable = false;
			t_bgLayer.m_fTilesAllowed = true;

			CLayer t_playerLayer = new CLayer();
			t_playerLayer.m_strName = "Player";
			t_playerLayer.m_fEditable = false;
			t_playerLayer.m_fMoveable = true;
			t_playerLayer.m_fTilesAllowed = false;

			CLayer t_collisionLayer = new CLayer();
			t_collisionLayer.m_strName = "Collision";
			t_collisionLayer.m_fEditable = false;
			t_collisionLayer.m_fMoveable = true;
			t_collisionLayer.m_fTilesAllowed = false;

			m_lstLayers.Add(t_bgLayer);
			m_lstLayers.Add(t_playerLayer);
			m_lstLayers.Add(t_collisionLayer);
		}
	}
}
