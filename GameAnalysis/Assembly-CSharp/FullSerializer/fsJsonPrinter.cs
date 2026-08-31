using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace FullSerializer
{
	// Token: 0x0200046D RID: 1133
	public static class fsJsonPrinter
	{
		// Token: 0x060017FC RID: 6140 RVA: 0x0007C664 File Offset: 0x0007A864
		private static void InsertSpacing(TextWriter stream, int count)
		{
			for (int i = 0; i < count; i++)
			{
				stream.Write("    ");
			}
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x0007C688 File Offset: 0x0007A888
		private static string EscapeString(string str)
		{
			bool flag = false;
			int i = 0;
			while (i < str.Length)
			{
				char c = str[i];
				int num = Convert.ToInt32(c);
				if (num < 0 || num > 127)
				{
					flag = true;
					break;
				}
				if (c <= '\r')
				{
					if (c == '\0')
					{
						goto IL_005D;
					}
					switch (c)
					{
					case '\a':
					case '\b':
					case '\t':
					case '\n':
					case '\f':
					case '\r':
						goto IL_005D;
					}
				}
				else if (c == '"' || c == '\\')
				{
					goto IL_005D;
				}
				IL_005F:
				if (!flag)
				{
					i++;
					continue;
				}
				break;
				IL_005D:
				flag = true;
				goto IL_005F;
			}
			if (!flag)
			{
				return str;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c2 in str)
			{
				int num2 = Convert.ToInt32(c2);
				if (num2 < 0 || num2 > 127)
				{
					stringBuilder.Append(string.Format("\\u{0:x4} ", num2).Trim());
				}
				else
				{
					if (c2 <= '\r')
					{
						if (c2 == '\0')
						{
							stringBuilder.Append("\\0");
							goto IL_018E;
						}
						switch (c2)
						{
						case '\a':
							stringBuilder.Append("\\a");
							goto IL_018E;
						case '\b':
							stringBuilder.Append("\\b");
							goto IL_018E;
						case '\t':
							stringBuilder.Append("\\t");
							goto IL_018E;
						case '\n':
							stringBuilder.Append("\\n");
							goto IL_018E;
						case '\f':
							stringBuilder.Append("\\f");
							goto IL_018E;
						case '\r':
							stringBuilder.Append("\\r");
							goto IL_018E;
						}
					}
					else
					{
						if (c2 == '"')
						{
							stringBuilder.Append("\\\"");
							goto IL_018E;
						}
						if (c2 == '\\')
						{
							stringBuilder.Append("\\\\");
							goto IL_018E;
						}
					}
					stringBuilder.Append(c2);
				}
				IL_018E:;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0007C83C File Offset: 0x0007AA3C
		private static void BuildCompressedString(fsData data, TextWriter stream)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
			{
				stream.Write('[');
				bool flag = false;
				foreach (fsData fsData in data.AsList)
				{
					if (flag)
					{
						stream.Write(',');
					}
					flag = true;
					fsJsonPrinter.BuildCompressedString(fsData, stream);
				}
				stream.Write(']');
				return;
			}
			case fsDataType.Object:
			{
				stream.Write('{');
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
					}
					flag2 = true;
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(":");
					fsJsonPrinter.BuildCompressedString(keyValuePair.Value, stream);
				}
				stream.Write('}');
				return;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				return;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				return;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
					return;
				}
				stream.Write("false");
				return;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				return;
			case fsDataType.Null:
				stream.Write("null");
				return;
			default:
				return;
			}
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0007C9D8 File Offset: 0x0007ABD8
		private static void BuildPrettyString(fsData data, TextWriter stream, int depth)
		{
			switch (data.Type)
			{
			case fsDataType.Array:
			{
				if (data.AsList.Count == 0)
				{
					stream.Write("[]");
					return;
				}
				bool flag = false;
				stream.Write('[');
				stream.WriteLine();
				foreach (fsData fsData in data.AsList)
				{
					if (flag)
					{
						stream.Write(',');
						stream.WriteLine();
					}
					flag = true;
					fsJsonPrinter.InsertSpacing(stream, depth + 1);
					fsJsonPrinter.BuildPrettyString(fsData, stream, depth + 1);
				}
				stream.WriteLine();
				fsJsonPrinter.InsertSpacing(stream, depth);
				stream.Write(']');
				return;
			}
			case fsDataType.Object:
			{
				stream.Write('{');
				stream.WriteLine();
				bool flag2 = false;
				foreach (KeyValuePair<string, fsData> keyValuePair in data.AsDictionary)
				{
					if (flag2)
					{
						stream.Write(',');
						stream.WriteLine();
					}
					flag2 = true;
					fsJsonPrinter.InsertSpacing(stream, depth + 1);
					stream.Write('"');
					stream.Write(keyValuePair.Key);
					stream.Write('"');
					stream.Write(": ");
					fsJsonPrinter.BuildPrettyString(keyValuePair.Value, stream, depth + 1);
				}
				stream.WriteLine();
				fsJsonPrinter.InsertSpacing(stream, depth);
				stream.Write('}');
				return;
			}
			case fsDataType.Double:
				stream.Write(fsJsonPrinter.ConvertDoubleToString(data.AsDouble));
				return;
			case fsDataType.Int64:
				stream.Write(data.AsInt64);
				return;
			case fsDataType.Boolean:
				if (data.AsBool)
				{
					stream.Write("true");
					return;
				}
				stream.Write("false");
				return;
			case fsDataType.String:
				stream.Write('"');
				stream.Write(fsJsonPrinter.EscapeString(data.AsString));
				stream.Write('"');
				return;
			case fsDataType.Null:
				stream.Write("null");
				return;
			default:
				return;
			}
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0007CBD8 File Offset: 0x0007ADD8
		public static void PrettyJson(fsData data, TextWriter outputStream)
		{
			fsJsonPrinter.BuildPrettyString(data, outputStream, 0);
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x0007CBE4 File Offset: 0x0007ADE4
		public static string PrettyJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildPrettyString(data, stringWriter, 0);
				text = stringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0007CC2C File Offset: 0x0007AE2C
		public static void CompressedJson(fsData data, StreamWriter outputStream)
		{
			fsJsonPrinter.BuildCompressedString(data, outputStream);
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x0007CC38 File Offset: 0x0007AE38
		public static string CompressedJson(fsData data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				fsJsonPrinter.BuildCompressedString(data, stringWriter);
				text = stringBuilder.ToString();
			}
			return text;
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x0007CC80 File Offset: 0x0007AE80
		private static string ConvertDoubleToString(double d)
		{
			if (double.IsInfinity(d) || double.IsNaN(d))
			{
				return d.ToString(CultureInfo.InvariantCulture);
			}
			string text = d.ToString(CultureInfo.InvariantCulture);
			if (!text.Contains(".") && !text.Contains("e") && !text.Contains("E"))
			{
				text += ".0";
			}
			return text;
		}
	}
}
