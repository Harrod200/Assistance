using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000419 RID: 1049
public class LoadSaveButton : MonoBehaviour
{
	// Token: 0x06001565 RID: 5477 RVA: 0x00069850 File Offset: 0x00067A50
	public void SetSaveFileInfo(SaveFile saveFileEntry)
	{
		this.nameLabel.text = saveFileEntry.name;
		this.dateLabel.text = saveFileEntry.dateTime.ToShortDateString();
		this.timeLabel.text = saveFileEntry.dateTime.ToLongTimeString();
		this.saveInfo = saveFileEntry;
	}

	// Token: 0x040012BC RID: 4796
	public Button button;

	// Token: 0x040012BD RID: 4797
	public TMP_Text nameLabel;

	// Token: 0x040012BE RID: 4798
	public TMP_Text dateLabel;

	// Token: 0x040012BF RID: 4799
	public TMP_Text timeLabel;

	// Token: 0x040012C0 RID: 4800
	public SaveFile saveInfo;
}
