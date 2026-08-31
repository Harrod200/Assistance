using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.UI
{
	// Token: 0x020009BF RID: 2495
	public interface ICanvas
	{
		// Token: 0x1700102C RID: 4140
		// (get) Token: 0x06005E0A RID: 24074
		GameObject GameObject { get; }

		// Token: 0x1700102D RID: 4141
		// (get) Token: 0x06005E0B RID: 24075
		Canvas Canvas { get; }

		// Token: 0x06005E0C RID: 24076
		void Initialize();

		// Token: 0x06005E0D RID: 24077
		void Show();

		// Token: 0x06005E0E RID: 24078
		void Hide();

		// Token: 0x06005E0F RID: 24079
		void HideNoCache();

		// Token: 0x06005E10 RID: 24080
		void Refresh();

		// Token: 0x06005E11 RID: 24081
		void RefreshScaling();

		// Token: 0x06005E12 RID: 24082
		void SetUltraWideScaling();

		// Token: 0x06005E13 RID: 24083
		bool Visible();
	}
}
