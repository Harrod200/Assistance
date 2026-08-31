using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5B RID: 2651
	public class CancelCombatManeuverAction : PlayerAction
	{
		// Token: 0x06006503 RID: 25859 RVA: 0x002FADEF File Offset: 0x002F8FEF
		public CancelCombatManeuverAction(TISpaceShipState ship, CombatManeuver maneuver)
		{
			this.shipID = ship.ID;
			this.maneuver = maneuver;
		}

		// Token: 0x06006504 RID: 25860 RVA: 0x002FAE0A File Offset: 0x002F900A
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).RemoveCombatManeuver(this.maneuver);
		}

		// Token: 0x0400472A RID: 18218
		private GameStateID shipID;

		// Token: 0x0400472B RID: 18219
		private CombatManeuver maneuver;
	}
}
