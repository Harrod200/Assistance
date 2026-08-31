using System;
using Pixelplacement;
using UnityEngine;

// Token: 0x02000417 RID: 1047
[ExecuteInEditMode]
[RequireComponent(typeof(Spline))]
public class SplineControlledParticleSystem : MonoBehaviour
{
	// Token: 0x06001560 RID: 5472 RVA: 0x000694B2 File Offset: 0x000676B2
	private void Awake()
	{
		this._spline = base.GetComponent<Spline>();
	}

	// Token: 0x06001561 RID: 5473 RVA: 0x000694C0 File Offset: 0x000676C0
	private void LateUpdate()
	{
		if (this._particleSystem == null)
		{
			return;
		}
		if (this._particles == null)
		{
			this._particles = new ParticleSystem.Particle[this._particleSystem.main.maxParticles];
		}
		int particles = this._particleSystem.GetParticles(this._particles);
		for (int i = 0; i < particles; i++)
		{
			float num = Mathf.Pow(10f, (float)this._particles[i].randomSeed.ToString().Length);
			float num2 = this._particles[i].randomSeed / num;
			float num3 = 1f - this._particles[i].remainingLifetime / this._particles[i].startLifetime;
			if (!(this._spline.GetDirection(num3, false) == Vector3.zero))
			{
				Vector3 vector = Quaternion.AngleAxis(1080f * num2, -this._spline.GetDirection(num3, false)) * this._spline.Up(num3, true);
				Vector3 vector2 = Quaternion.AngleAxis(1080f * num2, -this._spline.GetDirection(num3 - 0.01f, false)) * this._spline.Up(num3 - 0.01f, false);
				Vector3 position = this._spline.GetPosition(num3, false);
				Vector3 vector3 = position;
				if (num3 - 0.01f >= 0f)
				{
					vector3 = this._spline.GetPosition(num3 - 0.01f, false);
				}
				float num4 = Mathf.Lerp(this.startRadius, this.endRadius, num3);
				float num5 = Mathf.Lerp(this.startRadius, this.endRadius, num3 - 0.01f);
				Vector3 vector4 = Vector3.zero;
				Vector3 vector5 = Vector3.zero;
				ParticleSystemSimulationSpace simulationSpace = this._particleSystem.main.simulationSpace;
				if (simulationSpace != ParticleSystemSimulationSpace.Local)
				{
					if (simulationSpace - ParticleSystemSimulationSpace.World <= 1)
					{
						vector4 = position + vector * num4;
						vector5 = position + vector2 * num5;
					}
				}
				else
				{
					vector4 = this._particleSystem.transform.InverseTransformPoint(position + vector * num4);
					vector5 = this._particleSystem.transform.InverseTransformPoint(vector3 + vector2 * num5);
				}
				this._particles[i].position = vector4;
				this._particles[i].velocity = vector4 - vector5;
			}
		}
		this._particleSystem.SetParticles(this._particles, this._particles.Length);
	}

	// Token: 0x040012B3 RID: 4787
	public float startRadius;

	// Token: 0x040012B4 RID: 4788
	public float endRadius;

	// Token: 0x040012B5 RID: 4789
	[SerializeField]
	private ParticleSystem _particleSystem;

	// Token: 0x040012B6 RID: 4790
	private Spline _spline;

	// Token: 0x040012B7 RID: 4791
	private ParticleSystem.Particle[] _particles;

	// Token: 0x040012B8 RID: 4792
	private const float _previousDiff = 0.01f;
}
