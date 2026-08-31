using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007C RID: 124
public class TIRegionCondition_bColony : TIRegionCondition
{
	// Token: 0x060002C7 RID: 711 RVA: 0x000115F4 File Offset: 0x0000F7F4
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x060002C8 RID: 712 RVA: 0x000115FC File Offset: 0x0000F7FC
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.colonyRegionInlineSpritePath;
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00011608 File Offset: 0x0000F808
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.colonyRegion, TIUtilities.GetBoolValue(this.strValue));
	}
}
