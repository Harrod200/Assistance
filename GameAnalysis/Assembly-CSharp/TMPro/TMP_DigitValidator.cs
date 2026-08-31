using System;

namespace TMPro
{
	// Token: 0x020004F9 RID: 1273
	[Serializable]
	public class TMP_DigitValidator : TMP_InputValidator
	{
		// Token: 0x06001F93 RID: 8083 RVA: 0x000A3983 File Offset: 0x000A1B83
		public override char Validate(ref string text, ref int pos, char ch)
		{
			if (ch >= '0' && ch <= '9')
			{
				pos++;
				return ch;
			}
			return '\0';
		}
	}
}
