using System;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001DD RID: 477
public class TIMissionEffect_DamageSpaceFacilities : TIMissionEffect
{
	// Token: 0x060006A6 RID: 1702 RVA: 0x0001F640 File Offset: 0x0001D840
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (target.isRegionSpaceFacility)
		{
			TICouncilorState councilor = mission.councilor;
			TIRegionSpaceFacilityState ref_regionSpaceFacility = target.ref_regionSpaceFacility;
			TIRegionState region = ref_regionSpaceFacility.region;
			switch (ref_regionSpaceFacility.spaceFacilityType)
			{
			case SpaceFacilityType.launchFacility:
				switch (outcome)
				{
				case TIMissionOutcome.CriticalFailure:
				{
					TIFactionState tifactionState = region.nation.WeightedRandomFactionByControlPoints();
					if (tifactionState == mission.councilor.faction)
					{
						return string.Empty;
					}
					if (tifactionState == null)
					{
						float num = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f, false);
						return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[] { num.ToPercent("P0") });
					}
					councilor.DetainCouncilor(tifactionState, 2f, 1f, true);
					return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[] { tifactionState.displayName });
				}
				case TIMissionOutcome.Success:
				{
					float num2 = Mathf.Clamp(10f - (region.boostPerYear_dekatons - 1f) * 2f, 0.05f, 1f);
					float num3 = Mathf.Max(1f, region.boostPerYear_dekatons * num2);
					int num4 = region.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num3, false, true);
					if (num4 == 0 && region.numSTOFighters > 0 && TIUtilities.RandomFloatValue() < 0.25f)
					{
						region.DestroyRandomSTOFighter();
						num4++;
					}
					TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, TIUtilities.FormatBigOrSmallNumber(region.boostPerYear_dekatons, 1, 7, 0, false, false), mission.missionTemplate.hate[(int)outcome], num4);
					return num3.ToString();
				}
				case TIMissionOutcome.CriticalSuccess:
				{
					float num5 = Mathf.Clamp(10f - (region.boostPerYear_dekatons - 3f) * 1f, 0.1f, 1f);
					float num6 = Mathf.Max(2f, region.boostPerYear_dekatons * num5);
					int num7 = region.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num6, false, true);
					if (region.numSTOFighters > 0)
					{
						region.DestroyRandomSTOFighter();
						num7++;
					}
					TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, TIUtilities.FormatBigOrSmallNumber(region.boostPerYear_dekatons, 1, 7, 0, false, false), mission.missionTemplate.hate[(int)outcome], num7);
					return num6.ToString();
				}
				}
				break;
			case SpaceFacilityType.missionControlFacility:
				switch (outcome)
				{
				case TIMissionOutcome.CriticalFailure:
				{
					TIFactionState tifactionState2 = region.nation.WeightedRandomFactionByControlPoints();
					if (tifactionState2 == mission.councilor.faction)
					{
						return string.Empty;
					}
					if (tifactionState2 == null)
					{
						float num8 = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f, false);
						return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[] { num8.ToPercent("P0") });
					}
					councilor.DetainCouncilor(tifactionState2, 2f, 1f, true);
					return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[] { tifactionState2.displayName });
				}
				case TIMissionOutcome.Success:
				{
					int num9 = 1;
					region.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)num9), false, true);
					TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, region.missionControl.ToString("N0"), mission.missionTemplate.hate[(int)outcome], 0);
					return num9.ToString();
				}
				case TIMissionOutcome.CriticalSuccess:
				{
					int num10 = 2;
					region.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)num10), false, true);
					TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, region.missionControl.ToString("N0"), mission.missionTemplate.hate[(int)outcome], 0);
					return num10.ToString();
				}
				}
				break;
			case SpaceFacilityType.spaceDefenseFacility:
				if (outcome != TIMissionOutcome.CriticalFailure)
				{
					if (outcome - TIMissionOutcome.Success <= 1)
					{
						region.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, true);
						TINotificationQueueState.LogSpaceFacilityBombed(ref_regionSpaceFacility, mission.councilor.faction, string.Empty, mission.missionTemplate.hate[(int)outcome], 0);
					}
				}
				else
				{
					TIFactionState tifactionState3 = region.nation.WeightedRandomFactionByControlPoints();
					if (tifactionState3 == mission.councilor.faction)
					{
						return string.Empty;
					}
					if (tifactionState3 == null)
					{
						float num11 = region.nation.PropagandaOnPop(councilor.faction.ideology, -5f, false);
						return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString(), new object[] { num11.ToPercent("P0") });
					}
					councilor.DetainCouncilor(tifactionState3, 2f, 1f, true);
					return Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[] { tifactionState3.displayName });
				}
				break;
			}
		}
		return string.Empty;
	}
}
