using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B8 RID: 1464
	public abstract class CanvasControllerBase : MonoBehaviour, ICanvas, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060027BE RID: 10174 RVA: 0x000D946A File Offset: 0x000D766A
		public GameObject GameObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060027BF RID: 10175 RVA: 0x000D9472 File Offset: 0x000D7672
		// (set) Token: 0x060027C0 RID: 10176 RVA: 0x000D947A File Offset: 0x000D767A
		public Canvas Canvas { get; private set; }

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060027C1 RID: 10177 RVA: 0x000D9483 File Offset: 0x000D7683
		// (set) Token: 0x060027C2 RID: 10178 RVA: 0x000D948B File Offset: 0x000D768B
		private protected CanvasManager canvasManager { protected get; private set; }

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x060027C3 RID: 10179 RVA: 0x000D9494 File Offset: 0x000D7694
		// (set) Token: 0x060027C4 RID: 10180 RVA: 0x000D949C File Offset: 0x000D769C
		public GameTimeManager gameTime { get; private set; }

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x060027C5 RID: 10181 RVA: 0x000D94A5 File Offset: 0x000D76A5
		// (set) Token: 0x060027C6 RID: 10182 RVA: 0x000D94AD File Offset: 0x000D76AD
		private protected bool componentsCached { protected get; private set; }

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x060027C7 RID: 10183 RVA: 0x000D94B6 File Offset: 0x000D76B6
		// (set) Token: 0x060027C8 RID: 10184 RVA: 0x000D94BE File Offset: 0x000D76BE
		public TIFactionState activePlayer { get; private set; }

		// Token: 0x060027C9 RID: 10185 RVA: 0x000D94C8 File Offset: 0x000D76C8
		public virtual void Initialize()
		{
			this.canvasManager = World.Active.GetExistingManager<CanvasManager>();
			this.Canvas = base.GetComponent<Canvas>();
			this.componentsToHide = new List<Behaviour>();
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.SetActivePlayer(true);
			if (this.Canvas == null)
			{
				throw new Exception("CanvasBase requires a Canvas component");
			}
			this.SetUltraWideScaling();
			this.UpdateUIScaling();
			Loc.SwapFonts(base.gameObject);
		}

		// Token: 0x060027CA RID: 10186 RVA: 0x000D9544 File Offset: 0x000D7744
		public virtual void Show()
		{
			if (this.Canvas != null)
			{
				if (this.componentsCached)
				{
					for (int i = 0; i < this.componentsToHide.Count; i++)
					{
						if (this.componentsToHide[i] != null)
						{
							this.componentsToHide[i].enabled = true;
						}
					}
					this.componentsToHide.Clear();
					this.componentsCached = false;
				}
				this.Canvas.enabled = true;
				this.SetUltraWideScaling();
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x000D95C8 File Offset: 0x000D77C8
		public virtual void Hide()
		{
			if (this.Canvas != null)
			{
				this.Canvas.enabled = false;
				if (!this.componentsCached)
				{
					foreach (Behaviour behaviour in base.GetComponentsInChildren<Behaviour>(true))
					{
						if (this.typesToHide.Contains(behaviour.GetType()) && behaviour.enabled)
						{
							this.componentsToHide.Add(behaviour);
							behaviour.enabled = false;
						}
					}
					this.componentsCached = true;
				}
			}
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x000D9646 File Offset: 0x000D7846
		public virtual void HideNoCache()
		{
			if (this.Canvas != null)
			{
				this.Canvas.enabled = false;
			}
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x000D9662 File Offset: 0x000D7862
		public virtual void OnDestroy()
		{
			CanvasManager canvasManager = this.canvasManager;
			if (canvasManager == null)
			{
				return;
			}
			canvasManager.ClearCanvas(this);
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x000D9675 File Offset: 0x000D7875
		public virtual bool Visible()
		{
			return this.Canvas.enabled;
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x060027CF RID: 10191 RVA: 0x000D9682 File Offset: 0x000D7882
		public bool Paused
		{
			get
			{
				return this.gameTime.Paused;
			}
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000D968F File Offset: 0x000D788F
		public virtual void Refresh()
		{
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x000D9691 File Offset: 0x000D7891
		public void RefreshScaling()
		{
			this.UpdateUIScaling();
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x000D9699 File Offset: 0x000D7899
		public void SetActivePlayer(bool startup)
		{
			this.activePlayer = GameControl.control.activePlayer;
			if (!startup)
			{
				this.UpdateActivePlayerUIElements(startup);
			}
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x000D96B5 File Offset: 0x000D78B5
		public virtual void UpdateActivePlayerUIElements(bool startup)
		{
		}

		// Token: 0x060027D4 RID: 10196 RVA: 0x000D96B7 File Offset: 0x000D78B7
		private IEnumerator ShuttingDownCanvas()
		{
			yield return CanvasControllerBase.delayPoint1;
			if (GameControl.canvasStack.IsVisible(base.name))
			{
				GameControl.canvasStack.Hide(base.name);
			}
			yield break;
		}

		// Token: 0x060027D5 RID: 10197 RVA: 0x000D96C8 File Offset: 0x000D78C8
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!GameControl.loadcycle100)
			{
				return;
			}
			if (GameControl.control.viewMgr == null)
			{
				return;
			}
			if (GameControl.control.viewMgr.earthObject != null)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.DeactivateRegionTooltips();
			}
		}

		// Token: 0x060027D6 RID: 10198 RVA: 0x000D9728 File Offset: 0x000D7928
		public void OnPointerExit(PointerEventData eventData)
		{
			if (!GameControl.loadcycle100)
			{
				return;
			}
			if (GameControl.control.viewMgr == null)
			{
				return;
			}
			if (GameControl.control.viewMgr.earthObject != null && !GameControl.control._canvasStack.IsShowingInfoScreen() && !TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !TutorialTip.TipVisible)
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ActivateRegionTooltips();
			}
		}

		// Token: 0x060027D7 RID: 10199 RVA: 0x000D97AC File Offset: 0x000D79AC
		public void OnApplicationFocus(bool focus)
		{
			if (!GameControl.loadcycle100)
			{
				return;
			}
			if (GameControl.control.viewMgr == null)
			{
				return;
			}
			if (focus)
			{
				if (!GameControl.control._canvasStack.IsShowingInfoScreen() && !TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !TutorialTip.TipVisible)
				{
					GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.ActivateRegionTooltips();
					return;
				}
			}
			else
			{
				GameControl.control.viewMgr.earthObject.GetComponent<SpaceObjectController>().mapController.DeactivateRegionTooltips();
			}
		}

		// Token: 0x060027D8 RID: 10200 RVA: 0x000D9838 File Offset: 0x000D7A38
		public virtual void SetUltraWideScaling()
		{
			float screenRatio = TIUtilities.GetScreenRatio();
			CanvasScaler component = base.gameObject.GetComponent<CanvasScaler>();
			if (screenRatio > 2.3f)
			{
				if (screenRatio > 4.5f)
				{
					component.matchWidthOrHeight = 0.95f;
					return;
				}
				component.matchWidthOrHeight = 0.9f;
				return;
			}
			else
			{
				if (screenRatio < 1.51f)
				{
					component.matchWidthOrHeight = 0.6f;
					return;
				}
				if (component.matchWidthOrHeight == 0.9f)
				{
					component.matchWidthOrHeight = 0.6f;
				}
				return;
			}
		}

		// Token: 0x060027D9 RID: 10201 RVA: 0x000D98AB File Offset: 0x000D7AAB
		public virtual void UpdateUIScaling()
		{
			base.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, (TIPlayerProfileManager.uiScaleSetting > 0) ? this.VerticalScaleValueLimit() : 1080f);
		}

		// Token: 0x060027DA RID: 10202 RVA: 0x000D98D8 File Offset: 0x000D7AD8
		public float VerticalScaleValueLimit()
		{
			return (float)((TIUtilities.GetScreenRatio() > 2.3f || TIUtilities.GetScreenRatio() < 1.7f) ? Mathf.Max(TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting], 1030) : TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting]);
		}

		// Token: 0x060027DB RID: 10203 RVA: 0x000D992A File Offset: 0x000D7B2A
		public void OnClickOpenCodex(string topic = "codex_welcome")
		{
			CodexController.ShowCodexPanel(topic);
		}

		// Token: 0x04001DCB RID: 7627
		[Header("Canvas Controls")]
		public bool hideIfNotTopOfStack;

		// Token: 0x04001DD0 RID: 7632
		protected List<Behaviour> componentsToHide;

		// Token: 0x04001DD1 RID: 7633
		private readonly List<Type> typesToHide = new List<Type>
		{
			typeof(GraphicRaycaster),
			typeof(TooltipTrigger),
			typeof(LayoutGroup),
			typeof(ScrollRect),
			typeof(Scrollbar),
			typeof(HorizontalLayoutGroup),
			typeof(VerticalLayoutGroup),
			typeof(GridLayoutGroup),
			typeof(RectMask2D)
		};

		// Token: 0x04001DD3 RID: 7635
		private static WaitForSeconds delayPoint1 = new WaitForSeconds(0.1f);
	}
}
