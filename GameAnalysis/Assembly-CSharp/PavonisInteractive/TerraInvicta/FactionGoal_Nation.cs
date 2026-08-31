using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072A RID: 1834
	public abstract class FactionGoal_Nation : FactionGoal_Earth
	{
		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06002D7E RID: 11646 RVA: 0x000FA1A4 File Offset: 0x000F83A4
		// (set) Token: 0x06002D7F RID: 11647 RVA: 0x000FA1AC File Offset: 0x000F83AC
		public TINationState nation { get; protected set; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06002D80 RID: 11648
		public abstract Dictionary<PriorityType, int> prioritiesAsNation { get; }

		// Token: 0x06002D81 RID: 11649 RVA: 0x000FA1B5 File Offset: 0x000F83B5
		public override bool PoliciesAsNationGoal()
		{
			return true;
		}

		// Token: 0x06002D82 RID: 11650 RVA: 0x000FA1B8 File Offset: 0x000F83B8
		public override bool PoliciesAtTargetNationGoal()
		{
			return true;
		}

		// Token: 0x06002D83 RID: 11651 RVA: 0x000FA1BB File Offset: 0x000F83BB
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002D84 RID: 11652 RVA: 0x000FA1BE File Offset: 0x000F83BE
		public override void ChangeTarget(TIGameState newTarget)
		{
			if (newTarget != null && newTarget.isNationState)
			{
				this.nation = newTarget.ref_nation;
				return;
			}
			this.nation = null;
		}
	}
}
