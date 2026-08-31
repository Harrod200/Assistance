using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200062C RID: 1580
	public class ObjectiveComplete : GameEvent
	{
		// Token: 0x06002851 RID: 10321 RVA: 0x000DA1BE File Offset: 0x000D83BE
		public ObjectiveComplete(TIObjectiveTemplate template, TIFactionState faction)
		{
			this.template = template;
			this.faction = faction;
		}

		// Token: 0x04001E68 RID: 7784
		public TIObjectiveTemplate template;

		// Token: 0x04001E69 RID: 7785
		public TIFactionState faction;
	}
}
