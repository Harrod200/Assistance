using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000595 RID: 1429
	public abstract class CometTailController<T> : CometParticleController where T : CometTailSample
	{
		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06002604 RID: 9732 RVA: 0x000CE37D File Offset: 0x000CC57D
		public float NearParticleRadius_m
		{
			get
			{
				return this.BaseNearParticleRadius_m;
			}
		}

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06002605 RID: 9733 RVA: 0x000CE385 File Offset: 0x000CC585
		public float FarParticleRadius_m
		{
			get
			{
				return this.TailLengthModifier * this.BaseFarParticleRadius_m;
			}
		}

		// Token: 0x17000554 RID: 1364
		// (get) Token: 0x06002606 RID: 9734 RVA: 0x000CE394 File Offset: 0x000CC594
		public override float TargetOpacityFactor
		{
			get
			{
				return base.TargetOpacityFactor * Mathf.Pow(this.CometController.DistanceBasedProductivity, 0.4f);
			}
		}

		// Token: 0x17000555 RID: 1365
		// (get) Token: 0x06002607 RID: 9735 RVA: 0x000CE3B2 File Offset: 0x000CC5B2
		public float DesiredTailLength_m
		{
			get
			{
				return this.BaseTailLength_m * this.CometController.DistanceBasedProductivity;
			}
		}

		// Token: 0x17000556 RID: 1366
		// (get) Token: 0x06002608 RID: 9736 RVA: 0x000CE3C6 File Offset: 0x000CC5C6
		public float TailLengthModifier
		{
			get
			{
				return this.DesiredTailLength_m / this.BaseTailLength_m;
			}
		}

		// Token: 0x17000557 RID: 1367
		// (get) Token: 0x06002609 RID: 9737 RVA: 0x000CE3D5 File Offset: 0x000CC5D5
		public override bool DoNotDisplay
		{
			get
			{
				return base.DoNotDisplay || this.VisualSizeEstimate_deg < 0.9f || this.Path.Segments.Count == 0;
			}
		}

		// Token: 0x17000558 RID: 1368
		// (get) Token: 0x0600260A RID: 9738 RVA: 0x000CE404 File Offset: 0x000CC604
		protected CometTailPath Path
		{
			get
			{
				if (this.pathCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedPath = new CometTailPath(this.GetCometTailSamples());
					if (this.cachedPath.Segments.Count == 0)
					{
						return this.cachedPath;
					}
					Vector3 position = CameraManager.Singleton.unityCamera.transform.position;
					if (this.VisualSize_deg < 4f)
					{
						this.bakedVisualTFunction = (double t) => t;
					}
					else
					{
						this.bakedVisualTFunction = this.cachedPath.GetBakedVisualTFunction(position, (Vector3d p) => base.GetUnityPosition(p), 10);
					}
					this.pathCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedPath;
			}
		}

		// Token: 0x17000559 RID: 1369
		// (get) Token: 0x0600260B RID: 9739 RVA: 0x000CE4CC File Offset: 0x000CC6CC
		public float VisualSizeEstimate_deg
		{
			get
			{
				double desiredTailLength_m = (double)this.DesiredTailLength_m;
				Vector3d position = CameraManager.Singleton.Position;
				Vector3d globalPosition = base.Comet.GetGlobalPosition();
				float num = (float)Vector3d.Distance(in position, in globalPosition);
				return (float)Mathd.AngularDiameterOfPlane(desiredTailLength_m, (double)num);
			}
		}

		// Token: 0x1700055A RID: 1370
		// (get) Token: 0x0600260C RID: 9740 RVA: 0x000CE50A File Offset: 0x000CC70A
		public float VisualSize_deg
		{
			get
			{
				return (float)this.cachedPath.GetVisualLength(CameraManager.Singleton.unityCamera.transform.position, (Vector3d p) => base.GetUnityPosition(p), 1);
			}
		}

		// Token: 0x0600260D RID: 9741 RVA: 0x000CE53E File Offset: 0x000CC73E
		public override void LateUpdate()
		{
			base.LateUpdate();
		}

		// Token: 0x0600260E RID: 9742 RVA: 0x000CE548 File Offset: 0x000CC748
		public override ParticleSystem.EmitParams SpawnParticle(float t)
		{
			ParticleSystem.EmitParams emitParams = base.SpawnParticle(t);
			float visualT = this.GetVisualT(t);
			emitParams.startLifetime = (1f - t) * base.ParticleLifetime_s;
			emitParams.startColor = new Color32(0, 0, 0, 0);
			Vector3d position = this.Path.GetPosition((double)visualT);
			emitParams.position = base.GetUnityPosition(position);
			emitParams.velocity = global::UnityEngine.Random.insideUnitSphere;
			return emitParams;
		}

		// Token: 0x0600260F RID: 9743 RVA: 0x000CE5B4 File Offset: 0x000CC7B4
		public override void UpdateParticle(int particleIndex, ParticleSystem.Particle[] particles, List<Vector4> customParticleData0, List<Vector4> customParticleData1)
		{
			base.UpdateParticle(particleIndex, particles, customParticleData0, customParticleData1);
			ParticleSystem.Particle particle = particles[particleIndex];
			float num = 1f - particle.startLifetime / base.ParticleLifetime_s;
			float num2 = num + (1f - num) * (1f - particle.remainingLifetime / particle.startLifetime);
			float visualT = this.GetVisualT(num2);
			Vector3d vector3d = this.Path.GetPosition((double)visualT) + particle.velocity * (visualT * this.ExpansionVelocity_mps + this.InitialExpansion_m);
			particle.position = base.GetUnityPosition(vector3d);
			float num3 = Mathf.Lerp(this.NearParticleRadius_m, this.FarParticleRadius_m, visualT);
			particle.startSize = TIUtilities.GetUnityRadius_Plane(vector3d, num3);
			particles[particleIndex] = particle;
		}

		// Token: 0x06002610 RID: 9744 RVA: 0x000CE680 File Offset: 0x000CC880
		protected float GetVisualT(float t)
		{
			Vector3 position = CameraManager.Singleton.unityCamera.transform.position;
			return (float)this.bakedVisualTFunction((double)t);
		}

		// Token: 0x06002611 RID: 9745
		protected abstract T CreateParticleSample(TIDateTime date);

		// Token: 0x06002612 RID: 9746 RVA: 0x000CE6A8 File Offset: 0x000CC8A8
		protected float GetTailLength_days()
		{
			Vector3d cometPosition = base.Comet.GetGlobalPosition();
			if ((in this.tailLengthCachedPosition) == (in cometPosition) || TIFrameCounter.FrameCount == this.tailLengthCachedFrame)
			{
				return this.cachedTailLength_days;
			}
			float desiredTailLength_m = this.DesiredTailLength_m;
			if (desiredTailLength_m == 0f)
			{
				return 0f;
			}
			TIDateTime tidateTime = TITimeState.Now();
			float num = ((this is CometGasTailController) ? 0.1f : 1f);
			T sample = default(T);
			Func<double> func = () => (sample.Position_m - cometPosition).magnitude;
			do
			{
				tidateTime = new TIDateTime(tidateTime);
				tidateTime.AddDays(-num);
				sample = this.CreateParticleSample(tidateTime);
			}
			while (func() < (double)desiredTailLength_m);
			bool flag = false;
			int num2 = 6;
			for (int i = 0; i < num2; i++)
			{
				float num3 = num / Mathf.Pow(2f, (float)(i + 1));
				if (flag)
				{
					num3 *= -1f;
				}
				tidateTime = new TIDateTime(tidateTime);
				tidateTime.AddDays(num3);
				sample = this.CreateParticleSample(tidateTime);
				flag = func() < (double)desiredTailLength_m;
			}
			this.cachedTailLength_days = (float)(TITimeState.Now() - sample.SpawnDate).TotalDays;
			this.tailLengthCachedPosition = cometPosition;
			this.tailLengthCachedFrame = TIFrameCounter.FrameCount;
			return this.cachedTailLength_days;
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x000CE80C File Offset: 0x000CCA0C
		protected List<CometTailSample> GetCometTailSamples()
		{
			List<CometTailSample> list = new List<CometTailSample>();
			float tailLength_days = this.GetTailLength_days();
			if (tailLength_days == 0f)
			{
				return list;
			}
			int sampleResolution = this.SampleResolution;
			for (int i = 0; i <= sampleResolution; i++)
			{
				TIDateTime tidateTime = TITimeState.Now();
				tidateTime.AddDays((float)(-(float)i) * tailLength_days / (float)sampleResolution);
				list.Add(this.CreateParticleSample(tidateTime));
			}
			return list;
		}

		// Token: 0x04001C58 RID: 7256
		public float BaseNearParticleRadius_m;

		// Token: 0x04001C59 RID: 7257
		public float BaseFarParticleRadius_m;

		// Token: 0x04001C5A RID: 7258
		public float ExpansionVelocity_mps;

		// Token: 0x04001C5B RID: 7259
		public float InitialExpansion_m;

		// Token: 0x04001C5C RID: 7260
		public float BaseTailLength_m;

		// Token: 0x04001C5D RID: 7261
		public int SampleResolution;

		// Token: 0x04001C5E RID: 7262
		private CometTailPath cachedPath;

		// Token: 0x04001C5F RID: 7263
		private Func<double, double> bakedVisualTFunction;

		// Token: 0x04001C60 RID: 7264
		private int pathCachedFrame = -1;

		// Token: 0x04001C61 RID: 7265
		private float cachedTailLength_days;

		// Token: 0x04001C62 RID: 7266
		private Vector3d tailLengthCachedPosition;

		// Token: 0x04001C63 RID: 7267
		private int tailLengthCachedFrame = -1;
	}
}
