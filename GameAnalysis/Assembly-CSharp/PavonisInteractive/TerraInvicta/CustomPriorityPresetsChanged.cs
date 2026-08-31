using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005ED RID: 1517
	public class CustomPriorityPresetsChanged : GameEvent
	{
		// Token: 0x06002812 RID: 10258 RVA: 0x000D9D38 File Offset: 0x000D7F38
		public CustomPriorityPresetsChanged(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E10 RID: 7696
		public TIFactionState faction;
	}
}
