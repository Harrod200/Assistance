using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A37 RID: 2615
	public class AssessDisengageOptionLeafNode : LeafNode
	{
		// Token: 0x0600649C RID: 25756 RVA: 0x002F7C04 File Offset: 0x002F5E04
		protected AssessDisengageOptionLeafNode()
		{
			this.extremeDistance_scale_sqrMag = SpaceCombatManager.km_to_scale(31315f) * SpaceCombatManager.km_to_scale(31315f);
			this.disengagementDistance_scale_sqrMag = SpaceCombatManager.km_to_scale(2000f) * SpaceCombatManager.km_to_scale(2000f);
		}

		// Token: 0x0600649D RID: 25757 RVA: 0x002F7C7C File Offset: 0x002F5E7C
		public AssessDisengageOptionLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.extremeDistance_scale_sqrMag = SpaceCombatManager.km_to_scale(31315f) * SpaceCombatManager.km_to_scale(31315f);
			this.disengagementDistance_scale_sqrMag = SpaceCombatManager.km_to_scale(2000f) * SpaceCombatManager.km_to_scale(2000f);
		}

		// Token: 0x0600649E RID: 25758 RVA: 0x002F7CF8 File Offset: 0x002F5EF8
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.IsDisengaging)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			TISpaceShipState shipState = this._sharedData.ShipController.ShipState;
			if (shipState.disengageFromCombat)
			{
				this._localData.IsDisengaging = true;
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			float num = this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps();
			CombatantController combatantController = this._sharedData.ShipController.primaryTarget;
			if (combatantController == null)
			{
				if (this._localData.TargetShip != null)
				{
					combatantController = this._localData.TargetShip;
				}
				else if (this._localData.TargetModule != null)
				{
					combatantController = this._localData.TargetModule;
				}
			}
			if ((!shipState.CanRotateAndRoll() || !shipState.CanSetWaypoints() || shipState.ThrustEffectivenessRatio == 0f || (shipState.PartDestroyed(shipState.radiatorModule) && shipState.overheated) || (shipState.AllWeaponsDisabledBeyondFieldRepair() || (combatantController != null && num < this._sharedData.MinimumDVThreshold)) || (combatantController != null && (combatantController.position - this._sharedData.ShipController.position).sqrMagnitude > this.extremeDistance_scale_sqrMag) || (GameControl.spaceCombat.combatState.stances[this._sharedData.FleetController.faction] == CombatStance.Evade && combatantController != null && (combatantController.position - this._sharedData.ShipController.position).sqrMagnitude > this.disengagementDistance_scale_sqrMag)) && this.disengageCommand.ActorCanPerformCommand(shipState))
			{
				this._localData.IsDisengaging = true;
				this.disengageCommand.OnCommandExecute(shipState, null);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x040046DD RID: 18141
		private float extremeDistance_scale_sqrMag;

		// Token: 0x040046DE RID: 18142
		private float disengagementDistance_scale_sqrMag;

		// Token: 0x040046DF RID: 18143
		private IShipCommand disengageCommand = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is DisengageCommand);
	}
}
