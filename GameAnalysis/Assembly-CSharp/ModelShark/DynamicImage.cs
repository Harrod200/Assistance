using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004AC RID: 1196
	public class DynamicImage : MonoBehaviour
	{
		// Token: 0x1700039B RID: 923
		// (get) Token: 0x06001AE0 RID: 6880 RVA: 0x0009173C File Offset: 0x0008F93C
		// (set) Token: 0x06001AE1 RID: 6881 RVA: 0x00091744 File Offset: 0x0008F944
		public string Name { get; set; }

		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06001AE2 RID: 6882 RVA: 0x0009174D File Offset: 0x0008F94D
		public Image PlaceholderImage
		{
			get
			{
				return base.GetComponent<Image>();
			}
		}

		// Token: 0x040016E1 RID: 5857
		public string placeholderName;

		// Token: 0x040016E2 RID: 5858
		[HideInInspector]
		public Image image;
	}
}
