using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C5 RID: 197
public class TIGlobalCondition_fAtmosphericN2O_ppm : TIGlobalCondition
{
	// Token: 0x06000399 RID: 921 RVA: 0x00013119 File Offset: 0x00011319
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.earthAtmosphericN2O_ppm, TIUtilities.GetFloatValue(this.strValue));
	}
}
