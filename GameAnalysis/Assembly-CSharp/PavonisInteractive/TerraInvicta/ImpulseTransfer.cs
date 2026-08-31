using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200078F RID: 1935
	public abstract class ImpulseTransfer : TrajectorySolver
	{
		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x06003DBA RID: 15802 RVA: 0x00184235 File Offset: 0x00182435
		public OrbitalElementsState transferOrbit
		{
			get
			{
				return this._transferOrbit;
			}
		}

		// Token: 0x040026A4 RID: 9892
		protected OrbitalElementsState _transferOrbit;
	}
}
