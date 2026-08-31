using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A44 RID: 2628
	public class DoNothingLeafNode : LeafNode
	{
		// Token: 0x060064C7 RID: 25799 RVA: 0x002F8BCC File Offset: 0x002F6DCC
		protected DoNothingLeafNode()
		{
		}

		// Token: 0x060064C8 RID: 25800 RVA: 0x002F8BD4 File Offset: 0x002F6DD4
		public DoNothingLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064C9 RID: 25801 RVA: 0x002F8BDE File Offset: 0x002F6DDE
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
