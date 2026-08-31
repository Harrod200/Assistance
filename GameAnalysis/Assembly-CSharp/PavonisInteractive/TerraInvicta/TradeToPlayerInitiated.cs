using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005E3 RID: 1507
	public class TradeToPlayerInitiated : GameEvent
	{
		// Token: 0x06002808 RID: 10248 RVA: 0x000D9CB7 File Offset: 0x000D7EB7
		public TradeToPlayerInitiated(TIMissionState mission, TICouncilorState targetCouncilor, TIFactionState contacted_Faction)
		{
			this.mission = mission;
			this.targetCouncilor = targetCouncilor;
			this.contacted_Faction = contacted_Faction;
		}

		// Token: 0x04001E09 RID: 7689
		public TIMissionState mission;

		// Token: 0x04001E0A RID: 7690
		public TICouncilorState targetCouncilor;

		// Token: 0x04001E0B RID: 7691
		public TIFactionState contacted_Faction;
	}
}
