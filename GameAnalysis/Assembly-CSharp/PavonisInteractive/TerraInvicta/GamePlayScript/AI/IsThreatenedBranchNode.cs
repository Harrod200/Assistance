using System;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A12 RID: 2578
	public class IsThreatenedBranchNode : BranchNode
	{
		// Token: 0x0600641B RID: 25627 RVA: 0x002F4132 File Offset: 0x002F2332
		protected IsThreatenedBranchNode()
		{
		}

		// Token: 0x0600641C RID: 25628 RVA: 0x002F413A File Offset: 0x002F233A
		public IsThreatenedBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x0600641D RID: 25629 RVA: 0x002F4148 File Offset: 0x002F2348
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (!this._sharedData.ShipController.ShipState.canSuicide && this._localData.IsRammingTargetShip)
			{
				this._localData.IsRammingTargetShip = false;
				this._sharedData.ShipController.ShipState.faction.playerControl.StartAction(new SetRammingSpeedAction(this._sharedData.ShipController.ShipState, false));
			}
			if (this._sharedData.ShipController.ShipState.canSuicide)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			if (this.GetUncontestedProjectileCount() == 0)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			for (int i = 0; i < this._childNodes.Length; i++)
			{
				this._childNodes[i].Execute();
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x0600641E RID: 25630 RVA: 0x002F4200 File Offset: 0x002F2400
		private int GetUncontestedProjectileCount()
		{
			int num = 0;
			foreach (ProjectileController projectileController in GameControl.spaceCombat._projectiles.Values)
			{
				if (!(projectileController == null) && !projectileController.hasHit && !projectileController.beenDestroyed && projectileController.clearedLauncher && projectileController.projectileState.shootingFaction != this._sharedData.FactionState && projectileController.warheadMass_kg > 1f && TIUtilities.WillHitSphere(this._sharedData.ShipController.position, this._sharedData.ShipController.velocityVector, projectileController.position, projectileController.velocityVector, this._sharedData.ShipController.ShipState.hull.length_m) && !this._sharedData.ShipController.IsProjectileContested(projectileController))
				{
					num++;
				}
			}
			Utilities.DebugDrawSphere(this._sharedData.ShipController.position, Quaternion.identity, this._sharedData.ShipController.ref_shipController.ShipState.hull.length_m * 0.5f * GameControl.spaceCombat.modelScalingFactor, (num > 0) ? Color.red : Color.green, 8, 0f);
			return num;
		}
	}
}
