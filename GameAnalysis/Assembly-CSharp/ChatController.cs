using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000426 RID: 1062
public class ChatController : MonoBehaviour
{
	// Token: 0x06001624 RID: 5668 RVA: 0x00070971 File Offset: 0x0006EB71
	private void OnEnable()
	{
		this.TMP_ChatInput.onSubmit.AddListener(new UnityAction<string>(this.AddToChatOutput));
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x0007098F File Offset: 0x0006EB8F
	private void OnDisable()
	{
		this.TMP_ChatInput.onSubmit.RemoveListener(new UnityAction<string>(this.AddToChatOutput));
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x000709B0 File Offset: 0x0006EBB0
	private void AddToChatOutput(string newText)
	{
		this.TMP_ChatInput.text = string.Empty;
		DateTime now = DateTime.Now;
		TMP_Text tmp_ChatOutput = this.TMP_ChatOutput;
		tmp_ChatOutput.text = string.Concat(new string[]
		{
			tmp_ChatOutput.text,
			"[<#FFFF80>",
			now.Hour.ToString("d2"),
			":",
			now.Minute.ToString("d2"),
			":",
			now.Second.ToString("d2"),
			"</color>] ",
			newText,
			"\n"
		});
		this.TMP_ChatInput.ActivateInputField();
		this.ChatScrollbar.value = 0f;
	}

	// Token: 0x04001434 RID: 5172
	public TMP_InputField TMP_ChatInput;

	// Token: 0x04001435 RID: 5173
	public TMP_Text TMP_ChatOutput;

	// Token: 0x04001436 RID: 5174
	public Scrollbar ChatScrollbar;
}
