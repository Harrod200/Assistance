using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

// Token: 0x02000032 RID: 50
public class BeamWeaponController : MonoBehaviour
{
	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000F62A File Offset: 0x0000D82A
	private float maxLength
	{
		get
		{
			return 100f;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060001E7 RID: 487 RVA: 0x0000F631 File Offset: 0x0000D831
	private bool hasTarget
	{
		get
		{
			return this.targetCombatant != null || this.targetProjectile != null || this.strategyLayerTarget != null;
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000F660 File Offset: 0x0000D860
	public void Initialize(IDamageable target)
	{
		this._collisionMask = LayerMask.NameToLayer("HurtBox");
		this.beamEffectController = base.GetComponent<BeamEffectController>();
		this.target = target;
		this.targetCombatant = target as CombatantController;
		this.targetProjectile = target as ProjectileController;
		this.initialized = true;
		this.beamEffectController.SetBeamPoints(base.transform.position, base.transform.position);
		this.UpdateVisualization();
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000F6DC File Offset: 0x0000D8DC
	public void Initialize(TIGameState shooter, TIGameState stratLayerTarget, TIDateTime time, int mask)
	{
		this._collisionMask = mask;
		this.beamEffectController = base.GetComponent<BeamEffectController>();
		this.targetCombatant = null;
		this.targetProjectile = null;
		this.strategyLayerTarget = stratLayerTarget;
		this.initialized = true;
		base.transform.localScale /= 20f;
		this.shooter = shooter;
		this.beamEffectController.SetBeamPoints(base.transform.position, base.transform.position);
		this.shotTime = time;
		this.UpdateVisualization();
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000F76E File Offset: 0x0000D96E
	public void OnEnable()
	{
		if (this.beamEffectController != null)
		{
			this.beamEffectController.enabled = true;
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x0000F78A File Offset: 0x0000D98A
	public void OnDisable()
	{
		if (this.beamEffectController != null)
		{
			this.beamEffectController.enabled = false;
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000F7A6 File Offset: 0x0000D9A6
	private void LateUpdate()
	{
		this.UpdateVisualization();
	}

	// Token: 0x060001ED RID: 493 RVA: 0x0000F7B0 File Offset: 0x0000D9B0
	private void UpdateVisualization()
	{
		if (!this.hasTarget || this.beamEffectController == null)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = base.transform.position;
		Vector3 vector3 = this.beamEffectController.EndPoint;
		if (!TIGlobalValuesState.isSpaceCombatEnabled)
		{
			if (!TIGameState.Valid(this.strategyLayerTarget) || !TIGameState.Valid(this.shooter))
			{
				this.DisableLaser();
				return;
			}
			TIDateTime tidateTime = TITimeState.Now();
			if (this.strategyLayerTarget.ref_habSite != null && !this.strategyLayerTarget.isSpaceShipState)
			{
				vector2 = this.shooter.ref_fleet.controller.transform.position;
				vector3 = this.strategyLayerTarget.ref_habSite.GetController().transform.position;
			}
			else if (this.strategyLayerTarget.ref_region != null && !this.strategyLayerTarget.isSpaceShipState)
			{
				vector2 = this.shooter.ref_fleet.controller.transform.position;
				vector3 = CameraManager.Singleton.ScaledPosition_DoNotTouchCache(this.strategyLayerTarget.ref_region.GetGlobalPosition(tidateTime));
			}
			else if (this.strategyLayerTarget.isSpaceShipState)
			{
				if (!TIGameState.Valid(this.strategyLayerTarget.ref_fleet))
				{
					this.DisableLaser();
					return;
				}
				if (this.shooter.ref_region != null)
				{
					vector2 = CameraManager.Singleton.ScaledPosition_DoNotTouchCache(this.shooter.ref_region.GetGlobalPosition(tidateTime));
				}
				else
				{
					vector2 = this.shooter.ref_habSite.GetController().transform.position;
				}
				vector3 = this.strategyLayerTarget.ref_fleet.controller.transform.position;
			}
			else if (this.strategyLayerTarget.ref_spaceObject != null)
			{
				vector3 = this.strategyLayerTarget.ref_spaceObject.controller.transform.position;
			}
		}
		else
		{
			if (this.targetCombatant != null)
			{
				vector3 = this.target.position;
			}
			if (this.target != null)
			{
				RaycastHit[] array = (from x in Physics.RaycastAll(base.transform.position, this.target.position - base.transform.position, this.maxLength, ~this._collisionMask)
					where x.collider.gameObject.layer == this._collisionMask
					select x).ToArray<RaycastHit>();
				if (this.targetCombatant != null)
				{
					array = array.Where<RaycastHit>((RaycastHit x) => x.collider.GetComponentInParent<CombatantController>() == this.targetCombatant).ToArray<RaycastHit>();
				}
				else if (this.targetProjectile != null)
				{
					array = array.Where<RaycastHit>((RaycastHit x) => x.collider.GetComponentInParent<ProjectileController>() == this.targetProjectile).ToArray<RaycastHit>();
				}
				else
				{
					array = array.Where<RaycastHit>((RaycastHit x) => x.collider.GetComponentInParent<SpaceBodyController>()).ToArray<RaycastHit>();
				}
				if (array.Length != 0)
				{
					RaycastHit raycastHit = array[0];
					vector3 = raycastHit.point;
					vector = this.target.position - raycastHit.point;
				}
				else if (this.targetProjectile != null)
				{
					IDamageable damageable = this.target;
					vector3 = ((damageable != null) ? damageable.position : this.target.position);
				}
				else
				{
					IDamageable damageable2 = this.target;
					vector3 = ((damageable2 != null) ? damageable2.position : this.target.position) + vector;
				}
			}
		}
		this.beamEffectController.enabled = true;
		this.beamEffectController.SetBeamPoints(vector2, vector3);
		if (this.strategyLayerTarget != null && !this.strategyLayerTarget.deleted && this.strategyLayerTarget.ref_spaceBody != null)
		{
			this.beamEffectController.hitParticleMaxScale = 4.5f * (float)this.strategyLayerTarget.ref_spaceBody.GetAngularDiameter() / CameraManager.Singleton.unityCamera.fieldOfView / vector3.magnitude;
			if (this.strategyLayerTarget.isSpaceShipState)
			{
				this.beamEffectController.hitParticleMaxScale *= 40f;
			}
		}
	}

	// Token: 0x060001EE RID: 494 RVA: 0x0000FBD6 File Offset: 0x0000DDD6
	public void DisableLaser()
	{
		if (this != null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
	public void EnableLaser()
	{
		base.enabled = true;
	}

	// Token: 0x040001FE RID: 510
	public static float beamScaling = 0.015f;

	// Token: 0x040001FF RID: 511
	private BeamEffectController beamEffectController;

	// Token: 0x04000200 RID: 512
	private bool initialized;

	// Token: 0x04000201 RID: 513
	private LayerMask _collisionMask;

	// Token: 0x04000202 RID: 514
	private TIGameState shooter;

	// Token: 0x04000203 RID: 515
	public IDamageable target;

	// Token: 0x04000204 RID: 516
	private CombatantController targetCombatant;

	// Token: 0x04000205 RID: 517
	private ProjectileController targetProjectile;

	// Token: 0x04000206 RID: 518
	private TIGameState strategyLayerTarget;

	// Token: 0x04000207 RID: 519
	private TIDateTime shotTime;
}
