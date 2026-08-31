using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1D RID: 2589
	public interface ITreeNode
	{
		// Token: 0x06006441 RID: 25665
		CombatShipBehaviourTree.ConditionResponse Execute();

		// Token: 0x06006442 RID: 25666
		void SetParent(ITreeNode parent);
	}
}
