using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000589 RID: 1417
	public abstract class AlienShipController : ShipModelController
	{
		// Token: 0x060025B1 RID: 9649 RVA: 0x000CC4A2 File Offset: 0x000CA6A2
		public override void SetSkin(TISpaceShipTemplate ship)
		{
		}

		// Token: 0x060025B2 RID: 9650 RVA: 0x000CC4A4 File Offset: 0x000CA6A4
		public override void SetRadiators(TISpaceShipTemplate ship)
		{
			if (ship.radiatorTemplate != null)
			{
				this.radiatorEmissivesFx = new List<ColorAnimationEffect>();
				this.radiatorAnimators = new List<Animator>();
				List<GameObject> list = this.WhichRadiators(ship);
				if (this.radiator1030 != null)
				{
					this.radiator1030.SetActive(list.Contains(this.radiator1030));
					this.radiatorAnimators.Add(this.radiator1030.GetComponent<Animator>());
					this.radiatorEmissivesFx.Add(this.radiator1030.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator130 != null)
				{
					this.radiator130.SetActive(list.Contains(this.radiator1030));
					this.radiatorAnimators.Add(this.radiator130.GetComponent<Animator>());
					this.radiatorEmissivesFx.Add(this.radiator130.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator430 != null)
				{
					this.radiator430.SetActive(list.Contains(this.radiator430));
					this.radiatorAnimators.Add(this.radiator430.GetComponent<Animator>());
					this.radiatorEmissivesFx.Add(this.radiator430.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator6 != null)
				{
					this.radiator6.SetActive(list.Contains(this.radiator6));
					this.radiatorAnimators.Add(this.radiator6.GetComponent<Animator>());
					this.radiatorEmissivesFx.Add(this.radiator6.GetComponent<ColorAnimationEffect>());
				}
				if (this.radiator730 != null)
				{
					this.radiator730.SetActive(list.Contains(this.radiator730));
					this.radiatorAnimators.Add(this.radiator730.GetComponent<Animator>());
					this.radiatorEmissivesFx.Add(this.radiator730.GetComponent<ColorAnimationEffect>());
				}
				this.radiatorAnimators.RemoveAll((Animator x) => x == null);
				this.radiatorEmissivesFx.RemoveAll((ColorAnimationEffect x) => x == null);
				return;
			}
			if (this.radiator1030 != null)
			{
				this.radiator1030.SetActive(false);
			}
			if (this.radiator130 != null)
			{
				this.radiator130.SetActive(false);
			}
			if (this.radiator430 != null)
			{
				this.radiator430.SetActive(false);
			}
			if (this.radiator6 != null)
			{
				this.radiator6.SetActive(false);
			}
			if (this.radiator730 != null)
			{
				this.radiator730.SetActive(false);
			}
		}
	}
}
