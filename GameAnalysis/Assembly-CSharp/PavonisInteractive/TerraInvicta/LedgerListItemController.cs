using System;
using System.Collections.Generic;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000837 RID: 2103
	public class LedgerListItemController : MonoBehaviour
	{
		// Token: 0x06004C28 RID: 19496 RVA: 0x002008AC File Offset: 0x001FEAAC
		private void SetEntryLine(LedgerEntryCategory category, float value, bool inactive = false, bool cost = false, bool percent = false)
		{
			this.values[category] = value;
			if (value == 0f)
			{
				this.SetEmptyLine(category);
				return;
			}
			string text;
			if (percent)
			{
				text = TIUtilities.ForceValueSign(value, false, true, "P0");
				if (inactive)
				{
					text = TIUtilities.HighlightLine(text);
				}
			}
			else
			{
				text = TIUtilities.FormatBigOrSmallNumber(value, 1, 7, 0, true, false);
				if (inactive)
				{
					text = TIUtilities.HighlightLine(text);
				}
				else if (cost)
				{
					text = TIUtilities.RedLine(text);
				}
				else
				{
					text = TIUtilities.GreenLine(text);
				}
			}
			this.ledgerEntry[(int)category].SetText(text);
		}

		// Token: 0x06004C29 RID: 19497 RVA: 0x0020092E File Offset: 0x001FEB2E
		private void SetEmptyLine(LedgerEntryCategory category)
		{
			this.values[category] = 0f;
			this.ledgerEntry[(int)category].SetText(string.Empty);
		}

		// Token: 0x06004C2A RID: 19498 RVA: 0x00200954 File Offset: 0x001FEB54
		private void SetListItem_Common(TIGameState state, TIDataTemplate template, TIGameState masterState, bool collapsable)
		{
			this.collapsable = collapsable;
			this.collapseButton.enabled = !collapsable;
			this.associatedState = state;
			this.associatedTemplate = template;
			this.parentGameState = masterState;
			this.indent.SetActive(collapsable);
			this.entryNameRect.sizeDelta = new Vector2((float)(collapsable ? 275 : 295), 30f);
		}

		// Token: 0x06004C2B RID: 19499 RVA: 0x002009C4 File Offset: 0x001FEBC4
		public void SetListItem(LedgerListItem_Data data, TIHabState hab)
		{
			this.SetListItem_Common(hab, null, null, false);
			this.entryName.SetText(data.entryName);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C2C RID: 19500 RVA: 0x00200A34 File Offset: 0x001FEC34
		public void SetListItem(LedgerListItem_Data data, TIHabModuleState habModule)
		{
			this.SetListItem_Common(habModule, null, habModule.hab, true);
			this.entryName.SetText(data.entryName);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C2D RID: 19501 RVA: 0x00200AA8 File Offset: 0x001FECA8
		public void SetListItem(LedgerListItem_Data data, TIFactionState faction, int which)
		{
			this.SetListItem_Common(faction, null, null, false);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			this.entryName.SetText(data.entryName);
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C2E RID: 19502 RVA: 0x00200B18 File Offset: 0x001FED18
		public void SetListItem(LedgerListItem_Data data, TISpaceFleetState fleet)
		{
			this.SetListItem_Common(fleet, null, null, false);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			this.entryName.SetText(data.entryName);
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C2F RID: 19503 RVA: 0x00200B88 File Offset: 0x001FED88
		public void SetListItem(LedgerListItem_Data data, TISpaceShipState ship)
		{
			this.SetListItem_Common(ship, null, ship.fleet, true);
			this.entryName.SetText(data.entryName);
			this.icon.enabled = false;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C30 RID: 19504 RVA: 0x00200BEC File Offset: 0x001FEDEC
		public void SetListItem(LedgerListItem_Data data, TINationState nation, TIFactionState faction)
		{
			this.SetListItem_Common(nation, null, faction, false);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			this.entryName.SetText(data.entryName);
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C31 RID: 19505 RVA: 0x00200C5C File Offset: 0x001FEE5C
		public void SetListItem(LedgerListItem_Data data, TICouncilorState councilor)
		{
			this.SetListItem_Common(councilor, null, null, false);
			this.entryName.SetText(data.entryName);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C32 RID: 19506 RVA: 0x00200CCC File Offset: 0x001FEECC
		public void SetListItem(LedgerListItem_Data data, TIOrgState org)
		{
			this.SetListItem_Common(org, null, org.hasCouncilor ? org.assignedCouncilor.ref_gameState : org.unassignedCouncil.ref_gameState, true);
			this.entryName.SetText(data.entryName);
			this.icon.sprite = data.entryIconSprite;
			this.icon.enabled = true;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C33 RID: 19507 RVA: 0x00200D58 File Offset: 0x001FEF58
		public void SetListItem(LedgerListItem_Data data, TITraitTemplate trait, TICouncilorState councilor)
		{
			this.SetListItem_Common(null, trait, councilor, true);
			this.entryName.SetText(data.entryName);
			this.icon.enabled = false;
			for (int i = 0; i < this.ledgerEntry.Length; i++)
			{
				this.ledgerEntry[i].SetText(data.ledgerValueText[i]);
			}
		}

		// Token: 0x06004C34 RID: 19508 RVA: 0x00200DB4 File Offset: 0x001FEFB4
		public void OnClickTextButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.parentController.LedgerCollapseListItem(this.associatedState);
		}

		// Token: 0x06004C35 RID: 19509 RVA: 0x00200DD4 File Offset: 0x001FEFD4
		public void OnClickIconButton()
		{
			TIGameState tigameState = this.associatedState ?? this.parentGameState;
			if (!TIGameState.Valid(tigameState))
			{
				return;
			}
			SoundEffectController.PlaySelectSound(tigameState);
			if (tigameState.isHabState || tigameState.isHabModuleState)
			{
				GameControl.eventManager.TriggerEvent(new HabDetailRequested(tigameState.ref_hab, true), null, Array.Empty<object>());
				return;
			}
			if (tigameState.isCouncilorState || tigameState.isOrgState)
			{
				GameControl.eventManager.TriggerEvent(new CouncilorDetailRequested(tigameState.ref_councilor), null, Array.Empty<object>());
				return;
			}
			this.parentController.CloseInfoScreen(false);
			TIUtilities.GotoGameState(tigameState, true, true, true, true, false, -1f);
		}

		// Token: 0x04002DD9 RID: 11737
		public GameObject indent;

		// Token: 0x04002DDA RID: 11738
		public Image icon;

		// Token: 0x04002DDB RID: 11739
		public TMP_Text entryName;

		// Token: 0x04002DDC RID: 11740
		public RectTransform entryNameRect;

		// Token: 0x04002DDD RID: 11741
		public bool collapsable;

		// Token: 0x04002DDE RID: 11742
		[fsIgnore]
		public TIGameState associatedState;

		// Token: 0x04002DDF RID: 11743
		[fsIgnore]
		public TIDataTemplate associatedTemplate;

		// Token: 0x04002DE0 RID: 11744
		[fsIgnore]
		public TIGameState parentGameState;

		// Token: 0x04002DE1 RID: 11745
		public TMP_Text[] ledgerEntry;

		// Token: 0x04002DE2 RID: 11746
		[fsIgnore]
		public Dictionary<LedgerEntryCategory, float> values = new Dictionary<LedgerEntryCategory, float>();

		// Token: 0x04002DE3 RID: 11747
		public Button collapseButton;

		// Token: 0x04002DE4 RID: 11748
		public CouncilGridController parentController;
	}
}
