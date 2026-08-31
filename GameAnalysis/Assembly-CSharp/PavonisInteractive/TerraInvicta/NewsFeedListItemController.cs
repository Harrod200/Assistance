using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A5 RID: 2213
	public class NewsFeedListItemController : MonoBehaviour
	{
		// Token: 0x06005384 RID: 21380 RVA: 0x00256770 File Offset: 0x00254970
		public void UpdateListItem(NotificationSummaryItem item, bool showText, bool useMoreSecondaryIcons)
		{
			this.item = item;
			if (!string.IsNullOrEmpty(item.iconResource))
			{
				this.itemIcon.enabled = true;
				this.itemIcon.color = Color.white;
				GameControl.assetLoader.LoadAssetForImageAssignment(item.iconResource, this.itemIcon);
				if (!string.IsNullOrEmpty(item.iconBackgroundResource))
				{
					this.itemIconBackground.enabled = true;
					GameControl.assetLoader.LoadAssetForImageAssignment(item.iconBackgroundResource, this.itemIconBackground);
					this.itemIconBackground.color = item.backgroundColor;
				}
				else
				{
					this.itemIconBackground.enabled = false;
				}
				this.detailTooltipTrigger.SetDelegate("BodyText", () => item.itemSummary);
				this.detailTooltipTrigger.enabled = true;
				this.gotoButton.enabled = TIGameState.Valid(item.gotoGameState);
			}
			else
			{
				this.itemIcon.enabled = false;
				this.itemIconBackground.enabled = false;
			}
			if (showText)
			{
				this.itemHeadline.text = item.itemSummary;
				this.itemHeadline.enabled = true;
			}
			else
			{
				this.itemHeadline.enabled = false;
			}
			if (this.secondaryIcon != null)
			{
				if (item.outcome != TIMissionOutcome.None)
				{
					switch (item.outcome)
					{
					case TIMissionOutcome.CriticalFailure:
						GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_mission_criticalfail", this.secondaryIcon);
						break;
					case TIMissionOutcome.Failure:
					case TIMissionOutcome.Aborted:
						GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_mission_fail", this.secondaryIcon);
						break;
					case TIMissionOutcome.Success:
						GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_mission_success", this.secondaryIcon);
						break;
					case TIMissionOutcome.CriticalSuccess:
						GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_mission_criticalsuccess", this.secondaryIcon);
						break;
					}
					this.secondaryIcon.gameObject.SetActive(true);
					return;
				}
				if (TIGameState.Valid(item.gotoGameState))
				{
					if (useMoreSecondaryIcons && item.gotoGameState.ref_nation != null && item.iconResource != item.gotoGameState.ref_nation.flagResource)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(item.gotoGameState.ref_nation.flagResource, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
						return;
					}
					if (item.gotoGameState.ref_naturalSpaceObject != null && !item.gotoGameState.ref_naturalSpaceObject.isSun && (useMoreSecondaryIcons || !item.gotoGameState.ref_naturalSpaceObject.isEarth) && item.iconResource != item.gotoGameState.ref_naturalSpaceObject.iconResource)
					{
						GameControl.assetLoader.LoadAssetForImageAssignment(item.gotoGameState.ref_naturalSpaceObject.iconResource, this.secondaryIcon);
						this.secondaryIcon.gameObject.SetActive(true);
						return;
					}
					this.secondaryIcon.gameObject.SetActive(false);
					return;
				}
				else
				{
					this.secondaryIcon.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005385 RID: 21381 RVA: 0x00256AE1 File Offset: 0x00254CE1
		public void FlashNewsIcon()
		{
			base.StartCoroutine(this.FlashNewsIconEffect());
		}

		// Token: 0x06005386 RID: 21382 RVA: 0x00256AF0 File Offset: 0x00254CF0
		private IEnumerator FlashNewsIconEffect()
		{
			this.itemIcon.color = Color.clear;
			Color additionalImageColor = Color.clear;
			Color secondaryImageColor = Color.clear;
			if (this.additionalImage != null)
			{
				additionalImageColor = this.additionalImage.color;
				this.additionalImage.color = Color.clear;
			}
			if (this.secondaryIcon != null)
			{
				secondaryImageColor = this.secondaryIcon.color;
				this.secondaryIcon.color = Color.clear;
			}
			if (this.itemIconBackground.enabled)
			{
				Color targetColor = this.itemIconBackground.color;
				this.itemIconBackground.color = Color.clear;
				for (float i = 0f; i <= 1f; i += Time.deltaTime)
				{
					this.itemIcon.color = new Color(1f, 1f, 1f, i);
					this.itemIconBackground.color = new Color(targetColor.r, targetColor.g, targetColor.b, i);
					if (this.additionalImage != null)
					{
						this.additionalImage.color = new Color(additionalImageColor.r, additionalImageColor.g, additionalImageColor.b, i);
					}
					if (this.secondaryIcon != null)
					{
						this.secondaryIcon.color = new Color(secondaryImageColor.r, secondaryImageColor.g, secondaryImageColor.b, i);
					}
					yield return null;
				}
				this.itemIcon.color = Color.white;
				this.itemIconBackground.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
				if (this.additionalImage != null)
				{
					this.additionalImage.color = new Color(additionalImageColor.r, additionalImageColor.g, additionalImageColor.b, 1f);
				}
				if (this.secondaryIcon != null)
				{
					this.secondaryIcon.color = new Color(secondaryImageColor.r, secondaryImageColor.g, secondaryImageColor.b, 1f);
				}
				targetColor = default(Color);
			}
			else
			{
				for (float i = 0f; i <= 1f; i += Time.deltaTime)
				{
					this.itemIcon.color = new Color(1f, 1f, 1f, i);
					if (this.additionalImage != null)
					{
						this.additionalImage.color = new Color(additionalImageColor.r, additionalImageColor.g, additionalImageColor.b, i);
					}
					if (this.secondaryIcon != null)
					{
						this.secondaryIcon.color = new Color(secondaryImageColor.r, secondaryImageColor.g, secondaryImageColor.b, i);
					}
					yield return null;
				}
				this.itemIcon.color = Color.white;
				if (this.additionalImage != null)
				{
					this.additionalImage.color = new Color(additionalImageColor.r, additionalImageColor.g, additionalImageColor.b, 1f);
				}
				if (this.secondaryIcon != null)
				{
					this.secondaryIcon.color = new Color(secondaryImageColor.r, secondaryImageColor.g, secondaryImageColor.b, 1f);
				}
			}
			yield break;
		}

		// Token: 0x06005387 RID: 21383 RVA: 0x00256AFF File Offset: 0x00254CFF
		public IEnumerator FadeOutIconEffect()
		{
			if (this.itemIconBackground.enabled)
			{
				Color targetColor = this.itemIconBackground.color;
				this.itemIconBackground.color = Color.clear;
				float i = 0f;
				while ((double)i <= 0.5)
				{
					this.itemIcon.color = new Color(1f, 1f, 1f, 1f - i * 2f);
					this.itemIconBackground.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f - i * 2f);
					if (this.additionalImage != null)
					{
						this.additionalImage.color = new Color(this.additionalImage.color.r, this.additionalImage.color.g, this.additionalImage.color.b, 1f - i * 2f);
					}
					yield return null;
					i += Time.deltaTime;
				}
				this.itemIconBackground.color = new Color(0f, 0f, 0f, 0f);
				targetColor = default(Color);
			}
			else
			{
				float i = 0f;
				while ((double)i <= 0.5)
				{
					this.itemIcon.color = new Color(1f, 1f, 1f, 1f - i * 2f);
					if (this.additionalImage != null)
					{
						this.additionalImage.color = new Color(this.additionalImage.color.r, this.additionalImage.color.g, this.additionalImage.color.b, 1f - i * 2f);
					}
					yield return null;
					i += Time.deltaTime;
				}
			}
			this.itemIcon.color = Color.clear;
			if (this.additionalImage != null)
			{
				this.additionalImage.color = Color.clear;
			}
			this.HideNewsItem();
			yield break;
		}

		// Token: 0x06005388 RID: 21384 RVA: 0x00256B10 File Offset: 0x00254D10
		public void OnClick()
		{
			TIGameState gotoGameState = this.item.gotoGameState;
			if (gotoGameState != null && !gotoGameState.deleted)
			{
				SoundEffectController.PlaySelectSound(this.item.gotoGameState);
				TIUtilities.GotoGameState(this.item.gotoGameState, true, true, true, true, false, -1f);
				if (this.item.timerFactions.Contains(GameControl.control.activePlayer))
				{
					this.FadeOutIcon();
				}
			}
		}

		// Token: 0x06005389 RID: 21385 RVA: 0x00256B85 File Offset: 0x00254D85
		public void OnRightClick()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseFinder", false, false);
			this.FadeOutIcon();
		}

		// Token: 0x0600538A RID: 21386 RVA: 0x00256B99 File Offset: 0x00254D99
		public void FadeOutIcon()
		{
			base.StartCoroutine(this.FadeOutIconEffect());
		}

		// Token: 0x0600538B RID: 21387 RVA: 0x00256BA8 File Offset: 0x00254DA8
		private void HideNewsItem()
		{
			base.gameObject.SetActive(false);
			using (IEnumerator<object> enumerator = NotificationScreenController.singleton.newsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NewsFeedListItemController.<>o__15.<>p__0 == null)
					{
						NewsFeedListItemController.<>o__15.<>p__0 = CallSite<Func<CallSite, object, NewsFeedListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NewsFeedListItemController), typeof(NewsFeedListItemController)));
					}
					if (NewsFeedListItemController.<>o__15.<>p__0.Target(NewsFeedListItemController.<>o__15.<>p__0, enumerator.Current).gameObject.activeInHierarchy)
					{
						return;
					}
				}
			}
			NotificationScreenController.singleton.SetCloseAllButton(false);
		}

		// Token: 0x04003957 RID: 14679
		public NotificationSummaryItem item;

		// Token: 0x04003958 RID: 14680
		public Image itemIcon;

		// Token: 0x04003959 RID: 14681
		public Image itemIconBackground;

		// Token: 0x0400395A RID: 14682
		public TMP_Text itemHeadline;

		// Token: 0x0400395B RID: 14683
		public TooltipTrigger detailTooltipTrigger;

		// Token: 0x0400395C RID: 14684
		public Button gotoButton;

		// Token: 0x0400395D RID: 14685
		public Image additionalImage;

		// Token: 0x0400395E RID: 14686
		public Image secondaryIcon;
	}
}
