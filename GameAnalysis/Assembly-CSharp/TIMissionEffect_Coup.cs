using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001DA RID: 474
public class TIMissionEffect_Coup : TIMissionEffect
{
	// Token: 0x060006A0 RID: 1696 RVA: 0x0001F180 File Offset: 0x0001D380
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TINationState ref_nation = target.ref_nation;
		if (ref_nation == null)
		{
			return string.Empty;
		}
		if (base.MissionSuccess(outcome))
		{
			int num = ((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1);
			ref_nation.Coup(councilor, num);
			return num.ToString();
		}
		if (outcome == TIMissionOutcome.CriticalFailure)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<TIControlPoint> list = ref_nation.FactionControlPoints(councilor.faction, false, false, true);
			if (list.Count > 0 && list.Count < ref_nation.numControlPoints)
			{
				list.ForEach(delegate(TIControlPoint x)
				{
					x.ResolveCrackdownEffect(3, councilor.faction, false, false, 0f);
				});
				stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special3").ToString()));
			}
			TIFactionState tifactionState = ref_nation.WeightedRandomFactionByControlPoints();
			if (tifactionState != mission.councilor.faction)
			{
				if (tifactionState == null || councilor.isAlien)
				{
					float num2 = ref_nation.PropagandaOnPop(councilor.faction.ideology, -5f, false);
					stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[] { num2.ToPercent("P0") }));
				}
				else
				{
					councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
					stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[] { tifactionState.displayNameWithColor }));
				}
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}
}
