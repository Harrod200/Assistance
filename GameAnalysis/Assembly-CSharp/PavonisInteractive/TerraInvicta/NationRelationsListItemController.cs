using System;
using System.Collections.Generic;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000892 RID: 2194
	internal class NationRelationsListItemController : MonoBehaviour
	{
		// Token: 0x17000EEC RID: 3820
		// (get) Token: 0x060052D6 RID: 21206 RVA: 0x0024B271 File Offset: 0x00249471
		private NationInfoController controller
		{
			get
			{
				return this.nationRelationsPaneController.controller;
			}
		}

		// Token: 0x17000EED RID: 3821
		// (get) Token: 0x060052D7 RID: 21207 RVA: 0x0024B27E File Offset: 0x0024947E
		private Dictionary<TINationState, RelationChange> proposedRelationsChanges
		{
			get
			{
				return this.nationRelationsPaneController.controller.proposedRelationsChanges;
			}
		}

		// Token: 0x060052D8 RID: 21208 RVA: 0x0024B290 File Offset: 0x00249490
		public void SetListItem(TINationState myNation, TINationState otherNation, NationRelationsPaneController nationRelationsPaneController)
		{
			this.myNation = myNation;
			this.otherNation = otherNation;
			this.nationRelationsPaneController = nationRelationsPaneController;
			this.nationFlag.sprite = otherNation.flag;
			this.nationName.SetText(otherNation.displayName);
			if (otherNation.numStandardArmies > 0)
			{
				this.numArmies.SetText(otherNation.numStandardArmies.ToString("N0"));
			}
			else
			{
				this.numArmies.SetText("-");
			}
			if (otherNation.numNavies > 0)
			{
				this.numNavies.SetText(otherNation.numNavies.ToString("N0"));
			}
			else
			{
				this.numNavies.SetText("-");
			}
			this.allyTip.SetDelegate("BodyText", () => this.AllyButtonTip(myNation, otherNation));
			this.normalizeTip.SetDelegate("BodyText", () => this.NormalizeButtonTip(myNation, otherNation));
			this.rivalTip.SetDelegate("BodyText", () => this.RivalryButtonTip(myNation, otherNation));
			this.UpdateListItem();
		}

		// Token: 0x060052D9 RID: 21209 RVA: 0x0024B3E4 File Offset: 0x002495E4
		public void UpdateListItem()
		{
			this.listenToggle = false;
			this.allyToggle.SetIsOnWithoutNotify(this.myNation.IsAlliedWith(this.otherNation, false));
			this.AllyToggleChange();
			this.allyCheckmark.color = Color.white;
			this.rivalToggle.SetIsOnWithoutNotify(this.myNation.IsEnemy(this.otherNation));
			this.RivalToggleChange();
			this.rivalCheckmark.color = Color.white;
			this.normalToggle.SetIsOnWithoutNotify(!this.allyToggle.isOn && !this.rivalToggle.isOn);
			this.NormalToggleChange();
			this.normalCheckmark.color = Color.white;
			this.warFlagImage.enabled = this.otherNation.wars.Count > 0;
			if (this.otherNation.wars.Contains(this.myNation))
			{
				this.warImage.sprite = this.myNation.flag;
				this.warImage.enabled = true;
			}
			else
			{
				this.warImage.enabled = false;
			}
			this.allyToggle.interactable = this.myNation.CanAlly(this.otherNation, false);
			if (this.allyToggle.interactable)
			{
				this.allyChance.SetText(StratPolicyResponseSelector.ChanceFormAlliance(this.myNation, this.otherNation).ToPercent("P0"));
			}
			else
			{
				this.allyChance.SetText(string.Empty);
			}
			this.normalToggle.interactable = this.myNation.CanNormalize(this.otherNation);
			if (this.normalToggle.interactable && this.myNation.rivals.Contains(this.otherNation))
			{
				this.endRivalryChance.SetText(StratPolicyResponseSelector.ChanceEndRivalry(this.myNation, this.otherNation).ToPercent("P0"));
			}
			else
			{
				this.endRivalryChance.SetText(string.Empty);
			}
			this.rivalToggle.interactable = this.myNation.CanRival(this.otherNation);
			if (this.proposedRelationsChanges.ContainsKey(this.otherNation))
			{
				switch (this.proposedRelationsChanges[this.otherNation])
				{
				case RelationChange.NormalToAlly:
					this.proposeAllianceArrow.enabled = true;
					this.allyToggle.SetIsOnWithoutNotify(true);
					this.AllyToggleChange();
					this.allyCheckmark.color = Color.green;
					this.endAllianceArrow.enabled = false;
					this.proposeEndRivalryArrow.enabled = false;
					this.initiateRivalryArrow.enabled = false;
					break;
				case RelationChange.AllyToNormal:
					this.endAllianceArrow.enabled = true;
					this.normalToggle.SetIsOnWithoutNotify(true);
					this.NormalToggleChange();
					this.normalCheckmark.color = Color.red;
					this.proposeAllianceArrow.enabled = false;
					this.proposeEndRivalryArrow.enabled = false;
					this.initiateRivalryArrow.enabled = false;
					break;
				case RelationChange.RivalToNormal:
					this.proposeEndRivalryArrow.enabled = true;
					this.normalToggle.SetIsOnWithoutNotify(true);
					this.NormalToggleChange();
					this.normalCheckmark.color = Color.green;
					this.proposeAllianceArrow.enabled = false;
					this.endAllianceArrow.enabled = false;
					this.initiateRivalryArrow.enabled = false;
					break;
				case RelationChange.NormalToRival:
					this.initiateRivalryArrow.enabled = true;
					this.rivalToggle.SetIsOnWithoutNotify(true);
					this.RivalToggleChange();
					this.rivalCheckmark.color = Color.red;
					this.proposeAllianceArrow.enabled = false;
					this.endAllianceArrow.enabled = false;
					this.proposeEndRivalryArrow.enabled = false;
					break;
				}
			}
			else
			{
				this.proposeAllianceArrow.enabled = false;
				this.endAllianceArrow.enabled = false;
				this.proposeEndRivalryArrow.enabled = false;
				this.initiateRivalryArrow.enabled = false;
			}
			this.controller.UpdateRelationsPanel();
			this.listenToggle = true;
			if (this.myNation.improveRelationsCooldowns.ContainsKey(this.otherNation) && this.myNation.improveRelationsCooldowns[this.otherNation] > TITimeState.Now())
			{
				this.notesText.SetText(this.myNation.improveRelationsCooldowns[this.otherNation].ToCustomDateString());
				return;
			}
			this.notesText.SetText(string.Empty);
		}

		// Token: 0x060052DA RID: 21210 RVA: 0x0024B84A File Offset: 0x00249A4A
		public void OnAllyToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.AllyToggleChange();
		}

		// Token: 0x060052DB RID: 21211 RVA: 0x0024B860 File Offset: 0x00249A60
		public void AllyToggleChange()
		{
			if (this.listenToggle)
			{
				if (this.proposedRelationsChanges.ContainsKey(this.otherNation))
				{
					int num = (int)this.proposedRelationsChanges[this.otherNation];
					this.controller.RemoveProposedRelationshipChange(this.otherNation);
					if (num != 1)
					{
						this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.NormalToAlly);
					}
				}
				else
				{
					this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.NormalToAlly);
				}
				this.UpdateListItem();
			}
		}

		// Token: 0x060052DC RID: 21212 RVA: 0x0024B8D9 File Offset: 0x00249AD9
		public void OnNormalToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.NormalToggleChange();
		}

		// Token: 0x060052DD RID: 21213 RVA: 0x0024B8F0 File Offset: 0x00249AF0
		public void NormalToggleChange()
		{
			if (this.listenToggle)
			{
				if (this.proposedRelationsChanges.ContainsKey(this.otherNation))
				{
					this.controller.RemoveProposedRelationshipChange(this.otherNation);
				}
				else if (this.myNation.rivals.Contains(this.otherNation))
				{
					this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.RivalToNormal);
				}
				else
				{
					this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.AllyToNormal);
				}
				this.UpdateListItem();
			}
		}

		// Token: 0x060052DE RID: 21214 RVA: 0x0024B96F File Offset: 0x00249B6F
		public void OnRivalToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.RivalToggleChange();
		}

		// Token: 0x060052DF RID: 21215 RVA: 0x0024B984 File Offset: 0x00249B84
		public void RivalToggleChange()
		{
			if (this.listenToggle)
			{
				if (this.proposedRelationsChanges.ContainsKey(this.otherNation))
				{
					int num = (int)this.proposedRelationsChanges[this.otherNation];
					this.controller.RemoveProposedRelationshipChange(this.otherNation);
					if (num != 4)
					{
						this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.NormalToRival);
					}
				}
				else
				{
					this.controller.AddProposedRelationshipChange(this.otherNation, RelationChange.NormalToRival);
				}
				this.UpdateListItem();
			}
		}

		// Token: 0x060052E0 RID: 21216 RVA: 0x0024BA00 File Offset: 0x00249C00
		public string AllyButtonTip(TINationState nation, TINationState otherNation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!nation.allies.Contains(otherNation))
			{
				stringBuilder.AppendLine(Loc.T("ProposeAllianceOption.description")).AppendLine();
				stringBuilder.AppendLine(nation.CanAllyFeedback(otherNation));
			}
			else
			{
				stringBuilder.AppendLine(nation.CanEndAllianceFeedback(otherNation));
			}
			if (nation.federation == null && otherNation.federation == null)
			{
				stringBuilder.AppendLine(nation.CanFormFederationFeedback(otherNation));
			}
			if (nation.federation != null && otherNation.federation == null)
			{
				stringBuilder.AppendLine(otherNation.CanJoinFederationFeedback(nation.federation, otherNation));
			}
			if (nation.federation == null && otherNation.federation != null)
			{
				stringBuilder.AppendLine(nation.CanJoinFederationFeedback(otherNation.federation, nation));
			}
			if (otherNation.federation != null && otherNation.federation == nation.federation)
			{
				stringBuilder.AppendLine(otherNation.CanLeaveFederationFeedback());
			}
			stringBuilder.AppendLine(nation.CanUnifyFeedback(otherNation));
			return stringBuilder.ToString();
		}

		// Token: 0x060052E1 RID: 21217 RVA: 0x0024BB24 File Offset: 0x00249D24
		public string NormalizeButtonTip(TINationState nation, TINationState otherNation)
		{
			if (nation.allies.Contains(otherNation))
			{
				return new StringBuilder(Loc.T("EndAllianceOption.description")).AppendLine().AppendLine(nation.CanEndAllianceFeedback(otherNation)).ToString();
			}
			if (nation.enemies.Contains(otherNation))
			{
				return new StringBuilder(Loc.T("EndRivalryOption.description")).AppendLine().AppendLine(nation.CanEndRivalryFeedback(otherNation)).ToString();
			}
			return new StringBuilder().AppendLine(nation.CanAllyFeedback(otherNation)).AppendLine().AppendLine(nation.CanRivalFeedback(otherNation))
				.ToString();
		}

		// Token: 0x060052E2 RID: 21218 RVA: 0x0024BBC0 File Offset: 0x00249DC0
		public string RivalryButtonTip(TINationState nation, TINationState otherNation)
		{
			if (!nation.enemies.Contains(otherNation))
			{
				return new StringBuilder(Loc.T("InitiateRivalryOption.description")).AppendLine().AppendLine(nation.CanRivalFeedback(otherNation)).ToString();
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(nation.CanEndRivalryFeedback(otherNation));
			stringBuilder.AppendLine(nation.CanAttackFeedback(otherNation));
			stringBuilder.AppendLine(otherNation.CanAttackFeedback(nation));
			return stringBuilder.ToString();
		}

		// Token: 0x040037C3 RID: 14275
		public Image nationFlag;

		// Token: 0x040037C4 RID: 14276
		public TMP_Text nationName;

		// Token: 0x040037C5 RID: 14277
		public Toggle allyToggle;

		// Token: 0x040037C6 RID: 14278
		public Toggle normalToggle;

		// Token: 0x040037C7 RID: 14279
		public Toggle rivalToggle;

		// Token: 0x040037C8 RID: 14280
		public TooltipTrigger allyTip;

		// Token: 0x040037C9 RID: 14281
		public TooltipTrigger normalizeTip;

		// Token: 0x040037CA RID: 14282
		public TooltipTrigger rivalTip;

		// Token: 0x040037CB RID: 14283
		public Image allyCheckmark;

		// Token: 0x040037CC RID: 14284
		public Image normalCheckmark;

		// Token: 0x040037CD RID: 14285
		public Image rivalCheckmark;

		// Token: 0x040037CE RID: 14286
		public Image warImage;

		// Token: 0x040037CF RID: 14287
		public Image warFlagImage;

		// Token: 0x040037D0 RID: 14288
		public TMP_Text numArmies;

		// Token: 0x040037D1 RID: 14289
		public TMP_Text numNavies;

		// Token: 0x040037D2 RID: 14290
		public TMP_Text notesText;

		// Token: 0x040037D3 RID: 14291
		private bool listenToggle;

		// Token: 0x040037D4 RID: 14292
		public TINationState myNation;

		// Token: 0x040037D5 RID: 14293
		public TINationState otherNation;

		// Token: 0x040037D6 RID: 14294
		public TMP_Text allyChance;

		// Token: 0x040037D7 RID: 14295
		public TMP_Text endRivalryChance;

		// Token: 0x040037D8 RID: 14296
		public Image proposeAllianceArrow;

		// Token: 0x040037D9 RID: 14297
		public Image endAllianceArrow;

		// Token: 0x040037DA RID: 14298
		public Image proposeEndRivalryArrow;

		// Token: 0x040037DB RID: 14299
		public Image initiateRivalryArrow;

		// Token: 0x040037DC RID: 14300
		private NationRelationsPaneController nationRelationsPaneController;
	}
}
