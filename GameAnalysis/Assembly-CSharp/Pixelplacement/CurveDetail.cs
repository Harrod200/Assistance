using System;

namespace Pixelplacement
{
	// Token: 0x02000517 RID: 1303
	public struct CurveDetail
	{
		// Token: 0x06002022 RID: 8226 RVA: 0x000A6F74 File Offset: 0x000A5174
		public CurveDetail(int currentCurve, float currentCurvePercentage)
		{
			this.currentCurve = currentCurve;
			this.currentCurvePercentage = currentCurvePercentage;
		}

		// Token: 0x040018D2 RID: 6354
		public int currentCurve;

		// Token: 0x040018D3 RID: 6355
		public float currentCurvePercentage;
	}
}
