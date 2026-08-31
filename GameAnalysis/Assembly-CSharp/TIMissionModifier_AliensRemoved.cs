using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200022F RID: 559
public class TIMissionModifier_AliensRemoved : TIMissionModifier_HideInCodex
{
	// Token: 0x06000763 RID: 1891 RVA: 0x000233C9 File Offset: 0x000215C9
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.aliensRemoved > 0;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x000233D4 File Offset: 0x000215D4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.ref_faction.IsAlienFaction)
		{
			return (float)attackingCouncilor.faction.aliensRemoved * TemplateManager.global.TIMissionModifier_AliensRemoved_Scaling;
		}
		return 0f;
	}
}
