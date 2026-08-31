using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000084 RID: 132
public class TIRegionCondition_iNuclearDetonations : TIRegionCondition
{
	// Token: 0x1700004E RID: 78
	// (get) Token: 0x060002DC RID: 732 RVA: 0x0001185F File Offset: 0x0000FA5F
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.nukedRegionInlineSpritePath;
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001186B File Offset: 0x0000FA6B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.nuclearDetonations, TIUtilities.GetIntValue(this.strValue));
	}
}
