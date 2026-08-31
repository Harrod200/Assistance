using System;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A34 RID: 2612
	public class IdleMissilesLeafNode : LeafNode
	{
		// Token: 0x06006493 RID: 25747 RVA: 0x002F798D File Offset: 0x002F5B8D
		protected IdleMissilesLeafNode()
		{
		}

		// Token: 0x06006494 RID: 25748 RVA: 0x002F7995 File Offset: 0x002F5B95
		public IdleMissilesLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x06006495 RID: 25749 RVA: 0x002F79A0 File Offset: 0x002F5BA0
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			foreach (IWeapon weapon in this._sharedData.ShipController.hull.IterateByClass<IWeapon>())
			{
				Weapon weapon2 = weapon as Weapon;
				if (weapon2.weaponTemplate.isMissileWeapon)
				{
					this._sharedData.ShipController.ShipState.faction.playerControl.StartAction(new SetWeaponModeAction(this._sharedData.ShipController.ShipState, weapon2, FireMode.Idle));
				}
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
