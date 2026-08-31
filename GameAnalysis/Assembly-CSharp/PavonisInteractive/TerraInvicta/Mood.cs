using System;
using PavonisInteractive.TerraInvicta.Plugins;
using PavonisInteractive.TerraInvicta.Systems.Camera;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200090B RID: 2315
	public static class Mood
	{
		// Token: 0x06005889 RID: 22665 RVA: 0x002897C8 File Offset: 0x002879C8
		public static void SetState(Mood.State state)
		{
			if (iCueDllHooks.Instance == null)
			{
				return;
			}
			if (state == Mood.State.None)
			{
				return;
			}
			Mood.State state2 = Mood.State.None;
			if (state - Mood.State.TRIN_FactionBlue > 6)
			{
				if (state - Mood.State.TRIN_Earth <= 2)
				{
					state2 = Mood.visualizationState;
					Mood.visualizationState = state;
				}
			}
			else
			{
				Mood.ClearState(Mood.State.TRIN_Menu);
				state2 = Mood.playerState;
				Mood.playerState = state;
			}
			if (state2 != state)
			{
				if (state2 != Mood.State.None)
				{
					Mood.ClearState(state2);
				}
				iCueDllHooks.Instance.SetState(state.ToString());
			}
		}

		// Token: 0x0600588A RID: 22666 RVA: 0x00289835 File Offset: 0x00287A35
		public static void ClearState(Mood.State state)
		{
			if (iCueDllHooks.Instance == null)
			{
				return;
			}
			iCueDllHooks.Instance.ClearState(state.ToString());
		}

		// Token: 0x0600588B RID: 22667 RVA: 0x00289856 File Offset: 0x00287A56
		public static void TriggerEvent(Mood.Event event_)
		{
			if (iCueDllHooks.Instance == null)
			{
				return;
			}
			iCueDllHooks.Instance.TriggerEvent(event_.ToString());
		}

		// Token: 0x0600588C RID: 22668 RVA: 0x00289877 File Offset: 0x00287A77
		public static void BeginFactionEncounter(TIFactionState encounteredFaction)
		{
			Mood.encounterState = (Mood.State)encounteredFaction.template.encMood;
			Mood.SetState(Mood.encounterState);
		}

		// Token: 0x0600588D RID: 22669 RVA: 0x00289893 File Offset: 0x00287A93
		public static void EndFactionEncounter()
		{
			Mood.ClearState(Mood.encounterState);
		}

		// Token: 0x0600588E RID: 22670 RVA: 0x0028989F File Offset: 0x00287A9F
		public static void UpdateActivePlayerState()
		{
			if (GameControl.control.activePlayer == null)
			{
				return;
			}
			Mood.SetState((Mood.State)GameControl.control.activePlayer.template.playerMood);
		}

		// Token: 0x0600588F RID: 22671 RVA: 0x002898D0 File Offset: 0x00287AD0
		public static void UpdateVisualizationState(bool dontReplaceZoomState = false)
		{
			if (dontReplaceZoomState && Mood.visualizationState == Mood.State.SDKL_Zoom)
			{
				return;
			}
			if (CameraManager.Singleton == null || !(CameraManager.Singleton.SelectedState != null) || CameraManager.Singleton.LOD != CameraManagerLOD.Surface)
			{
				Mood.SetState(Mood.State.TRIN_Space);
				return;
			}
			TISpaceObjectState ref_spaceObject = CameraManager.Singleton.SelectedState.ref_spaceObject;
			if (ref_spaceObject != null && ref_spaceObject.isEarth)
			{
				Mood.SetState(Mood.State.TRIN_Earth);
				return;
			}
			Mood.SetState(Mood.State.TRIN_Space);
		}

		// Token: 0x06005890 RID: 22672 RVA: 0x00289944 File Offset: 0x00287B44
		public static void GoodNews()
		{
			Mood.TriggerEvent(Mood.Event.SDKL_PulseBarGreen);
		}

		// Token: 0x06005891 RID: 22673 RVA: 0x0028994C File Offset: 0x00287B4C
		public static void BadNews()
		{
			Mood.TriggerEvent(Mood.Event.SDKL_PulseBarRed);
		}

		// Token: 0x04004059 RID: 16473
		private static Mood.State encounterState;

		// Token: 0x0400405A RID: 16474
		private static Mood.State playerState;

		// Token: 0x0400405B RID: 16475
		private static Mood.State visualizationState;

		// Token: 0x020011F0 RID: 4592
		public enum State
		{
			// Token: 0x040068A2 RID: 26786
			None,
			// Token: 0x040068A3 RID: 26787
			TRIN_FactionBlue,
			// Token: 0x040068A4 RID: 26788
			TRIN_FactionRed,
			// Token: 0x040068A5 RID: 26789
			TRIN_FactionOrange,
			// Token: 0x040068A6 RID: 26790
			TRIN_FactionPurple,
			// Token: 0x040068A7 RID: 26791
			TRIN_FactionCyan,
			// Token: 0x040068A8 RID: 26792
			TRIN_FactionGreen,
			// Token: 0x040068A9 RID: 26793
			TRIN_FactionYellow,
			// Token: 0x040068AA RID: 26794
			TRIN_EncounterBlue,
			// Token: 0x040068AB RID: 26795
			TRIN_EncounterRed,
			// Token: 0x040068AC RID: 26796
			TRIN_EncounterOrange,
			// Token: 0x040068AD RID: 26797
			TRIN_EncounterPurple,
			// Token: 0x040068AE RID: 26798
			TRIN_EncounterCyan,
			// Token: 0x040068AF RID: 26799
			TRIN_EncounterGreen,
			// Token: 0x040068B0 RID: 26800
			TRIN_EncounterYellow,
			// Token: 0x040068B1 RID: 26801
			TRIN_Menu,
			// Token: 0x040068B2 RID: 26802
			TRIN_EncounterAliens,
			// Token: 0x040068B3 RID: 26803
			TRIN_Earth,
			// Token: 0x040068B4 RID: 26804
			TRIN_Space,
			// Token: 0x040068B5 RID: 26805
			SDKL_Zoom
		}

		// Token: 0x020011F1 RID: 4593
		public enum Event
		{
			// Token: 0x040068B7 RID: 26807
			SDKL_Explosion,
			// Token: 0x040068B8 RID: 26808
			SDKL_AlertEdgesRed,
			// Token: 0x040068B9 RID: 26809
			SDKL_MushroomCloud,
			// Token: 0x040068BA RID: 26810
			SDKL_Alarm,
			// Token: 0x040068BB RID: 26811
			SDKL_PulseBarGreen,
			// Token: 0x040068BC RID: 26812
			SDKL_PulseBarRed
		}
	}
}
