using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CB RID: 2251
	public class CombatShipReinforceListItemHighlight : MonoBehaviour
	{
		// Token: 0x06005657 RID: 22103 RVA: 0x002780D4 File Offset: 0x002762D4
		private void Awake()
		{
			this.image = base.GetComponent<Image>();
		}

		// Token: 0x06005658 RID: 22104 RVA: 0x002780E2 File Offset: 0x002762E2
		private void OnEnable()
		{
			this.image.color = this.highlightColor;
			this.transitionTimer = 0f;
		}

		// Token: 0x06005659 RID: 22105 RVA: 0x00278100 File Offset: 0x00276300
		private void Update()
		{
			this.transitionTimer += Time.deltaTime;
			this.image.color = Color.Lerp(this.highlightColor, this.transparent, this.transitionTimer / this.transitionTime);
			if (this.transitionTimer >= this.transitionTime)
			{
				this.Turnoff();
			}
		}

		// Token: 0x0600565A RID: 22106 RVA: 0x0027815C File Offset: 0x0027635C
		private void Turnoff()
		{
			base.gameObject.SetActive(false);
		}

		// Token: 0x04003D65 RID: 15717
		private Image image;

		// Token: 0x04003D66 RID: 15718
		private Color highlightColor = new Color(1f, 0.66f, 0f, 1f);

		// Token: 0x04003D67 RID: 15719
		private Color transparent = new Color(0f, 0f, 0f, 0f);

		// Token: 0x04003D68 RID: 15720
		private float transitionTime = 6f;

		// Token: 0x04003D69 RID: 15721
		private float transitionTimer;
	}
}
