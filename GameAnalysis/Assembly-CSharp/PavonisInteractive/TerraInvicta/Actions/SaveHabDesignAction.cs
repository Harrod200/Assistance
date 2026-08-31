using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A82 RID: 2690
	public class SaveHabDesignAction : PlayerAction
	{
		// Token: 0x06006558 RID: 25944 RVA: 0x002FC5F7 File Offset: 0x002FA7F7
		public SaveHabDesignAction(TIFactionState faction, TIHabTemplate habDesign)
		{
			this.factionID = faction.ID;
			this.habDesign = habDesign;
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x002FC612 File Offset: 0x002FA812
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).SaveHabDesign(this.habDesign);
			GameControl.eventManager.TriggerEvent(new HabDesignTemplateModified(), null, Array.Empty<object>());
		}

		// Token: 0x0400478A RID: 18314
		public GameStateID factionID;

		// Token: 0x0400478B RID: 18315
		public TIHabTemplate habDesign;
	}
}
