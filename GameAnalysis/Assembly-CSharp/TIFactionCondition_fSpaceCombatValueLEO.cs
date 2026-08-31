using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000BA RID: 186
public class TIFactionCondition_fSpaceCombatValueLEO : TIFactionCondition
{
	// Token: 0x17000071 RID: 113
	// (get) Token: 0x0600037D RID: 893 RVA: 0x00012D5A File Offset: 0x00010F5A
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00012D70 File Offset: 0x00010F70
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.orbitState != null && x.orbitState.isEarthLEO).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()), (float)TIUtilities.GetIntValue(this.strValue));
	}
}
