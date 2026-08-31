using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CF RID: 207
public class TIGlobalCondition_fStartTime_temperature : TIGlobalCondition
{
	// Token: 0x060003AD RID: 941 RVA: 0x00013314 File Offset: 0x00011514
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.temperatureAnomaly_C_startTime, TIUtilities.GetFloatValue(this.strValue));
	}
}
