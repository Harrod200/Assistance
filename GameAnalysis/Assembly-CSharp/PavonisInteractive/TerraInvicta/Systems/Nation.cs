using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x0200098D RID: 2445
	[Serializable]
	public struct Nation : IComponentData
	{
		// Token: 0x17000FEC RID: 4076
		// (get) Token: 0x06005CD5 RID: 23765 RVA: 0x002C3B06 File Offset: 0x002C1D06
		public TINationState State
		{
			get
			{
				return this.ID.GetState<TINationState>(false);
			}
		}

		// Token: 0x0400422C RID: 16940
		public GameStateID ID;
	}
}
