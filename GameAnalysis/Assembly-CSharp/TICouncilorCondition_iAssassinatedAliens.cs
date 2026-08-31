using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F0 RID: 240
public class TICouncilorCondition_iAssassinatedAliens : TIFactionCondition
{
	// Token: 0x17000085 RID: 133
	// (get) Token: 0x0600040A RID: 1034 RVA: 0x0001437E File Offset: 0x0001257E
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x00014394 File Offset: 0x00012594
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor.assassinations.Keys.Contains(GameStateManager.AlienFaction()) && TICondition.PassesComparison(this.sign, state.ref_councilor.assassinations[GameStateManager.AlienFaction()], TIUtilities.GetIntValue(this.strValue));
	}
}
