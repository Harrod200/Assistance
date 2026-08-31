using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000151 RID: 337
public class ProjectTrigger
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000525 RID: 1317 RVA: 0x000167A4 File Offset: 0x000149A4
	public TIProjectTemplate projectTemplate
	{
		get
		{
			return TemplateManager.Find<TIProjectTemplate>(this.projectTemplateName, false);
		}
	}

	// Token: 0x0400023D RID: 573
	public string projectTemplateName;

	// Token: 0x0400023E RID: 574
	public float monthlyTriggerValue;
}
