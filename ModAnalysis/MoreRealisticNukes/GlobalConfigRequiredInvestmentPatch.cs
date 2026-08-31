using System;
using HarmonyLib;

namespace MoreRealisticNukes
{
	// Token: 0x02000009 RID: 9
	[HarmonyPatch(typeof(TIGlobalConfig), "GetRequiredInvestmentPoints")]
	internal static class GlobalConfigRequiredInvestmentPatch
	{
		// Token: 0x0600001C RID: 28 RVA: 0x00002CFC File Offset: 0x00000EFC
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
