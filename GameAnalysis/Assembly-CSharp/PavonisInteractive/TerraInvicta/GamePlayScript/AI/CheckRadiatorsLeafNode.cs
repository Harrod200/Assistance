using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A35 RID: 2613
	public class CheckRadiatorsLeafNode : LeafNode
	{
		// Token: 0x06006496 RID: 25750 RVA: 0x002F7A40 File Offset: 0x002F5C40
		protected CheckRadiatorsLeafNode()
		{
		}

		// Token: 0x06006497 RID: 25751 RVA: 0x002F7AB4 File Offset: 0x002F5CB4
		public CheckRadiatorsLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x06006498 RID: 25752 RVA: 0x002F7B28 File Offset: 0x002F5D28
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			TISpaceShipState shipState = this._sharedData.ShipController.ShipState;
			if (shipState.radiators.vulnerability <= 1)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			if (shipState.heatFraction > 4f && this.extendCommand.ActorCanPerformCommand(shipState))
			{
				this.extendCommand.OnCommandExecute(shipState, null);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			if ((shipState.heatFraction < 2f || shipState.thrustersActive) && this.retractCommand.ActorCanPerformCommand(shipState))
			{
				this.retractCommand.OnCommandExecute(shipState, null);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Running;
		}

		// Token: 0x040046DB RID: 18139
		private IShipCommand retractCommand = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is RetractRadiatorsCommand);

		// Token: 0x040046DC RID: 18140
		private IShipCommand extendCommand = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is ExtendRadiatorsCommand);
	}
}
