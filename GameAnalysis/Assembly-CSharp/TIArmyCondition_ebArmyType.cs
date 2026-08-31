using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000146 RID: 326
public class TIArmyCondition_ebArmyType : TIArmyCondition
{
	// Token: 0x060004DB RID: 1243 RVA: 0x00015D0B File Offset: 0x00013F0B
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00015D14 File Offset: 0x00013F14
	public override bool PassesCondition(TIGameState state)
	{
		ArmyType armyType = this.strIdx.ToEnum(ArmyType.Human);
		return state.ref_army != null && TICondition.PassesComparison(this.sign, state.ref_army.armyType == armyType, TIUtilities.GetBoolValue(this.strValue));
	}
}
