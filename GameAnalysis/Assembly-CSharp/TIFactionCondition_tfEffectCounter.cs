using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A1 RID: 161
public class TIFactionCondition_tfEffectCounter : TIFactionCondition
{
	// Token: 0x06000330 RID: 816 RVA: 0x00012328 File Offset: 0x00010528
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000331 RID: 817 RVA: 0x00012330 File Offset: 0x00010530
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				this.strIdx,
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x00012354 File Offset: 0x00010554
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, TIEffectsState.SumEffectsModifiers(this.strIdx.ToEnum(Context.None), state.ref_faction, 0f, null), TIUtilities.GetFloatValue(this.strValue));
	}
}
