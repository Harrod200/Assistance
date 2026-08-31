using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A13 RID: 2579
	public class AssessShipIsMissileCarrierBranchNode : BranchNode
	{
		// Token: 0x0600641F RID: 25631 RVA: 0x002F437C File Offset: 0x002F257C
		protected AssessShipIsMissileCarrierBranchNode()
		{
		}

		// Token: 0x06006420 RID: 25632 RVA: 0x002F4384 File Offset: 0x002F2584
		public AssessShipIsMissileCarrierBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006421 RID: 25633 RVA: 0x002F4390 File Offset: 0x002F2590
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			foreach (ModuleDataEntry moduleDataEntry in this._sharedData.ShipController.ShipState.AllWeaponModuleData())
			{
				if (moduleDataEntry.weaponTemplate.isMissileWeapon && !this._sharedData.ShipController.ShipState.PartDestroyed(moduleDataEntry) && this._sharedData.ShipController.ShipState.WeaponHasAmmo(moduleDataEntry))
				{
					for (int i = 0; i < this._childNodes.Length; i++)
					{
						this._childNodes[i].Execute();
					}
					return CombatShipBehaviourTree.ConditionResponse.Success;
				}
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
