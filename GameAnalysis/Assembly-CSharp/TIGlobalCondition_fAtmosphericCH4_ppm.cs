using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C4 RID: 196
public class TIGlobalCondition_fAtmosphericCH4_ppm : TIGlobalCondition
{
	// Token: 0x06000397 RID: 919 RVA: 0x000130EF File Offset: 0x000112EF
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.earthAtmosphericCH4_ppm, TIUtilities.GetFloatValue(this.strValue));
	}
}
