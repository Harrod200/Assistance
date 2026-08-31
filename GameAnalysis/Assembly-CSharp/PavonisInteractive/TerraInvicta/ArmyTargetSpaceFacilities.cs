using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200069A RID: 1690
	public class ArmyTargetSpaceFacilities : GameEvent
	{
		// Token: 0x060028C2 RID: 10434 RVA: 0x000DAA41 File Offset: 0x000D8C41
		public ArmyTargetSpaceFacilities(TIArmyState army, IOperation operationTemplate)
		{
			this.army = army;
			this.operationTemplate = operationTemplate;
		}

		// Token: 0x04001F01 RID: 7937
		public TIArmyState army;

		// Token: 0x04001F02 RID: 7938
		public IOperation operationTemplate;
	}
}
