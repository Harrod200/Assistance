using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009F RID: 159
public class TIFactionCondition_iAtrocities : TIFactionCondition
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000329 RID: 809 RVA: 0x0001226C File Offset: 0x0001046C
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600032A RID: 810 RVA: 0x00012281 File Offset: 0x00010481
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.atrocities, TIUtilities.GetIntValue(this.strValue));
	}
}
