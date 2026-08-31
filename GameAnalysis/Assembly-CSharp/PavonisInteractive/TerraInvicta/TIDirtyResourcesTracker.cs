using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000716 RID: 1814
	public class TIDirtyResourcesTracker
	{
		// Token: 0x06002B66 RID: 11110 RVA: 0x000EC9C0 File Offset: 0x000EABC0
		public bool IsResourceRevenueDirty(FactionResource factionResource)
		{
			bool flag;
			return !this.resourceRevenueDirty.TryGetValue(factionResource, out flag) || flag;
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x000EC9E0 File Offset: 0x000EABE0
		public bool IsResourceIncomeDirty(FactionResource factionResource)
		{
			bool flag;
			return !this.resourceIncomeDirty.TryGetValue(factionResource, out flag) || flag;
		}

		// Token: 0x06002B68 RID: 11112 RVA: 0x000ECA00 File Offset: 0x000EAC00
		public void SetResourceDirty(FactionResource factionResource)
		{
			this.resourceIncomeDirty[factionResource] = true;
			this.resourceRevenueDirty[factionResource] = true;
			if (factionResource == FactionResource.MissionControl)
			{
				this.resourceIncomeDirty[FactionResource.Money] = true;
				this.resourceIncomeDirty[FactionResource.Research] = true;
				this.resourceRevenueDirty[FactionResource.Money] = true;
				this.resourceRevenueDirty[FactionResource.Research] = true;
			}
		}

		// Token: 0x06002B69 RID: 11113 RVA: 0x000ECA5F File Offset: 0x000EAC5F
		public void MarkResourceIncomeUpdated(FactionResource factionResource)
		{
			this.resourceIncomeDirty[factionResource] = false;
		}

		// Token: 0x06002B6A RID: 11114 RVA: 0x000ECA6E File Offset: 0x000EAC6E
		public void MarkResourceRevenueUpdated(FactionResource factionResource)
		{
			this.resourceRevenueDirty[factionResource] = false;
		}

		// Token: 0x06002B6B RID: 11115 RVA: 0x000ECA80 File Offset: 0x000EAC80
		public void SetAllResourcesDirty()
		{
			this.resourceIncomeDirty.Keys.ToList<FactionResource>().ForEach(delegate(FactionResource x)
			{
				this.resourceIncomeDirty[x] = true;
			});
			this.resourceRevenueDirty.Keys.ToList<FactionResource>().ForEach(delegate(FactionResource x)
			{
				this.resourceRevenueDirty[x] = true;
			});
		}

		// Token: 0x0400214B RID: 8523
		private Dictionary<FactionResource, bool> resourceIncomeDirty = new Dictionary<FactionResource, bool>();

		// Token: 0x0400214C RID: 8524
		private Dictionary<FactionResource, bool> resourceRevenueDirty = new Dictionary<FactionResource, bool>();
	}
}
