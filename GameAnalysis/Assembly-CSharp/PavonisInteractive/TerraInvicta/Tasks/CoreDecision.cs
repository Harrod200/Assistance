using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093B RID: 2363
	internal class CoreDecision : ArchetypeDecision
	{
		// Token: 0x06005A7E RID: 23166 RVA: 0x002B21E8 File Offset: 0x002B03E8
		public CoreDecision()
			: base(ArchetypeDecision.HabModuleArchetype.Core, false)
		{
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x002B21F4 File Offset: 0x002B03F4
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			IEnumerable<TIHabModuleTemplate> enumerable = from x in base.Decide(faction, location, order)
				where !x.automated
				select x;
			if (!enumerable.Any<TIHabModuleTemplate>())
			{
				return HabSchematicDecision.Nothing;
			}
			TIHabModuleTemplate tihabModuleTemplate = enumerable.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
			return HabSchematicDecision.Nothing.Append(tihabModuleTemplate);
		}
	}
}
