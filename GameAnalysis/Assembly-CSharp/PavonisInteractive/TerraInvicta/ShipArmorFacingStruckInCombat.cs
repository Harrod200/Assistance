using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D9 RID: 1753
	public class ShipArmorFacingStruckInCombat : GameEvent
	{
		// Token: 0x06002905 RID: 10501 RVA: 0x000DAF08 File Offset: 0x000D9108
		public ShipArmorFacingStruckInCombat(TISpaceShipState ship, ArmorFacing armorFacing, TIShipWeaponTemplate weapon, float rawDamage, float penetratedDamage, float radiationDamage)
		{
			this.ship = ship;
			this.armorFacing = armorFacing;
			this.weapon = weapon;
			this.rawDamage = rawDamage;
			this.penetratedDamage = penetratedDamage;
			this.radiationDamage = radiationDamage;
		}

		// Token: 0x04001F60 RID: 8032
		public TISpaceShipState ship;

		// Token: 0x04001F61 RID: 8033
		public ArmorFacing armorFacing;

		// Token: 0x04001F62 RID: 8034
		public TIShipWeaponTemplate weapon;

		// Token: 0x04001F63 RID: 8035
		public float rawDamage;

		// Token: 0x04001F64 RID: 8036
		public float penetratedDamage;

		// Token: 0x04001F65 RID: 8037
		public float radiationDamage;
	}
}
