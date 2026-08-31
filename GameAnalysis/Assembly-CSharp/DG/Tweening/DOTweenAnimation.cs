using System;
using System.Collections.Generic;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DG.Tweening
{
	// Token: 0x02000545 RID: 1349
	[AddComponentMenu("DOTween/DOTween Animation")]
	public class DOTweenAnimation : ABSAnimationComponent
	{
		// Token: 0x06002287 RID: 8839 RVA: 0x000B2A1A File Offset: 0x000B0C1A
		private void Awake()
		{
			if (!this.isActive || !this.isValid)
			{
				return;
			}
			if (this.animationType != DOTweenAnimationType.Move || !this.useTargetAsV3)
			{
				this.CreateTween();
				this._tweenCreated = true;
			}
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x000B2A4B File Offset: 0x000B0C4B
		private void Start()
		{
			if (this._tweenCreated || !this.isActive || !this.isValid)
			{
				return;
			}
			this.CreateTween();
			this._tweenCreated = true;
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x000B2A73 File Offset: 0x000B0C73
		private void OnDestroy()
		{
			if (this.tween != null && this.tween.IsActive())
			{
				this.tween.Kill(false);
			}
			this.tween = null;
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x000B2AA0 File Offset: 0x000B0CA0
		public void CreateTween()
		{
			if (this.target == null)
			{
				Debug.LogWarning(string.Format("{0} :: This tween's target is NULL, because the animation was created with a DOTween Pro version older than 0.9.255. To fix this, exit Play mode then simply select this object, and it will update automatically", base.gameObject.name), base.gameObject);
				return;
			}
			if (this.forcedTargetType != TargetType.Unset)
			{
				this.targetType = this.forcedTargetType;
			}
			if (this.targetType == TargetType.Unset)
			{
				this.targetType = DOTweenAnimation.TypeToDOTargetType(this.target.GetType());
			}
			switch (this.animationType)
			{
			case DOTweenAnimationType.Move:
				if (this.useTargetAsV3)
				{
					this.isRelative = false;
					if (this.endValueTransform == null)
					{
						Debug.LogWarning(string.Format("{0} :: This tween's TO target is NULL, a Vector3 of (0,0,0) will be used instead", base.gameObject.name), base.gameObject);
						this.endValueV3 = Vector3.zero;
					}
					else if (this.targetType == TargetType.RectTransform)
					{
						RectTransform rectTransform = this.endValueTransform as RectTransform;
						if (rectTransform == null)
						{
							Debug.LogWarning(string.Format("{0} :: This tween's TO target should be a RectTransform, a Vector3 of (0,0,0) will be used instead", base.gameObject.name), base.gameObject);
							this.endValueV3 = Vector3.zero;
						}
						else
						{
							RectTransform rectTransform2 = this.target as RectTransform;
							if (rectTransform2 == null)
							{
								Debug.LogWarning(string.Format("{0} :: This tween's target and TO target are not of the same type. Please reassign the values", base.gameObject.name), base.gameObject);
							}
							else
							{
								this.endValueV3 = DOTweenUtils46.SwitchToRectTransform(rectTransform, rectTransform2);
							}
						}
					}
					else
					{
						this.endValueV3 = this.endValueTransform.position;
					}
				}
				switch (this.targetType)
				{
				case TargetType.RectTransform:
					this.tween = ((RectTransform)this.target).DOAnchorPos3D(this.endValueV3, this.duration, this.optionalBool0);
					break;
				case TargetType.Rigidbody:
					this.tween = ((Rigidbody)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
					break;
				case TargetType.Rigidbody2D:
					this.tween = ((Rigidbody2D)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
					break;
				case TargetType.Transform:
					this.tween = ((Transform)this.target).DOMove(this.endValueV3, this.duration, this.optionalBool0);
					break;
				}
				break;
			case DOTweenAnimationType.LocalMove:
				this.tween = base.transform.DOLocalMove(this.endValueV3, this.duration, this.optionalBool0);
				break;
			case DOTweenAnimationType.Rotate:
				switch (this.targetType)
				{
				case TargetType.Rigidbody:
					this.tween = ((Rigidbody)this.target).DORotate(this.endValueV3, this.duration, this.optionalRotationMode);
					break;
				case TargetType.Rigidbody2D:
					this.tween = ((Rigidbody2D)this.target).DORotate(this.endValueFloat, this.duration);
					break;
				case TargetType.Transform:
					this.tween = ((Transform)this.target).DORotate(this.endValueV3, this.duration, this.optionalRotationMode);
					break;
				}
				break;
			case DOTweenAnimationType.LocalRotate:
				this.tween = base.transform.DOLocalRotate(this.endValueV3, this.duration, this.optionalRotationMode);
				break;
			case DOTweenAnimationType.Scale:
			{
				TargetType targetType = this.targetType;
				this.tween = base.transform.DOScale(this.optionalBool0 ? new Vector3(this.endValueFloat, this.endValueFloat, this.endValueFloat) : this.endValueV3, this.duration);
				break;
			}
			case DOTweenAnimationType.Color:
				this.isRelative = false;
				switch (this.targetType)
				{
				case TargetType.Image:
					this.tween = ((Image)this.target).DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.Light:
					this.tween = ((Light)this.target).DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.Renderer:
					this.tween = ((Renderer)this.target).material.DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.SpriteRenderer:
					this.tween = ((SpriteRenderer)this.target).DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.Text:
					this.tween = ((Text)this.target).DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.TextMeshPro:
					this.tween = ((TextMeshPro)this.target).DOColor(this.endValueColor, this.duration);
					break;
				case TargetType.TextMeshProUGUI:
					this.tween = ((TextMeshProUGUI)this.target).DOColor(this.endValueColor, this.duration);
					break;
				}
				break;
			case DOTweenAnimationType.Fade:
				this.isRelative = false;
				switch (this.targetType)
				{
				case TargetType.CanvasGroup:
					this.tween = ((CanvasGroup)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.Image:
					this.tween = ((Image)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.Light:
					this.tween = ((Light)this.target).DOIntensity(this.endValueFloat, this.duration);
					break;
				case TargetType.Renderer:
					this.tween = ((Renderer)this.target).material.DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.SpriteRenderer:
					this.tween = ((SpriteRenderer)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.Text:
					this.tween = ((Text)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.TextMeshPro:
					this.tween = ((TextMeshPro)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				case TargetType.TextMeshProUGUI:
					this.tween = ((TextMeshProUGUI)this.target).DOFade(this.endValueFloat, this.duration);
					break;
				}
				break;
			case DOTweenAnimationType.Text:
			{
				TargetType targetType2 = this.targetType;
				if (targetType2 != TargetType.Text)
				{
					if (targetType2 != TargetType.TextMeshPro)
					{
						if (targetType2 == TargetType.TextMeshProUGUI)
						{
							this.tween = ((TextMeshProUGUI)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
						}
					}
					else
					{
						this.tween = ((TextMeshPro)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
					}
				}
				else
				{
					this.tween = ((Text)this.target).DOText(this.endValueString, this.duration, this.optionalBool0, this.optionalScrambleMode, this.optionalString);
				}
				break;
			}
			case DOTweenAnimationType.PunchPosition:
			{
				TargetType targetType2 = this.targetType;
				if (targetType2 != TargetType.RectTransform)
				{
					if (targetType2 == TargetType.Transform)
					{
						this.tween = ((Transform)this.target).DOPunchPosition(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0, this.optionalBool0);
					}
				}
				else
				{
					this.tween = ((RectTransform)this.target).DOPunchAnchorPos(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0, this.optionalBool0);
				}
				break;
			}
			case DOTweenAnimationType.PunchRotation:
				this.tween = base.transform.DOPunchRotation(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0);
				break;
			case DOTweenAnimationType.PunchScale:
				this.tween = base.transform.DOPunchScale(this.endValueV3, this.duration, this.optionalInt0, this.optionalFloat0);
				break;
			case DOTweenAnimationType.ShakePosition:
			{
				TargetType targetType2 = this.targetType;
				if (targetType2 != TargetType.RectTransform)
				{
					if (targetType2 == TargetType.Transform)
					{
						this.tween = ((Transform)this.target).DOShakePosition(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool0, true);
					}
				}
				else
				{
					this.tween = ((RectTransform)this.target).DOShakeAnchorPos(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, this.optionalBool0, true);
				}
				break;
			}
			case DOTweenAnimationType.ShakeRotation:
				this.tween = base.transform.DOShakeRotation(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, true);
				break;
			case DOTweenAnimationType.ShakeScale:
				this.tween = base.transform.DOShakeScale(this.duration, this.endValueV3, this.optionalInt0, this.optionalFloat0, true);
				break;
			case DOTweenAnimationType.CameraAspect:
				this.tween = ((Camera)this.target).DOAspect(this.endValueFloat, this.duration);
				break;
			case DOTweenAnimationType.CameraBackgroundColor:
				this.tween = ((Camera)this.target).DOColor(this.endValueColor, this.duration);
				break;
			case DOTweenAnimationType.CameraFieldOfView:
				this.tween = ((Camera)this.target).DOFieldOfView(this.endValueFloat, this.duration);
				break;
			case DOTweenAnimationType.CameraOrthoSize:
				this.tween = ((Camera)this.target).DOOrthoSize(this.endValueFloat, this.duration);
				break;
			case DOTweenAnimationType.CameraPixelRect:
				this.tween = ((Camera)this.target).DOPixelRect(this.endValueRect, this.duration);
				break;
			case DOTweenAnimationType.CameraRect:
				this.tween = ((Camera)this.target).DORect(this.endValueRect, this.duration);
				break;
			case DOTweenAnimationType.UIWidthHeight:
				this.tween = ((RectTransform)this.target).DOSizeDelta(this.optionalBool0 ? new Vector2(this.endValueFloat, this.endValueFloat) : this.endValueV2, this.duration, false);
				break;
			}
			if (this.tween == null)
			{
				return;
			}
			if (this.isFrom)
			{
				((Tweener)this.tween).From(this.isRelative);
			}
			else
			{
				this.tween.SetRelative(this.isRelative);
			}
			this.tween.SetTarget(base.gameObject).SetDelay(this.delay).SetLoops(this.loops, this.loopType)
				.SetAutoKill(this.autoKill)
				.OnKill(delegate
				{
					this.tween = null;
				});
			if (this.isSpeedBased)
			{
				this.tween.SetSpeedBased<Tween>();
			}
			if (this.easeType == Ease.INTERNAL_Custom)
			{
				this.tween.SetEase(this.easeCurve);
			}
			else
			{
				this.tween.SetEase(this.easeType);
			}
			if (!string.IsNullOrEmpty(this.id))
			{
				this.tween.SetId(this.id);
			}
			this.tween.SetUpdate(this.isIndependentUpdate);
			if (this.hasOnStart)
			{
				if (this.onStart != null)
				{
					this.tween.OnStart(new TweenCallback(this.onStart.Invoke));
				}
			}
			else
			{
				this.onStart = null;
			}
			if (this.hasOnPlay)
			{
				if (this.onPlay != null)
				{
					this.tween.OnPlay(new TweenCallback(this.onPlay.Invoke));
				}
			}
			else
			{
				this.onPlay = null;
			}
			if (this.hasOnUpdate)
			{
				if (this.onUpdate != null)
				{
					this.tween.OnUpdate(new TweenCallback(this.onUpdate.Invoke));
				}
			}
			else
			{
				this.onUpdate = null;
			}
			if (this.hasOnStepComplete)
			{
				if (this.onStepComplete != null)
				{
					this.tween.OnStepComplete(new TweenCallback(this.onStepComplete.Invoke));
				}
			}
			else
			{
				this.onStepComplete = null;
			}
			if (this.hasOnComplete)
			{
				if (this.onComplete != null)
				{
					this.tween.OnComplete(new TweenCallback(this.onComplete.Invoke));
				}
			}
			else
			{
				this.onComplete = null;
			}
			if (this.hasOnRewind)
			{
				if (this.onRewind != null)
				{
					this.tween.OnRewind(new TweenCallback(this.onRewind.Invoke));
				}
			}
			else
			{
				this.onRewind = null;
			}
			if (this.autoPlay)
			{
				this.tween.Play<Tween>();
			}
			else
			{
				this.tween.Pause<Tween>();
			}
			if (this.hasOnTweenCreated && this.onTweenCreated != null)
			{
				this.onTweenCreated.Invoke();
			}
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x000B37C4 File Offset: 0x000B19C4
		public override void DOPlay()
		{
			DOTween.Play(base.gameObject);
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x000B37D2 File Offset: 0x000B19D2
		public override void DOPlayBackwards()
		{
			DOTween.PlayBackwards(base.gameObject);
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x000B37E0 File Offset: 0x000B19E0
		public override void DOPlayForward()
		{
			DOTween.PlayForward(base.gameObject);
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x000B37EE File Offset: 0x000B19EE
		public override void DOPause()
		{
			DOTween.Pause(base.gameObject);
		}

		// Token: 0x0600228F RID: 8847 RVA: 0x000B37FC File Offset: 0x000B19FC
		public override void DOTogglePause()
		{
			DOTween.TogglePause(base.gameObject);
		}

		// Token: 0x06002290 RID: 8848 RVA: 0x000B380C File Offset: 0x000B1A0C
		public override void DORewind()
		{
			this._playCount = -1;
			DOTweenAnimation[] components = base.gameObject.GetComponents<DOTweenAnimation>();
			for (int i = components.Length - 1; i > -1; i--)
			{
				Tween tween = components[i].tween;
				if (tween != null && tween.IsInitialized())
				{
					components[i].tween.Rewind(true);
				}
			}
		}

		// Token: 0x06002291 RID: 8849 RVA: 0x000B3860 File Offset: 0x000B1A60
		public override void DORestart(bool fromHere = false)
		{
			this._playCount = -1;
			if (this.tween == null)
			{
				if (Debugger.logPriority > 1)
				{
					Debugger.LogNullTween(this.tween);
				}
				return;
			}
			if (fromHere && this.isRelative)
			{
				this.ReEvaluateRelativeTween();
			}
			DOTween.Restart(base.gameObject, true, -1f);
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x000B38B3 File Offset: 0x000B1AB3
		public override void DOComplete()
		{
			DOTween.Complete(base.gameObject, false);
		}

		// Token: 0x06002293 RID: 8851 RVA: 0x000B38C2 File Offset: 0x000B1AC2
		public override void DOKill()
		{
			DOTween.Kill(base.gameObject, false);
			this.tween = null;
		}

		// Token: 0x06002294 RID: 8852 RVA: 0x000B38D8 File Offset: 0x000B1AD8
		public void DOPlayById(string id)
		{
			DOTween.Play(base.gameObject, id);
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x000B38E7 File Offset: 0x000B1AE7
		public void DOPlayAllById(string id)
		{
			DOTween.Play(id);
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x000B38F0 File Offset: 0x000B1AF0
		public void DOPauseAllById(string id)
		{
			DOTween.Pause(id);
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x000B38F9 File Offset: 0x000B1AF9
		public void DOPlayBackwardsById(string id)
		{
			DOTween.PlayBackwards(base.gameObject, id);
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x000B3908 File Offset: 0x000B1B08
		public void DOPlayBackwardsAllById(string id)
		{
			DOTween.PlayBackwards(id);
		}

		// Token: 0x06002299 RID: 8857 RVA: 0x000B3911 File Offset: 0x000B1B11
		public void DOPlayForwardById(string id)
		{
			DOTween.PlayForward(base.gameObject, id);
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x000B3920 File Offset: 0x000B1B20
		public void DOPlayForwardAllById(string id)
		{
			DOTween.PlayForward(id);
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x000B392C File Offset: 0x000B1B2C
		public void DOPlayNext()
		{
			DOTweenAnimation[] components = base.GetComponents<DOTweenAnimation>();
			while (this._playCount < components.Length - 1)
			{
				this._playCount++;
				DOTweenAnimation dotweenAnimation = components[this._playCount];
				if (dotweenAnimation != null && dotweenAnimation.tween != null && !dotweenAnimation.tween.IsPlaying() && !dotweenAnimation.tween.IsComplete())
				{
					dotweenAnimation.tween.Play<Tween>();
					return;
				}
			}
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x000B399E File Offset: 0x000B1B9E
		public void DORewindAndPlayNext()
		{
			this._playCount = -1;
			DOTween.Rewind(base.gameObject, true);
			this.DOPlayNext();
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x000B39BA File Offset: 0x000B1BBA
		public void DORestartById(string id)
		{
			this._playCount = -1;
			DOTween.Restart(base.gameObject, id, true, -1f);
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x000B39D6 File Offset: 0x000B1BD6
		public void DORestartAllById(string id)
		{
			this._playCount = -1;
			DOTween.Restart(id, true, -1f);
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x000B39EC File Offset: 0x000B1BEC
		public List<Tween> GetTweens()
		{
			List<Tween> list = new List<Tween>();
			foreach (DOTweenAnimation dotweenAnimation in base.GetComponents<DOTweenAnimation>())
			{
				list.Add(dotweenAnimation.tween);
			}
			return list;
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x000B3A28 File Offset: 0x000B1C28
		public static TargetType TypeToDOTargetType(Type t)
		{
			string text = t.ToString();
			int num = text.LastIndexOf(".");
			if (num != -1)
			{
				text = text.Substring(num + 1);
			}
			if (text.IndexOf("Renderer") != -1 && text != "SpriteRenderer")
			{
				text = "Renderer";
			}
			return (TargetType)Enum.Parse(typeof(TargetType), text);
		}

		// Token: 0x060022A1 RID: 8865 RVA: 0x000B3A8C File Offset: 0x000B1C8C
		private void ReEvaluateRelativeTween()
		{
			if (this.animationType == DOTweenAnimationType.Move)
			{
				((Tweener)this.tween).ChangeEndValue(base.transform.position + this.endValueV3, true);
				return;
			}
			if (this.animationType == DOTweenAnimationType.LocalMove)
			{
				((Tweener)this.tween).ChangeEndValue(base.transform.localPosition + this.endValueV3, true);
			}
		}

		// Token: 0x04001A33 RID: 6707
		public float delay;

		// Token: 0x04001A34 RID: 6708
		public float duration = 1f;

		// Token: 0x04001A35 RID: 6709
		public Ease easeType = Ease.OutQuad;

		// Token: 0x04001A36 RID: 6710
		public AnimationCurve easeCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 1f)
		});

		// Token: 0x04001A37 RID: 6711
		public LoopType loopType;

		// Token: 0x04001A38 RID: 6712
		public int loops = 1;

		// Token: 0x04001A39 RID: 6713
		public string id = "";

		// Token: 0x04001A3A RID: 6714
		public bool isRelative;

		// Token: 0x04001A3B RID: 6715
		public bool isFrom;

		// Token: 0x04001A3C RID: 6716
		public bool isIndependentUpdate;

		// Token: 0x04001A3D RID: 6717
		public bool autoKill = true;

		// Token: 0x04001A3E RID: 6718
		public bool isActive = true;

		// Token: 0x04001A3F RID: 6719
		public bool isValid;

		// Token: 0x04001A40 RID: 6720
		public Component target;

		// Token: 0x04001A41 RID: 6721
		public DOTweenAnimationType animationType;

		// Token: 0x04001A42 RID: 6722
		public TargetType targetType;

		// Token: 0x04001A43 RID: 6723
		public TargetType forcedTargetType;

		// Token: 0x04001A44 RID: 6724
		public bool autoPlay = true;

		// Token: 0x04001A45 RID: 6725
		public bool useTargetAsV3;

		// Token: 0x04001A46 RID: 6726
		public float endValueFloat;

		// Token: 0x04001A47 RID: 6727
		public Vector3 endValueV3;

		// Token: 0x04001A48 RID: 6728
		public Vector2 endValueV2;

		// Token: 0x04001A49 RID: 6729
		public Color endValueColor = new Color(1f, 1f, 1f, 1f);

		// Token: 0x04001A4A RID: 6730
		public string endValueString = "";

		// Token: 0x04001A4B RID: 6731
		public Rect endValueRect = new Rect(0f, 0f, 0f, 0f);

		// Token: 0x04001A4C RID: 6732
		public Transform endValueTransform;

		// Token: 0x04001A4D RID: 6733
		public bool optionalBool0;

		// Token: 0x04001A4E RID: 6734
		public float optionalFloat0;

		// Token: 0x04001A4F RID: 6735
		public int optionalInt0;

		// Token: 0x04001A50 RID: 6736
		public RotateMode optionalRotationMode;

		// Token: 0x04001A51 RID: 6737
		public ScrambleMode optionalScrambleMode;

		// Token: 0x04001A52 RID: 6738
		public string optionalString;

		// Token: 0x04001A53 RID: 6739
		private bool _tweenCreated;

		// Token: 0x04001A54 RID: 6740
		private int _playCount = -1;
	}
}
