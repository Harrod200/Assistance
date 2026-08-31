using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000140 RID: 320
public class TIArmyCondition_bOccupyingRegion : TIArmyCondition
{
	// Token: 0x060004CA RID: 1226 RVA: 0x00015B7B File Offset: 0x00013D7B
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x00015B83 File Offset: 0x00013D83
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.OccupyingRegion(true), TIUtilities.GetBoolValue(this.strValue));
	}
}
