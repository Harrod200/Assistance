using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D2 RID: 2258
	public class DebugLinkHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		// Token: 0x06005692 RID: 22162 RVA: 0x00279AD8 File Offset: 0x00277CD8
		public void OnPointerClick(PointerEventData eventData)
		{
			int num = TMP_TextUtilities.FindIntersectingLink(this.Text, eventData.position, null);
			if (num < 0)
			{
				return;
			}
			TMP_LinkInfo tmp_LinkInfo = this.Text.textInfo.linkInfo[num];
			TIGameState tigameState = GameStateManager.FindGameState(int.Parse(tmp_LinkInfo.GetLinkID()));
			TIFactionGoalState tifactionGoalState = tigameState as TIFactionGoalState;
			if (tifactionGoalState != null)
			{
				FactionGoal_Fleet factionGoal_Fleet = tifactionGoalState as FactionGoal_Fleet;
				if (factionGoal_Fleet != null)
				{
					Log.Debug(tifactionGoalState.ToString() + " pending ships : " + factionGoal_Fleet.PendingShips().ToCommaSeparatedString<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => string.Concat(new string[]
					{
						x.shipDesignTemplateName,
						" with ",
						x.shipDesign.TemplateSpaceCombatValue(false, -1f, 1f, false).ToString(),
						" strength at ",
						x.shipyard.GetDebugString(false),
						" at ",
						x.shipyard.GetLocationDebugString(false),
						". Cost paid? ",
						x.costPaid ? "true" : "false",
						". Is refit? ",
						x.isRefit ? "true" : "false",
						". Days to completion? ",
						x.daysToCompletion.Round().ToString(),
						". Days waiting? ",
						(TITimeState.Now() - x.startDate).TotalDays.ToString()
					})), Array.Empty<object>());
					return;
				}
			}
			else
			{
				TIUtilities.GotoGameState(tigameState, true, true, true, true, false, -1f);
			}
		}

		// Token: 0x04003DAA RID: 15786
		public TMP_Text Text;
	}
}
