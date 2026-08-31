using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000110 RID: 272
public class TIHabCondition_bWarDogContainment : TIHabCondition
{
	// Token: 0x06000452 RID: 1106 RVA: 0x00014CBC File Offset: 0x00012EBC
	public override bool PassesCondition(TIGameState state)
	{
		bool flag = state.ref_hab.GetNetTechBonusByFaction(TechCategory.LifeScience, state.ref_faction, false) + state.ref_hab.GetNetTechBonusByFaction(TechCategory.Materials, state.ref_faction, false) + state.ref_hab.GetNetTechBonusByFaction(TechCategory.MilitaryScience, state.ref_faction, false) + state.ref_hab.GetNetTechBonusByFaction(TechCategory.Xenology, state.ref_faction, false) >= 0.1f;
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, flag, TIUtilities.GetBoolValue(this.strValue));
	}
}
