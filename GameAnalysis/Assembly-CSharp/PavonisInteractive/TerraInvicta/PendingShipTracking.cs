using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200075D RID: 1885
	public struct PendingShipTracking
	{
		// Token: 0x06003171 RID: 12657 RVA: 0x00109DDC File Offset: 0x00107FDC
		public PendingShipTracking(string pendingShipDataName, TIHabModuleState shipyard, bool costPaid)
		{
			this.pendingShipDataName = pendingShipDataName;
			this.shipyard = shipyard;
			this.costPaid = costPaid;
			this.tempFleet = null;
		}

		// Token: 0x17000752 RID: 1874
		// (get) Token: 0x06003172 RID: 12658 RVA: 0x00109DFA File Offset: 0x00107FFA
		public TISpaceShipTemplate pendingShipTemplate
		{
			get
			{
				return TemplateManager.Find<TISpaceShipTemplate>(this.pendingShipDataName, false);
			}
		}

		// Token: 0x0400227E RID: 8830
		public string pendingShipDataName;

		// Token: 0x0400227F RID: 8831
		public TIHabModuleState shipyard;

		// Token: 0x04002280 RID: 8832
		public TISpaceFleetState tempFleet;

		// Token: 0x04002281 RID: 8833
		public bool costPaid;
	}
}
