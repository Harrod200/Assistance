using System;
using Steamworks;
using UnityEngine;

namespace LapinerTools.Steam.Data
{
	// Token: 0x02000541 RID: 1345
	[Serializable]
	public class WorkshopSortMode
	{
		// Token: 0x06002266 RID: 8806 RVA: 0x000B25E4 File Offset: 0x000B07E4
		public WorkshopSortMode()
		{
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x000B25EC File Offset: 0x000B07EC
		public WorkshopSortMode(EUGCQuery p_mode)
		{
			this.MODE = p_mode;
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x000B25FB File Offset: 0x000B07FB
		public WorkshopSortMode(EWorkshopSource p_source)
		{
			this.SOURCE = p_source;
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x000B260A File Offset: 0x000B080A
		public WorkshopSortMode(EUGCQuery p_mode, EWorkshopSource p_source)
		{
			this.MODE = p_mode;
			this.SOURCE = p_source;
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x000B2620 File Offset: 0x000B0820
		public override bool Equals(object p_other)
		{
			return p_other != null && p_other is WorkshopSortMode && p_other.GetHashCode() == this.GetHashCode();
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x000B2640 File Offset: 0x000B0840
		public override int GetHashCode()
		{
			return ((int)(this.MODE + (int)((EWorkshopSource)100 * this.SOURCE))).GetHashCode();
		}

		// Token: 0x04001A2A RID: 6698
		[SerializeField]
		public EUGCQuery MODE;

		// Token: 0x04001A2B RID: 6699
		[SerializeField]
		public EWorkshopSource SOURCE;
	}
}
