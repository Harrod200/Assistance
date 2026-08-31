using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000593 RID: 1427
	public class CometTailGasSample : CometTailDustSample
	{
		// Token: 0x060025F2 RID: 9714 RVA: 0x000CDC4A File Offset: 0x000CBE4A
		public CometTailGasSample(TIDateTime date, Vector3d position, Vector3d velocity_mps, double radius_m, double opacity, double expansionVelocity_mps)
			: base(date, position, velocity_mps, radius_m, opacity, expansionVelocity_mps, 0.001)
		{
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x000CDC64 File Offset: 0x000CBE64
		public override void CalculatePositionAndVelocity(TIDateTime time, out Vector3d position_m, out Vector3d velocity_mps, int resolution = 10)
		{
			position_m = base.SpawnPosition;
			double num = 500000.0;
			velocity_mps = position_m.normalized * num;
			double num2 = (time - base.SpawnDate).TotalSeconds / (double)resolution;
			for (int i = 0; i < resolution; i++)
			{
				position_m += velocity_mps * num2;
				velocity_mps = position_m.normalized * num;
			}
		}
	}
}
