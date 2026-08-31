using System;

namespace FullSerializer.Internal
{
	// Token: 0x02000486 RID: 1158
	public struct fsVersionedType
	{
		// Token: 0x060018CC RID: 6348 RVA: 0x00080342 File Offset: 0x0007E542
		public object Migrate(object ancestorInstance)
		{
			return Activator.CreateInstance(this.ModelType, new object[] { ancestorInstance });
		}

		// Token: 0x060018CD RID: 6349 RVA: 0x0008035C File Offset: 0x0007E55C
		public override string ToString()
		{
			string[] array = new string[7];
			array[0] = "fsVersionedType [ModelType=";
			int num = 1;
			Type modelType = this.ModelType;
			array[num] = ((modelType != null) ? modelType.ToString() : null);
			array[2] = ", VersionString=";
			array[3] = this.VersionString;
			array[4] = ", Ancestors.Length=";
			array[5] = this.Ancestors.Length.ToString();
			array[6] = "]";
			return string.Concat(array);
		}

		// Token: 0x060018CE RID: 6350 RVA: 0x000803C5 File Offset: 0x0007E5C5
		public static bool operator ==(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType == b.ModelType;
		}

		// Token: 0x060018CF RID: 6351 RVA: 0x000803D8 File Offset: 0x0007E5D8
		public static bool operator !=(fsVersionedType a, fsVersionedType b)
		{
			return a.ModelType != b.ModelType;
		}

		// Token: 0x060018D0 RID: 6352 RVA: 0x000803EB File Offset: 0x0007E5EB
		public override bool Equals(object obj)
		{
			return obj is fsVersionedType && this.ModelType == ((fsVersionedType)obj).ModelType;
		}

		// Token: 0x060018D1 RID: 6353 RVA: 0x0008040D File Offset: 0x0007E60D
		public override int GetHashCode()
		{
			return this.ModelType.GetHashCode();
		}

		// Token: 0x04001621 RID: 5665
		public fsVersionedType[] Ancestors;

		// Token: 0x04001622 RID: 5666
		public string VersionString;

		// Token: 0x04001623 RID: 5667
		public Type ModelType;
	}
}
