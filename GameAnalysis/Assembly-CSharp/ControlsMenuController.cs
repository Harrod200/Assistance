using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

// Token: 0x02000442 RID: 1090
public class ControlsMenuController : MonoBehaviour
{
	// Token: 0x060016AF RID: 5807 RVA: 0x00074093 File Offset: 0x00072293
	private void Start()
	{
		this.RefreshKeybindings();
		this.LoadLocalizedText();
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x000740A4 File Offset: 0x000722A4
	public void RefreshKeybindings()
	{
		for (int i = 0; i < this.keybindList.Count; i++)
		{
			this.keybindList[i].currentKeybindText.text = TIUtilities.CombineStrings(new string[]
			{
				(TIInputManager.keyBindingModifiers[i] == KeyCode.None) ? "" : TIUtilities.CombineStrings(new string[]
				{
					TIInputManager.keyBindingModifiers[i].ToString(),
					"+"
				}),
				TIInputManager.GetKeybind(i)
			});
			this.keybindList[i].keybindIndex = i;
		}
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x00074150 File Offset: 0x00072350
	public void LoadLocalizedText()
	{
		this.keybindTitleText.text = Loc.T("UI.Options.KeybindTitle");
		this.restoreDefaultsText.text = Loc.T("UI.Options.KeybindRestoreDefault");
		foreach (Keybind_UIMenuObject keybind_UIMenuObject in this.keybindList)
		{
			keybind_UIMenuObject.LoadLocalizedText();
		}
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x000741CC File Offset: 0x000723CC
	public void OnClickRestoreDefaults()
	{
		TIInputManager.ResetKeybindsToDefault();
		this.RefreshKeybindings();
		TIPlayerProfileManager.SavePlayerConfig();
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
	}

	// Token: 0x04001507 RID: 5383
	public GameObject keybindGrid;

	// Token: 0x04001508 RID: 5384
	public List<Keybind_UIMenuObject> keybindList = new List<Keybind_UIMenuObject>();

	// Token: 0x04001509 RID: 5385
	public TextMeshProUGUI keybindTitleText;

	// Token: 0x0400150A RID: 5386
	public TextMeshProUGUI restoreDefaultsText;
}
