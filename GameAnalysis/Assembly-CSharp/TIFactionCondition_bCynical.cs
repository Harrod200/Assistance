using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B1 RID: 177
public class TIFactionCondition_bCynical : TIFactionCondition
{
	// Token: 0x06000363 RID: 867 RVA: 0x000129C5 File Offset: 0x00010BC5
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000364 RID: 868 RVA: 0x000129CD File Offset: 0x00010BCD
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.cynical, TIUtilities.GetBoolValue(this.strValue));
	}
}
