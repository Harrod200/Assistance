using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CA RID: 202
public class TIGlobalCondition_bLooseNukes : TIGlobalCondition
{
	// Token: 0x060003A3 RID: 931 RVA: 0x000131EE File Offset: 0x000113EE
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.looseNukes > 0, TIUtilities.GetBoolValue(this.strValue));
	}
}
