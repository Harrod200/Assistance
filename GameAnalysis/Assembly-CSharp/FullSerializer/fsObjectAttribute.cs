using System;

namespace FullSerializer
{
	// Token: 0x0200046F RID: 1135
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class fsObjectAttribute : Attribute
	{
		// Token: 0x06001805 RID: 6149 RVA: 0x0007CCEB File Offset: 0x0007AEEB
		public fsObjectAttribute()
		{
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x0007CCFA File Offset: 0x0007AEFA
		public fsObjectAttribute(string versionString, params Type[] previousModels)
		{
			this.VersionString = versionString;
			this.PreviousModels = previousModels;
		}

		// Token: 0x040015F7 RID: 5623
		public Type[] PreviousModels;

		// Token: 0x040015F8 RID: 5624
		public string VersionString;

		// Token: 0x040015F9 RID: 5625
		public fsMemberSerialization MemberSerialization = fsMemberSerialization.Default;

		// Token: 0x040015FA RID: 5626
		public Type Converter;

		// Token: 0x040015FB RID: 5627
		public Type Processor;
	}
}
