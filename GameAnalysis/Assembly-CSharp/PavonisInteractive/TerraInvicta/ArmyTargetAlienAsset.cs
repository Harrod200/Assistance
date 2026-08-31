using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200069B RID: 1691
	public class ArmyTargetAlienAsset : GameEvent
	{
		// Token: 0x060028C3 RID: 10435 RVA: 0x000DAA57 File Offset: 0x000D8C57
		public ArmyTargetAlienAsset(TIArmyState army, IOperation operationTemplate)
		{
			this.army = army;
			this.operationTemplate = operationTemplate;
		}

		// Token: 0x04001F03 RID: 7939
		public TIArmyState army;

		// Token: 0x04001F04 RID: 7940
		public IOperation operationTemplate;
	}
}
