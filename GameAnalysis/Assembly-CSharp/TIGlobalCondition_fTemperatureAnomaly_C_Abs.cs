using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020000C2 RID: 194
public class TIGlobalCondition_fTemperatureAnomaly_C_Abs : TIGlobalCondition
{
	// Token: 0x06000393 RID: 915 RVA: 0x00013096 File Offset: 0x00011296
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, Mathf.Abs(TIGlobalValuesState.GlobalValues.temperatureAnomaly_C), TIUtilities.GetFloatValue(this.strValue));
	}
}
