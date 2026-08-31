using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000142 RID: 322
public class TIArmyCondition_bInFriendlyRegion : TIArmyCondition
{
	// Token: 0x060004D0 RID: 1232 RVA: 0x00015C03 File Offset: 0x00013E03
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00015C0B File Offset: 0x00013E0B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.InFriendlyRegion, TIUtilities.GetBoolValue(this.strValue));
	}
}
