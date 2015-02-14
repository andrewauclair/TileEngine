using System.Collections.Generic;
using System;

public class CByteStreamReader
{
	byte[] m_abytes;
	int m_i;
	
	public CByteStreamReader()
	{
		
	}
	
	public CByteStreamReader(byte[] p_abytes)
	{
		m_abytes = p_abytes;
		
		vReset();
	}
	
	public void vSetByteArray(byte[] p_abytes)
	{
		m_abytes = p_abytes;
		
		vReset();
	}
	
	public void vReset()
	{
		m_i = 0;
	}
	
	public byte byteRead()
	{
		byte t_b = m_abytes[m_i];
		m_i += 1;
		return t_b;
	}
	
	public bool boolRead()
	{
		bool t_f = BitConverter.ToBoolean(m_abytes, m_i);
		m_i += 1;
		return t_f;
	}
	
	public char charRead()
	{
		char t_char = BitConverter.ToChar(m_abytes, m_i);
		m_i += 2;
		return t_char;
	}
	
	public double doubleRead()
	{
		double t_double = BitConverter.ToDouble(m_abytes, m_i);
		m_i += 8;
		return t_double;
	}
	
	public short int16Read()
	{
		short t_n = BitConverter.ToInt16(m_abytes, m_i);
		m_i += 2;
		return t_n;
	}
	
	public int int32Read()
	{
		int t_n = BitConverter.ToInt32(m_abytes, m_i);
		m_i += 4;
		return t_n;
	}
	
	public long int64Read()
	{
		long t_n = BitConverter.ToInt64(m_abytes, m_i);
		m_i += 8;
		return t_n;
	}
	
	public float floatRead()
	{
		float t_r = BitConverter.ToSingle(m_abytes, m_i);
		m_i += 4;
		return t_r;
	}
	
	public string strRead()
	{
		ushort t_cChar = uint16Read();
		
		char[] t_aChars = new char[t_cChar];
		
		for( int t_iChar = 0; t_iChar < t_aChars.Length; ++t_iChar )
		{
			t_aChars[t_iChar] = charRead();
		}
		
		string t_str = new string(t_aChars);
		
		return t_str;
	}
	
	public ushort uint16Read()
	{
		ushort t_n = BitConverter.ToUInt16(m_abytes, m_i);
		m_i += 2;
		return t_n;
	}
	
	public uint uint32Read()
	{
		uint t_n = BitConverter.ToUInt16(m_abytes, m_i);
		m_i += 4;
		return t_n;
	}
	
	public ulong uint64Read()
	{
		ulong t_n = BitConverter.ToUInt16(m_abytes, m_i);
		m_i += 8;
		return t_n;
	}	
	
	public bool fReadCRC()
	{
		string t_str = strRead();
		
		return t_str == "BADBEEF";
	}
}

public class CByteStreamWriter
{
	private List<byte> m_listBytes;
    	
	public CByteStreamWriter()
	{
		m_listBytes = new List<byte>();
	}
	
	public CByteStreamWriter(int p_nInitialCapacity)
	{
		m_listBytes = new List<byte>(p_nInitialCapacity);
	}
	
	public void vReset()
	{
		m_listBytes.Clear();
	}
	
	public void vReset(int p_nInitialCapacity)
	{
		m_listBytes.Clear();
		m_listBytes.Capacity = p_nInitialCapacity;
	}
	
	public byte[] ToArray()
	{
		return m_listBytes.ToArray();
	}

    public int nArrayLength( )
    {
        return m_listBytes.Count;
    }

	public void vWriteByte(byte p_b)
	{
		m_listBytes.Add(p_b);
	}
	
	public void vWriteBool(bool p_f)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_f));
	}
	
	public void vWriteChar(char p_char)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_char));
	}
	
	public void vWriteDouble(double p_double)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_double));
	}
	
	public void vWriteInt16(short p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteInt32(int p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteInt64(long p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteFloat(float p_r)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_r));
	}
	
	public void vWriteStr(string p_str)
	{		
		char[] t_aChar = p_str.ToCharArray();
		
		vWriteUInt16( (ushort) t_aChar.Length );
		
		for( int t_iChar = 0; t_iChar < t_aChar.Length; ++t_iChar )
		{
			vWriteChar(t_aChar[t_iChar]);
		}
	}
	
	public void vWriteUInt16(ushort p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteUInt32(uint p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteUInt64(ulong p_n)
	{
		m_listBytes.AddRange(BitConverter.GetBytes(p_n));
	}
	
	public void vWriteCRC()
	{
		vWriteStr("BADBEEF");
	}
}
