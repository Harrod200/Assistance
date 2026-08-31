using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000141 RID: 321
public class TIArmyCondition_bIsFighting : TIArmyCondition
{
	// Token: 0x060004CD RID: 1229 RVA: 0x00015BBF File Offset: 0x00013DBF
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x00015BC7 File Offset: 0x00013DC7
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.IsFighting(false), TIUtilities.GetBoolValue(this.strValue));
	}
}
