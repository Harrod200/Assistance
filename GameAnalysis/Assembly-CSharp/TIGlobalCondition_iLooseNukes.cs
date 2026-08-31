using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C9 RID: 201
public class TIGlobalCondition_iLooseNukes : TIGlobalCondition
{
	// Token: 0x060003A1 RID: 929 RVA: 0x000131C4 File Offset: 0x000113C4
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, TIGlobalValuesState.GlobalValues.looseNukes, TIUtilities.GetIntValue(this.strValue));
	}
}
