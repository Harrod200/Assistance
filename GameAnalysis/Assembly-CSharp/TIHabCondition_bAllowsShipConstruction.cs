using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000109 RID: 265
public class TIHabCondition_bAllowsShipConstruction : TIHabCondition
{
	// Token: 0x06000443 RID: 1091 RVA: 0x00014ABD File Offset: 0x00012CBD
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AllowsShipConstruction(null, false, false), TIUtilities.GetBoolValue(this.strValue));
	}
}
