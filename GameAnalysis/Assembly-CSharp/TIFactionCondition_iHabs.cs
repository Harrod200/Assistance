using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B8 RID: 184
public class TIFactionCondition_iHabs : TIFactionCondition
{
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x06000377 RID: 887 RVA: 0x00012CA4 File Offset: 0x00010EA4
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000378 RID: 888 RVA: 0x00012CB9 File Offset: 0x00010EB9
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.habs.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
