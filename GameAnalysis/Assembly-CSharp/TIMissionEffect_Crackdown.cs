using System;
using System.Threading;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

// Token: 0x020001D1 RID: 465
public class TIMissionEffect_Crackdown : TIMissionEffect
{
	// Token: 0x06000688 RID: 1672 RVA: 0x0001DD47 File Offset: 0x0001BF47
	public TIMissionEffect_Crackdown()
	{
		if (TIUtilities.IsMainThread(Thread.CurrentThread) && Application.isPlaying)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0001DD74 File Offset: 0x0001BF74
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (target.isControlPointState)
		{
			TIControlPoint ref_controlPoint = target.ref_controlPoint;
			if (base.MissionSuccess(outcome))
			{
				DateTime now = this.gameTime.Now;
				TIDateTime crackdownExpiration = ref_controlPoint.crackdownExpiration;
				if (now >= ((crackdownExpiration != null) ? new DateTime?(crackdownExpiration.ExportTime()) : null))
				{
					ref_controlPoint.EnableBenefits();
				}
				TIDateTime tidateTime = ref_controlPoint.ResolveCrackdownEffect((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1, mission.councilor.faction, false, false, mission.missionTemplate.hate[(int)outcome]);
				return ((tidateTime != null) ? tidateTime.ToCustomDateString() : null) ?? string.Empty;
			}
			if (outcome == TIMissionOutcome.CriticalFailure)
			{
				ref_controlPoint.nation.PropagandaOnPop(mission.councilor.faction.ideology, (float)Mathf.Min(-1, mission.councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) - 11), false);
			}
		}
		return string.Empty;
	}

	// Token: 0x0400061C RID: 1564
	private readonly GameTimeManager gameTime;
}
