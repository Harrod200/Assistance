using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000770 RID: 1904
	public abstract class TIRegionAlienAssetState : TIRegionAlienEntityState
	{
		// Token: 0x060039CE RID: 14798
		public abstract string ResolveAssault(TIGameState assaultingState, TIFactionState assaultingFaction, TIMissionOutcome outcome);

		// Token: 0x060039CF RID: 14799
		public abstract List<CampaignMilestone> CampaignMilestonesGrantedOnCapture(TIFactionState capturingFaction, TIMissionOutcome outcome);

		// Token: 0x170009FE RID: 2558
		// (get) Token: 0x060039D0 RID: 14800 RVA: 0x00155C8A File Offset: 0x00153E8A
		public override bool isRegionAlienAsset
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x060039D1 RID: 14801 RVA: 0x00155C8D File Offset: 0x00153E8D
		public override TIRegionAlienAssetState ref_regionAlienAsset
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060039D2 RID: 14802
		public abstract float GetArmyAssaultDefenseScore();

		// Token: 0x060039D3 RID: 14803 RVA: 0x00155C90 File Offset: 0x00153E90
		public virtual string GetDestroyedIllustrationPath()
		{
			return "illustrations/Mission_AssaultAlienAsset";
		}

		// Token: 0x060039D4 RID: 14804 RVA: 0x00155C97 File Offset: 0x00153E97
		public bool UnderArmyAssault()
		{
			return base.region.armies.Any<TIArmyState>((TIArmyState x) => x.CurrentOperations().Count > 0 && x.CurrentOperations()[0].target == this && x.CurrentOperations()[0].operation is AssaultAlienAssetOperation);
		}
	}
}
