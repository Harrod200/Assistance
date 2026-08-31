using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000318 RID: 792
public class FoundAlienSurveillanceRing : FoundAlienSurveillanceHab
{
	// Token: 0x06000CD7 RID: 3287 RVA: 0x000417B2 File Offset: 0x0003F9B2
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundSurveillanceRing };
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x000417C1 File Offset: 0x0003F9C1
	public override int SortOrder()
	{
		return 19;
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x000417C5 File Offset: 0x0003F9C5
	public override int GetTier()
	{
		return 3;
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x000417C8 File Offset: 0x0003F9C8
	public override TIHabModuleTemplate CoreModule(bool alien = true)
	{
		return TemplateManager.Find<TIHabModuleTemplate>("AlienRingCore", false);
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x000417D8 File Offset: 0x0003F9D8
	public override List<string> AdditionalModules(bool alien = true)
	{
		return new List<string>
		{
			"AlienFusionReactorFarm", "AlienFusionReactorFarm", "AlienFusionReactorFarm", "AlienBattlestations", "AlienWatchtower", "AlienBattlestations", "AlienBattlestations", "AlienBattlestations", "AlienWatchtower", "AlienCitadel",
			"AlienGarrison", "AlienGarrison"
		};
	}
}
