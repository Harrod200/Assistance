using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E3 RID: 227
public class TICouncilorCondition_bHasTraitWithTag : TICouncilorCondition
{
	// Token: 0x060003E6 RID: 998 RVA: 0x00013E44 File Offset: 0x00012044
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700007C RID: 124
	// (get) Token: 0x060003E7 RID: 999 RVA: 0x00013E4C File Offset: 0x0001204C
	public override List<string> descriptionParams
	{
		get
		{
			List<string> list = new List<string>(1);
			list.Add(TIUtilities.ConstructTextList((from x in TICouncilorState.GetAllTraitsWithTag(this.strIdx)
				select x.displayName).ToList<string>(), true, false));
			return list;
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x00013EA0 File Offset: 0x000120A0
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.HasTraitWithTag(this.strIdx), TIUtilities.GetBoolValue(this.strValue));
	}
}
