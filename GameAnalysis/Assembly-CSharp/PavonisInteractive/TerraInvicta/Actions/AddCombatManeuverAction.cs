using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A5A RID: 2650
	public class AddCombatManeuverAction : PlayerAction
	{
		// Token: 0x06006501 RID: 25857 RVA: 0x002FADBB File Offset: 0x002F8FBB
		public AddCombatManeuverAction(TISpaceShipState ship, CombatManeuver maneuver)
		{
			this.shipID = ship.ID;
			this.maneuver = maneuver;
		}

		// Token: 0x06006502 RID: 25858 RVA: 0x002FADD6 File Offset: 0x002F8FD6
		public override void Execute()
		{
			this.shipID.GetState<TISpaceShipState>(false).AddCombatManeuver(this.maneuver);
		}

		// Token: 0x04004728 RID: 18216
		private GameStateID shipID;

		// Token: 0x04004729 RID: 18217
		private CombatManeuver maneuver;
	}
}
