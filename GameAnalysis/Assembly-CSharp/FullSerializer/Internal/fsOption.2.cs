using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000483 RID: 1155
	public static class fsOption
	{
		// Token: 0x060018AC RID: 6316 RVA: 0x0007FCAE File Offset: 0x0007DEAE
		public static fsOption<T> Just<T>(T value)
		{
			return new fsOption<T>(value);
		}
	}
}
