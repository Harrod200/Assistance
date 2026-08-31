using System;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta.Systems
{
	// Token: 0x0200098A RID: 2442
	[Serializable]
	public struct Faction : IComponentData
	{
		// Token: 0x17000FEB RID: 4075
		// (get) Token: 0x06005CD4 RID: 23764 RVA: 0x002C3AF8 File Offset: 0x002C1CF8
		public TIFactionState State
		{
			get
			{
				return this.ID.GetState<TIFactionState>(false);
			}
		}

		// Token: 0x0400422B RID: 16939
		public GameStateID ID;
	}
}
