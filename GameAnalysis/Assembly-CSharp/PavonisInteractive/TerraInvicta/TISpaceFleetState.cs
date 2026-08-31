using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using PavonisInteractive.TerraInvicta.Tasks;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007BA RID: 1978
	public class TISpaceFleetState : TISpaceAssetState, IOperationCapableState, IMobileAsset, ITransferTarget
	{
		// Token: 0x17000CC7 RID: 3271
		// (get) Token: 0x06004353 RID: 17235 RVA: 0x001B494C File Offset: 0x001B2B4C
		// (set) Token: 0x06004354 RID: 17236 RVA: 0x001B4954 File Offset: 0x001B2B54
		public List<TISpaceShipState> ships { get; private set; }

		// Token: 0x17000CC8 RID: 3272
		// (get) Token: 0x06004355 RID: 17237 RVA: 0x001B495D File Offset: 0x001B2B5D
		// (set) Token: 0x06004356 RID: 17238 RVA: 0x001B4965 File Offset: 0x001B2B65
		public Formation formation { get; private set; }

		// Token: 0x17000CC9 RID: 3273
		// (get) Token: 0x06004357 RID: 17239 RVA: 0x001B496E File Offset: 0x001B2B6E
		// (set) Token: 0x06004358 RID: 17240 RVA: 0x001B4976 File Offset: 0x001B2B76
		public Formation savedFormation { get; private set; }

		// Token: 0x17000CCA RID: 3274
		// (get) Token: 0x06004359 RID: 17241 RVA: 0x001B497F File Offset: 0x001B2B7F
		// (set) Token: 0x0600435A RID: 17242 RVA: 0x001B4987 File Offset: 0x001B2B87
		public Trajectory trajectory { get; private set; }

		// Token: 0x17000CCB RID: 3275
		// (get) Token: 0x0600435B RID: 17243 RVA: 0x001B4990 File Offset: 0x001B2B90
		// (set) Token: 0x0600435C RID: 17244 RVA: 0x001B4998 File Offset: 0x001B2B98
		public List<PropellantSharingEvent> propellantSharingPlan { get; private set; }

		// Token: 0x17000CCC RID: 3276
		// (get) Token: 0x0600435D RID: 17245 RVA: 0x001B49A1 File Offset: 0x001B2BA1
		// (set) Token: 0x0600435E RID: 17246 RVA: 0x001B49C4 File Offset: 0x001B2BC4
		public FleetTrajectoryData fleetTrajectoryData
		{
			get
			{
				if (this._fleetTrajectoryData == null && this.gameStateSubjectCreated)
				{
					this._fleetTrajectoryData = new FleetTrajectoryData();
				}
				return this._fleetTrajectoryData;
			}
			set
			{
				this._fleetTrajectoryData = value;
			}
		}

		// Token: 0x17000CCD RID: 3277
		// (get) Token: 0x0600435F RID: 17247 RVA: 0x001B49CD File Offset: 0x001B2BCD
		// (set) Token: 0x06004360 RID: 17248 RVA: 0x001B49D5 File Offset: 0x001B2BD5
		public bool inAccelerationPhase { get; private set; }

		// Token: 0x17000CCE RID: 3278
		// (get) Token: 0x06004361 RID: 17249 RVA: 0x001B49DE File Offset: 0x001B2BDE
		// (set) Token: 0x06004362 RID: 17250 RVA: 0x001B49E6 File Offset: 0x001B2BE6
		public bool inDecelerationPhase { get; private set; }

		// Token: 0x17000CCF RID: 3279
		// (get) Token: 0x06004363 RID: 17251 RVA: 0x001B49EF File Offset: 0x001B2BEF
		// (set) Token: 0x06004364 RID: 17252 RVA: 0x001B49F7 File Offset: 0x001B2BF7
		public bool unavailableForOperations { get; private set; }

		// Token: 0x17000CD0 RID: 3280
		// (get) Token: 0x06004365 RID: 17253 RVA: 0x001B4A00 File Offset: 0x001B2C00
		// (set) Token: 0x06004366 RID: 17254 RVA: 0x001B4A08 File Offset: 0x001B2C08
		public TIDateTime returnToOperationsTime { get; private set; }

		// Token: 0x17000CD1 RID: 3281
		// (get) Token: 0x06004367 RID: 17255 RVA: 0x001B4A11 File Offset: 0x001B2C11
		// (set) Token: 0x06004368 RID: 17256 RVA: 0x001B4A19 File Offset: 0x001B2C19
		public bool huntingXenofauna { get; private set; }

		// Token: 0x17000CD2 RID: 3282
		// (get) Token: 0x06004369 RID: 17257 RVA: 0x001B4A22 File Offset: 0x001B2C22
		// (set) Token: 0x0600436A RID: 17258 RVA: 0x001B4A2A File Offset: 0x001B2C2A
		public TIGameState bombardmentTarget { get; private set; }

		// Token: 0x17000CD3 RID: 3283
		// (get) Token: 0x0600436B RID: 17259 RVA: 0x001B4A33 File Offset: 0x001B2C33
		public bool waitingForCombat
		{
			get
			{
				return TISpaceFleetState.fleetsWaitingToInitiateCombat.Contains(this);
			}
		}

		// Token: 0x17000CD4 RID: 3284
		// (get) Token: 0x0600436C RID: 17260 RVA: 0x001B4A40 File Offset: 0x001B2C40
		public bool inCombatOrWaitingForCombat
		{
			get
			{
				return this.inCombat || this.waitingForCombat;
			}
		}

		// Token: 0x17000CD5 RID: 3285
		// (get) Token: 0x0600436D RID: 17261 RVA: 0x001B4A52 File Offset: 0x001B2C52
		// (set) Token: 0x0600436E RID: 17262 RVA: 0x001B4A5A File Offset: 0x001B2C5A
		public string fleetOperationCompleteName { get; private set; }

		// Token: 0x17000CD6 RID: 3286
		// (get) Token: 0x0600436F RID: 17263 RVA: 0x001B4A63 File Offset: 0x001B2C63
		// (set) Token: 0x06004370 RID: 17264 RVA: 0x001B4A6B File Offset: 0x001B2C6B
		public TIHabState homeport { get; private set; }

		// Token: 0x17000CD7 RID: 3287
		// (get) Token: 0x06004371 RID: 17265 RVA: 0x001B4A74 File Offset: 0x001B2C74
		public override bool inEarthSystem
		{
			get
			{
				if (base.inEarthSystem)
				{
					return true;
				}
				if (this.inTransfer)
				{
					TISpaceObjectState getSunOrbitingRelatedObject = this.GetSphereOfInfluence(false).GetSunOrbitingRelatedObject;
					return getSunOrbitingRelatedObject != null && getSunOrbitingRelatedObject.isEarth;
				}
				return false;
			}
		}

		// Token: 0x17000CD8 RID: 3288
		// (get) Token: 0x06004372 RID: 17266 RVA: 0x001B4AA1 File Offset: 0x001B2CA1
		public bool IsFullyInitialized
		{
			get
			{
				return this.gameStateSubjectCreated;
			}
		}

		// Token: 0x17000CD9 RID: 3289
		// (get) Token: 0x06004373 RID: 17267 RVA: 0x001B4AA9 File Offset: 0x001B2CA9
		public override bool isSpaceFleetState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000CDA RID: 3290
		// (get) Token: 0x06004374 RID: 17268 RVA: 0x001B4AAC File Offset: 0x001B2CAC
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x17000CDB RID: 3291
		// (get) Token: 0x06004375 RID: 17269 RVA: 0x001B4AAF File Offset: 0x001B2CAF
		public override TIFactionState ref_faction
		{
			get
			{
				return base.faction;
			}
		}

		// Token: 0x17000CDC RID: 3292
		// (get) Token: 0x06004376 RID: 17270 RVA: 0x001B4AB7 File Offset: 0x001B2CB7
		public override TIHabState ref_hab
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				return ((tispaceGameState != null) ? tispaceGameState.ref_hab : null) ?? null;
			}
		}

		// Token: 0x17000CDD RID: 3293
		// (get) Token: 0x06004377 RID: 17271 RVA: 0x001B4AD0 File Offset: 0x001B2CD0
		public override TIHabSiteState ref_habSite
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				return ((tispaceGameState != null) ? tispaceGameState.ref_habSite : null) ?? null;
			}
		}

		// Token: 0x17000CDE RID: 3294
		// (get) Token: 0x06004378 RID: 17272 RVA: 0x001B4AE9 File Offset: 0x001B2CE9
		public override TIOrbitState ref_orbit
		{
			get
			{
				return base.orbitState;
			}
		}

		// Token: 0x17000CDF RID: 3295
		// (get) Token: 0x06004379 RID: 17273 RVA: 0x001B4AF1 File Offset: 0x001B2CF1
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000CE0 RID: 3296
		// (get) Token: 0x0600437A RID: 17274 RVA: 0x001B4AF4 File Offset: 0x001B2CF4
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000CE1 RID: 3297
		// (get) Token: 0x0600437B RID: 17275 RVA: 0x001B4AF8 File Offset: 0x001B2CF8
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				TISpaceBodyState tispaceBodyState;
				if ((tispaceBodyState = ((tispaceGameState != null) ? tispaceGameState.ref_spaceBody : null)) == null)
				{
					if (!this.inTransfer)
					{
						TIOrbitState orbitState = base.orbitState;
						if (orbitState == null)
						{
							return null;
						}
						return orbitState.ref_spaceBody;
					}
					else
					{
						tispaceBodyState = this.trajectory.commonBarycenter.ref_spaceBody;
					}
				}
				return tispaceBodyState;
			}
		}

		// Token: 0x17000CE2 RID: 3298
		// (get) Token: 0x0600437C RID: 17276 RVA: 0x001B4B45 File Offset: 0x001B2D45
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				if ((tinaturalSpaceObjectState = ((tispaceGameState != null) ? tispaceGameState.ref_naturalSpaceObject : null)) == null)
				{
					if (!this.inTransfer)
					{
						TIOrbitState orbitState = base.orbitState;
						if (orbitState == null)
						{
							return null;
						}
						return orbitState.ref_naturalSpaceObject;
					}
					else
					{
						tinaturalSpaceObjectState = this.trajectory.commonBarycenter;
					}
				}
				return tinaturalSpaceObjectState;
			}
		}

		// Token: 0x17000CE3 RID: 3299
		// (get) Token: 0x0600437D RID: 17277 RVA: 0x001B4B82 File Offset: 0x001B2D82
		// (set) Token: 0x0600437E RID: 17278 RVA: 0x001B4B8A File Offset: 0x001B2D8A
		public override TINaturalSpaceObjectState barycenter
		{
			get
			{
				return this.ref_naturalSpaceObject;
			}
			set
			{
				base.barycenter = value;
			}
		}

		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x0600437F RID: 17279 RVA: 0x001B4B93 File Offset: 0x001B2D93
		public new TISpaceFleetTemplate template
		{
			get
			{
				return this.GetMyTemplate<TISpaceFleetTemplate>();
			}
		}

		// Token: 0x17000CE5 RID: 3301
		// (get) Token: 0x06004380 RID: 17280 RVA: 0x001B4B9B File Offset: 0x001B2D9B
		public List<TISpaceShipState> smallShips
		{
			get
			{
				return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.template.size == ShipSize.Small).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x17000CE6 RID: 3302
		// (get) Token: 0x06004381 RID: 17281 RVA: 0x001B4BCC File Offset: 0x001B2DCC
		public List<TISpaceShipState> mediumShips
		{
			get
			{
				return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.template.size == ShipSize.Medium).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x17000CE7 RID: 3303
		// (get) Token: 0x06004382 RID: 17282 RVA: 0x001B4BFD File Offset: 0x001B2DFD
		public List<TISpaceShipState> largeShips
		{
			get
			{
				return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.template.size == ShipSize.Large).ToList<TISpaceShipState>();
			}
		}

		// Token: 0x17000CE8 RID: 3304
		// (get) Token: 0x06004383 RID: 17283 RVA: 0x001B4C2E File Offset: 0x001B2E2E
		public override SpaceObjectType objectType
		{
			get
			{
				return SpaceObjectType.Fleet;
			}
		}

		// Token: 0x17000CE9 RID: 3305
		// (get) Token: 0x06004384 RID: 17284 RVA: 0x001B4C31 File Offset: 0x001B2E31
		public override double mass_kg
		{
			get
			{
				return (double)this.ships.Sum<TISpaceShipState>((TISpaceShipState ship) => ship.currentMass_kg);
			}
		}

		// Token: 0x17000CEA RID: 3306
		// (get) Token: 0x06004385 RID: 17285 RVA: 0x001B4C5E File Offset: 0x001B2E5E
		public float altitude_km
		{
			get
			{
				if (this.landed)
				{
					return 0f;
				}
				if (!this.bombarding)
				{
					return (float)(base.orbitState.semiMajorAxis_km - this.barycenter.meanRadius_km);
				}
				return this.bombardmentAltitude_km;
			}
		}

		// Token: 0x17000CEB RID: 3307
		// (get) Token: 0x06004386 RID: 17286 RVA: 0x001B4C95 File Offset: 0x001B2E95
		public bool dockedOrLanded
		{
			get
			{
				return this.dockedLocation != null;
			}
		}

		// Token: 0x17000CEC RID: 3308
		// (get) Token: 0x06004387 RID: 17287 RVA: 0x001B4CA3 File Offset: 0x001B2EA3
		public bool landed
		{
			get
			{
				return this.dockedOrLanded && this.dockedLocation.ref_habSite != null;
			}
		}

		// Token: 0x17000CED RID: 3309
		// (get) Token: 0x06004388 RID: 17288 RVA: 0x001B4CC0 File Offset: 0x001B2EC0
		public bool dockedAtHab
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				return ((tispaceGameState != null) ? tispaceGameState.ref_hab : null) != null;
			}
		}

		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x06004389 RID: 17289 RVA: 0x001B4CDC File Offset: 0x001B2EDC
		public bool landedAtBase
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				bool? flag;
				if (tispaceGameState == null)
				{
					flag = null;
				}
				else
				{
					TIHabState ref_hab = tispaceGameState.ref_hab;
					flag = ((ref_hab != null) ? new bool?(ref_hab.IsBase) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x17000CEF RID: 3311
		// (get) Token: 0x0600438A RID: 17290 RVA: 0x001B4D24 File Offset: 0x001B2F24
		public bool dockedAtStation
		{
			get
			{
				TISpaceGameState tispaceGameState = this.dockedLocation;
				bool? flag;
				if (tispaceGameState == null)
				{
					flag = null;
				}
				else
				{
					TIHabState ref_hab = tispaceGameState.ref_hab;
					flag = ((ref_hab != null) ? new bool?(ref_hab.IsStation) : null);
				}
				bool? flag2 = flag;
				return flag2.GetValueOrDefault();
			}
		}

		// Token: 0x17000CF0 RID: 3312
		// (get) Token: 0x0600438B RID: 17291 RVA: 0x001B4D6C File Offset: 0x001B2F6C
		public bool landedInOutback
		{
			get
			{
				return this.landed && this.dockedLocation.ref_hab == null;
			}
		}

		// Token: 0x17000CF1 RID: 3313
		// (get) Token: 0x0600438C RID: 17292 RVA: 0x001B4D89 File Offset: 0x001B2F89
		public bool transferAssigned
		{
			get
			{
				return this.trajectory != null;
			}
		}

		// Token: 0x17000CF2 RID: 3314
		// (get) Token: 0x0600438D RID: 17293 RVA: 0x001B4D94 File Offset: 0x001B2F94
		public bool inTransfer
		{
			get
			{
				return this.trajectory != null && base.orbitState == null;
			}
		}

		// Token: 0x17000CF3 RID: 3315
		// (get) Token: 0x0600438E RID: 17294 RVA: 0x001B4DAC File Offset: 0x001B2FAC
		public bool isCapableOfTransfering
		{
			get
			{
				return this.ships.All<TISpaceShipState>((TISpaceShipState x) => x.isCapableOfTransfering);
			}
		}

		// Token: 0x17000CF4 RID: 3316
		// (get) Token: 0x0600438F RID: 17295 RVA: 0x001B4DD8 File Offset: 0x001B2FD8
		public bool mayLegallyStartATransfer
		{
			get
			{
				return this.trajectory == null || this.trajectory.involuntary;
			}
		}

		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x06004390 RID: 17296 RVA: 0x001B4DEF File Offset: 0x001B2FEF
		public bool bombarding
		{
			get
			{
				return this.bombardmentTarget != null;
			}
		}

		// Token: 0x06004391 RID: 17297 RVA: 0x001B4DFD File Offset: 0x001B2FFD
		public override float CombatRange_km()
		{
			if (this.ships.Count != 0)
			{
				return this.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.combatRange_km);
			}
			return 0f;
		}

		// Token: 0x06004392 RID: 17298 RVA: 0x001B4E3C File Offset: 0x001B303C
		public override bool IsAlien()
		{
			return base.faction == GameStateManager.AlienFaction();
		}

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x06004393 RID: 17299 RVA: 0x001B4E50 File Offset: 0x001B3050
		public float cruiseAcceleration_kps2
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.cruiseAcceleration_mps2) / 1000f;
				}
				return 0f;
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x06004394 RID: 17300 RVA: 0x001B4EA0 File Offset: 0x001B30A0
		public float cruiseAcceleration_mps2
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.cruiseAcceleration_mps2);
				}
				return 0f;
			}
		}

		// Token: 0x17000CF8 RID: 3320
		// (get) Token: 0x06004395 RID: 17301 RVA: 0x001B4EDF File Offset: 0x001B30DF
		public float maxAcceleration_mps2
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.combatAcceleration_mps2);
				}
				return 0f;
			}
		}

		// Token: 0x17000CF9 RID: 3321
		// (get) Token: 0x06004396 RID: 17302 RVA: 0x001B4F1E File Offset: 0x001B311E
		public float pursuitAcceleration_mps2
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2);
				}
				return 0f;
			}
		}

		// Token: 0x17000CFA RID: 3322
		// (get) Token: 0x06004397 RID: 17303 RVA: 0x001B4F5D File Offset: 0x001B315D
		public float pursuitAcceleration_gs
		{
			get
			{
				return this.pursuitAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000CFB RID: 3323
		// (get) Token: 0x06004398 RID: 17304 RVA: 0x001B4F6B File Offset: 0x001B316B
		public float cruiseAcceleration_gs
		{
			get
			{
				return this.cruiseAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x06004399 RID: 17305 RVA: 0x001B4F79 File Offset: 0x001B3179
		public float maxAcceleration_gs
		{
			get
			{
				return this.maxAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x0600439A RID: 17306 RVA: 0x001B4F88 File Offset: 0x001B3188
		public float fullyLoadedAcceleration_gs
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template).Min<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCombatAcceleration_gs);
				}
				return 0f;
			}
		}

		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x0600439B RID: 17307 RVA: 0x001B4FF6 File Offset: 0x001B31F6
		public float maxDeltaV_kps
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.currentMaxDeltaV_kps);
				}
				return 0f;
			}
		}

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x0600439C RID: 17308 RVA: 0x001B5035 File Offset: 0x001B3235
		public float currentDeltaV_kps
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps);
				}
				return 0f;
			}
		}

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x0600439D RID: 17309 RVA: 0x001B5074 File Offset: 0x001B3274
		public float currentDeltaV_mps
		{
			get
			{
				return this.currentDeltaV_kps * 1000f;
			}
		}

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x0600439E RID: 17310 RVA: 0x001B5082 File Offset: 0x001B3282
		public List<TICouncilorState> councilorPassengers
		{
			get
			{
				return this.ships.SelectMany<TISpaceShipState, TICouncilorState>((TISpaceShipState ship) => ship.councilorPassengers).ToList<TICouncilorState>();
			}
		}

		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x0600439F RID: 17311 RVA: 0x001B50B3 File Offset: 0x001B32B3
		public List<TICouncilorState> alienPassengers
		{
			get
			{
				return this.councilorPassengers.Where<TICouncilorState>((TICouncilorState councilor) => councilor.isAlien).ToList<TICouncilorState>();
			}
		}

		// Token: 0x060043A0 RID: 17312 RVA: 0x001B50E4 File Offset: 0x001B32E4
		public override float SpaceCombatValue()
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
		}

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x060043A1 RID: 17313 RVA: 0x001B5110 File Offset: 0x001B3310
		// (set) Token: 0x060043A2 RID: 17314 RVA: 0x001B5118 File Offset: 0x001B3318
		public Vector3d dockOffset { get; private set; }

		// Token: 0x17000D04 RID: 3332
		// (get) Token: 0x060043A3 RID: 17315 RVA: 0x001B5121 File Offset: 0x001B3321
		public override string modelResource
		{
			get
			{
				return "ships/FleetContainer";
			}
		}

		// Token: 0x17000D05 RID: 3333
		// (get) Token: 0x060043A4 RID: 17316 RVA: 0x001B5128 File Offset: 0x001B3328
		public override double meanRadius_m
		{
			get
			{
				return 300.0;
			}
		}

		// Token: 0x17000D06 RID: 3334
		// (get) Token: 0x060043A5 RID: 17317 RVA: 0x001B5133 File Offset: 0x001B3333
		public override double meanRadius_km
		{
			get
			{
				return 0.30000001192092896;
			}
		}

		// Token: 0x17000D07 RID: 3335
		// (get) Token: 0x060043A6 RID: 17318 RVA: 0x001B513E File Offset: 0x001B333E
		public override float modelScale
		{
			get
			{
				return 525f;
			}
		}

		// Token: 0x17000D08 RID: 3336
		// (get) Token: 0x060043A7 RID: 17319 RVA: 0x001B5145 File Offset: 0x001B3345
		public bool allShipsHaveDeltaV
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.All<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps > 0f);
				}
				return false;
			}
		}

		// Token: 0x17000D09 RID: 3337
		// (get) Token: 0x060043A8 RID: 17320 RVA: 0x001B5180 File Offset: 0x001B3380
		public bool allShipsCanManeuver
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.All<TISpaceShipState>((TISpaceShipState x) => x.CanRotateAndRoll());
				}
				return false;
			}
		}

		// Token: 0x17000D0A RID: 3338
		// (get) Token: 0x060043A9 RID: 17321 RVA: 0x001B51BB File Offset: 0x001B33BB
		public float availableDeltaVforPrecombat_kps
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps());
				}
				return 0f;
			}
		}

		// Token: 0x17000D0B RID: 3339
		// (get) Token: 0x060043AA RID: 17322 RVA: 0x001B51FC File Offset: 0x001B33FC
		public float availableDeltaVforPrecombat_mps
		{
			get
			{
				if (this.ships.Count != 0)
				{
					return 1000f * this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps());
				}
				return 0f;
			}
		}

		// Token: 0x17000D0C RID: 3340
		// (get) Token: 0x060043AB RID: 17323 RVA: 0x001B524C File Offset: 0x001B344C
		public Vector3[] pipPosition
		{
			get
			{
				if (!this.IsAlien())
				{
					return this.defaultHumanFormation.pattern.pos;
				}
				return this.defaultAlienFormation.pattern.pos;
			}
		}

		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x060043AC RID: 17324 RVA: 0x001B5288 File Offset: 0x001B3488
		public Quaternion RotationNow
		{
			get
			{
				return base.gameObjectLink.transform.localRotation;
			}
		}

		// Token: 0x17000D0E RID: 3342
		// (get) Token: 0x060043AD RID: 17325 RVA: 0x001B529A File Offset: 0x001B349A
		// (set) Token: 0x060043AE RID: 17326 RVA: 0x001B52A2 File Offset: 0x001B34A2
		public Trajectory[] proposedTrajectories { get; private set; }

		// Token: 0x060043AF RID: 17327 RVA: 0x001B52AB File Offset: 0x001B34AB
		public void destroyProposedTrajectories()
		{
			this.proposedTrajectories = null;
		}

		// Token: 0x17000D0F RID: 3343
		// (get) Token: 0x060043B0 RID: 17328 RVA: 0x001B52B4 File Offset: 0x001B34B4
		public override double semiMajorAxis_m
		{
			get
			{
				if (!this.inTransfer)
				{
					return base.orbitState.semiMajorAxis_m;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).semiMajorAxis_m;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.semiMajorAxis_m;
			}
		}

		// Token: 0x17000D10 RID: 3344
		// (get) Token: 0x060043B1 RID: 17329 RVA: 0x001B5310 File Offset: 0x001B3510
		public override double ecc
		{
			get
			{
				if (!this.inTransfer)
				{
					return base.orbitState.eccentricity;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).eccentricity;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.eccentricity;
			}
		}

		// Token: 0x17000D11 RID: 3345
		// (get) Token: 0x060043B2 RID: 17330 RVA: 0x001B536C File Offset: 0x001B356C
		public override double inclination_Rad
		{
			get
			{
				if (!this.inTransfer)
				{
					return base.orbitState.inclination_Rad;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).inclination_Rad;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.inclination_Rad;
			}
		}

		// Token: 0x17000D12 RID: 3346
		// (get) Token: 0x060043B3 RID: 17331 RVA: 0x001B53C8 File Offset: 0x001B35C8
		public override double longAscendingNode_Rad
		{
			get
			{
				if (!this.inTransfer)
				{
					return base.orbitState.longitudeAscendingNode_Rad;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).longAscendingNode_Rad;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.longAscendingNode_Rad;
			}
		}

		// Token: 0x17000D13 RID: 3347
		// (get) Token: 0x060043B4 RID: 17332 RVA: 0x001B5424 File Offset: 0x001B3624
		public override double argPeriapsis_Rad
		{
			get
			{
				if (!this.inTransfer)
				{
					return base.orbitState.argPeriapsis_Rad;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).argPeriapsis_Rad;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.argPeriapsis_Rad;
			}
		}

		// Token: 0x17000D14 RID: 3348
		// (get) Token: 0x060043B5 RID: 17333 RVA: 0x001B5480 File Offset: 0x001B3680
		public override double meanAnomalyAtEpoch_Rad
		{
			get
			{
				if (!this.inTransfer)
				{
					return this._meanAnomalyAtEpoch_Rad;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).meanAnomalyAtEpoch_Rad;
				}
				return (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.meanAnomalyAtEpoch_Rad;
			}
		}

		// Token: 0x17000D15 RID: 3349
		// (get) Token: 0x060043B6 RID: 17334 RVA: 0x001B54D4 File Offset: 0x001B36D4
		public override double epoch_JYears
		{
			get
			{
				if (!this.inTransfer)
				{
					return this._epoch_JYears;
				}
				if (!this.trajectory.HasOrbitalElements())
				{
					return new TIDateTime(this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now()).epoch).ToJulianDate();
				}
				return new TIDateTime((this.trajectory as Trajectory_WithOrbitalElements).transferOrbit.epoch).ToJulianDate();
			}
		}

		// Token: 0x17000D16 RID: 3350
		// (get) Token: 0x060043B7 RID: 17335 RVA: 0x001B553C File Offset: 0x001B373C
		public override double meanLongitude_Rad
		{
			get
			{
				return this.meanAnomalyAtEpoch_Rad + this.longAscendingNode_Rad;
			}
		}

		// Token: 0x17000D17 RID: 3351
		// (get) Token: 0x060043B8 RID: 17336 RVA: 0x001B554B File Offset: 0x001B374B
		public override TISpaceGameState location
		{
			get
			{
				if (this.dockedOrLanded)
				{
					return this.dockedLocation;
				}
				if (this.inTransfer)
				{
					return this.GetSphereOfInfluence(false);
				}
				return this.ref_orbit;
			}
		}

		// Token: 0x060043B9 RID: 17337 RVA: 0x001B5574 File Offset: 0x001B3774
		public string GetLocationDescription(TIFactionState faction, bool capitalize, bool expand)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.landed)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.LandedLocation", new object[] { this.dockedLocation.GetDisplayName(faction) }));
				if (expand)
				{
					stringBuilder.Append(Loc.T("UI.Fleets.ExpandedLocation", new object[] { this.dockedLocation.ref_naturalSpaceObject.displayName }));
				}
			}
			else if (this.dockedOrLanded)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.DockedLocation", new object[] { this.dockedLocation.GetDisplayName(faction) }));
				if (expand)
				{
					stringBuilder.Append(Loc.T("UI.Fleets.ExpandedLocation", new object[] { this.dockedLocation.ref_orbit.displayName }));
				}
			}
			else if (this.inTransfer)
			{
				if (TIGameState.Valid(this.trajectory.destinationFleet))
				{
					if (this.trajectory.destinationFleet.faction == base.faction)
					{
						stringBuilder.Append(Loc.T("UI.Fleets.DestinationLocationFriendlyFleet", new object[] { this.trajectory.destinationFleet.GetDisplayName(faction) }));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Fleets.DestinationLocationFleet", new object[] { this.trajectory.destinationFleet.GetDisplayName(faction) }));
					}
					if (expand && !this.trajectory.destinationFleet.inTransfer)
					{
						stringBuilder.Append(Loc.T("UI.Fleets.ExpandedLocation", new object[] { this.trajectory.destinationFleet.GetLocationDescription(faction, false, false) }));
					}
				}
				else if (TIGameState.Valid(this.trajectory.destinationStation))
				{
					stringBuilder.Append(Loc.T("UI.Fleets.DestinationLocationHab", new object[] { this.trajectory.destinationStation.GetDisplayName(faction) }));
					if (expand)
					{
						stringBuilder.Append(Loc.T("UI.Fleets.ExpandedLocation", new object[] { this.trajectory.destinationStation.orbitState.displayName }));
					}
				}
				else if (this.trajectory.endsInCrash)
				{
					stringBuilder.Append(Loc.T("UI.Space.Fleet.Crashing", new object[] { this.trajectory.collisionTarget.displayName }));
				}
				else if (this.trajectory.exitsSolarSystem)
				{
					stringBuilder.Append(Loc.T("UI.Space.Fleet.LeavingSolarSystem"));
				}
				else if (this.trajectory.destinationOrbit != null)
				{
					stringBuilder.Append(Loc.T("UI.Fleets.DestinationLocationOrbit", new object[] { this.trajectory.destinationOrbit.displayName }));
				}
			}
			else if (base.orbitState != null)
			{
				stringBuilder.Append(Loc.T("UI.Fleets.OrbitLocation", new object[] { base.orbitState.displayName }));
			}
			string text = stringBuilder.ToString();
			if (capitalize)
			{
				text = Utilities.Capitalize(text);
			}
			return text;
		}

		// Token: 0x17000D18 RID: 3352
		// (get) Token: 0x060043BA RID: 17338 RVA: 0x001B5888 File Offset: 0x001B3A88
		public override string iconResource
		{
			get
			{
				if (this.ships.Count <= 3)
				{
					return base.faction.template.fleetIcon1Resource;
				}
				if (this.ships.Count <= 20)
				{
					return base.faction.template.fleetIcon2Resource;
				}
				return base.faction.template.fleetIcon3Resource;
			}
		}

		// Token: 0x060043BB RID: 17339 RVA: 0x001B58E4 File Offset: 0x001B3AE4
		public string FleetQuickDescription(TIFactionState viewingFaction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetDisplayName(viewingFaction));
			if (this.ships.Count == 1)
			{
				stringBuilder.AppendLine(Loc.T("UI.Fleets.NumShipsSingle", new object[] { this.ships.Count.ToString("N0") }));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Fleets.NumShipsPlural", new object[] { this.ships.Count.ToString("N0") }));
			}
			stringBuilder.AppendLine(Loc.T("UI.Fleets.CruiseAcceleration", new object[] { FleetsScreenController.accelerationStr((double)this.cruiseAcceleration_gs, false, false, true) }));
			stringBuilder.AppendLine(Loc.T("UI.Fleets.CombatAcceleration", new object[] { FleetsScreenController.accelerationStr((double)this.maxAcceleration_gs, true, false, true) }));
			stringBuilder.AppendLine(Loc.T("UI.Fleets.StdDV", new object[]
			{
				TemplateManager.global.deltaVInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(this.currentDeltaV_kps, 1, 7, 0, false, false)
			}));
			stringBuilder.AppendLine(Loc.T("UI.Global.2ItemsSpaced", new object[]
			{
				TemplateManager.global.spaceCombatScoreInlineSpritePath,
				this.SpaceCombatValue().ToString("N0")
			}));
			stringBuilder.AppendLine(Loc.T("UI.Global.2ItemsSpaced", new object[]
			{
				TemplateManager.global.spaceAssaultValueInlineSpritePath,
				this.AssaultCombatValue(false).ToString("N0")
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x060043BC RID: 17340 RVA: 0x001B5A80 File Offset: 0x001B3C80
		public override Vector3d GetGlobalPositionAtTime(TIDateTime time)
		{
			if (this.transferAssigned && time > this.trajectory.launchTime)
			{
				TIDateTime tidateTime = TITimeState.Now();
				bool flag;
				Vector3d vector3d = this.trajectory.PositionAtTime(time, time == tidateTime && this.IsFullyInitialized, out flag);
				if (time == tidateTime)
				{
					this.globalPosition = vector3d;
					this.globalPositionTime = time.ExportTime();
				}
				return vector3d;
			}
			if (this.landed)
			{
				return this.ref_habSite.GlobalPosition(time);
			}
			if (this.dockedAtHab)
			{
				return this.ref_hab.GetGlobalPositionAtTime(time) + this.dockOffset;
			}
			return base.GetGlobalPositionAtTime(time);
		}

		// Token: 0x060043BD RID: 17341 RVA: 0x001B5B29 File Offset: 0x001B3D29
		public bool InSphereOfInfluence(TISpaceObjectState spaceObject)
		{
			return spaceObject is TINaturalSpaceObjectState && this.GetSphereOfInfluence(true) == spaceObject;
		}

		// Token: 0x060043BE RID: 17342 RVA: 0x001B5B44 File Offset: 0x001B3D44
		public override TINaturalSpaceObjectState GetSphereOfInfluence(bool exact = false)
		{
			if (!this.inTransfer)
			{
				if (this.landed)
				{
					return this.ref_habSite.parentBody;
				}
				return this.ref_orbit.barycenter;
			}
			else
			{
				if (!exact)
				{
					return this.trajectory.GetBarycenterAtTime(this.gameTime.currentTime);
				}
				TIOrbitState originOrbit = this.trajectory.originOrbit;
				TINaturalSpaceObjectState tinaturalSpaceObjectState = ((originOrbit != null) ? originOrbit.barycenter : null);
				if (tinaturalSpaceObjectState != null && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(this, tinaturalSpaceObjectState) <= tinaturalSpaceObjectState.sphereOfInfluence_m)
				{
					return tinaturalSpaceObjectState;
				}
				TIOrbitState destinationOrbit = this.trajectory.destinationOrbit;
				TINaturalSpaceObjectState tinaturalSpaceObjectState2 = ((destinationOrbit != null) ? destinationOrbit.barycenter : null);
				if (tinaturalSpaceObjectState2 != null && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(this, tinaturalSpaceObjectState2) <= tinaturalSpaceObjectState2.sphereOfInfluence_m)
				{
					return tinaturalSpaceObjectState2;
				}
				TIOrbitState originOrbit2 = this.trajectory.originOrbit;
				TINaturalSpaceObjectState tinaturalSpaceObjectState3;
				if (originOrbit2 == null)
				{
					tinaturalSpaceObjectState3 = null;
				}
				else
				{
					TINaturalSpaceObjectState barycenter = originOrbit2.barycenter;
					tinaturalSpaceObjectState3 = ((barycenter != null) ? barycenter.barycenter : null);
				}
				TINaturalSpaceObjectState tinaturalSpaceObjectState4 = tinaturalSpaceObjectState3;
				if (tinaturalSpaceObjectState4 != null && !tinaturalSpaceObjectState4.isSun && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(this, tinaturalSpaceObjectState4) <= tinaturalSpaceObjectState4.sphereOfInfluence_m)
				{
					return tinaturalSpaceObjectState4;
				}
				TIOrbitState destinationOrbit2 = this.trajectory.destinationOrbit;
				TINaturalSpaceObjectState tinaturalSpaceObjectState5;
				if (destinationOrbit2 == null)
				{
					tinaturalSpaceObjectState5 = null;
				}
				else
				{
					TINaturalSpaceObjectState barycenter2 = destinationOrbit2.barycenter;
					tinaturalSpaceObjectState5 = ((barycenter2 != null) ? barycenter2.barycenter : null);
				}
				TINaturalSpaceObjectState tinaturalSpaceObjectState6 = tinaturalSpaceObjectState5;
				if (tinaturalSpaceObjectState6 != null && !tinaturalSpaceObjectState6.isSun && TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(this, tinaturalSpaceObjectState6) <= tinaturalSpaceObjectState6.sphereOfInfluence_m)
				{
					return tinaturalSpaceObjectState6;
				}
				return GameStateManager.Sol();
			}
		}

		// Token: 0x060043BF RID: 17343 RVA: 0x001B5C88 File Offset: 0x001B3E88
		private double MaxPreAerobreakVelocity_mps(double postAerobreakVelocity_mps, bool isSafe)
		{
			return this.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.MaxPreAerobreakVelocity_mps(postAerobreakVelocity_mps, isSafe));
		}

		// Token: 0x17000D19 RID: 3353
		// (get) Token: 0x060043C0 RID: 17344 RVA: 0x001B5CC0 File Offset: 0x001B3EC0
		public IEnumerable<TISpaceFleetState> RallyingFleets
		{
			get
			{
				return base.faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					FactionGoal_JoinFleet factionGoal_JoinFleet = x.AssignedGoal() as FactionGoal_JoinFleet;
					return factionGoal_JoinFleet != null && factionGoal_JoinFleet.targetFleet == this;
				});
			}
		}

		// Token: 0x17000D1A RID: 3354
		// (get) Token: 0x060043C1 RID: 17345 RVA: 0x001B5CE0 File Offset: 0x001B3EE0
		public bool IsStarterAlienCouncilorFleetInOriginalLocation
		{
			get
			{
				if (this.IsAlien() && this.location == this.campaignStartLocation)
				{
					return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.CouncilorTransport);
				}
				return false;
			}
		}

		// Token: 0x060043C2 RID: 17346 RVA: 0x001B5D34 File Offset: 0x001B3F34
		public override CartesianState? tryToGetGlobalCartesianState(TIDateTime time)
		{
			if (!this.transferAssigned || time < this.trajectory.launchTime)
			{
				return base.tryToGetGlobalCartesianState(time);
			}
			return new CartesianState?(this.trajectory.ToGlobalCartesianStateAtTime(time));
		}

		// Token: 0x060043C3 RID: 17347 RVA: 0x001B5D6C File Offset: 0x001B3F6C
		public override bool tryToGetLocalCartesianState(TIDateTime time, out CartesianState cartesianState, out TINaturalSpaceObjectState barycenter)
		{
			if (!this.transferAssigned || time < this.trajectory.launchTime)
			{
				return base.tryToGetLocalCartesianState(time, out cartesianState, out barycenter);
			}
			barycenter = this.trajectory.GetBarycenterAtTime(time);
			CartesianState? cartesianState2 = this.tryToGetGlobalCartesianState(time);
			cartesianState = ((cartesianState2 != null) ? cartesianState2.GetValueOrDefault().ToLocal(barycenter, time) : default(CartesianState));
			return cartesianState2 != null;
		}

		// Token: 0x060043C4 RID: 17348 RVA: 0x001B5DE7 File Offset: 0x001B3FE7
		public override TINaturalSpaceObjectState localBarycenter(TIDateTime time)
		{
			if (!this.transferAssigned || time < this.trajectory.launchTime)
			{
				return base.localBarycenter(time);
			}
			return this.trajectory.GetBarycenterAtTime(time);
		}

		// Token: 0x060043C5 RID: 17349 RVA: 0x001B5E18 File Offset: 0x001B4018
		public override void getOrbitalElementsState(TIDateTime time, out OrbitalElementsState orbitalElementsState, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood)
		{
			if (!this.transferAssigned || time <= this.trajectory.launchTime)
			{
				base.getOrbitalElementsState(time, out orbitalElementsState, out barycenter, out meanAnomalyIsGood);
				return;
			}
			barycenter = this.trajectory.GetBarycenterAtTime(time);
			orbitalElementsState = this.trajectory.GetOrbitalElementsAtTime(time);
			meanAnomalyIsGood = true;
		}

		// Token: 0x060043C6 RID: 17350 RVA: 0x001B5E70 File Offset: 0x001B4070
		CartesianState ITransferTarget.relevantGlobalCartesianState(TINaturalSpaceObjectState commonBarycenter, TIDateTime dateTime)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = (this.transferAssigned ? this.trajectory.GetBarycenterAtTime(dateTime) : this.barycenter);
			if (tinaturalSpaceObjectState == commonBarycenter)
			{
				if (!this.transferAssigned)
				{
					return this.ToGlobalCartesianStateAtTime(dateTime);
				}
				return this.trajectory.ToGlobalCartesianStateAtTime(dateTime);
			}
			else
			{
				if (tinaturalSpaceObjectState.barycenter == commonBarycenter || tinaturalSpaceObjectState.barycenter == null)
				{
					return tinaturalSpaceObjectState.ToGlobalCartesianStateAtTime(dateTime);
				}
				return tinaturalSpaceObjectState.barycenter.ToGlobalCartesianStateAtTime(dateTime);
			}
		}

		// Token: 0x060043C7 RID: 17351 RVA: 0x001B5EF0 File Offset: 0x001B40F0
		public override double common_a_m(TINaturalSpaceObjectState commonBarycenter)
		{
			if (!this.transferAssigned || !this.trajectory.launched)
			{
				return base.common_a_m(commonBarycenter);
			}
			TIDateTime tidateTime = TITimeState.Now();
			TINaturalSpaceObjectState barycenterAtTime = this.trajectory.GetBarycenterAtTime(tidateTime);
			if (barycenterAtTime == commonBarycenter)
			{
				return (this.trajectory.ToGlobalCartesianStateAtTime(tidateTime) - commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime)).position.magnitude;
			}
			if (barycenterAtTime.barycenter == commonBarycenter)
			{
				return barycenterAtTime.semiMajorAxis_m;
			}
			return barycenterAtTime.barycenter.semiMajorAxis_m;
		}

		// Token: 0x060043C8 RID: 17352 RVA: 0x001B5F7C File Offset: 0x001B417C
		public override double common_i_rad(TINaturalSpaceObjectState commonBarycenter)
		{
			if (!this.transferAssigned || !this.trajectory.launched)
			{
				return base.common_i_rad(commonBarycenter);
			}
			TIDateTime tidateTime = TITimeState.Now();
			TINaturalSpaceObjectState barycenterAtTime = this.trajectory.GetBarycenterAtTime(tidateTime);
			if (barycenterAtTime == commonBarycenter)
			{
				return this.trajectory.GetOrbitalElementsAtTime(tidateTime).inclination_Rad;
			}
			if (barycenterAtTime.barycenter == commonBarycenter)
			{
				return barycenterAtTime.inclination_Rad;
			}
			return barycenterAtTime.barycenter.inclination_Rad;
		}

		// Token: 0x060043C9 RID: 17353 RVA: 0x001B5FF4 File Offset: 0x001B41F4
		public override double common_M_rad(TINaturalSpaceObjectState commonBarycenter, TIDateTime time)
		{
			if (!this.transferAssigned || !(this.trajectory.launchTime < time))
			{
				return base.common_M_rad(commonBarycenter, time);
			}
			if (this.trajectory.arrivalTime <= time)
			{
				if (this.trajectory.endsInCrash || this.trajectory.exitsSolarSystem)
				{
					Log.Error("Attempted to get fleet's mean anomaly after a crash or exiting the solar system.", Array.Empty<object>());
					return -1.0;
				}
				return (this.trajectory.destination as ITransferTarget).common_M_rad(commonBarycenter, time);
			}
			else
			{
				TINaturalSpaceObjectState barycenterAtTime = this.trajectory.GetBarycenterAtTime(time);
				if (barycenterAtTime == commonBarycenter)
				{
					return this.trajectory.GetOrbitalElementsAtTime(time).MeanAnomalyAtTime_Rad(time.ExportTime(), barycenterAtTime.mass_kg);
				}
				if (barycenterAtTime.barycenter == commonBarycenter)
				{
					return barycenterAtTime.meanAnomaly_Rad(time);
				}
				return barycenterAtTime.barycenter.meanAnomaly_Rad(time);
			}
		}

		// Token: 0x17000D1B RID: 3355
		// (get) Token: 0x060043CA RID: 17354 RVA: 0x001B60E2 File Offset: 0x001B42E2
		public bool fleetIsLost
		{
			get
			{
				return this.inTransfer && (this.trajectory.exitsSolarSystem || this.trajectory.endsInCrash);
			}
		}

		// Token: 0x060043CB RID: 17355 RVA: 0x001B6108 File Offset: 0x001B4308
		public override bool Initialize()
		{
			this.ships = new List<TISpaceShipState>();
			base.epoch_DateTime = new TIDateTime();
			return base.Initialize();
		}

		// Token: 0x060043CC RID: 17356 RVA: 0x001B6126 File Offset: 0x001B4326
		public override void InitWithTemplate(TIDataTemplate template)
		{
			if (!this.gameStateSubjectCreated)
			{
				this.templateName = template.dataName;
				this.displayNameByFaction = new Dictionary<TIFactionState, string>();
			}
		}

		// Token: 0x060043CD RID: 17357 RVA: 0x001B6148 File Offset: 0x001B4348
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (!this.gameStateSubjectCreated)
			{
				TISpaceFleetTemplate template = this.template;
				base.faction = GameStateManager.FindByTemplate<TIFactionState>(template.factionName, false);
				if (base.faction == null)
				{
					base.faction = GameStateManager.AllHumanFactions().SelectRandomItem<TIFactionState>();
					Log.Error(string.Concat(new string[]
					{
						"Bad config attempting to assign fleet ",
						this.displayName,
						" to nonexistent ",
						this.template.factionName,
						". Granting to ",
						base.faction.displayName,
						" instead."
					}), Array.Empty<object>());
				}
				base.faction.fleets.Add(this);
				this.AssignFormation(this.template.defaultFormation, false, true, false, false, false);
				this.ships = new List<TISpaceShipState>();
				int num = -1;
				List<TISpaceShipState> list = new List<TISpaceShipState>();
				foreach (TISpaceFleetTemplate.ShipFleetDefinition shipFleetDefinition in this.template.filteredShipsInFleet)
				{
					TISpaceShipTemplate tispaceShipTemplate = TemplateManager.Find<TISpaceShipTemplate>(shipFleetDefinition.shipTemplateName, false);
					if (tispaceShipTemplate != null)
					{
						TISpaceShipState tispaceShipState = GameStateManager.CreateNewGameState<TISpaceShipState>();
						if (tispaceShipTemplate.designingFaction == null && !GameControl.control.skirmishMode)
						{
							tispaceShipTemplate.factionName = base.faction.templateName;
						}
						if (this.template.factionName != tispaceShipTemplate.factionName)
						{
							string text = new StringBuilder(tispaceShipTemplate.dataName).Append("_").Append(this.template.factionName).ToString();
							TISpaceShipTemplate tispaceShipTemplate2 = TemplateManager.Find<TISpaceShipTemplate>(text, false);
							if (tispaceShipTemplate2 == null)
							{
								tispaceShipTemplate2 = tispaceShipTemplate.Clone(text, this.template.factionName);
								tispaceShipTemplate2.SetClassDisplayName(false);
								TemplateManager.Add(tispaceShipTemplate2, typeof(TISpaceShipTemplate), false);
								TemplateManager.AddSkirmishModeTemplate(tispaceShipTemplate2);
							}
							tispaceShipState.InitWithTemplate(tispaceShipTemplate2);
						}
						else
						{
							tispaceShipState.InitWithTemplate(tispaceShipTemplate);
						}
						tispaceShipState.SetDisplayName(TISpaceAssetState.GetRandomAssetName(tispaceShipState, base.faction));
						list.Add(tispaceShipState);
						num++;
					}
				}
				this.currentOperations = new List<OperationData>();
				this._fleetTrajectoryData = new FleetTrajectoryData();
				this.AddShipsToFleet(list, null, false, true);
				this.AssignFormation(this.template.defaultFormation, false, true, false, false, false);
				this.TeleportAllToFormation(false, false);
			}
		}

		// Token: 0x060043CE RID: 17358 RVA: 0x001B63D8 File Offset: 0x001B45D8
		public override void PostGlobalGameStateCreateInit_2()
		{
			base.PostGlobalGameStateCreateInit_2();
			if (!this.gameStateSubjectCreated && base.orbitState == null)
			{
				TIOrbitState tiorbitState = GameStateManager.FindByTemplate<TIOrbitState>(this.template.orbitTemplateName, false);
				if (this.template.meanAnomalyAtEpoch_Deg != null)
				{
					base.AssumeOrbitFromState(tiorbitState, 0.017453292519943295 * this.template.meanAnomalyAtEpoch_Deg.Value, TITimeState.Now());
				}
				else
				{
					base.SetRandomizedOrbitFromState(tiorbitState, true);
				}
				this.campaignStartLocation = this.location;
			}
			foreach (TISpaceShipState tispaceShipState in this.ships.ToList<TISpaceShipState>())
			{
				if (!TIGameState.Valid(tispaceShipState))
				{
					this.ships.Remove(tispaceShipState);
					Log.Warn("Removed an invalid ship from " + base.ID.ToString(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x060043CF RID: 17359 RVA: 0x001B64E4 File Offset: 0x001B46E4
		public override void PostCanvasManagerCreateInit_3()
		{
			if (!this.gameStateSubjectCreated && !GameControl.control.skirmishMode && !base.faction.IsAlienFaction)
			{
				if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.allWeaponTemplates.Count > 0))
				{
					TIGlobalValuesState.GlobalValues.CheckGlobalMilestone(GlobalMilestone.FirstWarship, null, null);
				}
			}
		}

		// Token: 0x060043D0 RID: 17360 RVA: 0x001B654C File Offset: 0x001B474C
		public override void PostInitializationInit_4()
		{
			if (this.gameStateSubjectCreated)
			{
				if (this.trajectory == null)
				{
					for (int i = 0; i < this.currentOperations.Count; i++)
					{
						if (this.currentOperations[i].operation is TransferOperation)
						{
							this.currentOperations.Remove(this.currentOperations[i]);
						}
					}
				}
				Trajectory trajectory = this.trajectory;
				int num = 100;
				while (trajectory != null && num > 0)
				{
					num--;
					TIHabState destinationStation = trajectory.destinationStation;
					if (destinationStation != null && destinationStation.deleted)
					{
						goto IL_0091;
					}
					TISpaceFleetState destinationFleet = trajectory.destinationFleet;
					if (destinationFleet != null && destinationFleet.deleted)
					{
						goto IL_0091;
					}
					IL_0097:
					trajectory = trajectory.nextTrajectory;
					continue;
					IL_0091:
					trajectory.DestinationDestroyed();
					goto IL_0097;
				}
				if (this.trajectory != null)
				{
					Trajectory trajectory2 = this.trajectory;
					Trajectory trajectory3;
					if (trajectory2 == null)
					{
						trajectory3 = null;
					}
					else
					{
						TISpaceFleetState destinationFleet2 = trajectory2.destinationFleet;
						trajectory3 = ((destinationFleet2 != null) ? destinationFleet2.trajectory : null);
					}
					Trajectory trajectory4 = trajectory3 ?? null;
					Trajectory trajectory5 = this.trajectory;
					if (trajectory4 != (((trajectory5 != null) ? trajectory5.destinationFleetTrajectory : null) ?? null))
					{
						TISpaceFleetState tispaceFleetState = this.trajectory.destination as TISpaceFleetState;
						if (tispaceFleetState != null && tispaceFleetState.transferAssigned && (tispaceFleetState.trajectory.launched || tispaceFleetState.faction == base.faction))
						{
							this.trajectory.destinationFleetTrajectory = this.trajectory.destinationFleet.trajectory;
						}
					}
				}
				if (this.transferAssigned && this.trajectory.destination == null && !this.trajectory.destroyOnArrival)
				{
					this.trajectory.ReconstructMissingDestinationOrbit();
				}
				if (this.AI_FailedAttackEnemyStrength == null)
				{
					this.AI_FailedAttackEnemyStrength = new Dictionary<TIGameState, float>();
				}
				this.RepairPotentialInvalidCommonBarycenterForTrajectory();
				this.RepairTrajectorieWithHyperbolicMicrothrusting();
				if (this.trajectory != null)
				{
					this.trajectory.EnsureConsistentDestinationOrbitOnLoad();
				}
			}
			this.OnFleetCreated();
			this.InitializeRunTimeFleetData(false);
		}

		// Token: 0x060043D1 RID: 17361 RVA: 0x001B6710 File Offset: 0x001B4910
		private void RepairTrajectorieWithHyperbolicMicrothrusting()
		{
			if (this.transferAssigned)
			{
				Trajectory_Patched trajectory_Patched = this.trajectory as Trajectory_Patched;
				if (trajectory_Patched != null)
				{
					foreach (Trajectory_Patched.MicrothrustSegment microthrustSegment in trajectory_Patched.Segments.OfType<Trajectory_Patched.MicrothrustSegment>())
					{
						if (microthrustSegment.eccentricity >= 1.0)
						{
							Log.Error("Caught a hyperbolic microthrust on load -- circularizing to avoid future crashes.", Array.Empty<object>());
							microthrustSegment.eccentricity = 0.0;
						}
					}
					foreach (Trajectory_Patched.MicrothrustLERPSegment microthrustLERPSegment in trajectory_Patched.Segments.OfType<Trajectory_Patched.MicrothrustLERPSegment>())
					{
						if (microthrustLERPSegment.eccentricity >= 1.0)
						{
							Log.Error("Caught a hyperbolic microthrust on load -- circularizing to avoid future crashes.", Array.Empty<object>());
							microthrustLERPSegment.eccentricity = 0.0;
						}
						if (microthrustLERPSegment.endEccentricity >= 1.0)
						{
							Log.Error("Caught a hyperbolic microthrust on load -- circularizing to avoid future crashes.", Array.Empty<object>());
							microthrustLERPSegment.endEccentricity = 0.0;
						}
						if (microthrustLERPSegment.barycenter.mu / (microthrustLERPSegment.initialVelocity_mps * microthrustLERPSegment.initialVelocity_mps) + microthrustLERPSegment.startRadiusCorrection_m <= 0.0)
						{
							Log.Error("Caught negative radius microthrust on load -- removing radius correction to avoid future crashes.", Array.Empty<object>());
							microthrustLERPSegment.startRadiusCorrection_m = 0.0;
						}
						double num = microthrustLERPSegment.endTime.DifferenceInSeconds(microthrustLERPSegment.startTime);
						double num2 = microthrustLERPSegment.initialVelocity_mps + microthrustLERPSegment.fleetCruiseAcceleration_mps2 * num;
						if (microthrustLERPSegment.barycenter.mu / (num2 * num2) + microthrustLERPSegment.endRadiusCorrection_m <= 0.0)
						{
							Log.Error("Caught negative radius microthrust on load -- removing radius correction to avoid future crashes.", Array.Empty<object>());
							microthrustLERPSegment.endRadiusCorrection_m = 0.0;
						}
					}
				}
			}
		}

		// Token: 0x060043D2 RID: 17362 RVA: 0x001B6920 File Offset: 0x001B4B20
		private void RepairPotentialTrajectoryNullFleetReference_PreVisualizer()
		{
			if (this.transferAssigned)
			{
				if (this.trajectory.fleet != null)
				{
					TISpaceFleetState tispaceFleetState = (TISpaceFleetState)this.trajectory.fleet;
					if (tispaceFleetState != null && !tispaceFleetState.deleted)
					{
						return;
					}
				}
				this.trajectory.fleet = this;
			}
		}

		// Token: 0x060043D3 RID: 17363 RVA: 0x001B696C File Offset: 0x001B4B6C
		private void RepairPotentialInvalidTrajectoryFleetReference()
		{
			Trajectory trajectory = this.trajectory;
			TISpaceFleetState tispaceFleetState = ((trajectory != null) ? trajectory.fleet : null) as TISpaceFleetState;
			if (!this.transferAssigned || tispaceFleetState == this)
			{
				return;
			}
			if (tispaceFleetState == null || tispaceFleetState.deleted)
			{
				Debug.LogWarning("Trajectory had null or deleted fleet.  This should have been repaired before initializing the visualizer.");
				this.trajectory.fleet = this;
				return;
			}
			this.AssignTrajectory(this.trajectory.ShallowCopy(this));
		}

		// Token: 0x060043D4 RID: 17364 RVA: 0x001B69E0 File Offset: 0x001B4BE0
		private void RepairPotentialInvalidCommonBarycenterForTrajectory()
		{
			Trajectory_Patched trajectory_Patched = this.trajectory as Trajectory_Patched;
			if (trajectory_Patched != null)
			{
				trajectory_Patched.RecalculateCommonBarycenter();
			}
		}

		// Token: 0x060043D5 RID: 17365 RVA: 0x001B6A04 File Offset: 0x001B4C04
		private void RepairPotentialIllegalOrbit()
		{
			if (this.ref_orbit != null && this.ref_orbit.eccentricity < 1.0 && this.ref_orbit.eccentricity >= 0.0 && this.ref_orbit.semiMajorAxis_m > 0.0)
			{
				return;
			}
			if (this.trajectory != null && this.trajectory.launched)
			{
				return;
			}
			if (this.trajectory != null)
			{
				OrbitalElementsState orbitalElementsAtTime = this.trajectory.GetOrbitalElementsAtTime(this.trajectory.launchTime);
				TINaturalSpaceObjectState barycenterAtTime = this.trajectory.GetBarycenterAtTime(this.trajectory.launchTime);
				if (orbitalElementsAtTime.eccentricity < 1.0)
				{
					TIOrbitState closestMatchingLegalOrbitState = barycenterAtTime.GetClosestMatchingLegalOrbitState(orbitalElementsAtTime);
					base.AssumeOrbitFromState(closestMatchingLegalOrbitState, orbitalElementsAtTime.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElementsAtTime.epoch));
					return;
				}
				CartesianState cartesianState = this.trajectory.ToGlobalCartesianStateAtTime(this.trajectory.launchTime).ToLocal(barycenterAtTime, this.trajectory.launchTime);
				double magnitude = cartesianState.position.magnitude;
				double num = Mathd.Sqrt(barycenterAtTime.mu / magnitude);
				Vector3d normalized = cartesianState.velocity.normalized;
				Vector3d normalized2 = cartesianState.position.normalized;
				Vector3d normalized3 = (normalized - Vector3d.Dot(in normalized, in normalized2) * normalized2).normalized;
				if (normalized3.sqrMagnitude < 0.25)
				{
					normalized3 = new Vector3d(1f, 0f, 0f);
				}
				Vector3d vector3d = normalized3 * num;
				CartesianState cartesianState2 = new CartesianState(cartesianState.position, vector3d);
				OrbitalElementsState orbitalElementsState = cartesianState2.ToOrbitalElementsState(barycenterAtTime.mu, new DateTime?(this.trajectory.launchTime.ExportTime()));
				TIOrbitState closestMatchingLegalOrbitState2 = barycenterAtTime.GetClosestMatchingLegalOrbitState(orbitalElementsState);
				base.AssumeOrbitFromState(closestMatchingLegalOrbitState2, orbitalElementsState.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElementsState.epoch));
				return;
			}
			else
			{
				if (this.barycenter == null || this.barycenter.isSun)
				{
					TIOrbitState tiorbitState = GameStateManager.IterateByClass<TIOrbitState>(false).First<TIOrbitState>((TIOrbitState orbit) => orbit.barycenter.isEarth);
					base.AssumeOrbitFromState(tiorbitState, 0.0, null);
					return;
				}
				OrbitalElementsState orbitalElementsState2;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				this.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState, out flag);
				double num2 = double.PositiveInfinity;
				TIOrbitState tiorbitState2 = null;
				double semiMajorAxis_m = orbitalElementsState2.semiMajorAxis_m;
				foreach (TIOrbitState tiorbitState3 in this.barycenter.orbits)
				{
					double num3 = Mathd.Abs(semiMajorAxis_m - tiorbitState3.semiMajorAxis_m);
					if (num3 < num2)
					{
						num2 = num3;
						tiorbitState2 = tiorbitState3;
					}
				}
				base.AssumeOrbitFromState(tiorbitState2, 0.0, null);
				return;
			}
		}

		// Token: 0x060043D6 RID: 17366 RVA: 0x001B6CF0 File Offset: 0x001B4EF0
		public override void PostAllStartUpInit_5()
		{
			if (this.gameStateSubjectCreated)
			{
				if (this.huntingXenofauna)
				{
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CheckAutoBombardXenofauna), this.bombardXenofaunaCheck, null, true, false);
				}
				if (this.CurrentOperations().Any<OperationData>((OperationData x) => x.operation.GetTemplate() is BombardOperation))
				{
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.FireMission), this.FireMissionEventName, null, true, false);
					if (this.bombardmentTarget == null || this.bombardmentTargetBracketStatus == TISpaceFleetState.BombardmentBracketingStatus.NotBracketed)
					{
						if (this.bombardmentTarget != null)
						{
							this.gameTime.CancelTimeEvents(this.FireMissionEventName, this, this.bombardmentTarget, null);
						}
						OperationData operationData = this.CurrentOperations().First<OperationData>((OperationData x) => x.operation.GetTemplate() is BombardOperation);
						TIGameState target = operationData.target;
						if (target != null && target.exists)
						{
							this.InitiateBombardment(target, true, (operationData.operation as BombardOperation).bombardmentAltitude_km(target.ref_spaceBody));
						}
					}
					if (this.timeOfLastFireMission == null)
					{
						this.timeOfLastFireMission = TITimeState.Now();
					}
				}
				if (this.currentOperations.Count != this.CurrentOperations().Count)
				{
					this.currentOperations.RemoveAll((OperationData x) => x.operation == null);
				}
			}
			if (this.unavailableForOperations && (this.returnToOperationsTime == null || this.returnToOperationsTime <= TITimeState.Now()))
			{
				this.CombatRecovery();
				Log.Debug("Fixed bad return to ops time. Something went wrong in a battle.", Array.Empty<object>());
			}
			if (!base.faction.fleets.Contains(this))
			{
				base.faction.fleets.Add(this);
				Log.Error(this.displayName + " faction did not contain record of this fleet in fleets. Fixing " + base.ID.ToString(), Array.Empty<object>());
			}
			this.gameStateSubjectCreated = true;
			if (this.bombarding && this.bombardmentTarget.deleted)
			{
				Log.Error(this.displayName + " bombarding deleted game state. Fixing " + base.ID.ToString(), Array.Empty<object>());
				this.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.NotForDisplay);
			}
			this.RepairPotentialTrajectoryNullFleetReference_PreVisualizer();
		}

		// Token: 0x060043D7 RID: 17367 RVA: 0x001B6F5C File Offset: 0x001B515C
		public override void PostVisualizerCreationInit_7()
		{
			base.PostVisualizerCreationInit_7();
			if (this.transferAssigned)
			{
				if (this.trajectory.destinationFleet != null && this.trajectory.destinationFleet.GetFleetsWeAreIntercepting(false).Contains(this) && this.trajectory.launched)
				{
					this.trajectory.DestinationDestroyed();
					if (this.trajectory.destinationFleet != null)
					{
						base.gameObjectLink.Remove<TransferPlanComponent>(true);
						if (this.trajectory.originOrbit != null)
						{
							base.AssumeOrbitFromState(this.trajectory.originOrbit, 0.0, null);
						}
						else
						{
							OrbitalElementsState orbitalElementsAtTime = this.trajectory.GetOrbitalElementsAtTime(this.trajectory.launchTime);
							TIOrbitState closestMatchingLegalOrbitState = this.trajectory.commonBarycenter.GetClosestMatchingLegalOrbitState(orbitalElementsAtTime);
							base.AssumeOrbitFromState(closestMatchingLegalOrbitState, orbitalElementsAtTime.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElementsAtTime.epoch));
						}
						Log.Error("Teleporting " + this.displayName + " to " + base.orbitState.displayName, Array.Empty<object>());
						this.RemoveTransfer();
						return;
					}
				}
				if (this.inTransfer)
				{
					this.LaunchFleet(true);
				}
				else
				{
					if (this.trajectory.fleet != this)
					{
						if (this.trajectory.fleet == null)
						{
							Debug.LogWarning(this.displayName + "'s trajectory's back pointer pointed to null.");
						}
						else if (this.trajectory.fleetAsSpaceFleetState != null)
						{
							Debug.LogWarning(this.displayName + "'s trajectory's back pointer pointed to " + this.trajectory.fleetAsSpaceFleetState.displayName);
						}
						else
						{
							Debug.LogWarning(this.displayName + "'s trajectory's back pointer pointed to a virtual fleet");
						}
					}
					this.trajectory.fleetAsSpaceFleetState = this;
					this.trajectory.fleet = this;
				}
			}
			if (this.transferAssigned && this.trajectory.arrivalTime < TITimeState.Now())
			{
				if (!this.inTransfer)
				{
					this.RemoveTransfer();
				}
				else
				{
					string displayName = this.displayName;
					string text = " has a trajectory that completed before the time of load.  Arriving ";
					TISpaceGameState destination = this.trajectory.destination;
					Log.Error(displayName + text + ((destination != null) ? destination.displayName : null), Array.Empty<object>());
					if ((this.trajectory.destination == null || this.trajectory.destination.deleted || this.trajectory.destination.archived) && !this.trajectory.destroyOnArrival)
					{
						this.trajectory.ReconstructMissingDestinationOrbit();
					}
					Trajectory trajectory = this.trajectory;
					for (int i = 0; i < 10; i++)
					{
						trajectory = trajectory.nextTrajectory;
						if (trajectory == null)
						{
							break;
						}
						if ((trajectory.destination == null || trajectory.destination.deleted || trajectory.destination.archived) && !trajectory.destroyOnArrival)
						{
							trajectory.ReconstructMissingDestinationOrbit();
						}
					}
					this.ArriveFleet(true);
				}
			}
			this.RepairPotentialIllegalOrbit();
			this.RepairPotentialTrajectoryNullFleetReference_PreVisualizer();
			this.RepairPotentialMissingDestination();
			this.RepairPotentialNonsenseNextTrajectory();
			this.RepairPotentialLaunchInconsistency();
			this.RepairPotentialInvalidBurn();
			this.RepairPotentialInconsistendOperationTargetAndTransferDestination();
			this.RepairPotentialBrokenBurn();
		}

		// Token: 0x060043D8 RID: 17368 RVA: 0x001B727C File Offset: 0x001B547C
		public override void PostEverythingSaveRepair_8()
		{
			if (this.dummyFleet && this.ships.Count == 0 && this.combatState == null)
			{
				Log.Error("Disbanding illegal dummy fighter fleet.", Array.Empty<object>());
				this.Disband();
			}
			if (this.transferAssigned && !TIGameState.Valid(this.trajectory.originOrbit))
			{
				Log.Error("Invalid origin orbit or barycenter for " + this.displayName, Array.Empty<object>());
				this.trajectory.ReconstructMissingOriginOrbit();
			}
		}

		// Token: 0x060043D9 RID: 17369 RVA: 0x001B7300 File Offset: 0x001B5500
		private void RepairPotentialLaunchInconsistency()
		{
			if (this.trajectory != null && this.trajectory.launchTime > TITimeState.Now() && this.trajectory.launched)
			{
				if (base.controller != null)
				{
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
				}
				new TransferOperation().OnOperationConfirm(this, this.trajectory.destination, null, this.trajectory);
				this.trajectory.launched = false;
				if (base.orbitState == null)
				{
					base.AssumeOrbitFromState(this.trajectory.originOrbit, 0.0, null);
				}
			}
		}

		// Token: 0x060043DA RID: 17370 RVA: 0x001B73AC File Offset: 0x001B55AC
		private void RepairPotentialMissingDestination()
		{
			if (this.trajectory == null)
			{
				return;
			}
			if (this.trajectory.destinationFleet != null && (this.trajectory.destinationFleet.deleted || this.trajectory.destinationFleet.archived))
			{
				this.trajectory.ReconstructMissingDestinationOrbit();
				return;
			}
			if (this.trajectory.destination == null)
			{
				this.trajectory.ReconstructMissingDestinationOrbit();
			}
		}

		// Token: 0x060043DB RID: 17371 RVA: 0x001B7424 File Offset: 0x001B5624
		private void RepairPotentialNonsenseNextTrajectory()
		{
			Trajectory trajectory = this.trajectory;
			if (((trajectory != null) ? trajectory.nextTrajectory : null) == null)
			{
				return;
			}
			if (this.trajectory.nextTrajectory.arrivalTime < this.trajectory.arrivalTime)
			{
				this.trajectory.nextTrajectory = null;
				this.trajectory.ReconstructMissingDestinationOrbit();
			}
		}

		// Token: 0x060043DC RID: 17372 RVA: 0x001B7480 File Offset: 0x001B5680
		private void RepairPotentialBrokenBurn()
		{
			if (this.trajectory == null)
			{
				return;
			}
			Trajectory_Patched trajectory_Patched = this.trajectory as Trajectory_Patched;
			if (trajectory_Patched == null)
			{
				return;
			}
			for (int i = 0; i < trajectory_Patched.Segments.Count<Trajectory_Patched.IPatchSegment>(); i++)
			{
				Trajectory_Patched.BurnSegment burnSegment = trajectory_Patched.Segments[i] as Trajectory_Patched.BurnSegment;
				if (burnSegment != null)
				{
					if (i + 1 >= trajectory_Patched.Segments.Count<Trajectory_Patched.IPatchSegment>())
					{
						break;
					}
					Trajectory_Patched.IPatchSegment patchSegment = trajectory_Patched.Segments[i + 1];
					if (!(patchSegment.startTime <= burnSegment.endTime))
					{
						Log.Error("Fleet " + this.displayName + " has a burn that extends past its end time.  Reconstructing burn from surrounding segments.", Array.Empty<object>());
						TINaturalSpaceObjectState barycenter = burnSegment.barycenter;
						CartesianState cartesianState;
						if (i > 0)
						{
							cartesianState = trajectory_Patched.Segments[i - 1].GlobalCartesianStateAtTime(burnSegment.startTime).ToLocal(barycenter, burnSegment.startTime);
						}
						else
						{
							cartesianState = burnSegment.GlobalCartesianStateAtTime(burnSegment.startTime).ToLocal(barycenter, burnSegment.startTime);
						}
						CartesianState cartesianState2 = patchSegment.GlobalCartesianStateAtTime(patchSegment.startTime).ToLocal(barycenter, patchSegment.startTime);
						double num = patchSegment.startTime.DifferenceInSeconds(burnSegment.startTime);
						Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
						{
							startTime = burnSegment.startTime,
							burnDuration_s = num,
							fleetAccel_mps2 = burnSegment.fleetAccel_mps2,
							isBoost = burnSegment.isBoost,
							isImpulse = burnSegment.isImpulse,
							isTorch = burnSegment.isTorch,
							isOrbitPhasing = burnSegment.isOrbitPhasing,
							barycenter = burnSegment.barycenter,
							burnDescription = new BurnBezierDescription(cartesianState, cartesianState2, num)
						};
						trajectory_Patched.Segments[i] = burnSegment2;
					}
				}
			}
		}

		// Token: 0x060043DD RID: 17373 RVA: 0x001B7640 File Offset: 0x001B5840
		private void RepairPotentialInvalidBurn()
		{
			if (this.trajectory != null)
			{
				Trajectory_Patched trajectory_Patched = this.trajectory as Trajectory_Patched;
				if (trajectory_Patched != null)
				{
					for (int i = 0; i < trajectory_Patched.Segments.Count<Trajectory_Patched.IPatchSegment>(); i++)
					{
						Trajectory_Patched.BurnSegment burnSegment = trajectory_Patched.Segments[i] as Trajectory_Patched.BurnSegment;
						if (burnSegment != null)
						{
							double magnitude = burnSegment.burnDescription.endPosition.magnitude;
							if (magnitude < burnSegment.barycenter.meanRadius_m || magnitude < 1.0)
							{
								Log.Error("Fleet " + this.displayName + " has a trajectory with a burn that ends within its barycenter.  Correcting.", Array.Empty<object>());
								CartesianState cartesianState;
								if (i < trajectory_Patched.Segments.Count - 1)
								{
									Trajectory_Patched.IPatchSegment patchSegment = trajectory_Patched.Segments[i + 1];
									cartesianState = patchSegment.GlobalCartesianStateAtTime(patchSegment.startTime).ToLocal(burnSegment.barycenter, patchSegment.startTime);
								}
								else
								{
									cartesianState = trajectory_Patched.DestinationCartesianStateAtTime(trajectory_Patched.arrivalTime).ToLocal(burnSegment.barycenter, trajectory_Patched.arrivalTime);
								}
								double magnitude2 = cartesianState.position.magnitude;
								if (magnitude2 < burnSegment.barycenter.meanRadius_m || magnitude2 < 1.0)
								{
									if (magnitude2 > 0.0)
									{
										cartesianState.position *= burnSegment.barycenter.meanRadius_m / magnitude2;
									}
									else
									{
										Vector3d vector3d = burnSegment.burnDescription.startVelocityControlPoint - burnSegment.burnDescription.startPosition;
										if (vector3d.magnitude == 0.0)
										{
											vector3d = new Vector3d(1f, 0f, 0f);
										}
										cartesianState.position = vector3d * (burnSegment.barycenter.meanRadius_m / vector3d.magnitude);
									}
								}
								burnSegment.burnDescription.endPosition = cartesianState.position;
								Vector3d vector3d2 = cartesianState.velocity * (burnSegment.burnDuration_s / 3.0);
								burnSegment.burnDescription.endVelocityControlPoint = cartesianState.position - vector3d2;
							}
						}
					}
				}
			}
		}

		// Token: 0x060043DE RID: 17374 RVA: 0x001B7868 File Offset: 0x001B5A68
		private void RepairPotentialInconsistendOperationTargetAndTransferDestination()
		{
			if (this.trajectory == null)
			{
				return;
			}
			OperationData operationData = this.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is TransferOperation);
			if (operationData == null)
			{
				return;
			}
			if (operationData.target != this.trajectory.destination)
			{
				operationData.ChangeTarget(this.trajectory.destination);
			}
		}

		// Token: 0x060043DF RID: 17375 RVA: 0x001B78D8 File Offset: 0x001B5AD8
		public void OnFleetCreated()
		{
			this.fleetOperationCompleteName = new StringBuilder("FleetOperationComplete").Append(base.ID.ToString()).ToString();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), this.fleetOperationCompleteName, null, true, false);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CombatRecovery), this.CombatRecoveryEventName, null, true, false);
			if (this.currentOperations == null)
			{
				this.currentOperations = new List<OperationData>();
			}
			if (this.displayNameByFaction == null)
			{
				this.displayNameByFaction = new Dictionary<TIFactionState, string>();
			}
			if (this.AI_FailedAttackEnemyStrength == null)
			{
				this.AI_FailedAttackEnemyStrength = new Dictionary<TIGameState, float>();
			}
			this.AssignFormation(this.DefaultFormation(), false, false, false, false, false);
		}

		// Token: 0x060043E0 RID: 17376 RVA: 0x001B799C File Offset: 0x001B5B9C
		public void InitializeRunTimeFleetData(bool suppressLogging)
		{
			if (!this.gameStateSubjectCreated && !suppressLogging)
			{
				foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
				{
					tifactionState.SetIntel(this, base.IntelOnCreation(tifactionState, base.faction), null, false);
				}
			}
			if (this.waitingToInitiateCombatDatas != null)
			{
				if (this.waitingToInitiateCombatDatas.Count > 0)
				{
					TISpaceFleetState.fleetsWaitingToInitiateCombat.Add(this);
					return;
				}
				this.waitingToInitiateCombatDatas = null;
			}
		}

		// Token: 0x060043E1 RID: 17377 RVA: 0x001B7A30 File Offset: 0x001B5C30
		public void SetFaction(TIFactionState newFaction)
		{
			base.faction = newFaction;
		}

		// Token: 0x060043E2 RID: 17378 RVA: 0x001B7A39 File Offset: 0x001B5C39
		public FactionGoal_Fleet AssignedGoal()
		{
			TIFactionState faction = base.faction;
			if (faction == null || !faction.fleetGoalTracker.ContainsKey(this))
			{
				return null;
			}
			return base.faction.fleetGoalTracker[this];
		}

		// Token: 0x060043E3 RID: 17379 RVA: 0x001B7A68 File Offset: 0x001B5C68
		public static TISpaceFleetState CreateAtRunTime(TIFactionState faction, List<TISpaceShipState> ships, TIGameState location, TISpaceFleetState parentFleet, FactionGoal_Fleet AIBuiltForGoal = null, bool theft = false, bool spawnedFighters = false, Trajectory trajectory = null)
		{
			TISpaceFleetTemplate tispaceFleetTemplate = TemplateManager.Find<TISpaceFleetTemplate>("GenericFleetTemplate", false);
			if (tispaceFleetTemplate == null)
			{
				tispaceFleetTemplate = new TISpaceFleetTemplate("GenericFleetTemplate");
				TemplateManager.Add(tispaceFleetTemplate, typeof(TISpaceFleetTemplate), false);
			}
			TISpaceFleetState tispaceFleetState = (TISpaceFleetState)tispaceFleetTemplate.CreateGameState();
			tispaceFleetState.templateName = tispaceFleetTemplate.dataName;
			tispaceFleetState.Initialize();
			tispaceFleetState.OnFleetCreated();
			faction.AddFleet(tispaceFleetState);
			tispaceFleetState.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (!spawnedFighters)
			{
				tispaceFleetState.SetDisplayName(faction, null, false);
			}
			bool flag = location.ref_hab != null && (location.ref_hab.IsBase || location.ref_hab.dockedFleets.All<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(faction)));
			tispaceFleetState.parentFleet = parentFleet;
			if (!(parentFleet != null) || spawnedFighters)
			{
				tispaceFleetState.AddShipsToFleet(ships, null, false, false);
				if (location.isHabState)
				{
					if (flag)
					{
						tispaceFleetState.Dock(location.ref_hab, true);
					}
					else
					{
						tispaceFleetState.AssumeMatchingOrbitFromState(location.ref_hab, false);
					}
				}
				else if (location.isOrbitState)
				{
					if (parentFleet != null)
					{
						tispaceFleetState.AssumeMatchingOrbitFromState(parentFleet, false);
					}
					else
					{
						tispaceFleetState.AssumeOrbitFromState(location.ref_orbit, 0.0, null);
					}
				}
				tispaceFleetState.CreateVisualizer(tispaceFleetState.template);
			}
			else
			{
				TISpaceFleetState ref_fleet = location.ref_fleet;
				bool flag2 = false;
				float num = 0f;
				if (trajectory != null)
				{
					tispaceFleetState.AssignTrajectory(trajectory);
					num = (float)ref_fleet.fleetTrajectoryData.initialDeltaV_mps - ref_fleet.currentDeltaV_mps;
					flag2 = true;
				}
				else if ((ref_fleet != null) ? ref_fleet.dockedAtHab : location.isHabState)
				{
					if (flag)
					{
						tispaceFleetState.Dock(location.ref_hab, true);
					}
					else
					{
						tispaceFleetState.AssumeMatchingOrbitFromState(location.ref_hab, false);
					}
				}
				else if (ref_fleet.landedInOutback)
				{
					TIHabSiteState ref_habSite = location.ref_habSite;
					tispaceFleetState.AssumeOrbitFromState(ref_habSite.parentBody.interfaceOrbits[0], 0.0, TITimeState.Now());
					tispaceFleetState.dockedLocation = ref_habSite;
					tispaceFleetState.AssignFormation(tispaceFleetState.dockedFormation, false, true, false, false, false);
					tispaceFleetState.TeleportAllToFormation(false, false);
					ref_habSite.LandFleet(tispaceFleetState);
					GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(tispaceFleetState, ref_habSite, true), null, new object[] { tispaceFleetState, ref_habSite, ref_habSite.parentBody });
				}
				else if (!ref_fleet.inTransfer)
				{
					tispaceFleetState.AssumeMatchingOrbitFromState(ref_fleet, false);
				}
				else
				{
					tispaceFleetState.AssignTrajectory(ref_fleet.trajectory.ShallowCopy(null));
					tispaceFleetState._fleetTrajectoryData = new FleetTrajectoryData();
					num = (float)ref_fleet.fleetTrajectoryData.initialDeltaV_mps - ref_fleet.currentDeltaV_mps;
					float num2 = ships.Min<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps) * 1000f;
					tispaceFleetState._fleetTrajectoryData.initialDeltaV_mps = (double)(num2 + num);
				}
				tispaceFleetState.CreateVisualizer(tispaceFleetState.template);
				tispaceFleetState.AddShipsToFleet(ships, ref_fleet, false, false);
				if (flag2)
				{
					ref_fleet.fleetTrajectoryData.initialDeltaV_mps = (double)(ref_fleet.currentDeltaV_mps + num);
				}
			}
			tispaceFleetState.InitializeRunTimeFleetData(spawnedFighters || ships.Count == 0);
			tispaceFleetState.controller.UpdateOrbitComponentForAsset(false);
			tispaceFleetState.gameStateSubjectCreated = true;
			if (AIBuiltForGoal != null && !AIBuiltForGoal.skipGoal && AIBuiltForGoal.ValidNewGoal())
			{
				if (AIBuiltForGoal.assignedFleet == null)
				{
					AIBuiltForGoal.AssignFleet(tispaceFleetState);
				}
				else
				{
					faction.AddGoal(new FactionGoal_JoinFleet(faction, AIBuiltForGoal.assignedFleet), HandleDuplicateGoalRule.ResetImportanceIfHigher, tispaceFleetState);
				}
			}
			tispaceFleetState.AddFleetLog("Created");
			if (trajectory != null)
			{
				tispaceFleetState.LaunchFleet(true);
			}
			return tispaceFleetState;
		}

		// Token: 0x060043E4 RID: 17380 RVA: 0x001B7E25 File Offset: 0x001B6025
		public override void CreateVisualizer(TIDataTemplate myTemplate)
		{
			base.CreateVisualizer(myTemplate);
			if (this.trajectory != null)
			{
				this.RefreshTrajectory();
			}
		}

		// Token: 0x060043E5 RID: 17381 RVA: 0x001B7E3C File Offset: 0x001B603C
		public void Disband()
		{
			TIAdHocOrbitState tiadHocOrbitState = this.ref_orbit as TIAdHocOrbitState;
			Trajectory trajectory = this.trajectory;
			TIAdHocOrbitState tiadHocOrbitState2 = ((trajectory != null) ? trajectory.originOrbit : null) as TIAdHocOrbitState;
			if (this.ships.Count < 1)
			{
				if (!GameControl.control.skirmishMode)
				{
					this.ForceCancelCurrentOperations();
					foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
					{
						tifactionState.ExpireIntel(this, true);
						using (List<Alarm>.Enumerator enumerator2 = tifactionState.alarms.ToList<Alarm>().GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.associatedGameState == this)
								{
									tifactionState.playerControl.StartAction(new DeleteFleetAlarm(tifactionState, this));
								}
							}
						}
					}
					if (this.landed)
					{
						this.ref_habSite.landedFleets.Remove(this);
					}
					if (this.dockedAtHab)
					{
						this.ref_hab.RemoveDockedFleet(this);
					}
					TIOrbitState orbitState = base.orbitState;
					if (orbitState != null)
					{
						orbitState.assetsInOrbit.Remove(this);
					}
					foreach (TISpaceFleetState tispaceFleetState in GameStateManager.IterateByClass<TISpaceFleetState>(true).ToList<TISpaceFleetState>())
					{
						Trajectory trajectory2 = tispaceFleetState.trajectory;
						int num = 100;
						while (trajectory2 != null && num > 0)
						{
							num--;
							if (tispaceFleetState.trajectory.destination == this)
							{
								if (tispaceFleetState.inTransfer)
								{
									trajectory2.DestinationDestroyed();
									if (tispaceFleetState.trajectory == trajectory2)
									{
										tispaceFleetState.RefreshTrajectory();
										GameControl.eventManager.TriggerEvent(new StartFleetOperation(tispaceFleetState, TemplateManager.Find<TIOperationTemplate>("TransferOperation", true), tispaceFleetState.trajectory.destinationOrbit), null, Array.Empty<object>());
									}
								}
								else
								{
									tispaceFleetState.ForceCancelCurrentOperations();
									tispaceFleetState.gameObjectLink.Remove<TransferPlanComponent>(true);
									tispaceFleetState.RemoveTransfer();
								}
							}
							trajectory2 = trajectory2.nextTrajectory;
						}
						if (tispaceFleetState.transferAssigned)
						{
							tispaceFleetState.trajectory.destination == this;
						}
					}
					foreach (TICouncilorState ticouncilorState in GameStateManager.IterateByClass<TICouncilorState>(false).ToList<TICouncilorState>())
					{
						TIMissionState activeMission = ticouncilorState.activeMission;
						if (((activeMission != null) ? activeMission.target : null) == this)
						{
							ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.TargetFleetDestroyed, "");
						}
					}
					TINotificationQueueState.CleanQueueOfArchivedState(this, null);
					TIFactionState[] array = GameStateManager.AllFactions();
					for (int i = 0; i < array.Length; i++)
					{
						array[i].CleanStateFromGoalTargets(this);
					}
				}
				TIFactionState faction = base.faction;
				if (faction != null)
				{
					faction.RemoveFleet(this);
				}
				World.Active.GetExistingManager<GameTimeManager>().CancelAllTimeEventsForObject(this);
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.FireMission), this.FireMissionEventName);
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.OnTimedOperationComplete), this.fleetOperationCompleteName);
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CombatRecovery), this.CombatRecoveryEventName);
				GameControl.eventManager.TriggerEvent(new FleetDisbanded(this), null, new object[] { this, this.ref_hab, this.ref_habSite }.Where<object>((object x) => x != null).ToArray<object>());
				base.ArchiveState(true);
				this.HandleSpaceObjectSelection(true);
				GameStateManager.RemoveGameState<TISpaceFleetState>(base.ID, false);
				if (base.controller != null && base.gameObjectLink != null)
				{
					base.gameObjectLink.Remove<FleetComponent>(true);
					base.gameObjectLink.Remove<SpaceObjectComponent>(true);
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
					if (base.controller.orbitTrailLink != null)
					{
						global::UnityEngine.Object.Destroy(base.controller.orbitTrailLink);
					}
				}
				this.RemoveTransfer();
			}
			this.TryToRemoveAdHocOrbit(tiadHocOrbitState);
			this.TryToRemoveAdHocOrbit(tiadHocOrbitState2);
		}

		// Token: 0x17000D1C RID: 3356
		// (get) Token: 0x060043E6 RID: 17382 RVA: 0x001B8288 File Offset: 0x001B6488
		public string CombatRecoveryEventName
		{
			get
			{
				return new StringBuilder("FleetUnavailable").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x060043E7 RID: 17383 RVA: 0x001B82C0 File Offset: 0x001B64C0
		public void PostCombat(TISpaceCombatState combat, double combatDuration_s, bool relocate)
		{
			this.inCombat = false;
			if (combatDuration_s > 86400.0 || double.IsNaN(combatDuration_s) || double.IsInfinity(combatDuration_s))
			{
				combatDuration_s = 86400.0;
			}
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			list = this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.storedFaction != null && x.storedFaction != base.faction).ToList<TISpaceShipState>();
			if (base.faction != null && list.Count > 0)
			{
				TISpaceFleetState tispaceFleetState = TISpaceFleetState.CreateAtRunTime(list[0].storedFaction, list, this, this, null, false, false, null);
				tispaceFleetState.PostCombat(combat, combatDuration_s, relocate);
				tispaceFleetState.ships.ForEach(delegate(TISpaceShipState x)
				{
					x.storedFaction = null;
				});
			}
			TIFactionState tifactionState = combat.factions.FirstOrDefault<TIFactionState>((TIFactionState x) => x != base.faction);
			foreach (TISpaceShipState tispaceShipState in this.ships.ToList<TISpaceShipState>())
			{
				if (tispaceShipState.ShipDestroyed())
				{
					tispaceShipState.DestroyShip(true, tifactionState);
				}
				else
				{
					tispaceShipState.PostCombat(true);
				}
			}
			if (this.ships.Count == 0)
			{
				this.Disband();
				return;
			}
			if (relocate)
			{
				double num = Mathd.Min(1000.0 / base.orbitState.semiMajorAxis_km, 2.5);
				double num2 = this._meanAnomalyAtEpoch_Rad - num;
				if (num2 < 0.0)
				{
					num2 += 6.283185307179586;
				}
				base.SetNewOrbitalElements(num2, base.epoch_DateTime);
			}
			this.formation = this.DefaultFormation();
			this.TeleportAllToFormation(false, true);
			this.combatState = null;
			if (combatDuration_s > 0.0)
			{
				if (this.unavailableForOperations)
				{
					this.gameTime.ExtendTimeEvent(this.CombatRecoveryEventName, this, null, null, (int)combatDuration_s, TITimeQueueRepeatType.Second);
					this.returnToOperationsTime.AddSeconds((double)((int)combatDuration_s));
					if (this.returnToOperationsTime <= TITimeState.Now())
					{
						Log.Debug("Bad combat return to ops time, location 1:" + this.returnToOperationsTime.ToLongTimeString(), Array.Empty<object>());
						this.CombatRecovery();
					}
				}
				else
				{
					TIDateTime tidateTime = TITimeState.Now();
					tidateTime.AddSeconds(combatDuration_s);
					TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, this.CombatRecoveryEventName, true, false, TITimeQueueRepeatType.None, 1, true, false);
					this.returnToOperationsTime = new TIDateTime(tidateTime);
					this.unavailableForOperations = true;
					GameControl.eventManager.TriggerEvent(new FleetAvailabilityChange(this), null, new object[] { this });
					if (this.returnToOperationsTime <= TITimeState.Now())
					{
						Log.Debug("Bad combat return to ops time, location 2:" + this.returnToOperationsTime.ToLongTimeString(), Array.Empty<object>());
						this.CombatRecovery();
					}
				}
			}
			if (combat.hab != null)
			{
				if (((combat.winner == base.faction && (combat.hab.faction == base.faction || combat.hab.ActiveCombatModules().Count == 0)) || (combat.draw && combat.hab.faction == base.faction)) && !this.dockedOrLanded && combat.hab.dockedFleets.None<TISpaceFleetState>((TISpaceFleetState x) => !base.faction.permanentAlly(x.faction)))
				{
					this.Dock(combat.hab, false);
				}
				if ((combat.loser == base.faction && (combat.hab.faction != base.faction || combat.hab.ActiveCombatModules().Count == 0)) || (combat.draw && combat.hab.faction != base.faction))
				{
					if (this.dockedOrLanded)
					{
						this.DepartFromDockingLocation();
					}
					base.AssumeMatchingOrbitFromState(combat.hab, false);
					return;
				}
			}
			else
			{
				if (this.allShipsHaveDeltaV && this.allShipsCanManeuver && this.cruiseAcceleration_mps2 > 0f && this.transferAssigned && this.trajectory != null && combat.fleets[0] == this && (this.trajectory.destinationFleet != null || this.trajectory.interceptTrajectory))
				{
					this.VerifyAssignedTransfer(true);
					TIPromptQueueState.AddPromptStatic(base.faction, this, null, "PromptChangeTrajectory", 0);
				}
				else
				{
					this.VerifyAssignedTransfer(false);
				}
				IEnumerable<TISpaceFleetState> destroyedFleets = combat.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Count<TISpaceShipState>() == 0);
				foreach (TISpaceFleetState tispaceFleetState2 in GameStateManager.IterateByClass<TISpaceFleetState>(false).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					IEnumerable<TISpaceFleetState> destroyedFleets2 = destroyedFleets;
					Trajectory trajectory = x.trajectory;
					return destroyedFleets2.Contains((trajectory != null) ? trajectory.prevDestinationFleet : null);
				}))
				{
					TIPromptQueueState.AddPromptStatic(tispaceFleetState2.faction, tispaceFleetState2, null, "PromptChangeTrajectory", 0);
				}
			}
		}

		// Token: 0x060043E8 RID: 17384 RVA: 0x001B87C4 File Offset: 0x001B69C4
		public void CombatRecovery()
		{
			bool unavailableForOperations = this.unavailableForOperations;
			this.unavailableForOperations = false;
			this.returnToOperationsTime = null;
			if (unavailableForOperations)
			{
				GameControl.eventManager.TriggerEvent(new FleetAvailabilityChange(this), null, new object[] { this });
				TINotificationQueueState.LogFleetAvailableForOperations(this);
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.PostCombatFleetRecovery, this, null);
			}
		}

		// Token: 0x060043E9 RID: 17385 RVA: 0x001B8811 File Offset: 0x001B6A11
		public void CombatRecovery(TimeEventStart e)
		{
			if (TIGameState.Valid(this))
			{
				this.CombatRecovery();
			}
		}

		// Token: 0x060043EA RID: 17386 RVA: 0x001B8824 File Offset: 0x001B6A24
		public List<IOperation> VisibleOperationList(TINaturalSpaceObjectState naturalSpaceObject = null)
		{
			if (this._visibleOperationListCacheFrame != TIFrameCounter.FrameCount)
			{
				this._cachedVisibleOperationList = OperationsManager.fleetOperations.Where<IOperation>((IOperation x) => x.OpVisibleToActor(this, null)).ToList<IOperation>();
				this._visibleOperationListCacheFrame = TIFrameCounter.FrameCount;
			}
			return this._cachedVisibleOperationList;
		}

		// Token: 0x060043EB RID: 17387 RVA: 0x001B8870 File Offset: 0x001B6A70
		public List<IOperation> AllowedOpsList()
		{
			return (from x in this.VisibleOperationList(null)
				where x is TISpaceFleetOperationTemplate_Special
				select x).ToList<IOperation>();
		}

		// Token: 0x060043EC RID: 17388 RVA: 0x001B88A4 File Offset: 0x001B6AA4
		public List<IOperation> AvailableOperationList(TINaturalSpaceObjectState naturalSpaceobject = null)
		{
			if (this.unavailableForOperations)
			{
				return new List<IOperation>();
			}
			if (this._availableOperationListCacheFrame != TIFrameCounter.FrameCount)
			{
				this._cachedAvailableOperationList = (from x in this.VisibleOperationList(null)
					where x.ActorCanPerformOperation(this, null)
					select x).ToList<IOperation>();
				this._availableOperationListCacheFrame = TIFrameCounter.FrameCount;
			}
			return this._cachedAvailableOperationList;
		}

		// Token: 0x060043ED RID: 17389 RVA: 0x001B8900 File Offset: 0x001B6B00
		public List<OperationData> CurrentOperations()
		{
			return this.currentOperations.ToList<OperationData>();
		}

		// Token: 0x060043EE RID: 17390 RVA: 0x001B8910 File Offset: 0x001B6B10
		public void CancelOperation(OperationData operation)
		{
			if ((operation.operation as TISpaceFleetOperationTemplate).ExecuteUponCancel())
			{
				if (operation.operation is BombardOperation)
				{
					this.endBombardmentReason = TISpaceFleetState.EndBombardmentReason.NotForDisplay;
				}
				this.CompleteFleetOperation(operation.operation, operation.target);
				return;
			}
			operation.OnOperationCancel(this);
			this.currentOperations.Remove(operation);
		}

		// Token: 0x060043EF RID: 17391 RVA: 0x001B896C File Offset: 0x001B6B6C
		public void ForceCancelCurrentOperations()
		{
			foreach (OperationData operationData in new List<OperationData>(from x in this.CurrentOperations()
				where x.operation.GetOperationTiming() > OperationTiming.InstantExecution
				select x))
			{
				this.CancelOperation(operationData);
			}
		}

		// Token: 0x060043F0 RID: 17392 RVA: 0x001B89E8 File Offset: 0x001B6BE8
		public void ForceCancelCurrentOperations(TISpaceFleetOperationTemplate operation)
		{
			foreach (OperationData operationData in new List<OperationData>(from x in this.CurrentOperations()
				where x.operation.GetOperationTiming() != OperationTiming.InstantExecution && x.operationDataName == operation.dataName
				select x))
			{
				this.CancelOperation(operationData);
			}
		}

		// Token: 0x060043F1 RID: 17393 RVA: 0x001B8A60 File Offset: 0x001B6C60
		public void OnTimedOperationComplete(TimeEventStart e)
		{
			this.CompleteFleetOperation(e.eventDataTemplate as IOperation, e.eventObject2);
		}

		// Token: 0x060043F2 RID: 17394 RVA: 0x001B8A7C File Offset: 0x001B6C7C
		public void CompleteFleetOperation(IOperation operation, TIGameState target)
		{
			if (operation != null)
			{
				operation.OnOperationExecute(this, target);
				foreach (OperationData operationData in this.currentOperations)
				{
					if (operationData.operation == operation && operationData.target == target)
					{
						this.currentOperations.Remove(operationData);
						GameControl.eventManager.TriggerEvent(new TimeEventComplete(this, null), this.fleetOperationCompleteName, Array.Empty<object>());
						if (operation.GetOperationTiming() == OperationTiming.EffectWithDuration)
						{
							GameControl.eventManager.TriggerEvent(new FleetOperationWithDurationComplete(this), null, new object[] { this });
							break;
						}
						break;
					}
				}
				TISpaceFleetOperationTemplate tispaceFleetOperationTemplate = operation as TISpaceFleetOperationTemplate;
				if (tispaceFleetOperationTemplate != null && tispaceFleetOperationTemplate.UpdatePropulsionOnComplete())
				{
					this.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.SetPropulsionValuesDirty(true, false);
					});
					return;
				}
			}
			else
			{
				Debug.LogError("Tried to complete null operation for fleet:" + base.ID.ToString());
			}
		}

		// Token: 0x060043F3 RID: 17395 RVA: 0x001B8B9C File Offset: 0x001B6D9C
		public bool CanMerge(TISpaceFleetState otherFleet)
		{
			if (otherFleet != this && otherFleet.faction == base.faction && otherFleet.ships.Count > 0 && !otherFleet.inCombatOrWaitingForCombat && !otherFleet.unavailableForOperations)
			{
				if (!otherFleet.CurrentOperations().Any<OperationData>((OperationData x) => x.operation.IsBlockingOperation() && !(x.operation as TISpaceFleetOperationTemplate).BreakthroughOps().Contains(typeof(MergeFleetOperation))))
				{
					if (this.dockedOrLanded && otherFleet.dockedOrLanded && this.dockedLocation == otherFleet.dockedLocation)
					{
						return !this.underBombardment && !otherFleet.underBombardment;
					}
					if ((this.dockedOrLanded && otherFleet.dockedOrLanded) || this.landed || otherFleet.landed)
					{
						return false;
					}
					if (this.transferAssigned != otherFleet.transferAssigned)
					{
						return false;
					}
					if (this.transferAssigned && otherFleet.transferAssigned)
					{
						if (this.trajectory.arrivalTime == otherFleet.trajectory.arrivalTime && this.trajectory.destination == otherFleet.trajectory.destination)
						{
							TIDateTime tidateTime = TITimeState.Now();
							CartesianState cartesianState = this.ToGlobalCartesianStateAtTime(tidateTime);
							CartesianState cartesianState2 = otherFleet.ToGlobalCartesianStateAtTime(tidateTime);
							double magnitude = (cartesianState.position - cartesianState2.position).magnitude;
							double magnitude2 = (cartesianState.velocity - cartesianState2.velocity).magnitude;
							if (magnitude < 1000000.0 && magnitude2 < 1000.0)
							{
								return true;
							}
						}
						return false;
					}
					bool flag = base.orbitState == otherFleet.orbitState;
					if (!flag)
					{
						OrbitalElementsState orbitalElementsState;
						TINaturalSpaceObjectState tinaturalSpaceObjectState;
						bool flag2;
						this.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState, out tinaturalSpaceObjectState, out flag2);
						OrbitalElementsState orbitalElementsState2;
						TINaturalSpaceObjectState tinaturalSpaceObjectState2;
						otherFleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag2);
						if (tinaturalSpaceObjectState == tinaturalSpaceObjectState2 && orbitalElementsState.Approximately(orbitalElementsState2, 0.0))
						{
							flag = true;
						}
					}
					if (flag && (this.ToGlobalCartesianStateAtTime(TITimeState.Now()).position - otherFleet.ToGlobalCartesianStateAtTime(TITimeState.Now()).position).magnitude < 1000000.0)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060043F4 RID: 17396 RVA: 0x001B8DDC File Offset: 0x001B6FDC
		public override List<TISpaceFleetState> GetNearbyIdleAlliedFleets(TIDateTime time = null)
		{
			TISpaceFleetState.<>c__DisplayClass284_0 CS$<>8__locals1 = new TISpaceFleetState.<>c__DisplayClass284_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.time = time;
			if (CS$<>8__locals1.time == null)
			{
				CS$<>8__locals1.time = TITimeState.Now();
			}
			IEnumerable<TISpaceFleetState> enumerable = from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
				where x != CS$<>8__locals1.<>4__this && x.faction == CS$<>8__locals1.<>4__this.faction && (!x.transferAssigned || x.trajectory.launchTime > TITimeState.Now())
				select x;
			if (this.dockedOrLanded)
			{
				return enumerable.Where<TISpaceFleetState>((TISpaceFleetState x) => x.dockedOrLanded && CS$<>8__locals1.<>4__this.dockedLocation == x.dockedLocation).ToList<TISpaceFleetState>();
			}
			TINaturalSpaceObjectState ourBarycenter;
			OrbitalElementsState ourOrbitElements;
			if (!this.transferAssigned || this.trajectory.launchTime >= CS$<>8__locals1.time)
			{
				bool flag;
				this.getOrbitalElementsState(TITimeState.Now(), out ourOrbitElements, out ourBarycenter, out flag);
			}
			else
			{
				ourOrbitElements = this.trajectory.GetOrbitalElementsAtTime(TITimeState.Now());
				ourBarycenter = this.trajectory.GetBarycenterAtTime(TITimeState.Now());
			}
			Vector3d ourLocalPosition = this.ToLocalCartesianStateAtTime(CS$<>8__locals1.time).position;
			return enumerable.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				if (x.dockedOrLanded)
				{
					return false;
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

		// Token: 0x060043F5 RID: 17397 RVA: 0x001B8EFC File Offset: 0x001B70FC
		public void ScuttleShips(List<TISpaceShipState> ships)
		{
			bool dockedAtHab = this.dockedAtHab;
			foreach (TISpaceShipState tispaceShipState in ships.ToList<TISpaceShipState>())
			{
				if (tispaceShipState.ScuttleCost().CanAfford(base.faction, 1f, null, float.PositiveInfinity))
				{
					tispaceShipState.ScuttleCost().PayCost(base.faction, "Scuttle");
					tispaceShipState.DestroyShip(!dockedAtHab, null);
				}
			}
		}

		// Token: 0x060043F6 RID: 17398 RVA: 0x001B8F90 File Offset: 0x001B7190
		public bool MustAcceptCombatAsDefender()
		{
			if (this.dockedAtHab)
			{
				return true;
			}
			return this.CurrentOperations().Any<OperationData>((OperationData x) => (x.operation as TISpaceFleetOperationTemplate).MustAcceptCombat()) || (this.inTransfer && this.trajectory.CantManeuver(null)) || (this.maxAcceleration_mps2 <= 0f || !this.allShipsHaveDeltaV);
		}

		// Token: 0x060043F7 RID: 17399 RVA: 0x001B9008 File Offset: 0x001B7208
		public void InitiateBombardment(TIGameState target, bool fromSave, float altitude_km)
		{
			this.endBombardmentReason = TISpaceFleetState.EndBombardmentReason.None;
			this.bombardmentAltitude_km = altitude_km;
			TIDateTime tidateTime = TITimeState.Now();
			this.timeOfLastFireMission = TITimeState.Now();
			this.bombardmentTarget = target;
			this.bombardmentTargetBracketStatus = TISpaceFleetState.BombardmentBracketingStatus.NotBracketed;
			if (!fromSave)
			{
				this.AddToBombardmentLog(Loc.T("Bombard.Log.Start", new object[]
				{
					tidateTime.ToCustomTimeString(),
					this.GetDisplayName(GameControl.control.activePlayer),
					this.bombardmentTarget.displayName,
					this.bombardmentAltitude_km.ToString("N0")
				}), tidateTime);
				this.firstHitFromBombardmentRun = false;
			}
			if (target.ref_hab != null && target.ref_faction.permanentAlly(target.ref_hab.faction))
			{
				target.ref_hab.SetUnderBombardment();
				target.ref_hab.AddConflictFleet(this);
			}
			else
			{
				TIRegionState ref_region = target.ref_region;
				if (ref_region != null)
				{
					ref_region.UnderBombardment();
				}
			}
			float num = (float)base.orbitState.period_s;
			tidateTime.AddSeconds((double)(1f + num));
			float num2 = (float)base.orbitState.period_s / 360f;
			double num3 = base.orbitState.period_s / base.orbitState.ref_spaceBody.rotationperiod_s;
			int num4 = 1 + (int)(360.0 * num3 / 1.0);
			int num5 = 0;
			while ((float)num5 < 360f + (float)num4)
			{
				TIDateTime tidateTime2 = new TIDateTime(tidateTime);
				string fireMissionEventName = this.FireMissionEventName;
				TITimeEvent.CreateNewTimeEvent(tidateTime2, this, target, null, fireMissionEventName, false, false, TITimeQueueRepeatType.None, 1, true, false);
				tidateTime.AddSeconds((double)num2);
				num5++;
			}
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.FireMission), this.FireMissionEventName, null, false, false);
			if (base.faction.IsAlienFaction)
			{
				if (target.ref_naturalSpaceObject.isEarth)
				{
					GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
					{
						x.CompleteMilestone(CampaignMilestone.AliensBombardEarth);
					});
				}
				else
				{
					GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
					{
						x.CompleteMilestone(CampaignMilestone.AliensAttackInSpace);
					});
				}
			}
			TINotificationQueueState.LogInitiateOrbitalBombardment(this, target);
			GameControl.eventManager.TriggerEvent(new BeginBombardment(this, target, target.ref_spaceBody), null, new object[]
			{
				this,
				this.bombardmentTarget,
				this.bombardmentTarget.ref_spaceBody
			});
			if (!fromSave)
			{
				AIDailyFactionPlanner.AIReaction(AIReactionEvent.HostileFleetBeginsBombardmentofMyAsset, this, target);
			}
		}

		// Token: 0x060043F8 RID: 17400 RVA: 0x001B9278 File Offset: 0x001B7478
		public void ForceEndBombardment(TISpaceFleetState.EndBombardmentReason reason)
		{
			if (this.bombarding)
			{
				this.endBombardmentReason = reason;
				OperationData operationData = this.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is BombardOperation);
				if (operationData != null)
				{
					this.CompleteFleetOperation(operationData.operation, operationData.target);
				}
			}
		}

		// Token: 0x060043F9 RID: 17401 RVA: 0x001B92D4 File Offset: 0x001B74D4
		public void EndBombardment(TISpaceFleetState.EndBombardmentReason reason)
		{
			if (this.bombarding)
			{
				if (this.endBombardmentReason == TISpaceFleetState.EndBombardmentReason.None)
				{
					this.endBombardmentReason = reason;
				}
				OperationData operationData = this.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is BombardOperation);
				IOperation operation;
				if ((operation = ((operationData != null) ? operationData.operation : null)) == null)
				{
					operation = OperationsManager.fleetOperations.First<IOperation>((IOperation x) => x is BombardOperation);
				}
				IOperation operation2 = operation;
				if (TIGameState.Valid(this.bombardmentTarget))
				{
					if (this.ships.Count > 0)
					{
						TINotificationQueueState.LogOrbitalBombardmentComplete(this, this.bombardmentTarget, operation2, this.endBombardmentReason);
					}
					if (this.bombardmentTarget.ref_hab != null)
					{
						this.bombardmentTarget.ref_hab.TryClearBombardmentStatus(this);
					}
					else
					{
						TIRegionState ref_region = this.bombardmentTarget.ref_region;
						if (ref_region != null)
						{
							ref_region.EndBombardment(this);
						}
					}
					if (this.ships.Count > 0)
					{
						this.RecordFailedAttackOnTarget(this.bombardmentTarget, 1f, true);
					}
				}
				else
				{
					TISpaceBodyState ref_spaceBody = this.ref_spaceBody;
					foreach (TIHabState tihabState in ((ref_spaceBody != null) ? ref_spaceBody.surfaceBases : null))
					{
						if (TIGameState.Valid(tihabState) && tihabState.underBombardment)
						{
							tihabState.TryClearBombardmentStatus(this);
						}
					}
					this.RemoveFailedAttackRecord(this.bombardmentTarget);
				}
				GameControl.eventManager.TriggerEvent(new EndBombardment(this, this.bombardmentTarget, this.bombardmentTarget.ref_spaceBody), null, new object[]
				{
					this,
					this.bombardmentTarget,
					this.bombardmentTarget.ref_spaceBody
				});
				this.gameTime.CancelTimeEvents(this.fleetOperationCompleteName, this, this.bombardmentTarget, (operation2 != null) ? operation2.GetTemplate() : null);
				this.gameTime.CancelTimeEvents(this.FireMissionEventName, this, this.bombardmentTarget, null);
				this.bombardmentTarget = null;
				this.bombardmentTargetBracketStatus = TISpaceFleetState.BombardmentBracketingStatus.NotBracketed;
				this.endBombardmentReason = TISpaceFleetState.EndBombardmentReason.None;
				foreach (TISpaceShipState tispaceShipState in this.ships)
				{
					if (!tispaceShipState.ShipDestroyed())
					{
						tispaceShipState.PostCombat(!tispaceShipState.fleet.inCombatOrWaitingForCombat);
					}
				}
				GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.FireMission), this.FireMissionEventName);
			}
		}

		// Token: 0x17000D1D RID: 3357
		// (get) Token: 0x060043FA RID: 17402 RVA: 0x001B956C File Offset: 0x001B776C
		public string FireMissionEventName
		{
			get
			{
				return new StringBuilder("Fire Mission").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x060043FB RID: 17403 RVA: 0x001B95A4 File Offset: 0x001B77A4
		public void FireMission(TimeEventStart e)
		{
			if (e.eventObject == this && this.bombarding)
			{
				if (this.bombardmentTarget.deleted || this.bombardmentTarget.archived)
				{
					this.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.TargetDestroyed);
					return;
				}
				bool flag;
				bool flag2;
				this.FireMission(e.startTime, out flag, out flag2);
				if (flag && this.bombardmentTargetBracketStatus == TISpaceFleetState.BombardmentBracketingStatus.Bracketing)
				{
					World.Active.GetExistingManager<GameTimeManager>().CancelTimeEvents(this.FireMissionEventName, this, this.bombardmentTarget, null);
					float num = (float)(base.orbitState.period_s / base.orbitState.ref_spaceBody.rotationperiod_s);
					double num2;
					if (!Mathf.Approximately(num, 1f))
					{
						if (num > 1f)
						{
							num = (float)(base.orbitState.ref_spaceBody.rotationperiod_s / base.orbitState.period_s);
							num2 = base.orbitState.ref_spaceBody.rotationperiod_s / (double)(1f - num);
						}
						else
						{
							num2 = base.orbitState.period_s / (double)(1f - num);
						}
					}
					else
					{
						num2 = base.orbitState.period_s;
					}
					int num3 = (int)(1209600.0 / num2);
					TIDateTime tidateTime = new TIDateTime(e.startTime);
					for (int i = 0; i < num3; i++)
					{
						tidateTime.AddSeconds(num2);
						TIDateTime tidateTime2 = new TIDateTime(tidateTime);
						string fireMissionEventName = this.FireMissionEventName;
						TITimeEvent.CreateNewTimeEvent(tidateTime2, this, this.bombardmentTarget, null, fireMissionEventName, false, false, TITimeQueueRepeatType.None, 1, true, false);
					}
					this.bombardmentTargetBracketStatus = TISpaceFleetState.BombardmentBracketingStatus.Bracketed;
				}
				if (!flag2)
				{
					this.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.FleetUnableToContinue);
				}
			}
		}

		// Token: 0x060043FC RID: 17404 RVA: 0x001B9728 File Offset: 0x001B7928
		public static bool WeaponCanBombardSpaceBody(ModuleDataEntry weaponModuleData, TISpaceBodyState spaceBody)
		{
			return weaponModuleData.moduleTemplate.ref_weapon.GetLocalBombardmentValue(spaceBody) > 0f;
		}

		// Token: 0x060043FD RID: 17405 RVA: 0x001B9744 File Offset: 0x001B7944
		protected void FireMission(TIDateTime time, out bool targetHit, out bool anyCapableShip)
		{
			anyCapableShip = true;
			bool flag = TISpaceShipState.BombardmentTargetInLineOfSight(this.ships[0], this.bombardmentTarget, time);
			if (flag)
			{
				anyCapableShip = false;
			}
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			float num;
			if (this.bombardmentTarget.ref_hab != null)
			{
				list = this.bombardmentTarget.ref_hab.ActiveCombatModules();
				if (this.bombardmentAltitude_km <= BombardOperation_Low.alt_km)
				{
					num = list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km, null, false));
				}
				else if (this.bombardmentAltitude_km <= BombardOperation_Med.alt_km)
				{
					num = (list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km, null, false)) + list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km / 2f, null, false))) / 2f;
				}
				else
				{
					num = (list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km, null, false)) + list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km / 2f, null, false)) + list.Sum<TIHabModuleState>((TIHabModuleState x) => (float)this.bombardmentTarget.ref_hab.tier * x.defenseWeapon.EstimateDPS(this.bombardmentAltitude_km / 3f, null, false))) / 3f;
				}
				num *= TIGlobalConfig.globalConfig.habDefensesPDDPSMultiplier;
			}
			else
			{
				TIRegionState ref_region = this.bombardmentTarget.ref_region;
				if (ref_region != null && ref_region.antiSpaceDefenses)
				{
					if (this.bombardmentAltitude_km <= BombardOperation_Low.alt_km)
					{
						num = this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km, null, false);
					}
					else if (this.bombardmentAltitude_km <= BombardOperation_Med.alt_km)
					{
						num = (this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km, null, false) + this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km / 2f, null, false)) / 2f;
					}
					else
					{
						num = (this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km, null, false) + this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km / 2f, null, false) + this.bombardmentTarget.ref_region.spaceDefenseFacility.weaponTemplate.EstimateDPS(this.bombardmentAltitude_km / 3f, null, false)) / 3f;
					}
					if (this.bombardmentTarget is TISpaceDefensesFacilityState)
					{
						num *= TIGlobalConfig.globalConfig.regionDefensesPDAMultiplier_Self;
					}
					else
					{
						num *= TIGlobalConfig.globalConfig.regionDefensesPDAMultiplier_Region;
					}
				}
				else if (this.bombardmentTarget.isRegionLandedUFO)
				{
					num = 30f;
				}
				else if (this.bombardmentTarget.isArmyState && (this.bombardmentTarget.ref_army.HumanArmy || this.bombardmentTarget.ref_army.AlienRegularArmy))
				{
					num = ((this.bombardmentTarget.ref_army.adjustedTechLevel >= 4f) ? this.bombardmentTarget.ref_army.adjustedTechLevel : 0f);
				}
				else
				{
					num = 0f;
				}
			}
			float num2 = (from y in this.ships.SelectMany<TISpaceShipState, ModuleDataEntry>((TISpaceShipState ship) => from q in ship.AllWeaponModuleData()
					where ship.WeaponIsOperable(q)
					select q)
				where y.moduleTemplate.isGunTypeWeapon && y.moduleTemplate.ref_projectileWeapon.isPointDefenseTargetable && TISpaceFleetState.WeaponCanBombardSpaceBody(y, this.bombardmentTarget.ref_spaceBody)
				select y).Sum<ModuleDataEntry>((ModuleDataEntry z) => Mathf.Pow(z.moduleTemplate.ref_projectileWeapon.GetSurfaceImpactVelocity_kps(this.ref_spaceBody, this.bombardmentAltitude_km), 2f) * 0.5f * z.moduleTemplate.ref_projectileWeapon.warheadMass_kg / z.moduleTemplate.ref_weapon.averageCooldown_s);
			float num3 = 0f;
			using (List<TISpaceShipState>.Enumerator enumerator = this.ships.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TISpaceShipState ship = enumerator.Current;
					IEnumerable<ModuleDataEntry> enumerable = ship.AllWeaponModuleData();
					Func<ModuleDataEntry, bool> func;
					Func<ModuleDataEntry, bool> <>9__12;
					if ((func = <>9__12) == null)
					{
						func = (<>9__12 = (ModuleDataEntry q) => q.moduleTemplate.isMissileWeapon && ship.WeaponIsOperable(q) && q.moduleTemplate.ref_projectileWeapon.isPointDefenseTargetable && TISpaceFleetState.WeaponCanBombardSpaceBody(q, this.bombardmentTarget.ref_spaceBody));
					}
					foreach (ModuleDataEntry moduleDataEntry in enumerable.Where<ModuleDataEntry>(func))
					{
						num3 += 1f;
					}
				}
			}
			num2 += num3;
			num *= 1f + (this.bombardmentAltitude_km - TemplateManager.global.lowBombardmentAltitude_km) / TemplateManager.global.lowBombardmentAltitude_km;
			float num4 = (float)(time - this.timeOfLastFireMission).TotalSeconds;
			this.timeOfLastFireMission = new TIDateTime(time);
			List<CombatWeaponCarrierState> list2 = new List<CombatWeaponCarrierState>();
			list2.AddRange(this.ships);
			TIRegionState ref_region2 = this.bombardmentTarget.ref_region;
			if (ref_region2 != null && ref_region2.antiSpaceDefenses)
			{
				list2.Add(this.bombardmentTarget.ref_region.spaceDefenseFacility);
				list2.Add(this.bombardmentTarget.ref_region.spaceDefenseFacility);
				list2.Add(this.bombardmentTarget.ref_region.spaceDefenseFacility);
			}
			else
			{
				foreach (TIHabModuleState tihabModuleState in list)
				{
					for (int i = 0; i < tihabModuleState.tier; i++)
					{
						list2.Add(tihabModuleState);
					}
				}
			}
			if (flag)
			{
				list2 = list2.OrderByDescending<CombatWeaponCarrierState, float>((CombatWeaponCarrierState x) => this.<FireMission>g__Initiative|301_9(x)).ToList<CombatWeaponCarrierState>();
			}
			bool flag2 = false;
			bool flag3 = false;
			targetHit = false;
			bool flag4 = false;
			foreach (CombatWeaponCarrierState combatWeaponCarrierState in list2.ToList<CombatWeaponCarrierState>())
			{
				if (TIGameState.Valid(combatWeaponCarrierState.GetTargetableState()))
				{
					if (combatWeaponCarrierState.isShip())
					{
						TISpaceShipState tispaceShipState = combatWeaponCarrierState.ref_shipCarrier();
						tispaceShipState.availablePower_GJ = Mathf.Min(tispaceShipState.AuxPowerRequriedStorage_GJ, tispaceShipState.availablePower_GJ + tispaceShipState.PerSecondPowerGain() * num4);
						tispaceShipState.InitiateManuever(tispaceShipState.currentFleetOffset, (Quaternion)tispaceShipState.GetDesiredRotation(false));
						if (flag)
						{
							foreach (ModuleDataEntry moduleDataEntry2 in tispaceShipState.AllWeaponModuleData())
							{
								if (tispaceShipState.WeaponIsOperable(moduleDataEntry2))
								{
									if (TISpaceFleetState.WeaponCanBombardSpaceBody(moduleDataEntry2, this.ref_spaceBody))
									{
										tispaceShipState.Bombard(moduleDataEntry2, time, out flag2, out flag3, flag4 && !tispaceShipState.visualizerLink.gameObject.activeInHierarchy, (num2 > 1f) ? (num / num2) : num);
										flag4 = true;
										anyCapableShip = true;
									}
									if (flag3 && !flag2)
									{
										if (this.bombardmentTargetBracketStatus == TISpaceFleetState.BombardmentBracketingStatus.NotBracketed)
										{
											this.bombardmentTargetBracketStatus = TISpaceFleetState.BombardmentBracketingStatus.Bracketing;
											if (!this.firstHitFromBombardmentRun && this.bombardmentTarget != null && !this.bombardmentTarget.isRegionXenoformingState)
											{
												TIArmyState ref_army = this.bombardmentTarget.ref_army;
												if (ref_army == null || !ref_army.AlienMegafaunaArmy)
												{
													foreach (TIFactionState tifactionState in this.bombardmentTarget.ref_factions)
													{
														if ((tifactionState != base.faction && !tifactionState.AI_AtWarWithFaction(tispaceShipState.faction)) || this.bombardmentTarget.isRegionState || this.bombardmentTarget.isHabState)
														{
															tifactionState.GainFactionHate(base.faction, TemplateManager.global.factionHateForInitiatingBombardment_AnyTarget, false, "Orbital Bombardment begins", true);
														}
													}
													this.firstHitFromBombardmentRun = true;
												}
											}
										}
										targetHit = true;
									}
									if (flag2)
									{
										break;
									}
								}
							}
							if (flag2)
							{
								break;
							}
						}
					}
					else if (flag && TIGameState.Valid(this.bombardmentTarget))
					{
						if (this.bombardmentTarget.ref_region != null)
						{
							if (TISpaceDefensesFacilityState.STOShouldShootBack(this.bombardmentTarget.ref_region, this.bombardmentTarget))
							{
								TISpaceShipState tispaceShipState2 = TISpaceDefensesFacilityState.SelectEarthSTOTarget(this.bombardmentTarget.ref_region, time, this, true);
								if (TIGameState.Valid(tispaceShipState2))
								{
									this.bombardmentTarget.ref_region.spaceDefenseFacility.OnFireMissionOrder(tispaceShipState2, time);
								}
							}
						}
						else
						{
							TIHabModuleState tihabModuleState2 = combatWeaponCarrierState.ref_habModuleCarrier();
							if (TIGameState.Valid(tihabModuleState2))
							{
								TISpaceShipState tispaceShipState3 = TIHabModuleState.SelectSTOTarget(tihabModuleState2, time, this);
								if (TIGameState.Valid(tispaceShipState3))
								{
									tihabModuleState2.OnFireMissionOrder(tispaceShipState3, time);
								}
							}
						}
					}
				}
				if (!flag && this.bombardmentTargetBracketStatus == TISpaceFleetState.BombardmentBracketingStatus.Bracketed)
				{
					this.bombardmentTargetBracketStatus = TISpaceFleetState.BombardmentBracketingStatus.NotBracketed;
				}
			}
		}

		// Token: 0x17000D1E RID: 3358
		// (get) Token: 0x060043FE RID: 17406 RVA: 0x001BA000 File Offset: 0x001B8200
		public bool underBombardment
		{
			get
			{
				return this.landed && this.ref_spaceBody.fleetsInOrbit.Any<TISpaceFleetState>((TISpaceFleetState x) => x.bombardmentTarget == this);
			}
		}

		// Token: 0x060043FF RID: 17407 RVA: 0x001BA028 File Offset: 0x001B8228
		public void AddToBombardmentLog(string toAdd, TIDateTime logTime)
		{
			if (this.bombardmentTarget != null)
			{
				TINotificationQueueState.Rapid_LogBombardmentShot(this, this.bombardmentTarget, logTime, toAdd);
			}
		}

		// Token: 0x06004400 RID: 17408 RVA: 0x001BA048 File Offset: 0x001B8248
		public bool CanHuntXenofauna()
		{
			return !this.transferAssigned && base.orbitState.isEarthLEO && (base.faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming) || base.faction.MilestoneCompleted(CampaignMilestone.AlienMegafaunaSpawns)) && this.BombardmentValue(this.ref_spaceBody) > 0f;
		}

		// Token: 0x17000D1F RID: 3359
		// (get) Token: 0x06004401 RID: 17409 RVA: 0x001BA0A0 File Offset: 0x001B82A0
		private string bombardXenofaunaCheck
		{
			get
			{
				return new StringBuilder("Check Bombard Xenofauna").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06004402 RID: 17410 RVA: 0x001BA0D8 File Offset: 0x001B82D8
		public void SetHuntingXenofauna(bool setting, bool involuntary)
		{
			bool huntingXenofauna = this.huntingXenofauna;
			this.huntingXenofauna = setting;
			if (huntingXenofauna != this.huntingXenofauna)
			{
				if (this.huntingXenofauna)
				{
					TIDateTime tidateTime = TITimeState.Now();
					if (this.dockedAtStation)
					{
						tidateTime.AddDays(0.04f);
					}
					tidateTime.AddSeconds(1.0);
					TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, this.bombardXenofaunaCheck, false, false, TITimeQueueRepeatType.Day, 1, true, false);
					GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CheckAutoBombardXenofauna), this.bombardXenofaunaCheck, null, true, false);
				}
				else
				{
					this.gameTime.CancelTimeEvents(this.bombardXenofaunaCheck, this, null, null);
					GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CheckAutoBombardXenofauna), this.bombardXenofaunaCheck);
					if (involuntary)
					{
						TINotificationQueueState.LogAutoBombardementCancelled(this, this.BombardmentValue(this.ref_spaceBody) <= 0f);
					}
				}
				GameControl.eventManager.TriggerEvent(new FleetHuntingXenofaunaStatusChange(this), null, new object[] { this });
			}
		}

		// Token: 0x06004403 RID: 17411 RVA: 0x001BA1CE File Offset: 0x001B83CE
		private void CheckAutoBombardXenofauna(TimeEventStart e)
		{
			if (this.huntingXenofauna && !this.bombarding)
			{
				this.AttemptBombardXenofauna();
			}
		}

		// Token: 0x06004404 RID: 17412 RVA: 0x001BA1E8 File Offset: 0x001B83E8
		public void AttemptBombardXenofauna()
		{
			BombardOperation_Low bombardOperation_Low = new BombardOperation_Low();
			if (bombardOperation_Low.ActorCanPerformOperation(this, this))
			{
				TIGameState tigameState = null;
				List<TIGameState> possibleTargets = bombardOperation_Low.GetPossibleTargets(this, null);
				List<TIGameState> list = possibleTargets.Where<TIGameState>((TIGameState x) => x.isRegionXenoformingState && !TISpaceDefensesFacilityState.STOShouldShootBack(x.ref_region, x)).ToList<TIGameState>();
				List<TIGameState> list2 = possibleTargets.Where<TIGameState>((TIGameState x) => x.isArmyState && x.ref_army.AlienMegafaunaArmy && x.ref_faction != base.faction && !TISpaceDefensesFacilityState.STOShouldShootBack(x.ref_region, x)).ToList<TIGameState>();
				if (list2.Count > 0)
				{
					list2 = (from x in list2
						orderby x.ref_army.InBattleWithArmiesOrRegionDefenses() descending, x.ref_army.strength
						select x).ToList<TIGameState>();
					tigameState = list2.First<TIGameState>();
				}
				else if (list.Count > 0)
				{
					list = list.OrderByDescending<TIGameState, float>((TIGameState x) => x.ref_xenoforming.xenoformingLevel).ToList<TIGameState>();
					tigameState = list.First<TIGameState>();
				}
				if (tigameState != null)
				{
					bombardOperation_Low.OnOperationConfirm(this, tigameState, null, null);
				}
			}
		}

		// Token: 0x06004405 RID: 17413 RVA: 0x001BA304 File Offset: 0x001B8504
		public void PostAssaultDamage(TIMissionOutcome outcome, bool offense)
		{
			float num = 0f;
			float num2 = 0.5f;
			if (offense)
			{
				switch (outcome)
				{
				case TIMissionOutcome.CriticalFailure:
					num = 0.5f;
					num2 = 1f;
					break;
				case TIMissionOutcome.Failure:
					num = 0.25f;
					num2 = 0.75f;
					break;
				case TIMissionOutcome.Success:
					num = 0f;
					num2 = 0.5f;
					break;
				case TIMissionOutcome.CriticalSuccess:
					num = 0f;
					num2 = 0.1f;
					break;
				}
			}
			else
			{
				switch (outcome)
				{
				case TIMissionOutcome.CriticalFailure:
					num = 0f;
					num2 = 0.1f;
					break;
				case TIMissionOutcome.Failure:
					num = 0f;
					num2 = 0.5f;
					break;
				case TIMissionOutcome.Success:
					num = 1f;
					num2 = 1f;
					break;
				case TIMissionOutcome.CriticalSuccess:
					num = 1f;
					num2 = 1f;
					break;
				}
			}
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				foreach (ModuleDataEntry moduleDataEntry in tispaceShipState.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault) && tispaceShipState.GetPartFunction(moduleDataEntry) >= 0.001f)
					{
						float num3 = TIUtilities.RandomRange(num, num2) * moduleDataEntry.moduleTemplate.hitPoints;
						float num4;
						tispaceShipState.ApplyDamageToPart(moduleDataEntry, num3, out num4);
					}
				}
			}
		}

		// Token: 0x06004406 RID: 17414 RVA: 0x001BA4A4 File Offset: 0x001B86A4
		public List<TIOfficerState> PostAssaultPromotionsAndDeaths(TIMissionOutcome outcome, bool offense, out List<TIOfficerState> officerDeaths)
		{
			List<TIOfficerState> list = new List<TIOfficerState>();
			officerDeaths = new List<TIOfficerState>();
			foreach (TISpaceShipState tispaceShipState in this.ships.ToList<TISpaceShipState>().Shuffle<TISpaceShipState>())
			{
				if ((offense && (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.CriticalSuccess)) || (!offense && (outcome == TIMissionOutcome.Failure || outcome == TIMissionOutcome.CriticalFailure)))
				{
					list.AddRange(tispaceShipState.CheckForOfficerPromotionEvent(OfficerSpawnEventType.SuccessfulMarineAssault, 0f, false, null));
				}
				else
				{
					List<TIOfficerState> list2 = tispaceShipState.officers.Where<TIOfficerState>((TIOfficerState x) => x.template.spawnEventType == OfficerSpawnEventType.SuccessfulMarineAssault).ToList<TIOfficerState>();
					if (list2.Count > 0)
					{
						float num = 0f;
						if ((offense && outcome == TIMissionOutcome.Failure) || (!offense && outcome == TIMissionOutcome.Success))
						{
							num = 0.05f;
						}
						else if ((offense && outcome == TIMissionOutcome.CriticalFailure) || (!offense && outcome == TIMissionOutcome.CriticalSuccess))
						{
							num = 0.5f;
						}
						foreach (TIOfficerState tiofficerState in list2)
						{
							if (TIUtilities.RandomFloatValue() < num)
							{
								officerDeaths.Add(tiofficerState);
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06004407 RID: 17415 RVA: 0x001BA5F4 File Offset: 0x001B87F4
		public override float AssaultCombatValue(bool defense)
		{
			float num = this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.AssaultCombatValue(defense));
			return num + TIEffectsState.SumEffectsModifiers(Context.SpaceAssaultBonus, base.faction, num, null);
		}

		// Token: 0x06004408 RID: 17416 RVA: 0x001BA63C File Offset: 0x001B883C
		public float InvasionCombatValue()
		{
			float num = 0f;
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				num += tispaceShipState.InvasionCombatValue();
			}
			return num;
		}

		// Token: 0x06004409 RID: 17417 RVA: 0x001BA698 File Offset: 0x001B8898
		public bool AI_LegsToUseSiteForResupply(TIHabSiteState site, float hypetheticalDVPenalty_kps = 0f)
		{
			if (hypetheticalDVPenalty_kps > 0f && this.ships.Where<TISpaceShipState>(delegate(TISpaceShipState x)
			{
				float num = x.currentDeltaV_kps - hypetheticalDVPenalty_kps;
				return base.<AI_LegsToUseSiteForResupply>g__GetLandingCost_kps|0(x) > num;
			}).Any<TISpaceShipState>())
			{
				return false;
			}
			TIOrbitState orbit = site.ref_spaceBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km);
			bool willResupply = site.hasOperatingBase && site.ref_hab.AllowsResupply(base.faction, false, false);
			Dictionary<TISpaceShipState, float> cost = this.ships.ToDictionary<TISpaceShipState, TISpaceShipState, float>((TISpaceShipState x) => x, delegate(TISpaceShipState x)
			{
				float num2;
				if (willResupply)
				{
					num2 = x.currentDeltaV_kps - x.currentMaxDeltaV_kps;
				}
				else
				{
					num2 = base.<AI_LegsToUseSiteForResupply>g__GetLandingCost_kps|0(x);
				}
				float num3 = (float)orbit.DeltaVToReachFromSurface_kps(site.latitude, (double)this.maxAcceleration_mps2);
				return num2 + num3;
			});
			return cost.Keys.All<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps - cost[x] >= x.currentMaxDeltaV_kps * 0.85f);
		}

		// Token: 0x0600440A RID: 17418 RVA: 0x001BA7AD File Offset: 0x001B89AD
		public bool AI_NeedsRefuel()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.AI_NeedsRefuel());
		}

		// Token: 0x0600440B RID: 17419 RVA: 0x001BA7D9 File Offset: 0x001B89D9
		public bool NeedsRefuel()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel());
		}

		// Token: 0x0600440C RID: 17420 RVA: 0x001BA805 File Offset: 0x001B8A05
		public bool NeedsRearm()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.NeedsRearm());
		}

		// Token: 0x0600440D RID: 17421 RVA: 0x001BA831 File Offset: 0x001B8A31
		public bool NeedsRepair()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.damaged);
		}

		// Token: 0x0600440E RID: 17422 RVA: 0x001BA860 File Offset: 0x001B8A60
		public bool AI_NeedsRefuelBadly()
		{
			TISpaceFleetState.<>c__DisplayClass325_0 CS$<>8__locals1 = new TISpaceFleetState.<>c__DisplayClass325_0();
			TISpaceFleetState.<>c__DisplayClass325_0 CS$<>8__locals2 = CS$<>8__locals1;
			float num;
			if (!base.faction.CanResupplyShipsAtLocation(this.location, false))
			{
				TISpaceObjectState getSunOrbitingRelatedObject = this.GetSunOrbitingRelatedObject;
				num = Mathf.Max((float)((getSunOrbitingRelatedObject != null) ? new double?(getSunOrbitingRelatedObject.semiMajorAxis_AU) : null).Value * 15f, this.IsAlien() ? 150f : 50f);
			}
			else
			{
				num = 20f;
			}
			CS$<>8__locals2.neededDV_kps = num;
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.AI_NeedsRefuelBadly(CS$<>8__locals1.neededDV_kps));
		}

		// Token: 0x0600440F RID: 17423 RVA: 0x001BA8F4 File Offset: 0x001B8AF4
		public bool AI_InterfleetRefuelCandidate()
		{
			float num = this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.currentMaxDeltaV_kps);
			return this.currentDeltaV_kps / num < 0.9f;
		}

		// Token: 0x06004410 RID: 17424 RVA: 0x001BA93C File Offset: 0x001B8B3C
		public bool AI_NeedsRearmBadly()
		{
			return (float)this.ships.Count<TISpaceShipState>((TISpaceShipState x) => x.AI_NeedsRearmBadly()) / (float)this.ships.Count > 0.5f;
		}

		// Token: 0x06004411 RID: 17425 RVA: 0x001BA988 File Offset: 0x001B8B88
		public bool AI_NeedsRepairBadly()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.badlyDamaged);
		}

		// Token: 0x06004412 RID: 17426 RVA: 0x001BA9B4 File Offset: 0x001B8BB4
		public bool AI_SeekSpecialHabRepairIfNecessary()
		{
			FactionGoal_Fleet factionGoal_Fleet = this.AssignedGoal();
			if (factionGoal_Fleet != null)
			{
				GoalType goalType = factionGoal_Fleet.GetGoalType();
				if (goalType <= GoalType.CaptureHab)
				{
					switch (goalType)
					{
					case GoalType.FoundPlatform:
					case GoalType.FoundMaxStation:
						break;
					case GoalType.FoundBase:
						return (from x in this.ships.SelectMany<TISpaceShipState, DamagedShipPartData>((TISpaceShipState x) => x.damagedParts)
							where x.module.moduleTemplate.isUtilityModule && x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Intersect<SpecialModuleRule>(TISpaceShipState.FoundBaseRules).Any<SpecialModuleRule>()
							select x).Any<DamagedShipPartData>((DamagedShipPartData x) => x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenConstructionModulePresent));
					default:
						if (goalType != GoalType.CaptureHab)
						{
							return false;
						}
						return this.ships.SelectMany<TISpaceShipState, DamagedShipPartData>((TISpaceShipState x) => x.damagedParts).Any<DamagedShipPartData>(delegate(DamagedShipPartData x)
						{
							TIUtilityModuleTemplate ref_utilityModule = x.module.moduleTemplate.ref_utilityModule;
							return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenMarineModulePresent);
						});
					}
				}
				else
				{
					if (goalType == GoalType.RepairFleet)
					{
						return true;
					}
					if (goalType != GoalType.FoundStation)
					{
						if (goalType != GoalType.FoundSurveillanceStation)
						{
							return false;
						}
						return (from x in this.ships.SelectMany<TISpaceShipState, DamagedShipPartData>((TISpaceShipState x) => x.damagedParts)
							where x.module.moduleTemplate.isUtilityModule && x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Intersect<SpecialModuleRule>(TISpaceShipState.FoundSurveillanceStationRules).Any<SpecialModuleRule>()
							select x).Any<DamagedShipPartData>((DamagedShipPartData x) => x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenConstructionModulePresent));
					}
				}
				return (from x in this.ships.SelectMany<TISpaceShipState, DamagedShipPartData>((TISpaceShipState x) => x.damagedParts)
					where x.module.moduleTemplate.isUtilityModule && x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Intersect<SpecialModuleRule>(TISpaceShipState.FoundStandardStationRules).Any<SpecialModuleRule>()
					select x).Any<DamagedShipPartData>((DamagedShipPartData x) => x.module.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RepairOnlyWhenConstructionModulePresent));
			}
			return false;
		}

		// Token: 0x06004413 RID: 17427 RVA: 0x001BABC8 File Offset: 0x001B8DC8
		public void TruncateResupplyAndRepair(float fractionCompleted)
		{
			IOrderedEnumerable<PlannedResupplyAndRepair> orderedEnumerable = from x in this.ships
				where TIGameState.Valid(x) && x.plannedResupplyAndRepair.active
				select x.plannedResupplyAndRepair into x
				orderby x.duration_days
				select x;
			float num = orderedEnumerable.Sum<PlannedResupplyAndRepair>((PlannedResupplyAndRepair x) => x.duration_days) * fractionCompleted;
			float num2 = 0f;
			foreach (PlannedResupplyAndRepair plannedResupplyAndRepair in orderedEnumerable.ToList<PlannedResupplyAndRepair>())
			{
				if (num2 + plannedResupplyAndRepair.duration_days <= num)
				{
					if (TIGameState.Valid(plannedResupplyAndRepair.ship))
					{
						if (plannedResupplyAndRepair.resupplyCost.anyDebit || plannedResupplyAndRepair.repairCost.anyDebit)
						{
							num2 += plannedResupplyAndRepair.duration_days;
						}
						plannedResupplyAndRepair.ProcessResupplyAndRepair(plannedResupplyAndRepair.ship);
					}
				}
				else
				{
					if (num > 0f && plannedResupplyAndRepair.OnlyRefueling(true) && plannedResupplyAndRepair.duration_days > 0f)
					{
						float num3 = Mathf.Clamp((num - num2) / plannedResupplyAndRepair.duration_days, 0f, 1f);
						plannedResupplyAndRepair.propellantToReload *= num3;
						plannedResupplyAndRepair.ProcessResupplyAndRepair(plannedResupplyAndRepair.ship);
						break;
					}
					break;
				}
			}
		}

		// Token: 0x06004414 RID: 17428 RVA: 0x001BAD5C File Offset: 0x001B8F5C
		public bool ShipFinishedRepair(float fractionCompleted, TISpaceShipState ship)
		{
			IOrderedEnumerable<PlannedResupplyAndRepair> orderedEnumerable = from x in this.ships
				where x.plannedResupplyAndRepair.active
				select x.plannedResupplyAndRepair into x
				orderby x.duration_days
				select x;
			float num = orderedEnumerable.Sum<PlannedResupplyAndRepair>((PlannedResupplyAndRepair x) => x.duration_days) * fractionCompleted;
			float num2 = 0f;
			foreach (PlannedResupplyAndRepair plannedResupplyAndRepair in orderedEnumerable.ToList<PlannedResupplyAndRepair>())
			{
				if (num2 + plannedResupplyAndRepair.duration_days > num)
				{
					break;
				}
				if (plannedResupplyAndRepair.ship == ship)
				{
					return true;
				}
				num2 += plannedResupplyAndRepair.duration_days;
			}
			return false;
		}

		// Token: 0x17000D20 RID: 3360
		// (get) Token: 0x06004415 RID: 17429 RVA: 0x001BAE74 File Offset: 0x001B9074
		public Dictionary<TISpaceShipState, TISpaceShipTemplate> RefitsAvailable
		{
			get
			{
				return (from x in this.ships
					select new ValueTuple<TISpaceShipState, TISpaceShipTemplate>(x, x.BestExistingRefit) into x
					where x.Item2 != null
					select x).ToDictionary<ValueTuple<TISpaceShipState, TISpaceShipTemplate>, TISpaceShipState, TISpaceShipTemplate>(([TupleElementNames(new string[] { "x", "BestExistingRefit" })] ValueTuple<TISpaceShipState, TISpaceShipTemplate> x) => x.Item1, ([TupleElementNames(new string[] { "x", "BestExistingRefit" })] ValueTuple<TISpaceShipState, TISpaceShipTemplate> x) => x.Item2);
			}
		}

		// Token: 0x17000D21 RID: 3361
		// (get) Token: 0x06004416 RID: 17430 RVA: 0x001BAF12 File Offset: 0x001B9112
		public bool AllowUseBoostForRepairsResupply
		{
			get
			{
				if (!this.IsAlien())
				{
					TIHabState ref_hab = this.ref_hab;
					return ref_hab != null && ref_hab.GetSunOrbitingRelatedObject.isEarth;
				}
				return false;
			}
		}

		// Token: 0x06004417 RID: 17431 RVA: 0x001BAF34 File Offset: 0x001B9134
		public bool CanRefitAtLocation()
		{
			if (this.dockedAtHab && this.dockedLocation.ref_hab.faction == base.faction)
			{
				return this.dockedLocation.ref_hab.CompletedShipyards().Any<TIHabModuleState>((TIHabModuleState x) => x.powered);
			}
			return false;
		}

		// Token: 0x17000D22 RID: 3362
		// (get) Token: 0x06004418 RID: 17432 RVA: 0x001BAF9C File Offset: 0x001B919C
		public float RelativeValueOfRefittedFleet
		{
			get
			{
				Dictionary<TISpaceShipState, TISpaceShipTemplate> refits = this.RefitsAvailable;
				if (!refits.Any<KeyValuePair<TISpaceShipState, TISpaceShipTemplate>>())
				{
					return 1f;
				}
				float num = this.ships.Min<TISpaceShipState>(delegate(TISpaceShipState x)
				{
					if (!refits.ContainsKey(x))
					{
						return x.template.baseCruiseDeltaV_kps(false);
					}
					return refits[x].baseCruiseDeltaV_kps(false);
				});
				return this.maxDeltaV_kps / num * this.ships.Sum<TISpaceShipState>(delegate(TISpaceShipState x)
				{
					if (!refits.ContainsKey(x))
					{
						return 1f;
					}
					return x.template.GetRelativeValueOfRefit(refits[x]);
				}) / (float)this.ships.Count<TISpaceShipState>();
			}
		}

		// Token: 0x06004419 RID: 17433 RVA: 0x001BB013 File Offset: 0x001B9213
		public bool NeedsRefit()
		{
			return !base.faction.IsAlienFaction && this.RelativeValueOfRefittedFleet > 1.4f;
		}

		// Token: 0x0600441A RID: 17434 RVA: 0x001BB034 File Offset: 0x001B9234
		public bool CanAffordAnyPropellant(TIFactionState faction)
		{
			return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel()).Any<TISpaceShipState>((TISpaceShipState x) => x.GetPreferredPropellantTankCost(faction, Mathf.Min(100f, x.PropellantShortage_tons), true).CanAfford(faction, 1f, null, float.PositiveInfinity));
		}

		// Token: 0x0600441B RID: 17435 RVA: 0x001BB08C File Offset: 0x001B928C
		public bool CanAffordAnyReloading(TIHabState hab)
		{
			return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.NeedsRearm()).Any<TISpaceShipState>((TISpaceShipState x) => x.CanAffordAnyReload(hab));
		}

		// Token: 0x0600441C RID: 17436 RVA: 0x001BB0E4 File Offset: 0x001B92E4
		public bool CanAffordAnyRepairs(TIHabState hab)
		{
			return this.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.damaged).Any<TISpaceShipState>((TISpaceShipState x) => x.CanAffordAnyRepair(hab));
		}

		// Token: 0x0600441D RID: 17437 RVA: 0x001BB13C File Offset: 0x001B933C
		public bool IsResupplying()
		{
			using (List<OperationData>.Enumerator enumerator = this.CurrentOperations().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.operation is ResupplyOperation)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600441E RID: 17438 RVA: 0x001BB19C File Offset: 0x001B939C
		public bool IsRepairing()
		{
			using (List<OperationData>.Enumerator enumerator = this.CurrentOperations().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.operation is RepairFleetOperation)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600441F RID: 17439 RVA: 0x001BB1FC File Offset: 0x001B93FC
		public bool CanSharePropellant()
		{
			if (this.ships.Count > 1)
			{
				if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.propellant_tons > 0f))
				{
					if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel() && x.propellant == Propellant.Anything))
					{
						return true;
					}
				}
				foreach (PropellantGroup propellantGroup in this.BuildPropellantGroups())
				{
					if (propellantGroup.ships.Count > 1)
					{
						if (propellantGroup.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.propellant_tons > 0f))
						{
							if (propellantGroup.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.NeedsRefuel()))
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06004420 RID: 17440 RVA: 0x001BB320 File Offset: 0x001B9520
		public List<PropellantGroup> BuildPropellantGroups()
		{
			List<PropellantGroup> list = new List<PropellantGroup>();
			bool flag = false;
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				foreach (PropellantGroup propellantGroup in list)
				{
					flag = true;
					if (tispaceShipState.propellant == propellantGroup.propellant)
					{
						Dictionary<FactionResource, float> dictionary = tispaceShipState.drive.GetPerTankPropellantMaterials(base.faction).ToRVCollection(1f);
						if (dictionary.Count == propellantGroup.propellantComposition.Count)
						{
							using (Dictionary<FactionResource, float>.Enumerator enumerator3 = dictionary.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									KeyValuePair<FactionResource, float> keyValuePair = enumerator3.Current;
									if (!propellantGroup.propellantComposition.ContainsKey(keyValuePair.Key) || propellantGroup.propellantComposition[keyValuePair.Key] != keyValuePair.Value)
									{
										flag = false;
										break;
									}
								}
								goto IL_00E9;
							}
						}
						flag = false;
					}
					else
					{
						flag = false;
					}
					IL_00E9:
					if (flag)
					{
						propellantGroup.ships.Add(tispaceShipState);
						break;
					}
				}
				if (!flag)
				{
					list.Add(new PropellantGroup(new List<TISpaceShipState> { tispaceShipState }));
				}
			}
			return list;
		}

		// Token: 0x06004421 RID: 17441 RVA: 0x001BB4C8 File Offset: 0x001B96C8
		public void SetPropellantSharingPlan(List<PropellantSharingEvent> plan)
		{
			this.propellantSharingPlan = new List<PropellantSharingEvent>(plan);
		}

		// Token: 0x06004422 RID: 17442 RVA: 0x001BB4D8 File Offset: 0x001B96D8
		public void ExecutePropellantSharingPlan()
		{
			bool flag = this.transferAssigned && this.fleetTrajectoryData != null;
			double num = 0.0;
			if (flag)
			{
				num = this.fleetTrajectoryData.initialDeltaV_mps - (double)this.currentDeltaV_mps;
			}
			foreach (PropellantSharingEvent propellantSharingEvent in this.propellantSharingPlan)
			{
				if (TIGameState.Valid(propellantSharingEvent.giver) && TIGameState.Valid(propellantSharingEvent.taker))
				{
					propellantSharingEvent.giver.RefuelPropellant(-propellantSharingEvent.amount_tons);
					propellantSharingEvent.taker.RefuelPropellant(propellantSharingEvent.amount_tons);
				}
			}
			this.ships.ForEach(delegate(TISpaceShipState x)
			{
				x.SetPropulsionValuesDirty(true, true);
			});
			this.propellantSharingPlan.Clear();
			if (flag)
			{
				this.fleetTrajectoryData.initialDeltaV_mps = (double)this.currentDeltaV_mps + num;
				this.VerifyAssignedTransfer(false);
			}
		}

		// Token: 0x06004423 RID: 17443 RVA: 0x001BB5EC File Offset: 0x001B97EC
		public static List<PropellantSharingEvent> CreatePropellantSharingPlan_Equalization(List<TISpaceShipState> group, bool mustMeaningfullyImproveGroupDV = false)
		{
			float num = TISpaceFleetState.CalcEqualDeltaV(group);
			float num2 = group.Min<TISpaceShipState>((TISpaceShipState x) => x.currentMaxDeltaV_kps);
			float targetDeltaV = Math.Min(num, num2);
			if (mustMeaningfullyImproveGroupDV)
			{
				float num3 = group.Min<TISpaceShipState>((TISpaceShipState x) => x.currentDeltaV_kps);
				if (targetDeltaV / num3 >= 0.98f)
				{
					return new List<PropellantSharingEvent>();
				}
			}
			List<PropellantSharingEvent> list = new List<PropellantSharingEvent>();
			Dictionary<TISpaceShipState, float> dictionary = group.ToDictionary<TISpaceShipState, TISpaceShipState, float>((TISpaceShipState x) => x, (TISpaceShipState x) => x.propellant_tons - x.GetPropellantTonsForDesiredDv(targetDeltaV));
			for (;;)
			{
				KeyValuePair<TISpaceShipState, float> keyValuePair = dictionary.MaxBy<KeyValuePair<TISpaceShipState, float>, float>((KeyValuePair<TISpaceShipState, float> x) => x.Value);
				KeyValuePair<TISpaceShipState, float> keyValuePair2 = dictionary.MinBy<KeyValuePair<TISpaceShipState, float>, float>((KeyValuePair<TISpaceShipState, float> x) => x.Value);
				if (keyValuePair.Value <= 0f || keyValuePair2.Value >= 0f)
				{
					break;
				}
				float num4 = Math.Min(keyValuePair.Value, Math.Abs(keyValuePair2.Value));
				Dictionary<TISpaceShipState, float> dictionary2 = dictionary;
				TISpaceShipState tispaceShipState = keyValuePair.Key;
				dictionary2[tispaceShipState] -= num4;
				dictionary2 = dictionary;
				tispaceShipState = keyValuePair2.Key;
				dictionary2[tispaceShipState] += num4;
				list.Add(new PropellantSharingEvent
				{
					giver = keyValuePair.Key,
					taker = keyValuePair2.Key,
					amount_tons = num4
				});
			}
			return list;
		}

		// Token: 0x06004424 RID: 17444 RVA: 0x001BB7B8 File Offset: 0x001B99B8
		public bool AI_CreatePropellantSharingPlan_Equalization()
		{
			bool flag = false;
			this.propellantSharingPlan = new List<PropellantSharingEvent>();
			foreach (PropellantGroup propellantGroup in this.BuildPropellantGroups())
			{
				if (propellantGroup.ships.Count > 1)
				{
					List<PropellantSharingEvent> list = TISpaceFleetState.CreatePropellantSharingPlan_Equalization(propellantGroup.ships, true);
					if (list.Count > 0)
					{
						this.propellantSharingPlan.AddRange(list);
						flag = true;
					}
				}
			}
			return flag;
		}

		// Token: 0x06004425 RID: 17445 RVA: 0x001BB844 File Offset: 0x001B9A44
		private static float CalcEqualDeltaV(List<TISpaceShipState> group)
		{
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = 0f;
			foreach (TISpaceShipState tispaceShipState in group)
			{
				num = Math.Min(num, tispaceShipState.currentDeltaV_kps);
				num2 = Math.Max(num2, tispaceShipState.currentDeltaV_kps);
				num3 += tispaceShipState.propellant_tons;
			}
			float midDeltaV = (num + num2) / 2f;
			Func<TISpaceShipState, float> <>9__0;
			while (num2 - num > 0.001f)
			{
				Func<TISpaceShipState, float> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (TISpaceShipState x) => x.GetPropellantTonsForDesiredDv(midDeltaV));
				}
				if (group.Sum<TISpaceShipState>(func) > num3)
				{
					num2 = midDeltaV;
				}
				else
				{
					num = midDeltaV;
				}
				midDeltaV = (num + num2) / 2f;
			}
			return midDeltaV;
		}

		// Token: 0x06004426 RID: 17446 RVA: 0x001BB938 File Offset: 0x001B9B38
		public int GetOfficerCountInShips()
		{
			int num = 0;
			if (!this.IsAlien())
			{
				for (int i = 0; i < this.ships.Count; i++)
				{
					num += this.ships[i].officers.Count;
				}
			}
			return num;
		}

		// Token: 0x06004427 RID: 17447 RVA: 0x001BB97F File Offset: 0x001B9B7F
		public void SetOfficerTransferPlan(Dictionary<TIOfficerState, OfficerCarrierState> plan)
		{
			this.officerTransferPlan = new Dictionary<TIOfficerState, OfficerCarrierState>(plan);
		}

		// Token: 0x06004428 RID: 17448 RVA: 0x001BB990 File Offset: 0x001B9B90
		public void ExecuteOfficerTransferPlan()
		{
			foreach (TIOfficerState tiofficerState in this.officerTransferPlan.Keys.ToList<TIOfficerState>())
			{
				OfficerCarrierState officerCarrierState = this.officerTransferPlan[tiofficerState];
				TIGameState tigameState = ((officerCarrierState != null) ? officerCarrierState.GetState() : null);
				if (tigameState == null)
				{
					tiofficerState.DeleteOfficer(false);
				}
				else if (tiofficerState.ship != null && tigameState.isSpaceShipState)
				{
					tiofficerState.TransferOfficerBetweenShips(tigameState.ref_ship, false, false, true);
				}
				else if (tiofficerState.ship != null && tigameState.isHabState)
				{
					tiofficerState.TransferOfficer_ToHab(tigameState.ref_hab, true, false, true);
				}
				else if (tiofficerState.hab != null && tigameState.isSpaceShipState)
				{
					tiofficerState.TransferOfficer_FromHabToShip(tigameState.ref_ship, false, true);
				}
			}
			this.officerTransferPlan.Clear();
		}

		// Token: 0x06004429 RID: 17449 RVA: 0x001BBA98 File Offset: 0x001B9C98
		public void AI_OptimizeOfficers()
		{
			if (this.dockedAtHab)
			{
				using (List<TIOfficerState>.Enumerator enumerator = this.ref_hab.officersOnBoard.ToList<TIOfficerState>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIOfficerState officer = enumerator.Current;
						TISpaceShipState tispaceShipState = this.ships.Where<TISpaceShipState>((TISpaceShipState x) => officer.CanTransferOfficerFromHab(this.ref_hab, x, false, false, 0)).SelectRandomItem<TISpaceShipState>();
						if (tispaceShipState != null)
						{
							Dictionary<TIOfficerState, OfficerCarrierState> dictionary = new Dictionary<TIOfficerState, OfficerCarrierState>();
							dictionary[officer] = tispaceShipState;
							TIResourcesCost tiresourcesCost = TransferOfficersOperation.ResourceCostOptions(dictionary).FirstOrDefault<TIResourcesCost>();
							if (tiresourcesCost != null && tiresourcesCost.CanAfford(tispaceShipState.faction, 1f, null, float.PositiveInfinity))
							{
								base.faction.playerControl.StartAction(new BeginOfficerTransferOperationAction(this, dictionary));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600442A RID: 17450 RVA: 0x001BBB8C File Offset: 0x001B9D8C
		public Formation DefaultFormation()
		{
			if (base.faction == null)
			{
				TISpaceFleetTemplate template = this.template;
				if (((template != null) ? template.defaultFormation.pattern : null) != null)
				{
					return this.template.defaultFormation;
				}
			}
			if (base.faction.IsAlienFaction)
			{
				return this.defaultAlienFormation;
			}
			return this.defaultHumanFormation;
		}

		// Token: 0x0600442B RID: 17451 RVA: 0x001BBBE9 File Offset: 0x001B9DE9
		public void AssignFormation(string shapeDataName, FormationSpacing spacing, FormationConcentration concentration, FormationFocus focus, int numberOfPositions, bool invertZ = false, bool initialAssignment = false, bool forStratLayer = false)
		{
			this.AssignFormation(new Formation(shapeDataName, focus, spacing, concentration), invertZ, initialAssignment, false, false, forStratLayer);
		}

		// Token: 0x0600442C RID: 17452 RVA: 0x001BBC04 File Offset: 0x001B9E04
		public void AssignFormation(Formation formation, bool invertZ = false, bool initialAssignment = false, bool saveFormation = false, bool isCombatSetup = false, bool forStratLayer = false)
		{
			this.formation = new Formation(formation);
			if (saveFormation)
			{
				this.savedFormation = formation;
			}
			if (!initialAssignment)
			{
				if ((TIGlobalValuesState.isSpaceCombatEnabled | isCombatSetup) && !forStratLayer)
				{
					this.ships.ForEach(delegate(TISpaceShipState k)
					{
						k.SetCombatFormationOffset(this.ships, formation, this.ships.Count, invertZ, isCombatSetup);
					});
					if (this.ships.Count > 0)
					{
						Debug.Log(this.ships[0].faction.displayNameCapitalizedWithColor + " Assign Formation: " + formation.displayName);
						return;
					}
				}
				else
				{
					this.ships.ForEach(delegate(TISpaceShipState k)
					{
						k.SetFormationOffsetAndInitiateStationkeepingManeuver(this.ships.Count, invertZ);
					});
				}
			}
		}

		// Token: 0x0600442D RID: 17453 RVA: 0x001BBCD8 File Offset: 0x001B9ED8
		public void ResetFormation(bool invertZ = false, bool forStratLayer = false)
		{
			if (this.formation.pattern == null)
			{
				this.AssignFormation(this.DefaultFormation(), invertZ, false, false, false, forStratLayer);
				return;
			}
			this.AssignFormation(this.formation, invertZ, false, false, false, forStratLayer);
		}

		// Token: 0x0600442E RID: 17454 RVA: 0x001BBD18 File Offset: 0x001B9F18
		public void TeleportAllToFormation(bool invertZ = false, bool forStratLayer = false)
		{
			this.ResetFormation(invertZ, forStratLayer);
			this.ships.ForEach(delegate(TISpaceShipState x)
			{
				x.EndManuever();
			});
			for (int i = 0; i < this.ships.Count; i++)
			{
				this.ships[i].currentFleetOffset = this.ships[i].fleetFormationOffset;
				this.ships[i].currentRotation = (Quaternion)this.ships[0].GetDesiredRotation(false);
			}
			GameControl.eventManager.TriggerEvent(new ResetFleetFormationVisuals(this), null, new object[] { this });
		}

		// Token: 0x0600442F RID: 17455 RVA: 0x001BBDD4 File Offset: 0x001B9FD4
		public Vector3d FormationCenter(bool includeZ = false)
		{
			double num = this.ships.Average<TISpaceShipState>((TISpaceShipState ship) => ship.currentFleetOffset.x);
			double num2 = this.ships.Average<TISpaceShipState>((TISpaceShipState ship) => ship.currentFleetOffset.y);
			double num3 = this.ships.Max<TISpaceShipState>((TISpaceShipState ship) => ship.currentFleetOffset.z);
			return new Vector3d(num, num2, num3);
		}

		// Token: 0x17000D23 RID: 3363
		// (get) Token: 0x06004430 RID: 17456 RVA: 0x001BBE68 File Offset: 0x001BA068
		public float FormationWidth
		{
			get
			{
				return (float)(this.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.currentFleetOffset.x) - this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.currentFleetOffset.x));
			}
		}

		// Token: 0x06004431 RID: 17457 RVA: 0x001BBECC File Offset: 0x001BA0CC
		public void AddShipsToFleet(List<TISpaceShipState> newShips, TISpaceFleetState originFleet, bool storeFaction = false, bool startup = false)
		{
			if (originFleet != null)
			{
				if (storeFaction && base.faction != originFleet.faction)
				{
					newShips.ForEach(delegate(TISpaceShipState x)
					{
						x.storedFaction = originFleet.faction;
					});
				}
				if (!storeFaction && base.faction == originFleet.faction)
				{
					if (originFleet.ships.Count == newShips.Count && this.GetDisplayName(GameControl.control.activePlayer).StartsWith(base.faction.fleetNameBase) && !originFleet.GetDisplayName(GameControl.control.activePlayer).Contains(originFleet.faction.fleetNameBase))
					{
						this.displayNameByFaction[GameControl.control.activePlayer] = originFleet.GetDisplayName(GameControl.control.activePlayer);
					}
					if (originFleet.homeport != null && this.homeport == null)
					{
						this.SetHomePort(originFleet.homeport);
					}
				}
				originFleet.RemoveShipsFromFleet(newShips, this);
			}
			object obj = this.transferAssigned && this.fleetTrajectoryData != null;
			double num = 0.0;
			object obj2 = obj;
			if (obj2 != null)
			{
				num = this.fleetTrajectoryData.initialDeltaV_mps - (double)this.currentDeltaV_mps;
			}
			if (originFleet != null)
			{
				newShips.ForEach(delegate(TISpaceShipState x)
				{
					this.AddShipToFleet(x, this.GetGlobalPosition(), originFleet, originFleet.GetGlobalPosition());
				});
			}
			else
			{
				newShips.ForEach(delegate(TISpaceShipState x)
				{
					this.AddShipToFleet(x, Vector3d.zero, null, Vector3d.zero);
				});
			}
			this.ships = (from x in this.ships
				orderby x.hull.length_m descending, x.dryMass_kg descending
				select x).ToList<TISpaceShipState>();
			if (base.faction != null && !startup)
			{
				base.faction.GetMissionControlUsage();
			}
			this.ResetFormation(false, false);
			this.TeleportAllToFormation(false, false);
			if (obj2 != null)
			{
				this.fleetTrajectoryData.initialDeltaV_mps = (double)this.currentDeltaV_mps + num;
				this.VerifyAssignedTransfer(false);
			}
		}

		// Token: 0x06004432 RID: 17458 RVA: 0x001BC128 File Offset: 0x001BA328
		private void ForceIconResourceReset()
		{
			this._icon = null;
		}

		// Token: 0x06004433 RID: 17459 RVA: 0x001BC134 File Offset: 0x001BA334
		private void AddShipToFleet(TISpaceShipState ship, Vector3d currentPosition, TISpaceFleetState oldFleet, Vector3d oldFleetPosition)
		{
			float num = this.SpaceCombatValue();
			ship.JoinFleet(this);
			this.ships.Add(ship);
			base.faction.SetMissionControlUsageDataDirty();
			this.ForceIconResourceReset();
			EventManager eventManager = GameControl.eventManager;
			GameEvent gameEvent = new ShipsAddedToFleet(this);
			string text = null;
			object[] array = new object[5];
			array[0] = this;
			array[1] = ship;
			array[2] = this.dockedLocation;
			int num2 = 3;
			TISpaceGameState tispaceGameState = this.dockedLocation;
			array[num2] = ((tispaceGameState != null) ? tispaceGameState.ref_habSite : null);
			int num3 = 4;
			TIOrbitState orbitState = base.orbitState;
			array[num3] = ((orbitState != null) ? orbitState.ref_naturalSpaceObject : null);
			eventManager.TriggerEvent(gameEvent, text, (from x in array.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			if (oldFleet != null)
			{
				ship.currentFleetOffset = new Vector3d(oldFleetPosition.x - currentPosition.x, oldFleetPosition.y - currentPosition.y, oldFleetPosition.z - currentPosition.z);
			}
			else
			{
				ship.currentFleetOffset = ship.defaultPositionOnCreation;
			}
			if (base.faction != null && base.faction.isActivePlayer)
			{
				if (this.ships.Count >= 10)
				{
					base.faction.UnlockAchievement("controlBigFleet");
				}
				if (this.SpaceCombatValue() >= 9000f)
				{
					base.faction.UnlockAchievement("controlStrongFleet");
				}
			}
			if (this.SpaceCombatValue() > num && this.AI_FailedAttackEnemyStrength != null)
			{
				this.AI_FailedAttackEnemyStrength.Clear();
			}
		}

		// Token: 0x06004434 RID: 17460 RVA: 0x001BC2AC File Offset: 0x001BA4AC
		public void RemoveShipsFromFleet(List<TISpaceShipState> shipsToRemove, TISpaceFleetState newFleet = null)
		{
			bool flag = this.transferAssigned && this.fleetTrajectoryData != null;
			double num = 0.0;
			if (flag)
			{
				num = this.fleetTrajectoryData.initialDeltaV_mps - (double)this.currentDeltaV_mps;
			}
			foreach (TISpaceShipState tispaceShipState in shipsToRemove)
			{
				this.ships.Remove(tispaceShipState);
			}
			if (flag)
			{
				this.fleetTrajectoryData.initialDeltaV_mps = (double)this.currentDeltaV_mps + num;
			}
			this.ForceIconResourceReset();
			if (GameControl.control.skirmishMode)
			{
				if (this.ships.Count == 0)
				{
					this.Disband();
				}
				return;
			}
			TIFactionState faction = base.faction;
			if (faction != null)
			{
				faction.SetMissionControlUsageDataDirty();
			}
			if (this.ships.Count == 0)
			{
				if (((newFleet != null) ? newFleet.faction : null) == base.faction)
				{
					TIFactionState[] array = GameStateManager.AllFactions();
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SubstituteFleetAsGoalTarget(this, newFleet);
					}
				}
				else
				{
					TIFactionState[] array = GameStateManager.AllFactions();
					for (int i = 0; i < array.Length; i++)
					{
						array[i].SubstituteFleetAsGoalTarget(this, null);
					}
				}
				if (newFleet != null)
				{
					this.gameTime.SubstituteStatesInTimeQueue(this, newFleet);
				}
				EventManager eventManager = GameControl.eventManager;
				GameEvent gameEvent = new ShipsRemovedFromFleet(this, newFleet);
				string text = null;
				object[] array2 = new object[6];
				array2[0] = this;
				array2[1] = shipsToRemove;
				array2[2] = this.dockedLocation;
				int num2 = 3;
				TISpaceGameState tispaceGameState = this.dockedLocation;
				array2[num2] = ((tispaceGameState != null) ? tispaceGameState.ref_habSite : null);
				int num3 = 4;
				TIOrbitState orbitState = base.orbitState;
				array2[num3] = ((orbitState != null) ? orbitState.ref_naturalSpaceObject : null);
				int num4 = 5;
				TIOrbitState orbitState2 = base.orbitState;
				object obj;
				if (orbitState2 == null)
				{
					obj = null;
				}
				else
				{
					TILagrangePointState ref_lagrangePoint = orbitState2.ref_lagrangePoint;
					obj = ((ref_lagrangePoint != null) ? ref_lagrangePoint.GetSunOrbitingRelatedObject : null);
				}
				array2[num4] = obj;
				eventManager.TriggerEvent(gameEvent, text, (from x in array2.Distinct<object>()
					where x != null
					select x).ToArray<object>());
				this.Disband();
			}
			else
			{
				EventManager eventManager2 = GameControl.eventManager;
				GameEvent gameEvent2 = new ShipsRemovedFromFleet(this, null);
				string text2 = null;
				object[] array3 = new object[6];
				array3[0] = this;
				array3[1] = shipsToRemove;
				array3[2] = this.dockedLocation;
				int num5 = 3;
				TISpaceGameState tispaceGameState2 = this.dockedLocation;
				array3[num5] = ((tispaceGameState2 != null) ? tispaceGameState2.ref_habSite : null);
				int num6 = 4;
				TIOrbitState orbitState3 = base.orbitState;
				array3[num6] = ((orbitState3 != null) ? orbitState3.ref_naturalSpaceObject : null);
				int num7 = 5;
				TIOrbitState orbitState4 = base.orbitState;
				object obj2;
				if (orbitState4 == null)
				{
					obj2 = null;
				}
				else
				{
					TILagrangePointState ref_lagrangePoint2 = orbitState4.ref_lagrangePoint;
					obj2 = ((ref_lagrangePoint2 != null) ? ref_lagrangePoint2.GetSunOrbitingRelatedObject : null);
				}
				array3[num7] = obj2;
				eventManager2.TriggerEvent(gameEvent2, text2, (from x in array3.Distinct<object>()
					where x != null
					select x).ToArray<object>());
				if (!TIGlobalValuesState.isSpaceCombatEnabled && !this.inCombat)
				{
					this.ResetFormation(false, false);
					this.TeleportAllToFormation(false, false);
				}
			}
			TIFactionState faction2 = base.faction;
			if (faction2 == null)
			{
				return;
			}
			faction2.GetMissionControlUsage();
		}

		// Token: 0x06004435 RID: 17461 RVA: 0x001BC594 File Offset: 0x001BA794
		public TISpaceShipState GetFlagship()
		{
			if (this.ships.Count > 1)
			{
				return this.ships.MinBy<TISpaceShipState, float>((TISpaceShipState x) => x.FleetMissionControlMultiplier());
			}
			if (this.ships.Count == 1)
			{
				return this.ships[0];
			}
			return null;
		}

		// Token: 0x06004436 RID: 17462 RVA: 0x001BC5F6 File Offset: 0x001BA7F6
		public int RawMissionControlConsumption()
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.hull.missionControl);
		}

		// Token: 0x06004437 RID: 17463 RVA: 0x001BC624 File Offset: 0x001BA824
		public int MissionControlConsumption()
		{
			if (this.ships.Count > 0)
			{
				Trajectory trajectory = this.trajectory;
				if (trajectory == null || !trajectory.exitsSolarSystem)
				{
					int num = this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.missionControlConsumption);
					if (this.ships.Count > 1)
					{
						num = Mathf.CeilToInt((float)num * this.ships.Min<TISpaceShipState>((TISpaceShipState x) => x.FleetMissionControlMultiplier()));
					}
					return num;
				}
			}
			return 0;
		}

		// Token: 0x06004438 RID: 17464 RVA: 0x001BC6C8 File Offset: 0x001BA8C8
		public int GetFleetOrbitInterestLevel()
		{
			if (!base.faction.permanentAlly(GameControl.control.activePlayer))
			{
				TIOrbitState tiorbitState;
				if (this.inTransfer)
				{
					tiorbitState = TISpaceFleetState.FinalDestinationOrbit(this);
				}
				else
				{
					tiorbitState = base.orbitState;
				}
				if (tiorbitState != null)
				{
					return tiorbitState.OrbitInterestLevel(GameControl.control.activePlayer);
				}
			}
			return -1;
		}

		// Token: 0x06004439 RID: 17465 RVA: 0x001BC720 File Offset: 0x001BA920
		public static TIOrbitState FinalDestinationOrbit(TISpaceFleetState fleet)
		{
			if (fleet.inTransfer)
			{
				TISpaceGameState destination = fleet.trajectory.destination;
				if (destination != null)
				{
					if (destination.isOrbitState || destination.isHabState || (destination.isSpaceFleetState && !destination.ref_fleet.inTransfer))
					{
						return destination.ref_orbit;
					}
					if (destination.isSpaceFleetState)
					{
						return TISpaceFleetState.FinalDestinationOrbit(destination.ref_fleet);
					}
				}
			}
			return fleet.ref_orbit;
		}

		// Token: 0x0600443A RID: 17466 RVA: 0x001BC790 File Offset: 0x001BA990
		public static TINaturalSpaceObjectState FinalDestinationNaturalSpaceObject(TISpaceFleetState fleet)
		{
			if (fleet.inTransfer)
			{
				TISpaceGameState destination = fleet.trajectory.destination;
				if (destination != null)
				{
					if (destination.isOrbitState || destination.isHabState)
					{
						return destination.ref_naturalSpaceObject;
					}
					if (destination.isSpaceFleetState)
					{
						return TISpaceFleetState.FinalDestinationNaturalSpaceObject(destination.ref_fleet);
					}
				}
			}
			return fleet.ref_naturalSpaceObject;
		}

		// Token: 0x0600443B RID: 17467 RVA: 0x001BC7EB File Offset: 0x001BA9EB
		public TIDateTime GetArrivalTimeSortWeight()
		{
			if (this.trajectory == null || (this.trajectory != null && !this.trajectory.launched))
			{
				return new TIDateTime(DateTime.MaxValue);
			}
			return this.trajectory.arrivalTime;
		}

		// Token: 0x0600443C RID: 17468 RVA: 0x001BC820 File Offset: 0x001BAA20
		public List<TISpaceShipState> ShipsWithSpecialModuleRule(SpecialModuleRule rule)
		{
			return this.ShipsWithSpecialModuleRule(new List<SpecialModuleRule> { rule });
		}

		// Token: 0x0600443D RID: 17469 RVA: 0x001BC834 File Offset: 0x001BAA34
		public List<TISpaceShipState> ShipsWithSpecialModuleRule(List<SpecialModuleRule> rules)
		{
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			Func<SpecialModuleRule, bool> <>9__0;
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				IEnumerable<SpecialModuleRule> enumerable = tispaceShipState.SpecialModuleRules(false);
				Func<SpecialModuleRule, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (SpecialModuleRule x) => rules.Contains(x));
				}
				if (enumerable.Any<SpecialModuleRule>(func))
				{
					list.Add(tispaceShipState);
				}
			}
			return list;
		}

		// Token: 0x0600443E RID: 17470 RVA: 0x001BC8CC File Offset: 0x001BAACC
		public void ExpendSpecialModuleCapability(List<SpecialModuleRule> capability, bool all = false, bool destroyEntireShip = false)
		{
			List<TISpaceShipState> list = this.ShipsWithSpecialModuleRule(capability);
			if (destroyEntireShip)
			{
				if (all)
				{
					using (List<TISpaceShipState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceShipState tispaceShipState = enumerator.Current;
							tispaceShipState.DestroyShip(false, null);
						}
						return;
					}
				}
				TISpaceShipState tispaceShipState2 = list.OrderBy<TISpaceShipState, float>((TISpaceShipState x) => x.currentDeltaV_kps).FirstOrDefault<TISpaceShipState>();
				if (tispaceShipState2 != null)
				{
					tispaceShipState2.DestroyShip(false, null);
					return;
				}
			}
			else
			{
				if (all)
				{
					using (List<TISpaceShipState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceShipState tispaceShipState3 = enumerator.Current;
							tispaceShipState3.GetFunctionalUtilitySlotModuleTemplates(1f);
							foreach (ModuleDataEntry moduleDataEntry in tispaceShipState3.utilityModules)
							{
								TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
								if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Intersect<SpecialModuleRule>(capability).Any<SpecialModuleRule>())
								{
									tispaceShipState3.DestroyPart(moduleDataEntry);
								}
							}
						}
						return;
					}
				}
				TISpaceShipState tispaceShipState4 = list.OrderBy<TISpaceShipState, float>((TISpaceShipState x) => x.currentDeltaV_kps).FirstOrDefault<TISpaceShipState>();
				tispaceShipState4.GetFunctionalUtilitySlotModuleTemplates(1f);
				foreach (ModuleDataEntry moduleDataEntry2 in tispaceShipState4.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule2 = moduleDataEntry2.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Intersect<SpecialModuleRule>(capability).Any<SpecialModuleRule>() && tispaceShipState4.GetPartFunction(moduleDataEntry2) >= 1f)
					{
						tispaceShipState4.DestroyPart(moduleDataEntry2);
						break;
					}
				}
			}
		}

		// Token: 0x0600443F RID: 17471 RVA: 0x001BCAD8 File Offset: 0x001BACD8
		public bool HasSpecialModuleCapability(SpecialModuleRule capability)
		{
			return this.ShipsWithSpecialModuleRule(capability).Any<TISpaceShipState>((TISpaceShipState ship) => ship.utilityModules.Any<ModuleDataEntry>((ModuleDataEntry x) => ship.GetPartFunction(x) >= 1f));
		}

		// Token: 0x06004440 RID: 17472 RVA: 0x001BCB05 File Offset: 0x001BAD05
		public bool HasFoundHabCapability()
		{
			return TISpaceShipState.FoundAnyHabRules.Any<SpecialModuleRule>((SpecialModuleRule x) => this.HasSpecialModuleCapability(x));
		}

		// Token: 0x06004441 RID: 17473 RVA: 0x001BCB20 File Offset: 0x001BAD20
		public float BombardmentValue(TISpaceBodyState spaceBody)
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.BombardmentValue(spaceBody));
		}

		// Token: 0x06004442 RID: 17474 RVA: 0x001BCB54 File Offset: 0x001BAD54
		public float BombardmentValue(TISpaceBodyState spaceBody, float range_km)
		{
			return this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.BombardmentValue(spaceBody, range_km));
		}

		// Token: 0x06004443 RID: 17475 RVA: 0x001BCB8C File Offset: 0x001BAD8C
		public float SalvageBonus()
		{
			float num = 0f;
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				foreach (TIShipModuleTemplate tishipModuleTemplate in tispaceShipState.utilityModuleTemplates)
				{
					if (tishipModuleTemplate.isUtilityModule && tishipModuleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.SalvageBonus))
					{
						num += tishipModuleTemplate.ref_utilityModule.salvageBonus;
					}
				}
				num += tispaceShipState.SumOfficerEffectsModifiers(OfficerEffectType.Salvage, num);
			}
			if (num > 0.25f)
			{
				float num2 = num - 0.25f;
				num = 0.25f + 0.25f * (num2 / (num2 + 2f));
			}
			return num;
		}

		// Token: 0x06004444 RID: 17476 RVA: 0x001BCC80 File Offset: 0x001BAE80
		public new void SetDisplayName(string displayName)
		{
			this.SetDisplayName(base.faction, displayName, false);
		}

		// Token: 0x06004445 RID: 17477 RVA: 0x001BCC90 File Offset: 0x001BAE90
		public void ForceDisplayName(TIFactionState faction, string name)
		{
			this.displayNameByFaction[faction] = name;
		}

		// Token: 0x06004446 RID: 17478 RVA: 0x001BCCA0 File Offset: 0x001BAEA0
		public void SetDisplayName(TIFactionState detectingFaction, string forceString = null, bool nameEdit = false)
		{
			string text = string.Empty;
			if (string.IsNullOrEmpty(forceString))
			{
				if (!detectingFaction.factionFleetsEncountered.ContainsKey(base.faction))
				{
					detectingFaction.factionFleetsEncountered[base.faction] = 0;
				}
				text = new StringBuilder(base.faction.fleetNameBase).Append(detectingFaction.factionFleetsEncountered[base.faction]).ToString();
			}
			if (this.displayNameByFaction.ContainsKey(detectingFaction))
			{
				this.displayNameByFaction[detectingFaction] = text;
				if (nameEdit)
				{
					this.displayNameByFaction[detectingFaction] = forceString;
				}
			}
			else
			{
				this.displayNameByFaction.Add(detectingFaction, text);
			}
			if (detectingFaction == base.faction)
			{
				this.displayName = text;
			}
		}

		// Token: 0x06004447 RID: 17479 RVA: 0x001BCD5B File Offset: 0x001BAF5B
		public override string GetDisplayName(TIFactionState namingFaction)
		{
			if (namingFaction == null)
			{
				return this.displayName;
			}
			if (!this.displayNameByFaction.ContainsKey(namingFaction))
			{
				this.SetDisplayName(namingFaction, null, false);
			}
			return this.displayNameByFaction[namingFaction];
		}

		// Token: 0x06004448 RID: 17480 RVA: 0x001BCD90 File Offset: 0x001BAF90
		public bool DetailsVisibleToFaction(TIFactionState faction)
		{
			return faction.HasIntelOnFleetShipDetails(this);
		}

		// Token: 0x06004449 RID: 17481 RVA: 0x001BCD9C File Offset: 0x001BAF9C
		public List<TICouncilorState> CouncilorsPresentAndKnownToFaction(TIFactionState faction)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.councilorPassengers)
			{
				CouncilorView councilorView = new CouncilorView(ticouncilorState, faction);
				if (councilorView.location != null)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x0600444A RID: 17482 RVA: 0x001BCE10 File Offset: 0x001BB010
		public List<CouncilorView> CouncilorViewsPresentAndKnownToFaction(TIFactionState faction)
		{
			List<CouncilorView> list = new List<CouncilorView>();
			foreach (TICouncilorState ticouncilorState in this.councilorPassengers)
			{
				CouncilorView councilorView = new CouncilorView(ticouncilorState, faction);
				if (councilorView.location != null)
				{
					list.Add(councilorView);
				}
			}
			return list;
		}

		// Token: 0x0600444B RID: 17483 RVA: 0x001BCE84 File Offset: 0x001BB084
		public bool DoIKnowThisFleetIsTransfering(TIFactionState myFaction)
		{
			return this.transferAssigned && (this.trajectory.launchTime <= TITimeState.Now() || (!(myFaction == null) && base.faction.permanentAlly(myFaction)));
		}

		// Token: 0x0600444C RID: 17484 RVA: 0x001BCEC0 File Offset: 0x001BB0C0
		protected void RemoveTransfer()
		{
			foreach (OperationData operationData in (from x in this.CurrentOperations()
				where x.operation is TransferOperation
				select x).ToList<OperationData>())
			{
				this.CancelOperation(operationData);
			}
			this.trajectory = null;
			if (base.gameObjectLink != null && base.gameObjectLink.HasComponent<TransferPlanComponent>())
			{
				Log.Error(string.Concat(new string[]
				{
					"Transfer Plan Component on ",
					this.displayName,
					" ",
					this.ToString(),
					" was not cleaned up properly."
				}), Array.Empty<object>());
				base.gameObjectLink.Remove<TransferPlanComponent>(true);
			}
			World.Active.GetExistingManager<SpaceObjectPositioning>().TriggerForceUpdate();
		}

		// Token: 0x0600444D RID: 17485 RVA: 0x001BCFB8 File Offset: 0x001BB1B8
		public void Land(TIHabSiteState site)
		{
			this.ForceCancelCurrentOperations();
			this.dockedLocation = site;
			this.ships.ForEach(delegate(TISpaceShipState x)
			{
				x.ConsumeDeltaV((float)site.DeltaVToLandFromInterface_kps(this.orbitState, (double)this.maxAcceleration_mps2, false, x.SpecialModuleRules(false).Contains(SpecialModuleRule.ImmunetoAerobrakingDamage)), true);
			});
			this.AssignFormation(this.dockedFormation, false, true, false, false, false);
			this.TeleportAllToFormation(false, false);
			site.LandFleet(this);
			TIOrbitState orbitState = base.orbitState;
			if (orbitState != null)
			{
				orbitState.assetsInOrbit.Remove(this);
			}
			GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, site, true), null, new object[] { this, site, site.ref_spaceBody });
		}

		// Token: 0x0600444E RID: 17486 RVA: 0x001BD078 File Offset: 0x001BB278
		public void Dock(TIHabState hab, bool newFleet = false)
		{
			this.ForceCancelCurrentOperations();
			this.dockedLocation = hab;
			if (hab.IsStation)
			{
				base.AssumeMatchingOrbitFromState(hab, true);
				Vector3d vector3d;
				hab.DockFleet(this, out vector3d);
				this.dockOffset = vector3d;
				GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, base.orbitState, false), null, new object[] { this, hab, hab.ref_naturalSpaceObject });
			}
			else
			{
				base.AssumeOrbitFromState(hab.habSite.parentBody.interfaceOrbits[0], 0.0, TITimeState.Now());
				if (!newFleet)
				{
					this.AssignFormation(this.dockedFormation, false, true, false, false, false);
					this.TeleportAllToFormation(false, false);
				}
				GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, hab, true), null, new object[] { this, hab, hab.habSite, hab.ref_naturalSpaceObject });
				Vector3d vector3d2;
				hab.DockFleet(this, out vector3d2);
				this.dockOffset = vector3d2;
				TIOrbitState orbitState = base.orbitState;
				if (orbitState != null)
				{
					orbitState.assetsInOrbit.Remove(this);
				}
			}
			if (base.faction == hab.faction && hab.anyCoreCompleted && hab.GetSunOrbitingRelatedObject.semiMajorAxis_AU < 6.0 && !base.faction.unlockedVictoryObjective)
			{
				foreach (TIOfficerState tiofficerState in this.ships.SelectMany<TISpaceShipState, TIOfficerState>((TISpaceShipState x) => x.officers).ToList<TIOfficerState>())
				{
					if (TITimeState.Now() > tiofficerState.retirementDate)
					{
						tiofficerState.RetireOfficer();
					}
				}
			}
			if (base.faction.player.isAI)
			{
				this.AI_OptimizeOfficers();
			}
			World.Active.GetExistingManager<SpaceObjectPositioning>().TriggerForceUpdate();
		}

		// Token: 0x0600444F RID: 17487 RVA: 0x001BD274 File Offset: 0x001BB474
		public void DepartFromDockingLocation()
		{
			foreach (OperationData operationData in this.CurrentOperations())
			{
				if (operationData.operation.GetOperationTiming() != OperationTiming.InstantExecution && (operationData.operation as TISpaceFleetOperationTemplate).CancelUponDepartHab())
				{
					this.CancelOperation(operationData);
				}
			}
			TIHabState ref_hab = this.dockedLocation.ref_hab;
			if (this.dockedLocation.ref_habSite != null)
			{
				foreach (TISpaceFleetState tispaceFleetState in this.dockedLocation.ref_habSite.parentBody.interfaceOrbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit).ToList<TISpaceFleetState>())
				{
					if (tispaceFleetState.bombardmentTarget == this)
					{
						tispaceFleetState.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.TargetFleetTookOff);
					}
				}
			}
			if (ref_hab != null)
			{
				ref_hab.LaunchFleet(this);
			}
			else
			{
				TIHabSiteState ref_habSite = this.dockedLocation.ref_habSite;
				if (ref_habSite != null)
				{
					ref_habSite.LaunchFleet(this);
				}
			}
			GameControl.eventManager.TriggerEvent(new FleetUndocks(this, this.dockedLocation), null, (from x in new object[]
				{
					this,
					this.dockedLocation,
					this.dockedLocation.ref_hab,
					this.dockedLocation.ref_habSite,
					this.dockedLocation.ref_naturalSpaceObject
				}.Distinct<object>()
				where x != null
				select x).ToArray<object>());
			this.dockedLocation = null;
		}

		// Token: 0x06004450 RID: 17488 RVA: 0x001BD444 File Offset: 0x001BB644
		public void SetAccelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false)
		{
			if (!forceStop)
			{
				if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.inManeuver))
				{
					return;
				}
			}
			bool inAccelerationPhase = this.inAccelerationPhase;
			this.inAccelerationPhase = inPhase;
			if (this.inAccelerationPhase != inAccelerationPhase)
			{
				for (int i = 0; i < this.ships.Count; i++)
				{
					if (this.inAccelerationPhase || this.inDecelerationPhase)
					{
						this.ships[i].ActivateThrusters();
						this.ships[i].InitiateManuever(this.ships[i].currentFleetOffset, (Quaternion)this.ships[i].GetDesiredRotation(false));
					}
					else
					{
						this.ships[i].DeactivateThrusters();
					}
				}
				return;
			}
			if (forceRotation)
			{
				for (int j = 0; j < this.ships.Count; j++)
				{
					this.ships[j].InitiateManuever(this.ships[j].currentFleetOffset, (Quaternion)this.ships[j].GetDesiredRotation(false));
				}
			}
		}

		// Token: 0x06004451 RID: 17489 RVA: 0x001BD574 File Offset: 0x001BB774
		public void SetDecelerationPhaseStatus(bool inPhase, bool forceRotation = false, bool forceStop = false)
		{
			if (!forceStop)
			{
				if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.inManeuver))
				{
					return;
				}
			}
			bool inDecelerationPhase = this.inDecelerationPhase;
			this.inDecelerationPhase = inPhase;
			if (inDecelerationPhase != this.inDecelerationPhase)
			{
				for (int i = 0; i < this.ships.Count; i++)
				{
					if (this.inDecelerationPhase || this.inAccelerationPhase)
					{
						this.ships[i].ActivateThrusters();
						this.ships[i].InitiateManuever(this.ships[i].currentFleetOffset, (Quaternion)this.ships[i].GetDesiredRotation(false));
					}
					else
					{
						this.ships[i].DeactivateThrusters();
					}
				}
				return;
			}
			if (forceRotation)
			{
				for (int j = 0; j < this.ships.Count; j++)
				{
					this.ships[j].InitiateManuever(this.ships[j].currentFleetOffset, (Quaternion)this.ships[j].GetDesiredRotation(false));
				}
			}
		}

		// Token: 0x06004452 RID: 17490 RVA: 0x001BD69F File Offset: 0x001BB89F
		public void AssignTrajectory(Trajectory trajectoryToAssign)
		{
			if (trajectoryToAssign == null)
			{
				Log.Error("Attempted to assing a null trajectory to " + this.displayName, Array.Empty<object>());
				return;
			}
			this.trajectory = trajectoryToAssign;
			this.AddFleetLog("AssignTrajectory");
		}

		// Token: 0x06004453 RID: 17491 RVA: 0x001BD6D4 File Offset: 0x001BB8D4
		public void RefreshTrajectory()
		{
			this.trajectory.fleetAsSpaceFleetState = this;
			this.trajectory.fleet = this;
			if (base.gameObjectLink != null)
			{
				TransferPlanComponent orAdd = base.gameObjectLink.GetOrAdd<TransferPlanComponent>();
				orAdd.Value.fleet = this;
				orAdd.Value.StartPoint = this.trajectory.launchPosition;
				orAdd.Value.TotalSeconds = this.trajectory.duration_s;
				orAdd.Value.StartTime = this.trajectory.launchTime.ExportTime();
				orAdd.Value.EndTime = this.trajectory.arrivalTime.ExportTime();
				orAdd.Value.EndPoint = this.trajectory.destinationPosition;
				orAdd.Value.planningOnly = false;
				orAdd.Value.commonBarycenter = this.trajectory.commonBarycenter;
				if (orAdd.Value.TransferSegments == null)
				{
					orAdd.Value.TransferSegments = new List<Orbit>();
				}
			}
		}

		// Token: 0x06004454 RID: 17492 RVA: 0x001BD7DC File Offset: 0x001BB9DC
		public void GlobalCheckNotifyFleetLaunch()
		{
			List<TIFactionState> list = (from x in GameStateManager.AllFactions()
				where x != base.faction && base.VisibleToFaction(x)
				select x).ToList<TIFactionState>();
			TISpaceGameState destination = this.trajectory.destination;
			if (destination != null && destination.isSpaceAssetState)
			{
				List<TIFactionState> list2 = list.Intersect<TIFactionState>(this.trajectory.destination.ref_spaceAsset.ref_factions).ToList<TIFactionState>();
				if (list2.Count > 0)
				{
					TINotificationQueueState.LogFleetLaunchesTowardMyAsset(this, this.trajectory.destination.ref_spaceAsset, list2, this.trajectory.arrivalTime);
				}
			}
			else
			{
				if (this.IsAlien() && this.HasSpecialModuleCapability(SpecialModuleRule.LandArmy) && list.Count > 0)
				{
					TISpaceGameState destination2 = this.trajectory.destination;
					bool? flag;
					if (destination2 == null)
					{
						flag = null;
					}
					else
					{
						TISpaceBodyState ref_spaceBody = destination2.ref_spaceBody;
						flag = ((ref_spaceBody != null) ? new bool?(ref_spaceBody.isEarth) : null);
					}
					bool? flag2 = flag;
					if (flag2.GetValueOrDefault())
					{
						TINotificationQueueState.LogAssaultCarrierLaunchesTowardEarth(list, this, this.trajectory.arrivalTime);
						goto IL_0174;
					}
				}
				TISpaceGameState destination3 = this.trajectory.destination;
				if (destination3 != null && destination3.isOrbitState && this.trajectory.originOrbit != this.trajectory.destinationOrbit)
				{
					List<TIFactionState> list3 = list.Intersect<TIFactionState>(from x in GameStateManager.AllFactions()
						where this.trajectory.destinationOrbit.OrbitInterestLevel(x) == 3
						select x).ToList<TIFactionState>();
					TINotificationQueueState.LogFleetLaunchesTowardMyAsset(this, this.trajectory.destination.ref_orbit, list3, this.trajectory.arrivalTime);
				}
			}
			IL_0174:
			AIDailyFactionPlanner.AIReaction(AIReactionEvent.FleetStartedTransfer, this, null);
		}

		// Token: 0x06004455 RID: 17493 RVA: 0x001BD968 File Offset: 0x001BBB68
		public void LaunchFleet(bool alertInterceptingFleets = true)
		{
			if (this.trajectory.launchTime > TITimeState.Now())
			{
				if (base.controller != null)
				{
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
				}
				new TransferOperation().OnOperationConfirm(this, this.trajectory.destination, null, this.trajectory);
				this.trajectory.launched = false;
				if (base.orbitState == null)
				{
					base.AssumeOrbitFromState(this.trajectory.originOrbit, 0.0, null);
				}
				return;
			}
			if (!this.trajectory.launched)
			{
				this.trajectory.launched = true;
				if (this.dockedOrLanded)
				{
					this.DepartFromDockingLocation();
				}
				this.ForceCancelCurrentOperations();
				if (this.trajectory.RemainingDVatTime_mps(TITimeState.Now()) > (double)this.currentDeltaV_mps)
				{
					if (base.orbitState == null)
					{
						Debug.LogWarning("Aborting transfer prior to launch, but fleet lacks an orbitState.  Using trajectory.originOrbit instead.  This may be innaccurate if we were launching from a trajectory.");
						base.AssumeOrbitFromState(this.trajectory.originOrbit, 0.0, null);
					}
					TINotificationQueueState.LogTrajectoryAborted(this, 1, 4, null, null);
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
					this.trajectory = null;
					return;
				}
				this.fleetTrajectoryData.initialDeltaV_mps = (double)this.currentDeltaV_mps;
			}
			this.barycenter = this.trajectory.commonBarycenter;
			if (this.trajectory.HasOrbitalElements())
			{
				OrbitalElementsState transferOrbit = (this.trajectory as Trajectory_WithOrbitalElements).transferOrbit;
				base.epoch_DateTime = new TIDateTime(transferOrbit.epoch);
			}
			TIOrbitState orbitState = base.orbitState;
			if (orbitState != null)
			{
				orbitState.assetsInOrbit.Remove(this);
			}
			base.orbitState = null;
			base.controller.UpdateOrbitComponentForAsset(true);
			if (base.controller.orbitTrailLink != null)
			{
				global::UnityEngine.Object.Destroy(base.controller.orbitTrailLink);
			}
			this.RefreshTrajectory();
			TransferPlanComponent orAdd = base.gameObjectLink.GetOrAdd<TransferPlanComponent>();
			orAdd.Value.TransferSegments = new List<Orbit>();
			orAdd.Value.TransferSegments.Add(new Orbit
			{
				Barycenter = this.trajectory.commonBarycenter.gameObjectLink.GetComponent<TIGameObjectEntity>().Entity
			}.Fill(true).FillTransferOrbit(this, this.trajectory, out base.controller.orbitTrailLink));
			if (alertInterceptingFleets)
			{
				IEnumerable<TISpaceGameState> fleetsToIgnore = this.CheckForTransferTargetLoop().Union<TISpaceGameState>(this.GetChainedDestinations());
				List<TISpaceFleetState> nearbyAlliedFleets = this.GetNearbyIdleAlliedFleets(null);
				double num = 0.0;
				TISpaceFleetState tispaceFleetState = null;
				double num2 = 0.0;
				foreach (TISpaceFleetState tispaceFleetState2 in nearbyAlliedFleets)
				{
					double num3 = (double)tispaceFleetState2.SpaceCombatValue();
					num += num3;
					if (num3 > num2)
					{
						num2 = num3;
						tispaceFleetState = tispaceFleetState2;
					}
				}
				double num4 = (double)this.SpaceCombatValue();
				if (num > num4)
				{
					IEnumerable<TISpaceFleetState> enumerable = from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
						where x.faction != this.faction && x.transferAssigned && !fleetsToIgnore.Contains(x) && x.trajectory.destinationFleet == this
						select x;
					List<TISpaceGameState> chainedDestinations = tispaceFleetState.GetChainedDestinations();
					using (IEnumerator<TISpaceFleetState> enumerator2 = enumerable.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TISpaceFleetState tispaceFleetState3 = enumerator2.Current;
							if (chainedDestinations.Contains(tispaceFleetState3))
							{
								tispaceFleetState3.trajectory.ChangeDestinationFleet(this);
							}
							else
							{
								tispaceFleetState3.trajectory.ChangeDestinationFleet(tispaceFleetState);
							}
						}
						goto IL_03A0;
					}
				}
				if (num4 > num)
				{
					foreach (TISpaceFleetState tispaceFleetState4 in from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
						where x.faction != this.faction && x.transferAssigned && !fleetsToIgnore.Contains(x) && nearbyAlliedFleets.Contains(x.trajectory.destinationFleet)
						select x)
					{
						tispaceFleetState4.trajectory.ChangeDestinationFleet(this);
					}
				}
				IL_03A0:
				foreach (Trajectory trajectory in (from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
					where x.transferAssigned && !fleetsToIgnore.Contains(x)
					select x.trajectory into x
					where x.destinationFleet == this
					where x.destinationFleetTrajectory != this.trajectory
					select x).ToList<Trajectory>())
				{
					TISpaceFleetState tispaceFleetState5 = trajectory.fleet as TISpaceFleetState;
					if (tispaceFleetState5 != null)
					{
						tispaceFleetState5.TransferTargetFleetHasManeuvered();
					}
				}
			}
		}

		// Token: 0x06004456 RID: 17494 RVA: 0x001BDDEC File Offset: 0x001BBFEC
		public List<TISpaceFleetState> CheckForTransferTargetLoop()
		{
			if (!this.transferAssigned || this.trajectory.destinationFleet == null)
			{
				return new List<TISpaceFleetState>();
			}
			int num = GameStateManager.GetCount<TISpaceFleetState>(true) + 1;
			TISpaceFleetState tispaceFleetState = this;
			TISpaceFleetState tispaceFleetState2 = this;
			for (int i = 0; i < num; i++)
			{
				if (!tispaceFleetState2.transferAssigned || tispaceFleetState2.trajectory.destinationFleet == null)
				{
					return new List<TISpaceFleetState> { this };
				}
				tispaceFleetState2 = tispaceFleetState2.trajectory.destinationFleet;
				if (!tispaceFleetState2.transferAssigned || tispaceFleetState2.trajectory.destinationFleet == null)
				{
					return new List<TISpaceFleetState> { this };
				}
				if (tispaceFleetState2 == tispaceFleetState)
				{
					break;
				}
				tispaceFleetState2 = tispaceFleetState2.trajectory.destinationFleet;
				if (tispaceFleetState2 == tispaceFleetState)
				{
					break;
				}
				tispaceFleetState = tispaceFleetState.trajectory.destinationFleet;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Fleet targeting loop detected:");
			stringBuilder.Append(this.GetDisplayName(GameControl.control.activePlayer));
			stringBuilder.Append(" -> ");
			List<TISpaceFleetState> list = new List<TISpaceFleetState> { this };
			for (int j = 0; j < num; j++)
			{
				TISpaceFleetState destinationFleet = list.Last<TISpaceFleetState>().trajectory.destinationFleet;
				if (list.Contains(destinationFleet))
				{
					break;
				}
				list.Add(destinationFleet);
				stringBuilder.Append(destinationFleet.GetDisplayName(GameControl.control.activePlayer));
				stringBuilder.Append(" -> ");
			}
			stringBuilder.Append(list.Last<TISpaceFleetState>().trajectory.destinationFleet.GetDisplayName(GameControl.control.activePlayer));
			Log.Error(stringBuilder.ToString(), Array.Empty<object>());
			return list;
		}

		// Token: 0x06004457 RID: 17495 RVA: 0x001BDF9C File Offset: 0x001BC19C
		public List<TISpaceGameState> GetChainedDestinations()
		{
			List<TISpaceGameState> list = new List<TISpaceGameState>();
			if (this.trajectory == null)
			{
				return list;
			}
			TISpaceGameState tispaceGameState = this.trajectory.destination;
			list.Add(tispaceGameState);
			int count = GameStateManager.GetCount<TISpaceFleetState>(true);
			for (int i = 0; i < count; i++)
			{
				TISpaceFleetState tispaceFleetState = tispaceGameState as TISpaceFleetState;
				if (tispaceFleetState == null || tispaceFleetState.trajectory == null)
				{
					break;
				}
				tispaceGameState = tispaceFleetState.trajectory.destination;
				list.Add(tispaceGameState);
			}
			return list;
		}

		// Token: 0x06004458 RID: 17496 RVA: 0x001BE00C File Offset: 0x001BC20C
		public List<TISpaceFleetState> GetFleetsWeAreIntercepting(bool destroyBadFleet = false)
		{
			List<TISpaceFleetState> list = new List<TISpaceFleetState>();
			int num = GameStateManager.GetCount<TISpaceFleetState>(true) + 1;
			TISpaceFleetState tispaceFleetState = this;
			for (int i = 0; i < num; i++)
			{
				if (!tispaceFleetState.transferAssigned || tispaceFleetState.trajectory.destinationFleet == null)
				{
					return list;
				}
				tispaceFleetState = tispaceFleetState.trajectory.destinationFleet;
				if (list.Contains(tispaceFleetState))
				{
					break;
				}
				list.Add(tispaceFleetState);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Fleet targeting loop detected:");
			stringBuilder.Append(this.GetDisplayName(GameControl.control.activePlayer));
			stringBuilder.Append(" -> ");
			foreach (TISpaceFleetState tispaceFleetState2 in list)
			{
				stringBuilder.Append(tispaceFleetState2.GetDisplayName(GameControl.control.activePlayer));
				stringBuilder.Append(" -> ");
			}
			stringBuilder.Append(list.Last<TISpaceFleetState>().trajectory.destinationFleet.GetDisplayName(GameControl.control.activePlayer));
			Log.Error(stringBuilder.ToString(), Array.Empty<object>());
			return list;
		}

		// Token: 0x06004459 RID: 17497 RVA: 0x001BE140 File Offset: 0x001BC340
		public void TransferTargetFleetHasManeuvered()
		{
			Trajectory trajectory = this.trajectory;
			bool flag;
			if (trajectory == null)
			{
				flag = null != null;
			}
			else
			{
				TISpaceFleetState destinationFleet = trajectory.destinationFleet;
				flag = ((destinationFleet != null) ? destinationFleet.trajectory : null) != null;
			}
			if (!flag)
			{
				return;
			}
			TISpaceFleetState destinationFleet2 = this.trajectory.destinationFleet;
			Trajectory trajectory2 = this.trajectory.destinationFleetTrajectory;
			int num = 0;
			while (num < 100 && trajectory2 != null)
			{
				if (trajectory2 == this.trajectory.destinationFleet.trajectory)
				{
					return;
				}
				trajectory2 = trajectory2.nextTrajectory;
				num++;
			}
			Vector3d position = this.trajectory.ToGlobalCartesianStateAtTime(this.trajectory.arrivalTime).position;
			Vector3d position2 = destinationFleet2.ToGlobalCartesianStateAtTime(this.trajectory.arrivalTime).position;
			if ((position - position2).magnitude < 1000000.0)
			{
				return;
			}
			TIDateTime launchTime = this.trajectory.destinationFleet.trajectory.launchTime;
			double num2 = this.trajectory.DVConsumedOnTrajectory_mps(TITimeState.Now()) - this.trajectory.DVConsumedOnTrajectory_mps(launchTime);
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				tispaceShipState.RefundDeltaV((float)(-(float)num2 / 1000.0));
			}
			Trajectory trajectory3 = this.trajectory;
			TISpaceFleetState destinationFleet3 = this.trajectory.destinationFleet;
			bool flag2 = false;
			this.trajectory.DeTargetFleet();
			if (!this.trajectory.launched)
			{
				this.trajectory = null;
				flag2 = true;
			}
			TransferResult transferResult;
			try
			{
				double num3;
				transferResult = MasterTransferPlanner.RequestTrajectories(this, destinationFleet3, 64, delegate(Trajectory[] outTrajectories)
				{
					this.proposedTrajectories = outTrajectories;
				}, out num3, false, false, 1.0);
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message + "\n" + ex.StackTrace, Array.Empty<object>());
				transferResult = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			bool flag3 = false;
			if (flag2)
			{
				this.trajectory = trajectory3;
			}
			double num4;
			if (transferResult.TryGetMinimumDVneeded_mps(out num4))
			{
				flag3 = true;
				TINotificationQueueState.LogTrajectoryTargetManeuveredAndWeCannotChase(this, 0, destinationFleet2);
			}
			else if (transferResult.TryGetMinimumAccelerationNeeded(out num4, (double)this.cruiseAcceleration_mps2))
			{
				flag3 = true;
				TINotificationQueueState.LogTrajectoryTargetManeuveredAndWeCannotChase(this, 1, destinationFleet2);
			}
			else if (this.proposedTrajectories == null || this.proposedTrajectories.Length == 0)
			{
				flag3 = true;
				TINotificationQueueState.LogTrajectoryTargetManeuveredAndWeCannotChase(this, -1, destinationFleet2);
			}
			else
			{
				TIPromptQueueState.AddPromptStatic(base.faction, this, destinationFleet2, "PromptChangeTrajectory", 0);
			}
			if (flag3)
			{
				TIDateTime tidateTime = TITimeState.Now();
				DateTime dateTime = tidateTime.ExportTime();
				TINaturalSpaceObjectState exactBarycenterAtTime = this.trajectory.GetExactBarycenterAtTime(tidateTime);
				OrbitalElementsState orbitalElementsState = this.trajectory.ToGlobalCartesianStateAtTime(tidateTime).ToLocal(exactBarycenterAtTime, tidateTime).ToOrbitalElementsState(exactBarycenterAtTime.mu, new DateTime?(dateTime));
				if (orbitalElementsState.NextTimeAtMeanAnomaly(0.0, dateTime, exactBarycenterAtTime.mass_kg) > dateTime && orbitalElementsState.periapsis_m <= exactBarycenterAtTime.meanRadius_m)
				{
					flag3 = false;
				}
			}
			if (flag3)
			{
				this.destroyProposedTrajectories();
				this.AbortTransfer(0, null, false);
			}
		}

		// Token: 0x0600445A RID: 17498 RVA: 0x001BE450 File Offset: 0x001BC650
		public double TrajectoryFractionCompleted()
		{
			if (this.trajectory.duration_s <= 0.0)
			{
				return 1.0;
			}
			if (this.trajectory.launched)
			{
				return Mathd.Clamp(this.gameTime.Now.Subtract(this.trajectory.launchTime.ExportTime()).TotalSeconds / this.trajectory.durationFromLaunchToFinalArrival_s, 0.0, 1.0);
			}
			return 0.0;
		}

		// Token: 0x0600445B RID: 17499 RVA: 0x001BE4E2 File Offset: 0x001BC6E2
		public double DVConsumedOnTrajectory_kps()
		{
			if (this.inTransfer)
			{
				return this.trajectory.DVConsumedOnTrajectory_mps(TITimeState.Now()) / 1000.0;
			}
			return 0.0;
		}

		// Token: 0x0600445C RID: 17500 RVA: 0x001BE510 File Offset: 0x001BC710
		public float DVRequiredToCompleteTrajectory_kps()
		{
			if (this.inTransfer)
			{
				return (float)(this.trajectory.DV_kps - this.DVConsumedOnTrajectory_kps());
			}
			return 0f;
		}

		// Token: 0x0600445D RID: 17501 RVA: 0x001BE534 File Offset: 0x001BC734
		public void FinishWaitingToInitiateCombat()
		{
			if (this.waitingToInitiateCombatDatas == null || this.waitingToInitiateCombatDatas.Count == 0)
			{
				return;
			}
			TISpaceFleetState.WaitingToInitiateCombatData waitingToInitiateCombatData = this.waitingToInitiateCombatDatas.First<TISpaceFleetState.WaitingToInitiateCombatData>();
			this.waitingToInitiateCombatDatas.Remove(waitingToInitiateCombatData);
			if (this.waitingToInitiateCombatDatas.Count == 0)
			{
				this.waitingToInitiateCombatDatas = null;
				TISpaceFleetState.fleetsWaitingToInitiateCombat.Remove(this);
			}
			if (!TIGameState.Valid(this))
			{
				Log.Error("Invalid fleet no longer waiting to initiate combat.", Array.Empty<object>());
				return;
			}
			if ((waitingToInitiateCombatData.TargetFleet != null && waitingToInitiateCombatData.TargetFleet.deleted) || (waitingToInitiateCombatData.TargetHab != null && waitingToInitiateCombatData.TargetHab.deleted))
			{
				Log.Error("waitingToInitiateCombatData had stale data. Deleting.", Array.Empty<object>());
				return;
			}
			if (!this.InitiateCombat(waitingToInitiateCombatData.TargetFleet, waitingToInitiateCombatData.TargetHab, false))
			{
				this.waitingToInitiateCombatDatas.Insert(0, waitingToInitiateCombatData);
				return;
			}
			string[] array = new string[5];
			array[0] = ((this != null) ? this.ToString() : null);
			array[1] = " has finished waiting to initiate combat with ";
			int num = 2;
			TISpaceFleetState targetFleet = waitingToInitiateCombatData.TargetFleet;
			array[num] = ((targetFleet != null) ? targetFleet.ToString() : null) ?? "null";
			array[3] = " and ";
			int num2 = 4;
			TIHabState targetHab = waitingToInitiateCombatData.TargetHab;
			array[num2] = ((targetHab != null) ? targetHab.ToString() : null) ?? "null";
			Log.Debug(string.Concat(array), Array.Empty<object>());
		}

		// Token: 0x0600445E RID: 17502 RVA: 0x001BE684 File Offset: 0x001BC884
		public bool InitiateCombat(TISpaceFleetState targetFleet, TIHabState hab, bool allowDummyFleetToStart = false)
		{
			TISpaceFleetState.<>c__DisplayClass418_0 CS$<>8__locals1 = new TISpaceFleetState.<>c__DisplayClass418_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.targetFleet = targetFleet;
			CS$<>8__locals1.hab = hab;
			if (CS$<>8__locals1.targetFleet != null && CS$<>8__locals1.targetFleet.deleted)
			{
				CS$<>8__locals1.targetFleet = null;
			}
			if (CS$<>8__locals1.hab != null && CS$<>8__locals1.hab.deleted)
			{
				CS$<>8__locals1.hab = null;
			}
			if (CS$<>8__locals1.targetFleet == null && (CS$<>8__locals1.hab == null || (!allowDummyFleetToStart && CS$<>8__locals1.hab.SpaceCombatValue() == 0f && CS$<>8__locals1.hab.dockedFleets.None<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(CS$<>8__locals1.<>4__this.faction)))))
			{
				return true;
			}
			CS$<>8__locals1.unresolvedCombats = (from x in GameStateManager.IterateByClass<TISpaceCombatState>(false)
				where !x.archived
				select x).ToList<TISpaceCombatState>();
			if (CS$<>8__locals1.unresolvedCombats.Any<TISpaceCombatState>())
			{
				if (this.waitingToInitiateCombatDatas == null)
				{
					this.waitingToInitiateCombatDatas = new List<TISpaceFleetState.WaitingToInitiateCombatData>();
				}
				this.waitingToInitiateCombatDatas.Add(new TISpaceFleetState.WaitingToInitiateCombatData
				{
					TargetFleet = CS$<>8__locals1.targetFleet,
					TargetHab = CS$<>8__locals1.hab
				});
				TISpaceFleetState.fleetsWaitingToInitiateCombat.Add(this);
				string[] array = new string[7];
				array[0] = ((this != null) ? this.ToString() : null);
				array[1] = " is waiting to initiate combat with ";
				int num = 2;
				TISpaceFleetState targetFleet2 = CS$<>8__locals1.targetFleet;
				array[num] = ((targetFleet2 != null) ? targetFleet2.ToString() : null) ?? "null";
				array[3] = " and ";
				int num2 = 4;
				TIHabState hab2 = CS$<>8__locals1.hab;
				array[num2] = ((hab2 != null) ? hab2.ToString() : null) ?? "null";
				array[5] = ". Stack trace: ";
				array[6] = StackTraceUtility.ExtractStackTrace();
				Log.Debug(string.Concat(array), Array.Empty<object>());
				return false;
			}
			CS$<>8__locals1.preservedFleets = new Dictionary<TIFactionState, List<PreservedFleetRecord>>();
			if (CS$<>8__locals1.hab != null)
			{
				List<TISpaceFleetState> list = CS$<>8__locals1.hab.dockedFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x != CS$<>8__locals1.<>4__this && x != CS$<>8__locals1.targetFleet).ToList<TISpaceFleetState>();
				List<TISpaceFleetState> list2 = new List<TISpaceFleetState>();
				if (list.Count > 0 && CS$<>8__locals1.targetFleet == null)
				{
					if (CS$<>8__locals1.hab.faction.permanentAlly(base.faction))
					{
						CS$<>8__locals1.targetFleet = list.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(CS$<>8__locals1.<>4__this.faction)).MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
					}
					else if (list.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction == CS$<>8__locals1.hab.faction))
					{
						CS$<>8__locals1.targetFleet = list.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction == CS$<>8__locals1.hab.faction).MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
					}
					else if (list.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(CS$<>8__locals1.hab.faction)))
					{
						CS$<>8__locals1.targetFleet = list.Where<TISpaceFleetState>((TISpaceFleetState x) => x.faction.permanentAlly(CS$<>8__locals1.hab.faction)).MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
					}
					if (CS$<>8__locals1.targetFleet != null)
					{
						list.Remove(CS$<>8__locals1.targetFleet);
					}
				}
				foreach (TISpaceFleetState tispaceFleetState in list)
				{
					if (base.faction.permanentAlly(tispaceFleetState.faction))
					{
						CS$<>8__locals1.<InitiateCombat>g__MergeFleetIn|2(tispaceFleetState, this);
					}
					else
					{
						list2.Add(tispaceFleetState);
					}
				}
				foreach (TISpaceFleetState tispaceFleetState2 in list2)
				{
					tispaceFleetState2.faction.playerControl.StartAction(new ConfirmOperationAction(tispaceFleetState2, tispaceFleetState2, new CancelFleetOperation(), null, null));
					tispaceFleetState2.DepartFromDockingLocation();
					if (tispaceFleetState2.faction != CS$<>8__locals1.hab.faction)
					{
						TINotificationQueueState.LogFleetEjectedFromStation(tispaceFleetState2, CS$<>8__locals1.hab);
					}
				}
			}
			TISpaceFleetState targetFleet3 = CS$<>8__locals1.targetFleet;
			if (targetFleet3 != null && !targetFleet3.inTransfer && base.orbitState != null)
			{
				IEnumerable<TISpaceFleetState> enumerable = from x in (from x in base.orbitState.fleetsInOrbit
						where x != CS$<>8__locals1.<>4__this && x != CS$<>8__locals1.targetFleet
						where !x.landed
						where CS$<>8__locals1.unresolvedCombats.None<TISpaceCombatState>((TISpaceCombatState y) => y.cachedFleet1 == x || y.cachedFleet2 == x)
						where CS$<>8__locals1.unresolvedCombats.None<TISpaceCombatState>((TISpaceCombatState y) => y.fleets[0] == x || y.fleets[1] == x)
						select x).ToList<TISpaceFleetState>()
					where x.ships.Count > 0
					select x;
				if (CS$<>8__locals1.targetFleet != null)
				{
					foreach (TISpaceFleetState tispaceFleetState3 in enumerable.Intersect<TISpaceFleetState>(TIFactionState.GetDefenders(CS$<>8__locals1.targetFleet)))
					{
						CS$<>8__locals1.<InitiateCombat>g__MergeFleetIn|2(tispaceFleetState3, CS$<>8__locals1.targetFleet);
					}
				}
			}
			List<TISpaceShipState> ships = this.ships;
			if (ships != null)
			{
				ships.ForEach(delegate(TISpaceShipState x)
				{
					x.SetPropulsionValuesDirty(true, false);
				});
			}
			TISpaceFleetState targetFleet4 = CS$<>8__locals1.targetFleet;
			if (targetFleet4 != null)
			{
				List<TISpaceShipState> ships2 = targetFleet4.ships;
				if (ships2 != null)
				{
					ships2.ForEach(delegate(TISpaceShipState x)
					{
						x.SetPropulsionValuesDirty(true, false);
					});
				}
			}
			List<TISpaceShipState> ships3 = this.ships;
			if (ships3 != null)
			{
				ships3.ForEach(delegate(TISpaceShipState x)
				{
					x.SetCombatSystems();
				});
			}
			TISpaceFleetState targetFleet5 = CS$<>8__locals1.targetFleet;
			if (targetFleet5 != null)
			{
				List<TISpaceShipState> ships4 = targetFleet5.ships;
				if (ships4 != null)
				{
					ships4.ForEach(delegate(TISpaceShipState x)
					{
						x.SetCombatSystems();
					});
				}
			}
			TISpaceCombatState tispaceCombatState = GameStateManager.CreateNewGameState<TISpaceCombatState>();
			tispaceCombatState.preservedFleetCompositions = new Dictionary<TIFactionState, List<PreservedFleetRecord>>(CS$<>8__locals1.preservedFleets);
			tispaceCombatState.allowNoAttackingFleetAtInitialization = allowDummyFleetToStart;
			this.inCombat = true;
			this.combatState = tispaceCombatState;
			if (CS$<>8__locals1.targetFleet != null)
			{
				CS$<>8__locals1.targetFleet.inCombat = true;
				CS$<>8__locals1.targetFleet.combatState = tispaceCombatState;
			}
			tispaceCombatState.CacheCombatAssets(this, CS$<>8__locals1.targetFleet, CS$<>8__locals1.hab);
			return true;
		}

		// Token: 0x0600445F RID: 17503 RVA: 0x001BED10 File Offset: 0x001BCF10
		public Dictionary<float, List<TISpaceShipState>> GetAccelerationGroups(bool combat)
		{
			Dictionary<float, List<TISpaceShipState>> dictionary = new Dictionary<float, List<TISpaceShipState>>();
			foreach (TISpaceShipState tispaceShipState in this.ships)
			{
				float num = Mathf.Round(1000f * (combat ? tispaceShipState.combatAcceleration_mps2 : tispaceShipState.combatAcceleration_mps2)) / 1000f;
				if (!dictionary.ContainsKey(num))
				{
					dictionary.Add(num, new List<TISpaceShipState>());
				}
				dictionary[num].Add(tispaceShipState);
			}
			return dictionary;
		}

		// Token: 0x06004460 RID: 17504 RVA: 0x001BEDA8 File Offset: 0x001BCFA8
		public void AssumeTargetFleetTrajectory(Trajectory oldTrajectory)
		{
			double num = (double)this.currentDeltaV_mps;
			TIDateTime arrivalTime = this.trajectory.arrivalTime;
			Trajectory trajectory = oldTrajectory.ShallowCopy(this);
			this.AssignTrajectory(trajectory);
			this.trajectory.launched = true;
			this.trajectory.SetAsIntercept(true);
			this.LaunchFleet(true);
			double num2 = this.trajectory.DVConsumedOnTrajectory_mps(arrivalTime);
			this.fleetTrajectoryData.initialDeltaV_mps = num + num2;
		}

		// Token: 0x06004461 RID: 17505 RVA: 0x001BEE14 File Offset: 0x001BD014
		public bool PrecludeDockingWithEnemyStation(TIHabState station)
		{
			return station.dockedFleets.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				if (x.faction != this.faction && !x.faction.permanentAlly(station.faction))
				{
					return x.CurrentOperations().Any<OperationData>((OperationData x) => x.operation is AssaultHabOperation || x.operation is DestroyHabOperation);
				}
				return false;
			});
		}

		// Token: 0x06004462 RID: 17506 RVA: 0x001BEE56 File Offset: 0x001BD056
		public void ApproachDock(TIHabState hab)
		{
			if (hab.DockingRequiresCombat(this, hab.CanDefendHabWithSTOFighters()))
			{
				this.InitiateCombat(null, hab, hab.CanDefendHabWithSTOFighters());
				return;
			}
			if (hab.CanDock(this, false))
			{
				this.Dock(hab, false);
			}
		}

		// Token: 0x06004463 RID: 17507 RVA: 0x001BEE8C File Offset: 0x001BD08C
		public void ArriveFleet(bool startupFix = false)
		{
			if (this.trajectory == null)
			{
				this.RepairPotentialIllegalOrbit();
				Log.Error("ArriveFleet() called, but trajectory was null.  Putting the fleet into an orbit.", Array.Empty<object>());
				return;
			}
			this.unreachableLocations.Clear();
			TIOrbitState originOrbit = this.trajectory.originOrbit;
			TIAdHocOrbitState tiadHocOrbitState = originOrbit as TIAdHocOrbitState;
			if (this.trajectory.nextTrajectory != null)
			{
				if (this.trajectory.nextTrajectory.RemainingDVatTime_mps(TITimeState.Now()) > (double)this.currentDeltaV_mps)
				{
					this.AbortTransfer(1, null, false);
					return;
				}
				double num = (double)this.currentDeltaV_mps;
				TIDateTime arrivalTime = this.trajectory.arrivalTime;
				this.trajectory = this.trajectory.nextTrajectory;
				if (this.trajectory.fleet == this)
				{
					this.trajectory = this.trajectory.ShallowCopy(this);
				}
				this.LaunchFleet(true);
				double num2 = this.trajectory.DVConsumedOnTrajectory_mps(arrivalTime);
				this.fleetTrajectoryData.initialDeltaV_mps = num + num2;
				this.TryToRemoveAdHocOrbit(tiadHocOrbitState);
				return;
			}
			else
			{
				bool flag = false;
				this.alwaysShowOrbitTrailDuringTransfer = false;
				TISpaceFleetState tispaceFleetState = null;
				TIHabState tihabState = null;
				bool flag2 = false;
				this.SetAccelerationPhaseStatus(false, false, true);
				this.SetDecelerationPhaseStatus(false, false, true);
				base.gameObjectLink.Remove<TransferPlanComponent>(true);
				this.ships.ForEach(delegate(TISpaceShipState x)
				{
					x.SetPropulsionValuesDirty(true, false);
				});
				TISpaceGameState tispaceGameState = this.trajectory.destination;
				bool flag3 = false;
				if (this.trajectory.destroyOnArrival)
				{
					if (this.trajectory.endsInCrash)
					{
						TINotificationQueueState.LogFleetCrashes(this, this.trajectory.collisionTarget);
					}
					else if (this.trajectory.exitsSolarSystem)
					{
						TINotificationQueueState.LogFleetEscapesSolarSystem(this);
					}
					new List<TISpaceShipState>(this.ships).ForEach(delegate(TISpaceShipState ship)
					{
						ship.DestroyShip(true, null);
					});
					this.RemoveTransfer();
					this.TryToRemoveAdHocOrbit(tiadHocOrbitState);
					return;
				}
				if (this.trajectory.targetingFleet && TIGameState.Valid(this.trajectory.destinationFleet) && !this.trajectory.destinationFleet.landed)
				{
					TISpaceFleetState destinationFleet = this.trajectory.destinationFleet;
					tispaceGameState = destinationFleet;
					if (destinationFleet.inTransfer && destinationFleet.trajectory.launchTime < TITimeState.Now())
					{
						this.AssumeTargetFleetTrajectory(destinationFleet.trajectory);
						this.HandleSpaceObjectSelection(false);
						flag3 = true;
						GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, destinationFleet, false), null, new object[] { this });
					}
					else
					{
						if (destinationFleet.dockedAtStation && destinationFleet.faction == base.faction && destinationFleet.ref_hab.faction == base.faction)
						{
							this.Dock(destinationFleet.ref_hab, false);
						}
						else
						{
							if (!destinationFleet.dockedAtStation && this.dockedAtStation)
							{
								this.LaunchFleet(true);
							}
							if (destinationFleet.orbitState != null)
							{
								base.AssumeMatchingOrbitFromState(destinationFleet, false);
							}
							else
							{
								Log.Error(string.Concat(new string[] { "Error: ", this.displayName, " is trying to match orbits with ", destinationFleet.displayName, ", but no orbit found for target. Sending to ad hoc orbit instead." }), Array.Empty<object>());
								if (this.trajectory.destinationOrbit != null)
								{
									TIDateTime tidateTime = new TIDateTime(this.trajectory.arrivalTime, -1.0);
									OrbitalElementsState orbitalElementsAtTime = this.trajectory.GetOrbitalElementsAtTime(tidateTime);
									TINaturalSpaceObjectState barycenterAtTime = this.trajectory.GetBarycenterAtTime(tidateTime);
									if (orbitalElementsAtTime.eccentricity >= 1.0)
									{
										this.AbortTransfer(-1, tidateTime, false);
										return;
									}
									TIOrbitState tiorbitState;
									if (GameControl.loadcycle100)
									{
										tiorbitState = TIAdHocOrbitState.CreateAdHocOrbitState(barycenterAtTime, orbitalElementsAtTime, this);
									}
									else
									{
										tiorbitState = barycenterAtTime.GetClosestMatchingLegalOrbitState(orbitalElementsAtTime);
									}
									new TIDateTime(orbitalElementsAtTime.epoch);
									tispaceGameState = tiorbitState;
									base.AssumeOrbitStateFromPosition(tiorbitState, this.GetGlobalPositionAtTime(this.trajectory.arrivalTime), tiorbitState.barycenter.GetGlobalPositionAtTime(this.trajectory.arrivalTime), this.trajectory.arrivalTime, TISpaceAssetState.MeanAnomalyPrecision.Maximum);
								}
							}
						}
						this.HandleSpaceObjectSelection(false);
						GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, base.orbitState, false), null, new object[]
						{
							this,
							base.orbitState.barycenter
						});
						this.RemoveTransfer();
					}
					if (!destinationFleet.faction.permanentAlly(base.faction))
					{
						TIHabState tihabState2 = null;
						if (destinationFleet.dockedAtStation && destinationFleet.ref_hab.faction.permanentAlly(destinationFleet.faction))
						{
							tihabState2 = destinationFleet.ref_hab;
						}
						tispaceFleetState = destinationFleet;
						tihabState = tihabState2;
						flag = true;
					}
				}
				else if (this.trajectory.targetingStation)
				{
					TIHabState destinationStation = this.trajectory.destinationStation;
					tispaceGameState = destinationStation;
					if (destinationStation.CanDock(this, true))
					{
						if (destinationStation.faction == base.faction && this.trajectory.resupplyOnArrival)
						{
							flag2 = true;
						}
						this.Dock(this.trajectory.destinationStation, false);
					}
					else
					{
						base.AssumeMatchingOrbitFromState(this.trajectory.destinationStation, false);
						if (!destinationStation.deleted && destinationStation.DockingRequiresCombat(this, true))
						{
							if (!this.PrecludeDockingWithEnemyStation(destinationStation) && !destinationStation.faction.permanentAlly(base.faction))
							{
								tihabState = destinationStation;
								flag = true;
							}
							else if (destinationStation.faction.permanentAlly(base.faction))
							{
								if (tispaceFleetState == null)
								{
									tispaceFleetState = destinationStation.dockedFleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(base.faction));
								}
								flag = true;
							}
						}
					}
					this.RemoveTransfer();
				}
				else
				{
					TIOrbitState tiorbitState2 = this.trajectory.destinationOrbit;
					double? destinationOrbitMeanAnomalyAtEpoch = this.trajectory.destinationOrbitMeanAnomalyAtEpoch;
					TIDateTime tidateTime2 = this.trajectory.destinationOrbitEpoch;
					if (tiorbitState2 == null)
					{
						TIDateTime tidateTime3 = new TIDateTime(this.trajectory.arrivalTime, -1.0);
						OrbitalElementsState orbitalElementsAtTime2 = this.trajectory.GetOrbitalElementsAtTime(tidateTime3);
						TINaturalSpaceObjectState barycenterAtTime2 = this.trajectory.GetBarycenterAtTime(tidateTime3);
						if (orbitalElementsAtTime2.eccentricity >= 1.0)
						{
							this.AbortTransfer(-1, tidateTime3, false);
							return;
						}
						if (GameControl.loadcycle100)
						{
							tiorbitState2 = TIAdHocOrbitState.CreateAdHocOrbitState(barycenterAtTime2, orbitalElementsAtTime2, this);
						}
						else
						{
							tiorbitState2 = barycenterAtTime2.GetClosestMatchingLegalOrbitState(orbitalElementsAtTime2);
						}
						destinationOrbitMeanAnomalyAtEpoch = new double?(orbitalElementsAtTime2.meanAnomalyAtEpoch_Rad);
						tidateTime2 = new TIDateTime(orbitalElementsAtTime2.epoch);
					}
					tispaceGameState = tiorbitState2;
					if (destinationOrbitMeanAnomalyAtEpoch != null && tidateTime2 != null)
					{
						this.RemoveTransfer();
						double num3 = destinationOrbitMeanAnomalyAtEpoch.Value;
						base.AssumeOrbitStateGivenMeanAnomalyAtEpoch(tiorbitState2, tidateTime2, num3);
					}
					else
					{
						TIDateTime arrivalTime2 = this.trajectory.arrivalTime;
						Vector3d globalPositionAtTime = this.GetGlobalPositionAtTime(arrivalTime2);
						this.RemoveTransfer();
						base.AssumeOrbitStateFromPosition(tiorbitState2, globalPositionAtTime, tiorbitState2.barycenter.GetGlobalPositionAtTime(arrivalTime2), arrivalTime2, TISpaceAssetState.MeanAnomalyPrecision.Maximum);
					}
					this.HandleSpaceObjectSelection(false);
					GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(this, base.orbitState, false), null, new object[]
					{
						this,
						base.orbitState.barycenter
					});
				}
				this.ships.ForEach(delegate(TISpaceShipState x)
				{
					x.SetVisualizationDataDirty();
				});
				if (base.faction.fleetGoalTracker.ContainsKey(this))
				{
					FactionGoal_Fleet factionGoal_Fleet = base.faction.fleetGoalTracker[this];
					if (factionGoal_Fleet != null)
					{
						factionGoal_Fleet.OnTransferComplete();
					}
				}
				if (flag)
				{
					this.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.SetPropulsionValuesDirty(true, true);
					});
					if (!startupFix)
					{
						this.InitiateCombat(tispaceFleetState, tihabState, tihabState != null && tihabState.CanDefendHabWithSTOFighters());
					}
				}
				else
				{
					bool flag4 = false;
					bool flag5 = false;
					if (flag2)
					{
						bool flag6 = this.NeedsRefuel() || this.NeedsRearm();
						bool flag7 = this.NeedsRepair();
						bool flag8 = false;
						ResupplyAndRepairOperation resupplyAndRepairOperation = new ResupplyAndRepairOperation();
						if (flag7)
						{
							flag8 = resupplyAndRepairOperation.ActorCanPerformOperation(this, this);
						}
						if (flag6 || flag7)
						{
							if (flag6 && (!flag7 || !flag8))
							{
								if (new ResupplyOperation().ActorCanPerformOperation(this, this))
								{
									base.faction.playerControl.StartAction(new ConfirmOperationAction(this, this, OperationsManager.operationsLookup[typeof(ResupplyOperation)], null, null));
									flag4 = true;
								}
							}
							else if (resupplyAndRepairOperation.ActorCanPerformOperation(this, this))
							{
								base.faction.playerControl.StartAction(new ConfirmOperationAction(this, this, OperationsManager.operationsLookup[typeof(ResupplyAndRepairOperation)], null, null));
								flag5 = true;
							}
							else if (flag7 && new RepairFleetOperation().ActorCanPerformOperation(this, this))
							{
								base.faction.playerControl.StartAction(new ConfirmOperationAction(this, this, OperationsManager.operationsLookup[typeof(RepairFleetOperation)], null, null));
								flag5 = true;
							}
						}
					}
					List<TIOfficerState> list = new List<TIOfficerState>();
					TINaturalSpaceObjectState ref_naturalSpaceObject = originOrbit.ref_naturalSpaceObject;
					TIGameState tigameState = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.GetSunOrbitingRelatedObject : null);
					TINaturalSpaceObjectState ref_naturalSpaceObject2 = tispaceGameState.ref_naturalSpaceObject;
					if (tigameState != ((ref_naturalSpaceObject2 != null) ? ref_naturalSpaceObject2.GetSunOrbitingRelatedObject : null))
					{
						foreach (TISpaceShipState tispaceShipState in this.ships.ToList<TISpaceShipState>().Shuffle<TISpaceShipState>())
						{
							list.AddRange(tispaceShipState.CheckForOfficerPromotionEvent(OfficerSpawnEventType.CompleteLongTrajectory, 0f, false, null));
						}
					}
					Dictionary<TIFactionState, string> dictionary = new Dictionary<TIFactionState, string>();
					if (list.Count > 0)
					{
						dictionary.Add(base.faction, TIOfficerTemplate.BuildOfficerPromotionReport(list, base.faction));
					}
					if (!flag3)
					{
						TINotificationQueueState.LogFleetArrival(this, originOrbit, tispaceGameState, flag4, flag5, dictionary);
					}
					TISpaceBodyState tispaceBodyState;
					if (originOrbit == null)
					{
						tispaceBodyState = null;
					}
					else
					{
						TINaturalSpaceObjectState ref_naturalSpaceObject3 = originOrbit.ref_naturalSpaceObject;
						if (ref_naturalSpaceObject3 == null)
						{
							tispaceBodyState = null;
						}
						else
						{
							TISpaceObjectState getSunOrbitingRelatedObject = ref_naturalSpaceObject3.GetSunOrbitingRelatedObject;
							tispaceBodyState = ((getSunOrbitingRelatedObject != null) ? getSunOrbitingRelatedObject.ref_spaceBody : null);
						}
					}
					TISpaceBodyState tispaceBodyState2 = tispaceBodyState;
					TINaturalSpaceObjectState ref_naturalSpaceObject4 = this.ref_naturalSpaceObject;
					TISpaceBodyState tispaceBodyState3;
					if (ref_naturalSpaceObject4 == null)
					{
						tispaceBodyState3 = null;
					}
					else
					{
						TISpaceObjectState getSunOrbitingRelatedObject2 = ref_naturalSpaceObject4.GetSunOrbitingRelatedObject;
						tispaceBodyState3 = ((getSunOrbitingRelatedObject2 != null) ? getSunOrbitingRelatedObject2.ref_spaceBody : null);
					}
					TISpaceBodyState tispaceBodyState4 = tispaceBodyState3;
					if (!base.deleted && tispaceBodyState4 != null && tispaceBodyState4 != GameStateManager.Sol() && tispaceBodyState2 != tispaceBodyState4 && !startupFix)
					{
						AIDailyFactionPlanner.AIReaction(AIReactionEvent.HostileFleetArrivesAroundNaturalSpaceObject, this.ref_naturalSpaceObject, this);
					}
				}
				if (!startupFix)
				{
					this.TryToRemoveAdHocOrbit(tiadHocOrbitState);
				}
				this.AddFleetLog("Arrive");
				if (base.orbitState == null || base.ref_system == GameStateManager.Sol())
				{
					return;
				}
				if (!startupFix && TIGameState.Valid(this))
				{
					List<TISpaceFleetState> list2 = (from x in base.faction.fleets.Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(this.<ArriveFleet>g__IsRally|423_5))
						where x.inTransfer && (TITimeState.Now() - x.trajectory.arrivalTime).TotalMinutes < 30.0
						select x).ToList<TISpaceFleetState>();
					List<TISpaceFleetState> list3 = (from x in this.GetNearbyIdleAlliedFleets(null).Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(this.<ArriveFleet>g__IsRally|423_5))
						where !x.inTransfer
						select x).ToList<TISpaceFleetState>();
					if (base.faction != null && base.faction.player.isAI && list2.Count == 0 && list3.Count == 0)
					{
						AIDailyFactionPlanner.SingleFleetOperation(this, true);
					}
					foreach (TISpaceFleetState tispaceFleetState2 in list3)
					{
						AIDailyFactionPlanner.SingleFleetOperation(tispaceFleetState2, true);
					}
				}
				return;
			}
		}

		// Token: 0x06004464 RID: 17508 RVA: 0x001BF9F8 File Offset: 0x001BDBF8
		public void SetHomePort(TIHabState hab)
		{
			if (hab != null && !hab.deleted && hab.ref_factions.Contains(base.faction))
			{
				this.homeport = hab;
				return;
			}
			this.homeport = null;
		}

		// Token: 0x06004465 RID: 17509 RVA: 0x001BFA30 File Offset: 0x001BDC30
		protected override OrbitalElementsState ToOrbitalElementsState(TIDateTime time = null)
		{
			if (base.orbitState != null)
			{
				return base.ToOrbitalElementsState(time);
			}
			return default(OrbitalElementsState);
		}

		// Token: 0x06004466 RID: 17510 RVA: 0x001BFA5C File Offset: 0x001BDC5C
		public override CartesianState ToLocalCartesianStateAtTime(TIDateTime time)
		{
			if (this.inTransfer)
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState = this.trajectory.GetBarycenterAtTime(time);
				if (tinaturalSpaceObjectState == null)
				{
					Log.Error("Trajectory has no barycenter at " + ((time != null) ? time.ToString() : null) + ".", Array.Empty<object>());
					tinaturalSpaceObjectState = this.trajectory.GetBarycenterAtTime(time);
				}
				return this.trajectory.ToGlobalCartesianStateAtTime(time).ToLocal(tinaturalSpaceObjectState, time);
			}
			if (base.orbitState == null)
			{
				Log.Error("Fleet " + this.displayName + " has neither a transfer, nor an orbit state.  Its location is undefined.", Array.Empty<object>());
				return default(CartesianState);
			}
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(this.longAscendingNode_Rad, this.argPeriapsis_Rad, this.inclination_Rad, this.semiMajorAxis_m, this.ecc, this.meanAnomalyAtEpoch_Rad, base.epoch_DateTime.ExportTime());
			return orbitalElementsState.ToCartesianStateAtTime(time.ExportTime(), (this.barycenter == null) ? 0.0 : this.barycenter.mass_kg);
		}

		// Token: 0x06004467 RID: 17511 RVA: 0x001BFB70 File Offset: 0x001BDD70
		public bool AnyValidTrajectory(TIGameState destination)
		{
			bool found64 = false;
			try
			{
				Func<Trajectory, bool> <>9__1;
				double num;
				MasterTransferPlanner.RequestTrajectories(this, destination, 64, delegate(Trajectory[] trajectories)
				{
					bool flag;
					if (trajectories.Length != 0)
					{
						Func<Trajectory, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (Trajectory x) => x.DV_mps <= (double)this.currentDeltaV_mps && x.launchTime != x.arrivalTime);
						}
						flag = trajectories.Any<Trajectory>(func);
					}
					else
					{
						flag = false;
					}
					found64 = flag;
				}, out num, false, false, 1.0);
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message + "\n" + ex.StackTrace, Array.Empty<object>());
			}
			return found64;
		}

		// Token: 0x06004468 RID: 17512 RVA: 0x001BFBF0 File Offset: 0x001BDDF0
		public void VerifyAssignedTransfer(bool delayNotification = false)
		{
			if (!this.transferAssigned)
			{
				return;
			}
			double DVneededToCompleteTrajectory_mps = this.trajectory.RemainingDVatTime_mps(TITimeState.Now());
			if ((double)this.currentDeltaV_mps < DVneededToCompleteTrajectory_mps || (double)this.cruiseAcceleration_mps2 < this.trajectory.fleetCruiseAcceleration_mps2 / 2.0)
			{
				int num = 1;
				if (this.ships.Any<TISpaceShipState>((TISpaceShipState ship) => (double)(ship.currentDeltaV_kps * 1000f) >= DVneededToCompleteTrajectory_mps && (double)ship.cruiseAcceleration_mps2 >= this.trajectory.fleetCruiseAcceleration_mps2 / 2.0))
				{
					num = 2;
				}
				this.AbortTransfer(num, null, delayNotification);
			}
		}

		// Token: 0x06004469 RID: 17513 RVA: 0x001BFC7C File Offset: 0x001BDE7C
		public void AbortTransfer(int cause, TIDateTime now = null, bool delayNotification = false)
		{
			if (!this.transferAssigned)
			{
				return;
			}
			if (now == null)
			{
				now = TITimeState.Now();
			}
			if (this.inTransfer)
			{
				TIDateTime orbitEndTime = this.trajectory.getOrbitEndTime();
				if (orbitEndTime != null)
				{
					foreach (TISpaceFleetState tispaceFleetState in this.GetAllFleetsRendezvousingWithUs().ToList<TISpaceFleetState>())
					{
						if (tispaceFleetState.trajectory.arrivalTime > orbitEndTime)
						{
							tispaceFleetState.AbortTransfer(0, null, false);
						}
					}
				}
				if (cause != 2)
				{
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
					this.SetAccelerationPhaseStatus(false, false, false);
					this.SetDecelerationPhaseStatus(false, false, false);
				}
				if (this.trajectory.arrivalTime <= TITimeState.Now() && this.trajectory.destinationOrbit != null)
				{
					if (base.controller != null)
					{
						base.gameObjectLink.Remove<TransferPlanComponent>(true);
					}
					else if (GameControl.loadcycle100)
					{
						Log.Error("Couldn't remove TransferPlanComponent because controller was null.", Array.Empty<object>());
					}
					TIOrbitState destinationOrbit = this.trajectory.destinationOrbit;
					double num;
					TIDateTime tidateTime;
					if (this.trajectory.destinationOrbitMeanAnomalyAtEpoch != null && this.trajectory.destinationOrbitEpoch != null)
					{
						num = this.trajectory.destinationOrbitMeanAnomalyAtEpoch.Value;
						tidateTime = this.trajectory.destinationOrbitEpoch;
					}
					else
					{
						num = this.trajectory.getDestinationMeanAnomalyAtArrival();
						tidateTime = this.trajectory.arrivalTime;
					}
					base.AssumeOrbitFromState(destinationOrbit, num, tidateTime);
					this.trajectory = null;
					this.AddFleetLog("Abort2");
					return;
				}
				TINaturalSpaceObjectState exactBarycenterAtTime = this.trajectory.GetExactBarycenterAtTime(now);
				CartesianState cartesianState = this.trajectory.ToGlobalCartesianStateAtTime(now).ToLocal(exactBarycenterAtTime, now);
				OrbitalElementsState orbitalElementsState = cartesianState.ToOrbitalElementsState(exactBarycenterAtTime.mu, new DateTime?(now.ExportTime()));
				TISpaceBodyState tispaceBodyState = null;
				if (Mathd.Approximately(orbitalElementsState.eccentricity, 1.0))
				{
					double num2 = Mathd.Sqrt(exactBarycenterAtTime.mu / cartesianState.position.magnitude);
					Vector3d normalized = cartesianState.position.normalized;
					Vector3d normalized2 = (cartesianState.velocity - Vector3d.Dot(in cartesianState.velocity, in normalized) * normalized).normalized;
					Vector3d vector3d = num2 * normalized2;
					cartesianState.velocity += vector3d * 0.1;
					orbitalElementsState = cartesianState.ToOrbitalElementsState(exactBarycenterAtTime.mu, new DateTime?(now.ExportTime()));
				}
				Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
				bool flag = trajectory_Patched.BuildCoastTrajectory(this, this.trajectory, now, orbitalElementsState, exactBarycenterAtTime);
				int num3 = 0;
				TISpaceFleetState tispaceFleetState2 = null;
				if (flag)
				{
					this.trajectory = trajectory_Patched;
					tispaceBodyState = trajectory_Patched.collisionTarget;
					if (this.trajectory.exitsSolarSystem)
					{
						num3 = 2;
					}
					if (this.trajectory.endsInCrash)
					{
						num3 = 5;
					}
					try
					{
						if (this.trajectory.destroyOnArrival)
						{
							EmergencyBurnPlanner.EmergencyBurnSolution emergencyBurnSolution = EmergencyBurnPlanner.Solve(this, this.trajectory as Trajectory_Patched);
							if (emergencyBurnSolution.abandonedShips.Count < this.ships.Count && emergencyBurnSolution.rescueTrajectory != null)
							{
								this.trajectory = emergencyBurnSolution.rescueTrajectory;
								num3 = emergencyBurnSolution.outcome;
								if (emergencyBurnSolution.abandonedShips.Count > 0)
								{
									tispaceFleetState2 = TISpaceFleetState.CreateAtRunTime(base.faction, emergencyBurnSolution.abandonedShips, this, this, null, false, false, trajectory_Patched);
								}
							}
						}
					}
					catch (Exception ex)
					{
						Log.Error("Abort Transfer - Exception while planning emergency burn : " + ex.Message + ex.StackTrace.SplitLines().First<string>(), Array.Empty<object>());
					}
					this.LaunchFleet(true);
				}
				else
				{
					List<TISpaceShipState> list = this.ships.Where<TISpaceShipState>((TISpaceShipState ship) => (double)(ship.currentDeltaV_kps * 1000f) < this.trajectory.RemainingDVatTime_mps(now) || (double)ship.cruiseAcceleration_mps2 < this.trajectory.fleetCruiseAcceleration_mps2 / 2.0).ToList<TISpaceShipState>();
					if (list.Count == this.ships.Count || !GameControl.loadcycle100 || list.Count == 0)
					{
						base.gameObjectLink.Remove<TransferPlanComponent>(true);
						this.trajectory = null;
						TIOrbitState tiorbitState;
						if (GameControl.loadcycle100)
						{
							tiorbitState = TIAdHocOrbitState.CreateAdHocOrbitState(exactBarycenterAtTime, orbitalElementsState, this);
						}
						else
						{
							tiorbitState = exactBarycenterAtTime.GetClosestMatchingLegalOrbitState(orbitalElementsState);
						}
						base.AssumeOrbitFromState(tiorbitState, orbitalElementsState.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElementsState.epoch));
					}
					else
					{
						tispaceFleetState2 = TISpaceFleetState.CreateAtRunTime(base.faction, list, this, this, null, false, false, null);
						tispaceFleetState2.gameObjectLink.Remove<TransferPlanComponent>(true);
						tispaceFleetState2.SetAccelerationPhaseStatus(false, false, false);
						tispaceFleetState2.SetDecelerationPhaseStatus(false, false, false);
						tispaceFleetState2.trajectory = null;
						TIOrbitState tiorbitState2 = TIAdHocOrbitState.CreateAdHocOrbitState(exactBarycenterAtTime, orbitalElementsState, tispaceFleetState2);
						tispaceFleetState2.AssumeOrbitFromState(tiorbitState2, orbitalElementsState.meanAnomalyAtEpoch_Rad, new TIDateTime(orbitalElementsState.epoch));
					}
				}
				if (cause == 2 && tispaceFleetState2 == null)
				{
					cause = 1;
				}
				if (cause >= 0)
				{
					if (!delayNotification)
					{
						TINotificationQueueState.LogTrajectoryAborted(this, cause, num3, tispaceFleetState2, tispaceBodyState);
					}
					else
					{
						this.delayedTransferAbortNotification = new TISpaceFleetState.DelayedTransferAbortNotification
						{
							cause = cause,
							outcome = num3,
							doomedFleet = tispaceFleetState2,
							collisionTarget = tispaceBodyState
						};
					}
				}
			}
			else
			{
				if (base.controller != null)
				{
					base.gameObjectLink.Remove<TransferPlanComponent>(true);
				}
				else if (GameControl.loadcycle100)
				{
					Log.Error("Couldn't remove TransferPlanComponent because controller was null.", Array.Empty<object>());
				}
				this.trajectory = null;
			}
			this.AddFleetLog("Abort");
		}

		// Token: 0x0600446A RID: 17514 RVA: 0x001C021C File Offset: 0x001BE41C
		private IEnumerable<TISpaceFleetState> GetAllFleetsRendezvousingWithUs()
		{
			return from fleet in GameStateManager.IterateByClass<TISpaceFleetState>(false)
				where fleet.transferAssigned && fleet.trajectory.destinationFleet == this
				select fleet;
		}

		// Token: 0x0600446B RID: 17515 RVA: 0x001C0238 File Offset: 0x001BE438
		private void TryToRemoveAdHocOrbit(TIAdHocOrbitState adHocOrbit)
		{
			if (adHocOrbit == null)
			{
				return;
			}
			if (!GameStateManager.IterateByClass<TISpaceFleetState>(false).Any<TISpaceFleetState>((TISpaceFleetState fleet) => fleet != this && (fleet.ref_orbit == adHocOrbit || (fleet.transferAssigned && (fleet.trajectory.originOrbit == adHocOrbit || fleet.trajectory.destination == adHocOrbit || fleet.trajectory.destinationOrbit == adHocOrbit)))))
			{
				TINaturalSpaceObjectState barycenter = adHocOrbit.barycenter;
				if (barycenter != null)
				{
					barycenter.orbits.Remove(adHocOrbit);
				}
				adHocOrbit.ArchiveState(true);
				GameStateManager.RemoveGameState<TIAdHocOrbitState>(adHocOrbit.ID, false);
			}
		}

		// Token: 0x0600446C RID: 17516 RVA: 0x001C02C4 File Offset: 0x001BE4C4
		public bool IsTransferingAtTime(TIDateTime time)
		{
			if (!this.transferAssigned)
			{
				return false;
			}
			if (this.trajectory.launchTime > time)
			{
				return false;
			}
			Trajectory trajectory = this.trajectory;
			for (int i = 0; i < 100; i++)
			{
				if (trajectory.arrivalTime > time)
				{
					return true;
				}
				if (trajectory.nextTrajectory == null)
				{
					return false;
				}
				trajectory = this.trajectory;
			}
			return false;
		}

		// Token: 0x0600446D RID: 17517 RVA: 0x001C0326 File Offset: 0x001BE526
		private void HandleSpaceObjectSelection(bool removeSelection = false)
		{
			if (SpaceObjectSelection.GetSelectedSpaceObject() == this)
			{
				if (removeSelection)
				{
					SpaceObjectSelection.SelectSpaceObject(null, false, false, false);
					return;
				}
				SpaceObjectSelection.SelectSpaceObject(base.gameObjectLink, false, false, true);
			}
		}

		// Token: 0x0600446E RID: 17518 RVA: 0x001C0350 File Offset: 0x001BE550
		public bool CanFulfillGoal(FactionGoal_Fleet goal, bool emergency = false)
		{
			GoalType goalType = goal.GetGoalType();
			switch (goalType)
			{
			case GoalType.ProspectSites:
				return this.HasSpecialModuleCapability(SpecialModuleRule.Prospector) && !this.InvasionFleet();
			case GoalType.FoundPlatform:
			case GoalType.FoundMaxStation:
				break;
			case GoalType.FoundBase:
				return !this.InvasionFleet() && (this.HasSpecialModuleCapability(SpecialModuleRule.FoundFissionOutpost) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundFusionOutpost) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundSolarOutpost));
			default:
				switch (goalType)
				{
				case GoalType.DefendWithFleet:
				case GoalType.AttackWithFleet:
					if ((emergency || !this.NonCombatFleet()) && !this.InvasionFleet())
					{
						return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.noseWeaponTemplates.Count > 0 || x.template.hullWeaponTemplates.Count<TIShipWeaponTemplate>() > 0);
					}
					return false;
				case GoalType.CaptureHab:
					return this.HasSpecialModuleCapability(SpecialModuleRule.Assault) && !this.InvasionFleet();
				case GoalType.TransportCouncilorsViaFleet:
					if (!this.IsAlien())
					{
						return true;
					}
					if (this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.CouncilorTransport))
					{
						return true;
					}
					if (this.InvasionFleet())
					{
						return false;
					}
					return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.TroopCarrier) || this.HasSpecialModuleCapability(SpecialModuleRule.Crashdown);
				case GoalType.InvadeEarth:
					return this.InvasionFleet();
				case GoalType.SurveilEarth:
					return this.HasSpecialModuleCapability(SpecialModuleRule.Surveillance);
				case GoalType.FoundStation:
					goto IL_008E;
				case GoalType.FoundSurveillanceStation:
				{
					FactionGoal_FoundSurveillanceStation factionGoal_FoundSurveillanceStation = goal as FactionGoal_FoundSurveillanceStation;
					return !this.InvasionFleet() && ((factionGoal_FoundSurveillanceStation.tier == 1 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillancePlatform)) || (factionGoal_FoundSurveillanceStation.tier == 2 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillanceOrbital)) || (factionGoal_FoundSurveillanceStation.tier == 3 && this.HasSpecialModuleCapability(SpecialModuleRule.FoundSurveillanceRing)));
				}
				}
				return true;
			}
			IL_008E:
			return !this.InvasionFleet() && (this.HasSpecialModuleCapability(SpecialModuleRule.FoundFusionPlatform) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundFissionPlatform) || this.HasSpecialModuleCapability(SpecialModuleRule.FoundSolarPlatform));
		}

		// Token: 0x0600446F RID: 17519 RVA: 0x001C0556 File Offset: 0x001BE756
		public bool CombatFleet()
		{
			return !this.NonCombatFleet();
		}

		// Token: 0x06004470 RID: 17520 RVA: 0x001C0561 File Offset: 0x001BE761
		public bool NonCombatFleet()
		{
			return this.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.role == ShipRole.CouncilorTransport || x.role == ShipRole.Explorer || x.role == ShipRole.InnerSystemColonyShip || x.role == ShipRole.OuterSystemColonyShip);
		}

		// Token: 0x06004471 RID: 17521 RVA: 0x001C058D File Offset: 0x001BE78D
		public bool InvasionFleet()
		{
			return this.HasSpecialModuleCapability(SpecialModuleRule.LandArmy);
		}

		// Token: 0x06004472 RID: 17522 RVA: 0x001C0597 File Offset: 0x001BE797
		public bool SurveillanceFleet()
		{
			return this.HasSpecialModuleCapability(SpecialModuleRule.Surveillance);
		}

		// Token: 0x06004473 RID: 17523 RVA: 0x001C05A4 File Offset: 0x001BE7A4
		public void RecordFailedAttackOnTarget(TIGameState target, float value = 1f, bool additive = false)
		{
			if (!TIGameState.Valid(target))
			{
				this.RemoveFailedAttackRecord(target);
				return;
			}
			if (!this.AI_FailedAttackEnemyStrength.ContainsKey(target))
			{
				this.AI_FailedAttackEnemyStrength.Add(target, 0f);
			}
			if (additive)
			{
				Dictionary<TIGameState, float> ai_FailedAttackEnemyStrength = this.AI_FailedAttackEnemyStrength;
				ai_FailedAttackEnemyStrength[target] += value;
				return;
			}
			this.AI_FailedAttackEnemyStrength[target] = value;
		}

		// Token: 0x06004474 RID: 17524 RVA: 0x001C060A File Offset: 0x001BE80A
		public void RemoveFailedAttackRecord(TIGameState target)
		{
			if (this.AI_FailedAttackEnemyStrength.ContainsKey(target))
			{
				this.AI_FailedAttackEnemyStrength.Remove(target);
			}
		}

		// Token: 0x06004475 RID: 17525 RVA: 0x001C0627 File Offset: 0x001BE827
		public float GetFailedAttacksOnTargetValue(TIGameState target)
		{
			if (this.AI_FailedAttackEnemyStrength.ContainsKey(target))
			{
				return this.AI_FailedAttackEnemyStrength[target];
			}
			return 0f;
		}

		// Token: 0x06004476 RID: 17526 RVA: 0x001C0649 File Offset: 0x001BE849
		public int GetFailedAttacksOnTargetCount(TIGameState target)
		{
			return this.GetFailedAttacksOnTargetValue(target).Round();
		}

		// Token: 0x06004477 RID: 17527 RVA: 0x001C0658 File Offset: 0x001BE858
		public void AddFleetLog(string label)
		{
			if (!TIGameState.Valid(this))
			{
				return;
			}
			List<TISpaceFleetState.FleetLog> logs = this.Logs;
			TISpaceFleetState.FleetLog fleetLog = default(TISpaceFleetState.FleetLog);
			fleetLog.Label = label;
			fleetLog.Date = TITimeState.Now();
			fleetLog.Location = (this.inTransfer ? null : this.location);
			FactionGoal_Fleet factionGoal_Fleet = this.AssignedGoal();
			fleetLog.GoalType = ((factionGoal_Fleet != null) ? factionGoal_Fleet.GetGoalType() : GoalType.None);
			FactionGoal_Fleet factionGoal_Fleet2 = this.AssignedGoal();
			fleetLog.GoalTarget = ((factionGoal_Fleet2 != null) ? factionGoal_Fleet2.target() : null);
			FactionGoal_Fleet factionGoal_Fleet3 = this.AssignedGoal();
			TIFactionState tifactionState;
			if (factionGoal_Fleet3 == null)
			{
				tifactionState = null;
			}
			else
			{
				TIGameState tigameState = factionGoal_Fleet3.target();
				tifactionState = ((tigameState != null) ? tigameState.ref_faction : null);
			}
			fleetLog.GoalTargetFaction = tifactionState;
			List<TISpaceShipState> ships = this.ships;
			fleetLog.ShipCount = ((ships != null) ? ships.Count : 0);
			fleetLog.FuelMass_dekatons = this.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.propellant_tons) * TemplateManager.global.spaceResourceToTons;
			logs.Add(fleetLog);
			int num = 50;
			while (this.Logs.Count > num)
			{
				this.Logs.RemoveAt(0);
			}
		}

		// Token: 0x06004478 RID: 17528 RVA: 0x001C077C File Offset: 0x001BE97C
		[return: TupleElementNames(new string[] { "Type", "Target", "TargetFaction" })]
		public List<ValueTuple<GoalType, TIGameState, TIFactionState>> GetRecentGoalInfo(float maximumAge_days)
		{
			TIDateTime now = TITimeState.Now();
			List<ValueTuple<GoalType, TIGameState, TIFactionState>> list = (from x in (from i in Enumerable.Range(1, this.Logs.Count - 1)
					where this.Logs[i].GoalType != this.Logs[i - 1].GoalType
					where this.Logs[i - 1].GoalType > GoalType.None
					select new ValueTuple<TISpaceFleetState.FleetLog, float>(this.Logs[i - 1], (float)(now - this.Logs[i].Date).TotalDays)).ToList<ValueTuple<TISpaceFleetState.FleetLog, float>>()
				where x.Item2 < maximumAge_days
				select new ValueTuple<GoalType, TIGameState, TIFactionState>(x.Item1.GoalType, x.Item1.GoalTarget, x.Item1.GoalTargetFaction)).ToList<ValueTuple<GoalType, TIGameState, TIFactionState>>();
			FactionGoal_Fleet factionGoal_Fleet = this.AssignedGoal();
			if (factionGoal_Fleet != null)
			{
				List<ValueTuple<GoalType, TIGameState, TIFactionState>> list2 = list;
				GoalType goalType = factionGoal_Fleet.GetGoalType();
				TIGameState tigameState = factionGoal_Fleet.target();
				TIGameState tigameState2 = factionGoal_Fleet.target();
				list2.Add(new ValueTuple<GoalType, TIGameState, TIFactionState>(goalType, tigameState, (tigameState2 != null) ? tigameState2.ref_faction : null));
			}
			return list;
		}

		// Token: 0x06004479 RID: 17529 RVA: 0x001C0868 File Offset: 0x001BEA68
		public static void ClearStaticData()
		{
			TISpaceFleetState.fleetsWaitingToInitiateCombat.Clear();
		}

		// Token: 0x0600448A RID: 17546 RVA: 0x001C0B58 File Offset: 0x001BED58
		[CompilerGenerated]
		private float <FireMission>g__Initiative|301_9(CombatWeaponCarrierState shooter)
		{
			float num = shooter.TargetingBonus(null, null);
			if (shooter.isShip())
			{
				num += shooter.ref_shipCarrier().ECMValue(this.bombardmentTarget.ref_faction, null);
			}
			else if (shooter.isHabModule())
			{
				num += shooter.ref_habModuleCarrier().ECMValue(base.faction);
			}
			return num + TIUtilities.RandomFloatValue();
		}

		// Token: 0x06004495 RID: 17557 RVA: 0x001C0C81 File Offset: 0x001BEE81
		[CompilerGenerated]
		private bool <ArriveFleet>g__IsRally|423_5(TISpaceFleetState otherFleet)
		{
			return otherFleet.AssignedGoal() != null && otherFleet.AssignedGoal().GetGoalType() == GoalType.JoinFleet && otherFleet.AssignedGoal().target() == this;
		}

		// Token: 0x04002848 RID: 10312
		[SerializeField]
		private Dictionary<TIFactionState, string> displayNameByFaction;

		// Token: 0x04002849 RID: 10313
		public const double EXISTING_TRAJECTORY_ACCELERATION_FORGIVENESS = 2.0;

		// Token: 0x0400284A RID: 10314
		public List<OperationData> currentOperations;

		// Token: 0x0400284C RID: 10316
		public TISpaceGameState dockedLocation;

		// Token: 0x04002851 RID: 10321
		[SerializeField]
		private FleetTrajectoryData _fleetTrajectoryData;

		// Token: 0x04002852 RID: 10322
		private Vector3 battleFormationOffset;

		// Token: 0x04002859 RID: 10329
		public TISpaceCombatState combatState;

		// Token: 0x0400285C RID: 10332
		public TISpaceFleetState parentFleet;

		// Token: 0x0400285D RID: 10333
		public bool alwaysShowOrbitTrailDuringTransfer;

		// Token: 0x0400285E RID: 10334
		public Dictionary<TIGameState, float> AI_FailedAttackEnemyStrength;

		// Token: 0x0400285F RID: 10335
		public HashSet<TIGameState> unreachableLocations = new HashSet<TIGameState>();

		// Token: 0x04002860 RID: 10336
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002861 RID: 10337
		public bool dummyFleet;

		// Token: 0x04002862 RID: 10338
		public float bombardmentAltitude_km;

		// Token: 0x04002863 RID: 10339
		public TIDateTime timeOfLastFireMission;

		// Token: 0x04002866 RID: 10342
		public TISpaceFleetState.DelayedTransferAbortNotification delayedTransferAbortNotification;

		// Token: 0x04002867 RID: 10343
		public TISpaceGameState campaignStartLocation;

		// Token: 0x04002868 RID: 10344
		private int _visibleOperationListCacheFrame = -1;

		// Token: 0x04002869 RID: 10345
		private List<IOperation> _cachedVisibleOperationList;

		// Token: 0x0400286A RID: 10346
		private int _availableOperationListCacheFrame = -1;

		// Token: 0x0400286B RID: 10347
		private List<IOperation> _cachedAvailableOperationList;

		// Token: 0x0400286C RID: 10348
		public TISpaceFleetState.EndBombardmentReason endBombardmentReason;

		// Token: 0x0400286D RID: 10349
		[SerializeField]
		private TISpaceFleetState.BombardmentBracketingStatus bombardmentTargetBracketStatus;

		// Token: 0x0400286E RID: 10350
		[SerializeField]
		private bool firstHitFromBombardmentRun;

		// Token: 0x0400286F RID: 10351
		private const float degreesStep = 1f;

		// Token: 0x04002870 RID: 10352
		public static readonly List<TISpaceFleetState.EndBombardmentReason> ReportableEndBombardmentReasons = new List<TISpaceFleetState.EndBombardmentReason>
		{
			TISpaceFleetState.EndBombardmentReason.TargetDestroyed,
			TISpaceFleetState.EndBombardmentReason.DurationExpired,
			TISpaceFleetState.EndBombardmentReason.BombardingFriendly,
			TISpaceFleetState.EndBombardmentReason.FleetUnableToContinue,
			TISpaceFleetState.EndBombardmentReason.TargetFleetTookOff
		};

		// Token: 0x04002871 RID: 10353
		public const float requiredPartFunctionforMarineAssault = 0.001f;

		// Token: 0x04002872 RID: 10354
		private const float AI_NeedRefuelBadly_LocalResupply_kps = 20f;

		// Token: 0x04002873 RID: 10355
		private const float AI_NeedRefuelBadly_AlienMin_kps = 150f;

		// Token: 0x04002874 RID: 10356
		private const float AI_NeedRefuelBadly_HumanMin_kps = 50f;

		// Token: 0x04002875 RID: 10357
		private const float AI_NeedRefuelBadly_DVPerAU_kps = 15f;

		// Token: 0x04002876 RID: 10358
		public Dictionary<TIOfficerState, OfficerCarrierState> officerTransferPlan;

		// Token: 0x04002877 RID: 10359
		[fsIgnore]
		public readonly Formation defaultAlienFormation = new Formation("Staggered", FormationFocus.Armored, FormationSpacing.Spread, FormationConcentration.Dispersed);

		// Token: 0x04002878 RID: 10360
		[fsIgnore]
		public readonly Formation defaultHumanFormation = new Formation("Convoy", FormationFocus.Heavy, FormationSpacing.Spread, FormationConcentration.Center);

		// Token: 0x04002879 RID: 10361
		[fsIgnore]
		public readonly Formation defaultAlienCombatFormation = new Formation("Line", FormationFocus.Armored, FormationSpacing.Tight, FormationConcentration.Center);

		// Token: 0x0400287A RID: 10362
		[fsIgnore]
		public readonly Formation defaultHumanCombatFormation = new Formation("Line", FormationFocus.Heavy, FormationSpacing.Tight, FormationConcentration.Dispersed);

		// Token: 0x0400287B RID: 10363
		[fsIgnore]
		public readonly Formation dockedFormation = new Formation("Line", FormationFocus.Heavy, FormationSpacing.Spread, FormationConcentration.Center);

		// Token: 0x0400287C RID: 10364
		[SerializeField]
		private List<TISpaceFleetState.WaitingToInitiateCombatData> waitingToInitiateCombatDatas;

		// Token: 0x0400287D RID: 10365
		public static HashSet<TISpaceFleetState> fleetsWaitingToInitiateCombat = new HashSet<TISpaceFleetState>();

		// Token: 0x0400287E RID: 10366
		public List<TISpaceFleetState.FleetLog> Logs = new List<TISpaceFleetState.FleetLog>();

		// Token: 0x02000F39 RID: 3897
		public class DelayedTransferAbortNotification
		{
			// Token: 0x04005CFE RID: 23806
			public int cause;

			// Token: 0x04005CFF RID: 23807
			public int outcome;

			// Token: 0x04005D00 RID: 23808
			public TISpaceFleetState doomedFleet;

			// Token: 0x04005D01 RID: 23809
			public TISpaceBodyState collisionTarget;
		}

		// Token: 0x02000F3A RID: 3898
		private enum BombardmentBracketingStatus
		{
			// Token: 0x04005D03 RID: 23811
			NotBracketed,
			// Token: 0x04005D04 RID: 23812
			Bracketing,
			// Token: 0x04005D05 RID: 23813
			Bracketed
		}

		// Token: 0x02000F3B RID: 3899
		public enum EndBombardmentReason
		{
			// Token: 0x04005D07 RID: 23815
			None,
			// Token: 0x04005D08 RID: 23816
			NotForDisplay,
			// Token: 0x04005D09 RID: 23817
			TargetDestroyed,
			// Token: 0x04005D0A RID: 23818
			DurationExpired,
			// Token: 0x04005D0B RID: 23819
			BombardingFriendly,
			// Token: 0x04005D0C RID: 23820
			FleetUnableToContinue,
			// Token: 0x04005D0D RID: 23821
			TargetFleetTookOff
		}

		// Token: 0x02000F3C RID: 3900
		public struct WaitingToInitiateCombatData
		{
			// Token: 0x04005D0E RID: 23822
			public TISpaceFleetState TargetFleet;

			// Token: 0x04005D0F RID: 23823
			public TIHabState TargetHab;
		}

		// Token: 0x02000F3D RID: 3901
		public struct FleetLog
		{
			// Token: 0x04005D10 RID: 23824
			public string Label;

			// Token: 0x04005D11 RID: 23825
			public TIDateTime Date;

			// Token: 0x04005D12 RID: 23826
			public TIGameState Location;

			// Token: 0x04005D13 RID: 23827
			public GoalType GoalType;

			// Token: 0x04005D14 RID: 23828
			public TIGameState GoalTarget;

			// Token: 0x04005D15 RID: 23829
			public TIFactionState GoalTargetFaction;

			// Token: 0x04005D16 RID: 23830
			public int ShipCount;

			// Token: 0x04005D17 RID: 23831
			public float FuelMass_dekatons;
		}
	}
}
