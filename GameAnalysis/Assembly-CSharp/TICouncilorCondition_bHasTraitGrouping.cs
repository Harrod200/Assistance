using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E2 RID: 226
public class TICouncilorCondition_bHasTraitGrouping : TICouncilorCondition
{
	// Token: 0x060003E2 RID: 994 RVA: 0x00013D8D File Offset: 0x00011F8D
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060003E3 RID: 995 RVA: 0x00013D98 File Offset: 0x00011F98
	public override List<string> descriptionParams
	{
		get
		{
			List<string> list = new List<string>(1);
			list.Add(string.Join(", ", from x in TICouncilorState.GetAllTraitsOfGrouping(TIUtilities.GetIntValue(this.strIdx))
				select x.displayName));
			return list;
		}
	}

	// Token: 0x060003E4 RID: 996 RVA: 0x00013DF0 File Offset: 0x00011FF0
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.GetTraitGrouping(TIUtilities.GetIntValue(this.strIdx)) != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
