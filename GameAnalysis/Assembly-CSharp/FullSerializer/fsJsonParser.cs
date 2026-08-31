using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FullSerializer
{
	// Token: 0x0200046C RID: 1132
	public class fsJsonParser
	{
		// Token: 0x060017E4 RID: 6116 RVA: 0x0007BA78 File Offset: 0x00079C78
		private fsResult MakeFailure(string message)
		{
			int num = Math.Max(0, this._start - 20);
			int num2 = Math.Min(50, this._input.Length - num);
			return fsResult.Fail(string.Concat(new string[]
			{
				"Error while parsing: ",
				message,
				"; context = <",
				this._input.Substring(num, num2),
				">"
			}));
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x0007BAE6 File Offset: 0x00079CE6
		private bool TryMoveNext()
		{
			if (this._start < this._input.Length)
			{
				this._start++;
				return true;
			}
			return false;
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x0007BB0C File Offset: 0x00079D0C
		private bool HasValue()
		{
			return this.HasValue(0);
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x0007BB15 File Offset: 0x00079D15
		private bool HasValue(int offset)
		{
			return this._start + offset >= 0 && this._start + offset < this._input.Length;
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x0007BB39 File Offset: 0x00079D39
		private char Character()
		{
			return this.Character(0);
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x0007BB42 File Offset: 0x00079D42
		private char Character(int offset)
		{
			return this._input[this._start + offset];
		}

		// Token: 0x060017EA RID: 6122 RVA: 0x0007BB58 File Offset: 0x00079D58
		private void SkipSpace()
		{
			while (this.HasValue())
			{
				if (char.IsWhiteSpace(this.Character()))
				{
					this.TryMoveNext();
				}
				else
				{
					if (!this.HasValue(1) || this.Character(0) != '/')
					{
						break;
					}
					if (this.Character(1) == '/')
					{
						while (this.HasValue())
						{
							if (Environment.NewLine.Contains(this.Character().ToString() ?? ""))
							{
								break;
							}
							this.TryMoveNext();
						}
					}
					else if (this.Character(1) == '*')
					{
						this.TryMoveNext();
						this.TryMoveNext();
						while (this.HasValue(1))
						{
							if (this.Character(0) == '*' && this.Character(1) == '/')
							{
								this.TryMoveNext();
								this.TryMoveNext();
								this.TryMoveNext();
								break;
							}
							this.TryMoveNext();
						}
					}
				}
			}
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x0007BC41 File Offset: 0x00079E41
		private bool IsHex(char c)
		{
			return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x0007BC68 File Offset: 0x00079E68
		private uint ParseSingleChar(char c1, uint multipliyer)
		{
			uint num = 0U;
			if (c1 >= '0' && c1 <= '9')
			{
				num = (uint)(c1 - '0') * multipliyer;
			}
			else if (c1 >= 'A' && c1 <= 'F')
			{
				num = (uint)(c1 - 'A' + '\n') * multipliyer;
			}
			else if (c1 >= 'a' && c1 <= 'f')
			{
				num = (uint)(c1 - 'a' + '\n') * multipliyer;
			}
			return num;
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x0007BCB8 File Offset: 0x00079EB8
		private uint ParseUnicode(char c1, char c2, char c3, char c4)
		{
			uint num = this.ParseSingleChar(c1, 4096U);
			uint num2 = this.ParseSingleChar(c2, 256U);
			uint num3 = this.ParseSingleChar(c3, 16U);
			uint num4 = this.ParseSingleChar(c4, 1U);
			return num + num2 + num3 + num4;
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x0007BCF8 File Offset: 0x00079EF8
		private fsResult TryUnescapeChar(out char escaped)
		{
			this.TryMoveNext();
			if (!this.HasValue())
			{
				escaped = ' ';
				return this.MakeFailure("Unexpected end of input after \\");
			}
			char c = this.Character();
			if (c <= '\\')
			{
				if (c <= '/')
				{
					if (c == '"')
					{
						this.TryMoveNext();
						escaped = '"';
						return fsResult.Success;
					}
					if (c == '/')
					{
						this.TryMoveNext();
						escaped = '/';
						return fsResult.Success;
					}
				}
				else
				{
					if (c == '0')
					{
						this.TryMoveNext();
						escaped = '\0';
						return fsResult.Success;
					}
					if (c == '\\')
					{
						this.TryMoveNext();
						escaped = '\\';
						return fsResult.Success;
					}
				}
			}
			else if (c <= 'b')
			{
				if (c == 'a')
				{
					this.TryMoveNext();
					escaped = '\a';
					return fsResult.Success;
				}
				if (c == 'b')
				{
					this.TryMoveNext();
					escaped = '\b';
					return fsResult.Success;
				}
			}
			else
			{
				if (c == 'f')
				{
					this.TryMoveNext();
					escaped = '\f';
					return fsResult.Success;
				}
				if (c == 'n')
				{
					this.TryMoveNext();
					escaped = '\n';
					return fsResult.Success;
				}
				switch (c)
				{
				case 'r':
					this.TryMoveNext();
					escaped = '\r';
					return fsResult.Success;
				case 't':
					this.TryMoveNext();
					escaped = '\t';
					return fsResult.Success;
				case 'u':
					this.TryMoveNext();
					if (this.IsHex(this.Character(0)) && this.IsHex(this.Character(1)) && this.IsHex(this.Character(2)) && this.IsHex(this.Character(3)))
					{
						uint num = this.ParseUnicode(this.Character(0), this.Character(1), this.Character(2), this.Character(3));
						this.TryMoveNext();
						this.TryMoveNext();
						this.TryMoveNext();
						this.TryMoveNext();
						escaped = (char)num;
						return fsResult.Success;
					}
					escaped = '\0';
					return this.MakeFailure(string.Format("invalid escape sequence '\\u{0}{1}{2}{3}'\n", new object[]
					{
						this.Character(0),
						this.Character(1),
						this.Character(2),
						this.Character(3)
					}));
				}
			}
			escaped = '\0';
			return this.MakeFailure(string.Format("Invalid escape sequence \\{0}", this.Character()));
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x0007BF3C File Offset: 0x0007A13C
		private fsResult TryParseExact(string content)
		{
			for (int i = 0; i < content.Length; i++)
			{
				if (this.Character() != content[i])
				{
					return this.MakeFailure("Expected " + content[i].ToString());
				}
				if (!this.TryMoveNext())
				{
					return this.MakeFailure("Unexpected end of content when parsing " + content);
				}
			}
			return fsResult.Success;
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x0007BFA8 File Offset: 0x0007A1A8
		private fsResult TryParseTrue(out fsData data)
		{
			fsResult fsResult = this.TryParseExact("true");
			if (fsResult.Succeeded)
			{
				data = new fsData(true);
				return fsResult.Success;
			}
			data = null;
			return fsResult;
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x0007BFDC File Offset: 0x0007A1DC
		private fsResult TryParseFalse(out fsData data)
		{
			fsResult fsResult = this.TryParseExact("false");
			if (fsResult.Succeeded)
			{
				data = new fsData(false);
				return fsResult.Success;
			}
			data = null;
			return fsResult;
		}

		// Token: 0x060017F2 RID: 6130 RVA: 0x0007C010 File Offset: 0x0007A210
		private fsResult TryParseNull(out fsData data)
		{
			fsResult fsResult = this.TryParseExact("null");
			if (fsResult.Succeeded)
			{
				data = new fsData();
				return fsResult.Success;
			}
			data = null;
			return fsResult;
		}

		// Token: 0x060017F3 RID: 6131 RVA: 0x0007C043 File Offset: 0x0007A243
		private bool IsSeparator(char c)
		{
			return char.IsWhiteSpace(c) || c == ',' || c == '}' || c == ']';
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x0007C060 File Offset: 0x0007A260
		private fsResult TryParseNumber(out fsData data)
		{
			int start = this._start;
			while (this.TryMoveNext() && this.HasValue() && !this.IsSeparator(this.Character()))
			{
			}
			string text = this._input.Substring(start, this._start - start);
			bool flag = false;
			double num;
			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out num) && num > 9.223372036854776E+18)
			{
				flag = true;
			}
			if (text.Contains(".") || text.Contains("e") || text.Contains("E") || text == "Infinity" || text == "-Infinity" || text == "NaN" || flag)
			{
				if (!double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
				{
					data = null;
					return this.MakeFailure("Bad double format with " + text);
				}
				data = new fsData(num);
				return fsResult.Success;
			}
			else
			{
				long num2;
				if (!long.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out num2))
				{
					data = null;
					return this.MakeFailure("Bad Int64 format with " + text);
				}
				data = new fsData(num2);
				return fsResult.Success;
			}
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x0007C18C File Offset: 0x0007A38C
		private fsResult TryParseString(out string str)
		{
			this._cachedStringBuilder.Length = 0;
			if (this.Character() != '"' || !this.TryMoveNext())
			{
				str = string.Empty;
				return this.MakeFailure("Expected initial \" when parsing a string");
			}
			while (this.HasValue() && this.Character() != '"')
			{
				char c = this.Character();
				if (c == '\\')
				{
					char c2;
					fsResult fsResult = this.TryUnescapeChar(out c2);
					if (fsResult.Failed)
					{
						str = string.Empty;
						return fsResult;
					}
					this._cachedStringBuilder.Append(c2);
				}
				else
				{
					this._cachedStringBuilder.Append(c);
					if (!this.TryMoveNext())
					{
						str = string.Empty;
						return this.MakeFailure("Unexpected end of input when reading a string");
					}
				}
			}
			if (!this.HasValue() || this.Character() != '"' || !this.TryMoveNext())
			{
				str = string.Empty;
				return this.MakeFailure("No closing \" when parsing a string");
			}
			str = this._cachedStringBuilder.ToString();
			return fsResult.Success;
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x0007C27C File Offset: 0x0007A47C
		private fsResult TryParseArray(out fsData arr)
		{
			if (this.Character() != '[')
			{
				arr = null;
				return this.MakeFailure("Expected initial [ when parsing an array");
			}
			if (!this.TryMoveNext())
			{
				arr = null;
				return this.MakeFailure("Unexpected end of input when parsing an array");
			}
			this.SkipSpace();
			List<fsData> list = new List<fsData>();
			while (this.HasValue() && this.Character() != ']')
			{
				fsData fsData;
				fsResult fsResult = this.RunParse(out fsData);
				if (fsResult.Failed)
				{
					arr = null;
					return fsResult;
				}
				list.Add(fsData);
				this.SkipSpace();
				if (this.HasValue() && this.Character() == ',')
				{
					if (!this.TryMoveNext())
					{
						break;
					}
					this.SkipSpace();
				}
			}
			if (!this.HasValue() || this.Character() != ']' || !this.TryMoveNext())
			{
				arr = null;
				return this.MakeFailure("No closing ] for array");
			}
			arr = new fsData(list);
			return fsResult.Success;
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x0007C354 File Offset: 0x0007A554
		private fsResult TryParseObject(out fsData obj)
		{
			if (this.Character() != '{')
			{
				obj = null;
				return this.MakeFailure("Expected initial { when parsing an object");
			}
			if (!this.TryMoveNext())
			{
				obj = null;
				return this.MakeFailure("Unexpected end of input when parsing an object");
			}
			this.SkipSpace();
			Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>(fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
			while (this.HasValue() && this.Character() != '}')
			{
				this.SkipSpace();
				string text;
				fsResult fsResult = this.TryParseString(out text);
				if (fsResult.Failed)
				{
					obj = null;
					return fsResult;
				}
				this.SkipSpace();
				if (!this.HasValue() || this.Character() != ':' || !this.TryMoveNext())
				{
					obj = null;
					return this.MakeFailure("Expected : after key \"" + text + "\"");
				}
				this.SkipSpace();
				fsData fsData;
				fsResult = this.RunParse(out fsData);
				if (fsResult.Failed)
				{
					obj = null;
					return fsResult;
				}
				dictionary.Add(text, fsData);
				this.SkipSpace();
				if (this.HasValue() && this.Character() == ',')
				{
					if (!this.TryMoveNext())
					{
						break;
					}
					this.SkipSpace();
				}
			}
			if (!this.HasValue() || this.Character() != '}' || !this.TryMoveNext())
			{
				obj = null;
				return this.MakeFailure("No closing } for object");
			}
			obj = new fsData(dictionary);
			return fsResult.Success;
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x0007C4A4 File Offset: 0x0007A6A4
		private fsResult RunParse(out fsData data)
		{
			this.SkipSpace();
			if (!this.HasValue())
			{
				data = null;
				return this.MakeFailure("Unexpected end of input");
			}
			char c = this.Character();
			if (c <= '[')
			{
				if (c <= 'I')
				{
					switch (c)
					{
					case '"':
					{
						string text;
						fsResult fsResult = this.TryParseString(out text);
						if (fsResult.Failed)
						{
							data = null;
							return fsResult;
						}
						data = new fsData(text);
						return fsResult.Success;
					}
					case '#':
					case '$':
					case '%':
					case '&':
					case '\'':
					case '(':
					case ')':
					case '*':
					case ',':
					case '/':
						goto IL_011F;
					case '+':
					case '-':
					case '.':
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						break;
					default:
						if (c != 'I')
						{
							goto IL_011F;
						}
						break;
					}
				}
				else if (c != 'N')
				{
					if (c != '[')
					{
						goto IL_011F;
					}
					return this.TryParseArray(out data);
				}
				return this.TryParseNumber(out data);
			}
			if (c <= 'n')
			{
				if (c == 'f')
				{
					return this.TryParseFalse(out data);
				}
				if (c == 'n')
				{
					return this.TryParseNull(out data);
				}
			}
			else
			{
				if (c == 't')
				{
					return this.TryParseTrue(out data);
				}
				if (c == '{')
				{
					return this.TryParseObject(out data);
				}
			}
			IL_011F:
			data = null;
			return this.MakeFailure("unable to parse; invalid token \"" + this.Character().ToString() + "\"");
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x0007C5F6 File Offset: 0x0007A7F6
		public static fsResult Parse(string input, out fsData data)
		{
			if (string.IsNullOrEmpty(input))
			{
				data = null;
				return fsResult.Fail("No input");
			}
			return new fsJsonParser(input).RunParse(out data);
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x0007C61C File Offset: 0x0007A81C
		public static fsData Parse(string input)
		{
			fsData fsData;
			fsJsonParser.Parse(input, out fsData).AssertSuccess();
			return fsData;
		}

		// Token: 0x060017FB RID: 6139 RVA: 0x0007C63B File Offset: 0x0007A83B
		private fsJsonParser(string input)
		{
			this._input = input;
			this._start = 0;
		}

		// Token: 0x040015F0 RID: 5616
		private int _start;

		// Token: 0x040015F1 RID: 5617
		private string _input;

		// Token: 0x040015F2 RID: 5618
		private readonly StringBuilder _cachedStringBuilder = new StringBuilder(256);
	}
}
