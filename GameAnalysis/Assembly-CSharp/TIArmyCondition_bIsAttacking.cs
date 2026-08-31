using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000143 RID: 323
public class TIArmyCondition_bIsAttacking : TIArmyCondition
{
	// Token: 0x060004D3 RID: 1235 RVA: 0x00015C46 File Offset: 0x00013E46
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x00015C4E File Offset: 0x00013E4E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.IsAttacking(), TIUtilities.GetBoolValue(this.strValue));
	}
}
