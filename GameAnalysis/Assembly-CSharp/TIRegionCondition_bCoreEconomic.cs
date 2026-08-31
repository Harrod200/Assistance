using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200007A RID: 122
public class TIRegionCondition_bCoreEconomic : TIRegionCondition
{
	// Token: 0x060002BF RID: 703 RVA: 0x00011556 File Offset: 0x0000F756
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x060002C0 RID: 704 RVA: 0x0001155E File Offset: 0x0000F75E
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.coreEconomicRegionInlineSpritePath;
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x0001156A File Offset: 0x0000F76A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.coreEconomicRegion, TIUtilities.GetBoolValue(this.strValue));
	}
}
