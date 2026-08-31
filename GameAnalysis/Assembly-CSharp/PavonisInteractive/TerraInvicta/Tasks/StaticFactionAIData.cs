using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000934 RID: 2356
	public class StaticFactionAIData
	{
		// Token: 0x06005A18 RID: 23064 RVA: 0x0029DC18 File Offset: 0x0029BE18
		public StaticFactionAIData(TIFactionState faction, int index)
		{
			this.otherFactions = (from x in GameStateManager.AllFactions()
				where x != faction
				select x).ToList<TIFactionState>();
			this.enemyFactions = new List<TIFactionState>(this.otherFactions);
			if (faction.IsAlienFaction)
			{
				this.enemyFactions.Remove(GameStateManager.AlienProxy());
			}
			if (faction.IsAlienProxy)
			{
				this.enemyFactions.Remove(GameStateManager.AlienFaction());
			}
			this.enemyHumanFactions = this.enemyFactions.Where<TIFactionState>((TIFactionState x) => x.IsActiveHumanFaction).ToList<TIFactionState>();
			this.every3DaysOffset = index % 3;
			this.every4DaysOffset = index % 4;
			this.every7DaysOffset = index % 7;
			this.every14DaysOffset = index % 14;
			this.every14DaysOffsetLate = (15 - index) % 14;
			TIFactionState.LogAI(string.Concat(new string[]
			{
				"Setting ",
				faction.displayName,
				" timing idx ",
				index.ToString(),
				" ",
				this.every3DaysOffset.ToString(),
				"/3 ",
				this.every4DaysOffset.ToString(),
				"/4 ",
				this.every7DaysOffset.ToString(),
				"/7 ",
				this.every14DaysOffset.ToString(),
				"/14",
				this.every14DaysOffsetLate.ToString(),
				"/14"
			}), false);
		}

		// Token: 0x04004118 RID: 16664
		public readonly List<TIFactionState> otherFactions;

		// Token: 0x04004119 RID: 16665
		public readonly List<TIFactionState> enemyFactions;

		// Token: 0x0400411A RID: 16666
		public readonly List<TIFactionState> enemyHumanFactions;

		// Token: 0x0400411B RID: 16667
		public readonly int every3DaysOffset;

		// Token: 0x0400411C RID: 16668
		public readonly int every4DaysOffset;

		// Token: 0x0400411D RID: 16669
		public readonly int every7DaysOffset;

		// Token: 0x0400411E RID: 16670
		public readonly int every14DaysOffset;

		// Token: 0x0400411F RID: 16671
		public readonly int every14DaysOffsetLate;
	}
}
