using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AC RID: 2220
	public class ObjectivesScreenController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x0600541B RID: 21531 RVA: 0x00260B01 File Offset: 0x0025ED01
		public override void Initialize()
		{
			base.Initialize();
			this.UpdateActivePlayerUIElements(true);
		}

		// Token: 0x0600541C RID: 21532 RVA: 0x00260B10 File Offset: 0x0025ED10
		public override void UpdateActivePlayerUIElements(bool startup)
		{
			this.factionIcon.sprite = base.activePlayer.factionIcon256;
			GameControl.assetLoader.LoadAssetForImageAssignment(base.activePlayer.template.gradientPath, this.factionGradient);
		}

		// Token: 0x0600541D RID: 21533 RVA: 0x00260B48 File Offset: 0x0025ED48
		public override void Show()
		{
			base.Show();
			this.objectivesPanelHeader.SetText(Loc.T("UI.Objectives.Header", new object[] { base.activePlayer.adjective }));
			this.listContainerHeader.SetText(Loc.T("UI.Objectives.ListContainerHeader", new object[] { base.activePlayer.adjective }));
			this.detailContainerHeader.SetText(Loc.T("UI.Objectives.DetailContainerHeader", new object[] { base.activePlayer.adjective }));
			this.primaryCanvas.enabled = true;
			this.detailHeader.SetText(string.Empty);
			this.detailCategory.SetText(string.Empty);
			this.detailCategory.gameObject.SetActive(false);
			this.detailBodyText.SetText(string.Empty);
			this.detailImageContainer.SetActive(false);
			GameControl.eventManager.AddListener<ObjectiveComplete>(new EventManager.EventDelegate<ObjectiveComplete>(this.OnObjectiveComplete), null, base.activePlayer, true, false);
			this.UpdateObjectivesList(base.activePlayer);
			this.objectivesTutorialController.HoldTutorial(CampaignMilestone.UITutorial_Objectives, false, true);
		}

		// Token: 0x0600541E RID: 21534 RVA: 0x00260C6C File Offset: 0x0025EE6C
		public override void Hide()
		{
			this.primaryCanvas.enabled = false;
			GameControl.eventManager.RemoveListener<ObjectiveComplete>(new EventManager.EventDelegate<ObjectiveComplete>(this.OnObjectiveComplete), null);
			this.objectivesTutorialController.HideTutorial();
			base.Hide();
		}

		// Token: 0x0600541F RID: 21535 RVA: 0x00260CA2 File Offset: 0x0025EEA2
		public override void Refresh()
		{
		}

		// Token: 0x06005420 RID: 21536 RVA: 0x00260CA4 File Offset: 0x0025EEA4
		public override bool Visible()
		{
			return base.Visible() && base.canvasManager.IsShowingInfoScreen<ObjectivesScreenController>();
		}

		// Token: 0x06005421 RID: 21537 RVA: 0x00260CBB File Offset: 0x0025EEBB
		public void CloseInfoScreen(bool toggle = false)
		{
			if (this.primaryCanvas != null)
			{
				base.canvasManager.HideInfoScreen<ObjectivesScreenController>(toggle);
			}
		}

		// Token: 0x06005422 RID: 21538 RVA: 0x00260CD7 File Offset: 0x0025EED7
		public void OnExitButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.CloseInfoScreen(false);
		}

		// Token: 0x06005423 RID: 21539 RVA: 0x00260CEC File Offset: 0x0025EEEC
		public void OnCloseAndPlayClicked()
		{
			this.OnExitButtonSelected();
			base.gameTime.Play();
		}

		// Token: 0x06005424 RID: 21540 RVA: 0x00260CFF File Offset: 0x0025EEFF
		private void OnObjectiveComplete(ObjectiveComplete e)
		{
			this.UpdateObjectivesList(base.activePlayer);
		}

		// Token: 0x06005425 RID: 21541 RVA: 0x00260D10 File Offset: 0x0025EF10
		private void UpdateObjectivesList(TIFactionState faction)
		{
			List<TIObjectiveTemplate> objectivesByStatus = faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked);
			List<TIObjectiveTemplate> list = faction.GetObjectivesByType(ObjectiveType.Tutorial).Intersect<TIObjectiveTemplate>(objectivesByStatus).ToList<TIObjectiveTemplate>();
			List<TIObjectiveTemplate> objectivesByStatus2 = faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked);
			List<TIObjectiveTemplate> list2 = faction.GetObjectivesByType(ObjectiveType.Victory).Intersect<TIObjectiveTemplate>(objectivesByStatus2).ToList<TIObjectiveTemplate>();
			List<TIObjectiveTemplate> list3 = faction.GetObjectivesByType(ObjectiveType.Campaign).Intersect<TIObjectiveTemplate>(objectivesByStatus2).ToList<TIObjectiveTemplate>();
			List<TIObjectiveTemplate> list4 = faction.GetObjectivesByType(ObjectiveType.General).Intersect<TIObjectiveTemplate>(objectivesByStatus2).ToList<TIObjectiveTemplate>();
			List<TIObjectiveTemplate> objectivesByStatus3 = faction.GetObjectivesByStatus(ObjectiveStatus.Completed);
			List<TIObjectiveTemplate> list5 = faction.GetObjectivesByType(ObjectiveType.Campaign).Union<TIObjectiveTemplate>(faction.GetObjectivesByType(ObjectiveType.Tutorial)).Intersect<TIObjectiveTemplate>(objectivesByStatus3)
				.ToList<TIObjectiveTemplate>();
			int num = ((list.Count > 0) ? (list.Count + 1) : 0);
			int num2 = ((list2.Count > 0) ? (list2.Count + 1) : 0);
			int num3 = ((list3.Count > 0) ? (list3.Count + 1) : 0);
			int num4 = ((list4.Count > 0) ? (list4.Count + 1) : 0);
			int num5 = ((list5.Count > 0) ? (list5.Count + 1) : 0);
			this.objectivesList.SetListSize<ObjectivesListItemController>(num + num2 + num3 + num4 + num5, false, false);
			int num6 = ((list2.Count > 0) ? 0 : (-1));
			int num7 = ((list.Count > 0) ? num2 : (-1));
			int num8 = ((list3.Count > 0) ? (num + num2) : (-1));
			int num9 = ((list4.Count > 0) ? (num + num2 + num3) : (-1));
			int num10 = ((list5.Count > 0) ? (num + num2 + num3 + num4) : (-1));
			int num11 = 0;
			using (IEnumerator<object> enumerator = this.objectivesList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ObjectivesScreenController.<>o__28.<>p__0 == null)
					{
						ObjectivesScreenController.<>o__28.<>p__0 = CallSite<Func<CallSite, object, ObjectivesListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ObjectivesListItemController), typeof(ObjectivesScreenController)));
					}
					ObjectivesListItemController objectivesListItemController = ObjectivesScreenController.<>o__28.<>p__0.Target(ObjectivesScreenController.<>o__28.<>p__0, enumerator.Current);
					objectivesListItemController.Init(this);
					if (num11 > num10 && list5.Count > 0)
					{
						objectivesListItemController.UpdateObjectivesListItem(list5[num11 - num10 - 1], base.activePlayer, true, num11 != this.objectivesList.size - 1);
					}
					else if (num11 == num10)
					{
						objectivesListItemController.UpdateHeaderListItem(ObjectiveType.Campaign, true);
					}
					else if (num11 > num9 && list4.Count > 0)
					{
						objectivesListItemController.UpdateObjectivesListItem(list4[num11 - num9 - 1], base.activePlayer, false, true);
					}
					else if (num11 == num9)
					{
						objectivesListItemController.UpdateHeaderListItem(ObjectiveType.General, false);
					}
					else if (num11 > num8 && list3.Count > 0)
					{
						objectivesListItemController.UpdateObjectivesListItem(list3[num11 - num8 - 1], base.activePlayer, false, true);
					}
					else if (num11 == num8)
					{
						objectivesListItemController.UpdateHeaderListItem(ObjectiveType.Campaign, false);
					}
					else if (num11 > num7 && list.Count > 0)
					{
						objectivesListItemController.UpdateObjectivesListItem(list[num11 - num7 - 1], base.activePlayer, false, true);
					}
					else if (num11 == num7)
					{
						objectivesListItemController.UpdateHeaderListItem(ObjectiveType.Tutorial, false);
					}
					else if (num11 > num6 && list2.Count > 0)
					{
						objectivesListItemController.UpdateObjectivesListItem(list2[num11 - num6 - 1], base.activePlayer, false, true);
					}
					else if (num11 == num6)
					{
						objectivesListItemController.UpdateHeaderListItem(ObjectiveType.Victory, false);
					}
					num11++;
				}
			}
		}

		// Token: 0x06005426 RID: 21542 RVA: 0x00261098 File Offset: 0x0025F298
		public void SetSelectedObjectiveEntry(TIObjectiveTemplate objective, string heldDataName, TIFactionState faction)
		{
			if (objective == null)
			{
				this.factionIconPanel.SetActive(true);
				this.detailImageContainer.SetActive(false);
				if (!string.IsNullOrEmpty(heldDataName))
				{
					this.detailHeader.SetText(Loc.T(new StringBuilder("UI.Objectives.").Append(heldDataName).ToString()));
					this.detailCategory.SetText(string.Empty);
					this.detailCategory.gameObject.SetActive(false);
					this.detailBodyText.SetText(Loc.T(new StringBuilder("UI.Objectives.Detail.").Append(heldDataName).ToString()));
					return;
				}
				this.detailHeader.SetText(string.Empty);
				this.detailCategory.SetText(string.Empty);
				this.detailCategory.gameObject.SetActive(false);
				this.detailBodyText.SetText(string.Empty);
				return;
			}
			else
			{
				this.detailHeader.SetText(objective.displayName(base.activePlayer));
				this.detailCategory.SetText(Loc.T(new StringBuilder("UI.Objectives.").Append(Enum.GetName(typeof(ObjectiveType), objective.objectiveType)).ToString()));
				this.detailCategory.gameObject.SetActive(true);
				if (faction.GetObjectiveStatus(objective) == ObjectiveStatus.Completed)
				{
					this.detailBodyText.SetText(objective.resolution(base.activePlayer));
					if (!string.IsNullOrEmpty(objective.completedIllustrationResource))
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(objective.completedIllustrationResource, this.detailImage);
						this.factionIconPanel.SetActive(false);
						this.detailImageContainer.SetActive(true);
						return;
					}
					this.factionIconPanel.SetActive(true);
					this.detailImageContainer.SetActive(false);
					return;
				}
				else
				{
					if (TIObjectiveTemplate.HasChildMilestone(faction, objective))
					{
						this.detailBodyText.SetText(objective.fullParentMilestoneDescription(base.activePlayer));
					}
					else
					{
						this.detailBodyText.SetText(objective.fullDescription(base.activePlayer, true));
					}
					if (!string.IsNullOrEmpty(objective.assignedIllustrationResource))
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(objective.assignedIllustrationResource, this.detailImage);
						this.factionIconPanel.SetActive(false);
						this.detailImageContainer.SetActive(true);
						return;
					}
					this.factionIconPanel.SetActive(true);
					this.detailImageContainer.SetActive(false);
					return;
				}
			}
		}

		// Token: 0x04003A62 RID: 14946
		public TMP_Text objectivesPanelHeader;

		// Token: 0x04003A63 RID: 14947
		public TMP_Text listContainerHeader;

		// Token: 0x04003A64 RID: 14948
		public TMP_Text detailContainerHeader;

		// Token: 0x04003A65 RID: 14949
		public GameObject factionIconPanel;

		// Token: 0x04003A66 RID: 14950
		public Image factionIcon;

		// Token: 0x04003A67 RID: 14951
		public Image factionGradient;

		// Token: 0x04003A68 RID: 14952
		public GameObject detailImageContainer;

		// Token: 0x04003A69 RID: 14953
		public Image detailImage;

		// Token: 0x04003A6A RID: 14954
		public TMP_Text detailHeader;

		// Token: 0x04003A6B RID: 14955
		public TMP_Text detailCategory;

		// Token: 0x04003A6C RID: 14956
		public TMP_Text detailBodyText;

		// Token: 0x04003A6D RID: 14957
		public RectTransform objectiveDetailPanel;

		// Token: 0x04003A6E RID: 14958
		public VerticalLayoutGroup objectiveDetailPanelVLG;

		// Token: 0x04003A6F RID: 14959
		public RectTransform headerGradient;

		// Token: 0x04003A70 RID: 14960
		public RectTransform detailContainer;

		// Token: 0x04003A71 RID: 14961
		public Canvas primaryCanvas;

		// Token: 0x04003A72 RID: 14962
		public ListManagerBase objectivesList;

		// Token: 0x04003A73 RID: 14963
		public UITutorialController objectivesTutorialController;
	}
}
