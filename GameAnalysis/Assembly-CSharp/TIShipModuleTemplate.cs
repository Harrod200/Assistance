using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003CF RID: 975
public abstract class TIShipModuleTemplate : TIShipPartTemplate
{
	// Token: 0x06001280 RID: 4736 RVA: 0x00058C84 File Offset: 0x00056E84
	public override float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.mass_tons;
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00058C8C File Offset: 0x00056E8C
	public override TIResourcesCost buildCost(float value = 0f, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.buildMass_tons(value, value2, 0f, 0f, false) * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x040010F8 RID: 4344
	public float mass_tons;
}
