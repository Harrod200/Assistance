using System;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;

namespace MoreRealisticNukes
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(TINationState), "GetRequiredInvestmentPointsForPriority")]
	internal static class NationRequiredInvestmentPatch
	{
		// Token: 0x0600001D RID: 29 RVA: 0x00002D74 File Offset: 0x00000F74
		public static void Postfix(PriorityType priority, ref float __result)
		{
			if (Main.enabled && Main.settings != null)
			{
				if (priority == 16)
				{
					__result = Math.Max(0f, Main.settings.InitiateNuclearProgramIP);
				}
				else if (priority == 17)
				{
					__result = Math.Max(0f, Main.settings.BuildNuclearWeaponsIP);
				}
			}
		}
	}
}
