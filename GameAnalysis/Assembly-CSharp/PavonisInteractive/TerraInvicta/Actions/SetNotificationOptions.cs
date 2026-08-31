using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A97 RID: 2711
	public class SetNotificationOptions : PlayerAction
	{
		// Token: 0x06006583 RID: 25987 RVA: 0x002FD1FE File Offset: 0x002FB3FE
		public SetNotificationOptions(TIFactionState faction, string templateDataName, int type, NotificationOverrideBehavior behavior)
		{
			this.factionID = faction.ID;
			this.templateDataName = templateDataName;
			this.notificationType = type;
			this.overrideBehavior = behavior;
		}

		// Token: 0x06006584 RID: 25988 RVA: 0x002FD228 File Offset: 0x002FB428
		public override void Execute()
		{
			this.factionID.GetState<TIFactionState>(false).SetNotificationPreference(this.templateDataName, this.notificationType, this.overrideBehavior);
		}

		// Token: 0x040047C9 RID: 18377
		private GameStateID factionID;

		// Token: 0x040047CA RID: 18378
		private string templateDataName;

		// Token: 0x040047CB RID: 18379
		private int notificationType;

		// Token: 0x040047CC RID: 18380
		private NotificationOverrideBehavior overrideBehavior;
	}
}
