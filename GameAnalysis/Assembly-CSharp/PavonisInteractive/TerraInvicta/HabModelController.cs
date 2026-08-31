using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000598 RID: 1432
	public class HabModelController : MonoBehaviour
	{
		// Token: 0x06002635 RID: 9781 RVA: 0x000CEC24 File Offset: 0x000CCE24
		public void Initialize(TIHabState habState, bool fullSolarSystemVisualization, SpaceObjectController spaceObjectController = null)
		{
			this.habState = habState;
			this.fullSolarSystemVisualization = fullSolarSystemVisualization;
			this.spaceObjectController = spaceObjectController;
			this.habModuleControllers = base.transform.GetComponentsInChildren<HabModuleController>(true).ToList<HabModuleController>();
			if (fullSolarSystemVisualization)
			{
				if (this.councilorControllers == null || this.councilorControllers.Count == 0)
				{
					Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						Transform transform = componentsInChildren[i];
						if (transform.CompareTag("StationCouncilorMarkers"))
						{
							GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(AssetCacheManager.stationCouncilorMarker, transform.transform);
							gameObject.name = new StringBuilder(gameObject.name).Append(i).ToString();
							gameObject.transform.localPosition = Vector3.zero;
						}
					}
				}
				this.councilorControllers = base.transform.GetComponentsInChildren<SpaceCouncilorController>(true).ToList<SpaceCouncilorController>();
				this.sectorDataControllers = base.transform.GetComponentsInChildren<SectorDataPanelController>().ToList<SectorDataPanelController>();
				foreach (SpaceCouncilorController spaceCouncilorController in this.councilorControllers)
				{
					spaceCouncilorController.Initialize(this, habState);
					if (spaceCouncilorController.transform.parent.name.Contains("_2"))
					{
						spaceCouncilorController.tier = 3;
					}
					else if (spaceCouncilorController.transform.parent.name.Contains("_1"))
					{
						spaceCouncilorController.tier = 2;
					}
					else
					{
						spaceCouncilorController.tier = 1;
					}
					spaceCouncilorController.primaryCanvas.enabled = false;
				}
				int num = 0;
				foreach (SectorDataPanelController sectorDataPanelController in this.sectorDataControllers)
				{
					sectorDataPanelController.Initialize(habState.sectors[num++], this);
				}
				this.habModuleControllers.ForEach(delegate(HabModuleController x)
				{
					x.Initialize(true);
				});
				this.habModuleControllers.ForEach(delegate(HabModuleController x)
				{
					x.SetModuleValue(habState, this, true);
				});
				GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateAllCouncilors), null, habState, true, false);
				GameControl.eventManager.AddListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.UpdateAllCouncilors), null, habState, true, false);
				this.TurnOnIcons();
			}
			else
			{
				this.habModuleControllers.ForEach(delegate(HabModuleController x)
				{
					x.Initialize(false);
				});
				List<SpaceCouncilorController> list = this.councilorControllers;
				if (list != null)
				{
					list.Clear();
				}
				List<SectorDataPanelController> list2 = this.sectorDataControllers;
				if (list2 != null)
				{
					list2.ForEach(delegate(SectorDataPanelController x)
					{
						global::UnityEngine.Object.Destroy(x.gameObject);
					});
				}
				List<SectorDataPanelController> list3 = this.sectorDataControllers;
				if (list3 != null)
				{
					list3.Clear();
				}
				HabModuleUIElementController[] componentsInChildren2 = base.transform.GetComponentsInChildren<HabModuleUIElementController>(true);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					global::UnityEngine.Object.Destroy(componentsInChildren2[j].gameObject);
				}
				SpaceCouncilorController[] componentsInChildren3 = base.transform.GetComponentsInChildren<SpaceCouncilorController>(true);
				for (int j = 0; j < componentsInChildren3.Length; j++)
				{
					global::UnityEngine.Object.Destroy(componentsInChildren3[j].gameObject);
				}
				foreach (Transform transform2 in base.transform.GetComponentsInChildren<Transform>())
				{
					transform2.name += " Clone";
				}
				foreach (HabModuleController habModuleController in this.habModuleControllers)
				{
					habModuleController.DuplicateMaterialsForUIDisplay();
				}
				this.TurnOffIcons();
			}
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, habState, true, false);
			MeshRenderer[] componentsInChildren5 = base.GetComponentsInChildren<MeshRenderer>(true);
			for (int j = 0; j < componentsInChildren5.Length; j++)
			{
				componentsInChildren5[j].shadowCastingMode = ShadowCastingMode.Off;
			}
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.lastViewedTime = this.gameTime.currentTime;
		}

		// Token: 0x06002636 RID: 9782 RVA: 0x000CF07C File Offset: 0x000CD27C
		public void OnEnable()
		{
			if (this.habState == null || this.habState.archived)
			{
				return;
			}
			this.habComponentLink = base.transform.parent.GetComponent<HabComponent>();
			if (this.habComponentLink != null)
			{
				this.habComponentLink.habModelController = this;
				if (this.habState != null && this.fullSolarSystemVisualization)
				{
					this.TurnOnIcons();
					if (!this.habState.IsAlien())
					{
						if ((!this.viewed && TIUtilities.RandomFloatValue() < this.ShowShuttleChance()) || (this.gameTime.currentTime.DifferenceInDays(this.lastViewedTime) >= 1.0 && TIUtilities.RandomFloatValue() < this.ShowShuttleChance()))
						{
							this.RandomizeShuttleModel();
							if (this.shuttleObject != null)
							{
								this.RandomizeShuttleModule();
							}
						}
						else if (this.gameTime.currentTime.DifferenceInDays(this.lastViewedTime) >= 1.0 && this.shuttleObject != null)
						{
							this.shuttleObject.SetActive(false);
						}
						this.viewed = true;
						return;
					}
				}
				else
				{
					this.habComponentLink.enabled = false;
				}
			}
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x000CF1B9 File Offset: 0x000CD3B9
		public void OnDisable()
		{
			if (this.fullSolarSystemVisualization)
			{
				this.TurnOffIcons();
				if (this.spaceObjectController != null)
				{
					this.spaceObjectController.TurnOffAmbientAudio();
				}
			}
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x000CF1E4 File Offset: 0x000CD3E4
		public void TurnOffIcons()
		{
			this.showUIIcons = false;
			foreach (SpaceCouncilorController spaceCouncilorController in this.councilorControllers)
			{
				spaceCouncilorController.primaryCanvas.enabled = false;
			}
			foreach (SectorDataPanelController sectorDataPanelController in this.sectorDataControllers)
			{
				sectorDataPanelController.TurnOffSectorData();
			}
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x000CF280 File Offset: 0x000CD480
		public void TurnOnIcons()
		{
			this.showUIIcons = true;
			this.UpdateAllCouncilors(null);
			foreach (SectorDataPanelController sectorDataPanelController in this.sectorDataControllers)
			{
				sectorDataPanelController.SetSectorData();
			}
		}

		// Token: 0x0600263A RID: 9786 RVA: 0x000CF2E0 File Offset: 0x000CD4E0
		private void RandomizeShuttleModel()
		{
			if (this.shuttleObject != null)
			{
				global::UnityEngine.Object.Destroy(this.shuttleObject);
			}
			List<GameObject> list = new List<GameObject>();
			list.Add(AssetCacheManager.stationShuttleSoyuz);
			if (TIEffectsState.CheckForAnyEffectInContext(Context.AdvancedAircraft, GameControl.control.activePlayer))
			{
				list.Add(AssetCacheManager.stationShuttleA);
			}
			if (TIEffectsState.CheckForAnyEffectInContext(Context.HabNuclearFreighters, GameControl.control.activePlayer))
			{
				list.Add(AssetCacheManager.stationShuttleB);
			}
			GameObject gameObject = list.SelectRandomItem<GameObject>();
			this.shuttleObject = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x000CF36C File Offset: 0x000CD56C
		private bool RandomizeShuttleModule()
		{
			this.lastViewedTime = this.gameTime.currentTime;
			List<int> list = new List<int>();
			for (int i = 1; i < this.habModuleControllers.Count - 1; i++)
			{
				if (this.habModuleControllers[i].habModule != null && this.habModuleControllers[i].habModule.functional && this.habModuleControllers[i].moduleNum > 0)
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				int num = list[global::UnityEngine.Random.Range(0, list.Count)];
				this.shuttleObject.transform.SetParent(this.habModuleControllers[num].transform, false);
				switch (this.habModuleControllers[num].habModule.tier)
				{
				case 1:
					this.shuttleObject.transform.localPosition = new Vector3(0f, 0f, -17f);
					break;
				case 2:
					this.shuttleObject.transform.localPosition = new Vector3(0f, 0f, -45f);
					break;
				case 3:
					this.shuttleObject.transform.localPosition = new Vector3(0f, 0f, -75f);
					break;
				}
				return true;
			}
			this.shuttleObject.SetActive(false);
			return false;
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x000CF4E1 File Offset: 0x000CD6E1
		private float ShowShuttleChance()
		{
			return 0.25f + Mathf.Clamp((float)this.habState.crew / 5000f, 0f, 0.65f);
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x000CF50A File Offset: 0x000CD70A
		public void UpdateAllCouncilors(CouncilorPositionUpdated e)
		{
			this.UpdateAllCouncilors(e.councilor);
		}

		// Token: 0x0600263E RID: 9790 RVA: 0x000CF518 File Offset: 0x000CD718
		public void UpdateAllCouncilors(CouncilorDepartsHab e)
		{
			this.UpdateAllCouncilors(e.councilor);
		}

		// Token: 0x0600263F RID: 9791 RVA: 0x000CF528 File Offset: 0x000CD728
		public void UpdateAllCouncilors(TICouncilorState conditionalCouncilor = null)
		{
			if (this.showUIIcons)
			{
				List<TICouncilorState> list = this.habState.CouncilorsPresentAndKnownToFaction(GameControl.control.activePlayer, false, null);
				List<TICouncilorState> list2 = (from x in this.councilorControllers
					where x.councilor != null
					select x.councilor).ToList<TICouncilorState>();
				if (conditionalCouncilor == null || list.Contains(conditionalCouncilor) || list2.Contains(conditionalCouncilor))
				{
					List<TICouncilorState> list3 = list2.Except<TICouncilorState>(list).ToList<TICouncilorState>();
					foreach (SpaceCouncilorController spaceCouncilorController in this.councilorControllers)
					{
						if (list3.Contains(spaceCouncilorController.councilor))
						{
							spaceCouncilorController.currentlyActive = false;
							spaceCouncilorController.primaryCanvas.enabled = false;
							spaceCouncilorController.councilor = null;
						}
						if (list.Contains(spaceCouncilorController.councilor))
						{
							spaceCouncilorController.currentlyActive = true;
							spaceCouncilorController.primaryCanvas.enabled = true;
						}
					}
					foreach (TICouncilorState ticouncilorState in list)
					{
						if (!list2.Contains(ticouncilorState))
						{
							this.AddCouncilorMarker(ticouncilorState);
						}
					}
				}
			}
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x000CF6B4 File Offset: 0x000CD8B4
		public bool CanUseMarker(SpaceCouncilorController controller)
		{
			if (!controller.parentMesh.enabled || !controller.transform.parent.gameObject.activeInHierarchy)
			{
				return false;
			}
			string name = controller.parentMesh.material.name;
			if (!name.Contains("Torus"))
			{
				if (name.Contains("T1") && controller.tier != 1)
				{
					return false;
				}
				if (name.Contains("T2") && controller.tier != 2)
				{
					return false;
				}
				if (name.Contains("T3") && controller.tier != 3)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x000CF750 File Offset: 0x000CD950
		public void AddCouncilorMarker(TICouncilorState councilor)
		{
			SpaceCouncilorController spaceCouncilorController = this.councilorControllers.Where<SpaceCouncilorController>((SpaceCouncilorController x) => !x.currentlyActive && this.CanUseMarker(x)).SelectRandomItem<SpaceCouncilorController>();
			if (spaceCouncilorController != null)
			{
				spaceCouncilorController.UpdateController(councilor);
			}
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x000CF78A File Offset: 0x000CD98A
		public List<HabModuleController> GetModuleControllers()
		{
			return this.habModuleControllers;
		}

		// Token: 0x06002643 RID: 9795 RVA: 0x000CF794 File Offset: 0x000CD994
		public void HabHitByNukeInCombat(TIFactionState shootingFaction, Vector3 hitLocation)
		{
			List<HabModuleController> list = this.habModuleControllers.OrderByDescending<HabModuleController, float>((HabModuleController module) => (hitLocation - module.transform.position).sqrMagnitude).ToList<HabModuleController>();
			base.StartCoroutine(this.NuclearBlastDestroyModules(shootingFaction, list));
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x000CF7DA File Offset: 0x000CD9DA
		private IEnumerator NuclearBlastDestroyModules(TIFactionState shootingFaction, List<HabModuleController> modules)
		{
			foreach (HabModuleController habModuleController in modules)
			{
				if (!(habModuleController == null) && habModuleController.habModule.active && !habModuleController.habModule.isCombatModule && !habModuleController.habModule.moduleTemplate.coreModule && habModuleController.habModule.okay && habModuleController.habModule.moduleTemplate.SpecialRules.Intersect<HabModuleSpecialRule>(TIHabModuleTemplate.combatTroopsRules).Count<HabModuleSpecialRule>() == 0)
				{
					habModuleController.DestroyHabModule(shootingFaction);
					yield return new WaitForSeconds(global::UnityEngine.Random.Range(0.1f, 0.3f));
				}
			}
			List<HabModuleController>.Enumerator enumerator = default(List<HabModuleController>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x000CF7F0 File Offset: 0x000CD9F0
		public void OnHabDestroyed(HabDestroyed e)
		{
			this.TurnOffIcons();
			this.killerFleet = e.byFleet;
			if (this != null)
			{
				base.Invoke("HabDestroyed", 1.9f);
			}
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x000CF820 File Offset: 0x000CDA20
		private void HabDestroyed()
		{
			TISpaceObjectState spaceObjectStateSelected = World.Active.GetExistingManager<SpaceObjectSelection>().spaceObjectStateSelected;
			bool flag = base.gameObject.activeInHierarchy && spaceObjectStateSelected == this.habState;
			base.gameObject.SetActive(false);
			if (flag || (spaceObjectStateSelected != null && (spaceObjectStateSelected == this.killerFleet || spaceObjectStateSelected == this.habState)))
			{
				if (this.killerFleet != null && !this.killerFleet.deleted)
				{
					TIUtilities.GotoGameState(this.killerFleet, true, false, false, false, false, -1f);
				}
				else
				{
					TIUtilities.GotoGameState(this.habState.ref_naturalSpaceObject, true, false, false, false, false, -1f);
				}
			}
			global::UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06002647 RID: 9799 RVA: 0x000CF8E4 File Offset: 0x000CDAE4
		public void OnDestroy()
		{
			if (this.fullSolarSystemVisualization)
			{
				List<HabModuleController> list = this.habModuleControllers;
				if (list != null)
				{
					list.ForEach(delegate(HabModuleController x)
					{
						x.renderers.ForEach(delegate(MeshRenderer y)
						{
							global::UnityEngine.Object.Destroy(y.GetComponentInChildren<MeshRenderer>().material);
						});
					});
				}
				GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateAllCouncilors), null);
				GameControl.eventManager.RemoveListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.UpdateAllCouncilors), null);
			}
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null);
		}

		// Token: 0x04001C6E RID: 7278
		public HabComponent habComponentLink;

		// Token: 0x04001C6F RID: 7279
		public List<SpaceCouncilorController> councilorControllers;

		// Token: 0x04001C70 RID: 7280
		public List<SectorDataPanelController> sectorDataControllers;

		// Token: 0x04001C71 RID: 7281
		private List<HabModuleController> habModuleControllers;

		// Token: 0x04001C72 RID: 7282
		private SpaceObjectController spaceObjectController;

		// Token: 0x04001C73 RID: 7283
		private GameObject shuttleObject;

		// Token: 0x04001C74 RID: 7284
		private TIHabState habState;

		// Token: 0x04001C75 RID: 7285
		private bool showUIIcons;

		// Token: 0x04001C76 RID: 7286
		private bool viewed;

		// Token: 0x04001C77 RID: 7287
		private TIDateTime lastViewedTime;

		// Token: 0x04001C78 RID: 7288
		private GameTimeManager gameTime;

		// Token: 0x04001C79 RID: 7289
		[SerializeField]
		private bool fullSolarSystemVisualization;

		// Token: 0x04001C7A RID: 7290
		public bool mouseOverHabUIIcon;

		// Token: 0x04001C7B RID: 7291
		private TISpaceFleetState killerFleet;
	}
}
