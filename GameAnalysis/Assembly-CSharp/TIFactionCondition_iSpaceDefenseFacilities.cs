using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000BB RID: 187
public class TIFactionCondition_iSpaceDefenseFacilities : TIFactionCondition
{
	// Token: 0x17000072 RID: 114
	// (get) Token: 0x06000380 RID: 896 RVA: 0x00012DEF File Offset: 0x00010FEF
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000381 RID: 897 RVA: 0x00012E04 File Offset: 0x00011004
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, (from x in state.ref_faction.majorityControlNations.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions)
			where x.antiSpaceDefenses
			select x).Count<TIRegionState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
