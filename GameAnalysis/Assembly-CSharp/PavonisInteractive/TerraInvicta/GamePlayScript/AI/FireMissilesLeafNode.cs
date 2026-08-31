using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A33 RID: 2611
	public class FireMissilesLeafNode : LeafNode
	{
		// Token: 0x06006490 RID: 25744 RVA: 0x002F7744 File Offset: 0x002F5944
		protected FireMissilesLeafNode()
		{
		}

		// Token: 0x06006491 RID: 25745 RVA: 0x002F77B8 File Offset: 0x002F59B8
		public FireMissilesLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x06006492 RID: 25746 RVA: 0x002F782C File Offset: 0x002F5A2C
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.TargetShip != null && TIGameState.Valid(this._localData.TargetShip.ShipState))
			{
				if (!this._localData.TargetShip.isMissileSaturated || this._sharedData.ShipController.ShipState.ShipStructuralDamage())
				{
					this.selectTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, this._localData.TargetShip.ShipState);
				}
				else
				{
					this.clearTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, null);
					this._localData.TargetShip = null;
				}
			}
			else if (this._localData.TargetModule != null && (!this._localData.TargetModule.isMissileSaturated || this._sharedData.ShipController.ShipState.ShipStructuralDamage()))
			{
				this.selectTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, this._localData.TargetModule.habModule);
			}
			else
			{
				this.clearTarget.OnCommandExecute(this._sharedData.ShipController.ShipState, null);
				this._localData.TargetModule = null;
				this._localData.TargetShip = null;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046D9 RID: 18137
		private IShipCommand selectTarget = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is SelectSalvoTargetCommand);

		// Token: 0x040046DA RID: 18138
		private IShipCommand clearTarget = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is ClearTargetCommand);
	}
}
