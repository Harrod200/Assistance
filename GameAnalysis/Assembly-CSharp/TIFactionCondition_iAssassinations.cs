using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000BE RID: 190
public class TIFactionCondition_iAssassinations : TIFactionCondition
{
	// Token: 0x17000075 RID: 117
	// (get) Token: 0x06000389 RID: 905 RVA: 0x00012FAE File Offset: 0x000111AE
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00012FC4 File Offset: 0x000111C4
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.factionAssassinations.Sum<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Value), TIUtilities.GetIntValue(this.strValue));
	}
}
