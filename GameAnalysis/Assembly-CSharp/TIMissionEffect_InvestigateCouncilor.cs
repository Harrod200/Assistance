using System;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E0 RID: 480
public class TIMissionEffect_InvestigateCouncilor : TIMissionEffect
{
	// Token: 0x060006B0 RID: 1712 RVA: 0x0001FCF0 File Offset: 0x0001DEF0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		float num = 0f;
		StringBuilder stringBuilder = new StringBuilder();
		TICouncilorState councilor = mission.councilor;
		TICouncilorState ref_councilor = target.ref_councilor;
		if (base.MissionSuccess(outcome))
		{
			if (councilor.faction.isActivePlayer && !ref_councilor.faction.isActivePlayer)
			{
				councilor.faction.UnlockAchievement("investigateCouncilor");
			}
			if (outcome != TIMissionOutcome.Success)
			{
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					num = 0.5f + (float)councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) / 100f;
				}
			}
			else
			{
				num = 0.25f + (float)councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false) / 200f;
			}
			float intel = councilor.faction.GetIntel(target);
			councilor.faction.GainIntel(target, num, councilor, false);
			float intel2 = councilor.faction.GetIntel(target);
			if (intel < TemplateManager.global.intelToSeeCouncilorBasicData && intel2 >= TemplateManager.global.intelToSeeCouncilorBasicData)
			{
				if (councilor.isAlien)
				{
					TINotificationQueueState.LogAlienCouncilorDetected(councilor.faction, ref_councilor);
				}
				else
				{
					TINotificationQueueState.LogEnemyCouncilorLocationDetected(councilor.faction, ref_councilor);
				}
			}
			if (!ref_councilor.isAlien)
			{
				CouncilorView viewofCouncilor = councilor.faction.GetViewofCouncilor(ref_councilor);
				if (councilor.faction != ref_councilor.faction)
				{
					if (councilor.faction.HasIntelOnCouncilorBasicData(ref_councilor))
					{
						stringBuilder.Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial1", new object[]
						{
							viewofCouncilor.displayNameCurrent,
							viewofCouncilor.factionStringCurrentKnowledge(false, false),
							viewofCouncilor.councilorJobStringMemory
						}));
					}
					if (councilor.faction.HasIntelOnCouncilorDetails(ref_councilor) && !councilor.faction.HasIntelOnCouncilorMission(ref_councilor))
					{
						stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial2", new object[] { viewofCouncilor.GetAttributeString(CouncilorAttribute.Loyalty) }));
					}
					if (councilor.faction.HasIntelOnCouncilorMission(ref_councilor))
					{
						stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial3"));
					}
					if (!ref_councilor.isAlien && councilor.faction.HasIntelOnCouncilorSecrets(ref_councilor))
					{
						if (viewofCouncilor.agentForFaction != null)
						{
							stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial5", new object[] { viewofCouncilor.agentForFaction.displayName }));
						}
						else
						{
							stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial4", new object[]
							{
								viewofCouncilor.factionStringCurrentKnowledge(false, false),
								viewofCouncilor.GetAttributeString(CouncilorAttribute.Loyalty)
							}));
						}
					}
				}
				else if (councilor.faction.HasIntelOnCouncilorSecrets(ref_councilor))
				{
					if (viewofCouncilor.agentForFaction != null)
					{
						stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.EnemySpecial5", new object[] { viewofCouncilor.agentForFaction.displayName }));
						councilor.faction.SetSuspicion(ref_councilor, 100f);
					}
					else
					{
						stringBuilder.Append(" ").Append(Loc.T("TIMissionEffect_InvestigateCouncilor.MySpecial1", new object[] { viewofCouncilor.GetAttributeString(CouncilorAttribute.Loyalty) }));
						councilor.faction.SetSuspicion(ref_councilor, 0f);
					}
				}
			}
		}
		else if (outcome == TIMissionOutcome.CriticalFailure && councilor.faction == ref_councilor.faction && TIUtilities.RandomRange(0, 10) < ref_councilor.GetAttribute(CouncilorAttribute.Loyalty, true, true, true, false, false, false))
		{
			ref_councilor.ModifyAttribute(CouncilorAttribute.Loyalty, -1);
			ref_councilor.ModifyAttribute(CouncilorAttribute.ApparentLoyalty, -1);
			stringBuilder.Append(Loc.T("TIMissionEffect_InvestigateCouncilor.MySpecial3", new object[] { ref_councilor.familyName }));
		}
		return stringBuilder.ToString();
	}
}
