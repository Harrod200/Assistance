using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1E RID: 2590
	public class RootNode : ITreeNode
	{
		// Token: 0x06006443 RID: 25667 RVA: 0x002F4943 File Offset: 0x002F2B43
		protected RootNode()
		{
		}

		// Token: 0x06006444 RID: 25668 RVA: 0x002F494C File Offset: 0x002F2B4C
		public RootNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
		{
			this._sharedData = shared;
			this._localData = local;
			this._childNodes = children;
			for (int i = 0; i < this._childNodes.Length; i++)
			{
				this._childNodes[i].SetParent(this);
			}
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x002F4995 File Offset: 0x002F2B95
		public virtual void SetParent(ITreeNode parent)
		{
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x002F4997 File Offset: 0x002F2B97
		public virtual CombatShipBehaviourTree.ConditionResponse Execute()
		{
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046C2 RID: 18114
		protected ITreeNode[] _childNodes;

		// Token: 0x040046C3 RID: 18115
		protected CombatShipBehaviourTree.SharedBehaviourData _sharedData;

		// Token: 0x040046C4 RID: 18116
		protected CombatShipBehaviourTree.LocalBehaviourData _localData;
	}
}
