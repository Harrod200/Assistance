using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000321 RID: 801
public abstract class FoundAutomatedOutpostFromFleetOperation : FoundOutpostFromFleetOperation
{
	// Token: 0x06000CFF RID: 3327 RVA: 0x00041E22 File Offset: 0x00040022
	public override TIHabModuleTemplate CoreModule(bool alien)
	{
		if (!alien)
		{
			return TemplateManager.Find<TIHabModuleTemplate>("AutomatedOutpostCore", false);
		}
		return TemplateManager.Find<TIHabModuleTemplate>("AlienOutpostCore", false);
	}
}
