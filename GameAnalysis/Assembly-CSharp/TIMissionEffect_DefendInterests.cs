using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001D2 RID: 466
public class TIMissionEffect_DefendInterests : TIMissionEffect
{
	// Token: 0x0600068A RID: 1674 RVA: 0x0001DE6C File Offset: 0x0001C06C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		string text = string.Empty;
		if (target.isNationState)
		{
			List<TIControlPoint> list = target.ref_nation.FactionControlPoints(mission.councilor.faction, false, false, true);
			Dictionary<TIControlPoint, float> dictionary = list.ToDictionary<TIControlPoint, TIControlPoint, float>((TIControlPoint x) => x, (TIControlPoint x) => 0f);
			foreach (TIControlPoint ticontrolPoint in dictionary.Keys.ToList<TIControlPoint>())
			{
				if (ticontrolPoint.defendExpiration == null)
				{
					dictionary[ticontrolPoint] = 0f;
				}
				else
				{
					dictionary[ticontrolPoint] = (float)ticontrolPoint.defendExpiration.DifferenceInDays(TITimeState.Now());
				}
			}
			Dictionary<TIControlPoint, float> dictionary2 = new Dictionary<TIControlPoint, float>(dictionary);
			int num = TemplateManager.global.defendInterestPerCPDuration_days * list.Count + TemplateManager.global.defendInterestDistributableDuration_days;
			num += (int)TIEffectsState.SumEffectsModifiers(Context.DefendInterestEarthDuration, mission.councilor.faction, (float)num, null);
			num = (int)((float)num * (2f / TIMissionPhaseState.phasesPerMonth));
			for (int i = 0; i < num; i++)
			{
				float targetValue = dictionary2.Values.Min();
				TIControlPoint key = dictionary2.First<KeyValuePair<TIControlPoint, float>>((KeyValuePair<TIControlPoint, float> x) => x.Value <= targetValue).Key;
				Dictionary<TIControlPoint, float> dictionary3 = dictionary2;
				TIControlPoint ticontrolPoint2 = key;
				dictionary3[ticontrolPoint2] += 1f;
			}
			using (List<TIControlPoint>.Enumerator enumerator = list.ToList<TIControlPoint>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIControlPoint ticontrolPoint3 = enumerator.Current;
					text = ticontrolPoint3.ResolveDefendControlPointEffect((int)(dictionary2[ticontrolPoint3] - dictionary[ticontrolPoint3]));
				}
				return text;
			}
		}
		if (target.isHabState)
		{
			text = target.ref_hab.ResolveDefendHabEffect(mission.councilor.faction, 60);
		}
		return text;
	}
}
