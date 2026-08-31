using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200059B RID: 1435
	public class HabSiteController : MonoBehaviour
	{
		// Token: 0x17000572 RID: 1394
		// (get) Token: 0x0600267A RID: 9850 RVA: 0x000D0802 File Offset: 0x000CEA02
		// (set) Token: 0x0600267B RID: 9851 RVA: 0x000D080A File Offset: 0x000CEA0A
		[HideInInspector]
		public TIHabSiteState site { get; private set; }

		// Token: 0x0600267C RID: 9852 RVA: 0x000D0813 File Offset: 0x000CEA13
		public void Awake()
		{
			this.activePlayer = GameControl.control.activePlayer;
			this.cameraMgr = World.Active.GetExistingManager<CameraManager>();
			if (this.initialized)
			{
				this.SetMarkerData();
			}
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x000D0844 File Offset: 0x000CEA44
		public void SetActivePlayer(bool startup)
		{
			if (!startup)
			{
				GameControl.eventManager.RemoveListener<FactionExplorationRangeChanged>(new EventManager.EventDelegate<FactionExplorationRangeChanged>(this.UpdateSiteMarker), null);
			}
			this.activePlayer = GameControl.control.activePlayer;
			GameControl.eventManager.AddListener<FactionExplorationRangeChanged>(new EventManager.EventDelegate<FactionExplorationRangeChanged>(this.UpdateSiteMarker), null, this.activePlayer, true, false);
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x000D089C File Offset: 0x000CEA9C
		public void Initialize(TIGameState state, SpaceBodyController parentBodyController)
		{
			this.site = state.ref_habSite;
			this.mainCam = Camera.main;
			if (this.site == null)
			{
				return;
			}
			this.myCouncilors = new List<CouncilorView>();
			this.enemyCouncilors = new List<CouncilorView>();
			base.GetComponent<Canvas>().worldCamera = GameControl.control.mainCamera;
			this.gameTimeManager = World.Active.GetExistingManager<GameTimeManager>();
			this.selectionRenderer.enabled = false;
			this.SetMarkerData();
			this.sitePosition = Quaternion.AngleAxis(this.site.longitude, -Vector3.up) * Quaternion.AngleAxis(this.site.latitude, -Vector3.right) * Vector3.forward * this.site.parentBody.modelScale;
			if (this.site.parentBody.objectType == SpaceObjectType.Asteroid || this.site.parentBody.objectType == SpaceObjectType.AsteroidalMoon || this.site.parentBody.objectType == SpaceObjectType.Comet || (this.site.parentBody.objectType == SpaceObjectType.DwarfPlanet && this.site.parentBody.oblateness >= 0.10000000149011612))
			{
				SkinnedMeshRenderer component = parentBodyController.GetComponent<SkinnedMeshRenderer>();
				Vector3[] array;
				Vector3[] array2;
				if (component != null)
				{
					array = component.sharedMesh.vertices;
					array2 = component.sharedMesh.normals;
				}
				else
				{
					MeshFilter meshFilter = parentBodyController.GetComponent<MeshFilter>();
					if (meshFilter == null)
					{
						meshFilter = parentBodyController.GetComponentInChildren<MeshFilter>();
					}
					array = meshFilter.mesh.vertices;
					array2 = meshFilter.mesh.normals;
				}
				float num = float.PositiveInfinity;
				int num2 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 vector = array[i];
					float sqrMagnitude = (this.sitePosition - vector).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						num2 = i;
					}
				}
				Vector3 vector2 = array[num2];
				this.sitePosition.Set(vector2.x, vector2.y, vector2.z);
				this.sitePosition = Vector3.MoveTowards(this.sitePosition, parentBodyController.transform.position, -this.site.parentBody.modelScale / 108f);
				base.gameObject.transform.localPosition = this.sitePosition;
				Vector3 vector3 = this.site.parentBody.controller.transform.rotation * array2[num2];
				base.gameObject.transform.rotation = Quaternion.LookRotation(vector3 * -1f, this.site.parentBody.controller.transform.rotation * Vector3.up);
			}
			else
			{
				base.gameObject.transform.localPosition = this.sitePosition;
				base.gameObject.transform.rotation = Quaternion.FromToRotation(base.transform.forward * -1f, base.transform.position - parentBodyController.transform.position) * base.transform.rotation;
				base.gameObject.transform.Rotate(0f, 0f, -base.transform.eulerAngles.z);
			}
			this.habSiteMarker.transform.localScale = Vector3.one;
			this.habSiteMarkerModel.transform.localScale = Vector3.one;
			float num3 = this.site.parentBody.modelScale / 1400f;
			if (this.site.parentBody.isEarth)
			{
				num3 /= 2.5f;
			}
			base.gameObject.transform.localScale = new Vector3(num3, num3, num3);
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.site.SetController(this);
			this.initialized = true;
			this.habSiteTooltip.SetDelegate("BodyText", () => this.BuildMarkerTooltip());
			GameControl.eventManager.AddListener<HabCreated>(new EventManager.EventDelegate<HabCreated>(this.OnHabCreated), null, this.site, true, false);
			GameControl.eventManager.AddListener<SpaceBodyProspected>(new EventManager.EventDelegate<SpaceBodyProspected>(this.OnSpaceBodyProspected), null, this.site.parentBody, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateSiteMarker), null, this.site, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.UpdateSiteMarker), null, this.site, true, false);
			GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnBeginBombardment), null, this.site, true, false);
			GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnEndBombardment), null, this.site, true, false);
			GameControl.eventManager.AddListener<FleetDisbanded>(new EventManager.EventDelegate<FleetDisbanded>(this.OnFleetDisbanded), null, this.site, true, false);
			GameControl.eventManager.AddListener<CombatStarts>(new EventManager.EventDelegate<CombatStarts>(this.OnCombatStarts), null, null, true, false);
			TIHabState hab = this.site.hab;
			if (hab != null && hab.underBombardment)
			{
				this.InitializeForGroundFire();
			}
			this.SetActivePlayer(true);
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x000D0DE0 File Offset: 0x000CEFE0
		public void OnEnable()
		{
			if (this.initialized)
			{
				this.SetMarkerData();
			}
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x000D0DF0 File Offset: 0x000CEFF0
		private void OnSpaceBodyProspected(SpaceBodyProspected e)
		{
			if (e.faction == this.activePlayer)
			{
				this.SetMarkerData();
			}
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x000D0E0B File Offset: 0x000CF00B
		private void UpdateSiteMarker(SectorAssignedToFaction e)
		{
			this.SetMarkerData();
		}

		// Token: 0x06002682 RID: 9858 RVA: 0x000D0E13 File Offset: 0x000CF013
		private void UpdateSiteMarker(FleetUndocks e)
		{
			this.SetFleetData();
		}

		// Token: 0x06002683 RID: 9859 RVA: 0x000D0E1B File Offset: 0x000CF01B
		private void UpdateSiteMarker(FactionExplorationRangeChanged e)
		{
			this.SetMarkerData();
		}

		// Token: 0x06002684 RID: 9860 RVA: 0x000D0E23 File Offset: 0x000CF023
		private void UpdateSiteMarker(FleetArrivesAtDestination e)
		{
			this.SetMarkerData();
		}

		// Token: 0x06002685 RID: 9861 RVA: 0x000D0E2C File Offset: 0x000CF02C
		private void OnHabCreated(HabCreated e)
		{
			GameControl.eventManager.AddListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null, this.site.hab, false, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabModuleDestroyed), null, this.site.hab, false, false);
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.UpdateSiteMarker), null, this.site.hab, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilors), null, this.site.hab, true, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateCouncilors), null, this.site.hab, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.UpdateCouncilors), null, this.site.hab, true, false);
			GameControl.eventManager.AddListener<LaunchRocketFromHabEvent>(new EventManager.EventDelegate<LaunchRocketFromHabEvent>(this.OnLaunchRocketFromHabEvent), null, this.site.hab, true, false);
			GameControl.eventManager.AddListener<HabSymbolAssigned>(new EventManager.EventDelegate<HabSymbolAssigned>(this.OnHabSymbolAssigned), null, this.site.hab, true, false);
			this.SetMarkerData();
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x000D0F60 File Offset: 0x000CF160
		private void OnHabDestroyed(HabDestroyed e)
		{
			this.TriggerExplosion();
			GameControl.eventManager.RemoveListener<HabDestroyed>(new EventManager.EventDelegate<HabDestroyed>(this.OnHabDestroyed), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.OnHabModuleDestroyed), null);
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.UpdateSiteMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilors), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsHab>(new EventManager.EventDelegate<CouncilorDepartsHab>(this.UpdateCouncilors), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateCouncilors), null);
			GameControl.eventManager.RemoveListener<LaunchRocketFromHabEvent>(new EventManager.EventDelegate<LaunchRocketFromHabEvent>(this.OnLaunchRocketFromHabEvent), null);
			GameControl.eventManager.RemoveListener<HabSymbolAssigned>(new EventManager.EventDelegate<HabSymbolAssigned>(this.OnHabSymbolAssigned), null);
			this.SetMarkerData();
		}

		// Token: 0x06002687 RID: 9863 RVA: 0x000D1031 File Offset: 0x000CF231
		private void OnFleetDisbanded(FleetDisbanded e)
		{
			this.SetFleetData();
		}

		// Token: 0x06002688 RID: 9864 RVA: 0x000D1039 File Offset: 0x000CF239
		private void UpdateCouncilors(CouncilorPositionUpdated e)
		{
			this.SetCouncilorData();
		}

		// Token: 0x06002689 RID: 9865 RVA: 0x000D1041 File Offset: 0x000CF241
		private void UpdateCouncilors(CouncilorVisibilityChanged e)
		{
			this.SetCouncilorData();
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x000D1049 File Offset: 0x000CF249
		private void UpdateCouncilors(CouncilorDepartsHab e)
		{
			this.SetCouncilorData();
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x000D1051 File Offset: 0x000CF251
		private void OnLaunchRocketFromHabEvent(LaunchRocketFromHabEvent e)
		{
			this.TriggerLaunch();
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x000D1059 File Offset: 0x000CF259
		private void OnHabSymbolAssigned(HabSymbolAssigned e)
		{
			this.SetMarkerData();
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x000D1061 File Offset: 0x000CF261
		private void OnHabModuleDestroyed(HabModuleDestroyed e)
		{
			this.TriggerExplosion();
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x000D1069 File Offset: 0x000CF269
		public void TriggerExplosion()
		{
			this.explosionFX.Play();
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x000D1076 File Offset: 0x000CF276
		public void TriggerLaunch()
		{
			this.launchFX.Play();
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x000D1084 File Offset: 0x000CF284
		public void OnClickHabSite()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				TIUtilities.GotoGameState(this.site, true, true, false, true, false, -1f);
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabSiteState)))
				{
					this.spaceObjectSelection.BlockThisFrame = true;
					GameControl.eventManager.TriggerEvent(new HabSiteSelectedEvent(this.site), null, Array.Empty<object>());
					return;
				}
				if (this.site.hasPlannedOrOperatingBase && this.activePlayer.KnownBases.Contains(this.site.hab))
				{
					if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIHabState)))
					{
						this.spaceObjectSelection.BlockThisFrame = true;
						GameControl.eventManager.TriggerEvent(new HabSelectedEvent(this.site.hab), null, new object[] { this.site.hab });
						return;
					}
					if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TISectorState)))
					{
						this.spaceObjectSelection.BlockThisFrame = true;
						GameControl.eventManager.TriggerEvent(new SectorSelectedEvent(this.site.hab.sectors[0]), null, new object[] { this.site.hab.sectors[0] });
						return;
					}
				}
			}
			else if (this.site.hasPlannedOrOperatingBase && this.activePlayer.KnownBases.Contains(this.site.hab))
			{
				this.spaceObjectSelection.BlockThisFrame = true;
				if (GeneralControlsController.UIOtherSelectedState == this.site.hab)
				{
					GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.site.hab, false), null, Array.Empty<object>());
					return;
				}
				TIUtilities.GotoGameState(this.site.hab, true, true, true, false, false, -1f);
				return;
			}
			else
			{
				TIUtilities.GotoGameState(this.site, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x000D1278 File Offset: 0x000CF478
		public void OnClickSector(int value)
		{
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				TIUtilities.GotoGameState(this.site, false, true, true, true, false, -1f);
				if (GeneralControlsController.UITargetingMode.TargetedGameStates().Contains(typeof(TISectorState)))
				{
					GameControl.eventManager.TriggerEvent(new SectorSelectedEvent(this.site.hab.sectors[value]), null, new object[] { this.site.hab.sectors[value] });
					return;
				}
			}
			else
			{
				this.spaceObjectSelection.BlockThisFrame = true;
				GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.site.hab, true), null, Array.Empty<object>());
			}
		}

		// Token: 0x06002692 RID: 9874 RVA: 0x000D1330 File Offset: 0x000CF530
		public static string GetInlineResourceOutputIcon(FactionResource resource, TIMiningProfileTemplate template, float value)
		{
			if (value <= 0f)
			{
				return TemplateManager.global.zeroResourcesInlineSpritePath;
			}
			if (template.ZeroInBaseRange(resource))
			{
				return TemplateManager.global.unknownResourcesInlineSpritePath;
			}
			if (value <= TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource] * 0.333f)
			{
				return TemplateManager.global.level1ResourcesInlineSpritePath;
			}
			if (value <= TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource] * 0.667f)
			{
				return TemplateManager.global.level2ResourcesInlineSpritePath;
			}
			if (value <= TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource] * 0.95f)
			{
				return TemplateManager.global.level3ResourcesInlineSpritePath;
			}
			return TemplateManager.global.level4ResourcesInlineSpritePath;
		}

		// Token: 0x06002693 RID: 9875 RVA: 0x000D13DC File Offset: 0x000CF5DC
		public static string BuildOutputString(TIHabSiteState site)
		{
			StringBuilder stringBuilder = new StringBuilder();
			TIFactionState tifactionState = GameControl.control.activePlayer;
			TIGameState tigameState = tifactionState;
			TIHabState hab = site.hab;
			if (tigameState == ((hab != null) ? hab.faction : null))
			{
				TIHabModuleState mine = site.hab.mine;
				if (mine != null && mine.active)
				{
					TIHabModuleTemplate moduleTemplate = site.hab.mine.moduleTemplate;
					if (site.water_day > 0f)
					{
						stringBuilder.Append(TemplateManager.global.waterInlineSpritePath).Append(TIUtilities.FormatSmallNumber(moduleTemplate.GetMiningIncome_Month(tifactionState, site, FactionResource.Water), 7, 0, true, false)).Append(" ");
					}
					if (site.volatiles_day > 0f)
					{
						stringBuilder.Append(TemplateManager.global.volatilesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(moduleTemplate.GetMiningIncome_Month(tifactionState, site, FactionResource.Volatiles), 7, 0, true, false)).Append(" ");
					}
					if (site.metals_day > 0f)
					{
						stringBuilder.Append(TemplateManager.global.metalsInlineSpritePath).Append(TIUtilities.FormatSmallNumber(moduleTemplate.GetMiningIncome_Month(tifactionState, site, FactionResource.Metals), 7, 0, true, false)).Append(" ");
					}
					if (site.nobles_day > 0f)
					{
						stringBuilder.Append(TemplateManager.global.noblesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(moduleTemplate.GetMiningIncome_Month(tifactionState, site, FactionResource.NobleMetals), 7, 0, true, false)).Append(" ");
					}
					if (site.fissiles_day > 0f)
					{
						stringBuilder.Append(TemplateManager.global.fissilesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(moduleTemplate.GetMiningIncome_Month(tifactionState, site, FactionResource.Fissiles), 7, 0, false, false));
						goto IL_03E2;
					}
					goto IL_03E2;
				}
			}
			if (GameControl.control.activePlayer.Prospected(site.parentBody))
			{
				if (site.water_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.waterInlineSpritePath).Append(TIUtilities.FormatSmallNumber(site.GetMonthlyProduction(FactionResource.Water), 7, 0, true, false)).Append(" ");
				}
				if (site.volatiles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.volatilesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(site.GetMonthlyProduction(FactionResource.Volatiles), 7, 0, true, false)).Append(" ");
				}
				if (site.metals_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.metalsInlineSpritePath).Append(TIUtilities.FormatSmallNumber(site.GetMonthlyProduction(FactionResource.Metals), 7, 0, true, false)).Append(" ");
				}
				if (site.nobles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.noblesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(site.GetMonthlyProduction(FactionResource.NobleMetals), 7, 0, true, false)).Append(" ");
				}
				if (site.fissiles_day > 0f)
				{
					stringBuilder.Append(TemplateManager.global.fissilesInlineSpritePath).Append(TIUtilities.FormatSmallNumber(site.GetMonthlyProduction(FactionResource.Fissiles), 7, 0, false, false));
				}
			}
			else
			{
				stringBuilder.Append(TemplateManager.global.waterInlineSpritePath).Append(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Water, site.miningProfile, site.GetHabSiteExpectedProductivity_day(FactionResource.Water))).Append(" ");
				stringBuilder.Append(TemplateManager.global.volatilesInlineSpritePath).Append(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Volatiles, site.miningProfile, site.GetHabSiteExpectedProductivity_day(FactionResource.Volatiles))).Append(" ");
				stringBuilder.Append(TemplateManager.global.metalsInlineSpritePath).Append(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Metals, site.miningProfile, site.GetHabSiteExpectedProductivity_day(FactionResource.Metals))).Append(" ");
				stringBuilder.Append(TemplateManager.global.noblesInlineSpritePath).Append(HabSiteController.GetInlineResourceOutputIcon(FactionResource.NobleMetals, site.miningProfile, site.GetHabSiteExpectedProductivity_day(FactionResource.NobleMetals))).Append(" ");
				stringBuilder.Append(TemplateManager.global.fissilesInlineSpritePath).Append(HabSiteController.GetInlineResourceOutputIcon(FactionResource.Fissiles, site.miningProfile, site.GetHabSiteExpectedProductivity_day(FactionResource.Fissiles)));
			}
			IL_03E2:
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x000D17DC File Offset: 0x000CF9DC
		public void SetCouncilorData()
		{
			this.myCouncilors.Clear();
			this.enemyCouncilors.Clear();
			List<TICouncilorState> list = TIMissionPhaseState.GetVisibleCouncilorsAtLocation(this.activePlayer, this.site, TemplateManager.global.intelToSeeNeutralPawn, 1f, false);
			if (this.site.hasPlannedOrOperatingBase)
			{
				list.AddRange(this.site.hab.CouncilorsPresentAndKnownToFaction(this.activePlayer, false, null));
			}
			foreach (TISpaceFleetState tispaceFleetState in this.site.landedFleets)
			{
				list.AddRange(tispaceFleetState.CouncilorsPresentAndKnownToFaction(this.activePlayer));
			}
			list = list.Distinct<TICouncilorState>().ToList<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in list)
			{
				CouncilorView viewofCouncilor = this.activePlayer.GetViewofCouncilor(ticouncilorState);
				if (ticouncilorState.faction == this.activePlayer)
				{
					this.myCouncilors.Add(viewofCouncilor);
				}
				else
				{
					this.enemyCouncilors.Add(viewofCouncilor);
				}
			}
			if (this.myCouncilors.Count == 0)
			{
				this.myCouncilorGrid.gameObject.SetActive(false);
			}
			else
			{
				this.myCouncilorGrid.gameObject.SetActive(true);
				this.myCouncilorGrid.SetListSize<HabSiteCouncilorGridItemController>(this.myCouncilors.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator3 = this.myCouncilorGrid.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (HabSiteController.<>o__59.<>p__0 == null)
						{
							HabSiteController.<>o__59.<>p__0 = CallSite<Func<CallSite, object, HabSiteCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(HabSiteCouncilorGridItemController), typeof(HabSiteController)));
						}
						HabSiteCouncilorGridItemController habSiteCouncilorGridItemController = HabSiteController.<>o__59.<>p__0.Target(HabSiteController.<>o__59.<>p__0, enumerator3.Current);
						habSiteCouncilorGridItemController.Init(this.myCouncilors[num++]);
						habSiteCouncilorGridItemController.UpdateGridItem();
					}
				}
			}
			if (this.enemyCouncilors.Count == 0)
			{
				this.enemyCouncilorGrid.gameObject.SetActive(false);
				return;
			}
			this.enemyCouncilorGrid.gameObject.SetActive(true);
			this.enemyCouncilorGrid.SetListSize<HabSiteCouncilorGridItemController>(this.enemyCouncilors.Count, false, false);
			int num2 = 0;
			using (IEnumerator<object> enumerator3 = this.enemyCouncilorGrid.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (HabSiteController.<>o__59.<>p__1 == null)
					{
						HabSiteController.<>o__59.<>p__1 = CallSite<Func<CallSite, object, HabSiteCouncilorGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(HabSiteCouncilorGridItemController), typeof(HabSiteController)));
					}
					HabSiteCouncilorGridItemController habSiteCouncilorGridItemController2 = HabSiteController.<>o__59.<>p__1.Target(HabSiteController.<>o__59.<>p__1, enumerator3.Current);
					habSiteCouncilorGridItemController2.Init(this.enemyCouncilors[num2++]);
					habSiteCouncilorGridItemController2.UpdateGridItem();
				}
			}
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x000D1AEC File Offset: 0x000CFCEC
		public void SetFleetData()
		{
			List<TISpaceFleetState> list = (from x in this.site.landedFleets.Intersect<TISpaceFleetState>(this.activePlayer.KnownFleets)
				orderby x.faction.ID
				select x).ToList<TISpaceFleetState>();
			if (list.Count == 0)
			{
				this.landedFleetGrid.gameObject.SetActive(false);
				return;
			}
			this.landedFleetGrid.gameObject.SetActive(true);
			int num = 0;
			this.landedFleetGrid.SetListSize<HabSiteLandedFleetGridItemController>(list.Count, false, false);
			using (IEnumerator<object> enumerator = this.landedFleetGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (HabSiteController.<>o__60.<>p__0 == null)
					{
						HabSiteController.<>o__60.<>p__0 = CallSite<Func<CallSite, object, HabSiteLandedFleetGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(HabSiteLandedFleetGridItemController), typeof(HabSiteController)));
					}
					HabSiteController.<>o__60.<>p__0.Target(HabSiteController.<>o__60.<>p__0, enumerator.Current).UpdateFleetGridItem(list[num++]);
				}
			}
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x000D1C08 File Offset: 0x000CFE08
		public string BuildMarkerTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = this.site.hasPlannedOrOperatingBase && GameControl.control.activePlayer.HasIntelOnSpaceAssetLocation(this.site.hab);
			if (flag)
			{
				stringBuilder.AppendLine(this.site.hab.displayName);
			}
			stringBuilder.AppendLine(this.site.displayName);
			stringBuilder.AppendLine(HabSiteController.BuildOutputString(this.site));
			if (flag)
			{
				if (this.site.hab.SpaceCombatValue() > 0f)
				{
					stringBuilder.Append(TemplateManager.global.habDefenseScoreInlineSpritePath).Append(this.site.hab.SpaceCombatValue().ToString("N0")).AppendLine();
				}
				float num = this.site.hab.ModifiedDefenseCombatValue(false);
				float num2 = this.site.hab.ModifiedDefenseCombatValue(true);
				if (num > 0f)
				{
					if (num != num2)
					{
						stringBuilder.Append(TemplateManager.global.spaceAssaultValueInlineSpritePath).Append(Loc.T("UI.Space.Stations", new object[]
						{
							num.ToString("N0"),
							num2.ToString("N0")
						})).AppendLine();
					}
					else
					{
						stringBuilder.Append(TemplateManager.global.spaceAssaultValueInlineSpritePath).Append(num.ToString("N0")).AppendLine();
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x000D1D7F File Offset: 0x000CFF7F
		public static Sprite GetEmptyHabSiteIcon(TIHabSiteState site, TIFactionState faction)
		{
			if (faction.Prospected(site.parentBody))
			{
				return AssetCacheManager.prospectedHabSiteIcon;
			}
			if (faction.CanProspectFromShip(site.parentBody))
			{
				return AssetCacheManager.notProspectedHabSiteIcon;
			}
			return AssetCacheManager.beyondRangeHabSiteIcon;
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x000D1DB0 File Offset: 0x000CFFB0
		public void SetMarkerData()
		{
			if (this.site.hasPlannedOrOperatingBase && this.activePlayer.HasIntelOnSpaceAssetLocation(this.site.hab))
			{
				TIHabState hab = this.site.hab;
				this.habSiteMarker.sprite = hab.icon;
				this.habSiteMarkerModel.sprite = hab.icon;
				this.habSiteMarker.color = Color.white;
				for (int i = 1; i < 5; i++)
				{
					if (hab.sectors[i].active)
					{
						this.sectorImage[i - 1].sprite = hab.sectors[i].faction.factionIcon64;
						this.sectorImage[i - 1].enabled = true;
					}
					else
					{
						this.sectorImage[i - 1].enabled = false;
					}
				}
				if (!string.IsNullOrEmpty(this.site.hab.customHabIconResource))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.site.hab.customHabIconResource, this.habClassificationImage);
					this.habClassificationImage.gameObject.SetActive(true);
				}
				else
				{
					this.habClassificationImage.gameObject.SetActive(false);
				}
			}
			else
			{
				this.habSiteMarker.sprite = HabSiteController.GetEmptyHabSiteIcon(this.site, this.activePlayer);
				for (int j = 0; j < 4; j++)
				{
					this.sectorImage[j].enabled = false;
				}
				this.habClassificationImage.gameObject.SetActive(false);
			}
			this.SetCouncilorData();
			this.SetFleetData();
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x000D1F3C File Offset: 0x000D013C
		public void ToggleSurfaceModel(bool show)
		{
			if (show && !this.modelRoot.activeSelf)
			{
				this.modelRoot.SetActive(true);
				if (this.surfaceModelController == null)
				{
					this.surfaceModelController = global::UnityEngine.Object.Instantiate<GameObject>(AssetCacheManager.surfaceBasePrefab, this.modelRoot.transform).GetComponent<SurfaceBaseModelController>();
				}
				this.surfaceModelController.UpdateSurfaceModel(this.site.hab);
				this.habSiteMarkerCanvasGroup.alpha = 0f;
				this.sectorImagesCanvasGroup.alpha = 0f;
				this.habSiteMarkerModel.gameObject.SetActive(true);
				this.hoverHighlightObject.SetActive(false);
				this.hoverHighlightObject.SetActive(false);
				return;
			}
			if (!show && this.modelRoot.activeSelf)
			{
				this.modelRoot.SetActive(false);
				this.habSiteMarkerCanvasGroup.alpha = 1f;
				this.sectorImagesCanvasGroup.alpha = 1f;
				this.habSiteMarkerModel.gameObject.SetActive(false);
				this.hoverHighlightObject.SetActive(true);
				this.hoverHighlightObject.SetActive(true);
			}
		}

		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x0600269A RID: 9882 RVA: 0x000D2060 File Offset: 0x000D0260
		public float radius_gameUnits
		{
			get
			{
				SpaceObjectType objectType = this.site.parentBody.objectType;
				if (objectType - SpaceObjectType.Asteroid <= 1 || objectType == SpaceObjectType.AsteroidalMoon)
				{
					return Vector3.Distance(this.sitePosition, this.site.parentBody.controller.spaceObjectControllerTransform.position);
				}
				return this.site.parentBody.radius_gameUnits;
			}
		}

		// Token: 0x0600269B RID: 9883 RVA: 0x000D20C0 File Offset: 0x000D02C0
		public Vector3 GlobalPosition(TIDateTime time)
		{
			return this.site.ref_spaceBody.controller.spaceObjectControllerTransform.rotation * Quaternion.AngleAxis(this.site.longitude, -Vector3.up) * Quaternion.AngleAxis(this.site.latitude, -Vector3.right) * Vector3.forward * this.radius_gameUnits + (Vector3)this.site.ref_spaceBody.GetGlobalPositionAtTime(time);
		}

		// Token: 0x0600269C RID: 9884 RVA: 0x000D2155 File Offset: 0x000D0355
		private void OnBeginBombardment(BeginBombardment e)
		{
			this.InitializeForGroundFire();
		}

		// Token: 0x0600269D RID: 9885 RVA: 0x000D215D File Offset: 0x000D035D
		private void OnEndBombardment(EndBombardment e)
		{
			if (!this.site.ref_spaceBody.fleetsInOrbit.Any<TISpaceFleetState>((TISpaceFleetState x) => x.bombardmentTarget.ref_habSite == this.site))
			{
				this.initializedForGroundFire = false;
			}
			this.groundFireControllers.Clear();
		}

		// Token: 0x0600269E RID: 9886 RVA: 0x000D2194 File Offset: 0x000D0394
		private void InitializeForGroundFire()
		{
			this.groundFireControllers.Clear();
			foreach (TIHabModuleState tihabModuleState in this.site.hab.ActiveCombatModules())
			{
				this.groundFireControllers.Add(tihabModuleState, new GroundFireController
				{
					shotEffectPrefab = GameControl.assetLoader.LoadAsset<GameObject>(tihabModuleState.defenseWeapon.effectResource)
				});
			}
			this.initializedForGroundFire = true;
		}

		// Token: 0x0600269F RID: 9887 RVA: 0x000D2228 File Offset: 0x000D0428
		public void DisplayBeam(TIHabModuleState shooter, TISpaceShipState target, TIDateTime time)
		{
			if (!this.initializedForGroundFire || !this.groundFireControllers.ContainsKey(shooter))
			{
				this.InitializeForGroundFire();
			}
			if (!this.groundFireControllers.ContainsKey(shooter) || this.groundFireControllers[shooter].shotEffectInstance != null)
			{
				return;
			}
			if (this.site.ref_spaceBody.controller.modelLink.activeInHierarchy)
			{
				this.groundFireControllers[shooter].target = target;
				this.groundFireControllers[shooter].shotEffectInstance = global::UnityEngine.Object.Instantiate<GameObject>(this.groundFireControllers[shooter].shotEffectPrefab, this.GlobalPosition(time), Quaternion.identity);
				this.groundFireControllers[shooter].shotEffectInstance.transform.localScale = Vector3.one;
				this.groundFireControllers[shooter].shotEffectInstance.transform.parent = base.transform;
				this.groundFireControllers[shooter].beamController = this.groundFireControllers[shooter].shotEffectInstance.GetComponent<BeamWeaponController>();
				this.groundFireControllers[shooter].beamController.Initialize(shooter, target, time, LayerMask.NameToLayer("HurtBox"));
				LineRenderer component = this.groundFireControllers[shooter].shotEffectInstance.GetComponent<LineRenderer>();
				component.startWidth = (float)shooter.defenseWeapon.ref_laserWeapon.mirrorRadius_cm / 1000f;
				component.startWidth *= 1f + 1.22f * target.ref_fleet.bombardmentAltitude_km / 1000f;
			}
			string text = new StringBuilder("STOFireMissionDuration").Append(shooter.ID).ToString();
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddSeconds((double)Mathf.Max(1, this.gameTimeManager.currentSpeedIndex));
			string text2 = text;
			TITimeEvent.CreateNewTimeEvent(tidateTime, shooter, null, null, text2, false, false, TITimeQueueRepeatType.None, 1, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CeaseBeamFire), text, null, true, true);
		}

		// Token: 0x060026A0 RID: 9888 RVA: 0x000D242D File Offset: 0x000D062D
		private void CeaseBeamFire(TimeEventStart e)
		{
			if (e.eventObject != null)
			{
				this.CeaseBeamFire(e.eventObject.ref_habModule);
			}
		}

		// Token: 0x060026A1 RID: 9889 RVA: 0x000D2450 File Offset: 0x000D0650
		public void CeaseBeamFire(TIHabModuleState module)
		{
			if (this.groundFireControllers.ContainsKey(module) && this.groundFireControllers[module].shotEffectInstance != null)
			{
				this.groundFireControllers[module].beamController.DisableLaser();
				this.groundFireControllers[module].shotEffectInstance.SetActive(false);
				global::UnityEngine.Object.Destroy(this.groundFireControllers[module].shotEffectInstance);
			}
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x000D24C8 File Offset: 0x000D06C8
		public void OnCombatStarts(CombatStarts e)
		{
			this.ToggleSurfaceModel(false);
			foreach (TIHabModuleState tihabModuleState in this.groundFireControllers.Keys)
			{
				if (this.groundFireControllers[tihabModuleState].shotEffectInstance != null)
				{
					this.groundFireControllers[tihabModuleState].beamController.DisableLaser();
				}
			}
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x000D2550 File Offset: 0x000D0750
		private void Update()
		{
			if (this.cameraMgr.IsAltitudeChanging && this.site != null && this.site.hab != null && this.site.hasPlannedOrOperatingBase && this.activePlayer.HasIntelOnSpaceAssetLocation(this.site.hab))
			{
				if ((this.mainCam.transform.position - base.transform.position).magnitude < TemplateManager.global.distanceToViewSurfaceBases)
				{
					this.ToggleSurfaceModel(true);
					return;
				}
				this.ToggleSurfaceModel(false);
			}
		}

		// Token: 0x060026A4 RID: 9892 RVA: 0x000D25F8 File Offset: 0x000D07F8
		private void LateUpdate()
		{
			foreach (GroundFireController groundFireController in this.groundFireControllers.Values)
			{
				if (groundFireController.shotEffectInstance != null && groundFireController.shotEffectInstance.activeInHierarchy && groundFireController.target != null && groundFireController.target.fleet != null)
				{
					groundFireController.shotEffectInstance.transform.position = base.transform.position;
				}
			}
		}

		// Token: 0x04001C97 RID: 7319
		public Vector3 sitePosition;

		// Token: 0x04001C98 RID: 7320
		public Canvas primaryCanvas;

		// Token: 0x04001C99 RID: 7321
		public Image habSiteMarker;

		// Token: 0x04001C9A RID: 7322
		public Image habSiteMarkerModel;

		// Token: 0x04001C9B RID: 7323
		public TooltipTrigger habSiteTooltip;

		// Token: 0x04001C9C RID: 7324
		public Animator selectionAnim;

		// Token: 0x04001C9D RID: 7325
		public SpriteRenderer selectionRenderer;

		// Token: 0x04001C9E RID: 7326
		private RuntimeAnimatorController selectionAnimatorController;

		// Token: 0x04001C9F RID: 7327
		public Image[] sectorImage;

		// Token: 0x04001CA0 RID: 7328
		public CanvasGroup sectorImagesCanvasGroup;

		// Token: 0x04001CA1 RID: 7329
		public CanvasGroup habSiteMarkerCanvasGroup;

		// Token: 0x04001CA2 RID: 7330
		private bool initialized;

		// Token: 0x04001CA3 RID: 7331
		private TIFactionState activePlayer;

		// Token: 0x04001CA4 RID: 7332
		public Image habClassificationImage;

		// Token: 0x04001CA5 RID: 7333
		public ListManagerBase myCouncilorGrid;

		// Token: 0x04001CA6 RID: 7334
		public ListManagerBase enemyCouncilorGrid;

		// Token: 0x04001CA7 RID: 7335
		public ListManagerBase landedFleetGrid;

		// Token: 0x04001CA8 RID: 7336
		private List<CouncilorView> myCouncilors;

		// Token: 0x04001CA9 RID: 7337
		private List<CouncilorView> enemyCouncilors;

		// Token: 0x04001CAA RID: 7338
		public GameObject particleEffectsContainer;

		// Token: 0x04001CAB RID: 7339
		public ParticleSystem launchFX;

		// Token: 0x04001CAC RID: 7340
		public ParticleSystem explosionFX;

		// Token: 0x04001CAD RID: 7341
		public GameObject modelRoot;

		// Token: 0x04001CAE RID: 7342
		public GameObject selectionHighlightObject;

		// Token: 0x04001CAF RID: 7343
		public GameObject hoverHighlightObject;

		// Token: 0x04001CB0 RID: 7344
		public SurfaceBaseModelController surfaceModelController;

		// Token: 0x04001CB1 RID: 7345
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x04001CB2 RID: 7346
		private Dictionary<TIHabModuleState, GroundFireController> groundFireControllers = new Dictionary<TIHabModuleState, GroundFireController>();

		// Token: 0x04001CB3 RID: 7347
		private GameTimeManager gameTimeManager;

		// Token: 0x04001CB4 RID: 7348
		private Camera mainCam;

		// Token: 0x04001CB5 RID: 7349
		private CameraManager cameraMgr;

		// Token: 0x04001CB6 RID: 7350
		private bool initializedForGroundFire;
	}
}
