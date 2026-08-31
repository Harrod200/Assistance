using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000316 RID: 790
public class FoundAlienSurveillancePlatform : FoundAlienSurveillanceHab
{
	// Token: 0x06000CCB RID: 3275 RVA: 0x00041690 File Offset: 0x0003F890
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.FoundSurveillancePlatform };
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0004169F File Offset: 0x0003F89F
	public override int SortOrder()
	{
		return 17;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x000416A3 File Offset: 0x0003F8A3
	public override int GetTier()
	{
		return 1;
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000416A6 File Offset: 0x0003F8A6
	public override TIHabModuleTemplate CoreModule(bool alien = true)
	{
		return TemplateManager.Find<TIHabModuleTemplate>("AlienPlatformCore", false);
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x000416B3 File Offset: 0x0003F8B3
	public override List<string> AdditionalModules(bool alien = true)
	{
		return new List<string> { "AlienFusionPile", "AlienObservationPost", "AlienFusionPile", "AlienPointDefenseArray" };
	}
}
