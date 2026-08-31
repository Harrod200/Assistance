using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A20 RID: 2592
	public class LeafNode : ITreeNode
	{
		// Token: 0x06006449 RID: 25673 RVA: 0x002F49B4 File Offset: 0x002F2BB4
		protected LeafNode()
		{
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x002F49BC File Offset: 0x002F2BBC
		public LeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
		{
			this._sharedData = shared;
			this._localData = local;
			this._parentNode = null;
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x002F49D9 File Offset: 0x002F2BD9
		public virtual void SetParent(ITreeNode parent)
		{
			this._parentNode = parent;
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x002F49E2 File Offset: 0x002F2BE2
		public virtual CombatShipBehaviourTree.ConditionResponse Execute()
		{
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046C6 RID: 18118
		protected ITreeNode _parentNode;

		// Token: 0x040046C7 RID: 18119
		protected CombatShipBehaviourTree.SharedBehaviourData _sharedData;

		// Token: 0x040046C8 RID: 18120
		protected CombatShipBehaviourTree.LocalBehaviourData _localData;
	}
}
