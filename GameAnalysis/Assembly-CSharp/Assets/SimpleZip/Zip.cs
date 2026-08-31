using System;
using System.Text;
using Ionic.Zlib;

namespace Assets.SimpleZip
{
	// Token: 0x020004AB RID: 1195
	public static class Zip
	{
		// Token: 0x06001ADC RID: 6876 RVA: 0x00091708 File Offset: 0x0008F908
		public static byte[] Compress(string text)
		{
			return ZlibStream.CompressString(text);
		}

		// Token: 0x06001ADD RID: 6877 RVA: 0x00091710 File Offset: 0x0008F910
		public static string CompressToString(string text)
		{
			return Convert.ToBase64String(Zip.Compress(text));
		}

		// Token: 0x06001ADE RID: 6878 RVA: 0x0009171D File Offset: 0x0008F91D
		public static string Decompress(byte[] bytes)
		{
			return Encoding.UTF8.GetString(ZlibStream.UncompressBuffer(bytes));
		}

		// Token: 0x06001ADF RID: 6879 RVA: 0x0009172F File Offset: 0x0008F92F
		public static string Decompress(string data)
		{
			return Zip.Decompress(Convert.FromBase64String(data));
		}
	}
}
