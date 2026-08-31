using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A5 RID: 1957
	public class TIHabState : TISpaceAssetState, OfficerCarrierState
	{
		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x06003FB2 RID: 16306 RVA: 0x00199F82 File Offset: 0x00198182
		// (set) Token: 0x06003FB3 RID: 16307 RVA: 0x00199F8A File Offset: 0x0019818A
		public HabType habType { get; private set; }

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x06003FB4 RID: 16308 RVA: 0x00199F93 File Offset: 0x00198193
		// (set) Token: 0x06003FB5 RID: 16309 RVA: 0x00199F9B File Offset: 0x0019819B
		public int tier { get; private set; }

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x06003FB6 RID: 16310 RVA: 0x00199FA4 File Offset: 0x001981A4
		// (set) Token: 0x06003FB7 RID: 16311 RVA: 0x00199FAC File Offset: 0x001981AC
		public List<TICouncilorState> advisingCouncilors { get; private set; }

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x06003FB8 RID: 16312 RVA: 0x00199FB5 File Offset: 0x001981B5
		// (set) Token: 0x06003FB9 RID: 16313 RVA: 0x00199FBD File Offset: 0x001981BD
		public List<TISpaceFleetState> dockedFleets { get; private set; }

		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x06003FBA RID: 16314 RVA: 0x00199FC6 File Offset: 0x001981C6
		// (set) Token: 0x06003FBB RID: 16315 RVA: 0x00199FCE File Offset: 0x001981CE
		public TIHabState.RingStruct ringStruct { get; private set; }

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x06003FBC RID: 16316 RVA: 0x00199FD7 File Offset: 0x001981D7
		// (set) Token: 0x06003FBD RID: 16317 RVA: 0x00199FDF File Offset: 0x001981DF
		public TIHabState.BaseConnectionStruct connStruct { get; private set; }

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x06003FBE RID: 16318 RVA: 0x00199FE8 File Offset: 0x001981E8
		// (set) Token: 0x06003FBF RID: 16319 RVA: 0x00199FF0 File Offset: 0x001981F0
		public bool underAssault { get; private set; }

		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x06003FC0 RID: 16320 RVA: 0x00199FF9 File Offset: 0x001981F9
		// (set) Token: 0x06003FC1 RID: 16321 RVA: 0x0019A001 File Offset: 0x00198201
		public TIDateTime coreDefendExpiration { get; private set; }

		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x06003FC2 RID: 16322 RVA: 0x0019A00A File Offset: 0x0019820A
		// (set) Token: 0x06003FC3 RID: 16323 RVA: 0x0019A012 File Offset: 0x00198212
		[SerializeField]
		public bool createdFromTemplate { get; private set; }

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x06003FC4 RID: 16324 RVA: 0x0019A01B File Offset: 0x0019821B
		// (set) Token: 0x06003FC5 RID: 16325 RVA: 0x0019A023 File Offset: 0x00198223
		[SerializeField]
		public bool inEarthLEO { get; private set; }

		// Token: 0x17000B9D RID: 2973
		// (get) Token: 0x06003FC6 RID: 16326 RVA: 0x0019A02C File Offset: 0x0019822C
		// (set) Token: 0x06003FC7 RID: 16327 RVA: 0x0019A034 File Offset: 0x00198234
		public bool staticHab { get; private set; }

		// Token: 0x17000B9E RID: 2974
		// (get) Token: 0x06003FC8 RID: 16328 RVA: 0x0019A040 File Offset: 0x00198240
		// (set) Token: 0x06003FC9 RID: 16329 RVA: 0x0019A095 File Offset: 0x00198295
		public HabSchematic HabSchematic
		{
			get
			{
				if (this.habSchematic != null)
				{
					this.HabSchematic = this.habSchematic;
					this.habSchematic = null;
				}
				if (this.habSchematic_SaveRepair == null && this.habSchematicTemplateName != null)
				{
					this.habSchematic_SaveRepair = TemplateManager.Find<TIHabSchematicTemplate>(this.habSchematicTemplateName, false).HabSchematic;
				}
				return this.habSchematic_SaveRepair;
			}
			set
			{
				this.habSchematic_SaveRepair = value;
				HabSchematic habSchematic = this.habSchematic_SaveRepair;
				string text;
				if (habSchematic == null)
				{
					text = null;
				}
				else
				{
					TIHabSchematicTemplate template = habSchematic.Template;
					text = ((template != null) ? template.dataName : null);
				}
				this.habSchematicTemplateName = text;
				this.HabSchematicAssignedDate = TITimeState.Now();
			}
		}

		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x06003FCA RID: 16330 RVA: 0x0019A0CD File Offset: 0x001982CD
		public IEnumerable<TISpaceFleetState> ConflictFleets
		{
			get
			{
				return this.conflictFleets;
			}
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x06003FCB RID: 16331 RVA: 0x0019A0D5 File Offset: 0x001982D5
		public override bool isHabState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x06003FCC RID: 16332 RVA: 0x0019A0D8 File Offset: 0x001982D8
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x06003FCD RID: 16333 RVA: 0x0019A0DB File Offset: 0x001982DB
		public override TIHabState ref_hab
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x06003FCE RID: 16334 RVA: 0x0019A0DE File Offset: 0x001982DE
		public override TIOrbitState ref_orbit
		{
			get
			{
				if (!this.IsStation)
				{
					return null;
				}
				return base.orbitState;
			}
		}

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x06003FCF RID: 16335 RVA: 0x0019A0F0 File Offset: 0x001982F0
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				if (!this.IsBase)
				{
					return base.orbitState.ref_spaceBody;
				}
				return this.habSite.parentBody;
			}
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x06003FD0 RID: 16336 RVA: 0x0019A111 File Offset: 0x00198311
		public override TIFactionState ref_faction
		{
			get
			{
				return base.faction;
			}
		}

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x06003FD1 RID: 16337 RVA: 0x0019A119 File Offset: 0x00198319
		public override TIHabSiteState ref_habSite
		{
			get
			{
				return this.habSite;
			}
		}

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x06003FD2 RID: 16338 RVA: 0x0019A121 File Offset: 0x00198321
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				if (!this.IsBase)
				{
					return this;
				}
				return this.habSite.parentBody;
			}
		}

		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x06003FD3 RID: 16339 RVA: 0x0019A138 File Offset: 0x00198338
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState2;
				try
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					if (!this.IsBase)
					{
						TIOrbitState orbitState = base.orbitState;
						tinaturalSpaceObjectState = ((orbitState != null) ? orbitState.barycenter : null) ?? null;
					}
					else
					{
						tinaturalSpaceObjectState = this.habSite.parentBody;
					}
					tinaturalSpaceObjectState2 = tinaturalSpaceObjectState;
				}
				catch
				{
					tinaturalSpaceObjectState2 = (this.IsBase ? this.habSite.parentBody : base.orbitState.barycenter);
				}
				return tinaturalSpaceObjectState2;
			}
		}

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x06003FD4 RID: 16340 RVA: 0x0019A1AC File Offset: 0x001983AC
		public override TILagrangePointState ref_lagrangePoint
		{
			get
			{
				if (!this.IsStation || !base.orbitState.barycenter.isLagrangePointState)
				{
					return null;
				}
				return base.orbitState.barycenter.ref_lagrangePoint;
			}
		}

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x06003FD5 RID: 16341 RVA: 0x0019A1DA File Offset: 0x001983DA
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06003FD6 RID: 16342 RVA: 0x0019A1DD File Offset: 0x001983DD
		public TIGameState GetTargetableState()
		{
			if (!this.IsBase)
			{
				return this;
			}
			return null;
		}

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x06003FD7 RID: 16343 RVA: 0x0019A1EA File Offset: 0x001983EA
		// (set) Token: 0x06003FD8 RID: 16344 RVA: 0x0019A1F2 File Offset: 0x001983F2
		public bool underBombardment { get; private set; }

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x06003FD9 RID: 16345 RVA: 0x0019A1FB File Offset: 0x001983FB
		public bool IsBase
		{
			get
			{
				return this.habType == HabType.Base;
			}
		}

		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x06003FDA RID: 16346 RVA: 0x0019A206 File Offset: 0x00198406
		public bool IsStation
		{
			get
			{
				return this.habType == HabType.Station;
			}
		}

		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x06003FDB RID: 16347 RVA: 0x0019A211 File Offset: 0x00198411
		public int numActiveSectors
		{
			get
			{
				return this.sectors.Count<TISectorState>((TISectorState x) => x.active);
			}
		}

		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x06003FDC RID: 16348 RVA: 0x0019A23D File Offset: 0x0019843D
		public List<TISectorState> activeSectors
		{
			get
			{
				return this.sectors.Where<TISectorState>((TISectorState x) => x.active).ToList<TISectorState>();
			}
		}

		// Token: 0x06003FDD RID: 16349 RVA: 0x0019A26E File Offset: 0x0019846E
		public List<TIHabModuleState> AllModuleStates()
		{
			return this.sectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FDE RID: 16350 RVA: 0x0019A2A0 File Offset: 0x001984A0
		public List<TIHabModuleState> AllModules()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where !x.empty
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x0019A300 File Offset: 0x00198500
		public List<TIHabModuleState> CompletedModules()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.completed
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x0019A360 File Offset: 0x00198560
		public List<TIHabModuleState> OkayModules()
		{
			if (this.okayModulesCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedOkayModules = (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
					where x.okay
					select x).ToList<TIHabModuleState>();
				this.okayModulesCachedFrame = TIFrameCounter.FrameCount;
			}
			return this.cachedOkayModules;
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x0019A3E4 File Offset: 0x001985E4
		public List<TIHabModuleState> FunctionalModules()
		{
			if (this.functionalModulesCachedFrame != TIFrameCounter.FrameCount)
			{
				this.cachedFunctionalModules = (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
					where x.functional
					select x).ToList<TIHabModuleState>();
				this.functionalModulesCachedFrame = TIFrameCounter.FrameCount;
			}
			return this.cachedFunctionalModules;
		}

		// Token: 0x06003FE2 RID: 16354 RVA: 0x0019A468 File Offset: 0x00198668
		public List<TIHabModuleState> ActiveModules()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.active
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE3 RID: 16355 RVA: 0x0019A4C8 File Offset: 0x001986C8
		public List<TIHabModuleState> UnpoweredModules()
		{
			return (from x in this.FunctionalModules()
				where !x.powered
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x0019A4F9 File Offset: 0x001986F9
		public List<TIHabModuleState> ActiveCombatModules()
		{
			return (from x in this.ActiveModules()
				where x.moduleTemplate.spaceCombatModule
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE5 RID: 16357 RVA: 0x0019A52A File Offset: 0x0019872A
		public List<TIHabModuleState> FunctionalCombatModules()
		{
			return (from x in this.FunctionalModules()
				where x.moduleTemplate.spaceCombatModule
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x0019A55C File Offset: 0x0019875C
		public List<TIHabModuleState> UnderConstructionModules()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.underConstruction
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE7 RID: 16359 RVA: 0x0019A5BC File Offset: 0x001987BC
		public List<TIHabModuleState> PresentModules()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.present
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE8 RID: 16360 RVA: 0x0019A61C File Offset: 0x0019881C
		public List<TIHabModuleState> AvailableSlots()
		{
			return (from x in this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules)
				where x.empty || x.destroyed
				select x).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FE9 RID: 16361 RVA: 0x0019A67C File Offset: 0x0019887C
		public List<TIHabModuleState> AllSlots()
		{
			return this.activeSectors.SelectMany<TISectorState, TIHabModuleState>((TISectorState x) => x.habModules).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x0019A6AD File Offset: 0x001988AD
		public List<TIHabModuleState> CompletedShipyards()
		{
			return base.faction.nShipyardQueues.Keys.Where<TIHabModuleState>((TIHabModuleState x) => x.hab == this).ToList<TIHabModuleState>();
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x0019A6D5 File Offset: 0x001988D5
		public void SetModulesDirty()
		{
			this.okayModulesCachedFrame = -1;
			this.functionalModulesCachedFrame = -1;
		}

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x06003FEC RID: 16364 RVA: 0x0019A6E5 File Offset: 0x001988E5
		public TIHabModuleState MineSlot
		{
			get
			{
				if (!this.IsBase)
				{
					return null;
				}
				return this.GetModule(0, 1);
			}
		}

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x06003FED RID: 16365 RVA: 0x0019A6F9 File Offset: 0x001988F9
		public TIHabModuleState CoreSlot
		{
			get
			{
				return this.GetModule(0, 0);
			}
		}

		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x06003FEE RID: 16366 RVA: 0x0019A703 File Offset: 0x00198903
		public int numCompletedModules
		{
			get
			{
				return this.sectors.Sum<TISectorState>((TISectorState x) => x.numFunctionalModules);
			}
		}

		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x06003FEF RID: 16367 RVA: 0x0019A72F File Offset: 0x0019892F
		public TISectorState coreSector
		{
			get
			{
				return this.sectors[0];
			}
		}

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x06003FF0 RID: 16368 RVA: 0x0019A73D File Offset: 0x0019893D
		public TIHabModuleState coreModule
		{
			get
			{
				return this.sectors[0].habModules[0];
			}
		}

		// Token: 0x17000BB5 RID: 2997
		// (get) Token: 0x06003FF1 RID: 16369 RVA: 0x0019A756 File Offset: 0x00198956
		public TIFactionState coreFaction
		{
			get
			{
				return this.sectors[0].faction;
			}
		}

		// Token: 0x06003FF2 RID: 16370 RVA: 0x0019A769 File Offset: 0x00198969
		public bool CanSellResources(TIFactionState faction)
		{
			return this.IsStation && base.orbitState.isEarthLEO && this.anyCoreCompleted && this.ref_factions.Contains(faction);
		}

		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06003FF3 RID: 16371 RVA: 0x0019A796 File Offset: 0x00198996
		public new TIHabTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIHabTemplate>();
			}
		}

		// Token: 0x06003FF4 RID: 16372 RVA: 0x0019A79E File Offset: 0x0019899E
		public override bool IsAlien()
		{
			return this.coreModule.IsAlien();
		}

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06003FF5 RID: 16373 RVA: 0x0019A7AB File Offset: 0x001989AB
		public override SpaceObjectType objectType
		{
			get
			{
				return SpaceObjectType.Hab;
			}
		}

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06003FF6 RID: 16374 RVA: 0x0019A7B0 File Offset: 0x001989B0
		public override string iconResource
		{
			get
			{
				if (!this.IsBase)
				{
					return this.sectors[0].faction.template.stationIcon;
				}
				return this.sectors[0].faction.template.baseIcon;
			}
		}

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06003FF7 RID: 16375 RVA: 0x0019A7FC File Offset: 0x001989FC
		public override string modelResource
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000BBA RID: 3002
		// (get) Token: 0x06003FF8 RID: 16376 RVA: 0x0019A7FF File Offset: 0x001989FF
		public override double meanRadius_km
		{
			get
			{
				return this.meanRadius_m / 1000.0;
			}
		}

		// Token: 0x17000BBB RID: 3003
		// (get) Token: 0x06003FF9 RID: 16377 RVA: 0x0019A811 File Offset: 0x00198A11
		public override double meanRadius_m
		{
			get
			{
				return 525.0;
			}
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x06003FFA RID: 16378 RVA: 0x0019A81C File Offset: 0x00198A1C
		public override float modelScale
		{
			get
			{
				return 525f;
			}
		}

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x06003FFB RID: 16379 RVA: 0x0019A823 File Offset: 0x00198A23
		public override TISpaceGameState location
		{
			get
			{
				if (!this.IsBase)
				{
					return base.orbitState;
				}
				return this.habSite;
			}
		}

		// Token: 0x17000BBE RID: 3006
		// (get) Token: 0x06003FFC RID: 16380 RVA: 0x0019A83A File Offset: 0x00198A3A
		public int maxCouncilors
		{
			get
			{
				if (!this.anyCoreCompleted)
				{
					return 0;
				}
				return (2 + this.numCompletedModules) * ((this.tier - 1) * 20);
			}
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x0019A85C File Offset: 0x00198A5C
		public bool HasAnyFunctionalModules(bool skipCoreModule = false)
		{
			return this.sectors.Any<TISectorState>((TISectorState x) => x.HasAnyFunctionalModules(skipCoreModule));
		}

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06003FFE RID: 16382 RVA: 0x0019A88D File Offset: 0x00198A8D
		public bool irradiated
		{
			get
			{
				if (!this.IsStation)
				{
					return this.habSite.irradiated;
				}
				return base.orbitState.irradiated;
			}
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06003FFF RID: 16383 RVA: 0x0019A8AE File Offset: 0x00198AAE
		public float irradiatedMultiplier
		{
			get
			{
				return TIHabState.GetIrradiatedMultiplier(this.location);
			}
		}

		// Token: 0x06004000 RID: 16384 RVA: 0x0019A8BB File Offset: 0x00198ABB
		public static bool IsMineSlot(int sector, int slot, HabType habType)
		{
			return habType == HabType.Base && sector == 0 && slot == 1;
		}

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06004001 RID: 16385 RVA: 0x0019A8CA File Offset: 0x00198ACA
		public double localGravity_gs
		{
			get
			{
				if (!this.IsBase || !(this.habSite != null))
				{
					return 0.0;
				}
				return this.habSite.surfaceGravity_g;
			}
		}

		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06004002 RID: 16386 RVA: 0x0019A8F7 File Offset: 0x00198AF7
		public override double mass_kg
		{
			get
			{
				return (double)this.AllModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.Mass_tons(this.irradiatedMultiplier, this.ref_spaceBody, this.ref_naturalSpaceObject, base.faction) * 1000f);
			}
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x0019A911 File Offset: 0x00198B11
		public TIHabModuleState GetModule(int sector, int moduleNum)
		{
			return this.sectors[sector].habModules[moduleNum];
		}

		// Token: 0x17000BC3 RID: 3011
		// (get) Token: 0x06004004 RID: 16388 RVA: 0x0019A92A File Offset: 0x00198B2A
		public bool decommissioning
		{
			get
			{
				return this.coreModule.decommissioning;
			}
		}

		// Token: 0x17000BC4 RID: 3012
		// (get) Token: 0x06004005 RID: 16389 RVA: 0x0019A938 File Offset: 0x00198B38
		public bool HasMine
		{
			get
			{
				return !this.sectors[0].habModules[1].empty && this.sectors[0].habModules[1].moduleTemplate.mine;
			}
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06004006 RID: 16390 RVA: 0x0019A986 File Offset: 0x00198B86
		public bool HasMineFunctional
		{
			get
			{
				return this.HasMine && this.sectors[0].habModules[1].functional;
			}
		}

		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x06004007 RID: 16391 RVA: 0x0019A9AE File Offset: 0x00198BAE
		public bool HasActiveMine
		{
			get
			{
				return this.HasMine && this.sectors[0].habModules[1].active;
			}
		}

		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x06004008 RID: 16392 RVA: 0x0019A9D8 File Offset: 0x00198BD8
		public bool HasInactiveButPowerableMine
		{
			get
			{
				return this.HasMine && this.mine.okay && !this.mine.underConstruction && !this.mine.active && this.NetPower(false, false) >= -this.mine.moduleTemplate.ProspectivePower(this);
			}
		}

		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x06004009 RID: 16393 RVA: 0x0019AA35 File Offset: 0x00198C35
		public TIHabModuleState mine
		{
			get
			{
				if (!this.IsBase)
				{
					return null;
				}
				return this.sectors[0].habModules[1];
			}
		}

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x0600400A RID: 16394 RVA: 0x0019AA58 File Offset: 0x00198C58
		public int crew
		{
			get
			{
				return this.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.crew);
			}
		}

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x0600400B RID: 16395 RVA: 0x0019AA84 File Offset: 0x00198C84
		public override TISpaceObjectState GetSunOrbitingRelatedObject
		{
			get
			{
				if (!this.IsBase)
				{
					return base.GetSunOrbitingRelatedObject;
				}
				return this.habSite.parentBody.GetSunOrbitingRelatedObject;
			}
		}

		// Token: 0x0600400C RID: 16396 RVA: 0x0019AAA8 File Offset: 0x00198CA8
		public override float CombatRange_km()
		{
			float num = 0f;
			foreach (TIHabModuleState tihabModuleState in this.ActiveCombatModules())
			{
				float spaceCombatRange = tihabModuleState.GetSpaceCombatRange();
				if (spaceCombatRange > num)
				{
					num = spaceCombatRange;
				}
			}
			return num;
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x0019AB08 File Offset: 0x00198D08
		public int MissionControlCost(bool allowNegativeReturn, TIFactionState faction = null)
		{
			if (faction == null && this.coreFaction != null)
			{
				faction = this.coreFaction;
			}
			if (!allowNegativeReturn)
			{
				return Mathf.Max(-1 * (int)this.GetAnnualNetResourceIncome(faction, FactionResource.MissionControl), 0);
			}
			return -1 * (int)this.GetAnnualNetResourceIncome(faction, FactionResource.MissionControl);
		}

		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x0600400E RID: 16398 RVA: 0x0019AB54 File Offset: 0x00198D54
		public int maxTier
		{
			get
			{
				return this.ref_naturalSpaceObject.maxHabTier;
			}
		}

		// Token: 0x0600400F RID: 16399 RVA: 0x0019AB61 File Offset: 0x00198D61
		public static int maxModules(int tier)
		{
			switch (tier)
			{
			default:
				return 5;
			case 2:
				return 13;
			case 3:
				return 21;
			}
		}

		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x06004010 RID: 16400 RVA: 0x0019AB80 File Offset: 0x00198D80
		public string LocationName
		{
			get
			{
				if (this.IsStation)
				{
					return base.orbitState.displayName;
				}
				if (!this.IsBase)
				{
					return string.Empty;
				}
				if (this.habSite.parentBody.isaMoon)
				{
					return Loc.T("UI.Habs.ExtendedBaseLocationName", new object[]
					{
						this.habSite.displayName,
						this.habSite.parentBody.displayName,
						this.habSite.parentBody.barycenter.displayName
					});
				}
				return Loc.T("UI.Habs.BaseLocationName", new object[]
				{
					this.habSite.displayName,
					this.habSite.parentBody.displayName,
					this.habSite.parentBody.barycenter.displayName
				});
			}
		}

		// Token: 0x06004011 RID: 16401 RVA: 0x0019AC58 File Offset: 0x00198E58
		public static bool IsModuleAllowedForHab(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, IEnumerable<TIHabModuleTemplate> existingModules = null, bool skipOnePerHabUpgradeCheckForDowngrade = false)
		{
			HabType habType = ((location.ref_habSite != null) ? HabType.Base : HabType.Station);
			int num;
			if (location.isHabState && existingModules == null)
			{
				existingModules = from x in location.ref_hab.OkayModules()
					select x.moduleTemplate;
				num = location.ref_hab.tier;
			}
			else
			{
				int? num2;
				if (existingModules == null)
				{
					num2 = null;
				}
				else
				{
					TIHabModuleTemplate tihabModuleTemplate = existingModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
					num2 = ((tihabModuleTemplate != null) ? new int?(tihabModuleTemplate.tier) : null);
				}
				int? num3 = num2;
				num = num3.GetValueOrDefault();
			}
			int num4 = num;
			if (num4 == 0)
			{
				if (habType == HabType.Base)
				{
					num4 = faction.MaxBaseTier;
				}
				else
				{
					num4 = faction.MaxStationTier;
				}
			}
			if (moduleTemplate.onePerHab)
			{
				TIHabModuleTemplate tihabModuleTemplate2 = existingModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.SharesUpgradePath(moduleTemplate));
				if (tihabModuleTemplate2 != null && (skipOnePerHabUpgradeCheckForDowngrade || tihabModuleTemplate2.tier >= moduleTemplate.tier))
				{
					return false;
				}
			}
			bool? flag;
			if (existingModules == null)
			{
				flag = null;
			}
			else
			{
				TIHabModuleTemplate tihabModuleTemplate3 = existingModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
				flag = ((tihabModuleTemplate3 != null) ? new bool?(tihabModuleTemplate3.automated) : null);
			}
			bool? flag2 = flag;
			bool valueOrDefault = flag2.GetValueOrDefault();
			return moduleTemplate.tier <= location.ref_naturalSpaceObject.maxHabTier && ((!moduleTemplate.coreModule && moduleTemplate.tier <= num4) || (moduleTemplate.coreModule && moduleTemplate.tier > num)) && moduleTemplate.IsForHabType(habType) && moduleTemplate.automated == valueOrDefault && moduleTemplate.FactionCanBuild(faction) && moduleTemplate.AllowedLocation(location, location.isHabState ? location.ref_hab : null);
		}

		// Token: 0x06004012 RID: 16402 RVA: 0x0019AE7B File Offset: 0x0019907B
		public bool IsModuleAllowedForThisHab(TIFactionState faction, TIHabModuleTemplate moduleTemplate, bool downGradingOnePerHabModule = false)
		{
			return TIHabState.IsModuleAllowedForHab(faction, this, moduleTemplate, null, downGradingOnePerHabModule);
		}

		// Token: 0x06004013 RID: 16403 RVA: 0x0019AE88 File Offset: 0x00199088
		public List<TIHabModuleTemplate> AllowedModules(TIFactionState faction)
		{
			if (!this.decommissioning && !this.underBombardment && !this.underAssault)
			{
				return TemplateManager.HabModuleTemplates.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => this.IsModuleAllowedForThisHab(faction, x, false)).ToList<TIHabModuleTemplate>();
			}
			return new List<TIHabModuleTemplate>();
		}

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x06004014 RID: 16404 RVA: 0x0019AEE4 File Offset: 0x001990E4
		public double altitude
		{
			get
			{
				if (this.IsStation)
				{
					double num = base.orbitState.semiMajorAxis_km;
					if (this.barycenter.isSpaceBodyState)
					{
						num -= this.barycenter.meanRadius_km;
					}
					return num;
				}
				return 0.0;
			}
		}

		// Token: 0x17000BCE RID: 3022
		// (get) Token: 0x06004015 RID: 16405 RVA: 0x0019AF2C File Offset: 0x0019912C
		public string description
		{
			get
			{
				if (this.IsStation)
				{
					switch (this.tier)
					{
					case 1:
						return Loc.T("UI.Habs.Tier1Station");
					case 2:
						return Loc.T("UI.Habs.Tier2Station");
					case 3:
						return Loc.T("UI.Habs.Tier3Station");
					case 4:
						return Loc.T("UI.Habs.Tier4Station");
					}
				}
				else
				{
					switch (this.tier)
					{
					case 1:
						return Loc.T("UI.Habs.Tier1Base");
					case 2:
						return Loc.T("UI.Habs.Tier2Base");
					case 3:
						return Loc.T("UI.Habs.Tier3Base");
					case 4:
						return Loc.T("UI.Habs.Tier4Base");
					}
				}
				return string.Empty;
			}
		}

		// Token: 0x06004016 RID: 16406 RVA: 0x0019AFE0 File Offset: 0x001991E0
		public bool ModuleFunctioning(TIHabModuleTemplate moduleTemplate, bool includeUpgradePrereqs = false)
		{
			foreach (TIHabModuleState tihabModuleState in this.FunctionalModules())
			{
				if (tihabModuleState.moduleTemplate == moduleTemplate || (includeUpgradePrereqs && tihabModuleState.moduleTemplate.UpgradesTo == moduleTemplate))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004017 RID: 16407 RVA: 0x0019B050 File Offset: 0x00199250
		public bool ModuleUpgradePrereqModuleAlreadyOnHab(TIHabModuleTemplate candidateUpgradeModuleTemplate)
		{
			if (candidateUpgradeModuleTemplate.UpgradesFrom == null)
			{
				return false;
			}
			using (List<TIHabModuleState>.Enumerator enumerator = this.CompletedModules().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.moduleTemplate == candidateUpgradeModuleTemplate.UpgradesFrom)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004018 RID: 16408 RVA: 0x0019B0BC File Offset: 0x001992BC
		public bool HasAnyActiveModuleInUpgradeChain(TIHabModuleTemplate moduleTemplate)
		{
			foreach (TIHabModuleState tihabModuleState in this.ActiveModules())
			{
				if (tihabModuleState.moduleTemplate == moduleTemplate)
				{
					return true;
				}
				switch (moduleTemplate.tier)
				{
				case 1:
				{
					if (tihabModuleState.moduleTemplate.UpgradesFrom == moduleTemplate)
					{
						return true;
					}
					TIHabModuleTemplate upgradesFrom = tihabModuleState.moduleTemplate.UpgradesFrom;
					if (((upgradesFrom != null) ? upgradesFrom.UpgradesFrom : null) == moduleTemplate)
					{
						return true;
					}
					break;
				}
				case 2:
					if (tihabModuleState.moduleTemplate.UpgradesFrom == moduleTemplate)
					{
						return true;
					}
					if (tihabModuleState.moduleTemplate.UpgradesTo == moduleTemplate)
					{
						return true;
					}
					break;
				case 3:
				{
					if (tihabModuleState.moduleTemplate.UpgradesTo == moduleTemplate)
					{
						return true;
					}
					TIHabModuleTemplate upgradesTo = tihabModuleState.moduleTemplate.UpgradesTo;
					if (((upgradesTo != null) ? upgradesTo.UpgradesTo : null) == moduleTemplate)
					{
						return true;
					}
					break;
				}
				}
			}
			return false;
		}

		// Token: 0x06004019 RID: 16409 RVA: 0x0019B1C8 File Offset: 0x001993C8
		public bool GetUpgradeModuleLocation(TIHabModuleTemplate candidateUpgradeModuleTemplate, out int sector, out int moduleSlot)
		{
			sector = -1;
			moduleSlot = -1;
			foreach (TIHabModuleState tihabModuleState in this.CompletedModules())
			{
				if (tihabModuleState.moduleTemplate == candidateUpgradeModuleTemplate.UpgradesFrom)
				{
					sector = tihabModuleState.sectorNum;
					moduleSlot = tihabModuleState.slot;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600401A RID: 16410 RVA: 0x0019B240 File Offset: 0x00199440
		public bool OnlyUpgradeAllowed(TIHabModuleTemplate moduleTemplate)
		{
			if (moduleTemplate.onePerHab)
			{
				foreach (TIHabModuleState tihabModuleState in this.AllModules())
				{
					if (tihabModuleState.moduleTemplate == moduleTemplate)
					{
						return true;
					}
					switch (moduleTemplate.tier)
					{
					case 1:
					{
						if (tihabModuleState.moduleTemplate.UpgradesFrom == moduleTemplate)
						{
							return true;
						}
						TIHabModuleTemplate upgradesFrom = tihabModuleState.moduleTemplate.UpgradesFrom;
						if (((upgradesFrom != null) ? upgradesFrom.UpgradesFrom : null) == moduleTemplate)
						{
							return true;
						}
						break;
					}
					case 2:
						if (tihabModuleState.moduleTemplate.UpgradesFrom == moduleTemplate)
						{
							return true;
						}
						if (tihabModuleState.moduleTemplate.UpgradesTo == moduleTemplate)
						{
							return true;
						}
						break;
					case 3:
					{
						if (tihabModuleState.moduleTemplate.UpgradesTo == moduleTemplate)
						{
							return true;
						}
						TIHabModuleTemplate upgradesTo = tihabModuleState.moduleTemplate.UpgradesTo;
						if (((upgradesTo != null) ? upgradesTo.UpgradesTo : null) == moduleTemplate)
						{
							return true;
						}
						break;
					}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600401B RID: 16411 RVA: 0x0019B358 File Offset: 0x00199558
		public List<TIHabModuleState> ModulesElgibleForUpgradeTo(TIHabModuleTemplate candidateUpgradeModuleTemplate)
		{
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			foreach (TIHabModuleState tihabModuleState in this.CompletedModules())
			{
				if (tihabModuleState.moduleTemplate == candidateUpgradeModuleTemplate.UpgradesFrom)
				{
					list.Add(tihabModuleState);
				}
			}
			return list;
		}

		// Token: 0x0600401C RID: 16412 RVA: 0x0019B3C0 File Offset: 0x001995C0
		public TIHabModuleState GetSlotForNewModule(TIHabModuleTemplate moduleTemplate, bool allowUpgrades = true, IEnumerable<TIHabModuleState> slots = null)
		{
			if (moduleTemplate.coreModule)
			{
				return this.CoreSlot;
			}
			if (moduleTemplate.mine)
			{
				return this.MineSlot;
			}
			if (slots == null)
			{
				slots = this.AllModules();
			}
			if (moduleTemplate.onePerHab)
			{
				TIHabModuleState tihabModuleState = slots.Where<TIHabModuleState>((TIHabModuleState x) => !x.empty).FirstOrDefault<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SharesUpgradePath(moduleTemplate));
				if (tihabModuleState != null)
				{
					return tihabModuleState;
				}
			}
			int num;
			int num2;
			if (allowUpgrades && this.GetUpgradeModuleLocation(moduleTemplate, out num, out num2))
			{
				return this.GetModule(num, num2);
			}
			return (from x in this.AvailableSlots()
				where x != this.CoreSlot && x != this.MineSlot
				select x).FirstOrDefault<TIHabModuleState>();
		}

		// Token: 0x0600401D RID: 16413 RVA: 0x0019B49D File Offset: 0x0019969D
		public override float SpaceCombatValue()
		{
			return this.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.SpaceCombatValue());
		}

		// Token: 0x0600401E RID: 16414 RVA: 0x0019B4C9 File Offset: 0x001996C9
		public float FleetTargetingBonus()
		{
			return this.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.FleetTargetingBonus());
		}

		// Token: 0x0600401F RID: 16415 RVA: 0x0019B4F5 File Offset: 0x001996F5
		public float FleetECMBonus()
		{
			return this.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.FleetECMBonus());
		}

		// Token: 0x06004020 RID: 16416 RVA: 0x0019B521 File Offset: 0x00199721
		public float AggregateDefensiveScore_Station()
		{
			return this.PerceivedAggregateDefensiveScore_Station(null);
		}

		// Token: 0x06004021 RID: 16417 RVA: 0x0019B52C File Offset: 0x0019972C
		public float PerceivedAggregateDefensiveScore_Station(TIFactionState enemyFaction)
		{
			float num = this.SpaceCombatValue();
			float num2 = this.SpaceCombatValueFromDefendingFleets();
			if (enemyFaction != null)
			{
				num2 *= enemyFaction.GetPerceivedEnemyFleetStrengthFactor(base.faction);
			}
			float num3 = Mathf.Max(num, num2);
			if (num3 <= 0f)
			{
				return 0f;
			}
			return num3 * Mathf.Pow((num + num2) / num3, 0.25f);
		}

		// Token: 0x06004022 RID: 16418 RVA: 0x0019B588 File Offset: 0x00199788
		public bool IsSafeToVisit(TISpaceFleetState fleet)
		{
			float num = AIEvaluators.GetRiskAdjustedThreatLevelAtLocation(fleet.faction, this.location, true) * 1.2f;
			float num2 = fleet.SpaceCombatValue();
			if (fleet.faction.permanentAlly(base.faction))
			{
				num2 += this.SpaceCombatValue();
			}
			if (num2 > num)
			{
				return true;
			}
			if (base.ref_system.isEarth)
			{
				return true;
			}
			if ((from x in base.ref_system.habsInSystem
				where fleet.faction.permanentAlly(x.faction)
				where x.AllowsResupply(fleet.faction, false, false)
				select x).Any<TIHabState>())
			{
				return true;
			}
			return (from x in base.ref_system.habsInSystem
				where x.IsStation
				where x.faction.IsActiveHumanFaction
				where !GameStateManager.AlienFaction().permanentAlly(x.faction)
				select x.faction).Distinct<TIFactionState>().ToList<TIFactionState>().Count >= 3;
		}

		// Token: 0x06004023 RID: 16419 RVA: 0x0019B6E8 File Offset: 0x001998E8
		public bool AllowsShipConstruction(TIFactionState faction = null, bool checkInactives = false, bool checkUnderConstruction = false)
		{
			if (faction == null || faction == this.coreFaction)
			{
				List<TIHabModuleState> list;
				if (checkUnderConstruction)
				{
					list = this.AllModules();
				}
				else
				{
					list = (checkInactives ? this.FunctionalModules() : this.ActiveModules());
				}
				using (List<TIHabModuleState>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.moduleTemplate.allowsShipConstruction)
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06004024 RID: 16420 RVA: 0x0019B778 File Offset: 0x00199978
		public int ResupplySpeedDivisor()
		{
			int num = 0;
			foreach (TIHabModuleState tihabModuleState in this.ActiveModules())
			{
				if (tihabModuleState.moduleTemplate.allowsResupply)
				{
					if (tihabModuleState.moduleTemplate.allowsShipConstruction)
					{
						num += tihabModuleState.moduleTemplate.tier;
					}
					else
					{
						num += tihabModuleState.moduleTemplate.tier * 2;
					}
				}
			}
			return Mathf.Max(num, 1);
		}

		// Token: 0x06004025 RID: 16421 RVA: 0x0019B808 File Offset: 0x00199A08
		public float DaysUntilCanStartResupply()
		{
			TIDateTime tidateTime = null;
			foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets)
			{
				foreach (OperationData operationData in tispaceFleetState.CurrentOperations())
				{
					if (operationData.operation is ResupplyOperation && (tidateTime == null || operationData.completionDate > tidateTime))
					{
						tidateTime = operationData.completionDate;
					}
				}
			}
			if (tidateTime == null)
			{
				return 0f;
			}
			return (float)tidateTime.DifferenceInDays(TITimeState.Now());
		}

		// Token: 0x06004026 RID: 16422 RVA: 0x0019B8D8 File Offset: 0x00199AD8
		public bool CanFullyRepairFleet(TISpaceFleetState fleet)
		{
			return fleet.faction == base.faction && fleet.ships.All<TISpaceShipState>((TISpaceShipState x) => this.CanFullyRepairShip(x));
		}

		// Token: 0x06004027 RID: 16423 RVA: 0x0019B906 File Offset: 0x00199B06
		public bool CanPartiallyRepairFleet(TISpaceFleetState fleet)
		{
			return fleet.faction == base.faction && fleet.ships.Any<TISpaceShipState>((TISpaceShipState x) => this.CanPartiallyRepairShip(x));
		}

		// Token: 0x06004028 RID: 16424 RVA: 0x0019B934 File Offset: 0x00199B34
		public bool CanPartiallyRepairShip(TISpaceShipState ship)
		{
			if (this.AllowsShipConstruction(ship.faction, false, false))
			{
				if (ship.DamagedSystems().Count <= 0)
				{
					if (!ship.armor.Values.Any<TISpaceShipState.ArmorData>((TISpaceShipState.ArmorData x) => x.damaged))
					{
						return ship.damagedParts.Any<DamagedShipPartData>((DamagedShipPartData x) => this.CanBuildAndRepairShipPart(x.module.moduleTemplate));
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x0019B9AB File Offset: 0x00199BAB
		private bool CanFullyRepairShip(TISpaceShipState ship)
		{
			return this.AllowsShipConstruction(ship.faction, false, false) && ship.damagedParts.All<DamagedShipPartData>((DamagedShipPartData x) => this.CanBuildAndRepairShipPart(x.module.moduleTemplate));
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x0019B9D8 File Offset: 0x00199BD8
		public bool CanBuildAndRepairShipPart(TIShipPartTemplate part)
		{
			if (part.isUtilityModule)
			{
				if (part.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenMarineModulePresent))
				{
					return this.ActiveSpecialAbilities(base.faction).Contains(HabModuleSpecialRule.RepairsMarineShipModules);
				}
				if (part.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenConstructionModulePresent))
				{
					return this.ActiveSpecialAbilities(base.faction).Contains(HabModuleSpecialRule.RepairsHabKitShipModules);
				}
			}
			return true;
		}

		// Token: 0x0600402B RID: 16427 RVA: 0x0019BA40 File Offset: 0x00199C40
		public void CompleteShipConstruction(TISpaceShipState newShip, TISpaceShipState refitFrom = null)
		{
			foreach (ModuleDataEntry moduleDataEntry in newShip.utilityModules)
			{
				if (!this.CanBuildAndRepairShipPart(moduleDataEntry.moduleTemplate))
				{
					if (refitFrom != null)
					{
						newShip.SetPartDamage(moduleDataEntry, refitFrom.GetPartDamage(moduleDataEntry), false);
					}
					else
					{
						newShip.SetPartDamage(moduleDataEntry, 1f, false);
					}
				}
			}
		}

		// Token: 0x0600402C RID: 16428 RVA: 0x0019BAC4 File Offset: 0x00199CC4
		public int RepairSpeedDivisor()
		{
			int num = 0;
			foreach (TIHabModuleState tihabModuleState in this.ActiveModules())
			{
				if (tihabModuleState.moduleTemplate.allowsShipConstruction)
				{
					num += tihabModuleState.moduleTemplate.tier;
				}
			}
			return Mathf.Max(num, 1);
		}

		// Token: 0x0600402D RID: 16429 RVA: 0x0019BB34 File Offset: 0x00199D34
		public float DaysUntilCanStartRepair()
		{
			TIDateTime tidateTime = null;
			foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets)
			{
				foreach (OperationData operationData in tispaceFleetState.CurrentOperations())
				{
					if (operationData.operation is RepairFleetOperation && (tidateTime == null || operationData.completionDate > tidateTime))
					{
						tidateTime = operationData.completionDate;
					}
				}
			}
			if (tidateTime == null)
			{
				return 0f;
			}
			return (float)tidateTime.DifferenceInDays(TITimeState.Now());
		}

		// Token: 0x0600402E RID: 16430 RVA: 0x0019BC04 File Offset: 0x00199E04
		public bool AllowsResupply(TIFactionState checkingFaction, bool allowHumanTheft, bool checkInactives = false)
		{
			if (checkingFaction == this.coreFaction)
			{
				using (List<TIHabModuleState>.Enumerator enumerator = (checkInactives ? this.FunctionalModules() : this.ActiveModules()).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.moduleTemplate.allowsResupply)
						{
							return true;
						}
					}
				}
				return false;
			}
			if (allowHumanTheft)
			{
			}
			return false;
		}

		// Token: 0x0600402F RID: 16431 RVA: 0x0019BC80 File Offset: 0x00199E80
		public void UpdateAllModuleConstructionTimes()
		{
			float moduleConstructionTimeModifier = this.GetModuleConstructionTimeModifier(false, null);
			foreach (TIHabModuleState tihabModuleState in this.UnderConstructionModules())
			{
				if (tihabModuleState.baseBuildDuration_days > 0f && moduleConstructionTimeModifier != tihabModuleState.appliedBuildConstructionBonus)
				{
					float num;
					float num2;
					if (this.gameTime.Now <= tihabModuleState.startBuildDate)
					{
						num = tihabModuleState.baseBuildDuration_days * tihabModuleState.appliedBuildConstructionBonus;
						num2 = tihabModuleState.baseBuildDuration_days * moduleConstructionTimeModifier;
					}
					else
					{
						num = (float)(tihabModuleState.completionDate - this.gameTime.Now).TotalDays;
						num2 = num / tihabModuleState.appliedBuildConstructionBonus * moduleConstructionTimeModifier;
					}
					tihabModuleState.ChangeFutureCompletionDate(num2 - num);
					tihabModuleState.appliedBuildConstructionBonus = moduleConstructionTimeModifier;
				}
			}
		}

		// Token: 0x06004030 RID: 16432 RVA: 0x0019BD64 File Offset: 0x00199F64
		public float GetModuleConstructionTimeModifier(bool checkInactives = false, TIHabModuleState excludeModule = null)
		{
			float num = 1f;
			IEnumerable<TIHabModuleState> enumerable = (checkInactives ? this.FunctionalModules() : this.ActiveModules()).Where<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.moduleConstructionSpeedModifier != 1f);
			List<TIHabModuleState> list;
			if (enumerable == null)
			{
				list = null;
			}
			else
			{
				list = enumerable.OrderBy<TIHabModuleState, float>((TIHabModuleState x) => x.moduleTemplate.moduleConstructionSpeedModifier).ToList<TIHabModuleState>();
			}
			float num2 = 1f;
			foreach (TIHabModuleState tihabModuleState in list)
			{
				if (tihabModuleState != excludeModule)
				{
					float moduleConstructionSpeedModifier = tihabModuleState.moduleTemplate.moduleConstructionSpeedModifier;
					if (moduleConstructionSpeedModifier > 0f)
					{
						if (moduleConstructionSpeedModifier < 1f)
						{
							float num3 = (1f - tihabModuleState.moduleTemplate.moduleConstructionSpeedModifier) / (num2 * num2);
							num *= 1f - num3;
							num2 += 1f;
						}
						else
						{
							num *= tihabModuleState.moduleTemplate.moduleConstructionSpeedModifier;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06004031 RID: 16433 RVA: 0x0019BE80 File Offset: 0x0019A080
		public bool DropTroops(TIFactionState faction)
		{
			if (faction == base.faction)
			{
				return this.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops));
			}
			return false;
		}

		// Token: 0x06004032 RID: 16434 RVA: 0x0019BEBC File Offset: 0x0019A0BC
		public List<HabModuleSpecialRule> ActiveSpecialAbilities(TIFactionState faction)
		{
			return this.ActiveModules().SelectMany<TIHabModuleState, HabModuleSpecialRule>((TIHabModuleState x) => x.moduleTemplate.SpecialRules).Distinct<HabModuleSpecialRule>()
				.ToList<HabModuleSpecialRule>();
		}

		// Token: 0x06004033 RID: 16435 RVA: 0x0019BEF2 File Offset: 0x0019A0F2
		public List<HabModuleSpecialRule> SpecialAbilities(TIFactionState faction)
		{
			return this.FunctionalModules().SelectMany<TIHabModuleState, HabModuleSpecialRule>((TIHabModuleState x) => x.moduleTemplate.SpecialRules).Distinct<HabModuleSpecialRule>()
				.ToList<HabModuleSpecialRule>();
		}

		// Token: 0x06004034 RID: 16436 RVA: 0x0019BF28 File Offset: 0x0019A128
		public List<TISpaceShipTemplate> ShipsBeingBuiltAtHab(TIFactionState faction)
		{
			List<TISpaceShipTemplate> list = new List<TISpaceShipTemplate>();
			foreach (TIHabModuleState tihabModuleState in faction.nShipyardQueues.Keys.Where<TIHabModuleState>((TIHabModuleState x) => x.hab == this).ToList<TIHabModuleState>())
			{
				List<ShipConstructionQueueItem> list2 = faction.nShipyardQueues[tihabModuleState];
				if (list2.Count > 0 && list2[0].costPaid)
				{
					list.Add(faction.nShipyardQueues[tihabModuleState][0].shipDesign);
				}
			}
			return list;
		}

		// Token: 0x06004035 RID: 16437 RVA: 0x0019BFD8 File Offset: 0x0019A1D8
		public IEnumerable<ShipConstructionQueueItem> AllShipConstructionQueueItems(TIFactionState faction)
		{
			return this.CompletedShipyards().SelectMany<TIHabModuleState, ShipConstructionQueueItem>(delegate(TIHabModuleState shipyard)
			{
				List<ShipConstructionQueueItem> list;
				if (faction.nShipyardQueues.TryGetValue(shipyard, out list))
				{
					return list;
				}
				return Enumerable.Empty<ShipConstructionQueueItem>();
			});
		}

		// Token: 0x06004036 RID: 16438 RVA: 0x0019C00C File Offset: 0x0019A20C
		public bool HasResourceIncomeForFaction(FactionResource resource, TIFactionState faction)
		{
			foreach (TISectorState tisectorState in this.sectors)
			{
				if (tisectorState.faction == faction && tisectorState.HasIncome(resource))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004037 RID: 16439 RVA: 0x0019C078 File Offset: 0x0019A278
		public bool AtLeastOneSectorHasIncome(FactionResource resource)
		{
			for (int i = 0; i < this.sectors.Count; i++)
			{
				if (this.sectors[i].HasIncome(resource))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004038 RID: 16440 RVA: 0x0019C0B4 File Offset: 0x0019A2B4
		public float GetNetTechBonusByFaction(TechCategory category, TIFactionState faction, bool includeInactives)
		{
			float num = 0f;
			if (this.coreFaction == faction)
			{
				foreach (TIHabModuleState tihabModuleState in (includeInactives ? this.FunctionalModules() : this.ActiveModules()))
				{
					num += tihabModuleState.moduleTemplate.GetTechBonusByCategory(category);
				}
			}
			return num;
		}

		// Token: 0x06004039 RID: 16441 RVA: 0x0019C130 File Offset: 0x0019A330
		public bool AtLeastOneCoreSectorHasTechBonus(TechCategory techCategory, bool includeInactives)
		{
			using (List<TIHabModuleState>.Enumerator enumerator = (includeInactives ? this.FunctionalModules() : this.ActiveModules()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.moduleTemplate.GetTechBonusByCategory(techCategory) > 0f)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600403A RID: 16442 RVA: 0x0019C1A0 File Offset: 0x0019A3A0
		public static float GetIrradiatedMultiplier(TISpaceGameState location)
		{
			TIOrbitState tiorbitState = location as TIOrbitState;
			if (tiorbitState != null)
			{
				return tiorbitState.irradiatedValue;
			}
			TIHabSiteState tihabSiteState = location as TIHabSiteState;
			if (tihabSiteState != null)
			{
				return tihabSiteState.parentBody.irradiatedMultiplier;
			}
			if (location.ref_spaceBody != null)
			{
				return location.ref_spaceBody.irradiatedMultiplier;
			}
			return 1f;
		}

		// Token: 0x0600403B RID: 16443 RVA: 0x0019C1F4 File Offset: 0x0019A3F4
		public override void InitWithTemplate(TIDataTemplate rawTemplate)
		{
			TIHabTemplate tihabTemplate = rawTemplate as TIHabTemplate;
			this.templateName = tihabTemplate.dataName;
			this.SetDisplayName(tihabTemplate.displayName);
			this.tier = tihabTemplate.tier;
			this.habType = tihabTemplate.habType;
			this.sectors = new List<TISectorState>();
			this.councilorsOnBoard = new List<TICouncilorState>();
			base.epoch_DateTime = new TIDateTime();
			this.dockedFleets = new List<TISpaceFleetState>();
			this.createdFromTemplate = true;
		}

		// Token: 0x0600403C RID: 16444 RVA: 0x0019C26C File Offset: 0x0019A46C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (this.gameTime == null)
			{
				this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			}
			if (!this.gameStateSubjectCreated && this.createdFromTemplate)
			{
				int num = 0;
				if (base.faction == null)
				{
					base.faction = GameStateManager.FindByTemplate<TIFactionState>(this.template.sectors[0].faction, false);
				}
				foreach (SectorTemplate sectorTemplate in this.template.sectors)
				{
					TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(sectorTemplate.faction, false);
					if (tifactionState == null && !string.IsNullOrEmpty(sectorTemplate.faction))
					{
						if (num == 0)
						{
							Dictionary<TIFactionState, int> numHabs = GameStateManager.AllHumanFactions().ToDictionary<TIFactionState, TIFactionState, int>((TIFactionState x) => x, (TIFactionState x) => x.habs.Count);
							int min = numHabs.Values.Min();
							base.faction = numHabs.Keys.Where<TIFactionState>((TIFactionState x) => numHabs[x] == min).SelectRandomItem<TIFactionState>();
							Log.Info(string.Concat(new string[]
							{
								"Config gives ",
								this.displayName,
								" to non-existent ",
								sectorTemplate.faction,
								". Giving to randomly selected NPC human faction ",
								base.faction.displayName,
								" instead."
							}), Array.Empty<object>());
						}
						tifactionState = base.faction;
					}
					else if (num == 0)
					{
						base.faction = tifactionState;
					}
					this.InitializeSector(tifactionState, num);
					if (sectorTemplate.habModuleNames != null)
					{
						int num2 = 0;
						foreach (string text in sectorTemplate.habModuleNames)
						{
							if (!string.IsNullOrEmpty(text))
							{
								this.sectors[num].habModules[num2].SetCompletedModule(text, true);
							}
							num2++;
						}
					}
					num++;
				}
				this.UpdateAllModuleConnectors();
			}
		}

		// Token: 0x0600403D RID: 16445 RVA: 0x0019C4A4 File Offset: 0x0019A6A4
		public override void PostGlobalGameStateCreateInit_2()
		{
			base.PostGlobalGameStateCreateInit_2();
			if (!this.gameStateSubjectCreated)
			{
				if (this.IsStation)
				{
					TIOrbitState tiorbitState = GameStateManager.FindByTemplate<TIOrbitState>(this.template.orbitTemplateName, false);
					if (tiorbitState != null)
					{
						if (this.template.meanAnomalyAtEpoch_Deg == null)
						{
							base.SetRandomizedOrbitFromState(tiorbitState, true);
						}
						else
						{
							base.AssumeOrbitFromState(tiorbitState, this.template.MeanAnomalyAtEpoch_Rad, TITimeState.Now());
						}
					}
					else if (!string.IsNullOrEmpty(this.template.orbitTemplateName))
					{
						Log.Error("Bad orbitState " + this.template.orbitTemplateName + " defined in template " + this.template.dataName, Array.Empty<object>());
					}
				}
				else if (this.IsBase)
				{
					this.habSite = GameStateManager.FindByTemplate<TIHabSiteState>(this.template.habSite, false);
					if (this.habSite != null)
					{
						this.habSite.hab = this;
						this.barycenter = this.habSite.parentBody;
					}
				}
			}
			else
			{
				if (this.dockedFleets != null)
				{
					List<TISpaceFleetState> list = new List<TISpaceFleetState>(this.dockedFleets);
					bool flag = false;
					using (List<TISpaceFleetState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceFleetState fleet = enumerator.Current;
							if (fleet.faction == null || fleet.ships == null || fleet.ships.Count == 0 || fleet.dockedLocation != this)
							{
								Log.Warn("Bad fleet " + fleet.ID.ToString() + " at " + this.displayName, Array.Empty<object>());
								this.RemoveDockedFleet(fleet);
							}
							if (list.Where<TISpaceFleetState>((TISpaceFleetState x) => x == fleet).Count<TISpaceFleetState>() > 1)
							{
								Log.Error(string.Concat(new string[] { "Fleet ", fleet.displayName, " is registered docked at ", this.displayName, " multiple times. If you see this message in a game started in 0.2.07+, please report it." }), Array.Empty<object>());
								flag = true;
							}
						}
					}
					if (flag)
					{
						this.dockedFleets = this.dockedFleets.Distinct<TISpaceFleetState>().ToList<TISpaceFleetState>();
					}
				}
				foreach (TIHabModuleState tihabModuleState in this.CompletedModules())
				{
					if (tihabModuleState.isCombatModule)
					{
						tihabModuleState.SetSpaceCombatWeapons(this.ref_faction);
					}
				}
			}
			this.InitializeIncomes();
		}

		// Token: 0x0600403E RID: 16446 RVA: 0x0019C7A4 File Offset: 0x0019A9A4
		public override void PostAllStartUpInit_5()
		{
			bool flag = !this.gameStateSubjectCreated;
			this.OnHabCreated();
			if (this.createdFromTemplate)
			{
				TIGlobalValuesState.GlobalValues.CheckGlobalMilestoneOnHabFounding(this, true);
			}
			if (base.faction.player.isAI || flag)
			{
				this.UpdatePowerManagement(true, null, base.faction.player.isAI);
			}
			else if (this.NetPower(false, false) < 0)
			{
				this.UpdatePowerManagement(false, null, base.faction.player.isAI);
			}
			this.UpdateCurrentAnnualNetResourceIncomes(false);
			if (this.dockedFleets.Any<TISpaceFleetState>() && this.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !this.dockedFleets.First<TISpaceFleetState>().faction.permanentAlly(x.faction)))
			{
				foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => !base.faction.permanentAlly(x.faction)).ToList<TISpaceFleetState>())
				{
					Log.Error(string.Concat(new string[] { "Fleet ", tispaceFleetState.displayName, " is docked at ", this.displayName, " alongside enemy fleets. If you see this message in a game started in 0.2.07+, please report it." }), Array.Empty<object>());
					tispaceFleetState.DepartFromDockingLocation();
				}
			}
			this.enemyFleetInLineOfSight = new Dictionary<TISpaceFleetState, bool>();
			bool underBombardment = this.underBombardment;
			this.underBombardment = this.underBombardment && GameStateManager.IterateByClass<TISpaceFleetState>(false).Any<TISpaceFleetState>((TISpaceFleetState x) => x.bombardmentTarget == this);
			if (underBombardment != this.underBombardment)
			{
				Log.Error("Hab " + this.displayName + " recorded as under bombardment, but no fleet is bombarding it.", Array.Empty<object>());
			}
		}

		// Token: 0x0600403F RID: 16447 RVA: 0x0019C954 File Offset: 0x0019AB54
		public override void PostVisualizerCreationInit_7()
		{
			GameControl.eventManager.TriggerEvent(new HabCreated(this), null, new object[] { this, base.faction, this.ref_naturalSpaceObject, this.ref_habSite }.Where<object>((object x) => x != null).ToArray<object>());
		}

		// Token: 0x06004040 RID: 16448 RVA: 0x0019C9C0 File Offset: 0x0019ABC0
		public override void PostEverythingSaveRepair_8()
		{
			if (this.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(base.faction)))
			{
				if (this.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.isCombatModule) && this.habType == HabType.Station)
				{
					this.dockedFleets.First<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(base.faction)).InitiateCombat(this.dockedFleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(base.faction)), this, false);
				}
			}
		}

		// Token: 0x06004041 RID: 16449 RVA: 0x0019CA50 File Offset: 0x0019AC50
		public void InitializeNewHab(TIFactionState faction, TIGameState exactLocation, TIGameState founder, int tierSetting, float deliveryTime_days, List<string> additionalModuleNames = null)
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.tier = Mathf.Abs(tierSetting);
			this.habType = (exactLocation.isOrbitState ? HabType.Station : HabType.Base);
			this.createdFromTemplate = false;
			this.sectors = new List<TISectorState>();
			if (this.habType == HabType.Base && TIGlobalConfig.globalConfig.useSiteNameWhenNamingBases)
			{
				this.SetDisplayName(Loc.T("UI.Habs.BaseSiteName", new object[] { exactLocation.ref_habSite.displayName }));
			}
			else
			{
				this.SetDisplayName(TISpaceAssetState.GetRandomAssetName(this, faction));
			}
			base.faction = faction;
			this.InitializeSector(faction, 0);
			if (this.IsStation)
			{
				this.InitializeSector((this.tier >= 3) ? faction : null, 1);
				this.InitializeSector((this.tier >= 2) ? faction : null, 2);
				this.InitializeSector((this.tier >= 3) ? faction : null, 3);
				this.InitializeSector((this.tier >= 2) ? faction : null, 4);
				base.epoch_DateTime = new TIDateTime();
				if (!this.IsAlien())
				{
					switch (tierSetting)
					{
					case -1:
						this.ConstructFoundingModule("AutomatedPlatformCore", 0, 0, deliveryTime_days);
						break;
					case 1:
						this.ConstructFoundingModule("PlatformCore", 0, 0, deliveryTime_days);
						break;
					case 2:
						this.ConstructFoundingModule("OrbitalCore", 0, 0, deliveryTime_days);
						break;
					case 3:
						this.ConstructFoundingModule("RingCore", 0, 0, deliveryTime_days);
						break;
					}
				}
				else
				{
					switch (tierSetting)
					{
					case 1:
						this.ConstructFoundingModule("AlienPlatformCore", 0, 0, deliveryTime_days);
						break;
					case 2:
						this.ConstructFoundingModule("AlienOrbitalCore", 0, 0, deliveryTime_days);
						break;
					case 3:
						this.ConstructFoundingModule("AlienRingCore", 0, 0, deliveryTime_days);
						break;
					}
				}
				TIOrbitState ref_orbit = exactLocation.ref_orbit;
				base.SetRandomizedOrbitFromState(ref_orbit, true);
				if (founder.isSpaceFleetState)
				{
					this._meanAnomalyAtEpoch_Rad = ref_orbit.TestAndCorrectAnomalyToAvoidOverlap(this, founder.ref_fleet.meanAnomalyAtEpoch_Rad, false, true);
				}
				else
				{
					float circumference_km = ref_orbit.circumference_km;
					float desiredDistance = circumference_km / (float)(3 * ref_orbit.template.stationCapacity);
					Func<TIHabState, bool> <>9__1;
					for (int i = 0; i < 36; i++)
					{
						IEnumerable<TIHabState> stationsInOrbit = ref_orbit.stationsInOrbit;
						Func<TIHabState, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (TIHabState x) => TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(x, this) < (double)desiredDistance);
						}
						if (!stationsInOrbit.Any<TIHabState>(func))
						{
							break;
						}
						this._meanAnomalyAtEpoch_Rad += 0.08726000040769577;
					}
				}
				new List<TIFactionGoalState>();
				this.ringStruct = default(TIHabState.RingStruct);
			}
			else if (this.IsBase)
			{
				this.InitializeSector((this.tier >= 2) ? faction : null, 1);
				this.InitializeSector((this.tier >= 2) ? faction : null, 2);
				this.InitializeSector((this.tier >= 3) ? faction : null, 3);
				this.InitializeSector((this.tier >= 3) ? faction : null, 4);
				if (!this.IsAlien())
				{
					switch (tierSetting)
					{
					case -1:
						this.ConstructFoundingModule("AutomatedOutpostCore", 0, 0, deliveryTime_days);
						break;
					case 1:
						this.ConstructFoundingModule("OutpostCore", 0, 0, deliveryTime_days);
						break;
					case 2:
						this.ConstructFoundingModule("SettlementCore", 0, 0, deliveryTime_days);
						break;
					case 3:
						this.ConstructFoundingModule("ColonyCore", 0, 0, deliveryTime_days);
						break;
					}
				}
				else
				{
					switch (tierSetting)
					{
					case 1:
						this.ConstructFoundingModule("AlienOutpostCore", 0, 0, deliveryTime_days);
						break;
					case 2:
						this.ConstructFoundingModule("AlienSettlementCore", 0, 0, deliveryTime_days);
						break;
					case 3:
						this.ConstructFoundingModule("AlienColonyCore", 0, 0, deliveryTime_days);
						break;
					}
				}
				this.connStruct = default(TIHabState.BaseConnectionStruct);
				this.habSite = exactLocation.ref_habSite;
				this.habSite.hab = this;
				this.barycenter = this.habSite.parentBody;
			}
			if (additionalModuleNames != null)
			{
				int num = (this.IsBase ? 2 : 1);
				int num2 = 0;
				bool flag = false;
				foreach (string text in additionalModuleNames)
				{
					this.ConstructFoundingModule(text, num2, num++, deliveryTime_days);
					if (TemplateManager.Find<TIHabModuleTemplate>(text, false).SpecialRules.Contains(HabModuleSpecialRule.StaticHab))
					{
						flag = true;
					}
					if (num > this.sectors[num2].slots - 1)
					{
						foreach (TISectorState tisectorState in this.sectors)
						{
							if (tisectorState.active)
							{
								if (tisectorState.habModules.All<TIHabModuleState>((TIHabModuleState x) => x.empty))
								{
									num2 = this.sectors.IndexOf(tisectorState);
									break;
								}
							}
						}
						num = 0;
					}
				}
				if (flag)
				{
					this.staticHab = true;
				}
			}
			this.InitializeIncomes();
			this.OnHabCreated();
			if (this.IsStation && founder.isSpaceFleetState)
			{
				founder.ref_fleet.Dock(this, false);
			}
			GameControl.eventManager.TriggerEvent(new HabCreated(this), null, new object[] { this, faction, this.ref_naturalSpaceObject, this.ref_habSite }.Where<object>((object x) => x != null).ToArray<object>());
			this.councilorsOnBoard = new List<TICouncilorState>();
			this.UpdatePowerManagement(true, null, faction.player.isAI);
			this.CreateVisualizer(null);
		}

		// Token: 0x06004042 RID: 16450 RVA: 0x0019D000 File Offset: 0x0019B200
		public override void CreateVisualizer(TIDataTemplate myTemplate)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.IsStation ? AssetCacheManager.stationPrefab : AssetCacheManager.basePrefab);
			gameObject.name = base.ID.ToString();
			gameObject.SetActive(false);
			if (this.IsStation)
			{
				HabComponent component = gameObject.GetComponent<HabComponent>();
				component.InitStation();
				HabModule3D habModule3D;
				component.modules.TryGetValue("S0_M0", out habModule3D);
				if (habModule3D != null)
				{
					habModule3D.HideModule();
				}
				base.controller = gameObject.GetComponent<SpaceObjectController>();
				base.controller.GetComponent<HabComponent>().Initialize(this);
				base.controller.Initialize(this);
				component.Update3DModel();
				GameControl.solarSystem.AddObject(gameObject, true);
				base.controller.modelLink = base.controller.gameObject.GetComponentOnChild<Transform>("Model").gameObject;
				base.controller.SetAmbientAudioClip();
				gameObject.GetComponentInChildren<HabModelController>().Initialize(this, true, base.controller);
				base.controller.modelLink.SetActive(false);
			}
			else
			{
				gameObject.GetComponent<HabComponent>().Initialize(this);
				GameControl.solarSystem.AddObject(gameObject, true);
				GameObject gameObject2 = this.habSite.parentBody.gameObjectLink;
				if (gameObject2 == null)
				{
					gameObject2 = GameObject.Find(this.habSite.parentBody.ID.ToString());
				}
				gameObject.transform.SetParent(gameObject2.transform);
				this.baseObject = gameObject;
			}
			gameObject.layer = LayerMask.NameToLayer("Solar System");
			gameObject.SetActive(true);
		}

		// Token: 0x06004043 RID: 16451 RVA: 0x0019D190 File Offset: 0x0019B390
		public void InitializeSector(TIFactionState faction, int sectorNum)
		{
			TISectorState tisectorState = GameStateManager.CreateNewGameState<TISectorState>();
			tisectorState.Initialize();
			tisectorState.SetFaction(faction);
			tisectorState.hab = this;
			tisectorState.slots = ((this.sectors.Count == 0) ? 5 : 4);
			tisectorState.habModules = new List<TIHabModuleState>();
			tisectorState.sectorNum = sectorNum;
			this.sectors.Add(tisectorState);
			tisectorState.SetDisplayName();
			for (int i = 0; i < tisectorState.slots; i++)
			{
				TIHabModuleState tihabModuleState = GameStateManager.CreateNewGameState<TIHabModuleState>();
				tihabModuleState.InitializeEmpty(tisectorState, i);
				tisectorState.habModules.Add(tihabModuleState);
			}
		}

		// Token: 0x06004044 RID: 16452 RVA: 0x0019D220 File Offset: 0x0019B420
		public void InitializeIncomes()
		{
			this.netAnnualIncomes = new Dictionary<TIFactionState, Dictionary<FactionResource, float>>();
			foreach (TIFactionState tifactionState in this.ref_factions)
			{
				this.netAnnualIncomes[tifactionState] = TIResourcesCost.habResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource value) => 0f);
			}
			this.administrationModuleModifier = 1f;
		}

		// Token: 0x06004045 RID: 16453 RVA: 0x0019D2D8 File Offset: 0x0019B4D8
		public void OnHabCreated()
		{
			if (!this.gameStateSubjectCreated)
			{
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					tifactionState.SetIntel(this, base.IntelOnCreation(tifactionState, this.sectors[0].faction), null, false);
				}
			}
			if (this.dockedFleets == null)
			{
				this.dockedFleets = new List<TISpaceFleetState>();
			}
			if (this.advisingCouncilors == null)
			{
				this.advisingCouncilors = new List<TICouncilorState>();
			}
			if (this.officersOnBoard == null)
			{
				this.officersOnBoard = (this.officersOnBoard = new List<TIOfficerState>());
			}
			this.gameStateSubjectCreated = true;
			Quaternion quaternion = Quaternion.Euler(Vector3.up - Vector3.forward);
			Vector3d zero = Vector3d.zero;
			if (this._dockedShipAbovePositions == null)
			{
				int i = 2;
				int num = 2;
				int num2 = 0;
				this._dockedShipAbovePositions = new TIConeLayoutState(in zero, in quaternion, in i, in num, in num2);
			}
			this.inEarthLEO = this.IsStation && base.orbitState.isEarthLEO;
		}

		// Token: 0x06004046 RID: 16454 RVA: 0x0019D3D8 File Offset: 0x0019B5D8
		public void SetFaction(TIFactionState faction)
		{
			if (base.faction != faction)
			{
				if (base.faction.primaryHab == this)
				{
					base.faction.primaryHab = null;
				}
				foreach (TISpaceFleetState tispaceFleetState in base.faction.fleets)
				{
					tispaceFleetState.SetHomePort(null);
				}
				if (this.IsBase && !faction.Prospected(this.ref_habSite))
				{
					faction.ProspectSpaceBody(this.ref_spaceBody);
				}
			}
			base.faction = faction;
			if (!this.netAnnualIncomes.ContainsKey(faction))
			{
				this.netAnnualIncomes.Add(faction, new Dictionary<FactionResource, float>());
				this.netAnnualIncomes[faction] = TIResourcesCost.habResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource value) => 0f);
			}
			this.UpdateCurrentAnnualNetResourceIncomes(false);
			faction.SetResourceIncomeDataDirty();
		}

		// Token: 0x06004047 RID: 16455 RVA: 0x0019D504 File Offset: 0x0019B704
		public void SetCustomIconString(string iconString)
		{
			this.customHabIconResource = iconString;
			GameControl.eventManager.TriggerEvent(new HabSymbolAssigned(this), null, new object[] { this });
		}

		// Token: 0x06004048 RID: 16456 RVA: 0x0019D528 File Offset: 0x0019B728
		public string CaptureHab(TIFactionState capturingFaction, int successLevel, bool traded = false, bool defected = false, Dictionary<TIFactionState, string> factionStrings = null, TISpaceFleetState capturingFleet = null)
		{
			if (base.faction != null && base.faction.AISavingTarget.active)
			{
				TIGameState location = base.faction.AISavingTarget.location;
				if (((location != null) ? location.ref_hab : null) == this)
				{
					base.faction.AIClearSavingTarget("Hab captured");
				}
			}
			if (this.SpaceCombatValue() > 0f)
			{
				foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets.ToList<TISpaceFleetState>())
				{
					if (!capturingFaction.permanentAlly(tispaceFleetState.faction))
					{
						tispaceFleetState.DepartFromDockingLocation();
					}
				}
			}
			string empty = string.Empty;
			TIFactionState ref_faction = this.ref_faction;
			float num = this.GetNetCurrentMonthlyIncome(ref_faction, FactionResource.Research, true, false) * 12f;
			float netCurrentMonthlyIncome = this.GetNetCurrentMonthlyIncome(ref_faction, FactionResource.Research, true, false);
			List<TIProjectTemplate> list = ref_faction.StealableProjects(capturingFaction);
			List<TISpaceShipTemplate> list2 = this.ShipsBeingBuiltAtHab(ref_faction);
			bool flag = list2.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.requiresExotics);
			float num2 = 0f;
			float num3 = 0f;
			if (!this.IsAlien() && capturingFaction != GameStateManager.AlienFaction())
			{
				switch (successLevel)
				{
				case -1:
					capturingFaction.GainIntel(ref_faction, (float)(2 * this.tier), null, false);
					num3 = 0.5f;
					break;
				case 0:
					capturingFaction.GainIntel(ref_faction, (float)(5 * this.tier), null, false);
					num3 = 0.4f;
					break;
				case 1:
					capturingFaction.GainIntel(ref_faction, (float)(10 * this.tier), null, false);
					ref_faction.TransferResourceToFaction(num * 0.25f, FactionResource.Research, capturingFaction);
					num3 = 0.3f;
					break;
				case 2:
					capturingFaction.GainIntel(ref_faction, (float)(15 * this.tier), null, false);
					ref_faction.TransferResourceToFaction(num * 0.5f, FactionResource.Research, capturingFaction);
					this.ResolveDefendHabEffect(capturingFaction, 3);
					if (netCurrentMonthlyIncome > 0f && list.Count > 0)
					{
						TIProjectTemplate tiprojectTemplate = list.SelectRandomItem<TIProjectTemplate>();
						capturingFaction.OnProjectComplete(tiprojectTemplate, capturingFaction.GetSlotForProject(tiprojectTemplate), false, false);
					}
					break;
				case 3:
					capturingFaction.GainIntel(ref_faction, (float)(20 * this.tier), null, false);
					ref_faction.TransferResourceToFaction(num * 0.75f, FactionResource.Research, capturingFaction);
					if (netCurrentMonthlyIncome > 0f && list.Count > 0)
					{
						TIProjectTemplate tiprojectTemplate2 = list.SelectRandomItem<TIProjectTemplate>();
						capturingFaction.OnProjectComplete(tiprojectTemplate2, capturingFaction.GetSlotForProject(tiprojectTemplate2), false, false);
					}
					num3 = 0.1f;
					break;
				case 4:
					capturingFaction.GainIntel(ref_faction, (float)(30 * this.tier), null, false);
					ref_faction.TransferResourceToFaction(num, FactionResource.Research, capturingFaction);
					this.ResolveDefendHabEffect(capturingFaction, 6);
					if (netCurrentMonthlyIncome > 0f && list.Count > 0)
					{
						TIProjectTemplate tiprojectTemplate3 = list.SelectRandomItem<TIProjectTemplate>();
						capturingFaction.OnProjectComplete(tiprojectTemplate3, capturingFaction.GetSlotForProject(tiprojectTemplate3), false, false);
					}
					break;
				}
				List<TIHabModuleState> list3 = new List<TIHabModuleState>();
				foreach (TIHabModuleState tihabModuleState in this.AllModules())
				{
					if (tihabModuleState.decommissioning)
					{
						tihabModuleState.CancelDecommissionModule();
					}
				}
				foreach (TIHabModuleState tihabModuleState2 in this.OkayModules())
				{
					if (tihabModuleState2.moduleTemplate.objectiveModule)
					{
						list3.Add(tihabModuleState2);
					}
					else if (!tihabModuleState2.moduleTemplate.coreModule && (successLevel == 3 || successLevel <= 1))
					{
						if (tihabModuleState2.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops))
						{
							list3.Add(tihabModuleState2);
						}
						else if (TIUtilities.RandomFloatValue() < num3)
						{
							list3.Add(tihabModuleState2);
						}
					}
				}
				int num4 = 0;
				int num5 = 0;
				foreach (TIHabModuleState tihabModuleState3 in list3)
				{
					this.DestroyModule(capturingFaction, tihabModuleState3, out num4, out num5, true, true, true, (float)(tihabModuleState3.okay ? 1 : 0), false, false, true);
				}
				base.faction.ResetPrimaryHab();
				if (num4 > 0)
				{
					capturingFaction.CommitAtrocity(num4, TIFactionState.AtrocityCause.DestroyedCivilianModules, false, 0.333f);
				}
				if (num5 > 0)
				{
					base.faction.CommitAtrocity(0, TIFactionState.AtrocityCause.LostCivilianModules, true, 0.333f);
				}
				foreach (TISectorState tisectorState in this.sectors)
				{
					if (tisectorState.faction != null)
					{
						tisectorState.SetFaction(capturingFaction);
					}
				}
				foreach (TISpaceFleetState tispaceFleetState2 in GameStateManager.IterateByClass<TISpaceFleetState>(true))
				{
					if (tispaceFleetState2.homeport == this && tispaceFleetState2.faction != capturingFaction)
					{
						tispaceFleetState2.SetHomePort(null);
					}
				}
				if (!traded)
				{
					if (defected)
					{
						TINotificationQueueState.LogHabDefected(this, capturingFaction, ref_faction, list3.Count > 0);
					}
					else
					{
						TINotificationQueueState.LogHabAcquired(this, capturingFaction, ref_faction, list3.Count > 0, successLevel == 3 || successLevel <= 1, successLevel <= 0, factionStrings);
					}
					TINotificationQueueState.LogOurHabControlled(ref_faction, capturingFaction, this);
					AIDailyFactionPlanner.AIReaction(AIReactionEvent.MyHabCaptured, this, ref_faction);
				}
				IEnumerable<TICouncilorState> enumerable = GameStateManager.IterateByClass<TICouncilorState>(false);
				Func<TICouncilorState, bool> <>9__1;
				Func<TICouncilorState, bool> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (TICouncilorState x) => x.faction != null && x.detained && x.location.ref_hab == this);
				}
				using (IEnumerator<TICouncilorState> enumerator5 = enumerable.Where<TICouncilorState>(func).GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						TICouncilorState ticouncilorState = enumerator5.Current;
						ticouncilorState.ReleaseCouncilor(false);
					}
					goto IL_0734;
				}
			}
			bool flag2 = this.IsAlien();
			if (flag2)
			{
				List<HabModuleSpecialRule> list4 = this.ActiveSpecialAbilities(ref_faction);
				capturingFaction.CompleteMilestone(CampaignMilestone.AccessAlienTech);
				capturingFaction.CompleteMilestone(CampaignMilestone.AccessGriffinCorpus);
				capturingFaction.GainIntel(ref_faction, (float)(5 * (1 + successLevel) * this.tier), null, false);
				num2 += TemplateManager.global.ExoticsPerAlienHabTier * (float)this.tier * (float)(1 + successLevel) * (0.8f + TIUtilities.RandomRange(0f, 0.4f));
				flag = flag || num2 > 0f;
				if (this.tier >= 2)
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessHydraCorpus);
				}
				if (successLevel >= 3)
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessLiveGriffin);
				}
				if ((successLevel >= 3 && this.tier >= 2) || GameStateManager.AlienFaction().CouncilorsOnEarth.Count == 0)
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessLiveHydra);
				}
				if (list4.Contains(HabModuleSpecialRule.Griffins))
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessLiveGriffin);
				}
				if (list4.Contains(HabModuleSpecialRule.Salamanders))
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
				}
				if (list4.Contains(HabModuleSpecialRule.WarDogs))
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessWarDogCorpus);
				}
				if (list2.Count > 0)
				{
					capturingFaction.CompleteMilestone(CampaignMilestone.AccessAlienShip);
				}
			}
			if (list2.Count > 0)
			{
				num2 += list2.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.spaceResourceConstructionCost(false, null, false, true, false).GetSingleCostValue(FactionResource.Exotics) * Mathf.Clamp((float)successLevel / 4f, 0f, 1f));
				bool flag3 = flag || num2 > 0f;
			}
			this.DestroyHab(capturingFaction, (float)(2 + successLevel) / 10f, false, capturingFleet, num2);
			if (flag2)
			{
				capturingFaction.FixAssessedAlienHateToActualValue();
			}
			IL_0734:
			ref_faction.FactionExposed(capturingFaction);
			capturingFaction.SetMissionControlUsageDataDirty();
			ref_faction.SetMissionControlUsageDataDirty();
			return empty;
		}

		// Token: 0x06004049 RID: 16457 RVA: 0x0019DCD8 File Offset: 0x0019BED8
		public TIHabModuleState SelectModuleToDestroy()
		{
			return (from x in this.OkayModules()
				where !x.ref_habModule.moduleTemplate.coreModule && !x.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole)
				select x).SelectRandomItem<TIHabModuleState>();
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x0019DD09 File Offset: 0x0019BF09
		public TIHabModuleState SelectModuleToDestroy_Marines()
		{
			return (from x in this.OkayModules()
				where x.ref_habModule.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops)
				select x).SelectRandomItem<TIHabModuleState>();
		}

		// Token: 0x0600404B RID: 16459 RVA: 0x0019DD3A File Offset: 0x0019BF3A
		public TIHabModuleState SelectModuleToDestroy_Power()
		{
			return (from x in this.OkayModules()
				where x.ref_habModule.moduleTemplate.powerSource
				select x).SelectRandomItem<TIHabModuleState>();
		}

		// Token: 0x0600404C RID: 16460 RVA: 0x0019DD6B File Offset: 0x0019BF6B
		public IEnumerator DestroyModuleFromCombatDelayed(TIFactionState destroyer, TIHabModuleState moduleToDestroy, float delay)
		{
			yield return delay;
			this.DestroyModule(destroyer, moduleToDestroy, false, false, true, 0f, true, false);
			yield break;
		}

		// Token: 0x0600404D RID: 16461 RVA: 0x0019DD90 File Offset: 0x0019BF90
		public bool DestroyModule(TIFactionState destroyer, TIHabModuleState moduleToDestroy, bool suppressLogging = false, bool skipFullDestructioncheck = false, bool alwaysAlert = true, float hate = 0f, bool skipRepowerOrder = false, bool fromMission = false)
		{
			int num;
			int num2;
			return this.DestroyModule(destroyer, moduleToDestroy, out num, out num2, suppressLogging, skipFullDestructioncheck, alwaysAlert, hate, skipRepowerOrder, fromMission, false);
		}

		// Token: 0x0600404E RID: 16462 RVA: 0x0019DDB8 File Offset: 0x0019BFB8
		public bool DestroyModule(TIFactionState destroyer, TIHabModuleState moduleToDestroy, out int accumulatedAtrocities_Killer, out int accumulatedAtrocities_Loser, bool suppressLogging = false, bool skipFullDestructioncheck = false, bool alwaysAlert = true, float hate = 0f, bool skipRepowerOrder = false, bool fromMission = false, bool dontProcessAtrocitiesLocally = false)
		{
			bool flag = false;
			accumulatedAtrocities_Killer = 0;
			accumulatedAtrocities_Loser = 0;
			if (moduleToDestroy.okay)
			{
				if (base.faction.IsAlienFaction && this == base.faction.primaryHab && moduleToDestroy.moduleTemplate != null && (moduleToDestroy.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole) || moduleToDestroy.moduleTemplate.coreModule))
				{
					return false;
				}
				if (!suppressLogging)
				{
					if (moduleToDestroy.moduleTemplate.alertWorthy || alwaysAlert)
					{
						TINotificationQueueState.LogOurCriticalHabModuleDestroyed(moduleToDestroy, destroyer, hate, fromMission);
					}
					else
					{
						TINotificationQueueState.LogOurHabModuleDestroyed(moduleToDestroy, destroyer, hate, fromMission, "");
					}
				}
				if (moduleToDestroy.moduleTemplate.missionControl != 0)
				{
					moduleToDestroy.sector.faction.SetMissionControlUsageDataDirty();
				}
				if (destroyer != null && hate > 0f)
				{
					base.faction.GainFactionHate(destroyer, (float)moduleToDestroy.tier * TemplateManager.global.factionHateMultiplierPerModuleDestroyedPerTier, false, "Hab Module Destroyed", true);
					int num = moduleToDestroy.AtrocitiesToDestroy();
					int num2 = moduleToDestroy.AtrocitiesToLose();
					if (dontProcessAtrocitiesLocally)
					{
						accumulatedAtrocities_Killer += num;
						accumulatedAtrocities_Loser += num2;
					}
					else
					{
						if (num > 0)
						{
							destroyer.CommitAtrocity(num, TIFactionState.AtrocityCause.DestroyedCivilianModules, false, 0.333f);
						}
						if (num2 > 0)
						{
							base.faction.CommitAtrocity(0, TIFactionState.AtrocityCause.LostCivilianModules, true, 0.333f);
						}
					}
				}
				moduleToDestroy.DestroyModule();
				GameControl.eventManager.TriggerEvent(new HabModuleDestroyed(moduleToDestroy, TIGlobalValuesState.isSpaceCombatEnabled), null, new object[] { this });
				flag = true;
			}
			if (!skipFullDestructioncheck && this.OkayModules().Count == 0)
			{
				this.DestroyHab(destroyer, 0f, false, null, 0f);
				return true;
			}
			if (!skipRepowerOrder)
			{
				this.UpdatePowerManagement(false, null, base.faction.player.isAI);
				base.faction.ResetPrimaryHab();
			}
			return flag;
		}

		// Token: 0x0600404F RID: 16463 RVA: 0x0019DF6A File Offset: 0x0019C16A
		public int AtrocitiesFromDestruction()
		{
			return this.FunctionalModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.AtrocitiesToDestroy());
		}

		// Token: 0x06004050 RID: 16464 RVA: 0x0019DF96 File Offset: 0x0019C196
		public int AtrocitiesFromLoss()
		{
			return this.FunctionalModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.AtrocitiesToLose());
		}

		// Token: 0x06004051 RID: 16465 RVA: 0x0019DFC2 File Offset: 0x0019C1C2
		public void PostCombat()
		{
			this.UpdatePowerManagement(false, null, base.faction.player.isAI);
		}

		// Token: 0x06004052 RID: 16466 RVA: 0x0019DFDC File Offset: 0x0019C1DC
		public bool CoreInTransit()
		{
			if (!this.anyCoreCompleted)
			{
				TIHabModuleState coreModule = this.coreModule;
				bool flag;
				if (coreModule == null)
				{
					flag = false;
				}
				else
				{
					DateTime startBuildDate = coreModule.startBuildDate;
					flag = true;
				}
				if (flag)
				{
					return new TIDateTime(this.coreModule.startBuildDate) > TITimeState.Now();
				}
			}
			return false;
		}

		// Token: 0x06004053 RID: 16467 RVA: 0x0019E018 File Offset: 0x0019C218
		public void DestroyHab(TIFactionState destroyer, float recoveryMultiplier, bool peacefulDecommission = false, TISpaceFleetState destroyingFleet = null, float bonusExotics = 0f)
		{
			TIFactionState faction = base.faction;
			int tier = this.tier;
			if (faction != null && faction.AISavingTarget.active)
			{
				TIGameState location = faction.AISavingTarget.location;
				if (((location != null) ? location.ref_hab : null) == this)
				{
					faction.AIClearSavingTarget("Hab destroyed");
				}
			}
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (!peacefulDecommission)
			{
				if (destroyer != null && destroyer != faction && recoveryMultiplier > 0f)
				{
					recoveryMultiplier = Mathf.Clamp(recoveryMultiplier, 0f, 1f);
					List<TIHabModuleState> list = this.FunctionalModules();
					if (faction.IsAlienFaction && this == faction.primaryHab)
					{
						list.RemoveAll((TIHabModuleState x) => x.moduleTemplate.coreModule);
						list.RemoveAll((TIHabModuleState x) => x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole));
					}
					foreach (TIHabModuleState tihabModuleState in list)
					{
						tiresourcesCost.SumCosts_NoDuration(tihabModuleState.moduleTemplate.BuildMaterials(this.irradiatedMultiplier, this.ref_spaceBody, this.ref_naturalSpaceObject, faction, recoveryMultiplier).ToResourcesCost(1f));
					}
					if (bonusExotics > 0f)
					{
						tiresourcesCost.AddCost(FactionResource.Exotics, bonusExotics, false);
					}
					tiresourcesCost.RefundCost(destroyer, "Hab Destruction Salvage");
				}
				new Dictionary<TISpaceFleetState, Vector3>().Keys.GetEnumerator();
				if (destroyer != faction)
				{
					faction.GainFactionHate(destroyer, 1f + TemplateManager.global.factionHateForHabDestructionOperationPerTier * (float)this.tier, false, "My hab destroyed", true);
				}
				if (!this.coreModule.moduleTemplate.automated && this.coreModule.priorModuleTemplate != null && !this.coreModule.priorModuleTemplate.automated)
				{
					TITraitTemplate.ProcessLoyaltyChangeFromTraits(faction, SpecialTraitRule.LoyaltyLossOnFactionHabDestroyedPerTier, this.tier);
				}
			}
			float num = 0f;
			bool flag = this.anyCoreCompleted;
			bool flag2 = !this.CoreInTransit();
			int num2 = 0;
			int num3 = 0;
			foreach (TIHabModuleState tihabModuleState2 in this.AllModules())
			{
				if (tihabModuleState2.functional)
				{
					num += (float)(tihabModuleState2.tier * tihabModuleState2.tier);
					if (tihabModuleState2.moduleTemplate.mine)
					{
						num += (float)tihabModuleState2.tier;
					}
					else if (tihabModuleState2.moduleTemplate.constructionModule)
					{
						num += (float)tihabModuleState2.tier;
					}
				}
				else if (tihabModuleState2.underConstruction)
				{
					if (tihabModuleState2.priorModuleTemplate != null)
					{
						num += (float)Mathf.Min(tihabModuleState2.tier * tihabModuleState2.tier, tihabModuleState2.priorModuleTemplate.tier * tihabModuleState2.priorModuleTemplate.tier);
					}
					else if (this.anyCoreCompleted)
					{
						num += (float)(tihabModuleState2.tier * tihabModuleState2.tier) / (16f * TemplateManager.global.AI_GetHateBurnoffFromKillingHabmodulesDivisor(faction.IsAlienFaction));
					}
				}
				int num4;
				bool flag3 = this.DestroyModule(destroyer, tihabModuleState2, out num4, out num2, true, true, true, 1f, false, false, true);
				num3 += num4;
				if (flag3)
				{
					TINotificationQueueState.CleanQueueOfArchivedState(tihabModuleState2, this.IsStation ? base.orbitState.ref_gameState : this.habSite.ref_gameState);
				}
			}
			if (num3 > 0)
			{
				destroyer.CommitAtrocity(num3, TIFactionState.AtrocityCause.DestroyedCivilianModules, false, 0.333f);
			}
			if (num2 > 0)
			{
				faction.CommitAtrocity(0, TIFactionState.AtrocityCause.LostCivilianModules, true, 0.333f);
			}
			if (faction.IsAlienFaction && this == faction.primaryHab)
			{
				TINotificationQueueState.LogHabDestroyed(this, destroyer, faction, tiresourcesCost, null);
				return;
			}
			TINotificationQueueState.CleanQueueOfArchivedState(this, this.IsStation ? base.orbitState.ref_gameState : this.habSite.ref_gameState);
			if (!peacefulDecommission && destroyer != null)
			{
				float num5;
				if (flag)
				{
					num5 = (float)this.tier;
				}
				else if (flag2)
				{
					num5 = (float)this.tier / 2f;
				}
				else
				{
					num5 = 0f;
				}
				destroyer.RegisterKill(this, Mathf.Max(num5, num / TemplateManager.global.AI_GetHateBurnoffFromKillingHabmodulesDivisor(destroyer.IsAlienFaction)));
			}
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CleanStateFromGoalTargets(this);
			}
			if (this.underBombardment)
			{
				foreach (TISpaceFleetState tispaceFleetState in this.ref_spaceBody.fleetsInOrbit.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					TIGameState bombardmentTarget = x.bombardmentTarget;
					return ((bombardmentTarget != null) ? bombardmentTarget.ref_hab : null) == this;
				}))
				{
					tispaceFleetState.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.TargetDestroyed);
				}
			}
			foreach (TIMissionState timissionState in GameStateManager.AllActiveMissions())
			{
				if (timissionState.target.ref_hab == this && !GameStateManager.MissionPhase().currentlyResolvingMissions.Contains(timissionState))
				{
					timissionState.ResolveMission(TIMissionState.AbortReason.TargetHabDestroyed, "");
				}
			}
			foreach (TISpaceFleetState tispaceFleetState2 in this.dockedFleets)
			{
				tispaceFleetState2.ForceCancelCurrentOperations();
			}
			foreach (TISpaceFleetState tispaceFleetState3 in faction.fleets)
			{
				if (tispaceFleetState3.homeport == this)
				{
					tispaceFleetState3.SetHomePort(null);
				}
			}
			base.ArchiveState(true);
			List<TISpaceFleetState> list2 = new List<TISpaceFleetState>(this.dockedFleets);
			using (List<TICouncilorState>.Enumerator enumerator5 = GameStateManager.IterateByClass<TICouncilorState>(false).ToList<TICouncilorState>().GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					TICouncilorState councilor = enumerator5.Current;
					if (this.councilorsOnBoard.Contains(councilor) || councilor.location == this)
					{
						if (councilor.faction == null)
						{
							councilor.Retire();
						}
						else
						{
							List<TISpaceFleetState> list3 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == councilor.faction && x.ships.Count > 0).ToList<TISpaceFleetState>();
							if (list3.Count > 0)
							{
								councilor.SetLocation(list3.MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.currentDeltaV_kps).ships[0]);
							}
							else
							{
								bool flag4 = councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.Undercover);
								list3 = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.IsAlien()).ToList<TISpaceFleetState>();
								if (flag4 && list3.Count > 0)
								{
									councilor.SetLocation(list3.MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.currentDeltaV_kps).ships[0]);
								}
								else if (this.ref_spaceBody != null && (this.ref_spaceBody.isEarth || this.ref_spaceBody.isLuna))
								{
									councilor.SetLocation(GameStateManager.AllRegions().SelectRandomItem<TIRegionState>());
								}
								else if (!peacefulDecommission)
								{
									councilor.KillCouncilor(true, destroyer);
								}
								else
								{
									List<TIHabState> list4 = this.ref_naturalSpaceObject.habsInSystem.Where<TIHabState>((TIHabState x) => x != this).ToList<TIHabState>();
									if (list4.Count > 0)
									{
										List<TIHabState> list5 = list4.Where<TIHabState>((TIHabState x) => x.faction == councilor.faction).ToList<TIHabState>();
										if (list5.Count > 0)
										{
											councilor.SetLocation(list5.SelectRandomItem<TIHabState>());
										}
										else if (!councilor.isAlien)
										{
											List<TIHabState> list6 = list4.Where<TIHabState>((TIHabState x) => x.tier > 1).ToList<TIHabState>();
											if (list6.Count > 0)
											{
												councilor.SetLocation(list6.SelectRandomItem<TIHabState>());
											}
											else if (flag4)
											{
												councilor.SetLocation(list4.SelectRandomItem<TIHabState>());
											}
											else
											{
												councilor.KillCouncilor(false, null);
											}
										}
										else
										{
											councilor.KillCouncilor(false, null);
										}
									}
									else if (councilor.faction == faction || flag4 || this.tier > 1)
									{
										councilor.SetLocation(GameStateManager.AllRegions().SelectRandomItem<TIRegionState>());
									}
									else
									{
										councilor.KillCouncilor(false, null);
									}
								}
							}
						}
					}
				}
			}
			if (this.officersOnBoard.Count > 0)
			{
				if (!peacefulDecommission)
				{
					this.officersOnBoard.ToList<TIOfficerState>().ForEach(delegate(TIOfficerState x)
					{
						x.DeleteOfficer(true);
					});
				}
				else
				{
					foreach (TIOfficerState tiofficerState in this.officersOnBoard.ToList<TIOfficerState>())
					{
						if (!tiofficerState.Escape(false, false))
						{
							tiofficerState.DeleteOfficer(false);
						}
					}
				}
			}
			foreach (TISpaceFleetState tispaceFleetState4 in list2)
			{
				if (this.IsStation)
				{
					tispaceFleetState4.DepartFromDockingLocation();
					tispaceFleetState4.AssumeMatchingOrbitFromState(this, false);
					tispaceFleetState4.dockedLocation = null;
				}
				else
				{
					tispaceFleetState4.Land(this.habSite);
				}
			}
			if (!peacefulDecommission)
			{
				TINotificationQueueState.LogHabDestroyed(this, destroyer, faction, tiresourcesCost, destroyingFleet);
				TINotificationQueueState.LogOurHabDestroyed(this, destroyer, destroyingFleet);
				if (destroyer.IsAlienFaction)
				{
					GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
					{
						x.CompleteMilestone(CampaignMilestone.AliensAttackInSpace);
					});
				}
				faction.NeverForget(this, destroyer);
			}
			GameControl.eventManager.TriggerEvent(new HabDestroyed(this, list2.FirstOrDefault<TISpaceFleetState>()), null, new object[] { this, this.ref_naturalSpaceObject, this.ref_orbit, this.ref_habSite }.Where<object>((object x) => x != null).ToArray<object>());
			if (this.habSite != null)
			{
				this.habSite.hab = null;
			}
			if (base.orbitState != null)
			{
				base.orbitState.assetsInOrbit.Remove(this);
				if (!peacefulDecommission)
				{
					base.orbitState.DestroyedAssetsChange(tier);
				}
			}
			array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ExpireIntel(this, true);
			}
			foreach (TISpaceFleetState tispaceFleetState5 in GameStateManager.IterateByClass<TISpaceFleetState>(true))
			{
				int num6 = 100;
				Trajectory trajectory = tispaceFleetState5.trajectory;
				while (trajectory != null && num6 > 0)
				{
					num6--;
					if (trajectory.destination == this)
					{
						trajectory.DestinationDestroyed();
						if (tispaceFleetState5.inTransfer && tispaceFleetState5.trajectory == trajectory)
						{
							tispaceFleetState5.RefreshTrajectory();
							GameControl.eventManager.TriggerEvent(new StartFleetOperation(tispaceFleetState5, TemplateManager.Find<TIOperationTemplate>("TransferOperation", true), tispaceFleetState5.trajectory.destinationOrbit), null, new object[] { tispaceFleetState5 });
						}
					}
					trajectory = trajectory.nextTrajectory;
				}
				if (tispaceFleetState5.homeport == this)
				{
					tispaceFleetState5.SetHomePort(null);
				}
				if (tispaceFleetState5.dockedLocation == this)
				{
					if (this.IsStation)
					{
						tispaceFleetState5.dockedLocation = null;
						tispaceFleetState5.AssumeMatchingOrbitFromState(this, false);
					}
					else
					{
						tispaceFleetState5.Land(this.habSite);
					}
				}
			}
			foreach (TIHabModuleState tihabModuleState3 in new List<TIHabModuleState>(this.AllModuleStates()))
			{
				tihabModuleState3.ArchiveState(true);
				GameStateManager.RemoveGameState<TIHabModuleState>(tihabModuleState3.ID, false);
			}
			new List<TISectorState>(this.sectors);
			foreach (TISectorState tisectorState in this.sectors)
			{
				TIFactionState faction2 = tisectorState.faction;
				if (faction2 != null)
				{
					faction2.habSectors.Remove(tisectorState);
				}
				tisectorState.habModules.Clear();
				tisectorState.ArchiveState(true);
				GameStateManager.RemoveGameState<TISectorState>(tisectorState.ID, false);
			}
			this.habSite = null;
			this.anyCoreCompleted = false;
			this.sectors.Clear();
			faction.habs.Remove(this);
			GameStateManager.AllFactions().SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets).ToList<TISpaceFleetState>()
				.ForEach(delegate(TISpaceFleetState x)
				{
					x.RemoveFailedAttackRecord(this);
				});
			faction.ResetPrimaryHab();
			if (this.IsStation)
			{
				base.gameObjectLink.Remove<HabBuildComponent>(false);
				base.gameObjectLink.GetComponent<HabComponent>().enabled = false;
				base.gameObjectLink.GetComponent<SpaceObjectComponent>().enabled = false;
				global::UnityEngine.Object.Destroy(this.baseObject, 5f);
				if (base.controller.orbitTrailLink != null)
				{
					global::UnityEngine.Object.Destroy(base.controller.orbitTrailLink);
				}
			}
			else
			{
				this.baseObject.GetComponent<HabBuildComponent>().enabled = false;
				if (peacefulDecommission)
				{
					this.baseObject.Remove<HabBuildComponent>(false);
					global::UnityEngine.Object.Destroy(this.baseObject, 5f);
				}
				else
				{
					this.baseObject.Remove<HabBuildComponent>(false);
					global::UnityEngine.Object.Destroy(this.baseObject);
				}
			}
			base.ArchiveState(true);
			GameStateManager.RemoveGameState<TIHabState>(base.ID, false);
			faction.SetMissionControlUsageDataDirty();
		}

		// Token: 0x06004054 RID: 16468 RVA: 0x0019EF50 File Offset: 0x0019D150
		public void BeginDecommissionModule(TIHabModuleState module)
		{
			GameControl.eventManager.TriggerEvent(new HabModuleDecommissionStatusChange(module), null, Array.Empty<object>());
			module.BeginDecomissionModule();
		}

		// Token: 0x06004055 RID: 16469 RVA: 0x0019EF70 File Offset: 0x0019D170
		public void CompleteDecommissionModule(TIHabModuleState module, bool clearPriorModule)
		{
			bool coreModule = module.moduleTemplate.coreModule;
			module.CompleteDecommissionModule(clearPriorModule);
			if (coreModule)
			{
				this.DecommissionHab();
				return;
			}
			this.UpdateAllModuleConnectors();
			this.ValidateLocalPopulationRequirementsForAllNearbyHabs();
			GameControl.eventManager.TriggerEvent(new HabModuleConstructionStatusChange(module), null, new object[]
			{
				module,
				module.sector,
				this,
				module.sector.faction
			});
			base.faction.ResetPrimaryHab();
		}

		// Token: 0x06004056 RID: 16470 RVA: 0x0019EFE8 File Offset: 0x0019D1E8
		public TIResourcesCost DecommissionHabCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (TIHabModuleState tihabModuleState in this.OkayModules())
			{
				tiresourcesCost.SumCosts_NoDuration(tihabModuleState.DecommissionModuleCost());
			}
			tiresourcesCost.SetCompletionTime_Days(this.coreModule.DecommissionDuration_days());
			return tiresourcesCost;
		}

		// Token: 0x06004057 RID: 16471 RVA: 0x0019F058 File Offset: 0x0019D258
		public bool CanDecommissionHab()
		{
			return !this.decommissioning && !this.underBombardment && !this.underAssault;
		}

		// Token: 0x06004058 RID: 16472 RVA: 0x0019F078 File Offset: 0x0019D278
		public TIResourcesCost DecommissionHabRefund()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (TIHabModuleState tihabModuleState in this.OkayModules())
			{
				tiresourcesCost.SumCosts_NoDuration(tihabModuleState.DecomissionModuleResourceRefund());
			}
			return tiresourcesCost;
		}

		// Token: 0x06004059 RID: 16473 RVA: 0x0019F0D8 File Offset: 0x0019D2D8
		public void BeginDecommissionHab()
		{
			if (this.coreDefended)
			{
				this.ExpireDefense(false);
			}
			this.anyCoreCompleted = false;
			foreach (TIHabModuleState tihabModuleState in this.OkayModules())
			{
				this.BeginDecommissionModule(tihabModuleState);
			}
			GameControl.eventManager.TriggerEvent(new HabDecommissionStatusChange(this), null, Array.Empty<object>());
		}

		// Token: 0x0600405A RID: 16474 RVA: 0x0019F158 File Offset: 0x0019D358
		public void DecommissionHab()
		{
			TINotificationQueueState.LogDecommissionHabComplete(this);
			this.DestroyHab(base.faction, 0f, true, null, 0f);
		}

		// Token: 0x0600405B RID: 16475 RVA: 0x0019F178 File Offset: 0x0019D378
		public void ConstructFoundingModule(string moduleTemplateName, int sector, int slot, float deliveryAndBuildTime_days)
		{
			TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(moduleTemplateName, false);
			if (tihabModuleTemplate.mine)
			{
				sector = 0;
				slot = 1;
			}
			if (deliveryAndBuildTime_days <= 0f)
			{
				deliveryAndBuildTime_days = tihabModuleTemplate.buildTime_Days * TIGlobalValuesState.GetHabModuleConstructionTimeSettingsModifier(base.faction) * base.faction.GetHabConstructionDurationModifier();
			}
			this.sectors[sector].habModules[slot].InitiateConstructModule(moduleTemplateName, null, (double)deliveryAndBuildTime_days);
		}

		// Token: 0x0600405C RID: 16476 RVA: 0x0019F1E8 File Offset: 0x0019D3E8
		public void InitiateModuleConstruction(TISectorState sector, int slot, TIHabModuleTemplate moduleTemplate, TIResourcesCost cost)
		{
			if (sector == null || sector.faction == null || sector.habModules == null || sector.habModules.Count <= slot)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				return;
			}
			TIHabModuleState tihabModuleState = sector.habModules[slot];
			if (tihabModuleState.hasModule)
			{
				if (tihabModuleState.active && !tihabModuleState.moduleTemplate.coreModule)
				{
					tihabModuleState.SetPowerStatus(false, false);
				}
				if (sector.faction.nShipyardQueues.ContainsKey(tihabModuleState) && !moduleTemplate.allowsShipConstruction)
				{
					sector.faction.RemoveShipyardFromFaction(tihabModuleState, true);
				}
				this.ValidateLocalPopulationRequirementsForAllNearbyHabs();
			}
			tihabModuleState.InitiateConstructModule(moduleTemplate.dataName, cost, (double)cost.completionTime_days);
			cost.PayCost(sector.faction, "Construct Hab Module");
			base.faction.RecordExpenditure(TIFactionState.Expenditure.HabConstruction, cost);
			this.UpdateAllModuleConnectors();
			this.UpdatePowerManagement(false, null, base.faction.player.isAI);
			if (!base.faction.IsAlienFaction)
			{
				foreach (TIObjectiveTemplate tiobjectiveTemplate in base.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked))
				{
					if (!string.IsNullOrEmpty(tiobjectiveTemplate.targetHabModuleName) && tihabModuleState.moduleTemplate == tiobjectiveTemplate.targetHabModuleTemplate)
					{
						base.faction.primaryHab = this;
					}
				}
			}
			GameControl.eventManager.TriggerEvent(new HabModuleConstructionStatusChange(tihabModuleState), null, new object[] { tihabModuleState, sector, this, sector.faction });
			if (moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Solar_Power_Variable_Output))
			{
				base.faction.CompleteMilestone(CampaignMilestone.TutorialBuildStationPower);
			}
			if (moduleTemplate.incomeResearch_month > 0f && moduleTemplate.techBonuses.Length != 0)
			{
				base.faction.CompleteMilestone(CampaignMilestone.TutorialBuildStationLab);
			}
		}

		// Token: 0x0600405D RID: 16477 RVA: 0x0019F3C8 File Offset: 0x0019D5C8
		public void CompleteModuleConstruction(TIHabModuleState module)
		{
			bool flag = false;
			if (module.moduleTemplate != null)
			{
				flag = module.ModulePower() >= 0 || this.EnoughPowerForModule(module);
			}
			module.CompleteConstruction(false);
			if (module.moduleTemplate != null)
			{
				module.SetPowerStatus(flag, false);
				if (module.moduleTemplate.coreModule)
				{
					this.UpdatePowerManagement(true, null, this.coreFaction.player.isAI);
				}
				else if (module.moduleTemplate.powerSource)
				{
					this.UpdatePowerManagement(true, module, this.coreFaction.player.isAI);
				}
				if (module.moduleTemplate.alertWorthy)
				{
					TINotificationQueueState.LogCriticalHabModuleComplete(module.sector, module.moduleTemplate);
				}
				else
				{
					TINotificationQueueState.LogHabModuleComplete(module.sector, module.moduleTemplate, "");
				}
				if (module.moduleTemplate.coreModule)
				{
					int tier = this.tier;
					this.tier = module.moduleTemplate.tier;
					if (this.IsStation)
					{
						if (tier == 1 && this.tier > 1)
						{
							this.sectors[2].SetFaction(this.sectors[0].faction);
							this.sectors[4].SetFaction(this.sectors[0].faction);
						}
						if (this.tier == 3 && tier < 3)
						{
							this.sectors[1].SetFaction(this.sectors[0].faction);
							this.sectors[3].SetFaction(this.sectors[0].faction);
						}
					}
					else
					{
						if (tier == 1 && this.tier > 1)
						{
							this.sectors[1].SetFaction(this.sectors[0].faction);
							this.sectors[2].SetFaction(this.sectors[0].faction);
						}
						if (this.tier == 3 && tier < 3)
						{
							this.sectors[3].SetFaction(this.sectors[0].faction);
							this.sectors[4].SetFaction(this.sectors[0].faction);
						}
					}
				}
			}
			if (base.faction.isActivePlayer && !this.barycenter.isEarth && !this.barycenter.GetSunOrbitingRelatedObject.isEarth && this.barycenter.Populous())
			{
				base.faction.UnlockAchievement("spacebodyPopulation");
			}
			module.sector.habModules[module.slot] = module;
			this.UpdateAllModuleConnectors();
			GameControl.eventManager.TriggerEvent(new HabModuleConstructionStatusChange(module), null, new object[]
			{
				module.sector,
				this,
				module.sector.faction
			});
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x0019F6AC File Offset: 0x0019D8AC
		public void UpdateAllModuleConnectors()
		{
			for (int i = 0; i < this.sectors.Count; i++)
			{
				TISectorState tisectorState = this.sectors[i];
				for (int j = 0; j < tisectorState.habModules.Count; j++)
				{
					tisectorState.habModules[j] = TISectorState.UpdateModuleConnectorMap(this, tisectorState.habModules[j]);
				}
			}
			if (this.IsStation)
			{
				this.ringStruct = this.ActivateRings();
				return;
			}
			this.connStruct = this.ActivateBaseConnections();
		}

		// Token: 0x0600405F RID: 16479 RVA: 0x0019F734 File Offset: 0x0019D934
		public TIHabState.RingStruct ActivateRings()
		{
			TIHabState.RingStruct ringStruct = default(TIHabState.RingStruct);
			if (this.sectors[4].habModules[1].hasModule && this.sectors[1].habModules[3].hasModule)
			{
				ringStruct.NW = true;
			}
			if (this.sectors[4].habModules[3].hasModule && this.sectors[3].habModules[1].hasModule)
			{
				ringStruct.SW = true;
			}
			if (this.sectors[2].habModules[3].hasModule && this.sectors[1].habModules[1].hasModule)
			{
				ringStruct.NE = true;
			}
			if (this.sectors[2].habModules[1].hasModule && this.sectors[3].habModules[3].hasModule)
			{
				ringStruct.SE = true;
			}
			return ringStruct;
		}

		// Token: 0x06004060 RID: 16480 RVA: 0x0019F85C File Offset: 0x0019DA5C
		public TIHabState.BaseConnectionStruct ActivateBaseConnections()
		{
			TIHabState.BaseConnectionStruct baseConnectionStruct = default(TIHabState.BaseConnectionStruct);
			if (this.sectors[0].habModules[1].hasModule)
			{
				baseConnectionStruct.C42 = false;
			}
			if (this.sectors[2].habModules[3].hasModule && this.sectors[4].habModules[2].hasModule)
			{
				baseConnectionStruct.C16 = true;
			}
			if (this.sectors[1].habModules[1].hasModule && this.sectors[3].habModules[2].hasModule)
			{
				baseConnectionStruct.C76 = true;
			}
			if (this.sectors[4].HasAnyModules())
			{
				baseConnectionStruct.C36 = true;
				baseConnectionStruct.C46 = true;
			}
			if (this.sectors[3].HasAnyModules())
			{
				baseConnectionStruct.C56 = true;
				baseConnectionStruct.C46 = true;
			}
			return baseConnectionStruct;
		}

		// Token: 0x06004061 RID: 16481 RVA: 0x0019F968 File Offset: 0x0019DB68
		public List<TIHabModuleState> RebuildCandidates()
		{
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			List<TIHabModuleTemplate> list2 = this.AllowedModules(base.faction);
			foreach (TIHabModuleState tihabModuleState in this.AllModules())
			{
				if (tihabModuleState.destroyed && tihabModuleState.priorModuleTemplate != null && list2.Contains(tihabModuleState.priorModuleTemplate))
				{
					list.Add(tihabModuleState);
				}
			}
			return list;
		}

		// Token: 0x06004062 RID: 16482 RVA: 0x0019F9F0 File Offset: 0x0019DBF0
		public TIResourcesCost FullRebuildCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (TIHabModuleState tihabModuleState in this.RebuildCandidates())
			{
				tiresourcesCost.SumCosts_NoDuration(tihabModuleState.priorModuleTemplate.CostFromSpace(base.faction, this, false, false, 0, false));
			}
			return tiresourcesCost;
		}

		// Token: 0x06004063 RID: 16483 RVA: 0x0019FA60 File Offset: 0x0019DC60
		public List<TIHabModuleState> UpgradeCandidates()
		{
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			if (this.underBombardment || this.underAssault || this.decommissioning)
			{
				return list;
			}
			foreach (TIHabModuleState tihabModuleState in this.AllModules())
			{
				if (tihabModuleState.CanUpgrade(base.faction))
				{
					list.Add(tihabModuleState);
				}
			}
			return list;
		}

		// Token: 0x06004064 RID: 16484 RVA: 0x0019FAE4 File Offset: 0x0019DCE4
		public TIResourcesCost FullUpgradeCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (TIHabModuleState tihabModuleState in this.UpgradeCandidates())
			{
				tiresourcesCost.SumCosts_NoDuration(tihabModuleState.moduleTemplate.UpgradesTo.CostFromSpace(base.faction, this, true, false, 0, false));
			}
			return tiresourcesCost;
		}

		// Token: 0x06004065 RID: 16485 RVA: 0x0019FB58 File Offset: 0x0019DD58
		public List<TIHabModuleState> UpgradeCandidates(TIHabModuleTemplate template)
		{
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			if (this.underAssault || this.underBombardment || this.decommissioning)
			{
				return list;
			}
			IEnumerable<TIHabModuleState> enumerable = this.AllModules();
			Func<TIHabModuleState, bool> <>9__0;
			Func<TIHabModuleState, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TIHabModuleState x) => x.moduleTemplate == template);
			}
			foreach (TIHabModuleState tihabModuleState in enumerable.Where<TIHabModuleState>(func))
			{
				if (tihabModuleState.CanUpgrade(base.faction))
				{
					list.Add(tihabModuleState);
				}
			}
			return list;
		}

		// Token: 0x06004066 RID: 16486 RVA: 0x0019FC08 File Offset: 0x0019DE08
		public TIResourcesCost FullUpgradeCost(TIHabModuleTemplate template, bool allowSubstitutions)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (TIHabModuleState tihabModuleState in this.UpgradeCandidates(template))
			{
				tiresourcesCost.SumCosts_NoDuration(tihabModuleState.moduleTemplate.UpgradesTo.CostFromSpace(base.faction, this, true, allowSubstitutions, 0, false));
			}
			return tiresourcesCost;
		}

		// Token: 0x06004067 RID: 16487 RVA: 0x0019FC80 File Offset: 0x0019DE80
		public TIHabTemplate ConvertToTemplate(TIFactionState faction)
		{
			TIHabTemplate tihabTemplate = new TIHabTemplate();
			tihabTemplate.RenameDataName(new StringBuilder(faction.templateName).Append("HabTemplate").Append(faction.savedHabDesigns).ToString());
			string tryDisplayName = Loc.T("UI.Habs.HabTemplateName", new object[] { this.displayName, this.description });
			if (faction.habDesigns.Any<TIHabTemplate>((TIHabTemplate x) => x.displayName == tryDisplayName))
			{
				tryDisplayName = new StringBuilder(tryDisplayName).Append("-").Append(TITimeState.Now().ToCustomDateString()).ToString();
				if (faction.habDesigns.Any<TIHabTemplate>((TIHabTemplate x) => x.displayName == tryDisplayName))
				{
					tryDisplayName = new StringBuilder(tryDisplayName).Append("-").Append(this.OkayModules().Count).ToString();
				}
				tihabTemplate.SetDisplayName(tryDisplayName);
			}
			else
			{
				tihabTemplate.SetDisplayName(tryDisplayName);
			}
			tihabTemplate.habType = this.habType;
			tihabTemplate.tier = this.coreModule.tier;
			tihabTemplate.alien = this.IsAlien();
			if (this.IsBase)
			{
				tihabTemplate.habSite = this.habSite.templateName;
			}
			else
			{
				tihabTemplate.orbitTemplateName = base.orbitState.templateName;
			}
			tihabTemplate.sectors = new SectorTemplate[5];
			for (int i = 0; i < this.sectors.Count; i++)
			{
				tihabTemplate.sectors[i].habModuleNames = new string[(i == 0) ? 5 : 4];
				for (int j = 0; j < this.sectors[i].habModules.Count; j++)
				{
					TIHabModuleState tihabModuleState = this.sectors[i].habModules[j];
					if (tihabModuleState.okay && tihabModuleState.moduleTemplate.FactionCanBuild(faction))
					{
						tihabTemplate.sectors[i].habModuleNames[j] = this.sectors[i].habModules[j].moduleTemplate.dataName;
					}
					else
					{
						tihabTemplate.sectors[i].habModuleNames[j] = string.Empty;
					}
				}
			}
			string text;
			if (tihabTemplate.IsValid(out text))
			{
				return tihabTemplate;
			}
			return null;
		}

		// Token: 0x06004068 RID: 16488 RVA: 0x0019FEEF File Offset: 0x0019E0EF
		public bool CanApplySavedTemplate(TIHabTemplate newTemplate)
		{
			return newTemplate.habType == this.habType && !this.underBombardment && !this.underAssault && !this.decommissioning;
		}

		// Token: 0x06004069 RID: 16489 RVA: 0x0019FF1C File Offset: 0x0019E11C
		public List<TIHabModuleTemplate> ApplySavedTemplate(TIHabTemplate newTemplate, bool prospectiveOnly, bool replaceExisting, out TIResourcesCost baselineCost, out float netPower, out List<TIHabModuleTemplate> rejectedModules)
		{
			TIHabState.<>c__DisplayClass283_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.replaceExisting = replaceExisting;
			CS$<>8__locals1.prospectiveOnly = prospectiveOnly;
			baselineCost = new TIResourcesCost();
			netPower = (float)this.NetPower(true, false);
			CS$<>8__locals1.returnValue = new List<TIHabModuleTemplate>();
			rejectedModules = new List<TIHabModuleTemplate>();
			List<TIHabState.ModulePlacementOrder> list = new List<TIHabState.ModulePlacementOrder>();
			CS$<>8__locals1.allowedModules = from x in this.AllowedModules(base.faction)
				select x.dataName;
			if (this.CanApplySavedTemplate(newTemplate))
			{
				for (int i = 0; i < newTemplate.sectors.Length; i++)
				{
					TIHabModuleTemplate[] habModules = newTemplate.sectors[i].habModules;
					for (int j = 0; j < habModules.Length; j++)
					{
						if (habModules[j] != null && this.sectors[i].faction == base.faction && !this.<ApplySavedTemplate>g__TryInstallModule|283_1(habModules[j], i, j, ref rejectedModules, ref netPower, ref baselineCost, ref CS$<>8__locals1) && !habModules[j].coreModule)
						{
							list.Add(new TIHabState.ModulePlacementOrder(habModules[j].dataName, i, j));
						}
					}
				}
				if (list.Count > 0)
				{
					CS$<>8__locals1.allowedModules = from x in this.AllowedModules(base.faction)
						select x.dataName;
					foreach (TIHabState.ModulePlacementOrder modulePlacementOrder in list)
					{
						this.<ApplySavedTemplate>g__TryInstallModule|283_1(TemplateManager.Find<TIHabModuleTemplate>(modulePlacementOrder.module, false), modulePlacementOrder.sector, modulePlacementOrder.slot, ref rejectedModules, ref netPower, ref baselineCost, ref CS$<>8__locals1);
					}
				}
			}
			return CS$<>8__locals1.returnValue;
		}

		// Token: 0x0600406A RID: 16490 RVA: 0x001A00FC File Offset: 0x0019E2FC
		public void UpdatePowerAndResourceValues_N(bool turnEverythingOn = false, TIHabModuleState modulePowerJustSet = null)
		{
			this.UpdatePowerManagement(turnEverythingOn, modulePowerJustSet, base.faction.player.isAI);
		}

		// Token: 0x0600406B RID: 16491 RVA: 0x001A0116 File Offset: 0x0019E316
		public float GetAnnualNetResourceIncome(TIFactionState faction, FactionResource resource)
		{
			if (this.netAnnualIncomes.Keys.Contains(faction))
			{
				return this.netAnnualIncomes[faction][resource];
			}
			return 0f;
		}

		// Token: 0x0600406C RID: 16492 RVA: 0x001A0144 File Offset: 0x0019E344
		public void UpdateCurrentAnnualNetResourceIncomes(bool suppressFactionResourcesUpdatedEvent = false)
		{
			foreach (TIFactionState tifactionState in this.ref_factions)
			{
				this.netAnnualIncomes[tifactionState] = this.netAnnualIncomes[tifactionState].ToDictionary<KeyValuePair<FactionResource, float>, FactionResource, float>((KeyValuePair<FactionResource, float> x) => x.Key, (KeyValuePair<FactionResource, float> p) => 0f);
				this.administrationModuleModifier = 1f;
				foreach (TIHabModuleState tihabModuleState in this.ActiveModules())
				{
					if (tihabModuleState.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Efficiency))
					{
						this.administrationModuleModifier *= 1f + tihabModuleState.moduleTemplate.specialRulesValue;
					}
				}
				foreach (FactionResource factionResource in TIResourcesCost.habResources)
				{
					if (TIResourcesCost.unAccumulatableResources.Contains(factionResource))
					{
						this.netAnnualIncomes[tifactionState][factionResource] = this.GetNetCurrentMonthlyIncome(this.coreFaction, factionResource, false, false);
					}
					else
					{
						this.netAnnualIncomes[tifactionState][factionResource] = this.GetNetCurrentMonthlyIncome(this.coreFaction, factionResource, false, false) * 12f;
					}
				}
				if (!suppressFactionResourcesUpdatedEvent)
				{
					tifactionState.SetResourceIncomeDataDirty();
				}
				tifactionState.SetMissionControlUsageDataDirty();
			}
		}

		// Token: 0x17000BCF RID: 3023
		// (get) Token: 0x0600406D RID: 16493 RVA: 0x001A0334 File Offset: 0x0019E534
		public bool MayHaveFluctuatingIncomes
		{
			get
			{
				return base.faction.IsAlienFaction && this == base.faction.primaryHab;
			}
		}

		// Token: 0x0600406E RID: 16494 RVA: 0x001A035C File Offset: 0x0019E55C
		public int FarmCrewDiscount()
		{
			if (!this.anyCoreCompleted)
			{
				return 0;
			}
			return (int)(from x in this.ActiveModules()
				where x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Farm)
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRulesValue);
		}

		// Token: 0x0600406F RID: 16495 RVA: 0x001A03C2 File Offset: 0x0019E5C2
		public float FarmCrewCoveredPct()
		{
			if (this.crew <= 0)
			{
				return 0f;
			}
			return Mathf.Clamp01((float)this.FarmCrewDiscount() / (float)this.crew);
		}

		// Token: 0x06004070 RID: 16496 RVA: 0x001A03E8 File Offset: 0x0019E5E8
		public float GetMonthlySupportCost(FactionResource resource, bool includeInactivesIncomeAndSupport = false)
		{
			float num = 0f;
			float num2 = 0f;
			foreach (TIHabModuleState tihabModuleState in this.OkayModules())
			{
				if ((this.anyCoreCompleted && tihabModuleState.active) || (resource == FactionResource.MissionControl && this.coreModule == tihabModuleState) || includeInactivesIncomeAndSupport)
				{
					num += tihabModuleState.moduleTemplate.MonthlySupportCost(resource, true, base.faction, this);
					if (tihabModuleState.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Farm))
					{
						num2 += tihabModuleState.moduleTemplate.specialRulesValue;
					}
				}
				else
				{
					num += tihabModuleState.moduleTemplate.MonthlyCrewSupportCost(resource, base.faction, this);
				}
			}
			if (resource != FactionResource.Water)
			{
				if (resource == FactionResource.Volatiles)
				{
					num2 = Mathf.Min(num2, (float)this.crew);
					num -= num2 * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
				}
			}
			else
			{
				num2 = Mathf.Min(num2, (float)this.crew);
				num -= num2 * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
			}
			num = Mathf.Max(num, 0f);
			return num;
		}

		// Token: 0x06004071 RID: 16497 RVA: 0x001A0538 File Offset: 0x0019E738
		public float GetNetCurrentMonthlyIncome(TIFactionState faction, FactionResource resource, bool includeInactivesIncomeAndSupport, bool useCache = false)
		{
			if (useCache)
			{
				Dictionary<FactionResource, float> dictionary;
				float num;
				if (this.netAnnualIncomes.TryGetValue(faction, out dictionary) && dictionary.TryGetValue(resource, out num))
				{
					if (resource != FactionResource.Projects && resource != FactionResource.MissionControl)
					{
						num /= 12f;
					}
					return num;
				}
				Log.Error("netAnnualIncomes Cache did not contain the queried data.", Array.Empty<object>());
			}
			decimal num2 = 0m;
			decimal num3 = 0m;
			int num4 = 0;
			if (faction == this.coreFaction)
			{
				foreach (TIHabModuleState tihabModuleState in this.OkayModules())
				{
					if ((this.anyCoreCompleted && tihabModuleState.active) || (resource == FactionResource.MissionControl && this.coreModule == tihabModuleState) || includeInactivesIncomeAndSupport)
					{
						num2 += (decimal)tihabModuleState.moduleTemplate.MonthlyResourceIncome(resource, this, faction);
						num3 += (decimal)tihabModuleState.moduleTemplate.MonthlySupportCost(resource, true, faction, this);
						if (tihabModuleState.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Farm))
						{
							num4 += (int)tihabModuleState.moduleTemplate.specialRulesValue;
						}
						if (tihabModuleState.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.AlienWormhole))
						{
							float num5 = TIGlobalConfig.globalConfig.AI_AliensWormholeSetupFraction();
							num2 *= (decimal)num5;
						}
					}
					else
					{
						decimal num6 = num3;
						TIHabModuleTemplate moduleTemplate = tihabModuleState.moduleTemplate;
						num3 = num6 + (decimal)((moduleTemplate != null) ? moduleTemplate.MonthlyCrewSupportCost(resource, faction, this) : 0f);
					}
				}
			}
			switch (resource)
			{
			case FactionResource.Money:
				num2 *= (decimal)this.AdministrationAdviserMultiplier;
				num2 *= (decimal)this.administrationModuleModifier;
				break;
			case FactionResource.Influence:
			case FactionResource.Operations:
			case FactionResource.Exotics:
				num2 *= (decimal)this.administrationModuleModifier;
				break;
			case FactionResource.Research:
				num2 *= (decimal)this.ScienceAdviserMultiplier;
				num2 *= (decimal)this.administrationModuleModifier;
				break;
			case FactionResource.Water:
				num4 = Mathf.Min(num4, this.crew);
				num2 *= (decimal)this.AdministrationAdviserMultiplier;
				num2 *= (decimal)this.administrationModuleModifier;
				num3 -= (decimal)((float)num4 * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f);
				break;
			case FactionResource.Volatiles:
				num4 = Mathf.Min(num4, this.crew);
				num2 *= (decimal)this.AdministrationAdviserMultiplier;
				num2 *= (decimal)this.administrationModuleModifier;
				num3 -= (decimal)((float)num4 * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f);
				break;
			case FactionResource.Metals:
			case FactionResource.NobleMetals:
			case FactionResource.Fissiles:
				num2 *= (decimal)this.AdministrationAdviserMultiplier;
				num2 *= (decimal)this.administrationModuleModifier;
				break;
			}
			num3 = Math.Max(num3, 0m);
			return (float)(num2 - num3);
		}

		// Token: 0x06004072 RID: 16498 RVA: 0x001A087C File Offset: 0x0019EA7C
		public float GetMonthlyRevenue(FactionResource resource, bool dontRecalculate = false)
		{
			if (TIFrameCounter.FrameCount != this.monthlyRevenueCachedFrame)
			{
				this.cachedMonthlyRevenue.Clear();
				this.monthlyRevenueCachedFrame = TIFrameCounter.FrameCount;
			}
			float num;
			if (!dontRecalculate || !this.cachedMonthlyRevenue.TryGetValue(resource, out num))
			{
				num = (this.cachedMonthlyRevenue[resource] = (from x in this.ActiveModules()
					select x.moduleTemplate.MonthlyResourceRevenue(resource, this, this.faction)).Sum());
			}
			return num;
		}

		// Token: 0x06004073 RID: 16499 RVA: 0x001A0908 File Offset: 0x0019EB08
		public float GetMonthlyRevenue_WithAdviser(FactionResource resource, bool dontRecalculate = false)
		{
			float num = this.GetMonthlyRevenue(resource, dontRecalculate);
			switch (resource)
			{
			case FactionResource.Money:
				num *= this.AdministrationAdviserMultiplier;
				num *= this.administrationModuleModifier;
				break;
			case FactionResource.Influence:
			case FactionResource.Operations:
			case FactionResource.Exotics:
				num *= this.administrationModuleModifier;
				break;
			case FactionResource.Research:
				num *= this.ScienceAdviserMultiplier;
				num *= this.administrationModuleModifier;
				break;
			case FactionResource.Water:
				num *= this.AdministrationAdviserMultiplier;
				num *= this.administrationModuleModifier;
				break;
			case FactionResource.Volatiles:
				num *= this.AdministrationAdviserMultiplier;
				num *= this.administrationModuleModifier;
				break;
			case FactionResource.Metals:
			case FactionResource.NobleMetals:
			case FactionResource.Fissiles:
				num *= this.AdministrationAdviserMultiplier;
				num *= this.administrationModuleModifier;
				break;
			}
			return num;
		}

		// Token: 0x06004074 RID: 16500 RVA: 0x001A09CE File Offset: 0x0019EBCE
		public float GetYearlyRevenue(FactionResource resource, bool dontRecalculate = false)
		{
			if (resource == FactionResource.MissionControl || resource == FactionResource.Projects)
			{
				return this.GetMonthlyRevenue_WithAdviser(resource, dontRecalculate);
			}
			return this.GetMonthlyRevenue_WithAdviser(resource, dontRecalculate) * 12f;
		}

		// Token: 0x17000BD0 RID: 3024
		// (get) Token: 0x06004075 RID: 16501 RVA: 0x001A09EF File Offset: 0x0019EBEF
		public int controlPointCapacityValue
		{
			get
			{
				return this.sectors.Sum<TISectorState>((TISectorState x) => x.controlPointCapacityValue);
			}
		}

		// Token: 0x06004076 RID: 16502 RVA: 0x001A0A1B File Offset: 0x0019EC1B
		public void ResetIcon()
		{
			this._icon = null;
		}

		// Token: 0x06004077 RID: 16503 RVA: 0x001A0A24 File Offset: 0x0019EC24
		public int NetPower(bool includeUnderConstruction, bool includeDeactivated)
		{
			int num = 0;
			foreach (TISectorState tisectorState in this.sectors)
			{
				num += tisectorState.SectorNetPowerValue(includeUnderConstruction, includeDeactivated);
			}
			return num;
		}

		// Token: 0x06004078 RID: 16504 RVA: 0x001A0A80 File Offset: 0x0019EC80
		public bool EnoughPowerForModule(TIHabModuleState module)
		{
			if (module.sector == null)
			{
				Log.Error("Module sectorState is null in EnoughOrganicPowerForModule", Array.Empty<object>());
				return false;
			}
			return this.NetPower(false, false) >= -1 * module.ModulePower();
		}

		// Token: 0x06004079 RID: 16505 RVA: 0x001A0AB8 File Offset: 0x0019ECB8
		private void TurnOnPowerModulesToResolveDeficit(ref int netPower)
		{
			if (netPower >= 0)
			{
				return;
			}
			foreach (TIHabModuleState tihabModuleState in (from x in this.UnpoweredModules()
				where x.PowerProvider()
				select x).ToList<TIHabModuleState>())
			{
				tihabModuleState.SetPowerStatus(true, true);
				netPower += tihabModuleState.ModulePower();
				if (netPower >= 0)
				{
					break;
				}
			}
			this.UpdateCurrentAnnualNetResourceIncomes(false);
		}

		// Token: 0x0600407A RID: 16506 RVA: 0x001A0B54 File Offset: 0x0019ED54
		public void ResetPower()
		{
			foreach (TIHabModuleState tihabModuleState in from x in this.ActiveModules()
				where !x.moduleTemplate.PowerFirst
				where !x.moduleTemplate.coreModule
				where !x.PowerProvider()
				select x)
			{
				tihabModuleState.SetPowerStatus(false, true);
			}
			this.UpdatePowerManagement(true, null, true);
		}

		// Token: 0x0600407B RID: 16507 RVA: 0x001A0C18 File Offset: 0x0019EE18
		public void ValidateLocalPopulationRequirementsForAllNearbyHabs()
		{
			if (!this.location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.isEarth)
			{
				List<TIFactionState> list = new List<TIFactionState>();
				List<TIHabState> list2 = new List<TIHabState>();
				ulong population = this.location.ref_naturalSpaceObject.population;
				bool flag = population >= TemplateManager.global.colonizedSpaceObjectValue;
				bool flag2 = population >= TemplateManager.global.populousSpaceObjectValue;
				if (!flag)
				{
					using (List<TIHabState>.Enumerator enumerator = this.location.ref_naturalSpaceObject.habs.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIHabState tihabState = enumerator.Current;
							if (!tihabState.IsAlien())
							{
								foreach (TIHabModuleState tihabModuleState in tihabState.ActiveModules())
								{
									if (tihabModuleState.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Requires_Inhabited_Body) || tihabModuleState.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Requires_Colonized_Body))
									{
										tihabModuleState.SetPowerStatus(false, true);
										list2.Add(tihabState);
										list.Add(tihabState.faction);
									}
								}
							}
						}
						goto IL_01D0;
					}
				}
				if (!flag2)
				{
					foreach (TIHabState tihabState2 in this.location.ref_naturalSpaceObject.habs)
					{
						if (!tihabState2.IsAlien())
						{
							foreach (TIHabModuleState tihabModuleState2 in tihabState2.ActiveModules())
							{
								if (tihabModuleState2.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.Requires_Inhabited_Body))
								{
									tihabModuleState2.SetPowerStatus(false, true);
									list2.Add(tihabState2);
									list.Add(tihabState2.faction);
								}
							}
						}
					}
				}
				IL_01D0:
				foreach (TIHabState tihabState3 in list2.Distinct<TIHabState>())
				{
					tihabState3.UpdateCurrentAnnualNetResourceIncomes(false);
				}
				using (IEnumerator<TIFactionState> enumerator4 = list.Distinct<TIFactionState>().GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						TIFactionState faction = enumerator4.Current;
						TINotificationQueueState.LogHabModuleForcedOffDueToPopulationChanges(faction, this.location.ref_naturalSpaceObject, list2.Where<TIHabState>((TIHabState x) => x.faction == faction).ToList<TIHabState>());
					}
				}
			}
		}

		// Token: 0x0600407C RID: 16508 RVA: 0x001A0EE0 File Offset: 0x0019F0E0
		public void UpdatePowerManagement(bool turnEverythingPossibleOn = false, TIHabModuleState moduleJustPowerSet = null, bool AI = false)
		{
			TIHabState.<>c__DisplayClass307_0 CS$<>8__locals1 = new TIHabState.<>c__DisplayClass307_0();
			CS$<>8__locals1.turnEverythingPossibleOn = turnEverythingPossibleOn;
			CS$<>8__locals1.moduleJustPowerSet = moduleJustPowerSet;
			CS$<>8__locals1.<>4__this = this;
			int num = this.NetPower(false, false);
			if (CS$<>8__locals1.moduleJustPowerSet != null && CS$<>8__locals1.moduleJustPowerSet.powered && num < 0)
			{
				this.TurnOnPowerModulesToResolveDeficit(ref num);
			}
			CS$<>8__locals1.netPowerIgnoringOptionalModules = num;
			foreach (TIHabModuleState tihabModuleState in from x in this.ActiveModules()
				where x.PowerConsumer() && !x.moduleTemplate.PowerFirst
				select x)
			{
				CS$<>8__locals1.netPowerIgnoringOptionalModules -= tihabModuleState.ModulePower();
			}
			IOrderedEnumerable<TIHabModuleState> orderedEnumerable = (from x in this.AllModules()
				where x.present && !x.underConstruction && !x.powered && !x.decommissioning && x.PowerConsumer() && x.moduleTemplate.PowerFirst
				where -x.ModulePower() <= CS$<>8__locals1.netPowerIgnoringOptionalModules
				select x).OrderByDescending<TIHabModuleState, int>(new Func<TIHabModuleState, int>(CS$<>8__locals1.<UpdatePowerManagement>g__GetPowerPriority|0));
			while (orderedEnumerable.Any<TIHabModuleState>())
			{
				TIHabModuleState tihabModuleState2 = orderedEnumerable.First<TIHabModuleState>();
				int num2 = tihabModuleState2.ModulePower();
				List<TIHabModuleState> list = (from x in this.ActiveModules()
					where x.PowerConsumer() && !x.moduleTemplate.PowerFirst
					select x).OrderBy<TIHabModuleState, int>(new Func<TIHabModuleState, int>(CS$<>8__locals1.<UpdatePowerManagement>g__GetPowerPriority|0)).ToList<TIHabModuleState>();
				while (-num2 > num)
				{
					TIHabModuleState tihabModuleState3 = list.First<TIHabModuleState>();
					list.Remove(tihabModuleState3);
					tihabModuleState3.SetPowerStatus(false, true);
					num -= tihabModuleState3.ModulePower();
				}
				tihabModuleState2.SetPowerStatus(true, true);
				num += num2;
				CS$<>8__locals1.netPowerIgnoringOptionalModules += num2;
			}
			List<TIHabModuleState> list2 = (from x in this.UnpoweredModules()
				where CS$<>8__locals1.turnEverythingPossibleOn || x.moduleTemplate.PowerFirst
				select x).ToList<TIHabModuleState>();
			if (CS$<>8__locals1.moduleJustPowerSet != null && !CS$<>8__locals1.moduleJustPowerSet.powered && !CS$<>8__locals1.moduleJustPowerSet.moduleTemplate.PowerFirst)
			{
				list2 = list2.Where<TIHabModuleState>((TIHabModuleState x) => x != CS$<>8__locals1.moduleJustPowerSet).ToList<TIHabModuleState>();
			}
			int num3 = num + list2.Sum<TIHabModuleState>((TIHabModuleState x) => x.ModulePower());
			if (num3 < 0)
			{
				this.TurnOnPowerModulesToResolveDeficit(ref num3);
				num = this.NetPower(false, false);
			}
			foreach (TIHabModuleState tihabModuleState4 in list2.OrderByDescending<TIHabModuleState, int>(new Func<TIHabModuleState, int>(CS$<>8__locals1.<UpdatePowerManagement>g__GetPowerPriority|0)).ToList<TIHabModuleState>())
			{
				if (!tihabModuleState4.powered && (!tihabModuleState4.PowerConsumer() || tihabModuleState4.PowerConsumed() <= num))
				{
					tihabModuleState4.SetPowerStatus(true, true);
					num += tihabModuleState4.ModulePower();
				}
			}
			if (num < 0)
			{
				Queue<TIHabModuleState> queue = new Queue<TIHabModuleState>((from x in this.ActiveModules()
					where x.PowerConsumer()
					select x).OrderBy<TIHabModuleState, int>(new Func<TIHabModuleState, int>(CS$<>8__locals1.<UpdatePowerManagement>g__GetPowerPriority|0)));
				while (num < 0 && queue.Any<TIHabModuleState>())
				{
					TIHabModuleState tihabModuleState5 = queue.Dequeue();
					num += tihabModuleState5.PowerConsumed();
					tihabModuleState5.SetPowerStatus(false, true);
				}
			}
			this.UpdateCurrentAnnualNetResourceIncomes(false);
			base.faction.SetMissionControlUsageDataDirty();
			base.faction.SetResourceIncomeDataDirty();
			GameControl.eventManager.TriggerEvent(new HabPowerManagementUpdated(this), null, new object[] { this });
			if (this.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(CS$<>8__locals1.<>4__this.faction)))
			{
				bool flag = this.ActiveModules().Any<TIHabModuleState>((TIHabModuleState x) => x.isCombatModule);
				if (GameControl.loadcycle100 && flag && this.habType == HabType.Station)
				{
					this.dockedFleets.First<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(CS$<>8__locals1.<>4__this.faction)).InitiateCombat(this.dockedFleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(CS$<>8__locals1.<>4__this.faction)), this, false);
					return;
				}
				if (flag || this.AssaultCombatValue(true) > 0f)
				{
					foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets)
					{
						if (!tispaceFleetState.faction.permanentAlly(base.faction))
						{
							if (tispaceFleetState.IsResupplying())
							{
								foreach (TISpaceShipState tispaceShipState in tispaceFleetState.ships)
								{
									tispaceShipState.plannedResupplyAndRepair.CancelResupply(tispaceFleetState.faction);
								}
							}
							if (tispaceFleetState.IsRepairing())
							{
								foreach (TISpaceShipState tispaceShipState2 in tispaceFleetState.ships)
								{
									tispaceShipState2.plannedResupplyAndRepair.CancelRepair(tispaceFleetState.faction);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600407D RID: 16509 RVA: 0x001A1424 File Offset: 0x0019F624
		public List<HabModuleSpecialRule> HabConstructHabOptions(TIFactionState faction, bool includeInactives = false, bool includeUnderConstruction = false)
		{
			List<HabModuleSpecialRule> list = new List<HabModuleSpecialRule>();
			List<TIHabModuleState> list2 = new List<TIHabModuleState>();
			if (faction == this.coreFaction)
			{
				if (includeUnderConstruction)
				{
					list2 = (from x in this.AllModules()
						where x.okay
						select x).ToList<TIHabModuleState>();
				}
				else if (includeInactives)
				{
					list2 = this.FunctionalModules();
				}
				else
				{
					list2 = this.ActiveModules();
				}
			}
			foreach (TIHabModuleTemplate tihabModuleTemplate in list2.Select<TIHabModuleState, TIHabModuleTemplate>((TIHabModuleState x) => x.moduleTemplate))
			{
				if (!list.Contains(HabModuleSpecialRule.CanFoundTier1Habs) && tihabModuleTemplate.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier1Habs))
				{
					list.Add(HabModuleSpecialRule.CanFoundTier1Habs);
				}
				if (!list.Contains(HabModuleSpecialRule.CanFoundTier2Habs) && tihabModuleTemplate.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier2Habs))
				{
					list.Add(HabModuleSpecialRule.CanFoundTier2Habs);
				}
				if (!list.Contains(HabModuleSpecialRule.CanFoundTier3Habs) && tihabModuleTemplate.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier3Habs))
				{
					list.Add(HabModuleSpecialRule.CanFoundTier3Habs);
				}
			}
			return list.Distinct<HabModuleSpecialRule>().ToList<HabModuleSpecialRule>();
		}

		// Token: 0x0600407E RID: 16510 RVA: 0x001A1558 File Offset: 0x0019F758
		public float MarineModuleCombatValue()
		{
			if (!this.decommissioning)
			{
				float num = 0f;
				foreach (TIHabModuleState tihabModuleState in this.FunctionalModules())
				{
					foreach (HabModuleSpecialRule habModuleSpecialRule in tihabModuleState.moduleTemplate.SpecialRules)
					{
						if (habModuleSpecialRule - HabModuleSpecialRule.MarinePlatoon <= 5)
						{
							num += tihabModuleState.moduleTemplate.specialRulesValue;
						}
					}
				}
				return num;
			}
			return 0f;
		}

		// Token: 0x0600407F RID: 16511 RVA: 0x001A1614 File Offset: 0x0019F814
		public override float AssaultCombatValue(bool defense)
		{
			if (!this.decommissioning)
			{
				float num = (float)((this.IsBase && defense) ? this.coreModule.tier : 0);
				foreach (TIHabModuleState tihabModuleState in this.ActiveModules())
				{
					foreach (HabModuleSpecialRule habModuleSpecialRule in tihabModuleState.moduleTemplate.SpecialRules)
					{
						if (habModuleSpecialRule - HabModuleSpecialRule.MarinePlatoon <= 5)
						{
							num += tihabModuleState.moduleTemplate.specialRulesValue;
						}
					}
				}
				if (num > 0f)
				{
					num *= this.CommandAdviserMultiplier;
				}
				num += TIEffectsState.SumEffectsModifiers(Context.SpaceAssaultBonus, base.faction, num, null);
				return num;
			}
			return 0f;
		}

		// Token: 0x06004080 RID: 16512 RVA: 0x001A170C File Offset: 0x0019F90C
		public float ModifiedDefenseCombatValue(bool againstAirAssault)
		{
			float num = 0f;
			num = this.AssaultCombatValue(true);
			num -= (float)base.faction.MissionControlShortage;
			num -= base.faction.DailyHabBoostShortage();
			if (base.faction.Insolvent)
			{
				num += base.faction.GetMonthlyIncome(FactionResource.Money, false, false) / 10f;
			}
			foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets)
			{
				if (tispaceFleetState.faction == base.faction)
				{
					num += tispaceFleetState.AssaultCombatValue(true);
				}
			}
			if (!this.decommissioning)
			{
				if (againstAirAssault && this.IsBase)
				{
					num += (float)this.ActiveCombatModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier);
				}
				num += TIMissionModifier.CouncilCollectiveDefense(base.faction, CouncilorAttribute.Command);
			}
			num += this.GetProtectionBonus(CouncilorAttribute.Command);
			return num;
		}

		// Token: 0x06004081 RID: 16513 RVA: 0x001A1820 File Offset: 0x0019FA20
		public void ArriveCouncilor(TICouncilorState councilor)
		{
			if (!this.councilorsOnBoard.Contains(councilor))
			{
				this.councilorsOnBoard.Add(councilor);
			}
		}

		// Token: 0x06004082 RID: 16514 RVA: 0x001A183C File Offset: 0x0019FA3C
		public void DepartCouncilor(TICouncilorState councilor)
		{
			this.councilorsOnBoard.Remove(councilor);
		}

		// Token: 0x06004083 RID: 16515 RVA: 0x001A184C File Offset: 0x0019FA4C
		public List<TICouncilorState> councilorsPresent(TIFactionState limitToFaction = null)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.councilorsOnBoard.Where<TICouncilorState>((TICouncilorState x) => x.faction != null))
			{
				if (limitToFaction == null || ticouncilorState.faction == limitToFaction)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x06004084 RID: 16516 RVA: 0x001A18DC File Offset: 0x0019FADC
		public List<TICouncilorState> CouncilorsPresentAndKnownToFaction(TIFactionState faction, bool skipOurFaction = false, TIFactionState limitOutputToFaction = null)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.councilorsPresent(limitOutputToFaction))
			{
				if (faction.HasIntelOnCouncilorLocation(ticouncilorState) && (!skipOurFaction || faction != ticouncilorState.faction))
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x06004085 RID: 16517 RVA: 0x001A1954 File Offset: 0x0019FB54
		public void AddAdvisingCouncilor(TICouncilorState councilor)
		{
			this.advisingCouncilors.Add(councilor);
			this.UpdateCurrentAnnualNetResourceIncomes(false);
		}

		// Token: 0x06004086 RID: 16518 RVA: 0x001A1969 File Offset: 0x0019FB69
		public void RemoveAdvisingCouncilor(TICouncilorState councilor)
		{
			this.advisingCouncilors.Remove(councilor);
			this.UpdateCurrentAnnualNetResourceIncomes(false);
		}

		// Token: 0x06004087 RID: 16519 RVA: 0x001A197F File Offset: 0x0019FB7F
		public void ClearAdvisingCouncilors()
		{
			if (this.advisingCouncilors.Count == 0)
			{
				return;
			}
			this.advisingCouncilors.Clear();
			this.UpdateCurrentAnnualNetResourceIncomes(false);
		}

		// Token: 0x06004088 RID: 16520 RVA: 0x001A19A4 File Offset: 0x0019FBA4
		public float GetAdvisingAttribute(CouncilorAttribute attribute)
		{
			float num = 0f;
			List<TICouncilorState> advisingCouncilors = this.advisingCouncilors;
			if (advisingCouncilors != null && advisingCouncilors.Count > 0)
			{
				TICouncilorState[] array = (from x in this.advisingCouncilors
					where x.active
					orderby x.GetAttribute(attribute, true, true, true, false, false, false) descending
					select x).ToArray<TICouncilorState>();
				for (int i = 0; i < array.Length; i++)
				{
					num += array[i].AdvisingBonus(attribute) / (float)(i + 1);
				}
				return num;
			}
			return 0f;
		}

		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x06004089 RID: 16521 RVA: 0x001A1A47 File Offset: 0x0019FC47
		public float AdministrationAdviserMultiplier
		{
			get
			{
				return 1f + this.GetAdvisingAttribute(CouncilorAttribute.Administration);
			}
		}

		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x0600408A RID: 16522 RVA: 0x001A1A56 File Offset: 0x0019FC56
		public float CommandAdviserMultiplier
		{
			get
			{
				return 1f + this.GetAdvisingAttribute(CouncilorAttribute.Command);
			}
		}

		// Token: 0x17000BD3 RID: 3027
		// (get) Token: 0x0600408B RID: 16523 RVA: 0x001A1A65 File Offset: 0x0019FC65
		public float ScienceAdviserMultiplier
		{
			get
			{
				return 1f + this.GetAdvisingAttribute(CouncilorAttribute.Science);
			}
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x001A1A74 File Offset: 0x0019FC74
		public void AddDockedFleet(TISpaceFleetState fleet)
		{
			if (!this.dockedFleets.Contains(fleet))
			{
				this.dockedFleets.Add(fleet);
			}
		}

		// Token: 0x0600408D RID: 16525 RVA: 0x001A1A90 File Offset: 0x0019FC90
		public bool RemoveDockedFleet(TISpaceFleetState fleet)
		{
			if (this.dockedFleets.Remove(fleet))
			{
				if (this.IsStation)
				{
					this._dockedShipAbovePositions.RemoveItem(fleet);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600408E RID: 16526 RVA: 0x001A1AB7 File Offset: 0x0019FCB7
		public bool CanDefendHabWithSTOFighters()
		{
			return this.inEarthLEO && base.faction.CanLaunchSTOFighters && base.faction.GetCurrentResourceAmount(FactionResource.Boost) >= base.faction.cachedSTOFighterMinimumBoost;
		}

		// Token: 0x0600408F RID: 16527 RVA: 0x001A1AEC File Offset: 0x0019FCEC
		public bool DockingRequiresCombat(TISpaceFleetState fleet, bool checkForSTODefenses)
		{
			if (!fleet.faction.permanentAlly(base.faction))
			{
				if (this.SpaceCombatValue() > 0f)
				{
					return true;
				}
				if (checkForSTODefenses && this.CanDefendHabWithSTOFighters())
				{
					return true;
				}
			}
			return this.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !fleet.faction.permanentAlly(x.faction));
		}

		// Token: 0x06004090 RID: 16528 RVA: 0x001A1B51 File Offset: 0x0019FD51
		public bool CanDock(TISpaceFleetState fleet, bool checkForSTODefenses)
		{
			return !this.DockingRequiresCombat(fleet, checkForSTODefenses);
		}

		// Token: 0x06004091 RID: 16529 RVA: 0x001A1B5E File Offset: 0x0019FD5E
		public void DockFleet(TISpaceFleetState fleet, out Vector3d offset)
		{
			this.AddDockedFleet(fleet);
			if (this.IsBase)
			{
				this.habSite.LandFleet(fleet);
			}
			this._dockedShipAbovePositions.TryAddItem(fleet, out offset);
		}

		// Token: 0x06004092 RID: 16530 RVA: 0x001A1B89 File Offset: 0x0019FD89
		public void LaunchFleet(TISpaceFleetState fleet)
		{
			this.RemoveDockedFleet(fleet);
			if (this.IsBase)
			{
				this.habSite.LaunchFleet(fleet);
			}
		}

		// Token: 0x06004093 RID: 16531 RVA: 0x001A1BA7 File Offset: 0x0019FDA7
		public float SpaceCombatValueFromDockedFleets()
		{
			return this.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(base.faction)).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		}

		// Token: 0x06004094 RID: 16532 RVA: 0x001A1BE4 File Offset: 0x0019FDE4
		public float SpaceCombatValueFromDefendingFleets()
		{
			return TIFactionState.GetDefenders(this).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		}

		// Token: 0x06004095 RID: 16533 RVA: 0x001A1C10 File Offset: 0x0019FE10
		public float AssaultCombatValueFromDockedFleets(TIFactionState fleetFaction, bool defense)
		{
			float num = 0f;
			foreach (TISpaceFleetState tispaceFleetState in this.dockedFleets)
			{
				if (fleetFaction == tispaceFleetState.faction)
				{
					num += tispaceFleetState.AssaultCombatValue(defense);
				}
			}
			return num;
		}

		// Token: 0x06004096 RID: 16534 RVA: 0x001A1C7C File Offset: 0x0019FE7C
		public void TakeDamageFromParticipatingInAssault_Offense(TIMissionOutcome outcome, TIFactionState defender)
		{
			float num = 0f;
			if (outcome != TIMissionOutcome.CriticalFailure)
			{
				if (outcome == TIMissionOutcome.Failure)
				{
					num = 0.1f;
				}
			}
			else
			{
				num = 0.5f;
			}
			if (num > 0f)
			{
				foreach (TIHabModuleState tihabModuleState in (from x in this.ActiveModules()
					where x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops)
					select x).ToList<TIHabModuleState>())
				{
					if (TIUtilities.RandomFloatValue() < num / (float)tihabModuleState.tier)
					{
						this.DestroyModule(defender, tihabModuleState, false, true, false, 0f, false, false);
					}
				}
			}
		}

		// Token: 0x06004097 RID: 16535 RVA: 0x001A1D38 File Offset: 0x0019FF38
		public void TakeDamageFromParticipatingInAssault_Defense(TIMissionOutcome outcome, TIFactionState attacker)
		{
			float num = 0f;
			if (outcome == TIMissionOutcome.Failure && (base.faction.MissionControlShortage > 0 || base.faction.GetCurrentResourceAmount(FactionResource.Money) < 0f))
			{
				num = 0.1f;
			}
			if (num > 0f)
			{
				foreach (TIHabModuleState tihabModuleState in (from x in this.ActiveModules()
					where x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.DropTroops)
					select x).ToList<TIHabModuleState>())
				{
					if (TIUtilities.RandomFloatValue() < num)
					{
						this.DestroyModule(attacker, tihabModuleState, false, true, false, 0f, false, false);
					}
				}
			}
		}

		// Token: 0x06004098 RID: 16536 RVA: 0x001A1E04 File Offset: 0x001A0004
		public override List<TISpaceFleetState> GetNearbyIdleAlliedFleets(TIDateTime time = null)
		{
			TIHabState.<>c__DisplayClass338_0 CS$<>8__locals1 = new TIHabState.<>c__DisplayClass338_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.time = time;
			if (CS$<>8__locals1.time == null)
			{
				CS$<>8__locals1.time = TITimeState.Now();
			}
			IEnumerable<TISpaceFleetState> enumerable = from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
				where x != CS$<>8__locals1.<>4__this && x.faction == CS$<>8__locals1.<>4__this.faction && (!x.transferAssigned || x.trajectory.launchTime > TITimeState.Now())
				select x;
			if (this.IsBase)
			{
				return enumerable.Where<TISpaceFleetState>((TISpaceFleetState x) => x.dockedOrLanded && CS$<>8__locals1.<>4__this == x.dockedLocation).ToList<TISpaceFleetState>();
			}
			TINaturalSpaceObjectState ourBarycenter;
			OrbitalElementsState ourOrbitElements;
			bool flag;
			this.getOrbitalElementsState(TITimeState.Now(), out ourOrbitElements, out ourBarycenter, out flag);
			Vector3d ourLocalPosition = this.ToLocalCartesianStateAtTime(CS$<>8__locals1.time).position;
			return enumerable.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				if (x.dockedOrLanded)
				{
					return x.dockedLocation == x;
				}
				bool flag2 = CS$<>8__locals1.<>4__this.orbitState == x.orbitState;
				if (!flag2)
				{
					OrbitalElementsState orbitalElementsState;
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					bool flag3;
					x.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState, out tinaturalSpaceObjectState, out flag3);
					if (ourBarycenter == tinaturalSpaceObjectState && ourOrbitElements.Approximately(orbitalElementsState, 0.0))
					{
						Vector3d position = x.ToLocalCartesianStateAtTime(CS$<>8__locals1.time).position;
						if ((ourLocalPosition - position).magnitude < 5000.0)
						{
							flag2 = true;
						}
					}
				}
				return flag2;
			}).ToList<TISpaceFleetState>();
		}

		// Token: 0x06004099 RID: 16537 RVA: 0x001A1ECF File Offset: 0x001A00CF
		public void AddConflictFleet(TISpaceFleetState fleet)
		{
			this.conflictFleets.Add(fleet);
		}

		// Token: 0x0600409A RID: 16538 RVA: 0x001A1EDE File Offset: 0x001A00DE
		public bool IsConflictFleet(TISpaceFleetState fleet)
		{
			return this.conflictFleets.Contains(fleet);
		}

		// Token: 0x0600409B RID: 16539 RVA: 0x001A1EEC File Offset: 0x001A00EC
		public bool IsThreateningFleet(TISpaceFleetState fleet)
		{
			return TIGameState.Valid(fleet) && TIGameState.Valid(this) && !fleet.faction.permanentAlly(base.faction) && !(fleet.ref_system != base.ref_system) && (this.IsConflictFleet(fleet) || base.faction.enemyWarFactions.Contains(fleet.faction));
		}

		// Token: 0x0600409C RID: 16540 RVA: 0x001A1F56 File Offset: 0x001A0156
		public bool CanStoreOfficer(bool swap, int additionalProposedTransfersToHab)
		{
			return this.officersOnBoard.Count + additionalProposedTransfersToHab - (swap ? 1 : 0) < this.MaxOfficerStorageAllowed();
		}

		// Token: 0x0600409D RID: 16541 RVA: 0x001A1F75 File Offset: 0x001A0175
		public int MaxOfficerStorageAllowed()
		{
			return Mathf.RoundToInt((float)this.crew * 0.1f);
		}

		// Token: 0x0600409E RID: 16542 RVA: 0x001A1F89 File Offset: 0x001A0189
		public TIGameState GetState()
		{
			return this;
		}

		// Token: 0x0600409F RID: 16543 RVA: 0x001A1F8C File Offset: 0x001A018C
		public List<TIOfficerState> GetOfficers()
		{
			return this.officersOnBoard;
		}

		// Token: 0x060040A0 RID: 16544 RVA: 0x001A1F94 File Offset: 0x001A0194
		public void UpdateDefendHabStatus()
		{
			if (this.coreDefended && TITimeState.Now() > this.coreDefendExpiration)
			{
				this.ExpireDefense(true);
			}
		}

		// Token: 0x060040A1 RID: 16545 RVA: 0x001A1FB8 File Offset: 0x001A01B8
		public string ResolveDefendHabEffect(TIFactionState faction, int duration_months)
		{
			this.UpdateDefendHabStatus();
			TIDateTime tidateTime;
			if (this.coreDefended)
			{
				tidateTime = this.coreDefendExpiration;
			}
			else
			{
				tidateTime = TITimeState.Now();
			}
			tidateTime.AddMonths(duration_months);
			this.coreDefended = true;
			this.SetDefenseExpiry(tidateTime);
			GameControl.eventManager.TriggerEvent(new HabDefendInterestsUpdated(this), null, new object[] { this });
			return this.coreDefendExpiration.ToCustomDateString();
		}

		// Token: 0x060040A2 RID: 16546 RVA: 0x001A201D File Offset: 0x001A021D
		public void ExpireDefense(bool notify)
		{
			this.coreDefended = false;
			GameControl.eventManager.TriggerEvent(new HabDefendInterestsUpdated(this), null, new object[] { this });
			this.coreDefendExpiration = null;
			if (notify)
			{
				TINotificationQueueState.LogHabDefendInterestEnds(this);
			}
		}

		// Token: 0x060040A3 RID: 16547 RVA: 0x001A2051 File Offset: 0x001A0251
		public void SetDefenseExpiry(TIDateTime expiry)
		{
			this.coreDefendExpiration = TIControlPoint.FindMissionPhaseAfter(expiry);
			this.coreDefendExpiration.AddSeconds(-60.0);
		}

		// Token: 0x060040A4 RID: 16548 RVA: 0x001A2074 File Offset: 0x001A0274
		public List<TICouncilorState> GetProtectors()
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors))
			{
				if (ticouncilorState.active && ticouncilorState.location == this && ticouncilorState.protectingTarget == this)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x060040A5 RID: 16549 RVA: 0x001A2110 File Offset: 0x001A0310
		public float GetProtectionBonus(CouncilorAttribute attribute)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.GetProtectors())
			{
				num += (float)ticouncilorState.GetAttribute(attribute, true, true, true, false, false, false);
			}
			return num;
		}

		// Token: 0x060040A6 RID: 16550 RVA: 0x001A2174 File Offset: 0x001A0374
		public float GetLEOLabBonus(HabModuleSpecialRule rule, bool includeNonActive = false)
		{
			if (this.IsStation && base.orbitState.isEarthLEO)
			{
				float num;
				if (includeNonActive)
				{
					num = (from x in this.OkayModules()
						where x.moduleTemplate.specialRules.Contains(rule)
						select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRulesValue);
				}
				else
				{
					num = (from x in this.ActiveModules()
						where x.moduleTemplate.specialRules.Contains(rule)
						select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRulesValue);
				}
				switch (rule)
				{
				case HabModuleSpecialRule.LEOBonusArmyCombatValue:
				case HabModuleSpecialRule.LEOBonusPropagandaStrength:
				case HabModuleSpecialRule.LEOBonusMiltech:
				case HabModuleSpecialRule.LEOBonusWelfare:
				case HabModuleSpecialRule.LEOBonusLaunchFacilities:
				case HabModuleSpecialRule.LEOBonusKnowledge:
				case HabModuleSpecialRule.LEOBonusMissionControl:
				case HabModuleSpecialRule.LEOBonusEconomy:
				case HabModuleSpecialRule.LEOBonusUnity:
				case HabModuleSpecialRule.LEOBonusOppression:
				case HabModuleSpecialRule.LEOBonusEnvironment:
				case HabModuleSpecialRule.LEOBonusGovernment:
					return num;
				case HabModuleSpecialRule.LEOBonusAlienDetection:
				case HabModuleSpecialRule.LEOBonusHumanDetection:
					return (float)this.tier;
				}
			}
			return 0f;
		}

		// Token: 0x060040A7 RID: 16551 RVA: 0x001A228C File Offset: 0x001A048C
		public void SetUnderAssault(TIGameState assaulter, bool setting, bool alert)
		{
			bool underAssault = this.underAssault;
			this.underAssault = setting;
			if (underAssault != this.underAssault)
			{
				if (this.underAssault)
				{
					if (alert && assaulter.ref_faction != null)
					{
						TINotificationQueueState.LogMyHabAssaultInitiated(this, assaulter.ref_faction);
					}
					GameControl.eventManager.TriggerEvent(new BeginHabAssault(assaulter, this), null, new object[] { assaulter, this, this.ref_naturalSpaceObject });
					return;
				}
				GameControl.eventManager.TriggerEvent(new EndHabAssault(assaulter, this), null, new object[] { assaulter, this, this.ref_naturalSpaceObject });
			}
		}

		// Token: 0x060040A8 RID: 16552 RVA: 0x001A2328 File Offset: 0x001A0528
		public void SetUnderBombardment()
		{
			if (!this.underBombardment)
			{
				List<TIHabModuleState> list = this.ActiveCombatModules();
				if (list.Count > 0)
				{
					foreach (TIHabModuleState tihabModuleState in list)
					{
						tihabModuleState.InitializeForBombardment();
					}
				}
				foreach (TIHabModuleState tihabModuleState2 in this.FunctionalModules())
				{
					tihabModuleState2.ResetBombadardmentArmor();
				}
			}
			this.underBombardment = true;
		}

		// Token: 0x060040A9 RID: 16553 RVA: 0x001A23D4 File Offset: 0x001A05D4
		public bool CheckLOSToOrbitalTarget(TISpaceFleetState fleet, TIDateTime time)
		{
			if (this.cachedLOSCheckTime != time || !this.enemyFleetInLineOfSight.ContainsKey(fleet))
			{
				this.enemyFleetInLineOfSight[fleet] = TISpaceShipState.BombardmentTargetInLineOfSight(fleet.ships[0], this, time);
				this.cachedLOSCheckTime = new TIDateTime(time);
			}
			return this.enemyFleetInLineOfSight[fleet];
		}

		// Token: 0x060040AA RID: 16554 RVA: 0x001A2434 File Offset: 0x001A0634
		public void TryClearBombardmentStatus(TISpaceFleetState endingFleet)
		{
			List<TISpaceFleetState> fleetsInInterfaceOrbits = this.ref_spaceBody.fleetsInInterfaceOrbits;
			fleetsInInterfaceOrbits.Remove(endingFleet);
			this.underBombardment = fleetsInInterfaceOrbits.Any<TISpaceFleetState>((TISpaceFleetState x) => TIGameState.Valid(x) && x.bombardmentTarget != null && x.bombardmentTarget.ref_hab == this);
			if (!this.underBombardment)
			{
				this.enemyFleetInLineOfSight.Clear();
			}
		}

		// Token: 0x060040AB RID: 16555 RVA: 0x001A2480 File Offset: 0x001A0680
		public string GetLocalizedHabModuleList()
		{
			StringBuilder stringBuilder = new StringBuilder(this.displayName).AppendLine();
			stringBuilder.AppendLine();
			foreach (TIHabModuleState tihabModuleState in this.AllModules())
			{
				stringBuilder.AppendLine(tihabModuleState.displayName);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060040AC RID: 16556 RVA: 0x001A24F8 File Offset: 0x001A06F8
		public string BuildShortHabSummary(TIFactionState viewingFaction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.IsAlien())
			{
				if (this.crew > 0)
				{
					stringBuilder.Append(TemplateManager.global.populationInlineSpritePath).Append(this.crew.ToString()).Append(" ");
				}
				foreach (FactionResource factionResource in Enums.FactionResources)
				{
					float netCurrentMonthlyIncome = this.GetNetCurrentMonthlyIncome(this.coreFaction, factionResource, false, false);
					if (netCurrentMonthlyIncome != 0f)
					{
						stringBuilder.Append(TIUtilities.InlineResourceStr(factionResource)).Append(TIUtilities.FormatSmallNumber(netCurrentMonthlyIncome, 7, 0, true, false)).Append(" ");
					}
				}
				foreach (TechCategory techCategory in Enums.TechCategories)
				{
					float netTechBonusByFaction = this.GetNetTechBonusByFaction(techCategory, this.coreFaction, false);
					if (netTechBonusByFaction != 0f)
					{
						stringBuilder.Append(TIGenericTechTemplate.categoryInlineSprite(techCategory)).Append(netTechBonusByFaction.ToPercent("P0")).Append(" ");
					}
				}
			}
			if (this.AllowsResupply(this.coreFaction, false, false))
			{
				stringBuilder.Append(TemplateManager.global.habResupplyPresentInlineSpritePath);
			}
			if (this.AllowsShipConstruction(this.coreFaction, false, false))
			{
				stringBuilder.Append(TemplateManager.global.habShipyardPresentInlineSpritePath);
			}
			if (this.GetModuleConstructionTimeModifier(false, null) < 1f)
			{
				stringBuilder.Append(TemplateManager.global.habModuleConstructionInlineSpritePath);
			}
			float num = this.SpaceCombatValue();
			if (num > 0f)
			{
				stringBuilder.Append(TemplateManager.global.habDefenseScoreInlineSpritePath).Append(num.ToString("N0"));
			}
			float num2 = this.AssaultCombatValue(true);
			if (num2 > 0f)
			{
				stringBuilder.Append(TemplateManager.global.spaceAssaultValueInlineSpritePath).Append(num2.ToString("N0"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060040BF RID: 16575 RVA: 0x001A2888 File Offset: 0x001A0A88
		[CompilerGenerated]
		private bool <ApplySavedTemplate>g__TryInstallModule|283_1(TIHabModuleTemplate proposedTemplate, int k, int i, ref List<TIHabModuleTemplate> rejectedModules, ref float netPower, ref TIResourcesCost baselineCost, ref TIHabState.<>c__DisplayClass283_0 A_7)
		{
			if (A_7.allowedModules.Contains(proposedTemplate.dataName) && this.sectors[k].ValidModuleForSlot(proposedTemplate, i))
			{
				bool flag = false;
				bool flag2 = false;
				if (this.sectors[k].habModules[i].empty || this.sectors[k].habModules[i].destroyed || this.sectors[k].habModules[i].decommissioning)
				{
					flag = true;
				}
				else if (this.sectors[k].habModules[i].CanUpgrade(base.faction) && proposedTemplate.UpgradesFrom == this.sectors[k].habModules[i].moduleTemplate)
				{
					flag2 = true;
				}
				if ((flag || flag2 || (A_7.replaceExisting && this.sectors[k].habModules[i].moduleTemplate.dataName != proposedTemplate.dataName)) && (!proposedTemplate.powerSource || proposedTemplate.IsNonSolarPower || proposedTemplate.ProspectivePower(this) > 0))
				{
					A_7.returnValue.Add(proposedTemplate);
					netPower += (float)proposedTemplate.ProspectivePower(this);
					if ((flag2 && this.sectors[k].habModules[i].powered) || this.sectors[k].habModules[i].underConstruction)
					{
						netPower -= (float)this.sectors[k].habModules[i].ModulePower();
					}
					TIResourcesCost tiresourcesCost = proposedTemplate.CostFromSpace(base.faction, this, flag2, false, 0, false);
					baselineCost.SumCosts_NoDuration(tiresourcesCost);
					TIResourcesCost tiresourcesCost2;
					if (base.faction.UnlockedSpaceResources && base.faction.HasAnySpaceResources)
					{
						tiresourcesCost2 = new TIResourcesCost(tiresourcesCost);
						if (!tiresourcesCost2.CanAfford(base.faction, 1f, null, float.PositiveInfinity))
						{
							tiresourcesCost2 = proposedTemplate.CostFromSpace(base.faction, this, flag2, true, 0, false);
						}
					}
					else
					{
						tiresourcesCost2 = proposedTemplate.CostFromEarth(base.faction, this, flag2);
					}
					if (!A_7.prospectiveOnly && tiresourcesCost2.CanAfford(base.faction, 1f, null, float.PositiveInfinity))
					{
						base.faction.playerControl.StartAction(new BuildHabModuleAction(proposedTemplate, this.sectors[k], i, tiresourcesCost2, delegate
						{
						}));
					}
				}
				else
				{
					rejectedModules.Add(proposedTemplate);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0400275A RID: 10074
		public const int maxSectors = 5;

		// Token: 0x0400275B RID: 10075
		public const int maxSectorIdx = 4;

		// Token: 0x0400275C RID: 10076
		public List<TISectorState> sectors;

		// Token: 0x0400275D RID: 10077
		public List<TIHabDistrictState> districts;

		// Token: 0x0400275F RID: 10079
		public TIHabSiteState habSite;

		// Token: 0x04002761 RID: 10081
		public List<TICouncilorState> councilorsOnBoard;

		// Token: 0x04002762 RID: 10082
		public List<TIOfficerState> officersOnBoard;

		// Token: 0x04002765 RID: 10085
		public string customHabIconResource = "";

		// Token: 0x04002768 RID: 10088
		public bool anyCoreCompleted;

		// Token: 0x04002769 RID: 10089
		public bool coreDefended;

		// Token: 0x0400276C RID: 10092
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x0400276E RID: 10094
		public TIConeLayoutState _dockedShipAbovePositions;

		// Token: 0x04002770 RID: 10096
		[fsIgnore]
		private Dictionary<TIFactionState, Dictionary<FactionResource, float>> netAnnualIncomes;

		// Token: 0x04002771 RID: 10097
		private float administrationModuleModifier;

		// Token: 0x04002772 RID: 10098
		[SerializeField]
		private string habSchematicTemplateName;

		// Token: 0x04002773 RID: 10099
		public TIDateTime HabSchematicAssignedDate;

		// Token: 0x04002774 RID: 10100
		[SerializeField]
		private HabSchematic habSchematic;

		// Token: 0x04002775 RID: 10101
		private HabSchematic habSchematic_SaveRepair;

		// Token: 0x04002777 RID: 10103
		[SerializeField]
		private HashSet<TISpaceFleetState> conflictFleets = new HashSet<TISpaceFleetState>();

		// Token: 0x04002779 RID: 10105
		private GameObject baseObject;

		// Token: 0x0400277A RID: 10106
		private List<TIHabModuleState> cachedOkayModules;

		// Token: 0x0400277B RID: 10107
		private int okayModulesCachedFrame = -1;

		// Token: 0x0400277C RID: 10108
		private List<TIHabModuleState> cachedFunctionalModules;

		// Token: 0x0400277D RID: 10109
		private int functionalModulesCachedFrame = -1;

		// Token: 0x0400277E RID: 10110
		private const bool allowSupplyTheft = false;

		// Token: 0x0400277F RID: 10111
		public readonly List<FactionResource> FarmProvidedResources = new List<FactionResource>
		{
			FactionResource.Water,
			FactionResource.Volatiles
		};

		// Token: 0x04002780 RID: 10112
		private Dictionary<FactionResource, float> cachedMonthlyRevenue = new Dictionary<FactionResource, float>();

		// Token: 0x04002781 RID: 10113
		private int monthlyRevenueCachedFrame;

		// Token: 0x04002782 RID: 10114
		private Dictionary<TISpaceFleetState, bool> enemyFleetInLineOfSight = new Dictionary<TISpaceFleetState, bool>();

		// Token: 0x04002783 RID: 10115
		private TIDateTime cachedLOSCheckTime;

		// Token: 0x04002784 RID: 10116
		public static readonly HabMetric[] HabMetrics = (HabMetric[])Enum.GetValues(typeof(HabMetric));

		// Token: 0x02000EF3 RID: 3827
		public struct RingStruct
		{
			// Token: 0x04005B64 RID: 23396
			public bool NE;

			// Token: 0x04005B65 RID: 23397
			public bool NW;

			// Token: 0x04005B66 RID: 23398
			public bool SE;

			// Token: 0x04005B67 RID: 23399
			public bool SW;
		}

		// Token: 0x02000EF4 RID: 3828
		public struct BaseConnectionStruct
		{
			// Token: 0x04005B68 RID: 23400
			public bool C42;

			// Token: 0x04005B69 RID: 23401
			public bool C16;

			// Token: 0x04005B6A RID: 23402
			public bool C36;

			// Token: 0x04005B6B RID: 23403
			public bool C46;

			// Token: 0x04005B6C RID: 23404
			public bool C56;

			// Token: 0x04005B6D RID: 23405
			public bool C76;
		}

		// Token: 0x02000EF5 RID: 3829
		private struct ModulePlacementOrder
		{
			// Token: 0x06007B25 RID: 31525 RVA: 0x0032143D File Offset: 0x0031F63D
			public ModulePlacementOrder(string module, int sector, int slot)
			{
				this.module = module;
				this.sector = sector;
				this.slot = slot;
			}

			// Token: 0x04005B6E RID: 23406
			public string module;

			// Token: 0x04005B6F RID: 23407
			public int sector;

			// Token: 0x04005B70 RID: 23408
			public int slot;
		}
	}
}
