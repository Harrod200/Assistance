using System;
using System.Globalization;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace MoreRealisticNukes
{
	// Token: 0x02000002 RID: 2
	public static class Main
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static bool Load(UnityModManager.ModEntry modEntry)
		{
			Main.mod = modEntry;
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);
			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);
			bool flag;
			try
			{
				Harmony harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				modEntry.Logger.Log("Nuclear atrocity rebalance patches applied.");
				flag = true;
			}
			catch (Exception ex)
			{
				modEntry.Logger.Error("Failed to apply patches: " + ex);
				flag = false;
			}
			return flag;
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002108 File Offset: 0x00000308
		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.enabled = value;
			return true;
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002124 File Offset: 0x00000324
		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.Label("Nuclear priority costs:", new GUILayoutOption[0]);
			Main.settings.InitiateNuclearProgramIP = Main.DrawNamedFloat("Initiate nuclear program IP", Main.settings.InitiateNuclearProgramIP, 230f);
			Main.settings.BuildNuclearWeaponsIP = Main.DrawNamedFloat("Build nuclear weapons IP", Main.settings.BuildNuclearWeaponsIP, 230f);
			GUILayout.Space(8f);
			GUILayout.Label("Nuclear strike atrocity formula:", new GUILayoutOption[0]);
			GUILayout.Label("Alien battlefield means alien nation, alien occupation, alien regular armies, or megafauna.", new GUILayoutOption[0]);
			GUILayout.Label("Only nuclear-strike MassCasualtiesfromRegionDamage atrocities are changed.", new GUILayoutOption[0]);
			GUILayout.Space(8f);
			Main.DrawCurve("Alien battlefield", ref Main.settings.AlienMultiplier, ref Main.settings.AlienMin, ref Main.settings.AlienMax);
			Main.DrawCurve("Human defensive war", ref Main.settings.DefensiveMultiplier, ref Main.settings.DefensiveMin, ref Main.settings.DefensiveMax);
			Main.DrawCurve("Human offensive/default", ref Main.settings.OffensiveMultiplier, ref Main.settings.OffensiveMin, ref Main.settings.OffensiveMax);
			GUILayout.Space(8f);
			GUILayout.Label("Councilor mission:", new GUILayoutOption[0]);
			Main.settings.EnableDisarmNukesMission = GUILayout.Toggle(Main.settings.EnableDisarmNukesMission, "Enable Disarm Nukes mission for councilors with Sabotage Facilities (restart required)", new GUILayoutOption[0]);
			Main.settings.DisarmMissionUtilityScore = Main.DrawNamedFloat("Disarm Nukes AI utility score", Main.settings.DisarmMissionUtilityScore, 230f);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022B0 File Offset: 0x000004B0
		private static float DrawNamedFloat(string label, float value, float labelWidth)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(labelWidth) });
			float num = Main.DrawFloat(value, 90f);
			GUILayout.EndHorizontal();
			return num;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000022FC File Offset: 0x000004FC
		private static void DrawCurve(string label, ref float multiplier, ref float min, ref float max)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			GUILayout.Label(label, new GUILayoutOption[] { GUILayout.Width(190f) });
			GUILayout.Label("mult", new GUILayoutOption[] { GUILayout.Width(35f) });
			multiplier = Main.DrawFloat(multiplier, 70f);
			GUILayout.Label("min", new GUILayoutOption[] { GUILayout.Width(28f) });
			min = Main.DrawFloat(min, 60f);
			GUILayout.Label("max", new GUILayoutOption[] { GUILayout.Width(32f) });
			max = Main.DrawFloat(max, 60f);
			GUILayout.EndHorizontal();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000023C4 File Offset: 0x000005C4
		private static float DrawFloat(float value, float width)
		{
			string text = GUILayout.TextField(value.ToString("G6", CultureInfo.InvariantCulture), new GUILayoutOption[] { GUILayout.Width(width) });
			float num;
			float num2;
			if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
			{
				num2 = num;
			}
			else
			{
				num2 = value;
			}
			return num2;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000241F File Offset: 0x0000061F
		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002430 File Offset: 0x00000630
		internal static int CalculateAtrocities(float deathsMillions, NuclearAtrocityContext context)
		{
			int num;
			if (deathsMillions <= 0f)
			{
				num = 0;
			}
			else
			{
				Settings settings = Main.settings ?? new Settings();
				float num2;
				float num3;
				float num4;
				if (context.AlienBattlefield)
				{
					num2 = deathsMillions * settings.AlienMultiplier;
					num3 = settings.AlienMin;
					num4 = settings.AlienMax;
				}
				else if (context.HumanDefensiveWar)
				{
					num2 = deathsMillions * settings.DefensiveMultiplier;
					num3 = settings.DefensiveMin;
					num4 = settings.DefensiveMax;
				}
				else
				{
					num2 = deathsMillions * settings.OffensiveMultiplier;
					num3 = settings.OffensiveMin;
					num4 = settings.OffensiveMax;
				}
				if (num4 < num3)
				{
					float num5 = num3;
					num3 = num4;
					num4 = num5;
				}
				num = (int)Math.Max(num3, Math.Min(num4, num2));
			}
			return num;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000024FC File Offset: 0x000006FC
		internal static string Classify(NuclearAtrocityContext context)
		{
			string text;
			if (context.AlienBattlefield)
			{
				text = "alien battlefield";
			}
			else
			{
				text = (context.HumanDefensiveWar ? "human defensive war" : "human offensive war");
			}
			return text;
		}

		// Token: 0x04000001 RID: 1
		internal static bool enabled = true;

		// Token: 0x04000002 RID: 2
		internal static UnityModManager.ModEntry mod;

		// Token: 0x04000003 RID: 3
		internal static Settings settings;
	}
}
