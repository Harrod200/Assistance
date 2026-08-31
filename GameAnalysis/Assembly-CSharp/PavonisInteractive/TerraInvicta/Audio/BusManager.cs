using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009D5 RID: 2517
	public static class BusManager
	{
		// Token: 0x17001039 RID: 4153
		// (get) Token: 0x06005E7C RID: 24188 RVA: 0x002CDD7B File Offset: 0x002CBF7B
		// (set) Token: 0x06005E7D RID: 24189 RVA: 0x002CDD82 File Offset: 0x002CBF82
		public static Bus Master { get; private set; }

		// Token: 0x1700103A RID: 4154
		// (get) Token: 0x06005E7E RID: 24190 RVA: 0x002CDD8A File Offset: 0x002CBF8A
		// (set) Token: 0x06005E7F RID: 24191 RVA: 0x002CDD91 File Offset: 0x002CBF91
		public static Bus SFX { get; private set; }

		// Token: 0x1700103B RID: 4155
		// (get) Token: 0x06005E80 RID: 24192 RVA: 0x002CDD99 File Offset: 0x002CBF99
		// (set) Token: 0x06005E81 RID: 24193 RVA: 0x002CDDA0 File Offset: 0x002CBFA0
		public static Bus UI { get; private set; }

		// Token: 0x1700103C RID: 4156
		// (get) Token: 0x06005E82 RID: 24194 RVA: 0x002CDDA8 File Offset: 0x002CBFA8
		// (set) Token: 0x06005E83 RID: 24195 RVA: 0x002CDDAF File Offset: 0x002CBFAF
		public static Bus UI_Special_UI_Reverb { get; private set; }

		// Token: 0x1700103D RID: 4157
		// (get) Token: 0x06005E84 RID: 24196 RVA: 0x002CDDB7 File Offset: 0x002CBFB7
		// (set) Token: 0x06005E85 RID: 24197 RVA: 0x002CDDBE File Offset: 0x002CBFBE
		public static Bus Voice { get; private set; }

		// Token: 0x1700103E RID: 4158
		// (get) Token: 0x06005E86 RID: 24198 RVA: 0x002CDDC6 File Offset: 0x002CBFC6
		// (set) Token: 0x06005E87 RID: 24199 RVA: 0x002CDDCD File Offset: 0x002CBFCD
		public static Bus Ambient { get; private set; }

		// Token: 0x1700103F RID: 4159
		// (get) Token: 0x06005E88 RID: 24200 RVA: 0x002CDDD5 File Offset: 0x002CBFD5
		// (set) Token: 0x06005E89 RID: 24201 RVA: 0x002CDDDC File Offset: 0x002CBFDC
		public static Bus Music { get; private set; }

		// Token: 0x06005E8B RID: 24203 RVA: 0x002CDDE6 File Offset: 0x002CBFE6
		public static void Initialize()
		{
			BusManager.GetBuses();
			BusManager.SetInitialVolume();
		}

		// Token: 0x06005E8C RID: 24204 RVA: 0x002CDDF4 File Offset: 0x002CBFF4
		private static void GetBuses()
		{
			BusManager.Master = RuntimeManager.GetBus("bus:/Master");
			BusManager.SFX = RuntimeManager.GetBus("bus:/Master/SFX");
			BusManager.UI = RuntimeManager.GetBus("bus:/Master/UI");
			BusManager.Voice = RuntimeManager.GetBus("bus:/Master/Voice Processing");
			BusManager.Ambient = RuntimeManager.GetBus("bus:/Master/Ambient");
			BusManager.Music = RuntimeManager.GetBus("bus:/Master/Music");
			BusManager.UI_Special_UI_Reverb = RuntimeManager.GetBus("bus:/Master/UI_SPECIAL_UI_REVERB");
		}

		// Token: 0x06005E8D RID: 24205 RVA: 0x002CDE6C File Offset: 0x002CC06C
		private static void SetInitialVolume()
		{
			BusManager.SetVolume(BusManager.Master, TIPlayerProfileManager.GetFloatByKey("VolumeMaster", 50f) / 100f);
			BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.GetFloatByKey("VolumeEffects", 50f) / 100f);
			BusManager.SetVolume(BusManager.UI, TIPlayerProfileManager.GetFloatByKey("VolumeUI", 50f) / 100f);
			BusManager.SetVolume(BusManager.Voice, TIPlayerProfileManager.GetFloatByKey("VolumeVoice", 50f) / 100f);
			BusManager.SetVolume(BusManager.Ambient, TIPlayerProfileManager.GetFloatByKey("VolumeAmbience", 50f) / 100f);
			BusManager.SetVolume(BusManager.Music, TIPlayerProfileManager.GetFloatByKey("VolumeMusic", 50f) / 100f);
			BusManager.SetVolume(BusManager.UI_Special_UI_Reverb, TIPlayerProfileManager.GetFloatByKey("VolumeUI", 50f) / 100f);
		}

		// Token: 0x06005E8E RID: 24206 RVA: 0x002CDF52 File Offset: 0x002CC152
		public static void SetVolume(Bus bus, float volume)
		{
			bus.setVolume(Mathf.Clamp01(volume));
		}

		// Token: 0x06005E8F RID: 24207 RVA: 0x002CDF64 File Offset: 0x002CC164
		public static float GetVolume(Bus bus)
		{
			float num;
			bus.getVolume(out num);
			return num;
		}

		// Token: 0x06005E90 RID: 24208 RVA: 0x002CDF7C File Offset: 0x002CC17C
		public static void StopAllEvents(Bus bus, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.IMMEDIATE)
		{
			bus.stopAllEvents(stopMode);
		}
	}
}
