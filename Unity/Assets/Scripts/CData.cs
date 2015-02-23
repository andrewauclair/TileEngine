using System;
using UnityEngine;

public class CTile
{
	private JSONObject m_JSON;

	public CTile()
	{
		m_JSON = new JSONObject(JSONObject.Type.OBJECT);
	}
	public CTile(JSONObject p_JSON)
	{
		m_JSON = p_JSON;
	}
	public int Tile
	{
		get { return m_JSON.Get("tile", 0); }
		set { m_JSON.SetField("tile", value); }
	}

	public Vector2 UV1
	{
		get 
		{ 
			return new Vector2(
				m_JSON.Get("uv1x", 0f),
				m_JSON.Get("uv1y", 0f));
		}
		set
		{
			m_JSON.SetField("uv1x", value.x);
			m_JSON.SetField("uv1y", value.y);
		}
	}
	public Vector2 UV2
	{
		get
		{
			return new Vector2(
				m_JSON.Get("uv2x", 0f),
				m_JSON.Get("uv2y", 0f));
		}
		set
		{
			m_JSON.SetField("uv2x", value.x);
			m_JSON.SetField("uv2y", value.y);
		}
	}
	public Vector2 UV3
	{
		get
		{
			return new Vector2(
				m_JSON.Get("uv3x", 0f),
				m_JSON.Get("uv3y", 0f));
		}
		set
		{
			m_JSON.SetField("uv3x", value.x);
			m_JSON.SetField("uv3y", value.y);
		}
	}
	public Vector2 UV4
	{
		get
		{
			return new Vector2(
				m_JSON.Get("uv4x", 0f),
				m_JSON.Get("uv4y", 0f));
		}
		set
		{
			m_JSON.SetField("uv4x", value.x);
			m_JSON.SetField("uv4y", value.y);
		}
	}
	public JSONObject ToJSON()
	{
		return m_JSON;
	}
	public override string ToString()
	{
		return ToJSON().ToString();
	}
}

[Serializable]
public class CLayer
{
	public int m_nLayer = 0;
	public string m_strName = "";
	public bool m_fEditable = true;
	public bool m_fMoveable = true;
	public bool m_fTilesAllowed = true;
}
