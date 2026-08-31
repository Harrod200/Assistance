using System;
using TMPro;
using UnityEngine;

// Token: 0x02000418 RID: 1048
public class AutoscrollingText : MonoBehaviour
{
	// Token: 0x06001563 RID: 5475 RVA: 0x00069778 File Offset: 0x00067978
	private void Update()
	{
		RectTransform rectTransform = base.transform as RectTransform;
		RectTransform rectTransform2 = base.transform.parent as RectTransform;
		if (!this.associatedMenu.IsOpen || base.transform.localPosition.y > rectTransform.rect.height + 1.2f * rectTransform2.rect.height / 2f)
		{
			base.transform.localPosition = new Vector3(0f, -1.2f * rectTransform2.rect.height / 2f);
			return;
		}
		base.transform.localPosition += new Vector3(0f, Time.deltaTime * this.scrollSpeed);
	}

	// Token: 0x040012B9 RID: 4793
	public TMP_Text textToScroll;

	// Token: 0x040012BA RID: 4794
	public float scrollSpeed;

	// Token: 0x040012BB RID: 4795
	public Menu associatedMenu;
}
