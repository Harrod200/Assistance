using System;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008AF RID: 2223
	public class OperationButtonController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06005439 RID: 21561 RVA: 0x00261A37 File Offset: 0x0025FC37
		public void Init(OperationCanvasController controller)
		{
			this.controller = controller;
		}

		// Token: 0x0600543A RID: 21562 RVA: 0x00261A40 File Offset: 0x0025FC40
		public void SetOperationData(IOperation operation, TIGameState actingState, bool buttonInteractible, TIGameState baseTarget = null)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(operation.GetOperationIconImagePath_Off(), this.foregroundImage);
			this.interactable = buttonInteractible;
			base.GetComponentInChildren<Button>().interactable = this.interactable;
			this.operationType = operation;
			if (this.operationType is DeployArmyOperation_OpenTarget)
			{
				this.operationType = new DeployArmyOperation_OpenTarget(true);
			}
			else if (this.operationType is DeployArmiesOperation)
			{
				this.operationType = new DeployArmiesOperation(true);
			}
			if (this.highlightImage != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(operation.GetOperationIconImagePath_On(), this.highlightImage);
				SpriteState spriteState = base.GetComponentInChildren<Button>().spriteState;
				spriteState.highlightedSprite = this.highlightImage.sprite;
				spriteState.pressedSprite = this.highlightImage.sprite;
				this.foregroundImage.color = (base.GetComponentInChildren<Button>().interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.2f));
				spriteState.disabledSprite = this.foregroundImage.sprite;
				base.GetComponentInChildren<Button>().spriteState = spriteState;
			}
		}

		// Token: 0x0600543B RID: 21563 RVA: 0x00261B77 File Offset: 0x0025FD77
		public void OnButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OptionSelect", false, false);
			this.controller.OnOperationSelected(this, null);
		}

		// Token: 0x0600543C RID: 21564 RVA: 0x00261B92 File Offset: 0x0025FD92
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.interactable)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverActionIcon", false, false);
			}
			this.controller.OnOperationPointerEnter(this);
		}

		// Token: 0x0600543D RID: 21565 RVA: 0x00261BB4 File Offset: 0x0025FDB4
		public void OnPointerExit(PointerEventData eventData)
		{
			this.controller.OnOperationPointerExit(this);
		}

		// Token: 0x04003A83 RID: 14979
		public Image foregroundImage;

		// Token: 0x04003A84 RID: 14980
		public Image highlightImage;

		// Token: 0x04003A85 RID: 14981
		private OperationCanvasController controller;

		// Token: 0x04003A86 RID: 14982
		public IOperation operationType;

		// Token: 0x04003A87 RID: 14983
		public bool interactable;
	}
}
