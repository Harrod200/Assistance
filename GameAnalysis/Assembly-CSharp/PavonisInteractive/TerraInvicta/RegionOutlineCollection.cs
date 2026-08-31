using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000562 RID: 1378
	[Serializable]
	public class RegionOutlineCollection : ScriptableObject
	{
		// Token: 0x0600248F RID: 9359 RVA: 0x000C3B86 File Offset: 0x000C1D86
		public RegionOutlineCollection()
		{
			this.regionOutlines = new List<TIRegionOutline>();
		}

		// Token: 0x04001B88 RID: 7048
		public string collectionName;

		// Token: 0x04001B89 RID: 7049
		public float width;

		// Token: 0x04001B8A RID: 7050
		public float height;

		// Token: 0x04001B8B RID: 7051
		public bool overrideQuality;

		// Token: 0x04001B8C RID: 7052
		public float quality;

		// Token: 0x04001B8D RID: 7053
		public List<TIRegionOutline> regionOutlines;
	}
}
