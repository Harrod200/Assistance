using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000120 RID: 288
public class TISpaceBodyCondition_ebIsObjectType : TISpaceBodyCondition
{
	// Token: 0x0600047A RID: 1146 RVA: 0x000152FC File Offset: 0x000134FC
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_spaceBody != null && TICondition.PassesComparison(this.sign, state.ref_spaceBody.objectType == this.strIdx.ToEnum(SpaceObjectType.None), TIUtilities.GetBoolValue(this.strValue));
	}
}
