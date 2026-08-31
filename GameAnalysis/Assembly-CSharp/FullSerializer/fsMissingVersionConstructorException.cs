using System;

namespace FullSerializer
{
	// Token: 0x02000468 RID: 1128
	public sealed class fsMissingVersionConstructorException : Exception
	{
		// Token: 0x060017DD RID: 6109 RVA: 0x0007B9E5 File Offset: 0x00079BE5
		public fsMissingVersionConstructorException(Type versionedType, Type constructorType)
			: base(((versionedType != null) ? versionedType.ToString() : null) + " is missing a constructor for previous model type " + ((constructorType != null) ? constructorType.ToString() : null))
		{
		}
	}
}
