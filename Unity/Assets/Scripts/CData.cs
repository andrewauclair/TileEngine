using System;
using UnityEngine;

public class CTile
{
	public byte m_neighborMask = 0;

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

	public CTile tileTopLeft()
	{
		CTile t_tile = new CTile();

		float t_rUVWidth = UV2.x - UV1.x;
		float t_rUVHeight = UV2.y - UV1.y;

		t_tile.UV1 = new Vector2(UV1.x, UV1.y + t_rUVHeight / 2.0f);
		t_tile.UV2 = new Vector2(UV2.x - t_rUVWidth / 2.0f, UV2.y);
		t_tile.UV3 = new Vector2(UV3.x - t_rUVWidth / 2.0f, UV3.y + t_rUVHeight / 2.0f);
		t_tile.UV4 = UV4;

		return t_tile;
	}
	public CTile tileTopRight()
	{
		CTile t_tile = new CTile();

		float t_rUVWidth = UV2.x - UV1.x;
		float t_rUVHeight = UV2.y - UV1.y;

		t_tile.UV1 = new Vector2(UV1.x + t_rUVWidth / 2.0f, UV1.y + t_rUVHeight / 2.0f);
		t_tile.UV2 = UV2;
		t_tile.UV3 = new Vector2(UV3.x, UV3.y + t_rUVHeight / 2.0f);
		t_tile.UV4 = new Vector2(UV4.x + t_rUVWidth / 2.0f, UV4.y);

		return t_tile;
	}
	public CTile tileBottomLeft()
	{
		CTile t_tile = new CTile();

		float t_rUVWidth = UV2.x - UV1.x;
		float t_rUVHeight = UV2.y - UV1.y;

		t_tile.UV1 = UV1;
		t_tile.UV2 = new Vector2(UV2.x - t_rUVWidth / 2.0f, UV2.y - t_rUVHeight / 2.0f);
		t_tile.UV3 = new Vector2(UV3.x - t_rUVWidth / 2.0f, UV3.y);
		t_tile.UV4 = new Vector2(UV4.x, UV4.y - t_rUVHeight / 2.0f);

		return t_tile;
	}
	public CTile tileBottomRight()
	{
		CTile t_tile = new CTile();

		float t_rUVWidth = UV2.x - UV1.x;
		float t_rUVHeight = UV2.y - UV1.y;

		t_tile.UV1 = new Vector2(UV1.x + t_rUVWidth / 2.0f, UV1.y);
		t_tile.UV2 = new Vector2(UV2.x, UV2.y - t_rUVHeight / 2.0f);
		t_tile.UV3 = UV3;
		t_tile.UV4 = new Vector2(UV4.x + t_rUVWidth / 2.0f, UV4.y - t_rUVHeight / 2.0f);

		return t_tile;
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
