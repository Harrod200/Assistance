using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A31 RID: 2609
	public class FindTargetShipLeafNode : LeafNode
	{
		// Token: 0x06006486 RID: 25734 RVA: 0x002F6B14 File Offset: 0x002F4D14
		protected FindTargetShipLeafNode()
		{
		}

		// Token: 0x06006487 RID: 25735 RVA: 0x002F6B88 File Offset: 0x002F4D88
		public FindTargetShipLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
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

		// Token: 0x06006488 RID: 25736 RVA: 0x002F6C90 File Offset: 0x002F4E90
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.TargetShip && (this._localData.TargetShip.isDestroyed || this._localData.TargetShip.destructionTriggered || this._localData.TargetShip.departed))
			{
				this._localData.TargetShip = null;
			}
			if (this._localData.TargetModule && (this._localData.TargetModule.isDestroyed || this._localData.TargetModule.destructionTriggered))
			{
				this._localData.TargetModule = null;
			}
			if (this.TryAssignTargetShip())
			{
				this.selectTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, this._localData.TargetShip.ShipState);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			if (this.TryAssignTargetModule())
			{
				this.selectTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, this._localData.TargetModule.habModule);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			this.clearTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, null);
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x06006489 RID: 25737 RVA: 0x002F6DBC File Offset: 0x002F4FBC
		protected virtual bool TryAssignTargetShip()
		{
			switch (this._localData.TargetType)
			{
			case CombatTargetPriority.Weakest:
			{
				float num = float.MaxValue;
				if (this._sharedData.OpposingFleetController == null)
				{
					goto IL_020D;
				}
				using (IEnumerator<CombatShipController> enumerator = this._sharedData.OpposingFleetController.activeShipControllers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CombatShipController combatShipController = enumerator.Current;
						if (!combatShipController.isDestroyed && !combatShipController.destructionTriggered && (!this._sharedData.ShipController.AI_IsMissileBoat || !combatShipController.isMissileSaturated))
						{
							float worstArmor = combatShipController.ShipState.GetWorstArmor();
							if (num > worstArmor)
							{
								num = worstArmor;
								this._localData.TargetShip = combatShipController;
							}
						}
					}
					goto IL_020D;
				}
				break;
			}
			case CombatTargetPriority.Closest:
				break;
			case CombatTargetPriority.Strongest:
				goto IL_0170;
			default:
				return false;
			}
			float num2 = float.MaxValue;
			if (this._sharedData.OpposingFleetController == null)
			{
				goto IL_020D;
			}
			using (IEnumerator<CombatShipController> enumerator = this._sharedData.OpposingFleetController.activeShipControllers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CombatShipController combatShipController2 = enumerator.Current;
					if (!combatShipController2.isDestroyed && !combatShipController2.destructionTriggered && (!this._sharedData.ShipController.AI_IsMissileBoat || !combatShipController2.isMissileSaturated))
					{
						float magnitude = (combatShipController2.position - this._sharedData.ShipController.position).magnitude;
						if (num2 > magnitude)
						{
							num2 = magnitude;
							this._localData.TargetShip = combatShipController2;
						}
					}
				}
				goto IL_020D;
			}
			IL_0170:
			float num3 = 0f;
			if (this._sharedData.OpposingFleetController != null)
			{
				foreach (CombatShipController combatShipController3 in this._sharedData.OpposingFleetController.activeShipControllers)
				{
					if (!combatShipController3.isDestroyed && !combatShipController3.destructionTriggered && (!this._sharedData.ShipController.AI_IsMissileBoat || !combatShipController3.isMissileSaturated))
					{
						float bestArmor = combatShipController3.ShipState.GetBestArmor();
						if (num3 < bestArmor)
						{
							num3 = bestArmor;
							this._localData.TargetShip = combatShipController3;
						}
					}
				}
			}
			IL_020D:
			return this._localData.TargetShip;
		}

		// Token: 0x0600648A RID: 25738 RVA: 0x002F7014 File Offset: 0x002F5214
		protected virtual bool TryAssignTargetModule()
		{
			TIDateTime tidateTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			for (int i = 0; i < 5; i++)
			{
				tidateTime.AddSeconds(60.0);
			}
			Vector3 vector = this._sharedData.ShipController.positionAtTime(tidateTime.ExportTime());
			float num = float.MaxValue;
			foreach (CombatHabModuleController combatHabModuleController in this._sharedData.HabModuleControllers)
			{
				if (this._sharedData.FactionState != combatHabModuleController.faction && !combatHabModuleController.isDestroyed && !combatHabModuleController.destructionTriggered && combatHabModuleController.weapons != null && combatHabModuleController.weapons.Count > 0)
				{
					if (this._localData.TargetModule == null)
					{
						this._localData.TargetModule = combatHabModuleController;
						num = (combatHabModuleController.positionAtTime(tidateTime.ExportTime()) - vector).magnitude;
					}
					else
					{
						float magnitude = (combatHabModuleController.positionAtTime(tidateTime.ExportTime()) - vector).magnitude;
						if (num > magnitude)
						{
							this._localData.TargetModule = combatHabModuleController;
							num = magnitude;
						}
					}
				}
			}
			return this._localData.TargetModule;
		}

		// Token: 0x040046D5 RID: 18133
		private IShipCommand selectTarget = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is SelectTargetCommand);

		// Token: 0x040046D6 RID: 18134
		private IShipCommand clearTarget = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is ClearTargetCommand);
	}
}
