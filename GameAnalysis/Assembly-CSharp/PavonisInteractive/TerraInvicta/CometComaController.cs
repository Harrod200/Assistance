using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200058E RID: 1422
	public class CometComaController : CometParticleController
	{
		// Token: 0x17000537 RID: 1335
		// (get) Token: 0x060025C7 RID: 9671 RVA: 0x000CCFEC File Offset: 0x000CB1EC
		public float RelativeRadius
		{
			get
			{
				return (float)base.Comet.meanRadius_m / 2400f;
			}
		}

		// Token: 0x17000538 RID: 1336
		// (get) Token: 0x060025C8 RID: 9672 RVA: 0x000CD000 File Offset: 0x000CB200
		public float DustRadius_m
		{
			get
			{
				return this.BaseDustRadius_m * this.RelativeRadius;
			}
		}

		// Token: 0x17000539 RID: 1337
		// (get) Token: 0x060025C9 RID: 9673 RVA: 0x000CD00F File Offset: 0x000CB20F
		public float ExpansionVelocity_mps
		{
			get
			{
				return this.BaseExpansionVelocity_mps * Mathf.Pow(this.RelativeRadius, 0.5f);
			}
		}

		// Token: 0x1700053A RID: 1338
		// (get) Token: 0x060025CA RID: 9674 RVA: 0x000CD028 File Offset: 0x000CB228
		public float SolarWindAcceleration_mps2
		{
			get
			{
				return this.BaseSolarWindAcceleration_mps2 * Mathf.Pow(this.RelativeRadius, 0.5f);
			}
		}

		// Token: 0x1700053B RID: 1339
		// (get) Token: 0x060025CB RID: 9675 RVA: 0x000CD041 File Offset: 0x000CB241
		public override bool DoNotDisplay
		{
			get
			{
				return base.DoNotDisplay && (!GameControl.control.skirmishMode || !this.CometController.IsInOverrideRenderMode);
			}
		}

		// Token: 0x060025CC RID: 9676 RVA: 0x000CD06C File Offset: 0x000CB26C
		public override void LateUpdate()
		{
			if (this.DoNotDisplay)
			{
				this.ParticleSystem.Clear();
				GameObject glow = this.Glow;
				if (glow == null)
				{
					return;
				}
				glow.SetActive(false);
				return;
			}
			else
			{
				bool flag3;
				if (!this.CometController.IsInOverrideRenderMode)
				{
					SpaceObjectController spaceObjectController = this.CometController.SpaceObjectController;
					bool? flag;
					if (spaceObjectController == null)
					{
						flag = null;
					}
					else
					{
						GameObject modelLink = spaceObjectController.modelLink;
						flag = ((modelLink != null) ? new bool?(modelLink.activeInHierarchy) : null);
					}
					bool? flag2 = flag;
					flag3 = flag2.GetValueOrDefault();
				}
				else
				{
					flag3 = true;
				}
				if (!flag3)
				{
					this.ParticleSystem.Clear();
				}
				else
				{
					base.LateUpdate();
				}
				if (!(this.Glow == null) && !this.CometController.IsInOverrideRenderMode)
				{
					this.Glow.SetActive(true);
					Vector3d normalized = (base.Comet.GetGlobalPosition() - CameraManager.Singleton.Position).normalized;
					Vector3d vector3d = base.Comet.GetGlobalPosition() + normalized * base.Comet.meanRadius_m * 2.0;
					Vector3 vector = CameraManager.Singleton.ScaledPosition(vector3d);
					float num = this.CometController.DistanceBasedProductivity * (1f - this.CometController.VolatileWaterFraction);
					float num2 = this.GlowEdgeWidth_deg * num;
					float num3 = (float)base.Comet.GetAngularDiameter() + num2 * 2f;
					float unityRadiusFromAngularDiameter_Plane = TIUtilities.GetUnityRadiusFromAngularDiameter_Plane(Vector3.Distance(CameraManager.Singleton.unityCamera.transform.position, vector), num3);
					this.Glow.transform.position = vector;
					this.Glow.transform.localScale = Vector3.one * unityRadiusFromAngularDiameter_Plane * 2f;
					this.Glow.transform.LookAt(CameraManager.Singleton.unityCamera.transform.position);
					this.GlowRenderer.material.color = new Color(this.GlowColor.r, this.GlowColor.g, this.GlowColor.b, this.GlowColor.a * num);
					return;
				}
				GameObject glow2 = this.Glow;
				if (glow2 == null)
				{
					return;
				}
				glow2.SetActive(false);
				return;
			}
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x000CD2A4 File Offset: 0x000CB4A4
		public override ParticleSystem.EmitParams SpawnParticle(float t)
		{
			ParticleSystem.EmitParams emitParams = base.SpawnParticle(t);
			emitParams.startLifetime = (1f - t) * base.ParticleLifetime_s;
			emitParams.velocity = this.ExpansionVelocity_mps * global::UnityEngine.Random.insideUnitSphere;
			emitParams.position = this.CometController.transform.position;
			return emitParams;
		}

		// Token: 0x060025CE RID: 9678 RVA: 0x000CD300 File Offset: 0x000CB500
		public override void UpdateParticle(int particleIndex, ParticleSystem.Particle[] particles, List<Vector4> customParticleData0, List<Vector4> customParticleData1)
		{
			base.UpdateParticle(particleIndex, particles, customParticleData0, customParticleData1);
			ParticleSystem.Particle particle = particles[particleIndex];
			Vector3 vector = customParticleData0[particleIndex];
			Vector3 velocity = particle.velocity;
			if (vector.magnitude == 0f)
			{
				particle.remainingLifetime = particle.startLifetime;
				particle.startLifetime = base.ParticleLifetime_s;
				vector = particle.velocity.normalized * (float)base.Comet.meanRadius_m * this.RelativeSpawnAltitude;
				float num = 1f - particle.remainingLifetime / base.ParticleLifetime_s;
				if (num > 0.001f)
				{
					int num2 = 10;
					float num3 = num * base.ParticleLifetime_s / (float)num2;
					for (int i = 0; i < num2; i++)
					{
						this.Timestep(ref vector, ref velocity, num3);
					}
				}
			}
			if (this.CometController.IsInOverrideRenderMode)
			{
				particle.position = this.CometController.transform.position + vector * this.CometController.OverrideSizeFactor;
			}
			else
			{
				particle.position = base.GetUnityPosition(base.Comet.GetGlobalPosition() + vector);
			}
			this.Timestep(ref vector, ref velocity, Time.deltaTime);
			customParticleData0[particleIndex] = vector;
			particle.velocity = velocity;
			float magnitude = vector.magnitude;
			float num4 = 1f + this.RadiusScalingFactor * (magnitude - (float)base.Comet.meanRadius_m) / (float)base.Comet.meanRadius_m;
			if (this.CometController.IsInOverrideRenderMode)
			{
				particle.startSize = this.DustRadius_m * num4 * this.CometController.OverrideSizeFactor / base.transform.lossyScale.x;
			}
			else
			{
				particle.startSize = TIUtilities.GetUnityRadius_Plane(base.Comet.GetGlobalPosition() + vector, this.DustRadius_m * num4);
			}
			particle.startColor = new Color(this.Color.r, this.Color.g, this.Color.b, this.Color.a * num4);
			particles[particleIndex] = particle;
		}

		// Token: 0x060025CF RID: 9679 RVA: 0x000CD544 File Offset: 0x000CB744
		private void Timestep(ref Vector3 offsetFromComet, ref Vector3 velocity, float timeStep_s)
		{
			float num = 1f;
			float magnitude = offsetFromComet.magnitude;
			if ((double)magnitude < base.Comet.meanRadius_m * (double)this.ComaStillnessZone)
			{
				num = 0f;
			}
			else if ((double)magnitude < base.Comet.meanRadius_m * (double)this.ComaSlownessZone)
			{
				num = (float)(((double)magnitude - base.Comet.meanRadius_m * (double)this.ComaStillnessZone) / (base.Comet.meanRadius_m * (double)this.ComaSlownessZone - base.Comet.meanRadius_m * (double)this.ComaStillnessZone));
				num = Mathf.Clamp(num, 0f, 1f);
			}
			Vector3 vector;
			if (this.CometController.IsInOverrideRenderMode)
			{
				vector = Vector3.Cross(this.CometController.transform.position - this.CometController.OverrideCamera.transform.position, Vector3.up).normalized;
			}
			else
			{
				vector = (Vector3)(base.Comet.GetGlobalPosition() + offsetFromComet).normalized;
			}
			offsetFromComet += velocity * timeStep_s;
			velocity += num * this.SolarWindAcceleration_mps2 * vector * timeStep_s;
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x000CD6A0 File Offset: 0x000CB8A0
		public override void InitiateOverrideRenderMode(bool drawingToRenderTexture)
		{
			base.InitiateOverrideRenderMode(drawingToRenderTexture);
			if (drawingToRenderTexture)
			{
				ParticleSystem.ColorOverLifetimeModule colorOverLifetime = this.ParticleSystem.colorOverLifetime;
				Gradient gradient = new Gradient();
				gradient.SetKeys(new GradientColorKey[]
				{
					new GradientColorKey(Color.white, 0f),
					new GradientColorKey(Color.white, 1f)
				}, new GradientAlphaKey[]
				{
					new GradientAlphaKey(0f, 0f),
					new GradientAlphaKey(0f, 0.35f),
					new GradientAlphaKey(1f, 0.45f),
					new GradientAlphaKey(1f, 0.9f),
					new GradientAlphaKey(0f, 1f)
				});
				colorOverLifetime.color = gradient;
			}
		}

		// Token: 0x04001C39 RID: 7225
		public float BaseExpansionVelocity_mps;

		// Token: 0x04001C3A RID: 7226
		public float BaseSolarWindAcceleration_mps2;

		// Token: 0x04001C3B RID: 7227
		public float ComaStillnessZone;

		// Token: 0x04001C3C RID: 7228
		public float ComaSlownessZone;

		// Token: 0x04001C3D RID: 7229
		public float BaseDustRadius_m;

		// Token: 0x04001C3E RID: 7230
		public float RadiusScalingFactor;

		// Token: 0x04001C3F RID: 7231
		public float RelativeSpawnAltitude;

		// Token: 0x04001C40 RID: 7232
		private const float calibrationRadius_m = 2400f;

		// Token: 0x04001C41 RID: 7233
		public GameObject Glow;

		// Token: 0x04001C42 RID: 7234
		public Renderer GlowRenderer;

		// Token: 0x04001C43 RID: 7235
		public float GlowEdgeWidth_deg = 5f;

		// Token: 0x04001C44 RID: 7236
		public Color GlowColor;
	}
}
