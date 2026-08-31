using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using FMOD.Studio;
using ModelShark;
using PavonisInteractive.TerraInvicta.Animations;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000558 RID: 1368
	public class MarkerController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x170004FB RID: 1275
		// (get) Token: 0x060023B7 RID: 9143 RVA: 0x000BD2F2 File Offset: 0x000BB4F2
		// (set) Token: 0x060023B8 RID: 9144 RVA: 0x000BD2FA File Offset: 0x000BB4FA
		public MarkerController.MarkerAnimations currentSelectionAnimation { get; private set; }

		// Token: 0x170004FC RID: 1276
		// (get) Token: 0x060023B9 RID: 9145 RVA: 0x000BD303 File Offset: 0x000BB503
		// (set) Token: 0x060023BA RID: 9146 RVA: 0x000BD30B File Offset: 0x000BB50B
		public TIGameState associatedState
		{
			get
			{
				return this.associatedState_;
			}
			set
			{
				if (this.Army != null)
				{
					this.UpdateArmyPathVisibility(true);
				}
				this.associatedState_ = value;
			}
		}

		// Token: 0x170004FD RID: 1277
		// (get) Token: 0x060023BB RID: 9147 RVA: 0x000BD329 File Offset: 0x000BB529
		// (set) Token: 0x060023BC RID: 9148 RVA: 0x000BD331 File Offset: 0x000BB531
		public string cachedAnimTrigger { get; private set; }

		// Token: 0x170004FE RID: 1278
		// (get) Token: 0x060023BD RID: 9149 RVA: 0x000BD33C File Offset: 0x000BB53C
		public float width
		{
			get
			{
				return this.rectTransform.rect.width;
			}
		}

		// Token: 0x170004FF RID: 1279
		// (get) Token: 0x060023BE RID: 9150 RVA: 0x000BD35C File Offset: 0x000BB55C
		public float scaledWidth
		{
			get
			{
				return this.ModelSizeAdjustment * this.rectTransform.rect.width * this.rectTransform.localScale.x;
			}
		}

		// Token: 0x17000500 RID: 1280
		// (get) Token: 0x060023BF RID: 9151 RVA: 0x000BD394 File Offset: 0x000BB594
		public float height
		{
			get
			{
				return this.rectTransform.rect.height;
			}
		}

		// Token: 0x17000501 RID: 1281
		// (get) Token: 0x060023C0 RID: 9152 RVA: 0x000BD3B4 File Offset: 0x000BB5B4
		public float scaledHeight
		{
			get
			{
				return this.ModelSizeAdjustment * this.rectTransform.rect.height * this.rectTransform.localScale.y;
			}
		}

		// Token: 0x17000502 RID: 1282
		// (get) Token: 0x060023C1 RID: 9153 RVA: 0x000BD3EC File Offset: 0x000BB5EC
		private float ModelSizeAdjustment
		{
			get
			{
				if (!this.modelActive)
				{
					return 1f;
				}
				if (this.markerType == MarkerType.Army)
				{
					return 1.3f;
				}
				return 1f;
			}
		}

		// Token: 0x17000503 RID: 1283
		// (get) Token: 0x060023C2 RID: 9154 RVA: 0x000BD410 File Offset: 0x000BB610
		public bool IsArmyMarker
		{
			get
			{
				return this.markerType == MarkerType.Army || this.markerType == MarkerType.NavalTransport;
			}
		}

		// Token: 0x17000504 RID: 1284
		// (get) Token: 0x060023C3 RID: 9155 RVA: 0x000BD427 File Offset: 0x000BB627
		public ArmyMarkerController ArmyMarkerController
		{
			get
			{
				if (this.armyMarkerController == null)
				{
					this.armyMarkerController = base.transform.GetComponentInParent<ArmyMarkerController>(true);
				}
				return this.armyMarkerController;
			}
		}

		// Token: 0x17000505 RID: 1285
		// (get) Token: 0x060023C4 RID: 9156 RVA: 0x000BD44F File Offset: 0x000BB64F
		public SeaMarkerController SeaMarkerController
		{
			get
			{
				if (this.seaMarkerController == null)
				{
					this.seaMarkerController = base.transform.GetComponentInParent<SeaMarkerController>(true);
				}
				return this.seaMarkerController;
			}
		}

		// Token: 0x17000506 RID: 1286
		// (get) Token: 0x060023C5 RID: 9157 RVA: 0x000BD477 File Offset: 0x000BB677
		public TIArmyState Army
		{
			get
			{
				TIGameState associatedState = this.associatedState;
				if (associatedState == null)
				{
					return null;
				}
				return associatedState.ref_army;
			}
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x000BD48C File Offset: 0x000BB68C
		public void Initialize(MarkerType mType, TIGameState location)
		{
			this.group.enabled = true;
			this.missionTimerObject.SetActive(false);
			this.topRightIconObject.SetActive(false);
			this.backgroundIconObject.SetActive(false);
			this.centralIconAnimObject.SetActive(false);
			this.nationImageObject.SetActive(false);
			this.factionImageObject.SetActive(false);
			this.armyFactionImageObject.SetActive(false);
			this.numberTextObject.SetActive(false);
			this.percentageBGObject.SetActive(false);
			this.controlPoint6PanelObject.SetActive(false);
			this.toHitTextObject.SetActive(false);
			this.selectionAnimObject.SetActive(false);
			this.armyMovementArrow.SetActive(false);
			this.hoverImageObject.SetActive(false);
			this.hoverImage.enabled = false;
			for (int i = 0; i <= 5; i++)
			{
				this.controlPointObject[i].SetActive(false);
				this.controlPoint6TextObject[i].SetActive(false);
				this.controlPointAnimObject[i].SetActive(false);
				this.CP6Status[i].gameObject.SetActive(false);
			}
			this.toHitText_Low.gameObject.SetActive(false);
			this.toHitText_Centered.gameObject.SetActive(false);
			this.toHitText_Lowest.gameObject.SetActive(false);
			this.UpdateUIScale();
			this.modelAnimatorController = this.model.GetComponent<ModelAnimatorController>();
			this.model.SetActive(false);
			this.markerCollider.enabled = false;
			this.markerType = mType;
			this.location = location;
			this.markerTooltipTrigger.enabled = true;
			switch (mType)
			{
			case MarkerType.Councilor:
				this.numberTextObject.SetActive(true);
				this.backgroundIconObject.SetActive(true);
				this.factionImageObject.SetActive(true);
				this.topRightIconObject.SetActive(false);
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = true;
				break;
			case MarkerType.AlienCouncilor:
				this.numberTextObject.SetActive(true);
				this.factionImageObject.SetActive(true);
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = true;
				break;
			case MarkerType.Army:
				this.numberTextObject.SetActive(true);
				this.percentageBGObject.SetActive(true);
				this.armyFactionImageObject.SetActive(true);
				this.nationImageObject.SetActive(true);
				this.topRightIconObject.SetActive(true);
				this.highPriority = true;
				this.armyMovementArrow.SetActive(true);
				this.armyMovementArrowImage.enabled = false;
				this.hoverImageObject.SetActive(true);
				this.hasModel = true;
				this.useBackgroundIcon = true;
				this.markerCollider.enabled = true;
				break;
			case MarkerType.HumanLaserFacility:
			case MarkerType.HumanMissionControlFacility:
				this.centralButton.enabled = true;
				this.numberTextObject.SetActive(true);
				this.shadow.enabled = false;
				this.highPriority = false;
				this.hoverImageObject.SetActive(true);
				this.SetHoverSprite(0);
				this.rectTransform.parent.GetComponent<Canvas>().sortingOrder = 4;
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.HumanLaunchFacility:
				this.centralButton.enabled = true;
				this.numberTextObject.SetActive(true);
				this.shadow.enabled = false;
				this.highPriority = false;
				this.topRightIconObject.SetActive(true);
				this.hoverImageObject.SetActive(true);
				this.SetHoverSprite(0);
				this.rectTransform.parent.GetComponent<Canvas>().sortingOrder = 4;
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.AlienActivity:
				this.shadow.enabled = false;
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.AlienLanding:
				this.shadow.enabled = false;
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.AlienCrashdown:
				this.shadow.enabled = false;
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.AlienFacility:
				this.shadow.enabled = false;
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.Xenoforming:
				this.shadow.enabled = false;
				this.highPriority = true;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = false;
				this.relativeScaling = 0.65f;
				break;
			case MarkerType.RegionalStatusIcon:
				this.centralButton.enabled = true;
				this.centralIcon.raycastTarget = true;
				this.shadow.enabled = false;
				this.highPriority = true;
				this.rectTransform.parent.GetComponent<Canvas>().sortingOrder = 4;
				this.hasModel = true;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.OccupationMarker:
				this.centralButton.enabled = false;
				this.centralIcon.raycastTarget = true;
				this.shadow.enabled = false;
				this.highPriority = true;
				this.rectTransform.parent.GetComponent<Canvas>().sortingOrder = 4;
				this.hasModel = false;
				this.useBackgroundIcon = false;
				break;
			case MarkerType.Capital:
			{
				this.centralButton.enabled = true;
				this.centralIcon.raycastTarget = true;
				this.controlPoint6PanelObject.SetActive(true);
				this.topRightIconObject.SetActive(true);
				this.topRightIcon.enabled = false;
				this.shadow.enabled = false;
				this.highPriority = true;
				Canvas componentInParent = base.GetComponentInParent<Canvas>();
				if (componentInParent != null)
				{
					componentInParent.sortingOrder = 4;
				}
				this.hasModel = false;
				this.useBackgroundIcon = false;
				for (int j = 0; j <= 5; j++)
				{
					this.CP6Status[j].gameObject.SetActive(true);
					this.CP6Status[j].enabled = false;
				}
				break;
			}
			case MarkerType.NavalTransport:
				this.numberTextObject.SetActive(true);
				this.percentageBGObject.SetActive(true);
				this.armyFactionImageObject.SetActive(true);
				this.nationImageObject.SetActive(true);
				this.topRightIconObject.SetActive(true);
				this.highPriority = true;
				this.armyMovementArrow.SetActive(true);
				this.armyMovementArrowImage.enabled = false;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = true;
				break;
			case MarkerType.Org:
				this.numberTextObject.SetActive(true);
				this.backgroundIconObject.SetActive(false);
				this.factionImageObject.SetActive(true);
				this.topRightIconObject.SetActive(false);
				this.highPriority = false;
				this.hoverImageObject.SetActive(true);
				this.hasModel = false;
				this.useBackgroundIcon = false;
				this.relativeScaling = 0.5f;
				this.numberTextObject.SetActive(true);
				break;
			case MarkerType.Canal:
				this.centralButton.enabled = false;
				this.centralIcon.raycastTarget = true;
				this.shadow.enabled = false;
				this.hasModel = false;
				this.useBackgroundIcon = false;
				break;
			}
			GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
			Loc.SwapFonts(base.gameObject);
			if (this.IsArmyMarker)
			{
				GameControl.eventManager.AddListener<ArmyPathChanged>(new EventManager.EventDelegate<ArmyPathChanged>(this.OnArmyPathChanged), null, location.ref_region, true, false);
				GameControl.eventManager.AddListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.OnOperationTargetSelected), null, null, true, false);
			}
		}

		// Token: 0x060023C7 RID: 9159 RVA: 0x000BDC32 File Offset: 0x000BBE32
		private void Update()
		{
			this.UpdateArmyPathVisibility(false);
			this.JustPointedAt = false;
		}

		// Token: 0x060023C8 RID: 9160 RVA: 0x000BDC44 File Offset: 0x000BBE44
		public void SetAmbientAudioClip(string path)
		{
			if (this.markerType != MarkerType.Army)
			{
				return;
			}
			if (!string.IsNullOrEmpty(path))
			{
				if (this.ambientSFX.isValid())
				{
					this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
					this.ambientSFX.Release();
				}
				this.ambientSFX = AudioManager.CreateFMODInstance(path);
				this.ambientSFX.SetTime(global::UnityEngine.Random.Range(0, this.ambientSFX.GetLength()));
				this.ambientSFX.SetVolume(0f);
			}
			if (string.IsNullOrEmpty(path) && this.ambientSFX.isValid())
			{
				this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
				this.ambientSFX.Release();
			}
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000BDCF4 File Offset: 0x000BBEF4
		public void SetHoverSprite(int setting)
		{
			switch (setting)
			{
			default:
				this.hoverImage.sprite = GeneralControlsController.cyanReticle;
				return;
			case 1:
				this.hoverImage.sprite = GeneralControlsController.redReticle;
				return;
			case 2:
				this.hoverImage.sprite = GeneralControlsController.greenReticle;
				return;
			}
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x000BDD45 File Offset: 0x000BBF45
		public void SetHoverSpriteByFaction(TIFactionState faction)
		{
			if (faction == GameControl.control.activePlayer)
			{
				this.SetHoverSprite(2);
				return;
			}
			if (faction != null)
			{
				this.SetHoverSprite(1);
				return;
			}
			this.SetHoverSprite(0);
		}

		// Token: 0x17000507 RID: 1287
		// (get) Token: 0x060023CB RID: 9163 RVA: 0x000BDD79 File Offset: 0x000BBF79
		// (set) Token: 0x060023CC RID: 9164 RVA: 0x000BDD81 File Offset: 0x000BBF81
		public bool IsPointedAt { get; private set; }

		// Token: 0x17000508 RID: 1288
		// (get) Token: 0x060023CD RID: 9165 RVA: 0x000BDD8A File Offset: 0x000BBF8A
		// (set) Token: 0x060023CE RID: 9166 RVA: 0x000BDD92 File Offset: 0x000BBF92
		public bool JustPointedAt { get; private set; }

		// Token: 0x060023CF RID: 9167 RVA: 0x000BDD9C File Offset: 0x000BBF9C
		public void OnPointerEnter(PointerEventData eventData)
		{
			this.IsPointedAt = true;
			this.JustPointedAt = true;
			if (this.hoverImageObject.activeSelf)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverMapElement", false, false);
				this.hoverImage.enabled = true;
				if (GeneralControlsController.UITargetingMode != null)
				{
					if (GeneralControlsController.UITargetingMode.GetPossibleTargets.Contains(this.associatedState))
					{
						TIInputManager.SetCursor(TIInputManager.targetCursorValid, true);
						return;
					}
					TIInputManager.SetCursor(TIInputManager.targetCursor, true);
				}
			}
		}

		// Token: 0x060023D0 RID: 9168 RVA: 0x000BDE14 File Offset: 0x000BC014
		public void OnPointerExit(PointerEventData eventData)
		{
			this.IsPointedAt = false;
			if (this.hoverImageObject.activeSelf)
			{
				this.hoverImage.enabled = false;
				if (GeneralControlsController.UITargetingMode != null)
				{
					TIInputManager.SetCursor(TIInputManager.targetCursor, true);
				}
			}
			if (this.markerType == MarkerType.Army)
			{
				this.UpdateArmyPathVisibility(false);
			}
		}

		// Token: 0x060023D1 RID: 9169 RVA: 0x000BDE63 File Offset: 0x000BC063
		public void OnRightClick()
		{
		}

		// Token: 0x060023D2 RID: 9170 RVA: 0x000BDE68 File Offset: 0x000BC068
		public void OnButtonPressed()
		{
			if (GameControl.control._canvasStack.IsShowingInfoScreen())
			{
				return;
			}
			if (this.del != null && (this.markerType != MarkerType.Army || !(this.Army.faction == GameControl.control.activePlayer) || !TIInputManager.IsShiftKeyDown))
			{
				this.del(this);
			}
			if (TIInputManager.IsControlKeyDown && TIMissionPhaseState.InMissionPhase())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				GameControl.eventManager.TriggerEvent(new MissionOptionsForTargetRequested(this.associatedState), null, Array.Empty<object>());
			}
			else if (this.markerType == MarkerType.RegionalStatusIcon)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				GameControl.eventManager.TriggerEvent(new RegionStateSelected(this.associatedState.ref_region), null, new object[] { this.associatedState.ref_region });
			}
			if (this.markerType == MarkerType.Army && this.Army.faction == GameControl.control.activePlayer && TIInputManager.IsShiftKeyDown)
			{
				OperationCanvasController.Singleton.AddArmyToMultiSelectGroup(this.Army);
			}
		}

		// Token: 0x060023D3 RID: 9171 RVA: 0x000BDF80 File Offset: 0x000BC180
		public void AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations animationValue)
		{
			if (this.currentSelectionAnimation != animationValue)
			{
				Sprite sprite;
				RuntimeAnimatorController runtimeAnimatorController;
				switch (animationValue)
				{
				case MarkerController.MarkerAnimations.Targeting:
					sprite = Resources.Load<Sprite>("Selection Reticle/ReticleSpriteSheet");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Selection Reticle/TI_selection_reticle");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					goto IL_0134;
				case MarkerController.MarkerAnimations.RedSquare:
					sprite = Resources.Load<Sprite>("Square Reticle/RedSquare/RedSquareReticleSS");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/RedSquare/RedAnimator");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					goto IL_0134;
				case MarkerController.MarkerAnimations.GreenSquare:
					sprite = Resources.Load<Sprite>("Square Reticle/GreenSquare/GreenSquareReticleSS");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/GreenSquare/GreenAnimator");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					goto IL_0134;
				case MarkerController.MarkerAnimations.AlienChevron:
					sprite = Resources.Load<Sprite>("AlienReticle/AlienReticle_Anim_SS");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("AlienReticle/AlienReticle_Anim_Animator");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					goto IL_0134;
				case MarkerController.MarkerAnimations.RedTargetSquare:
					sprite = Resources.Load<Sprite>("Square Reticle/RedTarget/RedTargetZoomSS");
					runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/RedTarget/RedAnimator");
					this.selectionAnimatorController = runtimeAnimatorController;
					this.selectionRenderer.sprite = sprite;
					goto IL_0134;
				}
				sprite = Resources.Load<Sprite>("Square Reticle/CyanSquare/CyanSquareReticleSS");
				runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Square Reticle/CyanSquare/CyanAnimator");
				this.selectionAnimatorController = runtimeAnimatorController;
				this.selectionRenderer.sprite = sprite;
				IL_0134:
				this.selectionAnim.runtimeAnimatorController = runtimeAnimatorController;
			}
			this.currentSelectionAnimation = animationValue;
		}

		// Token: 0x060023D4 RID: 9172 RVA: 0x000BE0D4 File Offset: 0x000BC2D4
		public void StartSelectionAnimation()
		{
			if (this.selectionAnimating)
			{
				this.StopSelectionAnimation();
			}
			if (base.gameObject.activeInHierarchy)
			{
				this.selectionAnimObject.SetActive(true);
				if (this.selectionAnimObject.activeInHierarchy)
				{
					this.selectionAnim.SetTrigger("Active");
					this.selectionAnimating = true;
				}
			}
		}

		// Token: 0x060023D5 RID: 9173 RVA: 0x000BE12C File Offset: 0x000BC32C
		public void StopSelectionAnimation()
		{
			if (this.selectionAnimating && this.selectionAnimObject.activeInHierarchy)
			{
				this.selectionAnim.SetTrigger("Exit");
			}
			this.selectionAnimObject.SetActive(false);
			this.selectionAnimating = false;
		}

		// Token: 0x060023D6 RID: 9174 RVA: 0x000BE168 File Offset: 0x000BC368
		private void SetCentralIconAnimating(bool setting)
		{
			this.animating = setting;
			if (this.markerType == MarkerType.Army || this.markerType == MarkerType.NavalTransport)
			{
				this.centralIcon.color = ((!setting && !this.modelActive) ? Color.white : Color.clear);
			}
			else
			{
				this.centralIcon.color = Color.white;
			}
			this.centralIconAnimObject.SetActive(setting && !this.modelActive);
		}

		// Token: 0x060023D7 RID: 9175 RVA: 0x000BE1E0 File Offset: 0x000BC3E0
		public void StartAnimations(string trigger)
		{
			if (this.animating)
			{
				this.StopCentralIconAnimation();
			}
			this.cachedAnimTrigger = trigger;
			this.SetCentralIconAnimating(true);
			if (this.centralIconAnimObject.activeInHierarchy)
			{
				this.centralIconAnimator.SetTrigger(trigger);
			}
			if (this.hasModel && base.gameObject.activeInHierarchy && this.modelActive)
			{
				if (trigger == "Fire")
				{
					this.modelAnimatorController.PlayAttack(true);
					return;
				}
				if (trigger == "Move")
				{
					this.modelAnimatorController.PlayMove(true);
				}
			}
		}

		// Token: 0x060023D8 RID: 9176 RVA: 0x000BE272 File Offset: 0x000BC472
		public void StopCentralIconAnimation()
		{
			if (this.animating && this.centralIconAnimObject.activeInHierarchy)
			{
				this.centralIconAnimator.SetTrigger("Exit");
			}
			this.SetCentralIconAnimating(false);
			this.cachedAnimTrigger = string.Empty;
		}

		// Token: 0x060023D9 RID: 9177 RVA: 0x000BE2AC File Offset: 0x000BC4AC
		public void AssignAnimationToCentralIconSprite(TIArmyState army, bool firing, bool atSea = false)
		{
			string text;
			string text2;
			if (!atSea)
			{
				text = army.AnimatorResource;
				text2 = (firing ? army.FightingSpriteSheet : army.MovingSpriteSheet);
			}
			else if (army.AlienRegularArmy)
			{
				text = "Alien_Ship_Animator";
				text2 = "SpriteSheet_alien_ship";
			}
			else
			{
				text = "Transport_Ship_Animator";
				text2 = "SpriteSheet_ship";
			}
			Sprite sprite = Resources.Load<Sprite>(text2);
			RuntimeAnimatorController runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(text);
			this.centralIconAnimatorController = runtimeAnimatorController;
			this.centralIconAnimator.runtimeAnimatorController = this.centralIconAnimatorController;
			this.centralIconSpriteRenderer.sprite = sprite;
			if (this.centralIconAnimObject.activeInHierarchy)
			{
				this.centralIconAnimator.SetTrigger("Base");
			}
		}

		// Token: 0x060023DA RID: 9178 RVA: 0x000BE348 File Offset: 0x000BC548
		public void AssignAnimationToCentralIconSprite(TIMissionTemplate mission, bool pending)
		{
			string iconAnimationController = mission.iconAnimationController;
			Sprite sprite = Resources.Load<Sprite>(pending ? mission.pendingAnimation : mission.resolvingAnimation);
			RuntimeAnimatorController runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(iconAnimationController);
			this.centralIconAnimatorController = runtimeAnimatorController;
			this.centralIconAnimator.runtimeAnimatorController = this.centralIconAnimatorController;
			this.centralIconSpriteRenderer.sprite = sprite;
		}

		// Token: 0x060023DB RID: 9179 RVA: 0x000BE39C File Offset: 0x000BC59C
		public void StartCPTargetingAnimation(int CP)
		{
			this.controlPointAnimObject[CP].SetActive(true);
			this.controlPointAnimObject[CP].GetComponent<Animator>().SetTrigger("Active");
		}

		// Token: 0x060023DC RID: 9180 RVA: 0x000BE3C3 File Offset: 0x000BC5C3
		public void StopCPTargetingAnimation(int CP)
		{
			if (this.controlPointAnimObject[CP].GetComponent<Animator>().isActiveAndEnabled)
			{
				this.controlPointAnimObject[CP].GetComponent<Animator>().SetTrigger("Stop");
				this.controlPointAnimObject[CP].SetActive(false);
			}
		}

		// Token: 0x060023DD RID: 9181 RVA: 0x000BE400 File Offset: 0x000BC600
		public void StopAllCPTargetingAnimations()
		{
			for (int i = 0; i <= 5; i++)
			{
				this.StopCPTargetingAnimation(i);
			}
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x000BE420 File Offset: 0x000BC620
		public void TriggerAttacking()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.modelAnimatorController.PlayAttack(false);
			}
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x000BE43B File Offset: 0x000BC63B
		public void TriggerDestruction()
		{
			if (base.gameObject.activeInHierarchy)
			{
				this.modelAnimatorController.PlayDestroyed(false);
			}
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x000BE458 File Offset: 0x000BC658
		public void TriggerExplosion()
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (this.explosionParticleSystem == null)
				{
					this.InitParticleEffect(ref this.explosionParticleSystem, "vfx/BigExplosion");
				}
				this.explosionParticleSystem.transform.Rotate(Vector3.up, (float)global::UnityEngine.Random.Range(0, 359));
				float num = 20f * (0.9f + global::UnityEngine.Random.Range(0f, 0.2f));
				this.explosionParticleSystem.transform.localScale = new Vector3(num, num, num);
				base.StartCoroutine(this.TriggerExplosionWaiter());
			}
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x000BE4F6 File Offset: 0x000BC6F6
		private IEnumerator TriggerExplosionWaiter()
		{
			yield return new WaitForSeconds(TIUtilities.RandomFloatValue());
			this.explosionParticleSystem.Play();
			ModelAnimatorController.AnimationState current = this.modelAnimatorController.GetAnimationState;
			this.modelAnimatorController.PlayDamaged(false);
			yield return new WaitForSeconds(this.explosionParticleSystem.main.duration);
			if (current != ModelAnimatorController.AnimationState.Damaged)
			{
				this.modelAnimatorController.PlayAnimationState(current);
			}
			yield break;
		}

		// Token: 0x060023E2 RID: 9186 RVA: 0x000BE505 File Offset: 0x000BC705
		public void TriggerLaunch()
		{
			if (this.launchParticleSystem == null)
			{
				this.InitParticleEffect(ref this.launchParticleSystem, "vfx/RocketTrail_MarkerPrefab");
			}
			this.launchParticleSystem.Play();
		}

		// Token: 0x060023E3 RID: 9187 RVA: 0x000BE531 File Offset: 0x000BC731
		public void TriggerGeneralFires()
		{
			if (this.fireParticleSystem == null)
			{
				this.InitParticleEffect(ref this.fireParticleSystem, "vfx/TinyFlames");
			}
			if (!this.fireParticleSystem.isPlaying)
			{
				this.fireParticleSystem.Play();
			}
		}

		// Token: 0x060023E4 RID: 9188 RVA: 0x000BE56A File Offset: 0x000BC76A
		public void StopGeneralFires()
		{
			if (this.fireParticleSystem.isPlaying)
			{
				this.fireParticleSystem.Stop();
			}
		}

		// Token: 0x060023E5 RID: 9189 RVA: 0x000BE584 File Offset: 0x000BC784
		public void TriggerArtilleryFlashes()
		{
			if (this.flashParticleSystem == null)
			{
				this.InitParticleEffect(ref this.flashParticleSystem, "vfx/Artillery Flashes");
			}
			if (!this.flashParticleSystem.isPlaying)
			{
				this.flashParticleSystem.Play();
			}
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x000BE5BD File Offset: 0x000BC7BD
		public void StopArtilleryFlashes()
		{
			if (this.flashParticleSystem == null)
			{
				this.InitParticleEffect(ref this.flashParticleSystem, "vfx/Artillery Flashes");
			}
			if (this.flashParticleSystem.isPlaying)
			{
				this.flashParticleSystem.Stop();
			}
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x000BE5F6 File Offset: 0x000BC7F6
		public void TriggerNuclearLaunch()
		{
			if (this.nukeLaunchParticleSystem == null)
			{
				this.InitParticleEffect(ref this.nukeLaunchParticleSystem, "vfx/Nuke Launch");
			}
			if (!this.nukeLaunchParticleSystem.isPlaying)
			{
				this.nukeLaunchParticleSystem.Play();
			}
		}

		// Token: 0x060023E8 RID: 9192 RVA: 0x000BE630 File Offset: 0x000BC830
		public void TriggerNuclearStrike()
		{
			if (this.nukeStrikeParticleSystem == null)
			{
				this.InitParticleEffect(ref this.nukeStrikeParticleSystem, "vfx/Nuke Strike");
			}
			if (!this.nukeStrikeParticleSystem.isPlaying)
			{
				this.nukeStrikeParticleSystem.Play();
				AudioManager.PlayOneShot("event:/SFX/Environment/trig_SFX_Nuclear_Detonation", false, false);
				this.TriggerGeneralFires();
			}
		}

		// Token: 0x060023E9 RID: 9193 RVA: 0x000BE686 File Offset: 0x000BC886
		public void TriggerLinearFires()
		{
			if (this.linearFireParticleSystem == null)
			{
				this.InitParticleEffect(ref this.linearFireParticleSystem, "vfx/LinearFlames");
			}
			if (!this.linearFireParticleSystem.isPlaying)
			{
				this.linearFireParticleSystem.Play();
			}
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x000BE6BF File Offset: 0x000BC8BF
		public void StopLinearFires()
		{
			if (this.linearFireParticleSystem.isPlaying)
			{
				this.linearFireParticleSystem.Stop();
			}
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x000BE6DC File Offset: 0x000BC8DC
		public void TriggerAlienLights(int intensity)
		{
			if (this.alienLightsParticleSystem == null)
			{
				this.InitParticleEffect(ref this.alienLightsParticleSystem, "vfx/Alien Mobile Lights");
			}
			this.alienLightsParticleSystem.main.maxParticles = intensity;
			if (!this.alienLightsParticleSystem.isPlaying)
			{
				this.alienLightsParticleSystem.Play();
			}
		}

		// Token: 0x060023EC RID: 9196 RVA: 0x000BE734 File Offset: 0x000BC934
		public void StopAlienLights()
		{
			if (this.alienLightsParticleSystem.isPlaying)
			{
				this.alienLightsParticleSystem.Stop();
			}
		}

		// Token: 0x060023ED RID: 9197 RVA: 0x000BE750 File Offset: 0x000BC950
		public void TriggerAlienGlow(int intensity)
		{
			this.alienGlowParticleSystem.main.maxParticles = intensity;
			if (!this.alienGlowParticleSystem.isPlaying)
			{
				this.alienGlowParticleSystem.Play();
			}
		}

		// Token: 0x060023EE RID: 9198 RVA: 0x000BE789 File Offset: 0x000BC989
		public void StopAlienGlow()
		{
			if (this.alienGlowParticleSystem.isPlaying)
			{
				this.alienGlowParticleSystem.Stop();
			}
		}

		// Token: 0x060023EF RID: 9199 RVA: 0x000BE7A4 File Offset: 0x000BC9A4
		public void TriggerTouchdown(IMarkerControl marker)
		{
			if (this.touchdownParticleSystem == null)
			{
				this.InitParticleEffect(ref this.touchdownParticleSystem, "vfx/Reentry Flames");
			}
			if (!this.touchdownParticleSystem.isPlaying)
			{
				AlienMarkerController alienMarkerController = marker as AlienMarkerController;
				if (alienMarkerController != null)
				{
					alienMarkerController.crashdownVisualizationFired = true;
					this.touchdownParticleSystem.Play();
					base.Invoke("TriggerLinearFires", 12f);
				}
			}
		}

		// Token: 0x060023F0 RID: 9200 RVA: 0x000BE810 File Offset: 0x000BCA10
		private void InitParticleEffect(ref ParticleSystem particleSystem, string path)
		{
			if (particleSystem != null)
			{
				return;
			}
			particleSystem = TIVFXManager.GetVFX(path, this.particleEffectsContainer.transform).GetComponent<ParticleSystem>();
			particleSystem.gameObject.SetActive(true);
			if (path != null)
			{
				if (path == "vfx/Nuke Launch")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.NukeLaunchGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.NukeLaunchGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.NukeLaunchGO.transform.localScale;
					return;
				}
				if (path == "vfx/LinearFlames")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.LinearFlamesGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.LinearFlamesGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.LinearFlamesGO.transform.localScale;
					return;
				}
				if (path == "vfx/TinyFlames")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.TinyFlamesGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.TinyFlamesGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.TinyFlamesGO.transform.localScale;
					return;
				}
				if (path == "vfx/RocketTrail_MarkerPrefab")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.RocketTrailGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.RocketTrailGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.RocketTrailGO.transform.localScale;
					return;
				}
				if (path == "vfx/BigExplosion")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.BigExplosionGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.BigExplosionGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.BigExplosionGO.transform.localScale;
					return;
				}
				if (path == "vfx/Reentry Flames")
				{
					particleSystem.transform.localPosition = TIVFXManager.Instance.ReentryFlamesGO.transform.localPosition;
					particleSystem.transform.localRotation = TIVFXManager.Instance.ReentryFlamesGO.transform.localRotation;
					particleSystem.transform.localScale = TIVFXManager.Instance.ReentryFlamesGO.transform.localScale;
					return;
				}
			}
			particleSystem.transform.localPosition = Vector3.zero;
			RectTransform component = particleSystem.GetComponent<RectTransform>();
			if (component != null)
			{
				component.anchoredPosition3D = Vector3.zero;
			}
			particleSystem.transform.localScale = Vector3.one;
			particleSystem.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060023F1 RID: 9201 RVA: 0x000BEB4A File Offset: 0x000BCD4A
		public void SetButtonPressed(MarkerController.OnMarkerButtonPressed del)
		{
			this.del = del;
			this.centralButton.enabled = del != null;
		}

		// Token: 0x060023F2 RID: 9202 RVA: 0x000BEB64 File Offset: 0x000BCD64
		public void CPButtonPressed(int buttonPosition)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			TIRegionState ref_region = this.location.ref_region;
			TIControlPoint controlPoint = ref_region.nation.GetControlPoint(buttonPosition);
			GameControl.eventManager.TriggerEvent(new ControlPointTargetSelected(controlPoint), null, Array.Empty<object>());
			TIUtilities.GotoGameState(ref_region, true, true, !GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint)), true, false, -1f);
			if (TIInputManager.IsControlKeyDown && TIMissionPhaseState.InMissionPhase())
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				GameControl.eventManager.TriggerEvent(new MissionOptionsForTargetRequested(controlPoint), null, Array.Empty<object>());
			}
		}

		// Token: 0x060023F3 RID: 9203 RVA: 0x000BEBFB File Offset: 0x000BCDFB
		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}

		// Token: 0x060023F4 RID: 9204 RVA: 0x000BEC0C File Offset: 0x000BCE0C
		public void MoveMarker(Vector2 newPosition, float time = 0f)
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (time == 0f)
				{
					this.rectTransform.localPosition = newPosition;
					return;
				}
				if (this.lerpCoroutine != null)
				{
					base.StopCoroutine(this.lerpCoroutine);
				}
				this.lerpCoroutine = this.LerpMarker(this.rectTransform.localPosition, newPosition, time);
				base.StartCoroutine(this.lerpCoroutine);
			}
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x000BEC7F File Offset: 0x000BCE7F
		private IEnumerator LerpMarker(Vector3 source, Vector3 target, float overTime)
		{
			float startTime = Time.time;
			while (Time.time < startTime + overTime)
			{
				this.rectTransform.localPosition = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
				yield return null;
			}
			this.rectTransform.localPosition = target;
			yield break;
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x000BECA3 File Offset: 0x000BCEA3
		private IEnumerator LerpMarkerWithAlpha(Vector3 startPosition, Vector3 endPosition, float startAlpha, float endAlpha, float overTime)
		{
			float startTime = Time.time;
			while (Time.time < startTime + overTime)
			{
				this.rectTransform.localPosition = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / overTime);
				float num = Mathf.Lerp(startAlpha, endAlpha, (Time.time - startTime) / overTime);
				this.group.alpha = num;
				yield return null;
			}
			this.rectTransform.localPosition = endPosition;
			this.group.alpha = endAlpha;
			yield break;
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x000BECD7 File Offset: 0x000BCED7
		public void SetMarkerModel(string ambientAudioPath = null)
		{
			this.modelAnimatorController.UpdateAnimatorController(this.cachedModel);
			this.SetAmbientAudioClip(ambientAudioPath);
		}

		// Token: 0x060023F8 RID: 9208 RVA: 0x000BECF4 File Offset: 0x000BCEF4
		public void SetModelSFXVolume(float distance)
		{
			if (!this.model.activeSelf || !GameControl.loadcycle100)
			{
				return;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(Camera.main.transform.position, this.model.transform.position - Camera.main.transform.position, out raycastHit, 40f))
			{
				if (raycastHit.transform != this.model.transform.GetChild(0))
				{
					if (this.ambientSFX.isValid())
					{
						this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
						this.ambientSFX.SetVolume(0f);
					}
					return;
				}
				float num = -(distance - 6.588f) / 18f;
				if (this.ambientSFX.isValid())
				{
					if (!this.ambientSFX.IsPlaying())
					{
						this.ambientSFX.Play();
					}
					this.ambientSFX.SetVolume(Mathf.Clamp(num, 0f, 1f));
				}
			}
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x000BEDF6 File Offset: 0x000BCFF6
		public void SetCentralIcon(string imagePath)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(imagePath, this.centralIcon);
		}

		// Token: 0x060023FA RID: 9210 RVA: 0x000BEE09 File Offset: 0x000BD009
		public void SetCentralIcon(Sprite image)
		{
			this.centralIcon.sprite = image;
		}

		// Token: 0x060023FB RID: 9211 RVA: 0x000BEE18 File Offset: 0x000BD018
		public void TurnOn3dElements()
		{
			this.model.SetActive(true);
			this.modelActive = true;
			if (this.useBackgroundIcon)
			{
				this.backgroundIcon.color = Color.clear;
			}
			this.centralIcon.color = Color.clear;
			this.centralIconAnimObject.SetActive(false);
			if (this.ambientSFX.isValid())
			{
				this.ambientSFX.setVolume(0f);
				this.ambientSFX.Play();
			}
		}

		// Token: 0x060023FC RID: 9212 RVA: 0x000BEE98 File Offset: 0x000BD098
		public void TurnOff3dElements()
		{
			this.model.transform.localScale = Vector3.one;
			this.model.SetActive(false);
			this.modelActive = false;
			this.hasBeenScaled = false;
			if (this.backgroundIcon.enabled)
			{
				this.backgroundIcon.color = this.backgroundColor;
			}
			if (this.animating)
			{
				this.StartAnimations(this.cachedAnimTrigger);
			}
			else
			{
				this.SetCentralIconAnimating(false);
			}
			if (this.ambientSFX.isValid())
			{
				this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x060023FD RID: 9213 RVA: 0x000BEF29 File Offset: 0x000BD129
		public void TurnOffAmbientVolume()
		{
			if (this.ambientSFX.isValid())
			{
				this.ambientSFX.setVolume(0f);
				this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x060023FE RID: 9214 RVA: 0x000BEF56 File Offset: 0x000BD156
		public void RemoveAmbientAudio()
		{
			if (this.ambientSFX.isValid())
			{
				this.ambientSFX.Stop(STOP_MODE.IMMEDIATE);
				this.ambientSFX.Release();
			}
		}

		// Token: 0x060023FF RID: 9215 RVA: 0x000BEF7E File Offset: 0x000BD17E
		public void SetTopRightIcon(Sprite sprite = null, ClearFlag clear = ClearFlag.NoChange)
		{
			if (clear == ClearFlag.TurnOff)
			{
				this.topRightIcon.enabled = false;
				return;
			}
			this.topRightIcon.sprite = sprite;
			if (clear == ClearFlag.TurnOn)
			{
				this.topRightIcon.enabled = true;
			}
		}

		// Token: 0x06002400 RID: 9216 RVA: 0x000BEFAD File Offset: 0x000BD1AD
		public void SetCentralIconShadow(bool drawShadow)
		{
			this.shadow.enabled = drawShadow;
		}

		// Token: 0x06002401 RID: 9217 RVA: 0x000BEFBC File Offset: 0x000BD1BC
		public void SetCPImages(TINationState nation = null, ClearFlag clear = ClearFlag.NoChange, bool activateButtons = true, TIFactionState targetingCouncil = null)
		{
			if (clear != ClearFlag.NoChange)
			{
				if (clear == ClearFlag.TurnOff || (nation != null && nation.NumNativeControlPoints == 6))
				{
					for (int i = 0; i <= 5; i++)
					{
						this.controlPointObject[i].SetActive(false);
						this.CP6Image[i].enabled = false;
						this.CP6Image[i].raycastTarget = false;
						this.CP6Image[i].GetComponent<Button>().enabled = false;
						this.CP6Status[i].enabled = false;
					}
				}
				int num = 0;
				if (clear == ClearFlag.TurnOn && nation != null)
				{
					for (int j = 0; j <= nation.maxControlPointIndex; j++)
					{
						if (nation.controlPoints[j].owned)
						{
							this.controlPointObject[j].SetActive(true);
							this.CP6Image[j].sprite = nation.controlPoints[j].GetIcon(false, false);
							this.CP6Image[j].enabled = true;
							this.CP6Image[j].raycastTarget = true;
							num++;
							if (targetingCouncil != null)
							{
								this.CP6Image[j].GetComponent<Button>().enabled = activateButtons;
							}
							if (nation.controlPoints[j].benefitsDisabled)
							{
								this.CP6Status[j].sprite = AssetCacheManager.smallCrackdownIcon;
								this.CP6Status[j].enabled = true;
							}
							else if (nation.controlPoints[j].defended)
							{
								this.CP6Status[j].sprite = AssetCacheManager.smallDefendInterestsIcon;
								this.CP6Status[j].enabled = true;
							}
							else
							{
								this.CP6Status[j].enabled = false;
							}
						}
						else
						{
							this.controlPointObject[j].SetActive(true);
							this.CP6Image[j].enabled = false;
							this.CP6Image[j].raycastTarget = false;
							this.CP6Image[j].GetComponent<Button>().enabled = false;
							this.CP6Status[j].enabled = false;
						}
					}
					for (int k = nation.numControlPoints; k < 6; k++)
					{
						this.controlPointObject[k].SetActive(false);
						this.CP6Image[k].enabled = false;
						this.CP6Image[k].raycastTarget = false;
						this.CP6Status[k].enabled = false;
						this.CP6Image[k].GetComponent<Button>().enabled = false;
					}
					if (num == 4)
					{
						this.CPGrid.startAxis = GridLayoutGroup.Axis.Vertical;
						return;
					}
					this.CPGrid.startAxis = GridLayoutGroup.Axis.Horizontal;
				}
			}
		}

		// Token: 0x06002402 RID: 9218 RVA: 0x000BF22C File Offset: 0x000BD42C
		public void SetPrimaryIconBackground(Sprite sprite, Color color, ClearFlag clear = ClearFlag.NoChange)
		{
			if (clear != ClearFlag.NoChange)
			{
				if (clear == ClearFlag.TurnOff)
				{
					this.backgroundIconObject.SetActive(false);
					return;
				}
				if (clear == ClearFlag.TurnOn)
				{
					this.backgroundIcon.sprite = sprite;
					this.backgroundColor = color;
					this.backgroundIconObject.SetActive(true);
					if (!this.model.activeSelf)
					{
						this.backgroundIconObject.GetComponent<Image>().color = color;
					}
				}
			}
		}

		// Token: 0x06002403 RID: 9219 RVA: 0x000BF290 File Offset: 0x000BD490
		public void SetCPToHitNumber(int CPvalue, string newValue = null, ClearFlag clear = ClearFlag.NoChange)
		{
			if (clear != ClearFlag.NoChange)
			{
				if (clear == ClearFlag.TurnOff)
				{
					this.controlPoint6TextObject[CPvalue].SetActive(false);
					return;
				}
				if (clear == ClearFlag.TurnOn)
				{
					this.controlPoint6TextObject[CPvalue].SetActive(true);
					this.CP6Text[CPvalue].enabled = true;
					if (newValue != null)
					{
						this.CP6Text[CPvalue].SetText(newValue);
					}
				}
			}
		}

		// Token: 0x06002404 RID: 9220 RVA: 0x000BF2E4 File Offset: 0x000BD4E4
		public void SetNumber(string newValue, ClearFlag clear = ClearFlag.NoChange, bool richText = false)
		{
			this.numberText.richText = richText;
			if (clear != ClearFlag.TurnOff)
			{
				this.numberText.SetText(newValue);
			}
			if (clear != ClearFlag.NoChange)
			{
				this.numberText.enabled = clear == ClearFlag.TurnOn;
			}
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x000BF314 File Offset: 0x000BD514
		public void SetToHitNumber(string value = "", bool useAutomaticSymbols = true, ClearFlag clear = ClearFlag.NoChange, int position = 0)
		{
			if (clear != ClearFlag.NoChange)
			{
				if (clear == ClearFlag.TurnOff)
				{
					this.toHitTextObject.SetActive(false);
					return;
				}
				string text = null;
				if (useAutomaticSymbols)
				{
					if (value != "")
					{
						text = "100%";
					}
				}
				else
				{
					text = ((value == "") ? null : value);
				}
				this.toHitTextObject.SetActive(true);
				if (text != null)
				{
					this.toHitText_Low.SetText(text);
					this.toHitText_Centered.SetText(text);
					this.toHitText_Lowest.SetText(text);
				}
				this.toHitText_Low.gameObject.SetActive(position == 0);
				this.toHitText_Centered.gameObject.SetActive(position == 1);
				this.toHitText_Lowest.gameObject.SetActive(position == 2);
			}
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x000BF3D6 File Offset: 0x000BD5D6
		public void SetFactionImage(Sprite sprite = null, ClearFlag clear = ClearFlag.NoChange)
		{
			if (sprite != null && clear != ClearFlag.TurnOff)
			{
				this.factionImage.sprite = sprite;
			}
			if (clear != ClearFlag.NoChange)
			{
				this.factionImage.enabled = clear == ClearFlag.TurnOn;
			}
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x000BF404 File Offset: 0x000BD604
		public void SetArmyFactionImage(Sprite image = null, ClearFlag clear = ClearFlag.NoChange)
		{
			if (image != null && clear != ClearFlag.TurnOff)
			{
				this.factionArmyImage.sprite = image;
				this.factionArmySprite.sprite = image;
			}
			if (clear != ClearFlag.NoChange)
			{
				this.factionArmyImage.enabled = false;
				this.factionArmySprite.enabled = clear == ClearFlag.TurnOn;
			}
		}

		// Token: 0x06002408 RID: 9224 RVA: 0x000BF454 File Offset: 0x000BD654
		public void SetNationImage(Sprite image = null, ClearFlag clear = ClearFlag.NoChange)
		{
			if (image != null && clear != ClearFlag.TurnOff)
			{
				this.nationImage.sprite = image;
			}
			if (clear != ClearFlag.NoChange)
			{
				this.nationImage.enabled = clear == ClearFlag.TurnOn;
			}
		}

		// Token: 0x06002409 RID: 9225 RVA: 0x000BF481 File Offset: 0x000BD681
		public void SetPercentage(float newValue, ClearFlag clear = ClearFlag.NoChange)
		{
			if (clear != ClearFlag.NoChange)
			{
				this.percentBG.SetActive(clear == ClearFlag.TurnOn);
			}
			if (this.percentBG.activeInHierarchy)
			{
				this.percentBar.fillAmount = newValue;
			}
		}

		// Token: 0x0600240A RID: 9226 RVA: 0x000BF4AE File Offset: 0x000BD6AE
		public void SetPercentColor(Color newColor)
		{
			this.percentBar.color = newColor;
		}

		// Token: 0x0600240B RID: 9227 RVA: 0x000BF4BC File Offset: 0x000BD6BC
		public void SetMissionTimer(TICouncilorState councilor)
		{
			if (TIGameState.Valid(councilor) && councilor.activeMission != null && councilor.activeMission.resolveTimeAssigned && GameControl.control.activePlayer.HasIntelOnCouncilorMission(councilor) && councilor.activeMission.startTime != null)
			{
				TimeSpan timeSpan = councilor.activeMission.resolveTime - councilor.activeMission.startTime;
				float num = (float)((TITimeState.Now() - councilor.activeMission.startTime).TotalSeconds / timeSpan.TotalSeconds);
				this.missionTimerImage.fillAmount = num;
				this.missionTimerObject.SetActive(true);
				return;
			}
			this.missionTimerObject.SetActive(false);
		}

		// Token: 0x0600240C RID: 9228 RVA: 0x000BF584 File Offset: 0x000BD784
		public string BuildTooltipText(string baseText, TIFactionState viewingCouncil, bool targeting = false, TIGameState target = null)
		{
			StringBuilder stringBuilder = new StringBuilder(baseText);
			if (targeting && target != null)
			{
				stringBuilder.AppendLine();
				stringBuilder.Append(TIMissionTemplate.MissionTargetingList(viewingCouncil, target));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x000BF5C1 File Offset: 0x000BD7C1
		public void SetTooltip(ParameterizedTextField.BuildStringOnTooltipHover del)
		{
			this.markerTooltipTrigger.SetDelegate("BodyText", del);
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x000BF5D4 File Offset: 0x000BD7D4
		public static string BuildInvalidTargetTooltip(List<string> reasons)
		{
			StringBuilder stringBuilder = new StringBuilder(TIUtilities.RedLine(Loc.T("UI.Markers.InvalidTarget"))).AppendLine();
			if (reasons.Count > 0)
			{
				foreach (string text in reasons)
				{
					if (text != "_Pass")
					{
						if (Loc.T(text)[0] != '-')
						{
							stringBuilder.Append("-");
						}
						stringBuilder.AppendLine(Loc.T(text));
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x000BF67C File Offset: 0x000BD87C
		public void UpdateArmyPathVisibility(bool forcePathUpdate = false)
		{
			if (!GameControl.loadcycle100)
			{
				return;
			}
			bool flag = false;
			if (this.IsArmyMarker)
			{
				if (this.IsPointedAt)
				{
					flag = true;
				}
				else
				{
					ArmyDetailController singleton = ArmyDetailController.Singleton;
					if (singleton != null && singleton.Canvas.enabled && (ArmyDetailController.Singleton.myArmy == this.Army || ArmyDetailController.Singleton.otherArmy == this.Army || OperationCanvasController.Singleton.GetSelectedArmies().Contains(this.Army)))
					{
						flag = true;
					}
				}
				if (this.armyMovementArrow != null)
				{
					this.armyMovementArrow.SetActive(!flag);
				}
			}
			if (flag && this.armyPath == null)
			{
				this.armyPath = global::UnityEngine.Object.Instantiate<ArmyPathController>(this.armyPathPrefab, GameStateManager.Earth().controller.modelLink.transform, false);
				this.armyPath.MarkerController = this;
			}
			if (this.armyPath != null && this.armyPath.gameObject.activeSelf != flag)
			{
				this.armyPath.gameObject.SetActive(flag);
			}
			if (flag && forcePathUpdate)
			{
				this.armyPath.UpdateVisualization(true);
			}
		}

		// Token: 0x06002410 RID: 9232 RVA: 0x000BF7AC File Offset: 0x000BD9AC
		public void OnArmyPathChanged(ArmyPathChanged e)
		{
			if (this == null)
			{
				return;
			}
			if (this.IsArmyMarker)
			{
				this.UpdateArmyPathVisibility(true);
			}
		}

		// Token: 0x06002411 RID: 9233 RVA: 0x000BF7C8 File Offset: 0x000BD9C8
		public void OnOperationTargetSelected(OperationTargettedEvent e)
		{
			if (this == null)
			{
				return;
			}
			if (e.actorState.isArmyState && e.target.isRegionState && this.IsArmyMarker)
			{
				if (this.markerType == MarkerType.Army)
				{
					this.ArmyMarkerController.MoveToFront(e.actorState.ref_army);
				}
				else if (this.markerType == MarkerType.NavalTransport)
				{
					this.SeaMarkerController.MoveToFront(e.actorState.ref_army);
				}
				this.UpdateArmyPathVisibility(true);
			}
		}

		// Token: 0x06002412 RID: 9234 RVA: 0x000BF849 File Offset: 0x000BDA49
		private void OnUIScaleChanged(UIScaleSettingChange e)
		{
			this.UpdateUIScale();
		}

		// Token: 0x06002413 RID: 9235 RVA: 0x000BF854 File Offset: 0x000BDA54
		private void UpdateUIScale()
		{
			float num = 72f;
			num *= TIUtilities.UIScaleFactor();
			this.toHitText_Centered.fontSizeMax = num;
			this.toHitText_Low.fontSizeMax = num;
			this.toHitText_Lowest.fontSizeMax = num;
		}

		// Token: 0x06002414 RID: 9236 RVA: 0x000BF894 File Offset: 0x000BDA94
		private void OnDestroy()
		{
			this.RemoveAmbientAudio();
			if (this.nukeLaunchParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Nuke Launch", this.nukeLaunchParticleSystem.gameObject);
			}
			if (this.explosionParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/BigExplosion", this.explosionParticleSystem.gameObject);
			}
			if (this.launchParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/RocketTrail_MarkerPrefab", this.launchParticleSystem.gameObject);
			}
			if (this.fireParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/TinyFlames", this.fireParticleSystem.gameObject);
			}
			if (this.flashParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Artillery Flashes", this.flashParticleSystem.gameObject);
			}
			if (this.alienLightsParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Alien Mobile Lights", this.alienLightsParticleSystem.gameObject);
			}
			if (this.nukeStrikeParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Nuke Strike", this.nukeStrikeParticleSystem.gameObject);
			}
			if (this.linearFireParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/LinearFlames", this.linearFireParticleSystem.gameObject);
			}
			if (this.alienGlowParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Alien Glow", this.alienGlowParticleSystem.gameObject);
			}
			if (this.touchdownParticleSystem != null)
			{
				TIVFXManager.ReturnVFX("vfx/Reentry Flames", this.touchdownParticleSystem.gameObject);
			}
			GameControl.eventManager.RemoveListener<ArmyPathChanged>(new EventManager.EventDelegate<ArmyPathChanged>(this.OnArmyPathChanged), null);
			GameControl.eventManager.RemoveListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.OnOperationTargetSelected), null);
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
			if (this.armyPath != null)
			{
				global::UnityEngine.Object.Destroy(this.armyPath.gameObject);
			}
		}

		// Token: 0x04001AF2 RID: 6898
		public RectTransform rectTransform;

		// Token: 0x04001AF3 RID: 6899
		public CanvasGroup group;

		// Token: 0x04001AF4 RID: 6900
		public GameObject missionTimerObject;

		// Token: 0x04001AF5 RID: 6901
		public Image missionTimerImage;

		// Token: 0x04001AF6 RID: 6902
		public GameObject armyMovementArrow;

		// Token: 0x04001AF7 RID: 6903
		public Image armyMovementArrowImage;

		// Token: 0x04001AF8 RID: 6904
		public ArmyPathController armyPathPrefab;

		// Token: 0x04001AF9 RID: 6905
		private ArmyPathController armyPath;

		// Token: 0x04001AFA RID: 6906
		public List<TIRegionState> prospectiveDestinationQueue = new List<TIRegionState>();

		// Token: 0x04001AFB RID: 6907
		public GameObject topRightIconObject;

		// Token: 0x04001AFC RID: 6908
		public Image topRightIcon;

		// Token: 0x04001AFD RID: 6909
		public GameObject backgroundIconObject;

		// Token: 0x04001AFE RID: 6910
		public Image backgroundIcon;

		// Token: 0x04001AFF RID: 6911
		public bool useBackgroundIcon;

		// Token: 0x04001B00 RID: 6912
		public Color backgroundColor;

		// Token: 0x04001B01 RID: 6913
		public GameObject primaryCentralIconObject;

		// Token: 0x04001B02 RID: 6914
		public GameObject centralIconAnimObject;

		// Token: 0x04001B03 RID: 6915
		public Image centralIcon;

		// Token: 0x04001B04 RID: 6916
		public Button centralButton;

		// Token: 0x04001B05 RID: 6917
		public SpriteRenderer factionArmySprite;

		// Token: 0x04001B06 RID: 6918
		public Shadow shadow;

		// Token: 0x04001B07 RID: 6919
		public Animator centralIconAnimator;

		// Token: 0x04001B08 RID: 6920
		public SpriteRenderer centralIconSpriteRenderer;

		// Token: 0x04001B09 RID: 6921
		private RuntimeAnimatorController centralIconAnimatorController;

		// Token: 0x04001B0A RID: 6922
		public bool animating;

		// Token: 0x04001B0B RID: 6923
		public GameObject numberTextObject;

		// Token: 0x04001B0C RID: 6924
		public TMP_Text numberText;

		// Token: 0x04001B0D RID: 6925
		public GameObject factionImageObject;

		// Token: 0x04001B0E RID: 6926
		public Image factionImage;

		// Token: 0x04001B0F RID: 6927
		public GameObject armyFactionImageObject;

		// Token: 0x04001B10 RID: 6928
		public Image factionArmyImage;

		// Token: 0x04001B11 RID: 6929
		public GameObject nationImageObject;

		// Token: 0x04001B12 RID: 6930
		public Image nationImage;

		// Token: 0x04001B13 RID: 6931
		public GameObject percentageBGObject;

		// Token: 0x04001B14 RID: 6932
		public GameObject percentBG;

		// Token: 0x04001B15 RID: 6933
		public Image percentBar;

		// Token: 0x04001B16 RID: 6934
		public GameObject toHitTextObject;

		// Token: 0x04001B17 RID: 6935
		public TMP_Text toHitText_Centered;

		// Token: 0x04001B18 RID: 6936
		public TMP_Text toHitText_Low;

		// Token: 0x04001B19 RID: 6937
		public TMP_Text toHitText_Lowest;

		// Token: 0x04001B1A RID: 6938
		public GameObject controlPoint6PanelObject;

		// Token: 0x04001B1B RID: 6939
		public GridLayoutGroup CPGrid;

		// Token: 0x04001B1C RID: 6940
		public GameObject[] controlPointObject;

		// Token: 0x04001B1D RID: 6941
		public Image[] CP6Image = new Image[6];

		// Token: 0x04001B1E RID: 6942
		public Image[] CP6Status;

		// Token: 0x04001B1F RID: 6943
		public TMP_Text[] CP6Text = new TMP_Text[6];

		// Token: 0x04001B20 RID: 6944
		public GameObject[] controlPoint6TextObject;

		// Token: 0x04001B21 RID: 6945
		public GameObject[] controlPointAnimObject;

		// Token: 0x04001B22 RID: 6946
		public Animator[] controlPointAnimator;

		// Token: 0x04001B23 RID: 6947
		public GameObject selectionAnimObject;

		// Token: 0x04001B24 RID: 6948
		public Animator selectionAnim;

		// Token: 0x04001B25 RID: 6949
		public SpriteRenderer selectionRenderer;

		// Token: 0x04001B26 RID: 6950
		private RuntimeAnimatorController selectionAnimatorController;

		// Token: 0x04001B27 RID: 6951
		public bool selectionAnimating;

		// Token: 0x04001B29 RID: 6953
		public GameObject hoverImageObject;

		// Token: 0x04001B2A RID: 6954
		public Image hoverImage;

		// Token: 0x04001B2B RID: 6955
		public TooltipTrigger markerTooltipTrigger;

		// Token: 0x04001B2C RID: 6956
		public MarkerType markerType;

		// Token: 0x04001B2D RID: 6957
		public CapsuleCollider markerCollider;

		// Token: 0x04001B2E RID: 6958
		private TIGameState location;

		// Token: 0x04001B2F RID: 6959
		private TIGameState associatedState_;

		// Token: 0x04001B30 RID: 6960
		public bool highPriority;

		// Token: 0x04001B31 RID: 6961
		public bool hasModel;

		// Token: 0x04001B32 RID: 6962
		public GameObject model;

		// Token: 0x04001B33 RID: 6963
		public GameObject cachedModel;

		// Token: 0x04001B34 RID: 6964
		public ModelAnimatorController modelAnimatorController;

		// Token: 0x04001B35 RID: 6965
		public bool hasBeenScaled;

		// Token: 0x04001B36 RID: 6966
		public bool modelActive;

		// Token: 0x04001B37 RID: 6967
		public float relativeScaling = 1f;

		// Token: 0x04001B38 RID: 6968
		public GameObject particleEffectsContainer;

		// Token: 0x04001B39 RID: 6969
		public ParticleSystem explosionParticleSystem;

		// Token: 0x04001B3A RID: 6970
		public ParticleSystem fireParticleSystem;

		// Token: 0x04001B3B RID: 6971
		public ParticleSystem launchParticleSystem;

		// Token: 0x04001B3C RID: 6972
		public ParticleSystem flashParticleSystem;

		// Token: 0x04001B3D RID: 6973
		public ParticleSystem nukeLaunchParticleSystem;

		// Token: 0x04001B3E RID: 6974
		public ParticleSystem nukeStrikeParticleSystem;

		// Token: 0x04001B3F RID: 6975
		public ParticleSystem linearFireParticleSystem;

		// Token: 0x04001B40 RID: 6976
		public ParticleSystem alienLightsParticleSystem;

		// Token: 0x04001B41 RID: 6977
		public ParticleSystem alienGlowParticleSystem;

		// Token: 0x04001B42 RID: 6978
		public ParticleSystem touchdownParticleSystem;

		// Token: 0x04001B43 RID: 6979
		public EventInstance ambientSFX;

		// Token: 0x04001B45 RID: 6981
		private ArmyMarkerController armyMarkerController;

		// Token: 0x04001B46 RID: 6982
		private SeaMarkerController seaMarkerController;

		// Token: 0x04001B47 RID: 6983
		private IEnumerator lerpCoroutine;

		// Token: 0x04001B48 RID: 6984
		private MarkerController.OnMarkerButtonPressed del;

		// Token: 0x02000CDA RID: 3290
		// (Invoke) Token: 0x06006E30 RID: 28208
		public delegate void OnMarkerButtonPressed(MarkerController controller);

		// Token: 0x02000CDB RID: 3291
		public enum MarkerAnimations
		{
			// Token: 0x04004FA6 RID: 20390
			None,
			// Token: 0x04004FA7 RID: 20391
			Targeting,
			// Token: 0x04004FA8 RID: 20392
			RedSquare,
			// Token: 0x04004FA9 RID: 20393
			CyanSquare,
			// Token: 0x04004FAA RID: 20394
			GreenSquare,
			// Token: 0x04004FAB RID: 20395
			AlienChevron,
			// Token: 0x04004FAC RID: 20396
			RedTargetSquare
		}
	}
}
