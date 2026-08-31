using System;
using System.Collections;
using System.Text;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009D4 RID: 2516
	public static class AudioManager
	{
		// Token: 0x06005E66 RID: 24166 RVA: 0x002CD93C File Offset: 0x002CBB3C
		public static void Initialize()
		{
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x002CD93E File Offset: 0x002CBB3E
		public static void CreateFMODObjects(string eventPath, out EventDescription eventDescription, out EventInstance eventInstance)
		{
			eventDescription = RuntimeManager.GetEventDescription(eventPath);
			eventDescription.createInstance(out eventInstance);
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x002CD954 File Offset: 0x002CBB54
		public static EventInstance CreateFMODInstance(string eventPath)
		{
			return RuntimeManager.CreateInstance(eventPath);
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x002CD95C File Offset: 0x002CBB5C
		public static EventInstance CreateFMODInstance(string eventPath, GameObject target)
		{
			EventInstance eventInstance = RuntimeManager.CreateInstance(eventPath);
			eventInstance.set3DAttributes(target.To3DAttributes());
			return eventInstance;
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x002CD97F File Offset: 0x002CBB7F
		public static EventDescription CreateFMODDescription(string eventPath)
		{
			return RuntimeManager.GetEventDescription(eventPath);
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x002CD987 File Offset: 0x002CBB87
		public static void PlayEvent(EventInstance instance)
		{
			if (instance.isValid())
			{
				instance.Play();
			}
		}

		// Token: 0x06005E6C RID: 24172 RVA: 0x002CD999 File Offset: 0x002CBB99
		public static void StopEvent(EventInstance instance, global::FMOD.Studio.STOP_MODE stopMode = global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
		{
			if (instance.isValid())
			{
				instance.Stop(stopMode);
			}
		}

		// Token: 0x06005E6D RID: 24173 RVA: 0x002CD9AC File Offset: 0x002CBBAC
		public static void StopAllEvents()
		{
			BusManager.StopAllEvents(BusManager.Ambient, global::FMOD.Studio.STOP_MODE.IMMEDIATE);
			BusManager.StopAllEvents(BusManager.SFX, global::FMOD.Studio.STOP_MODE.IMMEDIATE);
			BusManager.StopAllEvents(BusManager.Voice, global::FMOD.Studio.STOP_MODE.IMMEDIATE);
		}

		// Token: 0x06005E6E RID: 24174 RVA: 0x002CD9D0 File Offset: 0x002CBBD0
		public static void PlayTutorialVO(string eventPath)
		{
			if (!string.IsNullOrEmpty(eventPath))
			{
				eventPath = eventPath.Replace(".Name", "");
				eventPath = new StringBuilder("event:/VO/ENG/Faction/Tutorial_").Append(eventPath).ToString();
				if (AudioManager.VerifyPath(eventPath, false))
				{
					AudioManager.StopTutorialVO();
					AudioManager.tutorialVOEvent = AudioManager.CreateFMODInstance(eventPath);
					if (AudioManager.tutorialVOEvent.isValid() && !AudioManager.tutorialVOEvent.IsPlaying())
					{
						AudioManager.tutorialVOEvent.Play();
					}
				}
			}
		}

		// Token: 0x06005E6F RID: 24175 RVA: 0x002CDA4A File Offset: 0x002CBC4A
		public static void StopTutorialVO()
		{
			if (AudioManager.tutorialVOEvent.isValid() && AudioManager.tutorialVOEvent.IsPlaying())
			{
				AudioManager.tutorialVOEvent.Stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x06005E70 RID: 24176 RVA: 0x002CDA70 File Offset: 0x002CBC70
		public static void PlayCinematicAudio(string eventPath)
		{
			if (!string.IsNullOrEmpty(eventPath))
			{
				eventPath = eventPath.Replace(".Name", "");
				eventPath = new StringBuilder("event:/VO/ENG/Faction/Cinematics/").Append(eventPath).ToString();
				if (AudioManager.VerifyPath(eventPath, true))
				{
					AudioManager.StopCinematicAudio();
					AudioManager.cinematicAudioEvent = AudioManager.CreateFMODInstance(eventPath);
					if (AudioManager.cinematicAudioEvent.isValid() && !AudioManager.cinematicAudioEvent.IsPlaying())
					{
						AudioManager.cinematicAudioEvent.Play();
					}
				}
			}
		}

		// Token: 0x06005E71 RID: 24177 RVA: 0x002CDAEA File Offset: 0x002CBCEA
		public static void StopCinematicAudio()
		{
			if (AudioManager.cinematicAudioEvent.isValid() && AudioManager.cinematicAudioEvent.IsPlaying())
			{
				AudioManager.cinematicAudioEvent.Stop(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x06005E72 RID: 24178 RVA: 0x002CDB10 File Offset: 0x002CBD10
		public static void PlayOneShot(string eventPath, bool useUIDelay = false, bool useGlobalUIDelay = false)
		{
			if (AudioManager.VerifyPath(eventPath, true))
			{
				if (!eventPath.Contains("UI_SFX"))
				{
					if (AudioManager.lastEventPlayed == eventPath && Time.time - AudioManager.lastTimePlayedUISpecial <= AudioManager.uiSpecialrepetitionDelay)
					{
						return;
					}
					AudioManager.lastEventPlayed = eventPath;
					AudioManager.lastTimePlayedUISpecial = Time.time;
				}
				else if (useUIDelay)
				{
					if (Time.time - AudioManager.lastTimePlayedUI <= AudioManager.uiSFXRepetitionDelay)
					{
						return;
					}
					AudioManager.lastTimePlayedUI = Time.time;
				}
				else if (useGlobalUIDelay && Time.time - AudioManager.lastTimePlayedUIGlobal <= AudioManager.uiSFXRepetitionDelayGlobal)
				{
					return;
				}
				AudioManager.lastTimePlayedUIGlobal = Time.time;
				RuntimeManager.PlayOneShot(eventPath, default(Vector3));
				return;
			}
			global::UnityEngine.Debug.LogWarning("FMOD Event " + eventPath + " Not Found!");
		}

		// Token: 0x06005E73 RID: 24179 RVA: 0x002CDBCF File Offset: 0x002CBDCF
		public static void PlayOneShot(string eventPath, Vector3 position)
		{
			if (AudioManager.VerifyPath(eventPath, true))
			{
				RuntimeManager.PlayOneShot(eventPath, position);
				return;
			}
			global::UnityEngine.Debug.LogError("FMOD Event " + eventPath + " Not Found!");
		}

		// Token: 0x06005E74 RID: 24180 RVA: 0x002CDBF7 File Offset: 0x002CBDF7
		public static void PlayOneShot(string eventPath, GameObject gameObject)
		{
			if (AudioManager.VerifyPath(eventPath, true))
			{
				RuntimeManager.PlayOneShotAttached(eventPath, gameObject);
				return;
			}
			global::UnityEngine.Debug.LogError("FMOD Event " + eventPath + " Not Found!");
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x002CDC20 File Offset: 0x002CBE20
		public static bool VerifyPath(string eventPath, bool logError = true)
		{
			EventDescription eventDescription;
			if (RuntimeManager.StudioSystem.getEvent(eventPath, out eventDescription) == RESULT.OK)
			{
				return true;
			}
			if (logError)
			{
				global::UnityEngine.Debug.Log("AudioManager failed to play sound event: " + eventPath);
			}
			return false;
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x002CDC58 File Offset: 0x002CBE58
		public static string GetScenarioAudioPostFix(string audioPath)
		{
			string text = TIUtilities.CombineStrings(new string[]
			{
				audioPath,
				GameControl.control.scenarioTemplate.scenarioLocalizationPostfix
			});
			if (AudioManager.VerifyPath(text, false))
			{
				return text;
			}
			return audioPath;
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x002CDC94 File Offset: 0x002CBE94
		public static bool SetIntensity(float intensity)
		{
			if (intensity >= 0f)
			{
				RESULT result = RuntimeManager.StudioSystem.setParameterByName("Intensity", intensity, false);
				if (result == RESULT.OK)
				{
					return true;
				}
				global::UnityEngine.Debug.LogError("Error setting intensity paramter: " + result.ToString());
			}
			return false;
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x002CDCE0 File Offset: 0x002CBEE0
		public static float GetIntensity()
		{
			float num;
			RuntimeManager.StudioSystem.getParameterByName("Intensity", out num);
			return num;
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x002CDD04 File Offset: 0x002CBF04
		public static float GetCombatAudioMaxDistance(EventInstance eventInstance)
		{
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			float num;
			eventDescription.getMaximumDistance(out num);
			num *= 1.5f * SpaceCombatManager.GetScalingAdjustmentFactor();
			return num;
		}

		// Token: 0x06005E7A RID: 24186 RVA: 0x002CDD34 File Offset: 0x002CBF34
		public static float GetCombatAudioMinDistance(EventInstance eventInstance)
		{
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			float num;
			eventDescription.getMinimumDistance(out num);
			num *= SpaceCombatManager.GetScalingAdjustmentFactor();
			return num;
		}

		// Token: 0x06005E7B RID: 24187 RVA: 0x002CDD5E File Offset: 0x002CBF5E
		public static IEnumerator FadeAudio(EventInstance eventInstance, float time, float targetVolume)
		{
			float originalTime = time;
			float originalVolume = eventInstance.GetVolume();
			while (time > 0f && originalVolume != targetVolume)
			{
				yield return null;
				if (originalVolume > targetVolume)
				{
					eventInstance.SetVolume(Mathf.Lerp(targetVolume, originalVolume, time / originalTime));
				}
				else
				{
					eventInstance.SetVolume(Mathf.Lerp(targetVolume, originalVolume, time / originalTime));
				}
				time -= Time.deltaTime;
			}
			eventInstance.SetVolume(targetVolume);
			yield break;
		}

		// Token: 0x04004378 RID: 17272
		private static string lastEventPlayed;

		// Token: 0x04004379 RID: 17273
		private static float lastTimePlayedUISpecial;

		// Token: 0x0400437A RID: 17274
		private static float lastTimePlayedUI;

		// Token: 0x0400437B RID: 17275
		private static float lastTimePlayedUIGlobal;

		// Token: 0x0400437C RID: 17276
		private static float uiSpecialrepetitionDelay = 2f;

		// Token: 0x0400437D RID: 17277
		private static float uiSFXRepetitionDelay = 0.75f;

		// Token: 0x0400437E RID: 17278
		private static float uiSFXRepetitionDelayGlobal = 0.15f;

		// Token: 0x0400437F RID: 17279
		private static EventInstance tutorialVOEvent;

		// Token: 0x04004380 RID: 17280
		private static EventInstance cinematicAudioEvent;
	}
}
