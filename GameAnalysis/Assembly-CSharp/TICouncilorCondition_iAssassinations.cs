using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F1 RID: 241
public class TICouncilorCondition_iAssassinations : TIFactionCondition
{
	// Token: 0x17000086 RID: 134
	// (get) Token: 0x0600040D RID: 1037 RVA: 0x000143F2 File Offset: 0x000125F2
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x00014408 File Offset: 0x00012608
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_councilor.assassinations.Sum<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Value), TIUtilities.GetIntValue(this.strValue));
	}
}
