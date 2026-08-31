using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000945 RID: 2373
	internal class WildCardDecision : ScoreDecisionBase
	{
		// Token: 0x06005AB7 RID: 23223 RVA: 0x002B3D13 File Offset: 0x002B1F13
		public override IEnumerable<TIHabModuleTemplate> GetChoices(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			return TemplateManager.HabModuleTemplates.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.coreModule && !x.powerSource && !x.mine && !x.IsFarm);
		}
	}
}
