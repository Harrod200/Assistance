using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000590 RID: 1424
	public class CometDustTailController : CometTailController<CometTailDustSample>
	{
		// Token: 0x060025E8 RID: 9704 RVA: 0x000CDAE5 File Offset: 0x000CBCE5
		public override void LateUpdate()
		{
			if (this.DoNotDisplay)
			{
				this.ParticleSystem.Clear();
				return;
			}
			this.BaseNearParticleRadius_m = this.CometController.ComaController.DustRadius_m;
			base.LateUpdate();
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x000CDB18 File Offset: 0x000CBD18
		protected override CometTailDustSample CreateParticleSample(TIDateTime date)
		{
			Vector3d globalPositionAtTime = base.Comet.GetGlobalPositionAtTime(date);
			Vector3d velocityVectorAtTime = base.Comet.GetVelocityVectorAtTime(date);
			return new CometTailDustSample(date, globalPositionAtTime, velocityVectorAtTime, (double)base.NearParticleRadius_m, 1.0, (double)this.ExpansionVelocity_mps, 0.001);
		}
	}
}
