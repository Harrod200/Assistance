using System;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000875 RID: 2165
	public class IntelAlienEarthAssetListItemController : MonoBehaviour
	{
		// Token: 0x06005108 RID: 20744 RVA: 0x00236DC4 File Offset: 0x00234FC4
		public void UpdateListItem(TIRegionAlienEntityState asset)
		{
			this.asset = asset;
			this.icon.sprite = asset.GetIcon(GameControl.control.activePlayer);
			this.assetName.SetText(asset.GetDisplayName(GameControl.control.activePlayer));
			this.regionName.SetText(Loc.T("UI.Global.2IC", new object[]
			{
				asset.region.displayName,
				asset.region.nation.displayName
			}));
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x00236E4A File Offset: 0x0023504A
		public void OnClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.asset, true, true, true, true, false, -1f);
		}

		// Token: 0x040034D9 RID: 13529
		public Image icon;

		// Token: 0x040034DA RID: 13530
		public TMP_Text assetName;

		// Token: 0x040034DB RID: 13531
		public TMP_Text regionName;

		// Token: 0x040034DC RID: 13532
		private TIRegionAlienEntityState asset;
	}
}
