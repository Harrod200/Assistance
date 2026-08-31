using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200013C RID: 316
public class TIOfficerCondition_ebShipLocation : TIOfficerCondition
{
	// Token: 0x060004C0 RID: 1216 RVA: 0x00015AAC File Offset: 0x00013CAC
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x00015AB4 File Offset: 0x00013CB4
	public override bool PassesCondition(TIGameState state)
	{
		ShipSystem shipSystem = this.strIdx.ToEnum(ShipSystem.None);
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, state.ref_officer.template.location == shipSystem, TIUtilities.GetBoolValue(this.strValue));
	}
}
