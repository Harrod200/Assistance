using System;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000925 RID: 2341
	public interface IListPaneItem<T>
	{
		// Token: 0x06005974 RID: 22900
		void Initialize(T item);

		// Token: 0x06005975 RID: 22901
		void Refresh();
	}
}
