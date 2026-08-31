using System;
using UnityModManagerNet;

namespace MoreRealisticNukes
{
	// Token: 0x02000003 RID: 3
	public class Settings : UnityModManager.ModSettings
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00002540 File Offset: 0x00000740
		public override void Save(UnityModManager.ModEntry modEntry)
		{
			UnityModManager.ModSettings.Save<Settings>(this, modEntry);
		}

		// Token: 0x04000004 RID: 4
		public float AlienMultiplier = 0.5f;

		// Token: 0x04000005 RID: 5
		public float AlienMin = 0f;

		// Token: 0x04000006 RID: 6
		public float AlienMax = 3f;

		// Token: 0x04000007 RID: 7
		public float DefensiveMultiplier = 2f;

		// Token: 0x04000008 RID: 8
		public float DefensiveMin = 1f;

		// Token: 0x04000009 RID: 9
		public float DefensiveMax = 10f;

		// Token: 0x0400000A RID: 10
		public float OffensiveMultiplier = 10f;

		// Token: 0x0400000B RID: 11
		public float OffensiveMin = 4f;

		// Token: 0x0400000C RID: 12
		public float OffensiveMax = 20f;

		// Token: 0x0400000D RID: 13
		public float InitiateNuclearProgramIP = 600f;

		// Token: 0x0400000E RID: 14
		public float BuildNuclearWeaponsIP = 200f;

		// Token: 0x0400000F RID: 15
		public bool EnableDisarmNukesMission = true;

		// Token: 0x04000010 RID: 16
		public float DisarmMissionUtilityScore = 0f;
	}
}
