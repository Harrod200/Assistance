using System;
using System.Collections.Generic;
using System.Linq;

namespace FullSerializer
{
	// Token: 0x02000472 RID: 1138
	public struct fsResult
	{
		// Token: 0x06001810 RID: 6160 RVA: 0x0007CD4C File Offset: 0x0007AF4C
		public void AddMessage(string message)
		{
			if (this._messages == null)
			{
				this._messages = new List<string>();
			}
			this._messages.Add(message);
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x0007CD6D File Offset: 0x0007AF6D
		public void AddMessages(fsResult result)
		{
			if (result._messages == null)
			{
				return;
			}
			if (this._messages == null)
			{
				this._messages = new List<string>();
			}
			this._messages.AddRange(result._messages);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x0007CD9C File Offset: 0x0007AF9C
		public fsResult Merge(fsResult other)
		{
			this._success = this._success && other._success;
			if (other._messages != null)
			{
				if (this._messages == null)
				{
					this._messages = new List<string>(other._messages);
				}
				else
				{
					this._messages.AddRange(other._messages);
				}
			}
			return this;
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x0007CDFC File Offset: 0x0007AFFC
		public static fsResult Warn(string warning)
		{
			return new fsResult
			{
				_success = true,
				_messages = new List<string> { warning }
			};
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x0007CE30 File Offset: 0x0007B030
		public static fsResult Fail(string warning)
		{
			return new fsResult
			{
				_success = false,
				_messages = new List<string> { warning }
			};
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x0007CE61 File Offset: 0x0007B061
		public static fsResult operator +(fsResult a, fsResult b)
		{
			return a.Merge(b);
		}

		// Token: 0x17000358 RID: 856
		// (get) Token: 0x06001816 RID: 6166 RVA: 0x0007CE6B File Offset: 0x0007B06B
		public bool Failed
		{
			get
			{
				return !this._success;
			}
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x06001817 RID: 6167 RVA: 0x0007CE76 File Offset: 0x0007B076
		public bool Succeeded
		{
			get
			{
				return this._success;
			}
		}

		// Token: 0x1700035A RID: 858
		// (get) Token: 0x06001818 RID: 6168 RVA: 0x0007CE7E File Offset: 0x0007B07E
		public bool HasWarnings
		{
			get
			{
				return this._messages != null && this._messages.Any<string>();
			}
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x0007CE95 File Offset: 0x0007B095
		public fsResult AssertSuccess()
		{
			if (this.Failed)
			{
				throw this.AsException;
			}
			return this;
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x0007CEAC File Offset: 0x0007B0AC
		public fsResult AssertSuccessWithoutWarnings()
		{
			if (this.Failed || this.RawMessages.Any<string>())
			{
				throw this.AsException;
			}
			return this;
		}

		// Token: 0x1700035B RID: 859
		// (get) Token: 0x0600181B RID: 6171 RVA: 0x0007CED0 File Offset: 0x0007B0D0
		public Exception AsException
		{
			get
			{
				if (!this.Failed && !this.RawMessages.Any<string>())
				{
					throw new Exception("Only a failed result can be converted to an exception");
				}
				return new Exception(this.FormattedMessages);
			}
		}

		// Token: 0x1700035C RID: 860
		// (get) Token: 0x0600181C RID: 6172 RVA: 0x0007CEFD File Offset: 0x0007B0FD
		public IEnumerable<string> RawMessages
		{
			get
			{
				if (this._messages != null)
				{
					return this._messages;
				}
				return fsResult.EmptyStringArray;
			}
		}

		// Token: 0x1700035D RID: 861
		// (get) Token: 0x0600181D RID: 6173 RVA: 0x0007CF13 File Offset: 0x0007B113
		public string FormattedMessages
		{
			get
			{
				return string.Join(",\n", this.RawMessages.ToArray<string>());
			}
		}

		// Token: 0x040015FE RID: 5630
		private static readonly string[] EmptyStringArray = new string[0];

		// Token: 0x040015FF RID: 5631
		private bool _success;

		// Token: 0x04001600 RID: 5632
		private List<string> _messages;

		// Token: 0x04001601 RID: 5633
		public static fsResult Success = new fsResult
		{
			_success = true
		};
	}
}
