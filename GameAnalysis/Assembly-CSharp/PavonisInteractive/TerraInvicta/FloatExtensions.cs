using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F5 RID: 2037
	public static class FloatExtensions
	{
		// Token: 0x060049F8 RID: 18936 RVA: 0x001F0F33 File Offset: 0x001EF133
		public static string ToPercent(this float inputFloat, string args)
		{
			return FloatExtensions.HandleLocExceptionsPercent(inputFloat.ToString(args));
		}

		// Token: 0x060049F9 RID: 18937 RVA: 0x001F0F42 File Offset: 0x001EF142
		public static string ToPercent(this int inputInt, string args)
		{
			return FloatExtensions.HandleLocExceptionsPercent(inputInt.ToString(args));
		}

		// Token: 0x060049FA RID: 18938 RVA: 0x001F0F51 File Offset: 0x001EF151
		public static string ToPercent(this double inputDouble, string args)
		{
			return FloatExtensions.HandleLocExceptionsPercent(inputDouble.ToString(args));
		}

		// Token: 0x060049FB RID: 18939 RVA: 0x001F0F60 File Offset: 0x001EF160
		private static string HandleLocExceptionsPercent(string inputString)
		{
			string text = inputString;
			if (Loc.CurrentLanguage == "fr")
			{
				text = text.Replace(" ", "\u00a0");
			}
			SystemLanguage systemLanguage = Application.systemLanguage;
			if (systemLanguage <= SystemLanguage.French)
			{
				if (systemLanguage != SystemLanguage.English)
				{
					if (systemLanguage == SystemLanguage.French)
					{
						string text2 = "[\\u066A]";
						string text3 = "%";
						text = new Regex(text2).Replace(text, text3);
						text = text.Replace(" ", "\u00a0");
					}
				}
				else if (Loc.CurrentLanguage != "fr")
				{
					text = text.Replace(" ", "");
				}
			}
			else if (systemLanguage != SystemLanguage.Norwegian)
			{
				if (systemLanguage == SystemLanguage.Swedish)
				{
					text = text.Replace(" ", "");
				}
			}
			else
			{
				string text4 = "[\\u066A]";
				string text3 = "%";
				text = new Regex(text4).Replace(text, text3);
			}
			return text;
		}
	}
}
