using System;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AE RID: 2222
	public class NavPanelListItemController : MonoBehaviour
	{
		// Token: 0x17000EF7 RID: 3831
		// (get) Token: 0x06005435 RID: 21557 RVA: 0x002619F0 File Offset: 0x0025FBF0
		public TargetSelectionTool locationSelector
		{
			get
			{
				return base.GetComponentInParent<TargetSelectionTool>();
			}
		}

		// Token: 0x06005436 RID: 21558 RVA: 0x002619F8 File Offset: 0x0025FBF8
		public void SetListItem(TINaturalSpaceObjectState naturalSpaceObjectState)
		{
			this.naturalSpaceObject = naturalSpaceObjectState;
			this.buttonImage.sprite = this.naturalSpaceObject.icon;
		}

		// Token: 0x06005437 RID: 21559 RVA: 0x00261A17 File Offset: 0x0025FC17
		public void OnClicked()
		{
			SpaceObjectSelection.BlockSelectionFrame();
			this.locationSelector.OnNavigatorListButtonClicked(this.naturalSpaceObject);
		}

		// Token: 0x04003A81 RID: 14977
		public Image buttonImage;

		// Token: 0x04003A82 RID: 14978
		public TINaturalSpaceObjectState naturalSpaceObject;
	}
}
