using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007B RID: 123
public class TIRegionCondition_bCoreResource : TIRegionCondition
{
	// Token: 0x060002C3 RID: 707 RVA: 0x000115A5 File Offset: 0x0000F7A5
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x060002C4 RID: 708 RVA: 0x000115AD File Offset: 0x0000F7AD
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.miningRegionInlineSpritePath;
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x000115B9 File Offset: 0x0000F7B9
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.resourceRegion, TIUtilities.GetBoolValue(this.strValue));
	}
}
