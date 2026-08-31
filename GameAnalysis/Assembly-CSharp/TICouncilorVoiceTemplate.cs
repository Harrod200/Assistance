using System;
using System.Text;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;

// Token: 0x02000182 RID: 386
public class TICouncilorVoiceTemplate : TIDataTemplate
{
	// Token: 0x170000B9 RID: 185
	// (get) Token: 0x060005A8 RID: 1448 RVA: 0x0001A290 File Offset: 0x00018490
	public CouncilorGender eGender
	{
		get
		{
			string text = this.gender;
			if (text != null)
			{
				if (text == "M")
				{
					return CouncilorGender.Male;
				}
				if (text == "F")
				{
					return CouncilorGender.Female;
				}
			}
			return CouncilorGender.Nonbinary;
		}
	}

	// Token: 0x170000BA RID: 186
	// (get) Token: 0x060005A9 RID: 1449 RVA: 0x0001A2C8 File Offset: 0x000184C8
	public new string displayName
	{
		get
		{
			return Loc.T("UI.Councilor.VoiceCat", new object[]
			{
				Loc.T(new StringBuilder("TICouncilorVoiceTemplate.").Append(this.language).ToString()),
				Loc.T(new StringBuilder("TICouncilorVoiceTemplate.").Append(this.accent).ToString())
			});
		}
	}

	// Token: 0x170000BB RID: 187
	// (get) Token: 0x060005AA RID: 1450 RVA: 0x0001A32C File Offset: 0x0001852C
	public string displayIdx
	{
		get
		{
			return Loc.T("UI.Councilor.VoiceIdx", new object[]
			{
				Loc.T(new StringBuilder("UI.Councilor.").Append(this.gender).ToString()),
				(this.index + 1).ToString().ToString()
			});
		}
	}

	// Token: 0x170000BC RID: 188
	// (get) Token: 0x060005AB RID: 1451 RVA: 0x0001A383 File Offset: 0x00018583
	public string category
	{
		get
		{
			return new StringBuilder(this.language).Append("_").Append(this.accent).ToString();
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x060005AC RID: 1452 RVA: 0x0001A3AA File Offset: 0x000185AA
	public string categoryGender
	{
		get
		{
			return new StringBuilder(this.language).Append("_").Append(this.accent).Append("_")
				.Append(this.gender)
				.ToString();
		}
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x0001A3E8 File Offset: 0x000185E8
	public void PlayMissionVoice(TIMissionTemplate missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation voiceMissionSituation, bool onEarth, bool queueVoice = true)
	{
		if (this.councilorEventInstance.isValid() && (this.councilorEventInstance.IsPlaying() || missionTemplate == null))
		{
			return;
		}
		string text = string.Concat(new string[]
		{
			"event:/VO/",
			base.dataName.Replace('_', '/'),
			"/",
			missionTemplate.dataName,
			"/",
			voiceMissionSituation.ToString()
		});
		if (!AudioManager.VerifyPath(text, false))
		{
			if (voiceMissionSituation == TICouncilorVoiceTemplate.VoiceMissionSituation.Success)
			{
				text = string.Concat(new string[]
				{
					"event:/VO/",
					base.dataName.Replace('_', '/'),
					"/",
					missionTemplate.dataName,
					"/",
					TICouncilorVoiceTemplate.VoiceMissionSituation.Assigned.ToString()
				});
			}
			if (voiceMissionSituation == TICouncilorVoiceTemplate.VoiceMissionSituation.Aborted)
			{
				text = "event:/VO/" + base.dataName.Replace('_', '/') + "/Generic/" + TICouncilorVoiceTemplate.VoiceMissionSituation.MissionAbort.ToString();
			}
			if (voiceMissionSituation == TICouncilorVoiceTemplate.VoiceMissionSituation.Assigned)
			{
				text = "event:/VO/" + base.dataName.Replace('_', '/') + "/Selection/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionAssigned.ToString();
			}
			if (!AudioManager.VerifyPath(text, true))
			{
				return;
			}
		}
		if (!queueVoice)
		{
			this.councilorEventInstance.Stop(STOP_MODE.IMMEDIATE);
		}
		AudioManager.CreateFMODObjects(text, out this.councilorEventDescription, out this.councilorEventInstance);
		if (queueVoice)
		{
			VOController.Instance.AddVOToQueue(this.councilorEventInstance, onEarth);
			return;
		}
		if (this.councilorEventInstance.isValid())
		{
			this.councilorEventInstance.Play();
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x0001A582 File Offset: 0x00018782
	public void PlayMissionVoice(TIMissionTemplate missionTemplate, TIMissionOutcome voiceMissionOutcome, bool onEarth)
	{
		if (voiceMissionOutcome == TIMissionOutcome.Failure || voiceMissionOutcome == TIMissionOutcome.CriticalFailure)
		{
			this.PlayMissionVoice(missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation.Failure, onEarth, true);
			return;
		}
		if (voiceMissionOutcome == TIMissionOutcome.Success || voiceMissionOutcome == TIMissionOutcome.CriticalSuccess)
		{
			this.PlayMissionVoice(missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation.Success, onEarth, true);
			return;
		}
		this.PlayMissionVoice(missionTemplate, TICouncilorVoiceTemplate.VoiceMissionSituation.Aborted, onEarth, true);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x0001A5B4 File Offset: 0x000187B4
	public void PlaySelectionVoice(TICouncilorState councilorState, bool onEarth)
	{
		bool flag = councilorState.turned && councilorState.faction != GameControl.control.activePlayer && councilorState.agentForFaction == GameControl.control.activePlayer;
		bool flag2 = true;
		if (this.councilorEventInstance.isValid() && (this.councilorEventInstance.IsPlaying() || councilorState == null))
		{
			return;
		}
		string text = "event:/VO/" + base.dataName.Replace('_', '/') + "/Selection";
		if (flag)
		{
			text += "Turncoat";
		}
		if (TIMissionPhaseState.InMissionPhase())
		{
			if (flag)
			{
				text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionGeneric.ToString();
			}
			else
			{
				text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionAssignmentPhase.ToString();
			}
		}
		else if (councilorState.HasMission)
		{
			text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionAssigned.ToString();
		}
		else if (councilorState.completedMission != null)
		{
			if (flag)
			{
				text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionGeneric.ToString();
			}
			else
			{
				text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionCompleted.ToString();
			}
		}
		else
		{
			text = text + "/" + TICouncilorVoiceTemplate.VoiceCouncilorSituation.MissionGeneric.ToString();
			flag2 = false;
		}
		if (!AudioManager.VerifyPath(text, flag2))
		{
			return;
		}
		AudioManager.CreateFMODObjects(text, out this.councilorEventDescription, out this.councilorEventInstance);
		VOController.Instance.AddVOToQueue(this.councilorEventInstance, onEarth);
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x0001A754 File Offset: 0x00018954
	public bool ValidForCharacter(TICouncilorState councilorState, CouncilorGender gender, string language, string accent)
	{
		return this.enable && !councilorState.isAlien && (gender == CouncilorGender.Nonbinary || gender == this.eGender) && !(language != this.language) && !(accent != this.accent);
	}

	// Token: 0x0400059F RID: 1439
	public bool enable;

	// Token: 0x040005A0 RID: 1440
	public bool specific_person;

	// Token: 0x040005A1 RID: 1441
	public string language;

	// Token: 0x040005A2 RID: 1442
	public string accent;

	// Token: 0x040005A3 RID: 1443
	public int index;

	// Token: 0x040005A4 RID: 1444
	public string gender;

	// Token: 0x040005A5 RID: 1445
	private EventInstance councilorEventInstance;

	// Token: 0x040005A6 RID: 1446
	private EventDescription councilorEventDescription;

	// Token: 0x02000B06 RID: 2822
	public enum VoiceMissionSituation
	{
		// Token: 0x04004965 RID: 18789
		Assigned,
		// Token: 0x04004966 RID: 18790
		Success,
		// Token: 0x04004967 RID: 18791
		Failure,
		// Token: 0x04004968 RID: 18792
		Aborted,
		// Token: 0x04004969 RID: 18793
		MissionAbort,
		// Token: 0x0400496A RID: 18794
		MissionFailure
	}

	// Token: 0x02000B07 RID: 2823
	public enum VoiceCouncilorSituation
	{
		// Token: 0x0400496C RID: 18796
		MissionAssignmentPhase,
		// Token: 0x0400496D RID: 18797
		MissionAssigned,
		// Token: 0x0400496E RID: 18798
		MissionCompleted,
		// Token: 0x0400496F RID: 18799
		MissionGeneric
	}
}
