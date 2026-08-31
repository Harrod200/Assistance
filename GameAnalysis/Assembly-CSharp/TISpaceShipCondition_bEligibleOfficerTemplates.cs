using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000133 RID: 307
public class TISpaceShipCondition_bEligibleOfficerTemplates : TISpaceShipCondition
{
	// Token: 0x060004A9 RID: 1193 RVA: 0x000158A0 File Offset: 0x00013AA0
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x000158A8 File Offset: 0x00013AA8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.EligibleFreeOfficerCreationTemplates().Count > 0, TIUtilities.GetBoolValue(this.strValue));
	}
}
