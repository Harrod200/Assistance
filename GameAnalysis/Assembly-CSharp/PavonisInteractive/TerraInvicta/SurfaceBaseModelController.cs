using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005A7 RID: 1447
	public class SurfaceBaseModelController : MonoBehaviour
	{
		// Token: 0x0600275C RID: 10076 RVA: 0x000D767C File Offset: 0x000D587C
		public void UpdateSurfaceModel(TIHabState hab)
		{
			foreach (GameObject gameObject in this.humanCores)
			{
				gameObject.SetActive(false);
			}
			foreach (GameObject gameObject2 in this.alienCores)
			{
				gameObject2.SetActive(false);
			}
			foreach (GameObject gameObject3 in this.humanSectors)
			{
				gameObject3.SetActive(false);
			}
			foreach (GameObject gameObject4 in this.alienSectors)
			{
				gameObject4.SetActive(false);
			}
			this.humanMine.SetActive(false);
			this.alienMine.SetActive(false);
			this.humanBottomSharedConnector.SetActive(false);
			this.alienBottomSharedConnector.SetActive(false);
			int num = hab.tier * 4 - 4;
			if (hab.IsAlien())
			{
				this.alienCores[hab.tier - 1].SetActive(true);
				this.alienMine.SetActive(hab.HasMine);
				for (int i = 1; i < hab.sectors.Count; i++)
				{
					if (hab.sectors[i].active)
					{
						this.alienSectors[i - 1].SetActive(true);
						if (i == 3 || i == 4)
						{
							this.alienBottomSharedConnector.SetActive(true);
						}
					}
				}
				for (int j = 0; j < this.alienConditionalCoreConnectors.Count; j++)
				{
					if (j < num)
					{
						this.alienConditionalCoreConnectors[j].SetActive(false);
					}
					else
					{
						this.alienConditionalCoreConnectors[j].SetActive(true);
					}
				}
				return;
			}
			this.humanCores[hab.tier - 1].SetActive(true);
			this.humanMine.SetActive(hab.HasMine);
			for (int k = 1; k < hab.sectors.Count; k++)
			{
				if (hab.sectors[k].active)
				{
					this.humanSectors[k - 1].SetActive(true);
					if (k == 3 || k == 4)
					{
						this.humanBottomSharedConnector.SetActive(true);
					}
				}
			}
			for (int l = 0; l < this.humanConditionalCoreConnectors.Count; l++)
			{
				if (l < num)
				{
					this.humanConditionalCoreConnectors[l].SetActive(false);
				}
				else
				{
					this.humanConditionalCoreConnectors[l].SetActive(true);
				}
			}
		}

		// Token: 0x04001D3F RID: 7487
		public List<GameObject> humanCores = new List<GameObject>();

		// Token: 0x04001D40 RID: 7488
		public List<GameObject> alienCores = new List<GameObject>();

		// Token: 0x04001D41 RID: 7489
		public List<GameObject> humanSectors = new List<GameObject>();

		// Token: 0x04001D42 RID: 7490
		public List<GameObject> alienSectors = new List<GameObject>();

		// Token: 0x04001D43 RID: 7491
		public GameObject humanMine;

		// Token: 0x04001D44 RID: 7492
		public GameObject alienMine;

		// Token: 0x04001D45 RID: 7493
		public GameObject humanBottomSharedConnector;

		// Token: 0x04001D46 RID: 7494
		public GameObject alienBottomSharedConnector;

		// Token: 0x04001D47 RID: 7495
		public List<GameObject> humanConditionalCoreConnectors = new List<GameObject>();

		// Token: 0x04001D48 RID: 7496
		public List<GameObject> alienConditionalCoreConnectors = new List<GameObject>();
	}
}
