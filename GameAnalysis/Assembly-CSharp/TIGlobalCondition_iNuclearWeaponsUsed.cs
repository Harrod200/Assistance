using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C7 RID: 199
public class TIGlobalCondition_iNuclearWeaponsUsed : TIGlobalCondition
{
	// Token: 0x0600039D RID: 925 RVA: 0x0001316D File Offset: 0x0001136D
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.nuclearStrikes, TIUtilities.GetIntValue(this.strValue));
	}
}
