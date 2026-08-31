using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093A RID: 2362
	internal class ConstructionDecision : ArchetypeDecision
	{
		// Token: 0x06005A7C RID: 23164 RVA: 0x002B2109 File Offset: 0x002B0309
		public ConstructionDecision()
			: base(ArchetypeDecision.HabModuleArchetype.Construction, true)
		{
		}

		// Token: 0x06005A7D RID: 23165 RVA: 0x002B2114 File Offset: 0x002B0314
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			if (order.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding))
			{
				return HabSchematicDecision.Nothing;
			}
			if (location.ref_system.habsInSystem.Where<TIHabState>((TIHabState x) => x.faction == faction).Any<TIHabState>(delegate(TIHabState x)
			{
				if (x != location)
				{
					return x.OkayModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.EnablesLocalFounding);
				}
				return false;
			}))
			{
				return HabSchematicDecision.Nothing;
			}
			TIHabModuleTemplate tihabModuleTemplate = base.Decide(faction, location, order).MinBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
			if (tihabModuleTemplate == null)
			{
				return HabSchematicDecision.Nothing;
			}
			return HabSchematicDecision.Nothing.Append(tihabModuleTemplate);
		}
	}
}
