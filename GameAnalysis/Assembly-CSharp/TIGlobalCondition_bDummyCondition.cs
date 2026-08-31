using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D6 RID: 214
public class TIGlobalCondition_bDummyCondition : TIGlobalCondition
{
	// Token: 0x060003BE RID: 958 RVA: 0x00013701 File Offset: 0x00011901
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00013709 File Offset: 0x00011909
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, true, TIUtilities.GetBoolValue(this.strValue));
	}
}
