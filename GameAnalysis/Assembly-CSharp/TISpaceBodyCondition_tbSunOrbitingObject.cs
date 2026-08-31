using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000121 RID: 289
public class TISpaceBodyCondition_tbSunOrbitingObject : TISpaceBodyCondition
{
	// Token: 0x0600047C RID: 1148 RVA: 0x00015350 File Offset: 0x00013550
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_spaceBody != null && TICondition.PassesComparison(this.sign, state.ref_spaceBody.GetSunOrbitingRelatedObject.template == TIUtilities.GetTemplateValue<TINaturalSpaceObjectTemplate>(this.strIdx), TIUtilities.GetBoolValue(this.strValue));
	}
}
