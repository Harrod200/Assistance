using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012F RID: 303
public class TISpaceShipCondition_bLaserBonusPower : TISpaceShipCondition
{
	// Token: 0x0600049F RID: 1183 RVA: 0x0001575B File Offset: 0x0001395B
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x00015763 File Offset: 0x00013963
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.GetLaserBonusPower_MJ() > 0f, TIUtilities.GetBoolValue(this.strValue));
	}
}
