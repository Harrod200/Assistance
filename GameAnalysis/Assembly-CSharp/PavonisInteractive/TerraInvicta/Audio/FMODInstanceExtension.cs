using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009D6 RID: 2518
	public static class FMODInstanceExtension
	{
		// Token: 0x06005E91 RID: 24209 RVA: 0x002CDF87 File Offset: 0x002CC187
		public static bool Play(this EventInstance eventInstance)
		{
			eventInstance.start();
			return true;
		}

		// Token: 0x06005E92 RID: 24210 RVA: 0x002CDF92 File Offset: 0x002CC192
		public static bool Play(this EventInstance eventInstance, GameObject targetObject)
		{
			eventInstance.set3DAttributes(targetObject.To3DAttributes());
			eventInstance.start();
			return true;
		}

		// Token: 0x06005E93 RID: 24211 RVA: 0x002CDFAC File Offset: 0x002CC1AC
		public static bool IsPlaying(this EventInstance eventInstance)
		{
			PLAYBACK_STATE playback_STATE;
			eventInstance.getPlaybackState(out playback_STATE);
			return playback_STATE == PLAYBACK_STATE.PLAYING;
		}

		// Token: 0x06005E94 RID: 24212 RVA: 0x002CDFCC File Offset: 0x002CC1CC
		public static bool IsStopped(this EventInstance eventInstance)
		{
			PLAYBACK_STATE playback_STATE;
			eventInstance.getPlaybackState(out playback_STATE);
			return playback_STATE == PLAYBACK_STATE.STOPPED;
		}

		// Token: 0x06005E95 RID: 24213 RVA: 0x002CDFEA File Offset: 0x002CC1EA
		public static bool Stop(this EventInstance eventInstance, global::FMOD.Studio.STOP_MODE stopMode = global::FMOD.Studio.STOP_MODE.IMMEDIATE)
		{
			eventInstance.stop(stopMode);
			return true;
		}

		// Token: 0x06005E96 RID: 24214 RVA: 0x002CDFF6 File Offset: 0x002CC1F6
		public static bool Release(this EventInstance eventInstance)
		{
			return eventInstance.isValid() && eventInstance.release() == RESULT.OK;
		}

		// Token: 0x06005E97 RID: 24215 RVA: 0x002CE010 File Offset: 0x002CC210
		public static float GetVolume(this EventInstance eventInstance)
		{
			float num;
			eventInstance.getVolume(out num);
			return num;
		}

		// Token: 0x06005E98 RID: 24216 RVA: 0x002CE028 File Offset: 0x002CC228
		public static bool SetVolume(this EventInstance eventInstance, float volume)
		{
			if (volume >= 0f && volume <= 1f)
			{
				eventInstance.setVolume(volume);
				return true;
			}
			return false;
		}

		// Token: 0x06005E99 RID: 24217 RVA: 0x002CE046 File Offset: 0x002CC246
		public static bool ChangeVolume(this EventInstance eventInstance, float volumeDelta)
		{
			if (eventInstance.GetVolume() + volumeDelta >= 0f)
			{
				eventInstance.SetVolume(eventInstance.GetVolume() + volumeDelta);
				return true;
			}
			eventInstance.SetVolume(0f);
			return false;
		}

		// Token: 0x06005E9A RID: 24218 RVA: 0x002CE078 File Offset: 0x002CC278
		public static int GetLength(this EventInstance eventInstance)
		{
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			int num;
			eventDescription.getLength(out num);
			return num;
		}

		// Token: 0x06005E9B RID: 24219 RVA: 0x002CE09C File Offset: 0x002CC29C
		public static int GetTime(this EventInstance eventInstance)
		{
			int num;
			eventInstance.getTimelinePosition(out num);
			return num;
		}

		// Token: 0x06005E9C RID: 24220 RVA: 0x002CE0B4 File Offset: 0x002CC2B4
		public static bool SetTime(this EventInstance eventInstance, int time)
		{
			if (time <= eventInstance.GetLength())
			{
				eventInstance.setTimelinePosition(time);
				return true;
			}
			return false;
		}

		// Token: 0x06005E9D RID: 24221 RVA: 0x002CE0CB File Offset: 0x002CC2CB
		public static bool SetProperty(this EventInstance eventInstance, EVENT_PROPERTY property, float value)
		{
			if (eventInstance.isValid())
			{
				eventInstance.setProperty(property, value);
				return true;
			}
			return false;
		}

		// Token: 0x06005E9E RID: 24222 RVA: 0x002CE0E4 File Offset: 0x002CC2E4
		public static float GetProperty(this EventInstance eventInstance, EVENT_PROPERTY property)
		{
			float num = 0f;
			if (eventInstance.isValid())
			{
				eventInstance.getProperty(property, out num);
			}
			return num;
		}

		// Token: 0x06005E9F RID: 24223 RVA: 0x002CE10C File Offset: 0x002CC30C
		public static float GetParameter(this EventInstance eventInstance, string parameterName)
		{
			float num = 0f;
			if (eventInstance.isValid())
			{
				eventInstance.getParameterByName(parameterName, out num);
			}
			return num;
		}

		// Token: 0x06005EA0 RID: 24224 RVA: 0x002CE134 File Offset: 0x002CC334
		public static bool SetParameter(this EventInstance eventInstance, PARAMETER_TYPE parameter, float value)
		{
			if (eventInstance.isValid())
			{
				eventInstance.SetParameter(parameter, value);
				return true;
			}
			return false;
		}

		// Token: 0x06005EA1 RID: 24225 RVA: 0x002CE14B File Offset: 0x002CC34B
		public static bool SetParameter(this EventInstance eventInstance, string paramterName, float value)
		{
			if (eventInstance.isValid())
			{
				eventInstance.setParameterByName(paramterName, value, false);
				return true;
			}
			return false;
		}

		// Token: 0x06005EA2 RID: 24226 RVA: 0x002CE164 File Offset: 0x002CC364
		public static PLAYBACK_STATE GetPlaybackState(this EventInstance eventInstance)
		{
			if (eventInstance.isValid())
			{
				PLAYBACK_STATE playback_STATE;
				eventInstance.getPlaybackState(out playback_STATE);
				return playback_STATE;
			}
			return PLAYBACK_STATE.STOPPED;
		}

		// Token: 0x06005EA3 RID: 24227 RVA: 0x002CE187 File Offset: 0x002CC387
		public static bool SetDistance(this EventInstance eventInstance, float maximumDistance, float minimumDistance = 1f)
		{
			if (eventInstance.isValid() && maximumDistance > minimumDistance)
			{
				eventInstance.SetProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maximumDistance);
				eventInstance.SetProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minimumDistance);
				return true;
			}
			return false;
		}
	}
}
