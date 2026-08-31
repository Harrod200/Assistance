using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000807 RID: 2055
	public class NewGameOptionController : MonoBehaviour
	{
		// Token: 0x06004A70 RID: 19056 RVA: 0x001F37E0 File Offset: 0x001F19E0
		public void InitWithMetaTemplateCategory(string category)
		{
			base.gameObject.name = category;
			this.optionName.text = Loc.T(new StringBuilder("UI.StartScreen.").Append(category).ToString());
			this.templateOptions.Clear();
			foreach (TIMetaTemplate timetaTemplate in TemplateManager.IterateByClass<TIMetaTemplate>(true))
			{
				if (timetaTemplate.isNewCampaignOption && timetaTemplate.newCampaignOptionCategory == category && (timetaTemplate.listPriority != 999 || Application.isEditor) && ((!(timetaTemplate.dataName == "BrokenEarthScenario") && !(timetaTemplate.dataName == "2003Scenario")) || GameControl.DLCValidated))
				{
					this.templateOptions.Add(timetaTemplate);
				}
			}
			this.optionDropdown.options.Clear();
			if (Application.isEditor && category == "SolarSystem")
			{
				this.templateOptions = this.templateOptions.OrderByDescending<TIMetaTemplate, int>((TIMetaTemplate x) => x.listPriority).ToList<TIMetaTemplate>();
			}
			else
			{
				this.templateOptions = this.templateOptions.OrderBy<TIMetaTemplate, int>((TIMetaTemplate x) => x.listPriority).ToList<TIMetaTemplate>();
			}
			this.optionDropdown.captionText.text = this.templateOptions[0].displayNameCurrentForStartScreen();
			foreach (TIMetaTemplate timetaTemplate2 in this.templateOptions)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
				optionData.text = timetaTemplate2.displayNameCurrentForStartScreen();
				this.optionDropdown.options.Add(optionData);
			}
			if (!Application.isEditor && category == "SolarSystem")
			{
				this.optionDropdown.SetValueWithoutNotify(Mathf.Min(2, this.optionDropdown.options.Count));
			}
			this.OnDropdownOptionSelected();
		}

		// Token: 0x06004A71 RID: 19057 RVA: 0x001F3A10 File Offset: 0x001F1C10
		public void OnDropdownOptionSelected()
		{
			this.controller.UpdateStartOptions(base.gameObject.name, this.templateOptions[this.optionDropdown.value]);
		}

		// Token: 0x04002B6C RID: 11116
		public TMP_Text optionName;

		// Token: 0x04002B6D RID: 11117
		public TMP_Dropdown optionDropdown;

		// Token: 0x04002B6E RID: 11118
		public List<TIMetaTemplate> templateOptions = new List<TIMetaTemplate>();

		// Token: 0x04002B6F RID: 11119
		public StartMenuController controller;
	}
}
