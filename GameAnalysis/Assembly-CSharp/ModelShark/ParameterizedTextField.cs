using System;

namespace ModelShark
{
	// Token: 0x020004BB RID: 1211
	[Serializable]
	public class ParameterizedTextField
	{
		// Token: 0x04001738 RID: 5944
		public string name;

		// Token: 0x04001739 RID: 5945
		public string placeholder;

		// Token: 0x0400173A RID: 5946
		public string value;

		// Token: 0x0400173B RID: 5947
		public bool valueOnDemand;

		// Token: 0x0400173C RID: 5948
		public ParameterizedTextField.BuildStringOnTooltipHover del;

		// Token: 0x02000C68 RID: 3176
		// (Invoke) Token: 0x06006CA9 RID: 27817
		public delegate string BuildStringOnTooltipHover();
	}
}
