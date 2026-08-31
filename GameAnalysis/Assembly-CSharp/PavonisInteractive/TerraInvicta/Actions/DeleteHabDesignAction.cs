using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A6E RID: 2670
	public class DeleteHabDesignAction : PlayerAction
	{
		// Token: 0x0600652A RID: 25898 RVA: 0x002FB740 File Offset: 0x002F9940
		public DeleteHabDesignAction(TIFactionState faction, TIHabTemplate habDesign)
		{
			this.factionID = faction.ID;
			this.habDesign = habDesign;
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x002FB75B File Offset: 0x002F995B
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).DeleteHabDesign(this.habDesign.dataName);
			GameControl.eventManager.TriggerEvent(new HabDesignTemplateModified(), null, Array.Empty<object>());
		}

		// Token: 0x04004754 RID: 18260
		public GameStateID factionID;

		// Token: 0x04004755 RID: 18261
		public TIHabTemplate habDesign;
	}
}
