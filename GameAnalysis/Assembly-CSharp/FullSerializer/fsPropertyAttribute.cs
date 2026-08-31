using System;

namespace FullSerializer
{
	// Token: 0x02000471 RID: 1137
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class fsPropertyAttribute : Attribute
	{
		// Token: 0x0600180E RID: 6158 RVA: 0x0007CD30 File Offset: 0x0007AF30
		public fsPropertyAttribute()
			: this(string.Empty)
		{
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x0007CD3D File Offset: 0x0007AF3D
		public fsPropertyAttribute(string name)
		{
			this.Name = name;
		}

		// Token: 0x040015FC RID: 5628
		public string Name;

		// Token: 0x040015FD RID: 5629
		public Type Converter;
	}
}
