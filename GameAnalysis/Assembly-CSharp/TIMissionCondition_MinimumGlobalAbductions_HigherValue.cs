using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B7 RID: 439
public class TIMissionCondition_MinimumGlobalAbductions_HigherValue : TIMissionCondition
{
	// Token: 0x0600064B RID: 1611 RVA: 0x0001CA30 File Offset: 0x0001AC30
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TIFactionState faction = councilor.faction;
		if (!faction.IsAlienFaction || faction.abductions >= TemplateManager.global.globalAbductionsThreshhold_Higher || GameStateManager.AlienNation().extant)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
