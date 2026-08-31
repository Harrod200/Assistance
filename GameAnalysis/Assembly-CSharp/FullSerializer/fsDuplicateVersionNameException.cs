using System;

namespace FullSerializer
{
	// Token: 0x02000469 RID: 1129
	public sealed class fsDuplicateVersionNameException : Exception
	{
		// Token: 0x060017DE RID: 6110 RVA: 0x0007BA14 File Offset: 0x00079C14
		public fsDuplicateVersionNameException(Type typeA, Type typeB, string version)
			: base(string.Concat(new string[]
			{
				(typeA != null) ? typeA.ToString() : null,
				" and ",
				(typeB != null) ? typeB.ToString() : null,
				" have the same version string (",
				version,
				"); please change one of them."
			}))
		{
		}
	}
}
