using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B2 RID: 178
public class TIFactionCondition_bBelievers : TIFactionCondition
{
	// Token: 0x06000366 RID: 870 RVA: 0x00012A08 File Offset: 0x00010C08
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000367 RID: 871 RVA: 0x00012A10 File Offset: 0x00010C10
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.believers, TIUtilities.GetBoolValue(this.strValue));
	}
}
