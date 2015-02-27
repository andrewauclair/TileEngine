using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CChunk))]
public class CChunkEditor : Editor
{
	void OnDestroy()
	{
		Debug.Log("destroyed chunk");
		if (Application.isEditor)
		{
			CChunk t_chunk = (CChunk)target;

			if (t_chunk.m_mesh != null)
			{
				DestroyImmediate(t_chunk.m_mesh);
			}
		}
	}
}
