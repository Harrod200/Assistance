using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087D RID: 2173
	public class IntelScreenHabSiteListItem_Data
	{
		// Token: 0x0600513B RID: 20795 RVA: 0x00238544 File Offset: 0x00236744
		public void SetData(TIHabSiteState habSite)
		{
			this.habSiteState = habSite;
			this.habsiteName = this.habSiteState.displayName;
			this.spaceBodyName = this.habSiteState.ref_spaceBody.displayName;
			this.siteDescription = this.habSiteState.miningProfile.description;
			this.orbitValue = habSite.parentBody.semiMajorAxis_AU;
			TIHabState hab = this.habSiteState.hab;
			this.habNameSortString = ((hab != null) ? hab.displayName : null) ?? "";
			if (habSite.parentBody.isaMoon)
			{
				this.orbitValue = habSite.parentBody.semiMajorAxis_AU + habSite.parentBody.barycenter.semiMajorAxis_AU;
			}
			if (habSite.ref_spaceBody.barycenter.isEarth || habSite.ref_spaceBody.isEarth)
			{
				this.hasLaunchWindow = false;
				return;
			}
			this.hasLaunchWindow = true;
			double num;
			TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(this.controller.activePlayer, GameStateManager.Earth(), habSite.ref_spaceBody, TITimeState.Now(), out num);
			bool flag;
			double hohmannTimePenaltyFraction = TISpaceObjectState.GetHohmannTimePenaltyFraction(this.controller.activePlayer, nextHohmannLaunchWindowDate, num, out flag);
			this.launchWindowCloserToPrior = flag;
			this.launchWindowPenalty = hohmannTimePenaltyFraction.ToPercent("P0");
			this.launchWindowSort = hohmannTimePenaltyFraction * (double)(flag ? (-1) : 1);
		}

		// Token: 0x04003549 RID: 13641
		public bool showInList;

		// Token: 0x0400354A RID: 13642
		public IntelScreenController controller;

		// Token: 0x0400354B RID: 13643
		public TIHabSiteState habSiteState;

		// Token: 0x0400354C RID: 13644
		public string habsiteName;

		// Token: 0x0400354D RID: 13645
		public string spaceBodyName;

		// Token: 0x0400354E RID: 13646
		public string siteDescription;

		// Token: 0x0400354F RID: 13647
		public double orbitValue;

		// Token: 0x04003550 RID: 13648
		public string habNameSortString;

		// Token: 0x04003551 RID: 13649
		public string launchWindowPenalty;

		// Token: 0x04003552 RID: 13650
		public double launchWindowSort;

		// Token: 0x04003553 RID: 13651
		public bool launchWindowCloserToPrior;

		// Token: 0x04003554 RID: 13652
		public bool hasLaunchWindow;
	}
}
