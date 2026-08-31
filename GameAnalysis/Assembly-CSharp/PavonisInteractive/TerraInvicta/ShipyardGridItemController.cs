using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200085C RID: 2140
	public class ShipyardGridItemController : MonoBehaviour
	{
		// Token: 0x17000EB6 RID: 3766
		// (get) Token: 0x06004E70 RID: 20080 RVA: 0x0021BD42 File Offset: 0x00219F42
		public bool allowPayFromEarth
		{
			get
			{
				return this.shipyardIdx.shipyardAllowPayFromEarth;
			}
		}

		// Token: 0x06004E71 RID: 20081 RVA: 0x0021BD50 File Offset: 0x00219F50
		public void Init(FleetsScreenController controller, TIHabModuleState shipyardIdx)
		{
			this.controller = controller;
			this.shipyardIdx = shipyardIdx;
			this.ClearQueueButtonText.SetText(Loc.T("UI.Fleets.ClearQueueButton"));
			this.payFromEarthTooltip.SetText("BodyText", Loc.T("UI.Fleets.PayFromEarthTooltip"));
			this.currentConstructionTitle.SetText(Loc.T("UI.Fleets.CurrentlyBuildingTitle"));
			this.constructionQueueTitle.SetText(Loc.T("UI.Fleets.ConstructionQueueTitle"));
			this.allowPayFromEarthToggle.SetIsOnWithoutNotify(shipyardIdx.shipyardAllowPayFromEarth);
		}

		// Token: 0x06004E72 RID: 20082 RVA: 0x0021BDD8 File Offset: 0x00219FD8
		public void UpdateGridItem()
		{
			this.habName.SetText(this.shipyardIdx.sector.hab.displayName);
			this.shipyardDetails.SetText(Loc.T("UI.Fleets.ShipyardDetails", new object[]
			{
				this.shipyardIdx.moduleTemplate.displayName,
				this.shipyardIdx.sector.shortSectorString,
				this.shipyardIdx.active ? string.Empty : TIUtilities.RedLine(Loc.T("UI.Fleets.Inactive"))
			}));
			this.habLocation.SetText(this.shipyardIdx.sector.hab.LocationName);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.shipyardIdx.moduleTemplate.iconResource(this.shipyardIdx.sector.hab.habType), this.moduleImage);
			this.UpdateShipyardTierPips();
			GameControl.assetLoader.LoadAssetForImageAssignment(this.shipyardIdx.ref_faction.template.gradientPath, this.backgroundFactionGradient);
			List<ShipConstructionQueueItem> shipyardQueue = this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx);
			if (shipyardQueue != null && shipyardQueue.Count > 0)
			{
				if (shipyardQueue[0].costPaid || shipyardQueue[0].resourcesCost.CanAfford(GameControl.control.activePlayer, 1f, null, float.PositiveInfinity))
				{
					string text = Loc.T("UI.Fleets.CurrentlyBuildingValue", new object[]
					{
						shipyardQueue[0].shipDesign.fullClassName,
						shipyardQueue[0].daysToCompletion.ToString("N0")
					});
					if (!this.shipyardIdx.active)
					{
						TIUtilities.RedLine(text);
					}
					this.currentConstruction.SetText(text);
				}
				else
				{
					List<ResourceValue> list = shipyardQueue[0].resourcesCost.LackingResources(this.controller.activePlayer);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ResourceValue resourceValue in list)
					{
						stringBuilder.Append(resourceValue.ToString()).Append(" ");
					}
					this.currentConstruction.SetText(Loc.T("UI.Fleets.CurrentlyWaitingForResources", new object[]
					{
						shipyardQueue[0].shipDesign.fullClassName,
						stringBuilder.ToString().TrimEnd(Array.Empty<char>())
					}));
				}
			}
			else
			{
				this.currentConstruction.SetText(Loc.T("UI.Fleets.ShipyardIdle"));
			}
			this.UpdateGravityIcon();
			this.UpdateConstructionQueue();
			this.SetButtons();
		}

		// Token: 0x06004E73 RID: 20083 RVA: 0x0021C094 File Offset: 0x0021A294
		public void UpdateShipyardTierPips()
		{
			switch (this.shipyardIdx.sector.hab.tier)
			{
			default:
				GameControl.assetLoader.LoadAssetForImageAssignment(TIGlobalConfig.globalConfig.pathMaxTier1Hab, this.shipyardTier);
				return;
			case 2:
				GameControl.assetLoader.LoadAssetForImageAssignment(TIGlobalConfig.globalConfig.pathMaxTier2Hab, this.shipyardTier);
				return;
			case 3:
				GameControl.assetLoader.LoadAssetForImageAssignment(TIGlobalConfig.globalConfig.pathMaxTier3Hab, this.shipyardTier);
				return;
			case 4:
				GameControl.assetLoader.LoadAssetForImageAssignment(TIGlobalConfig.globalConfig.pathMaxTier4Hab, this.shipyardTier);
				return;
			}
		}

		// Token: 0x06004E74 RID: 20084 RVA: 0x0021C13A File Offset: 0x0021A33A
		private string GravityText(string str)
		{
			return new StringBuilder(TemplateManager.global.gravityInlineSpritePath).Append(str).ToString();
		}

		// Token: 0x06004E75 RID: 20085 RVA: 0x0021C158 File Offset: 0x0021A358
		public void UpdateGravityIcon()
		{
			if (this.shipyardIdx.hab.IsStation)
			{
				if (this.shipyardIdx.hab.ref_orbit.localGravity_gs >= 1E-06)
				{
					this.gravity_gs.SetText(this.GravityText(FleetsScreenController.accelerationStr(this.shipyardIdx.hab.orbitState.localGravity_gs, false, false, true)));
					this.gravityTip.SetText("BodyText", Loc.T("UI.Space.OrbitAccelGs"));
					this.gravityTip.enabled = true;
					return;
				}
				this.gravity_gs.SetText(this.GravityText(Loc.T("UI.Space.Negligible")));
				this.gravityTip.enabled = false;
				return;
			}
			else
			{
				if (this.shipyardIdx.hab.ref_habSite.surfaceGravity_g >= 1E-06)
				{
					if (this.controller.constructionManagerSelectedDesign == null || this.controller.constructionManagerSelectedDesign.CanTakeOffFromSurfaceShipyard(this.shipyardIdx))
					{
						this.gravity_gs.SetText(this.GravityText(FleetsScreenController.accelerationStr(this.shipyardIdx.hab.ref_habSite.surfaceGravity_g, false, false, true)));
					}
					else
					{
						this.gravity_gs.SetText(this.GravityText(TIUtilities.RedLine(FleetsScreenController.accelerationStr(this.shipyardIdx.hab.ref_habSite.surfaceGravity_g, false, false, true))));
					}
					this.gravityTip.SetText("BodyText", Loc.T("UI.Fleets.ShipyardShipMinimums", new object[]
					{
						FleetsScreenController.accelerationStr(this.shipyardIdx.hab.ref_habSite.surfaceGravity_g, false, false, true),
						TIUtilities.FormatSmallNumber(this.shipyardIdx.sector.hab.habSite.MinDeltaVToLaunch_kps((this.controller.constructionManagerSelectedDesign == null) ? ((float)this.shipyardIdx.hab.ref_habSite.surfaceGravity_mps2) : this.controller.constructionManagerSelectedDesign.baseCombatAcceleration_mps2), 7, 0, true, false)
					}));
					this.gravityTip.enabled = true;
					return;
				}
				this.gravity_gs.SetText(this.GravityText(Loc.T("UI.Space.Negligible")));
				this.gravityTip.enabled = false;
				return;
			}
		}

		// Token: 0x06004E76 RID: 20086 RVA: 0x0021C394 File Offset: 0x0021A594
		public void UpdateConstructionQueue()
		{
			List<ShipConstructionQueueItem> shipyardQueue = this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx);
			this.queueList.SetListSize<ShipConstructionQueueListItemController>(shipyardQueue.Count, false, false);
			if (this.defaultButtonSprite == null && shipyardQueue.Count > 0)
			{
				this.defaultButtonSprite = this.queueList.transform.GetChild(0).GetComponent<ShipConstructionQueueListItemController>().button.image.sprite;
			}
			int num = 0;
			using (IEnumerator<object> enumerator = this.queueList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ShipyardGridItemController.<>o__32.<>p__0 == null)
					{
						ShipyardGridItemController.<>o__32.<>p__0 = CallSite<Func<CallSite, object, ShipConstructionQueueListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipConstructionQueueListItemController), typeof(ShipyardGridItemController)));
					}
					ShipConstructionQueueListItemController shipConstructionQueueListItemController = ShipyardGridItemController.<>o__32.<>p__0.Target(ShipyardGridItemController.<>o__32.<>p__0, enumerator.Current);
					shipConstructionQueueListItemController.Init(this, this.defaultButtonSprite);
					shipConstructionQueueListItemController.UpdateListItem(shipyardQueue[num], num);
					num++;
				}
			}
			if (base.gameObject.activeSelf)
			{
				base.StartCoroutine(this.SetQueueScrollRect(shipyardQueue.Count > 4));
			}
		}

		// Token: 0x06004E77 RID: 20087 RVA: 0x0021C4C8 File Offset: 0x0021A6C8
		private void SetAddClearShipButtons()
		{
			this.AddShipButton.interactable = this.CanAddShip();
			bool flag = this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx).Count > 0;
			if (this.AddShipButton.interactable)
			{
				float num = -1f;
				if (this.refitting)
				{
					num = this.controller.designToRefitTo.RefitResourceCost(this.shipyardIdx, this.controller.shipSelectedForRefit.template, true, true, this.controller.shipSelectedForRefit).completionTime_days;
				}
				else if (this.controller.constructionManagerSelectedDesign != null)
				{
					num = this.controller.constructionManagerSelectedDesign.hullTemplate.constructionTime_Days(this.shipyardIdx);
				}
				if (num != -1f)
				{
					if (flag)
					{
						this.AddToQueueButtonText.SetText(Loc.T("UI.Fleets.AddToConstructionQueueButtonDays", new object[] { num.ToString("N0") }));
					}
					else if (this.refitting)
					{
						this.AddToQueueButtonText.SetText(Loc.T("UI.Fleets.RefitDays", new object[] { num.ToString("N0") }));
					}
					else
					{
						this.AddToQueueButtonText.SetText(Loc.T("UI.Fleets.BuildDays", new object[] { num.ToString("N0") }));
					}
				}
				else
				{
					this.AddToQueueButtonText.SetText(Loc.T("UI.Fleets.AddToConstructionQueueButton"));
				}
			}
			else
			{
				this.AddToQueueButtonText.SetText(Loc.T("UI.Fleets.AddToConstructionQueueButton"));
			}
			this.ClearQueueButton.interactable = flag;
		}

		// Token: 0x06004E78 RID: 20088 RVA: 0x0021C65C File Offset: 0x0021A85C
		public void SetButtons()
		{
			this.SetAddClearShipButtons();
			this.RemoveShipButton.interactable = this.CanRemoveFromQueue;
			this.MoveUpinQueueButton.interactable = this.CanMoveUpInQueue;
			this.MoveDownInQueueButton.interactable = this.CanMoveDownInQueue;
		}

		// Token: 0x06004E79 RID: 20089 RVA: 0x0021C697 File Offset: 0x0021A897
		public void SetSelectedQueueItem(ShipConstructionQueueItem item)
		{
			this.controller.SetSelectedConstructionQueueItem(item);
			this.SetButtons();
		}

		// Token: 0x17000EB7 RID: 3767
		// (get) Token: 0x06004E7A RID: 20090 RVA: 0x0021C6AB File Offset: 0x0021A8AB
		private bool refitting
		{
			get
			{
				return this.controller.refitScrollviews.activeSelf && this.controller.designToRefitTo != null && TIGameState.Valid(this.controller.shipSelectedForRefit);
			}
		}

		// Token: 0x06004E7B RID: 20091 RVA: 0x0021C6E0 File Offset: 0x0021A8E0
		public bool CanAddShip()
		{
			if (!this.controller.refitScrollviews.activeSelf)
			{
				return this.controller.constructionManagerSelectedDesign != null && this.controller.constructionManagerSelectedDesign.CanBuildAtShipyard(this.shipyardIdx.sector.habModules[this.shipyardIdx.slot]);
			}
			return this.controller.shipSelectedForRefit != null && this.controller.designToRefitTo != null;
		}

		// Token: 0x06004E7C RID: 20092 RVA: 0x0021C764 File Offset: 0x0021A964
		public void OnAddToQueueButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_InitiateBuildShip", false, false);
			bool refitting = this.refitting;
			TISpaceShipTemplate tispaceShipTemplate = null;
			TISpaceShipState tispaceShipState = null;
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			List<TIHabModuleState> list2 = new List<TIHabModuleState>();
			bool flag = refitting && this.controller.multiSelectedRefitShips.Count > 1;
			if (refitting)
			{
				tispaceShipTemplate = this.controller.originalShipTemplate;
				tispaceShipState = this.controller.shipSelectedForRefit;
				if (flag)
				{
					list2 = (from o in this.shipyardIdx.hab.CompletedShipyards()
						where !o.underConstruction && o.currentShipConstructionQueueItem == null
						select o).ToList<TIHabModuleState>();
					if (list2.Count == 0)
					{
						list2.Add(this.shipyardIdx);
					}
					foreach (TISpaceShipState tispaceShipState2 in this.controller.multiSelectedRefitShips)
					{
						list.Add(tispaceShipState2);
					}
				}
				this.controller.multiSelectedRefitShips.Clear();
				this.controller.designToRefitTo = null;
				this.controller.shipSelectedForRefit = null;
				this.controller.DeSelectRefitClasses();
			}
			if (flag)
			{
				int num = 0;
				using (List<TISpaceShipState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState3 = enumerator.Current;
						this.controller.activePlayer.playerControl.StartAction(new AddShipDesignToConstructionQueueAction(list2[num++], this.controller.constructionManagerSelectedDesign, this.allowPayFromEarth, 1f, null, refitting, tispaceShipTemplate, tispaceShipState3));
						if (num > list2.Count - 1)
						{
							num = 0;
						}
					}
					goto IL_01E9;
				}
			}
			this.controller.activePlayer.playerControl.StartAction(new AddShipDesignToConstructionQueueAction(this.shipyardIdx, this.controller.constructionManagerSelectedDesign, this.allowPayFromEarth, 1f, null, refitting, tispaceShipTemplate, tispaceShipState));
			IL_01E9:
			this.UpdateGridItem();
			if (this.controller.activePlayer.nShipyardQueues[this.shipyardIdx].Count > 0)
			{
				this.SetSelectedQueueItem(this.controller.activePlayer.nShipyardQueues[this.shipyardIdx].Last<ShipConstructionQueueItem>());
			}
			this.controller.RefreshConstructionManager();
		}

		// Token: 0x06004E7D RID: 20093 RVA: 0x0021C9D0 File Offset: 0x0021ABD0
		public void OnNewClassSelectedInFleetController()
		{
			this.SetAddClearShipButtons();
			this.UpdateGravityIcon();
		}

		// Token: 0x06004E7E RID: 20094 RVA: 0x0021C9DE File Offset: 0x0021ABDE
		public void OnNewConstructionQueueItemSelected()
		{
			this.UpdateConstructionQueue();
			this.SetButtons();
		}

		// Token: 0x06004E7F RID: 20095 RVA: 0x0021C9EC File Offset: 0x0021ABEC
		public void ToggleUseEarthResources()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.activePlayer.playerControl.StartAction(new UpdatePayMethodForConstructionQueueAction(this.shipyardIdx, this.allowPayFromEarthToggle.isOn));
			this.UpdateGridItem();
		}

		// Token: 0x06004E80 RID: 20096 RVA: 0x0021CA2C File Offset: 0x0021AC2C
		public void OnClearQueueButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			List<RemoveShipFromShipyardQueueAction> list = new List<RemoveShipFromShipyardQueueAction>();
			List<ShipConstructionQueueItem> shipyardQueue = this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx);
			for (int i = ((shipyardQueue.Count == 1) ? 0 : 1); i < shipyardQueue.Count; i++)
			{
				list.Add(new RemoveShipFromShipyardQueueAction(this.shipyardIdx, shipyardQueue[i]));
				if (shipyardQueue[i] == this.selectedQueueItem)
				{
					this.SetSelectedQueueItem(null);
				}
			}
			foreach (RemoveShipFromShipyardQueueAction removeShipFromShipyardQueueAction in list)
			{
				this.controller.activePlayer.playerControl.StartAction(removeShipFromShipyardQueueAction);
			}
			this.UpdateGridItem();
		}

		// Token: 0x06004E81 RID: 20097 RVA: 0x0021CB08 File Offset: 0x0021AD08
		public void OnHabModuleClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			GameControl.eventManager.TriggerEvent(new HabDetailRequested(this.shipyardIdx.ref_hab, true), null, Array.Empty<object>());
		}

		// Token: 0x17000EB8 RID: 3768
		// (get) Token: 0x06004E82 RID: 20098 RVA: 0x0021CB37 File Offset: 0x0021AD37
		public ShipConstructionQueueItem selectedQueueItem
		{
			get
			{
				return this.controller.constructionManagerSelectedQueueItem;
			}
		}

		// Token: 0x17000EB9 RID: 3769
		// (get) Token: 0x06004E83 RID: 20099 RVA: 0x0021CB44 File Offset: 0x0021AD44
		public int selectedItemIndex
		{
			get
			{
				if (this.selectedQueueItem == null)
				{
					return -1;
				}
				return this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx).IndexOf(this.selectedQueueItem);
			}
		}

		// Token: 0x17000EBA RID: 3770
		// (get) Token: 0x06004E84 RID: 20100 RVA: 0x0021CB74 File Offset: 0x0021AD74
		public bool CanMoveDownInQueue
		{
			get
			{
				return this.selectedQueueItem != null && this.selectedItemIndex >= 0 && this.selectedItemIndex < this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx).Count - 1 && !this.selectedQueueItem.costPaid;
			}
		}

		// Token: 0x17000EBB RID: 3771
		// (get) Token: 0x06004E85 RID: 20101 RVA: 0x0021CBC8 File Offset: 0x0021ADC8
		public bool CanMoveUpInQueue
		{
			get
			{
				return this.selectedQueueItem != null && (this.selectedItemIndex > 1 || (this.selectedItemIndex == 1 && !this.controller.activePlayer.GetShipyardQueue(this.shipyardIdx)[0].costPaid));
			}
		}

		// Token: 0x06004E86 RID: 20102 RVA: 0x0021CC1C File Offset: 0x0021AE1C
		public void OnMoveUpInQueueClicked()
		{
			if (this.CanMoveUpInQueue)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				this.controller.activePlayer.playerControl.StartAction(new RepositionShipinConstructionQueueAction(this.shipyardIdx, this.selectedQueueItem, this.selectedItemIndex - 1));
				this.UpdateGridItem();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004E87 RID: 20103 RVA: 0x0021CC80 File Offset: 0x0021AE80
		public void OnMoveDowninQueueClicked()
		{
			if (this.CanMoveDownInQueue)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				this.controller.activePlayer.playerControl.StartAction(new RepositionShipinConstructionQueueAction(this.shipyardIdx, this.selectedQueueItem, this.selectedItemIndex + 1));
				this.UpdateGridItem();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x17000EBC RID: 3772
		// (get) Token: 0x06004E88 RID: 20104 RVA: 0x0021CCE2 File Offset: 0x0021AEE2
		public bool CanRemoveFromQueue
		{
			get
			{
				return this.selectedQueueItem != null && this.selectedItemIndex != -1;
			}
		}

		// Token: 0x06004E89 RID: 20105 RVA: 0x0021CCFC File Offset: 0x0021AEFC
		public void OnRemoveFromQueueClicked()
		{
			if (this.CanRemoveFromQueue)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				this.controller.activePlayer.playerControl.StartAction(new RemoveShipFromShipyardQueueAction(this.shipyardIdx, this.selectedQueueItem));
				this.SetSelectedQueueItem(null);
				this.UpdateGridItem();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06004E8A RID: 20106 RVA: 0x0021CD5D File Offset: 0x0021AF5D
		private IEnumerator SetQueueScrollRect(bool enable)
		{
			yield return null;
			yield return null;
			this.shipQueueScrollRect.enabled = enable;
			yield break;
		}

		// Token: 0x0400320B RID: 12811
		public FleetsScreenController controller;

		// Token: 0x0400320C RID: 12812
		public TMP_Text habName;

		// Token: 0x0400320D RID: 12813
		public TMP_Text habLocation;

		// Token: 0x0400320E RID: 12814
		public TMP_Text shipyardDetails;

		// Token: 0x0400320F RID: 12815
		public Image shipyardTier;

		// Token: 0x04003210 RID: 12816
		public TMP_Text currentConstructionTitle;

		// Token: 0x04003211 RID: 12817
		public TMP_Text constructionQueueTitle;

		// Token: 0x04003212 RID: 12818
		public TMP_Text currentConstruction;

		// Token: 0x04003213 RID: 12819
		public Image moduleImage;

		// Token: 0x04003214 RID: 12820
		public TIHabModuleState shipyardIdx;

		// Token: 0x04003215 RID: 12821
		public Image backgroundFactionGradient;

		// Token: 0x04003216 RID: 12822
		public TMP_Text AddToQueueButtonText;

		// Token: 0x04003217 RID: 12823
		public TMP_Text ClearQueueButtonText;

		// Token: 0x04003218 RID: 12824
		public ListManagerBase queueList;

		// Token: 0x04003219 RID: 12825
		public ScrollRect shipQueueScrollRect;

		// Token: 0x0400321A RID: 12826
		public Button AddShipButton;

		// Token: 0x0400321B RID: 12827
		public Button RemoveShipButton;

		// Token: 0x0400321C RID: 12828
		public Button ClearQueueButton;

		// Token: 0x0400321D RID: 12829
		public Button MoveUpinQueueButton;

		// Token: 0x0400321E RID: 12830
		public Button MoveDownInQueueButton;

		// Token: 0x0400321F RID: 12831
		public Toggle allowPayFromEarthToggle;

		// Token: 0x04003220 RID: 12832
		public TooltipTrigger payFromEarthTooltip;

		// Token: 0x04003221 RID: 12833
		public TMP_Text gravity_gs;

		// Token: 0x04003222 RID: 12834
		public TooltipTrigger gravityTip;

		// Token: 0x04003223 RID: 12835
		private Sprite defaultButtonSprite;
	}
}
