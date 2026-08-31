using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000BD RID: 189
public class TIFactionCondition_iAssassinatedAliens : TIFactionCondition
{
	// Token: 0x17000074 RID: 116
	// (get) Token: 0x06000386 RID: 902 RVA: 0x00012F1A File Offset: 0x0001111A
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000387 RID: 903 RVA: 0x00012F30 File Offset: 0x00011130
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.factionAssassinations.Where<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Key.IsAlienFaction).Sum<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Value), TIUtilities.GetIntValue(this.strValue));
	}
}
