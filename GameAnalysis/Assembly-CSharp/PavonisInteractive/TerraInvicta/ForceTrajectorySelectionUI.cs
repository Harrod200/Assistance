using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005DE RID: 1502
	public class ForceTrajectorySelectionUI : GameEvent
	{
		// Token: 0x06002803 RID: 10243 RVA: 0x000D9C41 File Offset: 0x000D7E41
		public ForceTrajectorySelectionUI(TIFactionState maneuveringFleetFaction, TISpaceFleetState maneuveringFleet, TISpaceFleetState targetFleet, Trajectory[] validTrajectories = null)
		{
			this.maneuveringFleetFaction = maneuveringFleetFaction;
			this.maneuveringFleet = maneuveringFleet;
			this.targetFleet = targetFleet;
			this.validTrajectories = validTrajectories;
		}

		// Token: 0x04001DFE RID: 7678
		public TIFactionState maneuveringFleetFaction;

		// Token: 0x04001DFF RID: 7679
		public TISpaceFleetState maneuveringFleet;

		// Token: 0x04001E00 RID: 7680
		public TISpaceFleetState targetFleet;

		// Token: 0x04001E01 RID: 7681
		public Trajectory[] validTrajectories;
	}
}
