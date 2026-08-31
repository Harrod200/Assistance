using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BE RID: 1726
	public class CombatTargetedableStateSelected : GameEvent
	{
		// Token: 0x060028EA RID: 10474 RVA: 0x000DACD0 File Offset: 0x000D8ED0
		public CombatTargetedableStateSelected(CombatTargetableState target, bool boxSelected = false, bool isGroupSelectPrimarySelection = false)
		{
			this.target = target;
			this.boxSelected = boxSelected;
			this.isGroupSelectPrimarySelection = isGroupSelectPrimarySelection;
		}

		// Token: 0x04001F2E RID: 7982
		public CombatTargetableState target;

		// Token: 0x04001F2F RID: 7983
		public bool boxSelected;

		// Token: 0x04001F30 RID: 7984
		public bool isGroupSelectPrimarySelection;
	}
}
