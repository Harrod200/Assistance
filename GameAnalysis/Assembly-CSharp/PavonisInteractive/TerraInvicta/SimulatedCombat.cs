using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ModestTree;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A0 RID: 1952
	public class SimulatedCombat
	{
		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x06003EA9 RID: 16041 RVA: 0x001955AD File Offset: 0x001937AD
		public IEnumerable<SimulatedCombat.SimulatedShip> AllShips
		{
			get
			{
				return this.ShipsA.Concat<SimulatedCombat.SimulatedShip>(this.ShipsB);
			}
		}

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x06003EAA RID: 16042 RVA: 0x001955C0 File Offset: 0x001937C0
		public IEnumerable<SimulatedCombat.SimulatedCombatant> CombatantsA
		{
			get
			{
				IEnumerable<SimulatedCombat.SimulatedCombatant> enumerable = this.ShipsA;
				if (this.HabSupportsA)
				{
					enumerable = enumerable.Concat<SimulatedCombat.SimulatedCombatant>(this.CombatHabModules);
				}
				return enumerable;
			}
		}

		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x06003EAB RID: 16043 RVA: 0x001955EC File Offset: 0x001937EC
		public IEnumerable<SimulatedCombat.SimulatedCombatant> CombatantsB
		{
			get
			{
				IEnumerable<SimulatedCombat.SimulatedCombatant> enumerable = this.ShipsB;
				if (!this.HabSupportsA)
				{
					enumerable = enumerable.Concat<SimulatedCombat.SimulatedCombatant>(this.CombatHabModules);
				}
				return enumerable;
			}
		}

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x06003EAC RID: 16044 RVA: 0x00195616 File Offset: 0x00193816
		public IEnumerable<SimulatedCombat.SimulatedCombatant> AllCombatants
		{
			get
			{
				return this.CombatantsA.Concat<SimulatedCombat.SimulatedCombatant>(this.CombatantsB);
			}
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x06003EAD RID: 16045 RVA: 0x0019562C File Offset: 0x0019382C
		public IEnumerable<TIFactionState> Factions
		{
			get
			{
				return (from x in this.AllCombatants
					select x.GetFaction() into x
					where x != null
					select x).Distinct<TIFactionState>();
			}
		}

		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x06003EAE RID: 16046 RVA: 0x0019568C File Offset: 0x0019388C
		public Dictionary<TISpaceShipState, List<TIOfficerState>> DeadSimulatedOfficers
		{
			get
			{
				return this.AllShips.ToDictionary<SimulatedCombat.SimulatedShip, TISpaceShipState, List<TIOfficerState>>((SimulatedCombat.SimulatedShip x) => x.CopyShip, (SimulatedCombat.SimulatedShip x) => x.DeadSimulatedOfficers.ToList<TIOfficerState>());
			}
		}

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x06003EAF RID: 16047 RVA: 0x001956E4 File Offset: 0x001938E4
		public Dictionary<TIFactionState, List<string>> SimulatedOfficerDeathsRecord
		{
			get
			{
				return (from x in this.DeadSimulatedOfficers
					group x by x.Key.faction).ToDictionary<IGrouping<TIFactionState, KeyValuePair<TISpaceShipState, List<TIOfficerState>>>, TIFactionState, List<string>>((IGrouping<TIFactionState, KeyValuePair<TISpaceShipState, List<TIOfficerState>>> x) => x.Key, (IGrouping<TIFactionState, KeyValuePair<TISpaceShipState, List<TIOfficerState>>> x) => x.SelectMany<KeyValuePair<TISpaceShipState, List<TIOfficerState>>, string>((KeyValuePair<TISpaceShipState, List<TIOfficerState>> y) => y.Value.Select<TIOfficerState, string>((TIOfficerState z) => z.DisplayNameAndShipAndJob)).ToList<string>());
			}
		}

		// Token: 0x06003EB0 RID: 16048 RVA: 0x00195760 File Offset: 0x00193960
		public SimulatedCombat(IEnumerable<TISpaceShipState> shipsA, IEnumerable<TISpaceShipState> shipsB, float maxDuration_s, TIHabState hab = null)
		{
			this.ShipsA.UnionWith(shipsA.Select<TISpaceShipState, SimulatedCombat.SimulatedShip>((TISpaceShipState x) => new SimulatedCombat.SimulatedShip(this, x)));
			SimulatedCombat.SimulatedFormation simulatedFormation = new SimulatedCombat.SimulatedFormation(this, this.ShipsA);
			this.Formations.Add(simulatedFormation);
			this.ShipsB.UnionWith(shipsB.Select<TISpaceShipState, SimulatedCombat.SimulatedShip>((TISpaceShipState x) => new SimulatedCombat.SimulatedShip(this, x)));
			SimulatedCombat.SimulatedFormation simulatedFormation2 = new SimulatedCombat.SimulatedFormation(this, this.ShipsB);
			this.Formations.Add(simulatedFormation2);
			SimulatedCombat.SimulatedFormation simulatedFormation3 = null;
			if (hab != null)
			{
				this.Hab = hab;
				this.CombatHabModules.UnionWith(from x in hab.ActiveCombatModules()
					select new SimulatedCombat.SimulatedCombatHabModule(this, x));
				simulatedFormation3 = new SimulatedCombat.SimulatedFormation(this, this.CombatHabModules);
				this.Formations.Add(simulatedFormation3);
				if (this.ShipsA.Count > 0)
				{
					this.HabSupportsA = hab.faction.permanentAlly(this.ShipsA.First<SimulatedCombat.SimulatedShip>().GetFaction());
				}
				else
				{
					this.HabSupportsA = !hab.faction.permanentAlly(this.ShipsB.First<SimulatedCombat.SimulatedShip>().GetFaction());
				}
			}
			this.MaxDuration_s = maxDuration_s;
			this.SetupFormationDistances(simulatedFormation, simulatedFormation2, simulatedFormation3);
		}

		// Token: 0x06003EB1 RID: 16049 RVA: 0x001958C8 File Offset: 0x00193AC8
		public SimulatedCombat(IEnumerable<TISpaceShipTemplate> shipsA, IEnumerable<TISpaceShipTemplate> shipsB, float maxDuration_s)
		{
			this.ShipsA.UnionWith(shipsA.Select<TISpaceShipTemplate, SimulatedCombat.SimulatedShip>((TISpaceShipTemplate x) => new SimulatedCombat.SimulatedShip(this, x)));
			this.Formations.Add(new SimulatedCombat.SimulatedFormation(this, this.ShipsA));
			this.ShipsB.UnionWith(shipsB.Select<TISpaceShipTemplate, SimulatedCombat.SimulatedShip>((TISpaceShipTemplate x) => new SimulatedCombat.SimulatedShip(this, x)));
			this.Formations.Add(new SimulatedCombat.SimulatedFormation(this, this.ShipsB));
			this.MaxDuration_s = maxDuration_s;
		}

		// Token: 0x06003EB2 RID: 16050 RVA: 0x00195978 File Offset: 0x00193B78
		private void SetupFormationDistances(SimulatedCombat.SimulatedFormation shipsAFormation, SimulatedCombat.SimulatedFormation shipsBFormation, SimulatedCombat.SimulatedFormation habFormation = null)
		{
			float largestOffensiveRange_km = this.GetLargestOffensiveRange_km();
			shipsAFormation.AddStartingDistance(shipsBFormation, largestOffensiveRange_km);
			if (habFormation != null)
			{
				float num = largestOffensiveRange_km;
				if (this.ShipsA.Count > 0 && this.ShipsB.Count > 0)
				{
					num += 500f;
				}
				if (this.HabSupportsA)
				{
					habFormation.AddStartingDistance(shipsBFormation, num);
					return;
				}
				habFormation.AddStartingDistance(shipsAFormation, num);
			}
		}

		// Token: 0x06003EB3 RID: 16051 RVA: 0x001959D8 File Offset: 0x00193BD8
		private float GetLargestOffensiveRange_km()
		{
			return (from x in this.AllCombatants.SelectMany<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedCombatant x) => x.Weapons)
				where x.IsAttacking
				select x).Max<SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedWeapon x) => x.Template.targetingRange_km);
		}

		// Token: 0x06003EB4 RID: 16052 RVA: 0x00195A58 File Offset: 0x00193C58
		public CombatRecord GetCombatRecord(CombatRecord prologue = default(CombatRecord))
		{
			CombatRecord combatRecord = prologue.Copy();
			combatRecord.faction1 = this.Factions.First<TIFactionState>();
			combatRecord.faction2 = this.Factions.Second<TIFactionState>();
			foreach (SimulatedCombat.SimulatedShip simulatedShip in this.ShipsA.Concat<SimulatedCombat.SimulatedShip>(this.ShipsB))
			{
				if (simulatedShip.isDestroyed)
				{
					TISpaceShipState copyShip = simulatedShip.CopyShip;
					SimulatedCombat.SimulatedCombatant destroyer = simulatedShip.Destroyer;
					TIGameState tigameState = ((destroyer != null) ? destroyer.OriginalGameState : null);
					SimulatedCombat.SimulatedWeapon destroyerWeapon = simulatedShip.DestroyerWeapon;
					combatRecord.AddAssetDestroyedRecord(copyShip, tigameState, (destroyerWeapon != null) ? destroyerWeapon.Template : null);
				}
				else
				{
					combatRecord.AddAssetSurvivedRecord(simulatedShip.CopyShip, false, SingleAssetCombatOutcome.None);
				}
			}
			if (this.Hab != null)
			{
				SingleAssetCombatOutcome singleAssetCombatOutcome;
				if (this.CombatHabModules.Count > 0)
				{
					if (this.CombatHabModules.All<SimulatedCombat.SimulatedCombatHabModule>((SimulatedCombat.SimulatedCombatHabModule x) => x.isDestroyed))
					{
						singleAssetCombatOutcome = SingleAssetCombatOutcome.HabDisabled;
					}
					else
					{
						singleAssetCombatOutcome = SingleAssetCombatOutcome.Undamaged;
					}
				}
				else
				{
					singleAssetCombatOutcome = SingleAssetCombatOutcome.HabNoncombatant;
				}
				combatRecord.AddAssetSurvivedRecord(this.Hab, false, singleAssetCombatOutcome);
			}
			return combatRecord;
		}

		// Token: 0x06003EB5 RID: 16053 RVA: 0x00195B84 File Offset: 0x00193D84
		public static DamageSource GetDamageSource(TIFactionState attackerFaction, CombatWeaponCarrierState attacker, TIShipWeaponTemplate weaponTemplate, IDamageable target, Vector3 hitPosition, ArmorFacing armorFacingHit, float distance_km, float finalVelocity_kps)
		{
			Vector3 vector = hitPosition.normalized * -finalVelocity_kps;
			TILaserWeaponTemplate tilaserWeaponTemplate = weaponTemplate as TILaserWeaponTemplate;
			if (tilaserWeaponTemplate != null)
			{
				return new BeamWeapon.Beam(target, distance_km, hitPosition, tilaserWeaponTemplate, attacker);
			}
			TIParticleWeaponTemplate tiparticleWeaponTemplate = weaponTemplate as TIParticleWeaponTemplate;
			if (tiparticleWeaponTemplate != null)
			{
				return new BeamWeapon.Beam(target, distance_km, hitPosition, tiparticleWeaponTemplate, attacker);
			}
			TIPlasmaWeaponTemplate tiplasmaWeaponTemplate = weaponTemplate as TIPlasmaWeaponTemplate;
			if (tiplasmaWeaponTemplate != null)
			{
				return new BallisticProjectileController.ProjectileDamage(vector, hitPosition, attacker, target, tiplasmaWeaponTemplate, attackerFaction, tiplasmaWeaponTemplate.warheadMass_kg);
			}
			TIMagneticGunTemplate timagneticGunTemplate = weaponTemplate as TIMagneticGunTemplate;
			if (timagneticGunTemplate != null)
			{
				return new BallisticProjectileController.ProjectileDamage(vector, hitPosition, attacker, target, timagneticGunTemplate, attackerFaction, timagneticGunTemplate.warheadMass_kg);
			}
			TIGunTemplate tigunTemplate = weaponTemplate as TIGunTemplate;
			if (tigunTemplate != null)
			{
				return new BallisticProjectileController.ProjectileDamage(vector, hitPosition, attacker, target, tigunTemplate, attackerFaction, tigunTemplate.warheadMass_kg);
			}
			TIMissileTemplate timissileTemplate = weaponTemplate as TIMissileTemplate;
			if (timissileTemplate == null)
			{
				return null;
			}
			return new MissileController.MissileDamage(vector, hitPosition, attacker, target, timissileTemplate, attackerFaction, timissileTemplate.warheadMass_kg);
		}

		// Token: 0x06003EB6 RID: 16054 RVA: 0x00195C54 File Offset: 0x00193E54
		public static SimulatedCombat Simulate(TISpaceCombatState combat, float maxDuration_s, Action<SimulatedCombat> Callback = null)
		{
			SimulatedCombat.<>c__DisplayClass36_0 CS$<>8__locals1 = new SimulatedCombat.<>c__DisplayClass36_0();
			CS$<>8__locals1.combat = combat;
			TISpaceFleetState tispaceFleetState = CS$<>8__locals1.combat.fleets[0];
			IEnumerable<TISpaceShipState> enumerable = ((tispaceFleetState != null) ? tispaceFleetState.ships.Where<TISpaceShipState>(new Func<TISpaceShipState, bool>(CS$<>8__locals1.<Simulate>g__IsShipInCombat|0)) : null) ?? Enumerable.Empty<TISpaceShipState>();
			TISpaceFleetState tispaceFleetState2 = CS$<>8__locals1.combat.fleets[1];
			SimulatedCombat simulatedCombat = new SimulatedCombat(enumerable, ((tispaceFleetState2 != null) ? tispaceFleetState2.ships.Where<TISpaceShipState>(new Func<TISpaceShipState, bool>(CS$<>8__locals1.<Simulate>g__IsShipInCombat|0)) : null) ?? Enumerable.Empty<TISpaceShipState>(), maxDuration_s, CS$<>8__locals1.combat.hab);
			CoroutineDummy.Singleton.StartCoroutine(simulatedCombat.Simulate(Callback));
			return simulatedCombat;
		}

		// Token: 0x06003EB7 RID: 16055 RVA: 0x00195CF9 File Offset: 0x00193EF9
		public IEnumerator Simulate(Action<SimulatedCombat> Callback = null)
		{
			SimulatedCombat.<>c__DisplayClass37_0 CS$<>8__locals1 = new SimulatedCombat.<>c__DisplayClass37_0();
			CS$<>8__locals1.<>4__this = this;
			yield return null;
			IEnumerable<SimulatedCombat.SimulatedCombatant> enumerable = this.CombatantsA.Concat<SimulatedCombat.SimulatedCombatant>(this.CombatantsB);
			foreach (TISpaceShipState tispaceShipState in from x in this.ShipsA.Concat<SimulatedCombat.SimulatedShip>(this.ShipsB)
				select x.CopyShip)
			{
				if (tispaceShipState.canEverRetractRadiators)
				{
					tispaceShipState.RetractRadiators();
				}
			}
			CS$<>8__locals1.remainingCombatants = new HashSet<SimulatedCombat.SimulatedCombatant>(enumerable.Where<SimulatedCombat.SimulatedCombatant>((SimulatedCombat.SimulatedCombatant x) => !x.isDestroyed));
			CS$<>8__locals1.remainingCombatantsA = this.CombatantsA.Intersect<SimulatedCombat.SimulatedCombatant>(CS$<>8__locals1.remainingCombatants);
			CS$<>8__locals1.remainingCombatantsB = this.CombatantsB.Intersect<SimulatedCombat.SimulatedCombatant>(CS$<>8__locals1.remainingCombatants);
			CS$<>8__locals1.combatantsToDefensiveWeapons = enumerable.ToDictionary<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>>((SimulatedCombat.SimulatedCombatant x) => x, (SimulatedCombat.SimulatedCombatant x) => new HashSet<SimulatedCombat.SimulatedWeapon>(from x in x.Weapons
				where x.IsDefending
				select x into y
				where y.CanFire()
				select y));
			CS$<>8__locals1.combatantsToOffensiveWeapons = enumerable.ToDictionary<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>>((SimulatedCombat.SimulatedCombatant x) => x, (SimulatedCombat.SimulatedCombatant x) => new HashSet<SimulatedCombat.SimulatedWeapon>(from x in x.Weapons
				where x.IsAttacking
				select x into y
				where y.CanFire()
				select y));
			CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange = 0f;
			CS$<>8__locals1.<Simulate>g__RecalculateTotalOffensiveWeaponFrequency_InRange|7();
			IEnumerable<SimulatedCombat.SimulatedWeapon> enumerable2 = CS$<>8__locals1.combatantsToOffensiveWeapons.Values.SelectMany<HashSet<SimulatedCombat.SimulatedWeapon>, SimulatedCombat.SimulatedWeapon>((HashSet<SimulatedCombat.SimulatedWeapon> x) => x);
			if (enumerable2.Any<SimulatedCombat.SimulatedWeapon>())
			{
				enumerable2.Max<SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedWeapon x) => x.Template.targetingRange_km);
			}
			CS$<>8__locals1.combtantantToOffensiveWeaponsCollection = null;
			CS$<>8__locals1.attackerQueue = new Queue<ValueTuple<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedWeapon>>();
			CS$<>8__locals1.<Simulate>g__UpdateOffensiveWeaponData|10();
			CS$<>8__locals1.damageTaken = enumerable.ToDictionary<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedCombatant, float>((SimulatedCombat.SimulatedCombatant x) => x, (SimulatedCombat.SimulatedCombatant x) => 0f);
			CS$<>8__locals1.projectilesTargetingCombatants = new Dictionary<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedAttack>>();
			CS$<>8__locals1.attacks = new HashSet<SimulatedCombat.SimulatedAttack>();
			SimulatedCombat.SimulatedAttack instantAttack = null;
			GameControl.eventManager.TriggerEvent(new CombatSimulationUpdated(this, 0f), null, Array.Empty<object>());
			yield return null;
			CS$<>8__locals1.lastCombatProgressUpdateFrame = TIFrameCounter.FrameCount;
			int cyclesSinceLastYield = 0;
			int attackUpdatesSinceLastYield = 0;
			int cyclesSinceLastOffenseFrequencyUpdate = 0;
			float lowestDefensiveWeaponFrequency = 0f;
			int cyclesSinceLastDefenseFrequencyUpdate = 0;
			float cyclesBetweenAttackUpdates = 1f;
			int cyclesSinceLastAttackUpdate = 0;
			float lastDamageControlUpdateTime_s = 0f;
			int cycleCount = 0;
			int attackUpdateCount = 0;
			while (CS$<>8__locals1.<Simulate>g__ShouldKeepSimulating|19())
			{
				SimulatedCombat.<>c__DisplayClass37_4 CS$<>8__locals2 = new SimulatedCombat.<>c__DisplayClass37_4();
				while (CoroutineDummy.Singleton.pauseAll)
				{
					yield return null;
				}
				int num = cycleCount;
				cycleCount = num + 1;
				if (instantAttack != null)
				{
					CS$<>8__locals1.<Simulate>g__TryHit|16(instantAttack);
					instantAttack = null;
				}
				num = cyclesSinceLastAttackUpdate + 1;
				cyclesSinceLastAttackUpdate = num;
				if ((float)num >= cyclesBetweenAttackUpdates)
				{
					num = attackUpdateCount;
					attackUpdateCount = num + 1;
					bool flag = false;
					IEnumerable<SimulatedCombat.SimulatedAttack> attacks = CS$<>8__locals1.attacks;
					Func<SimulatedCombat.SimulatedAttack, bool> func;
					if ((func = CS$<>8__locals1.<>9__44) == null)
					{
						func = (CS$<>8__locals1.<>9__44 = (SimulatedCombat.SimulatedAttack x) => !CS$<>8__locals1.remainingCombatants.Contains(x.Target));
					}
					foreach (SimulatedCombat.SimulatedAttack simulatedAttack in attacks.Where<SimulatedCombat.SimulatedAttack>(func).ToList<SimulatedCombat.SimulatedAttack>())
					{
						CS$<>8__locals1.attacks.Remove(simulatedAttack);
						CS$<>8__locals1.projectilesTargetingCombatants[simulatedAttack.Target].Remove(simulatedAttack);
					}
					List<ValueTuple<SimulatedCombat.SimulatedAttack, float>> list = (from x in CS$<>8__locals1.attacks
						select new ValueTuple<SimulatedCombat.SimulatedAttack, float>(x, x.DistanceToTarget_km) into x
						orderby x.Item2
						select x).ToList<ValueTuple<SimulatedCombat.SimulatedAttack, float>>();
					float num2 = 0f;
					float num3 = 0f;
					int num4 = 0;
					int num5 = 0;
					bool flag2 = false;
					bool flag3 = false;
					foreach (SimulatedCombat.SimulatedAttack simulatedAttack2 in from x in list
						select x.Item1 into x
						where x.Weapon.Template.isPointDefenseTargetable
						select x)
					{
						if (flag2 && flag3)
						{
							break;
						}
						if ((!flag2 || !this.CombatantsA.Contains(simulatedAttack2.Target)) && (!flag3 || !this.CombatantsB.Contains(simulatedAttack2.Target)))
						{
							float num6;
							if (!CS$<>8__locals1.<Simulate>g__TryShootDown|15(simulatedAttack2, out num6))
							{
								if (this.CombatantsA.Contains(simulatedAttack2.Target))
								{
									flag2 = true;
								}
								else
								{
									flag3 = true;
								}
							}
							else
							{
								if (this.CombatantsA.Contains(simulatedAttack2.Target))
								{
									num2 += num6;
									num4++;
								}
								else if (this.CombatantsB.Contains(simulatedAttack2.Target))
								{
									num3 += num6;
									num5++;
								}
								CS$<>8__locals1.attacks.Remove(simulatedAttack2);
								CS$<>8__locals1.projectilesTargetingCombatants[simulatedAttack2.Target].Remove(simulatedAttack2);
								flag = true;
							}
						}
					}
					if (flag)
					{
						float num7 = ((num4 > 0) ? (num2 / (float)num4) : 0f);
						float num8 = ((num5 > 0) ? (num3 / (float)num5) : 0f);
						float num9 = CS$<>8__locals1.<Simulate>g__GetAverageCooldown|52(this.CombatantsA);
						float num10 = CS$<>8__locals1.<Simulate>g__GetAverageCooldown|52(this.CombatantsB);
						float num11 = Mathf.Max(num7 / num9, num8 / num10);
						if (num11 > 0.1f)
						{
							if (num11 > 1f)
							{
								cyclesBetweenAttackUpdates = 1f;
							}
							else if (num11 > 0.4f)
							{
								cyclesBetweenAttackUpdates *= 0.7f;
							}
							else
							{
								cyclesBetweenAttackUpdates *= 0.9f;
							}
						}
						else
						{
							cyclesBetweenAttackUpdates *= 1.1f;
						}
					}
					else
					{
						cyclesBetweenAttackUpdates = (cyclesBetweenAttackUpdates + 10f) * 1.4f;
					}
					if (list.Count > 0)
					{
						float num12 = list.Take_Random<ValueTuple<SimulatedCombat.SimulatedAttack, float>>(Mathf.Max(10, list.Count / 20)).Append(list.First<ValueTuple<SimulatedCombat.SimulatedAttack, float>>()).Distinct<ValueTuple<SimulatedCombat.SimulatedAttack, float>>()
							.Min<ValueTuple<SimulatedCombat.SimulatedAttack, float>>(([TupleElementNames(new string[] { "x", "DistanceToTarget_km" })] ValueTuple<SimulatedCombat.SimulatedAttack, float> x) => x.Item1.GetTimeRequiredToTravelGivenDistance_s(x.Item2));
						CS$<>8__locals1.<Simulate>g__RecalculateTotalOffensiveWeaponFrequency_InRange|7();
						cyclesSinceLastOffenseFrequencyUpdate = 0;
						int num13 = Mathf.Max(num12 * CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange, 1f).RoundUp();
						cyclesBetweenAttackUpdates = Mathf.Min(cyclesBetweenAttackUpdates, (float)num13);
					}
					IEnumerable<ValueTuple<SimulatedCombat.SimulatedAttack, float>> enumerable3 = list;
					Func<ValueTuple<SimulatedCombat.SimulatedAttack, float>, bool> func2;
					if ((func2 = CS$<>8__locals1.<>9__47) == null)
					{
						func2 = (CS$<>8__locals1.<>9__47 = ([TupleElementNames(new string[] { "x", "DistanceToTarget_km" })] ValueTuple<SimulatedCombat.SimulatedAttack, float> x) => CS$<>8__locals1.attacks.Contains(x.Item1));
					}
					foreach (SimulatedCombat.SimulatedAttack simulatedAttack3 in (from x in enumerable3.Where<ValueTuple<SimulatedCombat.SimulatedAttack, float>>(func2)
						where x.Item2 <= 0f
						select x.Item1).ToList<SimulatedCombat.SimulatedAttack>())
					{
						if (CS$<>8__locals1.remainingCombatants.Contains(simulatedAttack3.Target))
						{
							CS$<>8__locals1.<Simulate>g__TryHit|16(simulatedAttack3);
						}
						CS$<>8__locals1.attacks.Remove(simulatedAttack3);
						CS$<>8__locals1.projectilesTargetingCombatants[simulatedAttack3.Target].Remove(simulatedAttack3);
					}
					cyclesSinceLastAttackUpdate = 0;
					if (CS$<>8__locals1.<Simulate>g__ShouldYield|17(ref attackUpdatesSinceLastYield, 12))
					{
						yield return null;
					}
				}
				num = cyclesSinceLastOffenseFrequencyUpdate + 1;
				cyclesSinceLastOffenseFrequencyUpdate = num;
				if (num >= 25)
				{
					float totalOffensiveWeaponFrequency_InRange = CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange;
					CS$<>8__locals1.<Simulate>g__RecalculateTotalOffensiveWeaponFrequency_InRange|7();
					cyclesSinceLastOffenseFrequencyUpdate = 0;
					float num14 = CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange / totalOffensiveWeaponFrequency_InRange;
					if (num14 < 1f)
					{
						cyclesBetweenAttackUpdates = (float)(cyclesBetweenAttackUpdates * num14).RoundDown();
					}
				}
				num = cyclesSinceLastDefenseFrequencyUpdate + 1;
				cyclesSinceLastDefenseFrequencyUpdate = num;
				if (num >= 25 || lowestDefensiveWeaponFrequency <= 0f)
				{
					if (CS$<>8__locals1.combatantsToDefensiveWeapons.Any<KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>>>((KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>> x) => x.Value.Count > 0))
					{
						lowestDefensiveWeaponFrequency = CS$<>8__locals1.combatantsToDefensiveWeapons.Where<KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>>>((KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>> x) => x.Value.Count > 0).Min<KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>>>((KeyValuePair<SimulatedCombat.SimulatedCombatant, HashSet<SimulatedCombat.SimulatedWeapon>> x) => x.Value.Min<SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedWeapon y) => y.Frequency));
					}
					cyclesSinceLastDefenseFrequencyUpdate = 0;
				}
				if (CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange <= 0f && lowestDefensiveWeaponFrequency <= 0f)
				{
					this.ElapsedTime_s += 1f;
				}
				else
				{
					if (TIUtilities.RandomFloatValue() >= CS$<>8__locals1.totalOffensiveWeaponFrequency_InRange / lowestDefensiveWeaponFrequency)
					{
						this.ElapsedTime_s += 1f / lowestDefensiveWeaponFrequency;
						continue;
					}
					this.ElapsedTime_s += CS$<>8__locals1.<Simulate>g__GetTimeElapsedPerAttack|13();
				}
				ValueTuple<SimulatedCombat.SimulatedCombatant, SimulatedCombat.SimulatedWeapon> valueTuple = CS$<>8__locals1.<Simulate>g__GetNextAttacker|14();
				SimulatedCombat.SimulatedCombatant item = valueTuple.Item1;
				CS$<>8__locals2.weapon = valueTuple.Item2;
				if (item != null && TIUtilities.RandomFloatValue() <= item.Function)
				{
					List<SimulatedCombat.SimulatedCombatant> list2;
					if (this.CombatantsA.Contains(item))
					{
						list2 = CS$<>8__locals1.remainingCombatantsB.ToList<SimulatedCombat.SimulatedCombatant>();
					}
					else
					{
						list2 = CS$<>8__locals1.remainingCombatantsA.ToList<SimulatedCombat.SimulatedCombatant>();
					}
					list2 = list2.Where<SimulatedCombat.SimulatedCombatant>((SimulatedCombat.SimulatedCombatant x) => CS$<>8__locals2.weapon.IsInRange(x)).ToList<SimulatedCombat.SimulatedCombatant>();
					if (list2.Any<SimulatedCombat.SimulatedCombatant>())
					{
						IEnumerable<SimulatedCombat.SimulatedCombatant> enumerable4 = list2;
						Func<SimulatedCombat.SimulatedCombatant, float> func3;
						if ((func3 = CS$<>8__locals1.<>9__43) == null)
						{
							func3 = (CS$<>8__locals1.<>9__43 = delegate(SimulatedCombat.SimulatedCombatant x)
							{
								HashSet<SimulatedCombat.SimulatedAttack> hashSet2;
								CS$<>8__locals1.projectilesTargetingCombatants.TryGetValue(x, out hashSet2);
								int num16 = ((hashSet2 != null) ? hashSet2.Count : 0);
								float num17 = 0.3f + 1f / (float)(num16 + 1);
								float num18;
								if (x.isShip())
								{
									num18 = (float)x.ref_shipCarrier().hull.structuralIntegrity;
								}
								else
								{
									num18 = 3f;
								}
								float num19 = 0.001f;
								if (x.Weapons.Any<SimulatedCombat.SimulatedWeapon>())
								{
									num19 = Mathf.Max((float)x.Weapons.Count<SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedWeapon y) => y.CanFire()) / (float)x.Weapons.Count<SimulatedCombat.SimulatedWeapon>(), 0.001f);
								}
								return x.Function * num19 * num17 * Mathf.Pow(num18, 0.5f);
							});
						}
						SimulatedCombat.SimulatedCombatant simulatedCombatant = enumerable4.SelectRandomWeightedItem<SimulatedCombat.SimulatedCombatant>(func3, -1f, 1E-37f);
						SimulatedCombat.SimulatedAttack simulatedAttack4 = CS$<>8__locals2.weapon.Fire(simulatedCombatant);
						if (!CS$<>8__locals2.weapon.CanFire())
						{
							CS$<>8__locals1.combatantsToOffensiveWeapons[item].Remove(CS$<>8__locals2.weapon);
							CS$<>8__locals1.<Simulate>g__UpdateOffensiveWeaponData|10();
							if (CS$<>8__locals2.weapon.IsDefending)
							{
								CS$<>8__locals1.combatantsToDefensiveWeapons[item].Remove(CS$<>8__locals2.weapon);
							}
						}
						if (simulatedAttack4 != null)
						{
							if (simulatedAttack4.IsInstant)
							{
								instantAttack = simulatedAttack4;
							}
							else
							{
								CS$<>8__locals1.attacks.Add(simulatedAttack4);
								HashSet<SimulatedCombat.SimulatedAttack> hashSet;
								if (!CS$<>8__locals1.projectilesTargetingCombatants.TryGetValue(simulatedCombatant, out hashSet))
								{
									hashSet = (CS$<>8__locals1.projectilesTargetingCombatants[simulatedCombatant] = new HashSet<SimulatedCombat.SimulatedAttack>());
								}
								hashSet.Add(simulatedAttack4);
							}
							SimulatedCombat.SimulatedShip simulatedShip = simulatedCombatant as SimulatedCombat.SimulatedShip;
							if (simulatedShip != null)
							{
								float num15;
								ArmorFacing struckFacing = simulatedShip.GetStruckFacing(simulatedAttack4.DamageSource, out num15);
								TISpaceCombatState.CurrentActiveCombat.combatLog.AddAttack(new TIFactionState.CombatLog.Attack
								{
									WeaponDataName = CS$<>8__locals2.weapon.Template.dataName,
									Range_km = simulatedAttack4.Damage.range_km,
									ArmorFacing = struckFacing,
									Angle = num15,
									TargetingBonus = item.GetTargetingBonus(CS$<>8__locals2.weapon)
								});
							}
							if (this.ElapsedTime_s - lastDamageControlUpdateTime_s >= 15f)
							{
								foreach (SimulatedCombat.SimulatedCombatant simulatedCombatant2 in CS$<>8__locals1.remainingCombatants)
								{
									SimulatedCombat.SimulatedShip simulatedShip2 = simulatedCombatant2 as SimulatedCombat.SimulatedShip;
									if (simulatedShip2 != null)
									{
										List<SimulatedCombat.SimulatedWeapon> list3 = simulatedShip2.Weapons.Where<SimulatedCombat.SimulatedWeapon>((SimulatedCombat.SimulatedWeapon x) => x.IsDamaged && !x.CanFire()).ToList<SimulatedCombat.SimulatedWeapon>();
										simulatedShip2.CopyShip.DamageControl();
										foreach (SimulatedCombat.SimulatedWeapon simulatedWeapon in list3)
										{
											if (simulatedWeapon.CanFire())
											{
												if (simulatedWeapon.IsDefending)
												{
													CS$<>8__locals1.combatantsToDefensiveWeapons[simulatedCombatant2].Add(simulatedWeapon);
												}
												if (simulatedWeapon.IsAttacking)
												{
													CS$<>8__locals1.combatantsToOffensiveWeapons[simulatedCombatant2].Add(simulatedWeapon);
													CS$<>8__locals1.<Simulate>g__UpdateOffensiveWeaponData|10();
												}
											}
										}
									}
								}
								lastDamageControlUpdateTime_s = this.ElapsedTime_s;
							}
							if (CS$<>8__locals1.<Simulate>g__ShouldYield|17(ref cyclesSinceLastYield, 200))
							{
								yield return null;
							}
							CS$<>8__locals2 = null;
						}
					}
				}
			}
			this.ElapsedTime_s += 300f;
			foreach (SimulatedCombat.SimulatedCombatant simulatedCombatant3 in CS$<>8__locals1.remainingCombatants)
			{
				simulatedCombatant3.SimulatePassageOfCombatTime(this.ElapsedTime_s);
			}
			if (Callback != null)
			{
				Callback(this);
			}
			yield break;
		}

		// Token: 0x06003EB8 RID: 16056 RVA: 0x00195D0F File Offset: 0x00193F0F
		private static float GetDistanceTraveled_m(float startingVelocity_mps, float acceleration_mps2, float time_s)
		{
			return startingVelocity_mps * time_s + 0.5f * acceleration_mps2 * time_s * time_s;
		}

		// Token: 0x06003EB9 RID: 16057 RVA: 0x00195D20 File Offset: 0x00193F20
		private static float GetDistance_m(float startingDistance_m, float startingVelocity_mps, float accelerationA_mps2, float deltaVelocityA_mps, float accelerationB_mps2, float deltaVelocityB_mps, float time_s)
		{
			if (accelerationA_mps2 == 0f)
			{
				deltaVelocityA_mps = float.PositiveInfinity;
			}
			if (accelerationB_mps2 == 0f)
			{
				deltaVelocityB_mps = float.PositiveInfinity;
			}
			float num = 0f;
			float num2;
			if (accelerationA_mps2 == 0f)
			{
				num2 = float.PositiveInfinity;
			}
			else
			{
				num2 = deltaVelocityA_mps / accelerationA_mps2;
			}
			float num3;
			if (accelerationB_mps2 == 0f)
			{
				num3 = float.PositiveInfinity;
			}
			else
			{
				num3 = deltaVelocityB_mps / accelerationB_mps2;
			}
			bool flag = num2 > num3;
			float num4 = accelerationA_mps2 + accelerationB_mps2;
			float num5 = Mathf.Min(new float[] { num2, num3, time_s });
			num += SimulatedCombat.GetDistanceTraveled_m(startingVelocity_mps, num4, num5);
			if (num5 < time_s)
			{
				float num6 = startingVelocity_mps + num5 * num4;
				float num7 = (flag ? accelerationA_mps2 : accelerationB_mps2);
				float num8 = (flag ? (num2 - num3) : (num3 - num2));
				num8 = Mathf.Min(time_s - num5, num8);
				num += SimulatedCombat.GetDistanceTraveled_m(num6, num7, num8);
				if (time_s > num5 + num8)
				{
					float num9 = num6 + num8 * num7;
					float num10 = 0f;
					float num11 = time_s - (num5 + num8);
					num += SimulatedCombat.GetDistanceTraveled_m(num9, num10, num11);
				}
			}
			return Mathf.Max(0f, startingDistance_m - num);
		}

		// Token: 0x06003EBA RID: 16058 RVA: 0x00195E35 File Offset: 0x00194035
		private static float GetDistance_km(float startingDistance_km, float startingVelocity_kps, float accelerationA_mps2, float deltaVelocityA_kps, float accelerationB_mps2, float deltaVelocityB_kps, float time_s)
		{
			return SimulatedCombat.GetDistance_m(startingDistance_km * 1000f, startingVelocity_kps * 1000f, accelerationA_mps2, deltaVelocityA_kps * 1000f, accelerationB_mps2, deltaVelocityB_kps * 1000f, time_s) / 1000f;
		}

		// Token: 0x0400270C RID: 9996
		public HashSet<SimulatedCombat.SimulatedShip> ShipsA = new HashSet<SimulatedCombat.SimulatedShip>();

		// Token: 0x0400270D RID: 9997
		public HashSet<SimulatedCombat.SimulatedShip> ShipsB = new HashSet<SimulatedCombat.SimulatedShip>();

		// Token: 0x0400270E RID: 9998
		public TIHabState Hab;

		// Token: 0x0400270F RID: 9999
		public HashSet<SimulatedCombat.SimulatedCombatHabModule> CombatHabModules = new HashSet<SimulatedCombat.SimulatedCombatHabModule>();

		// Token: 0x04002710 RID: 10000
		public bool HabSupportsA;

		// Token: 0x04002711 RID: 10001
		public List<SimulatedCombat.SimulatedFormation> Formations = new List<SimulatedCombat.SimulatedFormation>();

		// Token: 0x04002712 RID: 10002
		private float MaxDuration_s;

		// Token: 0x04002713 RID: 10003
		public float ElapsedTime_s;

		// Token: 0x02000EDA RID: 3802
		public class SimulatedAttack
		{
			// Token: 0x170011D2 RID: 4562
			// (get) Token: 0x06007A07 RID: 31239 RVA: 0x0031DE09 File Offset: 0x0031C009
			public Damage Damage
			{
				get
				{
					return this.DamageSource.damage;
				}
			}

			// Token: 0x170011D3 RID: 4563
			// (get) Token: 0x06007A08 RID: 31240 RVA: 0x0031DE16 File Offset: 0x0031C016
			public bool IsInstant
			{
				get
				{
					return !this.Weapon.Template.isProjectileWeapon;
				}
			}

			// Token: 0x170011D4 RID: 4564
			// (get) Token: 0x06007A09 RID: 31241 RVA: 0x0031DE2B File Offset: 0x0031C02B
			public float Acceleration_mps2
			{
				get
				{
					if (this.Weapon.Template.isMissileWeapon)
					{
						return this.Weapon.Template.ref_missileWeapon.acceleration_mps2 * 0.85f;
					}
					return 0f;
				}
			}

			// Token: 0x170011D5 RID: 4565
			// (get) Token: 0x06007A0A RID: 31242 RVA: 0x0031DE60 File Offset: 0x0031C060
			public float DeltaVelocity_kps
			{
				get
				{
					if (this.Weapon.Template.isMissileWeapon)
					{
						return this.Weapon.Template.ref_missileWeapon.deltaV_kps * 0.9f;
					}
					return 0f;
				}
			}

			// Token: 0x170011D6 RID: 4566
			// (get) Token: 0x06007A0B RID: 31243 RVA: 0x0031DE95 File Offset: 0x0031C095
			public float DistanceToTarget_km
			{
				get
				{
					return this.GetDistanceFromEnemyCombatant(this.Target);
				}
			}

			// Token: 0x06007A0C RID: 31244 RVA: 0x0031DEA4 File Offset: 0x0031C0A4
			public SimulatedAttack(SimulatedCombat.SimulatedCombatant attacker, SimulatedCombat.SimulatedCombatant target, SimulatedCombat.SimulatedWeapon weapon)
			{
				this.Attacker = attacker;
				this.Target = target;
				this.Weapon = weapon;
				this.SpawnTime = attacker.Combat.ElapsedTime_s;
				this.SpawnDistance_km = attacker.GetDistance_km(target);
				this.SpawnVelocity_kps = attacker.GetClosingVelocity(target) + weapon.MuzzleVelocity_kps;
				this.DamageSource = this.GetDamageSource();
			}

			// Token: 0x06007A0D RID: 31245 RVA: 0x0031DF0C File Offset: 0x0031C10C
			public float GetCurrentVelocity_kps(float currentTime)
			{
				float num = Mathf.Max(currentTime - this.SpawnTime, 0f) * this.Acceleration_mps2 / 1000f;
				return this.SpawnVelocity_kps + Mathf.Min(num, this.DeltaVelocity_kps);
			}

			// Token: 0x06007A0E RID: 31246 RVA: 0x0031DF4C File Offset: 0x0031C14C
			public float GetCurrentVelocity_mps(float currentTime)
			{
				return this.GetCurrentVelocity_kps(currentTime) * 1000f;
			}

			// Token: 0x06007A0F RID: 31247 RVA: 0x0031DF5C File Offset: 0x0031C15C
			public float GetDistanceFromEnemyCombatant(SimulatedCombat.SimulatedCombatant enemyCombatant)
			{
				return SimulatedCombat.GetDistance_km(this.SpawnDistance_km, this.SpawnVelocity_kps, this.Acceleration_mps2, this.DeltaVelocity_kps, enemyCombatant.ExpectedAcceleration_mps2, float.PositiveInfinity, this.Attacker.Combat.ElapsedTime_s - this.SpawnTime);
			}

			// Token: 0x06007A10 RID: 31248 RVA: 0x0031DFA8 File Offset: 0x0031C1A8
			public float GetTimeRequiredToTravelGivenDistance_s(float distance_km)
			{
				float num = distance_km * 1000f;
				float currentVelocity_mps = this.GetCurrentVelocity_mps(this.Attacker.Combat.ElapsedTime_s);
				if (this.Acceleration_mps2 == 0f)
				{
					return num / currentVelocity_mps;
				}
				return (Mathf.Sqrt(2f * this.Acceleration_mps2 * num + Mathf.Pow(currentVelocity_mps, 2f)) - currentVelocity_mps) / this.Acceleration_mps2;
			}

			// Token: 0x06007A11 RID: 31249 RVA: 0x0031E010 File Offset: 0x0031C210
			public float GetChanceToHit()
			{
				if (this.IsInstant)
				{
					return 1f;
				}
				float chanceToEvade = this.Target.GetChanceToEvade(this);
				float ecmvalue = this.Target.GetECMValue();
				float targetingBonus = this.Attacker.GetTargetingBonus(this.Weapon);
				return (1f - chanceToEvade) * (1f - Mathf.Clamp01(ecmvalue - targetingBonus));
			}

			// Token: 0x06007A12 RID: 31250 RVA: 0x0031E06C File Offset: 0x0031C26C
			private ArmorFacing GetArmorFacingHit()
			{
				Dictionary<ArmorFacing, float> dictionary = new Dictionary<ArmorFacing, float>();
				dictionary[ArmorFacing.Nose] = 0.9275f;
				dictionary[ArmorFacing.Tail] = 0.0025f;
				dictionary[ArmorFacing.Left] = 0.035f;
				dictionary[ArmorFacing.Right] = 0.035f;
				return dictionary.SelectRandomWeightedItem<KeyValuePair<ArmorFacing, float>>((KeyValuePair<ArmorFacing, float> x) => x.Value, -1f, 1E-37f).Key;
			}

			// Token: 0x06007A13 RID: 31251 RVA: 0x0031E0E4 File Offset: 0x0031C2E4
			private DamageSource GetDamageSource()
			{
				SimulatedCombat.SimulatedCombatant attacker = this.Attacker;
				TIFactionState tifactionState = ((attacker != null) ? attacker.GetFaction() : null);
				ArmorFacing armorFacingHit = this.GetArmorFacingHit();
				Vector3 surfaceNormal = armorFacingHit.GetSurfaceNormal();
				Vector3 vector = Quaternion.AngleAxis(armorFacingHit.GenerateAngleOfIncidence_deg(), Vector3.up) * surfaceNormal;
				float num = 0f;
				if (this.Attacker != null)
				{
					num = (this.Attacker.ExpectedAcceleration_mps2 + this.Target.ExpectedAcceleration_mps2) * this.Attacker.Combat.ElapsedTime_s * (0.95f + TIUtilities.RandomFloatValue() * 0.1f) / 1000f;
				}
				float num2 = num + this.Weapon.ImpactVelocity_kps;
				return SimulatedCombat.GetDamageSource(tifactionState, this.Attacker, this.Weapon.Template, this.Target, vector, armorFacingHit, this.DistanceToTarget_km, num2);
			}

			// Token: 0x06007A14 RID: 31252 RVA: 0x0031E1AC File Offset: 0x0031C3AC
			public override string ToString()
			{
				string[] array = new string[5];
				int num = 0;
				SimulatedCombat.SimulatedCombatant attacker = this.Attacker;
				array[num] = ((attacker != null) ? attacker.ToString() : null);
				array[1] = "'s attack on ";
				int num2 = 2;
				SimulatedCombat.SimulatedCombatant target = this.Target;
				array[num2] = ((target != null) ? target.ToString() : null);
				array[3] = " with ";
				array[4] = this.Weapon.Template.dataName;
				return string.Concat(array);
			}

			// Token: 0x04005AC6 RID: 23238
			public SimulatedCombat.SimulatedCombatant Attacker;

			// Token: 0x04005AC7 RID: 23239
			public SimulatedCombat.SimulatedCombatant Target;

			// Token: 0x04005AC8 RID: 23240
			public SimulatedCombat.SimulatedWeapon Weapon;

			// Token: 0x04005AC9 RID: 23241
			public DamageSource DamageSource;

			// Token: 0x04005ACA RID: 23242
			public float SpawnTime;

			// Token: 0x04005ACB RID: 23243
			public float SpawnDistance_km;

			// Token: 0x04005ACC RID: 23244
			public float SpawnVelocity_kps;
		}

		// Token: 0x02000EDB RID: 3803
		public abstract class SimulatedWeapon
		{
			// Token: 0x170011D7 RID: 4567
			// (get) Token: 0x06007A15 RID: 31253 RVA: 0x0031E211 File Offset: 0x0031C411
			// (set) Token: 0x06007A16 RID: 31254 RVA: 0x0031E219 File Offset: 0x0031C419
			public SimulatedCombat.SimulatedCombatant Combatant { get; private set; }

			// Token: 0x170011D8 RID: 4568
			// (get) Token: 0x06007A17 RID: 31255 RVA: 0x0031E222 File Offset: 0x0031C422
			// (set) Token: 0x06007A18 RID: 31256 RVA: 0x0031E22A File Offset: 0x0031C42A
			public TIShipWeaponTemplate Template { get; private set; }

			// Token: 0x170011D9 RID: 4569
			// (get) Token: 0x06007A19 RID: 31257 RVA: 0x0031E233 File Offset: 0x0031C433
			// (set) Token: 0x06007A1A RID: 31258 RVA: 0x0031E23B File Offset: 0x0031C43B
			public FireMode FireMode { get; private set; }

			// Token: 0x170011DA RID: 4570
			// (get) Token: 0x06007A1B RID: 31259 RVA: 0x0031E244 File Offset: 0x0031C444
			public bool UsesAmmo
			{
				get
				{
					return this.Template.isProjectileWeapon;
				}
			}

			// Token: 0x170011DB RID: 4571
			// (get) Token: 0x06007A1C RID: 31260
			// (set) Token: 0x06007A1D RID: 31261
			public abstract int Ammo { get; protected set; }

			// Token: 0x170011DC RID: 4572
			// (get) Token: 0x06007A1E RID: 31262
			public abstract bool IsDamaged { get; }

			// Token: 0x170011DD RID: 4573
			// (get) Token: 0x06007A1F RID: 31263 RVA: 0x0031E251 File Offset: 0x0031C451
			public bool IsDefending
			{
				get
				{
					return this.FireMode == FireMode.Defense || this.FireMode == FireMode.Guardian;
				}
			}

			// Token: 0x170011DE RID: 4574
			// (get) Token: 0x06007A20 RID: 31264 RVA: 0x0031E267 File Offset: 0x0031C467
			public bool IsAttacking
			{
				get
				{
					return this.FireMode == FireMode.Offense || this.FireMode == FireMode.Guardian;
				}
			}

			// Token: 0x170011DF RID: 4575
			// (get) Token: 0x06007A21 RID: 31265 RVA: 0x0031E27D File Offset: 0x0031C47D
			// (set) Token: 0x06007A22 RID: 31266 RVA: 0x0031E285 File Offset: 0x0031C485
			public float AverageCooldown { get; protected set; }

			// Token: 0x170011E0 RID: 4576
			// (get) Token: 0x06007A23 RID: 31267 RVA: 0x0031E28E File Offset: 0x0031C48E
			public float Frequency
			{
				get
				{
					return 1f / this.AverageCooldown;
				}
			}

			// Token: 0x170011E1 RID: 4577
			// (get) Token: 0x06007A24 RID: 31268 RVA: 0x0031E29C File Offset: 0x0031C49C
			// (set) Token: 0x06007A25 RID: 31269 RVA: 0x0031E2A4 File Offset: 0x0031C4A4
			public float CooldownMoment { get; private set; }

			// Token: 0x170011E2 RID: 4578
			// (get) Token: 0x06007A26 RID: 31270 RVA: 0x0031E2AD File Offset: 0x0031C4AD
			public bool IsOnCooldown
			{
				get
				{
					return this.CooldownMoment > this.Combatant.Combat.ElapsedTime_s;
				}
			}

			// Token: 0x170011E3 RID: 4579
			// (get) Token: 0x06007A27 RID: 31271 RVA: 0x0031E2C8 File Offset: 0x0031C4C8
			public float MuzzleVelocity_kps
			{
				get
				{
					TIGunTypeWeaponTemplate tigunTypeWeaponTemplate = this.Template as TIGunTypeWeaponTemplate;
					if (tigunTypeWeaponTemplate != null)
					{
						return tigunTypeWeaponTemplate.muzzleVelocity_kps;
					}
					return 0f;
				}
			}

			// Token: 0x170011E4 RID: 4580
			// (get) Token: 0x06007A28 RID: 31272 RVA: 0x0031E2F0 File Offset: 0x0031C4F0
			public float ImpactVelocity_kps
			{
				get
				{
					TIProjectileWeaponTemplate tiprojectileWeaponTemplate = this.Template as TIProjectileWeaponTemplate;
					if (tiprojectileWeaponTemplate != null)
					{
						return tiprojectileWeaponTemplate.EstimatedImpactVelocity_kps;
					}
					return 0f;
				}
			}

			// Token: 0x06007A29 RID: 31273 RVA: 0x0031E318 File Offset: 0x0031C518
			public SimulatedWeapon(SimulatedCombat.SimulatedCombatant combatant, TIShipWeaponTemplate template, FireMode fireMode)
			{
				this.Combatant = combatant;
				this.Template = template;
				this.FireMode = fireMode;
				this.AverageCooldown = this.ComputeAverageCooldown();
			}

			// Token: 0x06007A2A RID: 31274 RVA: 0x0031E341 File Offset: 0x0031C541
			public virtual bool CanFire()
			{
				return true;
			}

			// Token: 0x06007A2B RID: 31275 RVA: 0x0031E344 File Offset: 0x0031C544
			public virtual bool TryFire(SimulatedCombat.SimulatedCombatant target, out SimulatedCombat.SimulatedAttack attack)
			{
				attack = null;
				if (!this.CanFire())
				{
					return false;
				}
				this.CooldownMoment = this.Combatant.Combat.ElapsedTime_s + this.AverageCooldown;
				if (target != null)
				{
					attack = new SimulatedCombat.SimulatedAttack(this.Combatant, target, this);
				}
				return true;
			}

			// Token: 0x06007A2C RID: 31276 RVA: 0x0031E384 File Offset: 0x0031C584
			public SimulatedCombat.SimulatedAttack Fire(SimulatedCombat.SimulatedCombatant target = null)
			{
				SimulatedCombat.SimulatedAttack simulatedAttack;
				if (this.TryFire(target, out simulatedAttack))
				{
					return simulatedAttack;
				}
				return null;
			}

			// Token: 0x06007A2D RID: 31277 RVA: 0x0031E39F File Offset: 0x0031C59F
			protected virtual float ComputeAverageCooldown()
			{
				return this.Template.averageCooldown_s;
			}

			// Token: 0x06007A2E RID: 31278 RVA: 0x0031E3AC File Offset: 0x0031C5AC
			public bool IsInRange(SimulatedCombat.SimulatedCombatant enemyCombatant)
			{
				return this.Combatant.GetDistance_km(enemyCombatant) <= this.Template.targetingRange_km;
			}
		}

		// Token: 0x02000EDC RID: 3804
		public class SimulatedHabWeapon : SimulatedCombat.SimulatedWeapon
		{
			// Token: 0x170011E5 RID: 4581
			// (get) Token: 0x06007A2F RID: 31279 RVA: 0x0031E3CA File Offset: 0x0031C5CA
			// (set) Token: 0x06007A30 RID: 31280 RVA: 0x0031E3CD File Offset: 0x0031C5CD
			public override int Ammo
			{
				get
				{
					return 0;
				}
				protected set
				{
				}
			}

			// Token: 0x170011E6 RID: 4582
			// (get) Token: 0x06007A31 RID: 31281 RVA: 0x0031E3CF File Offset: 0x0031C5CF
			public override bool IsDamaged
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06007A32 RID: 31282 RVA: 0x0031E3D2 File Offset: 0x0031C5D2
			public SimulatedHabWeapon(SimulatedCombat.SimulatedCombatant combatant, TIShipWeaponTemplate template, FireMode fireMode)
				: base(combatant, template, fireMode)
			{
			}
		}

		// Token: 0x02000EDD RID: 3805
		public class SimulatedShipWeapon : SimulatedCombat.SimulatedWeapon
		{
			// Token: 0x170011E7 RID: 4583
			// (get) Token: 0x06007A33 RID: 31283 RVA: 0x0031E3DD File Offset: 0x0031C5DD
			public SimulatedCombat.SimulatedShip Ship
			{
				get
				{
					return base.Combatant as SimulatedCombat.SimulatedShip;
				}
			}

			// Token: 0x170011E8 RID: 4584
			// (get) Token: 0x06007A34 RID: 31284 RVA: 0x0031E3EA File Offset: 0x0031C5EA
			// (set) Token: 0x06007A35 RID: 31285 RVA: 0x0031E3F2 File Offset: 0x0031C5F2
			public ModuleDataEntry Module { get; private set; }

			// Token: 0x170011E9 RID: 4585
			// (get) Token: 0x06007A36 RID: 31286 RVA: 0x0031E3FB File Offset: 0x0031C5FB
			// (set) Token: 0x06007A37 RID: 31287 RVA: 0x0031E422 File Offset: 0x0031C622
			public override int Ammo
			{
				get
				{
					if (!base.UsesAmmo)
					{
						return 0;
					}
					return this.Ship.CopyShip.ammo[this.Module];
				}
				protected set
				{
					if (!base.UsesAmmo)
					{
						return;
					}
					this.Ship.CopyShip.ammo[this.Module] = value;
				}
			}

			// Token: 0x170011EA RID: 4586
			// (get) Token: 0x06007A38 RID: 31288 RVA: 0x0031E449 File Offset: 0x0031C649
			public override bool IsDamaged
			{
				get
				{
					return this.Ship.CopyShip.WeaponDamaged(this.Module);
				}
			}

			// Token: 0x06007A39 RID: 31289 RVA: 0x0031E461 File Offset: 0x0031C661
			public SimulatedShipWeapon(SimulatedCombat.SimulatedShip ship, ModuleDataEntry module, FireMode fireMode)
				: base(ship, module.weaponTemplate, fireMode)
			{
				this.Module = module;
			}

			// Token: 0x06007A3A RID: 31290 RVA: 0x0031E478 File Offset: 0x0031C678
			public override bool CanFire()
			{
				return base.CanFire() && (!base.UsesAmmo || this.Ammo > 0) && !this.IsDamaged;
			}

			// Token: 0x06007A3B RID: 31291 RVA: 0x0031E4A0 File Offset: 0x0031C6A0
			public override bool TryFire(SimulatedCombat.SimulatedCombatant target, out SimulatedCombat.SimulatedAttack attack)
			{
				if (!base.TryFire(target, out attack))
				{
					return false;
				}
				int num;
				if (this.Ship.CopyShip.ammo.TryGetValue(this.Module, out num) && num > 0)
				{
					this.Ship.CopyShip.ammo[this.Module] = num - 1;
				}
				return true;
			}

			// Token: 0x06007A3C RID: 31292 RVA: 0x0031E4FC File Offset: 0x0031C6FC
			protected override float ComputeAverageCooldown()
			{
				float num;
				float num2;
				float num3;
				Weapon.GetAdjustedCooldownValues(base.Combatant, base.Template, out num, out num2, out num3);
				if (base.IsDefending)
				{
					num = num2;
				}
				int num4 = Mathf.Max(base.Template.salvo_shots, 1);
				return (num + num3 * (float)(num4 - 1)) / (float)num4;
			}
		}

		// Token: 0x02000EDE RID: 3806
		public class SimulatedFormation
		{
			// Token: 0x170011EB RID: 4587
			// (get) Token: 0x06007A3D RID: 31293 RVA: 0x0031E547 File Offset: 0x0031C747
			// (set) Token: 0x06007A3E RID: 31294 RVA: 0x0031E54F File Offset: 0x0031C74F
			public SimulatedCombat Combat { get; private set; }

			// Token: 0x170011EC RID: 4588
			// (get) Token: 0x06007A3F RID: 31295 RVA: 0x0031E558 File Offset: 0x0031C758
			public float Acceleration
			{
				get
				{
					return this.Combatants.Average<SimulatedCombat.SimulatedCombatant>((SimulatedCombat.SimulatedCombatant x) => x.ExpectedAcceleration_mps2);
				}
			}

			// Token: 0x06007A40 RID: 31296 RVA: 0x0031E584 File Offset: 0x0031C784
			public SimulatedFormation(SimulatedCombat combat, IEnumerable<SimulatedCombat.SimulatedCombatant> combatants)
			{
				this.Combat = combat;
				this.Combatants = new HashSet<SimulatedCombat.SimulatedCombatant>(combatants);
			}

			// Token: 0x06007A41 RID: 31297 RVA: 0x0031E5B8 File Offset: 0x0031C7B8
			public void AddStartingDistance(SimulatedCombat.SimulatedFormation otherFormation, float startingDistance)
			{
				Dictionary<SimulatedCombat.SimulatedFormation, float> dictionary = this.formationStartingDistances;
				otherFormation.formationStartingDistances[this] = startingDistance;
				dictionary[otherFormation] = startingDistance;
			}

			// Token: 0x06007A42 RID: 31298 RVA: 0x0031E5E4 File Offset: 0x0031C7E4
			public float GetDistance_km(SimulatedCombat.SimulatedFormation otherFormation)
			{
				if (otherFormation == this)
				{
					return 0f;
				}
				if (this.Combat.ElapsedTime_s >= this.clearCacheMoment)
				{
					this.cachedFormationDistances.Clear();
					this.clearCacheMoment += 10f;
				}
				float num;
				if (!this.cachedFormationDistances.TryGetValue(otherFormation, out num))
				{
					float num2;
					if (!this.formationStartingDistances.TryGetValue(otherFormation, out num2))
					{
						throw new Exception("SimulatedCombat : Tried to find distance to formation that does not have a starting distance entry");
					}
					num = (this.cachedFormationDistances[otherFormation] = Mathf.Max(100f, SimulatedCombat.GetDistance_km(num2, 0.2f, this.Acceleration, float.PositiveInfinity, otherFormation.Acceleration, float.PositiveInfinity, this.Combat.ElapsedTime_s)));
				}
				return num;
			}

			// Token: 0x06007A43 RID: 31299 RVA: 0x0031E69C File Offset: 0x0031C89C
			public bool IsAlly(SimulatedCombat.SimulatedFormation otherFormation)
			{
				if (this.Combatants.Count == 0 || otherFormation.Combatants.Count == 0)
				{
					return false;
				}
				SimulatedCombat.SimulatedCombatant simulatedCombatant = this.Combatants.First<SimulatedCombat.SimulatedCombatant>();
				SimulatedCombat.SimulatedCombatant simulatedCombatant2 = otherFormation.Combatants.First<SimulatedCombat.SimulatedCombatant>();
				return simulatedCombatant.IsAlly(simulatedCombatant2);
			}

			// Token: 0x06007A44 RID: 31300 RVA: 0x0031E6E2 File Offset: 0x0031C8E2
			public bool IsEnemy(SimulatedCombat.SimulatedFormation otherFormation)
			{
				return !this.IsAlly(otherFormation);
			}

			// Token: 0x04005AD4 RID: 23252
			public HashSet<SimulatedCombat.SimulatedCombatant> Combatants;

			// Token: 0x04005AD5 RID: 23253
			private Dictionary<SimulatedCombat.SimulatedFormation, float> formationStartingDistances = new Dictionary<SimulatedCombat.SimulatedFormation, float>();

			// Token: 0x04005AD6 RID: 23254
			private Dictionary<SimulatedCombat.SimulatedFormation, float> cachedFormationDistances = new Dictionary<SimulatedCombat.SimulatedFormation, float>();

			// Token: 0x04005AD7 RID: 23255
			private float clearCacheMoment;

			// Token: 0x04005AD8 RID: 23256
			public const float AssumedStartingRelativeVelocity_kps = 0.2f;
		}

		// Token: 0x02000EDF RID: 3807
		public abstract class SimulatedCombatant : IDamageable, CombatWeaponCarrierState
		{
			// Token: 0x170011ED RID: 4589
			// (get) Token: 0x06007A45 RID: 31301 RVA: 0x0031E6EE File Offset: 0x0031C8EE
			// (set) Token: 0x06007A46 RID: 31302 RVA: 0x0031E6F6 File Offset: 0x0031C8F6
			public SimulatedCombat Combat { get; private set; }

			// Token: 0x170011EE RID: 4590
			// (get) Token: 0x06007A47 RID: 31303
			public abstract TIGameState OriginalGameState { get; }

			// Token: 0x06007A48 RID: 31304
			public abstract TIFactionState GetFaction();

			// Token: 0x170011EF RID: 4591
			// (get) Token: 0x06007A49 RID: 31305 RVA: 0x0031E6FF File Offset: 0x0031C8FF
			public SimulatedCombat.SimulatedFormation Formation
			{
				get
				{
					if (this.formation == null)
					{
						this.formation = this.Combat.Formations.FirstOrDefault<SimulatedCombat.SimulatedFormation>((SimulatedCombat.SimulatedFormation x) => x.Combatants.Contains(this));
					}
					return this.formation;
				}
			}

			// Token: 0x170011F0 RID: 4592
			// (get) Token: 0x06007A4A RID: 31306 RVA: 0x0031E734 File Offset: 0x0031C934
			public TIHabState AlliedHab
			{
				get
				{
					if (!this.alliedHabWasSet)
					{
						this.alliedHab = ((this.Combat.CombatantsA.Contains(this) == this.Combat.HabSupportsA) ? this.Combat.Hab : null);
						this.alliedHabWasSet = true;
					}
					return this.alliedHab;
				}
			}

			// Token: 0x170011F1 RID: 4593
			// (get) Token: 0x06007A4B RID: 31307
			public abstract float Acceleration_mps2 { get; }

			// Token: 0x170011F2 RID: 4594
			// (get) Token: 0x06007A4C RID: 31308 RVA: 0x0031E788 File Offset: 0x0031C988
			public float ExpectedAcceleration_mps2
			{
				get
				{
					return this.Acceleration_mps2 * 0.2f;
				}
			}

			// Token: 0x170011F3 RID: 4595
			// (get) Token: 0x06007A4D RID: 31309 RVA: 0x0031E796 File Offset: 0x0031C996
			public bool IsMobile
			{
				get
				{
					return this.Acceleration_mps2 > 0f;
				}
			}

			// Token: 0x170011F4 RID: 4596
			// (get) Token: 0x06007A4E RID: 31310
			public abstract float Function { get; }

			// Token: 0x170011F5 RID: 4597
			// (get) Token: 0x06007A4F RID: 31311
			public abstract bool isDestroyed { get; }

			// Token: 0x170011F6 RID: 4598
			// (get) Token: 0x06007A50 RID: 31312 RVA: 0x0031E7A5 File Offset: 0x0031C9A5
			// (set) Token: 0x06007A51 RID: 31313 RVA: 0x0031E7AD File Offset: 0x0031C9AD
			public SimulatedCombat.SimulatedWeapon DestroyerWeapon { get; set; }

			// Token: 0x170011F7 RID: 4599
			// (get) Token: 0x06007A52 RID: 31314 RVA: 0x0031E7B6 File Offset: 0x0031C9B6
			public SimulatedCombat.SimulatedCombatant Destroyer
			{
				get
				{
					SimulatedCombat.SimulatedWeapon destroyerWeapon = this.DestroyerWeapon;
					if (destroyerWeapon == null)
					{
						return null;
					}
					return destroyerWeapon.Combatant;
				}
			}

			// Token: 0x170011F8 RID: 4600
			// (get) Token: 0x06007A53 RID: 31315
			public abstract IEnumerable<SimulatedCombat.SimulatedWeapon> Weapons { get; }

			// Token: 0x170011F9 RID: 4601
			// (get) Token: 0x06007A54 RID: 31316
			public abstract float ExpectedCombatRange_km { get; }

			// Token: 0x06007A55 RID: 31317 RVA: 0x0031E7C9 File Offset: 0x0031C9C9
			public SimulatedCombatant(SimulatedCombat combat)
			{
				this.Combat = combat;
			}

			// Token: 0x06007A56 RID: 31318 RVA: 0x0031E7D8 File Offset: 0x0031C9D8
			public SimulatedCombatant()
			{
			}

			// Token: 0x06007A57 RID: 31319 RVA: 0x0031E7E0 File Offset: 0x0031C9E0
			public bool IsAlly(SimulatedCombat.SimulatedCombatant otherCombatant)
			{
				return this.Combat.CombatantsA.Contains(this) == this.Combat.CombatantsA.Contains(otherCombatant);
			}

			// Token: 0x06007A58 RID: 31320 RVA: 0x0031E806 File Offset: 0x0031CA06
			public bool IsEnemy(SimulatedCombat.SimulatedCombatant otherCombatant)
			{
				return !this.IsAlly(otherCombatant);
			}

			// Token: 0x06007A59 RID: 31321
			public abstract float GetChanceToEvade(SimulatedCombat.SimulatedAttack attack);

			// Token: 0x06007A5A RID: 31322
			public abstract float GetECMValue();

			// Token: 0x06007A5B RID: 31323 RVA: 0x0031E812 File Offset: 0x0031CA12
			public float GetTargetingBonus(SimulatedCombat.SimulatedWeapon weapon)
			{
				return this.TargetingBonus(weapon.Template, this.AlliedHab);
			}

			// Token: 0x06007A5C RID: 31324 RVA: 0x0031E826 File Offset: 0x0031CA26
			public float GetDistance_km(SimulatedCombat.SimulatedCombatant otherCombatant)
			{
				return this.Formation.GetDistance_km(otherCombatant.Formation);
			}

			// Token: 0x06007A5D RID: 31325 RVA: 0x0031E839 File Offset: 0x0031CA39
			public float GetClosingVelocity(SimulatedCombat.SimulatedCombatant enemyCombatant)
			{
				return 0.2f + (this.ExpectedAcceleration_mps2 + enemyCombatant.ExpectedAcceleration_mps2) * this.Combat.ElapsedTime_s / 1000f;
			}

			// Token: 0x06007A5E RID: 31326
			public abstract float ApplyDamage(DamageSource source);

			// Token: 0x06007A5F RID: 31327
			public abstract float GetCrossSectionalArea_m2(float angle = 3.4028235E+38f);

			// Token: 0x06007A60 RID: 31328 RVA: 0x0031E860 File Offset: 0x0031CA60
			public virtual void SimulatePassageOfCombatTime(float time_s)
			{
			}

			// Token: 0x170011FA RID: 4602
			// (get) Token: 0x06007A61 RID: 31329 RVA: 0x0031E862 File Offset: 0x0031CA62
			public Vector3 position
			{
				get
				{
					return Vector3.zero;
				}
			}

			// Token: 0x170011FB RID: 4603
			// (get) Token: 0x06007A62 RID: 31330 RVA: 0x0031E869 File Offset: 0x0031CA69
			public Vector3 velocityVector
			{
				get
				{
					return Vector3.zero;
				}
			}

			// Token: 0x170011FC RID: 4604
			// (get) Token: 0x06007A63 RID: 31331 RVA: 0x0031E870 File Offset: 0x0031CA70
			public Vector3 velocityVector_kps
			{
				get
				{
					return Vector3.zero;
				}
			}

			// Token: 0x170011FD RID: 4605
			// (get) Token: 0x06007A64 RID: 31332 RVA: 0x0031E877 File Offset: 0x0031CA77
			public Vector3 accelerationVector
			{
				get
				{
					return Vector3.zero;
				}
			}

			// Token: 0x170011FE RID: 4606
			// (get) Token: 0x06007A65 RID: 31333 RVA: 0x0031E87E File Offset: 0x0031CA7E
			public Vector3 accelerationVector_kps
			{
				get
				{
					return Vector3.zero;
				}
			}

			// Token: 0x170011FF RID: 4607
			// (get) Token: 0x06007A66 RID: 31334
			public abstract IDamageableType damageableType { get; }

			// Token: 0x17001200 RID: 4608
			// (get) Token: 0x06007A67 RID: 31335 RVA: 0x0031E885 File Offset: 0x0031CA85
			public List<Collider> hitColliders
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17001201 RID: 4609
			// (get) Token: 0x06007A68 RID: 31336 RVA: 0x0031E88C File Offset: 0x0031CA8C
			public CombatTargetableState combatTargetableState
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17001202 RID: 4610
			// (get) Token: 0x06007A69 RID: 31337 RVA: 0x0031E893 File Offset: 0x0031CA93
			public TISpaceCombatProjectileState ref_projectile
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17001203 RID: 4611
			// (get) Token: 0x06007A6A RID: 31338 RVA: 0x0031E89A File Offset: 0x0031CA9A
			public Transform damageableTransform
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x17001204 RID: 4612
			// (get) Token: 0x06007A6B RID: 31339 RVA: 0x0031E8A1 File Offset: 0x0031CAA1
			public Transform transform
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			// Token: 0x06007A6C RID: 31340 RVA: 0x0031E8A8 File Offset: 0x0031CAA8
			public Vector3 positionAtTime(DateTime timeToProject)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06007A6D RID: 31341 RVA: 0x0031E8BA File Offset: 0x0031CABA
			public bool isShip()
			{
				return this.ref_shipCarrier() != null;
			}

			// Token: 0x06007A6E RID: 31342 RVA: 0x0031E8C8 File Offset: 0x0031CAC8
			public bool isHabModule()
			{
				return this is SimulatedCombat.SimulatedCombatHabModule;
			}

			// Token: 0x06007A6F RID: 31343
			public abstract TISpaceShipState ref_shipCarrier();

			// Token: 0x06007A70 RID: 31344
			public abstract TIHabModuleState ref_habModuleCarrier();

			// Token: 0x06007A71 RID: 31345
			public abstract bool WeaponIsOperable(ModuleDataEntry module);

			// Token: 0x06007A72 RID: 31346 RVA: 0x0031E8D3 File Offset: 0x0031CAD3
			public virtual bool WeaponCanFire(ModuleDataEntry module)
			{
				return this.WeaponIsOperable(module);
			}

			// Token: 0x06007A73 RID: 31347
			public abstract float FireControlFunction();

			// Token: 0x06007A74 RID: 31348
			public abstract float TargetingBonus(TIShipWeaponTemplate weapon, TIHabState alliedHab);

			// Token: 0x06007A75 RID: 31349 RVA: 0x0031E8DC File Offset: 0x0031CADC
			public TIGameState GetTargetableState()
			{
				throw new NotImplementedException();
			}

			// Token: 0x06007A76 RID: 31350 RVA: 0x0031E8E3 File Offset: 0x0031CAE3
			public void FireWeapon(ModuleDataEntry module, TISpaceCombatProjectileState targetedProjectile = null)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06007A77 RID: 31351 RVA: 0x0031E8EA File Offset: 0x0031CAEA
			public void AddTargetedProjectile(TISpaceCombatProjectileState projectile)
			{
				throw new NotImplementedException();
			}

			// Token: 0x06007A78 RID: 31352 RVA: 0x0031E8F1 File Offset: 0x0031CAF1
			public override string ToString()
			{
				return this.GetFaction().displayName + "." + this.OriginalGameState.displayName + (this.isDestroyed ? "(Dead)" : "");
			}

			// Token: 0x04005ADA RID: 23258
			private SimulatedCombat.SimulatedFormation formation;

			// Token: 0x04005ADB RID: 23259
			private TIHabState alliedHab;

			// Token: 0x04005ADC RID: 23260
			private bool alliedHabWasSet;
		}

		// Token: 0x02000EE0 RID: 3808
		public class SimulatedCombatHabModule : SimulatedCombat.SimulatedCombatant
		{
			// Token: 0x17001205 RID: 4613
			// (get) Token: 0x06007A7A RID: 31354 RVA: 0x0031E935 File Offset: 0x0031CB35
			// (set) Token: 0x06007A7B RID: 31355 RVA: 0x0031E93D File Offset: 0x0031CB3D
			public TIHabModuleState Module { get; private set; }

			// Token: 0x17001206 RID: 4614
			// (get) Token: 0x06007A7C RID: 31356 RVA: 0x0031E946 File Offset: 0x0031CB46
			public override TIGameState OriginalGameState
			{
				get
				{
					return this.Module;
				}
			}

			// Token: 0x06007A7D RID: 31357 RVA: 0x0031E94E File Offset: 0x0031CB4E
			public override TISpaceShipState ref_shipCarrier()
			{
				return null;
			}

			// Token: 0x06007A7E RID: 31358 RVA: 0x0031E951 File Offset: 0x0031CB51
			public override TIHabModuleState ref_habModuleCarrier()
			{
				return this.Module;
			}

			// Token: 0x17001207 RID: 4615
			// (get) Token: 0x06007A7F RID: 31359 RVA: 0x0031E959 File Offset: 0x0031CB59
			public override IDamageableType damageableType
			{
				get
				{
					return IDamageableType.StationModule;
				}
			}

			// Token: 0x06007A80 RID: 31360 RVA: 0x0031E95C File Offset: 0x0031CB5C
			public SimulatedCombatHabModule(SimulatedCombat combat, TIHabModuleState combatHabModule)
				: base(combat)
			{
				this.Module = combatHabModule;
				this.faction = this.Module.ref_faction;
				this.moduleTemplate = this.Module.moduleTemplate;
				this.armorTemplate = this.Module.armorTemplate;
				this.remainingHitPoints = (this.hitPoints = (float)this.Module.moduleTemplate.BaseStationModuleHitPoints(this.Module.ref_faction, this.Module.hab));
				this.remainingArmor = this.Module.StationModuleArmorPoints;
				this.irradiatedMultiplier = this.Module.hab.irradiatedMultiplier;
				this.weapons = new List<SimulatedCombat.SimulatedHabWeapon>
				{
					new SimulatedCombat.SimulatedHabWeapon(this, this.Module.PointDefenseWeaponTemplate, FireMode.Defense),
					new SimulatedCombat.SimulatedHabWeapon(this, this.Module.defenseWeaponTemplate, FireMode.Offense),
					new SimulatedCombat.SimulatedHabWeapon(this, this.Module.defenseWeaponTemplate_gun, FireMode.Offense)
				};
				if (this.Module.defenseWeaponTemplate_plasma != null)
				{
					this.weapons.Add(new SimulatedCombat.SimulatedHabWeapon(this, this.Module.defenseWeaponTemplate_plasma, FireMode.Offense));
				}
			}

			// Token: 0x06007A81 RID: 31361 RVA: 0x0031EA84 File Offset: 0x0031CC84
			public SimulatedCombatHabModule(TIFactionState faction, TIHabModuleTemplate template, TIShipArmorTemplate armor, float hitPoints, float armorPoints, float irradiatedMultiplier, IEnumerable<TIShipWeaponTemplate> weaponTemplates)
			{
				this.faction = faction;
				this.moduleTemplate = template;
				this.armorTemplate = armor;
				this.hitPoints = hitPoints;
				this.remainingHitPoints = hitPoints;
				this.remainingArmor = armorPoints;
				this.irradiatedMultiplier = irradiatedMultiplier;
				this.weapons = weaponTemplates.Select<TIShipWeaponTemplate, SimulatedCombat.SimulatedHabWeapon>((TIShipWeaponTemplate x) => new SimulatedCombat.SimulatedHabWeapon(this, x, x.isLaserWeapon ? FireMode.Defense : FireMode.Offense)).ToList<SimulatedCombat.SimulatedHabWeapon>();
			}

			// Token: 0x17001208 RID: 4616
			// (get) Token: 0x06007A82 RID: 31362 RVA: 0x0031EAEB File Offset: 0x0031CCEB
			public override float Acceleration_mps2
			{
				get
				{
					return 0f;
				}
			}

			// Token: 0x17001209 RID: 4617
			// (get) Token: 0x06007A83 RID: 31363 RVA: 0x0031EAF2 File Offset: 0x0031CCF2
			public override float Function
			{
				get
				{
					return 1f;
				}
			}

			// Token: 0x1700120A RID: 4618
			// (get) Token: 0x06007A84 RID: 31364 RVA: 0x0031EAF9 File Offset: 0x0031CCF9
			public override bool isDestroyed
			{
				get
				{
					return this.remainingHitPoints <= 0f;
				}
			}

			// Token: 0x1700120B RID: 4619
			// (get) Token: 0x06007A85 RID: 31365 RVA: 0x0031EB0B File Offset: 0x0031CD0B
			public override IEnumerable<SimulatedCombat.SimulatedWeapon> Weapons
			{
				get
				{
					return this.weapons;
				}
			}

			// Token: 0x1700120C RID: 4620
			// (get) Token: 0x06007A86 RID: 31366 RVA: 0x0031EB13 File Offset: 0x0031CD13
			public override float ExpectedCombatRange_km
			{
				get
				{
					return 800f;
				}
			}

			// Token: 0x06007A87 RID: 31367 RVA: 0x0031EB1C File Offset: 0x0031CD1C
			public override float ApplyDamage(DamageSource source)
			{
				float num;
				return CombatHabModuleController.ApplyDamage(this.moduleTemplate, source, this.armorTemplate, this.hitPoints, this.irradiatedMultiplier, ref this.remainingArmor, ref this.remainingHitPoints, out num, null);
			}

			// Token: 0x06007A88 RID: 31368 RVA: 0x0031EB56 File Offset: 0x0031CD56
			public override float GetCrossSectionalArea_m2(float angle = 3.4028235E+38f)
			{
				return this.moduleTemplate.GetCrossSectionalArea_m2(angle);
			}

			// Token: 0x06007A89 RID: 31369 RVA: 0x0031EB64 File Offset: 0x0031CD64
			public override float GetChanceToEvade(SimulatedCombat.SimulatedAttack attack)
			{
				return 0f;
			}

			// Token: 0x06007A8A RID: 31370 RVA: 0x0031EB6B File Offset: 0x0031CD6B
			public override float GetECMValue()
			{
				return this.moduleTemplate.ECMValue(this.GetFaction(), base.AlliedHab);
			}

			// Token: 0x06007A8B RID: 31371 RVA: 0x0031EB84 File Offset: 0x0031CD84
			public override float TargetingBonus(TIShipWeaponTemplate weaponTemplate, TIHabState alliedHab)
			{
				return this.moduleTemplate.TargetingBonus(this.GetFaction(), alliedHab);
			}

			// Token: 0x06007A8C RID: 31372 RVA: 0x0031EB98 File Offset: 0x0031CD98
			public override bool WeaponIsOperable(ModuleDataEntry module)
			{
				return true;
			}

			// Token: 0x06007A8D RID: 31373 RVA: 0x0031EB9B File Offset: 0x0031CD9B
			public override float FireControlFunction()
			{
				return 1f;
			}

			// Token: 0x06007A8E RID: 31374 RVA: 0x0031EBA2 File Offset: 0x0031CDA2
			public override TIFactionState GetFaction()
			{
				return this.faction;
			}

			// Token: 0x06007A8F RID: 31375 RVA: 0x0031EBAA File Offset: 0x0031CDAA
			public override string ToString()
			{
				return "Combat Hab Module";
			}

			// Token: 0x04005ADE RID: 23262
			private List<SimulatedCombat.SimulatedHabWeapon> weapons;

			// Token: 0x04005ADF RID: 23263
			private TIFactionState faction;

			// Token: 0x04005AE0 RID: 23264
			private TIHabModuleTemplate moduleTemplate;

			// Token: 0x04005AE1 RID: 23265
			private TIShipArmorTemplate armorTemplate;

			// Token: 0x04005AE2 RID: 23266
			private float hitPoints;

			// Token: 0x04005AE3 RID: 23267
			private float remainingHitPoints;

			// Token: 0x04005AE4 RID: 23268
			private float remainingArmor;

			// Token: 0x04005AE5 RID: 23269
			private float irradiatedMultiplier;
		}

		// Token: 0x02000EE1 RID: 3809
		public class SimulatedShip : SimulatedCombat.SimulatedCombatant
		{
			// Token: 0x1700120D RID: 4621
			// (get) Token: 0x06007A91 RID: 31377 RVA: 0x0031EBC6 File Offset: 0x0031CDC6
			public override TIGameState OriginalGameState
			{
				get
				{
					return this.OriginalShip;
				}
			}

			// Token: 0x06007A92 RID: 31378 RVA: 0x0031EBCE File Offset: 0x0031CDCE
			public override TISpaceShipState ref_shipCarrier()
			{
				return this.CopyShip;
			}

			// Token: 0x06007A93 RID: 31379 RVA: 0x0031EBD6 File Offset: 0x0031CDD6
			public override TIHabModuleState ref_habModuleCarrier()
			{
				return null;
			}

			// Token: 0x1700120E RID: 4622
			// (get) Token: 0x06007A94 RID: 31380 RVA: 0x0031EBD9 File Offset: 0x0031CDD9
			public override IDamageableType damageableType
			{
				get
				{
					return IDamageableType.Ship;
				}
			}

			// Token: 0x1700120F RID: 4623
			// (get) Token: 0x06007A95 RID: 31381 RVA: 0x0031EBDC File Offset: 0x0031CDDC
			public override float Acceleration_mps2
			{
				get
				{
					return this.CopyShip.combatAcceleration_mps2;
				}
			}

			// Token: 0x17001210 RID: 4624
			// (get) Token: 0x06007A96 RID: 31382 RVA: 0x0031EBE9 File Offset: 0x0031CDE9
			public override float Function
			{
				get
				{
					if (!this.CopyShip.PartDestroyed(this.CopyShip.radiatorModule))
					{
						return 1f;
					}
					return 0.33f;
				}
			}

			// Token: 0x17001211 RID: 4625
			// (get) Token: 0x06007A97 RID: 31383 RVA: 0x0031EC0E File Offset: 0x0031CE0E
			public override bool isDestroyed
			{
				get
				{
					return this.CopyShip.ShipDestroyed();
				}
			}

			// Token: 0x17001212 RID: 4626
			// (get) Token: 0x06007A98 RID: 31384 RVA: 0x0031EC1B File Offset: 0x0031CE1B
			public IEnumerable<TIOfficerState> DeadSimulatedOfficers
			{
				get
				{
					if (this.isDestroyed)
					{
						return this.SimulatedOfficersToRealOfficers.Keys;
					}
					return this.SimulatedOfficersToRealOfficers.Keys.Where<TIOfficerState>((TIOfficerState x) => !this.CopyShip.officers.Contains(x));
				}
			}

			// Token: 0x17001213 RID: 4627
			// (get) Token: 0x06007A99 RID: 31385 RVA: 0x0031EC50 File Offset: 0x0031CE50
			public override IEnumerable<SimulatedCombat.SimulatedWeapon> Weapons
			{
				get
				{
					if (this.weapons == null)
					{
						List<ModuleDataEntry> list = this.CopyShip.AllWeaponModuleData();
						List<ModuleDataEntry> list2 = list.Where<ModuleDataEntry>((ModuleDataEntry x) => x.weaponTemplate.attackMode).ToList<ModuleDataEntry>();
						IEnumerable<ModuleDataEntry> enumerable = list.Where<ModuleDataEntry>((ModuleDataEntry x) => !x.weaponTemplate.defenseMode);
						if (enumerable.Any<ModuleDataEntry>())
						{
							list2 = enumerable.ToList<ModuleDataEntry>();
						}
						list2.MaxBy<ModuleDataEntry, float>((ModuleDataEntry x) => x.weaponTemplate.EstimateDPS(this.ExpectedCombatRange_km, null, true));
						this.weapons = new List<SimulatedCombat.SimulatedShipWeapon>();
						foreach (ModuleDataEntry moduleDataEntry in list)
						{
							TIShipWeaponTemplate weaponTemplate = moduleDataEntry.weaponTemplate;
							FireMode fireMode = FireMode.Offense;
							if (weaponTemplate.guardianMode)
							{
								fireMode = FireMode.Guardian;
							}
							else if (weaponTemplate.defenseMode)
							{
								fireMode = FireMode.Defense;
							}
							this.weapons.Add(new SimulatedCombat.SimulatedShipWeapon(this, moduleDataEntry, fireMode));
						}
					}
					return this.weapons;
				}
			}

			// Token: 0x17001214 RID: 4628
			// (get) Token: 0x06007A9A RID: 31386 RVA: 0x0031ED6C File Offset: 0x0031CF6C
			public override float ExpectedCombatRange_km
			{
				get
				{
					return this.CopyShip.role.GetExpectedCombatRange_km();
				}
			}

			// Token: 0x06007A9B RID: 31387 RVA: 0x0031ED80 File Offset: 0x0031CF80
			public SimulatedShip(SimulatedCombat combat, TISpaceShipState ship)
				: base(combat)
			{
				this.OriginalShip = ship;
				this.CopyShip = ship.template.CreateDummyShip();
				this.CopyShip.BecomeCopyOf(ship);
				foreach (TIOfficerState tiofficerState in this.OriginalShip.officers)
				{
					this.SimulatedOfficersToRealOfficers[tiofficerState.CreateDummy(this.CopyShip)] = tiofficerState;
				}
				this.CopyShip.fleet = this.OriginalShip.fleet;
				this.dummyHull = StrategyShipController.CreateHull(this.CopyShip);
			}

			// Token: 0x06007A9C RID: 31388 RVA: 0x0031EE48 File Offset: 0x0031D048
			public SimulatedShip(SimulatedCombat combat, TISpaceShipTemplate shipTemplate)
				: base(combat)
			{
				this.CopyShip = shipTemplate.CreateDummyShip();
				this.dummyHull = StrategyShipController.CreateHull(this.CopyShip);
			}

			// Token: 0x06007A9D RID: 31389 RVA: 0x0031EE7C File Offset: 0x0031D07C
			public override void SimulatePassageOfCombatTime(float time_s)
			{
				float num = 0.0003f;
				float num2 = Mathf.Min(new float[]
				{
					base.Combat.ElapsedTime_s * num,
					this.CopyShip.AvailableDeltaVForCombat_kps(),
					Mathf.Max(this.CopyShip.currentDeltaV_kps / 2f, this.CopyShip.currentDeltaV_kps - 10f)
				});
				this.CopyShip.ConsumeDeltaV(num2, false);
			}

			// Token: 0x06007A9E RID: 31390 RVA: 0x0031EEF0 File Offset: 0x0031D0F0
			public override TIFactionState GetFaction()
			{
				return this.OriginalShip.GetFaction();
			}

			// Token: 0x06007A9F RID: 31391 RVA: 0x0031EEFD File Offset: 0x0031D0FD
			public override float GetChanceToEvade(SimulatedCombat.SimulatedAttack attack)
			{
				return 1f - attack.Weapon.Template.EstimateChanceToHit(0f, this.CopyShip, null, -1f);
			}

			// Token: 0x06007AA0 RID: 31392 RVA: 0x0031EF26 File Offset: 0x0031D126
			public override float GetECMValue()
			{
				return this.CopyShip.ECMValue(this.GetFaction(), base.AlliedHab);
			}

			// Token: 0x06007AA1 RID: 31393 RVA: 0x0031EF3F File Offset: 0x0031D13F
			public override float TargetingBonus(TIShipWeaponTemplate weaponTemplate, TIHabState alliedHab)
			{
				return this.CopyShip.TargetingBonus(weaponTemplate, alliedHab);
			}

			// Token: 0x06007AA2 RID: 31394 RVA: 0x0031EF4E File Offset: 0x0031D14E
			public ArmorFacing GetStruckFacing(DamageSource source, out float struckAngle)
			{
				return this.dummyHull.StruckFacing(source, Vector3.zero, Vector3.forward, out struckAngle);
			}

			// Token: 0x06007AA3 RID: 31395 RVA: 0x0031EF68 File Offset: 0x0031D168
			public ArmorFacing GetStruckFacing(DamageSource source)
			{
				float num;
				return this.GetStruckFacing(source, out num);
			}

			// Token: 0x06007AA4 RID: 31396 RVA: 0x0031EF80 File Offset: 0x0031D180
			public override float ApplyDamage(DamageSource source)
			{
				float num;
				ArmorFacing struckFacing = this.GetStruckFacing(source, out num);
				float num2;
				float num3;
				this.CopyShip.ApplyDamage(source.damage.weapon, struckFacing, source.damage.range_km, source.damage.amount, source.damage.chippingAmount, source.damage.type, num, source.attacker.GetFaction(), out num2, out num3, source.damage.shreddingAmount);
				return num2;
			}

			// Token: 0x06007AA5 RID: 31397 RVA: 0x0031F00E File Offset: 0x0031D20E
			public override float GetCrossSectionalArea_m2(float angle = 3.4028235E+38f)
			{
				return this.CopyShip.GetCrossSectionalArea_m2(angle);
			}

			// Token: 0x06007AA6 RID: 31398 RVA: 0x0031F01C File Offset: 0x0031D21C
			public override bool WeaponIsOperable(ModuleDataEntry module)
			{
				return this.weapons.First<SimulatedCombat.SimulatedShipWeapon>((SimulatedCombat.SimulatedShipWeapon x) => x.Module == module).CanFire();
			}

			// Token: 0x06007AA7 RID: 31399 RVA: 0x0031F052 File Offset: 0x0031D252
			public override float FireControlFunction()
			{
				return this.CopyShip.GetSystemFunction(ShipSystem.FireControl);
			}

			// Token: 0x04005AE7 RID: 23271
			private Hull dummyHull;

			// Token: 0x04005AE8 RID: 23272
			private List<SimulatedCombat.SimulatedShipWeapon> weapons;

			// Token: 0x04005AE9 RID: 23273
			public TISpaceShipState CopyShip;

			// Token: 0x04005AEA RID: 23274
			public TISpaceShipState OriginalShip;

			// Token: 0x04005AEB RID: 23275
			public Dictionary<TIOfficerState, TIOfficerState> SimulatedOfficersToRealOfficers = new Dictionary<TIOfficerState, TIOfficerState>();
		}
	}
}
