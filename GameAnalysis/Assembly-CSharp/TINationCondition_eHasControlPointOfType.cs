using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000046 RID: 70
public class TINationCondition_eHasControlPointOfType : TINationCondition
{
	// Token: 0x06000239 RID: 569 RVA: 0x000104AC File Offset: 0x0000E6AC
	public override bool PassesCondition(TIGameState state)
	{
		ControlPointType controlPointType;
		Enum.TryParse<ControlPointType>(this.strValue, out controlPointType);
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.GetControlPointOfType(controlPointType) != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
