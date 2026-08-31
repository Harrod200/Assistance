using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AE RID: 174
public class TIFactionCondition_bExtremist : TIFactionCondition
{
	// Token: 0x0600035A RID: 858 RVA: 0x000128FC File Offset: 0x00010AFC
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00012904 File Offset: 0x00010B04
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.extremist, TIUtilities.GetBoolValue(this.strValue));
	}
}
