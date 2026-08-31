using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModelShark
{
	// Token: 0x020004B5 RID: 1205
	public static class ExtensionMethods
	{
		// Token: 0x06001B0E RID: 6926 RVA: 0x00092E0C File Offset: 0x0009100C
		public static void FillParameterizedTextFields(this TMP_Text[] textFields, ref List<ParameterizedTextField> parameterizedTextFields, string delimiter)
		{
			List<string> fieldNames = new List<string>();
			foreach (TMP_Text tmp_Text in textFields)
			{
				string text = string.Format("{0}\\w*{0}", delimiter);
				foreach (object obj in Regex.Matches(tmp_Text.text, text, RegexOptions.IgnoreCase | RegexOptions.Multiline))
				{
					Match match = (Match)obj;
					string text2 = match.Value.Trim(new char[] { '%' });
					if (!fieldNames.Contains(text2))
					{
						fieldNames.Add(text2);
					}
					bool flag = false;
					foreach (ParameterizedTextField parameterizedTextField in parameterizedTextFields)
					{
						if (text2 == parameterizedTextField.name)
						{
							parameterizedTextField.placeholder = match.Value;
							flag = true;
						}
					}
					if (!flag)
					{
						parameterizedTextFields.Add(new ParameterizedTextField
						{
							name = text2,
							placeholder = match.Value,
							value = string.Empty
						});
					}
				}
			}
			parameterizedTextFields.RemoveAll((ParameterizedTextField x) => !fieldNames.Contains(x.name));
		}

		// Token: 0x06001B0F RID: 6927 RVA: 0x00092F80 File Offset: 0x00091180
		public static void FillDynamicImageFields(this DynamicImage[] imageFields, ref List<DynamicImageField> dynamicImageFields, string delimiter)
		{
			List<string> fieldNames = new List<string>();
			foreach (DynamicImage dynamicImage in imageFields)
			{
				string text = dynamicImage.placeholderName.Trim(new char[] { '%' });
				if (!fieldNames.Contains(text))
				{
					fieldNames.Add(text);
				}
				Image placeholderImage = dynamicImage.PlaceholderImage;
				bool flag = false;
				foreach (DynamicImageField dynamicImageField in dynamicImageFields)
				{
					if (text == dynamicImageField.name)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					dynamicImageFields.Add(new DynamicImageField
					{
						name = text,
						placeholderSprite = placeholderImage.sprite,
						replacementSprite = null
					});
				}
			}
			dynamicImageFields.RemoveAll((DynamicImageField x) => !fieldNames.Contains(x.name));
		}

		// Token: 0x06001B10 RID: 6928 RVA: 0x00093088 File Offset: 0x00091288
		public static void FillDynamicSectionFields(this DynamicSection[] sectionFields, ref List<DynamicSectionField> dynamicSectionFields, string delimiter)
		{
			List<string> fieldNames = new List<string>();
			foreach (DynamicSection dynamicSection in sectionFields)
			{
				string text = dynamicSection.placeholderName.Trim(new char[] { '%' });
				if (!fieldNames.Contains(text))
				{
					fieldNames.Add(text);
				}
				GameObject gameObject = dynamicSection.gameObject;
				bool flag = false;
				foreach (DynamicSectionField dynamicSectionField in dynamicSectionFields)
				{
					if (text == dynamicSectionField.name)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					dynamicSectionFields.Add(new DynamicSectionField
					{
						name = text,
						isOn = gameObject.activeSelf
					});
				}
			}
			dynamicSectionFields.RemoveAll((DynamicSectionField x) => !fieldNames.Contains(x.name));
		}
	}
}
