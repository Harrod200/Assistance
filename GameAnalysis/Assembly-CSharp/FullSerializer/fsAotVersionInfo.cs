using System;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000459 RID: 1113
	public struct fsAotVersionInfo
	{
		// Token: 0x040015C3 RID: 5571
		public bool IsConstructorPublic;

		// Token: 0x040015C4 RID: 5572
		public fsAotVersionInfo.Member[] Members;

		// Token: 0x02000C4E RID: 3150
		public struct Member
		{
			// Token: 0x06006C46 RID: 27718 RVA: 0x00306598 File Offset: 0x00304798
			public Member(fsMetaProperty property)
			{
				this.MemberName = property.MemberName;
				this.JsonName = property.JsonName;
				this.StorageType = property.StorageType.CSharpName(true);
				this.OverrideConverterType = null;
				if (property.OverrideConverterType != null)
				{
					this.OverrideConverterType = property.OverrideConverterType.CSharpName();
				}
			}

			// Token: 0x06006C47 RID: 27719 RVA: 0x003065F5 File Offset: 0x003047F5
			public override bool Equals(object obj)
			{
				return obj is fsAotVersionInfo.Member && this == (fsAotVersionInfo.Member)obj;
			}

			// Token: 0x06006C48 RID: 27720 RVA: 0x00306614 File Offset: 0x00304814
			public override int GetHashCode()
			{
				return this.MemberName.GetHashCode() + 17 * this.JsonName.GetHashCode() + 17 * this.StorageType.GetHashCode() + (string.IsNullOrEmpty(this.OverrideConverterType) ? 0 : (17 * this.OverrideConverterType.GetHashCode()));
			}

			// Token: 0x06006C49 RID: 27721 RVA: 0x0030666C File Offset: 0x0030486C
			public static bool operator ==(fsAotVersionInfo.Member a, fsAotVersionInfo.Member b)
			{
				return a.MemberName == b.MemberName && a.JsonName == b.JsonName && a.StorageType == b.StorageType && a.OverrideConverterType == b.OverrideConverterType;
			}

			// Token: 0x06006C4A RID: 27722 RVA: 0x003066C5 File Offset: 0x003048C5
			public static bool operator !=(fsAotVersionInfo.Member a, fsAotVersionInfo.Member b)
			{
				return !(a == b);
			}

			// Token: 0x04004E01 RID: 19969
			public string MemberName;

			// Token: 0x04004E02 RID: 19970
			public string JsonName;

			// Token: 0x04004E03 RID: 19971
			public string StorageType;

			// Token: 0x04004E04 RID: 19972
			public string OverrideConverterType;
		}
	}
}
