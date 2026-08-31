using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C6 RID: 198
public class TIGlobalCondition_fStratosphericAerosols_ppm : TIGlobalCondition
{
	// Token: 0x0600039B RID: 923 RVA: 0x00013143 File Offset: 0x00011343
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.stratosphericAerosols_ppm, TIUtilities.GetFloatValue(this.strValue));
	}
}
