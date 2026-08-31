using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000856 RID: 2134
	public class ShipDetailModuleListItemController : MonoBehaviour
	{
		// Token: 0x06004E5B RID: 20059 RVA: 0x0021B6FC File Offset: 0x002198FC
		public void Init(FleetsScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06004E5C RID: 20060 RVA: 0x0021B708 File Offset: 0x00219908
		public void UpdateListItem(ModuleDataEntry module, TIShipPartTemplate part, TISpaceShipState ship)
		{
			if (part.isWeapon && part.ref_projectileWeapon != null && part.ref_projectileWeapon.hasMagazine())
			{
				this.ModuleDataText.SetText(part.displayName);
				int num = part.ref_projectileWeapon.FullAmmoCount_Current(ship);
				int num2 = part.ref_projectileWeapon.FullAmmoCount_Max(ship.template);
				if (num == num2)
				{
					this.rightquantityText.SetText(Loc.T("UI.Fleets.TwoValues", new object[]
					{
						ship.ammo[module].ToString(),
						num.ToString()
					}));
				}
				else
				{
					this.rightquantityText.SetText(Loc.T("UI.Fleets.ThreeValues", new object[]
					{
						ship.ammo[module].ToString(),
						num.ToString(),
						num2.ToString()
					}));
				}
				if (ship.GetPartDamage(module) > 0f)
				{
					this.ModuleDataText.SetText(TIUtilities.RedLine(this.ModuleDataText.text));
					this.rightquantityText.SetText(TIUtilities.RedLine(this.rightquantityText.text));
				}
				else
				{
					this.ModuleDataText.SetText(TIUtilities.CyanLine(this.ModuleDataText.text));
					this.rightquantityText.SetText(TIUtilities.CyanLine(this.rightquantityText.text));
				}
			}
			else
			{
				this.rightquantityText.text = "";
				if (ship.GetPartFunction(module) <= 0f)
				{
					this.ModuleDataText.SetText(TIUtilities.RedLine(part.displayName));
				}
				else if (ship.GetPartFunction(module) < 1f)
				{
					this.ModuleDataText.SetText(TIUtilities.YellowLine(part.displayName));
				}
				else
				{
					this.ModuleDataText.SetText(part.displayName);
				}
			}
			if (part.iconResource != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(part.iconResource, this.moduleIcon);
				return;
			}
			Debug.LogError("Icon path missing for " + part.displayName);
		}

		// Token: 0x040031F1 RID: 12785
		private FleetsScreenController controller;

		// Token: 0x040031F2 RID: 12786
		public TMP_Text ModuleDataText;

		// Token: 0x040031F3 RID: 12787
		public TMP_Text rightquantityText;

		// Token: 0x040031F4 RID: 12788
		public Image moduleIcon;

		// Token: 0x040031F5 RID: 12789
		public int systemIndex;
	}
}
