using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C3 RID: 195
public class TIGlobalCondition_fAtmosphericCO2_ppm : TIGlobalCondition
{
	// Token: 0x06000395 RID: 917 RVA: 0x000130C5 File Offset: 0x000112C5
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.earthAtmosphericCO2_ppm, TIUtilities.GetFloatValue(this.strValue));
	}
}
