using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000776 RID: 1910
	public class TISpaceDefensesFacilityState : TIRegionSpaceFacilityState, CombatWeaponCarrierState
	{
		// Token: 0x06003A1B RID: 14875 RVA: 0x00156874 File Offset: 0x00154A74
		public override float GetAIValuation()
		{
			return (float)(this.Extant() ? 1 : 0);
		}

		// Token: 0x06003A1C RID: 14876 RVA: 0x00156883 File Offset: 0x00154A83
		public override string GetDisplayName(TIFactionState faction)
		{
			return Loc.T("TIRegionTemplate.SpaceDefenseName.Generic", new object[] { base.region.displayName });
		}

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06003A1D RID: 14877 RVA: 0x001568A3 File Offset: 0x00154AA3
		public override string descriptor
		{
			get
			{
				return Loc.T("UI.Nation.SpaceDefenseFacility");
			}
		}

		// Token: 0x17000A18 RID: 2584
		// (get) Token: 0x06003A1E RID: 14878 RVA: 0x001568AF File Offset: 0x00154AAF
		public override string description
		{
			get
			{
				return Loc.T("UI.Nation.SpaceDefenseDescription");
			}
		}

		// Token: 0x06003A1F RID: 14879 RVA: 0x001568BB File Offset: 0x00154ABB
		public override bool Extant()
		{
			return base.region.antiSpaceDefenses;
		}

		// Token: 0x06003A20 RID: 14880 RVA: 0x001568C8 File Offset: 0x00154AC8
		public override int GetSize()
		{
			return 1;
		}

		// Token: 0x06003A21 RID: 14881 RVA: 0x001568CB File Offset: 0x00154ACB
		public override Sprite GetIcon(TIFactionState faction)
		{
			return AssetCacheManager.spaceDefensesIcon;
		}

		// Token: 0x06003A22 RID: 14882 RVA: 0x001568D2 File Offset: 0x00154AD2
		public override string GetIconResourcePath(TIFactionState faction)
		{
			return TemplateManager.global.pathGeoscapeSpaceDefenses;
		}

		// Token: 0x06003A23 RID: 14883 RVA: 0x001568DE File Offset: 0x00154ADE
		public override string GetIllustrationPath(TIFactionState faction)
		{
			return TemplateManager.global.illus_spaceDefensesPath;
		}

		// Token: 0x06003A24 RID: 14884 RVA: 0x001568EC File Offset: 0x00154AEC
		public void SetLaserDefenseWeaponTemplate()
		{
			this.weaponTemplate = TILaserWeaponTemplate.GetBestHeavyDefenseLaser(base.region.nation.executiveFaction, this.ref_spaceBody, 0);
			if (this.weaponTemplate != null)
			{
				this.weaponTemplateName = this.weaponTemplate.dataName;
				return;
			}
			this.weaponTemplateName = "RegionDefenseIRLaser";
			this.weaponTemplate = TemplateManager.Find<TILaserWeaponTemplate>(this.weaponTemplateName, false);
		}

		// Token: 0x06003A25 RID: 14885 RVA: 0x00156952 File Offset: 0x00154B52
		public TIGameState GetTargetableState()
		{
			return this;
		}

		// Token: 0x06003A26 RID: 14886 RVA: 0x00156955 File Offset: 0x00154B55
		public TIFactionState GetFaction()
		{
			return this.ref_faction;
		}

		// Token: 0x06003A27 RID: 14887 RVA: 0x0015695D File Offset: 0x00154B5D
		public bool WeaponIsOperable(ModuleDataEntry weaponData)
		{
			return true;
		}

		// Token: 0x06003A28 RID: 14888 RVA: 0x00156960 File Offset: 0x00154B60
		public bool WeaponCanFire(ModuleDataEntry weaponData)
		{
			return this.WeaponIsOperable(weaponData);
		}

		// Token: 0x06003A29 RID: 14889 RVA: 0x00156969 File Offset: 0x00154B69
		public void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile = null)
		{
		}

		// Token: 0x06003A2A RID: 14890 RVA: 0x0015696B File Offset: 0x00154B6B
		public void AddTargetedProjectile(TISpaceCombatProjectileState projectile)
		{
		}

		// Token: 0x06003A2B RID: 14891 RVA: 0x0015696D File Offset: 0x00154B6D
		public float FireControlFunction()
		{
			return 1f;
		}

		// Token: 0x06003A2C RID: 14892 RVA: 0x00156974 File Offset: 0x00154B74
		public TISpaceShipState ref_shipCarrier()
		{
			return null;
		}

		// Token: 0x06003A2D RID: 14893 RVA: 0x00156977 File Offset: 0x00154B77
		public TIHabModuleState ref_habModuleCarrier()
		{
			return null;
		}

		// Token: 0x06003A2E RID: 14894 RVA: 0x0015697A File Offset: 0x00154B7A
		public bool isShip()
		{
			return false;
		}

		// Token: 0x06003A2F RID: 14895 RVA: 0x0015697D File Offset: 0x00154B7D
		public bool isHabModule()
		{
			return false;
		}

		// Token: 0x06003A30 RID: 14896 RVA: 0x00156980 File Offset: 0x00154B80
		public float TargetingBonus(TIShipWeaponTemplate weapon, TIHabState alliedHab)
		{
			return TIEffectsState.SumEffectsModifiers(Context.TargetingComputerBonus, base.region.nation.executiveFaction, 0f, null);
		}

		// Token: 0x06003A31 RID: 14897 RVA: 0x001569A4 File Offset: 0x00154BA4
		public override void PostInitializationInit_4()
		{
			base.PostInitializationInit_4();
			if (this.Extant())
			{
				if (!base.region.underBombardment || string.IsNullOrEmpty(this.weaponTemplateName))
				{
					this.SetLaserDefenseWeaponTemplate();
					return;
				}
				this.weaponTemplate = TemplateManager.Find<TILaserWeaponTemplate>(this.weaponTemplateName, false);
				if (this.weaponTemplate == null)
				{
					this.SetLaserDefenseWeaponTemplate();
				}
			}
		}

		// Token: 0x06003A32 RID: 14898 RVA: 0x00156A00 File Offset: 0x00154C00
		public static bool STOShouldShootBack(TIRegionState shooter, TIGameState bombardmentTarget)
		{
			if (bombardmentTarget.ref_region.spaceDefenseFacility.Extant())
			{
				bool flag = false;
				if (bombardmentTarget.isArmyState && bombardmentTarget.ref_army.homeNation.IsAlliedWith(shooter.nation, true))
				{
					flag = true;
				}
				else if (bombardmentTarget.isRegionState || bombardmentTarget.isRegionSpaceFacility)
				{
					flag = true;
				}
				else if (bombardmentTarget.isRegionAlienFacility || bombardmentTarget.isRegionLandedUFO)
				{
					TIFactionState executiveFaction = shooter.nation.executiveFaction;
					flag = executiveFaction != null && executiveFaction.permanentAlly(bombardmentTarget.ref_faction);
				}
				else if (bombardmentTarget.isRegionXenoformingState)
				{
					flag = shooter.nation.alienNation;
				}
				return flag;
			}
			return false;
		}

		// Token: 0x06003A33 RID: 14899 RVA: 0x00156AA4 File Offset: 0x00154CA4
		public static TISpaceShipState SelectEarthSTOTarget(TIRegionState shooter, TIDateTime time, TISpaceFleetState targetFleet = null, bool lineOfSightEstablished = false)
		{
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			if (targetFleet == null || !lineOfSightEstablished)
			{
				using (List<TISpaceFleetState>.Enumerator enumerator = shooter.ref_spaceBody.fleetsInOrbit.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceFleetState tispaceFleetState = enumerator.Current;
						TIGameState bombardmentTarget = tispaceFleetState.bombardmentTarget;
						if (bombardmentTarget != null && bombardmentTarget.ref_region == shooter.ref_region && TISpaceDefensesFacilityState.STOShouldShootBack(shooter, bombardmentTarget) && (lineOfSightEstablished || TISpaceShipState.BombardmentTargetInLineOfSight(tispaceFleetState.ships[0], shooter, time)))
						{
							list.AddRange(tispaceFleetState.ships);
						}
					}
					goto IL_00CA;
				}
			}
			list.AddRange(targetFleet.ships);
			IL_00CA:
			Dictionary<TISpaceShipState, float> targetDict = list.ToDictionary<TISpaceShipState, TISpaceShipState, float>((TISpaceShipState x) => x, (TISpaceShipState x) => x.BombardmentValue(shooter.ref_spaceBody));
			if (list.Count > 0)
			{
				if (targetDict.Values.Any<float>((float x) => x > 0f))
				{
					return list.SelectRandomWeightedItem<TISpaceShipState>((TISpaceShipState x) => targetDict[x], -1f, 1E-37f);
				}
			}
			return null;
		}

		// Token: 0x06003A34 RID: 14900 RVA: 0x00156C1C File Offset: 0x00154E1C
		public void OnFireMissionOrder(TISpaceShipState target, TIDateTime currentTime)
		{
			if (this.weapon == null)
			{
				if (base.region.spaceDefenseFacility.weapon == null)
				{
					this.SetLaserDefenseWeaponTemplate();
				}
				ModuleDataEntry moduleDataEntry = new ModuleDataEntry(base.region.spaceDefenseFacility.weaponTemplate, 0);
				this.weapon = new BeamWeapon(base.region.spaceDefenseFacility, moduleDataEntry);
			}
			if (TIUtilities.RandomFloatValue() < 1f + this.TargetingBonus(base.region.spaceDefenseFacility.weaponTemplate, null) - target.ECMValue(base.region.nation.executiveFaction, null))
			{
				TINaturalSpaceObjectState ref_naturalSpaceObject = this.ref_naturalSpaceObject;
				if (((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.controller : null) != null && this.ref_naturalSpaceObject.controller.modelLink != null && this.ref_naturalSpaceObject.controller.modelLink.activeInHierarchy)
				{
					this.FacilityMarkerController.DisplaySTOBeam(target, currentTime);
				}
				StrategyShipController component = target.visualizerLink.transform.parent.GetComponent<StrategyShipController>();
				this.weapon.SetTarget_Strategy(component, component.ShipState.globalPositionAtTime(currentTime));
				for (int i = 0; i < 2; i++)
				{
					if (TIGameState.Valid(target))
					{
						BeamWeapon.Beam damageSource = this.weapon.GetDamageSource(this, target.fleet.bombardmentAltitude_km);
						float num = component.ApplyDamage(damageSource);
						if (component.ShipState.ShipDestroyed())
						{
							component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireKill", new object[]
							{
								currentTime.ToCustomTimeString(),
								this.displayName,
								this.weapon.weaponTemplate.displayName,
								component.ShipState.displayName,
								TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
								TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false),
								base.region.nation.displayName
							}), currentTime);
							TINotificationQueueState.LogShipDestroyedInStrat(component.ShipState, base.region.ref_factions, component.ShipState.fleet.location, new Dictionary<TIFactionState, string> { 
							{
								component.ShipState.faction,
								component.ShipState.KillAllOfficersReport()
							} });
							TISpaceShipState shipState = component.ShipState;
							bool flag = true;
							CombatWeaponCarrierState attacker = damageSource.attacker;
							shipState.DestroyShip(flag, (attacker != null) ? attacker.GetFaction() : null);
						}
						else if (num > 0f)
						{
							component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireHit", new object[]
							{
								currentTime.ToCustomTimeString(),
								this.displayName,
								this.weapon.weaponTemplate.displayName,
								component.ShipState.displayName,
								TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
								TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false),
								base.region.nation.displayName
							}), currentTime);
						}
						else
						{
							component.ShipState.fleet.AddToBombardmentLog(Loc.T("Bombard.Log.CounterfireAbsorbed", new object[]
							{
								currentTime.ToCustomTimeString(),
								this.displayName,
								this.weapon.weaponTemplate.displayName,
								component.ShipState.displayName,
								TIUtilities.FormatBigOrSmallNumber(damageSource.damage.amount, 1, 7, 0, false, false),
								base.region.nation.displayName
							}), currentTime);
						}
					}
				}
				return;
			}
			target.ref_fleet.AddToBombardmentLog(Loc.T("Bombard.Log.ECMPreventedCounterfire", new object[]
			{
				currentTime.ToCustomTimeString(),
				target.displayName
			}), currentTime);
		}

		// Token: 0x04002576 RID: 9590
		[fsIgnore]
		public TILaserWeaponTemplate weaponTemplate;

		// Token: 0x04002577 RID: 9591
		public string weaponTemplateName;

		// Token: 0x04002578 RID: 9592
		private BeamWeapon weapon;

		// Token: 0x04002579 RID: 9593
		[SerializeField]
		private TIDateTime lastTimeFired;
	}
}
