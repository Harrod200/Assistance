using System;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000879 RID: 2169
	public class IntelAtrocitiesGridItemController : MonoBehaviour
	{
		// Token: 0x17000ED1 RID: 3793
		// (get) Token: 0x06005114 RID: 20756 RVA: 0x002370C5 File Offset: 0x002352C5
		// (set) Token: 0x06005115 RID: 20757 RVA: 0x002370CD File Offset: 0x002352CD
		public TIFactionState faction { get; private set; }

		// Token: 0x06005116 RID: 20758 RVA: 0x002370D6 File Offset: 0x002352D6
		public void InitListItem(TIFactionState faction)
		{
			this.factionIcon.sprite = faction.factionIcon64UI;
			this.faction = faction;
			this.SetListItem(false);
		}

		// Token: 0x06005117 RID: 20759 RVA: 0x002370F8 File Offset: 0x002352F8
		public void SetListItem(bool fillerItem = false)
		{
			this.numAtrocities.SetText(this.faction.atrocities.ToString("N0"));
			this.tip.enabled = !fillerItem;
			this.tip.SetDelegate("BodyText", () => this.faction.AtrocityCauseTable());
			this.factionIcon.gameObject.SetActive(!fillerItem);
			this.numAtrocities.gameObject.SetActive(!fillerItem);
			this.backgroundImage.sprite = (fillerItem ? this.fillerBackground : this.defaultBackground);
			this.fillerLines.SetActive(fillerItem);
		}

		// Token: 0x040034E7 RID: 13543
		public Image factionIcon;

		// Token: 0x040034E8 RID: 13544
		public TMP_Text numAtrocities;

		// Token: 0x040034EA RID: 13546
		public GameObject fillerLines;

		// Token: 0x040034EB RID: 13547
		public Image backgroundImage;

		// Token: 0x040034EC RID: 13548
		public Sprite defaultBackground;

		// Token: 0x040034ED RID: 13549
		public Sprite fillerBackground;

		// Token: 0x040034EE RID: 13550
		public TooltipTrigger tip;
	}
}
