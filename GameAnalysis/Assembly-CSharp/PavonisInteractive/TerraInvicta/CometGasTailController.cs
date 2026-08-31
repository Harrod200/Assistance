using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000592 RID: 1426
	public class CometGasTailController : CometTailController<CometTailGasSample>
	{
		// Token: 0x060025F0 RID: 9712 RVA: 0x000CDBFC File Offset: 0x000CBDFC
		protected override CometTailGasSample CreateParticleSample(TIDateTime date)
		{
			Vector3d globalPositionAtTime = base.Comet.GetGlobalPositionAtTime(date);
			Vector3d velocityVectorAtTime = base.Comet.GetVelocityVectorAtTime(date);
			return new CometTailGasSample(date, globalPositionAtTime, velocityVectorAtTime, (double)base.NearParticleRadius_m, 1.0, (double)this.ExpansionVelocity_mps);
		}
	}
}
