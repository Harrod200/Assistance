using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000311 RID: 785
public abstract class FoundRegularPlatformFromFleetOperation : FoundPlatformFromFleetOperation
{
	// Token: 0x06000CB9 RID: 3257 RVA: 0x000414CB File Offset: 0x0003F6CB
	public override TIHabModuleTemplate CoreModule(bool alien)
	{
		if (!alien)
		{
			return TemplateManager.Find<TIHabModuleTemplate>("PlatformCore", false);
		}
		return TemplateManager.Find<TIHabModuleTemplate>("AlienPlatformCore", false);
	}
}
