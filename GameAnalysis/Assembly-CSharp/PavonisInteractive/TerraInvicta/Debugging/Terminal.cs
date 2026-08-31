using System;
using System.Text;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x0200090C RID: 2316
	public class Terminal : IInitializable, IDisposable
	{
		// Token: 0x06005893 RID: 22675 RVA: 0x00289956 File Offset: 0x00287B56
		[global::Zenject.Inject]
		public Terminal(GameObject view, TerminalController controller)
		{
			this.view = view;
			this.controller = controller;
		}

		// Token: 0x06005894 RID: 22676 RVA: 0x00289973 File Offset: 0x00287B73
		public void Initialize()
		{
			this.CacheComponents();
			this.AddDelegates();
		}

		// Token: 0x06005895 RID: 22677 RVA: 0x00289984 File Offset: 0x00287B84
		private void CacheComponents()
		{
			this.stringBuilder = new StringBuilder();
			this.inputField = this.view.GetComponentInChildren<TMP_InputField>();
			this.enterButton = this.view.GetComponentInChildren<Button>();
			this.historyLog = this.view.GetComponentOnChild<TextMeshProUGUI>("Column 1");
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x06005896 RID: 22678 RVA: 0x002899E4 File Offset: 0x00287BE4
		private void AddDelegates()
		{
			this.enterButton.onClick.AddListener(new UnityAction(this.OnEnterClick));
			this.inputField.onSubmit.AddListener(new UnityAction<string>(this.SubmitInput));
			this.controller.OnOutput += this.AddToHistory;
			this.controller.OnOutputError += this.AddErrorToHistory;
		}

		// Token: 0x06005897 RID: 22679 RVA: 0x00289A57 File Offset: 0x00287C57
		private void OnEnterClick()
		{
			this.SubmitInput(this.inputField.text);
		}

		// Token: 0x06005898 RID: 22680 RVA: 0x00289A6A File Offset: 0x00287C6A
		private void SubmitInput(string txt)
		{
			this.AddToHistory(txt);
			this.controller.ParseCommand(txt);
			this.ClearInput();
		}

		// Token: 0x06005899 RID: 22681 RVA: 0x00289A85 File Offset: 0x00287C85
		private void AddToHistory(string txt)
		{
			this.stringBuilder.AppendLine(txt);
			this.historyLog.text = this.stringBuilder.ToString();
		}

		// Token: 0x0600589A RID: 22682 RVA: 0x00289AAC File Offset: 0x00287CAC
		private void AddErrorToHistory(string txt)
		{
			string text = "<color=\"red\">" + txt + "</color>";
			this.AddToHistory(text);
		}

		// Token: 0x0600589B RID: 22683 RVA: 0x00289AD1 File Offset: 0x00287CD1
		private void ClearInput()
		{
			this.inputField.text = "";
			this.inputField.caretPosition = 0;
			this.historyLineIndex = -1;
			this.historySplit = null;
			this.SetFocus();
		}

		// Token: 0x0600589C RID: 22684 RVA: 0x00289B04 File Offset: 0x00287D04
		public void Show()
		{
			this.view.SetActive(true);
			this.enterButton.enabled = true;
			this.inputField.enabled = true;
			this.historyLineIndex = -1;
			this.SetFocus();
			this.gameTime.PauseAndBlock();
			TIInputManager.BlockKeybindings();
			this.historyLog.SetText(string.Empty);
		}

		// Token: 0x0600589D RID: 22685 RVA: 0x00289B62 File Offset: 0x00287D62
		private void SetFocus()
		{
			this.inputField.OnPointerClick(new PointerEventData(EventSystem.current));
		}

		// Token: 0x0600589E RID: 22686 RVA: 0x00289B79 File Offset: 0x00287D79
		public void Hide()
		{
			this.inputField.enabled = false;
			this.enterButton.enabled = false;
			TIInputManager.RestoreKeybindings();
			this.view.SetActive(false);
			this.gameTime.UnBlock();
		}

		// Token: 0x0600589F RID: 22687 RVA: 0x00289BAF File Offset: 0x00287DAF
		public void PreviousCommand()
		{
			this.PullCommandFromHistory(-1);
		}

		// Token: 0x060058A0 RID: 22688 RVA: 0x00289BB8 File Offset: 0x00287DB8
		public void NextCommand()
		{
			this.PullCommandFromHistory(1);
		}

		// Token: 0x060058A1 RID: 22689 RVA: 0x00289BC4 File Offset: 0x00287DC4
		private void PullCommandFromHistory(int dir = 1)
		{
			if (this.historySplit == null)
			{
				this.historySplit = this.stringBuilder.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			}
			if (this.historySplit.Length == 0)
			{
				return;
			}
			if (this.historyLineIndex == -1)
			{
				this.historyLineIndex = this.historySplit.Length - 2;
			}
			else
			{
				this.historyLineIndex += dir;
				this.historyLineIndex = ((this.historyLineIndex < 0) ? 0 : ((this.historyLineIndex > this.historySplit.Length - 1) ? (this.historySplit.Length - 1) : this.historyLineIndex));
			}
			if (this.historyLineIndex < 0)
			{
				return;
			}
			if (this.historySplit[this.historyLineIndex].IndexOf("error", StringComparison.InvariantCultureIgnoreCase) == -1 && this.historySplit[this.historyLineIndex].IndexOf("unknown", StringComparison.InvariantCultureIgnoreCase) == -1)
			{
				this.inputField.text = this.historySplit[this.historyLineIndex];
				this.inputField.caretPosition = this.inputField.text.Length;
				return;
			}
			if ((dir == 1 && this.historyLineIndex == this.historySplit.Length - 1) || (dir == -1 && this.historyLineIndex == 0))
			{
				return;
			}
			this.PullCommandFromHistory(dir);
		}

		// Token: 0x060058A2 RID: 22690 RVA: 0x00289D05 File Offset: 0x00287F05
		public void Dispose()
		{
			this.RemoveDelegates();
			this.controller.Destroy();
			this.controller = null;
			this.inputField = null;
			this.historyLog = null;
			this.enterButton = null;
			this.stringBuilder = null;
			this.view = null;
		}

		// Token: 0x060058A3 RID: 22691 RVA: 0x00289D44 File Offset: 0x00287F44
		private void RemoveDelegates()
		{
			this.inputField.onSubmit.RemoveAllListeners();
			this.enterButton.onClick.RemoveAllListeners();
			this.controller.OnOutput -= this.AddToHistory;
			this.controller.OnOutputError -= this.AddErrorToHistory;
		}

		// Token: 0x0400405C RID: 16476
		private GameObject view;

		// Token: 0x0400405D RID: 16477
		private TMP_InputField inputField;

		// Token: 0x0400405E RID: 16478
		private Button enterButton;

		// Token: 0x0400405F RID: 16479
		private TextMeshProUGUI historyLog;

		// Token: 0x04004060 RID: 16480
		public TerminalController controller;

		// Token: 0x04004061 RID: 16481
		private StringBuilder stringBuilder;

		// Token: 0x04004062 RID: 16482
		private GameTimeManager gameTime;

		// Token: 0x04004063 RID: 16483
		private string[] historySplit;

		// Token: 0x04004064 RID: 16484
		private int historyLineIndex = -1;
	}
}
