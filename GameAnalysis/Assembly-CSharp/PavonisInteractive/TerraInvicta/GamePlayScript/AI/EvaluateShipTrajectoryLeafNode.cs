using System;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A22 RID: 2594
	public class EvaluateShipTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006450 RID: 25680 RVA: 0x002F4F98 File Offset: 0x002F3198
		protected EvaluateShipTrajectoryLeafNode()
		{
		}

		// Token: 0x06006451 RID: 25681 RVA: 0x002F4FA0 File Offset: 0x002F31A0
		public EvaluateShipTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x06006452 RID: 25682 RVA: 0x002F4FAC File Offset: 0x002F31AC
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			float num = 0f;
			if (this._sharedData.OpposingFleetController != null)
			{
				foreach (CombatShipController combatShipController in this._sharedData.OpposingFleetController.activeShipControllers)
				{
					float num2 = Vector3.Distance(this._sharedData.ShipController.position, combatShipController.position) * 0.05f;
					foreach (IWeapon weapon in combatShipController.hull.IterateByClass<IWeapon>())
					{
						if (weapon.target == this._sharedData.ShipController)
						{
							OffenseFireMode offenseFireMode = weapon.currentFireMode as OffenseFireMode;
							if (offenseFireMode != null)
							{
								num += offenseFireMode.GetExpectedDamage(num2, combatShipController);
							}
						}
					}
				}
			}
			float num3 = float.MaxValue;
			if (this._sharedData.ShipController.ShipState.CriticalDamageTotal > 0f)
			{
				num3 = num / this._sharedData.ShipController.ShipState.CriticalDamageTotal;
			}
			float num4 = 0f;
			if (this._localData.TargetShip)
			{
				float num5 = 0f;
				float num6 = Vector3.Distance(this._sharedData.ShipController.position, this._localData.TargetShip.position) * 0.05f;
				foreach (IWeapon weapon2 in this._sharedData.ShipController.hull.IterateByClass<IWeapon>())
				{
					if (weapon2.target == this._localData.TargetShip)
					{
						OffenseFireMode offenseFireMode2 = weapon2.currentFireMode as OffenseFireMode;
						if (offenseFireMode2 != null)
						{
							num5 += offenseFireMode2.GetExpectedDamage(num6, weapon2.target);
						}
					}
				}
				num4 = float.MaxValue;
				if (this._localData.TargetShip.ShipState.CriticalDamageTotal > 0f)
				{
					num4 = num5 / this._localData.TargetShip.ShipState.CriticalDamageTotal;
				}
			}
			else if (this._localData.TargetModule)
			{
				float num7 = 0f;
				float num8 = Vector3.Distance(this._sharedData.ShipController.position, this._localData.TargetModule.position) * 0.05f;
				foreach (IWeapon weapon3 in this._sharedData.ShipController.hull.IterateByClass<IWeapon>())
				{
					if (weapon3.target == this._localData.TargetModule)
					{
						OffenseFireMode offenseFireMode3 = weapon3.currentFireMode as OffenseFireMode;
						if (offenseFireMode3 != null)
						{
							num7 += offenseFireMode3.GetExpectedDamage(num8, weapon3.target);
						}
					}
				}
				num4 = this._localData.TargetModule.hitPoints / num7;
			}
			switch (this._sharedData.Priority)
			{
			case CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Aggression:
				if (num4 >= num3)
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
				return CombatShipBehaviourTree.ConditionResponse.Success;
			case CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.Defensive:
				if (num4 <= num3)
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
				return CombatShipBehaviourTree.ConditionResponse.Success;
			case CombatShipBehaviourTree.SharedBehaviourData.FleetPriority.BestForShip:
				return CombatShipBehaviourTree.ConditionResponse.Success;
			default:
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
		}
	}
}
