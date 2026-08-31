using System;
using System.Reflection;
using UnityEngine;

namespace FullSerializer
{
	// Token: 0x02000461 RID: 1121
	public class fsConfig
	{
		// Token: 0x040015D9 RID: 5593
		public Type[] SerializeAttributes = new Type[]
		{
			typeof(SerializeField),
			typeof(fsPropertyAttribute)
		};

		// Token: 0x040015DA RID: 5594
		public Type[] IgnoreSerializeAttributes = new Type[]
		{
			typeof(NonSerializedAttribute),
			typeof(fsIgnoreAttribute)
		};

		// Token: 0x040015DB RID: 5595
		public fsMemberSerialization DefaultMemberSerialization = fsMemberSerialization.Default;

		// Token: 0x040015DC RID: 5596
		public Func<string, MemberInfo, string> GetJsonNameFromMemberName = (string name, MemberInfo info) => name;

		// Token: 0x040015DD RID: 5597
		public bool EnablePropertySerialization = true;

		// Token: 0x040015DE RID: 5598
		public bool SerializeNonAutoProperties;

		// Token: 0x040015DF RID: 5599
		public bool SerializeNonPublicSetProperties = true;

		// Token: 0x040015E0 RID: 5600
		public string CustomDateTimeFormatString;

		// Token: 0x040015E1 RID: 5601
		public bool Serialize64BitIntegerAsString;

		// Token: 0x040015E2 RID: 5602
		public bool SerializeEnumsAsInteger;
	}
}
