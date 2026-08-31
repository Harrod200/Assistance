using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000585 RID: 1413
	public class STOFighterController : HumanShipController
	{
		// Token: 0x0600252F RID: 9519 RVA: 0x000C81BC File Offset: 0x000C63BC
		public void SetFlagMaterial(TINationState nation)
		{
			this.customMaterial = global::UnityEngine.Object.Instantiate<Material>(this.flagMaterial);
			if (nation != null)
			{
				this.customMaterial.SetTexture("_MainTex", nation.flag.texture);
				this.customMaterial.name = this.flagMaterial.name + nation.templateName;
				this.customMaterial.color = Color.white;
			}
			else
			{
				this.customMaterial.name = this.flagMaterial.name + "NoNation";
				this.customMaterial.color = Colors.Transparent;
			}
			this.customMaterial.hideFlags = HideFlags.DontSave;
			Material[] materials = this.hullModel.GetComponentInChildren<MeshRenderer>().materials;
			materials[1] = this.customMaterial;
			this.hullModel.GetComponentInChildren<MeshRenderer>().materials = materials;
		}

		// Token: 0x06002530 RID: 9520 RVA: 0x000C8299 File Offset: 0x000C6499
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			return new List<GameObject>();
		}

		// Token: 0x06002531 RID: 9521 RVA: 0x000C82A0 File Offset: 0x000C64A0
		public override int SlotToWeaponMountIndex(int slot, Mount mount)
		{
			return 0;
		}

		// Token: 0x04001BCF RID: 7119
		public Material flagMaterial;

		// Token: 0x04001BD0 RID: 7120
		public Material customMaterial;
	}
}
