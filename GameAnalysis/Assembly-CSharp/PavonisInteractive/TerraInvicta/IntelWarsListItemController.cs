using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000885 RID: 2181
	public class IntelWarsListItemController : MonoBehaviour
	{
		// Token: 0x0600519D RID: 20893 RVA: 0x0023E3F4 File Offset: 0x0023C5F4
		public void SetListItem(TIWarState warData)
		{
			this.warName.SetText(warData.displayName);
			this.warActiveText.SetText(Loc.T("UI.Intel.WarActive"));
			this.warDurationText.SetText(Loc.T("UI.Intel.WarDuration", new object[] { Mathf.FloorToInt((float)GameStateManager.Time().Time_Now().DifferenceInDays(warData.startDate)) }));
			TINationState attackingAllianceLeader = warData.attackingAllianceLeader;
			TINationState defendingAllianceLeader = warData.defendingAllianceLeader;
			if (warData.attackingAlliance.Count > 1)
			{
				this.attackerLeaderNation.SetText(Loc.T("UI.Intel.AllianceLeader", new object[] { attackingAllianceLeader.displayName }));
			}
			else
			{
				this.attackerLeaderNation.SetText(attackingAllianceLeader.displayName);
			}
			if (warData.defendingAlliance.Count > 1)
			{
				this.defenderLeaderNation.SetText(Loc.T("UI.Intel.AllianceLeader", new object[] { defendingAllianceLeader.displayName }));
			}
			else
			{
				this.defenderLeaderNation.SetText(defendingAllianceLeader.displayName);
			}
			if (attackingAllianceLeader.executiveFaction != null)
			{
				this.attackerFactionObject.SetActive(true);
				this.attackerFactionIcon.sprite = attackingAllianceLeader.executiveFaction.factionIcon64UI;
			}
			else
			{
				this.attackerFactionObject.SetActive(false);
			}
			if (defendingAllianceLeader.executiveFaction != null)
			{
				this.defenderFactionObject.SetActive(true);
				this.defenderFactionIcon.sprite = defendingAllianceLeader.executiveFaction.factionIcon64UI;
			}
			else
			{
				this.defenderFactionObject.SetActive(false);
			}
			int num = warData.attackingAlliance.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies.Where<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy)).Count<TIArmyState>();
			int num2 = warData.defendingAlliance.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies.Where<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy)).Count<TIArmyState>();
			if (num > 0)
			{
				this.attackerArmiesObject.SetActive(true);
				this.attackerArmiesText.SetText(num.ToString("N0"));
			}
			else
			{
				this.attackerArmiesObject.SetActive(false);
			}
			if (num2 > 0)
			{
				this.defenderArmiesObject.SetActive(true);
				this.defenderArmiesText.SetText(num2.ToString("N0"));
			}
			else
			{
				this.defenderArmiesObject.SetActive(false);
			}
			this.attackerNuclearObject.SetActive(warData.attackingAlliance.Sum<TINationState>((TINationState x) => x.numNuclearWeapons) > 0);
			this.defenderNuclearObject.SetActive(warData.defendingAlliance.Sum<TINationState>((TINationState x) => x.numNuclearWeapons) > 0);
			this.attackerNavalObject.SetActive(attackingAllianceLeader.navalFreedom);
			this.defenderNavalObject.SetActive(defendingAllianceLeader.navalFreedom);
			this.attackerFlagsList.SetListSize<ClaimListItemController>(warData.attackingAlliance.Count, false, false);
			int num3 = 0;
			using (IEnumerator<object> enumerator = this.attackerFlagsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelWarsListItemController.<>o__21.<>p__0 == null)
					{
						IntelWarsListItemController.<>o__21.<>p__0 = CallSite<Func<CallSite, object, ClaimListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ClaimListItemController), typeof(IntelWarsListItemController)));
					}
					IntelWarsListItemController.<>o__21.<>p__0.Target(IntelWarsListItemController.<>o__21.<>p__0, enumerator.Current).UpdateListItem(warData.attackingAlliance[num3++], null);
				}
			}
			this.attackerFlagsLayout.spacing = (float)this.GetFlagSpacing(warData.attackingAlliance.Count);
			this.defenderFlagsList.SetListSize<ClaimListItemController>(warData.defendingAlliance.Count, false, false);
			num3 = 0;
			using (IEnumerator<object> enumerator = this.defenderFlagsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelWarsListItemController.<>o__21.<>p__1 == null)
					{
						IntelWarsListItemController.<>o__21.<>p__1 = CallSite<Func<CallSite, object, ClaimListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ClaimListItemController), typeof(IntelWarsListItemController)));
					}
					IntelWarsListItemController.<>o__21.<>p__1.Target(IntelWarsListItemController.<>o__21.<>p__1, enumerator.Current).UpdateListItem(warData.defendingAlliance[num3++], null);
				}
			}
			this.defenderFlagsLayout.spacing = (float)this.GetFlagSpacing(warData.defendingAlliance.Count);
		}

		// Token: 0x0600519E RID: 20894 RVA: 0x0023E86C File Offset: 0x0023CA6C
		public int GetFlagSpacing(int allianceSize)
		{
			return Mathf.Clamp(allianceSize * -4 + 28, -46, -14);
		}

		// Token: 0x04003626 RID: 13862
		public TMP_Text warName;

		// Token: 0x04003627 RID: 13863
		public TMP_Text warActiveText;

		// Token: 0x04003628 RID: 13864
		public TMP_Text warDurationText;

		// Token: 0x04003629 RID: 13865
		public TMP_Text attackerLeaderNation;

		// Token: 0x0400362A RID: 13866
		public TMP_Text defenderLeaderNation;

		// Token: 0x0400362B RID: 13867
		public GameObject attackerFactionObject;

		// Token: 0x0400362C RID: 13868
		public Image attackerFactionIcon;

		// Token: 0x0400362D RID: 13869
		public GameObject attackerNuclearObject;

		// Token: 0x0400362E RID: 13870
		public GameObject attackerNavalObject;

		// Token: 0x0400362F RID: 13871
		public GameObject attackerArmiesObject;

		// Token: 0x04003630 RID: 13872
		public TMP_Text attackerArmiesText;

		// Token: 0x04003631 RID: 13873
		public GameObject defenderFactionObject;

		// Token: 0x04003632 RID: 13874
		public Image defenderFactionIcon;

		// Token: 0x04003633 RID: 13875
		public GameObject defenderNuclearObject;

		// Token: 0x04003634 RID: 13876
		public GameObject defenderNavalObject;

		// Token: 0x04003635 RID: 13877
		public GameObject defenderArmiesObject;

		// Token: 0x04003636 RID: 13878
		public TMP_Text defenderArmiesText;

		// Token: 0x04003637 RID: 13879
		public ListManagerBase attackerFlagsList;

		// Token: 0x04003638 RID: 13880
		public ListManagerBase defenderFlagsList;

		// Token: 0x04003639 RID: 13881
		public HorizontalLayoutGroup attackerFlagsLayout;

		// Token: 0x0400363A RID: 13882
		public HorizontalLayoutGroup defenderFlagsLayout;
	}
}
