using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200013F RID: 319
public class TIArmyCondition_bInBattle : TIArmyCondition
{
	// Token: 0x060004C7 RID: 1223 RVA: 0x00015B38 File Offset: 0x00013D38
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00015B40 File Offset: 0x00013D40
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.InBattleWithArmies(), TIUtilities.GetBoolValue(this.strValue));
	}
}
