using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1F RID: 2591
	public class BranchNode : RootNode
	{
		// Token: 0x06006447 RID: 25671 RVA: 0x002F499A File Offset: 0x002F2B9A
		protected BranchNode()
		{
		}

		// Token: 0x06006448 RID: 25672 RVA: 0x002F49A2 File Offset: 0x002F2BA2
		public BranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
			this._parentNode = null;
		}

		// Token: 0x040046C5 RID: 18117
		protected ITreeNode _parentNode;
	}
}
