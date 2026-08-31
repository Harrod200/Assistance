using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008B2 RID: 2226
	public class OperationTargetListItemController : MonoBehaviour
	{
		// Token: 0x17000F03 RID: 3843
		// (get) Token: 0x060054E9 RID: 21737 RVA: 0x00268B4D File Offset: 0x00266D4D
		public TargetSelectionTool locationSelector
		{
			get
			{
				return base.GetComponentInParent<TargetSelectionTool>();
			}
		}

		// Token: 0x060054EA RID: 21738 RVA: 0x00268B58 File Offset: 0x00266D58
		public void SetListItem(IOperation operation, TIGameState target, bool inIntelScreen = false)
		{
			this.target = target;
			TIFactionState activePlayer = GameControl.control.activePlayer;
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			this.supplyIcon.enabled = false;
			this.shipyardIcon.enabled = false;
			this.isTransferingIcon.enabled = false;
			string text = string.Empty;
			if (target.isOrbitState)
			{
				text = TIUtilities.GetStateDisplayName(target, activePlayer, false, false, false, false, true);
				TIOrbitState ref_orbit = target.ref_orbit;
				flag = ref_orbit.irradiated;
				stringBuilder.Append(Loc.T("UI.Operations.Altitude", new object[] { ref_orbit.altitude_km.ToString("N0") }));
				if (ref_orbit.interfaceOrbit)
				{
					stringBuilder.Append(Loc.T("UI.Operations.Interface"));
				}
				this.detailTextObject.SetActive(true);
				this.detailGridObject.SetActive(false);
				this.icon.sprite = AssetCacheManager.orbitIcon;
			}
			else if (target.isHabSiteState)
			{
				TIHabSiteState ref_habSite = target.ref_habSite;
				if (ref_habSite.hasPlannedOrOperatingBase)
				{
					text = TIUtilities.ConstructTextList(new List<TIGameState> { ref_habSite.hab, ref_habSite }, true, false);
				}
				else
				{
					text = TIUtilities.GetStateDisplayName(target, activePlayer, false, false, false, false, true);
				}
				flag = ref_habSite.irradiated;
				stringBuilder.Append(Loc.T(ref_habSite.ProductivityString(activePlayer.Prospected(ref_habSite.parentBody))));
				this.detailTextObject.SetActive(true);
				this.detailGridObject.SetActive(false);
				if (ref_habSite.hab != null)
				{
					this.icon.sprite = ref_habSite.hab.icon;
					if (ref_habSite.hab.AllowsResupply(activePlayer, true, false))
					{
						this.supplyIcon.enabled = true;
					}
					else
					{
						this.supplyIcon.enabled = false;
					}
					if (ref_habSite.hab.AllowsShipConstruction(activePlayer, false, false))
					{
						this.shipyardIcon.enabled = true;
					}
					else
					{
						this.shipyardIcon.enabled = false;
					}
				}
				else
				{
					this.icon.sprite = HabSiteController.GetEmptyHabSiteIcon(ref_habSite, activePlayer);
				}
			}
			else if (target.isSpaceAssetState)
			{
				text = TIUtilities.GetStateDisplayName(target, activePlayer, false, false, false, false, true);
				this.icon.sprite = target.ref_spaceAsset.icon;
				stringBuilder.Append(TIUtilities.GetLocationString(target.ref_spaceAsset.location, true, false));
				this.detailTextObject.SetActive(true);
				this.detailGridObject.SetActive(false);
				if (target.isSpaceFleetState && (target.ref_fleet.inTransfer || (target.ref_fleet.transferAssigned && target.ref_fleet.faction == GameControl.control.activePlayer)))
				{
					this.isTransferingIcon.enabled = true;
					stringBuilder.Append(Loc.T("UI.Nation.RelationsFeedback", new object[] { target.ref_fleet.GetLocationDescription(GameControl.control.activePlayer, true, true) }));
				}
				if (target.isHabState)
				{
					if (target.ref_hab.AllowsResupply(activePlayer, true, false))
					{
						this.supplyIcon.enabled = true;
					}
					else
					{
						this.supplyIcon.enabled = false;
					}
					if (target.ref_hab.AllowsShipConstruction(activePlayer, false, false))
					{
						this.shipyardIcon.enabled = true;
					}
					else
					{
						this.shipyardIcon.enabled = false;
					}
				}
			}
			else if (target.isNaturalSpaceObjectState)
			{
				text = TIUtilities.GetStateDisplayName(target, activePlayer, false, false, false, false, true);
				flag = target.ref_naturalSpaceObject.IsIrradiated();
				this.icon.sprite = target.ref_naturalSpaceObject.icon;
				this.detailTextObject.SetActive(false);
				List<TIFactionState> list;
				if (!target.isSpaceBodyState)
				{
					list = new List<TIFactionState>();
				}
				else
				{
					list = (from x in (from x in target.ref_spaceBody.surfaceBases
							where x.VisibleToFaction(activePlayer)
							select x.coreFaction).Distinct<TIFactionState>()
						orderby x.ID
						select x).ToList<TIFactionState>();
				}
				List<TIFactionState> list2 = list;
				List<TIFactionState> list3 = (from x in (from x in target.ref_naturalSpaceObject.stationsInOrbit
						where x.VisibleToFaction(activePlayer)
						select x.coreFaction).Distinct<TIFactionState>()
					orderby x.ID
					select x).ToList<TIFactionState>();
				List<TIFactionState> list4 = (from x in (from x in target.ref_naturalSpaceObject.fleetsInOrbit
						where x.VisibleToFaction(activePlayer)
						select x.faction).Distinct<TIFactionState>()
					orderby x.ID
					select x).ToList<TIFactionState>();
				List<TIFactionState> list5 = new List<TIFactionState>();
				list5.AddRange(list2);
				list5.AddRange(list3);
				list5.AddRange(list4);
				if (list5.Count > 0)
				{
					this.detailGridList.SetListSize<SpaceObjectSymbolMarkerPanelGridItemController>(list2.Count + list4.Count + list3.Count, false, false);
					if (list5.Count <= 10)
					{
						this.gridLayout.cellSize = new Vector2(24f, 24f);
					}
					else
					{
						this.gridLayout.cellSize = new Vector2(12f, 12f);
					}
					int num = 1;
					using (IEnumerator<object> enumerator = this.detailGridList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (OperationTargetListItemController.<>o__16.<>p__0 == null)
							{
								OperationTargetListItemController.<>o__16.<>p__0 = CallSite<Func<CallSite, object, SpaceObjectSymbolMarkerPanelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SpaceObjectSymbolMarkerPanelGridItemController), typeof(OperationTargetListItemController)));
							}
							SpaceObjectSymbolMarkerPanelGridItemController spaceObjectSymbolMarkerPanelGridItemController = OperationTargetListItemController.<>o__16.<>p__0.Target(OperationTargetListItemController.<>o__16.<>p__0, enumerator.Current);
							if (num <= list2.Count)
							{
								spaceObjectSymbolMarkerPanelGridItemController.UpdateBaseGridItem(list5[num - 1]);
							}
							else if (num <= list2.Count + list3.Count)
							{
								spaceObjectSymbolMarkerPanelGridItemController.UpdateStationGridItem(list5[num - 1]);
							}
							else
							{
								spaceObjectSymbolMarkerPanelGridItemController.UpdateFleetGridItem(list5[num - 1]);
							}
							spaceObjectSymbolMarkerPanelGridItemController.gameObject.SetActive(true);
							num++;
						}
					}
					this.detailGridObject.SetActive(true);
				}
				else if (target.isSpaceBodyState)
				{
					text = TIUtilities.GetStateDisplayName(target, activePlayer, false, false, false, false, true);
					string profileRatingAllIconsString = target.ref_spaceBody.GetProfileRatingAllIconsString(activePlayer.Prospected(target.ref_spaceBody));
					stringBuilder.Append(Loc.T("UI.Operations.Productivity", new object[] { profileRatingAllIconsString }));
					this.detailTextObject.SetActive(true);
					this.detailGridObject.SetActive(false);
				}
				else
				{
					this.detailGridObject.SetActive(false);
				}
			}
			else
			{
				this.detailGridObject.SetActive(false);
				this.detailTextObject.SetActive(false);
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			if (flag)
			{
				stringBuilder2.Append(TemplateManager.global.irradiatedInlineSpritePath);
			}
			stringBuilder2.Append(text);
			if (target.isOrbitState && target.ref_orbit.destroyedAssets > 0)
			{
				stringBuilder2.Append(TemplateManager.global.spaceDebrisInlineSpritePath);
			}
			this.targetDisplayName.SetText(stringBuilder2.ToString());
			this.operationDetailString.SetText(stringBuilder.ToString());
			this.disableUnexploredSelection = false;
			this.disableGoto = inIntelScreen;
			if (operation != null && operation.HasResourceCost())
			{
				List<TIResourcesCost> list6 = operation.ResourceCostOptions(activePlayer, target, activePlayer, false);
				StringBuilder sb2 = new StringBuilder();
				if (target.isOrbitState)
				{
					sb2.AppendLine(TIOrbitState.OrbitTooltip(target.ref_orbit));
				}
				foreach (TIResourcesCost tiresourcesCost in list6)
				{
					sb2.AppendLine(tiresourcesCost.GetString("Relevant", true, true, false, 7, false, false, activePlayer, false, FactionResource.None));
				}
				this.tooltipTrigger.enabled = true;
				this.tooltipTrigger.SetDelegate("BodyText", () => sb2.ToString());
				return;
			}
			if (!target.isSpaceGameState || GameControl.control.activePlayer.CanExplore(target as TISpaceGameState))
			{
				this.tooltipTrigger.enabled = false;
				return;
			}
			this.disableUnexploredSelection = !inIntelScreen;
			TIEffectTemplate requiredEffect = target.ref_naturalSpaceObject.GetStandardEffectToExplore();
			if (requiredEffect != null)
			{
				TITechTemplate requiredTech = TemplateManager.IterateByClass<TITechTemplate>(true).FirstOrDefault<TITechTemplate>((TITechTemplate x) => x.effects.Contains(requiredEffect.dataName));
				this.tooltipTrigger.enabled = true;
				this.tooltipTrigger.SetDelegate("BodyText", () => TIUtilities.RedLine(Loc.T("UI.Science.RequiredNation", new object[] { requiredTech.displayName })));
				return;
			}
			this.tooltipTrigger.enabled = false;
		}

		// Token: 0x060054EB RID: 21739 RVA: 0x002694D8 File Offset: 0x002676D8
		public void OnOperationTargetButtonPressed()
		{
			if (this.disableUnexploredSelection)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			TIGameState tigameState = this.target;
			this.locationSelector.OnElementClicked(this.target);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			SpaceObjectSelection.BlockSelectionFrame();
			if (!this.disableGoto && (tigameState.isHabState || tigameState.ref_faction != GameControl.control.activePlayer))
			{
				if (tigameState.isOrbitState)
				{
					if (tigameState.ref_spaceBody != null)
					{
						TIUtilities.GotoGameState(tigameState.ref_spaceBody, false, true, false, false, false, -1f);
						return;
					}
					if (tigameState.ref_naturalSpaceObject != null)
					{
						TIUtilities.GotoGameState(tigameState.ref_naturalSpaceObject, false, true, false, false, false, -1f);
						return;
					}
				}
				else
				{
					TIUtilities.GotoGameState(tigameState, false, true, false, false, false, -1f);
				}
			}
		}

		// Token: 0x060054EC RID: 21740 RVA: 0x002695AC File Offset: 0x002677AC
		public void OnRightClickTransferTarget()
		{
			if (this.disableGoto)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			if (this.target.isOrbitState)
			{
				TIUtilities.GotoGameState(this.target.ref_spaceBody, false, true, false, false, false, -1f);
				return;
			}
			TIUtilities.GotoGameState(this.target, false, true, false, false, false, -1f);
		}

		// Token: 0x04003B04 RID: 15108
		public TMP_Text targetDisplayName;

		// Token: 0x04003B05 RID: 15109
		public TMP_Text operationDetailString;

		// Token: 0x04003B06 RID: 15110
		public TooltipTrigger tooltipTrigger;

		// Token: 0x04003B07 RID: 15111
		public Image icon;

		// Token: 0x04003B08 RID: 15112
		public Image supplyIcon;

		// Token: 0x04003B09 RID: 15113
		public Image shipyardIcon;

		// Token: 0x04003B0A RID: 15114
		public Image isTransferingIcon;

		// Token: 0x04003B0B RID: 15115
		private TIGameState target;

		// Token: 0x04003B0C RID: 15116
		public GameObject detailTextObject;

		// Token: 0x04003B0D RID: 15117
		public GameObject detailGridObject;

		// Token: 0x04003B0E RID: 15118
		public GridLayoutGroup gridLayout;

		// Token: 0x04003B0F RID: 15119
		public ListManagerBase detailGridList;

		// Token: 0x04003B10 RID: 15120
		private bool disableGoto;

		// Token: 0x04003B11 RID: 15121
		private bool disableUnexploredSelection;
	}
}
