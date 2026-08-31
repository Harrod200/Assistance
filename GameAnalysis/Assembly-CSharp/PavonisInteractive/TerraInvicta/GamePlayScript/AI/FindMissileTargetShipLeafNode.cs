using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A32 RID: 2610
	public class FindMissileTargetShipLeafNode : FindTargetShipLeafNode
	{
		// Token: 0x0600648B RID: 25739 RVA: 0x002F7172 File Offset: 0x002F5372
		protected FindMissileTargetShipLeafNode()
		{
		}

		// Token: 0x0600648C RID: 25740 RVA: 0x002F71AC File Offset: 0x002F53AC
		public FindMissileTargetShipLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.retargetTime = shared.CurrentTime;
			switch (this._sharedData.ShipController.ShipState.role)
			{
			case ShipRole.LS_Penetrator:
			case ShipRole.MS_Strike:
			case ShipRole.SS_Interceptor:
				this._localData.CombatReady = true;
				this._localData.TargetType = CombatTargetPriority.Weakest;
				return;
			case ShipRole.LM_Protector:
			case ShipRole.LM_Interdictor:
			case ShipRole.LL_Bomber:
			case ShipRole.MM_SpaceSuperiority:
			case ShipRole.SM_Patrol:
				this._localData.CombatReady = true;
				this._localData.TargetType = CombatTargetPriority.Closest;
				return;
			case ShipRole.LL_Intruder:
			case ShipRole.ML_Standoff:
			case ShipRole.SL_Defender:
				this._localData.CombatReady = true;
				this._localData.TargetType = CombatTargetPriority.Strongest;
				return;
			default:
				return;
			}
		}

		// Token: 0x0600648D RID: 25741 RVA: 0x002F7294 File Offset: 0x002F5494
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			double totalSeconds = (this.retargetTime - this._sharedData.CurrentTime).TotalSeconds;
			bool flag = totalSeconds < 10.0 && this._localData.TargetShip == null && this._localData.TargetModule == null;
			if (totalSeconds > 0.0 && !flag)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			if (this._localData.TargetShip && (this._localData.TargetShip.isDestroyed || this._localData.TargetShip.destructionTriggered || this._localData.TargetShip.departed))
			{
				this._localData.TargetShip = null;
			}
			if (this._localData.TargetModule && (this._localData.TargetModule.isDestroyed || this._localData.TargetModule.destructionTriggered))
			{
				this._localData.TargetModule = null;
			}
			this.retargetTime = new TIDateTime(this._sharedData.CurrentTime);
			this.retargetTime.AddSeconds(15.0);
			if (this.TryAssignTarget())
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			this.clearTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, null);
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x0600648E RID: 25742 RVA: 0x002F73EC File Offset: 0x002F55EC
		private bool TryAssignTarget()
		{
			CombatantController combatantController = null;
			float num = float.MaxValue;
			bool flag = this._sharedData.ShipController.ShipState.ShipStructuralDamage();
			if (this._sharedData.OpposingFleetController != null)
			{
				foreach (CombatShipController combatShipController in this._sharedData.OpposingFleetController.activeShipControllers)
				{
					if (!combatShipController.isDestroyed && !combatShipController.destructionTriggered && (!combatShipController.isMissileSaturated || flag))
					{
						float sqrMagnitude = (combatShipController.position - this._sharedData.ShipController.position).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							combatantController = combatShipController;
						}
					}
				}
			}
			if (this._sharedData.HabModuleControllers != null)
			{
				foreach (CombatHabModuleController combatHabModuleController in this._sharedData.HabModuleControllers)
				{
					if (this._sharedData.FactionState != combatHabModuleController.faction && !combatHabModuleController.isDestroyed && !combatHabModuleController.destructionTriggered && (!combatHabModuleController.isMissileSaturated || flag))
					{
						float sqrMagnitude2 = (combatHabModuleController.position - this._sharedData.ShipController.position).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							combatantController = combatHabModuleController;
						}
					}
				}
			}
			if (combatantController != null)
			{
				if (combatantController.ref_shipController != null)
				{
					this._localData.TargetShip = (CombatShipController)combatantController;
				}
				else
				{
					this._localData.TargetModule = (CombatHabModuleController)combatantController;
				}
				this.LogMissingTargetCrashData(combatantController);
				return true;
			}
			return false;
		}

		// Token: 0x0600648F RID: 25743 RVA: 0x002F75A4 File Offset: 0x002F57A4
		private void LogMissingTargetCrashData(CombatantController badTarget)
		{
			try
			{
				badTarget.GetCombatantState().GetTargetableState().ID.GetState<TIGameState>(true);
			}
			catch (NullReferenceException ex)
			{
				Debug.Log("Null Ref Exception Hit during Targeting. Full message to follow debug information:");
				Debug.Log("Null Target ID: " + badTarget.GetCombatantState().GetTargetableState().ID.ToString());
				Debug.Log("====== Start Enemy List ======");
				foreach (CombatShipController combatShipController in this._sharedData.OpposingFleetController.activeShipControllers)
				{
					Debug.Log(string.Concat(new string[]
					{
						string.Format("Game State ID: {0},", combatShipController.GetCombatantState().GetTargetableState().ID),
						" Enemy Name: ",
						combatShipController.UIController().combatantListItemController.shipName.text,
						",",
						string.Format(" isDestroyed: {0}, destructionTriggered: {1},", badTarget.isDestroyed, badTarget.destructionTriggered),
						string.Format(" departed: {0}", badTarget.ref_shipController != null && badTarget.ref_shipController.departed)
					}));
				}
				Debug.Log("====== End Enemy List ======");
				throw new NullReferenceException(ex.Message);
			}
		}

		// Token: 0x040046D7 RID: 18135
		private TIDateTime retargetTime;

		// Token: 0x040046D8 RID: 18136
		private IShipCommand clearTarget = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is ClearTargetCommand);
	}
}
