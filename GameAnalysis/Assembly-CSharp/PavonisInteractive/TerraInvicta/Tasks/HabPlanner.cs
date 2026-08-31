using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000938 RID: 2360
	public abstract class HabPlanner
	{
		// Token: 0x06005A68 RID: 23144 RVA: 0x002B1AB4 File Offset: 0x002AFCB4
		public static HabPlanner GetPlanner(TIFactionState faction)
		{
			if (faction.IsAlienFaction)
			{
				return HabPlanner.AlienHabPlanner;
			}
			return HabPlanner.HumanHabPlanner;
		}

		// Token: 0x06005A69 RID: 23145 RVA: 0x002B1AC9 File Offset: 0x002AFCC9
		public static void ClearStaticData()
		{
			PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.nextTargetOrderCount.Clear();
			HabPlanner.HumanHabPlanner = new HumanHabPlanner();
			HabPlanner.AlienHabPlanner = new AlienHabPlanner();
		}

		// Token: 0x06005A6A RID: 23146
		public abstract void ManageHabGoals(TIFactionState faction);

		// Token: 0x06005A6B RID: 23147
		public abstract void FoundHabs(TIFactionState faction);

		// Token: 0x06005A6C RID: 23148
		public abstract void ManageHabs(TIFactionState faction);

		// Token: 0x04004145 RID: 16709
		public static HabPlanner HumanHabPlanner = new HumanHabPlanner();

		// Token: 0x04004146 RID: 16710
		public static HabPlanner AlienHabPlanner = new AlienHabPlanner();
	}
}
