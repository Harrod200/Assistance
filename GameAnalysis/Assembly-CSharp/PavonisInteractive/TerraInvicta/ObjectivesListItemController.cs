using System;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AB RID: 2219
	public class ObjectivesListItemController : MonoBehaviour
	{
		// Token: 0x06005416 RID: 21526 RVA: 0x00260983 File Offset: 0x0025EB83
		public void Init(ObjectivesScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06005417 RID: 21527 RVA: 0x0026098C File Offset: 0x0025EB8C
		public void UpdateObjectivesListItem(TIObjectiveTemplate objective, TIFactionState faction, bool completed = false, bool showDividerLine = true)
		{
			this.selectObjectiveButtonText.text = new StringBuilder("  ").Append(objective.displayName(faction)).ToString();
			this.heldObjective = objective;
			this.heldDataName = objective.dataName;
			this.heldFaction = faction;
			if (completed)
			{
				this.selectObjectiveButtonText.color = this.completedColor;
				this.completedCheckmark.enabled = true;
			}
			this.headerBackground.enabled = false;
			this.dividerLine.enabled = showDividerLine;
		}

		// Token: 0x06005418 RID: 21528 RVA: 0x00260A18 File Offset: 0x0025EC18
		public void UpdateHeaderListItem(ObjectiveType objectiveType, bool completed = false)
		{
			this.heldObjective = null;
			this.headerBackground.enabled = true;
			this.dividerLine.enabled = false;
			if (!completed)
			{
				this.heldDataName = objectiveType.ToString();
				this.selectObjectiveButtonText.SetText(Loc.T(new StringBuilder("UI.Objectives.").Append(this.heldDataName).ToString()));
			}
			else
			{
				this.heldDataName = "Completed";
				this.selectObjectiveButtonText.SetText(Loc.T("UI.Objectives.Completed"));
			}
			this.completedCheckmark.enabled = false;
		}

		// Token: 0x06005419 RID: 21529 RVA: 0x00260AB2 File Offset: 0x0025ECB2
		public void OnLineClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SetSelectedObjectiveEntry(this.heldObjective, this.heldDataName, this.heldFaction);
		}

		// Token: 0x04003A58 RID: 14936
		private ObjectivesScreenController controller;

		// Token: 0x04003A59 RID: 14937
		public Button selectObjectiveButton;

		// Token: 0x04003A5A RID: 14938
		public TMP_Text selectObjectiveButtonText;

		// Token: 0x04003A5B RID: 14939
		public Image headerBackground;

		// Token: 0x04003A5C RID: 14940
		public Image completedCheckmark;

		// Token: 0x04003A5D RID: 14941
		public Image dividerLine;

		// Token: 0x04003A5E RID: 14942
		public Color32 completedColor = new Color32(108, 129, 139, byte.MaxValue);

		// Token: 0x04003A5F RID: 14943
		private TIObjectiveTemplate heldObjective;

		// Token: 0x04003A60 RID: 14944
		private string heldDataName;

		// Token: 0x04003A61 RID: 14945
		private TIFactionState heldFaction;
	}
}
