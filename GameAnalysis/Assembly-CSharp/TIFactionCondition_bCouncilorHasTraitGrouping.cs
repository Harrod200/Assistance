using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A9 RID: 169
public class TIFactionCondition_bCouncilorHasTraitGrouping : TIFactionCondition
{
	// Token: 0x06000348 RID: 840 RVA: 0x00012632 File Offset: 0x00010832
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000349 RID: 841 RVA: 0x0001263C File Offset: 0x0001083C
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

	// Token: 0x0600034A RID: 842 RVA: 0x00012694 File Offset: 0x00010894
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.councilors.Any<TICouncilorState>((TICouncilorState x) => x.GetTraitGrouping(TIUtilities.GetIntValue(this.strIdx)) != null), TIUtilities.GetBoolValue(this.strValue));
	}
}
