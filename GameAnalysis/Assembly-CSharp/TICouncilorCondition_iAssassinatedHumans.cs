using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000EF RID: 239
public class TICouncilorCondition_iAssassinatedHumans : TICouncilorCondition
{
	// Token: 0x17000084 RID: 132
	// (get) Token: 0x06000407 RID: 1031 RVA: 0x000142E8 File Offset: 0x000124E8
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00014300 File Offset: 0x00012500
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_councilor.assassinations.Where<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Key.IsActiveHumanFaction).Sum<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Value), TIUtilities.GetIntValue(this.strValue));
	}
}
