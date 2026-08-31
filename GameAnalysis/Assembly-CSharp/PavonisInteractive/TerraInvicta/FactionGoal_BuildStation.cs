using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000744 RID: 1860
	public abstract class FactionGoal_BuildStation : FactionGoal_BuildHab
	{
		// Token: 0x06002F8B RID: 12171 RVA: 0x00103FE8 File Offset: 0x001021E8
		public override GoalType GetGoalType()
		{
			return GoalType.BuildFullStation;
		}
	}
}
