using System;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3A RID: 2618
	public class AssessTargetClusterIsWithinRange : LeafNode
	{
		// Token: 0x060064A5 RID: 25765 RVA: 0x002F8319 File Offset: 0x002F6519
		protected AssessTargetClusterIsWithinRange()
		{
		}

		// Token: 0x060064A6 RID: 25766 RVA: 0x002F8321 File Offset: 0x002F6521
		public AssessTargetClusterIsWithinRange(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064A7 RID: 25767 RVA: 0x002F832C File Offset: 0x002F652C
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.TargetShip)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			if (!this._localData.TargetModule)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			if (this._sharedData.FleetController.activeShipControllers.Where<CombatShipController>((CombatShipController x) => x != this._sharedData.ShipController).Where<CombatShipController>(new Func<CombatShipController, bool>(AssessTargetClusterIsWithinRange.<Execute>g__IsFiringMissilesAtHabModules|2_0)).Any<CombatShipController>())
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			float num = this._sharedData.ShipController.ShipState.allWeaponTemplates.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMissileWeapon).Average<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km);
			float scaledTargetingRange = SpaceCombatManager.km_to_scale(num);
			if ((float)this._sharedData.HabModuleControllers.Count<CombatHabModuleController>((CombatHabModuleController x) => Vector3.Distance(x.position, this._sharedData.ShipController.position) <= scaledTargetingRange) / (float)this._sharedData.HabModuleControllers.Length >= 0.75f)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x060064A8 RID: 25768 RVA: 0x002F844C File Offset: 0x002F664C
		[CompilerGenerated]
		internal static bool <Execute>g__IsFiringMissilesAtHabModules|2_0(CombatShipController otherShip)
		{
			return otherShip.weapons.Any<IWeapon>(delegate(IWeapon weapon)
			{
				MissileWeapon missileWeapon = weapon as MissileWeapon;
				return missileWeapon != null && weapon.currentFireMode is SalvoFireMode && weapon.target is CombatHabModuleController && otherShip.WeaponCarrierState.WeaponIsOperable(missileWeapon.weaponData);
			});
		}
	}
}
