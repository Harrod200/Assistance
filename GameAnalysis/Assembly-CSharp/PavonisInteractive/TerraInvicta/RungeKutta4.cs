using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B2 RID: 1458
	public static class RungeKutta4
	{
		// Token: 0x0600278C RID: 10124 RVA: 0x000D8420 File Offset: 0x000D6620
		public static Matrix rk4(double t, Matrix x, Matrix u, double mu, double dt, RungeKutta4.VectorRkDelegate f)
		{
			double num = 0.5 * dt;
			Matrix matrix = f(t, x, u, mu);
			Matrix matrix2 = f(t + num, x + matrix * num, u, mu);
			Matrix matrix3 = f(t + num, x + matrix2 * num, u, mu);
			Matrix matrix4 = f(t + dt, x + matrix3 * dt, u, mu);
			return x + (matrix + matrix2 * 2.0 + matrix3 * 2.0 + matrix4) * (dt * RungeKutta4.sixth);
		}

		// Token: 0x04001D68 RID: 7528
		private static double sixth = 0.16666666666666666;

		// Token: 0x02000D08 RID: 3336
		// (Invoke) Token: 0x06006ED4 RID: 28372
		public delegate Matrix VectorRkDelegate(double t, Matrix x, Matrix u, double mu);
	}
}
