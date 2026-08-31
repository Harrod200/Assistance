using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000894 RID: 2196
	public class PolicyDisplayListItemController : MonoBehaviour
	{
		// Token: 0x060052EC RID: 21228 RVA: 0x0024BF8C File Offset: 0x0024A18C
		public void SetListItem(TIPolicyOption policyOption, TINationState nationState)
		{
			this.policyNameContainer.SetActive(true);
			this.policyName.SetText(TIUtilities.HighlightLine(policyOption.GetDisplayName()));
			StringBuilder stringBuilder = new StringBuilder();
			IList<TIGameState> possibleTargets = policyOption.GetPossibleTargets(nationState);
			if (policyOption.RequiresTargets() && (possibleTargets.Count > 1 || (possibleTargets.Count == 1 && possibleTargets[0] != nationState)))
			{
				this.policyCandidatesContainer.SetActive(true);
				stringBuilder.Append(Loc.T("UI.Nation.PolicyCandidates"));
				int num = 1;
				foreach (TIGameState tigameState in possibleTargets)
				{
					if (num < possibleTargets.Count)
					{
						stringBuilder.Append(Loc.T("UI.Nation.PolicyCandidatesListItem", new object[] { tigameState.displayName }));
						num++;
					}
					else
					{
						stringBuilder.Append(" ").Append(tigameState.displayName);
					}
				}
				this.policyCandidates.SetText(stringBuilder.ToString());
			}
			else
			{
				this.policyCandidatesContainer.SetActive(false);
			}
			this.policyTip.SetDelegate("BodyText", () => PolicyDisplayListItemController.policyTipStr(policyOption, nationState));
			this.policyTip.enabled = true;
			this.mainVerticalLayout.enabled = true;
			this.candidatesVerticalLayout.enabled = true;
		}

		// Token: 0x060052ED RID: 21229 RVA: 0x0024C128 File Offset: 0x0024A328
		public void SetListItemAsCooldowns(TINationState nationState)
		{
			if (nationState.improveRelationsCooldowns.Any<KeyValuePair<TINationState, TIDateTime>>((KeyValuePair<TINationState, TIDateTime> x) => x.Key.extant && x.Value >= TITimeState.Now()))
			{
				this.policyNameContainer.SetActive(false);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.Cooldowns")));
				IOrderedEnumerable<TINationState> orderedEnumerable = nationState.improveRelationsCooldowns.Keys.OrderBy<TINationState, string>((TINationState x) => x.displayName);
				Func<TINationState, TIDateTime> <>9__2;
				Func<TINationState, TIDateTime> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (TINationState x) => nationState.improveRelationsCooldowns[x]);
				}
				foreach (TINationState tinationState in orderedEnumerable.ThenByDescending<TINationState, TIDateTime>(func))
				{
					if (tinationState.extant && nationState.improveRelationsCooldowns[tinationState] >= TITimeState.Now())
					{
						stringBuilder.AppendLine(Loc.T("UI.Nation.CooldownItem", new object[]
						{
							tinationState.displayName,
							nationState.improveRelationsCooldowns[tinationState].ToCustomDateString()
						}));
					}
				}
				this.policyCandidates.SetText(stringBuilder);
			}
			this.policyCandidatesContainer.SetActive(true);
			this.policyTip.enabled = false;
			this.mainVerticalLayout.enabled = true;
			this.candidatesVerticalLayout.enabled = true;
		}

		// Token: 0x060052EE RID: 21230 RVA: 0x0024C2C8 File Offset: 0x0024A4C8
		public static string policyTipStr(TIPolicyOption policyOption, TINationState nationState)
		{
			StringBuilder stringBuilder = new StringBuilder(TIUtilities.HighlightLine(policyOption.GetDisplayName())).Append("\n\n");
			string text = policyOption.GetDescription();
			if (policyOption.GetPolicyType() == PolicyType.TransferRegionsOption && policyOption.GetPossibleTargets(nationState).Contains(GameStateManager.AlienNation()))
			{
				text = new StringBuilder(text).Append(Loc.T("TransferRegionsOption.specialDescription")).ToString();
			}
			stringBuilder.Append(text);
			return stringBuilder.ToString();
		}

		// Token: 0x040037E3 RID: 14307
		public TMP_Text policyName;

		// Token: 0x040037E4 RID: 14308
		public TMP_Text policyCandidates;

		// Token: 0x040037E5 RID: 14309
		public GameObject policyNameContainer;

		// Token: 0x040037E6 RID: 14310
		public GameObject policyCandidatesContainer;

		// Token: 0x040037E7 RID: 14311
		public VerticalLayoutGroup mainVerticalLayout;

		// Token: 0x040037E8 RID: 14312
		public VerticalLayoutGroup candidatesVerticalLayout;

		// Token: 0x040037E9 RID: 14313
		public TooltipTrigger policyTip;
	}
}
