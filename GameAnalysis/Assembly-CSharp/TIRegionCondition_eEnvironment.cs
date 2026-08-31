using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000074 RID: 116
public class TIRegionCondition_eEnvironment : TIRegionCondition
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060002AE RID: 686 RVA: 0x00011351 File Offset: 0x0000F551
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { Loc.T(new StringBuilder("UI.Nation.Eco").Append(this.strValue).ToString()) };
		}
	}

	// Token: 0x060002AF RID: 687 RVA: 0x0001137E File Offset: 0x0000F57E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, (int)state.ref_region.template.environment, (int)this.strValue.ToEnum(EnvironmentType.None));
	}
}
