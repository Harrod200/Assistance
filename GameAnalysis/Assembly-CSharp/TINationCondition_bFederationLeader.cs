using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000063 RID: 99
public class TINationCondition_bFederationLeader : TINationCondition
{
	// Token: 0x0600027F RID: 639 RVA: 0x00010EC0 File Offset: 0x0000F0C0
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000280 RID: 640 RVA: 0x00010EC8 File Offset: 0x0000F0C8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && state.ref_nation.inFederation && TICondition.PassesComparison(this.sign, state.ref_nation == state.ref_nation.federation.leadNation, TIUtilities.GetBoolValue(this.strValue));
	}
}
