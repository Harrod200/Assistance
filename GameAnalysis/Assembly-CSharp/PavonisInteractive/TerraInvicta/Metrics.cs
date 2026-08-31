using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006EB RID: 1771
	public static class Metrics
	{
		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x06002934 RID: 10548 RVA: 0x000DBC82 File Offset: 0x000D9E82
		public static float lastFrametime
		{
			get
			{
				return TIFrameTiming.lastFrametime;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x06002935 RID: 10549 RVA: 0x000DBC89 File Offset: 0x000D9E89
		public static float lastFramerate
		{
			get
			{
				return 1f / TIFrameTiming.lastFrametime;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x06002936 RID: 10550 RVA: 0x000DBC96 File Offset: 0x000D9E96
		public static double secondsSinceStartOfUpdate
		{
			get
			{
				return TIFrameTiming.secondsSinceStartOfUpdate;
			}
		}

		// Token: 0x06002937 RID: 10551 RVA: 0x000DBC9D File Offset: 0x000D9E9D
		public static float GetAverageFrametime(int frameCount)
		{
			return TIFrameTiming.GetAverageFrametime(frameCount);
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x000DBCA5 File Offset: 0x000D9EA5
		public static float GetAverageFramerate(int frameCount)
		{
			return 1f / TIFrameTiming.GetAverageFrametime(frameCount);
		}
	}
}
