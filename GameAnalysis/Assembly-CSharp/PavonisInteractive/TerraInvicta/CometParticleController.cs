using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000594 RID: 1428
	public abstract class CometParticleController : MonoBehaviour
	{
		// Token: 0x1700054C RID: 1356
		// (get) Token: 0x060025F4 RID: 9716 RVA: 0x000CDCF0 File Offset: 0x000CBEF0
		public int TargetParticleCount
		{
			get
			{
				if (this.CometController.IsInOverrideRenderMode)
				{
					return this.ParticleSystem.main.maxParticles;
				}
				Vector3d globalPosition = this.Comet.GetGlobalPosition();
				Vector3d position = CameraManager.Singleton.Position;
				float num = (float)(Vector3d.Distance(in globalPosition, in position) / 149597870700.0);
				return Mathf.Max(Mathf.Min((int)((float)this.ParticleCountAtOneAU / num), this.ParticleSystem.main.maxParticles), this.MinimumParticleCount);
			}
		}

		// Token: 0x1700054D RID: 1357
		// (get) Token: 0x060025F5 RID: 9717 RVA: 0x000CDD78 File Offset: 0x000CBF78
		public virtual float TargetOpacityFactor
		{
			get
			{
				if (this.ParticleSystem.particleCount == 0)
				{
					return 1f;
				}
				return (float)this.ParticleSystem.main.maxParticles / (float)this.ParticleSystem.particleCount;
			}
		}

		// Token: 0x1700054E RID: 1358
		// (get) Token: 0x060025F6 RID: 9718 RVA: 0x000CDDB9 File Offset: 0x000CBFB9
		// (set) Token: 0x060025F7 RID: 9719 RVA: 0x000CDDC1 File Offset: 0x000CBFC1
		public float OpacityFactor { get; private set; } = 1f;

		// Token: 0x1700054F RID: 1359
		// (get) Token: 0x060025F8 RID: 9720 RVA: 0x000CDDCA File Offset: 0x000CBFCA
		public TISpaceBodyState Comet
		{
			get
			{
				return this.CometController.Comet;
			}
		}

		// Token: 0x17000550 RID: 1360
		// (get) Token: 0x060025F9 RID: 9721 RVA: 0x000CDDD7 File Offset: 0x000CBFD7
		public virtual bool DoNotDisplay
		{
			get
			{
				return this.CometController.DoNotDisplay;
			}
		}

		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x060025FA RID: 9722 RVA: 0x000CDDE4 File Offset: 0x000CBFE4
		public float ParticleLifetime_s
		{
			get
			{
				return this.ParticleSystem.main.startLifetime.Evaluate(0.5f);
			}
		}

		// Token: 0x060025FB RID: 9723 RVA: 0x000CDE14 File Offset: 0x000CC014
		public virtual ParticleSystem.EmitParams SpawnParticle(float t)
		{
			return new ParticleSystem.EmitParams
			{
				startColor = this.GetFeatheredColor(this.ParticleSystem.particleCount)
			};
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x000CDE42 File Offset: 0x000CC042
		public virtual void UpdateParticle(int particleIndex, ParticleSystem.Particle[] particles, List<Vector4> customParticleData0, List<Vector4> customParticleData1)
		{
			particles[particleIndex].startColor = this.GetFeatheredColor(particleIndex);
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x000CDE58 File Offset: 0x000CC058
		public virtual void LateUpdate()
		{
			if (this.DoNotDisplay)
			{
				this.ParticleSystem.Clear();
				return;
			}
			this.Color32 = this.Color;
			this.OpacityFactor = Mathf.Lerp(this.OpacityFactor, this.TargetOpacityFactor, this.OpacityLerpSpeed * Time.deltaTime);
			if (this.particlesArray == null || this.particlesArray.Length != this.ParticleSystem.main.maxParticles)
			{
				this.particlesArray = new ParticleSystem.Particle[this.ParticleSystem.main.maxParticles];
			}
			int particles = this.ParticleSystem.GetParticles(this.particlesArray);
			List<Vector4> list = null;
			List<Vector4> list2 = null;
			if (this.ParticleSystem.customData.enabled)
			{
				if (this.ParticleSystem.customData.GetMode(ParticleSystemCustomData.Custom1) != ParticleSystemCustomDataMode.Disabled)
				{
					this.ParticleSystem.GetCustomParticleData(list = new List<Vector4>(), ParticleSystemCustomData.Custom1);
				}
				if (this.ParticleSystem.customData.GetMode(ParticleSystemCustomData.Custom2) != ParticleSystemCustomDataMode.Disabled)
				{
					this.ParticleSystem.GetCustomParticleData(list2 = new List<Vector4>(), ParticleSystemCustomData.Custom2);
				}
			}
			for (int i = 0; i < particles; i++)
			{
				ParticleSystem.Particle[] array = this.particlesArray;
				int num = i;
				array[num].remainingLifetime = array[num].remainingLifetime - Time.deltaTime;
				this.UpdateParticle(i, this.particlesArray, list, list2);
			}
			for (int j = 0; j < particles - this.TargetParticleCount; j++)
			{
				int num2 = (int)(TIUtilities.RandomFloatValue() * 0.999999f * (float)particles);
				this.particlesArray[num2].remainingLifetime = 0f;
			}
			if (list != null)
			{
				this.ParticleSystem.SetCustomParticleData(list, ParticleSystemCustomData.Custom1);
			}
			if (list2 != null)
			{
				this.ParticleSystem.SetCustomParticleData(list2, ParticleSystemCustomData.Custom2);
			}
			this.ParticleSystem.SetParticles(this.particlesArray, particles);
			CometParticleController.ParticleSpawnBehavior particleSpawnBehavior;
			if ((float)this.ParticleSystem.particleCount / (float)this.TargetParticleCount < 0.96f)
			{
				particleSpawnBehavior = CometParticleController.ParticleSpawnBehavior.SpawnAlongPath;
			}
			else
			{
				particleSpawnBehavior = CometParticleController.ParticleSpawnBehavior.SpawnAtSource;
			}
			if (particleSpawnBehavior != CometParticleController.ParticleSpawnBehavior.SpawnAtSource)
			{
				if (particleSpawnBehavior != CometParticleController.ParticleSpawnBehavior.SpawnAlongPath)
				{
					return;
				}
				List<float> list3 = this.GetAgeSegments(10).ToList<float>();
				Dictionary<ValueTuple<float, float>, int> dictionary = new Dictionary<ValueTuple<float, float>, int>();
				for (int k = 0; k <= list3.Count; k++)
				{
					float num3 = 0f;
					if (k > 0)
					{
						num3 = list3[k - 1];
					}
					float num4 = 1f;
					if (k < list3.Count)
					{
						num4 = list3[k];
					}
					dictionary[new ValueTuple<float, float>(num3, num4)] = 0;
				}
				for (int l = 0; l < particles; l++)
				{
					float num5 = 1f - this.particlesArray[l].remainingLifetime / this.ParticleLifetime_s;
					if (num5 < 1f)
					{
						foreach (ValueTuple<float, float> valueTuple in dictionary.Keys.ToList<ValueTuple<float, float>>())
						{
							if (num5 >= valueTuple.Item1 && num5 <= valueTuple.Item2)
							{
								Dictionary<ValueTuple<float, float>, int> dictionary2 = dictionary;
								ValueTuple<float, float> valueTuple2 = valueTuple;
								int num6 = dictionary2[valueTuple2];
								dictionary2[valueTuple2] = num6 + 1;
							}
						}
					}
				}
				while (this.ParticleSystem.particleCount < this.TargetParticleCount)
				{
					ValueTuple<float, float> key = dictionary.MinBy<KeyValuePair<ValueTuple<float, float>, int>, int>((KeyValuePair<ValueTuple<float, float>, int> x) => x.Value).Key;
					float item = key.Item1;
					float item2 = key.Item2;
					float num7 = item + (item2 - item) * TIUtilities.RandomFloatValue();
					if (num7 != 1f)
					{
						this.ParticleSystem.Emit(this.SpawnParticle(num7), 1);
						Dictionary<ValueTuple<float, float>, int> dictionary3 = dictionary;
						ValueTuple<float, float> valueTuple2 = new ValueTuple<float, float>(item, item2);
						int num6 = dictionary3[valueTuple2];
						dictionary3[valueTuple2] = num6 + 1;
					}
				}
			}
			else
			{
				int num8 = ((float)this.TargetParticleCount / this.ParticleLifetime_s * Time.deltaTime).RoundUp();
				for (int m = 0; m < num8; m++)
				{
					if (this.ParticleSystem.particleCount >= this.TargetParticleCount)
					{
						return;
					}
					this.ParticleSystem.Emit(this.SpawnParticle(0f), 1);
				}
			}
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x000CE27C File Offset: 0x000CC47C
		public virtual void InitiateOverrideRenderMode(bool drawingToRenderTexture)
		{
			if (drawingToRenderTexture)
			{
				this.ParticleSystem.GetComponent<ParticleSystemRenderer>().material = this.RenderTextureParticleMaterial;
			}
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x000CE298 File Offset: 0x000CC498
		protected virtual IEnumerable<float> GetAgeSegments(int resolution = 10)
		{
			return from x in Enumerable.Range(0, resolution - 1)
				select (float)(x + 1) / (float)resolution;
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x000CE2D1 File Offset: 0x000CC4D1
		protected Vector3 GetUnityPosition(Vector3d position)
		{
			return CameraManager.Singleton.FastScaledPosition(position);
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x000CE2E4 File Offset: 0x000CC4E4
		public byte GetFeatheredAlphaByte(int index)
		{
			float num = this.Color.a * this.OpacityFactor * 255f * 0.999999f;
			int num2 = (int)num;
			if ((float)index < (num - (float)num2) * (float)this.ParticleSystem.particleCount)
			{
				num2++;
			}
			return (byte)Mathf.Clamp(num2, 0, 255);
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x000CE33C File Offset: 0x000CC53C
		public Color32 GetFeatheredColor(int index)
		{
			Color32 color = this.Color32;
			color.a = this.GetFeatheredAlphaByte(index);
			return color;
		}

		// Token: 0x04001C4E RID: 7246
		public CometController CometController;

		// Token: 0x04001C4F RID: 7247
		public ParticleSystem ParticleSystem;

		// Token: 0x04001C50 RID: 7248
		public Material RenderTextureParticleMaterial;

		// Token: 0x04001C51 RID: 7249
		public Color Color;

		// Token: 0x04001C52 RID: 7250
		private Color32 Color32;

		// Token: 0x04001C53 RID: 7251
		public int ParticleCountAtOneAU;

		// Token: 0x04001C54 RID: 7252
		public int MinimumParticleCount;

		// Token: 0x04001C55 RID: 7253
		public float OpacityLerpSpeed = 1f;

		// Token: 0x04001C57 RID: 7255
		private ParticleSystem.Particle[] particlesArray;

		// Token: 0x02000CF2 RID: 3314
		public enum ParticleSpawnBehavior
		{
			// Token: 0x0400500B RID: 20491
			None,
			// Token: 0x0400500C RID: 20492
			SpawnAtSource,
			// Token: 0x0400500D RID: 20493
			SpawnAlongPath
		}
	}
}
