using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000735 RID: 1845
	public abstract class FactionGoal_Faction : FactionGoal_Earth
	{
		// Token: 0x170006D9 RID: 1753
		// (get) Token: 0x06002E5E RID: 11870 RVA: 0x000FC4A2 File Offset: 0x000FA6A2
		public override bool PoliciesAsFactionActor
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170006DA RID: 1754
		// (get) Token: 0x06002E5F RID: 11871 RVA: 0x000FC4A5 File Offset: 0x000FA6A5
		// (set) Token: 0x06002E60 RID: 11872 RVA: 0x000FC4AD File Offset: 0x000FA6AD
		public TIFactionState targetFaction { get; protected set; }

		// Token: 0x06002E61 RID: 11873 RVA: 0x000FC4B6 File Offset: 0x000FA6B6
		public override void ChangeTarget(TIGameState newTarget)
		{
			this.targetFaction = ((newTarget != null) ? newTarget.ref_faction : null);
		}

		// Token: 0x06002E62 RID: 11874 RVA: 0x000FC4CA File Offset: 0x000FA6CA
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}
	}
}
