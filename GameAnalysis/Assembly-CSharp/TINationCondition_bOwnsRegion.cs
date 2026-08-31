using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000067 RID: 103
public class TINationCondition_bOwnsRegion : TINationCondition
{
	// Token: 0x06000289 RID: 649 RVA: 0x00010FEE File Offset: 0x0000F1EE
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x0600028A RID: 650 RVA: 0x00010FF6 File Offset: 0x0000F1F6
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { GameStateManager.FindByTemplate<TIRegionState>(this.strIdx, false).displayName };
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00011018 File Offset: 0x0000F218
	public override bool PassesCondition(TIGameState state)
	{
		TIRegionState tiregionState = GameStateManager.FindByTemplate<TIRegionState>(this.strIdx, false);
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.regions.Contains(tiregionState), TIUtilities.GetBoolValue(this.strValue));
	}
}
