using System;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x02000975 RID: 2421
	public abstract class BaseComponent : IComponent
	{
		// Token: 0x17000FC0 RID: 4032
		// (get) Token: 0x06005C2F RID: 23599 RVA: 0x002C0682 File Offset: 0x002BE882
		// (set) Token: 0x06005C30 RID: 23600 RVA: 0x002C068A File Offset: 0x002BE88A
		public ComponentMap map { get; private set; }

		// Token: 0x06005C31 RID: 23601 RVA: 0x002C0693 File Offset: 0x002BE893
		protected BaseComponent()
			: this(ComponentMap.single)
		{
		}

		// Token: 0x06005C32 RID: 23602 RVA: 0x002C06A0 File Offset: 0x002BE8A0
		protected BaseComponent(ComponentMap map)
		{
			this.map = map;
		}
	}
}
