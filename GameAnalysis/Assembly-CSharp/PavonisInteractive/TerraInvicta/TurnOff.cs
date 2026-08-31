using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000800 RID: 2048
	public class TurnOff : MonoBehaviour
	{
		// Token: 0x06004A48 RID: 19016 RVA: 0x001F2708 File Offset: 0x001F0908
		private void Awake()
		{
			base.gameObject.SetActive(false);
		}
	}
}
