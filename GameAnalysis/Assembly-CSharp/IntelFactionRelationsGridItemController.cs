using System;
using System.Collections.Generic;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200042E RID: 1070
public class IntelFactionRelationsGridItemController : MonoBehaviour
{
	// Token: 0x06001639 RID: 5689 RVA: 0x00071328 File Offset: 0x0006F528
	public void SetListItem(TIFactionState primaryFactioninUI, TIFactionState judgedFaction)
	{
		this.factionIcon.sprite = judgedFaction.factionIcon64UI;
		this.primaryFactioninUI = primaryFactioninUI;
		this.judgedFaction = judgedFaction;
		StringBuilder stringBuilder = new StringBuilder();
		string text = "";
		if (primaryFactioninUI.HasTruce(judgedFaction, false))
		{
			text = stringBuilder.Append(", ").Append(Loc.T("UI.Notifications.Diplomacy.Truce")).ToString();
			this.cancelTreatyButtonObject.SetActive(false);
		}
		else if (primaryFactioninUI.HasNAP(judgedFaction, false))
		{
			text = stringBuilder.Append(", ").Append(Loc.T("UI.Notifications.Diplomacy.NAP")).ToString();
			if (primaryFactioninUI.intelSharingFactions.Contains(judgedFaction))
			{
				text = stringBuilder.Append(", ").Append(Loc.T("UI.Notifications.Diplomacy.IntelSharing")).ToString();
				this.cancelTreatyButtonTip.SetDelegate("BodyText", () => Loc.T("UI.Intel.Faction.Relations.CancelTreaty_Intel"));
				this.cancelTreatyButtonObject.SetActive(GameControl.control.activePlayer == judgedFaction);
			}
			else
			{
				this.cancelTreatyButtonTip.SetDelegate("BodyText", () => Loc.T("UI.Intel.Faction.Relations.CancelTreaty_NAP"));
				this.cancelTreatyButtonObject.SetActive(GameControl.control.activePlayer == judgedFaction && !primaryFactioninUI.permanentAlly(judgedFaction));
			}
		}
		else
		{
			this.cancelTreatyButtonObject.SetActive(false);
		}
		stringBuilder.Clear();
		if (primaryFactioninUI.permanentAlly(judgedFaction))
		{
			this.attitudeDescription.SetText(stringBuilder.Append(Loc.T("UI.Intel.FactionLove")).Append(text));
		}
		else if (primaryFactioninUI.FindGoals(GoalType.WarOnFaction, primaryFactioninUI, judgedFaction, TIFactionState.GoalFilter.none, true).Count > 0)
		{
			this.attitudeDescription.SetText(stringBuilder.Append(Loc.T("UI.Intel.FactionWar")).Append(text));
		}
		else if (primaryFactioninUI.GetFactionHate(judgedFaction) <= 0f)
		{
			this.attitudeDescription.SetText(stringBuilder.Append(Loc.T("UI.Intel.FactionHate0")).Append(text));
		}
		else
		{
			this.attitudeDescription.SetText(stringBuilder.Append(Loc.T("UI.Intel.FactionHate10")).Append(text));
		}
		if (TIGlobalConfig.globalConfig.debug_showHateValues)
		{
			TMP_Text tmp_Text = this.attitudeDescription;
			tmp_Text.text = tmp_Text.text + " (" + primaryFactioninUI.GetFactionHate(judgedFaction).ToString() + " hate)";
		}
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x000715A4 File Offset: 0x0006F7A4
	public void OnClickCancelTreaty()
	{
		if (GameControl.control.activePlayer.intelSharingFactions.Contains(this.primaryFactioninUI))
		{
			GameControl.control.activePlayer.playerControl.StartAction(new BreakPactAction(GameControl.control.activePlayer, this.primaryFactioninUI, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.Intel }));
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		}
		else
		{
			GameControl.control.activePlayer.playerControl.StartAction(new BreakPactAction(GameControl.control.activePlayer, this.primaryFactioninUI, new List<TradeOffer.TreatyType> { TradeOffer.TreatyType.NAP }));
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		}
		this.SetListItem(this.primaryFactioninUI, this.judgedFaction);
	}

	// Token: 0x04001479 RID: 5241
	private TIFactionState primaryFactioninUI;

	// Token: 0x0400147A RID: 5242
	private TIFactionState judgedFaction;

	// Token: 0x0400147B RID: 5243
	public Image factionIcon;

	// Token: 0x0400147C RID: 5244
	public TMP_Text attitudeDescription;

	// Token: 0x0400147D RID: 5245
	public GameObject cancelTreatyButtonObject;

	// Token: 0x0400147E RID: 5246
	public TooltipTrigger cancelTreatyButtonTip;
}
