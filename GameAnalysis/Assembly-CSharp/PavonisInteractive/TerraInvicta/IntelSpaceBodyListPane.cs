using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.UI;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000884 RID: 2180
	public class IntelSpaceBodyListPane : ListPane<TISpaceBodyState>
	{
		// Token: 0x0600519A RID: 20890 RVA: 0x0023E365 File Offset: 0x0023C565
		protected override IEnumerable<TISpaceBodyState> ItemsToDisplay()
		{
			return (from n in GameStateManager.AllSpaceBodies()
				where n.objectType != SpaceObjectType.Star
				select n).ToList<TISpaceBodyState>();
		}

		// Token: 0x0600519B RID: 20891 RVA: 0x0023E398 File Offset: 0x0023C598
		public void CreateList()
		{
			this.spacebodyItem.Clear();
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				this.spacebodyItem.Add(base.gameObject.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x04003624 RID: 13860
		public IntelScreenController intelController;

		// Token: 0x04003625 RID: 13861
		public List<GameObject> spacebodyItem;
	}
}
