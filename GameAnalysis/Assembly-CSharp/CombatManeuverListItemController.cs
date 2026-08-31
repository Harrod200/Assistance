using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000438 RID: 1080
public class CombatManeuverListItemController : MonoBehaviour
{
	// Token: 0x0600166A RID: 5738 RVA: 0x00072773 File Offset: 0x00070973
	public void SetListItem(Sprite icon)
	{
		this.maneuverIcon.sprite = icon;
	}

	// Token: 0x040014D2 RID: 5330
	public Image maneuverIcon;
}
