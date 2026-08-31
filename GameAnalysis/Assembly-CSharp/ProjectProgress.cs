using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200014E RID: 334
public class ProjectProgress
{
	// Token: 0x0600051B RID: 1307 RVA: 0x00016695 File Offset: 0x00014895
	public ProjectProgress()
	{
	}

	// Token: 0x0600051C RID: 1308 RVA: 0x0001669D File Offset: 0x0001489D
	public ProjectProgress(TIProjectTemplate projectTemplate, int slot, float accumulatedResearch = 0f)
	{
		this.projectTemplateName = projectTemplate.dataName;
		this.slot = slot;
		this.accumulatedResearch = accumulatedResearch;
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x000166BF File Offset: 0x000148BF
	public ProjectProgress(string name, int slot, float accumulatedResearch = 0f)
	{
		this.projectTemplateName = name;
		this.slot = slot;
		this.accumulatedResearch = accumulatedResearch;
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x0600051E RID: 1310 RVA: 0x000166DC File Offset: 0x000148DC
	public TIProjectTemplate projectTemplate
	{
		get
		{
			return TemplateManager.Find<TIProjectTemplate>(this.projectTemplateName, false);
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x0600051F RID: 1311 RVA: 0x000166EA File Offset: 0x000148EA
	public TechCategory projectCategory
	{
		get
		{
			return this.projectTemplate.techCategory;
		}
	}

	// Token: 0x06000520 RID: 1312 RVA: 0x000166F7 File Offset: 0x000148F7
	public bool SufficientResearchAccumulated(TIFactionState faction)
	{
		return this.accumulatedResearch >= this.projectTemplate.GetResearchCost(faction);
	}

	// Token: 0x06000521 RID: 1313 RVA: 0x00016710 File Offset: 0x00014910
	public float progressFrac(TIFactionState faction)
	{
		return this.accumulatedResearch / this.projectTemplate.GetResearchCost(faction);
	}

	// Token: 0x04000233 RID: 563
	public string projectTemplateName;

	// Token: 0x04000234 RID: 564
	public float accumulatedResearch;

	// Token: 0x04000235 RID: 565
	public int slot;

	// Token: 0x04000236 RID: 566
	public bool completed;
}
