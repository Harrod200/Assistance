using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000903 RID: 2307
	public class RectTransformChangeListener : LayoutElement
	{
		// Token: 0x06005857 RID: 22615 RVA: 0x002883EC File Offset: 0x002865EC
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			UnityEvent onDimensionsChanged = this.OnDimensionsChanged;
			if (onDimensionsChanged == null)
			{
				return;
			}
			onDimensionsChanged.Invoke();
		}

		// Token: 0x0400400E RID: 16398
		public UnityEvent OnDimensionsChanged;
	}
}
