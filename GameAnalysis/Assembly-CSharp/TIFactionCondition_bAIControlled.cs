using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000091 RID: 145
public class TIFactionCondition_bAIControlled : TIFactionCondition
{
	// Token: 0x060002F8 RID: 760 RVA: 0x00011C23 File Offset: 0x0000FE23
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.player.isAI, TIUtilities.GetBoolValue(this.strValue));
	}
}
