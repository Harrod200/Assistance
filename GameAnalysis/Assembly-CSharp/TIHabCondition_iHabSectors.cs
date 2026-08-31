using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F7 RID: 247
public class TIHabCondition_iHabSectors : TIHabCondition_Numeric
{
	// Token: 0x0600041B RID: 1051 RVA: 0x00014540 File Offset: 0x00012740
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.sectors.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
