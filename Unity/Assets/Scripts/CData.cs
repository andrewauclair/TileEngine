using System;
using UnityEngine;

[Serializable]
public struct STile
{
	public int m_nTile;

	public Vector2 m_uv1;
	public Vector2 m_uv2;
	public Vector2 m_uv3;
	public Vector2 m_uv4;

	//public string ToString()
	//{
	//    return "Tile: " + m_nTile + ", uv1: " + m_uv1 + ", uv2: " + m_uv2 + ", " + m_uv3 + ", " + m_uv4;
	//}
}