using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000897 RID: 2199
	public class RegionListItem_Data
	{
		// Token: 0x06005306 RID: 21254 RVA: 0x0024D7B4 File Offset: 0x0024B9B4
		public void SetRegionData(TIRegionState region, bool isClaim, bool isHostileClaim_Perm, bool isHostileClaim_Temp, TINationState viewingNation)
		{
			this.regionState = region;
			this.claim = isClaim;
			this.hostileClaim_perm = isHostileClaim_Perm;
			this.hostileClaim_temp = isHostileClaim_Temp;
			this.viewingNation = viewingNation;
			this.regionNameString = new StringBuilder(region.displayName).Append("  ").Append(region.IconString(GameControl.control.activePlayer)).ToString();
			this.claimsOnRegion = (from x in region.NationsWithClaim(false, true, this.claim, false)
				orderby x.regions.Contains(region)
				select x).ToList<TINationState>();
			if (GameControl.control.activePlayer.CanCountAbductions && region.abductions > 0)
			{
				this.abductionsEnabled = true;
				this.abductionsText = ((region.abductions >= TemplateManager.global.minAbductionsinRegionForFacility) ? TIUtilities.GreenLine(region.abductions.ToString("N0")) : TIUtilities.RedLine(region.abductions.ToString("N0")));
				return;
			}
			this.abductionsEnabled = false;
		}

		// Token: 0x04003803 RID: 14339
		public TINationState viewingNation;

		// Token: 0x04003804 RID: 14340
		public bool showInList;

		// Token: 0x04003805 RID: 14341
		public TIRegionState regionState;

		// Token: 0x04003806 RID: 14342
		public bool claim;

		// Token: 0x04003807 RID: 14343
		public bool hostileClaim_perm;

		// Token: 0x04003808 RID: 14344
		public bool hostileClaim_temp;

		// Token: 0x04003809 RID: 14345
		public List<TINationState> claimsOnRegion = new List<TINationState>();

		// Token: 0x0400380A RID: 14346
		public bool abductionsEnabled;

		// Token: 0x0400380B RID: 14347
		public string abductionsText;

		// Token: 0x0400380C RID: 14348
		public string regionNameString;
	}
}
