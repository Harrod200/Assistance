using System;
using System.Collections.Generic;

// Token: 0x02000407 RID: 1031
public static class IntListExtension
{
	// Token: 0x0600152C RID: 5420 RVA: 0x000671F4 File Offset: 0x000653F4
	public static List<int> GetBitIndices(this int value)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < 32; i++)
		{
			if ((value & (1 << i)) != 0)
			{
				list.Add(i);
			}
		}
		return list;
	}
}
