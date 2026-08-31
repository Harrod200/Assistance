using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000CC RID: 204
public class TIGlobalCondition_iStartTime_year : TIGlobalCondition
{
	// Token: 0x060003A7 RID: 935 RVA: 0x00013241 File Offset: 0x00011441
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, GameStateManager.Time().template.year, TIUtilities.GetIntValue(this.strValue));
	}
}
