using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000944 RID: 2372
	internal class ScoreDecision : ScoreDecisionBase
	{
		// Token: 0x17000F59 RID: 3929
		// (get) Token: 0x06005AB1 RID: 23217 RVA: 0x002B3C48 File Offset: 0x002B1E48
		public IEnumerable<HabSchematicDecision> Decisions
		{
			get
			{
				return this.decisions;
			}
		}

		// Token: 0x06005AB2 RID: 23218 RVA: 0x002B3C50 File Offset: 0x002B1E50
		public override IEnumerable<TIHabModuleTemplate> GetChoices(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			return this.decisions.SelectMany<HabSchematicDecision, TIHabModuleTemplate>((HabSchematicDecision x) => x.Decide(faction, location, order));
		}

		// Token: 0x06005AB3 RID: 23219 RVA: 0x002B3C8F File Offset: 0x002B1E8F
		public ScoreDecision(IEnumerable<HabSchematicDecision> decisions)
		{
			this.decisions = decisions.ToList<HabSchematicDecision>();
		}

		// Token: 0x06005AB4 RID: 23220 RVA: 0x002B3CA3 File Offset: 0x002B1EA3
		public ScoreDecision(params HabSchematicDecision[] decisions)
		{
			this.decisions = decisions.ToList<HabSchematicDecision>();
		}

		// Token: 0x06005AB5 RID: 23221 RVA: 0x002B3CB7 File Offset: 0x002B1EB7
		public ScoreDecision(params ArchetypeDecision.HabModuleArchetype[] archetypes)
		{
			this.decisions = archetypes.Select<ArchetypeDecision.HabModuleArchetype, HabSchematicDecision>((ArchetypeDecision.HabModuleArchetype x) => new ArchetypeDecision(x, true)).ToList<HabSchematicDecision>();
		}

		// Token: 0x06005AB6 RID: 23222 RVA: 0x002B3CEF File Offset: 0x002B1EEF
		public ScoreDecision(ArchetypeDecision.HabModuleArchetype archetype)
		{
			this.decisions = Enumerable.Empty<HabSchematicDecision>().Append(new ArchetypeDecision(archetype, true)).ToList<HabSchematicDecision>();
		}

		// Token: 0x04004159 RID: 16729
		[fsProperty]
		private List<HabSchematicDecision> decisions;
	}
}
