using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Audio
{
	// Token: 0x020009DA RID: 2522
	public class SoundEffectController : MonoBehaviour
	{
		// Token: 0x06005EB3 RID: 24243 RVA: 0x002CE412 File Offset: 0x002CC612
		public static void PlaySound(string soundStr)
		{
			Debug.Log("Deprecated: Sound playing from SoundManager: " + soundStr);
		}

		// Token: 0x06005EB4 RID: 24244 RVA: 0x002CE424 File Offset: 0x002CC624
		public static void PlaySelectSound(TIGameState state)
		{
			if (!TIGameState.Valid(state))
			{
				return;
			}
			if (state.isSpaceObjectState)
			{
				SoundEffectController.PlaySelectSound(state.ref_spaceObject);
				return;
			}
			if (state.isCouncilorState)
			{
				SoundEffectController.PlaySelectSound(state.ref_councilor);
				return;
			}
			if (state.isArmyState)
			{
				SoundEffectController.PlaySelectSound(state.ref_army);
				return;
			}
			if (state.isRegionState)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
		}

		// Token: 0x06005EB5 RID: 24245 RVA: 0x002CE498 File Offset: 0x002CC698
		public static void PlaySelectSound(TISpaceObjectState spaceObjectState)
		{
			if (!TIGameState.Valid(spaceObjectState))
			{
				return;
			}
			SpaceObjectType objectType = spaceObjectState.objectType;
			if (objectType == SpaceObjectType.Fleet)
			{
				SoundEffectController.PlaySelectSound(spaceObjectState.ref_fleet);
				return;
			}
			if (objectType != SpaceObjectType.Hab)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
				return;
			}
			SoundEffectController.PlaySelectSound(spaceObjectState.ref_hab);
		}

		// Token: 0x06005EB6 RID: 24246 RVA: 0x002CE4E4 File Offset: 0x002CC6E4
		public static void PlaySelectSound(TICouncilorState councilor)
		{
			if (!TIGameState.Valid(councilor))
			{
				return;
			}
			if (councilor.faction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyCouncilorSelect", false, false);
				return;
			}
			if (councilor.faction != null && councilor.faction.IsAlienFaction)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienCouncilorSelect", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanCouncilorSelect", false, false);
		}

		// Token: 0x06005EB7 RID: 24247 RVA: 0x002CE554 File Offset: 0x002CC754
		public static void PlaySelectSound(TISpaceFleetState fleet)
		{
			if (!TIGameState.Valid(fleet))
			{
				return;
			}
			if (fleet.faction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyFleetSelect", false, false);
				return;
			}
			if (fleet.faction.IsAlienFaction)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienFleetSelect", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanFleetSelect", false, false);
		}

		// Token: 0x06005EB8 RID: 24248 RVA: 0x002CE5B4 File Offset: 0x002CC7B4
		public static void PlaySelectSound(TISectorState sector)
		{
			if (!TIGameState.Valid(sector))
			{
				return;
			}
			if (sector.hab.IsStation)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SelectSpaceStation", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SelectBase", false, false);
			}
			if (sector.ref_faction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyHabSelect", false, false);
				return;
			}
			TIFactionState faction = sector.faction;
			if (faction != null && faction.IsAlienFaction)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienHabSelect", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanHabSelect", false, false);
		}

		// Token: 0x06005EB9 RID: 24249 RVA: 0x002CE642 File Offset: 0x002CC842
		public static void PlaySelectSound(TIHabState hab)
		{
			if (!TIGameState.Valid(hab))
			{
				return;
			}
			SoundEffectController.PlaySelectSound(hab.coreSector);
		}

		// Token: 0x06005EBA RID: 24250 RVA: 0x002CE658 File Offset: 0x002CC858
		public static void PlaySelectSound(TIArmyState armyState)
		{
			if (!TIGameState.Valid(armyState))
			{
				return;
			}
			if (armyState.ref_faction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyArmySelect", false, false);
				return;
			}
			TIFactionState faction = armyState.faction;
			if (faction == null || !faction.IsAlienFaction)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OtherHumanArmySelect", false, false);
				return;
			}
			if (armyState.AlienMegafaunaArmy)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienMegafaunaArmySelect", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_AlienArmySelect", false, false);
		}

		// Token: 0x06005EBB RID: 24251 RVA: 0x002CE6D4 File Offset: 0x002CC8D4
		public static void PlayBuildHabModuleSound(TIHabModuleTemplate module, TIHabState hab)
		{
			if (!TIGameState.Valid(hab))
			{
				return;
			}
			if (hab.IsBase)
			{
				switch (module.tier)
				{
				case 1:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildBaseModuleT1", false, false);
					return;
				case 2:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildBaseModuleT2", false, false);
					return;
				case 3:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildBaseModuleT3", false, false);
					return;
				default:
					return;
				}
			}
			else
			{
				switch (module.tier)
				{
				case 1:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildStationModuleT1", false, false);
					return;
				case 2:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildStationModuleT2", false, false);
					return;
				case 3:
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildStationModuleT3", false, false);
					return;
				default:
					return;
				}
			}
		}
	}
}
