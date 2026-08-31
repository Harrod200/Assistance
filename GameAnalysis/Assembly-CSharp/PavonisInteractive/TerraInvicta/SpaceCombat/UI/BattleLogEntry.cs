using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A04 RID: 2564
	public class BattleLogEntry : MonoBehaviour
	{
		// Token: 0x060062BC RID: 25276 RVA: 0x002E7C9C File Offset: 0x002E5E9C
		public void Init(BattleLogEntry.BattleLogEntry_Data data)
		{
			this.timeStampText.SetText(data.timeStampText);
			this.battleLogText.SetText(data.battleLogText);
			this.battleLogTypeImage.sprite = GameControl.assetLoader.LoadAsset<Sprite>(data.imageTypeName);
			this.kiaIcon.enabled = data.enableKIAIcon;
			this.logType = data.logType;
		}

		// Token: 0x060062BD RID: 25277 RVA: 0x002E7D03 File Offset: 0x002E5F03
		public BattleLogController.BattleLogType GetLogType()
		{
			return this.logType;
		}

		// Token: 0x0400459D RID: 17821
		public TMP_Text timeStampText;

		// Token: 0x0400459E RID: 17822
		public TMP_Text battleLogText;

		// Token: 0x0400459F RID: 17823
		public Image battleLogTypeImage;

		// Token: 0x040045A0 RID: 17824
		public Image kiaIcon;

		// Token: 0x040045A1 RID: 17825
		private BattleLogController.BattleLogType logType;

		// Token: 0x0200139F RID: 5023
		public class BattleLogEntry_Data
		{
			// Token: 0x04007242 RID: 29250
			public int timeStampSeconds;

			// Token: 0x04007243 RID: 29251
			public string timeStampText;

			// Token: 0x04007244 RID: 29252
			public string battleLogText;

			// Token: 0x04007245 RID: 29253
			public string imageTypeName;

			// Token: 0x04007246 RID: 29254
			public bool enableKIAIcon;

			// Token: 0x04007247 RID: 29255
			public BattleLogController.BattleLogType logType;

			// Token: 0x04007248 RID: 29256
			public bool showInList;
		}
	}
}
