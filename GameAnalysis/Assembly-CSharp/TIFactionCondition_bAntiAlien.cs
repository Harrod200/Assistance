using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B0 RID: 176
public class TIFactionCondition_bAntiAlien : TIFactionCondition
{
	// Token: 0x06000360 RID: 864 RVA: 0x00012982 File Offset: 0x00010B82
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000361 RID: 865 RVA: 0x0001298A File Offset: 0x00010B8A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.antiAlien, TIUtilities.GetBoolValue(this.strValue));
	}
}
