using System;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A38 RID: 2616
	public class AssessShipRamOptionLeafNode : LeafNode
	{
		// Token: 0x0600649F RID: 25759 RVA: 0x002F7EC8 File Offset: 0x002F60C8
		protected AssessShipRamOptionLeafNode()
		{
		}

		// Token: 0x060064A0 RID: 25760 RVA: 0x002F7EFF File Offset: 0x002F60FF
		public AssessShipRamOptionLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064A1 RID: 25761 RVA: 0x002F7F38 File Offset: 0x002F6138
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.IsRammingTargetShip)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			CombatShipController combatShipController = this._sharedData.ShipController.primaryTarget as CombatShipController;
			if (combatShipController != null && this.rammingCommand.ActorCanPerformCommand(this._sharedData.ShipController.ShipState) && !this._sharedData.ShipController.ShipState.AnyWeaponCanFire() && !this._sharedData.ShipController.ShipState.isAlien)
			{
				float num = this._sharedData.ShipController.ShipState.combatAcceleration_kps2 * 0.05f;
				float num2 = this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps();
				float num3 = PhysicsHelpers.TimeFromDisplacementAndAcceleration((combatShipController.position - this._sharedData.ShipController.position).magnitude, num);
				float num4 = PhysicsHelpers.VelocityFromAccelerationAndTime(num, num3);
				float num5 = CollisionImpact.KineticEnergyDamage_MJ(combatShipController.ShipState.currentMass_kg, num4);
				float num6 = (combatShipController.ShipState.CriticalDamageTotal + num5) / (float)combatShipController.ShipState.hull.structuralIntegrity / 3f;
				if (num2 > this._sharedData.MinimumDVThreshold && (num6 >= 1f || this._sharedData.ShipController.ShipState.currentMass_kg > combatShipController.ShipState.currentMass_kg))
				{
					float num7 = 0f;
					if (this._sharedData.OpposingFleetController != null)
					{
						foreach (CombatShipController combatShipController2 in this._sharedData.OpposingFleetController.activeShipControllers)
						{
							float num8 = Vector3.Distance(this._sharedData.ShipController.position, combatShipController2.position) * 0.05f;
							foreach (IWeapon weapon in combatShipController2.hull.IterateByClass<IWeapon>())
							{
								if (weapon.target == this._sharedData.ShipController)
								{
									OffenseFireMode offenseFireMode = weapon.currentFireMode as OffenseFireMode;
									if (offenseFireMode != null)
									{
										num7 += offenseFireMode.GetExpectedDamage(num8, weapon.target);
									}
								}
							}
						}
					}
					float num9 = 0f;
					if (this._sharedData.ShipController.ShipState.CriticalDamageTotal > 0f)
					{
						num9 = num7 / this._sharedData.ShipController.ShipState.CriticalDamageTotal;
					}
					float num10 = Mathf.Clamp(this._sharedData.ShipController.ShipState.faction.aiValues.preserveLife, 0f, float.MaxValue);
					if (Mathf.Approximately(num10, 0f) || (num3 > num9 && num9 / num3 * num10 < TIUtilities.RandomRange(0f, 1f)))
					{
						this._localData.IsRammingTargetShip = true;
						this.rammingCommand.OnCommandExecute(this._sharedData.ShipController.ShipState, null);
						return CombatShipBehaviourTree.ConditionResponse.Success;
					}
				}
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x040046E0 RID: 18144
		private IShipCommand rammingCommand = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is RammingSpeedCommand);
	}
}
