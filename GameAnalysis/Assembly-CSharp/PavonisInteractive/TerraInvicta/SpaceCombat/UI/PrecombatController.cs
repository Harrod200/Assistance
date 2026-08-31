using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A02 RID: 2562
	public class PrecombatController : CanvasControllerBase, IHud, ICanvas
	{
		// Token: 0x170010FC RID: 4348
		// (get) Token: 0x06006272 RID: 25202 RVA: 0x002E242E File Offset: 0x002E062E
		// (set) Token: 0x06006273 RID: 25203 RVA: 0x002E2436 File Offset: 0x002E0636
		public TISpaceCombatState combat { get; private set; }

		// Token: 0x170010FD RID: 4349
		// (get) Token: 0x06006274 RID: 25204 RVA: 0x002E243F File Offset: 0x002E063F
		private TIHabState activePlayerHab
		{
			get
			{
				if (!(this.combat.hab != null) || !(this.combat.hab.ref_faction == base.activePlayer))
				{
					return null;
				}
				return this.combat.hab;
			}
		}

		// Token: 0x170010FE RID: 4350
		// (get) Token: 0x06006275 RID: 25205 RVA: 0x002E247E File Offset: 0x002E067E
		private bool side0HabOnly
		{
			get
			{
				return this.combat.fleets[0] == null;
			}
		}

		// Token: 0x170010FF RID: 4351
		// (get) Token: 0x06006276 RID: 25206 RVA: 0x002E2493 File Offset: 0x002E0693
		private bool side1HabOnly
		{
			get
			{
				return this.combat.fleets[1] == null;
			}
		}

		// Token: 0x17001100 RID: 4352
		// (get) Token: 0x06006277 RID: 25207 RVA: 0x002E24A8 File Offset: 0x002E06A8
		public bool activePlayerCombat
		{
			get
			{
				return this.combat.IncludesFaction(base.activePlayer);
			}
		}

		// Token: 0x06006278 RID: 25208 RVA: 0x002E24BC File Offset: 0x002E06BC
		public override void Initialize()
		{
			base.Initialize();
			GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.OnCombatInitiated), null, null, false, false);
			GameControl.eventManager.AddListener<CombatEnds>(new EventManager.EventDelegate<CombatEnds>(this.OnCombatComplete), null, null, false, false);
			GameControl.eventManager.AddListener<CombatSimulationUpdated>(new EventManager.EventDelegate<CombatSimulationUpdated>(this.OnCombatSimulationUpdated), null, null, false, false);
			this.precombatHeader.SetText(Loc.T("UI.Precombat.Header"));
			this.engageButtonText.SetText(Loc.T("UI.Precombat.Engage"));
			this.engageButtonDescription.SetText(Loc.T("UI.Precombat.EngageDetail"));
			this.acceptButtonText.SetText(Loc.T("UI.Precombat.Accept"));
			this.acceptButtonDescription.SetText(Loc.T("UI.Precombat.AcceptDetail"));
			this.evadeButtonText.SetText(Loc.T("UI.Precombat.Flee"));
			this.evadeButtonDescription.SetText(Loc.T("UI.Precombat.FleeDetail"));
			this.bidHeader.SetText(Loc.T("UI.Precombat.BiddingHeader"));
			this.autoResolveButtonText.SetText(Loc.T("UI.Precombat.Autoresolve"));
			this.closeButtonText.SetText(Loc.T("UI.Precombat.Close"));
			this.postCombatCloseButtonText.SetText(Loc.T("UI.Precombat.Close"));
			this.postCombatContinueText.SetText(Loc.T("UI.Notifications.ContinueButtonText"));
			this.postCombatGotoButtonText.SetText(Loc.T("UI.Notifications.GotoButtonText"));
			this.acceptAutoresolveButtonText.SetText(Loc.T("UI.Precombat.AcceptAutoresolve"));
			this.rejectAutoresolveButtonText.SetText(Loc.T("UI.Precombat.RejectAutoresolve"));
			this.addFightersButtonText.SetText(Loc.T("UI.Precombat.AddFightersButton"));
			this.addFightersButtonExplainer.SetText(Loc.T("UI.Precombat.AddFightersDetail", new object[] { TIGlobalConfig.globalConfig.boostInlineSpritePath }));
			this.STOFightersHeader.SetText(Loc.T("UI.Precombat.STOFightersHeader"));
			this.STOFightersExplainer.SetText(Loc.T("UI.Precombat.STOFightersExplainer"));
			this.STOFightersResetButtonText.SetText(Loc.T("UI.Precombat.STOFightersResetButtonText"));
			this.STOFightersLaunchEverybodyButtonText.SetText(Loc.T("UI.Precombat.STOFightersLaunchEverybodyButtonText"));
			this.STOFightersCloseButtonText.SetText(Loc.T("UI.Precombat.Confirm"));
			this.cancelAttackButtonText.SetText(Loc.T("UI.Precombat.CancelLaunch"));
			this.cancelAttackButtonExplainer.SetText(Loc.T("UI.Precombat.CancelLaunchExplainer"));
			this.postCombatUIObject.SetActive(false);
			this.STOFightersCanvasObject.SetActive(false);
		}

		// Token: 0x06006279 RID: 25209 RVA: 0x002E2740 File Offset: 0x002E0940
		public override void Refresh()
		{
			base.Refresh();
			if (this.progressBar.gameObject.activeInHierarchy)
			{
				float num = Mathf.Max(Mathf.Pow(this.combatProgress, 1f - this.combatProgress), 0.025f);
				float num2 = Mathf.Lerp(this.progressBar.anchorMin.x, 1f - this.progressBar.anchorMin.x, num);
				float num3 = Mathf.Lerp(this.progressBar.anchorMax.x, num2, Time.deltaTime * 1f);
				this.progressBar.anchorMax = new Vector2(num3, this.progressBar.anchorMax.y);
			}
		}

		// Token: 0x0600627A RID: 25210 RVA: 0x002E27FC File Offset: 0x002E09FC
		private void OnCombatInitiated(SpaceCombatInitiated e)
		{
			if (this.postCombatUIObject.activeSelf)
			{
				this.delayedCombatInitiationEvent = e;
				return;
			}
			this.postCombatUIObject.SetActive(false);
			this.extendedPursuitObject.SetActive(false);
			this.combat = e.combat;
			if (GameControl.control.skirmishMode)
			{
				this.preCombatUIObject.SetActive(false);
				return;
			}
			this.preCombatUIObject.SetActive(true);
			this.extendedPursuitToggle.SetIsOnWithoutNotify(false);
			if (this.activePlayerCombat)
			{
				TISpaceFleetState tispaceFleetState = this.combat.fleets[0];
				TIFactionState tifactionState = ((tispaceFleetState != null) ? tispaceFleetState.faction : null) ?? this.combat.hab.faction;
				TISpaceFleetState tispaceFleetState2 = this.combat.fleets[1];
				if (((tispaceFleetState2 != null) ? tispaceFleetState2.faction : null) == null)
				{
					TIFactionState faction = this.combat.hab.faction;
				}
				TIPromptQueueState.AddPromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
				this.FillOutCombatData();
				base.gameTime.PreserveStrategySpeed();
				base.gameTime.Pause();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatAlert", false, false);
				this.activePlayerFleet = ((tifactionState == base.activePlayer) ? this.combat.fleets[0] : this.combat.fleets[1]);
				this.otherFleet = ((tifactionState == base.activePlayer) ? this.combat.fleets[1] : this.combat.fleets[0]);
				this.OpenStanceUI();
				this.Show();
			}
			if (!this.combat.HaveStancesBeenSelected)
			{
				GameControl.eventManager.AddListener<CombatStanceSelected>(new EventManager.EventDelegate<CombatStanceSelected>(this.OnCombatStanceSelected), null, null, false, false);
				return;
			}
			if (this.combat.requiresBidding && !this.combat.HaveBidsBeenSubmitted)
			{
				GameControl.eventManager.AddListener<CombatBidSelected>(new EventManager.EventDelegate<CombatBidSelected>(this.OnCombatBidSelected), null, null, false, false);
				return;
			}
			this.EndPrecombatInteraction();
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x002E29EC File Offset: 0x002E0BEC
		private void OnCombatStanceSelected(CombatStanceSelected e)
		{
			if (this.combat.HaveStancesBeenSelected)
			{
				this.FillOutCombatData();
				GameControl.eventManager.RemoveListener<CombatStanceSelected>(new EventManager.EventDelegate<CombatStanceSelected>(this.OnCombatStanceSelected), null);
				if (this.activePlayerCombat)
				{
					if (this.combat.requiresBidding)
					{
						this.OpenBiddingUI();
						return;
					}
					this.OpenResolutionUI();
					return;
				}
				else
				{
					if (this.combat.requiresBidding)
					{
						GameControl.eventManager.AddListener<CombatBidSelected>(new EventManager.EventDelegate<CombatBidSelected>(this.OnCombatBidSelected), null, null, false, false);
						return;
					}
					this.EndPrecombatInteraction();
				}
			}
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x002E2A74 File Offset: 0x002E0C74
		private void OnCombatBidSelected(CombatBidSelected e)
		{
			if (this.combat.HaveBidsBeenSubmitted)
			{
				GameControl.eventManager.RemoveListener<CombatBidSelected>(new EventManager.EventDelegate<CombatBidSelected>(this.OnCombatBidSelected), null);
				if (this.activePlayerCombat)
				{
					this.OpenResolutionUI();
					return;
				}
				this.EndPrecombatInteraction();
			}
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x002E2AB0 File Offset: 0x002E0CB0
		private void FillOutFleetValues(TISpaceFleetState fleet1, TISpaceFleetState fleet2, TIHabState hab1, TIHabState hab2, bool envelop = false, List<TISpaceShipState> overrideShips = null, int overrideShipsFleet = -1)
		{
			if (fleet1 != null)
			{
				this.fleet1Header.SetText(fleet1.dummyFleet ? fleet1.GetDisplayName(base.activePlayer) : Loc.T("UI.Precombat.FleetName", new object[]
				{
					fleet1.faction.adjective,
					fleet1.GetDisplayName(base.activePlayer)
				}));
				this.fleet1Icon.sprite = fleet1.icon;
				if (overrideShips != null && overrideShipsFleet == 1)
				{
					this.fleet1ShipsCount.SetText(overrideShips.Count.ToString());
					this.fleet1CombatScore.SetText(TIUtilities.FormatBigNumber((double)overrideShips.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)), 1, false));
					TMP_Text tmp_Text = this.fleet1DeltaV;
					string text = "UI.Fleets.SingleDV";
					object[] array = new object[1];
					array[0] = TIUtilities.FormatBigNumber((double)overrideShips.Min<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps), 1, false);
					tmp_Text.SetText(Loc.T(text, array));
					this.fleet1Accel.SetText(FleetsScreenController.accelerationStr((double)overrideShips.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_gs), true, false, true));
					if (this.activePlayerFleet == fleet1)
					{
						this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(TISpaceCombatState.ExtendedDVBurn_kps(overrideShips, fleet1, fleet2, envelop), 1, 7, 0, false, false) }));
					}
					else
					{
						this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
					}
				}
				else
				{
					int num = fleet1.ships.Count;
					float num2 = fleet1.SpaceCombatValue();
					float num3 = fleet1.currentDeltaV_kps;
					float num4 = fleet1.pursuitAcceleration_gs;
					if (hab1 != null)
					{
						num2 += hab1.SpaceCombatValue();
					}
					if (this.showFleet1FighterTemplates)
					{
						if (this.combat.STOFighterPlans[fleet1.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count) > 0)
						{
							num += this.combat.STOFighterPlans[fleet1.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count);
							num2 += this.combat.STOFighterPlans[fleet1.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.TemplateSpaceCombatValue(false, -1f, 1f, false) * (float)x.Value.count);
							if (fleet1.ships.Count > 0)
							{
								num3 = Mathf.Min(num3, this.combat.STOFighterPlans[fleet1.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCruiseDeltaV_kps(false)));
								num4 = Mathf.Min(num4, this.combat.STOFighterPlans[fleet1.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCombatAcceleration_gs));
							}
							else
							{
								num3 = this.combat.STOFighterPlans[fleet1.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCruiseDeltaV_kps(false));
								num4 = this.combat.STOFighterPlans[fleet1.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCombatAcceleration_gs);
							}
						}
					}
					this.fleet1ShipsCount.SetText(num.ToString());
					string text2 = TIUtilities.FormatBigNumber((double)num2, 1, false);
					this.fleet1CombatScore.SetText(text2);
					this.fleet1DeltaV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { TIUtilities.FormatBigNumber((double)num3, 1, false) }));
					this.fleet1Accel.SetText(FleetsScreenController.accelerationStr((double)num4, true, false, true));
					if (this.activePlayerFleet == fleet1)
					{
						this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
					}
				}
			}
			else
			{
				this.fleet1Header.SetText(hab1.GetDisplayName(base.activePlayer));
				this.fleet1Icon.sprite = hab1.icon;
				this.fleet1ShipsCount.SetText("0");
				this.fleet1CombatScore.SetText(TIUtilities.FormatBigNumber((double)hab1.SpaceCombatValue(), 1, false));
				this.fleet1DeltaV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { "0" }));
				this.fleet1Accel.SetText(Loc.T("UI.Fleets.Accelgs", new object[] { "0" }));
			}
			if (fleet2 != null && (fleet2.ships.Count > 0 || this.combat.allowNoAttackingFleetAtInitialization))
			{
				this.fleet2Header.SetText(fleet2.dummyFleet ? fleet2.GetDisplayName(base.activePlayer) : Loc.T("UI.Precombat.FleetName", new object[]
				{
					fleet2.faction.adjective,
					fleet2.GetDisplayName(base.activePlayer)
				}));
				this.fleet2Icon.sprite = fleet2.icon;
				if (overrideShips != null && overrideShipsFleet == 2)
				{
					this.fleet2ShipsCount.SetText(overrideShips.Count.ToString());
					this.fleet2CombatScore.SetText(TIUtilities.FormatBigNumber((double)overrideShips.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)), 1, false));
					TMP_Text tmp_Text2 = this.fleet2DeltaV;
					string text3 = "UI.Fleets.SingleDV";
					object[] array2 = new object[1];
					array2[0] = TIUtilities.FormatBigNumber((double)overrideShips.Min<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps), 1, false);
					tmp_Text2.SetText(Loc.T(text3, array2));
					this.fleet2Accel.SetText(FleetsScreenController.accelerationStr((double)overrideShips.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_gs), true, false, true));
					if (this.activePlayerFleet == fleet2)
					{
						this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(TISpaceCombatState.ExtendedDVBurn_kps(overrideShips, fleet2, fleet1, envelop), 1, 7, 0, false, false) }));
						return;
					}
					this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
					return;
				}
				else
				{
					int num5 = fleet2.ships.Count;
					float num6 = fleet2.SpaceCombatValue();
					float num7 = fleet2.currentDeltaV_kps;
					float num8 = fleet2.pursuitAcceleration_gs;
					if (hab2 != null)
					{
						num6 += hab2.SpaceCombatValue();
					}
					if (this.showFleet2FighterTemplates)
					{
						if (this.combat.STOFighterPlans[fleet2.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count) > 0)
						{
							num5 += this.combat.STOFighterPlans[fleet2.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count);
							num6 += this.combat.STOFighterPlans[fleet2.faction].Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.TemplateSpaceCombatValue(false, -1f, 1f, false) * (float)x.Value.count);
							if (fleet2.ships.Count > 0)
							{
								num7 = Mathf.Min(num7, this.combat.STOFighterPlans[fleet2.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCruiseDeltaV_kps(false)));
								num8 = Mathf.Min(num8, this.combat.STOFighterPlans[fleet2.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCombatAcceleration_gs));
							}
							else
							{
								num7 = this.combat.STOFighterPlans[fleet2.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCruiseDeltaV_kps(false));
								num8 = this.combat.STOFighterPlans[fleet2.faction].Where<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count > 0).Min<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.fighter.baseCombatAcceleration_gs);
							}
						}
					}
					this.fleet2ShipsCount.SetText(num5.ToString());
					string text4 = TIUtilities.FormatBigNumber((double)num6, 1, false);
					this.fleet2CombatScore.SetText(text4);
					this.fleet2DeltaV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { TIUtilities.FormatBigNumber((double)num7, 1, false) }));
					this.fleet2Accel.SetText(FleetsScreenController.accelerationStr((double)num8, true, false, true));
					if (this.activePlayerFleet == fleet2)
					{
						this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
						return;
					}
				}
			}
			else
			{
				this.fleet2Header.SetText(this.combat.hab.GetDisplayName(base.activePlayer));
				this.fleet2Icon.sprite = this.combat.hab.icon;
				this.fleet2ShipsCount.SetText("0");
				this.fleet2CombatScore.SetText(TIUtilities.FormatBigNumber((double)this.combat.hab.SpaceCombatValue(), 1, false));
				this.fleet2DeltaV.SetText(Loc.T("UI.Fleets.SingleDV", new object[] { "0" }));
				this.fleet2Accel.SetText(Loc.T("UI.Fleets.Accelgs", new object[] { "0" }));
			}
		}

		// Token: 0x17001101 RID: 4353
		// (get) Token: 0x0600627E RID: 25214 RVA: 0x002E3664 File Offset: 0x002E1864
		private bool showFleet1FighterTemplates
		{
			get
			{
				TISpaceFleetState tispaceFleetState = this.combat.fleets[1];
				TISpaceFleetState tispaceFleetState2 = ((((tispaceFleetState != null) ? tispaceFleetState.faction : null) == base.activePlayer) ? this.combat.fleets[1] : this.combat.fleets[0]);
				if (this.combat.fightersInitialized || (!(tispaceFleetState2.faction == base.activePlayer) && !this.bidSelectionObject.activeInHierarchy && !this.resolutionSelectionObject.activeInHierarchy) || !this.combat.STOFighterPlans.ContainsKey(tispaceFleetState2.faction))
				{
					return false;
				}
				Dictionary<TINationState, PlannedFighters> dictionary = this.combat.STOFighterPlans[tispaceFleetState2.faction];
				if (dictionary == null)
				{
					return false;
				}
				return dictionary.Values.Sum<PlannedFighters>((PlannedFighters x) => x.count) > 0;
			}
		}

		// Token: 0x17001102 RID: 4354
		// (get) Token: 0x0600627F RID: 25215 RVA: 0x002E3754 File Offset: 0x002E1954
		private bool showFleet2FighterTemplates
		{
			get
			{
				TISpaceFleetState tispaceFleetState = this.combat.fleets[1];
				TISpaceFleetState tispaceFleetState2 = ((((tispaceFleetState != null) ? tispaceFleetState.faction : null) == base.activePlayer) ? this.combat.fleets[0] : this.combat.fleets[1]);
				if (this.combat.fightersInitialized || !(tispaceFleetState2 != null) || (!(tispaceFleetState2.faction == base.activePlayer) && !this.bidSelectionObject.activeInHierarchy && !this.resolutionSelectionObject.activeInHierarchy) || !this.combat.STOFighterPlans.ContainsKey(tispaceFleetState2.faction))
				{
					return false;
				}
				Dictionary<TINationState, PlannedFighters> dictionary = this.combat.STOFighterPlans[tispaceFleetState2.faction];
				if (dictionary == null)
				{
					return false;
				}
				return dictionary.Values.Sum<PlannedFighters>((PlannedFighters x) => x.count) > 0;
			}
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x002E3850 File Offset: 0x002E1A50
		public void FillOutCombatData()
		{
			TISpaceFleetState tispaceFleetState = this.combat.fleets[1];
			TISpaceFleetState tispaceFleetState2 = ((((tispaceFleetState != null) ? tispaceFleetState.faction : null) == base.activePlayer) ? this.combat.fleets[1] : this.combat.fleets[0]);
			TISpaceFleetState tispaceFleetState3 = this.combat.fleets[1];
			TISpaceFleetState tispaceFleetState4 = ((((tispaceFleetState3 != null) ? tispaceFleetState3.faction : null) == base.activePlayer) ? this.combat.fleets[0] : this.combat.fleets[1]);
			TIHabState tihabState = null;
			TIHabState tihabState2 = null;
			if (this.combat.hab != null)
			{
				if (tispaceFleetState2 != null)
				{
					TIHabState hab = this.combat.hab;
					if (((hab != null) ? hab.ref_faction : null) == tispaceFleetState2.faction)
					{
						goto IL_00D3;
					}
				}
				if (!(tispaceFleetState2 == null))
				{
					tihabState2 = this.combat.hab;
					goto IL_00ED;
				}
				IL_00D3:
				tihabState = this.combat.hab;
			}
			IL_00ED:
			this.FillOutFleetValues(tispaceFleetState2, tispaceFleetState4, tihabState, tihabState2, false, null, -1);
			ListManagerBase listManagerBase = this.fleet1List;
			int num = tispaceFleetState2.ships.Count + ((tihabState != null) ? 1 : 0);
			int num2;
			if (!this.showFleet1FighterTemplates)
			{
				num2 = 0;
			}
			else
			{
				num2 = this.combat.STOFighterPlans[tispaceFleetState2.faction].Values.Sum<PlannedFighters>((PlannedFighters x) => x.count);
			}
			listManagerBase.SetListSize<PrecombatShipListItemController>(num + num2, false, false);
			List<TIDataClass> list = new List<TIDataClass>();
			if (tihabState != null)
			{
				list.Add(tihabState);
			}
			if (tispaceFleetState2 != null)
			{
				List<TISpaceShipState> list2 = (from x in tispaceFleetState2.ships
					orderby x.hull.length_m descending, x.dryMass_kg descending
					select x).ToList<TISpaceShipState>();
				list.AddRange(list2);
			}
			if (this.showFleet1FighterTemplates)
			{
				foreach (TINationState tinationState in this.combat.STOFighterPlans[tispaceFleetState2.faction].Keys)
				{
					for (int i = 0; i < this.combat.STOFighterPlans[tispaceFleetState2.faction][tinationState].count; i++)
					{
						list.Add(this.combat.STOFighterPlans[tispaceFleetState2.faction][tinationState].fighter);
					}
				}
			}
			int num3 = 0;
			using (IEnumerator<object> enumerator2 = this.fleet1List.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (PrecombatController.<>o__117.<>p__0 == null)
					{
						PrecombatController.<>o__117.<>p__0 = CallSite<Func<CallSite, object, PrecombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PrecombatShipListItemController), typeof(PrecombatController)));
					}
					PrecombatShipListItemController precombatShipListItemController = PrecombatController.<>o__117.<>p__0.Target(PrecombatController.<>o__117.<>p__0, enumerator2.Current);
					precombatShipListItemController.SetListItem(list[num3++]);
					precombatShipListItemController.SetListItemShading(false);
				}
			}
			ListManagerBase listManagerBase2 = this.fleet2List;
			int num4 = ((tispaceFleetState4 != null) ? tispaceFleetState4.ships.Count : 0) + ((tihabState2 != null) ? 1 : 0);
			int num5;
			if (!this.showFleet2FighterTemplates)
			{
				num5 = 0;
			}
			else
			{
				num5 = this.combat.STOFighterPlans[tispaceFleetState4.faction].Values.Sum<PlannedFighters>((PlannedFighters x) => x.count);
			}
			listManagerBase2.SetListSize<PrecombatShipListItemController>(num4 + num5, false, false);
			num3 = 0;
			List<TIGameState> list3 = new List<TIGameState>();
			if (tihabState2 != null)
			{
				list3.Add(tihabState2);
			}
			if (tispaceFleetState4 != null)
			{
				List<TISpaceShipState> list4 = (from x in tispaceFleetState4.ships
					orderby x.hull.length_m descending, x.dryMass_kg descending
					select x).ToList<TISpaceShipState>();
				list3.AddRange(list4);
			}
			if (this.showFleet2FighterTemplates)
			{
				foreach (TINationState tinationState2 in this.combat.STOFighterPlans[tispaceFleetState4.faction].Keys)
				{
					for (int j = 0; j < this.combat.STOFighterPlans[tispaceFleetState4.faction][tinationState2].count; j++)
					{
						list.Add(this.combat.STOFighterPlans[tispaceFleetState4.faction][tinationState2].fighter);
					}
				}
			}
			using (IEnumerator<object> enumerator2 = this.fleet2List.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (PrecombatController.<>o__117.<>p__1 == null)
					{
						PrecombatController.<>o__117.<>p__1 = CallSite<Func<CallSite, object, PrecombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PrecombatShipListItemController), typeof(PrecombatController)));
					}
					PrecombatController.<>o__117.<>p__1.Target(PrecombatController.<>o__117.<>p__1, enumerator2.Current).SetListItem(list3[num3++]);
				}
			}
		}

		// Token: 0x06006281 RID: 25217 RVA: 0x002E3DD4 File Offset: 0x002E1FD4
		public void OpenStanceUI()
		{
			this.stanceSelectionObject.SetActive(true);
			this.acceptButton.interactable = false;
			this.evadeButton.interactable = false;
			this.engageButton.interactable = false;
			using (List<CombatStance>.Enumerator enumerator = this.combat.allowedStances[base.activePlayer].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case CombatStance.Pursue:
						this.engageButton.interactable = true;
						break;
					case CombatStance.Defend:
						this.acceptButton.interactable = true;
						break;
					case CombatStance.Evade:
						this.evadeButton.interactable = true;
						break;
					}
				}
			}
			this.bidSelectionObject.SetActive(false);
			this.resolutionSelectionObject.SetActive(false);
			this.STOFightersCanvasObject.SetActive(false);
			if (this.combat.STOFighterEligibleCombat(base.activePlayer))
			{
				this.addFightersButton.gameObject.SetActive(true);
				if (this.combat.CanContributeSTOFightersToCombat(base.activePlayer))
				{
					this.InitSTOFighterController(base.activePlayer);
					this.addFightersButton.interactable = true;
					GameObject gameObject = this.cancelAttackButton.gameObject;
					bool flag;
					if (this.combat.attacker.faction == base.activePlayer && this.combat.allowNoAttackingFleetAtInitialization)
					{
						flag = this.combat.attacker.ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.hull.simpleHull).Count<TISpaceShipState>() == 0;
					}
					else
					{
						flag = false;
					}
					gameObject.SetActive(flag);
				}
				else
				{
					this.addFightersButton.interactable = false;
					this.cancelAttackButton.gameObject.SetActive(false);
				}
			}
			else
			{
				this.addFightersButton.gameObject.SetActive(false);
				this.cancelAttackButton.gameObject.SetActive(false);
			}
			TIFactionState tifactionState = this.combat.factions.First<TIFactionState>((TIFactionState x) => x != GameControl.control.activePlayer);
			if (this.combat.CanContributeSTOFightersToCombat(tifactionState))
			{
				if (this.combat.FleetFor(tifactionState).dummyFleet)
				{
					this.enemyFighterNotes.SetText(Loc.T("UI.Precombat.EmptyFighters"));
				}
				else
				{
					this.enemyFighterNotes.SetText(Loc.T("UI.Precombat.PossibleFighters"));
				}
				this.enemyFighterNotesObject.SetActive(true);
				return;
			}
			this.enemyFighterNotesObject.SetActive(false);
		}

		// Token: 0x17001103 RID: 4355
		// (get) Token: 0x06006282 RID: 25218 RVA: 0x002E406C File Offset: 0x002E226C
		private bool extendedPursuit
		{
			get
			{
				return this.extendedPursuitToggle.isOn && this.attackerPursuitShipsToForceCombat.Count > 0;
			}
		}

		// Token: 0x17001104 RID: 4356
		// (get) Token: 0x06006283 RID: 25219 RVA: 0x002E408B File Offset: 0x002E228B
		private CombatStance extensionStance
		{
			get
			{
				if (!this.extendedPursuit)
				{
					return CombatStance.NotYetSet;
				}
				if (!this.envelop)
				{
					return CombatStance.ExtendedPursuit_Stretch;
				}
				return CombatStance.ExtendedPursuit_Envelop;
			}
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x002E40A4 File Offset: 0x002E22A4
		public void OpenBiddingUI()
		{
			this.attackerPursuitShipsToForceCombat.Clear();
			this.bidSubmitButtonText.SetText(Loc.T("UI.Precombat.SubmitBid"));
			GameControl.eventManager.AddListener<CombatBidSelected>(new EventManager.EventDelegate<CombatBidSelected>(this.OnCombatBidSelected), null, null, false, false);
			bool flag = this.combat.stances[base.activePlayer] == CombatStance.Evade;
			this.bidDetail.SetText(Loc.T(flag ? "UI.Precombat.WereFleeing" : "UI.Precombat.WereChasing", new object[] { TISpaceCombatState.GetPursuitDistance_m(this.combat.fleets[0], this.combat.fleets[1]) / 1000f }));
			this.stanceSelectionObject.SetActive(false);
			this.bidSelectionObject.SetActive(true);
			this.resolutionSelectionObject.SetActive(false);
			this.sliderObject.SetActive(true);
			this.maxBidInfoObject.SetActive(true);
			this.currentDVBid_kps = 0f;
			this.deltaVBidSlider.value = 0f;
			this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
			TISpaceFleetState tispaceFleetState = this.combat.FleetFor(base.activePlayer);
			TISpaceFleetState tispaceFleetState2 = this.combat.FleetAgainst(base.activePlayer);
			this.maxDVBid_kps = this.combat.MaxDVBidForPursuit_mps(tispaceFleetState, tispaceFleetState2) / 1000f;
			this.deltaVBidSlider.maxValue = this.maxDVBid_kps;
			this.biddingTip.SetText("BodyText", TISpaceCombatState.UseFixedPursuitDistance ? Loc.T("UI.Precombat.ChaseMechanics_Fixed", new object[] { (-0.001f).ToString("N0") }) : Loc.T("UI.Precombat.ChaseMechanics", new object[]
			{
				200f.ToString("N0"),
				(TISpaceCombatState.GetPursuitDistance_m(tispaceFleetState, tispaceFleetState2) / 1000f).ToString("N0")
			}));
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Precombat.AvailableKps", new object[] { TIUtilities.FormatBigOrSmallNumber(this.maxDVBid_kps, 1, 7, 0, false, false) }));
			bool flag2 = TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(tispaceFleetState, tispaceFleetState2);
			this.envelop = false;
			if (flag && flag2)
			{
				this.attackerPursuitShipsToForceCombat = TISpaceCombatState.PursuerSubsetThatCanCatchEnemyFleet(tispaceFleetState2, tispaceFleetState, out this.envelop);
			}
			else if (!flag && !flag2)
			{
				this.attackerPursuitShipsToForceCombat = TISpaceCombatState.PursuerSubsetThatCanCatchEnemyFleet(tispaceFleetState, tispaceFleetState2, out this.envelop);
			}
			if (flag2)
			{
				if (flag)
				{
					stringBuilder.Append(TIUtilities.GreenLine(Loc.T("UI.Precombat.AlwaysFlee")));
					if (this.attackerPursuitShipsToForceCombat.Count > 0)
					{
						this.extendedPursuitObject.SetActive(true);
						this.extendedPursuitToggleObject.SetActive(false);
						this.extendedPursuitWarningRightIconObject.SetActive(true);
						this.extendedPursuitText.SetText(this.envelop ? Loc.T("UI.Precombat.AIExtendedPursuitEnvelop") : Loc.T("UI.Precombat.AIExtendedPursuitNormal"));
					}
				}
				else
				{
					stringBuilder.Append(TIUtilities.GreenLine(Loc.T("UI.Precombat.AlwaysCombat")));
				}
			}
			else if (flag)
			{
				stringBuilder.Append(TIUtilities.RedLine(Loc.T("UI.Precombat.EnemyAlwaysCombat")));
			}
			else
			{
				stringBuilder.Append(TIUtilities.RedLine(Loc.T("UI.Precombat.EnemyAlwaysFlee")));
				if (this.attackerPursuitShipsToForceCombat.Count > 0)
				{
					this.extendedPursuitObject.SetActive(true);
					this.extendedPursuitToggleObject.SetActive(true);
					this.extendedPursuitToggle.isOn = false;
					this.extendedPursuitWarningRightIconObject.SetActive(false);
					this.extendedPursuitText.SetText(this.envelop ? Loc.T("UI.Precombat.ExtendedPursuitEnvelop", new object[] { Loc.T("UI.Precombat.Pincer") }) : Loc.T("UI.Precombat.ExtendedPursuitNormal", new object[] { Loc.T("UI.Precombat.Pincer") }));
				}
			}
			this.maxBidValue.SetText(stringBuilder.ToString());
		}

		// Token: 0x06006285 RID: 25221 RVA: 0x002E4490 File Offset: 0x002E2690
		public void OnDVSliderSet()
		{
			this.currentDVBid_kps = this.deltaVBidSlider.value;
			this.currentBidValue.SetText(Loc.T("UI.Precombat.Ourbid", new object[] { TIUtilities.FormatBigOrSmallNumber(this.deltaVBidSlider.value, 1, 7, 0, false, false) }));
		}

		// Token: 0x06006286 RID: 25222 RVA: 0x002E44E4 File Offset: 0x002E26E4
		public void OnExtendedPursuitToggleChanged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			if (this.extendedPursuitToggle.isOn)
			{
				this.deltaVBidSlider.value = this.maxDVBid_kps;
				this.sliderObject.SetActive(false);
				this.maxBidInfoObject.SetActive(false);
				using (IEnumerator<object> enumerator = this.fleet1List.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PrecombatController.<>o__129.<>p__0 == null)
						{
							PrecombatController.<>o__129.<>p__0 = CallSite<Func<CallSite, object, PrecombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PrecombatShipListItemController), typeof(PrecombatController)));
						}
						PrecombatShipListItemController precombatShipListItemController = PrecombatController.<>o__129.<>p__0.Target(PrecombatController.<>o__129.<>p__0, enumerator.Current);
						precombatShipListItemController.SetListItemShading(!this.attackerPursuitShipsToForceCombat.Contains(precombatShipListItemController.gameState));
					}
				}
				this.bidSubmitButtonText.SetText(Loc.T("UI.Precombat.Pincer"));
				this.FillOutFleetValues(this.activePlayerFleet, this.otherFleet, this.activePlayerHab, (this.combat.hab != null && this.combat.hab.faction != base.activePlayer) ? this.combat.hab : null, this.envelop, this.attackerPursuitShipsToForceCombat, 1);
				return;
			}
			using (IEnumerator<object> enumerator = this.fleet1List.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PrecombatController.<>o__129.<>p__1 == null)
					{
						PrecombatController.<>o__129.<>p__1 = CallSite<Func<CallSite, object, PrecombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PrecombatShipListItemController), typeof(PrecombatController)));
					}
					PrecombatController.<>o__129.<>p__1.Target(PrecombatController.<>o__129.<>p__1, enumerator.Current).SetListItemShading(false);
				}
			}
			this.FillOutFleetValues(this.activePlayerFleet, this.otherFleet, this.activePlayerHab, (this.combat.hab != null && this.combat.hab.faction != base.activePlayer) ? this.combat.hab : null, false, null, -1);
			this.sliderObject.SetActive(true);
			this.maxBidInfoObject.SetActive(true);
			this.bidSubmitButtonText.SetText(Loc.T("UI.Precombat.SubmitBid"));
		}

		// Token: 0x06006287 RID: 25223 RVA: 0x002E4744 File Offset: 0x002E2944
		public void OpenResolutionUI()
		{
			if (this.combat.combatOccurs)
			{
				this.playBattleButtonText.SetText(Loc.T(this.activePlayerCombat ? "UI.Precombat.InitiateCombat" : "UI.Precombat.Watch"));
				this.autoResolveButton.SetActive(true);
				this.liveResolveButton.SetActive(true);
				this.closeButton.SetActive(false);
				if (this.combat.requiresBidding)
				{
					foreach (KeyValuePair<TIFactionState, CombatStance> keyValuePair in this.combat.stances)
					{
						Debug.Log(keyValuePair.Key.displayName + ": " + keyValuePair.Value.ToString());
					}
					TISpaceFleetState _chasingFleet = this.combat.chasingFleet;
					TISpaceFleetState tispaceFleetState = this.combat.fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.faction != _chasingFleet.faction);
					float num = this.combat.PrecombatDVSpend_kps(_chasingFleet.ships[0], _chasingFleet, tispaceFleetState, this.attackerPursuitShipsToForceCombat);
					float num2 = this.combat.PrecombatDVSpend_kps(tispaceFleetState.ships[0], _chasingFleet, tispaceFleetState, this.attackerPursuitShipsToForceCombat);
					StringBuilder stringBuilder = new StringBuilder();
					if (num == num2)
					{
						stringBuilder.AppendLine(Loc.T("UI.Precombat.Chase", new object[]
						{
							tispaceFleetState.GetDisplayName(base.activePlayer),
							_chasingFleet.GetDisplayName(base.activePlayer),
							Loc.T("UI.Precombat.Spent", new object[] { TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false) })
						}));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Precombat.Chase", new object[]
						{
							tispaceFleetState.GetDisplayName(base.activePlayer),
							_chasingFleet.GetDisplayName(base.activePlayer),
							Loc.T("UI.Precombat.SpentDiff", new object[]
							{
								TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false),
								TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false)
							})
						}));
					}
					if (this.combat.stances[_chasingFleet.faction] == CombatStance.ExtendedPursuit_Stretch)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Precombat.SuccessStretch"));
					}
					else if (this.combat.stances[_chasingFleet.faction] == CombatStance.ExtendedPursuit_Envelop)
					{
						stringBuilder.AppendLine().AppendLine(Loc.T("UI.Precombat.SuccessEnvelop"));
					}
					this.preCombatResults.SetText(stringBuilder.ToString());
				}
				else
				{
					StringBuilder stringBuilder2 = new StringBuilder(Loc.T("UI.Precombat.Battle"));
					if (this.activePlayerCombat && this.combat.hab != null)
					{
						if (this.combat.hab.faction != base.activePlayer)
						{
							if (this.combat.hab.SpaceCombatValue() > 0f)
							{
								stringBuilder2.AppendLine(Loc.T("UI.Precombat.EnemyArmedHab", new object[] { this.combat.hab.GetDisplayName(base.activePlayer) }));
							}
							else if (this.combat.fleets.Any<TISpaceFleetState>((TISpaceFleetState x) => ((x != null) ? x.faction : null) != this.combat.attacker.faction && x != null && x.ships.Count > 0))
							{
								stringBuilder2.AppendLine(Loc.T("UI.Precombat.EnemyUnarmedHab", new object[] { this.combat.hab.GetDisplayName(base.activePlayer) }));
							}
							else
							{
								TISpaceFleetState tispaceFleetState2 = this.combat.FleetFor(base.activePlayer);
								bool flag;
								if (tispaceFleetState2 == null)
								{
									flag = false;
								}
								else
								{
									flag = tispaceFleetState2.ships.All<TISpaceShipState>((TISpaceShipState x) => x.hull.simpleHull);
								}
								if (flag)
								{
									stringBuilder2 = new StringBuilder(Loc.T("UI.Precombat.EnemyUnarmedHab_3", new object[] { this.combat.hab.GetDisplayName(base.activePlayer) }));
								}
								else
								{
									stringBuilder2 = new StringBuilder(Loc.T("UI.Precombat.EnemyUnarmedHab_2", new object[] { this.combat.hab.GetDisplayName(base.activePlayer) }));
								}
								this.autoResolveButton.SetActive(false);
								this.liveResolveButton.SetActive(false);
								this.closeButton.SetActive(true);
							}
						}
						else if (this.combat.hab.ActiveCombatModules().Count == 0)
						{
							if (!this.combat.fleets.None<TISpaceFleetState>((TISpaceFleetState x) => ((x != null) ? x.faction : null) == this.combat.hab.faction))
							{
								TISpaceFleetState tispaceFleetState3 = this.combat.FleetFor(this.combat.hab.faction);
								if (tispaceFleetState3 == null || tispaceFleetState3.ships.Count != 0)
								{
									goto IL_0507;
								}
							}
							stringBuilder2 = new StringBuilder(Loc.T("UI.Precombat.BattleButNoDefenders"));
							this.autoResolveButton.SetActive(false);
							this.liveResolveButton.SetActive(false);
							this.closeButton.SetActive(true);
						}
					}
					IL_0507:
					this.preCombatResults.SetText(stringBuilder2.ToString());
				}
			}
			else
			{
				this.autoResolveButton.SetActive(false);
				this.liveResolveButton.SetActive(false);
				this.closeButton.SetActive(true);
				if (this.combat.requiresBidding)
				{
					TISpaceFleetState chasingFleet = this.combat.chasingFleet;
					TISpaceFleetState fleeingFleet = this.combat.fleeingFleet;
					if (this.combat.precombatDuration_s > 0.0)
					{
						TIDateTime tidateTime = TITimeState.Now();
						tidateTime.AddSeconds(this.combat.precombatDuration_s);
						this.preCombatResults.SetText(Loc.T("UI.Precombat.Fled", new object[]
						{
							fleeingFleet.GetDisplayName(base.activePlayer),
							chasingFleet.GetDisplayName(base.activePlayer),
							Loc.T("UI.Precombat.Spent", new object[] { TIUtilities.FormatBigOrSmallNumber(this.combat.LowestPursuitDVBid_kps, 1, 7, 0, false, false) }),
							Loc.T("UI.Precombat.PostChase", new object[] { tidateTime.ToShortTimeString() })
						}));
					}
					else
					{
						this.preCombatResults.SetText(Loc.T("UI.Precombat.Fled", new object[]
						{
							this.combat.fleeingFleet.GetDisplayName(base.activePlayer),
							chasingFleet.GetDisplayName(base.activePlayer),
							Loc.T("UI.Precombat.Spent", new object[] { TIUtilities.FormatBigOrSmallNumber(this.combat.LowestPursuitDVBid_kps, 1, 7, 0, false, false) }),
							string.Empty
						}));
						if (fleeingFleet.faction.isActivePlayer && !chasingFleet.faction.IsActiveHumanFaction)
						{
							fleeingFleet.faction.UnlockAchievement("escapeAlienFleet");
						}
					}
				}
				else
				{
					this.preCombatResults.SetText(Loc.T("UI.Precombat.NoBattle"));
				}
			}
			this.stanceSelectionObject.SetActive(false);
			this.bidSelectionObject.SetActive(false);
			this.resolutionSelectionObject.SetActive(true);
		}

		// Token: 0x06006288 RID: 25224 RVA: 0x002E4E68 File Offset: 0x002E3068
		public void MinimizePreCombatButtonPressed()
		{
			this.preCombatBodyObject.SetActive(!this.preCombatBodyObject.activeSelf);
			if (this.preCombatBodyObject.activeSelf)
			{
				TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizePrecombatButton, false, false);
				this.preCombatCanvas.sortingOrder = 15;
				return;
			}
			TIUtilities.UpdateButtonSpritesPlusMinus(this.minimizePrecombatButton, true, false);
			this.preCombatCanvas.sortingOrder = 9;
		}

		// Token: 0x06006289 RID: 25225 RVA: 0x002E4ED0 File Offset: 0x002E30D0
		public void StanceSubmit(int stanceValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SelectCombatStance(this.combat, base.activePlayer, (CombatStance)stanceValue));
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x002E4F00 File Offset: 0x002E3100
		public void BidSubmit()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SelectCombatBid(this.combat, base.activePlayer, this.currentDVBid_kps, this.extensionStance, this.attackerPursuitShipsToForceCombat));
		}

		// Token: 0x0600628B RID: 25227 RVA: 0x002E4F4C File Offset: 0x002E314C
		public void AutoresolveSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.EndPrecombatInteraction();
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x002E4F60 File Offset: 0x002E3160
		public void LiveResolveSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
			this.combat.autoresolve = false;
			this.EndPrecombatInteraction();
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x002E4F98 File Offset: 0x002E3198
		public void CloseResolveSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
			this.EndPrecombatInteraction();
		}

		// Token: 0x0600628E RID: 25230 RVA: 0x002E4FC4 File Offset: 0x002E31C4
		public void CancelAttackButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.OnClearAllFighterPlans();
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, null, this.combat, "PromptSelectSpaceCombatStance", 0);
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
			this.preCombatUIObject.SetActive(false);
			this.Hide();
			this.combat.CancelCombat();
			this.combat = null;
			TIUtilities.GotoGameState(GameStateManager.Earth(), false, true, true, false, true, -1f);
		}

		// Token: 0x0600628F RID: 25231 RVA: 0x002E504C File Offset: 0x002E324C
		private void EndPrecombatInteraction()
		{
			if (this.combat != null)
			{
				GameControl.eventManager.TriggerEvent(new PrecombatComplete(this.combat), null, Array.Empty<object>());
				this.preCombatUIObject.SetActive(false);
				this.Hide();
			}
			this.progressBar.anchorMax = new Vector2(this.progressBar.anchorMin.x, this.progressBar.anchorMax.y);
		}

		// Token: 0x06006290 RID: 25232 RVA: 0x002E50C4 File Offset: 0x002E32C4
		public float AvailableBoostWithFighterPlan(TIFactionState faction)
		{
			return faction.GetCurrentResourceAmount(FactionResource.Boost) - this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.boostCost);
		}

		// Token: 0x06006291 RID: 25233 RVA: 0x002E50F8 File Offset: 0x002E32F8
		public void UpdateSTOFighterTotals()
		{
			this.STOFightersTotalCount.SetText(this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count).ToString("N0") + "/" + base.activePlayer.EarthSTOFightersAvailable.ToString("N0"));
			this.STOFightersTotalBoostSpend.SetText(this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.boostCost).ToString("N1"));
			Selectable selectable = this.engageButton;
			bool flag;
			if (this.combat.allowedStances[base.activePlayer].Contains(CombatStance.Pursue))
			{
				if (this.combat.allowNoAttackingFleetAtInitialization)
				{
					if (this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count) <= 0)
					{
						TISpaceFleetState fleet = this.combat.GetFleet(base.activePlayer);
						flag = fleet != null && fleet.ships.Count > 0;
						goto IL_012F;
					}
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			IL_012F:
			selectable.interactable = flag;
			Selectable selectable2 = this.acceptButton;
			bool flag2;
			if (this.combat.allowedStances[base.activePlayer].Contains(CombatStance.Defend))
			{
				if (this.combat.allowNoAttackingFleetAtInitialization)
				{
					if (this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count) <= 0 && !(this.combat.attacker.faction != base.activePlayer))
					{
						TISpaceFleetState fleet2 = this.combat.GetFleet(base.activePlayer);
						flag2 = fleet2 != null && fleet2.ships.Count > 0;
						goto IL_01DD;
					}
				}
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			IL_01DD:
			selectable2.interactable = flag2;
			this.UpdateAllSTOFighterButtons();
		}

		// Token: 0x06006292 RID: 25234 RVA: 0x002E52F0 File Offset: 0x002E34F0
		public void UpdateAllSTOFighterButtons()
		{
			if (this.STOFighterPlan.Sum<KeyValuePair<TINationState, PlannedFighters>>((KeyValuePair<TINationState, PlannedFighters> x) => x.Value.count) > 0)
			{
				using (IEnumerator<object> enumerator = this.STONationsLaunchList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PrecombatController.<>o__142.<>p__0 == null)
						{
							PrecombatController.<>o__142.<>p__0 = CallSite<Func<CallSite, object, STOFighterNationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(STOFighterNationListItemController), typeof(PrecombatController)));
						}
						PrecombatController.<>o__142.<>p__0.Target(PrecombatController.<>o__142.<>p__0, enumerator.Current).SetButtons();
					}
				}
			}
		}

		// Token: 0x06006293 RID: 25235 RVA: 0x002E53A8 File Offset: 0x002E35A8
		public void OnClick_OpenSTOFIghterControllerButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.OpenSTOFighterController(GameControl.control.activePlayer);
		}

		// Token: 0x06006294 RID: 25236 RVA: 0x002E53C6 File Offset: 0x002E35C6
		public void OpenSTOFighterController(TIFactionState faction)
		{
			this.STOFightersCanvasObject.SetActive(true);
		}

		// Token: 0x06006295 RID: 25237 RVA: 0x002E53D4 File Offset: 0x002E35D4
		public void InitSTOFighterController(TIFactionState faction)
		{
			this.STOFighterPlan = new Dictionary<TINationState, PlannedFighters>();
			List<TINationState> list = (from x in faction.executiveNations
				where x.numSTOFighters > 0
				orderby x.GDP descending
				select x).ToList<TINationState>();
			this.STONationsLaunchList.SetListSize<STOFighterNationListItemController>(list.Count<TINationState>(), false, false);
			int num = 0;
			List<TIShipWeaponTemplate> list2 = faction.AllowedFighterHullWeapons();
			using (IEnumerator<object> enumerator = this.STONationsLaunchList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PrecombatController.<>o__145.<>p__0 == null)
					{
						PrecombatController.<>o__145.<>p__0 = CallSite<Func<CallSite, object, STOFighterNationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(STOFighterNationListItemController), typeof(PrecombatController)));
					}
					STOFighterNationListItemController stofighterNationListItemController = PrecombatController.<>o__145.<>p__0.Target(PrecombatController.<>o__145.<>p__0, enumerator.Current);
					this.STOFighterPlan.Add(list[num], new PlannedFighters());
					stofighterNationListItemController.SetListItem(list[num], this, list2);
					num++;
				}
			}
			this.UpdateSTOFighterTotals();
		}

		// Token: 0x06006296 RID: 25238 RVA: 0x002E5504 File Offset: 0x002E3704
		public void CloseSTOFighterController()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			if (this.STOFighterPlan.Values.Sum<PlannedFighters>((PlannedFighters x) => x.count) > 0)
			{
				base.activePlayer.playerControl.StartAction(new SetSTOFightersForCombatAction(this.combat, base.activePlayer, this.STOFighterPlan));
			}
			this.FillOutCombatData();
			this.STOFightersCanvasObject.SetActive(false);
		}

		// Token: 0x06006297 RID: 25239 RVA: 0x002E5588 File Offset: 0x002E3788
		public void OnClearAllFighterPlans()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			using (IEnumerator<object> enumerator = this.STONationsLaunchList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PrecombatController.<>o__147.<>p__0 == null)
					{
						PrecombatController.<>o__147.<>p__0 = CallSite<Func<CallSite, object, STOFighterNationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(STOFighterNationListItemController), typeof(PrecombatController)));
					}
					PrecombatController.<>o__147.<>p__0.Target(PrecombatController.<>o__147.<>p__0, enumerator.Current).ExternalFighterCountChange(0);
				}
			}
		}

		// Token: 0x06006298 RID: 25240 RVA: 0x002E5620 File Offset: 0x002E3820
		public void MaxOutAllFighterPlans()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			using (IEnumerator<object> enumerator = this.STONationsLaunchList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (PrecombatController.<>o__148.<>p__0 == null)
					{
						PrecombatController.<>o__148.<>p__0 = CallSite<Func<CallSite, object, STOFighterNationListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(STOFighterNationListItemController), typeof(PrecombatController)));
					}
					STOFighterNationListItemController stofighterNationListItemController = PrecombatController.<>o__148.<>p__0.Target(PrecombatController.<>o__148.<>p__0, enumerator.Current);
					float num = this.AvailableBoostWithFighterPlan(base.activePlayer);
					int num2 = Mathf.Min(stofighterNationListItemController.nation.availableSTOFighters - this.STOFighterPlan[stofighterNationListItemController.nation].count, (int)Math.Truncate((double)(num / this.STOFighterPlan[stofighterNationListItemController.nation].singleFighterBoostCost)));
					if (num2 > 0)
					{
						stofighterNationListItemController.ExternalFighterCountChange(this.STOFighterPlan[stofighterNationListItemController.nation].count + num2);
					}
				}
			}
		}

		// Token: 0x06006299 RID: 25241 RVA: 0x002E5730 File Offset: 0x002E3930
		public void OnCombatSimulationUpdated(CombatSimulationUpdated e)
		{
			if (this.combat == null)
			{
				return;
			}
			if (!e.simulatedCombat.Factions.Contains(GameControl.control.activePlayer))
			{
				TISpaceCombatState combat = this.combat;
				if (((combat != null) ? combat.AutoresolveSecondsElapsed : 0f) <= 1f)
				{
					return;
				}
			}
			SimulatedCombat simulatedCombat = e.simulatedCombat;
			TISpaceCombatState combat2 = this.combat;
			this.DisplayCombatResults(simulatedCombat.GetCombatRecord((combat2 != null) ? combat2.combatRecord : default(CombatRecord)), e.progress, true);
		}

		// Token: 0x0600629A RID: 25242 RVA: 0x002E57B7 File Offset: 0x002E39B7
		public void OnCombatComplete(CombatEnds e)
		{
			if (GameControl.control.skirmishMode)
			{
				this.combat = e.combat;
			}
			this.DisplayCombatResults(e.combat.combatRecord, 1f, false);
			this.combat = null;
		}

		// Token: 0x0600629B RID: 25243 RVA: 0x002E57F0 File Offset: 0x002E39F0
		public void DisplayCombatResults(CombatRecord combatRecord, float progress, bool isSimulationResult)
		{
			TIFactionState faction1 = combatRecord.faction1;
			TIFactionState faction2 = combatRecord.faction2;
			bool flag = faction1 == base.activePlayer || faction2 == base.activePlayer;
			if (flag || progress < 1f)
			{
				base.gameTime.Pause();
				this.Show();
				this.postCombatButtonPanel.SetActive(!isSimulationResult);
				this.progressBar.gameObject.SetActive(isSimulationResult);
				if (isSimulationResult)
				{
					this.combatProgress = progress;
					if (this.combatProgress >= 1f)
					{
						this.progressBar.gameObject.SetActive(false);
						if (!this.combat.mayRejectAutoresolve)
						{
							this.OnAcceptAutoresolveSelected();
							return;
						}
						this.autoresoveButtonPanel.SetActive(true);
					}
				}
				List<CombatRecord.SingleAssetCombatRecord> list = combatRecord.singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == faction1).ToList<CombatRecord.SingleAssetCombatRecord>();
				List<CombatRecord.SingleAssetCombatRecord> list2 = combatRecord.singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction != faction1).ToList<CombatRecord.SingleAssetCombatRecord>();
				this.postCombatHeader.SetText(combatRecord.combatName);
				this.postCombatFleet1Header.SetText((combatRecord.Hab != null && list.Count <= 1 && combatRecord.Hab.faction == faction1) ? combatRecord.habName : combatRecord.fleet1Name);
				this.postCombatFleet2Header.SetText((combatRecord.Hab != null && list2.Count <= 1 && combatRecord.Hab.faction == faction2) ? combatRecord.habName : combatRecord.fleet2Name);
				this.postCombatFleet1Icon.sprite = faction1.factionIcon64;
				this.postCombatFleet2Icon.sprite = faction2.factionIcon64;
				list = list.OrderByDescending<CombatRecord.SingleAssetCombatRecord, bool>((CombatRecord.SingleAssetCombatRecord x) => x.asset.isHabState).ThenByDescending<CombatRecord.SingleAssetCombatRecord, float?>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TISpaceShipState ref_ship = x.asset.ref_ship;
					if (ref_ship == null)
					{
						return null;
					}
					return new float?(ref_ship.hull.length_m);
				}).ThenByDescending<CombatRecord.SingleAssetCombatRecord, double?>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TISpaceShipState ref_ship2 = x.asset.ref_ship;
					if (ref_ship2 == null)
					{
						return null;
					}
					return new double?(ref_ship2.dryMass_kg);
				})
					.ToList<CombatRecord.SingleAssetCombatRecord>();
				list2 = list2.OrderByDescending<CombatRecord.SingleAssetCombatRecord, bool>((CombatRecord.SingleAssetCombatRecord x) => x.asset.isHabState).ThenByDescending<CombatRecord.SingleAssetCombatRecord, float?>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TISpaceShipState ref_ship3 = x.asset.ref_ship;
					if (ref_ship3 == null)
					{
						return null;
					}
					return new float?(ref_ship3.hull.length_m);
				}).ThenByDescending<CombatRecord.SingleAssetCombatRecord, double?>(delegate(CombatRecord.SingleAssetCombatRecord x)
				{
					TISpaceShipState ref_ship4 = x.asset.ref_ship;
					if (ref_ship4 == null)
					{
						return null;
					}
					return new double?(ref_ship4.dryMass_kg);
				})
					.ToList<CombatRecord.SingleAssetCombatRecord>();
				int num = 0;
				this.postCombatFleet1List.SetListSize<PostcombatShipListItemController>(list.Count, false, false);
				using (IEnumerator<object> enumerator = this.postCombatFleet1List.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PrecombatController.<>o__152.<>p__0 == null)
						{
							PrecombatController.<>o__152.<>p__0 = CallSite<Func<CallSite, object, PostcombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PostcombatShipListItemController), typeof(PrecombatController)));
						}
						PostcombatShipListItemController postcombatShipListItemController = PrecombatController.<>o__152.<>p__0.Target(PrecombatController.<>o__152.<>p__0, enumerator.Current);
						CombatRecord.SingleAssetCombatRecord singleAssetCombatRecord = list[num++];
						TISpaceCombatState combat = this.combat;
						postcombatShipListItemController.SetListItem(singleAssetCombatRecord, combat != null && combat.autoDestroyHab);
					}
				}
				num = 0;
				this.postCombatFleet2List.SetListSize<PostcombatShipListItemController>(list2.Count, false, false);
				using (IEnumerator<object> enumerator = this.postCombatFleet2List.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (PrecombatController.<>o__152.<>p__1 == null)
						{
							PrecombatController.<>o__152.<>p__1 = CallSite<Func<CallSite, object, PostcombatShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PostcombatShipListItemController), typeof(PrecombatController)));
						}
						PostcombatShipListItemController postcombatShipListItemController2 = PrecombatController.<>o__152.<>p__1.Target(PrecombatController.<>o__152.<>p__1, enumerator.Current);
						CombatRecord.SingleAssetCombatRecord singleAssetCombatRecord2 = list2[num++];
						TISpaceCombatState combat2 = this.combat;
						postcombatShipListItemController2.SetListItem(singleAssetCombatRecord2, combat2 != null && combat2.autoDestroyHab);
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (isSimulationResult)
				{
					if (progress >= 1f)
					{
						stringBuilder.Append(Loc.T("UI.SpaceCombat.AutoresolveComplete"));
					}
					else if (flag)
					{
						stringBuilder.Append(Loc.T("UI.SpaceCombat.CombatInProgress"));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.SpaceCombat.MajorCombatInProgress"));
					}
				}
				else
				{
					if (this.combat.bothSidesDestroyed)
					{
						stringBuilder.Append(Loc.T("UI.SpaceCombat.Outcome.MAD"));
					}
					else if (this.combat.draw)
					{
						stringBuilder.Append(Loc.T("UI.SpaceCombat.Outcome.Indecisive"));
					}
					else
					{
						TIHabState hab = this.combat.hab;
						TIFactionState tifactionState = ((hab != null) ? hab.faction : null);
						if (tifactionState != null && this.combat.winner == tifactionState)
						{
							stringBuilder.Append(Loc.T("UI.SpaceCombat.Outcome.StationDefended", new object[]
							{
								this.combat.winner.GetDisplayName(tifactionState),
								this.combat.hab.GetDisplayName(tifactionState)
							}));
						}
						else
						{
							stringBuilder.Append(Loc.T("UI.SpaceCombat.Outcome.Victor", new object[] { this.combat.winner.GetDisplayName(base.activePlayer) }));
						}
					}
					if (!GameControl.control.skirmishMode)
					{
						if (this.combat.factions.Contains(base.activePlayer))
						{
							if (!this.combat.autoDestroyHab)
							{
								if (!combatRecord.singleAssetRecords.Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.asset is TIHabState && x.outcome == SingleAssetCombatOutcome.Destroyed))
								{
									if (this.combat.winner == faction1)
									{
										if (combatRecord.singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == faction2).Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.HabDisabled || x.outcome == SingleAssetCombatOutcome.HabNoncombatant))
										{
											stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Outcome.DockedAtHab", new object[]
											{
												combatRecord.fleet1Name,
												this.combat.hab.GetDisplayName(base.activePlayer)
											}));
											goto IL_0746;
										}
									}
									if (!(this.combat.winner == faction2))
									{
										goto IL_0746;
									}
									if (combatRecord.singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == faction1).Any<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.HabDisabled || x.outcome == SingleAssetCombatOutcome.HabNoncombatant))
									{
										stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Outcome.DockedAtHab", new object[]
										{
											combatRecord.fleet2Name,
											this.combat.hab.GetDisplayName(base.activePlayer)
										}));
										goto IL_0746;
									}
									goto IL_0746;
								}
							}
							stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Outcome.HabDestroyed", new object[] { combatRecord.habName }));
							IL_0746:
							if (this.combat.winner == base.activePlayer && combatRecord.winnerSalvage != null && combatRecord.winnerSalvage.anyDebit)
							{
								stringBuilder.AppendLine().AppendLine(Loc.T("UI.SpaceCombat.Outcome.Salvage", new object[] { combatRecord.winnerSalvage.ToString("Relevant", false, false, null, false, FactionResource.None) }));
							}
							List<CombatRecord.SingleAssetCombatRecord> list3 = combatRecord.singleAssetRecords.Where<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.faction == this.activePlayer).ToList<CombatRecord.SingleAssetCombatRecord>();
							if (list3.Count > 0)
							{
								int num2 = list3.Count<CombatRecord.SingleAssetCombatRecord>((CombatRecord.SingleAssetCombatRecord x) => x.outcome == SingleAssetCombatOutcome.MissionKilled);
								if (num2 == 1)
								{
									stringBuilder.AppendLine().AppendLine(Loc.T("UI.SpaceCombat.Outcome.MissionKilledOne"));
								}
								else if (num2 > 1)
								{
									stringBuilder.AppendLine().AppendLine(Loc.T("UI.SpaceCombat.Outcome.MissionKilledMultiple"));
								}
							}
						}
						this.gotoSpaceObject = null;
						if (TIGameState.Valid(this.combat.winningFleet) && this.combat.winningFleet.ships.Count > 0 && this.combat.winningFleet.faction == base.activePlayer)
						{
							this.gotoSpaceObject = this.combat.winningFleet;
						}
						else if (TIGameState.Valid(this.combat.losingFleet) && this.combat.losingFleet.ships.Count > 0 && this.combat.losingFleet.faction == base.activePlayer)
						{
							this.gotoSpaceObject = this.combat.losingFleet;
						}
						else if (this.combat.hab != null && !this.combat.hab.deleted)
						{
							this.gotoSpaceObject = this.combat.hab;
						}
						else if (TIGameState.Valid(this.combat.winningFleet) && this.combat.winningFleet.ships.Count > 0)
						{
							this.gotoSpaceObject = this.combat.winningFleet;
						}
						else if (this.combat.nearbyNaturalSpaceObject != null && !this.combat.nearbyNaturalSpaceObject.isSun)
						{
							this.gotoSpaceObject = this.combat.nearbyNaturalSpaceObject;
						}
					}
				}
				this.gotoSpaceObjectButton.SetActive(this.gotoSpaceObject != null && !GameControl.control.skirmishMode);
				this.continueSpaceObjectButton.SetActive(!GameControl.control.skirmishMode);
				stringBuilder.AppendLine();
				Dictionary<TIFactionState, List<TIOfficerState>> dictionary;
				Dictionary<TIFactionState, List<string>> dictionary2;
				if (isSimulationResult)
				{
					dictionary = new Dictionary<TIFactionState, List<TIOfficerState>>();
					dictionary2 = this.combat.SimulatedCombat.SimulatedOfficerDeathsRecord;
				}
				else
				{
					dictionary = this.combat.officerPromotions;
					dictionary2 = this.combat.officerDeathsRecord;
				}
				if (dictionary.ContainsKey(base.activePlayer) && dictionary[base.activePlayer].Count > 0)
				{
					stringBuilder.AppendLine().AppendLine(TIOfficerTemplate.BuildOfficerPromotionReport(dictionary[base.activePlayer], base.activePlayer));
				}
				if (dictionary2.ContainsKey(base.activePlayer) && dictionary2[base.activePlayer].Count > 0)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Precombat.OfficerKilled_Header"));
					foreach (string text in dictionary2[base.activePlayer])
					{
						stringBuilder.AppendLine(text);
					}
				}
				this.postCombatReportText.SetText(stringBuilder.ToString());
				this.postCombatUIObject.SetActive(true);
				return;
			}
			this.Hide();
			this.postCombatUIObject.SetActive(false);
			if (this.delayedCombatInitiationEvent != null)
			{
				this.OnCombatInitiated(this.delayedCombatInitiationEvent);
				this.delayedCombatInitiationEvent = null;
				return;
			}
			base.gameTime.Play();
		}

		// Token: 0x0600629C RID: 25244 RVA: 0x002E6328 File Offset: 0x002E4528
		public void OnClosePostCombatButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.postCombatUIObject.SetActive(false);
			if (GameControl.control.skirmishMode)
			{
				TemplateManager.ClearSkirmishModeTemplates();
				GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
				return;
			}
			if (this.delayedCombatInitiationEvent != null)
			{
				this.OnCombatInitiated(this.delayedCombatInitiationEvent);
				this.delayedCombatInitiationEvent = null;
				return;
			}
			this.Hide();
		}

		// Token: 0x0600629D RID: 25245 RVA: 0x002E6394 File Offset: 0x002E4594
		public void OnRejectAutoresolveSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
			this.autoresoveButtonPanel.SetActive(false);
			this.combat.autoresolve = false;
			GameControl.spaceCombat.AutoresolveRejected();
		}

		// Token: 0x0600629E RID: 25246 RVA: 0x002E63E7 File Offset: 0x002E45E7
		public void OnAcceptAutoresolveSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIPromptQueueState.RemovePromptStatic(base.activePlayer, this.combat, null, "PromptBeginCombat", 0);
			this.autoresoveButtonPanel.SetActive(false);
			this.combat.ApplySimulatedCombat();
		}

		// Token: 0x0600629F RID: 25247 RVA: 0x002E6424 File Offset: 0x002E4624
		public void OnGotoPostCombatButtonSelected()
		{
			SoundEffectController.PlaySelectSound(this.gotoSpaceObject);
			TIUtilities.GotoGameState(this.gotoSpaceObject, true, true, true, true, false, -1f);
			this.OnClosePostCombatButtonSelected();
		}

		// Token: 0x060062A0 RID: 25248 RVA: 0x002E644C File Offset: 0x002E464C
		public void OnContinueButtonSelected()
		{
			this.OnClosePostCombatButtonSelected();
			base.gameTime.Play();
		}

		// Token: 0x0400452C RID: 17708
		private TISpaceFleetState activePlayerFleet;

		// Token: 0x0400452D RID: 17709
		private TISpaceFleetState otherFleet;

		// Token: 0x0400452E RID: 17710
		[Header("Precombat")]
		public GameObject preCombatUIObject;

		// Token: 0x0400452F RID: 17711
		public GameObject preCombatBodyObject;

		// Token: 0x04004530 RID: 17712
		public Canvas preCombatCanvas;

		// Token: 0x04004531 RID: 17713
		public ListManagerBase fleet1List;

		// Token: 0x04004532 RID: 17714
		public ListManagerBase fleet2List;

		// Token: 0x04004533 RID: 17715
		public TMP_Text precombatHeader;

		// Token: 0x04004534 RID: 17716
		public TMP_Text fleet1Header;

		// Token: 0x04004535 RID: 17717
		public TMP_Text fleet2Header;

		// Token: 0x04004536 RID: 17718
		public Image fleet1Icon;

		// Token: 0x04004537 RID: 17719
		public Image fleet2Icon;

		// Token: 0x04004538 RID: 17720
		public TMP_Text fleet1ShipsCount;

		// Token: 0x04004539 RID: 17721
		public TMP_Text fleet2ShipsCount;

		// Token: 0x0400453A RID: 17722
		public TMP_Text fleet1CombatScore;

		// Token: 0x0400453B RID: 17723
		public TMP_Text fleet2CombatScore;

		// Token: 0x0400453C RID: 17724
		public TMP_Text fleet1Accel;

		// Token: 0x0400453D RID: 17725
		public TMP_Text fleet2Accel;

		// Token: 0x0400453E RID: 17726
		public TMP_Text fleet1DeltaV;

		// Token: 0x0400453F RID: 17727
		public TMP_Text fleet2DeltaV;

		// Token: 0x04004540 RID: 17728
		public GameObject stanceSelectionObject;

		// Token: 0x04004541 RID: 17729
		public GameObject enemyFighterNotesObject;

		// Token: 0x04004542 RID: 17730
		public TMP_Text enemyFighterNotes;

		// Token: 0x04004543 RID: 17731
		public Button engageButton;

		// Token: 0x04004544 RID: 17732
		public TMP_Text engageButtonText;

		// Token: 0x04004545 RID: 17733
		public TMP_Text engageButtonDescription;

		// Token: 0x04004546 RID: 17734
		public Button acceptButton;

		// Token: 0x04004547 RID: 17735
		public TMP_Text acceptButtonText;

		// Token: 0x04004548 RID: 17736
		public TMP_Text acceptButtonDescription;

		// Token: 0x04004549 RID: 17737
		public Button evadeButton;

		// Token: 0x0400454A RID: 17738
		public TMP_Text evadeButtonText;

		// Token: 0x0400454B RID: 17739
		public TMP_Text evadeButtonDescription;

		// Token: 0x0400454C RID: 17740
		public GameObject bidSelectionObject;

		// Token: 0x0400454D RID: 17741
		public TMP_Text bidHeader;

		// Token: 0x0400454E RID: 17742
		public TMP_Text bidDetail;

		// Token: 0x0400454F RID: 17743
		public Slider deltaVBidSlider;

		// Token: 0x04004550 RID: 17744
		public TMP_Text currentBidValue;

		// Token: 0x04004551 RID: 17745
		public TMP_Text maxBidValue;

		// Token: 0x04004552 RID: 17746
		public TMP_Text bidSubmitButtonText;

		// Token: 0x04004553 RID: 17747
		public TooltipTrigger biddingTip;

		// Token: 0x04004554 RID: 17748
		public GameObject sliderObject;

		// Token: 0x04004555 RID: 17749
		public GameObject maxBidInfoObject;

		// Token: 0x04004556 RID: 17750
		public GameObject resolutionSelectionObject;

		// Token: 0x04004557 RID: 17751
		public TMP_Text preCombatResults;

		// Token: 0x04004558 RID: 17752
		public TMP_Text autoResolveButtonText;

		// Token: 0x04004559 RID: 17753
		public TMP_Text playBattleButtonText;

		// Token: 0x0400455A RID: 17754
		public TMP_Text closeButtonText;

		// Token: 0x0400455B RID: 17755
		public GameObject autoResolveButton;

		// Token: 0x0400455C RID: 17756
		public GameObject liveResolveButton;

		// Token: 0x0400455D RID: 17757
		public GameObject closeButton;

		// Token: 0x0400455E RID: 17758
		public GameObject extendedPursuitObject;

		// Token: 0x0400455F RID: 17759
		public TMP_Text extendedPursuitText;

		// Token: 0x04004560 RID: 17760
		public Toggle extendedPursuitToggle;

		// Token: 0x04004561 RID: 17761
		public GameObject extendedPursuitToggleObject;

		// Token: 0x04004562 RID: 17762
		public GameObject extendedPursuitWarningRightIconObject;

		// Token: 0x04004563 RID: 17763
		public Button minimizePrecombatButton;

		// Token: 0x04004564 RID: 17764
		public Button addFightersButton;

		// Token: 0x04004565 RID: 17765
		public TMP_Text addFightersButtonText;

		// Token: 0x04004566 RID: 17766
		public TMP_Text addFightersButtonExplainer;

		// Token: 0x04004567 RID: 17767
		public Button cancelAttackButton;

		// Token: 0x04004568 RID: 17768
		public TMP_Text cancelAttackButtonText;

		// Token: 0x04004569 RID: 17769
		public TMP_Text cancelAttackButtonExplainer;

		// Token: 0x0400456A RID: 17770
		[Header("STO Fighters")]
		public GameObject STOFightersCanvasObject;

		// Token: 0x0400456B RID: 17771
		public TMP_Text STOFightersHeader;

		// Token: 0x0400456C RID: 17772
		public TMP_Text STOFightersExplainer;

		// Token: 0x0400456D RID: 17773
		public TMP_Text STOFightersTotalCount;

		// Token: 0x0400456E RID: 17774
		public TMP_Text STOFightersTotalBoostSpend;

		// Token: 0x0400456F RID: 17775
		public TMP_Text STOFightersCloseButtonText;

		// Token: 0x04004570 RID: 17776
		public TMP_Text STOFightersResetButtonText;

		// Token: 0x04004571 RID: 17777
		public TMP_Text STOFightersLaunchEverybodyButtonText;

		// Token: 0x04004572 RID: 17778
		public ListManagerBase STONationsLaunchList;

		// Token: 0x04004573 RID: 17779
		[Header("PostCombat")]
		public GameObject postCombatUIObject;

		// Token: 0x04004574 RID: 17780
		public TMP_Text postCombatHeader;

		// Token: 0x04004575 RID: 17781
		public GameObject postCombatCloseButton;

		// Token: 0x04004576 RID: 17782
		public TMP_Text postCombatContinueText;

		// Token: 0x04004577 RID: 17783
		public TMP_Text postCombatCloseButtonText;

		// Token: 0x04004578 RID: 17784
		public TMP_Text postCombatGotoButtonText;

		// Token: 0x04004579 RID: 17785
		public TMP_Text postCombatReportText;

		// Token: 0x0400457A RID: 17786
		public Image postCombatFleet1Icon;

		// Token: 0x0400457B RID: 17787
		public Image postCombatFleet2Icon;

		// Token: 0x0400457C RID: 17788
		public ListManagerBase postCombatFleet1List;

		// Token: 0x0400457D RID: 17789
		public ListManagerBase postCombatFleet2List;

		// Token: 0x0400457E RID: 17790
		public TMP_Text postCombatFleet1Header;

		// Token: 0x0400457F RID: 17791
		public TMP_Text postCombatFleet2Header;

		// Token: 0x04004580 RID: 17792
		public GameObject postCombatButtonPanel;

		// Token: 0x04004581 RID: 17793
		public RectTransform progressBar;

		// Token: 0x04004582 RID: 17794
		public GameObject gotoSpaceObjectButton;

		// Token: 0x04004583 RID: 17795
		private TISpaceObjectState gotoSpaceObject;

		// Token: 0x04004584 RID: 17796
		public GameObject continueSpaceObjectButton;

		// Token: 0x04004585 RID: 17797
		public GameObject autoresoveButtonPanel;

		// Token: 0x04004586 RID: 17798
		public GameObject acceptAutoresolveButton;

		// Token: 0x04004587 RID: 17799
		public GameObject rejectAutoresolveButton;

		// Token: 0x04004588 RID: 17800
		public TMP_Text acceptAutoresolveButtonText;

		// Token: 0x04004589 RID: 17801
		public TMP_Text rejectAutoresolveButtonText;

		// Token: 0x0400458A RID: 17802
		private SpaceCombatInitiated delayedCombatInitiationEvent;

		// Token: 0x0400458B RID: 17803
		private float maxDVBid_kps;

		// Token: 0x0400458C RID: 17804
		private float currentDVBid_kps;

		// Token: 0x0400458D RID: 17805
		private bool envelop;

		// Token: 0x0400458E RID: 17806
		private List<TISpaceShipState> attackerPursuitShipsToForceCombat = new List<TISpaceShipState>();

		// Token: 0x0400458F RID: 17807
		public Dictionary<TINationState, PlannedFighters> STOFighterPlan;

		// Token: 0x04004590 RID: 17808
		private float combatProgress;
	}
}
