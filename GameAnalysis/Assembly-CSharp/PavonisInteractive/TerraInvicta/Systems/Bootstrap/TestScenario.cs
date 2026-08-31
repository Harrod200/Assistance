using System;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009B6 RID: 2486
	public class TestScenario : BaseScenario
	{
		// Token: 0x06005DC4 RID: 24004 RVA: 0x002CA173 File Offset: 0x002C8373
		public override bool Initialize()
		{
			base.OnStartScene();
			base.activePlayerFaction = TemplateManager.Find<TIFactionTemplate>("ResistCouncil", false);
			TIMetaTemplate.LoadMetaTemplates(this.scenarioTemplate.templateNames);
			return true;
		}
	}
}
