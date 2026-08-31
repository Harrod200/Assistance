using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200021C RID: 540
public class TIMissionModifier_ExtractionDefense_Councilor : TIMissionModifier_StatBased
{
	// Token: 0x17000100 RID: 256
	// (get) Token: 0x0600073B RID: 1851 RVA: 0x00022C71 File Offset: 0x00020E71
	public override string displayName
	{
		get
		{
			return string.Format(Loc.T(base.displayName), TIUtilities.GetAttributeString(this.defenderAttribute));
		}
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00022C90 File Offset: 0x00020E90
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState tifactionState;
		if (target == null)
		{
			tifactionState = null;
		}
		else
		{
			TICouncilorState ref_councilor = target.ref_councilor;
			tifactionState = ((ref_councilor != null) ? ref_councilor.detainingFaction : null);
		}
		TIFactionState tifactionState2 = tifactionState;
		float num = 0f;
		if (tifactionState2 != null)
		{
			foreach (TICouncilorState ticouncilorState in tifactionState2.activeCouncilors)
			{
				if (TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, ticouncilorState) == TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, target.ref_councilor))
				{
					num += (float)ticouncilorState.GetAttribute(this.defenderAttribute, true, true, true, false, false, false);
				}
			}
		}
		return num;
	}
}
