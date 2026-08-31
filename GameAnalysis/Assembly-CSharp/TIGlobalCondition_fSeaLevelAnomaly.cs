using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CE RID: 206
public class TIGlobalCondition_fSeaLevelAnomaly : TIGlobalCondition
{
	// Token: 0x060003AB RID: 939 RVA: 0x000132E9 File Offset: 0x000114E9
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.globalSeaLevelAnomaly_cm, TIUtilities.GetFloatValue(this.strValue));
	}
}
