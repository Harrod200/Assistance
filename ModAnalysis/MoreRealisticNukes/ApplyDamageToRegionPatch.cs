using System;
using System.Linq;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace MoreRealisticNukes
{
	// Token: 0x0200000B RID: 11
	[HarmonyPatch(typeof(TIRegionState), "ApplyDamageToRegion")]
	internal static class ApplyDamageToRegionPatch
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002DEC File Offset: 0x00000FEC
		public static void Prefix(TIRegionState __instance, TIFactionState applyingFaction, TINationState applyingNation, bool nuclear, out NuclearAtrocityContext __state)
		{
			__state = ApplyDamageToRegionPatch.activeContext;
			if (Main.enabled && nuclear && !(__instance == null) && !(applyingFaction == null))
			{
				TINationState nation = __instance.nation;
				bool flag = ApplyDamageToRegionPatch.IsAlienBattlefield(__instance, nation);
				ApplyDamageToRegionPatch.activeContext = new NuclearAtrocityContext
				{
					Region = __instance,
					ApplyingFaction = applyingFaction,
					ApplyingNation = applyingNation,
					TargetNation = nation,
					InitialPopulationMillions = __instance.populationInMillions,
					AlienBattlefield = flag,
					HumanDefensiveWar = (!flag && ApplyDamageToRegionPatch.IsHumanDefensiveWar(applyingNation, nation))
				};
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002E88 File Offset: 0x00001088
		public static void Finalizer(NuclearAtrocityContext __state)
		{
			ApplyDamageToRegionPatch.activeContext = __state;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002E94 File Offset: 0x00001094
		private static bool IsAlienBattlefield(TIRegionState region, TINationState targetNation)
		{
			bool flag;
			if (targetNation != null && targetNation.alienNation)
			{
				flag = true;
			}
			else
			{
				try
				{
					TINationState tinationState = ((region != null) ? region.GetOccupierNation : null);
					if (tinationState != null && tinationState.alienNation)
					{
						return true;
					}
				}
				catch
				{
				}
				try
				{
					if (region == null || region.armies == null)
					{
						return false;
					}
					foreach (TIArmyState tiarmyState in region.armies)
					{
						if (!(tiarmyState == null) && !tiarmyState.atSea)
						{
							if (tiarmyState.AlienRegularArmy || tiarmyState.AlienMegafaunaArmy)
							{
								return true;
							}
							TIFactionState faction = tiarmyState.faction;
							if (faction != null && faction.IsAlienFaction)
							{
								return true;
							}
						}
					}
				}
				catch
				{
					return false;
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003048 File Offset: 0x00001248
		private static bool IsHumanDefensiveWar(TINationState applyingNation, TINationState targetNation)
		{
			bool flag;
			if (applyingNation == null || targetNation == null || targetNation.alienNation)
			{
				flag = false;
			}
			else
			{
				try
				{
					bool flag2;
					if (applyingNation.defensiveWarStates != null)
					{
						flag2 = applyingNation.defensiveWarStates.Any<TIWarState>((TIWarState war) => war != null && war.attackingAlliance != null && war.attackingAlliance.Contains(targetNation));
					}
					else
					{
						flag2 = false;
					}
					flag = flag2;
				}
				catch
				{
					flag = false;
				}
			}
			return flag;
		}

		// Token: 0x0400001A RID: 26
		[ThreadStatic]
		internal static NuclearAtrocityContext activeContext;
	}
}
