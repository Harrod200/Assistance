using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AA RID: 1962
	public class TIOfficerState : TIGameState
	{
		// Token: 0x17000BF1 RID: 3057
		// (get) Token: 0x0600410B RID: 16651 RVA: 0x001A48A6 File Offset: 0x001A2AA6
		// (set) Token: 0x0600410C RID: 16652 RVA: 0x001A48AE File Offset: 0x001A2AAE
		public string officerName { get; private set; }

		// Token: 0x17000BF2 RID: 3058
		// (get) Token: 0x0600410D RID: 16653 RVA: 0x001A48B7 File Offset: 0x001A2AB7
		// (set) Token: 0x0600410E RID: 16654 RVA: 0x001A48BF File Offset: 0x001A2ABF
		public int rank { get; private set; }

		// Token: 0x17000BF3 RID: 3059
		// (get) Token: 0x0600410F RID: 16655 RVA: 0x001A48C8 File Offset: 0x001A2AC8
		// (set) Token: 0x06004110 RID: 16656 RVA: 0x001A48D0 File Offset: 0x001A2AD0
		public int maxRank { get; private set; }

		// Token: 0x17000BF4 RID: 3060
		// (get) Token: 0x06004111 RID: 16657 RVA: 0x001A48D9 File Offset: 0x001A2AD9
		// (set) Token: 0x06004112 RID: 16658 RVA: 0x001A48E1 File Offset: 0x001A2AE1
		public TISpaceShipState ship { get; private set; }

		// Token: 0x17000BF5 RID: 3061
		// (get) Token: 0x06004113 RID: 16659 RVA: 0x001A48EA File Offset: 0x001A2AEA
		// (set) Token: 0x06004114 RID: 16660 RVA: 0x001A48F2 File Offset: 0x001A2AF2
		public TIHabState hab { get; private set; }

		// Token: 0x17000BF6 RID: 3062
		// (get) Token: 0x06004115 RID: 16661 RVA: 0x001A48FB File Offset: 0x001A2AFB
		// (set) Token: 0x06004116 RID: 16662 RVA: 0x001A4903 File Offset: 0x001A2B03
		[fsIgnore]
		public TIOfficerTemplate _template { get; private set; }

		// Token: 0x17000BF7 RID: 3063
		// (get) Token: 0x06004117 RID: 16663 RVA: 0x001A490C File Offset: 0x001A2B0C
		// (set) Token: 0x06004118 RID: 16664 RVA: 0x001A4914 File Offset: 0x001A2B14
		public TIDateTime creationDate { get; private set; }

		// Token: 0x17000BF8 RID: 3064
		// (get) Token: 0x06004119 RID: 16665 RVA: 0x001A491D File Offset: 0x001A2B1D
		// (set) Token: 0x0600411A RID: 16666 RVA: 0x001A4925 File Offset: 0x001A2B25
		public TIDateTime retirementDate { get; private set; }

		// Token: 0x17000BF9 RID: 3065
		// (get) Token: 0x0600411B RID: 16667 RVA: 0x001A492E File Offset: 0x001A2B2E
		public override bool isOfficerState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BFA RID: 3066
		// (get) Token: 0x0600411C RID: 16668 RVA: 0x001A4931 File Offset: 0x001A2B31
		public override TIOfficerState ref_officer
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000BFB RID: 3067
		// (get) Token: 0x0600411D RID: 16669 RVA: 0x001A4934 File Offset: 0x001A2B34
		public override TIFactionState ref_faction
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TIFactionState tifactionState;
				if ((tifactionState = ((ship != null) ? ship.ref_faction : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tifactionState = hab.ref_faction;
				}
				return tifactionState;
			}
		}

		// Token: 0x17000BFC RID: 3068
		// (get) Token: 0x0600411E RID: 16670 RVA: 0x001A495D File Offset: 0x001A2B5D
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				TISpaceShipState ship = this.ship;
				if (ship == null)
				{
					return null;
				}
				return ship.ref_fleet;
			}
		}

		// Token: 0x17000BFD RID: 3069
		// (get) Token: 0x0600411F RID: 16671 RVA: 0x001A4970 File Offset: 0x001A2B70
		public override TISpaceShipState ref_ship
		{
			get
			{
				return this.ship;
			}
		}

		// Token: 0x17000BFE RID: 3070
		// (get) Token: 0x06004120 RID: 16672 RVA: 0x001A4978 File Offset: 0x001A2B78
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TISpaceObjectState tispaceObjectState;
				if ((tispaceObjectState = ((ship != null) ? ship.ref_fleet.ref_spaceObject : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tispaceObjectState = hab.ref_spaceObject;
				}
				return tispaceObjectState;
			}
		}

		// Token: 0x17000BFF RID: 3071
		// (get) Token: 0x06004121 RID: 16673 RVA: 0x001A49A6 File Offset: 0x001A2BA6
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				if ((tinaturalSpaceObjectState = ((ship != null) ? ship.ref_naturalSpaceObject : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tinaturalSpaceObjectState = hab.ref_naturalSpaceObject;
				}
				return tinaturalSpaceObjectState;
			}
		}

		// Token: 0x17000C00 RID: 3072
		// (get) Token: 0x06004122 RID: 16674 RVA: 0x001A49CF File Offset: 0x001A2BCF
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TISpaceBodyState tispaceBodyState;
				if ((tispaceBodyState = ((ship != null) ? ship.ref_spaceBody : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tispaceBodyState = hab.ref_spaceBody;
				}
				return tispaceBodyState;
			}
		}

		// Token: 0x17000C01 RID: 3073
		// (get) Token: 0x06004123 RID: 16675 RVA: 0x001A49F8 File Offset: 0x001A2BF8
		public override TIOrbitState ref_orbit
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TIOrbitState tiorbitState;
				if ((tiorbitState = ((ship != null) ? ship.ref_orbit : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tiorbitState = hab.ref_orbit;
				}
				return tiorbitState;
			}
		}

		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x06004124 RID: 16676 RVA: 0x001A4A21 File Offset: 0x001A2C21
		public override TIHabState ref_hab
		{
			get
			{
				TISpaceShipState ship = this.ship;
				return ((ship != null) ? ship.ref_hab : null) ?? this.hab;
			}
		}

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06004125 RID: 16677 RVA: 0x001A4A3F File Offset: 0x001A2C3F
		public override TIHabSiteState ref_habSite
		{
			get
			{
				TISpaceShipState ship = this.ship;
				TIHabSiteState tihabSiteState;
				if ((tihabSiteState = ((ship != null) ? ship.ref_habSite : null)) == null)
				{
					TIHabState hab = this.hab;
					if (hab == null)
					{
						return null;
					}
					tihabSiteState = hab.habSite;
				}
				return tihabSiteState;
			}
		}

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06004126 RID: 16678 RVA: 0x001A4A68 File Offset: 0x001A2C68
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				TISpaceShipState ship = this.ship;
				return ((ship != null) ? ship.ref_spaceAsset : null) ?? this.hab;
			}
		}

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06004127 RID: 16679 RVA: 0x001A4A86 File Offset: 0x001A2C86
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004128 RID: 16680 RVA: 0x001A4A8C File Offset: 0x001A2C8C
		public override void PostCanvasManagerCreateInit_3()
		{
			this.maxRank = Mathf.Max(this.rank, this.maxRank);
			if (this.retirementDate == null)
			{
				this.retirementDate = TITimeState.Now();
				this.retirementDate.AddYears(15 + 5 * this.maxRank);
			}
		}

		// Token: 0x06004129 RID: 16681 RVA: 0x001A4ADF File Offset: 0x001A2CDF
		public void SetDisplayName()
		{
			this.displayName = (this.template.GetRankString(this.maxRank) + " " + this.officerName).Trim();
		}

		// Token: 0x17000C06 RID: 3078
		// (get) Token: 0x0600412A RID: 16682 RVA: 0x001A4B10 File Offset: 0x001A2D10
		public string DisplayNameAndShipAndJob
		{
			get
			{
				string text = "TIOfficerTemplate.OfficerShipNameAndJob";
				object[] array = new object[3];
				int num = 0;
				TISpaceShipState ship = this.ship;
				string text2;
				if ((text2 = ((ship != null) ? ship.displayName : null)) == null)
				{
					TIHabState hab = this.hab;
					text2 = ((hab != null) ? hab.displayName : null) ?? string.Empty;
				}
				array[num] = text2;
				array[1] = this.displayName;
				array[2] = this.template.displayName;
				return Loc.T(text, array);
			}
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x0600412B RID: 16683 RVA: 0x001A4B78 File Offset: 0x001A2D78
		public string DisplayNameAndJob
		{
			get
			{
				return Loc.T("TIOfficerTemplate.OfficerNameAndJob", new object[]
				{
					this.displayName,
					this.template.displayName
				});
			}
		}

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x0600412C RID: 16684 RVA: 0x001A4BA4 File Offset: 0x001A2DA4
		public string FullDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.DisplayNameAndJob);
				stringBuilder.AppendLine(this.template.description);
				stringBuilder.Append(this.template.EffectsAtRankString(this.rank));
				return stringBuilder.ToString();
			}
		}

		// Token: 0x17000C09 RID: 3081
		// (get) Token: 0x0600412D RID: 16685 RVA: 0x001A4BF4 File Offset: 0x001A2DF4
		public TIOfficerTemplate template
		{
			get
			{
				if (this._template == null)
				{
					this._template = this.GetMyTemplate<TIOfficerTemplate>();
				}
				return this._template;
			}
		}

		// Token: 0x0600412E RID: 16686 RVA: 0x001A4C20 File Offset: 0x001A2E20
		public string GetIconPath()
		{
			return new StringBuilder(this.template.baseIconPath).Append(this.rank.ToString("N0")).ToString();
		}

		// Token: 0x0600412F RID: 16687 RVA: 0x001A4C5C File Offset: 0x001A2E5C
		public static TIOfficerState CreateOfficer(string templateName, TISpaceShipState ship)
		{
			TIOfficerState tiofficerState = (TIOfficerState)TemplateManager.Find<TIOfficerTemplate>(templateName, false).CreateGameState();
			tiofficerState.templateName = templateName;
			tiofficerState.rank = 1;
			tiofficerState.maxRank = 1;
			tiofficerState.ship = ship;
			tiofficerState.creationDate = TITimeState.Now();
			tiofficerState.retirementDate = tiofficerState.creationDate;
			if (ship.IsAlien())
			{
				tiofficerState.officerName = "Xenoform Sierra";
				tiofficerState.retirementDate.AddYears(100);
			}
			else
			{
				tiofficerState.retirementDate.AddYears(20);
				TIRegionState tiregionState = TICouncilorState.RandomizeRegionWeightedByPopulation(true, ship.faction);
				CouncilorAncestry councilorAncestry = TICouncilorState.RandomizeAncestryFromRegion(tiregionState);
				CouncilorGender councilorGender = TICouncilorState.RandomizeGender(tiregionState);
				Tuple<string, string> tuple = TICouncilorState.GenerateNameFromRegionAncestry(tiregionState, councilorAncestry, councilorGender);
				tiofficerState.officerName = (tuple.Item1 + " " + tuple.Item2).Trim();
			}
			tiofficerState.SetDisplayName();
			ship.officers.Add(tiofficerState);
			tiofficerState.OnOfficerChange();
			GameControl.eventManager.TriggerEvent(new ShipGainsOfficer(tiofficerState), null, new object[] { tiofficerState, ship });
			ship.CheckOfficersOnShipAchievement();
			return tiofficerState;
		}

		// Token: 0x06004130 RID: 16688 RVA: 0x001A4D5E File Offset: 0x001A2F5E
		public bool OfficerAllowedForShip(TISpaceShipState candidateShip, bool swap, int additionalProposedTransfersToShip)
		{
			return this.OfficerAllowedForShipFail(candidateShip, swap, additionalProposedTransfersToShip).Count == 0;
		}

		// Token: 0x17000C0A RID: 3082
		// (get) Token: 0x06004131 RID: 16689 RVA: 0x001A4D74 File Offset: 0x001A2F74
		public OfficerCarrierState OfficerCarrier
		{
			get
			{
				OfficerCarrierState ship = this.ship;
				return ship ?? this.hab;
			}
		}

		// Token: 0x06004132 RID: 16690 RVA: 0x001A4D93 File Offset: 0x001A2F93
		public List<OfficerRequirement> OfficerAllowedForShipFail(TISpaceShipState candidateShip, bool swap, int additionalProposedTransfersToShip)
		{
			return this.template.OfficerTypeAllowedForShipFailReasons(candidateShip, swap, additionalProposedTransfersToShip);
		}

		// Token: 0x06004133 RID: 16691 RVA: 0x001A4DA4 File Offset: 0x001A2FA4
		public bool ProposedTransferIsSwap(OfficerCarrierState other, List<TIOfficerState> proposedNewOfficersOnOther)
		{
			Func<TIOfficerState, bool> <>9__1;
			Func<TIOfficerState, bool> <>9__2;
			return other.GetState().ref_ship != null && this.template.requirements.Any<OfficerRequirement>(delegate(OfficerRequirement x)
			{
				if (x.requirement == OfficerRequirementType.MaxPerShip)
				{
					IEnumerable<TIOfficerState> officers = other.GetState().ref_ship.officers;
					Func<TIOfficerState, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (TIOfficerState x) => x.templateName == this.templateName);
					}
					float num = (float)officers.Count<TIOfficerState>(func);
					IEnumerable<TIOfficerState> proposedNewOfficersOnOther2 = proposedNewOfficersOnOther;
					Func<TIOfficerState, bool> func2;
					if ((func2 = <>9__2) == null)
					{
						func2 = (<>9__2 = (TIOfficerState x) => x.templateName == this.templateName);
					}
					return num + (float)proposedNewOfficersOnOther2.Count<TIOfficerState>(func2) >= x.value;
				}
				return false;
			});
		}

		// Token: 0x06004134 RID: 16692 RVA: 0x001A4E08 File Offset: 0x001A3008
		public TIOfficerState ProposedOfficerSwap(TISpaceShipState otherShip, List<TIOfficerState> proposedNewOfficersOnOther)
		{
			if (this.ProposedTransferIsSwap(otherShip, proposedNewOfficersOnOther))
			{
				TIOfficerState tiofficerState = otherShip.officers.FirstOrDefault<TIOfficerState>((TIOfficerState x) => x.templateName == this.templateName);
				if (tiofficerState == null)
				{
					tiofficerState = proposedNewOfficersOnOther.FirstOrDefault<TIOfficerState>((TIOfficerState x) => x.templateName == this.templateName);
				}
				return tiofficerState;
			}
			return null;
		}

		// Token: 0x06004135 RID: 16693 RVA: 0x001A4E50 File Offset: 0x001A3050
		public bool CanTransferOfficer(OfficerCarrierState currentLocation, OfficerCarrierState candidateLocation, bool overrideLocation, bool swap, int additionalProposedTransfersToCandidate)
		{
			TIGameState state = currentLocation.GetState();
			TIGameState state2 = candidateLocation.GetState();
			if (state.ref_faction == state2.ref_faction)
			{
				if (state.isSpaceShipState)
				{
					if (state2.isSpaceShipState)
					{
						return this.CanTransferOfficerBetweenShips(state.ref_ship, state2.ref_ship, overrideLocation, swap, additionalProposedTransfersToCandidate);
					}
					if (state2.isHabState)
					{
						return this.CanTransferOfficerToHab(state2.ref_hab, overrideLocation, swap, additionalProposedTransfersToCandidate);
					}
				}
				if (state.isHabState)
				{
					if (state2.isSpaceShipState)
					{
						return this.CanTransferOfficerFromHab(state.ref_hab, state2.ref_ship, overrideLocation, swap, additionalProposedTransfersToCandidate);
					}
					if (state2.isHabState)
					{
						return this.CanTransferOfficersBetweenHabs(state2.ref_hab, overrideLocation, additionalProposedTransfersToCandidate);
					}
				}
			}
			return false;
		}

		// Token: 0x06004136 RID: 16694 RVA: 0x001A4F04 File Offset: 0x001A3104
		public bool CanTransferOfficerBetweenShips(TISpaceShipState currentShip, TISpaceShipState candidateShip, bool overrideLocation, bool swap, int additionalProposedTransfersToShip)
		{
			return TIGameState.Valid(currentShip) && TIGameState.Valid(candidateShip) && currentShip != candidateShip && (overrideLocation || currentShip.fleet == candidateShip.fleet || (currentShip.faction == candidateShip.faction && currentShip.fleet.dockedOrLanded && currentShip.fleet.dockedLocation == candidateShip.fleet.dockedLocation)) && this.OfficerAllowedForShip(candidateShip, swap, additionalProposedTransfersToShip);
		}

		// Token: 0x06004137 RID: 16695 RVA: 0x001A4F8A File Offset: 0x001A318A
		public bool CanTransferOfficerFromHab(TIHabState hab, TISpaceShipState candidateShip, bool overrideLocation, bool swap, int additionalProposedTransfersToShip)
		{
			return TIGameState.Valid(candidateShip) && TIGameState.Valid(hab) && (overrideLocation || candidateShip.fleet.dockedLocation == hab) && this.OfficerAllowedForShip(candidateShip, swap, additionalProposedTransfersToShip);
		}

		// Token: 0x06004138 RID: 16696 RVA: 0x001A4FC0 File Offset: 0x001A31C0
		public bool CanTransferOfficerToHab(TIHabState hab, bool overrideLocation, bool swap, int additionalProposedTransfersToHab)
		{
			return TIGameState.Valid(hab) && hab.faction == this.ref_faction && hab.CanStoreOfficer(swap, additionalProposedTransfersToHab) && (overrideLocation || this.ship.fleet.dockedLocation == hab);
		}

		// Token: 0x06004139 RID: 16697 RVA: 0x001A5010 File Offset: 0x001A3210
		public bool CanTransferOfficersBetweenHabs(TIHabState destination, bool overrideLocation, int additionalProposedTransfersToHab)
		{
			return overrideLocation && TIGameState.Valid(this.hab) && TIGameState.Valid(destination) && this.hab.faction == destination.faction && destination.CanStoreOfficer(false, additionalProposedTransfersToHab);
		}

		// Token: 0x0600413A RID: 16698 RVA: 0x001A504C File Offset: 0x001A324C
		public TIResourcesCost CostToTransfer(OfficerCarrierState destination)
		{
			if (destination.GetState().isSpaceShipState && this.template.GetOfficerEffects(OfficerEffectType.NoCostToTransfer, this.rank).Count == 0)
			{
				return new TIResourcesCost(FactionResource.Influence, TemplateManager.global.officerTransferCostPerRank * (float)this.rank);
			}
			return new TIResourcesCost();
		}

		// Token: 0x0600413B RID: 16699 RVA: 0x001A50A0 File Offset: 0x001A32A0
		public bool AnyEligibleTransfers(bool allowSwapsAndBigfoots)
		{
			TIHabState testHab = this.hab ?? ((this.ship.fleet.ref_hab != null && this.ship.fleet.ref_hab.faction == this.ship.fleet.faction) ? this.ship.fleet.ref_hab : null);
			List<TISpaceFleetState> list = new List<TISpaceFleetState>();
			if (testHab != null)
			{
				list.AddRange(testHab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == testHab.faction));
			}
			else
			{
				TISpaceShipState ship = this.ship;
				if (((ship != null) ? ship.fleet : null) != null)
				{
					list.Add(this.ship.fleet);
				}
			}
			List<TISpaceShipState> list2 = list.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).ToList<TISpaceShipState>();
			if (this.ship != null)
			{
				list2.Remove(this.ship);
				if (testHab != null && this.hab == null && this.CanTransferOfficerToHab(testHab, false, false, 0))
				{
					return true;
				}
				using (IEnumerator<TISpaceShipState> enumerator = list.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator.Current;
						if (this.CanTransferOfficerBetweenShips(this.ship, tispaceShipState, false, allowSwapsAndBigfoots, 0))
						{
							return true;
						}
					}
					return false;
				}
			}
			if (this.hab != null)
			{
				foreach (TISpaceShipState tispaceShipState2 in list.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships))
				{
					if (this.CanTransferOfficerFromHab(this.hab, tispaceShipState2, false, allowSwapsAndBigfoots, 0))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600413C RID: 16700 RVA: 0x001A52E8 File Offset: 0x001A34E8
		public List<OfficerCarrierState> GetEligibleTransfers(bool allowSwapsAndBigfoots)
		{
			List<OfficerCarrierState> list = new List<OfficerCarrierState>();
			TIHabState testHab = this.hab ?? ((this.ship.fleet.ref_hab != null && this.ship.fleet.ref_hab.faction == this.ship.fleet.faction) ? this.ship.fleet.ref_hab : null);
			List<TISpaceFleetState> list2 = new List<TISpaceFleetState>();
			if (testHab != null)
			{
				list2.AddRange(testHab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == testHab.faction));
			}
			else
			{
				TISpaceShipState ship = this.ship;
				if (((ship != null) ? ship.fleet : null) != null)
				{
					list2.Add(this.ship.fleet);
				}
			}
			List<TISpaceShipState> list3 = list2.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).ToList<TISpaceShipState>();
			if (this.ship != null)
			{
				list3.Remove(this.ship);
				if (testHab != null && this.hab == null && this.CanTransferOfficerToHab(testHab, false, false, 0))
				{
					list.Add(testHab);
				}
				using (IEnumerator<TISpaceShipState> enumerator = list2.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceShipState tispaceShipState = enumerator.Current;
						if (this.CanTransferOfficerBetweenShips(this.ship, tispaceShipState, false, allowSwapsAndBigfoots, 0))
						{
							list.Add(tispaceShipState);
						}
					}
					return list;
				}
			}
			if (this.hab != null)
			{
				foreach (TISpaceShipState tispaceShipState2 in list2.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships))
				{
					if (this.CanTransferOfficerFromHab(this.hab, tispaceShipState2, false, allowSwapsAndBigfoots, 0))
					{
						list.Add(tispaceShipState2);
					}
				}
			}
			return list;
		}

		// Token: 0x0600413D RID: 16701 RVA: 0x001A5550 File Offset: 0x001A3750
		public bool Promote()
		{
			if (TIGameState.Valid(this.ship))
			{
				int rank = this.rank;
				this.rank = Mathf.Clamp(this.rank + 1, 1, 3);
				if (this.ref_faction.isActivePlayer && this.rank > rank && this.rank == 3 && this.templateName == "Officer_Admiral")
				{
					this.ref_faction.UnlockAchievement("admiral");
				}
				if (this.rank > this.maxRank)
				{
					this.retirementDate.AddYears(5);
				}
				this.maxRank = this.rank;
				if (rank != this.rank)
				{
					GameControl.eventManager.TriggerEvent(new ShipOfficerPromoted(this), null, new object[] { this, this.ship });
					this.SetDisplayName();
					this.OnOfficerChange();
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600413E RID: 16702 RVA: 0x001A562C File Offset: 0x001A382C
		public void RetireOfficer()
		{
			TINotificationQueueState.LogOfficerRetires(this);
			this.DeleteOfficer(false);
		}

		// Token: 0x0600413F RID: 16703 RVA: 0x001A563C File Offset: 0x001A383C
		public void DeleteOfficer(bool KIA)
		{
			if (TIGameState.Valid(this.ship))
			{
				if (KIA)
				{
					GameControl.eventManager.TriggerEvent(new ShipOfficerKilled(this.ship, this.officerName, this.DisplayNameAndJob), null, new object[] { this.ship });
				}
				this.OnOfficerChange();
				this.ship.officers.Remove(this);
			}
			else if (TIGameState.Valid(this.hab))
			{
				this.hab.officersOnBoard.Remove(this);
			}
			GameStateManager.RemoveGameState<TIOfficerState>(base.ID, false);
		}

		// Token: 0x06004140 RID: 16704 RVA: 0x001A56D0 File Offset: 0x001A38D0
		public bool TransferOfficerBetweenShips(TISpaceShipState newShip, bool refitTransfer, bool swap, bool overrideChecks = false)
		{
			if (refitTransfer || overrideChecks || this.CanTransferOfficerBetweenShips(this.ship, newShip, false, swap, 0))
			{
				if (TIGameState.Valid(this.ship))
				{
					this.priorShips.AddUnique(this.ship);
					this.ship.officers.Remove(this);
				}
				if (!refitTransfer)
				{
					this.OnOfficerChange();
				}
				newShip.officers.Add(this);
				newShip.CheckOfficersOnShipAchievement();
				this.ship = newShip;
				this.OnOfficerChange();
				this.hab = null;
				return true;
			}
			return false;
		}

		// Token: 0x06004141 RID: 16705 RVA: 0x001A5758 File Offset: 0x001A3958
		public bool TransferOfficer_FromHabToShip(TISpaceShipState newShip, bool swap, bool skipValidation = false)
		{
			if (skipValidation || this.CanTransferOfficer(this.hab, newShip, skipValidation, swap, 0))
			{
				if (TIGameState.Valid(this.hab))
				{
					this.hab.officersOnBoard.Remove(this);
				}
				this.OnOfficerChange();
				newShip.officers.Add(this);
				newShip.CheckOfficersOnShipAchievement();
				this.ship = newShip;
				this.hab = null;
				this.OnOfficerChange();
				return true;
			}
			return false;
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x001A57C8 File Offset: 0x001A39C8
		public bool TransferOfficer_ToHab(TIHabState hab, bool overrideLocation, bool swap, bool skipValidation = false)
		{
			if (skipValidation || this.CanTransferOfficerToHab(hab, overrideLocation, swap, 0))
			{
				if (TIGameState.Valid(this.ship))
				{
					this.priorShips.AddUnique(this.ship);
					this.ship.officers.Remove(this);
				}
				hab.officersOnBoard.Add(this);
				this.OnOfficerChange();
				this.ship = null;
				this.hab = hab;
				return true;
			}
			return false;
		}

		// Token: 0x06004143 RID: 16707 RVA: 0x001A5839 File Offset: 0x001A3A39
		public bool TransferOfficerBetweenHabs(TIHabState destination)
		{
			if (this.CanTransferOfficersBetweenHabs(destination, true, 0))
			{
				this.hab.officersOnBoard.Remove(this);
				destination.officersOnBoard.Add(this);
				this.hab = destination;
				return true;
			}
			return false;
		}

		// Token: 0x06004144 RID: 16708 RVA: 0x001A586E File Offset: 0x001A3A6E
		public bool ValidEscapeHab(TIHabState destination)
		{
			return TIGameState.Valid(destination) && destination != this.hab && destination.faction == this.ref_faction && destination.CanStoreOfficer(false, 0);
		}

		// Token: 0x06004145 RID: 16709 RVA: 0x001A58A4 File Offset: 0x001A3AA4
		public bool Escape(bool allowToSameFleet, bool forceToDockedHab)
		{
			if (!this.location.GetState().isSpaceShipState)
			{
				return false;
			}
			if (forceToDockedHab && this.ship != null && this.ship.fleet.dockedAtHab && this.ship.fleet.dockedLocation.ref_faction == this.ship.faction && this.TransferOfficer_ToHab(this.ship.fleet.dockedLocation.ref_hab, true, false, false))
			{
				return true;
			}
			TINaturalSpaceObjectState ref_naturalSpaceObject = this.ref_naturalSpaceObject;
			List<TIHabState> list = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.habsInSystem.Where<TIHabState>((TIHabState x) => this.ValidEscapeHab(x)).ToList<TIHabState>() : null) ?? new List<TIHabState>();
			if (list.Count <= 0)
			{
				List<TISpaceFleetState> list2 = new List<TISpaceFleetState>();
				if (this.hab != null)
				{
					list2 = this.hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == this.ref_faction).ToList<TISpaceFleetState>();
					if (list2.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).None<TISpaceShipState>((TISpaceShipState x) => this.OfficerAllowedForShip(x, false, 0)))
					{
						list2 = (from x in this.hab.GetNearbyIdleAlliedFleets(null)
							where x.faction == this.ref_faction
							select x).ToList<TISpaceFleetState>();
					}
				}
				else
				{
					list2 = (from x in this.ship.fleet.GetNearbyIdleAlliedFleets(null)
						where x.faction == this.ref_faction
						select x).ToList<TISpaceFleetState>();
				}
				if (this.ref_fleet != null)
				{
					if (allowToSameFleet)
					{
						list2.AddUnique(this.ref_fleet);
					}
					else
					{
						list2.Remove(this.ref_fleet);
					}
				}
				if (list2.Count > 0)
				{
					List<TISpaceShipState> list3 = new List<TISpaceShipState>();
					if (allowToSameFleet && list2.Contains(this.ref_fleet))
					{
						list3 = this.ref_fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x != this.ship && this.OfficerAllowedForShip(x, false, 0)).ToList<TISpaceShipState>();
					}
					if (list3.Count == 0)
					{
						list3 = (from x in list2.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships)
							where x != this.ship && this.OfficerAllowedForShip(x, false, 0)
							select x).ToList<TISpaceShipState>();
					}
					if (list3.Count > 0)
					{
						if (this.hab != null)
						{
							return this.TransferOfficer_FromHabToShip(list3.SelectRandomItem<TISpaceShipState>(), false, false);
						}
						return this.TransferOfficerBetweenShips(list3.SelectRandomItem<TISpaceShipState>(), false, false, false);
					}
				}
				return false;
			}
			if (this.hab != null)
			{
				return this.TransferOfficerBetweenHabs(list.SelectRandomItem<TIHabState>());
			}
			return this.TransferOfficer_ToHab(list.SelectRandomItem<TIHabState>(), true, false, false);
		}

		// Token: 0x06004146 RID: 16710 RVA: 0x001A5B3F File Offset: 0x001A3D3F
		public void OnOfficerChange()
		{
			if (TIGameState.Valid(this.ship))
			{
				this.ship.SetMissionControlConsumption();
				this.ship.SetPropulsionValuesDirty(true, false);
			}
		}

		// Token: 0x06004147 RID: 16711 RVA: 0x001A5B68 File Offset: 0x001A3D68
		public float SumOfficerEffects(OfficerEffectType effectType, float baseValue)
		{
			float num = baseValue;
			foreach (OfficerEffect officerEffect in this.template.GetOfficerEffectsByLevel(this.rank))
			{
				if (officerEffect.effect == effectType)
				{
					switch (TIOfficerTemplate.OfficerEffectOperation[effectType])
					{
					case StatModSetOperation.SetToFixedValue:
						num = officerEffect.value;
						break;
					case StatModSetOperation.IncreaseToValue:
						if (num < officerEffect.value)
						{
							num = officerEffect.value;
						}
						break;
					case StatModSetOperation.DecreaseToValue:
						if (num > officerEffect.value)
						{
							num = officerEffect.value;
						}
						break;
					case StatModSetOperation.Additive:
						num += officerEffect.value;
						break;
					case StatModSetOperation.Multiplicative:
						num *= officerEffect.value;
						break;
					}
				}
			}
			return num - baseValue;
		}

		// Token: 0x17000C0B RID: 3083
		// (get) Token: 0x06004148 RID: 16712 RVA: 0x001A5C44 File Offset: 0x001A3E44
		public OfficerCarrierState location
		{
			get
			{
				OfficerCarrierState ship = this.ship;
				return ship ?? this.hab;
			}
		}

		// Token: 0x06004149 RID: 16713 RVA: 0x001A5C64 File Offset: 0x001A3E64
		public static string RankStarsInline(int rank)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < rank; i++)
			{
				stringBuilder.Append(TemplateManager.global.starInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000C0C RID: 3084
		// (get) Token: 0x0600414A RID: 16714 RVA: 0x001A5C9A File Offset: 0x001A3E9A
		// (set) Token: 0x0600414B RID: 16715 RVA: 0x001A5CA2 File Offset: 0x001A3EA2
		public bool isDummy { get; private set; }

		// Token: 0x0600414C RID: 16716 RVA: 0x001A5CAC File Offset: 0x001A3EAC
		public TIOfficerState CreateDummy(TISpaceShipState ship)
		{
			TIOfficerState tiofficerState = Activator.CreateInstance(typeof(TIOfficerState)) as TIOfficerState;
			tiofficerState.isDummy = true;
			tiofficerState.templateName = this.templateName;
			tiofficerState.rank = this.rank;
			tiofficerState.maxRank = this.maxRank;
			tiofficerState.creationDate = this.creationDate;
			tiofficerState.retirementDate = this.retirementDate;
			tiofficerState.officerName = this.officerName;
			tiofficerState.SetDisplayName();
			tiofficerState.ship = ship;
			ship.officers.Add(tiofficerState);
			return tiofficerState;
		}

		// Token: 0x0400279F RID: 10143
		public List<TISpaceShipState> priorShips = new List<TISpaceShipState>();
	}
}
