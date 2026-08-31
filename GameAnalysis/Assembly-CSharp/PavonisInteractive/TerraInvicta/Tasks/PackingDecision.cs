using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000941 RID: 2369
	internal class PackingDecision : WildCardDecision
	{
		// Token: 0x06005AA1 RID: 23201 RVA: 0x002B3630 File Offset: 0x002B1830
		public override IEnumerable<TIHabModuleTemplate> GetChoices(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			Func<TIHabModuleTemplate, int> <>9__2;
			Func<int> func = delegate
			{
				IEnumerable<TIHabModuleTemplate> order2 = order;
				Func<TIHabModuleTemplate, int> func2;
				if ((func2 = <>9__2) == null)
				{
					func2 = (<>9__2 = (TIHabModuleTemplate moduleTemplate) => moduleTemplate.ProspectivePower(location, faction));
				}
				return order2.Sum<TIHabModuleTemplate>(func2);
			};
			int excessPower = func();
			return from x in base.GetChoices(faction, location, order)
				where -x.power <= excessPower
				select x;
		}

		// Token: 0x06005AA2 RID: 23202 RVA: 0x002B369C File Offset: 0x002B189C
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			IEnumerable<TIHabModuleTemplate> enumerable = base.Decide(faction, location, order);
			if (enumerable.Any<TIHabModuleTemplate>())
			{
				return enumerable;
			}
			TIHabModuleTemplate bestPowerModuleTemplate = PowerDecision.GetBestPowerModuleTemplate(faction, location, order);
			if (bestPowerModuleTemplate == null)
			{
				return HabSchematicDecision.Nothing;
			}
			return HabSchematicDecision.Nothing.Append(bestPowerModuleTemplate);
		}
	}
}
