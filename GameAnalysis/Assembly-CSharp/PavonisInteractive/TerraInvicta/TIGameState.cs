using System;
using System.Collections.Generic;
using System.Reflection;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D6 RID: 2006
	[fsObject(Converter = typeof(TIGameStateConverter))]
	public abstract class TIGameState : TIDataClass, IEquatable<TIGameState>, IComparable<TIGameState>
	{
		// Token: 0x17000E2C RID: 3628
		// (get) Token: 0x0600481A RID: 18458 RVA: 0x001DCEDF File Offset: 0x001DB0DF
		// (set) Token: 0x0600481B RID: 18459 RVA: 0x001DCEE7 File Offset: 0x001DB0E7
		public bool archived { get; private set; }

		// Token: 0x17000E2D RID: 3629
		// (get) Token: 0x0600481C RID: 18460 RVA: 0x001DCEF0 File Offset: 0x001DB0F0
		// (set) Token: 0x0600481D RID: 18461 RVA: 0x001DCEF8 File Offset: 0x001DB0F8
		public GameStateID ID { get; set; }

		// Token: 0x0600481E RID: 18462 RVA: 0x001DCF01 File Offset: 0x001DB101
		public TIGameState()
		{
		}

		// Token: 0x0600481F RID: 18463 RVA: 0x001DCF10 File Offset: 0x001DB110
		public TIGameState(GameStateID ID)
		{
			this.ID = ID;
		}

		// Token: 0x06004820 RID: 18464 RVA: 0x001DCF26 File Offset: 0x001DB126
		public virtual bool Initialize()
		{
			return true;
		}

		// Token: 0x06004821 RID: 18465 RVA: 0x001DCF29 File Offset: 0x001DB129
		public void SetTemplate<T>(TIDataTemplate template) where T : TIDataTemplate
		{
			this.template = template;
		}

		// Token: 0x06004822 RID: 18466 RVA: 0x001DCF32 File Offset: 0x001DB132
		public virtual T GetMyTemplate<T>() where T : TIDataTemplate
		{
			if (this.template == null && this.templateName != null)
			{
				this.template = TemplateManager.Find<T>(this.templateName, true);
			}
			return this.template as T;
		}

		// Token: 0x06004823 RID: 18467 RVA: 0x001DCF6C File Offset: 0x001DB16C
		public TIDataTemplate GetMyTemplate()
		{
			PropertyInfo property = base.GetType().GetProperty("template", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			if (property == null)
			{
				string text = "Attempted to get template without a type, define a template property in ";
				Type type = base.GetType();
				Debug.LogError(text + ((type != null) ? type.ToString() : null));
				return null;
			}
			return property.GetGetMethod().Invoke(this, null) as TIDataTemplate;
		}

		// Token: 0x06004824 RID: 18468 RVA: 0x001DCFCA File Offset: 0x001DB1CA
		public virtual void InitWithTemplate(TIDataTemplate template)
		{
		}

		// Token: 0x06004825 RID: 18469 RVA: 0x001DCFCC File Offset: 0x001DB1CC
		public virtual string GetDisplayName(TIFactionState faction)
		{
			return this.displayName;
		}

		// Token: 0x06004826 RID: 18470 RVA: 0x001DCFD4 File Offset: 0x001DB1D4
		public virtual void PostGameStateCreateInit_OnCreationOnly_1()
		{
		}

		// Token: 0x06004827 RID: 18471 RVA: 0x001DCFD6 File Offset: 0x001DB1D6
		public virtual void PostGlobalGameStateCreateInit_2()
		{
		}

		// Token: 0x06004828 RID: 18472 RVA: 0x001DCFD8 File Offset: 0x001DB1D8
		public virtual void PostCanvasManagerCreateInit_3()
		{
		}

		// Token: 0x06004829 RID: 18473 RVA: 0x001DCFDA File Offset: 0x001DB1DA
		public virtual void PostInitializationInit_4()
		{
		}

		// Token: 0x0600482A RID: 18474 RVA: 0x001DCFDC File Offset: 0x001DB1DC
		public virtual void PostAllStartUpInit_5()
		{
		}

		// Token: 0x0600482B RID: 18475 RVA: 0x001DCFDE File Offset: 0x001DB1DE
		public virtual void PostVisualizerCreationInit_6()
		{
		}

		// Token: 0x0600482C RID: 18476 RVA: 0x001DCFE0 File Offset: 0x001DB1E0
		public virtual void PostVisualizerCreationInit_7()
		{
		}

		// Token: 0x0600482D RID: 18477 RVA: 0x001DCFE2 File Offset: 0x001DB1E2
		public virtual void PostEverythingSaveRepair_8()
		{
		}

		// Token: 0x0600482E RID: 18478 RVA: 0x001DCFE4 File Offset: 0x001DB1E4
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}", base.GetType().Name, this.ID, this.displayName + " (" + this.GetDisplayName(GameControl.control.activePlayer)) + " )";
		}

		// Token: 0x0600482F RID: 18479 RVA: 0x001DD03B File Offset: 0x001DB23B
		public void DeArchiveState()
		{
			this.archived = false;
		}

		// Token: 0x06004830 RID: 18480 RVA: 0x001DD044 File Offset: 0x001DB244
		public void ArchiveState(bool trigger = true)
		{
			this.archived = true;
			if (trigger)
			{
				GameControl.eventManager.TriggerEvent(new GameStateArchived(this), null, new object[] { this });
			}
		}

		// Token: 0x06004831 RID: 18481 RVA: 0x001DD06B File Offset: 0x001DB26B
		public virtual void SetDisplayName(string name)
		{
			this.displayName = name;
		}

		// Token: 0x06004832 RID: 18482 RVA: 0x001DD074 File Offset: 0x001DB274
		public bool Equals(TIGameState other)
		{
			return other != null && (this == other || this.ID.Equals(other.ID));
		}

		// Token: 0x06004833 RID: 18483 RVA: 0x001DD0A0 File Offset: 0x001DB2A0
		public int CompareTo(TIGameState other)
		{
			return this.ID.CompareTo(other.ID);
		}

		// Token: 0x06004834 RID: 18484 RVA: 0x001DD0C1 File Offset: 0x001DB2C1
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((TIGameState)obj)));
		}

		// Token: 0x06004835 RID: 18485 RVA: 0x001DD0F0 File Offset: 0x001DB2F0
		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		// Token: 0x06004836 RID: 18486 RVA: 0x001DD111 File Offset: 0x001DB311
		public static bool operator ==(TIGameState left, TIGameState right)
		{
			return object.Equals(left, right);
		}

		// Token: 0x06004837 RID: 18487 RVA: 0x001DD11A File Offset: 0x001DB31A
		public static bool operator !=(TIGameState left, TIGameState right)
		{
			return !object.Equals(left, right);
		}

		// Token: 0x17000E2E RID: 3630
		// (get) Token: 0x06004838 RID: 18488 RVA: 0x001DD126 File Offset: 0x001DB326
		public bool deleted
		{
			get
			{
				return !this.exists;
			}
		}

		// Token: 0x17000E2F RID: 3631
		// (get) Token: 0x06004839 RID: 18489 RVA: 0x001DD131 File Offset: 0x001DB331
		// (set) Token: 0x0600483A RID: 18490 RVA: 0x001DD139 File Offset: 0x001DB339
		public bool exists { get; set; }

		// Token: 0x0600483B RID: 18491 RVA: 0x001DD142 File Offset: 0x001DB342
		public static bool Valid(TIGameState gameState)
		{
			return gameState != null && gameState.exists;
		}

		// Token: 0x17000E30 RID: 3632
		// (get) Token: 0x0600483C RID: 18492 RVA: 0x001DD155 File Offset: 0x001DB355
		public virtual bool isRegionState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E31 RID: 3633
		// (get) Token: 0x0600483D RID: 18493 RVA: 0x001DD158 File Offset: 0x001DB358
		public virtual bool isNationState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E32 RID: 3634
		// (get) Token: 0x0600483E RID: 18494 RVA: 0x001DD15B File Offset: 0x001DB35B
		public virtual bool isFactionState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E33 RID: 3635
		// (get) Token: 0x0600483F RID: 18495 RVA: 0x001DD15E File Offset: 0x001DB35E
		public virtual bool isHabState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E34 RID: 3636
		// (get) Token: 0x06004840 RID: 18496 RVA: 0x001DD161 File Offset: 0x001DB361
		public virtual bool isHabSiteState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E35 RID: 3637
		// (get) Token: 0x06004841 RID: 18497 RVA: 0x001DD164 File Offset: 0x001DB364
		public virtual bool isOrbitState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E36 RID: 3638
		// (get) Token: 0x06004842 RID: 18498 RVA: 0x001DD167 File Offset: 0x001DB367
		public virtual bool isSpaceFleetState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E37 RID: 3639
		// (get) Token: 0x06004843 RID: 18499 RVA: 0x001DD16A File Offset: 0x001DB36A
		public virtual bool isCouncilorState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E38 RID: 3640
		// (get) Token: 0x06004844 RID: 18500 RVA: 0x001DD16D File Offset: 0x001DB36D
		public virtual bool isArmyState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E39 RID: 3641
		// (get) Token: 0x06004845 RID: 18501 RVA: 0x001DD170 File Offset: 0x001DB370
		public virtual bool isSpaceObjectState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3A RID: 3642
		// (get) Token: 0x06004846 RID: 18502 RVA: 0x001DD173 File Offset: 0x001DB373
		public virtual bool isSpaceGameState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3B RID: 3643
		// (get) Token: 0x06004847 RID: 18503 RVA: 0x001DD176 File Offset: 0x001DB376
		public virtual bool isNaturalSpaceObjectState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3C RID: 3644
		// (get) Token: 0x06004848 RID: 18504 RVA: 0x001DD179 File Offset: 0x001DB379
		public virtual bool isSpaceBodyState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3D RID: 3645
		// (get) Token: 0x06004849 RID: 18505 RVA: 0x001DD17C File Offset: 0x001DB37C
		public virtual bool isLagrangePointState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3E RID: 3646
		// (get) Token: 0x0600484A RID: 18506 RVA: 0x001DD17F File Offset: 0x001DB37F
		public virtual bool isSpaceAssetState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E3F RID: 3647
		// (get) Token: 0x0600484B RID: 18507 RVA: 0x001DD182 File Offset: 0x001DB382
		public virtual bool isSpaceShipState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E40 RID: 3648
		// (get) Token: 0x0600484C RID: 18508 RVA: 0x001DD185 File Offset: 0x001DB385
		public virtual bool isHabModuleState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E41 RID: 3649
		// (get) Token: 0x0600484D RID: 18509 RVA: 0x001DD188 File Offset: 0x001DB388
		public virtual bool isControlPointState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E42 RID: 3650
		// (get) Token: 0x0600484E RID: 18510 RVA: 0x001DD18B File Offset: 0x001DB38B
		public virtual bool isRegionAlienEntity
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E43 RID: 3651
		// (get) Token: 0x0600484F RID: 18511 RVA: 0x001DD18E File Offset: 0x001DB38E
		public virtual bool isRegionSpaceFacility
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E44 RID: 3652
		// (get) Token: 0x06004850 RID: 18512 RVA: 0x001DD191 File Offset: 0x001DB391
		public virtual bool isRegionAlienAsset
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E45 RID: 3653
		// (get) Token: 0x06004851 RID: 18513 RVA: 0x001DD194 File Offset: 0x001DB394
		public virtual bool isRegionUFOCrashdown
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E46 RID: 3654
		// (get) Token: 0x06004852 RID: 18514 RVA: 0x001DD197 File Offset: 0x001DB397
		public virtual bool isRegionXenoformingState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E47 RID: 3655
		// (get) Token: 0x06004853 RID: 18515 RVA: 0x001DD19A File Offset: 0x001DB39A
		public virtual bool isRegionLandedUFO
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E48 RID: 3656
		// (get) Token: 0x06004854 RID: 18516 RVA: 0x001DD19D File Offset: 0x001DB39D
		public virtual bool isRegionAlienFacility
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E49 RID: 3657
		// (get) Token: 0x06004855 RID: 18517 RVA: 0x001DD1A0 File Offset: 0x001DB3A0
		public virtual bool isRegionAlienActivity
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E4A RID: 3658
		// (get) Token: 0x06004856 RID: 18518 RVA: 0x001DD1A3 File Offset: 0x001DB3A3
		public virtual bool isWarState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E4B RID: 3659
		// (get) Token: 0x06004857 RID: 18519 RVA: 0x001DD1A6 File Offset: 0x001DB3A6
		public virtual bool isOrgState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E4C RID: 3660
		// (get) Token: 0x06004858 RID: 18520 RVA: 0x001DD1A9 File Offset: 0x001DB3A9
		public virtual bool isOfficerState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E4D RID: 3661
		// (get) Token: 0x06004859 RID: 18521 RVA: 0x001DD1AC File Offset: 0x001DB3AC
		public virtual Searchable searchable
		{
			get
			{
				return Searchable.never;
			}
		}

		// Token: 0x17000E4E RID: 3662
		// (get) Token: 0x0600485A RID: 18522 RVA: 0x001DD1AF File Offset: 0x001DB3AF
		public TIGameState ref_gameState
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000E4F RID: 3663
		// (get) Token: 0x0600485B RID: 18523 RVA: 0x001DD1B2 File Offset: 0x001DB3B2
		public virtual TIRegionState ref_region
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E50 RID: 3664
		// (get) Token: 0x0600485C RID: 18524 RVA: 0x001DD1B5 File Offset: 0x001DB3B5
		public virtual TINationState ref_nation
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E51 RID: 3665
		// (get) Token: 0x0600485D RID: 18525 RVA: 0x001DD1B8 File Offset: 0x001DB3B8
		public virtual TIFactionState ref_faction
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x0600485E RID: 18526 RVA: 0x001DD1BB File Offset: 0x001DB3BB
		public virtual List<TIFactionState> ref_factions
		{
			get
			{
				if (!(this.ref_faction != null))
				{
					return new List<TIFactionState>();
				}
				return new List<TIFactionState>(1) { this.ref_faction };
			}
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x0600485F RID: 18527 RVA: 0x001DD1E3 File Offset: 0x001DB3E3
		public virtual TIHabState ref_hab
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x06004860 RID: 18528 RVA: 0x001DD1E6 File Offset: 0x001DB3E6
		public virtual TIHabSiteState ref_habSite
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x06004861 RID: 18529 RVA: 0x001DD1E9 File Offset: 0x001DB3E9
		public virtual TIOrbitState ref_orbit
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E56 RID: 3670
		// (get) Token: 0x06004862 RID: 18530 RVA: 0x001DD1EC File Offset: 0x001DB3EC
		public virtual TISpaceFleetState ref_fleet
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E57 RID: 3671
		// (get) Token: 0x06004863 RID: 18531 RVA: 0x001DD1EF File Offset: 0x001DB3EF
		public virtual TICouncilorState ref_councilor
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E58 RID: 3672
		// (get) Token: 0x06004864 RID: 18532 RVA: 0x001DD1F2 File Offset: 0x001DB3F2
		public virtual TIArmyState ref_army
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E59 RID: 3673
		// (get) Token: 0x06004865 RID: 18533 RVA: 0x001DD1F5 File Offset: 0x001DB3F5
		public virtual TISpaceObjectState ref_spaceObject
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5A RID: 3674
		// (get) Token: 0x06004866 RID: 18534 RVA: 0x001DD1F8 File Offset: 0x001DB3F8
		public virtual TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5B RID: 3675
		// (get) Token: 0x06004867 RID: 18535 RVA: 0x001DD1FB File Offset: 0x001DB3FB
		public virtual TILagrangePointState ref_lagrangePoint
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5C RID: 3676
		// (get) Token: 0x06004868 RID: 18536 RVA: 0x001DD1FE File Offset: 0x001DB3FE
		public virtual TISpaceBodyState ref_spaceBody
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5D RID: 3677
		// (get) Token: 0x06004869 RID: 18537 RVA: 0x001DD201 File Offset: 0x001DB401
		public virtual TISpaceAssetState ref_spaceAsset
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5E RID: 3678
		// (get) Token: 0x0600486A RID: 18538 RVA: 0x001DD204 File Offset: 0x001DB404
		public virtual TISpaceShipState ref_ship
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E5F RID: 3679
		// (get) Token: 0x0600486B RID: 18539 RVA: 0x001DD207 File Offset: 0x001DB407
		public virtual TIHabModuleState ref_habModule
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E60 RID: 3680
		// (get) Token: 0x0600486C RID: 18540 RVA: 0x001DD20A File Offset: 0x001DB40A
		public virtual TIControlPoint ref_controlPoint
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E61 RID: 3681
		// (get) Token: 0x0600486D RID: 18541 RVA: 0x001DD20D File Offset: 0x001DB40D
		public virtual TIRegionAlienEntityState ref_regionAlienEntity
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E62 RID: 3682
		// (get) Token: 0x0600486E RID: 18542 RVA: 0x001DD210 File Offset: 0x001DB410
		public virtual TIRegionSpaceFacilityState ref_regionSpaceFacility
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E63 RID: 3683
		// (get) Token: 0x0600486F RID: 18543 RVA: 0x001DD213 File Offset: 0x001DB413
		public virtual TIRegionAlienAssetState ref_regionAlienAsset
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E64 RID: 3684
		// (get) Token: 0x06004870 RID: 18544 RVA: 0x001DD216 File Offset: 0x001DB416
		public virtual TIRegionXenoformingState ref_xenoforming
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E65 RID: 3685
		// (get) Token: 0x06004871 RID: 18545 RVA: 0x001DD219 File Offset: 0x001DB419
		public virtual TIRegionUFOLandingState ref_UFOLanding
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E66 RID: 3686
		// (get) Token: 0x06004872 RID: 18546 RVA: 0x001DD21C File Offset: 0x001DB41C
		public virtual TIRegionUFOCrashdownState ref_UFOCrashdown
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E67 RID: 3687
		// (get) Token: 0x06004873 RID: 18547 RVA: 0x001DD21F File Offset: 0x001DB41F
		public virtual TIRegionAlienFacilityState ref_alienFacility
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E68 RID: 3688
		// (get) Token: 0x06004874 RID: 18548 RVA: 0x001DD222 File Offset: 0x001DB422
		public virtual TIRegionAlienActivityState ref_regionAlienActivity
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E69 RID: 3689
		// (get) Token: 0x06004875 RID: 18549 RVA: 0x001DD225 File Offset: 0x001DB425
		public virtual TIOrgState ref_org
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E6A RID: 3690
		// (get) Token: 0x06004876 RID: 18550 RVA: 0x001DD228 File Offset: 0x001DB428
		public virtual TIOfficerState ref_officer
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E6B RID: 3691
		// (get) Token: 0x06004877 RID: 18551 RVA: 0x001DD22B File Offset: 0x001DB42B
		public TISpaceBodyState ref_system
		{
			get
			{
				TINaturalSpaceObjectState ref_naturalSpaceObject = this.ref_naturalSpaceObject;
				if (ref_naturalSpaceObject == null)
				{
					return null;
				}
				TISpaceObjectState getSunOrbitingRelatedObject = ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
				if (getSunOrbitingRelatedObject == null)
				{
					return null;
				}
				return getSunOrbitingRelatedObject.ref_spaceBody;
			}
		}

		// Token: 0x17000E6C RID: 3692
		// (get) Token: 0x06004878 RID: 18552 RVA: 0x001DD249 File Offset: 0x001DB449
		public virtual TIWarState ref_war
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000E6D RID: 3693
		// (get) Token: 0x06004879 RID: 18553 RVA: 0x001DD24C File Offset: 0x001DB44C
		public virtual bool hasMapObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E6E RID: 3694
		// (get) Token: 0x0600487A RID: 18554 RVA: 0x001DD24F File Offset: 0x001DB44F
		public virtual bool hasEarthMapObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E6F RID: 3695
		// (get) Token: 0x0600487B RID: 18555 RVA: 0x001DD252 File Offset: 0x001DB452
		public virtual bool inSpace
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000E70 RID: 3696
		// (get) Token: 0x0600487C RID: 18556 RVA: 0x001DD255 File Offset: 0x001DB455
		// (set) Token: 0x0600487D RID: 18557 RVA: 0x001DD25D File Offset: 0x001DB45D
		public virtual int finderSortOverride { get; set; } = -1;

		// Token: 0x040029AE RID: 10670
		public string templateName;

		// Token: 0x040029AF RID: 10671
		public string displayName;

		// Token: 0x040029B0 RID: 10672
		private TIDataTemplate template;
	}
}
