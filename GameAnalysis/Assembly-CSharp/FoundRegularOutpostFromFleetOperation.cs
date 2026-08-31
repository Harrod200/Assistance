using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200031D RID: 797
public abstract class FoundRegularOutpostFromFleetOperation : FoundOutpostFromFleetOperation
{
	// Token: 0x06000CF0 RID: 3312 RVA: 0x00041C66 File Offset: 0x0003FE66
	public override TIHabModuleTemplate CoreModule(bool alien)
	{
		if (!alien)
		{
			return TemplateManager.Find<TIHabModuleTemplate>("OutpostCore", false);
		}
		return TemplateManager.Find<TIHabModuleTemplate>("AlienOutpostCore", false);
	}
}
