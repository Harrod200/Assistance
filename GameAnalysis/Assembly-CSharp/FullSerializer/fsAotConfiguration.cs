using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullSerializer
{
	// Token: 0x02000458 RID: 1112
	public class fsAotConfiguration : ScriptableObject
	{
		// Token: 0x06001785 RID: 6021 RVA: 0x0007A4A0 File Offset: 0x000786A0
		public bool TryFindEntry(Type type, out fsAotConfiguration.Entry result)
		{
			string fullName = type.FullName;
			foreach (fsAotConfiguration.Entry entry in this.aotTypes)
			{
				if (entry.FullTypeName == fullName)
				{
					result = entry;
					return true;
				}
			}
			result = default(fsAotConfiguration.Entry);
			return false;
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x0007A518 File Offset: 0x00078718
		public void UpdateOrAddEntry(fsAotConfiguration.Entry entry)
		{
			for (int i = 0; i < this.aotTypes.Count; i++)
			{
				if (this.aotTypes[i].FullTypeName == entry.FullTypeName)
				{
					this.aotTypes[i] = entry;
					return;
				}
			}
			this.aotTypes.Add(entry);
		}

		// Token: 0x040015C1 RID: 5569
		public List<fsAotConfiguration.Entry> aotTypes = new List<fsAotConfiguration.Entry>();

		// Token: 0x040015C2 RID: 5570
		public string outputDirectory = "Assets/AotModels";

		// Token: 0x02000C4C RID: 3148
		public enum AotState
		{
			// Token: 0x04004DFC RID: 19964
			Default,
			// Token: 0x04004DFD RID: 19965
			Enabled,
			// Token: 0x04004DFE RID: 19966
			Disabled
		}

		// Token: 0x02000C4D RID: 3149
		[Serializable]
		public struct Entry
		{
			// Token: 0x06006C44 RID: 27716 RVA: 0x0030656B File Offset: 0x0030476B
			public Entry(Type type)
			{
				this.FullTypeName = type.FullName;
				this.State = fsAotConfiguration.AotState.Default;
			}

			// Token: 0x06006C45 RID: 27717 RVA: 0x00306580 File Offset: 0x00304780
			public Entry(Type type, fsAotConfiguration.AotState state)
			{
				this.FullTypeName = type.FullName;
				this.State = state;
			}

			// Token: 0x04004DFF RID: 19967
			public fsAotConfiguration.AotState State;

			// Token: 0x04004E00 RID: 19968
			public string FullTypeName;
		}
	}
}
