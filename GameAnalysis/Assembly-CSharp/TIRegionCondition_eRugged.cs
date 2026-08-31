using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000075 RID: 117
public class TIRegionCondition_eRugged : TIRegionCondition
{
	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060002B1 RID: 689 RVA: 0x000113BF File Offset: 0x0000F5BF
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.ruggedRegionInlineSpritePath;
		}
	}

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060002B2 RID: 690 RVA: 0x000113CB File Offset: 0x0000F5CB
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { Loc.T(new StringBuilder("UI.Nation.Terrain").Append(this.strValue).ToString()) };
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x000113F8 File Offset: 0x0000F5F8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, (int)state.ref_region.terrain, (int)this.strValue.ToEnum(TerrainType.None));
	}
}
