using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F5 RID: 1781
	public static class GameStateManager
	{
		// Token: 0x060029F2 RID: 10738 RVA: 0x000E38D2 File Offset: 0x000E1AD2
		public static bool CampaignHasAlienFaction()
		{
			return GameStateManager.AlienFaction() != null;
		}

		// Token: 0x060029F3 RID: 10739 RVA: 0x000E38DF File Offset: 0x000E1ADF
		public static bool CampaignHasAlienProxy()
		{
			return GameStateManager.AlienProxy() != null;
		}

		// Token: 0x060029F4 RID: 10740 RVA: 0x000E38EC File Offset: 0x000E1AEC
		public static bool CampaignHasAlienAppeaser()
		{
			return GameStateManager.AlienAppeaser() != null;
		}

		// Token: 0x060029F5 RID: 10741 RVA: 0x000E38FC File Offset: 0x000E1AFC
		public static void ClearAllGameStates()
		{
			GameStateManager.nations = null;
			GameStateManager.regions = null;
			GameStateManager.factions = null;
			GameStateManager.humanFactions = null;
			GameStateManager.spaceBodies = null;
			GameStateManager.lagrangePoints = null;
			GameStateManager.naturalSpaceObjects = null;
			GameStateManager.orbits = null;
			GameStateManager.regionAlienEntities = null;
			GameStateManager.regionLookup = null;
			GameStateManager.nationLookup = null;
			GameStateManager.mapRegionLookup = null;
			GameStateManager.globalResearch = null;
			GameStateManager.globalValues = null;
			GameStateManager.notificationQueue = null;
			GameStateManager.promptQueue = null;
			GameStateManager.time = null;
			GameStateManager.effects = null;
			GameStateManager.missionPhase = null;
			GameStateManager.alienNation = null;
			GameStateManager.solState = null;
			GameStateManager.lowEarthOrbitStates = null;
			GameStateManager.nearEarthOrbitStates = null;
			GameStateManager.mercury = null;
			GameStateManager.venus = null;
			GameStateManager.earth = null;
			GameStateManager.luna = null;
			GameStateManager.mars = null;
			GameStateManager.ceres = null;
			GameStateManager.jupiter = null;
			GameStateManager.saturn = null;
			GameStateManager.uranus = null;
			GameStateManager.neptune = null;
			GameStateManager.innerSystemAsteroids = null;
			GameStateManager.innerAsteroidBelt = null;
			GameStateManager.midAsteroidBelt = null;
			GameStateManager.outerAsteroidBelt = null;
			GameStateManager.centaurs = null;
			GameStateManager.kuiperBeltObjects = null;
			GameStateManager.sunOrbitingLagrangePoints = null;
			GameStateManager.alienFaction = null;
			GameStateManager.alienProxyFaction = null;
			GameStateManager.alienAppeaserFaction = null;
			GameStateManager.activeIdeologies = null;
			GameStateManager.activeHumanIdeologies = null;
			GameStateManager.undecidedIdeology = null;
			GameStateManager.supraRegionMembers = null;
			GameStateManager.metaData = null;
			GameControl.SetActivePlayer(null);
			Dictionary<Type, Dictionary<GameStateID, TIGameState>> dictionary = GameStateManager.gamestates.ToDictionary<KeyValuePair<Type, Dictionary<GameStateID, TIGameState>>, Type, Dictionary<GameStateID, TIGameState>>((KeyValuePair<Type, Dictionary<GameStateID, TIGameState>> entry) => entry.Key, (KeyValuePair<Type, Dictionary<GameStateID, TIGameState>> entry) => new Dictionary<GameStateID, TIGameState>(entry.Value));
			foreach (Type type in dictionary.Keys)
			{
				foreach (GameStateID gameStateID in dictionary[type].Keys)
				{
					GameStateManager.RemoveGameState<TIGameState>(gameStateID, true);
				}
			}
			GameStateManager.templateCache.Clear();
			GameStateManager.gamestates.Clear();
			GameStateManager.currentID = new GameStateID(0);
		}

		// Token: 0x170005CA RID: 1482
		// (get) Token: 0x060029F6 RID: 10742 RVA: 0x000E3B20 File Offset: 0x000E1D20
		public static bool HasGamestates
		{
			get
			{
				return GameStateManager.gamestates.Count > 0;
			}
		}

		// Token: 0x060029F7 RID: 10743 RVA: 0x000E3B2F File Offset: 0x000E1D2F
		public static TISpaceBodyState Mercury()
		{
			if (GameStateManager.mercury == null)
			{
				GameStateManager.mercury = GameStateManager.FindByTemplate<TISpaceBodyState>("Mercury", false);
			}
			return GameStateManager.mercury;
		}

		// Token: 0x060029F8 RID: 10744 RVA: 0x000E3B53 File Offset: 0x000E1D53
		public static TISpaceBodyState Venus()
		{
			if (GameStateManager.venus == null)
			{
				GameStateManager.venus = GameStateManager.FindByTemplate<TISpaceBodyState>("Venus", false);
			}
			return GameStateManager.venus;
		}

		// Token: 0x060029F9 RID: 10745 RVA: 0x000E3B77 File Offset: 0x000E1D77
		public static TISpaceBodyState Earth()
		{
			if (GameStateManager.earth == null)
			{
				GameStateManager.earth = GameStateManager.FindByTemplate<TISpaceBodyState>("Earth", false);
			}
			return GameStateManager.earth;
		}

		// Token: 0x060029FA RID: 10746 RVA: 0x000E3B9B File Offset: 0x000E1D9B
		public static TISpaceBodyState Luna()
		{
			if (GameStateManager.luna == null)
			{
				GameStateManager.luna = GameStateManager.FindByTemplate<TISpaceBodyState>("Luna", false);
			}
			return GameStateManager.luna;
		}

		// Token: 0x060029FB RID: 10747 RVA: 0x000E3BBF File Offset: 0x000E1DBF
		public static TISpaceBodyState Mars()
		{
			if (GameStateManager.mars == null)
			{
				GameStateManager.mars = GameStateManager.FindByTemplate<TISpaceBodyState>("Mars", false);
			}
			return GameStateManager.mars;
		}

		// Token: 0x060029FC RID: 10748 RVA: 0x000E3BE3 File Offset: 0x000E1DE3
		public static TISpaceBodyState Ceres()
		{
			if (GameStateManager.ceres == null)
			{
				GameStateManager.ceres = GameStateManager.FindByTemplate<TISpaceBodyState>("Ceres", false);
			}
			return GameStateManager.ceres;
		}

		// Token: 0x060029FD RID: 10749 RVA: 0x000E3C07 File Offset: 0x000E1E07
		public static TISpaceBodyState Jupiter()
		{
			if (GameStateManager.jupiter == null)
			{
				GameStateManager.jupiter = GameStateManager.FindByTemplate<TISpaceBodyState>("Jupiter", false);
			}
			return GameStateManager.jupiter;
		}

		// Token: 0x060029FE RID: 10750 RVA: 0x000E3C2B File Offset: 0x000E1E2B
		public static TISpaceBodyState Saturn()
		{
			if (GameStateManager.saturn == null)
			{
				GameStateManager.saturn = GameStateManager.FindByTemplate<TISpaceBodyState>("Saturn", false);
			}
			return GameStateManager.saturn;
		}

		// Token: 0x060029FF RID: 10751 RVA: 0x000E3C4F File Offset: 0x000E1E4F
		public static TISpaceBodyState Uranus()
		{
			if (GameStateManager.uranus == null)
			{
				GameStateManager.uranus = GameStateManager.FindByTemplate<TISpaceBodyState>("Uranus", false);
			}
			return GameStateManager.uranus;
		}

		// Token: 0x06002A00 RID: 10752 RVA: 0x000E3C73 File Offset: 0x000E1E73
		public static TISpaceBodyState Neptune()
		{
			if (GameStateManager.neptune == null)
			{
				GameStateManager.neptune = GameStateManager.FindByTemplate<TISpaceBodyState>("Neptune", false);
			}
			return GameStateManager.neptune;
		}

		// Token: 0x06002A01 RID: 10753 RVA: 0x000E3C98 File Offset: 0x000E1E98
		public static List<TISpaceBodyState> Planets()
		{
			if (GameStateManager.planets == null)
			{
				GameStateManager.planets = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.objectType == SpaceObjectType.Planet
					select x).ToList<TISpaceBodyState>();
			}
			return GameStateManager.planets;
		}

		// Token: 0x06002A02 RID: 10754 RVA: 0x000E3CE8 File Offset: 0x000E1EE8
		public static List<TILagrangePointState> SunOrbitingLangragePoints()
		{
			if (GameStateManager.sunOrbitingLagrangePoints == null)
			{
				GameStateManager.sunOrbitingLagrangePoints = (from x in GameStateManager.AllLagrangePoints()
					where x.barycenter.isSun
					select x).ToList<TILagrangePointState>();
			}
			return GameStateManager.sunOrbitingLagrangePoints;
		}

		// Token: 0x06002A03 RID: 10755 RVA: 0x000E3D34 File Offset: 0x000E1F34
		public static List<TISpaceBodyState> InnerSystemAsteroids(bool includeSatellites)
		{
			if (GameStateManager.innerSystemAsteroids == null)
			{
				GameStateManager.innerSystemAsteroids = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.innerSystemAsteroid(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.innerSystemAsteroids.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.innerSystemAsteroids;
		}

		// Token: 0x06002A04 RID: 10756 RVA: 0x000E3DB4 File Offset: 0x000E1FB4
		public static List<TISpaceBodyState> InnerAsteroidBelt(bool includeSatellites)
		{
			if (GameStateManager.innerAsteroidBelt == null)
			{
				GameStateManager.innerAsteroidBelt = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.innerMainBeltAsteroid(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.innerAsteroidBelt.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.innerAsteroidBelt;
		}

		// Token: 0x06002A05 RID: 10757 RVA: 0x000E3E34 File Offset: 0x000E2034
		public static List<TISpaceBodyState> MidAsteroidBelt(bool includeSatellites)
		{
			if (GameStateManager.midAsteroidBelt == null)
			{
				GameStateManager.midAsteroidBelt = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.midMainBeltAsteroid(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.midAsteroidBelt.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.midAsteroidBelt;
		}

		// Token: 0x06002A06 RID: 10758 RVA: 0x000E3EB4 File Offset: 0x000E20B4
		public static List<TISpaceBodyState> OuterAsteroidBelt(bool includeSatellites)
		{
			if (GameStateManager.outerAsteroidBelt == null)
			{
				GameStateManager.outerAsteroidBelt = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.outerMainBeltAsteroid(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.outerAsteroidBelt.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.outerAsteroidBelt;
		}

		// Token: 0x06002A07 RID: 10759 RVA: 0x000E3F33 File Offset: 0x000E2133
		public static List<TISpaceBodyState> FullAsteroidBelt(bool includeSatellites)
		{
			List<TISpaceBodyState> list = new List<TISpaceBodyState>();
			list.AddRange(GameStateManager.InnerAsteroidBelt(includeSatellites));
			list.AddRange(GameStateManager.MidAsteroidBelt(includeSatellites));
			list.AddRange(GameStateManager.OuterAsteroidBelt(includeSatellites));
			return list;
		}

		// Token: 0x06002A08 RID: 10760 RVA: 0x000E3F60 File Offset: 0x000E2160
		public static List<TISpaceBodyState> Centaurs(bool includeSatellites)
		{
			if (GameStateManager.centaurs == null)
			{
				GameStateManager.centaurs = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.centaur(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.centaurs.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.centaurs;
		}

		// Token: 0x06002A09 RID: 10761 RVA: 0x000E3FE0 File Offset: 0x000E21E0
		public static List<TISpaceBodyState> KuiperBeltObjects(bool includeSatellites)
		{
			if (GameStateManager.kuiperBeltObjects == null)
			{
				GameStateManager.kuiperBeltObjects = (from x in GameStateManager.IterateByClass<TISpaceBodyState>(false)
					where x.kuiperBeltObject(true)
					select x).ToList<TISpaceBodyState>();
			}
			if (!includeSatellites)
			{
				return GameStateManager.kuiperBeltObjects.Where<TISpaceBodyState>((TISpaceBodyState x) => !x.isaMoon).ToList<TISpaceBodyState>();
			}
			return GameStateManager.kuiperBeltObjects;
		}

		// Token: 0x06002A0A RID: 10762 RVA: 0x000E4060 File Offset: 0x000E2260
		public static List<List<TISpaceBodyState>> ColonizableSpaceBodiesByRegion()
		{
			return new List<List<TISpaceBodyState>>
			{
				new List<TISpaceBodyState> { GameStateManager.Mercury() },
				new List<TISpaceBodyState> { GameStateManager.Venus() },
				new List<TISpaceBodyState> { GameStateManager.Luna() },
				GameStateManager.InnerSystemAsteroids(true),
				new List<TISpaceBodyState>
				{
					GameStateManager.Mars(),
					GameStateManager.Mars().naturalSatellites[0],
					GameStateManager.Mars().naturalSatellites[1]
				},
				GameStateManager.InnerAsteroidBelt(true),
				GameStateManager.MidAsteroidBelt(true),
				GameStateManager.OuterAsteroidBelt(true),
				new List<TISpaceBodyState> { GameStateManager.Jupiter() },
				GameStateManager.Jupiter().AllNaturalSatellites.ToList<TISpaceBodyState>(),
				GameStateManager.Centaurs(true),
				new List<TISpaceBodyState> { GameStateManager.Saturn() },
				GameStateManager.Saturn().AllNaturalSatellites.ToList<TISpaceBodyState>(),
				new List<TISpaceBodyState> { GameStateManager.Uranus() },
				GameStateManager.Uranus().AllNaturalSatellites.ToList<TISpaceBodyState>(),
				new List<TISpaceBodyState> { GameStateManager.Neptune() },
				GameStateManager.Neptune().AllNaturalSatellites.ToList<TISpaceBodyState>(),
				GameStateManager.KuiperBeltObjects(true)
			};
		}

		// Token: 0x06002A0B RID: 10763 RVA: 0x000E41EC File Offset: 0x000E23EC
		public static List<TIOrbitState> LEOStates()
		{
			if (GameStateManager.lowEarthOrbitStates == null)
			{
				GameStateManager.lowEarthOrbitStates = (from x in GameStateManager.IterateByClass<TIOrbitState>(false)
					where x.barycenter.isEarth && x.interfaceOrbit
					select x).ToList<TIOrbitState>();
			}
			return GameStateManager.lowEarthOrbitStates;
		}

		// Token: 0x06002A0C RID: 10764 RVA: 0x000E423C File Offset: 0x000E243C
		public static List<TIOrbitState> NEOStates()
		{
			if (GameStateManager.nearEarthOrbitStates == null)
			{
				GameStateManager.nearEarthOrbitStates = GameStateManager.IterateByClass<TIOrbitState>(false).Where<TIOrbitState>(delegate(TIOrbitState x)
				{
					if (!x.barycenter.isEarth)
					{
						TINaturalSpaceObjectState barycenter = x.barycenter.barycenter;
						return barycenter != null && barycenter.isEarth;
					}
					return true;
				}).ToList<TIOrbitState>();
			}
			return GameStateManager.nearEarthOrbitStates;
		}

		// Token: 0x06002A0D RID: 10765 RVA: 0x000E4289 File Offset: 0x000E2489
		public static TISpaceBodyState Sol()
		{
			if (GameStateManager.solState == null)
			{
				GameStateManager.solState = GameStateManager.FindByTemplate<TISpaceBodyState>("Sol", false);
			}
			return GameStateManager.solState;
		}

		// Token: 0x06002A0E RID: 10766 RVA: 0x000E42AD File Offset: 0x000E24AD
		public static TIRegionState[] AllRegions()
		{
			if (GameStateManager.regions == null)
			{
				GameStateManager.regions = GameStateManager.GetAllGameStates<TIRegionState>(false);
			}
			return GameStateManager.regions;
		}

		// Token: 0x06002A0F RID: 10767 RVA: 0x000E42C8 File Offset: 0x000E24C8
		public static List<TIRegionState> SupraRegionMembers(SupraRegion supraRegion)
		{
			if (GameStateManager.supraRegionMembers == null)
			{
				GameStateManager.supraRegionMembers = new Dictionary<SupraRegion, List<TIRegionState>>();
				foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
				{
					SupraRegion supraRegion2 = tiregionState.mapRegionTemplate.supraRegion;
					if (!GameStateManager.supraRegionMembers.ContainsKey(supraRegion2))
					{
						GameStateManager.supraRegionMembers.Add(supraRegion2, new List<TIRegionState>());
					}
					GameStateManager.supraRegionMembers[supraRegion2].Add(tiregionState);
				}
			}
			return GameStateManager.supraRegionMembers[supraRegion];
		}

		// Token: 0x06002A10 RID: 10768 RVA: 0x000E4343 File Offset: 0x000E2543
		public static TINationState[] AllNations()
		{
			if (GameStateManager.nations == null)
			{
				GameStateManager.nations = GameStateManager.GetAllGameStates<TINationState>(false);
			}
			return GameStateManager.nations;
		}

		// Token: 0x06002A11 RID: 10769 RVA: 0x000E435C File Offset: 0x000E255C
		public static Dictionary<string, TIRegionState> RegionLookup()
		{
			if (GameStateManager.regionLookup == null)
			{
				GameStateManager.regionLookup = new Dictionary<string, TIRegionState>();
				foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
				{
					GameStateManager.regionLookup[tiregionState.template.dataName] = tiregionState;
				}
			}
			return GameStateManager.regionLookup;
		}

		// Token: 0x06002A12 RID: 10770 RVA: 0x000E43B0 File Offset: 0x000E25B0
		public static TIRegionState MapRegionLookup(string mapRegion)
		{
			if (mapRegion != null)
			{
				if (GameStateManager.mapRegionLookup == null)
				{
					GameStateManager.mapRegionLookup = new Dictionary<string, TIRegionState>();
					foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
					{
						GameStateManager.mapRegionLookup[tiregionState.mapRegionTemplateName] = tiregionState;
					}
				}
				if (GameStateManager.mapRegionLookup.ContainsKey(mapRegion))
				{
					return GameStateManager.mapRegionLookup[mapRegion];
				}
			}
			return null;
		}

		// Token: 0x06002A13 RID: 10771 RVA: 0x000E4414 File Offset: 0x000E2614
		public static Dictionary<string, TINationState> NationLookup()
		{
			if (GameStateManager.nationLookup == null)
			{
				GameStateManager.nationLookup = new Dictionary<string, TINationState>();
				foreach (TINationState tinationState in GameStateManager.AllNations())
				{
					GameStateManager.nationLookup[tinationState.template.dataName] = tinationState;
				}
			}
			return GameStateManager.nationLookup;
		}

		// Token: 0x06002A14 RID: 10772 RVA: 0x000E4465 File Offset: 0x000E2665
		public static IEnumerable<TINationState> AllExtantNations()
		{
			return from x in GameStateManager.AllNations()
				where x.extant
				select x;
		}

		// Token: 0x06002A15 RID: 10773 RVA: 0x000E4490 File Offset: 0x000E2690
		public static IEnumerable<TINationState> AllExtantHumanNations()
		{
			return from x in GameStateManager.AllNations()
				where x.extant && !x.alienNation
				select x;
		}

		// Token: 0x06002A16 RID: 10774 RVA: 0x000E44BB File Offset: 0x000E26BB
		public static IEnumerable<TINationState> AllNonExtantHumanNations()
		{
			return from x in GameStateManager.AllNations()
				where !x.extant && !x.alienNation
				select x;
		}

		// Token: 0x06002A17 RID: 10775 RVA: 0x000E44E6 File Offset: 0x000E26E6
		public static IEnumerable<TINationState> AllHumanNations()
		{
			return from x in GameStateManager.AllNations()
				where !x.alienNation
				select x;
		}

		// Token: 0x06002A18 RID: 10776 RVA: 0x000E4511 File Offset: 0x000E2711
		public static TIFactionState[] AllFactions()
		{
			if (GameStateManager.factions == null)
			{
				GameStateManager.factions = GameStateManager.GetAllGameStates<TIFactionState>(false);
			}
			return GameStateManager.factions;
		}

		// Token: 0x06002A19 RID: 10777 RVA: 0x000E452A File Offset: 0x000E272A
		public static TIFactionIdeologyTemplate UndecidedIdeology()
		{
			if (GameStateManager.undecidedIdeology == null)
			{
				GameStateManager.undecidedIdeology = TemplateManager.IterateByClass<TIFactionIdeologyTemplate>(true).Single<TIFactionIdeologyTemplate>((TIFactionIdeologyTemplate x) => x.undecided);
			}
			return GameStateManager.undecidedIdeology;
		}

		// Token: 0x06002A1A RID: 10778 RVA: 0x000E4568 File Offset: 0x000E2768
		public static List<TIFactionIdeologyTemplate> ActiveIdeologies()
		{
			if (GameStateManager.activeIdeologies == null)
			{
				GameStateManager.activeIdeologies = (from x in GameStateManager.AllFactions()
					select x.ideology).ToList<TIFactionIdeologyTemplate>();
				GameStateManager.activeIdeologies.Add(GameStateManager.UndecidedIdeology());
				GameStateManager.activeIdeologies = GameStateManager.activeIdeologies.OrderBy<TIFactionIdeologyTemplate, int>((TIFactionIdeologyTemplate x) => x.sortOrder).ToList<TIFactionIdeologyTemplate>();
			}
			return GameStateManager.activeIdeologies;
		}

		// Token: 0x06002A1B RID: 10779 RVA: 0x000E45F8 File Offset: 0x000E27F8
		public static List<TIFactionIdeologyTemplate> ActiveHumanIdeologies()
		{
			if (GameStateManager.activeHumanIdeologies == null)
			{
				GameStateManager.activeHumanIdeologies = (from x in GameStateManager.AllHumanFactions()
					select x.ideology).ToList<TIFactionIdeologyTemplate>();
				GameStateManager.activeHumanIdeologies.Add(GameStateManager.UndecidedIdeology());
				GameStateManager.activeHumanIdeologies = GameStateManager.activeHumanIdeologies.OrderBy<TIFactionIdeologyTemplate, int>((TIFactionIdeologyTemplate x) => x.sortOrder).ToList<TIFactionIdeologyTemplate>();
			}
			return GameStateManager.activeHumanIdeologies;
		}

		// Token: 0x06002A1C RID: 10780 RVA: 0x000E4688 File Offset: 0x000E2888
		public static TIFactionState[] AllHumanFactions()
		{
			if (GameStateManager.humanFactions == null)
			{
				GameStateManager.humanFactions = (from x in GameStateManager.AllFactions()
					where !x.IsAlienFaction
					select x).ToArray<TIFactionState>();
			}
			return GameStateManager.humanFactions;
		}

		// Token: 0x06002A1D RID: 10781 RVA: 0x000E46D4 File Offset: 0x000E28D4
		public static TISpaceBodyState[] AllSpaceBodies()
		{
			if (GameStateManager.spaceBodies == null)
			{
				GameStateManager.spaceBodies = GameStateManager.GetAllGameStates<TISpaceBodyState>(false);
			}
			return GameStateManager.spaceBodies;
		}

		// Token: 0x06002A1E RID: 10782 RVA: 0x000E46ED File Offset: 0x000E28ED
		public static TILagrangePointState[] AllLagrangePoints()
		{
			if (GameStateManager.lagrangePoints == null)
			{
				GameStateManager.lagrangePoints = GameStateManager.GetAllGameStates<TILagrangePointState>(false);
			}
			return GameStateManager.lagrangePoints;
		}

		// Token: 0x06002A1F RID: 10783 RVA: 0x000E4706 File Offset: 0x000E2906
		public static TINaturalSpaceObjectState[] AllSpaceBodiesAndLPoints()
		{
			if (GameStateManager.naturalSpaceObjects == null)
			{
				List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>(GameStateManager.AllSpaceBodies());
				list.Remove(GameStateManager.Sol());
				list.AddRange(GameStateManager.AllLagrangePoints());
				GameStateManager.naturalSpaceObjects = list.ToArray();
			}
			return GameStateManager.naturalSpaceObjects;
		}

		// Token: 0x06002A20 RID: 10784 RVA: 0x000E473F File Offset: 0x000E293F
		public static TIOrbitState[] AllOrbits()
		{
			if (GameStateManager.orbits == null)
			{
				GameStateManager.orbits = GameStateManager.GetAllGameStates<TIOrbitState>(false);
			}
			return GameStateManager.orbits;
		}

		// Token: 0x06002A21 RID: 10785 RVA: 0x000E4758 File Offset: 0x000E2958
		public static TIRegionAlienEntityState[] AllAlienEntities()
		{
			if (GameStateManager.regionAlienEntities == null)
			{
				List<TIRegionAlienEntityState> list = new List<TIRegionAlienEntityState>();
				foreach (TIRegionAlienEntityState tiregionAlienEntityState in GameStateManager.IterateByClass<TIRegionAlienEntityState>(true))
				{
					list.Add(tiregionAlienEntityState);
				}
				GameStateManager.regionAlienEntities = list.ToArray();
			}
			return GameStateManager.regionAlienEntities;
		}

		// Token: 0x06002A22 RID: 10786 RVA: 0x000E47C4 File Offset: 0x000E29C4
		public static TINationState AlienNation()
		{
			if (GameStateManager.alienNation == null)
			{
				GameStateManager.alienNation = GameStateManager.FindByTemplate<TINationState>(TemplateManager.global.alienNationDataName, false);
			}
			return GameStateManager.alienNation;
		}

		// Token: 0x06002A23 RID: 10787 RVA: 0x000E47F0 File Offset: 0x000E29F0
		public static TIFactionState AlienFaction()
		{
			if (GameStateManager.alienFaction == null)
			{
				TIFactionState[] array = GameStateManager.AllFactions();
				TIFactionState tifactionState;
				if (array == null)
				{
					tifactionState = null;
				}
				else
				{
					tifactionState = array.FirstOrDefault<TIFactionState>((TIFactionState x) => x != null && x.IsAlienFaction);
				}
				GameStateManager.alienFaction = tifactionState;
				if (GameStateManager.alienFaction == null)
				{
					Log.Error("No Alien Faction Found.", Array.Empty<object>());
				}
			}
			return GameStateManager.alienFaction;
		}

		// Token: 0x06002A24 RID: 10788 RVA: 0x000E4860 File Offset: 0x000E2A60
		public static TIFactionState AlienProxy()
		{
			if (GameStateManager.alienProxyFaction == null)
			{
				GameStateManager.alienProxyFaction = (from x in GameStateManager.AllHumanFactions()
					where x.ideology.willProxy > 0
					select x).MinBy<TIFactionState, int>((TIFactionState y) => y.ideology.willProxy);
				if (GameStateManager.alienProxyFaction == null)
				{
					Log.Error("No Alien Proxy Faction Selected.", Array.Empty<object>());
				}
				else
				{
					Log.Info("Set Alien Proxy Faction as " + GameStateManager.alienProxyFaction.displayName, Array.Empty<object>());
				}
			}
			return GameStateManager.alienProxyFaction;
		}

		// Token: 0x06002A25 RID: 10789 RVA: 0x000E4910 File Offset: 0x000E2B10
		public static TIFactionState AlienAppeaser()
		{
			if (GameStateManager.alienAppeaserFaction == null)
			{
				GameStateManager.alienAppeaserFaction = (from x in GameStateManager.AllHumanFactions()
					where x.ideology.willAppease > 0
					select x).MinBy<TIFactionState, int>((TIFactionState y) => y.ideology.willAppease);
				if (GameStateManager.alienAppeaserFaction == null)
				{
					Log.Error("No Alien Appeaser Faction Selected.", Array.Empty<object>());
				}
				else
				{
					Log.Info("Set Alien Appeaser Faction as " + GameStateManager.alienAppeaserFaction.displayName, Array.Empty<object>());
				}
			}
			return GameStateManager.alienAppeaserFaction;
		}

		// Token: 0x06002A26 RID: 10790 RVA: 0x000E49C0 File Offset: 0x000E2BC0
		public static List<TIMissionState> AllActiveMissions()
		{
			return (from x in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.activeCouncilors)
				select x.activeMission into x
				where x != null
				select x).ToList<TIMissionState>();
		}

		// Token: 0x06002A27 RID: 10791 RVA: 0x000E4A43 File Offset: 0x000E2C43
		public static List<TIControlPoint> AllActiveControlPoints()
		{
			return GameStateManager.AllExtantNations().SelectMany<TINationState, TIControlPoint>((TINationState x) => x.controlPoints).ToList<TIControlPoint>();
		}

		// Token: 0x06002A28 RID: 10792 RVA: 0x000E4A73 File Offset: 0x000E2C73
		public static TIGlobalResearchState GlobalResearch()
		{
			if (GameStateManager.globalResearch == null)
			{
				GameStateManager.globalResearch = GameStateManager.FindGameState<TIGlobalResearchState>();
			}
			return GameStateManager.globalResearch;
		}

		// Token: 0x06002A29 RID: 10793 RVA: 0x000E4A91 File Offset: 0x000E2C91
		public static TIGlobalValuesState GlobalValues()
		{
			if (GameStateManager.globalValues == null)
			{
				GameStateManager.globalValues = GameStateManager.FindGameState<TIGlobalValuesState>();
			}
			return GameStateManager.globalValues;
		}

		// Token: 0x06002A2A RID: 10794 RVA: 0x000E4AAF File Offset: 0x000E2CAF
		public static TINotificationQueueState NotificationQueue()
		{
			if (GameStateManager.notificationQueue == null)
			{
				GameStateManager.notificationQueue = GameStateManager.FindGameState<TINotificationQueueState>();
			}
			return GameStateManager.notificationQueue;
		}

		// Token: 0x06002A2B RID: 10795 RVA: 0x000E4ACD File Offset: 0x000E2CCD
		public static TIPromptQueueState PromptQueue()
		{
			if (GameStateManager.promptQueue == null)
			{
				GameStateManager.promptQueue = GameStateManager.FindGameState<TIPromptQueueState>();
			}
			return GameStateManager.promptQueue;
		}

		// Token: 0x06002A2C RID: 10796 RVA: 0x000E4AEB File Offset: 0x000E2CEB
		public static TIEffectsState Effects()
		{
			if (GameStateManager.effects == null)
			{
				GameStateManager.effects = GameStateManager.FindGameState<TIEffectsState>();
			}
			return GameStateManager.effects;
		}

		// Token: 0x06002A2D RID: 10797 RVA: 0x000E4B09 File Offset: 0x000E2D09
		public static TITimeState Time()
		{
			if (GameStateManager.time == null)
			{
				GameStateManager.time = GameStateManager.FindGameState<TITimeState>();
			}
			return GameStateManager.time;
		}

		// Token: 0x06002A2E RID: 10798 RVA: 0x000E4B27 File Offset: 0x000E2D27
		public static TIMissionPhaseState MissionPhase()
		{
			if (GameStateManager.missionPhase == null)
			{
				GameStateManager.missionPhase = GameStateManager.FindGameState<TIMissionPhaseState>();
			}
			return GameStateManager.missionPhase;
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x000E4B45 File Offset: 0x000E2D45
		public static TIMetadataState MetaData()
		{
			if (GameStateManager.metaData == null)
			{
				GameStateManager.metaData = GameStateManager.FindGameState<TIMetadataState>();
			}
			return GameStateManager.metaData;
		}

		// Token: 0x06002A30 RID: 10800 RVA: 0x000E4B63 File Offset: 0x000E2D63
		public static IEnumerable<T> IterateByClass<T>(bool allowChild = false) where T : TIGameState
		{
			Type type = typeof(T);
			Dictionary<GameStateID, TIGameState> dictionary;
			if (allowChild)
			{
				foreach (Type type2 in GameStateManager.gamestates.Keys)
				{
					if (type.IsAssignableFrom(type2))
					{
						dictionary = GameStateManager.gamestates[type2];
						foreach (TIGameState tigameState in dictionary.Values)
						{
							yield return tigameState as T;
						}
						Dictionary<GameStateID, TIGameState>.ValueCollection.Enumerator enumerator2 = default(Dictionary<GameStateID, TIGameState>.ValueCollection.Enumerator);
					}
				}
				Dictionary<Type, Dictionary<GameStateID, TIGameState>>.KeyCollection.Enumerator enumerator = default(Dictionary<Type, Dictionary<GameStateID, TIGameState>>.KeyCollection.Enumerator);
			}
			else if (GameStateManager.gamestates.TryGetValue(typeof(T), out dictionary))
			{
				foreach (TIGameState tigameState2 in dictionary.Values)
				{
					yield return tigameState2 as T;
				}
				Dictionary<GameStateID, TIGameState>.ValueCollection.Enumerator enumerator2 = default(Dictionary<GameStateID, TIGameState>.ValueCollection.Enumerator);
			}
			yield break;
			yield break;
		}

		// Token: 0x06002A31 RID: 10801 RVA: 0x000E4B74 File Offset: 0x000E2D74
		public static int GetCount<T>(bool allowChild = true) where T : TIGameState
		{
			int num = 0;
			Dictionary<GameStateID, TIGameState> dictionary;
			if (allowChild)
			{
				Type typeFromHandle = typeof(T);
				using (Dictionary<Type, Dictionary<GameStateID, TIGameState>>.Enumerator enumerator = GameStateManager.gamestates.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<Type, Dictionary<GameStateID, TIGameState>> keyValuePair = enumerator.Current;
						if (typeFromHandle.IsAssignableFrom(keyValuePair.Key))
						{
							dictionary = GameStateManager.gamestates[keyValuePair.Key];
							num += dictionary.Count;
						}
					}
					return num;
				}
			}
			if (GameStateManager.gamestates.TryGetValue(typeof(T), out dictionary))
			{
				num = dictionary.Count;
			}
			return num;
		}

		// Token: 0x06002A32 RID: 10802 RVA: 0x000E4C1C File Offset: 0x000E2E1C
		public static T[] GetAllGameStates<T>(bool allowChild = true) where T : TIGameState
		{
			int num = 0;
			T[] array = new T[GameStateManager.GetCount<T>(allowChild)];
			foreach (T t in GameStateManager.IterateByClass<T>(allowChild))
			{
				array[num++] = t;
			}
			return array;
		}

		// Token: 0x06002A33 RID: 10803 RVA: 0x000E4C80 File Offset: 0x000E2E80
		public static bool RemoveGameState<T>(GameStateID ID, bool allowChild = false)
		{
			Dictionary<GameStateID, TIGameState> dictionary;
			if (GameStateManager.gamestates.TryGetValue(typeof(T), out dictionary) && dictionary.ContainsKey(ID))
			{
				dictionary[ID].exists = false;
				dictionary.Remove(ID);
				GameStateManager.gamestates[typeof(T)] = dictionary;
				return true;
			}
			if (allowChild)
			{
				Type typeFromHandle = typeof(T);
				foreach (KeyValuePair<Type, Dictionary<GameStateID, TIGameState>> keyValuePair in GameStateManager.gamestates)
				{
					if (typeFromHandle.IsAssignableFrom(keyValuePair.Key))
					{
						dictionary = GameStateManager.gamestates[keyValuePair.Key];
						if (dictionary.ContainsKey(ID))
						{
							dictionary[ID].exists = false;
							dictionary.Remove(ID);
							GameStateManager.gamestates[typeFromHandle] = dictionary;
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06002A34 RID: 10804 RVA: 0x000E4D80 File Offset: 0x000E2F80
		public static T CreateNewGameState<T>() where T : TIGameState
		{
			return GameStateManager.CreateNewGameState(typeof(T)) as T;
		}

		// Token: 0x06002A35 RID: 10805 RVA: 0x000E4D9C File Offset: 0x000E2F9C
		private static TIGameState CreateNewGameState(Type T)
		{
			GameStateID gameStateID = (GameStateManager.currentID = GameStateID.op_Increment(GameStateManager.currentID));
			TIGameState tigameState = Activator.CreateInstance(T) as TIGameState;
			if (tigameState != null)
			{
				tigameState.ID = gameStateID;
				if (tigameState.Initialize())
				{
					GameStateManager.AddGameState(tigameState, T, false);
					tigameState.exists = true;
					return tigameState;
				}
			}
			return null;
		}

		// Token: 0x06002A36 RID: 10806 RVA: 0x000E4DF0 File Offset: 0x000E2FF0
		private static void AddGameState(TIGameState newGameState, Type type, bool replaceDuplicate = false)
		{
			Dictionary<GameStateID, TIGameState> dictionary;
			if (!GameStateManager.gamestates.TryGetValue(type, out dictionary))
			{
				GameStateManager.gamestates[type] = new Dictionary<GameStateID, TIGameState> { { newGameState.ID, newGameState } };
				return;
			}
			if (dictionary.ContainsKey(newGameState.ID) && !replaceDuplicate)
			{
				return;
			}
			dictionary.Add(newGameState.ID, newGameState);
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x000E4E4C File Offset: 0x000E304C
		public static T FindGameState<T>() where T : TIGameState
		{
			Dictionary<GameStateID, TIGameState> dictionary;
			if (GameStateManager.gamestates.TryGetValue(typeof(T), out dictionary))
			{
				int count = dictionary.Count;
				if (count == 0)
				{
					return default(T);
				}
				if (count != 1)
				{
					Debug.LogError(string.Format("More than one GameState of type {0}", typeof(T)));
				}
				using (Dictionary<GameStateID, TIGameState>.ValueCollection.Enumerator enumerator = dictionary.Values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						return enumerator.Current as T;
					}
				}
			}
			return default(T);
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x000E4EFC File Offset: 0x000E30FC
		public static T FindGameState<T>(GameStateID ID, bool allowChild = false) where T : TIGameState
		{
			Dictionary<GameStateID, TIGameState> dictionary;
			TIGameState tigameState;
			if (GameStateManager.gamestates.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(ID, out tigameState))
			{
				return tigameState as T;
			}
			if (allowChild)
			{
				Type typeFromHandle = typeof(T);
				foreach (KeyValuePair<Type, Dictionary<GameStateID, TIGameState>> keyValuePair in GameStateManager.gamestates)
				{
					if (typeFromHandle.IsAssignableFrom(keyValuePair.Key))
					{
						dictionary = GameStateManager.gamestates[keyValuePair.Key];
						if (dictionary.TryGetValue(ID, out tigameState))
						{
							return tigameState as T;
						}
					}
				}
			}
			return default(T);
		}

		// Token: 0x06002A39 RID: 10809 RVA: 0x000E4FD0 File Offset: 0x000E31D0
		public static IEnumerable<T> FindGameStates<T>(IEnumerable<GameStateID> IDs, bool allowChild = false) where T : TIGameState
		{
			foreach (GameStateID gameStateID in IDs)
			{
				yield return GameStateManager.FindGameState<T>(gameStateID, allowChild);
			}
			IEnumerator<GameStateID> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06002A3A RID: 10810 RVA: 0x000E4FE8 File Offset: 0x000E31E8
		public static TIGameState FindGameState(GameStateID ID)
		{
			foreach (Type type in GameStateManager.gamestates.Keys)
			{
				foreach (GameStateID gameStateID in GameStateManager.gamestates[type].Keys)
				{
					if (ID == gameStateID)
					{
						return GameStateManager.gamestates[type][gameStateID];
					}
				}
			}
			return null;
		}

		// Token: 0x06002A3B RID: 10811 RVA: 0x000E50A0 File Offset: 0x000E32A0
		public static Type FindType(GameStateID ID)
		{
			foreach (Type type in GameStateManager.gamestates.Keys)
			{
				foreach (GameStateID gameStateID in GameStateManager.gamestates[type].Keys)
				{
					if (ID == gameStateID)
					{
						return type;
					}
				}
			}
			return null;
		}

		// Token: 0x06002A3C RID: 10812 RVA: 0x000E5148 File Offset: 0x000E3348
		public static T FindByTemplate<T>(string template, bool allowChild = false) where T : TIGameState
		{
			if (string.IsNullOrEmpty(template))
			{
				return default(T);
			}
			Type typeFromHandle = typeof(T);
			if (!GameStateManager.templateCache.ContainsKey(typeFromHandle))
			{
				GameStateManager.templateCache[typeFromHandle] = new Dictionary<string, TIGameState>();
			}
			if (!GameStateManager.templateCache[typeFromHandle].ContainsKey(template))
			{
				TIGameState tigameState = (from gs in GameStateManager.IterateByClass<T>(allowChild)
					where gs.templateName == template
					select gs).FirstOrDefault<T>();
				if (!(tigameState != null))
				{
					return default(T);
				}
				GameStateManager.templateCache[typeFromHandle][template] = tigameState;
			}
			return GameStateManager.templateCache[typeFromHandle][template] as T;
		}

		// Token: 0x06002A3D RID: 10813 RVA: 0x000E5227 File Offset: 0x000E3427
		public static IEnumerable<T> FindByTemplates<T>(IEnumerable<string> templates, bool allowChild = false) where T : TIGameState
		{
			foreach (string text in templates)
			{
				yield return GameStateManager.FindByTemplate<T>(text, allowChild);
			}
			IEnumerator<string> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x06002A3E RID: 10814 RVA: 0x000E5240 File Offset: 0x000E3440
		public static bool SaveAllGameStates(string filepath, bool doNotOpenSaveMenu = false)
		{
			bool flag;
			try
			{
				if (!GameControl.control.skirmishMode)
				{
					GameStateManager.MetaData().SetValues();
					SaveStructure saveStructure = new SaveStructure
					{
						currentID = GameStateManager.currentID,
						gamestates = GameStateManager.gamestates
					};
					if (!TIPlayerProfileManager.compressSaves)
					{
						StreamWriter streamWriter = new StreamWriter(filepath);
						fsJsonPrinter.PrettyJson(StringSerializationAPI.Serialize(typeof(SaveStructure), saveStructure), streamWriter);
						streamWriter.Close();
					}
					else
					{
						using (FileStream fileStream = File.Create(filepath))
						{
							using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
							{
								using (StreamWriter streamWriter2 = new StreamWriter(gzipStream, Encoding.UTF8))
								{
									fsJsonPrinter.PrettyJson(StringSerializationAPI.Serialize(typeof(SaveStructure), saveStructure), streamWriter2);
								}
							}
						}
					}
					GameControl.eventManager.TriggerEvent(new SaveFilesChangedEvent(), null, Array.Empty<object>());
				}
				flag = true;
			}
			catch (Exception ex)
			{
				Log.Error("Failed to save file to path " + filepath + ex.Message, Array.Empty<object>());
				StringBuilder stringBuilder = new StringBuilder(ex.Message);
				if (ex.Message.Contains("Win32 IO returned 112"))
				{
					stringBuilder.AppendLine(Loc.T("UI.Options.SaveFailLowDiskSpace"));
				}
				if (!doNotOpenSaveMenu)
				{
					SaveMenuController.Singleton.DisplaySavingFailedDialog(stringBuilder.ToString());
				}
				flag = false;
			}
			return flag;
		}

		// Token: 0x06002A3F RID: 10815 RVA: 0x000E53C4 File Offset: 0x000E35C4
		public static bool LoadAllGameStates(string filepath)
		{
			CoroutineDummy.Singleton.StopAll();
			SaveStructure saveStructure = SaveStructure.Load(filepath);
			if (saveStructure == null || saveStructure.gamestates == null)
			{
				Debug.LogError("Failed to load save, invalid data or incompatible game version");
				GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
				GameControl.control.StartCoroutine(GameControl.PassErrorToStartScreen(Loc.T("UI.StartScreen.FailedToLoadSave"), Loc.T("UI.StartScreen.FailedToLoadSaveDescription")));
				return false;
			}
			GameStateManager.currentID = saveStructure.currentID;
			GameStateManager.gamestates = saveStructure.gamestates;
			return true;
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x000E5444 File Offset: 0x000E3644
		public static bool IsValid()
		{
			return GameStateManager.gamestates != null && GameStateManager.gamestates.Count > 0;
		}

		// Token: 0x04002055 RID: 8277
		private static GameStateID currentID = new GameStateID(0);

		// Token: 0x04002056 RID: 8278
		private static Dictionary<Type, Dictionary<GameStateID, TIGameState>> gamestates = new Dictionary<Type, Dictionary<GameStateID, TIGameState>>();

		// Token: 0x04002057 RID: 8279
		private static readonly Dictionary<Type, Dictionary<string, TIGameState>> templateCache = new Dictionary<Type, Dictionary<string, TIGameState>>();

		// Token: 0x04002058 RID: 8280
		private static TINationState[] nations;

		// Token: 0x04002059 RID: 8281
		private static TIRegionState[] regions;

		// Token: 0x0400205A RID: 8282
		private static TIFactionState[] factions;

		// Token: 0x0400205B RID: 8283
		private static TIFactionState[] humanFactions;

		// Token: 0x0400205C RID: 8284
		private static TISpaceBodyState[] spaceBodies;

		// Token: 0x0400205D RID: 8285
		private static TILagrangePointState[] lagrangePoints;

		// Token: 0x0400205E RID: 8286
		private static TINaturalSpaceObjectState[] naturalSpaceObjects;

		// Token: 0x0400205F RID: 8287
		private static TIOrbitState[] orbits;

		// Token: 0x04002060 RID: 8288
		private static TIRegionAlienEntityState[] regionAlienEntities;

		// Token: 0x04002061 RID: 8289
		private static Dictionary<string, TIRegionState> regionLookup;

		// Token: 0x04002062 RID: 8290
		private static Dictionary<string, TINationState> nationLookup;

		// Token: 0x04002063 RID: 8291
		private static Dictionary<string, TIRegionState> mapRegionLookup;

		// Token: 0x04002064 RID: 8292
		private static Dictionary<SupraRegion, List<TIRegionState>> supraRegionMembers;

		// Token: 0x04002065 RID: 8293
		private static TIGlobalResearchState globalResearch;

		// Token: 0x04002066 RID: 8294
		private static TIGlobalValuesState globalValues;

		// Token: 0x04002067 RID: 8295
		private static TINotificationQueueState notificationQueue;

		// Token: 0x04002068 RID: 8296
		private static TIPromptQueueState promptQueue;

		// Token: 0x04002069 RID: 8297
		private static TITimeState time;

		// Token: 0x0400206A RID: 8298
		private static TIEffectsState effects;

		// Token: 0x0400206B RID: 8299
		private static TIMissionPhaseState missionPhase;

		// Token: 0x0400206C RID: 8300
		private static TIMetadataState metaData;

		// Token: 0x0400206D RID: 8301
		private static TINationState alienNation;

		// Token: 0x0400206E RID: 8302
		private static TISpaceBodyState solState;

		// Token: 0x0400206F RID: 8303
		private static TISpaceBodyState mercury;

		// Token: 0x04002070 RID: 8304
		private static TISpaceBodyState venus;

		// Token: 0x04002071 RID: 8305
		private static TISpaceBodyState earth;

		// Token: 0x04002072 RID: 8306
		private static TISpaceBodyState luna;

		// Token: 0x04002073 RID: 8307
		private static TISpaceBodyState mars;

		// Token: 0x04002074 RID: 8308
		private static TISpaceBodyState ceres;

		// Token: 0x04002075 RID: 8309
		private static TISpaceBodyState jupiter;

		// Token: 0x04002076 RID: 8310
		private static TISpaceBodyState saturn;

		// Token: 0x04002077 RID: 8311
		private static TISpaceBodyState uranus;

		// Token: 0x04002078 RID: 8312
		private static TISpaceBodyState neptune;

		// Token: 0x04002079 RID: 8313
		private static List<TISpaceBodyState> planets;

		// Token: 0x0400207A RID: 8314
		private static List<TILagrangePointState> sunOrbitingLagrangePoints;

		// Token: 0x0400207B RID: 8315
		private static List<TISpaceBodyState> innerSystemAsteroids;

		// Token: 0x0400207C RID: 8316
		private static List<TISpaceBodyState> innerAsteroidBelt;

		// Token: 0x0400207D RID: 8317
		private static List<TISpaceBodyState> midAsteroidBelt;

		// Token: 0x0400207E RID: 8318
		private static List<TISpaceBodyState> outerAsteroidBelt;

		// Token: 0x0400207F RID: 8319
		private static List<TISpaceBodyState> centaurs;

		// Token: 0x04002080 RID: 8320
		private static List<TISpaceBodyState> kuiperBeltObjects;

		// Token: 0x04002081 RID: 8321
		private static List<TIOrbitState> lowEarthOrbitStates;

		// Token: 0x04002082 RID: 8322
		private static List<TIOrbitState> nearEarthOrbitStates;

		// Token: 0x04002083 RID: 8323
		private static List<TIFactionIdeologyTemplate> activeIdeologies;

		// Token: 0x04002084 RID: 8324
		private static List<TIFactionIdeologyTemplate> activeHumanIdeologies;

		// Token: 0x04002085 RID: 8325
		private static TIFactionIdeologyTemplate undecidedIdeology;

		// Token: 0x04002086 RID: 8326
		private static TIFactionState alienFaction;

		// Token: 0x04002087 RID: 8327
		private static TIFactionState alienProxyFaction;

		// Token: 0x04002088 RID: 8328
		private static TIFactionState alienAppeaserFaction;
	}
}
