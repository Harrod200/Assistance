using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C1 RID: 1985
	public class TISpaceShipState : TIGameState, CombatTargetableState, CombatWeaponCarrierState, OfficerCarrierState
	{
		// Token: 0x17000D5B RID: 3419
		// (get) Token: 0x06004519 RID: 17689 RVA: 0x001C3626 File Offset: 0x001C1826
		// (set) Token: 0x0600451A RID: 17690 RVA: 0x001C362E File Offset: 0x001C182E
		public float cruiseAcceleration_mps2 { get; private set; }

		// Token: 0x17000D5C RID: 3420
		// (get) Token: 0x0600451B RID: 17691 RVA: 0x001C3637 File Offset: 0x001C1837
		// (set) Token: 0x0600451C RID: 17692 RVA: 0x001C363F File Offset: 0x001C183F
		public float combatAcceleration_mps2 { get; private set; }

		// Token: 0x17000D5D RID: 3421
		// (get) Token: 0x0600451D RID: 17693 RVA: 0x001C3648 File Offset: 0x001C1848
		// (set) Token: 0x0600451E RID: 17694 RVA: 0x001C3650 File Offset: 0x001C1850
		public float currentDeltaV_kps { get; private set; }

		// Token: 0x17000D5E RID: 3422
		// (get) Token: 0x0600451F RID: 17695 RVA: 0x001C3659 File Offset: 0x001C1859
		// (set) Token: 0x06004520 RID: 17696 RVA: 0x001C3661 File Offset: 0x001C1861
		public float currentMaxDeltaV_kps { get; private set; }

		// Token: 0x17000D5F RID: 3423
		// (get) Token: 0x06004521 RID: 17697 RVA: 0x001C366A File Offset: 0x001C186A
		// (set) Token: 0x06004522 RID: 17698 RVA: 0x001C3672 File Offset: 0x001C1872
		public float currentMass_kg { get; private set; }

		// Token: 0x17000D60 RID: 3424
		// (get) Token: 0x06004523 RID: 17699 RVA: 0x001C367B File Offset: 0x001C187B
		// (set) Token: 0x06004524 RID: 17700 RVA: 0x001C3683 File Offset: 0x001C1883
		public float angular_acceleration_rads2 { get; private set; }

		// Token: 0x17000D61 RID: 3425
		// (get) Token: 0x06004525 RID: 17701 RVA: 0x001C368C File Offset: 0x001C188C
		// (set) Token: 0x06004526 RID: 17702 RVA: 0x001C3694 File Offset: 0x001C1894
		public float max_angular_velocity_rad_s { get; private set; }

		// Token: 0x17000D62 RID: 3426
		// (get) Token: 0x06004527 RID: 17703 RVA: 0x001C369D File Offset: 0x001C189D
		public float manueverRating
		{
			get
			{
				return this.angularAcceleration_degs2 * 60f * this.combatAcceleration_gs;
			}
		}

		// Token: 0x17000D63 RID: 3427
		// (get) Token: 0x06004528 RID: 17704 RVA: 0x001C36B2 File Offset: 0x001C18B2
		// (set) Token: 0x06004529 RID: 17705 RVA: 0x001C36BA File Offset: 0x001C18BA
		public int missionControlConsumption { get; private set; }

		// Token: 0x17000D64 RID: 3428
		// (get) Token: 0x0600452A RID: 17706 RVA: 0x001C36C3 File Offset: 0x001C18C3
		public float pursuitAcceleration_mps2
		{
			get
			{
				return this.cruiseAcceleration_mps2;
			}
		}

		// Token: 0x17000D65 RID: 3429
		// (get) Token: 0x0600452B RID: 17707 RVA: 0x001C36CB File Offset: 0x001C18CB
		public float pursuitAcceleration_gs
		{
			get
			{
				return this.pursuitAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x0600452C RID: 17708 RVA: 0x001C36D9 File Offset: 0x001C18D9
		// (set) Token: 0x0600452D RID: 17709 RVA: 0x001C36E1 File Offset: 0x001C18E1
		public bool radiatorsExtending { get; private set; }

		// Token: 0x17000D67 RID: 3431
		// (get) Token: 0x0600452E RID: 17710 RVA: 0x001C36EA File Offset: 0x001C18EA
		// (set) Token: 0x0600452F RID: 17711 RVA: 0x001C36F2 File Offset: 0x001C18F2
		public bool radiatorsRetracting { get; private set; }

		// Token: 0x17000D68 RID: 3432
		// (get) Token: 0x06004530 RID: 17712 RVA: 0x001C36FB File Offset: 0x001C18FB
		// (set) Token: 0x06004531 RID: 17713 RVA: 0x001C3703 File Offset: 0x001C1903
		public bool radiatorsExtended { get; private set; }

		// Token: 0x17000D69 RID: 3433
		// (get) Token: 0x06004532 RID: 17714 RVA: 0x001C370C File Offset: 0x001C190C
		// (set) Token: 0x06004533 RID: 17715 RVA: 0x001C3714 File Offset: 0x001C1914
		public float accumulatedHeat_GJ { get; private set; }

		// Token: 0x17000D6A RID: 3434
		// (get) Token: 0x06004534 RID: 17716 RVA: 0x001C371D File Offset: 0x001C191D
		// (set) Token: 0x06004535 RID: 17717 RVA: 0x001C3725 File Offset: 0x001C1925
		public float currentHeatSinkCapacity_GJ { get; private set; }

		// Token: 0x17000D6B RID: 3435
		// (get) Token: 0x06004536 RID: 17718 RVA: 0x001C372E File Offset: 0x001C192E
		// (set) Token: 0x06004537 RID: 17719 RVA: 0x001C3736 File Offset: 0x001C1936
		public bool thrustersActive { get; private set; }

		// Token: 0x17000D6C RID: 3436
		// (get) Token: 0x06004538 RID: 17720 RVA: 0x001C373F File Offset: 0x001C193F
		// (set) Token: 0x06004539 RID: 17721 RVA: 0x001C3747 File Offset: 0x001C1947
		public bool canSuicide { get; private set; }

		// Token: 0x17000D6D RID: 3437
		// (get) Token: 0x0600453A RID: 17722 RVA: 0x001C3750 File Offset: 0x001C1950
		// (set) Token: 0x0600453B RID: 17723 RVA: 0x001C3758 File Offset: 0x001C1958
		public bool combatAIControl { get; private set; }

		// Token: 0x17000D6E RID: 3438
		// (get) Token: 0x0600453C RID: 17724 RVA: 0x001C3761 File Offset: 0x001C1961
		// (set) Token: 0x0600453D RID: 17725 RVA: 0x001C3769 File Offset: 0x001C1969
		public bool disengageFromCombat { get; private set; }

		// Token: 0x17000D6F RID: 3439
		// (get) Token: 0x0600453E RID: 17726 RVA: 0x001C3772 File Offset: 0x001C1972
		// (set) Token: 0x0600453F RID: 17727 RVA: 0x001C377A File Offset: 0x001C197A
		public bool hasDisengaged { get; private set; }

		// Token: 0x17000D70 RID: 3440
		// (get) Token: 0x06004540 RID: 17728 RVA: 0x001C3783 File Offset: 0x001C1983
		// (set) Token: 0x06004541 RID: 17729 RVA: 0x001C378B File Offset: 0x001C198B
		public bool isDamageControlSuspended { get; private set; }

		// Token: 0x17000D71 RID: 3441
		// (get) Token: 0x06004542 RID: 17730 RVA: 0x001C3794 File Offset: 0x001C1994
		// (set) Token: 0x06004543 RID: 17731 RVA: 0x001C379C File Offset: 0x001C199C
		public CombatTargetableState combatPrimaryTarget { get; private set; }

		// Token: 0x17000D72 RID: 3442
		// (get) Token: 0x06004544 RID: 17732 RVA: 0x001C37A5 File Offset: 0x001C19A5
		// (set) Token: 0x06004545 RID: 17733 RVA: 0x001C37AD File Offset: 0x001C19AD
		public CombatTargetableState combatManeuverTarget { get; private set; }

		// Token: 0x17000D73 RID: 3443
		// (get) Token: 0x06004546 RID: 17734 RVA: 0x001C37B6 File Offset: 0x001C19B6
		public Propellant propellant
		{
			get
			{
				return this.drive.propellant;
			}
		}

		// Token: 0x17000D74 RID: 3444
		// (get) Token: 0x06004547 RID: 17735 RVA: 0x001C37C3 File Offset: 0x001C19C3
		// (set) Token: 0x06004548 RID: 17736 RVA: 0x001C37CB File Offset: 0x001C19CB
		public Dictionary<ModuleDataEntry, int> ammo { get; private set; }

		// Token: 0x17000D75 RID: 3445
		// (get) Token: 0x06004549 RID: 17737 RVA: 0x001C37D4 File Offset: 0x001C19D4
		// (set) Token: 0x0600454A RID: 17738 RVA: 0x001C37DC File Offset: 0x001C19DC
		public TISpaceFleetState fleet { get; set; }

		// Token: 0x17000D76 RID: 3446
		// (get) Token: 0x0600454B RID: 17739 RVA: 0x001C37E5 File Offset: 0x001C19E5
		// (set) Token: 0x0600454C RID: 17740 RVA: 0x001C37ED File Offset: 0x001C19ED
		public Vector3d fleetFormationOffset { get; private set; }

		// Token: 0x17000D77 RID: 3447
		// (get) Token: 0x0600454D RID: 17741 RVA: 0x001C37F6 File Offset: 0x001C19F6
		// (set) Token: 0x0600454E RID: 17742 RVA: 0x001C37FE File Offset: 0x001C19FE
		public bool propulsionValuesDataDirty { get; private set; }

		// Token: 0x17000D78 RID: 3448
		// (get) Token: 0x0600454F RID: 17743 RVA: 0x001C3807 File Offset: 0x001C1A07
		// (set) Token: 0x06004550 RID: 17744 RVA: 0x001C380F File Offset: 0x001C1A0F
		public TIDateTime launchDate { get; private set; }

		// Token: 0x17000D79 RID: 3449
		// (get) Token: 0x06004551 RID: 17745 RVA: 0x001C3818 File Offset: 0x001C1A18
		// (set) Token: 0x06004552 RID: 17746 RVA: 0x001C3820 File Offset: 0x001C1A20
		public TIDateTime lastRefitDate { get; private set; }

		// Token: 0x17000D7A RID: 3450
		// (get) Token: 0x06004553 RID: 17747 RVA: 0x001C3829 File Offset: 0x001C1A29
		// (set) Token: 0x06004554 RID: 17748 RVA: 0x001C3831 File Offset: 0x001C1A31
		public TIRegionState homeRegion { get; private set; }

		// Token: 0x17000D7B RID: 3451
		// (get) Token: 0x06004555 RID: 17749 RVA: 0x001C383A File Offset: 0x001C1A3A
		public ShipManeuverSequence CurrentManeuverSequence
		{
			get
			{
				return this.currentManeuverSequence;
			}
		}

		// Token: 0x17000D7C RID: 3452
		// (get) Token: 0x06004556 RID: 17750 RVA: 0x001C3842 File Offset: 0x001C1A42
		public override bool isSpaceShipState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D7D RID: 3453
		// (get) Token: 0x06004557 RID: 17751 RVA: 0x001C3845 File Offset: 0x001C1A45
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x06004558 RID: 17752 RVA: 0x001C3848 File Offset: 0x001C1A48
		public bool isShip()
		{
			return true;
		}

		// Token: 0x06004559 RID: 17753 RVA: 0x001C384B File Offset: 0x001C1A4B
		public bool isHabModule()
		{
			return false;
		}

		// Token: 0x17000D7E RID: 3454
		// (get) Token: 0x0600455A RID: 17754 RVA: 0x001C384E File Offset: 0x001C1A4E
		public override TIFactionState ref_faction
		{
			get
			{
				return this.faction;
			}
		}

		// Token: 0x17000D7F RID: 3455
		// (get) Token: 0x0600455B RID: 17755 RVA: 0x001C3856 File Offset: 0x001C1A56
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				return this.fleet;
			}
		}

		// Token: 0x17000D80 RID: 3456
		// (get) Token: 0x0600455C RID: 17756 RVA: 0x001C385E File Offset: 0x001C1A5E
		public override TIOrbitState ref_orbit
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.ref_orbit;
			}
		}

		// Token: 0x17000D81 RID: 3457
		// (get) Token: 0x0600455D RID: 17757 RVA: 0x001C3871 File Offset: 0x001C1A71
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.ref_naturalSpaceObject;
			}
		}

		// Token: 0x17000D82 RID: 3458
		// (get) Token: 0x0600455E RID: 17758 RVA: 0x001C3884 File Offset: 0x001C1A84
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.ref_spaceBody;
			}
		}

		// Token: 0x17000D83 RID: 3459
		// (get) Token: 0x0600455F RID: 17759 RVA: 0x001C3897 File Offset: 0x001C1A97
		public override TIHabState ref_hab
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.ref_hab;
			}
		}

		// Token: 0x17000D84 RID: 3460
		// (get) Token: 0x06004560 RID: 17760 RVA: 0x001C38AA File Offset: 0x001C1AAA
		public override TIHabSiteState ref_habSite
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.ref_habSite;
			}
		}

		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06004561 RID: 17761 RVA: 0x001C38BD File Offset: 0x001C1ABD
		public override TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return this.fleet;
			}
		}

		// Token: 0x17000D86 RID: 3462
		// (get) Token: 0x06004562 RID: 17762 RVA: 0x001C38C5 File Offset: 0x001C1AC5
		public override TISpaceShipState ref_ship
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000D87 RID: 3463
		// (get) Token: 0x06004563 RID: 17763 RVA: 0x001C38C8 File Offset: 0x001C1AC8
		public override TIRegionState ref_region
		{
			get
			{
				return this.homeRegion;
			}
		}

		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x06004564 RID: 17764 RVA: 0x001C38D0 File Offset: 0x001C1AD0
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x06004565 RID: 17765 RVA: 0x001C38D3 File Offset: 0x001C1AD3
		public override bool inSpace
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06004566 RID: 17766 RVA: 0x001C38D6 File Offset: 0x001C1AD6
		public TIGameState GetTargetableState()
		{
			return this;
		}

		// Token: 0x06004567 RID: 17767 RVA: 0x001C38D9 File Offset: 0x001C1AD9
		public TISpaceShipState ref_shipCarrier()
		{
			return this;
		}

		// Token: 0x06004568 RID: 17768 RVA: 0x001C38DC File Offset: 0x001C1ADC
		public TIHabModuleState ref_habModuleCarrier()
		{
			return null;
		}

		// Token: 0x06004569 RID: 17769 RVA: 0x001C38DF File Offset: 0x001C1ADF
		public bool IsAlien()
		{
			return this.ref_faction.IsAlienFaction;
		}

		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x0600456A RID: 17770 RVA: 0x001C38EC File Offset: 0x001C1AEC
		public TISpaceShipTemplate template
		{
			get
			{
				return this.GetMyTemplate<TISpaceShipTemplate>();
			}
		}

		// Token: 0x17000D8B RID: 3467
		// (get) Token: 0x0600456B RID: 17771 RVA: 0x001C38F4 File Offset: 0x001C1AF4
		public TIShipHullTemplate hull
		{
			get
			{
				return this.template.hullTemplate;
			}
		}

		// Token: 0x17000D8C RID: 3468
		// (get) Token: 0x0600456C RID: 17772 RVA: 0x001C3901 File Offset: 0x001C1B01
		public TIFactionState faction
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					return null;
				}
				return fleet.faction;
			}
		}

		// Token: 0x17000D8D RID: 3469
		// (get) Token: 0x0600456D RID: 17773 RVA: 0x001C3914 File Offset: 0x001C1B14
		public List<TIShipModuleTemplate> utilitySlotModules
		{
			get
			{
				return this.template.utilitySlotModuleTemplates.ToList<TIShipModuleTemplate>();
			}
		}

		// Token: 0x17000D8E RID: 3470
		// (get) Token: 0x0600456E RID: 17774 RVA: 0x001C3926 File Offset: 0x001C1B26
		public List<TIShipPartTemplate> partTemplates
		{
			get
			{
				return this.template.partTemplates;
			}
		}

		// Token: 0x17000D8F RID: 3471
		// (get) Token: 0x0600456F RID: 17775 RVA: 0x001C3933 File Offset: 0x001C1B33
		public TIRadiatorTemplate radiators
		{
			get
			{
				return this.template.radiatorTemplate;
			}
		}

		// Token: 0x17000D90 RID: 3472
		// (get) Token: 0x06004570 RID: 17776 RVA: 0x001C3940 File Offset: 0x001C1B40
		public TIDriveTemplate drive
		{
			get
			{
				return this.template.driveTemplate;
			}
		}

		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x06004571 RID: 17777 RVA: 0x001C394D File Offset: 0x001C1B4D
		public TIPowerPlantTemplate powerPlant
		{
			get
			{
				return this.template.powerPlantTemplate;
			}
		}

		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x06004572 RID: 17778 RVA: 0x001C395A File Offset: 0x001C1B5A
		public ModuleDataEntry driveModule
		{
			get
			{
				if (this._driveModule == null)
				{
					this._driveModule = new ModuleDataEntry(this.drive, this.hull.GetUniqueSlotIndex(ShipModuleSlotType.Drive));
				}
				return this._driveModule;
			}
		}

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x06004573 RID: 17779 RVA: 0x001C3987 File Offset: 0x001C1B87
		public ModuleDataEntry powerPlantModule
		{
			get
			{
				if (this._powerPlantModule == null)
				{
					this._powerPlantModule = new ModuleDataEntry(this.powerPlant, this.hull.GetUniqueSlotIndex(ShipModuleSlotType.PowerPlant));
				}
				return this._powerPlantModule;
			}
		}

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x06004574 RID: 17780 RVA: 0x001C39B4 File Offset: 0x001C1BB4
		public ModuleDataEntry radiatorModule
		{
			get
			{
				if (this._radiatorModule == null)
				{
					this._radiatorModule = new ModuleDataEntry(this.radiators, this.hull.GetUniqueSlotIndex(ShipModuleSlotType.Radiator));
				}
				return this._radiatorModule;
			}
		}

		// Token: 0x17000D95 RID: 3477
		// (get) Token: 0x06004575 RID: 17781 RVA: 0x001C39E1 File Offset: 0x001C1BE1
		public TIShipArmorTemplate noseArmorTemplate
		{
			get
			{
				return this.template.noseArmorTemplate;
			}
		}

		// Token: 0x17000D96 RID: 3478
		// (get) Token: 0x06004576 RID: 17782 RVA: 0x001C39EE File Offset: 0x001C1BEE
		public TIShipArmorTemplate lateralArmorTemplate
		{
			get
			{
				return this.template.lateralArmorTemplate;
			}
		}

		// Token: 0x17000D97 RID: 3479
		// (get) Token: 0x06004577 RID: 17783 RVA: 0x001C39FB File Offset: 0x001C1BFB
		public TIShipArmorTemplate tailArmorTemplate
		{
			get
			{
				return this.template.tailArmorTemplate;
			}
		}

		// Token: 0x17000D98 RID: 3480
		// (get) Token: 0x06004578 RID: 17784 RVA: 0x001C3A08 File Offset: 0x001C1C08
		public float noseArmorThickness_m
		{
			get
			{
				return this.template.noseArmorThickness;
			}
		}

		// Token: 0x17000D99 RID: 3481
		// (get) Token: 0x06004579 RID: 17785 RVA: 0x001C3A15 File Offset: 0x001C1C15
		public float lateralArmorThickness_m
		{
			get
			{
				return this.template.lateralArmorThickness_m;
			}
		}

		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x0600457A RID: 17786 RVA: 0x001C3A22 File Offset: 0x001C1C22
		public float tailArmorThickness_m
		{
			get
			{
				return this.template.tailArmorThickness;
			}
		}

		// Token: 0x17000D9B RID: 3483
		// (get) Token: 0x0600457B RID: 17787 RVA: 0x001C3A30 File Offset: 0x001C1C30
		public List<TICouncilorState> councilorPassengers
		{
			get
			{
				return (from x in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors)
					where x.location == this
					select x).ToList<TICouncilorState>();
			}
		}

		// Token: 0x17000D9C RID: 3484
		// (get) Token: 0x0600457C RID: 17788 RVA: 0x001C3A7C File Offset: 0x001C1C7C
		public List<TICouncilorState> alienCouncilorPassengers
		{
			get
			{
				return this.councilorPassengers.Where<TICouncilorState>((TICouncilorState councilor) => councilor.isAlien).ToList<TICouncilorState>();
			}
		}

		// Token: 0x17000D9D RID: 3485
		// (get) Token: 0x0600457D RID: 17789 RVA: 0x001C3AAD File Offset: 0x001C1CAD
		public bool crashdownEligible
		{
			get
			{
				return this.SpecialModuleRules(false).Contains(SpecialModuleRule.Crashdown) && this.alienCouncilorPassengers.Count > 0;
			}
		}

		// Token: 0x17000D9E RID: 3486
		// (get) Token: 0x0600457E RID: 17790 RVA: 0x001C3ACF File Offset: 0x001C1CCF
		public bool landArmyEligible
		{
			get
			{
				return this.SpecialModuleRules(false).Contains(SpecialModuleRule.LandArmy);
			}
		}

		// Token: 0x17000D9F RID: 3487
		// (get) Token: 0x0600457F RID: 17791 RVA: 0x001C3ADF File Offset: 0x001C1CDF
		public float maxThrust_combatExhaustVelocity_kps
		{
			get
			{
				return this.drive.EV_kps / this.drive.thrustCap;
			}
		}

		// Token: 0x17000DA0 RID: 3488
		// (get) Token: 0x06004580 RID: 17792 RVA: 0x001C3AF8 File Offset: 0x001C1CF8
		public float cruiseAcceleration_gs
		{
			get
			{
				return this.cruiseAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000DA1 RID: 3489
		// (get) Token: 0x06004581 RID: 17793 RVA: 0x001C3B06 File Offset: 0x001C1D06
		public float cruiseAcceleration_kps2
		{
			get
			{
				return this.cruiseAcceleration_mps2 / 1000f;
			}
		}

		// Token: 0x17000DA2 RID: 3490
		// (get) Token: 0x06004582 RID: 17794 RVA: 0x001C3B14 File Offset: 0x001C1D14
		public float combatAcceleration_kps2
		{
			get
			{
				return this.combatAcceleration_mps2 / 1000f;
			}
		}

		// Token: 0x17000DA3 RID: 3491
		// (get) Token: 0x06004583 RID: 17795 RVA: 0x001C3B22 File Offset: 0x001C1D22
		public float combatAcceleration_gs
		{
			get
			{
				return this.combatAcceleration_mps2 / 9.80665f;
			}
		}

		// Token: 0x17000DA4 RID: 3492
		// (get) Token: 0x06004584 RID: 17796 RVA: 0x001C3B30 File Offset: 0x001C1D30
		public bool isAlien
		{
			get
			{
				return this.hull.alien;
			}
		}

		// Token: 0x17000DA5 RID: 3493
		// (get) Token: 0x06004585 RID: 17797 RVA: 0x001C3B3D File Offset: 0x001C1D3D
		public double dryMass_kg
		{
			get
			{
				return (double)this.template.dryMass_kg;
			}
		}

		// Token: 0x17000DA6 RID: 3494
		// (get) Token: 0x06004586 RID: 17798 RVA: 0x001C3B4B File Offset: 0x001C1D4B
		public double dryMass_tons
		{
			get
			{
				return (double)this.template.dryMass_tons(false);
			}
		}

		// Token: 0x17000DA7 RID: 3495
		// (get) Token: 0x06004587 RID: 17799 RVA: 0x001C3B5A File Offset: 0x001C1D5A
		public double wetMass_kg
		{
			get
			{
				return (double)this.template.wetMass_kg;
			}
		}

		// Token: 0x17000DA8 RID: 3496
		// (get) Token: 0x06004588 RID: 17800 RVA: 0x001C3B68 File Offset: 0x001C1D68
		public double wetMass_tons
		{
			get
			{
				return (double)this.template.wetMass_tons;
			}
		}

		// Token: 0x17000DA9 RID: 3497
		// (get) Token: 0x06004589 RID: 17801 RVA: 0x001C3B76 File Offset: 0x001C1D76
		public double currentMass_tons
		{
			get
			{
				return (double)(this.currentMass_kg / 1000f);
			}
		}

		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x0600458A RID: 17802 RVA: 0x001C3B85 File Offset: 0x001C1D85
		public List<TIShipWeaponTemplate> noseWeaponTemplates
		{
			get
			{
				return this.template.noseWeaponTemplates.ToList<TIShipWeaponTemplate>();
			}
		}

		// Token: 0x17000DAB RID: 3499
		// (get) Token: 0x0600458B RID: 17803 RVA: 0x001C3B97 File Offset: 0x001C1D97
		public List<TIShipWeaponTemplate> hullWeaponTemplates
		{
			get
			{
				return this.template.hullWeaponTemplates.ToList<TIShipWeaponTemplate>();
			}
		}

		// Token: 0x17000DAC RID: 3500
		// (get) Token: 0x0600458C RID: 17804 RVA: 0x001C3BA9 File Offset: 0x001C1DA9
		public List<TIShipWeaponTemplate> allWeaponTemplates
		{
			get
			{
				return this.template.allWeaponTemplates;
			}
		}

		// Token: 0x17000DAD RID: 3501
		// (get) Token: 0x0600458D RID: 17805 RVA: 0x001C3BB6 File Offset: 0x001C1DB6
		public List<TIShipModuleTemplate> utilityModuleTemplates
		{
			get
			{
				return this.template.utilitySlotModuleTemplates.ToList<TIShipModuleTemplate>();
			}
		}

		// Token: 0x17000DAE RID: 3502
		// (get) Token: 0x0600458E RID: 17806 RVA: 0x001C3BC8 File Offset: 0x001C1DC8
		public float noseArmorValue
		{
			get
			{
				return (float)this.armor[ArmorFacing.Nose].armorValue;
			}
		}

		// Token: 0x17000DAF RID: 3503
		// (get) Token: 0x0600458F RID: 17807 RVA: 0x001C3BDC File Offset: 0x001C1DDC
		public float leftArmorValue
		{
			get
			{
				return (float)this.armor[ArmorFacing.Left].armorValue;
			}
		}

		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x06004590 RID: 17808 RVA: 0x001C3BF0 File Offset: 0x001C1DF0
		public float rightArmorValue
		{
			get
			{
				return (float)this.armor[ArmorFacing.Right].armorValue;
			}
		}

		// Token: 0x17000DB1 RID: 3505
		// (get) Token: 0x06004591 RID: 17809 RVA: 0x001C3C04 File Offset: 0x001C1E04
		public float tailArmorValue
		{
			get
			{
				return (float)this.armor[ArmorFacing.Tail].armorValue;
			}
		}

		// Token: 0x17000DB2 RID: 3506
		// (get) Token: 0x06004592 RID: 17810 RVA: 0x001C3C18 File Offset: 0x001C1E18
		public float sumArmorValue
		{
			get
			{
				return this.noseArmorValue + this.leftArmorValue + this.rightArmorValue + this.tailArmorValue;
			}
		}

		// Token: 0x17000DB3 RID: 3507
		// (get) Token: 0x06004593 RID: 17811 RVA: 0x001C3C35 File Offset: 0x001C1E35
		public ShipRole role
		{
			get
			{
				return this.template.role;
			}
		}

		// Token: 0x17000DB4 RID: 3508
		// (get) Token: 0x06004594 RID: 17812 RVA: 0x001C3C42 File Offset: 0x001C1E42
		public bool combatant
		{
			get
			{
				return this.template.combatant;
			}
		}

		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x06004595 RID: 17813 RVA: 0x001C3C4F File Offset: 0x001C1E4F
		public bool nonCombatant
		{
			get
			{
				return this.template.nonCombatant;
			}
		}

		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x06004596 RID: 17814 RVA: 0x001C3C5C File Offset: 0x001C1E5C
		public float spaceScienceResearchBonus
		{
			get
			{
				if (!this.fleet.dockedAtHab && !this.fleet.inEarthSystem)
				{
					return this.GetFunctionalUtilitySlotModuleTemplates(1f).Sum<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.shipSpaceScienceModuleResearchBonus);
				}
				return 0f;
			}
		}

		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x06004597 RID: 17815 RVA: 0x001C3CB8 File Offset: 0x001C1EB8
		public bool damaged
		{
			get
			{
				if (!this.damagedParts.Any<DamagedShipPartData>((DamagedShipPartData x) => x.damage > 0f))
				{
					if (!this.damagedSystems.Any<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => x.Value > 0f))
					{
						return this.armor.Values.Any<TISpaceShipState.ArmorData>((TISpaceShipState.ArmorData x) => x.damaged);
					}
				}
				return true;
			}
		}

		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x06004598 RID: 17816 RVA: 0x001C3D50 File Offset: 0x001C1F50
		public bool internalDamage
		{
			get
			{
				if (!this.damagedParts.Any<DamagedShipPartData>((DamagedShipPartData x) => x.damage > 0f))
				{
					return this.damagedSystems.Any<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => x.Value > 0f);
				}
				return true;
			}
		}

		// Token: 0x17000DB9 RID: 3513
		// (get) Token: 0x06004599 RID: 17817 RVA: 0x001C3DB8 File Offset: 0x001C1FB8
		public bool badlyDamaged
		{
			get
			{
				if (!this.damagedParts.Any<DamagedShipPartData>((DamagedShipPartData x) => (double)x.damage > 0.1))
				{
					return this.damagedSystems.Any<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => (double)x.Value > 0.1);
				}
				return true;
			}
		}

		// Token: 0x17000DBA RID: 3514
		// (get) Token: 0x0600459A RID: 17818 RVA: 0x001C3E20 File Offset: 0x001C2020
		public bool seriouslyDamaged
		{
			get
			{
				if (!this.damagedParts.Any<DamagedShipPartData>((DamagedShipPartData x) => (double)x.damage > 0.5))
				{
					return this.damagedSystems.Any<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => (double)x.Value > 0.5);
				}
				return true;
			}
		}

		// Token: 0x17000DBB RID: 3515
		// (get) Token: 0x0600459B RID: 17819 RVA: 0x001C3E85 File Offset: 0x001C2085
		public bool isCapableOfTransfering
		{
			get
			{
				return this.CanRotateAndRoll() && this.currentDeltaV_kps > 0f && this.cruiseAcceleration_gs > 0f;
			}
		}

		// Token: 0x17000DBC RID: 3516
		// (get) Token: 0x0600459C RID: 17820 RVA: 0x001C3EAB File Offset: 0x001C20AB
		public Vector3d globalPosition
		{
			get
			{
				return this.fleet.GetGlobalPosition() + this.currentFleetOffset;
			}
		}

		// Token: 0x0600459D RID: 17821 RVA: 0x001C3EC3 File Offset: 0x001C20C3
		public Vector3d globalPositionAtTime(TIDateTime time)
		{
			return this.fleet.GetGlobalPositionAtTime(time) + this.currentFleetOffset;
		}

		// Token: 0x17000DBD RID: 3517
		// (get) Token: 0x0600459E RID: 17822 RVA: 0x001C3EDC File Offset: 0x001C20DC
		public Vector3d desiredGlobalPosition
		{
			get
			{
				return this.fleet.GetGlobalPosition() + this.fleetFormationOffset;
			}
		}

		// Token: 0x0600459F RID: 17823 RVA: 0x001C3EF4 File Offset: 0x001C20F4
		public float SpaceCombatValue(bool forceIt = false, float prospectiveDVChange_kps = 0f)
		{
			if (this.spaceCombatValueDataDirty || forceIt || prospectiveDVChange_kps > 0f)
			{
				float num = this.template.TemplateSpaceCombatValue(false, -1f, 1f, false);
				int count = this.AllWeaponModuleData().Count;
				if (count > 0)
				{
					float num2 = (float)this.AllWeaponModuleData().Count<ModuleDataEntry>((ModuleDataEntry x) => this.WeaponIsOperable(x)) / (float)count;
					if (num2 < 1f)
					{
						num *= num2;
					}
					else if (this.AI_NeedsRearmBadly())
					{
						num *= 0.85f;
					}
				}
				float num3 = 0.9f + Mathf.Clamp01((this.currentDeltaV_kps + prospectiveDVChange_kps) / Mathf.Clamp(this.currentMaxDeltaV_kps, 1f, 40f)) * 0.1f;
				num *= num3;
				int num4 = this.damagedParts.Count<DamagedShipPartData>((DamagedShipPartData x) => x.damage > 0f) + this.damagedSystems.Count<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => x.Value > 0f);
				num *= 0.9f + (float)(25 - Mathf.Min(25, num4)) / 25f * 0.1f;
				this.spaceCombatValueDataDirty = false;
				this._cachedSpaceCombatValue = num;
			}
			return this._cachedSpaceCombatValue;
		}

		// Token: 0x17000DBE RID: 3518
		// (get) Token: 0x060045A0 RID: 17824 RVA: 0x001C403C File Offset: 0x001C223C
		public float combatRange_km
		{
			get
			{
				IEnumerable<ModuleDataEntry> enumerable = this.template.allWeapons.Where<ModuleDataEntry>((ModuleDataEntry x) => this.WeaponIsOperable(x));
				if (enumerable.Count<ModuleDataEntry>() > 0)
				{
					return enumerable.Select<ModuleDataEntry, TIShipWeaponTemplate>((ModuleDataEntry x) => x.moduleTemplate.ref_weapon).Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.targetingRange_km);
				}
				return 1000f;
			}
		}

		// Token: 0x060045A1 RID: 17825 RVA: 0x001C40C0 File Offset: 0x001C22C0
		private void CacheEffectiveBeamRanges()
		{
			if (this.effectiveBeamWeaponRange_km == null)
			{
				this.effectiveBeamWeaponRange_km = new Dictionary<float, Dictionary<string, float>>
				{
					{
						TemplateManager.global.DP_DestroyMissile,
						new Dictionary<string, float>()
					},
					{
						TemplateManager.global.DP_FireAtMagRound,
						new Dictionary<string, float>()
					},
					{
						1f,
						new Dictionary<string, float>()
					}
				};
			}
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.allWeaponTemplates)
			{
				TIBeamWeaponTemplate ref_beamWeapon = tishipWeaponTemplate.ref_beamWeapon;
				if (ref_beamWeapon != null)
				{
					if (!this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_DestroyMissile].ContainsKey(ref_beamWeapon.dataName))
					{
						this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_DestroyMissile].Add(ref_beamWeapon.dataName, ref_beamWeapon.RangeToDoDamage_km(TemplateManager.global.DP_DestroyMissile, this));
					}
					else
					{
						this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_DestroyMissile][ref_beamWeapon.dataName] = ref_beamWeapon.RangeToDoDamage_km(TemplateManager.global.DP_DestroyMissile, this);
					}
					if (!this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_FireAtMagRound].ContainsKey(ref_beamWeapon.dataName))
					{
						this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_FireAtMagRound].Add(ref_beamWeapon.dataName, ref_beamWeapon.RangeToDoDamage_km(TemplateManager.global.DP_FireAtMagRound, this));
					}
					else
					{
						this.effectiveBeamWeaponRange_km[TemplateManager.global.DP_FireAtMagRound][ref_beamWeapon.dataName] = ref_beamWeapon.RangeToDoDamage_km(TemplateManager.global.DP_FireAtMagRound, this);
					}
					if (!this.effectiveBeamWeaponRange_km[1f].ContainsKey(ref_beamWeapon.dataName))
					{
						this.effectiveBeamWeaponRange_km[1f].Add(ref_beamWeapon.dataName, ref_beamWeapon.RangeToDoDamage_km(1f, this));
					}
					else
					{
						this.effectiveBeamWeaponRange_km[1f][ref_beamWeapon.dataName] = ref_beamWeapon.RangeToDoDamage_km(1f, this);
					}
				}
			}
		}

		// Token: 0x060045A2 RID: 17826 RVA: 0x001C42F0 File Offset: 0x001C24F0
		public string ThrusterSFXString()
		{
			string text = (this.ref_fleet.IsAlien() ? "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Alien_Vector_Thruster" : "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Human_Vector_Thruster");
			if (!this.ref_fleet.IsAlien())
			{
				switch (this.drive.driveClassification)
				{
				case DriveClassification.Chemical:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Chemical_Rocket_Thruster";
					break;
				case DriveClassification.Electrothermal:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Electrothermal_Rocket_Thruster";
					break;
				case DriveClassification.Electromagnetic:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Electromagnetic_Rocket_Thruster";
					break;
				case DriveClassification.Electrostatic:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Electrostatic_Rocket_Thruster";
					break;
				case DriveClassification.Fission_Thermal:
				case DriveClassification.NuclearSaltWater:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_NuclearFission_Rocket_Thruster";
					break;
				case DriveClassification.Fission_Pulse:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_PulsedFission_Rocket_Thruster";
					break;
				case DriveClassification.Fusion_Thermal:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Nuclear_Fusion_Main_Thruster";
					break;
				case DriveClassification.Fusion_Pulse:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Pulsed_Fusion_Main_Thruster";
					break;
				case DriveClassification.Antimatter:
					text = "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_AntiMatter_Rocket_Thruster";
					break;
				}
			}
			return text;
		}

		// Token: 0x060045A3 RID: 17827 RVA: 0x001C43AC File Offset: 0x001C25AC
		public string ThrusterSFXStringStrategyLayer()
		{
			string text = (this.ref_fleet.IsAlien() ? "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Alien_Vector_Thruster" : "event:/SFX/Game_SFX/Ship_Thrusters/trig_SFX_Human_Vector_Thruster");
			if (!this.ref_fleet.IsAlien())
			{
				switch (this.drive.driveClassification)
				{
				case DriveClassification.Chemical:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Chemical_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Electrothermal:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Electrothermal_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Electromagnetic:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Electromagnetic_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Electrostatic:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Electrostatic_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Fission_Thermal:
				case DriveClassification.NuclearSaltWater:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_NuclearFission_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Fission_Pulse:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_PulsedFission_Rocket_Thruster_strategy";
					break;
				case DriveClassification.Fusion_Thermal:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Nuclear_Fusion_Main_Thruster_strategy";
					break;
				case DriveClassification.Fusion_Pulse:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_Pulsed_Fusion_Main_Thruster_strategy";
					break;
				case DriveClassification.Antimatter:
					text = "event:/SFX/Game_SFX/Ship_Thrusters_Strategy/trig_SFX_AntiMatter_Rocket_Thruster_strategy";
					break;
				}
			}
			return text;
		}

		// Token: 0x060045A4 RID: 17828 RVA: 0x001C4468 File Offset: 0x001C2668
		public string NameWithDamageIcons()
		{
			StringBuilder stringBuilder = new StringBuilder(this.GetDisplayName(GameControl.control.activePlayer));
			if (this.damaged)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.shipDamageInlineSpritePath);
				if (this.seriouslyDamaged)
				{
					stringBuilder.Append(TIGlobalConfig.globalConfig.shipDamageInlineSpritePath);
					if (this.ShipStructuralDamage())
					{
						stringBuilder.Append(TIGlobalConfig.globalConfig.shipDamageInlineSpritePath);
					}
				}
			}
			if (!this.isCapableOfTransfering)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.warningInlineSpritePath);
			}
			if (this.AllWeaponsDry() && this.faction == GameControl.control.activePlayer)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.noneIconInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000DBF RID: 3519
		// (get) Token: 0x060045A5 RID: 17829 RVA: 0x001C4525 File Offset: 0x001C2725
		public float MaxSafeAerobreakingEnergy_MJ
		{
			get
			{
				return 19f * (1f + this.noseArmorValue * 0.01f);
			}
		}

		// Token: 0x17000DC0 RID: 3520
		// (get) Token: 0x060045A6 RID: 17830 RVA: 0x001C453F File Offset: 0x001C273F
		public float MaxUnsaveAerobreakingEnergy_MJ
		{
			get
			{
				return Mathf.Max(this.noseArmorValue * 20f * 0.9f, this.MaxSafeAerobreakingEnergy_MJ);
			}
		}

		// Token: 0x060045A7 RID: 17831 RVA: 0x001C455E File Offset: 0x001C275E
		public double MaxPreAerobreakVelocity_mps(double postAerobreakVelocity_mps, bool isSafe)
		{
			return Mathd.Sqrt((double)(isSafe ? this.MaxSafeAerobreakingEnergy_MJ : this.MaxUnsaveAerobreakingEnergy_MJ) * 1000000.0 / (double)this.currentMass_kg + postAerobreakVelocity_mps * postAerobreakVelocity_mps);
		}

		// Token: 0x060045A8 RID: 17832 RVA: 0x001C458D File Offset: 0x001C278D
		public TIFactionState GetFaction()
		{
			return this.faction;
		}

		// Token: 0x060045A9 RID: 17833 RVA: 0x001C4595 File Offset: 0x001C2795
		public void AddTargetedProjectile(TISpaceCombatProjectileState projectile)
		{
			projectile.EnemyTargetsMe(this);
		}

		// Token: 0x060045AA RID: 17834 RVA: 0x001C45A0 File Offset: 0x001C27A0
		public override void InitWithTemplate(TIDataTemplate rawTemplate)
		{
			TISpaceShipTemplate tispaceShipTemplate = rawTemplate as TISpaceShipTemplate;
			if (tispaceShipTemplate != null)
			{
				this.templateName = tispaceShipTemplate.dataName;
				base.InitWithTemplate(tispaceShipTemplate);
				base.SetTemplate<TISpaceShipTemplate>(tispaceShipTemplate);
				this.InitShip();
			}
		}

		// Token: 0x060045AB RID: 17835 RVA: 0x001C45D8 File Offset: 0x001C27D8
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.CacheInternalPowerStats();
			if (this.template.utilityModules.Any<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate == null))
			{
				this.template.moduleTemplateEntries.RemoveAll((ModuleDataTemplateEntry x) => x.moduleTemplate == null);
				this.template.ReCacheUtilityModules();
			}
			if (this.utilityModules.Any<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate == null))
			{
				this.utilityModules.RemoveAll((ModuleDataEntry x) => x.moduleTemplate == null);
			}
			this.damagedParts.RemoveAll((DamagedShipPartData x) => x.module.moduleTemplate == null);
			this.damagedPartsCache.Clear();
			this.damagedParts.ForEach(delegate(DamagedShipPartData x)
			{
				this.damagedPartsCache[x.module] = x;
			});
			this.plannedResupplyAndRepair.modulesToRepair.RemoveAll((DamagedShipPartData x) => x.module.moduleTemplate == null);
		}

		// Token: 0x060045AC RID: 17836 RVA: 0x001C472C File Offset: 0x001C292C
		public override void PostCanvasManagerCreateInit_3()
		{
			this.plannedResupplyAndRepair.ship = this;
			foreach (TIOfficerState tiofficerState in this.officers.ToList<TIOfficerState>())
			{
				if (tiofficerState.ship != this)
				{
					tiofficerState.TransferOfficerBetweenShips(this, false, false, false);
					Log.Error("Removed officer from bad shipState to " + this.displayName, Array.Empty<object>());
				}
				if (!TIGameState.Valid(tiofficerState))
				{
					this.officers.Remove(tiofficerState);
					Log.Error("Removed bad officer from " + this.displayName, Array.Empty<object>());
				}
			}
			if (this.batteryCharge == null)
			{
				this.batteryCharge = this.utilityModules.Where<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.isBattery).ToDictionary<ModuleDataEntry, ModuleDataEntry, float>((ModuleDataEntry x) => x, (ModuleDataEntry x) => x.moduleTemplate.ref_battery.energyCapacity_GJ);
			}
			this.SetCombatSystems();
			this.UpdatePropulsionValues(false);
		}

		// Token: 0x060045AD RID: 17837 RVA: 0x001C4874 File Offset: 0x001C2A74
		public override void PostInitializationInit_4()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.CompleteShipInitialization();
			}
			this.SetMissionControlConsumption();
		}

		// Token: 0x060045AE RID: 17838 RVA: 0x001C488C File Offset: 0x001C2A8C
		public override void PostVisualizerCreationInit_7()
		{
			this.BuildInternalDamageTables();
			this.spaceCombatValueDataDirty = true;
			this.SpaceCombatValue(false, 0f);
			this.CacheEffectiveBeamRanges();
			this.currentManeuverSequence = new ShipManeuverSequence(0.124499954f, 0.124499954f, 0.0019163926f, 0.8832783f);
			if (this.inManeuver && this.currentManeuver.maneuverFinish == null)
			{
				this.inManeuver = false;
			}
			if (this.inManeuverSequence && this.currentManeuverSequence.End == null)
			{
				this.inManeuverSequence = false;
			}
			this.RepairFleetMembership();
		}

		// Token: 0x060045AF RID: 17839 RVA: 0x001C491C File Offset: 0x001C2B1C
		public override void PostEverythingSaveRepair_8()
		{
			if (this.fleet != null && this.fleet.ships != null && !this.fleet.ships.Contains(this))
			{
				bool flag = true;
				foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
				{
					foreach (TIHabModuleState tihabModuleState in tifactionState.nShipyardQueues.Keys)
					{
						foreach (ShipConstructionQueueItem shipConstructionQueueItem in tifactionState.nShipyardQueues[tihabModuleState])
						{
							if (shipConstructionQueueItem.shipDesign == this.template || shipConstructionQueueItem.refit_originalShipDesign == this.template)
							{
								flag = false;
							}
						}
					}
				}
				if (flag)
				{
					Debug.LogError("Save Repair, deleting phantom refitted shipstate: " + base.ID.ToString() + ", " + this.displayName);
					this.DestroyShip(false, null);
				}
			}
			bool flag2 = false;
			IEnumerable<ModuleDataEntry> enumerable = this.template.noseWeapons;
			using (List<ModuleDataEntry>.Enumerator enumerator3 = this.noseWeapons.ToList<ModuleDataEntry>().GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					ModuleDataEntry weaponData2 = enumerator3.Current;
					if (enumerable.None<ModuleDataEntry>((ModuleDataEntry x) => x.slotIndex == weaponData2.slotIndex))
					{
						string[] array2 = new string[9];
						array2[0] = base.ID.ToString();
						array2[1] = " ";
						array2[2] = this.displayName;
						array2[3] = "/";
						array2[4] = this.template.fullClassName;
						array2[5] = ": Reparing bad slot assginment for ";
						array2[6] = weaponData2.moduleTemplateName;
						array2[7] = " Slot ";
						int num = 8;
						int i = weaponData2.slotIndex;
						array2[num] = i.ToString();
						Log.Error(string.Concat(array2), Array.Empty<object>());
						flag2 = true;
						List<int> list = (from x in enumerable
							where x.weaponTemplate == weaponData2.weaponTemplate
							select x.slotIndex).Except<int>(from x in this.noseWeapons
							where x.weaponTemplate == weaponData2.weaponTemplate
							select x.slotIndex).ToList<int>();
						if (list.Count > 0)
						{
							weaponData2.CorrectBrokenSlot(list.First<int>());
						}
					}
				}
			}
			IEnumerable<ModuleDataEntry> enumerable2 = this.template.hullWeapons;
			using (List<ModuleDataEntry>.Enumerator enumerator3 = this.hullWeapons.ToList<ModuleDataEntry>().GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					ModuleDataEntry weaponData = enumerator3.Current;
					if (enumerable2.None<ModuleDataEntry>((ModuleDataEntry x) => x.slotIndex == weaponData.slotIndex))
					{
						string[] array3 = new string[9];
						array3[0] = base.ID.ToString();
						array3[1] = " ";
						array3[2] = this.displayName;
						array3[3] = "/";
						array3[4] = this.template.fullClassName;
						array3[5] = ": Repairing bad hull slot assginment for ";
						array3[6] = weaponData.moduleTemplateName;
						array3[7] = " Slot ";
						int num2 = 8;
						int i = weaponData.slotIndex;
						array3[num2] = i.ToString();
						Log.Error(string.Concat(array3), Array.Empty<object>());
						flag2 = true;
						List<int> list2 = (from x in enumerable2
							where x.weaponTemplate == weaponData.weaponTemplate
							select x.slotIndex).Except<int>(from x in this.hullWeapons
							where x.weaponTemplate == weaponData.weaponTemplate
							select x.slotIndex).ToList<int>();
						if (list2.Count > 0)
						{
							weaponData.CorrectBrokenSlot(list2.First<int>());
						}
					}
				}
			}
			if (this.AllWeaponModuleData().Any<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIProjectileWeaponTemplate ref_projectileWeapon = x.moduleTemplate.ref_projectileWeapon;
				return ref_projectileWeapon != null && ref_projectileWeapon.hasMagazine() && this.ammo[x] > x.weaponTemplate.ref_projectileWeapon.FullAmmoCount_Current(this);
			}))
			{
				flag2 = true;
			}
			if (flag2)
			{
				this.ammo.Clear();
				this.LoadAmmo();
			}
			if (TIGameState.Valid(this.fleet) && !this.inManeuver && !this.inManeuverSequence && this.currentRotation != this.fleet.ships[0].currentRotation)
			{
				this.currentRotation = this.fleet.ships[0].currentRotation;
			}
		}

		// Token: 0x060045B0 RID: 17840 RVA: 0x001C4E6C File Offset: 0x001C306C
		public void CompleteShipInitialization()
		{
			this.template.designingFaction.RecordShipBuilt(this.template);
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x060045B1 RID: 17841 RVA: 0x001C4E8C File Offset: 0x001C308C
		private void RepairFleetMembership()
		{
			if (this.fleet == null)
			{
				bool flag = false;
				TIFactionState[] array = GameStateManager.AllFactions();
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].nShipyardQueues.Any<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value.Any<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.originalSpaceShipState == this)))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Log.Error(this.displayName + " has no fleet and is not being refit.", Array.Empty<object>());
					return;
				}
			}
			else
			{
				foreach (TISpaceFleetState tispaceFleetState in GameStateManager.IterateByClass<TISpaceFleetState>(false))
				{
					if (tispaceFleetState.ships.Contains(this) && tispaceFleetState != this.fleet)
					{
						tispaceFleetState.ships.Remove(this);
						Log.Error(this.displayName + " duplicated in wrong fleet", Array.Empty<object>());
					}
				}
			}
		}

		// Token: 0x060045B2 RID: 17842 RVA: 0x001C4F80 File Offset: 0x001C3180
		public void InitShip()
		{
			this.SetDisplayName(this.hull.displayName);
			this.currentMass_kg = this.template.dryMass_kg;
			this.noseWeapons = new List<ModuleDataEntry>(this.template.noseWeapons);
			this.hullWeapons = new List<ModuleDataEntry>(this.template.hullWeapons);
			this.officers = new List<TIOfficerState>();
			this.kills = new List<string>();
			this.utilityModules = this.template.utilityModules.ToList<ModuleDataEntry>();
			this.radiatorsExtended = true;
			this.batteryCharge = this.template.utilityModules.Where<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.isBattery).ToDictionary<ModuleDataEntry, ModuleDataEntry, float>((ModuleDataEntry x) => x, (ModuleDataEntry x) => x.moduleTemplate.ref_battery.energyCapacity_GJ);
			this.UpdateHeatSinkCapacity_GJ();
			this.currentRotation = Quaternion.identity;
			this.armor = new Dictionary<ArmorFacing, TISpaceShipState.ArmorData>
			{
				{
					ArmorFacing.Nose,
					new TISpaceShipState.ArmorData(this.template.noseArmorValue)
				},
				{
					ArmorFacing.Left,
					new TISpaceShipState.ArmorData(this.template.lateralArmorValue)
				},
				{
					ArmorFacing.Right,
					new TISpaceShipState.ArmorData(this.template.lateralArmorValue)
				},
				{
					ArmorFacing.Tail,
					new TISpaceShipState.ArmorData(this.template.tailArmorValue)
				}
			};
			this.ammo = new Dictionary<ModuleDataEntry, int>();
			this.InstantFullRepair();
			this.CacheInternalPowerStats();
			this.SetCombatSystems();
			this.BuildInternalDamageTables();
			this.CacheEffectiveBeamRanges();
			this.currentManeuverSequence = new ShipManeuverSequence(0.124499954f, 0.124499954f, 0.0019163926f, 0.8832783f);
			this.SetMissionControlConsumption();
			this.launchDate = TITimeState.Now();
		}

		// Token: 0x060045B3 RID: 17843 RVA: 0x001C5158 File Offset: 0x001C3358
		public void CopyDataForRefit(TISpaceShipState originalShip)
		{
			this.SetDisplayName(originalShip.displayName);
			if (originalShip.kills != null)
			{
				this.kills = new List<string>(originalShip.kills);
			}
			if (originalShip.launchDate != null)
			{
				this.launchDate = new TIDateTime(originalShip.launchDate);
			}
			this.lastRefitDate = TITimeState.Now();
			foreach (TIOfficerState tiofficerState in originalShip.officers.ToList<TIOfficerState>())
			{
				tiofficerState.TransferOfficerBetweenShips(this, true, false, false);
			}
		}

		// Token: 0x060045B4 RID: 17844 RVA: 0x001C5204 File Offset: 0x001C3404
		public void CreateVisualizer(ShipVisController controller)
		{
			this.visualizerLink = controller;
		}

		// Token: 0x060045B5 RID: 17845 RVA: 0x001C520D File Offset: 0x001C340D
		public void InitDamageLayer(DamageLayer damageLayer)
		{
			if (this.damagePoints != null)
			{
				damageLayer.LoadDamagePoints(this.damagePoints);
			}
			this.damageLayer = damageLayer;
		}

		// Token: 0x060045B6 RID: 17846 RVA: 0x001C522A File Offset: 0x001C342A
		public override void SetDisplayName(string newName)
		{
			if (this.displayName != newName)
			{
				this.displayName = newName;
			}
		}

		// Token: 0x060045B7 RID: 17847 RVA: 0x001C5244 File Offset: 0x001C3444
		public static List<IHullSection> SetUpArmorSections(TISpaceShipState ship)
		{
			float num;
			if (TIGlobalValuesState.GlobalValues.scenarioCustomizations.cinematicCombatRealismScale)
			{
				num = 30f;
			}
			else
			{
				num = ship.hull.baseArmorCapAngleCoverage_deg_realisticScaling * 2f;
			}
			float num2 = 90f - num;
			List<IHullSection> list = new List<IHullSection>(3);
			HullSection hullSection = new HullSection(ship, new Facing(0f, num, ArmorFacing.Nose));
			HullSection hullSection2 = new HullSection(ship);
			hullSection2.AddFacing(new Facing(90f, num2, ArmorFacing.Right));
			hullSection2.AddFacing(new Facing(-90f, num2, ArmorFacing.Left));
			HullSection hullSection3 = new HullSection(ship, new Facing(180f, num, ArmorFacing.Tail));
			list.Add(hullSection);
			list.Add(hullSection2);
			list.Add(hullSection3);
			return list;
		}

		// Token: 0x060045B8 RID: 17848 RVA: 0x001C52F2 File Offset: 0x001C34F2
		public float GetMonthlyNetIncome(FactionResource resource)
		{
			return this.template.GetMonthlyNetIncome(resource);
		}

		// Token: 0x060045B9 RID: 17849 RVA: 0x001C5300 File Offset: 0x001C3500
		public float GetMonthlyGrossRevenue(FactionResource resource)
		{
			return this.template.GetMonthlyGrossRevenue(resource);
		}

		// Token: 0x060045BA RID: 17850 RVA: 0x001C530E File Offset: 0x001C350E
		public float GetMonthlyExpenses(FactionResource resource)
		{
			return this.template.GetMonthlyExpenses(resource);
		}

		// Token: 0x060045BB RID: 17851 RVA: 0x001C531C File Offset: 0x001C351C
		public void DestroyShip(bool killPersonnel, TIFactionState destroyer)
		{
			if (!GameControl.control.skirmishMode)
			{
				if (killPersonnel)
				{
					this.councilorPassengers.ForEach(delegate(TICouncilorState x)
					{
						x.KillCouncilor(true, destroyer);
					});
					this.officers.ToList<TIOfficerState>().ForEach(delegate(TIOfficerState x)
					{
						x.DeleteOfficer(true);
					});
				}
				else
				{
					foreach (TIOfficerState tiofficerState in this.officers.ToList<TIOfficerState>())
					{
						if (!tiofficerState.Escape(true, true))
						{
							tiofficerState.DeleteOfficer(false);
						}
					}
					foreach (TICouncilorState ticouncilorState in this.councilorPassengers)
					{
						if (ticouncilorState.HasMission)
						{
							ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.CouncilorUnavailable, "");
						}
					}
					TISpaceFleetState fleet = this.fleet;
					bool flag;
					if (fleet == null)
					{
						flag = false;
					}
					else
					{
						List<TISpaceShipState> ships = fleet.ships;
						int? num = ((ships != null) ? new int?(ships.Count) : null);
						int num2 = 1;
						flag = (num.GetValueOrDefault() > num2) & (num != null);
					}
					if (flag)
					{
						Func<TISpaceShipState, bool> <>9__3;
						this.councilorPassengers.ForEach(delegate(TICouncilorState x)
						{
							IEnumerable<TISpaceShipState> ships2 = this.fleet.ships;
							Func<TISpaceShipState, bool> func;
							if ((func = <>9__3) == null)
							{
								func = (<>9__3 = (TISpaceShipState x) => x != this);
							}
							x.SetLocation(ships2.First<TISpaceShipState>(func));
						});
					}
					else if (this.faction != null)
					{
						TISpaceFleetState altFleet = this.faction.fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x.location == this.fleet.location && x != this.fleet);
						if (altFleet != null)
						{
							this.councilorPassengers.ForEach(delegate(TICouncilorState x)
							{
								x.SetLocation(altFleet.ships.First<TISpaceShipState>());
							});
						}
						else
						{
							TIHabState altHab = ((this.fleet.dockedAtHab && TIGameState.Valid(this.fleet.dockedLocation.ref_hab)) ? this.fleet.dockedLocation.ref_hab : this.faction.habs.FirstOrDefault<TIHabState>((TIHabState x) => x.location == this.fleet.location));
							if (altHab != null)
							{
								this.councilorPassengers.ForEach(delegate(TICouncilorState x)
								{
									x.SetLocation(altHab);
								});
							}
							TISpaceBodyState ref_spaceBody = this.ref_spaceBody;
							if (ref_spaceBody != null && ref_spaceBody.isEarth)
							{
								this.councilorPassengers.ForEach(delegate(TICouncilorState x)
								{
									x.SetLocation(GameStateManager.AllRegions().SelectRandomItem<TIRegionState>());
								});
							}
							else if (this.ref_spaceBody != null)
							{
								TIHabState altBase = this.ref_spaceBody.habs.FirstOrDefault<TIHabState>((TIHabState x) => x.faction == this.faction);
								if (altBase != null)
								{
									this.councilorPassengers.ForEach(delegate(TICouncilorState x)
									{
										x.SetLocation(altBase);
									});
								}
								else
								{
									this.councilorPassengers.ForEach(delegate(TICouncilorState x)
									{
										x.KillCouncilor(true, destroyer);
									});
								}
							}
						}
					}
				}
				if (this.fleet != null)
				{
					if (this.fleet.faction != null)
					{
						this.fleet.faction.SetMissionControlUsageDataDirty();
						this.fleet.faction.SetResourceIncomeDataDirty(TISpaceShipState.relevantIncomeResources);
					}
					if (destroyer != null && this.fleet.ships != null)
					{
						destroyer.RegisterKill(this, (float)this.hull.consTier);
					}
				}
			}
			TISpaceFleetState fleet2 = this.fleet;
			if (((fleet2 != null) ? fleet2.ships : null) != null)
			{
				this.fleet.RemoveShipsFromFleet(new List<TISpaceShipState> { this }, null);
			}
			this.fleet = null;
			this.inManeuver = false;
			if (!GameControl.control.skirmishMode)
			{
				foreach (TIMissionState timissionState in GameStateManager.AllActiveMissions())
				{
					if (timissionState.target.ref_ship == this)
					{
						timissionState.ResolveMission(TIMissionState.AbortReason.TargetShipDestroyed, "");
					}
				}
				foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
				{
					tifactionState.ExpireIntel(this, true);
				}
				World.Active.GetExistingManager<GameTimeManager>().CancelAllTimeEventsForObject(this);
			}
			this.officers.Clear();
			this.ClearRadiatorAudio();
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdatePropulsionValues_Combat), this._combatUpdatePropulsionEventName);
			GameControl.eventManager.RemoveListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null);
			if (this.visualizerLink != null)
			{
				global::UnityEngine.Object.Destroy(this.visualizerLink.gameObject);
			}
			base.ArchiveState(true);
			GameStateManager.RemoveGameState<TISpaceShipState>(base.ID, false);
		}

		// Token: 0x060045BC RID: 17852 RVA: 0x001C5838 File Offset: 0x001C3A38
		public TIResourcesCost ScuttleCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (this.ref_fleet.dockedAtHab && this.ref_hab.faction == this.faction)
			{
				if (this.ref_hab.AllowsShipConstruction(this.ref_hab.faction, true, false))
				{
					tiresourcesCost = this.template.spaceResourceConstructionCost(false, null, false, false, true);
					tiresourcesCost = tiresourcesCost.MultiplyCost(-TemplateManager.global.scuttleRefund);
				}
			}
			else
			{
				int crewBillets = this.template.crewBillets;
				IEnumerable<TISpaceShipState> enumerable = this.ref_fleet.ships.Except<TISpaceShipState>(new List<TISpaceShipState> { this });
				float num;
				if (enumerable == null)
				{
					num = (float)0;
				}
				else
				{
					num = (float)enumerable.Sum<TISpaceShipState>((TISpaceShipState x) => x.template.crewBillets);
				}
				int num2 = (int)(num * 0.25f);
				int num3 = crewBillets - num2;
				if (num3 > 0)
				{
					TIOrbitState ref_orbit = this.ref_orbit;
					if (ref_orbit == null || !ref_orbit.isEarthLEO)
					{
						TIOrbitState ref_orbit2 = this.ref_orbit;
						float num4 = ((ref_orbit2 != null && ref_orbit2.isEarthLEO) ? 0.1f : ((float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(this.ref_faction, this.fleet.landed ? this.ref_habSite.ref_gameState : this.ref_orbit.ref_gameState, (float)num3 * TemplateManager.global.scuttlePerCrewMassCost)));
						tiresourcesCost.AddCost(FactionResource.Boost, num4, true);
					}
				}
			}
			return tiresourcesCost;
		}

		// Token: 0x060045BD RID: 17853 RVA: 0x001C5992 File Offset: 0x001C3B92
		public void SetVisualizationDataDirty()
		{
			if (this.visualizationDataDirtyFrame != TIFrameCounter.FrameCount)
			{
				GameControl.eventManager.TriggerEvent(new ShipVisualizationDataDirty(this), null, new object[] { this, this.fleet });
				this.visualizationDataDirtyFrame = TIFrameCounter.FrameCount;
			}
		}

		// Token: 0x060045BE RID: 17854 RVA: 0x001C59D0 File Offset: 0x001C3BD0
		public void ClearShipDamageVisualizations()
		{
			this.damagePoints.Clear();
			if (this.damageLayer == null && this.visualizerLink != null)
			{
				this.damageLayer = this.visualizerLink.GetComponentInChildren<DamageLayer>(true);
			}
			if (this.damageLayer != null)
			{
				this.damageLayer.ClearDamagePoints();
			}
			this.damageVisualizationDirty = true;
		}

		// Token: 0x17000DC1 RID: 3521
		// (get) Token: 0x060045BF RID: 17855 RVA: 0x001C5A36 File Offset: 0x001C3C36
		public float angularAcceleration_degs2
		{
			get
			{
				return 57.29578f * this.angular_acceleration_rads2;
			}
		}

		// Token: 0x17000DC2 RID: 3522
		// (get) Token: 0x060045C0 RID: 17856 RVA: 0x001C5A44 File Offset: 0x001C3C44
		public float currentThrust_N
		{
			get
			{
				float num = 1f;
				foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule != null)
					{
						float thrustMultiplier = ref_utilityModule.thrustMultiplier;
						if (thrustMultiplier != 0f && this.GetPartFunction(moduleDataEntry) >= 1f)
						{
							num *= thrustMultiplier;
						}
					}
				}
				return this.drive.thrust_N * num * this.damage_mainThrustModifier;
			}
		}

		// Token: 0x17000DC3 RID: 3523
		// (get) Token: 0x060045C1 RID: 17857 RVA: 0x001C5AE0 File Offset: 0x001C3CE0
		public float currentEV_kps
		{
			get
			{
				float num = 1f;
				foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule != null && ref_utilityModule.EVMultiplier != 0f && this.GetPartFunction(moduleDataEntry) >= 1f)
					{
						num *= ref_utilityModule.EVMultiplier;
					}
				}
				return this.drive.EV_kps * num;
			}
		}

		// Token: 0x060045C2 RID: 17858 RVA: 0x001C5B74 File Offset: 0x001C3D74
		public void SetPropulsionValuesDirty(bool immediate = false, bool forceUseCurrentMass = false)
		{
			if (immediate)
			{
				this.UpdatePropulsionValues(forceUseCurrentMass);
				return;
			}
			this.propulsionValuesDataDirty = true;
		}

		// Token: 0x060045C3 RID: 17859 RVA: 0x001C5B88 File Offset: 0x001C3D88
		private void UpdatePropulsionValues_Combat(TimeEventStart e)
		{
			this.UpdatePropulsionValues(false);
		}

		// Token: 0x060045C4 RID: 17860 RVA: 0x001C5B94 File Offset: 0x001C3D94
		public float CombatAccelerationGivenRemainingDV_mps2(float remainingDV_mps)
		{
			float num = this.maxCombatAccleration_g * 9.80665f;
			float num2 = (float)this.dryMass_kg * Mathf.Exp(remainingDV_mps / (this.currentEV_kps * 1000f));
			return Mathf.Min(this.currentThrust_N * (this.modifiedThrustCap / num2), num);
		}

		// Token: 0x17000DC4 RID: 3524
		// (get) Token: 0x060045C5 RID: 17861 RVA: 0x001C5BE0 File Offset: 0x001C3DE0
		public float modifiedThrustCap
		{
			get
			{
				float thrustCap = this.drive.thrustCap;
				return thrustCap + this.SumOfficerEffectsModifiers(OfficerEffectType.DriveCombatThrustMultiplier, thrustCap);
			}
		}

		// Token: 0x060045C6 RID: 17862 RVA: 0x001C5C06 File Offset: 0x001C3E06
		public float NotionalDeltaVChange_kps(float propellantChange_tons)
		{
			return (float)((double)this.currentEV_kps * Mathd.Log(((double)(this.propellant_tons + propellantChange_tons) + this.dryMass_tons) / this.dryMass_tons)) - this.currentDeltaV_kps;
		}

		// Token: 0x17000DC5 RID: 3525
		// (get) Token: 0x060045C7 RID: 17863 RVA: 0x001C5C34 File Offset: 0x001C3E34
		public float maxCruiseAcceleration_g
		{
			get
			{
				float num = (this.isAlien ? TemplateManager.global.maxAlienCruiseAcceleration_g : TemplateManager.global.baselineMaxHumanCruiseAcceleration_g);
				num += TIEffectsState.SumEffectsModifiers(Context.Ship_MaxSurvivableCruiseAcceleration_Bonus, this.faction, num, null);
				return num + this.SumOfficerEffectsModifiers(OfficerEffectType.MaxSurvivableCruiseAcceleration, num);
			}
		}

		// Token: 0x17000DC6 RID: 3526
		// (get) Token: 0x060045C8 RID: 17864 RVA: 0x001C5C84 File Offset: 0x001C3E84
		public float maxCombatAccleration_g
		{
			get
			{
				float num = (this.isAlien ? TemplateManager.global.maxAlienCombatAcceleration_g : TemplateManager.global.baselineMaxHumanCombatAcceleration_g);
				num += TIEffectsState.SumEffectsModifiers(Context.Ship_MaxSurvivableCombatAcceleration_Bonus, this.faction, num, null);
				return num + this.SumOfficerEffectsModifiers(OfficerEffectType.MaxSurvivableCombatAcceleration, num);
			}
		}

		// Token: 0x17000DC7 RID: 3527
		// (get) Token: 0x060045C9 RID: 17865 RVA: 0x001C5CD2 File Offset: 0x001C3ED2
		public float maxAngularVelocity_rad_s
		{
			get
			{
				return this.template.maxAngularVelocity_mps / (this.hull.length_m * 0.5f);
			}
		}

		// Token: 0x060045CA RID: 17866 RVA: 0x001C5CF4 File Offset: 0x001C3EF4
		public void UpdatePropulsionValues(bool forceUseCurrentMass = false)
		{
			float cruiseAcceleration_mps = this.cruiseAcceleration_mps2;
			float combatAcceleration_mps = this.combatAcceleration_mps2;
			float angular_acceleration_rads = this.angular_acceleration_rads2;
			float currentMaxDeltaV_kps = this.currentMaxDeltaV_kps;
			float num = this.maxCruiseAcceleration_g * 9.80665f;
			float num2 = this.maxCombatAccleration_g * 9.80665f;
			TISpaceFleetState fleet = this.fleet;
			bool flag = fleet != null && fleet.inCombat;
			if (flag)
			{
				this.combatAcceleration_mps2 = Mathf.Min(this.currentThrust_N * this.modifiedThrustCap / this.currentMass_kg, num2);
				this.SetAngularAcceleration_rads2(-1f);
			}
			else if (forceUseCurrentMass)
			{
				this.combatAcceleration_mps2 = Mathf.Min(this.currentThrust_N * this.modifiedThrustCap / this.currentMass_kg, num2);
				this.SetAngularAcceleration_rads2(-1f);
			}
			else
			{
				this.cruiseAcceleration_mps2 = Mathf.Min(this.currentThrust_N / this.template.wetMass_kg, num);
				this.combatAcceleration_mps2 = Mathf.Min(this.currentThrust_N * this.modifiedThrustCap / (float)this.wetMass_kg, num2);
				this.SetAngularAcceleration_rads2(this.template.wetMass_kg);
			}
			this.max_angular_velocity_rad_s = this.maxAngularVelocity_rad_s;
			this.currentMaxDeltaV_kps = this.currentEV_kps * Mathf.Log(this.template.wetMass_tons / this.template.dryMass_tons(false));
			this.SetCurrentDeltaVFromPropellantMass();
			if (this.currentMaxDeltaV_kps < currentMaxDeltaV_kps)
			{
				TISpaceFleetState fleet2 = this.fleet;
				if (fleet2 != null && fleet2.inCombatOrWaitingForCombat && flag)
				{
					TISpaceCombatState combatState = GameControl.spaceCombat.combatState;
					if (combatState != null)
					{
						combatState.UpdateMaxDeltaVForShip(this);
					}
				}
			}
			if (!flag && (Mathf.Abs(cruiseAcceleration_mps - this.cruiseAcceleration_mps2) >= 1E-45f || Mathf.Abs(angular_acceleration_rads - this.angular_acceleration_rads2) >= 0.001f || Mathf.Abs(currentMaxDeltaV_kps - this.currentMaxDeltaV_kps) >= 1E-45f))
			{
				GameControl.eventManager.TriggerEvent(new ShipPropulsionValuesUpdated(this), null, new object[] { this });
			}
			else if (flag && (Mathf.Abs(combatAcceleration_mps - this.combatAcceleration_mps2) >= 1E-45f || Mathf.Abs(angular_acceleration_rads - this.angular_acceleration_rads2) >= 0.001f))
			{
				GameControl.eventManager.TriggerEvent(new CombatShipPropulsionValuesUpdated(this), null, new object[] { this });
			}
			this.propulsionValuesDataDirty = false;
			this.spaceCombatValueDataDirty = true;
		}

		// Token: 0x060045CB RID: 17867 RVA: 0x001C5F2D File Offset: 0x001C412D
		private void ChangeCurrentMass_kg(float change_kg, bool instantPropulsionUpdate = false)
		{
			this.currentMass_kg += change_kg;
			this.SetPropulsionValuesDirty(instantPropulsionUpdate, false);
		}

		// Token: 0x060045CC RID: 17868 RVA: 0x001C5F48 File Offset: 0x001C4148
		private void ChangePropellant_tons(float change_tons, bool instantPropulsionUpdate = false)
		{
			float num = this.propellant_tons;
			this.propellant_tons = Mathf.Clamp(this.propellant_tons + change_tons, 0f, this.template.propellantMass_tons);
			change_tons = this.propellant_tons - num;
			this.ChangeCurrentMass_kg(change_tons * 1000f, instantPropulsionUpdate);
		}

		// Token: 0x060045CD RID: 17869 RVA: 0x001C5F97 File Offset: 0x001C4197
		private void SetPropellant_tons(float value, bool instantPropulsionUpdate = false)
		{
			value = Mathf.Clamp(value, 0f, this.template.propellantMass_tons);
			this.ChangePropellant_tons(value - this.propellant_tons, instantPropulsionUpdate);
		}

		// Token: 0x060045CE RID: 17870 RVA: 0x001C5FC0 File Offset: 0x001C41C0
		public float DriveHeat_GJ()
		{
			return this.powerPlant.WasteHeat_GW(this.drive.openCycleCooling, this.drive.powerRequirement_GW, 0f);
		}

		// Token: 0x060045CF RID: 17871 RVA: 0x001C5FE8 File Offset: 0x001C41E8
		public void RunDriveInCombat(float combatDV_kps, float combatAcceleration_kps2, float massPriorToBurn_kg)
		{
			if (combatAcceleration_kps2 == 0f)
			{
				combatAcceleration_kps2 = this.cruiseAcceleration_kps2;
			}
			float num = this.DVconsumedInCombat(combatDV_kps, combatAcceleration_kps2, massPriorToBurn_kg);
			float num2 = combatDV_kps / combatAcceleration_kps2 * this.DriveHeat_GJ();
			this.ConsumeDeltaV(num, true);
			this.ApplyHeat(num2, num2 != 0f);
		}

		// Token: 0x060045D0 RID: 17872 RVA: 0x001C6034 File Offset: 0x001C4234
		private float GetCombatRealismDVConsumptionFactor()
		{
			return (float)(GameStateManager.GlobalValues().scenarioCustomizations.cinematicCombatRealismDV ? 0 : 1);
		}

		// Token: 0x060045D1 RID: 17873 RVA: 0x001C604C File Offset: 0x001C424C
		public float GetDVConservingCombatAcceleration_mps2(float targetThrustDuration_s, float dvBudget_kps)
		{
			float num = dvBudget_kps * 1000f / targetThrustDuration_s;
			if (num <= this.cruiseAcceleration_mps2)
			{
				return this.cruiseAcceleration_mps2;
			}
			if (this.GetCombatRealismDVConsumptionFactor() == 0f)
			{
				return Mathf.Min(num, this.combatAcceleration_mps2);
			}
			return Mathf.Clamp(Mathf.Pow(this.cruiseAcceleration_mps2 * dvBudget_kps * 1000f / targetThrustDuration_s, 0.5f), this.cruiseAcceleration_mps2, this.combatAcceleration_mps2);
		}

		// Token: 0x060045D2 RID: 17874 RVA: 0x001C60B8 File Offset: 0x001C42B8
		public float DVconsumedInCombat(float combatDV_kps, float combatAcceleration_kps2, float massPriorToBurn_kg)
		{
			if (combatDV_kps == 0f)
			{
				return 0f;
			}
			float combatRealismDVConsumptionFactor = this.GetCombatRealismDVConsumptionFactor();
			if (combatRealismDVConsumptionFactor == 0f)
			{
				return combatDV_kps;
			}
			if (this.drive.driveClassification == DriveClassification.Fission_Pulse || this.drive.driveClassification == DriveClassification.Fusion_Pulse)
			{
				return combatDV_kps;
			}
			if (combatAcceleration_kps2 <= this.cruiseAcceleration_kps2)
			{
				return combatDV_kps;
			}
			float currentEV_kps = this.currentEV_kps;
			float num = this.cruiseAcceleration_kps2 * (float)this.wetMass_kg / combatAcceleration_kps2;
			if (num > massPriorToBurn_kg)
			{
				return combatDV_kps;
			}
			float num2 = currentEV_kps * num;
			float num3 = num2 * (1f / num - 1f / massPriorToBurn_kg);
			if (num3 >= combatDV_kps)
			{
				float num4 = num2 * massPriorToBurn_kg / (combatDV_kps * massPriorToBurn_kg + num2);
				float num5 = currentEV_kps * Mathf.Log(massPriorToBurn_kg / num4);
				return Mathf.Lerp(combatDV_kps, num5, combatRealismDVConsumptionFactor);
			}
			float num6 = currentEV_kps * Mathf.Log(massPriorToBurn_kg / num);
			float num7 = combatDV_kps - num3;
			float num8 = num6 + num7;
			return Mathf.Lerp(combatDV_kps, num8, combatRealismDVConsumptionFactor);
		}

		// Token: 0x060045D3 RID: 17875 RVA: 0x001C6188 File Offset: 0x001C4388
		public void ConsumeDeltaV(float DV_kps_consumed, bool instantPropulsionUpdate = true)
		{
			if (DV_kps_consumed > 0f)
			{
				this.currentDeltaV_kps = Mathf.Max(this.currentDeltaV_kps - DV_kps_consumed, 0f);
				float num = (Mathf.Exp(this.currentDeltaV_kps / this.currentEV_kps) - 1f) * (float)this.dryMass_tons;
				this.SetPropellant_tons(num, instantPropulsionUpdate);
				GameControl.eventManager.TriggerEvent(new ShipDeltaVChange(this), null, new object[] { this });
			}
		}

		// Token: 0x060045D4 RID: 17876 RVA: 0x001C61FC File Offset: 0x001C43FC
		public void RefundDeltaV(float DV_kps_refunded)
		{
			if (DV_kps_refunded > 0f)
			{
				this.currentDeltaV_kps += DV_kps_refunded;
				this.currentDeltaV_kps = Mathf.Min(this.currentDeltaV_kps, this.currentMaxDeltaV_kps);
				float num = (Mathf.Exp(this.currentDeltaV_kps / this.currentEV_kps) - 1f) * (float)this.dryMass_tons;
				this.SetPropellant_tons(num, false);
			}
		}

		// Token: 0x060045D5 RID: 17877 RVA: 0x001C6260 File Offset: 0x001C4460
		private void SetCurrentDeltaVFromPropellantMass()
		{
			float num = (float)((double)this.currentEV_kps * Mathd.Log(((double)this.propellant_tons + this.dryMass_tons) / this.dryMass_tons));
			bool flag = num > this.currentDeltaV_kps + 0.001f || num < this.currentDeltaV_kps - 0.001f;
			this.currentDeltaV_kps = num;
			if (flag)
			{
				GameControl.eventManager.TriggerEvent(new ShipDeltaVChange(this), null, new object[] { this });
			}
		}

		// Token: 0x060045D6 RID: 17878 RVA: 0x001C62D8 File Offset: 0x001C44D8
		public float AvailableDeltaVForCombat_kps()
		{
			if (this.fleet != null && !this.fleet.inTransfer)
			{
				return this.currentDeltaV_kps;
			}
			return Mathf.Max(0f, this.currentDeltaV_kps - ((this.fleet != null) ? this.fleet.DVRequiredToCompleteTrajectory_kps() : 0f));
		}

		// Token: 0x060045D7 RID: 17879 RVA: 0x001C6338 File Offset: 0x001C4538
		public bool DoesDriveHeatExceedRadiatorAndOverheatInOneSecond()
		{
			float num = this.DriveHeat_GJ() + this.RadiatorCooling_GJ();
			return num > 0f && this.accumulatedHeat_GJ + num >= this.currentHeatSinkCapacity_GJ;
		}

		// Token: 0x060045D8 RID: 17880 RVA: 0x001C6370 File Offset: 0x001C4570
		public float GetCombatDeltaVFromPropellantMass()
		{
			return (float)((double)this.maxThrust_combatExhaustVelocity_kps * Mathd.Log(((double)this.propellant_tons + this.dryMass_tons) / this.dryMass_tons));
		}

		// Token: 0x060045D9 RID: 17881 RVA: 0x001C6395 File Offset: 0x001C4595
		public float GetTotalMassFromDVRemaining(float DV_remaining_kps)
		{
			return Mathf.Exp(DV_remaining_kps / this.currentEV_kps) * (float)this.dryMass_kg;
		}

		// Token: 0x060045DA RID: 17882 RVA: 0x001C63AC File Offset: 0x001C45AC
		public float ConvertToCombatDeltaV_kps(float cruiseDeltaV_kps)
		{
			return this.maxThrust_combatExhaustVelocity_kps * (cruiseDeltaV_kps / this.currentEV_kps);
		}

		// Token: 0x060045DB RID: 17883 RVA: 0x001C63BD File Offset: 0x001C45BD
		public bool MissionKilled()
		{
			return this.fleet.inTransfer && this.fleet.DVRequiredToCompleteTrajectory_kps() > this.currentDeltaV_kps;
		}

		// Token: 0x060045DC RID: 17884 RVA: 0x001C63E2 File Offset: 0x001C45E2
		public void JoinFleet(TISpaceFleetState newFleet)
		{
			this.fleet = newFleet;
		}

		// Token: 0x17000DC8 RID: 3528
		// (get) Token: 0x060045DD RID: 17885 RVA: 0x001C63EB File Offset: 0x001C45EB
		public Vector3d defaultPositionOnCreation
		{
			get
			{
				return new Vector3d((float)(this.fleet.ships.IndexOf(this) * 100), 0f, 0f);
			}
		}

		// Token: 0x060045DE RID: 17886 RVA: 0x001C6414 File Offset: 0x001C4614
		public Vector3d GetShipFormationOffSet(int numberOfPositions, bool invertZ = false)
		{
			Dictionary<TISpaceShipState, Vector3d> dictionary = this.fleet.formation.pattern.RelativeShipPositions_Units(this.fleet.ships, this.fleet.formation, numberOfPositions, invertZ);
			Vector3d vector3d = Vector3.zero;
			if (dictionary.ContainsKey(this))
			{
				vector3d = dictionary[this];
			}
			else
			{
				Debug.LogError("Shipstate key for " + base.ID.ToString() + " not present in fleet formation for fleet " + this.fleet.ID.ToString());
			}
			return new Vector3d(vector3d.x * TIFormationTemplate.GetSpacingOffset_km(false, true)[(int)this.fleet.formation.spacing].x, vector3d.y * TIFormationTemplate.GetSpacingOffset_km(false, true)[(int)this.fleet.formation.spacing].y, vector3d.z * TIFormationTemplate.GetSpacingOffset_km(false, true)[(int)this.fleet.formation.spacing].z);
		}

		// Token: 0x060045DF RID: 17887 RVA: 0x001C6530 File Offset: 0x001C4730
		public void SetFormationOffsetAndInitiateStationkeepingManeuver(int numberOfPositions, bool invertZ)
		{
			this.fleetFormationOffset = this.GetShipFormationOffSet(numberOfPositions, invertZ);
			Vector3d fleetFormationOffset = this.fleetFormationOffset;
			if ((in this.currentFleetOffset) != (in fleetFormationOffset))
			{
				this.EndManuever();
				this.InitiateManuever(this.fleetFormationOffset, Quaternion.identity);
				this.SetVisualizationDataDirty();
			}
		}

		// Token: 0x060045E0 RID: 17888 RVA: 0x001C6580 File Offset: 0x001C4780
		public Vector3d GetCombatFormationOffSet(List<TISpaceShipState> shipsInFormation, Formation formation, int numberOfPositions, bool invertZForCombat = false, bool isCombatSetup = false)
		{
			Vector3d vector3d = formation.pattern.RelativeShipPositions_Units(shipsInFormation, formation, numberOfPositions, invertZForCombat)[this];
			return new Vector3d(vector3d.x * TIFormationTemplate.GetSpacingOffset_km(isCombatSetup, false)[(int)formation.spacing].x, vector3d.y * TIFormationTemplate.GetSpacingOffset_km(isCombatSetup, false)[(int)formation.spacing].y, vector3d.z * TIFormationTemplate.GetSpacingOffset_km(isCombatSetup, false)[(int)formation.spacing].z);
		}

		// Token: 0x060045E1 RID: 17889 RVA: 0x001C6607 File Offset: 0x001C4807
		public void SetCombatFormationOffset(List<TISpaceShipState> shipsInFormation, Formation formation, int numberOfPositions, bool invertZForCombat = false, bool isCombatSetup = false)
		{
			this.fleetFormationOffset = this.GetCombatFormationOffSet(shipsInFormation, formation, numberOfPositions, invertZForCombat, isCombatSetup);
		}

		// Token: 0x060045E2 RID: 17890 RVA: 0x001C661C File Offset: 0x001C481C
		public Quaterniond GetDesiredRotation(bool realspace)
		{
			if (this.fleet.inTransfer)
			{
				if (this.fleet.inAccelerationPhase)
				{
					if (!realspace)
					{
						return Quaterniond.LookRotation(this.fleet.trajectory.DesiredOrientationVector_Acceleration().xzy);
					}
					return Quaterniond.LookRotation(this.fleet.trajectory.DesiredOrientationVector_Acceleration());
				}
				else if (this.fleet.inDecelerationPhase)
				{
					if (!realspace)
					{
						return Quaterniond.LookRotation(this.fleet.trajectory.DesiredOrientationVector_Deceleration().xzy);
					}
					return Quaterniond.LookRotation(this.fleet.trajectory.DesiredOrientationVector_Deceleration());
				}
			}
			else if (this.fleet.bombarding)
			{
				if (!realspace)
				{
					return Quaterniond.LookRotation(TISpaceShipState.BombardmentTargetGlobalPosition(this.fleet.bombardmentTarget, TITimeState.Now()).xzy - this.fleet.GetGlobalPosition().xzy);
				}
				return Quaterniond.LookRotation(TISpaceShipState.BombardmentTargetGlobalPosition(this.fleet.bombardmentTarget, TITimeState.Now()) - this.fleet.GetGlobalPosition());
			}
			return new Quaterniond(this.fleet.ships[0].currentRotation);
		}

		// Token: 0x060045E3 RID: 17891 RVA: 0x001C675C File Offset: 0x001C495C
		public void InitiateManuever(Vector3d desiredOffset, Quaternion desiredOrientation)
		{
			if (!this.inManeuver && ((in desiredOffset) != (in this.currentFleetOffset) || desiredOrientation != this.currentRotation))
			{
				TIDateTime tidateTime = TITimeState.Now();
				TIDateTime tidateTime2 = TITimeState.Now();
				float num = (float)Mathd.Sqrt(Vector3d.Distance(in desiredOffset, in this.currentFleetOffset) / this.sideways_acceleration);
				float num2 = (float)Mathd.Sqrt((double)(Quaternion.Angle(this.currentRotation, desiredOrientation) / this.angular_acceleration_rads2));
				tidateTime2.AddSeconds((double)Mathf.Max(num, num2));
				this.currentManeuver = new StratManeuver
				{
					startingOffset = this.currentFleetOffset,
					desiredOffset = desiredOffset,
					startingOrientation = this.currentRotation,
					desiredOrientation = desiredOrientation,
					maneuverStart = tidateTime,
					maneuverFinish = new TIDateTime(tidateTime2)
				};
				this.inManeuver = true;
			}
		}

		// Token: 0x060045E4 RID: 17892 RVA: 0x001C683C File Offset: 0x001C4A3C
		public void InitiateManeuverSequence(Vector3d driftTarget, Vector3d burnTarget, Vector3d desiredOffset, Quaternion desiredOrientation)
		{
			if (!this.inManeuver && ((in desiredOffset) != (in this.currentFleetOffset) || desiredOrientation != this.currentRotation))
			{
				ProposedWaypoint proposedWaypoint = new ProposedWaypoint
				{
					Timing = TITimeState.Now(),
					Position = (Vector3)this.currentFleetOffset,
					Rotation = this.currentRotation,
					Velocity = Vector3.zero
				};
				ProposedWaypoint proposedWaypoint2 = new ProposedWaypoint
				{
					Position = (Vector3)desiredOffset,
					Rotation = desiredOrientation,
					Velocity = Vector3.zero
				};
				this.currentManeuverSequence.CreateManeuverSequence(proposedWaypoint, (Vector3)driftTarget, (Vector3)burnTarget, proposedWaypoint2);
				this.inManeuverSequence = true;
				this.inManeuver = true;
			}
		}

		// Token: 0x060045E5 RID: 17893 RVA: 0x001C68FC File Offset: 0x001C4AFC
		public int Maneuver_Phase(TIDateTime time)
		{
			double num = time.DifferenceInSeconds(this.currentManeuver.maneuverStart) / this.currentManeuver.maneuverFinish.DifferenceInSeconds(this.currentManeuver.maneuverStart);
			if (num < 0.25)
			{
				return 0;
			}
			if (num < 0.5)
			{
				return 1;
			}
			if (num < 0.75)
			{
				return 2;
			}
			return 3;
		}

		// Token: 0x060045E6 RID: 17894 RVA: 0x001C6964 File Offset: 0x001C4B64
		public Quaternion Maneuver_RotationAtTime(TIDateTime time)
		{
			float num = (float)(time.DifferenceInSeconds(this.currentManeuver.maneuverStart) / this.currentManeuver.maneuverFinish.DifferenceInSeconds(this.currentManeuver.maneuverStart));
			if (num > 0f)
			{
				return Quaternion.Slerp(this.currentManeuver.startingOrientation, this.currentManeuver.desiredOrientation, num);
			}
			if (num >= 1f)
			{
				return this.currentManeuver.desiredOrientation;
			}
			return this.currentManeuver.startingOrientation;
		}

		// Token: 0x060045E7 RID: 17895 RVA: 0x001C69E4 File Offset: 0x001C4BE4
		public Vector3d Maneuver_PositionAtTime(TIDateTime time)
		{
			return Vector3d.Lerp(this.currentManeuver.startingOffset, this.currentManeuver.desiredOffset, this.CurrentManeuverCompletePercentage(time));
		}

		// Token: 0x060045E8 RID: 17896 RVA: 0x001C6A08 File Offset: 0x001C4C08
		public void EndManuever()
		{
			this.currentManeuver = default(StratManeuver);
			this.inManeuver = false;
			this.inManeuverSequence = false;
		}

		// Token: 0x060045E9 RID: 17897 RVA: 0x001C6A24 File Offset: 0x001C4C24
		public void UpdateCurrentManeuver()
		{
			TIDateTime tidateTime = TITimeState.Now();
			if (!this.inManeuverSequence)
			{
				if (this.inManeuver)
				{
					this.currentFleetOffset = this.Maneuver_PositionAtTime(tidateTime);
					this.currentRotation = this.Maneuver_RotationAtTime(tidateTime);
					if (tidateTime >= this.currentManeuver.maneuverFinish)
					{
						this.EndManuever();
						this.SetVisualizationDataDirty();
					}
				}
				return;
			}
			if (this.currentManeuverSequence.End.Timing > tidateTime)
			{
				Vector3 vector;
				Quaternion quaternion;
				this.currentManeuverSequence.PositionAndRotationAt(tidateTime, out vector, out quaternion);
				this.currentFleetOffset = vector;
				this.currentRotation = quaternion;
				return;
			}
			this.EndManuever();
			this.SetVisualizationDataDirty();
		}

		// Token: 0x060045EA RID: 17898 RVA: 0x001C6ACC File Offset: 0x001C4CCC
		public double CurrentManeuverCompletePercentage(TIDateTime time)
		{
			if (this.inManeuver)
			{
				double num = this.currentManeuver.maneuverFinish.DifferenceInSeconds(this.currentManeuver.maneuverStart);
				if (num > 0.0)
				{
					return Mathd.Clamp(time.DifferenceInSeconds(this.currentManeuver.maneuverStart) / num, 0.0, 1.0);
				}
			}
			return 1.0;
		}

		// Token: 0x17000DC9 RID: 3529
		// (get) Token: 0x060045EB RID: 17899 RVA: 0x001C6B3D File Offset: 0x001C4D3D
		public List<IShipCommand> visibleCommands
		{
			get
			{
				return ShipCommandsManager.shipCommands.Where<IShipCommand>((IShipCommand x) => x.CommandVisibleToActor(this)).ToList<IShipCommand>();
			}
		}

		// Token: 0x17000DCA RID: 3530
		// (get) Token: 0x060045EC RID: 17900 RVA: 0x001C6B5A File Offset: 0x001C4D5A
		public List<IShipCommand> availableCommands
		{
			get
			{
				return this.visibleCommands.Where<IShipCommand>((IShipCommand x) => x.ActorCanPerformCommand(this)).ToList<IShipCommand>();
			}
		}

		// Token: 0x060045ED RID: 17901 RVA: 0x001C6B78 File Offset: 0x001C4D78
		public TIResourcesCost GetRammingSpeedCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (GameControl.control.skirmishMode)
			{
				return tiresourcesCost;
			}
			tiresourcesCost.AddCost(FactionResource.Influence, TemplateManager.global.influenceCostBaseForRammingSpeed / (1f + Mathf.Abs(this.faction.ideologyCoordinates.x)), true);
			return tiresourcesCost;
		}

		// Token: 0x060045EE RID: 17902 RVA: 0x001C6BC8 File Offset: 0x001C4DC8
		public void SetRammingSpeed(bool enabled)
		{
			this.canSuicide = enabled;
		}

		// Token: 0x060045EF RID: 17903 RVA: 0x001C6BD1 File Offset: 0x001C4DD1
		public void SetDisengageOrder(bool enabled)
		{
			this.disengageFromCombat = enabled;
		}

		// Token: 0x060045F0 RID: 17904 RVA: 0x001C6BDA File Offset: 0x001C4DDA
		public void CompleteDisengage()
		{
			this.hasDisengaged = true;
			GameControl.eventManager.TriggerEvent(new ShipRetreatsFromCombat(this), null, Array.Empty<object>());
		}

		// Token: 0x060045F1 RID: 17905 RVA: 0x001C6BF9 File Offset: 0x001C4DF9
		public void SetAIControl(bool setting)
		{
			this.combatAIControl = setting;
			GameControl.eventManager.TriggerEvent(new ShipAIControlChange(this, setting), null, new object[] { this });
		}

		// Token: 0x060045F2 RID: 17906 RVA: 0x001C6C1E File Offset: 0x001C4E1E
		public void SetCombatPrimaryTarget(CombatTargetableState target)
		{
			this.combatPrimaryTarget = target;
			GameControl.eventManager.TriggerEvent(new ShipPrimaryTargetSelected(this, target), null, new object[] { this });
		}

		// Token: 0x060045F3 RID: 17907 RVA: 0x001C6C43 File Offset: 0x001C4E43
		public void SetCombatManeuverTarget(CombatTargetableState maneuverTarget)
		{
			this.combatManeuverTarget = maneuverTarget;
			GameControl.eventManager.TriggerEvent(new ShipManeuverTargetSelected(this, maneuverTarget), null, new object[] { this });
		}

		// Token: 0x17000DCB RID: 3531
		// (get) Token: 0x060045F4 RID: 17908 RVA: 0x001C6C68 File Offset: 0x001C4E68
		public float availablePowerFraction
		{
			get
			{
				return (this.availablePower_GJ + this.currentBatteryCharge_GJ) / (this._auxPowerRequriedStorage_GJ + this.template.BatteryCapacity_GJ(false));
			}
		}

		// Token: 0x17000DCC RID: 3532
		// (get) Token: 0x060045F5 RID: 17909 RVA: 0x001C6C8B File Offset: 0x001C4E8B
		public float availablePowerStorageFraction
		{
			get
			{
				return (this._auxPowerRequriedStorage_GJ + this.CurrentBatteryCapacity_GJ()) / (this._auxPowerRequriedStorage_GJ + this.template.BatteryCapacity_GJ(false));
			}
		}

		// Token: 0x060045F6 RID: 17910 RVA: 0x001C6CB0 File Offset: 0x001C4EB0
		public void CombatPerSecondChanges(bool triggerUIUpdate)
		{
			this.ApplyHeat(this._systemsPowerGenerationRequirement_GW * (1f - this.powerPlant.efficiency), triggerUIUpdate);
			float num = this.CurrentBatteryCapacity_GJ() - this.currentBatteryCharge_GJ;
			if (num > 0f)
			{
				float num2 = Mathf.Min(this.availablePower_GJ * this.GetSystemFunction(ShipSystem.PowerCoupling), num);
				this.ChangeAvailablePower(-this.ChangeBatteryCharge(num2, triggerUIUpdate), triggerUIUpdate);
			}
			if (this.combatSecondCounter % 15 == 0 && this.internalDamage)
			{
				this.DamageControl();
			}
			this.combatSecondCounter++;
		}

		// Token: 0x060045F7 RID: 17911 RVA: 0x001C6D3E File Offset: 0x001C4F3E
		public void CombatFractionalSecondChanges(double timeElapsed_s)
		{
			this.BleedHeatFromRadiators_s(timeElapsed_s, true);
			this.ResolveHeatInSinks();
		}

		// Token: 0x17000DCD RID: 3533
		// (get) Token: 0x060045F8 RID: 17912 RVA: 0x001C6D4E File Offset: 0x001C4F4E
		public float AuxPowerRequriedStorage_GJ
		{
			get
			{
				return this._auxPowerRequriedStorage_GJ;
			}
		}

		// Token: 0x060045F9 RID: 17913 RVA: 0x001C6D58 File Offset: 0x001C4F58
		private void CacheInternalPowerStats()
		{
			this._systemsPowerGenerationRequirement_GW = this.template.requiredSystemsPower_GW / this.powerPlant.efficiency;
			this._weaponsPowerGenerationRequirement_GW = this.template.requiredWeaponsPowerGeneration_GW / this.powerPlant.efficiency;
			this._auxReactorPowerGenerationRequirement_GW = this._systemsPowerGenerationRequirement_GW + this._weaponsPowerGenerationRequirement_GW;
			this._propulsionPowerGenerationRequirement_GW = this.template.drivePowerRequirement_GW;
			this._allPowerGenerationRequirement_GW = this._auxReactorPowerGenerationRequirement_GW + this._propulsionPowerGenerationRequirement_GW;
			this._auxPowerRequriedStorage_GJ = this.template.requiredWeaponsPowerStorage_GJ + this.template.requiredSystemsPower_GW;
			this.availablePower_GJ = this._auxPowerRequriedStorage_GJ;
			this._wasteHeat_GW = this.template.wasteHeat_GW;
		}

		// Token: 0x17000DCE RID: 3534
		// (get) Token: 0x060045FA RID: 17914 RVA: 0x001C6E10 File Offset: 0x001C5010
		public float WasteHeat_GW
		{
			get
			{
				return this._wasteHeat_GW;
			}
		}

		// Token: 0x060045FB RID: 17915 RVA: 0x001C6E18 File Offset: 0x001C5018
		public float PerSecondPowerGain()
		{
			float num = 0f;
			float num2 = this.GetSystemFunction(ShipSystem.SystemsReactor) * this.GetSystemFunction(ShipSystem.PowerCoupling) * this._auxReactorPowerGenerationRequirement_GW;
			float num3 = 0f;
			num += num2;
			switch (this.drive.powerGen)
			{
			case PowerGenerationType.Always:
				num3 = this.<PerSecondPowerGain>g__propulsionPowerGain|432_0();
				break;
			case PowerGenerationType.DriveIdle:
				if (!this.thrustersActive)
				{
					num3 = this.<PerSecondPowerGain>g__propulsionPowerGain|432_0();
				}
				break;
			case PowerGenerationType.DriveActive:
				if (this.thrustersActive)
				{
					num3 = this.<PerSecondPowerGain>g__propulsionPowerGain|432_0();
				}
				break;
			}
			return num + num3;
		}

		// Token: 0x060045FC RID: 17916 RVA: 0x001C6E9C File Offset: 0x001C509C
		private void ChangeAvailablePower(float byAmount, bool triggerUIUpdate)
		{
			float num = this.availablePower_GJ;
			this.availablePower_GJ = Mathf.Min(this.availablePower_GJ + byAmount, this._auxPowerRequriedStorage_GJ);
			if (this.availablePower_GJ < 0f)
			{
				this.ChangeBatteryCharge(this.availablePower_GJ, false);
				if (this.currentBatteryCharge_GJ <= 0f)
				{
					this.systemsDepowered = true;
				}
				else
				{
					this.systemsDepowered = false;
				}
				this.availablePower_GJ = 0f;
			}
			else
			{
				this.systemsDepowered = false;
			}
			if (num != this.availablePower_GJ && triggerUIUpdate)
			{
				GameControl.eventManager.TriggerEvent(new ShipPowerSystemsChargeChange(this), null, new object[] { this });
			}
		}

		// Token: 0x060045FD RID: 17917 RVA: 0x001C6F40 File Offset: 0x001C5140
		private void SetAvailablePower(float toAmount, bool triggerUIUpdate)
		{
			float num = toAmount - this.availablePower_GJ;
			this.ChangeAvailablePower(num, triggerUIUpdate);
		}

		// Token: 0x060045FE RID: 17918 RVA: 0x001C6F60 File Offset: 0x001C5160
		public void CombatPerQuarterSecondChanges(bool final)
		{
			if (this.CanGainHeat)
			{
				float num = this.availablePower_GJ;
				float num2 = this.PerSecondPowerGain();
				num2 *= 0.25f;
				this.ChangeAvailablePower(num2, false);
				if (this.availablePower_GJ > num)
				{
					this.ApplyHeat((this.availablePower_GJ - num) * (1f - this.powerPlant.efficiency), final);
					this.generatorWorking = true;
				}
				else
				{
					this.generatorWorking = false;
				}
				this.ChangeAvailablePower(-this._systemsPowerGenerationRequirement_GW * this.powerPlant.efficiency * 0.25f, final);
			}
		}

		// Token: 0x060045FF RID: 17919 RVA: 0x001C6FED File Offset: 0x001C51ED
		public void AddCombatManeuver(CombatManeuver maneuver)
		{
			if (!this.activeCombatManeuvers.Contains(maneuver))
			{
				this.activeCombatManeuvers.Add(maneuver);
			}
		}

		// Token: 0x06004600 RID: 17920 RVA: 0x001C7009 File Offset: 0x001C5209
		public bool PerformingCombatManeuver()
		{
			return this.activeCombatManeuvers.Any<CombatManeuver>();
		}

		// Token: 0x06004601 RID: 17921 RVA: 0x001C7016 File Offset: 0x001C5216
		public bool PerformingCombatManeuver(CombatManeuver maneuver)
		{
			return this.activeCombatManeuvers.Contains(maneuver);
		}

		// Token: 0x06004602 RID: 17922 RVA: 0x001C7024 File Offset: 0x001C5224
		public bool PerformingCombatManeuver(List<CombatManeuver> maneuvers)
		{
			return this.activeCombatManeuvers.Intersect<CombatManeuver>(maneuvers).Any<CombatManeuver>();
		}

		// Token: 0x06004603 RID: 17923 RVA: 0x001C7037 File Offset: 0x001C5237
		public bool Rolling()
		{
			return this.activeCombatManeuvers.Contains(CombatManeuver.Roll180) || this.activeCombatManeuvers.Contains(CombatManeuver.Roll90Port) || this.activeCombatManeuvers.Contains(CombatManeuver.Roll90Starboard);
		}

		// Token: 0x06004604 RID: 17924 RVA: 0x001C7063 File Offset: 0x001C5263
		public void RemoveCombatManeuver(CombatManeuver maneuver)
		{
			this.activeCombatManeuvers.Remove(maneuver);
			GameControl.eventManager.TriggerEvent(new CombatManeuverComplete(this), null, new object[] { this });
		}

		// Token: 0x06004605 RID: 17925 RVA: 0x001C7090 File Offset: 0x001C5290
		public float GetWorstArmor()
		{
			if (this._worstArmor == -1)
			{
				this._worstArmor = this.armor.Values.Min<TISpaceShipState.ArmorData>((TISpaceShipState.ArmorData x) => x.armorValue);
			}
			return (float)this._worstArmor;
		}

		// Token: 0x06004606 RID: 17926 RVA: 0x001C70E4 File Offset: 0x001C52E4
		public float GetBestArmor()
		{
			if (this._bestArmor == -1)
			{
				this._bestArmor = this.armor.Values.Max<TISpaceShipState.ArmorData>((TISpaceShipState.ArmorData x) => x.armorValue);
			}
			return (float)this._bestArmor;
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x001C7136 File Offset: 0x001C5336
		public void RecordKill(TIShipHullTemplate hull)
		{
			this.kills.Add(hull.dataName);
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x001C7149 File Offset: 0x001C5349
		public void SetCombatSystems()
		{
			this.ChargeBatteriesToMax();
			this.SetAvailablePower(this._auxPowerRequriedStorage_GJ, true);
			this.generatorWorking = false;
			this.systemsDepowered = false;
			this.UpdateHeatSinkCapacity_GJ();
		}

		// Token: 0x06004609 RID: 17929 RVA: 0x001C7174 File Offset: 0x001C5374
		public void EnterCombat()
		{
			this.SetCombatSystems();
			this.activeCombatManeuvers = new List<CombatManeuver>();
			this.DeactivateThrusters();
			this.combatPrimaryTarget = null;
			this.canSuicide = false;
			this.disengageFromCombat = false;
			this.hasDisengaged = false;
			this.combatSecondCounter = 0;
			this.combatAIControl = this.faction.player.isAI;
			this._combatUpdatePropulsionEventName = new StringBuilder("UpdateShipPropulsionValues_Combat").Append(base.ID.ToString()).ToString();
			TITimeEvent.CreateNewTimeEvent(TITimeState.Now(), this, null, null, this._combatUpdatePropulsionEventName, true, false, TITimeQueueRepeatType.Minute, 1, true, true);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdatePropulsionValues_Combat), this._combatUpdatePropulsionEventName, null, false, false);
			GameControl.eventManager.AddListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null, this, false, false);
		}

		// Token: 0x0600460A RID: 17930 RVA: 0x001C7254 File Offset: 0x001C5454
		public void SetPartDamage(ModuleDataEntry module, float value, bool add = false)
		{
			DamagedShipPartData damagedShipPartData;
			this.damagedPartsCache.TryGetValue(module, out damagedShipPartData);
			float num = 0f;
			if (damagedShipPartData == null)
			{
				if (value > 0f)
				{
					damagedShipPartData = new DamagedShipPartData(module, Mathf.Clamp(value, 0f, 1f));
					this.damagedParts.Add(damagedShipPartData);
					this.damagedPartsCache[damagedShipPartData.module] = damagedShipPartData;
					num = damagedShipPartData.damage;
				}
			}
			else
			{
				float damage = damagedShipPartData.damage;
				if (add)
				{
					damagedShipPartData.damage += value;
					num = value;
				}
				else
				{
					damagedShipPartData.damage = value;
					num = damage - value;
				}
				damagedShipPartData.damage = Mathf.Clamp(damagedShipPartData.damage, 0f, 1f);
				if (damagedShipPartData.damage == 0f)
				{
					this.damagedParts.Remove(damagedShipPartData);
					if (this.damagedPartsCache.ContainsKey(damagedShipPartData.module))
					{
						this.damagedPartsCache.Remove(damagedShipPartData.module);
					}
				}
			}
			if (num != 0f)
			{
				if (damagedShipPartData.module.moduleTemplate.isDrive || damagedShipPartData.module.moduleTemplate.isPowerPlant || damagedShipPartData.module.moduleTemplate.isRadiator)
				{
					this.SetPropulsionValuesDirty(true, false);
				}
				else
				{
					TIUtilityModuleTemplate ref_utilityModule = damagedShipPartData.module.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule != null && ref_utilityModule.thrustMultiplier == (float)1)
					{
						TIUtilityModuleTemplate ref_utilityModule2 = damagedShipPartData.module.moduleTemplate.ref_utilityModule;
						if (ref_utilityModule2 != null && ref_utilityModule2.EVMultiplier == (float)1)
						{
							goto IL_0182;
						}
					}
					this.SetPropulsionValuesDirty(true, false);
				}
				IL_0182:
				if (damagedShipPartData.module.moduleTemplate.isHeatSink || damagedShipPartData.module.moduleTemplate.isRadiator)
				{
					this.UpdateHeatSinkCapacity_GJ();
				}
				else if (damagedShipPartData.module.moduleTemplate.isBattery && this.currentBatteryCharge_GJ > this.CurrentBatteryCapacity_GJ())
				{
					float num2 = this.currentBatteryCharge_GJ - this.CurrentBatteryCapacity_GJ();
					this.ChangeBatteryCharge(-num2, true);
				}
				if (this.isDummy)
				{
					return;
				}
				this._cachedFunctionalUtilitySlotModulesFrame = -1;
				if (damagedShipPartData.module.weaponTemplate != null && damagedShipPartData.module.weaponTemplate.hullWeapon)
				{
					if (damagedShipPartData.damage == 1f)
					{
						GameControl.eventManager.TriggerEvent(new ShipDestroyedWeaponExplosion(this, damagedShipPartData.module), null, new object[] { this });
					}
				}
				else if (damagedShipPartData.module.weaponTemplate != null && damagedShipPartData.module.weaponTemplate.noseWeapon && damagedShipPartData.damage == 1f)
				{
					GameControl.eventManager.TriggerEvent(new ShipDestroyedWeaponExplosion(this, damagedShipPartData.module), null, new object[] { this });
				}
				if (damagedShipPartData.module.moduleTemplate.isRadiator && damagedShipPartData.damage == 1f)
				{
					GameControl.eventManager.TriggerEvent(new ShipRadiatorDestroyed(this, damagedShipPartData.module), null, new object[] { this });
				}
				GameControl.eventManager.TriggerEvent(new ShipPartDamageChange(this, damagedShipPartData.module, num < 0f), null, new object[] { this });
			}
		}

		// Token: 0x0600460B RID: 17931 RVA: 0x001C755C File Offset: 0x001C575C
		public void PostCombat(bool allowRepairs = true)
		{
			if (allowRepairs)
			{
				List<ShipSystem> list = new List<ShipSystem>();
				foreach (KeyValuePair<ShipSystem, float> keyValuePair in this.damagedSystems)
				{
					if (!this.PartsDestroyedDuringOperation[keyValuePair.Key] && keyValuePair.Value < TISpaceShipState.LessThanDamageToRepairInCombat[keyValuePair.Key])
					{
						list.Add(keyValuePair.Key);
					}
				}
				foreach (ShipSystem shipSystem in list)
				{
					this.damagedSystems.Remove(shipSystem);
				}
				List<DamagedShipPartData> list2 = new List<DamagedShipPartData>();
				foreach (DamagedShipPartData damagedShipPartData in this.damagedParts)
				{
					ShipSystem systemTypeFromModuleData = this.GetSystemTypeFromModuleData(damagedShipPartData.module);
					if (!damagedShipPartData.module.moduleTemplate.noCombatRepair && !this.PartsDestroyedDuringOperation[systemTypeFromModuleData] && TISpaceShipState.LessThanDamageToRepairInCombat[systemTypeFromModuleData] < 1f)
					{
						list2.Add(damagedShipPartData);
					}
				}
				foreach (DamagedShipPartData damagedShipPartData2 in list2)
				{
					this.SetPartDamage(damagedShipPartData2.module, 0f, false);
				}
			}
			if (this.GetPartFunction(this.powerPlantModule) <= 0.050000012f)
			{
				this.SetPartDamage(this.powerPlantModule, (this.GetRepairBayBonusCrew() > 0f) ? 0.75f : 0.95f, false);
				this.PartsDestroyedDuringOperation[ShipSystem.PowerPlant] = true;
			}
			if (this.GetPartFunction(this.driveModule) <= 0.050000012f)
			{
				this.SetPartDamage(this.driveModule, (this.GetRepairBayBonusCrew() > 0f) ? 0.75f : 0.95f, false);
				this.PartsDestroyedDuringOperation[ShipSystem.Drive] = true;
			}
			if (this.GetPartFunction(this.radiatorModule) <= 0.050000012f)
			{
				this.SetPartDamage(this.radiatorModule, (this.GetRepairBayBonusCrew() > 0f) ? 0.75f : 0.95f, false);
				this.PartsDestroyedDuringOperation[ShipSystem.Radiators] = true;
				this.visualizerLink.ModelController.OnRadiatorRepaired();
			}
			if (this.GetSystemFunction(ShipSystem.VectorThrusters) <= 0.050000012f)
			{
				this.damagedSystems[ShipSystem.VectorThrusters] = ((this.GetRepairBayBonusCrew() > 0f) ? 0.75f : 0.95f);
				this.PartsDestroyedDuringOperation[ShipSystem.VectorThrusters] = true;
				GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, ShipSystem.VectorThrusters, true), null, new object[] { this });
			}
			if (this.GetRepairBayBonusCrew() > 0f)
			{
				if (this.GetSystemFunction(ShipSystem.DamageControl) < 0.25f)
				{
					this.damagedSystems[ShipSystem.DamageControl] = 0.75f;
					this.PartsDestroyedDuringOperation[ShipSystem.DamageControl] = true;
					GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, ShipSystem.DamageControl, true), null, new object[] { this });
				}
				if (this.GetSystemFunction(ShipSystem.Bridge) < 0.25f)
				{
					this.damagedSystems[ShipSystem.Bridge] = 0.75f;
					this.PartsDestroyedDuringOperation[ShipSystem.Bridge] = true;
					GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, ShipSystem.Bridge, true), null, new object[] { this });
				}
				if (this.GetSystemFunction(ShipSystem.LifeSupportMain) < 0.25f)
				{
					this.damagedSystems[ShipSystem.LifeSupportMain] = 0.75f;
					this.PartsDestroyedDuringOperation[ShipSystem.LifeSupportMain] = true;
					GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, ShipSystem.LifeSupportMain, true), null, new object[] { this });
				}
				if (this.GetSystemFunction(ShipSystem.LifeSupportBackup) < 0.25f)
				{
					this.damagedSystems[ShipSystem.LifeSupportBackup] = 0.75f;
					this.PartsDestroyedDuringOperation[ShipSystem.LifeSupportBackup] = true;
					GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, ShipSystem.LifeSupportBackup, true), null, new object[] { this });
				}
			}
			this.SetCombatSystems();
			this.DeactivateThrusters();
			this.UpdatePropulsionValues(true);
			this.accumulatedHeat_GJ = 0f;
			this.generatorWorking = false;
			this.systemsDepowered = false;
			this.combatPrimaryTarget = null;
			this.canSuicide = false;
			this.disengageFromCombat = false;
			this.hasDisengaged = false;
			this.combatAIControl = this.faction.player.isAI;
			this.ExtendRadiators();
			this.spaceCombatValueDataDirty = true;
		}

		// Token: 0x0600460C RID: 17932 RVA: 0x001C7A00 File Offset: 0x001C5C00
		public void PostCombatVis()
		{
			this.radiatorsRetracting = false;
			this.radiatorsExtending = false;
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteExtendRadiators), "Ship Extend Radiators Complete");
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteRetractRadiators), "Ship Retract Radiators Complete");
			this.activeCombatManeuvers.Clear();
			GameControl.eventManager.RemoveListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.UpdatePropulsionValues_Combat), this._combatUpdatePropulsionEventName);
			this.ClearRadiatorAudio();
			GameControl.eventManager.RemoveListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null);
		}

		// Token: 0x0600460D RID: 17933 RVA: 0x001C7A98 File Offset: 0x001C5C98
		private float GetRepairBayBonusCrew()
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Repair))
				{
					float num2 = (float)(this.hull.crew / 2) * (this.faction.IsAlienFaction ? 1.5f : 1f) * 0.5f;
					num2 += (float)moduleDataEntry.moduleTemplate.crew * (this.faction.IsAlienFaction ? 1.5f : 1f);
					num += num2 * this.GetPartFunction(moduleDataEntry);
				}
			}
			return num;
		}

		// Token: 0x0600460E RID: 17934 RVA: 0x001C7B74 File Offset: 0x001C5D74
		public void DamageControl()
		{
			if (this.isDamageControlSuspended)
			{
				return;
			}
			float repairBayBonusCrew = this.GetRepairBayBonusCrew();
			float num = (float)((int)Mathf.Ceil((float)this.hull.crew / 2f * (this.faction.IsAlienFaction ? 1.5f : 1f) + repairBayBonusCrew));
			float num2 = 0.0010416667f;
			float num3 = 0.045454547f;
			float num4 = num * num2 * this.GetSystemFunction(ShipSystem.DamageControl);
			float num5 = TIEffectsState.SumEffectsModifiers(Context.Combat_ShipRepairSpeed, this, num4, null);
			float num6 = this.SumOfficerEffectsModifiers(OfficerEffectType.DamageControlSpeed, num4);
			num4 += num5 + num6;
			int num7 = (int)Mathf.Ceil(num4 / num3);
			float num8 = num4 - (float)(num7 - 1) * num3;
			List<DamagedShipPartData> list = new List<DamagedShipPartData>();
			List<ShipSystem> list2 = new List<ShipSystem>();
			if (num4 > 0f)
			{
				IOrderedEnumerable<ShipSystem> orderedEnumerable = from y in this.damagedSystems.Keys.Where<ShipSystem>((ShipSystem x) => this.damagedSystems[x] < TISpaceShipState.LessThanDamageToRepairInCombat[x]).Intersect<ShipSystem>(TISpaceShipState.CombatSystemRepairPriority.Keys)
					orderby TISpaceShipState.CombatSystemRepairPriority[y]
					select y;
				for (int i = 0; i < orderedEnumerable.Count<ShipSystem>(); i++)
				{
					float num9 = ((num7 == 1) ? num8 : num3);
					if (num7 == 0)
					{
						break;
					}
					ShipSystem shipSystem = orderedEnumerable.ElementAt<ShipSystem>(i);
					if (this.PartsDestroyedDuringOperation[shipSystem])
					{
						float num10 = ((repairBayBonusCrew > 0f) ? 0.75f : 0.95f);
						if (this.damagedSystems[shipSystem] - num9 <= num10)
						{
							num9 = this.damagedSystems[shipSystem] - num10;
						}
						if (num9 == 0f)
						{
							break;
						}
					}
					if (shipSystem == ShipSystem.DriveCoupling || shipSystem == ShipSystem.PowerCoupling || shipSystem == ShipSystem.Sensors)
					{
						num9 *= 3.5f;
					}
					Dictionary<ShipSystem, float> dictionary = this.damagedSystems;
					ShipSystem shipSystem2 = shipSystem;
					dictionary[shipSystem2] -= num9;
					if (this.damagedSystems[shipSystem] <= 0f)
					{
						this.damagedSystems[shipSystem] = 0f;
						this.damagedSystems.Remove(shipSystem);
					}
					else
					{
						list2.Add(shipSystem);
					}
					if (shipSystem == ShipSystem.DriveCoupling || shipSystem == ShipSystem.PowerCoupling || shipSystem == ShipSystem.VectorThrusters)
					{
						this.SetPropulsionValuesDirty(true, false);
					}
					GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, shipSystem, true), null, new object[] { this });
					num7--;
				}
				IOrderedEnumerable<DamagedShipPartData> orderedEnumerable2 = from x in this.damagedParts
					where x.damage > 0f && x.damage < 1f && !x.module.moduleTemplate.noCombatRepair
					select x into y
					orderby TISpaceShipState.ModuleRepairPriority[y.module.moduleTemplate.allowedSlots[0]]
					select y;
				for (int j = 0; j < orderedEnumerable2.Count<DamagedShipPartData>(); j++)
				{
					float num11 = ((num7 == 1) ? num8 : num3);
					if (num7 == 0)
					{
						break;
					}
					DamagedShipPartData damagedShipPartData = orderedEnumerable2.ElementAt<DamagedShipPartData>(j);
					if (this.PartsDestroyedDuringOperation[this.GetSystemTypeFromModuleData(damagedShipPartData.module)])
					{
						float num12 = ((repairBayBonusCrew > 0f) ? 0.75f : 0.95f);
						if (damagedShipPartData.damage - num11 <= num12)
						{
							num11 = damagedShipPartData.damage - num12;
						}
						if (num11 == 0f)
						{
							break;
						}
					}
					this.SetPartDamage(damagedShipPartData.module, -num11, true);
					if (damagedShipPartData.damage >= 0f)
					{
						list.Add(damagedShipPartData);
					}
					num7--;
				}
			}
			foreach (ShipSystem shipSystem3 in list2)
			{
				if (!this.prevSystemsBeingRepaired.Contains(shipSystem3))
				{
					GameControl.eventManager.TriggerEvent(new ShipSystemBeingRepaired(this, shipSystem3), null, new object[] { this });
				}
			}
			foreach (ShipSystem shipSystem4 in this.prevSystemsBeingRepaired)
			{
				if (!list2.Contains(shipSystem4) || this.damagedSystems[shipSystem4] == 0f)
				{
					GameControl.eventManager.TriggerEvent(new ShipSystemNoLongerBeingRepaired(this, shipSystem4), null, new object[] { this });
				}
			}
			foreach (DamagedShipPartData damagedShipPartData2 in list)
			{
				if (!this.prevPartsBeingRepaired.Contains(damagedShipPartData2))
				{
					GameControl.eventManager.TriggerEvent(new ShipPartBeingRepaired(this, damagedShipPartData2.module), null, new object[] { this });
				}
			}
			foreach (DamagedShipPartData damagedShipPartData3 in this.prevPartsBeingRepaired)
			{
				if (!list.Contains(damagedShipPartData3) || damagedShipPartData3.damage == 0f)
				{
					GameControl.eventManager.TriggerEvent(new ShipPartNoLongerBeingRepaired(this, damagedShipPartData3.module), null, new object[] { this });
				}
			}
			this.prevSystemsBeingRepaired = list2;
			this.prevPartsBeingRepaired = list;
		}

		// Token: 0x0600460F RID: 17935 RVA: 0x001C80A8 File Offset: 0x001C62A8
		private void OnShipDamageControlRotationStatusChanged(ShipDamageControlRotationStatusChanged e)
		{
			this.isDamageControlSuspended = !e.damageControlEnabled;
		}

		// Token: 0x06004610 RID: 17936 RVA: 0x001C80B9 File Offset: 0x001C62B9
		public float GetLaserBonusPower_MJ()
		{
			return this.template.GetLaserBonusPower_MJ(new Func<ModuleDataEntry, float>(this.GetPartFunction));
		}

		// Token: 0x06004611 RID: 17937 RVA: 0x001C80D2 File Offset: 0x001C62D2
		public float GetParticleBonusPower_MJ()
		{
			return this.template.GetParticleBonusPower_MJ(new Func<ModuleDataEntry, float>(this.GetPartFunction));
		}

		// Token: 0x06004612 RID: 17938 RVA: 0x001C80EB File Offset: 0x001C62EB
		public float GetBonusPowerForWeapon_MJ(TIShipWeaponTemplate weapon)
		{
			return this.template.GetBonusPowerForWeapon_MJ(weapon, new Func<ModuleDataEntry, float>(this.GetPartFunction));
		}

		// Token: 0x06004613 RID: 17939 RVA: 0x001C8105 File Offset: 0x001C6305
		public float GetBonusPowerForWeapon_GJ(TIShipWeaponTemplate weapon)
		{
			return this.template.GetBonusPowerForWeapon_GJ(weapon, new Func<ModuleDataEntry, float>(this.GetPartFunction));
		}

		// Token: 0x06004614 RID: 17940 RVA: 0x001C811F File Offset: 0x001C631F
		public List<ModuleDataEntry> AllWeaponModuleData()
		{
			List<ModuleDataEntry> list = new List<ModuleDataEntry>(this.noseWeapons);
			list.AddRange(this.hullWeapons);
			return list;
		}

		// Token: 0x06004615 RID: 17941 RVA: 0x001C8138 File Offset: 0x001C6338
		public IEnumerable<ModuleDataEntry> AllModuleData()
		{
			return this.noseWeapons.Concat<ModuleDataEntry>(this.hullWeapons).Concat<ModuleDataEntry>(this.utilityModules).Append(this.driveModule)
				.Append(this.powerPlantModule)
				.Append(this.radiatorModule);
		}

		// Token: 0x06004616 RID: 17942 RVA: 0x001C8177 File Offset: 0x001C6377
		public List<ModuleDataEntry> NuclearWeaponModuleData()
		{
			return new List<ModuleDataEntry>(this.AllWeaponModuleData().Where<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIMissileTemplate ref_missileWeapon = x.moduleTemplate.ref_missileWeapon;
				if (ref_missileWeapon == null || ref_missileWeapon.warheadClass != WarheadClass.Nuclear)
				{
					TIMissileTemplate ref_missileWeapon2 = x.moduleTemplate.ref_missileWeapon;
					return ref_missileWeapon2 != null && ref_missileWeapon2.warheadClass == WarheadClass.ShapedNuclear;
				}
				return true;
			}));
		}

		// Token: 0x17000DCF RID: 3535
		// (get) Token: 0x06004617 RID: 17943 RVA: 0x001C81A8 File Offset: 0x001C63A8
		public float functionalMagazineModulesAmmoMultiplier
		{
			get
			{
				return (from x in this.GetFunctionalUtilitySlotModuleTemplates(1f)
					where x.specialModuleRules.Contains(SpecialModuleRule.Magazine)
					select x).Sum<TIUtilityModuleTemplate>((TIUtilityModuleTemplate x) => x.specialModuleValue);
			}
		}

		// Token: 0x06004618 RID: 17944 RVA: 0x001C8208 File Offset: 0x001C6408
		public void LoadAmmo()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData().Where<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIProjectileWeaponTemplate ref_projectileWeapon2 = x.moduleTemplate.ref_projectileWeapon;
				return ref_projectileWeapon2 != null && ref_projectileWeapon2.hasMagazine();
			}))
			{
				TIProjectileWeaponTemplate ref_projectileWeapon = moduleDataEntry.moduleTemplate.ref_projectileWeapon;
				if (!this.ammo.ContainsKey(moduleDataEntry))
				{
					this.ammo.Add(moduleDataEntry, ref_projectileWeapon.FullAmmoCount_Current(this));
				}
				else
				{
					this.ammo[moduleDataEntry] = ref_projectileWeapon.FullAmmoCount_Current(this);
				}
			}
		}

		// Token: 0x06004619 RID: 17945 RVA: 0x001C82B4 File Offset: 0x001C64B4
		public bool WeaponHasAmmo(ModuleDataEntry module)
		{
			return !module.moduleTemplate.ref_weapon.hasMagazine() || this.ammo[module] > 0;
		}

		// Token: 0x0600461A RID: 17946 RVA: 0x001C82D9 File Offset: 0x001C64D9
		public bool WeaponNeedsBatteries(float ask_GJ)
		{
			return this.SystemDestroyed(ShipSystem.PowerCoupling) || ask_GJ > this.availablePower_GJ;
		}

		// Token: 0x0600461B RID: 17947 RVA: 0x001C82F0 File Offset: 0x001C64F0
		public bool WeaponHasPower(ModuleDataEntry module)
		{
			TIShipWeaponTemplate ref_weapon = module.moduleTemplate.ref_weapon;
			if (ref_weapon.selfPowered)
			{
				return true;
			}
			float num = ref_weapon.EnergyUsage_GJ(this.GetBonusPowerForWeapon_GJ(ref_weapon));
			return !this.WeaponNeedsBatteries(num) || num <= this.availablePower_GJ + this.currentBatteryCharge_GJ;
		}

		// Token: 0x0600461C RID: 17948 RVA: 0x001C8340 File Offset: 0x001C6540
		public bool WeaponFireExceedsHeatCapacity(ModuleDataEntry module)
		{
			if (!this.radiatorsExtended || this.SystemDestroyed(ShipSystem.Radiators))
			{
				TIShipWeaponTemplate ref_weapon = module.moduleTemplate.ref_weapon;
				float num = ref_weapon.EnergyUsage_GJ(this.GetBonusPowerForWeapon_GJ(ref_weapon)) / this.powerPlant.efficiency;
				if (!ref_weapon.selfPowered && num + this.accumulatedHeat_GJ > this.currentHeatSinkCapacity_GJ)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600461D RID: 17949 RVA: 0x001C83A0 File Offset: 0x001C65A0
		public static bool LaserDownfiring(TIShipWeaponTemplate weapon, TISpaceCombatProjectileState targetedProjectile)
		{
			return weapon.isBeamWeapon && weapon.attackMode && targetedProjectile != null && (targetedProjectile.originWeapon.isMissileWeapon || targetedProjectile.originWeapon.isNavalGunWeapon);
		}

		// Token: 0x0600461E RID: 17950 RVA: 0x001C83D8 File Offset: 0x001C65D8
		public void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile)
		{
			TIShipWeaponTemplate ref_weapon = module.moduleTemplate.ref_weapon;
			if (ref_weapon.hasMagazine())
			{
				this.ChangeAmmoValue(module, -1);
			}
			float bonusPowerForWeapon_GJ = this.GetBonusPowerForWeapon_GJ(ref_weapon);
			float num;
			if (TISpaceShipState.LaserDownfiring(ref_weapon, targetedProjectile))
			{
				num = ref_weapon.EnergyUsage_GJ(bonusPowerForWeapon_GJ) / 20f;
			}
			else
			{
				num = ref_weapon.EnergyUsage_GJ(bonusPowerForWeapon_GJ);
			}
			if (!ref_weapon.selfPowered)
			{
				this.ChangeAvailablePower(-num, false);
				this.ApplyHeat(ref_weapon.HeatGeneration_GJ(bonusPowerForWeapon_GJ), true);
			}
			GameControl.eventManager.TriggerEvent(new ShipWeaponFired(this, module), null, new object[]
			{
				this,
				this.fleet.faction
			});
			if (targetedProjectile != null)
			{
				this.AddTargetedProjectile(targetedProjectile);
			}
		}

		// Token: 0x0600461F RID: 17951 RVA: 0x001C8484 File Offset: 0x001C6684
		public void ChangeAmmoValue(ModuleDataEntry module, int delta)
		{
			Dictionary<ModuleDataEntry, int> ammo = this.ammo;
			ammo[module] += delta;
			if (this.ammo[module] == 0)
			{
				this.spaceCombatValueDataDirty = true;
				GameControl.eventManager.TriggerEvent(new ShipWeaponOutOfAmmo(this, module), null, Array.Empty<object>());
			}
		}

		// Token: 0x06004620 RID: 17952 RVA: 0x001C84D8 File Offset: 0x001C66D8
		public static float BombardmentTargetRadiusValue_gameUnits(TIGameState target)
		{
			if (target.ref_habSite != null)
			{
				return target.ref_habSite.radius_gameUnits;
			}
			if (target.ref_region != null)
			{
				return target.ref_spaceBody.radius_gameUnits;
			}
			Log.Error("Bad target passed to BombardmentTargetRadiusValue_gameUnits: " + target.displayName, Array.Empty<object>());
			return -1f;
		}

		// Token: 0x06004621 RID: 17953 RVA: 0x001C8538 File Offset: 0x001C6738
		public static Vector3d BombardmentTargetGlobalPosition(TIGameState state, TIDateTime time)
		{
			if (state.ref_habSite != null)
			{
				return state.ref_habSite.GlobalPosition(time);
			}
			if (state.ref_region != null)
			{
				return state.ref_region.GetGlobalPosition(time);
			}
			return default(Vector3d);
		}

		// Token: 0x06004622 RID: 17954 RVA: 0x001C8584 File Offset: 0x001C6784
		public static bool BombardmentTargetInLineOfSight(TISpaceShipState ship, TIGameState target, TIDateTime time)
		{
			if (target == null || target.deleted)
			{
				return false;
			}
			float num = 90f;
			TISpaceBodyState ref_spaceBody = ship.fleet.ref_orbit.ref_spaceBody;
			Vector3d globalPositionAtTime = ref_spaceBody.GetGlobalPositionAtTime(time);
			Vector3d vector3d = TISpaceShipState.BombardmentTargetGlobalPosition(target, time);
			Vector3d globalPositionAtTime2 = ship.fleet.GetGlobalPositionAtTime(time);
			TIDateTime tidateTime = new TIDateTime(time);
			tidateTime.AddHours(1.0);
			Vector3d vector3d2 = ship.fleet.GetGlobalPositionAtTime(tidateTime) - ref_spaceBody.GetGlobalPositionAtTime(tidateTime);
			Vector3d normalized = Vector3d.Cross(globalPositionAtTime2 - globalPositionAtTime, vector3d2).normalized;
			Vector3d normalized2 = (vector3d - globalPositionAtTime).normalized;
			Vector3d vector3d3 = normalized2 - normalized * Vector3d.Dot(in normalized2, in normalized);
			Vector3d vector3d4 = globalPositionAtTime + vector3d3.normalized * ref_spaceBody.meanRadius_m;
			Vector3d vector3d5 = globalPositionAtTime2 - vector3d4;
			return (float)((int)Vector3d.Angle(in vector3d3, in vector3d5)) <= num;
		}

		// Token: 0x06004623 RID: 17955 RVA: 0x001C8684 File Offset: 0x001C6884
		public void Bombard(ModuleDataEntry bombardingWeaponModule, TIDateTime time, out bool targetDestroyed, out bool targetHit, bool doNotVisualize, float projectileVPDRatio)
		{
			targetDestroyed = false;
			TIGameState bombardmentTarget = this.fleet.bombardmentTarget;
			if (this.fleet.bombarding && bombardmentTarget.ref_faction != this.faction)
			{
				TISpaceShipState.<>c__DisplayClass480_0 CS$<>8__locals1 = new TISpaceShipState.<>c__DisplayClass480_0();
				CS$<>8__locals1.<>4__this = this;
				targetHit = true;
				CS$<>8__locals1.weapon = bombardingWeaponModule.moduleTemplate.ref_weapon;
				float num = 0f;
				bool isSpaceAssetState = bombardmentTarget.isSpaceAssetState;
				float num2;
				switch (CS$<>8__locals1.weapon.weaponClass)
				{
				case WeaponClass.Laser:
				case WeaponClass.Particle:
				{
					DamageBreakdown damageBreakdown = CS$<>8__locals1.weapon.DamageAtRange_points(this.fleet.bombardmentAltitude_km, 2827.4333f, this, 0f, 0f, true);
					if (isSpaceAssetState)
					{
						num2 = damageBreakdown.directDamage_Points * bombardmentTarget.ref_spaceBody.LaserEffectivenessFactorThroughAtmo();
						num = damageBreakdown.chippingDamage_Points * bombardmentTarget.ref_spaceBody.LaserEffectivenessFactorThroughAtmo();
						goto IL_01C9;
					}
					num2 = Mathf.Max(damageBreakdown.directDamage_Points, damageBreakdown.directDamage_Points * 0.5f + damageBreakdown.chippingDamage_Points * 0.5f);
					num2 *= bombardmentTarget.ref_spaceBody.LaserEffectivenessFactorThroughAtmo();
					goto IL_01C9;
				}
				}
				DamageBreakdown damageBreakdown2 = CS$<>8__locals1.weapon.DamageAtRange_points(this.fleet.bombardmentAltitude_km, 2827.4333f, this, CS$<>8__locals1.weapon.ref_projectileWeapon.GetSurfaceImpactVelocity_kps(bombardmentTarget.ref_spaceBody, this.fleet.bombardmentAltitude_km), CS$<>8__locals1.weapon.ref_projectileWeapon.warheadMass_kg, true);
				if (isSpaceAssetState && damageBreakdown2.directDamage_Points > 0f)
				{
					num2 = damageBreakdown2.directDamage_Points;
					num = damageBreakdown2.chippingDamage_Points;
				}
				else
				{
					num2 = Mathf.Max(damageBreakdown2.directDamage_Points, damageBreakdown2.directDamage_Points * 0.5f + damageBreakdown2.chippingDamage_Points * 0.5f);
				}
				IL_01C9:
				num2 += TIEffectsState.SumEffectsModifiers(Context.BombardmentDamageBonus, this.faction, num2, null);
				if (num2 <= 0.1f)
				{
					targetHit = false;
					return;
				}
				this.FireWeapon(bombardingWeaponModule, null);
				float num3;
				float num4;
				Transform transform;
				Transform transform2;
				Vector3 vector = TISpaceShipState.BombardmentTargetPosition_Display(this.fleet, time, out num3, out num4, out transform, out transform2);
				GameControl.eventManager.TriggerEvent(new FireMissionOrder(this, bombardmentTarget, bombardingWeaponModule, vector, num3, num4, transform, time, doNotVisualize), null, new object[] { this, this.fleet });
				float num5 = num2;
				string displayName = bombardmentTarget.GetDisplayName(this.faction);
				bool flag = false;
				bool flag2 = false;
				string text = string.Empty;
				bool flag3 = false;
				if (bombardmentTarget.ref_region != null)
				{
					if (CS$<>8__locals1.weapon.isProjectileWeapon && CS$<>8__locals1.weapon.ref_projectileWeapon.isPointDefenseTargetable && (bombardmentTarget.ref_region != null && (TISpaceDefensesFacilityState.STOShouldShootBack(bombardmentTarget.ref_region, bombardmentTarget) || bombardmentTarget.isRegionLandedUFO)) && TIUtilities.RandomFloatValue() < projectileVPDRatio)
					{
						num5 = 0f;
						flag3 = true;
					}
					if (num5 > 0f)
					{
						if (bombardmentTarget.isArmyState)
						{
							float num6;
							switch (CS$<>8__locals1.weapon.weaponClass)
							{
							default:
								num6 = 0.00125f;
								break;
							case WeaponClass.Magnetic:
								num6 = 0.001875f;
								break;
							case WeaponClass.Missile:
								if (CS$<>8__locals1.weapon.ref_missileWeapon.AOEWeapon)
								{
									num6 = 0.0062499996f;
								}
								else if (CS$<>8__locals1.weapon.ref_missileWeapon.warheadClass == WarheadClass.Explosive)
								{
									num6 = 0.001875f;
								}
								else
								{
									num6 = 0.00125f;
								}
								break;
							}
							TIArmyState ref_army = bombardmentTarget.ref_army;
							float num7;
							if (ref_army.AlienMegafaunaArmy)
							{
								num7 = ref_army.techLevel * (ref_army.IsFighting(false) ? TemplateManager.global.armyBombardmentDamageDivisor_InBattle : TemplateManager.global.armyBombardmentDamageDivisor_Dispersed) * 1f / Mathf.Max(0.1f, ref_army.strength);
							}
							else
							{
								num7 = ref_army.techLevel * (ref_army.IsFighting(false) ? TemplateManager.global.armyBombardmentDamageDivisor_InBattle : TemplateManager.global.armyBombardmentDamageDivisor_Dispersed) * 1f / ref_army.strength;
							}
							CS$<>8__locals1.<Bombard>g__CollateralDamage|0(ref_army.currentRegion, num5, 1f);
							num5 += TIEffectsState.SumEffectsModifiers(Context.BombardmentArmyDefenseBonus, ref_army.ref_faction, num5, null);
							targetDestroyed = ref_army.TakeDamage(Mathf.Min(num6, num5 / num7), this.fleet.ref_faction, null, true);
							if (targetDestroyed && ref_army.faction != null && !ref_army.faction.AI_AtWarWithFaction(this.fleet.ref_faction))
							{
								ref_army.faction.GainFactionHate(this.fleet.ref_faction, TemplateManager.global.factionHateForDestroyingArmyOutsideofWar, false, "Army Destroyed Outside of War", true);
							}
						}
						else if (bombardmentTarget.isRegionState)
						{
							bombardmentTarget.ref_region.ApplyDamageToRegion(Mathf.Min(0.05f, num5 / 10000f), this.faction, null, false, false, false, false);
						}
						else if (bombardmentTarget.isRegionSpaceFacility)
						{
							TIRegionSpaceFacilityState ref_regionSpaceFacility = bombardmentTarget.ref_regionSpaceFacility;
							switch (ref_regionSpaceFacility.spaceFacilityType)
							{
							case SpaceFacilityType.launchFacility:
								if (TIUtilities.RandomFloatValue() < num5 / 10f)
								{
									float num8 = Mathf.Max(ref_regionSpaceFacility.region.boostPerYear_dekatons * num5 / 100f, 0.1f);
									ref_regionSpaceFacility.region.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -num8, false, true);
								}
								else
								{
									num5 = 0f;
								}
								targetDestroyed = ref_regionSpaceFacility.region.boostPerMonth_dekatons <= 0f;
								break;
							case SpaceFacilityType.missionControlFacility:
								if (TIUtilities.RandomFloatValue() < num5 / 10f)
								{
									ref_regionSpaceFacility.region.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, -1f, false, true);
								}
								else
								{
									num5 = 0f;
								}
								targetDestroyed = ref_regionSpaceFacility.region.missionControl <= 0;
								break;
							case SpaceFacilityType.spaceDefenseFacility:
								if (TIUtilities.RandomFloatValue() < Mathf.Min(0.05f, num5 / 1000f))
								{
									ref_regionSpaceFacility.region.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, true);
								}
								else
								{
									num5 = 0f;
								}
								targetDestroyed = !ref_regionSpaceFacility.region.antiSpaceDefenses;
								break;
							}
							if (targetDestroyed)
							{
								ref_regionSpaceFacility.ref_factions.ForEach(delegate(TIFactionState x)
								{
									x.GainFactionHate(this.faction, TIFactionState.sabotageSpaceFacilityMission.hate[4], false, "Space Facility Bombarded and Destroyed", true);
								});
								this.faction.RegisterKill(bombardmentTarget, TIFactionState.sabotageSpaceFacilityMission.hate[4]);
							}
						}
						else if (bombardmentTarget.isRegionLandedUFO)
						{
							targetDestroyed = bombardmentTarget.ref_UFOLanding.Bombed(this.fleet, num5);
							if (targetDestroyed)
							{
								this.faction.RegisterKill(bombardmentTarget, 1f);
							}
						}
						else if (bombardmentTarget.isRegionAlienFacility)
						{
							targetDestroyed = bombardmentTarget.ref_alienFacility.Bombed(this.fleet, num5);
							if (targetDestroyed)
							{
								this.faction.RegisterKill(bombardmentTarget, 1f);
							}
						}
						else if (bombardmentTarget.isRegionXenoformingState && num5 > 0f)
						{
							GameControl.eventManager.TriggerEvent(new XenoformingDamaged(this.fleet.bombardmentTarget.ref_xenoforming), null, new object[] { this.fleet.bombardmentTarget.ref_xenoforming });
							CS$<>8__locals1.<Bombard>g__CollateralDamage|0(this.fleet.bombardmentTarget.ref_region, num5, this.fleet.bombardmentTarget.ref_xenoforming.xenoformingLevel);
							this.fleet.bombardmentTarget.ref_xenoforming.ChangeXenoformingLevel(Mathf.Min(-0.005f, -num5 / 100f));
							targetDestroyed = this.fleet.bombardmentTarget.ref_xenoforming.xenoformingLevel <= 0f;
							if (targetDestroyed)
							{
								GameControl.eventManager.TriggerEvent(new XenoformingDestroyed(this.fleet.bombardmentTarget.ref_xenoforming), null, new object[] { this.fleet.bombardmentTarget.ref_xenoforming });
							}
						}
					}
				}
				else
				{
					if (bombardmentTarget.ref_hab != null && bombardmentTarget.ref_faction.permanentAlly(bombardmentTarget.ref_hab.faction) && CS$<>8__locals1.weapon.isProjectileWeapon && CS$<>8__locals1.weapon.ref_projectileWeapon.isPointDefenseTargetable && TIUtilities.RandomFloatValue() < projectileVPDRatio && (!CS$<>8__locals1.weapon.isMissileWeapon || CS$<>8__locals1.weapon.ref_missileWeapon.warheadClass != WarheadClass.ShapedNuclear || TIUtilities.RandomFloatValue() > 0.2f))
					{
						num5 = 0f;
						flag3 = true;
					}
					if (num5 > 0f)
					{
						if (bombardmentTarget.isHabState)
						{
							TIHabState ref_hab = bombardmentTarget.ref_hab;
							if ((CS$<>8__locals1.weapon.isMissileWeapon && (CS$<>8__locals1.weapon.ref_missileWeapon.warheadClass == WarheadClass.Nuclear || CS$<>8__locals1.weapon.ref_missileWeapon.warheadClass == WarheadClass.Antimatter)) || ref_hab.OkayModules().Count == 0)
							{
								ref_hab.DestroyHab(this.faction, 0f, false, this.fleet, 0f);
								targetDestroyed = true;
							}
							else
							{
								List<TIHabModuleState> list = ref_hab.OkayModules().ToList<TIHabModuleState>();
								if (list.Count > 1)
								{
									list.Remove(ref_hab.coreModule);
								}
								TIHabModuleState tihabModuleState = list.SelectRandomWeightedItem<TIHabModuleState>((TIHabModuleState x) => (float)base.<Bombard>g__TargetedModuleWeight|3(x), -1f, 1E-37f);
								flag = true;
								text = tihabModuleState.displayName;
								num5 *= 0.8f + 4f * TIUtilities.RandomFloatValue() / 10f;
								if (TIUtilities.RandomFloatValue() > tihabModuleState.armorChipped)
								{
									float num9 = tihabModuleState.AntiBombardmentArmor(true);
									if (num9 > 0f)
									{
										if (CS$<>8__locals1.weapon.isLaserWeapon)
										{
											num9 = CS$<>8__locals1.weapon.ref_laserWeapon.ModifyArmorValueForLaserShot(this.fleet.bombardmentAltitude_km, num9, -1f);
										}
										if (num9 > num5)
										{
											num9 = num5;
										}
									}
									num5 -= num9;
								}
								if (num5 > 0f)
								{
									num5 += TIEffectsState.SumEffectsModifiers(Context.BombardmentHabDefenseBonus, ref_hab.ref_faction, num5, null);
									float num10 = (float)tihabModuleState.tier / num5;
									float num11 = TIUtilities.RandomFloatValue();
									if (CS$<>8__locals1.weapon.isMissileWeapon && CS$<>8__locals1.weapon.ref_missileWeapon.warheadClass == WarheadClass.ShapedNuclear)
									{
										num10 = 0f;
									}
									if (num11 > num10)
									{
										if (tihabModuleState.functional)
										{
											int num12 = tihabModuleState.tier * tihabModuleState.tier;
											if (tihabModuleState.moduleTemplate.mine)
											{
												num12 += tihabModuleState.tier;
											}
											else if (tihabModuleState.moduleTemplate.constructionModule)
											{
												num12 += tihabModuleState.tier;
											}
											this.faction.RegisterKill(tihabModuleState, (float)num12 / TemplateManager.global.AI_GetHateBurnoffFromKillingHabmodulesDivisor(this.faction.IsAlienFaction));
										}
										else if (tihabModuleState.underConstruction)
										{
											if (tihabModuleState.priorModuleTemplate != null)
											{
												this.faction.RegisterKill(tihabModuleState, (float)Mathf.Min(tihabModuleState.tier * tihabModuleState.tier, tihabModuleState.priorModuleTemplate.tier * tihabModuleState.priorModuleTemplate.tier) / TemplateManager.global.AI_GetHateBurnoffFromKillingHabmodulesDivisor(this.faction.IsAlienFaction));
											}
											else if (tihabModuleState.hab.anyCoreCompleted)
											{
												this.faction.RegisterKill(tihabModuleState, (float)(tihabModuleState.tier * tihabModuleState.tier) / (16f * TemplateManager.global.AI_GetHateBurnoffFromKillingHabmodulesDivisor(this.faction.IsAlienFaction)));
											}
										}
										ref_hab.DestroyModule(this.fleet.faction, tihabModuleState, false, false, false, 1f, false, false);
										tihabModuleState.destroyedTime = new TIDateTime(time);
										flag2 = true;
									}
									else
									{
										int num13 = ((tihabModuleState.tier == 1) ? 100 : ((tihabModuleState.tier == 2) ? 900 : 2500)) * (tihabModuleState.moduleTemplate.mine ? 5 : 1);
										float num14 = num / (float)num13;
										tihabModuleState.ChipBombardmentArmor(num14);
										num5 = 0f;
									}
								}
								if (ref_hab.deleted || ref_hab.OkayModules().Count == 0 || (ref_hab.IsAlien() && ref_hab.faction.primaryHab == ref_hab && ref_hab.OkayModules().Count <= 2))
								{
									targetDestroyed = true;
								}
							}
						}
						else if (bombardmentTarget.isSpaceFleetState)
						{
							if (CS$<>8__locals1.weapon.isMissileWeapon && (double)CS$<>8__locals1.weapon.flatDamage_MJ >= 1000000.0)
							{
								bombardmentTarget.ref_fleet.ships.ToList<TISpaceShipState>().ForEach(delegate(TISpaceShipState x)
								{
									x.DestroyShip(true, this.faction);
								});
							}
							else
							{
								List<TISpaceShipState> ships = bombardmentTarget.ref_fleet.ships;
								TISpaceShipState tispaceShipState = ((ships != null) ? ships.SelectRandomItem<TISpaceShipState>() : null);
								if (tispaceShipState != null)
								{
									flag = true;
									text = tispaceShipState.displayName;
									ArmorFacing armorFacing = (((double)TIUtilities.RandomFloatValue() < 0.5) ? ArmorFacing.Right : ArmorFacing.Left);
									float num15;
									float num16;
									tispaceShipState.ApplyDamage(CS$<>8__locals1.weapon, armorFacing, this.fleet.bombardmentAltitude_km, num5, CS$<>8__locals1.weapon.chipping(this.fleet.bombardmentAltitude_km), CS$<>8__locals1.weapon.GetDamageType(), (float)((armorFacing == ArmorFacing.Right) ? 90 : 270), this.faction, out num15, out num16, 0);
									if (num15 > 0f && tispaceShipState.ShipDestroyed())
									{
										TINotificationQueueState.LogShipDestroyedInStrat(tispaceShipState, new List<TIFactionState> { this.faction }, tispaceShipState.fleet.location, new Dictionary<TIFactionState, string> { 
										{
											tispaceShipState.faction,
											tispaceShipState.KillAllOfficersReport()
										} });
										tispaceShipState.DestroyShip(true, this.faction);
									}
								}
							}
							if (bombardmentTarget == null || bombardmentTarget.deleted || bombardmentTarget.ref_fleet.ships.Count == 0)
							{
								targetDestroyed = true;
							}
						}
					}
				}
				string text2;
				if (num5 > 0f)
				{
					if (flag)
					{
						if (flag2)
						{
							text2 = Loc.T("Bombard.Log.FireKillSubTarget", new object[]
							{
								time.ToCustomTimeString(),
								this.displayName,
								CS$<>8__locals1.weapon.displayName,
								text,
								TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, false, false),
								displayName
							});
						}
						else
						{
							text2 = Loc.T("Bombard.Log.FireHitSubTarget", new object[]
							{
								time.ToCustomTimeString(),
								this.displayName,
								CS$<>8__locals1.weapon.displayName,
								text,
								TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, false, false),
								displayName
							});
						}
					}
					else if (targetDestroyed)
					{
						text2 = Loc.T("Bombard.Log.FireKill", new object[]
						{
							time.ToCustomTimeString(),
							this.displayName,
							CS$<>8__locals1.weapon.displayName,
							displayName,
							TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, false, false)
						});
					}
					else
					{
						text2 = Loc.T("Bombard.Log.FireHit", new object[]
						{
							time.ToCustomTimeString(),
							this.displayName,
							CS$<>8__locals1.weapon.displayName,
							displayName,
							TIUtilities.FormatBigOrSmallNumber(num5, 1, 7, 0, false, false)
						});
					}
				}
				else if (flag)
				{
					text2 = Loc.T("Bombard.Log.FireMissSubTarget", new object[]
					{
						time.ToCustomTimeString(),
						this.displayName,
						CS$<>8__locals1.weapon.displayName,
						text,
						displayName
					});
				}
				else
				{
					text2 = Loc.T("Bombard.Log.FireMiss", new object[]
					{
						time.ToCustomTimeString(),
						this.displayName,
						CS$<>8__locals1.weapon.displayName,
						displayName
					});
				}
				if (flag3)
				{
					text2 = new StringBuilder(text2).Append(Loc.T("Bombard.Log.ProjectileShotDown", new object[] { TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false) })).ToString();
				}
				else if (num2 != num5)
				{
					text2 = new StringBuilder(text2).Append(Loc.T("Bombard.Log.BaseDamage", new object[] { TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false) })).ToString();
				}
				this.fleet.AddToBombardmentLog(text2, time);
				if (targetDestroyed)
				{
					this.fleet.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.TargetDestroyed);
					return;
				}
				if (bombardmentTarget.ref_faction == this.faction)
				{
					this.fleet.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.BombardingFriendly);
					return;
				}
			}
			else
			{
				targetHit = false;
				if (!TIGameState.Valid(bombardmentTarget))
				{
					this.fleet.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.TargetDestroyed);
					return;
				}
				if (bombardmentTarget.ref_faction == this.faction)
				{
					this.fleet.ForceEndBombardment(TISpaceFleetState.EndBombardmentReason.BombardingFriendly);
				}
			}
		}

		// Token: 0x06004624 RID: 17956 RVA: 0x001C96A6 File Offset: 0x001C78A6
		public bool WeaponDamaged(ModuleDataEntry moduleData)
		{
			return this.GetPartFunction(moduleData) < 1f;
		}

		// Token: 0x06004625 RID: 17957 RVA: 0x001C96B6 File Offset: 0x001C78B6
		public bool WeaponDestroyed(ModuleDataEntry moduleData)
		{
			return this.GetPartFunction(moduleData) == 0f;
		}

		// Token: 0x06004626 RID: 17958 RVA: 0x001C96C6 File Offset: 0x001C78C6
		public bool WeaponIsOperable(ModuleDataEntry moduleData)
		{
			return this.WeaponHasAmmo(moduleData) && !this.WeaponDamaged(moduleData);
		}

		// Token: 0x06004627 RID: 17959 RVA: 0x001C96DD File Offset: 0x001C78DD
		public bool WeaponCanFire(ModuleDataEntry moduleData)
		{
			return this.WeaponIsOperable(moduleData) && this.WeaponHasPower(moduleData) && !this.WeaponFireExceedsHeatCapacity(moduleData) && this.FireControlActive();
		}

		// Token: 0x06004628 RID: 17960 RVA: 0x001C9702 File Offset: 0x001C7902
		public bool WeaponDisabledBeyondFieldRepair(ModuleDataEntry moduleData)
		{
			return !this.WeaponHasAmmo(moduleData) || this.PartDestroyed(moduleData) || (this.PartDamaged(moduleData) && this.GetSystemFunction(ShipSystem.DamageControl) <= 0f);
		}

		// Token: 0x06004629 RID: 17961 RVA: 0x001C9738 File Offset: 0x001C7938
		public bool AnyWeaponCanFire()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (this.WeaponIsOperable(moduleDataEntry))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600462A RID: 17962 RVA: 0x001C9794 File Offset: 0x001C7994
		public bool AllWeaponsDestroyed()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (this.GetPartFunction(moduleDataEntry) > 0f)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600462B RID: 17963 RVA: 0x001C97F8 File Offset: 0x001C79F8
		public bool AllWeaponsDisabledBeyondFieldRepair()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (!this.WeaponDisabledBeyondFieldRepair(moduleDataEntry))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600462C RID: 17964 RVA: 0x001C9854 File Offset: 0x001C7A54
		public bool AnyOffensiveWeaponCanFire()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (moduleDataEntry.weaponTemplate.attackMode && this.WeaponIsOperable(moduleDataEntry))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600462D RID: 17965 RVA: 0x001C98C0 File Offset: 0x001C7AC0
		public bool AnyOffensiveMissileWeaponCanFire()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (moduleDataEntry.weaponTemplate.attackMode && moduleDataEntry.weaponTemplate.isMissileWeapon && this.WeaponIsOperable(moduleDataEntry))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600462E RID: 17966 RVA: 0x001C9938 File Offset: 0x001C7B38
		public static Vector3 BombardmentTargetPosition_Display(TISpaceFleetState fleet, TIDateTime time, out float targetLongitude, out float targetLatitude, out Transform parentSpaceBody, out Transform targetTransform)
		{
			if (fleet.bombardmentTarget.hasEarthMapObject)
			{
				TIRegionState ref_region = fleet.bombardmentTarget.ref_region;
				MapController mapController = GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController;
				RegionController regionController = mapController.GetRegionController(fleet.bombardmentTarget.ref_region);
				targetLongitude = ref_region.longitude;
				targetLatitude = ref_region.latitude;
				SpaceObjectController component = mapController.transform.parent.GetComponent<SpaceObjectController>();
				parentSpaceBody = component.modelController.transform;
				targetTransform = regionController.transform;
				return component.modelController.transform.rotation * Quaternion.AngleAxis(targetLongitude, -Vector3.up) * Quaternion.AngleAxis(targetLatitude, -Vector3.right) * Vector3.forward * component.radius_gameUnits + CameraManager.Singleton.ScaledPosition_DoNotTouchCache(ref_region.ref_spaceBody.GetGlobalPositionAtTime(time));
			}
			if (fleet.ref_spaceBody != null)
			{
				Transform transform = fleet.ref_spaceBody.controller.modelLink.transform;
				targetLongitude = fleet.bombardmentTarget.ref_habSite.longitude;
				targetLatitude = fleet.bombardmentTarget.ref_habSite.latitude;
				parentSpaceBody = transform;
				targetTransform = fleet.bombardmentTarget.ref_habSite.GetController().transform;
				return transform.rotation * Quaternion.AngleAxis(targetLongitude, -Vector3.up) * Quaternion.AngleAxis(targetLatitude, -Vector3.right) * Vector3.forward * fleet.bombardmentTarget.ref_habSite.radius_gameUnits + CameraManager.Singleton.ScaledPosition_DoNotTouchCache(fleet.bombardmentTarget.ref_spaceBody.GetGlobalPositionAtTime(time));
			}
			targetLongitude = 0f;
			targetLatitude = 0f;
			parentSpaceBody = null;
			targetTransform = null;
			return CameraManager.Singleton.ScaledPosition_DoNotTouchCache(fleet.GetGlobalPositionAtTime(time));
		}

		// Token: 0x0600462F RID: 17967 RVA: 0x001C9B40 File Offset: 0x001C7D40
		public float BombardmentValue(TISpaceBodyState spaceBody)
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in from x in this.AllWeaponModuleData()
				where this.WeaponIsOperable(x)
				select x)
			{
				num += moduleDataEntry.moduleTemplate.ref_weapon.GetLocalBombardmentValue(spaceBody);
			}
			return num;
		}

		// Token: 0x06004630 RID: 17968 RVA: 0x001C9BB4 File Offset: 0x001C7DB4
		public float BombardmentValue(TISpaceBodyState spaceBody, float range_km)
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in from x in this.AllWeaponModuleData()
				where this.WeaponIsOperable(x)
				select x)
			{
				num += moduleDataEntry.moduleTemplate.ref_weapon.GetLocalBombardmentValue(spaceBody, range_km);
			}
			return num;
		}

		// Token: 0x06004631 RID: 17969 RVA: 0x001C9C28 File Offset: 0x001C7E28
		public float CurrentBatteryCapacity_GJ()
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				TIBatteryTemplate ref_battery = moduleDataEntry.moduleTemplate.ref_battery;
				if (ref_battery != null)
				{
					num += ref_battery.energyCapacity_GJ * this.GetPartFunction(moduleDataEntry);
				}
			}
			return num;
		}

		// Token: 0x17000DD0 RID: 3536
		// (get) Token: 0x06004632 RID: 17970 RVA: 0x001C9C9C File Offset: 0x001C7E9C
		public float currentBatteryCharge_GJ
		{
			get
			{
				return this.batteryCharge.Sum<KeyValuePair<ModuleDataEntry, float>>((KeyValuePair<ModuleDataEntry, float> x) => x.Value);
			}
		}

		// Token: 0x06004633 RID: 17971 RVA: 0x001C9CC8 File Offset: 0x001C7EC8
		public float ChangeBatteryCharge(float energyDelta_GJ, bool triggerUIUpdate)
		{
			float num = 0f;
			if (TIGlobalValuesState.isSpaceCombatEnabled)
			{
				if (energyDelta_GJ > 0f && this.CanGainHeat)
				{
					foreach (ModuleDataEntry moduleDataEntry in this.batteryCharge.Keys.ToList<ModuleDataEntry>())
					{
						float num2 = Mathf.Min(energyDelta_GJ, moduleDataEntry.moduleTemplate.ref_battery.rechargeRate_GJs);
						Dictionary<ModuleDataEntry, float> dictionary = this.batteryCharge;
						ModuleDataEntry moduleDataEntry2 = moduleDataEntry;
						dictionary[moduleDataEntry2] += num2;
						float num3 = this.batteryCharge[moduleDataEntry] - moduleDataEntry.moduleTemplate.ref_battery.GetCapacity(this.hull.simpleHull) * this.GetPartFunction(moduleDataEntry);
						if (num3 > 0f)
						{
							dictionary = this.batteryCharge;
							moduleDataEntry2 = moduleDataEntry;
							dictionary[moduleDataEntry2] -= num3;
							num2 -= num3;
						}
						num += num2;
						energyDelta_GJ -= num2;
					}
					this.ApplyHeat(num / this.powerPlant.efficiency, true);
				}
				else
				{
					foreach (ModuleDataEntry moduleDataEntry3 in this.batteryCharge.Keys.ToList<ModuleDataEntry>())
					{
						float num4 = Mathf.Max(energyDelta_GJ, -this.batteryCharge[moduleDataEntry3]);
						Dictionary<ModuleDataEntry, float> dictionary = this.batteryCharge;
						ModuleDataEntry moduleDataEntry2 = moduleDataEntry3;
						dictionary[moduleDataEntry2] += num4;
						num += num4;
					}
				}
				if (triggerUIUpdate && num != 0f)
				{
					GameControl.eventManager.TriggerEvent(new ShipPowerSystemsChargeChange(this), null, new object[] { this });
				}
			}
			else
			{
				this.ChargeBatteriesToMax();
			}
			return num;
		}

		// Token: 0x06004634 RID: 17972 RVA: 0x001C9EB0 File Offset: 0x001C80B0
		public void ChargeBatteriesToMax()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.batteryCharge.Keys.ToList<ModuleDataEntry>())
			{
				this.batteryCharge[moduleDataEntry] = moduleDataEntry.moduleTemplate.ref_battery.energyCapacity_GJ * this.GetPartFunction(moduleDataEntry);
			}
		}

		// Token: 0x17000DD1 RID: 3537
		// (get) Token: 0x06004635 RID: 17973 RVA: 0x001C9F2C File Offset: 0x001C812C
		public float heatCapFraction
		{
			get
			{
				if (this.template.HeatCapacity_GJ(false) > 0f)
				{
					return this.currentHeatSinkCapacity_GJ / this.template.HeatCapacity_GJ(false);
				}
				return 1f;
			}
		}

		// Token: 0x17000DD2 RID: 3538
		// (get) Token: 0x06004636 RID: 17974 RVA: 0x001C9F5A File Offset: 0x001C815A
		public float heatFraction
		{
			get
			{
				if (this.template.HeatCapacity_GJ(false) > 0f)
				{
					return this.accumulatedHeat_GJ / this.template.HeatCapacity_GJ(false);
				}
				return 1f;
			}
		}

		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06004637 RID: 17975 RVA: 0x001C9F88 File Offset: 0x001C8188
		public bool CanGainHeat
		{
			get
			{
				return this.cooling || this.accumulatedHeat_GJ < this.currentHeatSinkCapacity_GJ * 0.98f;
			}
		}

		// Token: 0x17000DD4 RID: 3540
		// (get) Token: 0x06004638 RID: 17976 RVA: 0x001C9FA8 File Offset: 0x001C81A8
		public bool cooling
		{
			get
			{
				return (this.radiatorsExtended || this.radiatorsExtending) && !this.PartDestroyed(this.radiatorModule);
			}
		}

		// Token: 0x17000DD5 RID: 3541
		// (get) Token: 0x06004639 RID: 17977 RVA: 0x001C9FCB File Offset: 0x001C81CB
		public bool overheated
		{
			get
			{
				return this.accumulatedHeat_GJ >= this.currentHeatSinkCapacity_GJ;
			}
		}

		// Token: 0x0600463A RID: 17978 RVA: 0x001C9FDE File Offset: 0x001C81DE
		public void UpdateHeatSinkCapacity_GJ()
		{
			this.currentHeatSinkCapacity_GJ = this.template.heatSinkModules.Sum<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.ref_heatSink.heatCapacity_GJ * this.GetPartFunction(x));
			if (this.overheated)
			{
				this.accumulatedHeat_GJ = this.currentHeatSinkCapacity_GJ;
			}
		}

		// Token: 0x0600463B RID: 17979 RVA: 0x001CA016 File Offset: 0x001C8216
		public void ApplyHeat(float heatValue_GJ, bool triggerUpdateEvent)
		{
			if (!this.cooling)
			{
				this.ChangeHeatInSinks(heatValue_GJ, triggerUpdateEvent);
			}
		}

		// Token: 0x0600463C RID: 17980 RVA: 0x001CA028 File Offset: 0x001C8228
		public float RadiatorCooling_GJ()
		{
			return -this.WasteHeat_GW * this.GetPartFunction(this.radiatorModule);
		}

		// Token: 0x0600463D RID: 17981 RVA: 0x001CA03E File Offset: 0x001C823E
		public void BleedHeatFromRadiators_s(double timeElapsed_s, bool triggerUpdateEvent)
		{
			if (this.cooling)
			{
				this.ChangeHeatInSinks((float)timeElapsed_s * this.RadiatorCooling_GJ(), triggerUpdateEvent);
			}
		}

		// Token: 0x0600463E RID: 17982 RVA: 0x001CA058 File Offset: 0x001C8258
		public float InternalDamageModifier()
		{
			float num = 1f + this.SumOfficerEffectsModifiers(OfficerEffectType.InternalDamageTaken, 1f);
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.ComponentArmor))
				{
					return num * (1f - (1f - moduleDataEntry.moduleTemplate.ref_utilityModule.specialModuleValue) * this.GetPartFunction(moduleDataEntry));
				}
			}
			return num;
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x001CA104 File Offset: 0x001C8304
		public void ChangeHeatInSinks(float heatValue_GJ, bool triggerUpdateEvent)
		{
			this.accumulatedHeat_GJ = Mathf.Max(this.accumulatedHeat_GJ + heatValue_GJ, 0f);
			if (triggerUpdateEvent && this.oldHeatAtLastUIUpdate_GJ != this.accumulatedHeat_GJ)
			{
				GameControl.eventManager.TriggerEvent(new ShipHeatChange(this), null, new object[] { this });
				this.oldHeatAtLastUIUpdate_GJ = this.accumulatedHeat_GJ;
			}
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x001CA164 File Offset: 0x001C8364
		public void ResolveHeatInSinks()
		{
			bool flag = false;
			if (this.accumulatedHeat_GJ > this.currentHeatSinkCapacity_GJ && !this.radiatorsExtended && !this.radiatorsExtending && !this.radiatorsRetracting && !this.PartDestroyed(this.radiatorModule))
			{
				this.InitiateExtendRadiators();
				this.accumulatedHeat_GJ = this.currentHeatSinkCapacity_GJ;
				GameControl.eventManager.TriggerEvent(new ShipHeatChange(this), null, new object[] { this });
				flag = true;
			}
			if (this.accumulatedHeat_GJ > this.currentHeatSinkCapacity_GJ)
			{
				float num = (this.accumulatedHeat_GJ - this.currentHeatSinkCapacity_GJ) * 1000f;
				num *= this.InternalDamageModifier();
				this.ApplyInternalDamage(ArmorFacing.Core, ArmorFacing.Core, num / 20f, true, 1f, 0f);
				this.accumulatedHeat_GJ = this.currentHeatSinkCapacity_GJ;
				if (!flag)
				{
					GameControl.eventManager.TriggerEvent(new ShipHeatChange(this), null, new object[] { this });
				}
			}
		}

		// Token: 0x17000DD6 RID: 3542
		// (get) Token: 0x06004641 RID: 17985 RVA: 0x001CA245 File Offset: 0x001C8445
		public float damage_mainThrustModifier
		{
			get
			{
				return this.GetPartFunction(this.driveModule) * this.GetSystemFunction(ShipSystem.DriveCoupling) * this.GetPartFunction(this.powerPlantModule);
			}
		}

		// Token: 0x17000DD7 RID: 3543
		// (get) Token: 0x06004642 RID: 17986 RVA: 0x001CA268 File Offset: 0x001C8468
		public float damage_vectorThrustModifier
		{
			get
			{
				return this.GetSystemFunction(ShipSystem.VectorThrusters) * this.GetPartFunction(this.powerPlantModule);
			}
		}

		// Token: 0x17000DD8 RID: 3544
		// (get) Token: 0x06004643 RID: 17987 RVA: 0x001CA27F File Offset: 0x001C847F
		public float weaponCooldownModifier_Pct
		{
			get
			{
				return this.GetSystemFunction(ShipSystem.FireControl);
			}
		}

		// Token: 0x06004644 RID: 17988 RVA: 0x001CA288 File Offset: 0x001C8488
		public bool CanPerformShipCommands()
		{
			return !this.ShipDestroyed() && this.GetSystemFunction(ShipSystem.Bridge) > 0.5f;
		}

		// Token: 0x06004645 RID: 17989 RVA: 0x001CA2A2 File Offset: 0x001C84A2
		public bool CanSetWaypoints()
		{
			return this.GetSystemFunction(ShipSystem.Bridge) > 0f && this.GetPartFunction(this.powerPlantModule) > 0f;
		}

		// Token: 0x06004646 RID: 17990 RVA: 0x001CA2C7 File Offset: 0x001C84C7
		public bool FireControlActive()
		{
			return this.GetSystemFunction(ShipSystem.FireControl) > 0f;
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x001CA2D7 File Offset: 0x001C84D7
		public bool CanRotateAndRoll()
		{
			return this.GetPartFunction(this.powerPlantModule) > 0f;
		}

		// Token: 0x17000DD9 RID: 3545
		// (get) Token: 0x06004648 RID: 17992 RVA: 0x001CA2EC File Offset: 0x001C84EC
		public float ManeuverEffectivenessRatio
		{
			get
			{
				return (this.GetPartFunction(this.driveModule) + this.GetPartFunction(this.powerPlantModule)) / 2f;
			}
		}

		// Token: 0x17000DDA RID: 3546
		// (get) Token: 0x06004649 RID: 17993 RVA: 0x001CA30D File Offset: 0x001C850D
		public float ThrustEffectivenessRatio
		{
			get
			{
				return this.GetPartFunction(this.driveModule);
			}
		}

		// Token: 0x17000DDB RID: 3547
		// (get) Token: 0x0600464A RID: 17994 RVA: 0x001CA31B File Offset: 0x001C851B
		public bool VisiblyDamaged
		{
			get
			{
				return TISpaceShipState.visiblyDamagedSystems.Any<ShipSystem>((ShipSystem x) => this.GetSystemDamage(x) > 0f);
			}
		}

		// Token: 0x17000DDC RID: 3548
		// (get) Token: 0x0600464B RID: 17995 RVA: 0x001CA333 File Offset: 0x001C8533
		public float VisibleDamageFraction
		{
			get
			{
				return TISpaceShipState.visiblyDamagedSystems.Average<ShipSystem>((ShipSystem x) => this.GetSystemDamage(x));
			}
		}

		// Token: 0x17000DDD RID: 3549
		// (get) Token: 0x0600464C RID: 17996 RVA: 0x001CA34B File Offset: 0x001C854B
		public float CriticalDamageTotal
		{
			get
			{
				return (this.GetSystemDamage(ShipSystem.NoseStructure) + this.GetSystemDamage(ShipSystem.CentralStructure) + this.GetSystemDamage(ShipSystem.TailStructure)) * (float)this.hull.structuralIntegrity;
			}
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x001CA374 File Offset: 0x001C8574
		public void BuildInternalDamageTables()
		{
			this.internalDamageTables.Clear();
			this.internalDamageTables.Add(ArmorFacing.Nose, new Dictionary<ShipSystem, float>());
			this.internalDamageTables.Add(ArmorFacing.Left, new Dictionary<ShipSystem, float>());
			this.internalDamageTables.Add(ArmorFacing.Tail, new Dictionary<ShipSystem, float>());
			this.internalDamageTables.Add(ArmorFacing.Core, new Dictionary<ShipSystem, float>());
			this.internalDamageTables[ArmorFacing.Nose].Add(ShipSystem.NoseStructure, 1f);
			foreach (TIShipWeaponTemplate tishipWeaponTemplate in this.noseWeaponTemplates)
			{
				if (!this.internalDamageTables[ArmorFacing.Nose].ContainsKey(ShipSystem.NoseWeapons))
				{
					this.internalDamageTables[ArmorFacing.Nose].Add(ShipSystem.NoseWeapons, 0f);
				}
				Dictionary<ShipSystem, float> dictionary = this.internalDamageTables[ArmorFacing.Nose];
				dictionary[ShipSystem.NoseWeapons] = dictionary[ShipSystem.NoseWeapons] + (float)tishipWeaponTemplate.internalSize;
			}
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.CentralStructure, 1f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.PowerCoupling, 5f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.PowerPlant, (float)this.template.powerPlantTemplate.internalSize);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.Drive, (float)this.template.driveTemplate.internalSize / 6f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.VectorThrusters, 1f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.Sensors, 1f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.Radiators, (float)this.template.radiatorTemplate.internalSize);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.Propellant, (float)this.template.propellantTanks / 10f);
			this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.DriveCoupling, 0.5f);
			foreach (TIShipWeaponTemplate tishipWeaponTemplate2 in this.hullWeaponTemplates)
			{
				if (!this.internalDamageTables[ArmorFacing.Left].ContainsKey(ShipSystem.HullWeapons))
				{
					this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.HullWeapons, 0f);
				}
				Dictionary<ShipSystem, float> dictionary = this.internalDamageTables[ArmorFacing.Left];
				dictionary[ShipSystem.HullWeapons] = dictionary[ShipSystem.HullWeapons] + (float)tishipWeaponTemplate2.internalSize;
			}
			foreach (TIShipModuleTemplate tishipModuleTemplate in this.utilityModuleTemplates.Where<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
			{
				TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
				return ref_utilityModule == null || !ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.ImmuneToDamage);
			}))
			{
				if (!this.internalDamageTables[ArmorFacing.Left].ContainsKey(ShipSystem.UtilityModules))
				{
					this.internalDamageTables[ArmorFacing.Left].Add(ShipSystem.UtilityModules, 0f);
				}
				Dictionary<ShipSystem, float> dictionary = this.internalDamageTables[ArmorFacing.Left];
				dictionary[ShipSystem.UtilityModules] = dictionary[ShipSystem.UtilityModules] + (float)tishipModuleTemplate.internalSize;
			}
			this.internalDamageTables[ArmorFacing.Right] = new Dictionary<ShipSystem, float>(this.internalDamageTables[ArmorFacing.Left]);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.Drive, (float)this.template.driveTemplate.internalSize);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.DriveCoupling, 3f);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.Radiators, (float)this.template.radiatorTemplate.internalSize);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.PowerCoupling, 5f);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.PowerPlant, 1f);
			this.internalDamageTables[ArmorFacing.Tail].Add(ShipSystem.TailStructure, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.NoseStructure, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.CentralStructure, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.TailStructure, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.Bridge, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.LifeSupportMain, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.LifeSupportBackup, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.FireControl, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.DamageControl, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.Sensors, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.SystemsReactor, 1f);
			this.internalDamageTables[ArmorFacing.Core].Add(ShipSystem.UtilityModules, (float)this.template.utilityModules.Count<ModuleDataEntry>(delegate(ModuleDataEntry x)
			{
				TIUtilityModuleTemplate ref_utilityModule2 = x.moduleTemplate.ref_utilityModule;
				return ref_utilityModule2 == null || !ref_utilityModule2.specialModuleRules.Contains(SpecialModuleRule.ImmuneToDamage);
			}));
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x001CA8B4 File Offset: 0x001C8AB4
		public float AbsorbAndApplyArmorDamage(TIShipWeaponTemplate attackingWeapon, ArmorFacing facing, float range_km, float damageAmount, float chippingAmount, DamageType damageType, float angle, TIFactionState attackingFaction, out float internalDamageAssessedHere, out float appliedRadiationDamage, int shreddingAmount = 0)
		{
			ModuleDataEntry partToDamage = this.GetPartToDamage(ShipSystem.Radiators, false);
			internalDamageAssessedHere = 0f;
			appliedRadiationDamage = 0f;
			TISpaceFleetState fleet = this.fleet;
			bool flag = fleet != null && fleet.bombarding;
			if ((this.radiatorsExtended || this.radiatorsExtending || this.radiatorsRetracting) && (!flag || !this.canEverRetractRadiators) && !this.PartDestroyed(partToDamage) && attackingWeapon != null && TIUtilities.RandomFloatValue() * 100f < (float)this.template.radiatorTemplate.vulnerability * ((this.radiatorsExtending || this.radiatorsRetracting) ? 0.5f : 1f))
			{
				float num = damageAmount + chippingAmount;
				if ((attackingWeapon == null || attackingWeapon.isParticleWeapon) && ((attackingWeapon != null) ? attackingWeapon.ref_particleWeapon : null) != null)
				{
					num *= attackingWeapon.ref_particleWeapon.heatFraction;
					damageAmount -= num;
				}
				else
				{
					damageAmount = num * attackingWeapon.chipping(range_km);
				}
				float num2;
				this.ApplyDamageToPart(partToDamage, damageAmount, out num2);
				internalDamageAssessedHere += num2;
				this.spaceCombatValueDataDirty = true;
				if (shreddingAmount == 0 && !attackingWeapon.isParticleWeapon && (facing == ArmorFacing.Nose || facing == ArmorFacing.Tail || TIUtilities.RandomFloatValue() < 0.25f))
				{
					return num2;
				}
			}
			if (facing == ArmorFacing.Tail && 180f - Mathf.Abs(angle) < 5f && TIUtilities.RandomFloatValue() < 0.25f)
			{
				float num3;
				this.ApplyDamageToPart(this.GetPartToDamage(ShipSystem.Drive, false), damageAmount, out num3);
				internalDamageAssessedHere += num3;
			}
			if (shreddingAmount > 0)
			{
				float num4;
				this.ApplyDamageToSystem(ShipSystem.Sensors, 0.5f, out num4);
				internalDamageAssessedHere += num4;
			}
			else if (attackingWeapon != null && attackingWeapon.isBeamWeapon && TIUtilities.RandomFloatValue() < this.GetSystemFunction(ShipSystem.Sensors) * 0.1f)
			{
				float num5;
				this.ApplyDamageToSystem(ShipSystem.Sensors, TIUtilities.RandomFloatValue() * 0.1f, out num5);
				internalDamageAssessedHere += num5;
			}
			else if (TIUtilities.RandomFloatValue() < this.GetSystemFunction(ShipSystem.Sensors) * 0.01f)
			{
				float num6;
				this.ApplyDamageToSystem(ShipSystem.Sensors, TIUtilities.RandomFloatValue() * 0.1f, out num6);
				internalDamageAssessedHere += num6;
			}
			TIShipArmorTemplate armorTemplate = TISpaceShipState.ArmorData.GetArmorTemplate(this, facing);
			int num7 = 0;
			int num8 = 1;
			bool flag2 = attackingWeapon != null && attackingWeapon.isMissileWeapon && attackingWeapon.ref_missileWeapon.warheadClass == WarheadClass.Fragmentation;
			if (flag2)
			{
				num8 = Mathf.RoundToInt(attackingWeapon.ref_missileWeapon.warheadMass_kg / 5f);
			}
			bool flag3 = false;
			float num9 = (float)num8;
			float num10 = 0f;
			for (int i = 0; i < num8; i++)
			{
				if (TIUtilities.RandomFloatValue() < this.armor[facing].chippedPct)
				{
					num7++;
					flag3 = true;
					if (num8 == 1)
					{
						damageAmount += chippingAmount;
					}
					else
					{
						num10 += chippingAmount / (float)num8;
					}
					num9 -= 1f;
				}
			}
			chippingAmount *= num9 / (float)num8;
			if (chippingAmount > 0f)
			{
				float armorFacingVolume_m = TISpaceShipState.ArmorData.GetArmorFacingVolume_m3(this, facing);
				if (armorFacingVolume_m > 0f && this.armor[facing].chippedPct < 1f && this.armor[facing].armorValue > 0)
				{
					chippingAmount *= armorTemplate.GetSpecialtyModifiers(ArmorSpecialty.ChippingResistance);
					float num11 = chippingAmount / armorFacingVolume_m;
					chippingAmount = this.armor[facing].ChipArmor(num11);
					if (this.armor[facing].chippedPct >= 1f)
					{
						flag3 = true;
						num7 = (flag2 ? num8 : 1);
					}
				}
				else
				{
					damageAmount += chippingAmount;
					chippingAmount = 0f;
					flag3 = true;
					num7 = (flag2 ? num8 : 1);
				}
			}
			float num12 = 0f;
			if (shreddingAmount > 0)
			{
				this.spaceCombatValueDataDirty = true;
				damageAmount = (float)this.armor[facing].ShredArmor(shreddingAmount);
				if (this.armor[facing].armorValue <= 0)
				{
					flag3 = true;
					num7 = (flag2 ? num8 : 1);
				}
				else
				{
					num12 = (float)shreddingAmount * this.armor[facing].chippedPct;
					if (num12 > 0f)
					{
						flag3 = true;
						num7 = (flag2 ? num8 : 1);
					}
				}
			}
			float num13 = (float)this.armor[facing].armorValue;
			if (attackingWeapon != null && !flag3 && num13 > 0f && this.armor[facing].chippedPct < 1f)
			{
				if (!TIGlobalValuesState.Customizations.cinematicCombatRealismScale)
				{
					if (facing != ArmorFacing.Right)
					{
						if (facing == ArmorFacing.Left)
						{
							num13 /= Mathf.Cos(0.017453292f * Mathf.Abs(-90f - angle));
						}
					}
					else
					{
						num13 /= Mathf.Cos(0.017453292f * Mathf.Abs(90f - angle));
					}
				}
				if (attackingWeapon.weaponClass == WeaponClass.Laser)
				{
					num13 = attackingWeapon.ref_laserWeapon.ModifyArmorValueForLaserShot(range_km, num13, -1f);
					damageAmount *= armorTemplate.GetSpecialtyModifiers(ArmorSpecialty.LaserResistance);
				}
				if (damageType == DamageType.Kinetic)
				{
					damageAmount *= armorTemplate.GetSpecialtyModifiers(ArmorSpecialty.KineticsResistance);
				}
			}
			damageAmount += TIEffectsState.SumEffectsModifiers(Context.DamageReductionAgainstAllShips, this.faction, damageAmount, null);
			if (attackingFaction != null && attackingFaction.IsAlienFaction)
			{
				damageAmount += TIEffectsState.SumEffectsModifiers(Context.DamageReductionAgainstAlienShips, this.faction, damageAmount, null);
			}
			if (flag3)
			{
				this.spaceCombatValueDataDirty = true;
				if (flag2)
				{
					this.spaceCombatValueDataDirty = true;
					float num14 = (float)num7 / (float)num8;
					float num15 = num14 * damageAmount + num10;
					if (num9 > 0f)
					{
						float num16 = (1f - num14) * damageAmount / num9 - num13;
						if (num16 > 0f)
						{
							num15 += num16 * num9;
						}
					}
					return Mathf.Max(0f, num15);
				}
				if (num12 > 0f)
				{
					return num12;
				}
			}
			else if (flag2)
			{
				float num17 = damageAmount / num9 - num13;
				damageAmount = 0f;
				if (num17 > 0f)
				{
					damageAmount += num17 * num9;
					this.spaceCombatValueDataDirty = true;
				}
				return damageAmount;
			}
			if (damageType == DamageType.ParticleBeam && attackingWeapon != null)
			{
				float num18 = damageAmount * attackingWeapon.ref_particleWeapon.heatFraction;
				float num19 = damageAmount * attackingWeapon.ref_particleWeapon.xRayFraction;
				float num20 = damageAmount * attackingWeapon.ref_particleWeapon.baryonFraction;
				if (!flag3)
				{
					num19 = Mathf.Max(0f, num19 * Mathf.Min(0.0625f, Mathf.Pow(0.5f, armorTemplate.armor_section_thickness_m((float)this.armor[facing].armorValue) * 100f / armorTemplate.xRayHalfValue_cm)));
					num20 = Mathf.Max(0f, num20 * Mathf.Min(0.0625f, Mathf.Pow(0.5f, armorTemplate.armor_section_thickness_m((float)this.armor[facing].armorValue) * 100f / armorTemplate.baryonicHalfValue_cm)));
				}
				num19 += this.SumOfficerEffectsModifiers(OfficerEffectType.RadiationDamageReduction, num19);
				num20 += this.SumOfficerEffectsModifiers(OfficerEffectType.RadiationDamageReduction, num20);
				appliedRadiationDamage = this.ApplyInternalRadiationDamage(num19, num20, facing, facing, angle, Mathf.Min(1f, attackingWeapon.ref_particleWeapon.SpotSurfaceArea_m2(range_km) / this.GetCrossSectionalArea_m2(angle)));
				internalDamageAssessedHere += appliedRadiationDamage;
				damageAmount = num18;
			}
			float num21 = 0f;
			if (damageAmount > 0f)
			{
				num21 = Mathf.Max(0f, damageAmount + (flag3 ? chippingAmount : 0f) - (flag3 ? 0f : num13));
			}
			if (num21 > 0f || internalDamageAssessedHere > 0f)
			{
				this.spaceCombatValueDataDirty = true;
			}
			return num21;
		}

		// Token: 0x0600464F RID: 17999 RVA: 0x001CAFDC File Offset: 0x001C91DC
		public void ApplyDamage(TIShipWeaponTemplate attackingWeapon, ArmorFacing facing, float range_km, float damageAmount, float chippingAmount, DamageType damageType, float angle, TIFactionState attackingFaction, out float internalDamageAssessedHere, out float appliedRadiationDamage, int shreddingAmount = 0)
		{
			float num = this.AbsorbAndApplyArmorDamage(attackingWeapon, facing, range_km, damageAmount, chippingAmount, damageType, angle, attackingFaction, out internalDamageAssessedHere, out appliedRadiationDamage, shreddingAmount);
			if (num > 0f)
			{
				float num2 = num * this.InternalDamageModifier();
				bool flag = damageType == DamageType.Explosive || damageType == DamageType.Nuclear || shreddingAmount > 0;
				this.ApplyInternalDamage(facing, facing, num2, flag, chippingAmount, angle);
				internalDamageAssessedHere += num2;
			}
		}

		// Token: 0x06004650 RID: 18000 RVA: 0x001CB040 File Offset: 0x001C9240
		public ArmorFacing GetNextInteralDamageLocation(ArmorFacing facing, ArmorFacing originalFacing, float angle, bool explosion, float damageDiffusion, out bool outTheOtherSide)
		{
			Dictionary<ArmorFacing, float> dictionary = new Dictionary<ArmorFacing, float> { 
			{
				facing,
				(float)((facing == ArmorFacing.Core) ? 2 : 4) * Mathf.Clamp01(damageDiffusion)
			} };
			switch (facing)
			{
			case ArmorFacing.Nose:
				dictionary.Add(ArmorFacing.Left, (angle >= 0f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Right, (angle <= 0f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Core, 1f);
				break;
			case ArmorFacing.Right:
				dictionary.Add(ArmorFacing.Nose, (angle >= 90f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Tail, (angle <= 90f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Core, 1f);
				break;
			case ArmorFacing.Left:
				dictionary.Add(ArmorFacing.Nose, (angle <= -90f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Tail, (angle >= -90f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Core, 1f);
				break;
			case ArmorFacing.Tail:
				dictionary.Add(ArmorFacing.Left, (angle <= 0f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Right, (angle >= 0f) ? 1.5f : 0.5f);
				dictionary.Add(ArmorFacing.Core, 1f);
				break;
			case ArmorFacing.Core:
				dictionary.Add(ArmorFacing.Nose, 1f);
				dictionary.Add(ArmorFacing.Tail, 1f);
				dictionary.Add(ArmorFacing.Left, 1f);
				dictionary.Add(ArmorFacing.Right, 1f);
				break;
			}
			outTheOtherSide = false;
			if (originalFacing != ArmorFacing.Core && !explosion)
			{
				if (originalFacing != facing)
				{
					dictionary.Remove(originalFacing);
				}
				bool flag = (double)TIUtilities.RandomFloatValue() < 0.3 + (double)((float)this.hull.consTier * 0.15f);
				if (originalFacing == ArmorFacing.Nose && (facing == ArmorFacing.Tail || (flag && (facing == ArmorFacing.Right || facing == ArmorFacing.Left))))
				{
					outTheOtherSide = true;
				}
				else if (originalFacing == ArmorFacing.Left && (facing == ArmorFacing.Right || (flag && (facing == ArmorFacing.Nose || facing == ArmorFacing.Tail))))
				{
					outTheOtherSide = true;
				}
				else if (originalFacing == ArmorFacing.Right && (facing == ArmorFacing.Left || (flag && (facing == ArmorFacing.Nose || facing == ArmorFacing.Tail))))
				{
					outTheOtherSide = true;
				}
				else if (originalFacing == ArmorFacing.Tail && (facing == ArmorFacing.Nose || (flag && (facing == ArmorFacing.Right || facing == ArmorFacing.Left))))
				{
					outTheOtherSide = true;
				}
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<ArmorFacing, float>>((KeyValuePair<ArmorFacing, float> x) => x.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06004651 RID: 18001 RVA: 0x001CB2B4 File Offset: 0x001C94B4
		public void ApplyInternalDamage(ArmorFacing facing, ArmorFacing originalFacing, float damageAmount, bool explosion, float weaponChipValue, float angle)
		{
			if (this.internalDamageTables.Count == 0)
			{
				return;
			}
			bool flag = false;
			float num = damageAmount;
			IEnumerable<KeyValuePair<ShipSystem, float>> enumerable = this.internalDamageTables[facing].Where<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => !this.SystemAsAWholeCanBeDamaged(x.Key) || this.GetSystemFunction(x.Key) > 0f);
			if (enumerable.Any<KeyValuePair<ShipSystem, float>>())
			{
				ShipSystem key = enumerable.SelectRandomWeightedItem<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => x.Value, -1f, 1E-37f).Key;
				if (key == ShipSystem.Propellant)
				{
					float num2 = this.propellant_tons / this.template.propellantMass_tons;
					int propellantTanks = this.template.propellantTanks;
					int num3 = (int)(num / 1f);
					if (num3 > this.template.propellantTanks)
					{
						num3 = this.template.propellantTanks;
					}
					for (int i = 0; i < num3; i++)
					{
						if (TIUtilities.RandomFloatValue() < num2)
						{
							this.ChangePropellant_tons(-100f, false);
							num -= 1f;
							if (num < 0f)
							{
								num = 0f;
							}
						}
					}
					this.SetPropulsionValuesDirty(true, false);
				}
				else if (TISpaceShipState.DirectDamageableSystems.Contains(key))
				{
					float num4;
					num = this.ApplyDamageToSystem(key, num, out num4);
				}
				else
				{
					ModuleDataEntry partToDamage = this.GetPartToDamage(key, false);
					float num5;
					num = this.ApplyDamageToPart(partToDamage, num, out flag, out num5);
				}
			}
			if (num > 0f && !this.ShipDestroyed())
			{
				bool flag2;
				ArmorFacing nextInteralDamageLocation = this.GetNextInteralDamageLocation(facing, originalFacing, angle, explosion || flag, weaponChipValue, out flag2);
				if (!flag2)
				{
					this.ApplyInternalDamage(nextInteralDamageLocation, originalFacing, num, explosion || flag, weaponChipValue, angle);
				}
				else
				{
					float num6 = num * weaponChipValue * TISpaceShipState.ArmorData.GetArmorTemplate(this, facing).GetSpecialtyModifiers(ArmorSpecialty.ChippingResistance) / TISpaceShipState.ArmorData.GetArmorFacingVolume_m3(this, facing);
					this.armor[facing].ChipArmor(num6);
					GameControl.eventManager.TriggerEvent(new ShipDamageGoesOutTheOtherSide(this, facing), null, new object[] { this });
				}
			}
			if (this.damagedSystems.Count > 0)
			{
				ShipSize size = this.template.size;
				float num7 = (float)(this.SystemDestroyed(ShipSystem.NoseStructure) ? (50 + this.template.size * (ShipSize)50) : (20 + this.template.size * (ShipSize)20));
				float num8 = (float)(this.SystemDestroyed(ShipSystem.TailStructure) ? (50 + this.template.size * (ShipSize)50) : (20 + this.template.size * (ShipSize)20));
				Mathf.Clamp(2f + (float)this.damagedSystems.Count * 2f, 2f, 50f);
				this.damagePoints.Add(DamageLayer.AddDamagePointInternal(new Vector3(0f, 0f, TIUtilities.RandomRange(-num8, num7)), Mathf.Clamp((float)this.damagedSystems.Count * 5f, 1f, 50f), DamageType.Thermal));
				this.damageVisualizationDirty = true;
			}
			this.spaceCombatValueDataDirty = true;
		}

		// Token: 0x06004652 RID: 18002 RVA: 0x001CB58C File Offset: 0x001C978C
		public float ApplyInternalRadiationDamage(float xRayDamage, float baryonicDamage, ArmorFacing facing, ArmorFacing originalFacing, float angle, float diffusion)
		{
			float num = 0f;
			ShipSystem key = this.internalDamageTables[facing].SelectRandomWeightedItem<KeyValuePair<ShipSystem, float>>((KeyValuePair<ShipSystem, float> x) => x.Value, -1f, 1E-37f).Key;
			if (TISpaceShipState.SoftSystems.Contains(key))
			{
				if (TISpaceShipState.DirectDamageableSystems.Contains(key))
				{
					if (baryonicDamage > 0f && !this.SystemDestroyed(key))
					{
						float num2;
						baryonicDamage = this.ApplyDamageToSystem(key, (1f - diffusion * 0.5f) * baryonicDamage * 5f, out num2) / 5f;
						num += num2;
					}
					if (xRayDamage > 0f && !this.SystemDestroyed(key))
					{
						float num3;
						xRayDamage = this.ApplyDamageToSystem(key, (1f - diffusion * 0.5f) * xRayDamage, out num3);
						num += num3;
					}
				}
				else
				{
					ModuleDataEntry partToDamage = this.GetPartToDamage(key, false);
					if (partToDamage.moduleTemplate.isHeatSink || (partToDamage.moduleTemplate.isUtilityModule && partToDamage.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RadHardened)))
					{
						xRayDamage = Mathf.Max(xRayDamage * 0.5f, 0f);
						baryonicDamage = Mathf.Max(baryonicDamage * 0.5f, 0f);
					}
					else
					{
						if (baryonicDamage > 0f && !this.PartDestroyed(partToDamage))
						{
							if (((partToDamage.moduleTemplate.isPowerPlant && partToDamage.moduleTemplate.ref_powerPlant.fissionPlant) || (partToDamage.moduleTemplate.isDrive && partToDamage.moduleTemplate.ref_drive.driveClassification == DriveClassification.NuclearSaltWater)) && TIUtilities.RandomFloatValue() < baryonicDamage / 100f)
							{
								this.ApplyInternalDamage(ArmorFacing.Core, originalFacing, 2000f, true, 0f, 0f);
								num += 20f;
							}
							else
							{
								float num5;
								float num4 = this.ApplyDamageToPart(partToDamage, baryonicDamage * 5f, out num5);
								baryonicDamage = num4 / 5f;
								num += num5 - num4;
							}
						}
						if (xRayDamage > 0f && !this.PartDestroyed(partToDamage))
						{
							float num6;
							xRayDamage = this.ApplyDamageToPart(partToDamage, xRayDamage, out num6);
							num += num6 - xRayDamage;
						}
					}
				}
			}
			else if (key == ShipSystem.Propellant && this.propellant_tons > 0f)
			{
				if (this.drive.driveClassification == DriveClassification.Fission_Pulse && baryonicDamage > 0f)
				{
					this.ChangePropellant_tons(-100f, false);
					baryonicDamage = Mathf.Max(baryonicDamage - 1f, 0f);
					if (baryonicDamage < 0f)
					{
						baryonicDamage = 0f;
					}
				}
				else if (this.propellant == Propellant.Hydrogen || this.propellant == Propellant.Water || this.propellant == Propellant.ReactionProducts)
				{
					baryonicDamage = 0f;
				}
				xRayDamage = Mathf.Max(xRayDamage * 0.5f, 0f);
			}
			else
			{
				xRayDamage = Mathf.Max(xRayDamage * 0.5f, 0f);
				baryonicDamage = Mathf.Max(baryonicDamage * 0.5f, 0f);
			}
			if (xRayDamage > 0f || baryonicDamage > 0f)
			{
				bool flag;
				ArmorFacing nextInteralDamageLocation = this.GetNextInteralDamageLocation(facing, originalFacing, angle, false, diffusion, out flag);
				if (!flag)
				{
					num += this.ApplyInternalRadiationDamage(xRayDamage, baryonicDamage, nextInteralDamageLocation, originalFacing, angle, diffusion);
				}
			}
			return num;
		}

		// Token: 0x06004653 RID: 18003 RVA: 0x001CB8BD File Offset: 0x001C9ABD
		public void SyncDamageVisuals()
		{
			if (this.damageVisualizationDirty && this.damageLayer != null)
			{
				this.damageLayer.SyncDamageVisualizations();
			}
		}

		// Token: 0x06004654 RID: 18004 RVA: 0x001CB8E0 File Offset: 0x001C9AE0
		public float GetSystemDamage(ShipSystem system)
		{
			if (this.damagedSystems.ContainsKey(system))
			{
				return this.damagedSystems[system];
			}
			if (!Enums.DamageableShipSystemsSet.Contains(system))
			{
				switch (system)
				{
				case ShipSystem.Radiators:
					return this.GetPartDamage(this.radiatorModule);
				case ShipSystem.PowerPlant:
					return this.GetPartDamage(this.powerPlantModule);
				case ShipSystem.Drive:
					return this.GetPartDamage(this.driveModule);
				}
			}
			return 0f;
		}

		// Token: 0x06004655 RID: 18005 RVA: 0x001CB958 File Offset: 0x001C9B58
		public bool SystemAsAWholeCanBeDamaged(ShipSystem system)
		{
			return system - ShipSystem.NoseWeapons > 2;
		}

		// Token: 0x06004656 RID: 18006 RVA: 0x001CB964 File Offset: 0x001C9B64
		public float GetSystemFunction(ShipSystem system)
		{
			return 1f - this.GetSystemDamage(system);
		}

		// Token: 0x06004657 RID: 18007 RVA: 0x001CB973 File Offset: 0x001C9B73
		float CombatWeaponCarrierState.FireControlFunction()
		{
			return this.GetSystemFunction(ShipSystem.FireControl);
		}

		// Token: 0x06004658 RID: 18008 RVA: 0x001CB97C File Offset: 0x001C9B7C
		public float GetPartDamage(ModuleDataEntry moduleData)
		{
			if (moduleData == null)
			{
				Log.Error("GetPartDamage() : module data was null. May be related to issue #5918", Array.Empty<object>());
			}
			else if (moduleData.moduleTemplate == null)
			{
				Log.Error("GetPartDamage() : module data was null. May be related to issue #5918", Array.Empty<object>());
			}
			DamagedShipPartData damagedShipPartData;
			if (this.damagedPartsCache.TryGetValue(moduleData, out damagedShipPartData))
			{
				return damagedShipPartData.damage;
			}
			return 0f;
		}

		// Token: 0x06004659 RID: 18009 RVA: 0x001CB9D0 File Offset: 0x001C9BD0
		public float GetPartFunction(ModuleDataEntry moduleData)
		{
			return 1f - this.GetPartDamage(moduleData);
		}

		// Token: 0x0600465A RID: 18010 RVA: 0x001CB9DF File Offset: 0x001C9BDF
		public bool PartDestroyed(ModuleDataEntry moduleData)
		{
			return this.GetPartFunction(moduleData) <= 0f;
		}

		// Token: 0x0600465B RID: 18011 RVA: 0x001CB9F2 File Offset: 0x001C9BF2
		public bool PartDamaged(ModuleDataEntry moduleData)
		{
			return this.GetPartFunction(moduleData) < 1f;
		}

		// Token: 0x0600465C RID: 18012 RVA: 0x001CBA04 File Offset: 0x001C9C04
		public bool PartDamagedButNotDestroyed(ModuleDataEntry moduleData)
		{
			float partFunction = this.GetPartFunction(moduleData);
			return partFunction > 0f && partFunction < 1f;
		}

		// Token: 0x0600465D RID: 18013 RVA: 0x001CBA2B File Offset: 0x001C9C2B
		public bool SystemDamaged(ShipSystem system)
		{
			return this.damagedSystems.ContainsKey(system) && this.damagedSystems[system] > 0f;
		}

		// Token: 0x0600465E RID: 18014 RVA: 0x001CBA50 File Offset: 0x001C9C50
		public bool SystemSeriouslyDamaged(ShipSystem system)
		{
			return this.damagedSystems.ContainsKey(system) && this.damagedSystems[system] > 0.5f;
		}

		// Token: 0x0600465F RID: 18015 RVA: 0x001CBA75 File Offset: 0x001C9C75
		public bool SystemDestroyed(ShipSystem system)
		{
			return this.damagedSystems.ContainsKey(system) && this.damagedSystems[system] >= 1f;
		}

		// Token: 0x06004660 RID: 18016 RVA: 0x001CBAA0 File Offset: 0x001C9CA0
		public bool SystemDamagedButNotDestroyed(ShipSystem system)
		{
			float systemDamage = this.GetSystemDamage(system);
			return systemDamage > 0f && systemDamage < 1f;
		}

		// Token: 0x06004661 RID: 18017 RVA: 0x001CBAC7 File Offset: 0x001C9CC7
		public bool ShipStructuralDamage()
		{
			return this.SystemSeriouslyDamaged(ShipSystem.NoseStructure) || this.SystemSeriouslyDamaged(ShipSystem.CentralStructure) || this.SystemSeriouslyDamaged(ShipSystem.TailStructure);
		}

		// Token: 0x06004662 RID: 18018 RVA: 0x001CBAE4 File Offset: 0x001C9CE4
		public bool ShipDestroyed()
		{
			return this.SystemDestroyed(ShipSystem.NoseStructure) && this.SystemDestroyed(ShipSystem.CentralStructure) && this.SystemDestroyed(ShipSystem.TailStructure);
		}

		// Token: 0x06004663 RID: 18019 RVA: 0x001CBB04 File Offset: 0x001C9D04
		public float ApplyDamageToSystem(ShipSystem system, float damagePoints, out float damagePointsToApply)
		{
			this.OnInternalDamage_Officers(system, damagePoints);
			damagePointsToApply = 0f;
			if (this.SystemDestroyed(system))
			{
				return damagePoints;
			}
			if (!this.damagedSystems.ContainsKey(system))
			{
				this.damagedSystems.Add(system, 0f);
			}
			float num = (float)this.hull.structuralIntegrity * this.damagedSystems[system];
			float num2 = (float)this.hull.structuralIntegrity - num;
			if (system == ShipSystem.VectorThrusters)
			{
				int num3 = this.utilityModuleTemplates.Count<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
				{
					TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
					if (ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.RotationalThrust))
					{
						TIUtilityModuleTemplate ref_utilityModule2 = x.ref_utilityModule;
						return ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Contains(SpecialModuleRule.ImmuneToDamage);
					}
					return false;
				});
				if (num3 > 0)
				{
					damagePoints /= (float)num3;
				}
			}
			damagePointsToApply = Mathf.Min(num2, damagePoints);
			float num4 = damagePoints / (float)this.hull.structuralIntegrity;
			Dictionary<ShipSystem, float> dictionary = this.damagedSystems;
			dictionary[system] += num4;
			if (system == ShipSystem.VectorThrusters)
			{
				this.damagedSystems[system] = Mathf.Clamp(this.damagedSystems[system], 0.05f, 1f);
			}
			else
			{
				this.damagedSystems[system] = Mathf.Min(this.damagedSystems[system], 1f);
			}
			if (system - ShipSystem.DriveCoupling <= 1)
			{
				this.SetPropulsionValuesDirty(true, false);
			}
			GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, system, false), null, new object[] { this, this.fleet });
			return damagePoints - damagePointsToApply;
		}

		// Token: 0x06004664 RID: 18020 RVA: 0x001CBC6C File Offset: 0x001C9E6C
		public void ApplyPercentDamageToSystem(ShipSystem system, float damagePercentage)
		{
			if (this.SystemDestroyed(system))
			{
				return;
			}
			if (!this.damagedSystems.ContainsKey(system))
			{
				this.damagedSystems.Add(system, 0f);
			}
			damagePercentage = Mathf.Clamp01(damagePercentage);
			this.damagedSystems[system] = damagePercentage;
			if (system == ShipSystem.VectorThrusters)
			{
				this.damagedSystems[system] = Mathf.Clamp(this.damagedSystems[system], 0.05f, 1f);
			}
			else
			{
				this.damagedSystems[system] = Mathf.Min(this.damagedSystems[system], 1f);
			}
			if (system - ShipSystem.DriveCoupling <= 1)
			{
				this.SetPropulsionValuesDirty(true, false);
			}
			GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, system, false), null, new object[] { this });
		}

		// Token: 0x06004665 RID: 18021 RVA: 0x001CBD34 File Offset: 0x001C9F34
		public float TargetingBonus(TIShipWeaponTemplate weapon, TIHabState alliedHab)
		{
			if (this.systemsDepowered)
			{
				return -2f;
			}
			if (this.targetingFrame != TIFrameCounter.FrameCount)
			{
				float num = 0f;
				List<TIUtilityModuleTemplate> functionalUtilitySlotModuleTemplates = this.GetFunctionalUtilitySlotModuleTemplates(1f);
				if (functionalUtilitySlotModuleTemplates.Count > 0)
				{
					num = functionalUtilitySlotModuleTemplates.Max<TIUtilityModuleTemplate>(delegate(TIUtilityModuleTemplate x)
					{
						TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
						if (ref_utilityModule == null)
						{
							return null;
						}
						return new float?(ref_utilityModule.targetingValue);
					}).GetValueOrDefault();
				}
				else if (this.hull.noShipyardBuild)
				{
					num += TIEffectsState.SumEffectsModifiers(Context.TargetingComputerBonus, this.faction, num, null);
				}
				num += TIEffectsState.SumEffectsModifiers(Context.GlobalTargetingBonus, this.faction, num, null);
				num += this.SumOfficerEffectsModifiers(OfficerEffectType.GlobalTargeting, num);
				if (weapon != null)
				{
					if (weapon.isBeamWeapon)
					{
						num += this.SumOfficerEffectsModifiers(OfficerEffectType.BeamTargeting, num);
					}
					else if (weapon.isGunTypeWeapon)
					{
						num += this.SumOfficerEffectsModifiers(OfficerEffectType.GunTargeting, num);
					}
					else if (weapon.isMissileWeapon)
					{
						num += this.SumOfficerEffectsModifiers(OfficerEffectType.MissileTargeting, num);
					}
				}
				num += ((alliedHab != null) ? alliedHab.FleetTargetingBonus() : 0f);
				num -= 1f - this.GetSystemFunction(ShipSystem.FireControl);
				num -= 1f - this.GetSystemFunction(ShipSystem.Sensors);
				this._cacheTargetingBonus = num;
				this.targetingFrame = TIFrameCounter.FrameCount;
			}
			return this._cacheTargetingBonus;
		}

		// Token: 0x06004666 RID: 18022 RVA: 0x001CBE7C File Offset: 0x001CA07C
		public float ECMValue(TIFactionState attacker, TIHabState alliedHab)
		{
			if (this.systemsDepowered)
			{
				return 0f;
			}
			if ((this.ECMFrame != TIFrameCounter.FrameCount || this.spaceCombatValueDataDirty) && attacker != null && (attacker.IsActiveHumanFaction || TIEffectsState.CheckForAnyEffectInContext(Context.HumanECMAgainstAliens, this.faction)))
			{
				float num = 0f;
				List<TIUtilityModuleTemplate> functionalUtilitySlotModuleTemplates = this.GetFunctionalUtilitySlotModuleTemplates(1f);
				if (functionalUtilitySlotModuleTemplates.Count > 0)
				{
					num += functionalUtilitySlotModuleTemplates.Max<TIUtilityModuleTemplate>(delegate(TIUtilityModuleTemplate x)
					{
						TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
						if (ref_utilityModule == null)
						{
							return 0f;
						}
						return ref_utilityModule.ECMValue;
					});
				}
				else if (this.hull.noShipyardBuild)
				{
					num += TIEffectsState.SumEffectsModifiers(Context.STOFighterECM, this.faction, num, null);
				}
				num += TIEffectsState.SumEffectsModifiers(Context.GlobalECMBonus, this.faction, num, null);
				num += this.SumOfficerEffectsModifiers(OfficerEffectType.ECM, num);
				num += ((alliedHab != null) ? alliedHab.FleetECMBonus() : 0f);
				this._cacheECMValue = num;
				this.ECMFrame = TIFrameCounter.FrameCount;
			}
			return this._cacheECMValue;
		}

		// Token: 0x06004667 RID: 18023 RVA: 0x001CBF8C File Offset: 0x001CA18C
		public void ApplyThermalShredInStrategyLayer(int shreddingPoints)
		{
			float num;
			float num2;
			this.ApplyDamage(null, ArmorFacing.Nose, 0f, 0f, 0f, DamageType.Thermal, 0f, null, out num, out num2, shreddingPoints);
			if (num > 0f && this.ShipDestroyed())
			{
				GameControl.eventManager.TriggerEvent(new ShipDestroyedByHeat(this), null, new object[] { this, this.fleet });
				TINotificationQueueState.LogShipDestroyedInStrat(this, new List<TIFactionState> { this.faction }, this.fleet.location, new Dictionary<TIFactionState, string> { 
				{
					this.faction,
					this.KillAllOfficersReport()
				} });
				this.DestroyShip(true, this.faction);
			}
		}

		// Token: 0x06004668 RID: 18024 RVA: 0x001CC036 File Offset: 0x001CA236
		public float GetCrossSectionalArea_m2(float angle_degrees = -3.4028235E+38f)
		{
			return this.template.GetCrossSectionalArea_m2(angle_degrees);
		}

		// Token: 0x06004669 RID: 18025 RVA: 0x001CC044 File Offset: 0x001CA244
		public ShipSystem GetSystemTypeFromModuleData(ModuleDataEntry module)
		{
			if (module.moduleTemplate.isDrive)
			{
				return ShipSystem.Drive;
			}
			if (module.moduleTemplate.isWeapon)
			{
				if (module.weaponTemplate.noseWeapon)
				{
					return ShipSystem.NoseWeapons;
				}
				return ShipSystem.HullWeapons;
			}
			else
			{
				if (module.moduleTemplate.isUtilityModule || module.moduleTemplate.isHeatSink || module.moduleTemplate.isBattery)
				{
					return ShipSystem.UtilityModules;
				}
				if (module.moduleTemplate.isPowerPlant)
				{
					return ShipSystem.PowerPlant;
				}
				if (module.moduleTemplate.isRadiator)
				{
					return ShipSystem.Radiators;
				}
				return ShipSystem.None;
			}
		}

		// Token: 0x0600466A RID: 18026 RVA: 0x001CC0CC File Offset: 0x001CA2CC
		public ModuleDataEntry GetPartToDamage(ShipSystem system, bool suppressError = false)
		{
			ModuleDataEntry moduleDataEntry = new ModuleDataEntry();
			switch (system)
			{
			case ShipSystem.NoseWeapons:
			{
				Dictionary<ModuleDataEntry, float> dictionary = new Dictionary<ModuleDataEntry, float>();
				foreach (ModuleDataEntry moduleDataEntry2 in this.template.noseWeapons)
				{
					dictionary.Add(moduleDataEntry2, (float)moduleDataEntry2.moduleTemplate.internalSize);
				}
				moduleDataEntry = dictionary.SelectRandomWeightedItem<KeyValuePair<ModuleDataEntry, float>>((KeyValuePair<ModuleDataEntry, float> x) => x.Value, -1f, 1E-37f).Key;
				break;
			}
			case ShipSystem.HullWeapons:
			{
				Dictionary<ModuleDataEntry, float> dictionary2 = new Dictionary<ModuleDataEntry, float>();
				foreach (ModuleDataEntry moduleDataEntry3 in this.template.hullWeapons)
				{
					dictionary2.Add(moduleDataEntry3, (float)moduleDataEntry3.moduleTemplate.internalSize);
				}
				moduleDataEntry = dictionary2.SelectRandomWeightedItem<KeyValuePair<ModuleDataEntry, float>>((KeyValuePair<ModuleDataEntry, float> x) => x.Value, -1f, 1E-37f).Key;
				break;
			}
			case ShipSystem.UtilityModules:
			{
				Dictionary<ModuleDataEntry, float> dictionary3 = new Dictionary<ModuleDataEntry, float>();
				foreach (ModuleDataEntry moduleDataEntry4 in this.template.utilityModules)
				{
					TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry4.moduleTemplate.ref_utilityModule;
					if (ref_utilityModule == null || !ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.ImmuneToDamage))
					{
						dictionary3.Add(moduleDataEntry4, (float)moduleDataEntry4.moduleTemplate.internalSize);
					}
				}
				moduleDataEntry = dictionary3.SelectRandomWeightedItem<KeyValuePair<ModuleDataEntry, float>>((KeyValuePair<ModuleDataEntry, float> x) => x.Value, -1f, 1E-37f).Key;
				break;
			}
			case ShipSystem.Radiators:
				moduleDataEntry = this.radiatorModule;
				break;
			case ShipSystem.PowerPlant:
				moduleDataEntry = this.powerPlantModule;
				break;
			case ShipSystem.Drive:
				moduleDataEntry = this.driveModule;
				break;
			}
			if (!suppressError)
			{
				if (moduleDataEntry == null)
				{
					Log.Error("Failed to find module template to damage. Null module. Assigned System: " + system.ToString(), Array.Empty<object>());
				}
				else if (moduleDataEntry.moduleTemplate == null)
				{
					Log.Error("Failed to find module template to damage. Null module template. Assigned System: " + system.ToString(), Array.Empty<object>());
				}
			}
			return moduleDataEntry;
		}

		// Token: 0x0600466B RID: 18027 RVA: 0x001CC368 File Offset: 0x001CA568
		public void DestroyPart(ModuleDataEntry part)
		{
			float num;
			this.ApplyDamageToPart(part, part.moduleTemplate.hitPoints, out num);
		}

		// Token: 0x0600466C RID: 18028 RVA: 0x001CC38C File Offset: 0x001CA58C
		public float ApplyDamageToPart(ModuleDataEntry moduleData, float damageValue, out float internalDamageApplied)
		{
			bool flag;
			return this.ApplyDamageToPart(moduleData, damageValue, out flag, out internalDamageApplied);
		}

		// Token: 0x0600466D RID: 18029 RVA: 0x001CC3A4 File Offset: 0x001CA5A4
		public float ApplyDamageToPart(ModuleDataEntry moduleData, float damageValue, out bool secondaryExplosion, out float damagePointsApplied)
		{
			damagePointsApplied = 0f;
			if (moduleData == null || moduleData.moduleTemplate == null)
			{
				Log.Error("Null part sent to ApplyDamageToPart", Array.Empty<object>());
				secondaryExplosion = false;
				return 0f;
			}
			this.OnInternalDamage_Officers(moduleData.moduleTemplate, damageValue);
			if (damageValue > 0f && !this.PartDestroyed(moduleData))
			{
				TIShipPartTemplate moduleTemplate = moduleData.moduleTemplate;
				float num = 0f;
				foreach (DamagedShipPartData damagedShipPartData in this.damagedParts)
				{
					if (damagedShipPartData.SamePart(moduleData.moduleTemplate, moduleData.slotIndex))
					{
						num = damagedShipPartData.damage;
						break;
					}
				}
				this.SetPartDamage(moduleData, num + damageValue / moduleTemplate.hitPoints, false);
				damagePointsApplied += Mathf.Min(moduleTemplate.hitPoints - moduleTemplate.hitPoints * num, damageValue);
				bool flag = damagePointsApplied > 0f && moduleTemplate.Explosive(this, moduleData);
				if (flag)
				{
					int num2 = TIUtilities.RandomRange(1, 4) + TIUtilities.RandomRange(1, 4);
					if (moduleTemplate.HighlyExplosive(this))
					{
						num2 *= TIUtilities.RandomRange(2, 12);
					}
					damageValue += (float)num2;
					GameControl.eventManager.TriggerEvent(new ShipSecondaryExplosion(this, moduleData), null, new object[] { this });
				}
				secondaryExplosion = flag;
				return Mathf.Abs(damageValue - damagePointsApplied);
			}
			secondaryExplosion = false;
			return damageValue;
		}

		// Token: 0x0600466E RID: 18030 RVA: 0x001CC510 File Offset: 0x001CA710
		public void ApplyPercentDamageToPart(ModuleDataEntry moduleData, float damagePercentage)
		{
			damagePercentage = Mathf.Clamp01(damagePercentage);
			if (moduleData.moduleTemplate == null)
			{
				Log.Error("Null part sent to ApplyPercentDamageToPart", Array.Empty<object>());
				return;
			}
			if (damagePercentage > 0f && !this.PartDestroyed(moduleData))
			{
				using (List<DamagedShipPartData>.Enumerator enumerator = this.damagedParts.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.SamePart(moduleData.moduleTemplate, moduleData.slotIndex))
						{
							break;
						}
					}
				}
				this.SetPartDamage(moduleData, damagePercentage, false);
				return;
			}
		}

		// Token: 0x0600466F RID: 18031 RVA: 0x001CC5AC File Offset: 0x001CA7AC
		public List<ModuleDataEntry> GetFunctionalUtilitySlotModules(float minHealth)
		{
			return this.utilityModules.Where<ModuleDataEntry>((ModuleDataEntry x) => x.moduleTemplate.ref_utilityModule != null && this.GetPartFunction(x) >= minHealth).ToList<ModuleDataEntry>();
		}

		// Token: 0x06004670 RID: 18032 RVA: 0x001CC5EC File Offset: 0x001CA7EC
		public List<TIUtilityModuleTemplate> GetFunctionalUtilitySlotModuleTemplates(float minHealth)
		{
			if (this._cachedFunctionalUtilitySlotModulesFrame != TIFrameCounter.FrameCount || minHealth < 1f)
			{
				List<ModuleDataEntry> functionalUtilitySlotModules = this.GetFunctionalUtilitySlotModules(minHealth);
				if (minHealth < 1f)
				{
					return functionalUtilitySlotModules.Select<ModuleDataEntry, TIUtilityModuleTemplate>((ModuleDataEntry x) => x.moduleTemplate.ref_utilityModule).ToList<TIUtilityModuleTemplate>();
				}
				this._cachedFunctionalUtilitySlotModules = new List<TIUtilityModuleTemplate>();
				foreach (ModuleDataEntry moduleDataEntry in functionalUtilitySlotModules)
				{
					this._cachedFunctionalUtilitySlotModules.Add(moduleDataEntry.moduleTemplate.ref_utilityModule);
				}
				this._cachedFunctionalUtilitySlotModulesFrame = TIFrameCounter.FrameCount;
			}
			return this._cachedFunctionalUtilitySlotModules;
		}

		// Token: 0x17000DDE RID: 3550
		// (get) Token: 0x06004671 RID: 18033 RVA: 0x001CC6BC File Offset: 0x001CA8BC
		public double sideways_acceleration
		{
			get
			{
				return (double)(Mathf.Max(this.maneuverThrust_N, 1000f) * 2f / this.currentMass_kg);
			}
		}

		// Token: 0x17000DDF RID: 3551
		// (get) Token: 0x06004672 RID: 18034 RVA: 0x001CC6DC File Offset: 0x001CA8DC
		public float maneuverThrust_N
		{
			get
			{
				if (!this.isAlien)
				{
					return 2500000f + this.utilityModules.Sum<ModuleDataEntry>(delegate(ModuleDataEntry x)
					{
						TIUtilityModuleTemplate ref_utilityModule = x.moduleTemplate.ref_utilityModule;
						return (((ref_utilityModule != null) ? new float?(ref_utilityModule.vectorThrustBonus) : null) * this.GetPartFunction(x)).GetValueOrDefault();
					}) * this.damage_vectorThrustModifier;
				}
				return 4000000f * this.damage_vectorThrustModifier;
			}
		}

		// Token: 0x06004673 RID: 18035 RVA: 0x001CC718 File Offset: 0x001CA918
		private void SetAngularAcceleration_rads2(float overrideMass_kg = -1f)
		{
			float num = this.maneuverThrust_N * 2f * this.template.hullTemplate.length_m / 2f;
			float num2;
			if (overrideMass_kg <= 0f)
			{
				num2 = 0.083333336f * this.currentMass_kg * Mathf.Pow(this.template.hullTemplate.length_m, 2f);
			}
			else
			{
				num2 = 0.083333336f * overrideMass_kg * Mathf.Pow(this.template.hullTemplate.length_m, 2f);
			}
			if (num2 > 0f && num > 0f)
			{
				this.angular_acceleration_rads2 = Mathf.Max(num / num2, 0.001f);
			}
			else
			{
				this.angular_acceleration_rads2 = 0.001f;
			}
			if (Mathf.Abs(this.angularAcceleration_degs2) == float.PositiveInfinity)
			{
				this.angular_acceleration_rads2 = 0.001f;
			}
		}

		// Token: 0x06004674 RID: 18036 RVA: 0x001CC7EB File Offset: 0x001CA9EB
		public void ActivateThrusters()
		{
			if (!this.thrustersActive)
			{
				this.thrustersActive = true;
				this.SetVisualizationDataDirty();
			}
		}

		// Token: 0x06004675 RID: 18037 RVA: 0x001CC802 File Offset: 0x001CAA02
		public void DeactivateThrusters()
		{
			if (this.thrustersActive)
			{
				this.thrustersActive = false;
				this.SetVisualizationDataDirty();
			}
		}

		// Token: 0x17000DE0 RID: 3552
		// (get) Token: 0x06004676 RID: 18038 RVA: 0x001CC819 File Offset: 0x001CAA19
		public bool canEverRetractRadiators
		{
			get
			{
				return this.currentHeatSinkCapacity_GJ > 0f;
			}
		}

		// Token: 0x17000DE1 RID: 3553
		// (get) Token: 0x06004677 RID: 18039 RVA: 0x001CC828 File Offset: 0x001CAA28
		public bool canIssueRetractRadiatorsCommand
		{
			get
			{
				return this.canEverRetractRadiators && this.radiatorsExtended && !this.radiatorsExtending && !this.radiatorsRetracting;
			}
		}

		// Token: 0x17000DE2 RID: 3554
		// (get) Token: 0x06004678 RID: 18040 RVA: 0x001CC84D File Offset: 0x001CAA4D
		public bool canIssueExtendRadiatorsCommand
		{
			get
			{
				return !this.radiatorsExtended && !this.radiatorsExtending && !this.radiatorsRetracting;
			}
		}

		// Token: 0x06004679 RID: 18041 RVA: 0x001CC86A File Offset: 0x001CAA6A
		public void RetractRadiators()
		{
			this.radiatorsExtended = false;
		}

		// Token: 0x0600467A RID: 18042 RVA: 0x001CC873 File Offset: 0x001CAA73
		public void ExtendRadiators()
		{
			this.radiatorsExtended = true;
		}

		// Token: 0x0600467B RID: 18043 RVA: 0x001CC87C File Offset: 0x001CAA7C
		public void InitiateExtendRadiators()
		{
			string text = "";
			switch (this.radiators.radiatorType)
			{
			case RadiatorType.Fin:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Fin_Radiator_Extending";
				break;
			case RadiatorType.Droplet:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Droplet_Radiator_Extending";
				break;
			case RadiatorType.Spike:
			case RadiatorType.AlienSpike:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Spike_Radiator_Extending";
				break;
			}
			if (this.visualizerLink != null)
			{
				if (this.radiatorEventInstance.isValid())
				{
					this.radiatorEventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
					this.radiatorEventInstance.Release();
				}
				this.radiatorEventInstance = AudioManager.CreateFMODInstance(text);
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					this.radiatorEventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.radiatorEventInstance), AudioManager.GetCombatAudioMinDistance(this.radiatorEventInstance));
				}
				this.radiatorEventInstance.setParameterByName("END", 0f, false);
				this.radiatorEventInstance.setPaused(TIGlobalValuesState.isSpaceCombatEnabled && World.Active.GetExistingManager<GameTimeManager>().currentSpeed == 0f);
				RuntimeManager.AttachInstanceToGameObject(this.radiatorEventInstance, this.visualizerLink.transform);
				this.radiatorEventInstance.Play();
				GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnRadiatorGameTimeSpeedChanged), null);
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnRadiatorGameTimeSpeedChanged), null, null, true, false);
			}
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddSeconds(60.0);
			bool isSpaceCombatEnabled = TIGlobalValuesState.isSpaceCombatEnabled;
			TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, "Ship Extend Radiators Complete", isSpaceCombatEnabled, false, TITimeQueueRepeatType.None, 1, true, TIGlobalValuesState.isSpaceCombatEnabled);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteExtendRadiators), "Ship Extend Radiators Complete", null, true, false);
			GameControl.eventManager.TriggerEvent(new InitiateExtendRadiatorsEvent(this), null, new object[] { this });
			this.radiatorsExtending = true;
		}

		// Token: 0x0600467C RID: 18044 RVA: 0x001CCA40 File Offset: 0x001CAC40
		public void InitiateRetractRadiators()
		{
			string text = "";
			switch (this.radiators.radiatorType)
			{
			case RadiatorType.Fin:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Fin_Radiator_Retracting";
				break;
			case RadiatorType.Droplet:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Droplet_Radiator_Retracting";
				break;
			case RadiatorType.Spike:
			case RadiatorType.AlienSpike:
				text = "event:/SFX/Game_SFX/Ship_Radiators/trig_SFX_Spike_Radiator_Retracting";
				break;
			}
			if (this.visualizerLink != null)
			{
				if (this.radiatorEventInstance.isValid())
				{
					this.radiatorEventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
					this.radiatorEventInstance.Release();
				}
				this.radiatorEventInstance = AudioManager.CreateFMODInstance(text);
				if (TIGlobalValuesState.isSpaceCombatEnabled)
				{
					this.radiatorEventInstance.SetDistance(AudioManager.GetCombatAudioMaxDistance(this.radiatorEventInstance), AudioManager.GetCombatAudioMinDistance(this.radiatorEventInstance));
				}
				this.radiatorEventInstance.setParameterByName("END", 0f, false);
				this.radiatorEventInstance.setPaused(TIGlobalValuesState.isSpaceCombatEnabled && World.Active.GetExistingManager<GameTimeManager>().currentSpeed == 0f);
				RuntimeManager.AttachInstanceToGameObject(this.radiatorEventInstance, this.visualizerLink.transform);
				this.radiatorEventInstance.Play();
				GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnRadiatorGameTimeSpeedChanged), null);
				GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnRadiatorGameTimeSpeedChanged), null, null, true, false);
			}
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddSeconds(60.0);
			bool isSpaceCombatEnabled = TIGlobalValuesState.isSpaceCombatEnabled;
			TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, null, "Ship Retract Radiators Complete", isSpaceCombatEnabled, false, TITimeQueueRepeatType.None, 1, true, TIGlobalValuesState.isSpaceCombatEnabled);
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.CompleteRetractRadiators), "Ship Retract Radiators Complete", null, true, false);
			GameControl.eventManager.TriggerEvent(new InitiateRetractRadiatorsEvent(this), null, new object[] { this });
			this.radiatorsRetracting = true;
		}

		// Token: 0x0600467D RID: 18045 RVA: 0x001CCC04 File Offset: 0x001CAE04
		public void CompleteExtendRadiators(TimeEventStart e)
		{
			if (this.radiatorEventInstance.isValid())
			{
				this.radiatorEventInstance.setParameterByName("END", 1f, false);
			}
			if (e.eventObject == this)
			{
				this.radiatorsExtending = false;
				this.ExtendRadiators();
				GameControl.eventManager.TriggerEvent(new CompleteExtendRadiatorsEvent(this), null, new object[] { this });
			}
		}

		// Token: 0x0600467E RID: 18046 RVA: 0x001CCC6C File Offset: 0x001CAE6C
		public void CompleteRetractRadiators(TimeEventStart e)
		{
			if (this.radiatorEventInstance.isValid())
			{
				this.radiatorEventInstance.setParameterByName("END", 1f, false);
			}
			if (e.eventObject == this)
			{
				this.radiatorsRetracting = false;
				this.RetractRadiators();
				GameControl.eventManager.TriggerEvent(new CompleteRetractRadiatorsEvent(this), null, new object[] { this });
			}
		}

		// Token: 0x0600467F RID: 18047 RVA: 0x001CCCD3 File Offset: 0x001CAED3
		private void OnRadiatorGameTimeSpeedChanged(GameTimeSpeedChanged e)
		{
			if (this.radiatorEventInstance.isValid())
			{
				this.radiatorEventInstance.setPaused(TIGlobalValuesState.isSpaceCombatEnabled && World.Active.GetExistingManager<GameTimeManager>().currentSpeed == 0f);
			}
		}

		// Token: 0x06004680 RID: 18048 RVA: 0x001CCD0E File Offset: 0x001CAF0E
		public void ClearRadiatorAudio()
		{
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnRadiatorGameTimeSpeedChanged), null);
			if (this.radiatorEventInstance.isValid())
			{
				this.radiatorEventInstance.Stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
				this.radiatorEventInstance.Release();
			}
		}

		// Token: 0x06004681 RID: 18049 RVA: 0x001CCD50 File Offset: 0x001CAF50
		public float FleetMissionControlMultiplier()
		{
			float num = 1f;
			foreach (ModuleDataEntry moduleDataEntry in this.utilityModules)
			{
				TIUtilityModuleTemplate ref_utilityModule = moduleDataEntry.moduleTemplate.ref_utilityModule;
				if (ref_utilityModule != null && ref_utilityModule.fleetMCValue < num && this.GetPartFunction(moduleDataEntry) == 1f)
				{
					num = moduleDataEntry.moduleTemplate.ref_utilityModule.fleetMCValue;
				}
			}
			if (num < 1f)
			{
				num += this.SumOfficerEffectsModifiers(OfficerEffectType.FleetMissionControl, num);
			}
			return num;
		}

		// Token: 0x17000DE3 RID: 3555
		// (get) Token: 0x06004682 RID: 18050 RVA: 0x001CCDF4 File Offset: 0x001CAFF4
		private bool AllowBoostForRepairsResupply
		{
			get
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet == null)
				{
					TIHabState ref_hab = this.ref_hab;
					return ref_hab != null && ref_hab.GetSunOrbitingRelatedObject.isEarth;
				}
				return fleet.AllowUseBoostForRepairsResupply;
			}
		}

		// Token: 0x06004683 RID: 18051 RVA: 0x001CCE1C File Offset: 0x001CB01C
		public void InstantFullRepair()
		{
			foreach (ArmorFacing armorFacing in this.armor.Keys)
			{
				this.armor[armorFacing].RepairArmor();
			}
			this.damagedParts.Clear();
			this.damagedPartsCache.Clear();
			this.damagedSystems.Clear();
			this.ChargeBatteriesToMax();
			this.LoadAmmo();
			this.RePropellantToMax();
		}

		// Token: 0x06004684 RID: 18052 RVA: 0x001CCEB4 File Offset: 0x001CB0B4
		public void RePropellantToMax()
		{
			this.SetPropellant_tons(this.template.propellantMass_tons, true);
			this.SetCurrentDeltaVFromPropellantMass();
			GameControl.eventManager.TriggerEvent(new ShipResupplied(this), null, new object[] { this, this.fleet });
		}

		// Token: 0x06004685 RID: 18053 RVA: 0x001CCEF4 File Offset: 0x001CB0F4
		public bool CanRefuelFromJovianAtmosphere()
		{
			if (this.SpecialModuleRules(false).Contains(SpecialModuleRule.RefuelFromAtmospheres))
			{
				TIOrbitState orbitState = this.fleet.orbitState;
				if (orbitState != null && orbitState.interfaceOrbit)
				{
					TISpaceBodyState ref_spaceBody = this.fleet.orbitState.ref_spaceBody;
					if (ref_spaceBody != null && ref_spaceBody.atmosphere == Atmosphere.Massive)
					{
						return this.drive.propellant == Propellant.Anything || (this.drive.propellant == Propellant.Hydrogen && this.drive.GetPerTankPropellantMaterials(this.faction).water + this.drive.GetPerTankPropellantMaterials(this.faction).volatiles >= 1f);
					}
				}
			}
			return false;
		}

		// Token: 0x06004686 RID: 18054 RVA: 0x001CCFA8 File Offset: 0x001CB1A8
		public bool CanRefuelFromHabSite(TIHabSiteState site)
		{
			if (site != null)
			{
				ResourceCostBuilder perTankPropellantMaterials = this.drive.GetPerTankPropellantMaterials(this.faction);
				return (this.SpecialModuleRules(false).Contains(SpecialModuleRule.RefuelFromUnimprovedSites) || this.drive.freeISRU) && (this.drive.propellant == Propellant.Anything || ((perTankPropellantMaterials.water == 0f || site.water_day > 0f) && (perTankPropellantMaterials.volatiles == 0f || site.volatiles_day > 0f) && (perTankPropellantMaterials.metals == 0f || site.metals_day > 0f) && (perTankPropellantMaterials.nobleMetals == 0f || site.nobles_day > 0f) && (perTankPropellantMaterials.fissiles == 0f || site.fissiles_day > 0f) && perTankPropellantMaterials.exotics == 0f && perTankPropellantMaterials.antimatter == 0f));
			}
			return false;
		}

		// Token: 0x06004687 RID: 18055 RVA: 0x001CD0AA File Offset: 0x001CB2AA
		public bool AI_NeedsRefuelBadly(float minLocalFunctionalDV_kps)
		{
			return this.propellant_tons < this.template.propellantMass_tons * 0.5f && this.currentDeltaV_kps < minLocalFunctionalDV_kps;
		}

		// Token: 0x06004688 RID: 18056 RVA: 0x001CD0D0 File Offset: 0x001CB2D0
		public bool AI_NeedsRefuel()
		{
			return this.propellant_tons < this.template.propellantMass_tons * 0.99f;
		}

		// Token: 0x06004689 RID: 18057 RVA: 0x001CD0EB File Offset: 0x001CB2EB
		public bool NeedsRefuel()
		{
			return this.propellant_tons < this.template.propellantMass_tons * 0.999999f;
		}

		// Token: 0x17000DE4 RID: 3556
		// (get) Token: 0x0600468A RID: 18058 RVA: 0x001CD106 File Offset: 0x001CB306
		public float PropellantShortage_tons
		{
			get
			{
				return this.template.propellantMass_tons - this.propellant_tons;
			}
		}

		// Token: 0x0600468B RID: 18059 RVA: 0x001CD11A File Offset: 0x001CB31A
		public void RefuelPropellant(float tons)
		{
			this.ChangePropellant_tons(tons, false);
			this.SetCurrentDeltaVFromPropellantMass();
		}

		// Token: 0x0600468C RID: 18060 RVA: 0x001CD12C File Offset: 0x001CB32C
		public TIResourcesCost GetPreferredPropellantTankCost(TIFactionState faction, float tonsToFill, bool textMixed)
		{
			float num = ((tonsToFill < 100f) ? (tonsToFill / 100f) : 1f);
			TIResourcesCost tiresourcesCost = this.template.singlePropellantTankCost(faction, num);
			if (textMixed && !tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity))
			{
				TISpaceFleetState fleet = this.fleet;
				if (fleet != null && fleet.dockedAtHab && this.AllowBoostForRepairsResupply)
				{
					tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(faction, this.fleet.ref_hab, tiresourcesCost, faction.AvailableSpaceResources(1f), false);
				}
			}
			return tiresourcesCost;
		}

		// Token: 0x0600468D RID: 18061 RVA: 0x001CD1B1 File Offset: 0x001CB3B1
		public float GetPropellantTonsForDesiredDv(float desiredDv)
		{
			return (float)((double)Mathf.Exp(desiredDv / this.currentEV_kps) * this.dryMass_tons - this.dryMass_tons);
		}

		// Token: 0x0600468E RID: 18062 RVA: 0x001CD1D0 File Offset: 0x001CB3D0
		public bool NeedsRearm()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				TIProjectileWeaponTemplate ref_projectileWeapon = moduleDataEntry.moduleTemplate.ref_projectileWeapon;
				if (ref_projectileWeapon != null && ref_projectileWeapon.hasMagazine() && this.ammo[moduleDataEntry] < ref_projectileWeapon.FullAmmoCount_Current(this))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600468F RID: 18063 RVA: 0x001CD250 File Offset: 0x001CB450
		public bool AI_NeedsRearmBadly()
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				TIProjectileWeaponTemplate ref_projectileWeapon = moduleDataEntry.moduleTemplate.ref_projectileWeapon;
				if (ref_projectileWeapon != null && ref_projectileWeapon.hasMagazine() && (float)this.ammo[moduleDataEntry] < (float)ref_projectileWeapon.FullAmmoCount_Current(this) / 2f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004690 RID: 18064 RVA: 0x001CD2D8 File Offset: 0x001CB4D8
		public bool AllWeaponsDry()
		{
			bool flag = false;
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				if (moduleDataEntry.moduleTemplate.ref_weapon.hasMagazine())
				{
					flag = true;
					if (this.ammo[moduleDataEntry] > 0)
					{
						return false;
					}
				}
			}
			return flag;
		}

		// Token: 0x06004691 RID: 18065 RVA: 0x001CD358 File Offset: 0x001CB558
		public bool AI_InvoluntaryNoncombatant()
		{
			if (this.template.combatant)
			{
				return (from x in this.AllWeaponModuleData()
					where x.weaponTemplate.attackMode
					select x).None<ModuleDataEntry>((ModuleDataEntry x) => this.WeaponIsOperable(x));
			}
			return false;
		}

		// Token: 0x06004692 RID: 18066 RVA: 0x001CD3B0 File Offset: 0x001CB5B0
		public bool CanAffordAnyReload(TIHabState hab)
		{
			foreach (ModuleDataEntry moduleDataEntry in this.AllWeaponModuleData())
			{
				TIProjectileWeaponTemplate ref_projectileWeapon = moduleDataEntry.moduleTemplate.ref_projectileWeapon;
				if (ref_projectileWeapon != null && ref_projectileWeapon.hasMagazine() && this.ammo[moduleDataEntry] < moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(this))
				{
					int num = Math.Min(50, moduleDataEntry.moduleTemplate.ref_projectileWeapon.FullAmmoCount_Current(this) - this.ammo[moduleDataEntry]);
					float num2 = moduleDataEntry.moduleTemplate.ref_projectileWeapon.ammoMass_kg * ((float)num / 1000f) * TemplateManager.global.spaceResourceToTons;
					TIResourcesCost tiresourcesCost = moduleDataEntry.moduleTemplate.ref_projectileWeapon.ammoMaterials.ToResourcesCost(num2);
					if (!tiresourcesCost.CanAfford(this.faction, 1f, null, float.PositiveInfinity) && hab != null && this.AllowBoostForRepairsResupply)
					{
						tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(this.faction, hab, tiresourcesCost, this.faction.AvailableSpaceResources(1f), false);
					}
					if (tiresourcesCost.CanAfford(this.faction, 1f, null, float.PositiveInfinity))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004693 RID: 18067 RVA: 0x001CD520 File Offset: 0x001CB720
		public TIResourcesCost CostToReloadPartialAmmo(ModuleDataEntry weaponModuleData, int ammoToReload, TIHabState hab, bool testBoost)
		{
			float num = weaponModuleData.moduleTemplate.ref_projectileWeapon.ammoMass_kg * ((float)ammoToReload / 1000f) * TemplateManager.global.spaceResourceToTons;
			TIResourcesCost tiresourcesCost = weaponModuleData.moduleTemplate.ref_projectileWeapon.ammoMaterials.ToResourcesCost(num);
			if (testBoost && !tiresourcesCost.CanAfford(this.faction, 1f, null, float.PositiveInfinity) && hab != null && this.AllowBoostForRepairsResupply)
			{
				tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(this.faction, hab, tiresourcesCost, this.faction.AvailableSpaceResources(1f), false);
			}
			return tiresourcesCost;
		}

		// Token: 0x17000DE5 RID: 3557
		// (get) Token: 0x06004694 RID: 18068 RVA: 0x001CD5B8 File Offset: 0x001CB7B8
		public TISpaceShipTemplate BestExistingRefit
		{
			get
			{
				if (this.bestExistingRefitCachedFrame != TIFrameCounter.FrameCount)
				{
					ValueTuple<TISpaceShipTemplate, float> valueTuple = (from shipTemplate in this.faction.GetShipDesignsThreadSafe()
						select new ValueTuple<TISpaceShipTemplate, float>(shipTemplate, this.template.GetRelativeValueOfRefit(shipTemplate))).MaxBy<ValueTuple<TISpaceShipTemplate, float>, float>(([TupleElementNames(new string[] { "shipTemplate", null })] ValueTuple<TISpaceShipTemplate, float> x) => x.Item2);
					if (valueTuple.Item2 < 1f)
					{
						this.cachedBestExistingRefit = null;
					}
					else
					{
						this.cachedBestExistingRefit = valueTuple.Item1;
					}
					this.bestExistingRefitCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedBestExistingRefit;
			}
		}

		// Token: 0x17000DE6 RID: 3558
		// (get) Token: 0x06004695 RID: 18069 RVA: 0x001CD646 File Offset: 0x001CB846
		public bool CanRefit
		{
			get
			{
				return this.BestExistingRefit != null;
			}
		}

		// Token: 0x17000DE7 RID: 3559
		// (get) Token: 0x06004696 RID: 18070 RVA: 0x001CD651 File Offset: 0x001CB851
		public bool NeedsRefit
		{
			get
			{
				return !this.faction.IsAlienFaction && (double)this.template.GetRelativeValueOfRefit(this.BestExistingRefit) > 1.4;
			}
		}

		// Token: 0x06004697 RID: 18071 RVA: 0x001CD680 File Offset: 0x001CB880
		public TIResourcesCost SystemRepairCost(ShipSystem system, TIFactionState payingFaction, TIHabState repairingHab, bool testBoost)
		{
			TIResourcesCost tiresourcesCost;
			switch (system)
			{
			case ShipSystem.NoseStructure:
			case ShipSystem.CentralStructure:
			case ShipSystem.TailStructure:
				tiresourcesCost = this.hull.buildCost(0f, 0f).MultiplyCost(0.1f * this.GetSystemDamage(system));
				tiresourcesCost.SetCompletionTime_Days(this.hull.baseConstructionTime_days * 0.1f * this.GetSystemDamage(system));
				goto IL_01E8;
			case ShipSystem.Bridge:
				tiresourcesCost = this.hull.buildCost(0f, 0f).MultiplyCost(0.01f * this.GetSystemDamage(system));
				tiresourcesCost.SetCompletionTime_Days((float)(2 * this.hull.consTier) * this.GetSystemDamage(system));
				goto IL_01E8;
			case ShipSystem.FireControl:
			case ShipSystem.DamageControl:
				tiresourcesCost = this.hull.buildCost(0f, 0f).MultiplyCost(0.005f * this.GetSystemDamage(system));
				tiresourcesCost.SetCompletionTime_Days((float)this.hull.consTier * this.GetSystemDamage(system));
				goto IL_01E8;
			case ShipSystem.SystemsReactor:
				tiresourcesCost = this.powerPlant.buildCost(this._systemsPowerGenerationRequirement_GW + this._weaponsPowerGenerationRequirement_GW, 0f).MultiplyCost(this.GetSystemDamage(system));
				tiresourcesCost.SetCompletionTime_Days(2f * this.GetSystemDamage(system));
				goto IL_01E8;
			case ShipSystem.LifeSupportMain:
			case ShipSystem.LifeSupportBackup:
				tiresourcesCost = this.hull.buildCost(0f, 0f).CreateSingleCost(FactionResource.Volatiles).MultiplyCost(0.05f * this.GetSystemDamage(system));
				tiresourcesCost.SetCompletionTime_Days(0.5f * (float)this.hull.consTier * this.GetSystemDamage(system));
				goto IL_01E8;
			}
			tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.AddCost(FactionResource.Metals, 0.1f, true);
			tiresourcesCost.SetCompletionTime_Days(0.5f * (float)this.hull.consTier * this.GetSystemDamage(system));
			IL_01E8:
			if (testBoost && repairingHab != null && this.AllowBoostForRepairsResupply && !tiresourcesCost.CanAfford(payingFaction, 1f, null, float.PositiveInfinity))
			{
				tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(payingFaction, repairingHab, tiresourcesCost, payingFaction.AvailableSpaceResources(1f), false);
			}
			TIResourcesCost tiresourcesCost2 = tiresourcesCost;
			float num = tiresourcesCost.completionTime_days + this.SumOfficerEffectsModifiers(OfficerEffectType.DockRepairSpeed, tiresourcesCost.completionTime_days) + TIEffectsState.SumEffectsModifiers(Context.DockyardRepairSpeed, this.fleet.faction, tiresourcesCost.completionTime_days, null);
			int? num2 = ((repairingHab != null) ? new int?(repairingHab.RepairSpeedDivisor()) : null);
			tiresourcesCost2.SetCompletionTime_Days((num / ((num2 != null) ? new float?((float)num2.GetValueOrDefault()) : null)) ?? 1f);
			return tiresourcesCost;
		}

		// Token: 0x06004698 RID: 18072 RVA: 0x001CD968 File Offset: 0x001CBB68
		public TIResourcesCost PartRepairCost(ModuleDataEntry module, TIFactionState payingFaction, TIHabState repairingHab, bool testBoost)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (module.moduleTemplate.ref_powerPlant != null)
			{
				tiresourcesCost = this.template.powerPlantBuildCost;
			}
			else if (module.moduleTemplate.ref_radiator != null)
			{
				tiresourcesCost = this.template.radiatorsBuildCost;
			}
			else if (module.moduleTemplate.ref_armor != null)
			{
				if (module.slotIndex == this.hull.GetUniqueSlotIndex(ShipModuleSlotType.NoseArmor))
				{
					tiresourcesCost = module.moduleTemplate.buildCost(this.template.noseArmorMass_tons, 0f);
				}
				else if (module.slotIndex == this.hull.GetUniqueSlotIndex(ShipModuleSlotType.LateralArmor))
				{
					tiresourcesCost = module.moduleTemplate.buildCost(this.template.lateralArmorMass_tons, 0f);
				}
				else if (module.slotIndex == this.hull.GetUniqueSlotIndex(ShipModuleSlotType.TailArmor))
				{
					tiresourcesCost = module.moduleTemplate.buildCost(this.template.tailArmorMass_tons, 0f);
				}
			}
			else
			{
				tiresourcesCost = module.moduleTemplate.buildCost(0f, 0f);
			}
			tiresourcesCost = tiresourcesCost.MultiplyCost(module.moduleTemplate.repairCostMultipler * this.GetPartDamage(module));
			tiresourcesCost.SetCompletionTime_Days(2f * this.GetPartDamage(module));
			if (testBoost && repairingHab != null && !tiresourcesCost.CanAfford(payingFaction, 1f, null, float.PositiveInfinity) && this.AllowBoostForRepairsResupply)
			{
				tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(payingFaction, repairingHab, tiresourcesCost, payingFaction.AvailableSpaceResources(1f), false);
			}
			TIResourcesCost tiresourcesCost2 = tiresourcesCost;
			float num = tiresourcesCost.completionTime_days + this.SumOfficerEffectsModifiers(OfficerEffectType.DockRepairSpeed, tiresourcesCost.completionTime_days) + TIEffectsState.SumEffectsModifiers(Context.DockyardRepairSpeed, this.fleet.faction, tiresourcesCost.completionTime_days, null);
			int? num2 = ((repairingHab != null) ? new int?(repairingHab.RepairSpeedDivisor()) : null);
			tiresourcesCost2.SetCompletionTime_Days((num / ((num2 != null) ? new float?((float)num2.GetValueOrDefault()) : null)) ?? 1f);
			return tiresourcesCost;
		}

		// Token: 0x06004699 RID: 18073 RVA: 0x001CDB94 File Offset: 0x001CBD94
		public TIResourcesCost ArmorFacingRepairCost(ArmorFacing facing, TIFactionState designingFaction, TIHabState repairingHab, bool testBoost)
		{
			if (this.armor[facing].maxArmor > 0 && (this.armor[facing].chippedPct > 0f || this.armor[facing].armorValue < this.armor[facing].maxArmor))
			{
				TIResourcesCost tiresourcesCost = new TIResourcesCost();
				switch (facing)
				{
				case ArmorFacing.Nose:
					tiresourcesCost = this.template.noseArmorBuildCost;
					break;
				case ArmorFacing.Right:
				case ArmorFacing.Left:
					tiresourcesCost = this.template.lateralArmorBuildCost;
					break;
				case ArmorFacing.Tail:
					tiresourcesCost = this.template.tailArmorBuildCost;
					break;
				}
				int num = (this.armor[facing].maxArmor - this.armor[facing].armorValue) / this.armor[facing].maxArmor;
				TIResourcesCost tiresourcesCost2 = tiresourcesCost.MultiplyCost((float)num);
				tiresourcesCost2.SetCompletionTime_Days(1f * (float)num);
				TIResourcesCost tiresourcesCost3 = tiresourcesCost.MultiplyCost(this.armor[facing].chippedPct * TISpaceShipState.ArmorData.GetArmorTemplate(this, facing).repairCostMultipler);
				tiresourcesCost3.SetCompletionTime_Days(1f * this.armor[facing].chippedPct);
				tiresourcesCost2.SumCosts_NoDuration(tiresourcesCost3);
				tiresourcesCost2.SetCompletionTime_Days(tiresourcesCost2.completionTime_days + tiresourcesCost3.completionTime_days);
				if (testBoost && repairingHab != null && !tiresourcesCost2.CanAfford(designingFaction, 1f, null, float.PositiveInfinity) && this.AllowBoostForRepairsResupply)
				{
					tiresourcesCost2 = TISpaceShipTemplate.MixedResourceConstructionCost(designingFaction, repairingHab, tiresourcesCost, designingFaction.AvailableSpaceResources(1f), false);
				}
				TIResourcesCost tiresourcesCost4 = tiresourcesCost2;
				float num2 = tiresourcesCost2.completionTime_days + this.SumOfficerEffectsModifiers(OfficerEffectType.DockRepairSpeed, tiresourcesCost2.completionTime_days) + TIEffectsState.SumEffectsModifiers(Context.DockyardRepairSpeed, this.fleet.faction, tiresourcesCost2.completionTime_days, null);
				int? num3 = ((repairingHab != null) ? new int?(repairingHab.RepairSpeedDivisor()) : null);
				tiresourcesCost4.SetCompletionTime_Days((num2 / ((num3 != null) ? new float?((float)num3.GetValueOrDefault()) : null)) ?? 1f);
				return tiresourcesCost2;
			}
			return null;
		}

		// Token: 0x0600469A RID: 18074 RVA: 0x001CDDE6 File Offset: 0x001CBFE6
		public List<ShipSystem> DamagedSystems()
		{
			return this.damagedSystems.Keys.ToList<ShipSystem>();
		}

		// Token: 0x0600469B RID: 18075 RVA: 0x001CDDF8 File Offset: 0x001CBFF8
		public List<ShipSystem> GetAffordableSystemRepairs(TIFactionState payingFaction, TIHabState repairingHab)
		{
			if (this.damagedSystems.Count > 0)
			{
				Dictionary<ShipSystem, TIResourcesCost> systemRepairCosts = this.damagedSystems.ToDictionary<KeyValuePair<ShipSystem, float>, ShipSystem, TIResourcesCost>((KeyValuePair<ShipSystem, float> x) => x.Key, (KeyValuePair<ShipSystem, float> x) => this.SystemRepairCost(x.Key, payingFaction, repairingHab, false));
				return this.damagedSystems.Keys.Where<ShipSystem>((ShipSystem x) => systemRepairCosts[x].CanAfford(payingFaction, 1f, null, float.PositiveInfinity)).ToList<ShipSystem>();
			}
			return new List<ShipSystem>();
		}

		// Token: 0x0600469C RID: 18076 RVA: 0x001CDEA4 File Offset: 0x001CC0A4
		public List<DamagedShipPartData> GetAffordablePartRepairs(TIFactionState payingFaction, TIHabState repairHab)
		{
			if (this.damagedParts.Count > 0)
			{
				Dictionary<DamagedShipPartData, TIResourcesCost> partRepairCosts = this.damagedParts.ToDictionary<DamagedShipPartData, DamagedShipPartData, TIResourcesCost>((DamagedShipPartData x) => x, (DamagedShipPartData x) => this.PartRepairCost(x.module, payingFaction, repairHab, false));
				return this.damagedParts.Where<DamagedShipPartData>((DamagedShipPartData x) => partRepairCosts[x].CanAfford(payingFaction, 1f, null, float.PositiveInfinity)).ToList<DamagedShipPartData>();
			}
			return new List<DamagedShipPartData>();
		}

		// Token: 0x0600469D RID: 18077 RVA: 0x001CDF4C File Offset: 0x001CC14C
		public bool CanAffordAnyRepair(TIHabState repairingHab)
		{
			if (this.damagedSystems.Count > 0 && this.GetAffordableSystemRepairs(this.faction, repairingHab).Count > 0)
			{
				return true;
			}
			if (this.damagedParts.Count > 0 && this.GetAffordablePartRepairs(this.faction, repairingHab).Count > 0)
			{
				return true;
			}
			foreach (ArmorFacing armorFacing in TISpaceShipState.repairableArmorFacings)
			{
				TIResourcesCost tiresourcesCost = this.ArmorFacingRepairCost(armorFacing, this.faction, repairingHab, false);
				if (tiresourcesCost != null && tiresourcesCost != null && tiresourcesCost.CanAfford(this.faction, 1f, null, float.PositiveInfinity))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600469E RID: 18078 RVA: 0x001CE018 File Offset: 0x001CC218
		public void RepairSystem(ShipSystem system)
		{
			this.PartsDestroyedDuringOperation[system] = false;
			this.damagedSystems.Remove(system);
			if (system == ShipSystem.DriveCoupling || system == ShipSystem.PowerCoupling || system == ShipSystem.VectorThrusters)
			{
				this.SetPropulsionValuesDirty(false, false);
			}
			GameControl.eventManager.TriggerEvent(new ShipSystemDamageChange(this, system, true), null, new object[] { this });
		}

		// Token: 0x0600469F RID: 18079 RVA: 0x001CE074 File Offset: 0x001CC274
		public void RepairPart(DamagedShipPartData part)
		{
			ShipSystem systemTypeFromModuleData = this.GetSystemTypeFromModuleData(part.module);
			if (this.PartsDestroyedDuringOperation.ContainsKey(systemTypeFromModuleData))
			{
				this.PartsDestroyedDuringOperation[systemTypeFromModuleData] = false;
			}
			if (part.module.moduleTemplate.isRadiator)
			{
				this.visualizerLink.ModelController.OnRadiatorRepaired();
			}
			this.SetPartDamage(part.module, 0f, false);
		}

		// Token: 0x060046A0 RID: 18080 RVA: 0x001CE0DD File Offset: 0x001CC2DD
		public void RepairArmorFacing(ArmorFacing facing)
		{
			this.armor[facing].RepairArmor();
		}

		// Token: 0x060046A1 RID: 18081 RVA: 0x001CE0F0 File Offset: 0x001CC2F0
		public bool CanFulfillGoal(GoalType goal)
		{
			switch (goal)
			{
			case GoalType.ProspectSites:
				return this.HasSpecialModuleRule(SpecialModuleRule.Prospector, false);
			case GoalType.FoundPlatform:
			case GoalType.FoundMaxStation:
				break;
			case GoalType.FoundBase:
				return this.HasSpecialModuleRule(SpecialModuleRule.FoundFissionOutpost, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundFusionOutpost, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundSolarOutpost, false);
			default:
				switch (goal)
				{
				case GoalType.DefendWithFleet:
				case GoalType.AttackWithFleet:
					return this.template.combatant && (this.noseWeaponTemplates.Count > 0 || this.hullWeaponTemplates.Count > 0);
				case GoalType.SecureEarthSpace:
					break;
				case GoalType.CaptureHab:
					return this.HasSpecialModuleRule(SpecialModuleRule.Assault, false);
				default:
					switch (goal)
					{
					case GoalType.InvadeEarth:
						return this.HasSpecialModuleRule(SpecialModuleRule.LandArmy, false);
					case GoalType.SurveilEarth:
						return this.HasSpecialModuleRule(SpecialModuleRule.Surveillance, false);
					case GoalType.FoundStation:
						goto IL_0079;
					case GoalType.FoundSurveillanceStation:
						return this.HasSpecialModuleRule(SpecialModuleRule.FoundSurveillancePlatform, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundSurveillanceOrbital, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundSurveillanceRing, false);
					}
					break;
				}
				return true;
			}
			IL_0079:
			return this.HasSpecialModuleRule(SpecialModuleRule.FoundFusionPlatform, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundFissionPlatform, false) || this.HasSpecialModuleRule(SpecialModuleRule.FoundSolarPlatform, false);
		}

		// Token: 0x060046A2 RID: 18082 RVA: 0x001CE210 File Offset: 0x001CC410
		public bool HasSpecialModuleRule(SpecialModuleRule rule, bool includeNonFunctional = false)
		{
			foreach (TIUtilityModuleTemplate tiutilityModuleTemplate in this.GetFunctionalUtilitySlotModuleTemplates(includeNonFunctional ? 0f : 1f))
			{
				if (tiutilityModuleTemplate != null && tiutilityModuleTemplate.specialModuleRules.Contains(rule))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060046A3 RID: 18083 RVA: 0x001CE288 File Offset: 0x001CC488
		public List<SpecialModuleRule> SpecialModuleRules(bool includeNonFunctional = false)
		{
			List<SpecialModuleRule> list = new List<SpecialModuleRule>();
			foreach (TIUtilityModuleTemplate tiutilityModuleTemplate in this.GetFunctionalUtilitySlotModuleTemplates(includeNonFunctional ? 0f : 1f))
			{
				if (tiutilityModuleTemplate.specialModuleRules != null)
				{
					list.AddRange(tiutilityModuleTemplate.specialModuleRules.Where<SpecialModuleRule>((SpecialModuleRule x) => x > SpecialModuleRule.None));
				}
			}
			return list.Distinct<SpecialModuleRule>().ToList<SpecialModuleRule>();
		}

		// Token: 0x060046A4 RID: 18084 RVA: 0x001CE32C File Offset: 0x001CC52C
		public int SpecialModuleRuleCount(SpecialModuleRule rule)
		{
			int num = 0;
			using (List<TIUtilityModuleTemplate>.Enumerator enumerator = this.GetFunctionalUtilitySlotModuleTemplates(1f).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.specialModuleRules.Contains(rule))
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x060046A5 RID: 18085 RVA: 0x001CE390 File Offset: 0x001CC590
		public float AssaultCombatValue(bool defense)
		{
			float num = 0f;
			foreach (ModuleDataEntry moduleDataEntry in this.GetFunctionalUtilitySlotModules(defense ? 0.001f : 0.001f))
			{
				if (moduleDataEntry.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.Assault) || (defense && moduleDataEntry.moduleTemplate.ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.MarineOpsDefenseOnly)))
				{
					num += moduleDataEntry.moduleTemplate.ref_utilityModule.marineOpsValue * this.GetPartFunction(moduleDataEntry);
				}
			}
			num += this.SumOfficerEffectsModifiers(OfficerEffectType.AssaultCombatValue, num);
			return num;
		}

		// Token: 0x060046A6 RID: 18086 RVA: 0x001CE450 File Offset: 0x001CC650
		public float InvasionCombatValue()
		{
			float num = 0f;
			if (this.utilityModuleTemplates.Any<TIShipModuleTemplate>(delegate(TIShipModuleTemplate x)
			{
				if (x.isUtilityModule)
				{
					TIUtilityModuleTemplate ref_utilityModule = x.ref_utilityModule;
					return ref_utilityModule != null && ref_utilityModule.specialModuleRules.Contains(SpecialModuleRule.LandArmy);
				}
				return false;
			}))
			{
				ModuleDataEntry moduleDataEntry = this.utilityModules.First<ModuleDataEntry>(delegate(ModuleDataEntry x)
				{
					TIUtilityModuleTemplate ref_utilityModule2 = x.moduleTemplate.ref_utilityModule;
					return ref_utilityModule2 != null && ref_utilityModule2.specialModuleRules.Contains(SpecialModuleRule.LandArmy);
				});
				num += moduleDataEntry.moduleTemplate.ref_utilityModule.marineOpsValue * (float)TemplateManager.global.alienArmiesFromLanding * this.GetPartFunction(moduleDataEntry);
			}
			return num;
		}

		// Token: 0x060046A7 RID: 18087 RVA: 0x001CE4E4 File Offset: 0x001CC6E4
		public bool LongRangeFighter(bool includeOtherRoles)
		{
			ShipRole role = this.role;
			if (role == ShipRole.LL_Intruder || role == ShipRole.ML_Standoff || role == ShipRole.SL_Defender)
			{
				return true;
			}
			if (includeOtherRoles)
			{
				return (from x in this.AllWeaponModuleData()
					where this.WeaponIsOperable(x)
					select x).Any<ModuleDataEntry>((ModuleDataEntry x) => (x.moduleTemplate as TIShipWeaponTemplate).targetingRange_km >= 800f);
			}
			return false;
		}

		// Token: 0x060046A8 RID: 18088 RVA: 0x001CE54C File Offset: 0x001CC74C
		public bool MediumRangeFighter(bool includeOtherRoles)
		{
			ShipRole role = this.role;
			if (role == ShipRole.LM_Interdictor || role == ShipRole.MM_SpaceSuperiority || role == ShipRole.SM_Patrol)
			{
				return true;
			}
			if (includeOtherRoles)
			{
				return (from x in this.AllWeaponModuleData()
					where this.WeaponIsOperable(x)
					select x).Any<ModuleDataEntry>((ModuleDataEntry x) => (x.moduleTemplate as TIShipWeaponTemplate).targetingRange_km >= 500f);
			}
			return false;
		}

		// Token: 0x060046A9 RID: 18089 RVA: 0x001CE5B4 File Offset: 0x001CC7B4
		public List<TICouncilorState> CouncilorStatesPresentAndKnownToFaction(TIFactionState faction)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in this.councilorPassengers)
			{
				CouncilorView councilorView = new CouncilorView(ticouncilorState, faction);
				if (councilorView.location == this)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x060046AA RID: 18090 RVA: 0x001CE628 File Offset: 0x001CC828
		public List<CouncilorView> CouncilorViewsPresentAndKnownToFaction(TIFactionState faction)
		{
			List<CouncilorView> list = new List<CouncilorView>();
			foreach (TICouncilorState ticouncilorState in this.councilorPassengers)
			{
				CouncilorView councilorView = new CouncilorView(ticouncilorState, faction);
				if (councilorView.location == this)
				{
					list.Add(councilorView);
				}
			}
			return list;
		}

		// Token: 0x060046AB RID: 18091 RVA: 0x001CE69C File Offset: 0x001CC89C
		public void SetMissionControlConsumption()
		{
			this.missionControlConsumption = this.hull.missionControl;
			this.missionControlConsumption += (int)this.SumOfficerEffectsModifiers(OfficerEffectType.ShipMissionControl, (float)this.missionControlConsumption);
			this.missionControlConsumption = Mathf.Max(this.missionControlConsumption, 1);
			TIFactionState faction = this.faction;
			if (faction == null)
			{
				return;
			}
			faction.SetMissionControlUsageDataDirty();
		}

		// Token: 0x060046AC RID: 18092 RVA: 0x001CE6F8 File Offset: 0x001CC8F8
		public List<TIOfficerState> GetOfficers()
		{
			return this.officers;
		}

		// Token: 0x060046AD RID: 18093 RVA: 0x001CE700 File Offset: 0x001CC900
		public int GetMaxRankOfficers()
		{
			return this.officers.Where<TIOfficerState>((TIOfficerState x) => x.rank == 3).Count<TIOfficerState>();
		}

		// Token: 0x060046AE RID: 18094 RVA: 0x001CE731 File Offset: 0x001CC931
		public TIGameState GetState()
		{
			return this;
		}

		// Token: 0x060046AF RID: 18095 RVA: 0x001CE734 File Offset: 0x001CC934
		public List<TIOfficerState> CheckForOfficerPromotionEvent(OfficerSpawnEventType spawnEventType, float chanceModifier = 0f, bool chanceModifierMult = false, List<TIOfficerState> existingPromotions = null)
		{
			List<TIOfficerState> list = new List<TIOfficerState>();
			if (this.hull.noShipyardBuild)
			{
				return list;
			}
			int num = 0;
			IEnumerable<TIOfficerState> enumerable = this.officers;
			Func<TIOfficerState, bool> <>9__1;
			Func<TIOfficerState, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = delegate(TIOfficerState x)
				{
					if (x.template.spawnEventType == spawnEventType && x.rank < 3)
					{
						List<TIOfficerState> existingPromotions2 = existingPromotions;
						return existingPromotions2 == null || !existingPromotions2.Contains(x);
					}
					return false;
				});
			}
			foreach (TIOfficerState tiofficerState in enumerable.Where<TIOfficerState>(func))
			{
				float num2 = tiofficerState.template.spawnChance;
				if (chanceModifierMult)
				{
					num2 *= chanceModifier;
				}
				else
				{
					num2 += chanceModifier;
				}
				num2 *= 1f + this.SumOfficerEffectsModifiers(OfficerEffectType.OfficerPromotionChance, num2);
				num2 *= 1f + TIEffectsState.SumEffectsModifiers(Context.ShipOfficerPromotion, this.faction, num2, null);
				num2 /= (float)(num + 1);
				if (TIUtilities.RandomFloatValue() < num2)
				{
					tiofficerState.Promote();
					list.Add(tiofficerState);
					num++;
				}
			}
			bool flag = false;
			if (this.faction.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.officers.Count) == 0)
			{
				flag = true;
			}
			int num3 = 0;
			foreach (TIOfficerTemplate tiofficerTemplate in TemplateManager.IterateByClass<TIOfficerTemplate>(true).ToList<TIOfficerTemplate>().Shuffle<TIOfficerTemplate>())
			{
				if (tiofficerTemplate.spawnEventType == spawnEventType && tiofficerTemplate.OfficerTypeAllowedForShip(this, false, num3) && this.GetSystemFunction(tiofficerTemplate.location) > 0.5f)
				{
					float num4 = tiofficerTemplate.spawnChance;
					if (flag)
					{
						num4 = 1f;
						flag = false;
					}
					else
					{
						if (chanceModifierMult)
						{
							num4 *= chanceModifier;
						}
						else
						{
							num4 += chanceModifier;
						}
						num4 *= 1f + this.SumOfficerEffectsModifiers(OfficerEffectType.OfficerPromotionChance, num4);
						num4 *= 1f + TIEffectsState.SumEffectsModifiers(Context.ShipOfficerPromotion, this.faction, num4, null);
						num4 *= this.GetSystemFunction(tiofficerTemplate.location);
						num4 /= (float)(num + 1);
					}
					if (TIUtilities.RandomFloatValue() < num4)
					{
						TIOfficerState tiofficerState2 = this.CreateOfficer(tiofficerTemplate.dataName);
						list.Add(tiofficerState2);
						num3++;
						num++;
					}
				}
			}
			return list;
		}

		// Token: 0x060046B0 RID: 18096 RVA: 0x001CE9B0 File Offset: 0x001CCBB0
		public List<TIOfficerTemplate> EligibleFreeOfficerCreationTemplates()
		{
			List<TIOfficerTemplate> list = new List<TIOfficerTemplate>();
			foreach (TIOfficerTemplate tiofficerTemplate in TemplateManager.IterateByClass<TIOfficerTemplate>(true).ToList<TIOfficerTemplate>())
			{
				if (tiofficerTemplate.OfficerTypeAllowedForShip(this, false, 0))
				{
					list.Add(tiofficerTemplate);
				}
			}
			return list;
		}

		// Token: 0x060046B1 RID: 18097 RVA: 0x001CEA1C File Offset: 0x001CCC1C
		public List<TIOfficerState> FreeOfficerCreationEvent(int officersToCreate = 1)
		{
			List<TIOfficerState> list = new List<TIOfficerState>();
			int i = 0;
			int num = 0;
			while (i < officersToCreate)
			{
				List<TIOfficerTemplate> list2 = this.EligibleFreeOfficerCreationTemplates();
				if (list2.Count == 0 || num >= 1000)
				{
					break;
				}
				TIOfficerTemplate tiofficerTemplate = list2.SelectRandomItem<TIOfficerTemplate>();
				if (this.GetSystemFunction(tiofficerTemplate.location) > 0.5f)
				{
					float num2 = tiofficerTemplate.spawnChance;
					num2 *= this.GetSystemFunction(tiofficerTemplate.location);
					if (TIUtilities.RandomFloatValue() < num2)
					{
						TIOfficerState tiofficerState = this.CreateOfficer(tiofficerTemplate.dataName);
						list.Add(tiofficerState);
						i++;
					}
				}
				num++;
			}
			return list;
		}

		// Token: 0x060046B2 RID: 18098 RVA: 0x001CEAB0 File Offset: 0x001CCCB0
		public TIOfficerState CreateOfficer(string templateName)
		{
			return TIOfficerState.CreateOfficer(templateName, this);
		}

		// Token: 0x060046B3 RID: 18099 RVA: 0x001CEABC File Offset: 0x001CCCBC
		public float SumOfficerEffectsModifiers(OfficerEffectType effectType, float baseValue)
		{
			float num = baseValue;
			foreach (TIOfficerState tiofficerState in this.officers)
			{
				num += tiofficerState.SumOfficerEffects(effectType, baseValue);
			}
			return num - baseValue;
		}

		// Token: 0x060046B4 RID: 18100 RVA: 0x001CEB18 File Offset: 0x001CCD18
		public void OnInternalDamage_Officers(TIShipPartTemplate hitModule, float newDamageToSystem_points)
		{
			if (hitModule.isDrive)
			{
				this.OnInternalDamage_Officers(ShipSystem.Drive, newDamageToSystem_points);
				return;
			}
			if (hitModule.isPowerPlant)
			{
				this.OnInternalDamage_Officers(ShipSystem.PowerPlant, newDamageToSystem_points);
			}
		}

		// Token: 0x060046B5 RID: 18101 RVA: 0x001CEB40 File Offset: 0x001CCD40
		public string KillAllOfficersReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.officers.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("TIOfficerTemplate.OfficersKilled"));
				foreach (TIOfficerState tiofficerState in this.officers)
				{
					stringBuilder.AppendLine(tiofficerState.DisplayNameAndJob);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060046B6 RID: 18102 RVA: 0x001CEBC4 File Offset: 0x001CCDC4
		public void OnInternalDamage_Officers(ShipSystem hitSystem, float newDamageToSystem_points)
		{
			foreach (TIOfficerState tiofficerState in this.officers.ToList<TIOfficerState>())
			{
				if (hitSystem == tiofficerState.template.location)
				{
					float num = newDamageToSystem_points / 50f;
					num += this.SumOfficerEffectsModifiers(OfficerEffectType.OfficerDeathChance, num);
					if (TIUtilities.RandomFloatValue() < num)
					{
						if (tiofficerState.isDummy)
						{
							this.officers.Remove(tiofficerState);
						}
						else
						{
							if (this.fleet.inCombat)
							{
								this.fleet.combatState.RecordOfficerKilled(tiofficerState);
							}
							else
							{
								TINotificationQueueState.LogOfficerKilledOutsideofCombat(tiofficerState);
							}
							tiofficerState.DeleteOfficer(true);
						}
					}
				}
			}
		}

		// Token: 0x060046B7 RID: 18103 RVA: 0x001CEC84 File Offset: 0x001CCE84
		public void CheckOfficersOnShipAchievement()
		{
			TIFactionState faction = this.faction;
			if (faction == null || !faction.isActivePlayer)
			{
				return;
			}
			if (this.GetMaxRankOfficers() >= 5)
			{
				this.faction.UnlockAchievement("officersOnShip");
			}
		}

		// Token: 0x060046B8 RID: 18104 RVA: 0x001CECB8 File Offset: 0x001CCEB8
		public void BecomeCopyOf(TISpaceShipState shipToCopy)
		{
			this.cruiseAcceleration_mps2 = shipToCopy.cruiseAcceleration_mps2;
			this.combatAcceleration_mps2 = shipToCopy.combatAcceleration_mps2;
			this.currentDeltaV_kps = shipToCopy.currentDeltaV_kps;
			this.currentMaxDeltaV_kps = shipToCopy.currentMaxDeltaV_kps;
			this.currentMass_kg = shipToCopy.currentMass_kg;
			this.angular_acceleration_rads2 = shipToCopy.angular_acceleration_rads2;
			this.max_angular_velocity_rad_s = shipToCopy.max_angular_velocity_rad_s;
			this.missionControlConsumption = shipToCopy.missionControlConsumption;
			this.currentHeatSinkCapacity_GJ = shipToCopy.currentHeatSinkCapacity_GJ;
			this.isDamageControlSuspended = this.isDamageControlSuspended;
			foreach (KeyValuePair<ArmorFacing, TISpaceShipState.ArmorData> keyValuePair in this.armor)
			{
				TISpaceShipState.ArmorData armorData = shipToCopy.armor[keyValuePair.Key];
				keyValuePair.Value.maxArmor = armorData.maxArmor;
				keyValuePair.Value.armorValue = armorData.armorValue;
				keyValuePair.Value.chippedPct = armorData.chippedPct;
			}
			this.propellant_tons = shipToCopy.propellant_tons;
			foreach (KeyValuePair<ModuleDataEntry, int> keyValuePair2 in shipToCopy.ammo)
			{
				this.ammo[keyValuePair2.Key] = keyValuePair2.Value;
			}
			List<ModuleDataEntry> list = this.AllModuleData().ToList<ModuleDataEntry>();
			this.damagedSystems = new Dictionary<ShipSystem, float>(shipToCopy.damagedSystems);
			this.damagedParts = new List<DamagedShipPartData>();
			this.damagedPartsCache = new Dictionary<ModuleDataEntry, DamagedShipPartData>();
			using (List<DamagedShipPartData>.Enumerator enumerator3 = shipToCopy.damagedParts.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					DamagedShipPartData damagedPartToCopy = enumerator3.Current;
					ModuleDataEntry moduleDataEntry = list.FirstOrDefault<ModuleDataEntry>((ModuleDataEntry x) => damagedPartToCopy.module.Equals(x));
					if (moduleDataEntry != null)
					{
						DamagedShipPartData damagedShipPartData = new DamagedShipPartData(moduleDataEntry, damagedPartToCopy.damage);
						this.damagedParts.Add(damagedShipPartData);
						this.damagedPartsCache[moduleDataEntry] = damagedShipPartData;
					}
				}
			}
		}

		// Token: 0x060046B9 RID: 18105 RVA: 0x001CEEF0 File Offset: 0x001CD0F0
		public override bool Equals(object obj)
		{
			TISpaceShipState tispaceShipState = obj as TISpaceShipState;
			if (tispaceShipState == null)
			{
				return false;
			}
			if (this.isDummy || tispaceShipState.isDummy)
			{
				return this == tispaceShipState;
			}
			return base.Equals(obj);
		}

		// Token: 0x060046C5 RID: 18117 RVA: 0x001CF611 File Offset: 0x001CD811
		[CompilerGenerated]
		private float <PerSecondPowerGain>g__propulsionPowerGain|432_0()
		{
			return this._propulsionPowerGenerationRequirement_GW * this.GetSystemFunction(ShipSystem.PowerPlant) * this.GetSystemFunction(ShipSystem.PowerCoupling);
		}

		// Token: 0x04002895 RID: 10389
		public List<ModuleDataEntry> noseWeapons = new List<ModuleDataEntry>();

		// Token: 0x04002896 RID: 10390
		public List<ModuleDataEntry> hullWeapons = new List<ModuleDataEntry>();

		// Token: 0x04002897 RID: 10391
		public List<ModuleDataEntry> utilityModules = new List<ModuleDataEntry>();

		// Token: 0x04002898 RID: 10392
		public const int RadiatorAnimationDuration_s = 60;

		// Token: 0x04002899 RID: 10393
		public const float TEMP_WARM_THRESHOLD = 0.45f;

		// Token: 0x0400289A RID: 10394
		public const float TEMP_HOT_THRESHOLD = 0.15f;

		// Token: 0x0400289B RID: 10395
		public const float TEMP_MELTDOWN_THRESHOLD = 0f;

		// Token: 0x0400289C RID: 10396
		public const float BATT_LOW_TRESHOLD = 0.3f;

		// Token: 0x0400289D RID: 10397
		public const float BATT_CRITICAL_TRESHOLD = 0.1f;

		// Token: 0x0400289E RID: 10398
		public const float DAM_CON_FASTEST_REPAIR_MIN = 5.5f;

		// Token: 0x0400289F RID: 10399
		public const float DAM_CON_SLOWEST_REPAIR_MIN = 240f;

		// Token: 0x040028A0 RID: 10400
		public const float ALIEN_REPAIR_BONUS = 1.5f;

		// Token: 0x040028A1 RID: 10401
		public const float REPAIR_BAY_BONUS = 0.5f;

		// Token: 0x040028A2 RID: 10402
		private const float MINIMUM_ANGULAR_ACCELERATION_RADS2 = 0.001f;

		// Token: 0x040028B0 RID: 10416
		public List<CombatManeuver> activeCombatManeuvers;

		// Token: 0x040028B1 RID: 10417
		public Dictionary<ArmorFacing, TISpaceShipState.ArmorData> armor;

		// Token: 0x040028B2 RID: 10418
		public float propellant_tons;

		// Token: 0x040028B3 RID: 10419
		public List<string> kills;

		// Token: 0x040028B4 RID: 10420
		public List<TIOfficerState> officers;

		// Token: 0x040028B6 RID: 10422
		[SerializeField]
		private Dictionary<ShipSystem, float> damagedSystems = new Dictionary<ShipSystem, float>();

		// Token: 0x040028B7 RID: 10423
		public List<DamagedShipPartData> damagedParts = new List<DamagedShipPartData>();

		// Token: 0x040028B8 RID: 10424
		private Dictionary<ModuleDataEntry, DamagedShipPartData> damagedPartsCache = new Dictionary<ModuleDataEntry, DamagedShipPartData>();

		// Token: 0x040028B9 RID: 10425
		private Dictionary<ArmorFacing, Dictionary<ShipSystem, float>> internalDamageTables = new Dictionary<ArmorFacing, Dictionary<ShipSystem, float>>();

		// Token: 0x040028BA RID: 10426
		public List<Vector4> damagePoints = new List<Vector4>();

		// Token: 0x040028BB RID: 10427
		public List<DamagedShipPartData> prevPartsBeingRepaired = new List<DamagedShipPartData>();

		// Token: 0x040028BC RID: 10428
		public List<ShipSystem> prevSystemsBeingRepaired = new List<ShipSystem>();

		// Token: 0x040028BD RID: 10429
		public PlannedResupplyAndRepair plannedResupplyAndRepair = new PlannedResupplyAndRepair();

		// Token: 0x040028C0 RID: 10432
		public Vector3d currentFleetOffset;

		// Token: 0x040028C1 RID: 10433
		public Quaternion currentRotation;

		// Token: 0x040028C2 RID: 10434
		public StratManeuver currentManeuver;

		// Token: 0x040028C3 RID: 10435
		private ShipManeuverSequence currentManeuverSequence;

		// Token: 0x040028C4 RID: 10436
		[fsIgnore]
		public bool inManeuver;

		// Token: 0x040028C5 RID: 10437
		[fsIgnore]
		public bool inManeuverSequence;

		// Token: 0x040028CA RID: 10442
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040028CB RID: 10443
		[fsIgnore]
		public ShipVisController visualizerLink;

		// Token: 0x040028CC RID: 10444
		[fsIgnore]
		public int _worstArmor = -1;

		// Token: 0x040028CD RID: 10445
		[fsIgnore]
		public int _bestArmor = -1;

		// Token: 0x040028CE RID: 10446
		private string _combatUpdatePropulsionEventName;

		// Token: 0x040028CF RID: 10447
		public TIFactionState storedFaction;

		// Token: 0x040028D0 RID: 10448
		private float _wasteHeat_GW;

		// Token: 0x040028D1 RID: 10449
		private float _systemsPowerGenerationRequirement_GW;

		// Token: 0x040028D2 RID: 10450
		private float _weaponsPowerGenerationRequirement_GW;

		// Token: 0x040028D3 RID: 10451
		private float _propulsionPowerGenerationRequirement_GW;

		// Token: 0x040028D4 RID: 10452
		private float _auxReactorPowerGenerationRequirement_GW;

		// Token: 0x040028D5 RID: 10453
		private float _allPowerGenerationRequirement_GW;

		// Token: 0x040028D6 RID: 10454
		private float _auxPowerRequriedStorage_GJ;

		// Token: 0x040028D7 RID: 10455
		public bool isDummy;

		// Token: 0x040028D8 RID: 10456
		private ModuleDataEntry _driveModule;

		// Token: 0x040028D9 RID: 10457
		private ModuleDataEntry _powerPlantModule;

		// Token: 0x040028DA RID: 10458
		private ModuleDataEntry _radiatorModule;

		// Token: 0x040028DB RID: 10459
		private EventInstance radiatorEventInstance;

		// Token: 0x040028DC RID: 10460
		private DamageLayer damageLayer;

		// Token: 0x040028DD RID: 10461
		private bool damageVisualizationDirty;

		// Token: 0x040028DE RID: 10462
		private float _cachedSpaceCombatValue;

		// Token: 0x040028DF RID: 10463
		public bool spaceCombatValueDataDirty;

		// Token: 0x040028E0 RID: 10464
		public static readonly FactionResource[] relevantIncomeResources = new FactionResource[] { FactionResource.Money };

		// Token: 0x040028E1 RID: 10465
		[fsIgnore]
		public Dictionary<float, Dictionary<string, float>> effectiveBeamWeaponRange_km;

		// Token: 0x040028E2 RID: 10466
		private int visualizationDataDirtyFrame = -1;

		// Token: 0x040028E3 RID: 10467
		[SerializeField]
		private int combatSecondCounter;

		// Token: 0x040028E4 RID: 10468
		public float availablePower_GJ;

		// Token: 0x040028E5 RID: 10469
		public bool systemsDepowered;

		// Token: 0x040028E6 RID: 10470
		public bool generatorWorking;

		// Token: 0x040028E7 RID: 10471
		public static readonly Dictionary<ShipSystem, int> SystemRepairPriority = new Dictionary<ShipSystem, int>
		{
			{
				ShipSystem.LifeSupportMain,
				0
			},
			{
				ShipSystem.LifeSupportBackup,
				1
			},
			{
				ShipSystem.Bridge,
				2
			},
			{
				ShipSystem.DriveCoupling,
				3
			},
			{
				ShipSystem.PowerCoupling,
				4
			},
			{
				ShipSystem.VectorThrusters,
				5
			},
			{
				ShipSystem.SystemsReactor,
				6
			},
			{
				ShipSystem.CentralStructure,
				7
			},
			{
				ShipSystem.NoseStructure,
				8
			},
			{
				ShipSystem.TailStructure,
				9
			},
			{
				ShipSystem.DamageControl,
				10
			},
			{
				ShipSystem.FireControl,
				11
			},
			{
				ShipSystem.Sensors,
				12
			},
			{
				ShipSystem.None,
				999
			}
		};

		// Token: 0x040028E8 RID: 10472
		public static readonly Dictionary<ShipSystem, int> CombatSystemRepairPriority = new Dictionary<ShipSystem, int>
		{
			{
				ShipSystem.LifeSupportMain,
				0
			},
			{
				ShipSystem.DamageControl,
				1
			},
			{
				ShipSystem.Bridge,
				2
			},
			{
				ShipSystem.SystemsReactor,
				3
			},
			{
				ShipSystem.DriveCoupling,
				4
			},
			{
				ShipSystem.VectorThrusters,
				5
			},
			{
				ShipSystem.PowerCoupling,
				6
			},
			{
				ShipSystem.LifeSupportBackup,
				7
			},
			{
				ShipSystem.FireControl,
				8
			},
			{
				ShipSystem.Sensors,
				9
			}
		};

		// Token: 0x040028E9 RID: 10473
		public static readonly Dictionary<ShipModuleSlotType, int> ModuleRepairPriority = new Dictionary<ShipModuleSlotType, int>
		{
			{
				ShipModuleSlotType.Radiator,
				0
			},
			{
				ShipModuleSlotType.PowerPlant,
				1
			},
			{
				ShipModuleSlotType.Drive,
				2
			},
			{
				ShipModuleSlotType.HullHardPoint,
				3
			},
			{
				ShipModuleSlotType.NoseHardPoint,
				4
			},
			{
				ShipModuleSlotType.Utility,
				5
			},
			{
				ShipModuleSlotType.None,
				999
			}
		};

		// Token: 0x040028EA RID: 10474
		private static readonly Dictionary<ShipSystem, float> LessThanDamageToRepairInCombat = new Dictionary<ShipSystem, float>
		{
			{
				ShipSystem.Bridge,
				1f
			},
			{
				ShipSystem.CentralStructure,
				0f
			},
			{
				ShipSystem.DamageControl,
				1f
			},
			{
				ShipSystem.Drive,
				1f
			},
			{
				ShipSystem.DriveCoupling,
				2f
			},
			{
				ShipSystem.FireControl,
				2f
			},
			{
				ShipSystem.HullWeapons,
				1f
			},
			{
				ShipSystem.LifeSupportBackup,
				1f
			},
			{
				ShipSystem.LifeSupportMain,
				1f
			},
			{
				ShipSystem.NoseStructure,
				0f
			},
			{
				ShipSystem.NoseWeapons,
				1f
			},
			{
				ShipSystem.PowerCoupling,
				2f
			},
			{
				ShipSystem.PowerPlant,
				1f
			},
			{
				ShipSystem.Propellant,
				0f
			},
			{
				ShipSystem.Radiators,
				1f
			},
			{
				ShipSystem.Sensors,
				2f
			},
			{
				ShipSystem.SystemsReactor,
				1f
			},
			{
				ShipSystem.TailStructure,
				0f
			},
			{
				ShipSystem.UtilityModules,
				1f
			},
			{
				ShipSystem.VectorThrusters,
				1f
			}
		};

		// Token: 0x040028EB RID: 10475
		private readonly Dictionary<ShipSystem, bool> PartsDestroyedDuringOperation = new Dictionary<ShipSystem, bool>
		{
			{
				ShipSystem.Bridge,
				false
			},
			{
				ShipSystem.CentralStructure,
				false
			},
			{
				ShipSystem.DamageControl,
				false
			},
			{
				ShipSystem.Drive,
				false
			},
			{
				ShipSystem.DriveCoupling,
				false
			},
			{
				ShipSystem.FireControl,
				false
			},
			{
				ShipSystem.HullWeapons,
				false
			},
			{
				ShipSystem.LifeSupportBackup,
				false
			},
			{
				ShipSystem.LifeSupportMain,
				false
			},
			{
				ShipSystem.NoseStructure,
				false
			},
			{
				ShipSystem.NoseWeapons,
				false
			},
			{
				ShipSystem.PowerCoupling,
				false
			},
			{
				ShipSystem.PowerPlant,
				false
			},
			{
				ShipSystem.Propellant,
				false
			},
			{
				ShipSystem.Radiators,
				false
			},
			{
				ShipSystem.Sensors,
				false
			},
			{
				ShipSystem.SystemsReactor,
				false
			},
			{
				ShipSystem.TailStructure,
				false
			},
			{
				ShipSystem.UtilityModules,
				false
			},
			{
				ShipSystem.VectorThrusters,
				false
			}
		};

		// Token: 0x040028EC RID: 10476
		private const float MAX_REPAIR_AFTER_DESTRUCTION = 0.95f;

		// Token: 0x040028ED RID: 10477
		private const float MAX_REPAIR_AFTER_DESTRUCTION_WITH_REPAIR_BAY = 0.75f;

		// Token: 0x040028EE RID: 10478
		public Dictionary<ModuleDataEntry, float> batteryCharge;

		// Token: 0x040028EF RID: 10479
		private float oldHeatAtLastUIUpdate_GJ = -1f;

		// Token: 0x040028F0 RID: 10480
		public static readonly List<ShipSystem> visiblyDamagedSystems = new List<ShipSystem>
		{
			ShipSystem.NoseStructure,
			ShipSystem.CentralStructure,
			ShipSystem.TailStructure,
			ShipSystem.Drive,
			ShipSystem.Radiators
		};

		// Token: 0x040028F1 RID: 10481
		public const float innateHullRadiationProtectionMultiplier = 0.0625f;

		// Token: 0x040028F2 RID: 10482
		private static readonly HashSet<ShipSystem> DirectDamageableSystems = new HashSet<ShipSystem>
		{
			ShipSystem.NoseStructure,
			ShipSystem.CentralStructure,
			ShipSystem.TailStructure,
			ShipSystem.Bridge,
			ShipSystem.FireControl,
			ShipSystem.PowerCoupling,
			ShipSystem.DriveCoupling,
			ShipSystem.VectorThrusters,
			ShipSystem.SystemsReactor,
			ShipSystem.LifeSupportBackup,
			ShipSystem.LifeSupportMain,
			ShipSystem.DamageControl,
			ShipSystem.Sensors
		};

		// Token: 0x040028F3 RID: 10483
		private static readonly HashSet<ShipSystem> SoftSystems = new HashSet<ShipSystem>
		{
			ShipSystem.Bridge,
			ShipSystem.FireControl,
			ShipSystem.PowerCoupling,
			ShipSystem.DriveCoupling,
			ShipSystem.DamageControl,
			ShipSystem.Sensors,
			ShipSystem.NoseWeapons,
			ShipSystem.HullWeapons,
			ShipSystem.UtilityModules,
			ShipSystem.PowerPlant,
			ShipSystem.Drive
		};

		// Token: 0x040028F4 RID: 10484
		private const float DamagePointsToDestroyPropellantTank = 1f;

		// Token: 0x040028F5 RID: 10485
		private int targetingFrame = -1;

		// Token: 0x040028F6 RID: 10486
		private float _cacheTargetingBonus;

		// Token: 0x040028F7 RID: 10487
		private int ECMFrame = -1;

		// Token: 0x040028F8 RID: 10488
		private float _cacheECMValue;

		// Token: 0x040028F9 RID: 10489
		private int _cachedFunctionalUtilitySlotModulesFrame = -1;

		// Token: 0x040028FA RID: 10490
		private List<TIUtilityModuleTemplate> _cachedFunctionalUtilitySlotModules;

		// Token: 0x040028FB RID: 10491
		public const int numVectorThrusters = 2;

		// Token: 0x040028FC RID: 10492
		public const int shipAmmoReloadStep = 50;

		// Token: 0x040028FD RID: 10493
		private int bestExistingRefitCachedFrame;

		// Token: 0x040028FE RID: 10494
		private TISpaceShipTemplate cachedBestExistingRefit;

		// Token: 0x040028FF RID: 10495
		public const float StructuralDamageHullCostRepairMultiplier = 0.1f;

		// Token: 0x04002900 RID: 10496
		public const float BasePartRepairDuration_days = 2f;

		// Token: 0x04002901 RID: 10497
		public const float BaseArmorRepairDuration_days = 1f;

		// Token: 0x04002902 RID: 10498
		public const bool testBoostForRepairResupply = false;

		// Token: 0x04002903 RID: 10499
		private static readonly List<ArmorFacing> repairableArmorFacings = new List<ArmorFacing>
		{
			ArmorFacing.Nose,
			ArmorFacing.Tail,
			ArmorFacing.Right,
			ArmorFacing.Left
		};

		// Token: 0x04002904 RID: 10500
		[fsIgnore]
		public static readonly List<SpecialModuleRule> FoundBaseRules = new List<SpecialModuleRule>
		{
			SpecialModuleRule.FoundAutomatedFissionOutpost,
			SpecialModuleRule.FoundAutomatedSolarOutpost,
			SpecialModuleRule.FoundFissionOutpost,
			SpecialModuleRule.FoundFusionOutpost,
			SpecialModuleRule.FoundSolarOutpost
		};

		// Token: 0x04002905 RID: 10501
		[fsIgnore]
		public static readonly List<SpecialModuleRule> FoundStandardStationRules = new List<SpecialModuleRule>
		{
			SpecialModuleRule.FoundAutomatedFissionPlatform,
			SpecialModuleRule.FoundAutomatedSolarPlatform,
			SpecialModuleRule.FoundFissionPlatform,
			SpecialModuleRule.FoundFusionPlatform,
			SpecialModuleRule.FoundSolarPlatform
		};

		// Token: 0x04002906 RID: 10502
		[fsIgnore]
		public static readonly List<SpecialModuleRule> FoundSurveillanceStationRules = new List<SpecialModuleRule>
		{
			SpecialModuleRule.FoundSurveillancePlatform,
			SpecialModuleRule.FoundSurveillanceOrbital,
			SpecialModuleRule.FoundSurveillanceRing
		};

		// Token: 0x04002907 RID: 10503
		[fsIgnore]
		public static readonly List<SpecialModuleRule> FoundAnyStationRules = new List<SpecialModuleRule>
		{
			SpecialModuleRule.FoundAutomatedFissionPlatform,
			SpecialModuleRule.FoundAutomatedSolarPlatform,
			SpecialModuleRule.FoundFissionPlatform,
			SpecialModuleRule.FoundFusionPlatform,
			SpecialModuleRule.FoundSolarPlatform,
			SpecialModuleRule.FoundSurveillancePlatform,
			SpecialModuleRule.FoundSurveillanceOrbital,
			SpecialModuleRule.FoundSurveillanceRing
		};

		// Token: 0x04002908 RID: 10504
		[fsIgnore]
		public static readonly List<SpecialModuleRule> FoundAnyHabRules = new List<SpecialModuleRule>
		{
			SpecialModuleRule.FoundAutomatedFissionOutpost,
			SpecialModuleRule.FoundAutomatedFissionPlatform,
			SpecialModuleRule.FoundAutomatedSolarOutpost,
			SpecialModuleRule.FoundAutomatedSolarPlatform,
			SpecialModuleRule.FoundFissionOutpost,
			SpecialModuleRule.FoundFissionPlatform,
			SpecialModuleRule.FoundFusionOutpost,
			SpecialModuleRule.FoundFusionPlatform,
			SpecialModuleRule.FoundSolarOutpost,
			SpecialModuleRule.FoundSolarPlatform,
			SpecialModuleRule.FoundSurveillancePlatform,
			SpecialModuleRule.FoundSurveillanceOrbital,
			SpecialModuleRule.FoundSurveillanceRing
		};

		// Token: 0x02000F64 RID: 3940
		public class ArmorData
		{
			// Token: 0x06007DDB RID: 32219 RVA: 0x003242BA File Offset: 0x003224BA
			public ArmorData(int armorValue)
			{
				this.armorValue = armorValue;
				this.maxArmor = armorValue;
				this.chippedPct = 0f;
			}

			// Token: 0x06007DDC RID: 32220 RVA: 0x003242DB File Offset: 0x003224DB
			public void RepairArmor()
			{
				this.armorValue = this.maxArmor;
				this.chippedPct = 0f;
			}

			// Token: 0x06007DDD RID: 32221 RVA: 0x003242F4 File Offset: 0x003224F4
			public int ShredArmor(int armorValueToShred)
			{
				int num = this.armorValue;
				this.armorValue -= armorValueToShred;
				int num2 = armorValueToShred;
				if (this.armorValue < 0)
				{
					num2 = num;
					this.armorValue = 0;
				}
				return Mathf.Max(0, armorValueToShred - num2);
			}

			// Token: 0x06007DDE RID: 32222 RVA: 0x00324334 File Offset: 0x00322534
			public float ChipArmor(float chip)
			{
				float num = 1f - this.chippedPct;
				this.chippedPct += chip;
				float num2 = chip;
				if (this.chippedPct >= 1f)
				{
					this.chippedPct = 1f;
					num2 = num;
				}
				return Mathf.Max(0f, chip - num2);
			}

			// Token: 0x17001219 RID: 4633
			// (get) Token: 0x06007DDF RID: 32223 RVA: 0x00324385 File Offset: 0x00322585
			public bool damaged
			{
				get
				{
					return this.maxArmor > 0 && (this.chippedPct > 0f || this.armorValue < this.maxArmor);
				}
			}

			// Token: 0x06007DE0 RID: 32224 RVA: 0x003243AF File Offset: 0x003225AF
			public float GetArmorIntegrity()
			{
				if (this.maxArmor != 0)
				{
					return (float)this.armorValue * (1f - this.chippedPct) / (float)this.maxArmor;
				}
				return 0f;
			}

			// Token: 0x06007DE1 RID: 32225 RVA: 0x003243DC File Offset: 0x003225DC
			public static float GetArmorFacingVolume_m3(TISpaceShipState ship, ArmorFacing facing)
			{
				if (facing == ArmorFacing.Nose)
				{
					return ship.noseArmorTemplate.armor_section_volume((float)ship.template.noseArmorValue, ship.hull.length_m, ship.hull.width_m, ship.lateralArmorThickness_m, false);
				}
				if (facing == ArmorFacing.Tail)
				{
					return ship.tailArmorTemplate.armor_section_volume((float)ship.template.tailArmorValue, ship.hull.length_m, ship.hull.width_m, ship.lateralArmorThickness_m, false);
				}
				if (facing != ArmorFacing.Core)
				{
					return ship.lateralArmorTemplate.armor_section_volume((float)ship.template.lateralArmorValue, ship.hull.length_m, ship.hull.width_m, ship.lateralArmorThickness_m, true) / 2f;
				}
				return ship.hull.volume_m3;
			}

			// Token: 0x06007DE2 RID: 32226 RVA: 0x003244A6 File Offset: 0x003226A6
			public static TIShipArmorTemplate GetArmorTemplate(TISpaceShipState ship, ArmorFacing facing)
			{
				if (facing == ArmorFacing.Nose)
				{
					return ship.noseArmorTemplate;
				}
				if (facing == ArmorFacing.Tail)
				{
					return ship.tailArmorTemplate;
				}
				if (facing != ArmorFacing.Core)
				{
					return ship.lateralArmorTemplate;
				}
				return null;
			}

			// Token: 0x04005DF3 RID: 24051
			public int maxArmor;

			// Token: 0x04005DF4 RID: 24052
			public int armorValue;

			// Token: 0x04005DF5 RID: 24053
			public float chippedPct;
		}
	}
}
