using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007B1 RID: 1969
	[Serializable]
	public class TISpaceCombatProjectileState : TIGameState, CombatTargetableState
	{
		// Token: 0x17000C94 RID: 3220
		// (get) Token: 0x060042A6 RID: 17062 RVA: 0x001AE775 File Offset: 0x001AC975
		// (set) Token: 0x060042A7 RID: 17063 RVA: 0x001AE77D File Offset: 0x001AC97D
		public List<CombatWeaponCarrierState> enemiesTargetingMe { get; private set; }

		// Token: 0x17000C95 RID: 3221
		// (get) Token: 0x060042A8 RID: 17064 RVA: 0x001AE786 File Offset: 0x001AC986
		public float effectiveMass_kg
		{
			get
			{
				return this.originWeapon.warheadMass_kg - this.massDamage_kg;
			}
		}

		// Token: 0x17000C96 RID: 3222
		// (get) Token: 0x060042A9 RID: 17065 RVA: 0x001AE79A File Offset: 0x001AC99A
		public override TIFactionState ref_faction
		{
			get
			{
				return this.shootingFaction;
			}
		}

		// Token: 0x17000C97 RID: 3223
		// (get) Token: 0x060042AA RID: 17066 RVA: 0x001AE7A2 File Offset: 0x001AC9A2
		public override TISpaceFleetState ref_fleet
		{
			get
			{
				TISpaceShipState tispaceShipState = this.origin as TISpaceShipState;
				return ((tispaceShipState != null) ? tispaceShipState.fleet : null) ?? null;
			}
		}

		// Token: 0x060042AB RID: 17067 RVA: 0x001AE7C0 File Offset: 0x001AC9C0
		public TIGameState GetTargetableState()
		{
			return this;
		}

		// Token: 0x060042AC RID: 17068 RVA: 0x001AE7C3 File Offset: 0x001AC9C3
		public bool IsAlien()
		{
			return this.shootingFaction.IsAlienFaction;
		}

		// Token: 0x060042AD RID: 17069 RVA: 0x001AE7D0 File Offset: 0x001AC9D0
		public override bool Initialize()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.enemiesTargetingMe = new List<CombatWeaponCarrierState>();
			return base.Initialize();
		}

		// Token: 0x060042AE RID: 17070 RVA: 0x001AE7F4 File Offset: 0x001AC9F4
		private void FireCommon(CombatWeaponCarrierState origin, TIDateTime launchTime, Vector3 originPosition, Vector3 originVelocity_kps, Vector3 expectedTargetPosition)
		{
			this.origin = origin;
			this.originPosition = originPosition;
			this.expectedTargetPosition = expectedTargetPosition;
			this.launchTime = new TIDateTime();
			this.launchTime.CopyDateTime(launchTime);
			this.massDamage_kg = 0f;
			this.shootingFaction = origin.GetFaction();
			this.shootingTeam = GameControl.spaceCombat.combatState.primaryCombatFaction(this.shootingFaction);
			this.position = originPosition;
			this.enemiesTargetingMe.Clear();
		}

		// Token: 0x060042AF RID: 17071 RVA: 0x001AE874 File Offset: 0x001ACA74
		public void Fire(CombatWeaponCarrierState origin, TIGunTypeWeaponTemplate originWeapon, TIDateTime launchTime, Vector3 originPosition, Vector3 expectedTargetPosition, Vector3 originVelocity_kps)
		{
			this.originWeapon = originWeapon;
			this.FireCommon(origin, launchTime, originPosition, originVelocity_kps, expectedTargetPosition);
			this.velocityVector_kps = originVelocity_kps + (expectedTargetPosition - originPosition).normalized * originWeapon.muzzleVelocity_kps;
			Dictionary<TIFactionState, int> liveBallistics = GameControl.spaceCombat.liveBallistics;
			TIFactionState tifactionState = this.shootingTeam;
			liveBallistics[tifactionState]++;
		}

		// Token: 0x060042B0 RID: 17072 RVA: 0x001AE8E4 File Offset: 0x001ACAE4
		public void Fire(CombatWeaponCarrierState origin, TIMissileTemplate originWeapon, TIDateTime launchTime, Vector3 originPosition, Vector3 expectedTargetPosition, Vector3 originVelocity_kps)
		{
			this.originWeapon = originWeapon;
			this.FireCommon(origin, launchTime, originPosition, originVelocity_kps, expectedTargetPosition);
			this.velocityVector_kps = originVelocity_kps;
			Dictionary<TIFactionState, int> liveMissiles = GameControl.spaceCombat.liveMissiles;
			TIFactionState tifactionState = this.shootingTeam;
			liveMissiles[tifactionState]++;
		}

		// Token: 0x060042B1 RID: 17073 RVA: 0x001AE930 File Offset: 0x001ACB30
		public float ECMValue(TIFactionState attacker, TIHabState alliedHab = null)
		{
			switch (this.originWeapon.weaponClass)
			{
			case WeaponClass.Missile:
				return TIEffectsState.SumEffectsModifiers(Context.MissileECM, this.shootingFaction, 0f, null);
			}
			return 0f;
		}

		// Token: 0x060042B2 RID: 17074 RVA: 0x001AE988 File Offset: 0x001ACB88
		public Vector3 ProjectedLinearPositionAtTime_FromOrigin(DateTime dateTime)
		{
			float num = (float)(dateTime - this.launchTime.ExportTime()).TotalSeconds;
			return this.originPosition + num * 0.05f * this.velocityVector_kps;
		}

		// Token: 0x060042B3 RID: 17075 RVA: 0x001AE9D0 File Offset: 0x001ACBD0
		public Vector3 ProjectedLinearPositionAtTime_FromCurrent(DateTime dateTime)
		{
			float num = (float)(dateTime - this.gameTime.currentTime.ExportTime()).TotalSeconds;
			return this.position + num * 0.05f * this.velocityVector_kps;
		}

		// Token: 0x060042B4 RID: 17076 RVA: 0x001AEA1A File Offset: 0x001ACC1A
		public void UpdatePosition(Vector3 position)
		{
			this.position = position;
		}

		// Token: 0x060042B5 RID: 17077 RVA: 0x001AEA23 File Offset: 0x001ACC23
		public void EnemyTargetsMe(CombatWeaponCarrierState shooter)
		{
			this.enemiesTargetingMe.Add(shooter);
		}

		// Token: 0x060042B6 RID: 17078 RVA: 0x001AEA34 File Offset: 0x001ACC34
		public void RemoveFromLiveProjectiles()
		{
			Dictionary<TIFactionState, int> dictionary;
			TIFactionState tifactionState;
			if (this.originWeapon.isMissileWeapon)
			{
				dictionary = GameControl.spaceCombat.liveMissiles;
				tifactionState = this.shootingTeam;
				dictionary[tifactionState]--;
				return;
			}
			dictionary = GameControl.spaceCombat.liveBallistics;
			tifactionState = this.shootingTeam;
			dictionary[tifactionState]--;
		}

		// Token: 0x060042B7 RID: 17079 RVA: 0x001AEA93 File Offset: 0x001ACC93
		public void OnDestroyed()
		{
			this.enemiesTargetingMe.Clear();
		}

		// Token: 0x060042B8 RID: 17080 RVA: 0x001AEAA0 File Offset: 0x001ACCA0
		public static float CrossSectionalArea_m2(TIShipWeaponTemplate weaponTemplate, float angle_degrees = -3.4028235E+38f)
		{
			float num;
			if (weaponTemplate.isMissileWeapon)
			{
				if (weaponTemplate.ref_missileWeapon.ammoMass_kg < 2500f)
				{
					num = 0.3f;
				}
				else
				{
					num = 0.45f;
				}
			}
			else
			{
				num = (float)weaponTemplate.internalSize / 20f;
			}
			return 3.1415927f * num * num * 1.5f;
		}

		// Token: 0x060042B9 RID: 17081 RVA: 0x001AEAFC File Offset: 0x001ACCFC
		public float CrossSectionalArea_m2(float angle_degrees = -3.4028235E+38f)
		{
			return TISpaceCombatProjectileState.CrossSectionalArea_m2(this.originWeapon, angle_degrees);
		}

		// Token: 0x060042BA RID: 17082 RVA: 0x001AEB0C File Offset: 0x001ACD0C
		public bool WillHitSphere_Old(Vector3 targetPosition, Vector3 targetVelocity, IDamageableType targetType, CombatantController targetController)
		{
			Vector3 normalized = this.velocityVector_kps.normalized;
			float num = Vector3.Distance(targetPosition, this.position);
			Vector3 vector = normalized * num + this.position;
			float num2 = Vector3.Distance(targetPosition + targetVelocity * num, vector);
			bool flag;
			if (targetType != IDamageableType.Ship)
			{
				if (targetType != IDamageableType.StationModule)
				{
					flag = num2 <= 0.02f * GameControl.spaceCombat.modelScalingFactor;
				}
				else
				{
					flag = num2 <= targetController.ref_habModuleController.combatHitColliders[0].radius * GameControl.spaceCombat.modelScalingFactor;
				}
			}
			else
			{
				flag = num2 <= targetController.ref_shipController.ShipState.hull.length_m * GameControl.spaceCombat.modelScalingFactor;
			}
			return flag;
		}

		// Token: 0x060042BB RID: 17083 RVA: 0x001AEBD0 File Offset: 0x001ACDD0
		public bool WillHitSphere(Vector3 targetPosition, Vector3 targetVelocity, IDamageableType targetType, CombatantController targetController)
		{
			Vector3 vector = this.velocityVector_kps * 0.05f;
			float num = targetPosition.x - this.position.x;
			float num2 = targetVelocity.x - vector.x;
			float num3 = targetPosition.y - this.position.y;
			float num4 = targetVelocity.y - vector.y;
			float num5 = targetPosition.z - this.position.z;
			float num6 = targetVelocity.z - vector.z;
			float num7 = -1f * ((num * num2 + num3 * num4 + num5 * num6) / (num2 * num2 + num4 * num4 + num6 * num6));
			if (num7 > 0f)
			{
				Vector3 vector2 = this.position + num7 * vector;
				Vector3 vector3 = targetPosition + num7 * targetVelocity;
				float num8 = Vector3.Distance(vector2, vector3);
				bool flag;
				if (targetType != IDamageableType.Ship)
				{
					if (targetType != IDamageableType.StationModule)
					{
						flag = num8 <= 0.02f * GameControl.spaceCombat.modelScalingFactor;
					}
					else
					{
						flag = num8 <= targetController.ref_habModuleController.combatHitColliders[0].radius * GameControl.spaceCombat.modelScalingFactor;
					}
				}
				else
				{
					flag = num8 <= targetController.ref_shipController.ShipState.hull.length_m * GameControl.spaceCombat.modelScalingFactor;
				}
				return flag;
			}
			return false;
		}

		// Token: 0x060042BC RID: 17084 RVA: 0x001AED34 File Offset: 0x001ACF34
		public static Vector3 FirstOrderInterceptPosition(Vector3 shooterPosition, Vector3 shooterVelocity_u, float shotSpeed_u, Vector3 targetPosition, Vector3 targetVelocity_u, out bool impossible)
		{
			Vector3 vector = targetPosition - shooterPosition;
			Vector3 vector2 = targetVelocity_u - shooterVelocity_u;
			double num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2);
			impossible = num <= 0.0;
			return targetPosition + (float)num * vector2;
		}

		// Token: 0x060042BD RID: 17085 RVA: 0x001AED7C File Offset: 0x001ACF7C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static double FirstOrderInterceptTime(double shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
		{
			double num = (double)targetRelativeVelocity.sqrMagnitude - shotSpeed * shotSpeed;
			if (Mathd.Abs(num) < 9.999999747378752E-06)
			{
				return Mathd.Max((double)(-(double)targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition))), 0.0);
			}
			double num2 = (double)(2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
			double num3 = (double)targetRelativePosition.sqrMagnitude;
			double num4 = num2 * num2 - 4.0 * num * num3;
			if (num4 > 0.0)
			{
				double num5 = (-num2 + Mathd.Sqrt(num4)) / (2.0 * num);
				double num6 = (-num2 - Mathd.Sqrt(num4)) / (2.0 * num);
				if (num5 <= 0.0)
				{
					return Mathd.Max(num6, 0.0);
				}
				if (num6 > 0.0)
				{
					return Mathd.Min(num5, num6);
				}
				return num5;
			}
			else
			{
				if (num4 < 0.0)
				{
					return 0.0;
				}
				return Mathd.Max(-num2 / (2.0 * num), 0.0);
			}
		}

		// Token: 0x060042BE RID: 17086 RVA: 0x001AEE9C File Offset: 0x001AD09C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool SolveQuadratic(double a, double b, double c, out double t1, out double t2)
		{
			double num = b * b - 4.0 * a * c;
			t1 = 0.0;
			t2 = t1;
			if (num > 0.0)
			{
				t1 = (-b + Math.Sqrt(num)) / (2.0 * a);
				t2 = (-b - Math.Sqrt(num)) / (2.0 * a);
				return true;
			}
			return num >= 0.0;
		}

		// Token: 0x060042BF RID: 17087 RVA: 0x001AEF18 File Offset: 0x001AD118
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Vector3 SecondOrderInterceptPosition(Vector3 shooterPosition, Vector3 shooterVelocity_u, float shotSpeed_u, Vector3 targetPosition, Vector3 targetVelocity_u, Vector3 targetAcceleration_u, float shootercooldown_s, out bool impossible)
		{
			if (targetAcceleration_u == Vector3.zero)
			{
				return TISpaceCombatProjectileState.FirstOrderInterceptPosition(shooterPosition, shooterVelocity_u, shotSpeed_u, targetPosition, targetVelocity_u, out impossible);
			}
			Vector3 vector = targetPosition - shooterPosition;
			Vector3 vector2 = targetVelocity_u - shooterVelocity_u;
			double num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2);
			double num2 = num;
			num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2 + targetAcceleration_u * (float)Mathd.Min(num, 160.0) / 2f);
			if (num != num2)
			{
				double num3 = num;
				num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2 + targetAcceleration_u * (float)Mathd.Min(num, 160.0) / 2f);
				if (num3 != num)
				{
					double num4 = num;
					num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2 + targetAcceleration_u * (float)Mathd.Min(num, 160.0) / 2f);
					if (num4 != num)
					{
						num = TISpaceCombatProjectileState.FirstOrderInterceptTime((double)shotSpeed_u, vector, vector2 + targetAcceleration_u * (float)Mathd.Min(num, 160.0) / 2f);
					}
				}
			}
			impossible = num <= 0.0;
			if (impossible)
			{
				return TISpaceCombatProjectileState.FirstOrderInterceptPosition(shooterPosition, shooterVelocity_u, shotSpeed_u, targetPosition, targetVelocity_u, out impossible);
			}
			return targetPosition + (float)num * (vector2 + targetAcceleration_u * (float)Mathd.Min(num, 160.0) / 2f);
		}

		// Token: 0x060042C0 RID: 17088 RVA: 0x001AF088 File Offset: 0x001AD288
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool FirstOrderInterceptPosition(Vector3 chaserPosition, float chaserSpeed, Vector3 runnerPosition, Vector3 runnerVelocity, out Vector3 collisionPoint)
		{
			if (chaserPosition == runnerPosition)
			{
				collisionPoint = chaserPosition;
				return false;
			}
			float magnitude = runnerVelocity.magnitude;
			if (Mathf.Approximately(chaserSpeed, 0f))
			{
				collisionPoint = Vector3.zero;
				return false;
			}
			if (Mathf.Approximately(magnitude, 0f))
			{
				collisionPoint = Vector3.zero;
				return false;
			}
			Vector3 vector = chaserPosition - runnerPosition;
			double num = (double)vector.magnitude;
			double num2 = (double)(chaserSpeed * chaserSpeed - magnitude * magnitude);
			double num3 = (double)(2f * Vector3.Dot(vector, runnerVelocity));
			double num4 = -num * num;
			double num5;
			double num6;
			if (!TISpaceCombatProjectileState.SolveQuadratic(num2, num3, num4, out num5, out num6))
			{
				collisionPoint = Vector3.zero;
				return false;
			}
			if (num5 < 0.0 && num6 < 0.0)
			{
				collisionPoint = Vector3.zero;
				return false;
			}
			double num7;
			if (num5 > 0.0 && num6 > 0.0)
			{
				num7 = Math.Min(num5, num6);
			}
			else
			{
				num7 = Math.Max(num5, num6);
			}
			collisionPoint = runnerPosition + runnerVelocity * (float)num7;
			return true;
		}

		// Token: 0x040027DC RID: 10204
		public bool thrustersEnabled;

		// Token: 0x040027DD RID: 10205
		public float thrustAmount;

		// Token: 0x040027DE RID: 10206
		public Vector3 position;

		// Token: 0x040027DF RID: 10207
		public Vector3 originPosition;

		// Token: 0x040027E0 RID: 10208
		public Vector3 expectedTargetPosition;

		// Token: 0x040027E1 RID: 10209
		public Vector3 velocityVector_kps;

		// Token: 0x040027E2 RID: 10210
		public float deltaV;

		// Token: 0x040027E3 RID: 10211
		public TIDateTime launchTime;

		// Token: 0x040027E4 RID: 10212
		public CombatWeaponCarrierState origin;

		// Token: 0x040027E5 RID: 10213
		public TIProjectileWeaponTemplate originWeapon;

		// Token: 0x040027E6 RID: 10214
		public TIFactionState shootingFaction;

		// Token: 0x040027E7 RID: 10215
		public TIFactionState shootingTeam;

		// Token: 0x040027E8 RID: 10216
		private GameTimeManager gameTime;

		// Token: 0x040027EA RID: 10218
		public float massDamage_kg;
	}
}
