using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200043B RID: 1083
public class ChangeDropdownValue : MonoBehaviour
{
	// Token: 0x06001671 RID: 5745 RVA: 0x00072C64 File Offset: 0x00070E64
	public void NextOption()
	{
		int num = this.dropdown.value;
		num++;
		if (num == this.dropdown.options.Count)
		{
			num = 0;
		}
		this.dropdown.value = num;
	}

	// Token: 0x06001672 RID: 5746 RVA: 0x00072CA4 File Offset: 0x00070EA4
	public void LastOption()
	{
		int num = this.dropdown.value;
		num--;
		if (num < 0)
		{
			num = this.dropdown.options.Count - 1;
		}
		this.dropdown.value = num;
	}

	// Token: 0x040014D3 RID: 5331
	public Dropdown dropdown;
}
