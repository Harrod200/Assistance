using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000153 RID: 339
public class TradeOffer
{
	// Token: 0x0600052E RID: 1326 RVA: 0x000168EB File Offset: 0x00014AEB
	public TradeOffer(TIFactionState offeringFaction)
	{
		this.offeringFaction = offeringFaction;
		this.Blank();
	}

	// Token: 0x0600052F RID: 1327 RVA: 0x00016900 File Offset: 0x00014B00
	public TradeOffer()
	{
		this.Blank();
	}

	// Token: 0x06000530 RID: 1328 RVA: 0x00016910 File Offset: 0x00014B10
	public void ModifyOffer(ResourceValue newValue)
	{
		if (this.resourceValues.Any<ResourceValue>((ResourceValue x) => x.resource == newValue.resource))
		{
			this.resourceValues.Single<ResourceValue>((ResourceValue x) => x.resource == newValue.resource).value = newValue.value;
			return;
		}
		this.resourceValues.Add(newValue);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001697F File Offset: 0x00014B7F
	public void ModifyOffer(TIProjectTemplate projectTemplate)
	{
		if (this.projects.Contains(projectTemplate))
		{
			this.projects.Remove(projectTemplate);
			return;
		}
		this.projects.Add(projectTemplate);
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x000169A9 File Offset: 0x00014BA9
	public void ModifyOffer(TISectorState habSector)
	{
		if (this.habSectors.Contains(habSector))
		{
			this.habSectors.Remove(habSector);
			return;
		}
		this.habSectors.Add(habSector);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x000169D3 File Offset: 0x00014BD3
	public void ModifyOffer(TIOrgState org)
	{
		if (this.orgs.Contains(org))
		{
			this.orgs.Remove(org);
			return;
		}
		this.orgs.Add(org);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x000169FD File Offset: 0x00014BFD
	public void ModifyOffer(TIControlPoint controlPoint)
	{
		if (this.controlPoints.Contains(controlPoint))
		{
			this.controlPoints.Remove(controlPoint);
			return;
		}
		this.controlPoints.Add(controlPoint);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x00016A28 File Offset: 0x00014C28
	public void ToggleAlienIntelOffer()
	{
		if (this.intelData.Any<TIGameState>((TIGameState x) => x.ref_councilor.isAlien))
		{
			List<TIGameState> list = new List<TIGameState>();
			foreach (TIGameState tigameState in this.intelData)
			{
				if (tigameState.ref_councilor.isAlien)
				{
					list.Add(tigameState);
				}
			}
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState2 = enumerator.Current;
					this.intelData.Remove(tigameState2);
				}
				return;
			}
		}
		List<TICouncilorState> list2 = this.offeringFaction.EnemyCouncilorsIHaveIntelOn(GameStateManager.AlienFaction(), false);
		if (list2.Count > 0)
		{
			this.intelData.AddRange(list2);
		}
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x00016B2C File Offset: 0x00014D2C
	public void ToggleHumanCouncilorsIntelOffer()
	{
		if (this.intelData.Any<TIGameState>((TIGameState x) => x.ref_councilor.isHuman))
		{
			List<TIGameState> list = new List<TIGameState>();
			foreach (TIGameState tigameState in this.intelData)
			{
				if (tigameState.ref_councilor.isHuman)
				{
					list.Add(tigameState);
				}
			}
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState2 = enumerator.Current;
					this.intelData.Remove(tigameState2);
				}
				return;
			}
		}
		List<TICouncilorState> list2 = this.offeringFaction.EnemyCouncilorsIHaveIntelOn(null, true);
		if (list2.Count > 0)
		{
			this.intelData.AddRange(list2);
		}
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x00016C2C File Offset: 0x00014E2C
	public void ToggleProspectorData()
	{
		if (this.intelData.Any<TIGameState>((TIGameState x) => x.isSpaceBodyState))
		{
			List<TIGameState> list = new List<TIGameState>();
			foreach (TIGameState tigameState in this.intelData)
			{
				if (tigameState.ref_spaceBody == tigameState)
				{
					list.Add(tigameState);
				}
			}
			using (List<TIGameState>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIGameState tigameState2 = enumerator.Current;
					this.intelData.Remove(tigameState2);
				}
				return;
			}
		}
		List<TISpaceBodyState> list2 = this.offeringFaction.ProspectedSpaceBodies();
		if (list2.Count > 0)
		{
			this.intelData.AddRange(list2);
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000538 RID: 1336 RVA: 0x00016D2C File Offset: 0x00014F2C
	public IEnumerable<FactionResource> ResourcesOffered
	{
		get
		{
			return this.resourceValues.Select<ResourceValue, FactionResource>((ResourceValue x) => x.resource).Distinct<FactionResource>();
		}
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x00016D60 File Offset: 0x00014F60
	public float GetResourceQuantityOffered(FactionResource resource)
	{
		TIFactionState tifactionState = this.offeringFaction;
		return this.resourceValues.Where<ResourceValue>((ResourceValue x) => x.resource == resource).Sum<ResourceValue>((ResourceValue x) => x.value);
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x00016DBC File Offset: 0x00014FBC
	public TradeOffer MergeWith(TradeOffer otherOffer)
	{
		TradeOffer tradeOffer = new TradeOffer();
		tradeOffer.offeringFaction = this.offeringFaction;
		tradeOffer.resourceValues = (from x in this.resourceValues.Concat<ResourceValue>(otherOffer.resourceValues)
			group x by x.resource into x
			select new ResourceValue(x.Key, x.Sum<ResourceValue>((ResourceValue y) => y.value))).ToList<ResourceValue>();
		tradeOffer.projects = this.projects.Union<TIProjectTemplate>(otherOffer.projects).ToList<TIProjectTemplate>();
		tradeOffer.habSectors = this.habSectors.Union<TISectorState>(otherOffer.habSectors).ToList<TISectorState>();
		tradeOffer.habs = this.habs.Union<TIHabState>(otherOffer.habs).ToList<TIHabState>();
		tradeOffer.controlPoints = this.controlPoints.Union<TIControlPoint>(otherOffer.controlPoints).ToList<TIControlPoint>();
		tradeOffer.orgs = this.orgs.Union<TIOrgState>(otherOffer.orgs).ToList<TIOrgState>();
		tradeOffer.intelData = this.intelData.Union<TIGameState>(otherOffer.intelData).ToList<TIGameState>();
		tradeOffer.treatyType = ((this.treatyType != TradeOffer.TreatyType.None) ? this.treatyType : otherOffer.treatyType);
		tradeOffer.intelExchange = this.intelExchange || otherOffer.intelExchange;
		return tradeOffer;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00016F1C File Offset: 0x0001511C
	public void BecomeCopyOf(TradeOffer offer)
	{
		this.offeringFaction = offer.offeringFaction;
		this.resourceValues = offer.resourceValues.ToList<ResourceValue>();
		this.projects = offer.projects.ToList<TIProjectTemplate>();
		this.habSectors = offer.habSectors.ToList<TISectorState>();
		this.habs = offer.habs.ToList<TIHabState>();
		this.controlPoints = offer.controlPoints.ToList<TIControlPoint>();
		this.orgs = offer.orgs.ToList<TIOrgState>();
		this.intelData = offer.intelData.ToList<TIGameState>();
		this.treatyType = offer.treatyType;
		this.intelExchange = offer.intelExchange;
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00016FC4 File Offset: 0x000151C4
	public TradeOffer Copy()
	{
		TradeOffer tradeOffer = new TradeOffer();
		tradeOffer.BecomeCopyOf(this);
		return tradeOffer;
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x00016FD4 File Offset: 0x000151D4
	public void Blank()
	{
		this.resourceValues = new List<ResourceValue>();
		this.orgs = new List<TIOrgState>();
		this.controlPoints = new List<TIControlPoint>();
		this.habSectors = new List<TISectorState>();
		this.habs = new List<TIHabState>();
		this.projects = new List<TIProjectTemplate>();
		this.intelData = new List<TIGameState>();
		this.treatyType = TradeOffer.TreatyType.None;
	}

	// Token: 0x0400024D RID: 589
	public TIFactionState offeringFaction;

	// Token: 0x0400024E RID: 590
	public List<ResourceValue> resourceValues;

	// Token: 0x0400024F RID: 591
	public List<TIProjectTemplate> projects;

	// Token: 0x04000250 RID: 592
	public List<TISectorState> habSectors;

	// Token: 0x04000251 RID: 593
	public List<TIHabState> habs;

	// Token: 0x04000252 RID: 594
	public List<TIControlPoint> controlPoints;

	// Token: 0x04000253 RID: 595
	public List<TIOrgState> orgs;

	// Token: 0x04000254 RID: 596
	public List<TIGameState> intelData;

	// Token: 0x04000255 RID: 597
	public TradeOffer.TreatyType treatyType;

	// Token: 0x04000256 RID: 598
	public bool intelExchange;

	// Token: 0x02000AEC RID: 2796
	public enum TreatyType
	{
		// Token: 0x040048EF RID: 18671
		None,
		// Token: 0x040048F0 RID: 18672
		Truce,
		// Token: 0x040048F1 RID: 18673
		NAP,
		// Token: 0x040048F2 RID: 18674
		Intel
	}

	// Token: 0x02000AED RID: 2797
	public struct TradeAgreement
	{
		// Token: 0x17001137 RID: 4407
		// (get) Token: 0x06006698 RID: 26264 RVA: 0x002FFA3D File Offset: 0x002FDC3D
		public IEnumerable<TIFactionState> Factions
		{
			get
			{
				return Enumerable.Empty<TIFactionState>().Append(this.OfferA.offeringFaction).Append(this.OfferB.offeringFaction);
			}
		}

		// Token: 0x06006699 RID: 26265 RVA: 0x002FFA64 File Offset: 0x002FDC64
		public TradeOffer GetOffer(TIFactionState faction)
		{
			if (this.OfferA.offeringFaction == faction)
			{
				return this.OfferA;
			}
			if (this.OfferB.offeringFaction == faction)
			{
				return this.OfferB;
			}
			throw new Exception("Faction is not a part of this TradeAgreement");
		}

		// Token: 0x0600669A RID: 26266 RVA: 0x002FFAA4 File Offset: 0x002FDCA4
		public TradeOffer GetOtherPartysOffer(TIFactionState faction)
		{
			if (this.OfferA.offeringFaction == faction)
			{
				return this.OfferB;
			}
			if (this.OfferB.offeringFaction == faction)
			{
				return this.OfferA;
			}
			throw new Exception("Faction is not a part of this TradeAgreement");
		}

		// Token: 0x17001138 RID: 4408
		// (get) Token: 0x0600669B RID: 26267 RVA: 0x002FFAE4 File Offset: 0x002FDCE4
		public IEnumerable<FactionResource> ResourcesTraded
		{
			get
			{
				return this.OfferA.ResourcesOffered.Union<FactionResource>(this.OfferB.ResourcesOffered);
			}
		}

		// Token: 0x0600669C RID: 26268 RVA: 0x002FFB04 File Offset: 0x002FDD04
		public float GetResourceQuantityReceived(TIFactionState faction, FactionResource resource)
		{
			float resourceQuantityOffered = this.GetOffer(faction).GetResourceQuantityOffered(resource);
			return this.GetOtherPartysOffer(faction).GetResourceQuantityOffered(resource) - resourceQuantityOffered;
		}

		// Token: 0x0600669D RID: 26269 RVA: 0x002FFB30 File Offset: 0x002FDD30
		public static implicit operator TradeOffer.TradeAgreement([TupleElementNames(new string[] { "A", "B" })] ValueTuple<TradeOffer, TradeOffer> offers)
		{
			return new TradeOffer.TradeAgreement
			{
				OfferA = offers.Item1,
				OfferB = offers.Item2
			};
		}

		// Token: 0x040048F3 RID: 18675
		public TradeOffer OfferA;

		// Token: 0x040048F4 RID: 18676
		public TradeOffer OfferB;
	}
}
