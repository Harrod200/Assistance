using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C8 RID: 200
public class TIGlobalCondition_bNuclearWeaponsUsed : TIGlobalCondition
{
	// Token: 0x0600039F RID: 927 RVA: 0x00013197 File Offset: 0x00011397
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.nuclearStrikes > 0, TIUtilities.GetBoolValue(this.strValue));
	}
}
