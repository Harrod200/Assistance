using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000896 RID: 2198
	public class RegionListItemController : MonoBehaviour
	{
		// Token: 0x06005302 RID: 21250 RVA: 0x0024D2CC File Offset: 0x0024B4CC
		public void UpdateListItem(RegionListItem_Data data)
		{
			this.region = data.regionState;
			TIFactionState activePlayer = GameControl.control.activePlayer;
			this.regionName.SetText(data.regionNameString);
			this.regionPop.text = this.region.populationInMillions.ToString("N2");
			this.regionBoost.text = (this.region.canLaunch ? TIUtilities.FormatBigOrSmallNumber(this.region.boostPerMonth_dekatons, 1, 2, 0, false, false) : "-");
			this.regionMC.text = Loc.T("UI.Nation.MissionControlValue", new object[]
			{
				(this.region.missionControl > 0) ? this.region.missionControl.ToString("N0") : "-",
				this.region.maxMissionControl.ToString("N0")
			});
			this.regionTooltip.SetDelegate("BodyText", () => NationInfoController.BuildRegionDataTooltip(this.region, activePlayer, data.viewingNation));
			if (this.region.isBeingAnnexed)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(OperationsManager.operationsLookup[typeof(AnnexRegionOperation)].GetOperationIconImagePath_Off(), this.occupierFlag);
				this.occupationPct.SetText(Loc.T("UI.Nation.ExecutiveConsolidationTimer", new object[] { this.region.annexationDaysLeft.ToString("N0") }));
				this.occupierFlag.enabled = true;
				this.occupationPct.enabled = true;
			}
			else if (this.region.OccupiedOrOccupationUnderway())
			{
				TINationState tinationState;
				List<TINationState> list;
				float highestWarAllianceOccupationValue = this.region.GetHighestWarAllianceOccupationValue(out tinationState, out list);
				if (tinationState != null)
				{
					this.occupierFlag.sprite = tinationState.flag;
					string text = highestWarAllianceOccupationValue.ToPercent("P0");
					this.occupationPct.SetText(text);
					this.occupierFlag.enabled = true;
					this.occupationPct.enabled = true;
				}
				else
				{
					this.occupierFlag.enabled = false;
					this.occupationPct.enabled = false;
				}
			}
			else
			{
				this.occupierFlag.enabled = false;
				this.occupationPct.enabled = false;
			}
			if (data.abductionsEnabled)
			{
				this.abductions.enabled = true;
				this.abductions.SetText(data.abductionsText);
			}
			else
			{
				this.abductions.enabled = false;
			}
			this.regionName.enabled = true;
			this.regionPop.enabled = true;
			this.regionBoost.enabled = true;
			this.regionMC.enabled = true;
			this.regionTooltip.enabled = true;
			this.backgroundImage.color = (data.claim ? (data.hostileClaim_perm ? RegionListItemController.hostileClaimColor : (data.hostileClaim_temp ? RegionListItemController.tempHostileClaimColor : RegionListItemController.friendlyClaimColor)) : Color.clear);
			List<TINationState> claimsOnRegion = data.claimsOnRegion;
			int num = math.min(claimsOnRegion.Count, 6);
			this.claimsList.SetListSize<ClaimListItemController>(num, false, false);
			int num2 = 0;
			float num3 = ((claimsOnRegion.Count == 1) ? 30f : 22f);
			float num4 = (float)(60 / math.max(num, 1));
			using (IEnumerator<object> enumerator = this.claimsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (RegionListItemController.<>o__15.<>p__0 == null)
					{
						RegionListItemController.<>o__15.<>p__0 = CallSite<Func<CallSite, object, ClaimListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ClaimListItemController), typeof(RegionListItemController)));
					}
					ClaimListItemController claimListItemController = RegionListItemController.<>o__15.<>p__0.Target(RegionListItemController.<>o__15.<>p__0, enumerator.Current);
					if (num2 < claimsOnRegion.Count)
					{
						claimListItemController.UpdateListItem(claimsOnRegion[num2], this.region);
						claimListItemController.transform.localPosition = new Vector3(num3 + num4 * (float)num2, 0f, 0f);
						claimListItemController.gameObject.SetActive(true);
					}
					else
					{
						claimListItemController.gameObject.SetActive(false);
					}
					num2++;
				}
			}
		}

		// Token: 0x06005303 RID: 21251 RVA: 0x0024D720 File Offset: 0x0024B920
		public void OnRegionButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
			TIUtilities.GotoGameState(this.region, true, true, true, true, false, -1f);
		}

		// Token: 0x040037F4 RID: 14324
		private TIRegionState region;

		// Token: 0x040037F5 RID: 14325
		public TMP_Text regionName;

		// Token: 0x040037F6 RID: 14326
		public TMP_Text regionPop;

		// Token: 0x040037F7 RID: 14327
		public TMP_Text regionBoost;

		// Token: 0x040037F8 RID: 14328
		public TMP_Text regionMC;

		// Token: 0x040037F9 RID: 14329
		public LayoutGroup ClaimsPanel;

		// Token: 0x040037FA RID: 14330
		public TooltipTrigger regionTooltip;

		// Token: 0x040037FB RID: 14331
		public Image occupierFlag;

		// Token: 0x040037FC RID: 14332
		public TMP_Text occupationPct;

		// Token: 0x040037FD RID: 14333
		public Image backgroundImage;

		// Token: 0x040037FE RID: 14334
		public ListManagerBase claimsList;

		// Token: 0x040037FF RID: 14335
		public TMP_Text abductions;

		// Token: 0x04003800 RID: 14336
		public static readonly Color friendlyClaimColor = new Color(0f, 0.67058825f, 0.4f, 0.1254902f);

		// Token: 0x04003801 RID: 14337
		public static readonly Color tempHostileClaimColor = new Color(1f, 0.25f, 0f, 0.1254902f);

		// Token: 0x04003802 RID: 14338
		public static readonly Color hostileClaimColor = new Color(0.17254902f, 0f, 0.043137256f, 0.49803922f);
	}
}
