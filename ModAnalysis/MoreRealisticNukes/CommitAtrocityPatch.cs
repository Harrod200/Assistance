using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace MoreRealisticNukes
{
	// Token: 0x0200000C RID: 12
	[HarmonyPatch(typeof(TIFactionState), "CommitAtrocity")]
	internal static class CommitAtrocityPatch
	{
		// Token: 0x06000022 RID: 34 RVA: 0x000030E0 File Offset: 0x000012E0
		public static void Prefix(TIFactionState __instance, ref int numAtrocities, TIFactionState.AtrocityCause cause, bool propagandaHitWhenZero, float multiplier)
		{
			NuclearAtrocityContext activeContext = ApplyDamageToRegionPatch.activeContext;
			if (Main.enabled && activeContext != null && !(activeContext.Region == null) && !(__instance != activeContext.ApplyingFaction) && cause == 2)
			{
				float num = Math.Max(0f, activeContext.InitialPopulationMillions - activeContext.Region.populationInMillions);
				int num2 = numAtrocities;
				int num3 = Main.CalculateAtrocities(num, activeContext);
				numAtrocities = num3;
				if (Main.mod != null)
				{
					Main.mod.Logger.Log(string.Concat(new object[]
					{
						"Nuclear atrocity rebalance: ",
						Main.Classify(activeContext),
						", deathsM=",
						num.ToString("0.###"),
						", original=",
						num2,
						", new=",
						num3
					}));
				}
			}
		}
	}
}
