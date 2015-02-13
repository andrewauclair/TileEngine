using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
	public enum AssertType
	{
		Equal = 0,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public class AssertAttribute : PropertyAttribute
	{
		public readonly int m_nValue;

		public AssertAttribute(int p_nValue, AssertType p_eType)
		{
			int t_nActual = p_nValue;
			
			switch (p_eType)
			{
				case AssertType.Equal:
					{
						Match(4);
					}break;
			}
		}

		public void OnValidate()
		{
			
		}
	}
}