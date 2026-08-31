using System;

namespace FullSerializer
{
	// Token: 0x02000470 RID: 1136
	public abstract class fsObjectProcessor
	{
		// Token: 0x06001807 RID: 6151 RVA: 0x0007CD17 File Offset: 0x0007AF17
		public virtual bool CanProcess(Type type)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0007CD1E File Offset: 0x0007AF1E
		public virtual void OnBeforeSerialize(Type storageType, object instance)
		{
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0007CD20 File Offset: 0x0007AF20
		public virtual void OnAfterSerialize(Type storageType, object instance, ref fsData data)
		{
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x0007CD22 File Offset: 0x0007AF22
		public virtual void OnBeforeDeserialize(Type storageType, ref fsData data)
		{
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x0007CD24 File Offset: 0x0007AF24
		public virtual void OnBeforeDeserializeAfterInstanceCreation(Type storageType, object instance, ref fsData data)
		{
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x0007CD26 File Offset: 0x0007AF26
		public virtual void OnAfterDeserialize(Type storageType, object instance)
		{
		}
	}
}
