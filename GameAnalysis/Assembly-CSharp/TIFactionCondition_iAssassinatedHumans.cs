using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000BC RID: 188
public class TIFactionCondition_iAssassinatedHumans : TIFactionCondition
{
	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000383 RID: 899 RVA: 0x00012E87 File Offset: 0x00011087
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00012E9C File Offset: 0x0001109C
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.factionAssassinations.Where<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Key.IsActiveHumanFaction).Sum<KeyValuePair<TIFactionState, int>>((KeyValuePair<TIFactionState, int> x) => x.Value), TIUtilities.GetIntValue(this.strValue));
	}
}
