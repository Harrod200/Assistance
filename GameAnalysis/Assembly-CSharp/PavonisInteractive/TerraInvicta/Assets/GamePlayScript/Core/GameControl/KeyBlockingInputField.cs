using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Assets.GamePlayScript.Core.GameControl
{
	// Token: 0x020009C4 RID: 2500
	[RequireComponent(typeof(TMP_InputField))]
	public class KeyBlockingInputField : MonoBehaviour
	{
		// Token: 0x17001032 RID: 4146
		// (get) Token: 0x06005E23 RID: 24099 RVA: 0x002CC1E3 File Offset: 0x002CA3E3
		public TMP_InputField InputField
		{
			get
			{
				return base.GetComponent<TMP_InputField>();
			}
		}

		// Token: 0x06005E24 RID: 24100 RVA: 0x002CC1EC File Offset: 0x002CA3EC
		private void Awake()
		{
			this.InputField.onSelect.AddListener(delegate(string eventString)
			{
				TIInputManager.BlockKeybindings();
			});
			this.InputField.onDeselect.AddListener(delegate(string eventString)
			{
				TIInputManager.RestoreKeybindings();
			});
		}
	}
}
