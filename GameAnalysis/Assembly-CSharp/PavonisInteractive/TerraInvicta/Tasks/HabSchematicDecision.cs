using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093E RID: 2366
	public abstract class HabSchematicDecision
	{
		// Token: 0x06005A90 RID: 23184
		public abstract IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order);

		// Token: 0x17000F53 RID: 3923
		// (get) Token: 0x06005A91 RID: 23185 RVA: 0x002B3151 File Offset: 0x002B1351
		protected static IEnumerable<TIHabModuleTemplate> Nothing
		{
			get
			{
				return Enumerable.Empty<TIHabModuleTemplate>();
			}
		}

		// Token: 0x06005A92 RID: 23186 RVA: 0x002B3158 File Offset: 0x002B1358
		protected static bool IsValidModule(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, HabSchematicOrder order)
		{
			return moduleTemplate != null && TIHabState.IsModuleAllowedForHab(faction, location, moduleTemplate, order, false) && (!moduleTemplate.onePerHab || !order.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.SharesUpgradePath(moduleTemplate)));
		}

		// Token: 0x06005A93 RID: 23187 RVA: 0x002B31B4 File Offset: 0x002B13B4
		protected static IEnumerable<TIHabModuleTemplate> InvalidModulesRemoved(TIFactionState faction, TIGameState location, IEnumerable<TIHabModuleTemplate> moduleTemplates, HabSchematicOrder order)
		{
			return moduleTemplates.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => HabSchematicDecision.IsValidModule(faction, location, x, order));
		}
	}
}
