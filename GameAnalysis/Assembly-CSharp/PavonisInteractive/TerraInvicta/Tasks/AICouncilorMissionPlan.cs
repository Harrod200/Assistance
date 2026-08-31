using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200092F RID: 2351
	public class AICouncilorMissionPlan
	{
		// Token: 0x060059CC RID: 22988 RVA: 0x00292F3B File Offset: 0x0029113B
		public AICouncilorMissionPlan(IReadOnlyList<AIMissionEntry> selectedMissions, TIFactionState faction, IReadOnlyDictionary<TICouncilorState, PolicyOptionWithTarget> councilorPolicy, IReadOnlyDictionary<TICouncilorState, float> totalWeights = null, IReadOnlyDictionary<TICouncilorState, AIMissionEntry> highestScore = null)
		{
			this.selectedMissions = selectedMissions;
			this.faction = faction;
			this.councilorPolicy = councilorPolicy;
			this.totalWeights = totalWeights;
			this.highestScore = highestScore;
		}

		// Token: 0x040040DB RID: 16603
		public readonly IReadOnlyList<AIMissionEntry> selectedMissions;

		// Token: 0x040040DC RID: 16604
		public readonly TIFactionState faction;

		// Token: 0x040040DD RID: 16605
		public readonly IReadOnlyDictionary<TICouncilorState, PolicyOptionWithTarget> councilorPolicy;

		// Token: 0x040040DE RID: 16606
		public readonly IReadOnlyDictionary<TICouncilorState, float> totalWeights;

		// Token: 0x040040DF RID: 16607
		public readonly IReadOnlyDictionary<TICouncilorState, AIMissionEntry> highestScore;
	}
}
