using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000482 RID: 1154
	public struct fsOption<T>
	{
		// Token: 0x1700035F RID: 863
		// (get) Token: 0x060018A8 RID: 6312 RVA: 0x0007FC70 File Offset: 0x0007DE70
		public bool HasValue
		{
			get
			{
				return this._hasValue;
			}
		}

		// Token: 0x17000360 RID: 864
		// (get) Token: 0x060018A9 RID: 6313 RVA: 0x0007FC78 File Offset: 0x0007DE78
		public bool IsEmpty
		{
			get
			{
				return !this._hasValue;
			}
		}

		// Token: 0x17000361 RID: 865
		// (get) Token: 0x060018AA RID: 6314 RVA: 0x0007FC83 File Offset: 0x0007DE83
		public T Value
		{
			get
			{
				if (this.IsEmpty)
				{
					throw new InvalidOperationException("fsOption is empty");
				}
				return this._value;
			}
		}

		// Token: 0x060018AB RID: 6315 RVA: 0x0007FC9E File Offset: 0x0007DE9E
		public fsOption(T value)
		{
			this._hasValue = true;
			this._value = value;
		}

		// Token: 0x0400161A RID: 5658
		private bool _hasValue;

		// Token: 0x0400161B RID: 5659
		private T _value;

		// Token: 0x0400161C RID: 5660
		public static fsOption<T> Empty;
	}
}
