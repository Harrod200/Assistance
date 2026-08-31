using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000317 RID: 791
public class FoundAlienSurveillanceOrbital : FoundAlienSurveillanceHab
{
	// Token: 0x06000CD1 RID: 3281 RVA: 0x000416EE File Offset: 0x0003F8EE
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundSurveillanceOrbital };
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x000416FD File Offset: 0x0003F8FD
	public override int SortOrder()
	{
		return 18;
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x00041701 File Offset: 0x0003F901
	public override int GetTier()
	{
		return 2;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00041704 File Offset: 0x0003F904
	public override TIHabModuleTemplate CoreModule(bool alien = true)
	{
		return TemplateManager.Find<TIHabModuleTemplate>("AlienOrbitalCore", false);
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x00041714 File Offset: 0x0003F914
	public override List<string> AdditionalModules(bool alien = true)
	{
		return new List<string>
		{
			"AlienFusionReactorArray", "AlienFusionReactorArray", "AlienFusionReactorArray", "AlienFusionReactorArray", "AlienSurveillanceArray", "AlienLayeredDefenseArray", "AlienLayeredDefenseArray", "AlienLayeredDefenseArray", "AlienSurveillanceArray", "AlienGarrison",
			"AlienGarrison", "AlienGarrison"
		};
	}
}
