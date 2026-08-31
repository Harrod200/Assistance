using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005F RID: 95
public class TINationCondition_bSpaceflightProgram : TINationCondition
{
	// Token: 0x06000273 RID: 627 RVA: 0x00010D2A File Offset: 0x0000EF2A
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00010D32 File Offset: 0x0000EF32
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.spaceFlightProgram, TIUtilities.GetBoolValue(this.strValue));
	}
}
