using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.UI;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087E RID: 2174
	public class IntelHabSiteListPane : ListPane<TIHabSiteState>
	{
		// Token: 0x0600513D RID: 20797 RVA: 0x00238691 File Offset: 0x00236891
		protected override IEnumerable<TIHabSiteState> ItemsToDisplay()
		{
			return GameControl.control.activePlayer.ProspectedSpaceBodies().SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites);
		}

		// Token: 0x0600513E RID: 20798 RVA: 0x002386C8 File Offset: 0x002368C8
		public void CreateList()
		{
			this.habSiteItem.Clear();
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				this.habSiteItem.Add(base.gameObject.transform.GetChild(i).gameObject);
			}
		}

		// Token: 0x04003555 RID: 13653
		public IntelScreenController intelController;

		// Token: 0x04003556 RID: 13654
		public List<GameObject> habSiteItem;
	}
}
