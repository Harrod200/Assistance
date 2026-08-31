using System;
using ModelShark;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000429 RID: 1065
public class FleetsScreenFleetOperationGridItemController : MonoBehaviour
{
	// Token: 0x06001635 RID: 5685 RVA: 0x0007112C File Offset: 0x0006F32C
	public void SetGridItem(IOperation op)
	{
		GameControl.assetLoader.LoadAssetForImageAssignment(op.GetOperationIconImagePath_Off(), this.operationIcon);
		this.tooltip.SetDelegate("BodyText", () => op.GetDisplayName());
	}

	// Token: 0x04001456 RID: 5206
	public Image operationIcon;

	// Token: 0x04001457 RID: 5207
	public TooltipTrigger tooltip;
}
