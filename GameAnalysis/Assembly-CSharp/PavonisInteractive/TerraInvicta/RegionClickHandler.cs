using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000560 RID: 1376
	public class RegionClickHandler : MonoBehaviour
	{
		// Token: 0x06002460 RID: 9312 RVA: 0x000C0FEC File Offset: 0x000BF1EC
		private void OnMouseUp()
		{
			if (Application.isFocused && !EventSystem.current.IsPointerOverGameObject() && !GameControl.control._canvasStack.IsShowingInfoScreen() && !TIStandaloneInputModule.current.IsPointerOverUIGameObject())
			{
				this.owner.MouseUp();
			}
		}

		// Token: 0x06002461 RID: 9313 RVA: 0x000C102C File Offset: 0x000BF22C
		private void OnMouseOver()
		{
			if (!Application.isFocused)
			{
				return;
			}
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			if (TIStandaloneInputModule.current.IsPointerOverUIGameObject())
			{
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				this.owner.mapVisualizer.lastLeftClickedRegion = this;
			}
			if (Input.GetMouseButtonDown(1))
			{
				this.owner.mapVisualizer.lastRightClickedRegion = this;
			}
			this.owner.MouseOver();
			if (this.owner.mapVisualizer.lastRightClickedRegion == this && Input.GetMouseButtonUp(1) && TIInputManager.IsShiftKeyDown)
			{
				TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
				if (uiselectedAssetState != null && uiselectedAssetState.isArmyState)
				{
					GameControl.eventManager.TriggerEvent(new DeployArmyToRegionRequested(GeneralControlsController.UISelectedAssetState.ref_army, this.owner.region, TIInputManager.IsControlKeyDown), null, Array.Empty<object>());
					return;
				}
			}
			else if (this.owner.mapVisualizer.lastLeftClickedRegion == this && Input.GetMouseButtonUp(0) && TIInputManager.IsControlKeyDown && TIMissionPhaseState.InMissionPhase())
			{
				TIGameState uiselectedAssetState2 = GeneralControlsController.UISelectedAssetState;
				if ((uiselectedAssetState2 == null || !uiselectedAssetState2.isArmyState) && TIMissionPhaseState.InMissionPhase())
				{
					GameControl.eventManager.TriggerEvent(new MissionOptionsForTargetRequested(this.owner.region), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06002462 RID: 9314 RVA: 0x000C1170 File Offset: 0x000BF370
		private void OnMouseExit()
		{
			this.owner.MouseExit();
		}

		// Token: 0x04001B5F RID: 7007
		public RegionController owner;

		// Token: 0x04001B60 RID: 7008
		public const float sensitivity = 0.01f;
	}
}
