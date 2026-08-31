using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F6 RID: 2038
	public static class IntExtensions
	{
		// Token: 0x060049FC RID: 18940 RVA: 0x001F1032 File Offset: 0x001EF232
		public static int EuclidianModulo(int i, int i_max)
		{
			return (i % i_max + i_max) % i_max;
		}
	}
}
