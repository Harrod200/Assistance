using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A95 RID: 2709
	public class SetLongTermTechTargetAction : PlayerAction
	{
		// Token: 0x0600657F RID: 25983 RVA: 0x002FD16E File Offset: 0x002FB36E
		public SetLongTermTechTargetAction(TIFactionState faction, string dataName)
		{
			this.factionID = faction.ID;
			this.dataName = dataName;
		}

		// Token: 0x06006580 RID: 25984 RVA: 0x002FD189 File Offset: 0x002FB389
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).SetLongTermTechTarget(this.dataName);
		}

		// Token: 0x040047C4 RID: 18372
		public GameStateID factionID;

		// Token: 0x040047C5 RID: 18373
		public string dataName;
	}
}
