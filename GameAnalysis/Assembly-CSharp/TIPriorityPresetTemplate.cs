using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002C6 RID: 710
public class TIPriorityPresetTemplate : TIDataTemplate
{
	// Token: 0x06000A50 RID: 2640 RVA: 0x00031A88 File Offset: 0x0002FC88
	public Dictionary<PriorityType, int> GetAllSettings()
	{
		if (this._settings == null)
		{
			this.SetAllPresets();
		}
		return this._settings.ToDictionary<KeyValuePair<PriorityType, int>, PriorityType, int>((KeyValuePair<PriorityType, int> entry) => entry.Key, (KeyValuePair<PriorityType, int> entry) => entry.Value);
	}

	// Token: 0x06000A51 RID: 2641 RVA: 0x00031AEC File Offset: 0x0002FCEC
	public TIPriorityPresetTemplate(string dataNameToSet)
	{
		base.dataName = dataNameToSet;
	}

	// Token: 0x06000A52 RID: 2642 RVA: 0x00031AFB File Offset: 0x0002FCFB
	public void SetDisplayName(string displayName)
	{
		this._displayName = displayName;
	}

	// Token: 0x06000A53 RID: 2643 RVA: 0x00031B04 File Offset: 0x0002FD04
	public static void ResetPreset(TIPriorityPresetTemplate template)
	{
		template.economySetting = 0;
		template.welfareSetting = 0;
		template.environmentSetting = 0;
		template.governmentSetting = 0;
		template.knowledgeSetting = 0;
		template.oppressionSetting = 0;
		template.unitySetting = 0;
		template.militarySetting = 0;
		template.spoilsSetting = 0;
		template.boostSetting = 0;
		template.missionControlSetting = 0;
		template.spaceProgramSetting = 0;
		template.foundMilitarySetting = 0;
		template.armySetting = 0;
		template.navySetting = 0;
		template.initNuclearWeaponsSetting = 0;
		template.nuclearProgramSetting = 0;
		template.spaceDefenseSetting = 0;
		template.stoSetting = 0;
		template.initSpaceProgramSetting = 0;
		template.SetAllPresets();
	}

	// Token: 0x06000A54 RID: 2644 RVA: 0x00031BA4 File Offset: 0x0002FDA4
	public static void DuplicatePreset(TIPriorityPresetTemplate templateToCopy, ref TIPriorityPresetTemplate duplicateTemplate)
	{
		duplicateTemplate.economySetting = templateToCopy.economySetting;
		duplicateTemplate.welfareSetting = templateToCopy.welfareSetting;
		duplicateTemplate.environmentSetting = templateToCopy.environmentSetting;
		duplicateTemplate.governmentSetting = templateToCopy.governmentSetting;
		duplicateTemplate.knowledgeSetting = templateToCopy.knowledgeSetting;
		duplicateTemplate.oppressionSetting = templateToCopy.oppressionSetting;
		duplicateTemplate.unitySetting = templateToCopy.unitySetting;
		duplicateTemplate.militarySetting = templateToCopy.militarySetting;
		duplicateTemplate.spoilsSetting = templateToCopy.spoilsSetting;
		duplicateTemplate.boostSetting = templateToCopy.boostSetting;
		duplicateTemplate.missionControlSetting = templateToCopy.missionControlSetting;
		duplicateTemplate.spaceProgramSetting = templateToCopy.spaceProgramSetting;
		duplicateTemplate.foundMilitarySetting = templateToCopy.foundMilitarySetting;
		duplicateTemplate.armySetting = templateToCopy.armySetting;
		duplicateTemplate.navySetting = templateToCopy.navySetting;
		duplicateTemplate.initNuclearWeaponsSetting = templateToCopy.initNuclearWeaponsSetting;
		duplicateTemplate.nuclearProgramSetting = templateToCopy.nuclearProgramSetting;
		duplicateTemplate.spaceDefenseSetting = templateToCopy.spaceDefenseSetting;
		duplicateTemplate.stoSetting = templateToCopy.stoSetting;
		duplicateTemplate.initSpaceProgramSetting = templateToCopy.initSpaceProgramSetting;
		duplicateTemplate.SetAllPresets();
	}

	// Token: 0x06000A55 RID: 2645 RVA: 0x00031CBC File Offset: 0x0002FEBC
	public void SetPreset(PriorityType priority, int value)
	{
		value = Mathf.Clamp(value, 0, 3);
		switch (priority)
		{
		case PriorityType.Economy:
			this.economySetting = value;
			break;
		case PriorityType.Welfare:
			this.welfareSetting = value;
			break;
		case PriorityType.Environment:
			this.environmentSetting = value;
			break;
		case PriorityType.Knowledge:
			this.knowledgeSetting = value;
			break;
		case PriorityType.Government:
			this.governmentSetting = value;
			break;
		case PriorityType.Unity:
			this.unitySetting = value;
			break;
		case PriorityType.Oppression:
			this.oppressionSetting = value;
			break;
		case PriorityType.Funding:
			this.spaceProgramSetting = value;
			break;
		case PriorityType.Spoils:
			this.spoilsSetting = value;
			break;
		case PriorityType.Civilian_InitiateSpaceflightProgram:
			this.initSpaceProgramSetting = value;
			break;
		case PriorityType.LaunchFacilities:
			this.boostSetting = value;
			break;
		case PriorityType.MissionControl:
			this.missionControlSetting = value;
			break;
		case PriorityType.Military_FoundMilitary:
			this.foundMilitarySetting = value;
			break;
		case PriorityType.Military:
			this.militarySetting = value;
			break;
		case PriorityType.Military_BuildArmy:
			this.armySetting = value;
			break;
		case PriorityType.Military_BuildNavy:
			this.navySetting = value;
			break;
		case PriorityType.Military_InitiateNuclearProgram:
			this.initNuclearWeaponsSetting = value;
			break;
		case PriorityType.Military_BuildNuclearWeapons:
			this.nuclearProgramSetting = value;
			break;
		case PriorityType.Military_BuildSpaceDefenses:
			this.spaceDefenseSetting = value;
			break;
		case PriorityType.Military_BuildSTOSquadron:
			this.stoSetting = value;
			break;
		}
		this.SetAllPresets();
	}

	// Token: 0x06000A56 RID: 2646 RVA: 0x00031DF8 File Offset: 0x0002FFF8
	public void SetAllPresets()
	{
		Dictionary<PriorityType, int> dictionary = new Dictionary<PriorityType, int>();
		dictionary[PriorityType.Economy] = this.economySetting;
		dictionary[PriorityType.Welfare] = this.welfareSetting;
		dictionary[PriorityType.Environment] = this.environmentSetting;
		dictionary[PriorityType.Knowledge] = this.knowledgeSetting;
		dictionary[PriorityType.Government] = this.governmentSetting;
		dictionary[PriorityType.Military] = this.militarySetting;
		dictionary[PriorityType.Oppression] = this.oppressionSetting;
		dictionary[PriorityType.Spoils] = this.spoilsSetting;
		dictionary[PriorityType.Unity] = this.unitySetting;
		dictionary[PriorityType.LaunchFacilities] = this.boostSetting;
		dictionary[PriorityType.MissionControl] = this.missionControlSetting;
		dictionary[PriorityType.Funding] = this.spaceProgramSetting;
		dictionary[PriorityType.Military_FoundMilitary] = this.foundMilitarySetting;
		dictionary[PriorityType.Military_BuildArmy] = this.armySetting;
		dictionary[PriorityType.Military_BuildNavy] = this.navySetting;
		dictionary[PriorityType.Military_InitiateNuclearProgram] = this.initNuclearWeaponsSetting;
		dictionary[PriorityType.Military_BuildSTOSquadron] = this.stoSetting;
		dictionary[PriorityType.Military_BuildSpaceDefenses] = this.spaceDefenseSetting;
		dictionary[PriorityType.Military_BuildNuclearWeapons] = this.nuclearProgramSetting;
		dictionary[PriorityType.Civilian_InitiateSpaceflightProgram] = this.initSpaceProgramSetting;
		this._settings = dictionary;
	}

	// Token: 0x06000A57 RID: 2647 RVA: 0x00031F1F File Offset: 0x0003011F
	public int GetPreset(PriorityType priority)
	{
		if (this._settings == null)
		{
			this.SetAllPresets();
		}
		return this._settings[priority];
	}

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x06000A58 RID: 2648 RVA: 0x00031F3B File Offset: 0x0003013B
	public TIFactionState assignToFaction
	{
		get
		{
			return GameStateManager.FindByTemplate<TIFactionState>(this.factionName, false);
		}
	}

	// Token: 0x06000A59 RID: 2649 RVA: 0x00031F4C File Offset: 0x0003014C
	public bool ValidPreset_Global()
	{
		if (!string.IsNullOrEmpty(this.displayName) && this.displayName.Length > 0)
		{
			return this._settings.Values.Any<int>((int x) => x > 0 && x <= 3);
		}
		return false;
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x06000A5A RID: 2650 RVA: 0x00031FA5 File Offset: 0x000301A5
	public int TotalWeights
	{
		get
		{
			return Math.Max(1, this._settings.Sum<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value));
		}
	}

	// Token: 0x06000A5B RID: 2651 RVA: 0x00031FD8 File Offset: 0x000301D8
	protected bool ValidPresetForNation(TINationState nation)
	{
		if (this.deleted)
		{
			return false;
		}
		if (this._settings == null)
		{
			this.SetAllPresets();
		}
		foreach (PriorityType priorityType in this._settings.Keys)
		{
			if (this.GetPreset(priorityType) > 0 && nation.ValidPriority(priorityType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0003205C File Offset: 0x0003025C
	public bool ValidPresetForFaction(TIFactionState faction)
	{
		if (faction == null && !this.nationalAIOption)
		{
			return false;
		}
		TIFactionState assignToFaction = this.assignToFaction;
		return (!(faction != null) || !(assignToFaction != null) || !(assignToFaction != faction)) && !this.deleted;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x000320AB File Offset: 0x000302AB
	public bool ValidPreset(TINationState nation, TIFactionState faction = null)
	{
		return this.ValidPresetForFaction(faction) && this.ValidPresetForNation(nation);
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000320C0 File Offset: 0x000302C0
	public bool CheckArgumentsForOnlyThreeValues(int testValue, int ignoreValue1, int ignoreValue2, params object[] args)
	{
		foreach (object obj in args)
		{
			if ((int)obj != testValue && (int)obj != ignoreValue1 && (int)obj != ignoreValue2)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00032100 File Offset: 0x00030300
	public bool MatchesPreset(Dictionary<PriorityType, int> presetData, List<PriorityType> skipPriorities)
	{
		bool flag = presetData.Where<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => !skipPriorities.Contains(x.Key)).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 1);
		bool flag2 = presetData.Where<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => !skipPriorities.Contains(x.Key)).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 2);
		bool flag3 = presetData.Where<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => !skipPriorities.Contains(x.Key)).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 3);
		Dictionary<PriorityType, int> dictionary = presetData.ToDictionary<KeyValuePair<PriorityType, int>, PriorityType, int>((KeyValuePair<PriorityType, int> x) => x.Key, (KeyValuePair<PriorityType, int> x) => x.Value);
		if ((from x in this.GetAllSettings()
			where !skipPriorities.Contains(x.Key)
			select x).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 1) && (flag2 || flag3))
		{
			using (List<PriorityType>.Enumerator enumerator = dictionary.Keys.ToList<PriorityType>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PriorityType priorityType = enumerator.Current;
					if (dictionary[priorityType] > 0)
					{
						dictionary[priorityType] = 1;
					}
				}
				goto IL_02AB;
			}
		}
		if ((from x in this.GetAllSettings()
			where !skipPriorities.Contains(x.Key)
			select x).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 2) && (flag || flag3))
		{
			using (List<PriorityType>.Enumerator enumerator = dictionary.Keys.ToList<PriorityType>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PriorityType priorityType2 = enumerator.Current;
					if (dictionary[priorityType2] > 0)
					{
						dictionary[priorityType2] = 2;
					}
				}
				goto IL_02AB;
			}
		}
		if ((from x in this.GetAllSettings()
			where !skipPriorities.Contains(x.Key)
			select x).All<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value == 0 || x.Value == 3) && (flag || flag2))
		{
			foreach (PriorityType priorityType3 in dictionary.Keys.ToList<PriorityType>())
			{
				if (dictionary[priorityType3] > 0)
				{
					dictionary[priorityType3] = 3;
				}
			}
		}
		IL_02AB:
		foreach (PriorityType priorityType4 in Enums.PriorityTypes)
		{
			if (!skipPriorities.Contains(priorityType4) && this.GetPreset(priorityType4) != dictionary[priorityType4])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400085A RID: 2138
	public bool nationalAIOption;

	// Token: 0x0400085B RID: 2139
	public string factionName;

	// Token: 0x0400085C RID: 2140
	public int economySetting;

	// Token: 0x0400085D RID: 2141
	public int welfareSetting;

	// Token: 0x0400085E RID: 2142
	public int environmentSetting;

	// Token: 0x0400085F RID: 2143
	public int knowledgeSetting;

	// Token: 0x04000860 RID: 2144
	public int governmentSetting;

	// Token: 0x04000861 RID: 2145
	public int unitySetting;

	// Token: 0x04000862 RID: 2146
	public int oppressionSetting;

	// Token: 0x04000863 RID: 2147
	public int spaceProgramSetting;

	// Token: 0x04000864 RID: 2148
	public int spoilsSetting;

	// Token: 0x04000865 RID: 2149
	public int initSpaceProgramSetting;

	// Token: 0x04000866 RID: 2150
	public int boostSetting;

	// Token: 0x04000867 RID: 2151
	public int missionControlSetting;

	// Token: 0x04000868 RID: 2152
	public int foundMilitarySetting;

	// Token: 0x04000869 RID: 2153
	public int militarySetting;

	// Token: 0x0400086A RID: 2154
	public int armySetting;

	// Token: 0x0400086B RID: 2155
	public int navySetting;

	// Token: 0x0400086C RID: 2156
	public int initNuclearWeaponsSetting;

	// Token: 0x0400086D RID: 2157
	public int nuclearProgramSetting;

	// Token: 0x0400086E RID: 2158
	public int spaceDefenseSetting;

	// Token: 0x0400086F RID: 2159
	public int stoSetting;

	// Token: 0x04000870 RID: 2160
	public bool customDesign;

	// Token: 0x04000871 RID: 2161
	public bool deleted;

	// Token: 0x04000872 RID: 2162
	private Dictionary<PriorityType, int> _settings;
}
