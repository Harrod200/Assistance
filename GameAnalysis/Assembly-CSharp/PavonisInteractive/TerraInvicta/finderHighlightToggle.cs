using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000865 RID: 2149
	public class finderHighlightToggle : MonoBehaviour
	{
		// Token: 0x06004FAD RID: 20397 RVA: 0x00226B05 File Offset: 0x00224D05
		private void Update()
		{
			if (this.highlightTimer > 0f)
			{
				this.highlightTimer -= Time.deltaTime;
				if (this.highlightTimer <= 0f)
				{
					base.GetComponent<FinderListItemController>().ForceHighlight(false);
				}
			}
		}

		// Token: 0x0400331A RID: 13082
		public float highlightTimer;
	}
}
