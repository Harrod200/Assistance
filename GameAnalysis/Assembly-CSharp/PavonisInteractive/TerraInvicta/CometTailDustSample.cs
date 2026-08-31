using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000591 RID: 1425
	public class CometTailDustSample : CometTailSample
	{
		// Token: 0x1700054A RID: 1354
		// (get) Token: 0x060025EB RID: 9707 RVA: 0x000CDB6F File Offset: 0x000CBD6F
		public override Color Color
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x1700054B RID: 1355
		// (get) Token: 0x060025EC RID: 9708 RVA: 0x000CDB76 File Offset: 0x000CBD76
		// (set) Token: 0x060025ED RID: 9709 RVA: 0x000CDB7E File Offset: 0x000CBD7E
		public double GrainDiameter_mm { get; private set; }

		// Token: 0x060025EE RID: 9710 RVA: 0x000CDB87 File Offset: 0x000CBD87
		public CometTailDustSample(TIDateTime date, Vector3d position, Vector3d velocity, double radius_m, double opacity, double expansionVelocity_mps, double grainDiameter_mm = 0.001)
			: base(date, position, velocity, radius_m, opacity, expansionVelocity_mps)
		{
			this.GrainDiameter_mm = grainDiameter_mm;
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x000CDBA0 File Offset: 0x000CBDA0
		public override Vector3d GetAccelerationVector_mps2(Vector3d position_m)
		{
			double num = position_m.magnitude / GameStateManager.Earth().semiMajorAxis_m;
			double num2 = 1.0 / Mathd.Pow(num, 2.0) * 1361.0 / 299792458.0;
			return position_m.normalized * num2;
		}
	}
}
