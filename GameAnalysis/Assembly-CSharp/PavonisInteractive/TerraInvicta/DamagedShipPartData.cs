using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C3 RID: 1987
	public class DamagedShipPartData
	{
		// Token: 0x060046D4 RID: 18132 RVA: 0x001CF777 File Offset: 0x001CD977
		public DamagedShipPartData(ModuleDataEntry module, float damage)
		{
			this.module = module;
			this.damage = damage;
		}

		// Token: 0x060046D5 RID: 18133 RVA: 0x001CF78D File Offset: 0x001CD98D
		public bool SamePart(TIShipPartTemplate comparePart, int compareSlot)
		{
			return comparePart == this.module.moduleTemplate && this.module.slotIndex == compareSlot;
		}

		// Token: 0x0400291F RID: 10527
		public ModuleDataEntry module;

		// Token: 0x04002920 RID: 10528
		public float damage;
	}
}
