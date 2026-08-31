using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Poly2Tri;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000561 RID: 1377
	public class RegionController : MonoBehaviour
	{
		// Token: 0x17000516 RID: 1302
		// (get) Token: 0x06002464 RID: 9316 RVA: 0x000C1185 File Offset: 0x000BF385
		// (set) Token: 0x06002465 RID: 9317 RVA: 0x000C118D File Offset: 0x000BF38D
		public TIRegionState region { get; protected set; }

		// Token: 0x06002466 RID: 9318 RVA: 0x000C1198 File Offset: 0x000BF398
		public void Initialize(TIRegionState gamestate, NationController nationVis)
		{
			if (SceneManager.GetActiveScene().name == "RegionVisualizerTestScene")
			{
				if (!this.is3D)
				{
					GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(base.gameObject);
					gameObject.GetComponent<RegionController>().is3D = true;
					gameObject.GetComponent<RegionController>().Initialize(gamestate, nationVis);
				}
			}
			else
			{
				this.is3D = true;
			}
			this.region = gamestate;
			base.gameObject.name = this.region.mapRegionTemplate.dataName;
			this.mapVisualizer = nationVis.mapVisualizer;
			this.nationVisualizer = nationVis;
			string text = this.region.mapRegionTemplate.dataName.Remove(0, 4);
			this.outline = this.mapVisualizer.GetOutlineData(text);
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.OnRegionSelected), null, this.region, true, false);
			GameControl.eventManager.AddListener<RegionControlChanged>(new EventManager.EventDelegate<RegionControlChanged>(this.ChangeRegionOwner), null, this.region, false, false);
			GameControl.eventManager.AddListener<RegionFlashEvent>(new EventManager.EventDelegate<RegionFlashEvent>(this.FlashRegion), null, this.region, true, false);
			GameControl.eventManager.AddListener<OccupationStatusChange>(new EventManager.EventDelegate<OccupationStatusChange>(this.OnOccupationStatusChange), null, this.region, true, false);
			GameControl.eventManager.AddListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnRegionDeselected), null, this.region, true, false);
			this.CreateVisualizers();
		}

		// Token: 0x17000517 RID: 1303
		// (get) Token: 0x06002467 RID: 9319 RVA: 0x000C12F2 File Offset: 0x000BF4F2
		private static MapColorationStyle mapColorationStyle
		{
			get
			{
				return GeneralControlsController.mapColorationStyle;
			}
		}

		// Token: 0x06002468 RID: 9320 RVA: 0x000C12FC File Offset: 0x000BF4FC
		protected Color GetRegionFillColor()
		{
			TINationState nation = this.nationVisualizer.nationState;
			switch (RegionController.mapColorationStyle)
			{
			default:
				return nation.template.color;
			case MapColorationStyle.byTerrain:
				return Colors.TransparentWhite;
			case MapColorationStyle.byExecutiveFaction:
				if (nation.executiveFaction != null)
				{
					Color color = nation.executiveFaction.template.color;
					return new Color(color.r, color.g, color.b, 0.5f);
				}
				return Colors.TransparentWhite;
			case MapColorationStyle.byFactionPopularity:
			{
				TIFactionIdeologyTemplate mostPopularIdeology = nation.GetMostPopularIdeology(true);
				float mostPopularFactionValue = nation.GetMostPopularFactionValue(true);
				if (!mostPopularIdeology.human || mostPopularIdeology.undecided)
				{
					return Colors.TransparentWhite;
				}
				Color color2 = TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(mostPopularIdeology).template.color;
				if (mostPopularFactionValue >= 0.5f)
				{
					return new Color(color2.r, color2.g, color2.b, 0.6f);
				}
				return new Color(color2.r, color2.g, color2.b, 0.5f);
			}
			case MapColorationStyle.byPopulation:
				if (RegionController.cachedFrame != TIFrameCounter.FrameCount)
				{
					RegionController.cachedMaxPopulation = Mathf.Log10(GameStateManager.AllRegions().Max<TIRegionState>((TIRegionState region) => region.population));
					RegionController.cachedMinPopulation = Mathf.Log10(GameStateManager.AllRegions().TopPercentage<TIRegionState, float>((TIRegionState x) => x.population, 0.9f).Min<TIRegionState>((TIRegionState region) => region.population));
					RegionController.cachedFrame = TIFrameCounter.FrameCount;
				}
				return Color.Lerp(Color.red, Color.green, (Mathf.Log10(this.region.population) - RegionController.cachedMinPopulation) / (RegionController.cachedMaxPopulation - RegionController.cachedMinPopulation));
			case MapColorationStyle.byInvestmentPoints:
				if (RegionController.cachedFrame != TIFrameCounter.FrameCount)
				{
					RegionController.cachedMaxIPs = GameStateManager.AllNations().Max<TINationState>((TINationState nation) => nation.BaseInvestmentPoints_month());
					RegionController.cachedFrame = TIFrameCounter.FrameCount;
				}
				return Color.Lerp(Color.red, Color.green, nation.BaseInvestmentPoints_month() / RegionController.cachedMaxIPs);
			case MapColorationStyle.byPerCapitaGDP:
				if (RegionController.cachedFrame != TIFrameCounter.FrameCount)
				{
					RegionController.cachedMinPerCapitaGDP = Mathf.Log10(GameStateManager.AllRegions().Min<TIRegionState>((TIRegionState region) => (float)region.regionalPerCapitaGDP));
					RegionController.cachedMaxPerCapitaGDP = Mathf.Log10(GameStateManager.AllRegions().Max<TIRegionState>((TIRegionState region) => (float)region.regionalPerCapitaGDP));
					RegionController.cachedFrame = TIFrameCounter.FrameCount;
				}
				return Color.Lerp(Color.red, Color.green, (Mathf.Log10((float)this.region.regionalPerCapitaGDP) - RegionController.cachedMinPerCapitaGDP) / (RegionController.cachedMaxPerCapitaGDP - RegionController.cachedMinPerCapitaGDP));
			case MapColorationStyle.byControlPoints:
				return Color.Lerp(Color.red, Color.green, (float)nation.controlPoints.Count / 6f);
			case MapColorationStyle.byMilitaryTechLevel:
				if (RegionController.cachedFrame != TIFrameCounter.FrameCount)
				{
					RegionController.cachedMaxMilitaryTechLevel = GameStateManager.AllNations().Max<TINationState>((TINationState nation) => nation.militaryTechLevel);
					RegionController.cachedFrame = TIFrameCounter.FrameCount;
				}
				return Color.Lerp(Color.red, Color.green, (nation.militaryTechLevel - 2.8f) / (RegionController.cachedMaxMilitaryTechLevel - 2.8f));
			case MapColorationStyle.byBoostIncome:
				if (RegionController.cachedFrame != TIFrameCounter.FrameCount)
				{
					RegionController.cachedMaxBoostIncome = GameStateManager.AllNations().Max<TINationState>((TINationState nation) => nation.boostIncome_year_dekatons);
					if (RegionController.cachedMaxBoostIncome == 0f)
					{
						RegionController.cachedMaxBoostIncome = 0.1f;
					}
					RegionController.cachedFrame = TIFrameCounter.FrameCount;
				}
				return Color.Lerp(Color.red, Color.green, nation.boostIncome_year_dekatons / RegionController.cachedMaxBoostIncome);
			case MapColorationStyle.byUnrest:
				return Color.Lerp(Color.green, Color.red, nation.unrest / 9f);
			case MapColorationStyle.byDemocracy:
				return Color.Lerp(Color.red, Color.green, nation.democracy / 10f);
			case MapColorationStyle.bySustainability:
			{
				float sustainability = this.region.nation.sustainability;
				if (sustainability <= 0f)
				{
					return Color.green;
				}
				if (sustainability < 0.33333334f)
				{
					return Color.blue;
				}
				if (sustainability < 0.6666667f)
				{
					return Color.yellow;
				}
				if (sustainability <= 1f)
				{
					return Colors.Orange;
				}
				return Color.red;
			}
			case MapColorationStyle.byXenoformingLevel:
				if (this.region.xenoforming.VisibleToFaction(GameControl.control.activePlayer))
				{
					return Color.Lerp(Colors.Blue, Color.red, this.region.xenoforming.xenoformingLevel / TIRegionXenoformingState.stage3Xenoforming);
				}
				return Colors.TransparentWhite;
			case MapColorationStyle.byIsFederatedNation:
				if (nation.inFederation)
				{
					return nation.federation.leadNation.template.color + new Color(0f, 0f, 0f, 1f);
				}
				return Colors.TransparentWhite;
			case MapColorationStyle.bySelectedNationAlliances:
				if (GeneralControlsController.UIOtherSelectedState == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == nation)
				{
					return nation.template.color;
				}
				if (nation.allies.Contains(GeneralControlsController.UIOtherSelectedState.ref_nation))
				{
					return Color.green;
				}
				if (nation.rivals.Contains(GeneralControlsController.UIOtherSelectedState.ref_nation))
				{
					return Colors.Orange;
				}
				if (nation.enemies.Contains(GeneralControlsController.UIOtherSelectedState.ref_nation))
				{
					return Color.red;
				}
				return Colors.TransparentWhite;
			case MapColorationStyle.bySelectedNationClaims:
				if (GeneralControlsController.UIOtherSelectedState == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == nation)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation.hostileClaims.Contains(this.region))
				{
					return Color.red;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation.claims.Contains(this.region) && GeneralControlsController.UIOtherSelectedState.ref_nation.HostileClaimDueToDemocracy(this.region.nation))
				{
					return Colors.Orange;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation.claims.Contains(this.region))
				{
					return Color.green;
				}
				return Colors.TransparentWhite;
			case MapColorationStyle.bySelectedNationFederation:
				if (GeneralControlsController.UIOtherSelectedState == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == nation && !nation.inFederation)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation.inFederation)
				{
					if (nation.inFederation && nation.federation == GeneralControlsController.UIOtherSelectedState.ref_nation.federation)
					{
						return GeneralControlsController.UIOtherSelectedState.ref_nation.federation.leadNation.template.color + new Color(0f, 0f, 0f, 1f);
					}
					if (GeneralControlsController.UIOtherSelectedState.ref_nation.federation.CanAddNation(nation))
					{
						return Color.green;
					}
					if (!GeneralControlsController.UIOtherSelectedState.ref_nation.federation.MemberClaims(false).Any<TIRegionState>((TIRegionState x) => x.nation == nation))
					{
						if (!nation.nonHostileClaims.Any<TIRegionState>((TIRegionState x) => GeneralControlsController.UIOtherSelectedState.ref_nation.federation.members.Contains(x.nation)))
						{
							goto IL_08C7;
						}
					}
					return Color.yellow;
				}
				IL_08C7:
				return Colors.TransparentWhite;
			case MapColorationStyle.bySelectedNationCanJoinFederation:
				if (GeneralControlsController.UIOtherSelectedState == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == null)
				{
					return nation.template.color;
				}
				if (GeneralControlsController.UIOtherSelectedState.ref_nation == nation)
				{
					return nation.template.color;
				}
				if (!GeneralControlsController.UIOtherSelectedState.ref_nation.inFederation && nation.inFederation && nation.federation.CanAddNation(GeneralControlsController.UIOtherSelectedState.ref_nation))
				{
					return nation.federation.leadNation.template.color + new Color(0f, 0f, 0f, 1f);
				}
				return Colors.TransparentWhite;
			}
		}

		// Token: 0x06002469 RID: 9321 RVA: 0x000C1CB8 File Offset: 0x000BFEB8
		private void CreateVisualizers()
		{
			if (this.outline != null && this.defaultMaterial != null)
			{
				if (this.is3D)
				{
					this.Update3DShapeWithQuality(null);
				}
				else
				{
					this.regionOutlines = RegionUtility.CreateSegmentedPolysAsVectorLine(this.outline, 1f, 1f, null, null);
					List<Polygon> list = RegionUtility.VectorLineListToPolygonList(this.regionOutlines);
					foreach (Polygon polygon in list)
					{
						RegionUtility.TriangulatePolygon(polygon, null);
					}
					RegionUtility.Draw2DRegionPolyOutlines(this.regionOutlines, -0.0035f, base.transform);
					this.regionSurfacePoints = RegionUtility.MeshFromPolygon(list, false, -0.025f);
					int num = 0;
					foreach (Vector3[] array in this.regionSurfacePoints)
					{
						GameObject gameObject = RegionUtility.CreateSurface(this.region.templateName + num++.ToString() + "_surf", array, this.defaultMaterial);
						gameObject.transform.SetParent(base.transform, false);
						gameObject.transform.localPosition = new Vector3(0f, 0f, -0.0025f);
						gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
						this.regionSurfaces.Add(gameObject);
						this.surfaceRenderers.Add(gameObject.GetComponent<Renderer>());
						this.surfaceColliders.Add(gameObject.GetComponent<MeshCollider>());
					}
				}
				Color regionFillColor = this.GetRegionFillColor();
				if (regionFillColor.a > 0f)
				{
					this.SetBaselineTexture(regionFillColor);
				}
				this.RestoreRegionTexture();
			}
		}

		// Token: 0x0600246A RID: 9322 RVA: 0x000C1EB0 File Offset: 0x000C00B0
		public void ChangeRegionOwner(RegionControlChanged e)
		{
			NationController nation = this.mapVisualizer.GetNation(e.oldNation.templateName);
			NationController nation2 = this.mapVisualizer.GetNation(e.newNation.templateName);
			nation.regionVisualizers.Remove(this);
			nation2.regionVisualizers.Add(this);
			this.nationVisualizer = nation2;
			base.transform.SetParent(this.nationVisualizer.transform);
			this.SetBaselineTexture(this.GetRegionFillColor());
		}

		// Token: 0x0600246B RID: 9323 RVA: 0x000C1F2B File Offset: 0x000C012B
		public bool GetCouncilorLocation(out Vector3 location)
		{
			if (this.tokenPositions.TryGetValue("Council", out location))
			{
				return true;
			}
			location = Vector3.zero;
			return false;
		}

		// Token: 0x0600246C RID: 9324 RVA: 0x000C1F4E File Offset: 0x000C014E
		public bool GetArmyLocation(out Vector3 location)
		{
			if (this.tokenPositions.TryGetValue("Army", out location))
			{
				return true;
			}
			location = Vector3.zero;
			return false;
		}

		// Token: 0x0600246D RID: 9325 RVA: 0x000C1F71 File Offset: 0x000C0171
		public bool GetSeaLocation(out Vector3 location)
		{
			if (this.tokenPositions.TryGetValue("Sea", out location))
			{
				return true;
			}
			location = Vector3.zero;
			return false;
		}

		// Token: 0x0600246E RID: 9326 RVA: 0x000C1F94 File Offset: 0x000C0194
		public List<MarkerController> GetMarkers(List<MarkerType> filterForTypes)
		{
			List<MarkerController> list = new List<MarkerController>();
			Func<MarkerController, bool> <>9__0;
			foreach (MarkerContainerController markerContainerController in this.markerContainers)
			{
				if (filterForTypes == null || filterForTypes.Count == 0)
				{
					list.AddRange(markerContainerController.GetMarkers());
				}
				else
				{
					List<MarkerController> list2 = list;
					IEnumerable<MarkerController> markers = markerContainerController.GetMarkers();
					Func<MarkerController, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (MarkerController x) => filterForTypes.Contains(x.markerType));
					}
					list2.AddRange(markers.Where<MarkerController>(func));
				}
			}
			return list;
		}

		// Token: 0x0600246F RID: 9327 RVA: 0x000C204C File Offset: 0x000C024C
		public List<IMarkerControl> GetIMarkerControllers()
		{
			return this.iMarkerControllers;
		}

		// Token: 0x06002470 RID: 9328 RVA: 0x000C2054 File Offset: 0x000C0254
		public T GetIMarkerController<T>() where T : IMarkerControl
		{
			IMarkerControl markerControl = this.iMarkerControllers.FirstOrDefault<IMarkerControl>((IMarkerControl x) => x is T);
			if (markerControl == null)
			{
				return default(T);
			}
			return (T)((object)markerControl);
		}

		// Token: 0x17000518 RID: 1304
		// (get) Token: 0x06002471 RID: 9329 RVA: 0x000C209F File Offset: 0x000C029F
		public ArmyMarkerController ArmyMarkerController
		{
			get
			{
				return this.GetIMarkerController<ArmyMarkerController>();
			}
		}

		// Token: 0x06002472 RID: 9330 RVA: 0x000C20A8 File Offset: 0x000C02A8
		private string RegionTooltip(TooltipTrigger tip)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Global.2IC", new object[]
			{
				this.region.displayName,
				this.region.nation.displayName
			})).Append(" ").AppendLine(this.region.IconString(GameControl.control.activePlayer));
			switch (GeneralControlsController.mapColorationStyle)
			{
			case MapColorationStyle.byNation:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.populationInlineSpritePath,
					this.region.population.ToString("N0")
				}));
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					Loc.T("UI.Nation.GDP"),
					this.region.GDPstring
				}));
				goto IL_096F;
			case MapColorationStyle.byTerrain:
			case MapColorationStyle.bySelectedNationAlliances:
				goto IL_096F;
			case MapColorationStyle.byExecutiveFaction:
			case MapColorationStyle.byControlPoints:
			{
				using (IEnumerator<TIControlPoint> enumerator = this.region.nation.controlPoints.OrderByDescending<TIControlPoint, int>((TIControlPoint x) => x.positionInNation).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIControlPoint ticontrolPoint = enumerator.Current;
						StringBuilder stringBuilder2 = stringBuilder;
						string text = "UI.Region.2C";
						object[] array = new object[2];
						array[0] = ticontrolPoint.controlPointTypeDisplayName;
						int num = 1;
						TIFactionState faction = ticontrolPoint.faction;
						array[num] = ((faction != null) ? faction.displayNameCapitalizedWithColor : null) ?? "";
						stringBuilder2.AppendLine(Loc.T(text, array));
					}
					goto IL_096F;
				}
				break;
			}
			case MapColorationStyle.byFactionPopularity:
				break;
			case MapColorationStyle.byPopulation:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.populationInlineSpritePath,
					this.region.population.ToString("N0")
				}));
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					Loc.T("UI.Region.PopGrowth"),
					this.region.annualPopulationGrowth.ToPercent("P2")
				}));
				goto IL_096F;
			case MapColorationStyle.byInvestmentPoints:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.investmentInlineSpritePath,
					this.region.nation.BaseInvestmentPoints_month().ToString("N2")
				}));
				goto IL_096F;
			case MapColorationStyle.byPerCapitaGDP:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.perCapitaGDPInlineSpritePath,
					this.region.perCapitaGDPstr
				}));
				goto IL_096F;
			case MapColorationStyle.byMilitaryTechLevel:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.militaryTechInlineSpritePath,
					TIUtilities.FormatSmallNumber(this.region.nation.militaryTechLevel, 7, 0, true, false)
				}));
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					Loc.T("UI.Region.BasicDefenses"),
					TIUtilities.FormatSmallNumber(this.region.GenericLocalForcesDefenseLevel(true), 7, 0, true, false)
				}));
				goto IL_096F;
			case MapColorationStyle.byBoostIncome:
				if (this.region.boostPerYear_dekatons > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
					{
						TemplateManager.global.boostInlineSpritePath,
						TIUtilities.FormatSmallNumber(this.region.boostPerYear_dekatons, 7, 0, true, false)
					}));
				}
				if (this.region.missionControl > 0)
				{
					stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
					{
						TemplateManager.global.missionControlInlineSpritePath,
						this.region.missionControl.ToString("N0")
					}));
				}
				if (this.region.numSTOFighters > 0)
				{
					stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
					{
						TemplateManager.global.STO_InlineSpritePath,
						Loc.T("UI.Region.Slash", new object[]
						{
							this.region.availableSTOFighters.ToString("N0"),
							this.region.numSTOFighters.ToString("N0")
						})
					}));
					goto IL_096F;
				}
				goto IL_096F;
			case MapColorationStyle.byUnrest:
			{
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.unrestInlineSpritePath,
					this.region.nation.GetUnrestDescriptiveStringAndValue(1)
				}));
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.cohesionInlineSpritePath,
					this.region.nation.GetCohesionDescriptiveStringAndValue(1)
				}));
				List<TINationState> list = this.region.SecessionCandidates();
				if (list.Count > 0)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					string text2 = "UI.Region.SecessionCandidates";
					object[] array2 = new object[1];
					array2[0] = TIUtilities.ConstructTextList(list.ConvertAll<TIGameState>((TINationState x) => x.ref_gameState), true, false);
					stringBuilder3.AppendLine(Loc.T(text2, array2));
					goto IL_096F;
				}
				goto IL_096F;
			}
			case MapColorationStyle.byDemocracy:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.democracyInlineSpritePath,
					this.region.nation.GetDemocracyDescriptiveStringAndValue(1)
				}));
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					TemplateManager.global.educationInlineSpritePath,
					this.region.nation.GetEducationDescriptiveStringAndValue(1)
				}));
				goto IL_096F;
			case MapColorationStyle.bySustainability:
				stringBuilder.AppendLine(Loc.T("UI.Region.2C", new object[]
				{
					this.region.nation.SustainabilityIconInlinePath(),
					TINationState.SustainabilityValueForDisplay(this.region.nation.sustainability)
				}));
				goto IL_096F;
			case MapColorationStyle.byXenoformingLevel:
				if (this.region.xenoforming.VisibleToFaction(GameControl.control.activePlayer))
				{
					stringBuilder.AppendLine(Loc.T(this.region.xenoforming.severityDescription));
					goto IL_096F;
				}
				goto IL_096F;
			case MapColorationStyle.byIsFederatedNation:
			case MapColorationStyle.bySelectedNationFederation:
			case MapColorationStyle.bySelectedNationCanJoinFederation:
			{
				if (this.region.nation.inFederation)
				{
					stringBuilder.AppendLine(Loc.T((this.region.nation.federation.leadNation == this.region.nation) ? "UI.Nation.FederationTooltipLeader" : "UI.Nation.FederationTooltipMember", new object[]
					{
						this.region.nation.displayNameWithArticleCapitalized,
						this.region.nation.federation.displayNameWithArticle,
						this.region.nation.federation.leadNation.displayNameWithArticle
					}));
					goto IL_096F;
				}
				List<TINationState> list2 = new List<TINationState>();
				foreach (TINationState tinationState in GameStateManager.AllExtantNations())
				{
					if (this.region.nation.CanFormFederation(tinationState))
					{
						list2.Add(tinationState);
					}
				}
				if (list2.Count > 0)
				{
					StringBuilder stringBuilder4 = stringBuilder;
					string text3 = "UI.Region.FormFederationCandidates";
					object[] array3 = new object[1];
					array3[0] = TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TINationState x) => x.ref_gameState), true, false);
					stringBuilder4.AppendLine(Loc.T(text3, array3));
					goto IL_096F;
				}
				List<TIFederationState> list3 = new List<TIFederationState>();
				foreach (TIFederationState tifederationState in from x in GameStateManager.IterateByClass<TIFederationState>(false)
					where x.members.Count > 0
					select x)
				{
					if (tifederationState.CanAddNation(this.region.nation))
					{
						list3.Add(tifederationState);
					}
				}
				if (list3.Count > 0)
				{
					StringBuilder stringBuilder5 = stringBuilder;
					string text4 = "UI.Region.JoinFederationCandidates";
					object[] array4 = new object[1];
					array4[0] = TIUtilities.ConstructTextList(list3.ConvertAll<TIGameState>((TIFederationState x) => x.ref_gameState), true, false);
					stringBuilder5.AppendLine(Loc.T(text4, array4));
					goto IL_096F;
				}
				goto IL_096F;
			}
			case MapColorationStyle.bySelectedNationClaims:
			{
				List<TINationState> breakawayCapitals2 = this.region.SecessionCandidates();
				stringBuilder.AppendLine(Loc.T("UI.Region.ClaimedBy", new object[] { TIUtilities.ConstructTextList(this.region.NationsWithClaim(false, true, true, false).Select<TINationState, string>(delegate(TINationState x)
				{
					if (breakawayCapitals2.Contains(x))
					{
						return new StringBuilder(x.displayName).Append(TIGlobalConfig.globalConfig.capitalRegionInlineSpritePath).ToString();
					}
					if (!x.ClaimWillBeHostile(this.region, false))
					{
						return x.displayName;
					}
					return new StringBuilder(TIUtilities.RedLine(x.displayName)).Append(TIGlobalConfig.globalConfig.unrestInlineSpritePath).ToString();
				}).ToList<string>(), true, false) }));
				goto IL_096F;
			}
			default:
				goto IL_096F;
			}
			stringBuilder.AppendLine(Loc.T("UI.Nation.PublicOpinionHeader"));
			foreach (TIFactionState tifactionState in from x in GameStateManager.AllHumanFactions()
				orderby x.isActivePlayer descending, this.region.nation.publicOpinion[x.ideology.ideology] descending
				select x)
			{
				stringBuilder.AppendLine(NationInfoController.BuildPublicOpinionLine(this.region.nation, tifactionState.ideology.ideology, true));
			}
			stringBuilder.AppendLine(NationInfoController.BuildPublicOpinionLine(this.region.nation, FactionIdeology.Undecided, true));
			IL_096F:
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TIRegionState)) && !GeneralControlsController.CurrentValidTarget(this.region))
				{
					TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
					if (uiselectedAssetState != null && uiselectedAssetState.isCouncilorState)
					{
						TIMissionTargeting timissionTargeting = GeneralControlsController.UITargetingMode as TIMissionTargeting;
						if (timissionTargeting != null)
						{
							stringBuilder.AppendLine();
							TIMissionTemplate missionTemplate = timissionTargeting.missionTemplate;
							stringBuilder.AppendLine(MarkerController.BuildInvalidTargetTooltip(missionTemplate.target.ValidateSingleTarget(missionTemplate, uiselectedAssetState.ref_councilor, this.region)));
						}
					}
				}
				else if (GeneralControlsController.CurrentlyTargetingStateType(typeof(TINationState)) && !GeneralControlsController.CurrentValidTarget(this.region.nation))
				{
					TIGameState uiselectedAssetState2 = GeneralControlsController.UISelectedAssetState;
					if (uiselectedAssetState2 != null && uiselectedAssetState2.isCouncilorState)
					{
						TIMissionTargeting timissionTargeting2 = GeneralControlsController.UITargetingMode as TIMissionTargeting;
						if (timissionTargeting2 != null)
						{
							stringBuilder.AppendLine();
							TIMissionTemplate missionTemplate2 = timissionTargeting2.missionTemplate;
							stringBuilder.AppendLine(MarkerController.BuildInvalidTargetTooltip(missionTemplate2.target.ValidateSingleTarget(missionTemplate2, uiselectedAssetState2.ref_councilor, this.region.nation)));
						}
					}
				}
			}
			else
			{
				TIGameState uiselectedAssetState3 = GeneralControlsController.UISelectedAssetState;
				if (uiselectedAssetState3 != null && uiselectedAssetState3.isArmyState)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Army.RightClickBehavior"));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06002473 RID: 9331 RVA: 0x000C2BA8 File Offset: 0x000C0DA8
		public void Update3DShapeWithQuality(float? quality = null)
		{
			if (quality != null)
			{
				RegionUtility.segmentationQuality = quality.Value;
			}
			float num = Time.realtimeSinceStartup;
			if (this.outline.regionShapes != null)
			{
				this.regionShapes = RegionUtility.ConvertVector3List(this.outline.regionShapes);
			}
			else
			{
				this.regionShapes = RegionUtility.CreateSegmentedPolysAsVector3(this.outline, null, null);
			}
			RegionController.timeSegmentingCurves += Time.realtimeSinceStartup - num;
			num = Time.realtimeSinceStartup;
			this.polyLatLons = RegionUtility.VectorListToPolygonList(this.regionShapes, this.region.templateName);
			RegionController.timePolyConvert += Time.realtimeSinceStartup - num;
			num = Time.realtimeSinceStartup;
			if (this.outline.regionSurfacePoints != null)
			{
				this.regionSurfacePoints = RegionUtility.ConvertVector3Array(this.outline.regionSurfacePoints);
			}
			else
			{
				foreach (Polygon polygon in this.polyLatLons)
				{
					RegionUtility.TriangulatePolygon(polygon, null);
				}
				this.regionSurfacePoints = RegionUtility.MeshFromPolygon(this.polyLatLons, true, -0.0025f);
			}
			RegionController.timeTriangulate += Time.realtimeSinceStartup - num;
			RegionController.timeDisplayOutline += Time.realtimeSinceStartup - num;
			num = Time.realtimeSinceStartup;
			if (this.polyLatLons.Count > 1)
			{
				this.surfaceContainer = this.NewContainer("Surface Container");
			}
			int num2 = 0;
			foreach (Vector3[] array in this.regionSurfacePoints)
			{
				GameObject gameObject = RegionUtility.CreateSurface(this.region.templateName + num2++.ToString() + "_surf", array, this.defaultMaterial);
				gameObject.transform.SetParent((this.polyLatLons.Count > 1) ? this.surfaceContainer : base.transform, false);
				gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
				gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
				gameObject.AddComponent<MeshCollider>();
				RegionClickHandler regionClickHandler = gameObject.AddComponent<RegionClickHandler>();
				this.regionTooltip = gameObject.AddComponent<TooltipTrigger>();
				this.regionTooltip.worldSpace = true;
				TooltipStyle tooltipStyle = Resources.Load<TooltipStyle>("TITooltipStyleSimple");
				this.regionTooltip.tooltipStyle = tooltipStyle;
				this.regionTooltip.neverRotate = false;
				this.regionTooltip.tipPosition = TipPosition.MouseTopLeftCorner;
				this.regionTooltip.minTextWidth = 100;
				this.regionTooltip.maxTextWidth = 400;
				this.mapVisualizer.regionTooltips.Add(this.regionTooltip);
				this.regionTooltip.SetDelegate("BodyText", () => this.RegionTooltip(this.regionTooltip));
				gameObject.transform.SetLayer(LayerMask.NameToLayer("Earth Regions"), true);
				regionClickHandler.owner = this;
				gameObject.AddComponent<RegionMeshBorderEffect>();
				this.regionSurfaces.Add(gameObject);
				this.surfaceRenderers.Add(gameObject.GetComponent<Renderer>());
				this.surfaceColliders.Add(gameObject.GetComponent<MeshCollider>());
			}
			RegionController.timeDisplayMesh += Time.realtimeSinceStartup - num;
			this.CreateLabelPositions();
			this.scalingVector = this.ComputeScalingVector();
		}

		// Token: 0x06002474 RID: 9332 RVA: 0x000C2F40 File Offset: 0x000C1140
		private Transform NewContainer(string name)
		{
			Transform transform = new GameObject(name).transform;
			transform.position = Vector3.zero;
			transform.SetParent(base.transform, false);
			return transform;
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x000C2F68 File Offset: 0x000C1168
		private void CreateLabelPositions()
		{
			IEnumerable<TIMapGroupVisualizerTemplate> enumerable = TemplateManager.IterateByClass<TIMapGroupVisualizerTemplate>(true);
			if (this.outline.labelPositions.Count > 0)
			{
				this.tokenContainer = this.NewContainer("Token Container");
			}
			foreach (LabelPosition labelPosition in this.outline.labelPositions)
			{
				Transform transform = new GameObject(labelPosition.labelName).transform;
				transform.SetParent(this.tokenContainer, false);
				CurvedPolyPoint curvedPolyPoint = labelPosition.labelPosition;
				double num = (double)curvedPolyPoint.x;
				curvedPolyPoint = labelPosition.labelPosition;
				Vector3 vector = RegionUtility.ThreeDimFromTwoDimCartesian(num, (double)(-(double)curvedPolyPoint.y), 20.005f);
				this.tokenPositions[labelPosition.labelName] = vector;
				transform.localPosition = vector;
				transform.localRotation = Quaternion.LookRotation(-vector);
				if (this.mapVisualizer.markerContainerPrefab != null)
				{
					GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.mapVisualizer.markerContainerPrefab, transform, false);
					MarkerContainerController component = gameObject.GetComponent<MarkerContainerController>();
					TIMapGroupVisualizerTemplate timapGroupVisualizerTemplate = null;
					foreach (TIMapGroupVisualizerTemplate timapGroupVisualizerTemplate2 in enumerable)
					{
						if (timapGroupVisualizerTemplate2.mapGroupLabel == labelPosition.labelName)
						{
							timapGroupVisualizerTemplate = timapGroupVisualizerTemplate2;
							break;
						}
					}
					if (timapGroupVisualizerTemplate != null)
					{
						gameObject.name = timapGroupVisualizerTemplate.dataName;
						Component component2 = gameObject.AddComponent(timapGroupVisualizerTemplate.mapGroupControlType);
						component.transform.localScale = Vector3.one * timapGroupVisualizerTemplate.groupScale;
						component.InitializeWithRegionInfo(this, timapGroupVisualizerTemplate);
						IMarkerControl markerControl = component2 as IMarkerControl;
						if (markerControl != null)
						{
							markerControl.InitializeWithRegion(this, component);
							this.iMarkerControllers.Add(markerControl);
						}
						this.markerContainers.Add(component);
					}
				}
			}
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x000C3178 File Offset: 0x000C1378
		public void MouseUp()
		{
			World.Active.GetExistingManager<SpaceObjectSelection>().BlockThisFrame = true;
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
			if (World.Active.GetExistingManager<GameTimeManager>().currentSpeedIndex > 0)
			{
				TIUtilities.GotoGameState(this.region, false, true, true, true, false, -1f);
				return;
			}
			TIUtilities.GotoGameState(this.region, false, true, true, true, false, -1f);
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x000C31E0 File Offset: 0x000C13E0
		public void MouseOver()
		{
			if (!this.mouseOver)
			{
				this.mouseOver = true;
				this.SetHighlightTexture(this.nationVisualizer.nationState.template.color);
				if (GeneralControlsController.UITargetingMode != null)
				{
					IList<TIGameState> getPossibleTargets = GeneralControlsController.UITargetingMode.GetPossibleTargets;
					if (getPossibleTargets == null || !getPossibleTargets.Contains(this.region.nation.ref_gameState))
					{
						IList<TIGameState> getPossibleTargets2 = GeneralControlsController.UITargetingMode.GetPossibleTargets;
						if (getPossibleTargets2 == null || !getPossibleTargets2.Contains(this.region.ref_gameState))
						{
							TIInputManager.SetCursor(TIInputManager.targetCursor, true);
							return;
						}
					}
					TIInputManager.SetCursor(TIInputManager.targetCursorValid, true);
					return;
				}
			}
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x000C3283 File Offset: 0x000C1483
		public void MouseExit()
		{
			if (this.mouseOver)
			{
				this.mouseOver = false;
				this.RestoreRegionTexture();
				if (GeneralControlsController.UITargetingMode != null)
				{
					TIInputManager.SetCursor(TIInputManager.targetCursor, true);
				}
			}
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x000C32AC File Offset: 0x000C14AC
		private bool RegionContainsPoint(PolygonPoint point)
		{
			using (List<Polygon>.Enumerator enumerator = this.polyLatLons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Contains(point))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x000C3308 File Offset: 0x000C1508
		private Vector3 ComputeScalingVector()
		{
			this.boundRect = new Rect2D
			{
				MinX = double.PositiveInfinity,
				MaxX = double.NegativeInfinity,
				MinY = double.PositiveInfinity,
				MaxY = double.NegativeInfinity
			};
			foreach (Polygon polygon in this.polyLatLons)
			{
				Rect2D bounds = polygon.Bounds;
				if (bounds.MinX < this.boundRect.MinX)
				{
					this.boundRect.MinX = bounds.MinX;
				}
				if (bounds.MaxX > this.boundRect.MaxX)
				{
					this.boundRect.MaxX = bounds.MaxX;
				}
				if (bounds.MinY < this.boundRect.MinY)
				{
					this.boundRect.MinY = bounds.MinY;
				}
				if (bounds.MaxY > this.boundRect.MaxY)
				{
					this.boundRect.MaxY = bounds.MaxY;
				}
			}
			Point2D center = this.boundRect.GetCenter();
			return RegionUtility.ThreeDimFromTwoDimCartesian((double)center.Xf, (double)center.Yf, 20.005f);
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x000C345C File Offset: 0x000C165C
		private void OnRegionSelected(RegionStateSelected e)
		{
			this.RestoreRegionTexture();
		}

		// Token: 0x0600247C RID: 9340 RVA: 0x000C3464 File Offset: 0x000C1664
		public void OnRegionDeselected(CurrentOtherStateDeselected e)
		{
			this.RestoreRegionTexture();
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x000C346C File Offset: 0x000C166C
		public void RestoreRegionTexture()
		{
			this.mouseOver = false;
			if (!(GeneralControlsController.UIOtherSelectedState == this.region) && !(GeneralControlsController.UIOtherSelectedState == this.region.nation))
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				if (uiotherSelectedState == null || !uiotherSelectedState.isRegionState || !(GeneralControlsController.UIOtherSelectedState.ref_nation == this.region.nation))
				{
					if (GeneralControlsController.UIPlayerInTargetingMode && (GeneralControlsController.CurrentValidTarget(this.region) || GeneralControlsController.CurrentValidTarget(this.region.nation)))
					{
						this.SetAllowedTargetTexture(this.GetRegionFillColor());
						return;
					}
					if (this.region.IsFullyOccupied())
					{
						this.SetOccupiedTexture(this.GetRegionFillColor());
						return;
					}
					this.SetBaselineTexture(this.GetRegionFillColor());
					return;
				}
			}
			this.SetSelectedTexture(this.GetRegionFillColor());
		}

		// Token: 0x0600247E RID: 9342 RVA: 0x000C353E File Offset: 0x000C173E
		public void OnOccupationStatusChange(OccupationStatusChange e)
		{
			this.RestoreRegionTexture();
		}

		// Token: 0x0600247F RID: 9343 RVA: 0x000C3548 File Offset: 0x000C1748
		public void SetBaselineTexture(Color color)
		{
			this.SetTextureProperties(color, new int?(0), new float?(0.05f), new float?(0.05f), new float?(0.15f), true, new Vector2(250f, 250f), false, default(Vector2), new float?(color.a), 0.1f);
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x000C35AC File Offset: 0x000C17AC
		public void SetHighlightTexture(Color color)
		{
			this.SetTextureProperties(color, new int?(2), new float?(0.15f), new float?(0.15f), new float?(0.5f), true, new Vector2(250f, 250f), false, default(Vector2), new float?(0.75f), 0.5f);
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x000C3610 File Offset: 0x000C1810
		public void SetSelectedTexture(Color color)
		{
			if (color.a == 0f)
			{
				color.a = 0.1f;
			}
			this.SetTextureProperties(color, new int?(2), new float?(0.05f), new float?(0.05f), new float?(0.5f), true, new Vector2(250f, 250f), false, default(Vector2), null, 0.3f);
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x000C368C File Offset: 0x000C188C
		public void SetAllowedTargetTexture(Color color)
		{
			this.SetTextureProperties(color, new int?(0), new float?(0.05f), new float?(0.05f), new float?(0.5f), true, new Vector2(250f, 250f), false, default(Vector2), new float?(0.4f), 0.5f);
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x000C36F0 File Offset: 0x000C18F0
		public void SetOccupiedTexture(Color color)
		{
			this.SetTextureProperties(color, new int?(3), new float?(0.05f), new float?(0.05f), new float?(0.3f), true, new Vector2(250f, 250f), false, default(Vector2), new float?(color.a), 0.1f);
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x000C3754 File Offset: 0x000C1954
		private void SetTextureProperties(Color difColor, int? textureValue, float? xAnimSpeed, float? yAnimSpeed, float? patternOpacity, bool setTiling = false, Vector2 tiling = default(Vector2), bool setTilingOffset = false, Vector2 tilingOffset = default(Vector2), float? alpha = null, float emissionStrength = 0.1f)
		{
			if (this.customMaterial == null)
			{
				this.customMaterial = global::UnityEngine.Object.Instantiate<Material>(this.defaultMaterial);
				this.customMaterial.name = this.defaultMaterial.name + this.region.templateName;
				this.customMaterial.hideFlags = HideFlags.DontSave;
			}
			if (alpha != null)
			{
				difColor.a = alpha.Value;
			}
			this.customMaterial.color = difColor;
			Color color = difColor * Mathf.LinearToGammaSpace(emissionStrength);
			this.customMaterial.SetColor("_EmissionColor", color);
			if (xAnimSpeed != null)
			{
				this.customMaterial.SetFloat("_SpeedX", xAnimSpeed.Value);
			}
			if (yAnimSpeed != null)
			{
				this.customMaterial.SetFloat("_SpeedY", yAnimSpeed.Value);
			}
			if (patternOpacity != null)
			{
				this.customMaterial.SetFloat("_PatternOpacity", patternOpacity.Value);
			}
			if (textureValue != null)
			{
				if (textureValue != null)
				{
					switch (textureValue.GetValueOrDefault())
					{
					case 1:
						this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_diamond"));
						goto IL_01DC;
					case 2:
						this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_dot"));
						goto IL_01DC;
					case 3:
						this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_thickstripe"));
						goto IL_01DC;
					case 4:
						this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_thinstripe"));
						goto IL_01DC;
					case 5:
						this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_solid_alpha25"));
						goto IL_01DC;
					}
				}
				this.customMaterial.SetTexture("_MainTex", Resources.Load<Texture>("pattern_solid"));
			}
			IL_01DC:
			if (setTiling)
			{
				this.customMaterial.SetTextureScale("_MainTex", tiling);
			}
			if (setTilingOffset)
			{
				this.customMaterial.SetTextureOffset("_MainTex", tilingOffset);
			}
			Color color2 = new Color(this.region.nation.template.color.r * 1.2f, this.region.nation.template.color.g * 1.2f, this.region.nation.template.color.b * 1.2f, 0.6f);
			this.customMaterial.SetColor("_BorderColor", color2);
			for (int i = 0; i < this.regionSurfaces.Count; i++)
			{
				this.surfaceRenderers[i].sharedMaterial = this.customMaterial;
			}
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x000C3A14 File Offset: 0x000C1C14
		public void SetWidth(float newWidth)
		{
			foreach (VectorLine vectorLine in this.regionOutlines)
			{
				vectorLine.SetWidth(newWidth);
				vectorLine.Draw3D();
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x000C3A6C File Offset: 0x000C1C6C
		public void SetLiftValue(float newLift)
		{
			base.transform.localPosition = newLift * this.scalingVector;
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x000C3A88 File Offset: 0x000C1C88
		public void EnableRegionVisualizers(bool enable = true)
		{
			for (int i = 0; i < this.regionSurfaces.Count; i++)
			{
				this.surfaceRenderers[i].enabled = enable;
				this.surfaceColliders[i].enabled = enable;
			}
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x000C3ACF File Offset: 0x000C1CCF
		public void EnableMarkerVisualizers(bool enable = true)
		{
			this.tokenContainer.gameObject.SetActive(enable);
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x000C3AE2 File Offset: 0x000C1CE2
		public void FlashRegion(RegionFlashEvent e)
		{
			base.StartCoroutine(this.FlashRegion());
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x000C3AF1 File Offset: 0x000C1CF1
		public IEnumerator FlashRegion()
		{
			this.SetBaselineTexture(Color.white);
			yield return RegionController.delay1;
			this.RestoreRegionTexture();
			yield break;
		}

		// Token: 0x04001B62 RID: 7010
		public Material defaultMaterial;

		// Token: 0x04001B63 RID: 7011
		public MapController mapVisualizer;

		// Token: 0x04001B64 RID: 7012
		[SerializeField]
		private NationController nationVisualizer;

		// Token: 0x04001B65 RID: 7013
		[SerializeField]
		private TIRegionOutline outline;

		// Token: 0x04001B66 RID: 7014
		private List<List<Vector3>> regionShapes;

		// Token: 0x04001B67 RID: 7015
		private List<Vector3[]> regionSurfacePoints;

		// Token: 0x04001B68 RID: 7016
		private List<Polygon> polyLatLons;

		// Token: 0x04001B69 RID: 7017
		[SerializeField]
		private List<VectorLine> regionOutlines;

		// Token: 0x04001B6A RID: 7018
		[SerializeField]
		private List<GameObject> regionSurfaces;

		// Token: 0x04001B6B RID: 7019
		private List<Renderer> surfaceRenderers = new List<Renderer>();

		// Token: 0x04001B6C RID: 7020
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x04001B6D RID: 7021
		[SerializeField]
		private Material customMaterial;

		// Token: 0x04001B6E RID: 7022
		[SerializeField]
		private bool is3D;

		// Token: 0x04001B6F RID: 7023
		[SerializeField]
		private Transform tokenContainer;

		// Token: 0x04001B70 RID: 7024
		private Dictionary<string, Vector3> tokenPositions = new Dictionary<string, Vector3>();

		// Token: 0x04001B71 RID: 7025
		public List<MarkerContainerController> markerContainers = new List<MarkerContainerController>();

		// Token: 0x04001B72 RID: 7026
		private List<IMarkerControl> iMarkerControllers = new List<IMarkerControl>();

		// Token: 0x04001B73 RID: 7027
		[SerializeField]
		private Transform surfaceContainer;

		// Token: 0x04001B74 RID: 7028
		[SerializeField]
		private bool mouseOver;

		// Token: 0x04001B75 RID: 7029
		public TooltipTrigger regionTooltip;

		// Token: 0x04001B76 RID: 7030
		private bool heldValidTarget;

		// Token: 0x04001B77 RID: 7031
		public Vector3 scalingVector;

		// Token: 0x04001B78 RID: 7032
		[SerializeField]
		private Rect2D boundRect;

		// Token: 0x04001B79 RID: 7033
		public static float timeSegmentingCurves;

		// Token: 0x04001B7A RID: 7034
		public static float timePolyConvert;

		// Token: 0x04001B7B RID: 7035
		public static float timeAddInteriorPoints;

		// Token: 0x04001B7C RID: 7036
		public static float timeTriangulate;

		// Token: 0x04001B7D RID: 7037
		public static float timeDisplayOutline;

		// Token: 0x04001B7E RID: 7038
		public static float timeDisplayMesh;

		// Token: 0x04001B7F RID: 7039
		private static float cachedMaxPopulation;

		// Token: 0x04001B80 RID: 7040
		private static float cachedMinPopulation;

		// Token: 0x04001B81 RID: 7041
		private static float cachedMinPerCapitaGDP;

		// Token: 0x04001B82 RID: 7042
		private static float cachedMaxPerCapitaGDP;

		// Token: 0x04001B83 RID: 7043
		private static float cachedMaxMilitaryTechLevel;

		// Token: 0x04001B84 RID: 7044
		private static float cachedMaxBoostIncome;

		// Token: 0x04001B85 RID: 7045
		private static float cachedMaxIPs;

		// Token: 0x04001B86 RID: 7046
		private static int cachedFrame = -1;

		// Token: 0x04001B87 RID: 7047
		private static WaitForSeconds delay1 = new WaitForSeconds(1f);
	}
}
