using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200011A RID: 282
public class TINaturalSpaceObjectCondition_fSemiMajorAxis_AU : TINaturalSpaceObjectCondition
{
	// Token: 0x0600046C RID: 1132 RVA: 0x00015147 File Offset: 0x00013347
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_naturalSpaceObject != null && TICondition.PassesComparison(this.sign, state.ref_naturalSpaceObject.semiMajorAxis_AU, (double)TIUtilities.GetFloatValue(this.strValue));
	}
}
