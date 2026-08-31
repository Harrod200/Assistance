using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;

// Token: 0x02000449 RID: 1097
public class UITutorialController : MonoBehaviour
{
	// Token: 0x060016FD RID: 5885 RVA: 0x00076852 File Offset: 0x00074A52
	private void Start()
	{
		if (GameControl.control.activePlayer.MilestoneCompleted(CampaignMilestone.UITutorial_Intro))
		{
			UITutorialController.CanHoldTutorials = true;
		}
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x00076870 File Offset: 0x00074A70
	public static void SetTutorialMilestone(CampaignMilestone milestone)
	{
		UITutorialController.tutorialMilestone = milestone;
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x00076878 File Offset: 0x00074A78
	public void InitTutorialTip(CampaignMilestone milestone)
	{
		UITutorialController.SetTutorialMilestone(milestone);
		this.tutorialTipObject = TutorialTip.Instance.gameObject;
		Loc.SwapFonts(this.tutorialTipObject);
		this.tutorialTip = TutorialTip.Instance;
		this.tutorialTip.transform.SetSiblingIndex(0);
		this.tutorialTip.transform.localScale = new Vector3(1f, 1f, 1f);
		this.tutorialTip.uiTutorialTipList = this.uiTutorialTip;
		this.tutorialTip.parentController = this;
		this.tutorialTip.currentTipIndex = 0;
		if (milestone == CampaignMilestone.UITutorial_GeneralControlsCanvas || milestone == CampaignMilestone.UITutorial_Intro || milestone == CampaignMilestone.UITutorial_SpaceCombatCanvas || milestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_Formations || milestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_FriendlyShipDetail || milestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_Waypoints || milestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_GroupSelection || milestone == CampaignMilestone.UITutorial_IntelScreenCanvas_AlienThreat)
		{
			this.tutorialTip.hideTipButton.gameObject.SetActive(false);
			return;
		}
		this.tutorialTip.hideTipButton.gameObject.SetActive(true);
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x0007697C File Offset: 0x00074B7C
	public void ResetTutorial(bool showImmediate = false)
	{
		this.dontShowAgain = false;
		if (this.tutorialTip != null)
		{
			this.tutorialTip.currentTipIndex = 0;
		}
		if (showImmediate)
		{
			this.ShowTutorialTips(UITutorialController.tutorialMilestone, true, true);
		}
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x000769B0 File Offset: 0x00074BB0
	public void HoldTutorial(CampaignMilestone milestone, bool overrideMilestone = false, bool nextFrame = true)
	{
		if (!UITutorialController.CanHoldTutorials)
		{
			return;
		}
		if (this.dontShowAgain || !TIGlobalValuesState.isTutorialActive)
		{
			return;
		}
		if (this.prereqMilestone != CampaignMilestone.None && !GameControl.control.activePlayer.MilestoneCompleted(this.prereqMilestone))
		{
			return;
		}
		if (GameControl.control.activePlayer.MilestoneCompleted(milestone) && !overrideMilestone)
		{
			return;
		}
		GeneralControlsController generalControlsController = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
		if (generalControlsController != null)
		{
			generalControlsController.HoldUITutorial(this, milestone, overrideMilestone, nextFrame);
		}
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x00076A34 File Offset: 0x00074C34
	public void CompleteTutorial(bool dontShowAgain = false)
	{
		if (UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_GeneralControlsCanvas || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_Intro || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_Formations || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_FriendlyShipDetail || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_Waypoints || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_GroupSelection || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_IntelScreenCanvas_AlienThreat)
		{
			dontShowAgain = true;
		}
		if (dontShowAgain)
		{
			GameControl.control.activePlayer.CompleteMilestone(UITutorialController.tutorialMilestone);
		}
		else
		{
			this.tutorialTip.currentTipIndex = 0;
		}
		if (!this.is3D)
		{
			TIInputManager.RestoreKeybindings();
		}
		this.ClearHeldTutorial();
		UITutorialController.SetTutorialMilestone(CampaignMilestone.None);
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x00076AE4 File Offset: 0x00074CE4
	public void ShowTutorialTips(CampaignMilestone milestone, bool overrideMilestone = false, bool nextFrame = true)
	{
		if (this.dontShowAgain || !TIGlobalValuesState.isTutorialActive)
		{
			return;
		}
		if (UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_Intro)
		{
			this.CompleteTutorial(true);
		}
		UITutorialController.SetTutorialMilestone(milestone);
		if (this.prereqMilestone != CampaignMilestone.None && !GameControl.control.activePlayer.MilestoneCompleted(this.prereqMilestone))
		{
			return;
		}
		if (GameControl.control.activePlayer.MilestoneCompleted(milestone) && !overrideMilestone)
		{
			return;
		}
		if (TIGlobalValuesState.isSpaceCombatEnabled)
		{
			if (GameControl.spaceCombat.activeShips == null)
			{
				return;
			}
			List<CombatShipController> list = GameControl.spaceCombat.activeShips.Where<CombatShipController>((CombatShipController x) => x.activePlayerShip).ToList<CombatShipController>();
			if ((UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_Waypoints || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_FriendlyShipDetail || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas_GroupSelection || UITutorialController.tutorialMilestone == CampaignMilestone.UITutorial_SpaceCombatCanvas) && list.Count < 1)
			{
				return;
			}
		}
		if (!this.is3D)
		{
			TIInputManager.BlockKeybindings();
		}
		this.InitTutorialTip(milestone);
		if (this.uiTutorialTip.Count > 0)
		{
			Canvas canvas;
			if (this.tutorialTipObject.TryGetComponent<Canvas>(out canvas) && !canvas.enabled)
			{
				canvas.enabled = true;
			}
			if (!this.tutorialTipObject.activeSelf)
			{
				this.tutorialTipObject.SetActive(true);
			}
			if (!this.is3D && this.tutorialTip.currentTipIndex < this.tutorialTip.uiTutorialTipList.Count)
			{
				this.tutorialTip.SetupTip(this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipLOCName, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipLOCDesc, this.uiTutorialTip[this.tutorialTip.currentTipIndex].targetElement, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipAction, this.uiTutorialTip[this.tutorialTip.currentTipIndex].disableHighlightBlocker, nextFrame, this.uiTutorialTip[this.tutorialTip.currentTipIndex].arrowDirectionOverride, this.uiTutorialTip[this.tutorialTip.currentTipIndex].FallbackTargetElement, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tutorialImage, this.uiTutorialTip[this.tutorialTip.currentTipIndex].controlIDs, null);
			}
			if (this.is3D)
			{
				this.FindAndDisplay3DTipTarget();
			}
		}
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x00076D68 File Offset: 0x00074F68
	public void FindAndDisplay3DTipTarget()
	{
		GameObject gameObject = GameControl.spaceCombat.activeShips[0].gameObject;
		switch (this.findTipTarget[this.tutorialTip.currentTipIndex])
		{
		case UITutorialController.FindTipTarget.FriendlyWaypoint1:
			gameObject = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(1).gameObject;
			break;
		case UITutorialController.FindTipTarget.FriendlyWaypoint2:
			gameObject = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(2).gameObject;
			break;
		case UITutorialController.FindTipTarget.FriendlyWaypoint3:
			gameObject = GameControl.spaceCombat.activeShips[0]._waypointNavigationController.WaypointContainer.transform.GetChild(3).gameObject;
			break;
		case UITutorialController.FindTipTarget.MiddleShip:
			gameObject = GameControl.spaceCombat.activeShips[0].gameObject;
			break;
		}
		this.tutorialTip.SetupTip(this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipLOCName, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipLOCDesc, this.uiTutorialTip[this.tutorialTip.currentTipIndex].targetElement, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tipAction, this.uiTutorialTip[this.tutorialTip.currentTipIndex].disableHighlightBlocker, false, this.uiTutorialTip[this.tutorialTip.currentTipIndex].arrowDirectionOverride, this.uiTutorialTip[this.tutorialTip.currentTipIndex].FallbackTargetElement, this.uiTutorialTip[this.tutorialTip.currentTipIndex].tutorialImage, this.uiTutorialTip[this.tutorialTip.currentTipIndex].controlIDs, gameObject);
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00076F5E File Offset: 0x0007515E
	public void HideTutorial()
	{
		if (!UITutorialController.disallowHidingTutorialTipObject && this.tutorialTipObject != null)
		{
			this.tutorialTipObject.SetActive(false);
		}
		this.ClearHeldTutorial();
	}

	// Token: 0x06001706 RID: 5894 RVA: 0x00076F88 File Offset: 0x00075188
	private void ClearHeldTutorial()
	{
		if (this.generalControlsController != null)
		{
			this.generalControlsController.ClearHeldTutorial(this);
			return;
		}
		this.generalControlsController = World.Active.GetExistingManager<CanvasManager>().StrategyHud as GeneralControlsController;
		if (this.generalControlsController != null)
		{
			this.generalControlsController.ClearHeldTutorial(this);
		}
	}

	// Token: 0x04001578 RID: 5496
	[Tooltip("Setup the UI tutorial here. Each tip requires 2 LOC entries(Name & Description), as well as a UI element to point to. There is currently a position value bug with anchored elements, so I am using a blank UI element as the pointer")]
	public List<UITutorial> uiTutorialTip;

	// Token: 0x04001579 RID: 5497
	private GameObject tutorialTipObject;

	// Token: 0x0400157A RID: 5498
	private TutorialTip tutorialTip;

	// Token: 0x0400157B RID: 5499
	public static CampaignMilestone tutorialMilestone;

	// Token: 0x0400157C RID: 5500
	public static bool disallowHidingTutorialTipObject;

	// Token: 0x0400157D RID: 5501
	[Tooltip("tutorial won't show unless this milestone has been met first")]
	public CampaignMilestone prereqMilestone;

	// Token: 0x0400157E RID: 5502
	[Tooltip("used for combat tutorial, 3d space tooltip, not ui canvas")]
	public bool is3D;

	// Token: 0x0400157F RID: 5503
	[Tooltip("used for finding tooltip placement target during runtime i.e. ship waypoint")]
	public List<UITutorialController.FindTipTarget> findTipTarget = new List<UITutorialController.FindTipTarget>();

	// Token: 0x04001580 RID: 5504
	public UITutorialController transitionToTutorial;

	// Token: 0x04001581 RID: 5505
	public CampaignMilestone transitionToMilestone;

	// Token: 0x04001582 RID: 5506
	[HideInInspector]
	public bool dontShowAgain;

	// Token: 0x04001583 RID: 5507
	private GeneralControlsController generalControlsController;

	// Token: 0x04001584 RID: 5508
	public UITutorialActionType closeTutorialAction;

	// Token: 0x04001585 RID: 5509
	public static bool CanHoldTutorials;

	// Token: 0x02000C43 RID: 3139
	public enum FindTipTarget
	{
		// Token: 0x04004DE2 RID: 19938
		None,
		// Token: 0x04004DE3 RID: 19939
		FriendlyWaypoint1,
		// Token: 0x04004DE4 RID: 19940
		FriendlyWaypoint2,
		// Token: 0x04004DE5 RID: 19941
		FriendlyWaypoint3,
		// Token: 0x04004DE6 RID: 19942
		MiddleShip
	}
}
