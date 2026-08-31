using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200067C RID: 1660
	public class OrbitSelectedEvent : GameEvent
	{
		// Token: 0x060028A4 RID: 10404 RVA: 0x000DA877 File Offset: 0x000D8A77
		public OrbitSelectedEvent(TIOrbitState orbit)
		{
			this.orbit = orbit;
		}

		// Token: 0x04001EE2 RID: 7906
		public TIOrbitState orbit;
	}
}
