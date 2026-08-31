using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200078B RID: 1931
	public interface CombatWeaponCarrierState
	{
		// Token: 0x06003D79 RID: 15737
		TIGameState GetTargetableState();

		// Token: 0x06003D7A RID: 15738
		TIFactionState GetFaction();

		// Token: 0x06003D7B RID: 15739
		bool WeaponIsOperable(ModuleDataEntry weaponData);

		// Token: 0x06003D7C RID: 15740
		bool WeaponCanFire(ModuleDataEntry weaponData);

		// Token: 0x06003D7D RID: 15741
		void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile = null);

		// Token: 0x06003D7E RID: 15742
		void AddTargetedProjectile(TISpaceCombatProjectileState projectile);

		// Token: 0x06003D7F RID: 15743
		float FireControlFunction();

		// Token: 0x06003D80 RID: 15744
		TISpaceShipState ref_shipCarrier();

		// Token: 0x06003D81 RID: 15745
		TIHabModuleState ref_habModuleCarrier();

		// Token: 0x06003D82 RID: 15746
		bool isShip();

		// Token: 0x06003D83 RID: 15747
		bool isHabModule();

		// Token: 0x06003D84 RID: 15748
		float TargetingBonus(TIShipWeaponTemplate weapon, TIHabState alliedHab);
	}
}
