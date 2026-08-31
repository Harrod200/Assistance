using System;

namespace FullSerializer
{
	// Token: 0x0200045C RID: 1116
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class fsForwardAttribute : Attribute
	{
		// Token: 0x0600178C RID: 6028 RVA: 0x0007A6D8 File Offset: 0x000788D8
		public fsForwardAttribute(string memberName)
		{
			this.MemberName = memberName;
		}

		// Token: 0x040015CF RID: 5583
		public string MemberName;
	}
}
