using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000149 RID: 329
public class TIArmyCondition_bFactionControlled : TIArmyCondition
{
	// Token: 0x060004E5 RID: 1253 RVA: 0x00015E51 File Offset: 0x00014051
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00015E59 File Offset: 0x00014059
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.faction != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
