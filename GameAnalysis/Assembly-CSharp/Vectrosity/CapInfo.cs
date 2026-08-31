using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x02000497 RID: 1175
	public class CapInfo
	{
		// Token: 0x06001929 RID: 6441 RVA: 0x000819A0 File Offset: 0x0007FBA0
		public CapInfo(EndCap capType, Texture texture, float ratio1, float ratio2, float offset1, float offset2, float scale1, float scale2, float[] uvHeights)
		{
			this.capType = capType;
			this.texture = texture;
			this.ratio1 = ratio1;
			this.ratio2 = ratio2;
			this.offset1 = offset1;
			this.offset2 = offset2;
			this.scale1 = scale1;
			this.scale2 = scale2;
			this.uvHeights = uvHeights;
		}

		// Token: 0x04001634 RID: 5684
		public EndCap capType;

		// Token: 0x04001635 RID: 5685
		public Texture texture;

		// Token: 0x04001636 RID: 5686
		public float ratio1;

		// Token: 0x04001637 RID: 5687
		public float ratio2;

		// Token: 0x04001638 RID: 5688
		public float offset1;

		// Token: 0x04001639 RID: 5689
		public float offset2;

		// Token: 0x0400163A RID: 5690
		public float scale1;

		// Token: 0x0400163B RID: 5691
		public float scale2;

		// Token: 0x0400163C RID: 5692
		public float[] uvHeights;
	}
}
