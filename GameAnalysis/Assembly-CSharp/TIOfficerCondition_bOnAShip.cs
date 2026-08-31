using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200013A RID: 314
public class TIOfficerCondition_bOnAShip : TIOfficerCondition
{
	// Token: 0x060004BA RID: 1210 RVA: 0x00015A1C File Offset: 0x00013C1C
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x00015A24 File Offset: 0x00013C24
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, TIGameState.Valid(state.ref_officer.ship), TIUtilities.GetBoolValue(this.strValue));
	}
}
