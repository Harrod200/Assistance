using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008FF RID: 2303
	[RequireComponent(typeof(TMP_InputField))]
	public class InputFieldInputHandler : MonoBehaviour
	{
		// Token: 0x06005834 RID: 22580 RVA: 0x00287398 File Offset: 0x00285598
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				UnityEvent unityEvent = this.onSubmitEvent;
				if (unityEvent != null)
				{
					unityEvent.Invoke();
				}
				EventSystem.current.SetSelectedGameObject(null);
				TIInputManager.RestoreKeybindings();
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				UnityEvent unityEvent2 = this.onCancelEvent;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke();
				}
				EventSystem.current.SetSelectedGameObject(null);
				TIInputManager.RestoreKeybindings();
			}
		}

		// Token: 0x04003FC8 RID: 16328
		public UnityEvent onSubmitEvent;

		// Token: 0x04003FC9 RID: 16329
		public UnityEvent onCancelEvent;
	}
}
