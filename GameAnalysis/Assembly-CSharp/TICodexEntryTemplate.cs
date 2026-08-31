using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002CA RID: 714
public class TICodexEntryTemplate : TIDataTemplate
{
	// Token: 0x17000161 RID: 353
	// (get) Token: 0x06000A6C RID: 2668 RVA: 0x00032B79 File Offset: 0x00030D79
	public string titleText
	{
		get
		{
			return Loc.T(new StringBuilder(this.locPath).Append(".Title").ToString());
		}
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x00032B9C File Offset: 0x00030D9C
	public string textParamToStrValue(int param)
	{
		if (param >= 0 && param < this.textParams.Count)
		{
			TIGlobalConfig global = TemplateManager.global;
			FieldInfo field = global.GetType().GetField(this.textParams[param]);
			if (field != null)
			{
				string text = "Missing";
				object value = field.GetValue(global);
				if (value is double)
				{
					double num = (double)value;
					text = TIUtilities.FormatBigOrSmallNumber(num, 1, 7, 0, false, false);
				}
				else if (value is float)
				{
					float num2 = (float)value;
					text = TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false);
				}
				else if (value is int)
				{
					int num3 = (int)value;
					text = TIUtilities.FormatBigOrSmallNumber((float)num3, 1, 7, 0, false, false);
				}
				return text;
			}
		}
		return "ERROR, bad param in codex config " + base.dataName;
	}

	// Token: 0x040008C2 RID: 2242
	public float index;

	// Token: 0x040008C3 RID: 2243
	public bool mainTopic;

	// Token: 0x040008C4 RID: 2244
	public string locPath;

	// Token: 0x040008C5 RID: 2245
	public string imgPath;

	// Token: 0x040008C6 RID: 2246
	public string illustrationPath;

	// Token: 0x040008C7 RID: 2247
	public string unlockTech;

	// Token: 0x040008C8 RID: 2248
	public string templateToPull;

	// Token: 0x040008C9 RID: 2249
	public List<string> textParams = new List<string>();
}
