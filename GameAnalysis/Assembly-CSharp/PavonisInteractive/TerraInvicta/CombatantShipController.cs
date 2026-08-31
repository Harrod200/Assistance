using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B6 RID: 1462
	public abstract class CombatantShipController : CombatantController
	{
		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060027B6 RID: 10166
		// (set) Token: 0x060027B7 RID: 10167
		public abstract TISpaceShipState ShipState { get; protected set; }

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060027B8 RID: 10168
		// (set) Token: 0x060027B9 RID: 10169
		public abstract ShipModelController ModelController { get; protected set; }

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060027BA RID: 10170 RVA: 0x000D8740 File Offset: 0x000D6940
		public override CombatTargetableState combatTargetableState
		{
			get
			{
				return this.ShipState;
			}
		}

		// Token: 0x060027BB RID: 10171 RVA: 0x000D8748 File Offset: 0x000D6948
		public override float GetCrossSectionalArea_m2(float angle)
		{
			return this.ShipState.GetCrossSectionalArea_m2(angle);
		}
	}
}
