using System;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000844 RID: 2116
	public class ModifierListItemController : MonoBehaviour
	{
		// Token: 0x06004CF5 RID: 19701 RVA: 0x0020B364 File Offset: 0x00209564
		public void Init(CouncilorMissionCanvasController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06004CF6 RID: 19702 RVA: 0x0020B370 File Offset: 0x00209570
		public void SetModifiers(string modifierName, float modifierValue)
		{
			this.modifierName.text = modifierName;
			if (Mathf.Round(modifierValue) == modifierValue)
			{
				this.modifierValue.text = modifierValue.ToString("N0");
				return;
			}
			this.modifierValue.text = modifierValue.ToString("N1");
		}

		// Token: 0x04002F40 RID: 12096
		public CouncilorMissionCanvasController controller;

		// Token: 0x04002F41 RID: 12097
		public TMP_Text modifierName;

		// Token: 0x04002F42 RID: 12098
		public TMP_Text modifierValue;
	}
}
