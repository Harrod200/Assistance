using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AC RID: 1452
	public class CasabaSizeByShape : MonoBehaviour
	{
		// Token: 0x06002775 RID: 10101 RVA: 0x000D7ECC File Offset: 0x000D60CC
		public void SetEffectRange(float range_km, float angle_deg)
		{
			ParticleSystem.ShapeModule shape = this.mainSystem.shape;
			ParticleSystem.ShapeModule shape2 = this.fireSystem.shape;
			ParticleSystem.ShapeModule shape3 = this.backwardSystem.shape;
			float num = shape.angle / shape2.angle;
			float num2 = shape3.angle / shape2.angle;
			shape.angle = angle_deg * num;
			shape2.angle = angle_deg;
			shape3.angle = angle_deg * num2;
			ParticleSystem.MainModule main = this.mainSystem.main;
			ParticleSystem.MainModule main2 = this.fireSystem.main;
			ParticleSystem.MainModule main3 = this.backwardSystem.main;
			ParticleSystem.MinMaxCurve startLifetime = this.mainSystem.main.startLifetime;
			ParticleSystem.MinMaxCurve startSpeed = this.mainSystem.main.startSpeed;
			ParticleSystem.MinMaxCurve startLifetime2 = this.fireSystem.main.startLifetime;
			ParticleSystem.MinMaxCurve startSpeed2 = this.fireSystem.main.startSpeed;
			ParticleSystem.MinMaxCurve startSpeed3 = this.backwardSystem.main.startSpeed;
			float num3 = startSpeed.constantMin / startSpeed.constantMax;
			float num4 = startSpeed3.constantMax / startSpeed.constantMax;
			float num5 = Mathf.Max(startSpeed.constantMax / 2f, range_km / startLifetime.constantMax);
			float num6 = Mathf.Max(startSpeed.constantMin / 2f, num3 * num5);
			float num7 = num5 / Mathf.Cos(shape.angle * 0.017453292f);
			float num8 = num6 / Mathf.Cos(shape.angle * 0.017453292f);
			main.startSpeed = new ParticleSystem.MinMaxCurve(num8, num7);
			float num9 = startSpeed2.constantMin / startSpeed2.constantMax;
			num5 = Mathf.Max(startSpeed2.constantMax / 2f, range_km / startLifetime2.constantMax);
			float num10 = Mathf.Max(startSpeed2.constantMin / 2f, num9 * num5);
			num7 = num5 / Mathf.Cos(shape2.angle * 0.017453292f);
			num8 = num10 / Mathf.Cos(shape2.angle * 0.017453292f);
			main2.startSpeed = new ParticleSystem.MinMaxCurve(num8, num7);
			float num11 = startSpeed3.constantMin / startSpeed3.constantMax;
			num5 = Mathf.Max(startSpeed3.constantMax / 2f, num4 * main.startSpeed.constantMax);
			float num12 = Mathf.Max(startSpeed3.constantMin / 2f, num11 * num5);
			num7 = num5 / Mathf.Cos(shape3.angle * 0.017453292f);
			num8 = num12 / Mathf.Cos(shape3.angle * 0.017453292f);
			main3.startSpeed = new ParticleSystem.MinMaxCurve(num8, num7);
		}

		// Token: 0x04001D5B RID: 7515
		[SerializeField]
		private ParticleSystem mainSystem;

		// Token: 0x04001D5C RID: 7516
		[SerializeField]
		private ParticleSystem fireSystem;

		// Token: 0x04001D5D RID: 7517
		[SerializeField]
		private ParticleSystem backwardSystem;
	}
}
