using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000157 RID: 343
public class HabBuildQueue
{
	// Token: 0x0600053E RID: 1342 RVA: 0x00017035 File Offset: 0x00015235
	public void AddItemToList(TIHabModuleTemplate module, int position)
	{
		if (position >= this.orderedBuildList.Count)
		{
			this.orderedBuildList.Add(module);
			return;
		}
		this.orderedBuildList.Insert(position, module);
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001705F File Offset: 0x0001525F
	public void RemoveItemFromList(int position)
	{
		this.orderedBuildList.RemoveAt(position);
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x00017070 File Offset: 0x00015270
	public bool RepositionItemInList(int position, bool up)
	{
		if ((position == 0 && up) || (position == this.orderedBuildList.Count - 1 && !up))
		{
			return false;
		}
		TIHabModuleTemplate tihabModuleTemplate = this.orderedBuildList[position];
		this.orderedBuildList.RemoveAt(position);
		if (up)
		{
			this.orderedBuildList.Insert(position - 1, tihabModuleTemplate);
		}
		else
		{
			this.orderedBuildList.Insert(position + 1, tihabModuleTemplate);
		}
		return true;
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x000170D7 File Offset: 0x000152D7
	public void ApplyQueueToHab(TIHabState hab)
	{
	}

	// Token: 0x04000267 RID: 615
	public List<TIHabModuleTemplate> orderedBuildList = new List<TIHabModuleTemplate>();
}
