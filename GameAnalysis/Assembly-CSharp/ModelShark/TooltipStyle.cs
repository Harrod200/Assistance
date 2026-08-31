using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004AF RID: 1199
	public class TooltipStyle : MonoBehaviour
	{
		// Token: 0x06001AE9 RID: 6889 RVA: 0x000917B4 File Offset: 0x0008F9B4
		private void OnDestroy()
		{
		}

		// Token: 0x040016E6 RID: 5862
		public Sprite topLeftCorner;

		// Token: 0x040016E7 RID: 5863
		public Sprite topRightCorner;

		// Token: 0x040016E8 RID: 5864
		public Sprite bottomLeftCorner;

		// Token: 0x040016E9 RID: 5865
		public Sprite bottomRightCorner;

		// Token: 0x040016EA RID: 5866
		public Sprite topMiddle;

		// Token: 0x040016EB RID: 5867
		public Sprite bottomMiddle;

		// Token: 0x040016EC RID: 5868
		public Sprite leftMiddle;

		// Token: 0x040016ED RID: 5869
		public Sprite rightMiddle;

		// Token: 0x040016EE RID: 5870
		public int tipOffset;

		// Token: 0x040016EF RID: 5871
		public LayoutElement mainTextContainer;
	}
}
