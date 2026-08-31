using System;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000715 RID: 1813
	public struct ResourceValue
	{
		// Token: 0x06002B63 RID: 11107 RVA: 0x000EC962 File Offset: 0x000EAB62
		public ResourceValue(FactionResource resource, float value)
		{
			this.resource = resource;
			this.value = value;
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x000EC972 File Offset: 0x000EAB72
		public bool Equality(ResourceValue checkValue)
		{
			return this.resource == checkValue.resource && this.value == checkValue.value;
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x000EC992 File Offset: 0x000EAB92
		public override string ToString()
		{
			return new StringBuilder(TIUtilities.InlineResourceStr(this.resource)).Append(TIUtilities.FormatBigOrSmallNumber(this.value, 1, 7, 0, true, false)).ToString();
		}

		// Token: 0x04002149 RID: 8521
		public FactionResource resource;

		// Token: 0x0400214A RID: 8522
		public float value;
	}
}
