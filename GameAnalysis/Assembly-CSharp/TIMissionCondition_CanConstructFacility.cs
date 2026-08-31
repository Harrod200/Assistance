using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001B9 RID: 441
public class TIMissionCondition_CanConstructFacility : TIMissionCondition
{
	// Token: 0x170000EB RID: 235
	// (get) Token: 0x0600064F RID: 1615 RVA: 0x0001CACB File Offset: 0x0001ACCB
	public override List<string> feedback
	{
		get
		{
			return new List<string>
			{
				base.GetType().Name,
				"TIMissionCondition_CanConstructFacility1",
				"TIMissionCondition_CanConstructFacility2"
			};
		}
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0001CAFC File Offset: 0x0001ACFC
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		TIRegionState ref_region = possibleTarget.ref_region;
		if (ref_region.hasAlienFacility)
		{
			return "TIMissionCondition_CanConstructFacility";
		}
		TIFactionState totalOwningFaction = ref_region.nation.TotalOwningFaction;
		if (totalOwningFaction == null || !totalOwningFaction.permanentAlly(GameStateManager.AlienFaction()) || ref_region.abductions < TemplateManager.global.minAbductionsinRegionForFacility || ref_region.OccupiedOrOccupationUnderway())
		{
			return "TIMissionCondition_CanConstructFacility2";
		}
		if (councilor.faction.IsAlienFaction || TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, councilor.faction))
		{
			return "_Pass";
		}
		return "TIMissionCondition_CanConstructFacility1";
	}
}
