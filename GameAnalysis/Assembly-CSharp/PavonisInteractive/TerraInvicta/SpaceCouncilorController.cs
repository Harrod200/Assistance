using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A2 RID: 1442
	public class SpaceCouncilorController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x17000576 RID: 1398
		// (get) Token: 0x060026D0 RID: 9936 RVA: 0x000D3430 File Offset: 0x000D1630
		// (set) Token: 0x060026D1 RID: 9937 RVA: 0x000D3438 File Offset: 0x000D1638
		public MarkerController.MarkerAnimations currentSelectionAnimation { get; private set; }

		// Token: 0x060026D2 RID: 9938 RVA: 0x000D3441 File Offset: 0x000D1641
		public void Awake()
		{
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			base.gameObject.SetActive(true);
		}

		// Token: 0x060026D3 RID: 9939 RVA: 0x000D3460 File Offset: 0x000D1660
		private void InitializeCommon()
		{
			this.parentMesh = base.transform.parent.transform.parent.GetComponent<MeshRenderer>();
			this.activePlayer = GameControl.control.activePlayer;
			this.spaceObjectSelection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.selectionAnimObject.SetActive(false);
			base.gameObject.SetActive(false);
		}

		// Token: 0x060026D4 RID: 9940 RVA: 0x000D34C5 File Offset: 0x000D16C5
		public void Initialize(HabModelController modelController, TIHabState habState)
		{
			this.InitializeCommon();
			this.habModelController = modelController;
			this.parentState = habState;
		}

		// Token: 0x060026D5 RID: 9941 RVA: 0x000D34DB File Offset: 0x000D16DB
		public void Initialize(ShipModelController modelController, TISpaceShipState shipState)
		{
			this.InitializeCommon();
			this.parentState = shipState;
		}

		// Token: 0x060026D6 RID: 9942 RVA: 0x000D34EC File Offset: 0x000D16EC
		public void UpdateController(TICouncilorState councilor)
		{
			this.ClearCouncilor();
			if (!TIGameState.Valid(councilor) || councilor.faction == null)
			{
				return;
			}
			base.gameObject.SetActive(true);
			this.councilor = councilor;
			CouncilorView viewofCouncilor = this.activePlayer.GetViewofCouncilor(councilor);
			GameControl.assetLoader.LoadAssetForImageAssignment(viewofCouncilor.mapIconResourcePathCurrent, this.councilorIcon);
			if (viewofCouncilor.factionCurrent != null)
			{
				this.councilorBackground.color = councilor.faction.template.color;
				this.councilorBackground.enabled = true;
				this.factionIcon.sprite = councilor.faction.factionIcon64;
				this.factionIcon.enabled = true;
			}
			else
			{
				this.councilorBackground.enabled = false;
				this.factionIcon.enabled = false;
			}
			this.councilorName.SetText(viewofCouncilor.displayNameCurrent);
			this.primaryCanvas.enabled = true;
			this.currentlyActive = true;
			this.councilorName.enabled = false;
			this.hoverImage.enabled = false;
			this.SetHoverSpriteByFaction(viewofCouncilor.factionCurrent);
			if (viewofCouncilor.HasMission)
			{
				this.AssignAnimationToCentralIconSprite(viewofCouncilor.GetActiveMission.missionTemplate, true);
				this.StartCentralIconAnimation("Pending");
			}
			else if (viewofCouncilor.GetCompletedMission != null && viewofCouncilor.GetCompletedMission.missionTemplate.persistentEffect)
			{
				this.AssignAnimationToCentralIconSprite(viewofCouncilor.GetCompletedMission.missionTemplate, false);
				this.StartCentralIconAnimation("Resolving");
			}
			else
			{
				this.StopCentralIconAnimation();
			}
			if (GeneralControlsController.UISelectedAssetState == councilor)
			{
				this.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				this.StartSelectionAnimation();
				GameControl.eventManager.AddListener<CurrentAssetDeSelected>(new EventManager.EventDelegate<CurrentAssetDeSelected>(this.OnCouncilorAssetDeselected), null, null, true, true);
			}
			else if (GeneralControlsController.UIOtherSelectedState == councilor)
			{
				if (councilor.agentForFaction == this.activePlayer)
				{
					this.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
				}
				else
				{
					this.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.RedSquare);
				}
				this.StartSelectionAnimation();
				GameControl.eventManager.AddListener<CurrentOtherStateDeselected>(new EventManager.EventDelegate<CurrentOtherStateDeselected>(this.OnCouncilorOtherStateDeselected), null, null, true, true);
			}
			else
			{
				this.StopSelectionAnimation();
			}
			this.SetTooltip(() => this.SetStackTooltip(councilor));
			if (viewofCouncilor.HasMission)
			{
				TIMissionState getActiveMission = viewofCouncilor.GetActiveMission;
				float successChance = getActiveMission.missionTemplate.resolutionMethod.GetSuccessChance(getActiveMission.missionTemplate, councilor, getActiveMission.target, getActiveMission.resources, true);
				string text;
				if (getActiveMission.missionTemplate.resolutionMethod.automaticSuccess)
				{
					if (successChance >= 1f)
					{
						text = "100%";
					}
					else
					{
						text = "<sprite=4>";
					}
				}
				else
				{
					text = successChance.ToPercent("P0");
				}
				this.tohitValue.enabled = true;
				this.tohitValue.SetText(text);
			}
			else
			{
				this.tohitValue.enabled = false;
			}
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateMarker), null, councilor, false, false);
			GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMarker), null, councilor, false, false);
			GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMarker), null, councilor, false, false);
			GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateMarker), null, councilor, false, false);
		}

		// Token: 0x060026D7 RID: 9943 RVA: 0x000D3878 File Offset: 0x000D1A78
		public void ClearListeners()
		{
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateMarker), null);
			GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateMarker), null);
		}

		// Token: 0x060026D8 RID: 9944 RVA: 0x000D38E1 File Offset: 0x000D1AE1
		public void ClearCouncilor()
		{
			if (this != null)
			{
				base.gameObject.SetActive(false);
			}
			this.councilor = null;
			this.currentlyActive = false;
			this.ClearListeners();
		}

		// Token: 0x060026D9 RID: 9945 RVA: 0x000D390C File Offset: 0x000D1B0C
		public void OnDestroy()
		{
			this.ClearListeners();
		}

		// Token: 0x060026DA RID: 9946 RVA: 0x000D3914 File Offset: 0x000D1B14
		private void UpdateMarker(CouncilorPositionUpdated e)
		{
			if (e.councilor.location != this.parentState)
			{
				this.ClearCouncilor();
				return;
			}
			this.councilorDataDirty = true;
		}

		// Token: 0x060026DB RID: 9947 RVA: 0x000D393C File Offset: 0x000D1B3C
		private void UpdateMarker(CouncilCompositionChanged e)
		{
			this.councilorDataDirty = true;
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x000D3945 File Offset: 0x000D1B45
		private void UpdateMarker(CouncilorVisibilityChanged e)
		{
			this.councilorDataDirty = true;
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x000D394E File Offset: 0x000D1B4E
		private void UpdateMarker(CouncilorMissionUpdated e)
		{
			this.councilorDataDirty = true;
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x000D3957 File Offset: 0x000D1B57
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.habModelController != null)
			{
				this.habModelController.mouseOverHabUIIcon = true;
			}
			this.hoverImage.enabled = true;
			this.councilorName.enabled = true;
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x000D398B File Offset: 0x000D1B8B
		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.habModelController != null)
			{
				this.habModelController.mouseOverHabUIIcon = false;
			}
			this.hoverImage.enabled = false;
			this.councilorName.enabled = false;
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x000D39BF File Offset: 0x000D1BBF
		public void OnClicked()
		{
			this.spaceObjectSelection.BlockThisFrame = true;
			this.UpdateController(this.councilor);
			if (TIGameState.Valid(this.councilor))
			{
				SoundEffectController.PlaySelectSound(this.councilor);
				TIUtilities.GotoGameState(this.councilor, false, true, true);
			}
		}

		// Token: 0x060026E1 RID: 9953 RVA: 0x000D39FF File Offset: 0x000D1BFF
		public void SetTooltip(ParameterizedTextField.BuildStringOnTooltipHover del)
		{
			this.markerTooltipTrigger.SetDelegate("BodyText", del);
		}

		// Token: 0x060026E2 RID: 9954 RVA: 0x000D3A14 File Offset: 0x000D1C14
		private string SetStackTooltip(TICouncilorState councilor)
		{
			StringBuilder stringBuilder = new StringBuilder();
			CouncilorView viewofCouncilor = this.activePlayer.GetViewofCouncilor(councilor);
			if (councilor.HasMission && this.activePlayer.HasIntelOnCouncilorMission(councilor))
			{
				if (councilor.activeMission.resolveTimeAssigned)
				{
					stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.TooltipWithMissionResolveTime", new object[]
					{
						councilor.faction.template.inlineColorString,
						viewofCouncilor.displayNameCurrent,
						viewofCouncilor.councilorJobStringCurrent,
						viewofCouncilor.currentMissionDisplayName,
						viewofCouncilor.currentMissionTargetDisplayName,
						viewofCouncilor.currentMissionResolveTime
					}));
				}
				else
				{
					stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.TooltipWithMission", new object[]
					{
						councilor.faction.template.inlineColorString,
						viewofCouncilor.displayNameCurrent,
						viewofCouncilor.councilorJobStringCurrent,
						viewofCouncilor.currentMissionDisplayName,
						viewofCouncilor.currentMissionTargetDisplayName
					}));
				}
			}
			else if (this.activePlayer.HasIntelOnCouncilorBasicData(councilor))
			{
				stringBuilder.AppendLine(Loc.T("UI.Markers.CouncilorMarker.Tooltip", new object[]
				{
					councilor.faction.template.inlineColorString,
					viewofCouncilor.displayNameCurrent,
					viewofCouncilor.councilorJobStringCurrent
				}));
			}
			else
			{
				stringBuilder.AppendLine(viewofCouncilor.displayNameCurrent);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060026E3 RID: 9955 RVA: 0x000D3B74 File Offset: 0x000D1D74
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

		// Token: 0x060026E4 RID: 9956 RVA: 0x000D3CC8 File Offset: 0x000D1EC8
		public void StartSelectionAnimation()
		{
			if (this.selectionAnimating)
			{
				this.StopSelectionAnimation();
			}
			this.selectionAnimObject.SetActive(true);
			if (this.selectionAnimObject.activeInHierarchy)
			{
				this.selectionAnim.SetTrigger("Active");
				this.selectionAnimating = true;
			}
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000D3D08 File Offset: 0x000D1F08
		public void StopSelectionAnimation()
		{
			if (this.selectionAnimating)
			{
				if (this.selectionAnimObject.activeInHierarchy)
				{
					this.selectionAnim.SetTrigger("Exit");
				}
				this.selectionAnimObject.SetActive(false);
				this.selectionAnimating = false;
			}
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x000D3D44 File Offset: 0x000D1F44
		public void AssignAnimationToCentralIconSprite(TIMissionTemplate mission, bool pending)
		{
			string iconAnimationController = mission.iconAnimationController;
			Sprite sprite = Resources.Load<Sprite>(pending ? mission.pendingAnimation : mission.resolvingAnimation);
			RuntimeAnimatorController runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(iconAnimationController);
			this.centralIconAnimatorController = runtimeAnimatorController;
			this.centralIconAnimator.runtimeAnimatorController = this.centralIconAnimatorController;
			this.centralIconSpriteRenderer.sprite = sprite;
		}

		// Token: 0x060026E7 RID: 9959 RVA: 0x000D3D98 File Offset: 0x000D1F98
		public void StartCentralIconAnimation(string trigger)
		{
			if (this.centralIconAnimating)
			{
				this.StopCentralIconAnimation();
			}
			this.centralIconAnimObject.SetActive(true);
			if (this.centralIconAnimObject.activeInHierarchy)
			{
				this.centralIconAnimator.SetTrigger(trigger);
				this.centralIconAnimating = true;
			}
			this.cachedAnimTrigger = trigger;
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x000D3DE6 File Offset: 0x000D1FE6
		public void StopCentralIconAnimation()
		{
			if (this.centralIconAnimating)
			{
				if (this.centralIconAnimObject.activeInHierarchy)
				{
					this.centralIconAnimator.SetTrigger("Exit");
				}
				this.centralIconAnimObject.SetActive(false);
				this.centralIconAnimating = false;
			}
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000D3E20 File Offset: 0x000D2020
		private void OnCouncilorAssetDeselected(CurrentAssetDeSelected e)
		{
			this.councilorDataDirty = true;
		}

		// Token: 0x060026EA RID: 9962 RVA: 0x000D3E29 File Offset: 0x000D2029
		private void OnCouncilorOtherStateDeselected(CurrentOtherStateDeselected e)
		{
			this.councilorDataDirty = true;
		}

		// Token: 0x060026EB RID: 9963 RVA: 0x000D3E34 File Offset: 0x000D2034
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

		// Token: 0x060026EC RID: 9964 RVA: 0x000D3E85 File Offset: 0x000D2085
		public void SetHoverSpriteByFaction(TIFactionState faction)
		{
			if (faction == this.activePlayer)
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

		// Token: 0x060026ED RID: 9965 RVA: 0x000D3EB8 File Offset: 0x000D20B8
		public void Update()
		{
			if (!TIGlobalValuesState.isSpaceCombatEnabled && this.primaryCanvas.enabled)
			{
				if (this.councilorDataDirty)
				{
					this.UpdateController(this.councilor);
					this.councilorDataDirty = false;
				}
				base.transform.rotation = this.cameraManager.BillboardRotation;
			}
		}

		// Token: 0x04001CD5 RID: 7381
		private CameraManager cameraManager;

		// Token: 0x04001CD6 RID: 7382
		[HideInInspector]
		public TICouncilorState councilor;

		// Token: 0x04001CD7 RID: 7383
		private TIFactionState activePlayer;

		// Token: 0x04001CD8 RID: 7384
		private SpaceObjectSelection spaceObjectSelection;

		// Token: 0x04001CD9 RID: 7385
		private HabModelController habModelController;

		// Token: 0x04001CDA RID: 7386
		private TIGameState parentState;

		// Token: 0x04001CDB RID: 7387
		public Canvas primaryCanvas;

		// Token: 0x04001CDC RID: 7388
		public Image councilorIcon;

		// Token: 0x04001CDD RID: 7389
		public Image councilorBackground;

		// Token: 0x04001CDE RID: 7390
		public TMP_Text councilorName;

		// Token: 0x04001CDF RID: 7391
		public TMP_Text tohitValue;

		// Token: 0x04001CE0 RID: 7392
		public Image factionIcon;

		// Token: 0x04001CE1 RID: 7393
		public TooltipTrigger markerTooltipTrigger;

		// Token: 0x04001CE2 RID: 7394
		public GameObject centralIconAnimObject;

		// Token: 0x04001CE3 RID: 7395
		public Animator centralIconAnimator;

		// Token: 0x04001CE4 RID: 7396
		public SpriteRenderer centralIconSpriteRenderer;

		// Token: 0x04001CE5 RID: 7397
		private RuntimeAnimatorController centralIconAnimatorController;

		// Token: 0x04001CE6 RID: 7398
		public bool centralIconAnimating;

		// Token: 0x04001CE7 RID: 7399
		public Animator selectionAnim;

		// Token: 0x04001CE8 RID: 7400
		public SpriteRenderer selectionRenderer;

		// Token: 0x04001CE9 RID: 7401
		private RuntimeAnimatorController selectionAnimatorController;

		// Token: 0x04001CEA RID: 7402
		public bool selectionAnimating;

		// Token: 0x04001CEB RID: 7403
		public GameObject selectionAnimObject;

		// Token: 0x04001CEC RID: 7404
		private string cachedAnimTrigger;

		// Token: 0x04001CED RID: 7405
		private bool councilorDataDirty;

		// Token: 0x04001CEE RID: 7406
		public Image hoverImage;

		// Token: 0x04001CF0 RID: 7408
		[HideInInspector]
		public MeshRenderer parentMesh;

		// Token: 0x04001CF1 RID: 7409
		[HideInInspector]
		public bool currentlyActive;

		// Token: 0x04001CF2 RID: 7410
		public int tier;
	}
}
