using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200026A RID: 618
public class TIMissionTarget_Army : MissionTarget<TIArmyState>
{
	// Token: 0x06000803 RID: 2051 RVA: 0x000253AC File Offset: 0x000235AC
	public override TIFactionState GetRelevantFaction(TIGameState target)
	{
		return target.ref_faction;
	}

	// Token: 0x06000804 RID: 2052 RVA: 0x000253B4 File Offset: 0x000235B4
	public override List<string> ValidateSingleTarget(TIMissionTemplate mission, TICouncilorState councilor, TIGameState target)
	{
		TIArmyState ref_army = target.ref_army;
		List<string> list = new List<string>();
		foreach (TIMissionCondition timissionCondition in mission.conditions)
		{
			list.Add(timissionCondition.CanTarget(councilor, ref_army));
		}
		return list;
	}

	// Token: 0x06000805 RID: 2053 RVA: 0x0002541C File Offset: 0x0002361C
	public override IList<TIArmyState> GetValidTargets(TIMissionTemplate mission, TICouncilorState councilor)
	{
		List<TIArmyState> list = new List<TIArmyState>();
		foreach (TIArmyState tiarmyState in this.GetAllPotentialTargets(null))
		{
			if (base.ValidTarget(this.ValidateSingleTarget(mission, councilor, tiarmyState)))
			{
				list.Add(tiarmyState);
			}
		}
		return list;
	}

	// Token: 0x06000806 RID: 2054 RVA: 0x00025484 File Offset: 0x00023684
	public override IEnumerable<TIArmyState> GetAllPotentialTargets(TIFactionState faction = null)
	{
		return GameStateManager.AllExtantNations().SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies);
	}
}
