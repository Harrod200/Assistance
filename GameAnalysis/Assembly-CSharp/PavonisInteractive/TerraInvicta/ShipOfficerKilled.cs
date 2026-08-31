using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200065E RID: 1630
	public class ShipOfficerKilled : GameEvent
	{
		// Token: 0x06002885 RID: 10373 RVA: 0x000DA5C2 File Offset: 0x000D87C2
		public ShipOfficerKilled(TISpaceShipState ship, string officerName, string officerNameAndJob)
		{
			this.ship = ship;
			this.officerName = officerName;
			this.officerNameAndJob = officerNameAndJob;
		}

		// Token: 0x04001EBA RID: 7866
		public TISpaceShipState ship;

		// Token: 0x04001EBB RID: 7867
		public string officerName;

		// Token: 0x04001EBC RID: 7868
		public string officerNameAndJob;
	}
}
