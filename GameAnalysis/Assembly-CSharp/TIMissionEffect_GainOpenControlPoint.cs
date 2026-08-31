using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001D0 RID: 464
public class TIMissionEffect_GainOpenControlPoint : TIMissionEffect
{
	// Token: 0x06000686 RID: 1670 RVA: 0x0001DBE8 File Offset: 0x0001BDE8
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		float num = 0f;
		TIFactionState faction = mission.councilor.faction;
		TINationState ref_nation = target.ref_nation;
		if (ref_nation == null)
		{
			return string.Empty;
		}
		if (base.MissionSuccess(outcome))
		{
			TIControlPoint ticontrolPoint = ref_nation.FirstNativeControlPoint();
			if (ticontrolPoint != null)
			{
				if (ticontrolPoint != ref_nation.executiveControlPoint || ref_nation.numControlPoints == 1 || ref_nation.CountFactionControlPoints(ref_nation.numberTwoControlPoint.faction, true, false, true) >= 2)
				{
					int num2 = ref_nation.numControlPoints - ref_nation.StartOfTurnNativeControlPoints;
					if (ref_nation.GetControlPoint(num2).owned)
					{
						for (int i = ref_nation.FirstNativeControlPoint().positionInNation; i > num2; i--)
						{
							ref_nation.ChangeControlPointOwner(i, ControlPointChangeCause.Politics, ref_nation.GetControlPoint(i - 1).faction);
						}
					}
					ref_nation.ChangeControlPointOwner(num2, ControlPointChangeCause.Politics, faction);
				}
				else
				{
					ref_nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Politics, faction);
				}
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					num = Mathf.Max(0f, ref_nation.PropagandaOnPop(faction.ideology, TemplateManager.global.basePropagandaStrength, false));
				}
			}
		}
		else if (outcome == TIMissionOutcome.CriticalFailure)
		{
			num = Mathf.Min(-0.01f, ref_nation.PropagandaOnPop(faction.ideology, (float)(mission.councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) - 11), false));
		}
		return Mathf.Abs(num).ToPercent("P0");
	}
}
