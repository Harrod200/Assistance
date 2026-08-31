using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026F RID: 623
public class TIMissionTarget_Councilor : MissionTarget<TICouncilorState>
{
	// Token: 0x0600081C RID: 2076 RVA: 0x00025B08 File Offset: 0x00023D08
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x00025B10 File Offset: 0x00023D10
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		List<string> list = new List<string>();
		if (((ref_councilor != null) ? ref_councilor.faction : null) != null)
		{
			using (List<TIMissionCondition>.Enumerator enumerator = mission.conditions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIMissionCondition timissionCondition = enumerator.Current;
					list.Add(timissionCondition.CanTarget(councilor, ref_councilor));
				}
				return list;
			}
		}
		list.Add("_Fail");
		return list;
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x00025B98 File Offset: 0x00023D98
	public override IEnumerable<TICouncilorState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors);
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x00025BC4 File Offset: 0x00023DC4
	public override IList<TICouncilorState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TICouncilorState> list = new List<TICouncilorState>();
		foreach (TICouncilorState ticouncilorState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, ticouncilorState)))
			{
				list.Add(ticouncilorState);
			}
		}
		return list;
	}
}
