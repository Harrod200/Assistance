using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000319 RID: 793
public abstract class FoundAutomatedPlatformFromFleetOperation : FoundPlatformFromFleetOperation
{
	// Token: 0x06000CDD RID: 3293 RVA: 0x00041876 File Offset: 0x0003FA76
	public override TIHabModuleTemplate CoreModule(bool alien)
	{
		if (!alien)
		{
			return TemplateManager.Find<TIHabModuleTemplate>("AutomatedPlatformCore", false);
		}
		return TemplateManager.Find<TIHabModuleTemplate>("AlienPlatformCore", false);
	}
}
