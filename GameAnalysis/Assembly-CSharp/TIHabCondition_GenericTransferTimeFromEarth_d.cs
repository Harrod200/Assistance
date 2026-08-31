using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F9 RID: 249
public class TIHabCondition_GenericTransferTimeFromEarth_d : TIHabCondition_Numeric
{
	// Token: 0x0600041F RID: 1055 RVA: 0x000145BB File Offset: 0x000127BB
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(state.ref_faction, state), TIUtilities.GetFloatValue(this.strValue));
	}
}
