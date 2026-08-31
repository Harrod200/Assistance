using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086E RID: 2158
	public interface IHabitatsPreviewer
	{
		// Token: 0x06005025 RID: 20517
		void SelectHabFromMenu(TIHabState hab);

		// Token: 0x06005026 RID: 20518
		void SelectModule(HabGridCell item);

		// Token: 0x06005027 RID: 20519
		bool IsManaging();

		// Token: 0x06005028 RID: 20520
		void ManageHab();

		// Token: 0x06005029 RID: 20521
		HabitatsScreenController GetController();
	}
}
