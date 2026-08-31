using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C1 RID: 193
public class TIGlobalCondition_fTemperatureAnomaly_C : TIGlobalCondition
{
	// Token: 0x06000391 RID: 913 RVA: 0x0001306C File Offset: 0x0001126C
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.temperatureAnomaly_C, TIUtilities.GetFloatValue(this.strValue));
	}
}
