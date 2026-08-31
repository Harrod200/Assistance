using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000444 RID: 1092
public class Keybind_UIMenuObject : MonoBehaviour
{
	// Token: 0x060016CE RID: 5838 RVA: 0x00075416 File Offset: 0x00073616
	private void Start()
	{
		if (!this.editable)
		{
			this.editButton.interactable = false;
			this.LoadLocalizedText();
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
		}
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x00075443 File Offset: 0x00073643
	public void LoadLocalizedText()
	{
		this.currentKeybindName.text = Loc.T(this.displayText);
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x0007545B File Offset: 0x0007365B
	public void ClickedNewKeybind()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		this.currentKeybindText.text = "...";
		this.waitingForNewKey = true;
		TIInputManager.waitingForKeybind = true;
		TIInputManager.currentRebind = this;
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x0007548C File Offset: 0x0007368C
	public void OnClickRemoveKeybind()
	{
		TIInputManager.RemoveKeyBind(this);
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x00075494 File Offset: 0x00073694
	private void OnLanguageChangedEvent()
	{
		this.LoadLocalizedText();
		Loc.SwapFonts(base.gameObject);
	}

	// Token: 0x060016D3 RID: 5843 RVA: 0x000754A7 File Offset: 0x000736A7
	private void OnDestroy()
	{
		Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
	}

	// Token: 0x04001543 RID: 5443
	[Tooltip("This is the localization text key path")]
	public string displayText;

	// Token: 0x04001544 RID: 5444
	public TextMeshProUGUI currentKeybindText;

	// Token: 0x04001545 RID: 5445
	public TextMeshProUGUI currentKeybindName;

	// Token: 0x04001546 RID: 5446
	public bool waitingForNewKey;

	// Token: 0x04001547 RID: 5447
	public int keybindIndex;

	// Token: 0x04001548 RID: 5448
	public bool editable = true;

	// Token: 0x04001549 RID: 5449
	public Button editButton;
}
