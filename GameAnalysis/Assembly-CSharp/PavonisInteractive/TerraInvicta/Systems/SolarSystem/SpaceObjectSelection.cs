using System;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.SolarSystem
{
	// Token: 0x020009A3 RID: 2467
	[UpdateInGroup(typeof(PipelineStages.InputHandleStage))]
	[AlwaysUpdateSystem]
	public class SpaceObjectSelection : StrategyLayerComponentSystem
	{
		// Token: 0x17000FF0 RID: 4080
		// (get) Token: 0x06005D04 RID: 23812 RVA: 0x002C5CB9 File Offset: 0x002C3EB9
		// (set) Token: 0x06005D05 RID: 23813 RVA: 0x002C5CD6 File Offset: 0x002C3ED6
		public GameObject ObjectSelected
		{
			get
			{
				if (!(this.SpaceObjectController == null))
				{
					return this.SpaceObjectController.gameObject;
				}
				return null;
			}
			set
			{
				this.SpaceObjectController = ((value != null) ? value.GetComponent<SpaceObjectController>() : null);
			}
		}

		// Token: 0x17000FF1 RID: 4081
		// (get) Token: 0x06005D06 RID: 23814 RVA: 0x002C5CEA File Offset: 0x002C3EEA
		// (set) Token: 0x06005D07 RID: 23815 RVA: 0x002C5CF2 File Offset: 0x002C3EF2
		public TISpaceObjectState spaceObjectStateSelected { get; private set; }

		// Token: 0x17000FF2 RID: 4082
		// (get) Token: 0x06005D08 RID: 23816 RVA: 0x002C5CFB File Offset: 0x002C3EFB
		public bool HasSelection
		{
			get
			{
				return this.ObjectSelected != null;
			}
		}

		// Token: 0x06005D09 RID: 23817 RVA: 0x002C5D0C File Offset: 0x002C3F0C
		protected override void OnUpdate()
		{
			if (this.BlockThisFrame)
			{
				this.BlockThisFrame = false;
				return;
			}
			if (TIStandaloneInputModule.current.IsPointerOverUIGameObject() || TIStandaloneInputModule.current.IsPointerOverSurfaceIcon())
			{
				return;
			}
			Physics.Raycast(this.camera.unityCamera.ScreenPointToRay(Input.mousePosition), out this.hit, 2000f, 2048);
			if (this.hit.collider != null)
			{
				GameObject gameObject = this.hit.collider.gameObject.transform.parent.gameObject;
				SpaceObjectController component = gameObject.GetComponent<SpaceObjectController>();
				TISpaceObjectState tispaceObjectState = null;
				if (component != null)
				{
					tispaceObjectState = component.spaceObjectState;
				}
				if (tispaceObjectState == null || (tispaceObjectState.isSpaceAssetState && !tispaceObjectState.ref_spaceAsset.VisibleToFaction(GameControl.control.activePlayer)))
				{
					return;
				}
				if (gameObject != this.objectHovered)
				{
					this.SetHoverObject(gameObject);
				}
				if (Input.GetMouseButtonUp(0) && !GameControl.control._canvasStack.IsShowingInfoScreen() && (GameControl.control.viewMgr.currentView != ViewType.PoliticalMap || gameObject != GameControl.control.viewMgr.earthObject))
				{
					bool flag = this.objectHovered == this.ObjectSelected && (this.SpaceObjectController.symbolController.visible || GeneralControlsController.UIOtherSelectedState == tispaceObjectState || GeneralControlsController.UISelectedAssetState == tispaceObjectState);
					this.SelectObject(this.objectHovered, true, false);
					if (this.spaceObjectStateSelected != null)
					{
						SoundEffectController.PlaySelectSound(this.spaceObjectStateSelected);
						TIUtilities.GotoGameState(this.spaceObjectStateSelected, true, true, true, flag, false, -1f);
						return;
					}
				}
			}
			else
			{
				this.SetHoverObject(null);
			}
		}

		// Token: 0x06005D0A RID: 23818 RVA: 0x002C5ECD File Offset: 0x002C40CD
		public static void BlockSelectionFrame()
		{
			World.Active.GetExistingManager<SpaceObjectSelection>().BlockThisFrame = true;
		}

		// Token: 0x06005D0B RID: 23819 RVA: 0x002C5EDF File Offset: 0x002C40DF
		public static TISpaceObjectState GetSelectedSpaceObject()
		{
			return World.Active.GetExistingManager<SpaceObjectSelection>().spaceObjectStateSelected;
		}

		// Token: 0x06005D0C RID: 23820 RVA: 0x002C5EF0 File Offset: 0x002C40F0
		public static void SelectSpaceObject(GameObject selection, bool setAsGlobalSelectedGameState, bool blockFrame = false, bool barycenterFallback = false)
		{
			if (selection == null)
			{
				selection = GameStateManager.Sol().gameObjectLink;
			}
			SpaceObjectSelection existingManager = World.Active.GetExistingManager<SpaceObjectSelection>();
			if (barycenterFallback || existingManager.ObjectSelected != selection)
			{
				existingManager.SelectObject(selection, setAsGlobalSelectedGameState, barycenterFallback);
				existingManager.BlockThisFrame = blockFrame;
			}
		}

		// Token: 0x06005D0D RID: 23821 RVA: 0x002C5F40 File Offset: 0x002C4140
		public void SelectObject(GameObject newSelection, bool setAsGlobalSelectedGameState, bool barycenterFallback = false)
		{
			SpaceObjectController spaceObjectController = ((newSelection != null) ? newSelection.GetComponent<SpaceObjectController>() : null);
			if (spaceObjectController != null && double.IsNaN(spaceObjectController.SpaceObject.Position.magnitude))
			{
				return;
			}
			if (this.HasSelection && this.ObjectSelected.GetComponent<SpaceObjectController>().HasSymbol)
			{
				this.ObjectSelected.GetComponent<SpaceObjectController>().symbolController.SetSelected(false);
			}
			if (newSelection == null)
			{
				this.ObjectSelected = null;
				if (setAsGlobalSelectedGameState)
				{
					GeneralControlsController.SetSelectedState(null, false);
				}
				this.spaceObjectStateSelected = null;
				return;
			}
			if (spaceObjectController.HasSymbol)
			{
				newSelection.GetComponent<SpaceObjectController>().symbolController.SetSelected(true);
			}
			this.ObjectSelected = newSelection;
			if (setAsGlobalSelectedGameState)
			{
				GeneralControlsController.SetSelectedState(spaceObjectController.spaceObjectState, true);
			}
			this.spaceObjectStateSelected = this.ObjectSelected.GetComponent<SpaceObjectController>().spaceObjectState;
			this.gameTime.ResetSpeed(barycenterFallback);
			this.camera.OnSelectionChanged();
		}

		// Token: 0x06005D0E RID: 23822 RVA: 0x002C6033 File Offset: 0x002C4233
		private void SetHoverObject(GameObject go)
		{
			this.ToggleHover(false);
			this.objectHovered = go;
			this.ToggleHover(true);
		}

		// Token: 0x06005D0F RID: 23823 RVA: 0x002C604C File Offset: 0x002C424C
		private void ToggleHover(bool isHovered)
		{
			if (this.objectHovered == null)
			{
				return;
			}
			SpaceObjectController component = this.objectHovered.GetComponent<SpaceObjectController>();
			if (component != null && component.HasSymbol)
			{
				component.symbolController.hoverImage.enabled = isHovered;
				if (isHovered)
				{
					this.wasShowingDisplayName = component.symbolController.objectName.enabled;
					component.symbolController.ShowDisplayName();
					return;
				}
				if (this.wasShowingDisplayName)
				{
					component.symbolController.SetDisplayName();
					return;
				}
				component.symbolController.HideDisplayName();
			}
		}

		// Token: 0x04004290 RID: 17040
		private const int SelectionLayer = 2048;

		// Token: 0x04004291 RID: 17041
		private GameObject objectHovered;

		// Token: 0x04004293 RID: 17043
		public SpaceObjectController SpaceObjectController;

		// Token: 0x04004294 RID: 17044
		private RaycastHit hit;

		// Token: 0x04004295 RID: 17045
		[Inject]
		private CameraManager camera;

		// Token: 0x04004296 RID: 17046
		[Inject]
		private GameTimeManager gameTime;

		// Token: 0x04004297 RID: 17047
		public bool BlockThisFrame;

		// Token: 0x04004298 RID: 17048
		private bool wasShowingDisplayName;
	}
}
