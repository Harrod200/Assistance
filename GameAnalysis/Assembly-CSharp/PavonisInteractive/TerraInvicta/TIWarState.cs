using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077B RID: 1915
	public class TIWarState : TIGameState
	{
		// Token: 0x17000A7C RID: 2684
		// (get) Token: 0x06003B3F RID: 15167 RVA: 0x0015DBEB File Offset: 0x0015BDEB
		// (set) Token: 0x06003B40 RID: 15168 RVA: 0x0015DBF3 File Offset: 0x0015BDF3
		public TINationState originalAttacker { get; private set; }

		// Token: 0x17000A7D RID: 2685
		// (get) Token: 0x06003B41 RID: 15169 RVA: 0x0015DBFC File Offset: 0x0015BDFC
		// (set) Token: 0x06003B42 RID: 15170 RVA: 0x0015DC04 File Offset: 0x0015BE04
		public TINationState originalDefender { get; private set; }

		// Token: 0x17000A7E RID: 2686
		// (get) Token: 0x06003B43 RID: 15171 RVA: 0x0015DC0D File Offset: 0x0015BE0D
		// (set) Token: 0x06003B44 RID: 15172 RVA: 0x0015DC15 File Offset: 0x0015BE15
		public TINationState attacker { get; private set; }

		// Token: 0x17000A7F RID: 2687
		// (get) Token: 0x06003B45 RID: 15173 RVA: 0x0015DC1E File Offset: 0x0015BE1E
		// (set) Token: 0x06003B46 RID: 15174 RVA: 0x0015DC26 File Offset: 0x0015BE26
		public TINationState defender { get; private set; }

		// Token: 0x17000A80 RID: 2688
		// (get) Token: 0x06003B47 RID: 15175 RVA: 0x0015DC2F File Offset: 0x0015BE2F
		// (set) Token: 0x06003B48 RID: 15176 RVA: 0x0015DC37 File Offset: 0x0015BE37
		public TIDateTime startDate { get; private set; }

		// Token: 0x17000A81 RID: 2689
		// (get) Token: 0x06003B49 RID: 15177 RVA: 0x0015DC40 File Offset: 0x0015BE40
		// (set) Token: 0x06003B4A RID: 15178 RVA: 0x0015DC48 File Offset: 0x0015BE48
		public List<TIRegionState> nukedRegions { get; private set; }

		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x06003B4B RID: 15179 RVA: 0x0015DC51 File Offset: 0x0015BE51
		// (set) Token: 0x06003B4C RID: 15180 RVA: 0x0015DC59 File Offset: 0x0015BE59
		public List<TIRegionState> annexedRegions { get; private set; }

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x06003B4D RID: 15181 RVA: 0x0015DC62 File Offset: 0x0015BE62
		// (set) Token: 0x06003B4E RID: 15182 RVA: 0x0015DC6A File Offset: 0x0015BE6A
		public int defensiveNukes { get; private set; }

		// Token: 0x17000A84 RID: 2692
		// (get) Token: 0x06003B4F RID: 15183 RVA: 0x0015DC73 File Offset: 0x0015BE73
		// (set) Token: 0x06003B50 RID: 15184 RVA: 0x0015DC7B File Offset: 0x0015BE7B
		public TIDateTime dateOfLastFighting { get; private set; }

		// Token: 0x17000A85 RID: 2693
		// (get) Token: 0x06003B51 RID: 15185 RVA: 0x0015DC84 File Offset: 0x0015BE84
		public TINationState attackingAllianceLeader
		{
			get
			{
				return this._attackingAlliance.MaxBy<TINationState, float>((TINationState x) => x.militaryStrength);
			}
		}

		// Token: 0x17000A86 RID: 2694
		// (get) Token: 0x06003B52 RID: 15186 RVA: 0x0015DCB0 File Offset: 0x0015BEB0
		public TINationState defendingAllianceLeader
		{
			get
			{
				return this._defendingAlliance.MaxBy<TINationState, float>((TINationState x) => x.militaryStrength);
			}
		}

		// Token: 0x17000A87 RID: 2695
		// (get) Token: 0x06003B53 RID: 15187 RVA: 0x0015DCDC File Offset: 0x0015BEDC
		public List<TINationState> allBelligerents
		{
			get
			{
				return this._attackingAlliance.Union<TINationState>(this._defendingAlliance).ToList<TINationState>();
			}
		}

		// Token: 0x17000A88 RID: 2696
		// (get) Token: 0x06003B54 RID: 15188 RVA: 0x0015DCF4 File Offset: 0x0015BEF4
		public string displayNameWithArticle
		{
			get
			{
				return Loc.T("UI.Intel.WarNameWithArticle", new object[] { this.displayName });
			}
		}

		// Token: 0x17000A89 RID: 2697
		// (get) Token: 0x06003B55 RID: 15189 RVA: 0x0015DD0F File Offset: 0x0015BF0F
		public override bool isWarState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A8A RID: 2698
		// (get) Token: 0x06003B56 RID: 15190 RVA: 0x0015DD12 File Offset: 0x0015BF12
		public override TIWarState ref_war
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000A8B RID: 2699
		// (get) Token: 0x06003B57 RID: 15191 RVA: 0x0015DD15 File Offset: 0x0015BF15
		public bool stalemate
		{
			get
			{
				return TITimeState.Now().DifferenceInDays(this.dateOfLastFighting) > 150.0;
			}
		}

		// Token: 0x17000A8C RID: 2700
		// (get) Token: 0x06003B58 RID: 15192 RVA: 0x0015DD32 File Offset: 0x0015BF32
		public float stalemateDuration_days
		{
			get
			{
				return Mathf.Max(0f, (float)TITimeState.Now().DifferenceInDays(this.dateOfLastFighting) - 150f);
			}
		}

		// Token: 0x17000A8D RID: 2701
		// (get) Token: 0x06003B59 RID: 15193 RVA: 0x0015DD58 File Offset: 0x0015BF58
		public override List<TIFactionState> ref_factions
		{
			get
			{
				List<TINationState> list = new List<TINationState>(this.attackingAlliance);
				list.AddRange(this.defendingAlliance);
				return list.SelectMany<TINationState, TIFactionState>((TINationState x) => x.ref_factions).Distinct<TIFactionState>().ToList<TIFactionState>();
			}
		}

		// Token: 0x17000A8E RID: 2702
		// (get) Token: 0x06003B5A RID: 15194 RVA: 0x0015DDAA File Offset: 0x0015BFAA
		public IReadOnlyList<TINationState> attackingAlliance
		{
			get
			{
				return this._attackingAlliance.AsReadOnly();
			}
		}

		// Token: 0x17000A8F RID: 2703
		// (get) Token: 0x06003B5B RID: 15195 RVA: 0x0015DDB7 File Offset: 0x0015BFB7
		public IReadOnlyList<TINationState> defendingAlliance
		{
			get
			{
				return this._defendingAlliance.AsReadOnly();
			}
		}

		// Token: 0x06003B5C RID: 15196 RVA: 0x0015DDC4 File Offset: 0x0015BFC4
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.nukedRegions == null)
			{
				this.nukedRegions = new List<TIRegionState>();
			}
			if (this.cohesionGainByNation == null)
			{
				this.cohesionGainByNation = new Dictionary<TINationState, float>();
			}
			if (this.annexedRegions == null)
			{
				this.annexedRegions = new List<TIRegionState>();
			}
			if (this.dateOfLastFighting == null)
			{
				this.dateOfLastFighting = TITimeState.Now();
			}
		}

		// Token: 0x06003B5D RID: 15197 RVA: 0x0015DE28 File Offset: 0x0015C028
		public void SetWarData(TINationState attacker, TINationState defender, List<TINationState> attackingAlliance, List<TINationState> defendingAlliance, TIDateTime startDate)
		{
			this.originalAttacker = attacker;
			this.originalDefender = defender;
			this.attacker = attacker;
			this.defender = defender;
			this._attackingAlliance = new List<TINationState>(attackingAlliance);
			this._defendingAlliance = new List<TINationState>(defendingAlliance);
			this._attackingAlliance.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength);
			this._defendingAlliance.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength);
			this.startDate = startDate;
			this.displayName = Loc.T("UI.Intel.WarName", new object[] { attacker.displayName, defender.displayName });
			this.nukedRegions = new List<TIRegionState>();
			this.dateOfLastFighting = new TIDateTime(startDate);
			if (this.cohesionGainByNation == null)
			{
				this.cohesionGainByNation = new Dictionary<TINationState, float>();
			}
			if (this.annexedRegions == null)
			{
				this.annexedRegions = new List<TIRegionState>();
			}
			foreach (TINationState tinationState in attackingAlliance)
			{
				this.cohesionGainByNation.Add(tinationState, 0f);
			}
			foreach (TINationState tinationState2 in defendingAlliance)
			{
				this.cohesionGainByNation.Add(tinationState2, 0f);
			}
		}

		// Token: 0x06003B5E RID: 15198 RVA: 0x0015DFC4 File Offset: 0x0015C1C4
		public bool DuplicateWar(TIWarState war)
		{
			if (war != this && this.startDate <= war.startDate)
			{
				IEnumerable<TINationState> enumerable = this.attackingAlliance.Except<TINationState>(war.attackingAlliance).ToList<TINationState>();
				List<TINationState> list = war.attackingAlliance.Except<TINationState>(this.attackingAlliance).ToList<TINationState>();
				if (!enumerable.Any<TINationState>() && !list.Any<TINationState>())
				{
					IEnumerable<TINationState> enumerable2 = this.defendingAlliance.Except<TINationState>(war.defendingAlliance).ToList<TINationState>();
					List<TINationState> list2 = war.defendingAlliance.Except<TINationState>(this.defendingAlliance).ToList<TINationState>();
					if (!enumerable2.Any<TINationState>() && !list2.Any<TINationState>())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003B5F RID: 15199 RVA: 0x0015E06C File Offset: 0x0015C26C
		public TIWarState AgglomerateDuplicateWars()
		{
			List<TIWarState> list = new List<TIWarState>();
			foreach (TIWarState tiwarState in TIGlobalValuesState.GlobalValues.interstateWars.OrderBy<TIWarState, TIDateTime>((TIWarState x) => x.startDate).ToList<TIWarState>())
			{
				if (this.DuplicateWar(tiwarState))
				{
					list.Add(tiwarState);
				}
			}
			foreach (TIWarState tiwarState2 in list)
			{
				this.nukedRegions.AddRange(tiwarState2.nukedRegions);
				foreach (KeyValuePair<TINationState, List<TIDateTime>> keyValuePair in tiwarState2.peaceOfferHistory)
				{
					if (this.peaceOfferHistory.ContainsKey(keyValuePair.Key))
					{
						this.peaceOfferHistory[keyValuePair.Key].AddRange(keyValuePair.Value);
					}
					else
					{
						this.peaceOfferHistory.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				foreach (TINationState tinationState in tiwarState2.cohesionGainByNation.Keys)
				{
					if (!this.cohesionGainByNation.ContainsKey(tinationState))
					{
						this.cohesionGainByNation.Add(tinationState, 0f);
					}
					Dictionary<TINationState, float> dictionary = this.cohesionGainByNation;
					TINationState tinationState2 = tinationState;
					dictionary[tinationState2] += tiwarState2.cohesionGainByNation[tinationState];
				}
				foreach (TINationState tinationState3 in tiwarState2.attackingAlliance)
				{
					foreach (TINationState tinationState4 in tiwarState2.defendingAlliance)
					{
						this.attacker.wars.Remove(tinationState4);
						this.defender.wars.Remove(tinationState3);
					}
				}
				TIGlobalValuesState.GlobalValues.DeleteWar(tiwarState2);
			}
			return this;
		}

		// Token: 0x06003B60 RID: 15200 RVA: 0x0015E354 File Offset: 0x0015C554
		public void JoinAttackers(TINationState nation)
		{
			this._attackingAlliance.Add(nation);
			this._attackingAlliance = this._attackingAlliance.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>();
			if (!this.cohesionGainByNation.ContainsKey(nation))
			{
				this.cohesionGainByNation.Add(nation, 0f);
			}
		}

		// Token: 0x06003B61 RID: 15201 RVA: 0x0015E3C4 File Offset: 0x0015C5C4
		public void JoinDefenders(TINationState nation)
		{
			this._defendingAlliance.Add(nation);
			this._defendingAlliance = this._defendingAlliance.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>();
			if (!this.cohesionGainByNation.ContainsKey(nation))
			{
				this.cohesionGainByNation.Add(nation, 0f);
			}
		}

		// Token: 0x06003B62 RID: 15202 RVA: 0x0015E431 File Offset: 0x0015C631
		public bool LeaveWar(TINationState nation)
		{
			if (this.attackingAlliance.Contains(nation))
			{
				return this.LeaveAttackers(nation);
			}
			return this.defendingAlliance.Contains(nation) && this.LeaveDefenders(nation);
		}

		// Token: 0x06003B63 RID: 15203 RVA: 0x0015E460 File Offset: 0x0015C660
		private bool LeaveAttackers(TINationState nation)
		{
			this._attackingAlliance.Remove(nation);
			if (this._attackingAlliance.Count > 0 && nation == this.attacker)
			{
				this.attacker = this.attackingAllianceLeader;
			}
			return this._attackingAlliance.Count == 0;
		}

		// Token: 0x06003B64 RID: 15204 RVA: 0x0015E4B0 File Offset: 0x0015C6B0
		private bool LeaveDefenders(TINationState nation)
		{
			this._defendingAlliance.Remove(nation);
			if (this._defendingAlliance.Count > 0 && nation == this.defender)
			{
				this.defender = this.defendingAllianceLeader;
			}
			return this._defendingAlliance.Count == 0;
		}

		// Token: 0x06003B65 RID: 15205 RVA: 0x0015E500 File Offset: 0x0015C700
		public List<TINationState> WarLeaders()
		{
			return new List<TINationState> { this.attackingAllianceLeader, this.defendingAllianceLeader };
		}

		// Token: 0x06003B66 RID: 15206 RVA: 0x0015E520 File Offset: 0x0015C720
		public List<TINationState> WarNationsWithNavalFreedom()
		{
			float num = this.attackingAlliance.Sum<TINationState>((TINationState x) => x.nationNavalScore);
			float num2 = this.defendingAlliance.Sum<TINationState>((TINationState x) => x.nationNavalScore);
			if (num > num2)
			{
				return this.attackingAlliance.ToList<TINationState>();
			}
			if (num2 > num)
			{
				return this.defendingAlliance.ToList<TINationState>();
			}
			List<TINationState> list = new List<TINationState>(this.attackingAlliance);
			list.AddRange(this.defendingAlliance);
			return list;
		}

		// Token: 0x06003B67 RID: 15207 RVA: 0x0015E5BA File Offset: 0x0015C7BA
		public TINationState AllianceWarLeader(TINationState nation)
		{
			if (this.attackingAlliance.Contains(nation))
			{
				return this.attackingAllianceLeader;
			}
			if (this.defendingAlliance.Contains(nation))
			{
				return this.defendingAllianceLeader;
			}
			return null;
		}

		// Token: 0x06003B68 RID: 15208 RVA: 0x0015E5E7 File Offset: 0x0015C7E7
		public IReadOnlyList<TINationState> Alliance(TINationState nation)
		{
			if (this.attackingAlliance.Contains(nation))
			{
				return this.attackingAlliance;
			}
			if (this.defendingAlliance.Contains(nation))
			{
				return this.defendingAlliance;
			}
			return new List<TINationState>();
		}

		// Token: 0x06003B69 RID: 15209 RVA: 0x0015E618 File Offset: 0x0015C818
		public IReadOnlyList<TINationState> ProspectiveAlliance(TINationState nation)
		{
			bool flag = nation.allies.Any<TINationState>((TINationState x) => this.attackingAlliance.Contains(x));
			bool flag2 = nation.allies.Any<TINationState>((TINationState x) => this.defendingAlliance.Contains(x));
			if (flag == flag2)
			{
				return new List<TINationState>();
			}
			if (flag)
			{
				return this.attackingAlliance;
			}
			return this.defendingAlliance;
		}

		// Token: 0x06003B6A RID: 15210 RVA: 0x0015E670 File Offset: 0x0015C870
		public TINationState EnemyWarLeader(TINationState nation, bool includeNonWarringAlliances = false)
		{
			if (this.attackingAlliance.Contains(nation))
			{
				return this.defendingAllianceLeader;
			}
			if (this.defendingAlliance.Contains(nation))
			{
				return this.attackingAllianceLeader;
			}
			if (includeNonWarringAlliances)
			{
				if (this.defender.InThisWar(this))
				{
					if (this.defendingAlliance.SelectMany<TINationState, TINationState>((TINationState x) => x.allies).Contains(nation))
					{
						return this.attackingAllianceLeader;
					}
				}
				if (this.attacker.InThisWar(this))
				{
					if (this.attackingAlliance.SelectMany<TINationState, TINationState>((TINationState x) => x.allies).Contains(nation))
					{
						return this.defendingAllianceLeader;
					}
				}
			}
			return null;
		}

		// Token: 0x06003B6B RID: 15211 RVA: 0x0015E73C File Offset: 0x0015C93C
		public IReadOnlyList<TINationState> EnemyAlliance(TINationState nation)
		{
			if (this.attackingAlliance.Contains(nation))
			{
				return this.defendingAlliance;
			}
			if (this.defendingAlliance.Contains(nation))
			{
				return this.attackingAlliance;
			}
			return new List<TINationState>();
		}

		// Token: 0x06003B6C RID: 15212 RVA: 0x0015E770 File Offset: 0x0015C970
		public IReadOnlyList<TINationState> ProspectiveEnemyAlliance(TINationState nation)
		{
			bool flag = nation.allies.Any<TINationState>((TINationState x) => this.attackingAlliance.Contains(x));
			bool flag2 = nation.allies.Any<TINationState>((TINationState x) => this.defendingAlliance.Contains(x));
			if (flag == flag2)
			{
				return new List<TINationState>();
			}
			if (flag)
			{
				return this.defendingAlliance;
			}
			return this.attackingAlliance;
		}

		// Token: 0x06003B6D RID: 15213 RVA: 0x0015E7C7 File Offset: 0x0015C9C7
		public void TallyDefensiveNuke()
		{
			this.defensiveNukes++;
		}

		// Token: 0x06003B6E RID: 15214 RVA: 0x0015E7D7 File Offset: 0x0015C9D7
		public void AddNukedRegion(TIRegionState region)
		{
			this.nukedRegions.Add(region);
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x0015E7E5 File Offset: 0x0015C9E5
		public void LogPeaceOffer(TINationState offerer)
		{
			if (!this.peaceOfferHistory.ContainsKey(offerer))
			{
				this.peaceOfferHistory[offerer] = new List<TIDateTime>();
			}
			this.peaceOfferHistory[offerer].Add(TITimeState.Now());
		}

		// Token: 0x06003B70 RID: 15216 RVA: 0x0015E81C File Offset: 0x0015CA1C
		public IEnumerable<TIDateTime> GetPeaceOffers(TINationState offerer)
		{
			if (!this.peaceOfferHistory.ContainsKey(offerer))
			{
				return Enumerable.Empty<TIDateTime>();
			}
			return this.peaceOfferHistory[offerer];
		}

		// Token: 0x06003B71 RID: 15217 RVA: 0x0015E83E File Offset: 0x0015CA3E
		public void FightingOccurs()
		{
			this.dateOfLastFighting = TITimeState.Now();
		}

		// Token: 0x06003B72 RID: 15218 RVA: 0x0015E84C File Offset: 0x0015CA4C
		public List<TIRegionState> ActiveOccupations(TINationState allianceMember, bool includeIncompleteOccupations, bool includeLiberations)
		{
			IReadOnlyList<TINationState> readOnlyList = this.Alliance(allianceMember);
			IReadOnlyList<TINationState> enemies = this.EnemyAlliance(allianceMember);
			List<TIRegionState> list = new List<TIRegionState>();
			foreach (TIRegionState tiregionState in enemies.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions))
			{
				if (tiregionState.IsFullyOccupied() && readOnlyList.Contains(tiregionState.GetLeadOccupierInFullOccupation()))
				{
					list.Add(tiregionState);
				}
				else if (includeIncompleteOccupations && tiregionState.OccupationUnderwayButNotComplete())
				{
					TINationState tinationState;
					List<TINationState> list2;
					tiregionState.GetHighestWarAllianceOccupationValue(out tinationState, out list2);
					if (list2.Count == readOnlyList.Count && list2.All<TINationState>(new Func<TINationState, bool>(readOnlyList.Contains<TINationState>)))
					{
						list.AddUnique(tiregionState);
					}
				}
			}
			if (includeLiberations)
			{
				IEnumerable<TIRegionState> enumerable = readOnlyList.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions);
				Func<KeyValuePair<TINationState, float>, bool> <>9__3;
				foreach (TIArmyState tiarmyState in readOnlyList.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies))
				{
					if (enumerable.Contains(tiarmyState.currentRegion) && tiarmyState.OccupyingRegion(true))
					{
						IEnumerable<KeyValuePair<TINationState, float>> occupations = tiarmyState.currentRegion.occupations;
						Func<KeyValuePair<TINationState, float>, bool> func;
						if ((func = <>9__3) == null)
						{
							func = (<>9__3 = (KeyValuePair<TINationState, float> x) => x.Value > 0f && enemies.Contains(x.Key));
						}
						if (occupations.Any<KeyValuePair<TINationState, float>>(func))
						{
							list.AddUnique(tiarmyState.currentRegion);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x040025C0 RID: 9664
		[SerializeField]
		private Dictionary<TINationState, List<TIDateTime>> peaceOfferHistory = new Dictionary<TINationState, List<TIDateTime>>();

		// Token: 0x040025C6 RID: 9670
		[SerializeField]
		private List<TINationState> _attackingAlliance;

		// Token: 0x040025C7 RID: 9671
		[SerializeField]
		private List<TINationState> _defendingAlliance;

		// Token: 0x040025CC RID: 9676
		public const float stalemate_days = 150f;

		// Token: 0x040025CD RID: 9677
		public Dictionary<TINationState, float> cohesionGainByNation;
	}
}
