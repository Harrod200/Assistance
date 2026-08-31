using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000882 RID: 2178
	public class IntelSpaceBodyListItemController : MonoBehaviour, IListPaneItem<TISpaceBodyState>
	{
		// Token: 0x0600518A RID: 20874 RVA: 0x0023CF58 File Offset: 0x0023B158
		public void Initialize(TISpaceBodyState spaceBody)
		{
			this.spaceBody = spaceBody;
			this.spaceBodyName.SetText(spaceBody.displayName);
			this.intelController = base.transform.parent.gameObject.GetComponent<IntelSpaceBodyListPane>().intelController;
			this.description.SetText(spaceBody.template.descriptor1);
			this.dimensions.SetText(SpaceObjectDetailController.SpaceBodyDiameterText(spaceBody));
			this.orbitSemimajor_Axis.SetText(SpaceObjectDetailController.OrbitAxisText(spaceBody));
			this.habSitesCount.SetText(spaceBody.habSites.Length.ToString());
			this.prospectedIcon.enabled = false;
			this.description2.SetText(spaceBody.GetMiningPotentialString());
			this.spaceBodyIcon.sprite = spaceBody.icon;
			if (spaceBody.barycenter.objectType != SpaceObjectType.Star)
			{
				this.orbitIcon.sprite = spaceBody.barycenter.icon;
			}
			else
			{
				this.orbitIcon.sprite = AssetCacheManager.orbitIcon;
			}
			this.dimensions.SetText(SpaceObjectDetailController.SpaceBodyDiameterText(spaceBody));
			this.orbitSemimajor_Axis.SetText(SpaceObjectDetailController.OrbitAxisText(spaceBody));
			this.habSitesCount.SetText(spaceBody.habSites.Length.ToString());
			if (spaceBody.habSites.Length == 0)
			{
				this.waterValueIcon.enabled = false;
				this.volatilesValueIcon.enabled = false;
				this.metalsValueIcon.enabled = false;
				this.noblesValueIcon.enabled = false;
				this.fertilesValueIcon.enabled = false;
			}
			else
			{
				this.waterValueIcon.enabled = true;
				this.volatilesValueIcon.enabled = true;
				this.metalsValueIcon.enabled = true;
				this.noblesValueIcon.enabled = true;
				this.fertilesValueIcon.enabled = true;
				this.SetSiteProfileRatingIconsAndValues();
			}
			this.sumSolar = TIHabModuleState.NaturalSolarPowerMultiplier(spaceBody.orbits.MaxBy<TIOrbitState, float>((TIOrbitState x) => x.solarMultiplier));
			GameControl.assetLoader.LoadAssetForImageAssignment(spaceBody.SolarInsolationIconPath(false), this.solarValueIcon);
			this.solarValueIcon.enabled = true;
			this.prospectedRecorded = false;
			this.habInfoTip1.SetDelegate("BodyText", () => this.SetHabValuesTooltip());
			this.habInfoTip2.SetDelegate("BodyText", () => this.SetHabValuesTooltip());
			this.habSiteHohmannTip.SetDelegate("BodyText", () => SpaceObjectDetailController.SetTimePenaltyTip(spaceBody));
			this.SetPlanetTag();
			this.Refresh();
		}

		// Token: 0x0600518B RID: 20875 RVA: 0x0023D240 File Offset: 0x0023B440
		public void SetSiteProfileRatingIconsAndValues()
		{
			bool flag = GameControl.control.activePlayer.Prospected(this.spaceBody);
			this.sumWater = this.spaceBody.GetSiteProfileRating(FactionResource.Water, flag);
			this.sumVolatiles = this.spaceBody.GetSiteProfileRating(FactionResource.Volatiles, flag);
			this.sumMetals = this.spaceBody.GetSiteProfileRating(FactionResource.Metals, flag);
			this.sumNobles = this.spaceBody.GetSiteProfileRating(FactionResource.NobleMetals, flag);
			this.sumFissiles = this.spaceBody.GetSiteProfileRating(FactionResource.Fissiles, flag);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.spaceBody.GetProfileRatingIconPath(FactionResource.Water, false, flag), this.waterValueIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.spaceBody.GetProfileRatingIconPath(FactionResource.Volatiles, false, flag), this.volatilesValueIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.spaceBody.GetProfileRatingIconPath(FactionResource.Metals, false, flag), this.metalsValueIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.spaceBody.GetProfileRatingIconPath(FactionResource.NobleMetals, false, flag), this.noblesValueIcon);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.spaceBody.GetProfileRatingIconPath(FactionResource.Fissiles, false, flag), this.fertilesValueIcon);
		}

		// Token: 0x0600518C RID: 20876 RVA: 0x0023D360 File Offset: 0x0023B560
		public void OnClickSortButton(int sortValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.intelController.OnChangeSpaceSort(sortValue);
		}

		// Token: 0x0600518D RID: 20877 RVA: 0x0023D37A File Offset: 0x0023B57A
		public void OnClickGotoButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
			this.intelController.Close();
			TIUtilities.GotoGameState(this.spaceBody, true, true, true, true, false, -1f);
		}

		// Token: 0x0600518E RID: 20878 RVA: 0x0023D3A8 File Offset: 0x0023B5A8
		public void OnClickProspectButton()
		{
			List<TIResourcesCost> list = this.intelController.activePlayer.CanOvertakeProbeWithProbe(this.spaceBody);
			if (list.Count > 0)
			{
				new LaunchOverrideProbeOperation().OnOperationConfirm(this.intelController.activePlayer, this.spaceBody, list[0], null);
				AudioManager.PlayOneShot("event:/SFX/Game_SFX/Guns/trig_SFX_Missile_Launch", false, false);
				this.Refresh();
			}
			else
			{
				LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
				List<TIResourcesCost> list2 = launchProbeOperation.ResourceCostOptions(this.intelController.activePlayer, this.spaceBody, this.intelController.activePlayer, true);
				if (list2.Count > 0)
				{
					launchProbeOperation.OnOperationConfirm(this.intelController.activePlayer, this.spaceBody, list2[0], null);
					AudioManager.PlayOneShot("event:/SFX/Game_SFX/Guns/trig_SFX_Missile_Launch", false, false);
					this.Refresh();
				}
			}
			this.intelController.UpdateSpaceBodyListModelData();
			this.intelController.SetProbeAllButton();
		}

		// Token: 0x0600518F RID: 20879 RVA: 0x0023D488 File Offset: 0x0023B688
		private string SetProspectTooltip(TIResourcesCost cost, bool overtake)
		{
			if (overtake)
			{
				TIDateTime tidateTime = this.intelController.activePlayer.ProspectorArrival(this.spaceBody);
				TIDateTime tidateTime2 = TITimeState.Now();
				tidateTime2.AddDays(cost.completionTime_days);
				return Loc.T("UI.Intel.OverrideProspectTipText", new object[]
				{
					cost.GetString("Relevant", true, true, false, 7, false, false, this.intelController.activePlayer, false, FactionResource.None),
					tidateTime.ToCustomDateString(),
					tidateTime2.ToCustomDateString()
				});
			}
			return Loc.T("UI.Intel.ProspectTipText", new object[] { cost.GetString("Relevant", true, true, false, 7, false, false, this.intelController.activePlayer, false, FactionResource.None) });
		}

		// Token: 0x06005190 RID: 20880 RVA: 0x0023D538 File Offset: 0x0023B738
		private string SetHabValuesTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.spaceBody.displayName);
			TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>("SolarCollector", false);
			if (this.spaceBody.orbits.Count > 0)
			{
				float num = Mathf.Min(8f, (float)TIHabModuleState.SolarPowerOutput(this.spaceBody.orbits[0], (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, true) / (float)TIHabModuleState.SolarPowerOutput(GameStateManager.Earth().orbits[0], (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, true));
				stringBuilder.AppendLine(Loc.T("UI.Intel.HabValueSpaceSolar", new object[]
				{
					this.spaceBody.displayName,
					num.ToPercent("P0"),
					tihabModuleTemplate.displayName,
					TIHabModuleState.SolarPowerOutput(this.spaceBody.orbits[0], (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, false).ToString("N0")
				}));
				if (this.spaceBody.orbits.Any<TIOrbitState>((TIOrbitState x) => x.irradiated))
				{
					stringBuilder.AppendLine(Loc.T("UI.Intel.HabValueSpaceIrradiated", new object[] { this.spaceBody.displayName }));
				}
			}
			if (this.spaceBody.habSites.Length != 0)
			{
				float num2 = Mathf.Min(8f, (float)TIHabModuleState.SolarPowerOutput(this.spaceBody.habSites[0], (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, true) / (float)TIHabModuleState.SolarPowerOutput(GameStateManager.Luna().habSites[0], (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, true));
				stringBuilder.AppendLine(Loc.T("UI.Intel.HabValueSurfaceSolar", new object[]
				{
					this.spaceBody.displayName,
					num2.ToPercent("P0"),
					tihabModuleTemplate.displayName,
					TIHabModuleState.SolarPowerOutput(this.spaceBody, (float)tihabModuleTemplate.power, GameControl.control.activePlayer, 1, false).ToString("N0")
				}));
				if (this.spaceBody.irradiated)
				{
					stringBuilder.AppendLine(Loc.T("UI.Intel.HabValueSurfaceIrradiated", new object[] { this.spaceBody.displayName }));
				}
				TIHabModuleTemplate tihabModuleTemplate2 = TemplateManager.Find<TIHabModuleTemplate>("OutpostMiningComplex", false);
				stringBuilder.AppendLine(Loc.T("UI.Intel.HabValueMineGravity", new object[]
				{
					this.spaceBody.displayName,
					tihabModuleTemplate2.displayName,
					TemplateManager.global.boostInlineSpritePath,
					tihabModuleTemplate2.BoostCostFromEarth(this.spaceBody.irradiatedMultiplier, this.spaceBody, GameControl.control.activePlayer, this.spaceBody, 1f, null).ToString("N2"),
					tihabModuleTemplate2.BuildMaterials(this.spaceBody.irradiatedMultiplier, this.spaceBody, this.spaceBody, GameControl.control.activePlayer, 1f).ToResourcesCost(1f).ToString("Relevant", false, false, GameControl.control.activePlayer, false, FactionResource.None),
					-tihabModuleTemplate2.ProspectivePower(this.spaceBody, GameControl.control.activePlayer)
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005191 RID: 20881 RVA: 0x0023D8B4 File Offset: 0x0023BAB4
		public void Refresh()
		{
			if (!this.prospectedRecorded)
			{
				if (this.intelController.activePlayer.Prospected(this.spaceBody) && this.spaceBody.habSites.Length != 0)
				{
					this.prospectedIcon.sprite = AssetCacheManager.prospectedIcon;
					this.prospectedIcon.enabled = true;
					GameControl.control.activePlayer.Prospected(this.spaceBody);
					this.SetSiteProfileRatingIconsAndValues();
					this.prospectedRecorded = true;
					this.orderProspecting.gameObject.SetActive(false);
					this.canResendProspectorMarker.enabled = false;
				}
				else if (this.intelController.activePlayer.ProspectingSpaceBody(this.spaceBody))
				{
					if (this.intelController.activePlayer.ProspectorEnRoute(this.spaceBody))
					{
						List<TIResourcesCost> overtakeOptions = this.intelController.activePlayer.CanOvertakeProbeWithProbe(this.spaceBody);
						if (overtakeOptions.Count > 0)
						{
							this.canResendProspectorMarker.enabled = true;
							this.prospectTooltip.SetDelegate("BodyText", () => this.SetProspectTooltip(overtakeOptions[0], true));
							this.orderProspecting.gameObject.SetActive(true);
							LaunchOverrideProbeOperation launchOverrideProbeOperation = new LaunchOverrideProbeOperation();
							this.orderProspecting.interactable = launchOverrideProbeOperation.ActorCanPerformOperation(this.intelController.activePlayer, this.spaceBody) && overtakeOptions.Any<TIResourcesCost>((TIResourcesCost x) => x.CanAfford(this.intelController.activePlayer, 1f, null, float.PositiveInfinity));
						}
						else
						{
							this.prospectedIcon.sprite = AssetCacheManager.prospectingUnderway;
							this.prospectedIcon.enabled = true;
							this.canResendProspectorMarker.enabled = false;
							this.orderProspecting.gameObject.SetActive(false);
						}
					}
					else
					{
						this.prospectedIcon.sprite = AssetCacheManager.prospectingUnderway;
						this.prospectedIcon.enabled = true;
						this.canResendProspectorMarker.enabled = false;
						this.orderProspecting.gameObject.SetActive(false);
					}
				}
				else
				{
					this.prospectedIcon.enabled = false;
					this.canResendProspectorMarker.enabled = false;
					if (this.intelController.activePlayer.CanProspectWithProbe(this.spaceBody, false))
					{
						LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
						List<TIResourcesCost> costs = launchProbeOperation.ResourceCostOptions(this.intelController.activePlayer, this.spaceBody, this.intelController.activePlayer, true);
						this.orderProspecting.gameObject.SetActive(true);
						if (costs.Count > 0)
						{
							costs = (from x in costs
								orderby x.completionTime_days, x.GetSingleCostValue(FactionResource.Boost)
								select x).ToList<TIResourcesCost>();
							this.orderProspecting.interactable = launchProbeOperation.ActorCanPerformOperation(this.intelController.activePlayer, this.spaceBody);
							this.prospectTooltip.SetDelegate("BodyText", () => this.SetProspectTooltip(costs[0], false));
							this.prospectTooltip.enabled = true;
						}
						else
						{
							this.orderProspecting.interactable = false;
							this.prospectTooltip.enabled = false;
						}
					}
					else
					{
						this.orderProspecting.gameObject.SetActive(false);
					}
				}
			}
			List<TIFactionState> list = new List<TIFactionState>();
			List<TIFactionState> factionsPresent = new List<TIFactionState>();
			foreach (TIHabState tihabState in this.spaceBody.stationsInOrbit.Intersect<TIHabState>(this.intelController.cachedKnownStationsList))
			{
				list.Add(tihabState.faction);
			}
			factionsPresent = list.Distinct<TIFactionState>().ToList<TIFactionState>();
			this.stationsGrid.SetListSize<TinyHabGridItemController>(factionsPresent.Count<TIFactionState>(), false, false);
			int k = 0;
			using (IEnumerator<object> enumerator2 = this.stationsGrid.GetEnumerator())
			{
				Func<TIFactionState, bool> <>9__5;
				while (enumerator2.MoveNext())
				{
					if (IntelSpaceBodyListItemController.<>o__46.<>p__0 == null)
					{
						IntelSpaceBodyListItemController.<>o__46.<>p__0 = CallSite<Func<CallSite, object, TinyHabGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TinyHabGridItemController), typeof(IntelSpaceBodyListItemController)));
					}
					TinyHabGridItemController tinyHabGridItemController = IntelSpaceBodyListItemController.<>o__46.<>p__0.Target(IntelSpaceBodyListItemController.<>o__46.<>p__0, enumerator2.Current);
					TIFactionState tifactionState = factionsPresent[k];
					IEnumerable<TIFactionState> enumerable = list;
					Func<TIFactionState, bool> func;
					if ((func = <>9__5) == null)
					{
						func = (<>9__5 = (TIFactionState x) => x == factionsPresent[k]);
					}
					tinyHabGridItemController.SetGridItem(tifactionState, enumerable.Count<TIFactionState>(func), true);
					int num = k;
					k = num + 1;
				}
			}
			List<TIFactionState> list2 = new List<TIFactionState>();
			foreach (TIHabState tihabState2 in this.spaceBody.surfaceBases.Intersect<TIHabState>(this.intelController.cachedKnownHabsList))
			{
				list2.Add(tihabState2.faction);
			}
			factionsPresent = list2.Distinct<TIFactionState>().ToList<TIFactionState>();
			this.basesGrid.SetListSize<TinyHabGridItemController>(list2.Distinct<TIFactionState>().Count<TIFactionState>(), false, false);
			k = 0;
			using (IEnumerator<object> enumerator2 = this.basesGrid.GetEnumerator())
			{
				Func<TIFactionState, bool> <>9__6;
				while (enumerator2.MoveNext())
				{
					if (IntelSpaceBodyListItemController.<>o__46.<>p__1 == null)
					{
						IntelSpaceBodyListItemController.<>o__46.<>p__1 = CallSite<Func<CallSite, object, TinyHabGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TinyHabGridItemController), typeof(IntelSpaceBodyListItemController)));
					}
					TinyHabGridItemController tinyHabGridItemController2 = IntelSpaceBodyListItemController.<>o__46.<>p__1.Target(IntelSpaceBodyListItemController.<>o__46.<>p__1, enumerator2.Current);
					TIFactionState tifactionState2 = factionsPresent[k];
					IEnumerable<TIFactionState> enumerable2 = list2;
					Func<TIFactionState, bool> func2;
					if ((func2 = <>9__6) == null)
					{
						func2 = (<>9__6 = (TIFactionState x) => x == factionsPresent[k]);
					}
					tinyHabGridItemController2.SetGridItem(tifactionState2, enumerable2.Count<TIFactionState>(func2), false);
					int num = k;
					k = num + 1;
				}
			}
			if (this.spaceBody.barycenter.isEarth || this.spaceBody.isEarth)
			{
				this.earthLaunchWindow.SetText(string.Empty);
				return;
			}
			double num2;
			TIDateTime nextHohmannLaunchWindowDate = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(this.intelController.activePlayer, GameStateManager.Earth(), this.spaceBody, TITimeState.Now(), out num2);
			bool flag;
			double hohmannTimePenaltyFraction = TISpaceObjectState.GetHohmannTimePenaltyFraction(this.intelController.activePlayer, nextHohmannLaunchWindowDate, num2, out flag);
			this.earthLaunchWindow.SetText(Loc.T("UI.Intel.LaunchWindowText", new object[]
			{
				TemplateManager.global.orbitInlineSpritePath,
				hohmannTimePenaltyFraction.ToPercent("P0"),
				flag ? TemplateManager.global.upRedArrowInlineSpritePath : TemplateManager.global.downGreenArrowInlineSpritePath
			}));
		}

		// Token: 0x06005192 RID: 20882 RVA: 0x0023DFD4 File Offset: 0x0023C1D4
		private void SetPlanetTag()
		{
			switch (this.spaceBody.playerTag)
			{
			case PlayerTag.Red:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagRed;
				return;
			case PlayerTag.Green:
				this.playerTagButtonImage.color = SpaceObjectSymbolController.PlanetTagGreen;
				return;
			}
			this.playerTagButtonImage.color = Color.white;
		}

		// Token: 0x06005193 RID: 20883 RVA: 0x0023E034 File Offset: 0x0023C234
		public void OnClickPlanetTag()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			PlayerTag[] array = (PlayerTag[])Enum.GetValues(typeof(PlayerTag));
			int num = (int)((this.spaceBody.playerTag + 1) % (PlayerTag)array.Length);
			this.intelController.activePlayer.playerControl.StartAction(new SetPlanetTagAction(this.spaceBody, array[num]));
			this.SetPlanetTag();
			this.intelController.UpdateSpaceBodyListSortTag();
		}

		// Token: 0x040035EB RID: 13803
		public Image spaceBodyIcon;

		// Token: 0x040035EC RID: 13804
		public TMP_Text spaceBodyName;

		// Token: 0x040035ED RID: 13805
		public TMP_Text description;

		// Token: 0x040035EE RID: 13806
		public TMP_Text description2;

		// Token: 0x040035EF RID: 13807
		public Image orbitIcon;

		// Token: 0x040035F0 RID: 13808
		public TMP_Text orbitSemimajor_Axis;

		// Token: 0x040035F1 RID: 13809
		public TMP_Text dimensions;

		// Token: 0x040035F2 RID: 13810
		public Image waterValueIcon;

		// Token: 0x040035F3 RID: 13811
		public Image volatilesValueIcon;

		// Token: 0x040035F4 RID: 13812
		public Image metalsValueIcon;

		// Token: 0x040035F5 RID: 13813
		public Image noblesValueIcon;

		// Token: 0x040035F6 RID: 13814
		public Image fertilesValueIcon;

		// Token: 0x040035F7 RID: 13815
		public Image solarValueIcon;

		// Token: 0x040035F8 RID: 13816
		public Image prospectedIcon;

		// Token: 0x040035F9 RID: 13817
		public Image canResendProspectorMarker;

		// Token: 0x040035FA RID: 13818
		public TMP_Text habSitesCount;

		// Token: 0x040035FB RID: 13819
		public GameObject habSitesIcon;

		// Token: 0x040035FC RID: 13820
		public TMP_Text earthLaunchWindow;

		// Token: 0x040035FD RID: 13821
		public TooltipTrigger habSiteHohmannTip;

		// Token: 0x040035FE RID: 13822
		public Button orderProspecting;

		// Token: 0x040035FF RID: 13823
		public TooltipTrigger prospectTooltip;

		// Token: 0x04003600 RID: 13824
		public double orbitSortWeight;

		// Token: 0x04003601 RID: 13825
		public double DescSortWeight;

		// Token: 0x04003602 RID: 13826
		public double orbitValue;

		// Token: 0x04003603 RID: 13827
		public double sizeValue;

		// Token: 0x04003604 RID: 13828
		public SiteProfileRating sumWater;

		// Token: 0x04003605 RID: 13829
		public SiteProfileRating sumVolatiles;

		// Token: 0x04003606 RID: 13830
		public SiteProfileRating sumMetals;

		// Token: 0x04003607 RID: 13831
		public SiteProfileRating sumNobles;

		// Token: 0x04003608 RID: 13832
		public SiteProfileRating sumFissiles;

		// Token: 0x04003609 RID: 13833
		public float sumSolar;

		// Token: 0x0400360A RID: 13834
		private bool prospectedRecorded;

		// Token: 0x0400360B RID: 13835
		public TISpaceBodyState spaceBody;

		// Token: 0x0400360C RID: 13836
		private IntelScreenController intelController;

		// Token: 0x0400360D RID: 13837
		public ListManagerBase stationsGrid;

		// Token: 0x0400360E RID: 13838
		public ListManagerBase basesGrid;

		// Token: 0x0400360F RID: 13839
		public TooltipTrigger habInfoTip1;

		// Token: 0x04003610 RID: 13840
		public TooltipTrigger habInfoTip2;

		// Token: 0x04003611 RID: 13841
		public Image playerTagButtonImage;
	}
}
