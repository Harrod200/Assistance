using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007BC RID: 1980
	public abstract class TISpaceGameState : TIGameState
	{
		// Token: 0x17000D24 RID: 3364
		// (get) Token: 0x06004498 RID: 17560 RVA: 0x001C0CD8 File Offset: 0x001BEED8
		// (set) Token: 0x06004499 RID: 17561 RVA: 0x001C0CE0 File Offset: 0x001BEEE0
		public virtual TINaturalSpaceObjectState barycenter { get; set; }

		// Token: 0x17000D25 RID: 3365
		// (get) Token: 0x0600449A RID: 17562 RVA: 0x001C0CE9 File Offset: 0x001BEEE9
		public override bool isSpaceGameState
		{
			get
			{
				return true;
			}
		}
	}
}
