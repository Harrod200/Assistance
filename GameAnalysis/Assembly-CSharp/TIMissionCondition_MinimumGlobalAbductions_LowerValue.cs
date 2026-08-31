using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B8 RID: 440
public class TIMissionCondition_MinimumGlobalAbductions_LowerValue : TIMissionCondition
{
	// Token: 0x0600064D RID: 1613 RVA: 0x0001CA84 File Offset: 0x0001AC84
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TIFactionState faction = councilor.faction;
		if (!faction.IsAlienFaction || faction.abductions >= TemplateManager.global.globalAbductionsThreshhold_Lower)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
