using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000588 RID: 1416
	public abstract class HumanShipController : ShipModelController
	{
		// Token: 0x060025AD RID: 9645 RVA: 0x000CBA7C File Offset: 0x000C9C7C
		public override void SetSkin(TISpaceShipTemplate ship)
		{
			string shipMaterialSuffix = TIFactionTemplate.GetShipMaterialSuffix(ship.designingFaction);
			foreach (object obj in this.hullModel.transform)
			{
				Transform transform = (Transform)obj;
				MeshRenderer component = transform.GetComponent<MeshRenderer>();
				if (component != null && !transform.gameObject.name.ToLowerInvariant().Contains("common"))
				{
					component.sharedMaterial = GameControl.assetLoader.LoadAsset<Material>(new StringBuilder(ship.designingFaction.template.GetShipMaterialBundlePath(ship.GetHullAppearanceIndex)).Append("/MAT_").Append(transform.gameObject.name).Append(shipMaterialSuffix)
						.ToString());
				}
			}
			STOFighterController stofighterController = this as STOFighterController;
			if (stofighterController != null)
			{
				stofighterController.SetFlagMaterial(ship.nation);
			}
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x000CBB80 File Offset: 0x000C9D80
		public override List<GameObject> WhichRadiators(TISpaceShipTemplate ship)
		{
			List<GameObject> list = new List<GameObject>();
			float num = 0f;
			if (ship.powerPlantTemplate != null && ship.driveTemplate != null)
			{
				num = ship.radiatorTemplate.radiatorArea_m2(ship.wasteHeat_GW);
			}
			switch (ship.radiatorTemplate.radiatorType)
			{
			case RadiatorType.Fin:
				if (num < 800f)
				{
					list.Add(this.radiator3);
					list.Add(this.radiator9);
				}
				else if (num < 1200f)
				{
					list.Add(this.radiator12);
					list.Add(this.radiator4);
					list.Add(this.radiator8);
				}
				else if (ship.size >= ShipSize.Medium)
				{
					list.Add(this.radiator12);
					list.Add(this.radiator3);
					list.Add(this.radiator6);
					list.Add(this.radiator9);
				}
				else
				{
					list.Add(this.radiator130);
					list.Add(this.radiator430);
					list.Add(this.radiator730);
					list.Add(this.radiator1030);
				}
				break;
			case RadiatorType.Droplet:
				list.Add(this.dropletRadiator12);
				list.Add(this.dropletRadiator4);
				list.Add(this.dropletRadiator8);
				break;
			case RadiatorType.Spike:
				list.Add(this.spikesRadiator3);
				list.Add(this.spikesRadiator9);
				if (num > 400f)
				{
					list.Add(this.spikesRadiator6);
					list.Add(this.spikesRadiator12);
				}
				break;
			}
			return list;
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x000CBD04 File Offset: 0x000C9F04
		public override void SetRadiators(TISpaceShipTemplate ship)
		{
			if (ship.radiatorTemplate != null && !ship.hullTemplate.simpleHull)
			{
				List<GameObject> list = this.WhichRadiators(ship);
				this.radiator12.SetActive(list.Contains(this.radiator12));
				this.radiator130.SetActive(list.Contains(this.radiator130));
				this.radiator3.SetActive(list.Contains(this.radiator3));
				this.radiator4.SetActive(list.Contains(this.radiator4));
				this.radiator430.SetActive(list.Contains(this.radiator430));
				this.radiator6.SetActive(list.Contains(this.radiator6));
				this.radiator730.SetActive(list.Contains(this.radiator730));
				this.radiator8.SetActive(list.Contains(this.radiator8));
				this.radiator9.SetActive(list.Contains(this.radiator9));
				this.radiator1030.SetActive(list.Contains(this.radiator1030));
				this.dropletRadiator12.SetActive(list.Contains(this.dropletRadiator12));
				this.dropletRadiator4.SetActive(list.Contains(this.dropletRadiator4));
				this.dropletRadiator8.SetActive(list.Contains(this.dropletRadiator8));
				this.spikesRadiator3.SetActive(list.Contains(this.spikesRadiator3));
				this.spikesRadiator6.SetActive(list.Contains(this.spikesRadiator6));
				this.spikesRadiator9.SetActive(list.Contains(this.spikesRadiator9));
				this.spikesRadiator12.SetActive(list.Contains(this.spikesRadiator12));
				this.radiatorAnimators = new List<Animator>();
				if (this.radiator12.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator12.GetComponent<Animator>());
				}
				if (this.radiator130.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator130.GetComponent<Animator>());
				}
				if (this.radiator3.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator3.GetComponent<Animator>());
				}
				if (this.radiator4.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator4.GetComponent<Animator>());
				}
				if (this.radiator430.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator430.GetComponent<Animator>());
				}
				if (this.radiator6.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator6.GetComponent<Animator>());
				}
				if (this.radiator730.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator730.GetComponent<Animator>());
				}
				if (this.radiator8.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator8.GetComponent<Animator>());
				}
				if (this.radiator9.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator9.GetComponent<Animator>());
				}
				if (this.radiator1030.activeSelf)
				{
					this.radiatorAnimators.Add(this.radiator1030.GetComponent<Animator>());
				}
				if (this.dropletRadiator12.activeSelf)
				{
					this.radiatorAnimators.Add(this.dropletRadiator12.GetComponent<Animator>());
				}
				if (this.dropletRadiator4.activeSelf)
				{
					this.radiatorAnimators.Add(this.dropletRadiator4.GetComponent<Animator>());
				}
				if (this.dropletRadiator8.activeSelf)
				{
					this.radiatorAnimators.Add(this.dropletRadiator8.GetComponent<Animator>());
				}
				if (this.spikesRadiator3.activeSelf)
				{
					this.radiatorAnimators.Add(this.spikesRadiator3.GetComponent<Animator>());
				}
				if (this.spikesRadiator6.activeSelf)
				{
					this.radiatorAnimators.Add(this.spikesRadiator6.GetComponent<Animator>());
				}
				if (this.spikesRadiator9.activeSelf)
				{
					this.radiatorAnimators.Add(this.spikesRadiator9.GetComponent<Animator>());
				}
				if (this.spikesRadiator12.activeSelf)
				{
					this.radiatorAnimators.Add(this.spikesRadiator12.GetComponent<Animator>());
				}
				this.radiatorAnimators.Where<Animator>((Animator x) => x.gameObject.activeInHierarchy).ToList<Animator>().ForEach(delegate(Animator x)
				{
					x.Play("Extend", 0, 1f);
				});
				this.radiatorEmissivesFx = new List<ColorAnimationEffect>();
				if (this.radiator12.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator12.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator130.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator130.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator3.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator3.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator4.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator4.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator430.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator430.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator6.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator6.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator730.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator730.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator8.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator8.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator9.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator9.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator1030.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.radiator1030.GetComponent<ColorAnimationEffect>());
				}
				if (this.dropletRadiator12.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.dropletRadiator12.GetComponent<ColorAnimationEffect>());
				}
				if (this.dropletRadiator4.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.dropletRadiator4.GetComponent<ColorAnimationEffect>());
				}
				if (this.dropletRadiator8.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.dropletRadiator8.GetComponent<ColorAnimationEffect>());
				}
				if (this.spikesRadiator3.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.spikesRadiator3.GetComponent<ColorAnimationEffect>());
				}
				if (this.spikesRadiator6.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.spikesRadiator6.GetComponent<ColorAnimationEffect>());
				}
				if (this.spikesRadiator9.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.spikesRadiator9.GetComponent<ColorAnimationEffect>());
				}
				if (this.spikesRadiator12.activeSelf)
				{
					this.radiatorEmissivesFx.Add(this.spikesRadiator12.GetComponent<ColorAnimationEffect>());
					return;
				}
			}
			else
			{
				this.radiator12.SetActive(false);
				this.radiator130.SetActive(false);
				this.radiator3.SetActive(false);
				this.radiator4.SetActive(false);
				this.radiator430.SetActive(false);
				this.radiator6.SetActive(false);
				this.radiator730.SetActive(false);
				this.radiator8.SetActive(false);
				this.radiator9.SetActive(false);
				this.radiator1030.SetActive(false);
				this.dropletRadiator12.SetActive(false);
				this.dropletRadiator4.SetActive(false);
				this.dropletRadiator8.SetActive(false);
				this.spikesRadiator3.SetActive(false);
				this.spikesRadiator6.SetActive(false);
				this.spikesRadiator9.SetActive(false);
				this.spikesRadiator12.SetActive(false);
			}
		}

		// Token: 0x04001C18 RID: 7192
		[Header("Human Ship Controller")]
		public GameObject hullModel;

		// Token: 0x04001C19 RID: 7193
		public GameObject spikesRadiator12;

		// Token: 0x04001C1A RID: 7194
		public GameObject spikesRadiator3;

		// Token: 0x04001C1B RID: 7195
		public GameObject spikesRadiator6;

		// Token: 0x04001C1C RID: 7196
		public GameObject spikesRadiator9;

		// Token: 0x04001C1D RID: 7197
		public GameObject dropletRadiator12;

		// Token: 0x04001C1E RID: 7198
		public GameObject dropletRadiator4;

		// Token: 0x04001C1F RID: 7199
		public GameObject dropletRadiator8;
	}
}
