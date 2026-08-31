using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000079 RID: 121
public class TIRegionCondition_bCapital : TIRegionCondition
{
	// Token: 0x060002BB RID: 699 RVA: 0x000114EA File Offset: 0x0000F6EA
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x060002BC RID: 700 RVA: 0x000114F2 File Offset: 0x0000F6F2
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.capitalRegionInlineSpritePath;
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00011500 File Offset: 0x0000F700
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region == state.ref_region.nation.capital, TIUtilities.GetBoolValue(this.strValue));
	}
}
