using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008D6 RID: 2262
	public class OrbitsListItemController : MonoBehaviour
	{
		// Token: 0x060056A0 RID: 22176 RVA: 0x0027A207 File Offset: 0x00278407
		public void Init(TIOrbitState orbit)
		{
			this.orbit = orbit;
		}

		// Token: 0x060056A1 RID: 22177 RVA: 0x0027A210 File Offset: 0x00278410
		public void UpdateListItem()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.orbit.irradiated)
			{
				stringBuilder.Append(TemplateManager.global.irradiatedInlineSpritePath);
			}
			stringBuilder.Append(this.orbit.displayName);
			if (this.orbit.destroyedAssets > 0)
			{
				stringBuilder.Append(TemplateManager.global.spaceDebrisInlineSpritePath);
			}
			this.orbitName.SetText(stringBuilder.ToString());
			if (this.interfaceOrbit != null)
			{
				if (this.orbit.interfaceOrbit)
				{
					this.interfaceOrbit.sprite = this.orbit.barycenter.icon;
					this.interfaceOrbit.enabled = true;
				}
				else
				{
					this.interfaceOrbit.enabled = false;
				}
			}
			if (this.orbit.amat_ugpy > 0f)
			{
				string text = TIUtilities.FormatBigOrSmallNumber(this.orbit.antimatterPerMonth_dekatonnes, 1, 3, 0, true, false);
				this.amatProduction.SetText(text);
			}
			else
			{
				this.amatProduction.SetText("-");
			}
			this.orbitAltitude.SetText(this.orbit.altitude_km.ToString("N0"));
			TMP_Text tmp_Text = this.habCapacity;
			string text2 = "UI.Space.Stations";
			object[] array = new object[2];
			array[0] = this.orbit.stationsInOrbit.Where<TIHabState>((TIHabState z) => z.VisibleToFaction(GameControl.control.activePlayer)).Count<TIHabState>().ToString("N0");
			array[1] = this.orbit.stationCapacity.ToString("N0");
			tmp_Text.SetText(Loc.T(text2, array));
			if (this.orbit.localGravity_gs >= 1E-06)
			{
				this.accel_g_kps.SetText(FleetsScreenController.accelerationStr(this.orbit.localGravity_gs, false, false, true));
			}
			else
			{
				this.accel_g_kps.SetText(Loc.T("UI.Space.Negligible"));
			}
			this.orbitTip.SetDelegate("BodyText", () => TIOrbitState.OrbitTooltip(this.orbit));
		}

		// Token: 0x060056A2 RID: 22178 RVA: 0x0027A41D File Offset: 0x0027861D
		public void OnOrbitButtonPressed()
		{
			SoundEffectController.PlaySelectSound(this.orbit);
			TIUtilities.GotoGameState(this.orbit, true, true, true, true, false, -1f);
		}

		// Token: 0x04003DC3 RID: 15811
		private TIOrbitState orbit;

		// Token: 0x04003DC4 RID: 15812
		public TMP_Text orbitName;

		// Token: 0x04003DC5 RID: 15813
		public TMP_Text orbitAltitude;

		// Token: 0x04003DC6 RID: 15814
		public TMP_Text amatProduction;

		// Token: 0x04003DC7 RID: 15815
		public TMP_Text habCapacity;

		// Token: 0x04003DC8 RID: 15816
		public Image interfaceOrbit;

		// Token: 0x04003DC9 RID: 15817
		public TMP_Text accel_g_kps;

		// Token: 0x04003DCA RID: 15818
		public TooltipTrigger orbitTip;
	}
}
