using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000267 RID: 615
public abstract class MissionTarget<T> : IMissionTarget<T>, IMissionTarget where T : TIGameState
{
	// Token: 0x060007F1 RID: 2033
	public abstract TIFactionState GetRelevantFaction(TIGameState target);

	// Token: 0x060007F2 RID: 2034
	public abstract List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target);

	// Token: 0x060007F3 RID: 2035
	public abstract IEnumerable<T> GetAllPotentialTargets(TIFactionState faction);

	// Token: 0x060007F4 RID: 2036
	public abstract IList<T> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor);

	// Token: 0x060007F5 RID: 2037 RVA: 0x00025165 File Offset: 0x00023365
	public bool ValidTarget(List<string> results)
	{
		if (results.Count != 0)
		{
			return results.All<string>((string x) => x == "_Pass");
		}
		return true;
	}

	// Token: 0x060007F6 RID: 2038 RVA: 0x00025196 File Offset: 0x00023396
	IEnumerable<TIGameState> IMissionTarget.GetAllPotentialTargets(TIFactionState faction)
	{
		return new List<TIGameState>(this.GetAllPotentialTargets(faction));
	}

	// Token: 0x060007F7 RID: 2039 RVA: 0x000251A4 File Offset: 0x000233A4
	IList<TIGameState> IMissionTarget.GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		return new List<TIGameState>(this.GetValidTargets(mission, councilor));
	}
}
