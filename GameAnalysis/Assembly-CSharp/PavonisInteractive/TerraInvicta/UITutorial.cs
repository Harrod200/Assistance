using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000905 RID: 2309
	[Serializable]
	public class UITutorial
	{
		// Token: 0x17000F2E RID: 3886
		// (get) Token: 0x0600585B RID: 22619 RVA: 0x00288484 File Offset: 0x00286684
		public GameObject FallbackTargetElement
		{
			get
			{
				if (this.fallbackTargetElements == null || this.fallbackTargetElements.Count == 0)
				{
					return null;
				}
				return this.fallbackTargetElements.FirstOrDefault<GameObject>((GameObject go) => go != null && go.activeInHierarchy);
			}
		}

		// Token: 0x04004012 RID: 16402
		public string tipLOCName;

		// Token: 0x04004013 RID: 16403
		public string tipLOCDesc;

		// Token: 0x04004014 RID: 16404
		public GameObject targetElement;

		// Token: 0x04004015 RID: 16405
		public UITutorialActionType tipAction;

		// Token: 0x04004016 RID: 16406
		public bool disableHighlightBlocker;

		// Token: 0x04004017 RID: 16407
		public TutorialTip.ArrowDirection arrowDirectionOverride;

		// Token: 0x04004018 RID: 16408
		public List<GameObject> fallbackTargetElements;

		// Token: 0x04004019 RID: 16409
		public Sprite tutorialImage;

		// Token: 0x0400401A RID: 16410
		public List<int> controlIDs;
	}
}
