using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000265 RID: 613
public interface IMissionTarget
{
	// Token: 0x060007EA RID: 2026
	TIFactionState GetRelevantFaction(TIGameState target);

	// Token: 0x060007EB RID: 2027
	IList<TIGameState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor);

	// Token: 0x060007EC RID: 2028
	List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target);

	// Token: 0x060007ED RID: 2029
	bool ValidTarget(List<string> results);

	// Token: 0x060007EE RID: 2030
	IEnumerable<TIGameState> GetAllPotentialTargets(TIFactionState faction);
}
