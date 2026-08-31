using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000878 RID: 2168
	public class IntelAlienHabListItemController : MonoBehaviour
	{
		// Token: 0x06005111 RID: 20753 RVA: 0x00237048 File Offset: 0x00235248
		public void UpdateListItem(TIHabState hab)
		{
			this.hab = hab;
			this.habName.SetText(Loc.T("UI.Intel.AlienHabDescription", new object[] { hab.displayName, hab.description }));
			this.habLocation.SetText(hab.LocationName);
		}

		// Token: 0x06005112 RID: 20754 RVA: 0x0023709A File Offset: 0x0023529A
		public void OnClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.hab, true, true, true, true, false, -1f);
		}

		// Token: 0x040034E4 RID: 13540
		public TMP_Text habName;

		// Token: 0x040034E5 RID: 13541
		public TMP_Text habLocation;

		// Token: 0x040034E6 RID: 13542
		private TIHabState hab;
	}
}
