using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200083C RID: 2108
	public class OrganizerCouncilorListItem : MonoBehaviour
	{
		// Token: 0x06004C53 RID: 19539 RVA: 0x00203CA7 File Offset: 0x00201EA7
		public void SetListItem(TICouncilorState councilor, CouncilGridController controller)
		{
			this.councilor = councilor;
			this.gridController = controller;
			this.activePlayer = GameControl.control.activePlayer;
			this.dragDestination.SetCouncilor(councilor, controller);
		}

		// Token: 0x06004C54 RID: 19540 RVA: 0x00203CD4 File Offset: 0x00201ED4
		public void UpdateListItem()
		{
			if (this.councilor == null)
			{
				return;
			}
			this.councilor.prospectiveOrgs = (from x in this.gridController.tempFactionCouncilorOrgs
				where x.Value == this.councilor
				select x.Key).ToList<TIOrgState>();
			this.councilorImage.sprite = this.councilor.GetIcon(false);
			this.councilorHomeFlag.sprite = this.councilor.homeNation.flag;
			this.councilorNameText.SetText(this.councilor.GetDisplayName(this.activePlayer));
			this.professionText.SetText(this.councilor.typeTemplate.displayName);
			this.adminValueText.SetText(Loc.T("UI.Council.OrgManagement.AdminUsedByOrgs", new object[]
			{
				this.councilor.prospectiveOrgsWeight,
				this.councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, true, false).ToString()
			}));
			this.adminValue2Text.SetText(this.councilor.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, true, false).ToString());
			this.persuasionValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, true, false).ToString());
			this.investigationValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, true, false).ToString());
			this.espionageValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Espionage, true, true, true, false, true, false).ToString());
			this.commandValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Command, true, true, true, false, true, false).ToString());
			this.scienceValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Science, true, true, true, false, true, false).ToString());
			this.securityValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.Security, true, true, true, false, true, false).ToString());
			this.loyaltyValueText.SetText(this.councilor.GetAttribute(CouncilorAttribute.ApparentLoyalty, true, true, false, true, true, false).ToString());
			this.adminTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Administration));
			this.persuasionTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Persuasion));
			this.investigationTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Investigation));
			this.espionageTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Espionage));
			this.commandTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Command));
			this.scienceTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Science));
			this.securityTooltip.SetDelegate("BodyText", () => CouncilGridController.StatDetail(this.councilor, CouncilorAttribute.Security));
			this.loyaltyTooltip.SetText("BodyText", Loc.T("UI.Councilor.LoyaltyTip"));
			this.orgTotalText.SetText(Loc.T("UI.Councilor.Orgs"));
			this.orgLimitText.SetText(this.councilor.prospectiveOrgs.Count.ToString());
			int num = 0;
			this.councilorTraitsList.SetListSize<OrganizerCouncilorTraitListItem>(this.councilor.traits.Count, false, false);
			using (IEnumerator<object> enumerator = this.councilorTraitsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OrganizerCouncilorListItem.<>o__36.<>p__0 == null)
					{
						OrganizerCouncilorListItem.<>o__36.<>p__0 = CallSite<Func<CallSite, object, OrganizerCouncilorTraitListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorTraitListItem), typeof(OrganizerCouncilorListItem)));
					}
					OrganizerCouncilorListItem.<>o__36.<>p__0.Target(OrganizerCouncilorListItem.<>o__36.<>p__0, enumerator.Current).SetListItem(this.councilor.traits[num++]);
				}
			}
			num = 0;
			List<TIMissionTemplate> possibleMissionList = this.councilor.GetPossibleMissionList(false, true, true, null, true);
			this.councilorMissionsList.SetListSize<OrganizerCouncilorMissionListItem>(possibleMissionList.Count, false, false);
			using (IEnumerator<object> enumerator = this.councilorMissionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OrganizerCouncilorListItem.<>o__36.<>p__1 == null)
					{
						OrganizerCouncilorListItem.<>o__36.<>p__1 = CallSite<Func<CallSite, object, OrganizerCouncilorMissionListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerCouncilorMissionListItem), typeof(OrganizerCouncilorListItem)));
					}
					OrganizerCouncilorListItem.<>o__36.<>p__1.Target(OrganizerCouncilorListItem.<>o__36.<>p__1, enumerator.Current).SetListItem(possibleMissionList[num++]);
				}
			}
			num = 0;
			List<TIOrgState> list = new List<TIOrgState>();
			foreach (KeyValuePair<TIOrgState, TICouncilorState> keyValuePair in this.gridController.tempFactionCouncilorOrgs.Where<KeyValuePair<TIOrgState, TICouncilorState>>((KeyValuePair<TIOrgState, TICouncilorState> o) => o.Value == this.councilor).ToList<KeyValuePair<TIOrgState, TICouncilorState>>())
			{
				list.Add(keyValuePair.Key);
			}
			base.StartCoroutine(this.UpdateScrollRectEnabled(this.orgScrollRect, list.Count > 15));
			base.StartCoroutine(this.UpdateScrollRectEnabled(this.traitScrollRect, this.councilor.traits.Count > 15));
			base.StartCoroutine(this.UpdateScrollRectEnabled(this.missionScrollRect, possibleMissionList.Count > 18));
			this.councilorOrgsList.SetListSize<OrganizerOrgListItem>(list.Count, false, false);
			using (IEnumerator<object> enumerator = this.councilorOrgsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (OrganizerCouncilorListItem.<>o__36.<>p__2 == null)
					{
						OrganizerCouncilorListItem.<>o__36.<>p__2 = CallSite<Func<CallSite, object, OrganizerOrgListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(OrganizerOrgListItem), typeof(OrganizerCouncilorListItem)));
					}
					OrganizerCouncilorListItem.<>o__36.<>p__2.Target(OrganizerCouncilorListItem.<>o__36.<>p__2, enumerator.Current).SetListItem(list[num++], OrganizerOrgListItem.OrgStatus.ASSIGNED, this.gridController, this);
				}
			}
		}

		// Token: 0x06004C55 RID: 19541 RVA: 0x0020430C File Offset: 0x0020250C
		public void ToggleMinimize()
		{
			this.minimized = !this.minimized;
			AudioManager.PlayOneShot(this.minimized ? "event:/SFX/UI_SFX/trig_SFX_CycleBack" : "event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.orgScrollRect.gameObject.SetActive(!this.minimized);
			base.GetComponent<RectTransform>().sizeDelta = new Vector2(base.GetComponent<RectTransform>().sizeDelta.x, (float)(this.minimized ? 100 : 344));
			base.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
			base.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
			TIUtilities.UpdateButtonSpritesPlusMinusAlt(this.minimizeButton, this.minimized);
		}

		// Token: 0x06004C56 RID: 19542 RVA: 0x002043CB File Offset: 0x002025CB
		public void PushToTop()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			base.transform.SetSiblingIndex(0);
		}

		// Token: 0x06004C57 RID: 19543 RVA: 0x002043E8 File Offset: 0x002025E8
		public void UpdateOrgIsValid(TIOrgState org)
		{
			if (this.councilor == null)
			{
				this.borderImage.color = ((org.AllowedOnFactionMarket(GameControl.control.activePlayer) && (org.ref_councilor == null || !org.ref_councilor.OrgProvidingActiveMission(org))) ? TIUtilities.UIColorIndicatorPositive : TIUtilities.UIColorIndicatorNegative);
				return;
			}
			this.borderImage.color = ((org.CouncilorCanAcquire(this.councilor) && (org.ref_councilor == null || !org.ref_councilor.OrgProvidingActiveMission(org))) ? TIUtilities.UIColorIndicatorPositive : TIUtilities.UIColorIndicatorNegative);
		}

		// Token: 0x06004C58 RID: 19544 RVA: 0x0020448A File Offset: 0x0020268A
		public void ResetBorder()
		{
			this.borderImage.color = TIUtilities.UITextColor;
		}

		// Token: 0x06004C59 RID: 19545 RVA: 0x0020449C File Offset: 0x0020269C
		public void UpdateBorderForValidCouncilor()
		{
			string text;
			this.borderImage.color = (this.councilor.AreProspectiveOrgsValid(out text) ? TIUtilities.UIColorIndicatorPositive : TIUtilities.UIColorIndicatorNegative);
		}

		// Token: 0x06004C5A RID: 19546 RVA: 0x002044CF File Offset: 0x002026CF
		public IEnumerator UpdateScrollRectEnabled(ScrollRect scrollRect, bool enabled)
		{
			yield return null;
			scrollRect.verticalNormalizedPosition = 1f;
			scrollRect.enabled = enabled;
			yield break;
		}

		// Token: 0x04002E30 RID: 11824
		public Image councilorImage;

		// Token: 0x04002E31 RID: 11825
		public Image councilorHomeFlag;

		// Token: 0x04002E32 RID: 11826
		public TMP_Text councilorNameText;

		// Token: 0x04002E33 RID: 11827
		public TMP_Text professionText;

		// Token: 0x04002E34 RID: 11828
		public TMP_Text adminValueText;

		// Token: 0x04002E35 RID: 11829
		public TMP_Text adminValue2Text;

		// Token: 0x04002E36 RID: 11830
		public TMP_Text persuasionValueText;

		// Token: 0x04002E37 RID: 11831
		public TMP_Text investigationValueText;

		// Token: 0x04002E38 RID: 11832
		public TMP_Text espionageValueText;

		// Token: 0x04002E39 RID: 11833
		public TMP_Text commandValueText;

		// Token: 0x04002E3A RID: 11834
		public TMP_Text scienceValueText;

		// Token: 0x04002E3B RID: 11835
		public TMP_Text securityValueText;

		// Token: 0x04002E3C RID: 11836
		public TMP_Text loyaltyValueText;

		// Token: 0x04002E3D RID: 11837
		public TMP_Text orgTotalText;

		// Token: 0x04002E3E RID: 11838
		public TMP_Text orgLimitText;

		// Token: 0x04002E3F RID: 11839
		public TooltipTrigger adminTooltip;

		// Token: 0x04002E40 RID: 11840
		public TooltipTrigger persuasionTooltip;

		// Token: 0x04002E41 RID: 11841
		public TooltipTrigger investigationTooltip;

		// Token: 0x04002E42 RID: 11842
		public TooltipTrigger espionageTooltip;

		// Token: 0x04002E43 RID: 11843
		public TooltipTrigger commandTooltip;

		// Token: 0x04002E44 RID: 11844
		public TooltipTrigger scienceTooltip;

		// Token: 0x04002E45 RID: 11845
		public TooltipTrigger securityTooltip;

		// Token: 0x04002E46 RID: 11846
		public TooltipTrigger loyaltyTooltip;

		// Token: 0x04002E47 RID: 11847
		public ListManagerBase councilorOrgsList;

		// Token: 0x04002E48 RID: 11848
		public ListManagerBase councilorTraitsList;

		// Token: 0x04002E49 RID: 11849
		public ListManagerBase councilorMissionsList;

		// Token: 0x04002E4A RID: 11850
		public ScrollRect orgScrollRect;

		// Token: 0x04002E4B RID: 11851
		public ScrollRect traitScrollRect;

		// Token: 0x04002E4C RID: 11852
		public ScrollRect missionScrollRect;

		// Token: 0x04002E4D RID: 11853
		public CouncilorOrgsDragDestination dragDestination;

		// Token: 0x04002E4E RID: 11854
		public Button minimizeButton;

		// Token: 0x04002E4F RID: 11855
		private CouncilGridController gridController;

		// Token: 0x04002E50 RID: 11856
		private TICouncilorState councilor;

		// Token: 0x04002E51 RID: 11857
		private TIFactionState activePlayer;

		// Token: 0x04002E52 RID: 11858
		public Image borderImage;

		// Token: 0x04002E53 RID: 11859
		private bool minimized;
	}
}
