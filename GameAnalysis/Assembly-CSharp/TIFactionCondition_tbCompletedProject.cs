using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A5 RID: 165
public class TIFactionCondition_tbCompletedProject : TIFactionCondition
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x0600033A RID: 826 RVA: 0x00012462 File Offset: 0x00010662
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIProjectTemplate>(this.strValue).displayName };
		}
	}

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x0600033B RID: 827 RVA: 0x00012480 File Offset: 0x00010680
	public override string symbolResource
	{
		get
		{
			return TemplateManager.global.projectsInlineSpritePath;
		}
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0001248C File Offset: 0x0001068C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison<TIProjectTemplate>(this.sign, TIUtilities.GetTemplateValue<TIProjectTemplate>(this.strValue), state.ref_faction.completedProjects);
	}
}
