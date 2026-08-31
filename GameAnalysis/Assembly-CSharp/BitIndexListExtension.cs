using System;
using System.Collections.Generic;

// Token: 0x02000408 RID: 1032
public static class BitIndexListExtension
{
	// Token: 0x0600152D RID: 5421 RVA: 0x00067228 File Offset: 0x00065428
	public static int ToIntFromBitIndices(this List<int> bitIndices)
	{
		if (bitIndices == null || bitIndices.Count == 0)
		{
			return 0;
		}
		int num = 0;
		foreach (int num2 in bitIndices)
		{
			if (num2 >= 0 && num2 < 32)
			{
				num |= 1 << num2;
			}
		}
		return num;
	}
}
