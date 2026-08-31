using System;
using PavonisInteractive.TerraInvicta;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000445 RID: 1093
public class UITextLocalizer : MonoBehaviour
{
	// Token: 0x060016D5 RID: 5845 RVA: 0x000754C9 File Offset: 0x000736C9
	private void Start()
	{
		this.LocalizeText(this.displayText);
		Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x000754E8 File Offset: 0x000736E8
	public void LocalizeText(string locString)
	{
		TextMeshProUGUI textMeshProUGUI;
		if (base.TryGetComponent<TextMeshProUGUI>(out textMeshProUGUI))
		{
			textMeshProUGUI.text = Loc.T(locString);
		}
		Text text;
		if (base.TryGetComponent<Text>(out text))
		{
			text.text = Loc.T(locString);
		}
	}

	// Token: 0x060016D7 RID: 5847 RVA: 0x00075521 File Offset: 0x00073721
	private void OnLanguageChangedEvent()
	{
		this.LocalizeText(this.displayText);
		Loc.SwapFonts(base.gameObject);
	}

	// Token: 0x060016D8 RID: 5848 RVA: 0x0007553A File Offset: 0x0007373A
	private void OnDestroy()
	{
		Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
	}

	// Token: 0x0400154A RID: 5450
	[Tooltip("This is the localization text key path")]
	public string displayText;

	// Token: 0x0400154B RID: 5451
	[Tooltip("Only needed for the main menu")]
	public bool updateOnLanguageChange;
}
