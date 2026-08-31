using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C2 RID: 1730
	public class HabModuleDamagedInCombat : GameEvent
	{
		// Token: 0x060028EE RID: 10478 RVA: 0x000DAD13 File Offset: 0x000D8F13
		public HabModuleDamagedInCombat(TIHabModuleState habModule, TIShipWeaponTemplate weapon, float rawDamage, float absorbedDamage)
		{
			this.habModule = habModule;
			this.weapon = weapon;
			this.rawDamage = rawDamage;
			this.absorbedDamage = absorbedDamage;
		}

		// Token: 0x04001F33 RID: 7987
		public TIHabModuleState habModule;

		// Token: 0x04001F34 RID: 7988
		public TIShipWeaponTemplate weapon;

		// Token: 0x04001F35 RID: 7989
		public float rawDamage;

		// Token: 0x04001F36 RID: 7990
		public float absorbedDamage;
	}
}
