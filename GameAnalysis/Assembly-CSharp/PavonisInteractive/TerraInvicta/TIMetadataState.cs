using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Modding;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000786 RID: 1926
	public class TIMetadataState : TIGameState
	{
		// Token: 0x06003C54 RID: 15444 RVA: 0x0016E528 File Offset: 0x0016C728
		public void SetValues()
		{
			this.selectedFactionsForScenario.Clear();
			TIFactionState activePlayer = GameControl.control.activePlayer;
			this.playerFactionName = activePlayer.displayNameCapitalized;
			this.gameTimeString = World.Active.GetExistingManager<GameTimeManager>().currentTime.ToString();
			this.difficulty = Loc.T(new StringBuilder("UI.Options.Difficulty").Append(GameStateManager.GlobalValues().difficulty.ToString()).ToString());
			this.requiredDLC = GameControl.control.scenarioTemplate.requiredDLC.ToList<string>();
			this.customDifficulty = GameStateManager.GlobalValues().scenarioCustomizations.customDifficulty;
			this.playedWithMods = GameStateManager.GlobalValues().moddingUsedAnytime;
			this.researchSpeedMultiplier = GameStateManager.GlobalValues().scenarioCustomizations.researchSpeedMultiplier.ToPercent("P0");
			this.controlPointMaintenanceFreebieBonus = GameStateManager.GlobalValues().scenarioCustomizations.controlPointMaintenanceFreebieBonus.ToString();
			this.controlPointMaintenanceFreebieBonusAI = GameStateManager.GlobalValues().scenarioCustomizations.controlPointMaintenanceFreebieBonusAI.ToString();
			this.missionControlBonus = GameStateManager.GlobalValues().scenarioCustomizations.missionControlBonus.ToString();
			this.missionControlBonusAI = GameStateManager.GlobalValues().scenarioCustomizations.missionControlBonusAI.ToString();
			this.alienProgressionSpeed = GameStateManager.GlobalValues().scenarioCustomizations.alienProgressionSpeed.ToPercent("P0");
			this.miningProductivityMultiplier = GameStateManager.GlobalValues().scenarioCustomizations.miningProductivityMultiplier.ToPercent("P0");
			this.nationalIPMultiplier = GameStateManager.GlobalValues().scenarioCustomizations.nationalIPMultiplier.ToPercent("P0");
			this.averageMonthlyEvents = GameStateManager.GlobalValues().scenarioCustomizations.averageMonthlyEvents.ToString();
			this.playerFactionIconPath = activePlayer.template.councilIcon256;
			this.playerFactionGradientPath = activePlayer.template.gradientPath;
			List<TIObjectiveTemplate> objectivesByTypeAndStatus = activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Completed);
			if (objectivesByTypeAndStatus.Count > 0)
			{
				this.lastCompletedObjectiveArtPath = objectivesByTypeAndStatus.Last<TIObjectiveTemplate>().completedIllustrationResource;
				this.lastCompletedObjectiveName = objectivesByTypeAndStatus.Last<TIObjectiveTemplate>().displayName(activePlayer);
			}
			else
			{
				List<TIObjectiveTemplate> objectivesByTypeAndStatus2 = activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked);
				this.lastCompletedObjectiveArtPath = ((objectivesByTypeAndStatus2.Count > 0) ? objectivesByTypeAndStatus2.Last<TIObjectiveTemplate>().assignedIllustrationResource : "");
				if (string.IsNullOrEmpty(this.lastCompletedObjectiveArtPath))
				{
					this.lastCompletedObjectiveArtPath = this.GetFirstFallbackObjectiveIllustration();
				}
				this.lastCompletedObjectiveName = ((objectivesByTypeAndStatus2.Count > 0) ? objectivesByTypeAndStatus2.Last<TIObjectiveTemplate>().displayName(activePlayer) : "");
			}
			if (string.IsNullOrEmpty(this.lastCompletedObjectiveArtPath))
			{
				this.lastCompletedObjectiveArtPath = activePlayer.template.gradientPath;
			}
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				this.selectedFactionsForScenario.Add(tifactionState.templateName);
			}
		}

		// Token: 0x06003C55 RID: 15445 RVA: 0x0016E7E9 File Offset: 0x0016C9E9
		public string GetFirstFallbackObjectiveIllustration()
		{
			return TemplateManager.global.illus_alienCrashdown;
		}

		// Token: 0x06003C56 RID: 15446 RVA: 0x0016E7F5 File Offset: 0x0016C9F5
		public override void PostInitializationInit_4()
		{
			this.SetValues();
		}

		// Token: 0x06003C57 RID: 15447 RVA: 0x0016E800 File Offset: 0x0016CA00
		public static TIMetadataState LoadMetaData(string filePath, out bool valid, bool allowLongSearch = false)
		{
			valid = true;
			if (filePath.Contains(".gz"))
			{
				string text = filePath.Replace(".gz", ".json");
				string text2 = filePath.Replace(".json", ".gz");
				if (TIPlayerProfileManager.compressSaves && File.Exists(text2))
				{
					try
					{
						using (FileStream fileStream = File.Open(text2, FileMode.Open))
						{
							using (FileStream fileStream2 = File.Create(text))
							{
								using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
								{
									gzipStream.CopyTo(fileStream2);
									filePath = text;
								}
							}
						}
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
						valid = false;
					}
				}
			}
			if (!valid)
			{
				return null;
			}
			TIMetadataState.FindMetadataFromSave(filePath, false);
			if (TIMetadataState.dataStrings.Count == 0 && allowLongSearch)
			{
				TIMetadataState.FindMetadataFromSave(filePath, true);
			}
			if (TIMetadataState.dataStrings.Count == 0)
			{
				if (TIPlayerProfileManager.compressSaves)
				{
					File.Delete(filePath);
				}
				return null;
			}
			TIMetadataState timetadataState = new TIMetadataState();
			timetadataState.playerFactionName = TIMetadataState.GetValue("playerFactionName");
			timetadataState.gameTimeString = TIMetadataState.GetValue("gameTimeString");
			timetadataState.difficulty = TIMetadataState.GetValue("difficulty");
			timetadataState.requiredDLC = TIMetadataState.GetListFromKey("requiredDLC");
			timetadataState.customDifficulty = TIMetadataState.GetBoolValueFromKey("customDifficulty", false);
			timetadataState.playedWithMods = TIMetadataState.GetBoolValueFromKey("playedWithMods", false);
			timetadataState.playerFactionIconPath = TIMetadataState.GetValue("playerFactionIconPath");
			timetadataState.playerFactionGradientPath = TIMetadataState.GetValue("playerFactionGradientPath");
			timetadataState.researchSpeedMultiplier = TIMetadataState.GetValue("researchSpeedMultiplier");
			timetadataState.controlPointMaintenanceFreebieBonus = TIMetadataState.GetValue("controlPointMaintenanceFreebieBonus");
			timetadataState.controlPointMaintenanceFreebieBonusAI = TIMetadataState.GetValue("controlPointMaintenanceFreebieBonusAI");
			timetadataState.missionControlBonus = TIMetadataState.GetValue("missionControlBonus");
			timetadataState.missionControlBonusAI = TIMetadataState.GetValue("missionControlBonusAI");
			timetadataState.alienProgressionSpeed = TIMetadataState.GetValue("alienProgressionSpeed");
			timetadataState.miningProductivityMultiplier = TIMetadataState.GetValue("miningProductivityMultiplier");
			timetadataState.nationalIPMultiplier = TIMetadataState.GetValue("nationalIPMultiplier");
			timetadataState.averageMonthlyEvents = TIMetadataState.GetValue("averageMonthlyEvents");
			timetadataState.selectedFactionsForScenario = TIMetadataState.GetListFromKey("selectedFactionsForScenario");
			timetadataState.lastCompletedObjectiveArtPath = TIMetadataState.GetValue("lastCompletedObjectiveArtPath");
			timetadataState.lastCompletedObjectiveName = TIMetadataState.GetValue("lastCompletedObjectiveName");
			if (TIPlayerProfileManager.compressSaves)
			{
				File.Delete(filePath);
			}
			if (!timetadataState.requiredDLC.All<string>((string x) => ModManager.dlcNames.Contains(x)))
			{
				valid = false;
			}
			return timetadataState;
		}

		// Token: 0x06003C58 RID: 15448 RVA: 0x0016EAA4 File Offset: 0x0016CCA4
		private static void FindMetadataFromSave(string filePath, bool longSearch)
		{
			List<string> list = new List<string>();
			if (longSearch)
			{
				list = File.ReadLines(filePath).ToList<string>();
			}
			else
			{
				list = File.ReadLines(filePath).Take<string>(200).ToList<string>();
			}
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			TIMetadataState.dataStrings.Clear();
			foreach (string text in list)
			{
				if (text.Contains("TIMetadataState") && text.Contains("["))
				{
					flag = true;
				}
				if (text.Contains("["))
				{
					num++;
				}
				if (flag && text.Contains("]"))
				{
					num--;
					if (num == 0)
					{
						flag2 = true;
					}
				}
				if (flag)
				{
					string text2 = text.TrimStart(Array.Empty<char>()).Replace("\"", "").TrimEnd(new char[] { ',' });
					TIMetadataState.dataStrings.Add(text2);
				}
				if (flag2)
				{
					break;
				}
			}
		}

		// Token: 0x06003C59 RID: 15449 RVA: 0x0016EBBC File Offset: 0x0016CDBC
		private static bool GetBoolValueFromKey(string key, bool defaultValue)
		{
			bool flag;
			if (bool.TryParse(TIMetadataState.GetValue(key), out flag))
			{
				return flag;
			}
			return defaultValue;
		}

		// Token: 0x06003C5A RID: 15450 RVA: 0x0016EBDC File Offset: 0x0016CDDC
		private static List<string> GetListFromKey(string searchKey)
		{
			List<string> list = new List<string>();
			int num = 0;
			bool flag = false;
			foreach (string text in TIMetadataState.dataStrings)
			{
				if (text.Contains("]"))
				{
					break;
				}
				if (flag)
				{
					list.Add(text);
				}
				if (text.Split(new char[] { ':' })[0] == searchKey)
				{
					flag = true;
				}
				num++;
			}
			return list;
		}

		// Token: 0x06003C5B RID: 15451 RVA: 0x0016EC70 File Offset: 0x0016CE70
		private static string GetValue(string searchKey)
		{
			foreach (string text in TIMetadataState.dataStrings)
			{
				if (text.Split(new char[] { ':' })[0] == searchKey)
				{
					return text.Substring(text.IndexOf(':') + 1).TrimStart(new char[] { ' ' });
				}
			}
			return string.Empty;
		}

		// Token: 0x04002660 RID: 9824
		public string playerFactionName;

		// Token: 0x04002661 RID: 9825
		public string gameTimeString;

		// Token: 0x04002662 RID: 9826
		public string difficulty;

		// Token: 0x04002663 RID: 9827
		public List<string> requiredDLC = new List<string>();

		// Token: 0x04002664 RID: 9828
		public bool playedWithMods;

		// Token: 0x04002665 RID: 9829
		public bool customDifficulty;

		// Token: 0x04002666 RID: 9830
		public List<string> selectedFactionsForScenario = new List<string>();

		// Token: 0x04002667 RID: 9831
		public string researchSpeedMultiplier;

		// Token: 0x04002668 RID: 9832
		public string controlPointMaintenanceFreebieBonus;

		// Token: 0x04002669 RID: 9833
		public string controlPointMaintenanceFreebieBonusAI;

		// Token: 0x0400266A RID: 9834
		public string missionControlBonus;

		// Token: 0x0400266B RID: 9835
		public string missionControlBonusAI;

		// Token: 0x0400266C RID: 9836
		public string alienProgressionSpeed;

		// Token: 0x0400266D RID: 9837
		public string miningProductivityMultiplier;

		// Token: 0x0400266E RID: 9838
		public string nationalIPMultiplier;

		// Token: 0x0400266F RID: 9839
		public string averageMonthlyEvents;

		// Token: 0x04002670 RID: 9840
		public string playerFactionIconPath;

		// Token: 0x04002671 RID: 9841
		public string playerFactionGradientPath;

		// Token: 0x04002672 RID: 9842
		public string lastCompletedObjectiveArtPath;

		// Token: 0x04002673 RID: 9843
		public string lastCompletedObjectiveName;

		// Token: 0x04002674 RID: 9844
		private static List<string> dataStrings = new List<string>();
	}
}
