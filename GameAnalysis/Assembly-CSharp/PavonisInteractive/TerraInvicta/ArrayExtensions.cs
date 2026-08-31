using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F3 RID: 2035
	public static class ArrayExtensions
	{
		// Token: 0x060049F4 RID: 18932 RVA: 0x001F0EEF File Offset: 0x001EF0EF
		public static bool IsNullOrEmpty(this Array array)
		{
			return array == null || array.Length == 0;
		}

		// Token: 0x060049F5 RID: 18933 RVA: 0x001F0EFF File Offset: 0x001EF0FF
		public static bool IsNotNullOrEmpty(this Array array)
		{
			return array != null && array.Length > 0;
		}
	}
}
