using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000748 RID: 1864
	public abstract class FactionGoal_BuildBase : FactionGoal_BuildHab
	{
		// Token: 0x06002FAF RID: 12207 RVA: 0x001044D5 File Offset: 0x001026D5
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002FB0 RID: 12208 RVA: 0x001044EA File Offset: 0x001026EA
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.hab == null || base.hab.archived || base.hab.faction != this.faction;
		}
	}
}
