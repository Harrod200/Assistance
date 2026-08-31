using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005E RID: 94
public class TINationCondition_bExecFactionOwnsCPType : TINationCondition
{
	// Token: 0x0600026F RID: 623 RVA: 0x00010CA0 File Offset: 0x0000EEA0
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x06000270 RID: 624 RVA: 0x00010CA8 File Offset: 0x0000EEA8
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetControlPointString(this.strIdx.ToEnum(ControlPointType.none)) };
		}
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00010CC8 File Offset: 0x0000EEC8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.executiveFaction == state.ref_nation.GetControlPointTypeOwner(this.strIdx.ToEnum(ControlPointType.none)), TIUtilities.GetBoolValue(this.strValue));
	}
}
