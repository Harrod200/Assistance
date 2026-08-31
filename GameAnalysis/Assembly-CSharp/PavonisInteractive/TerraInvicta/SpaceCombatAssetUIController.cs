using System;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008CF RID: 2255
	public abstract class SpaceCombatAssetUIController : MonoBehaviour
	{
		// Token: 0x17000F17 RID: 3863
		// (get) Token: 0x0600566F RID: 22127 RVA: 0x00278509 File Offset: 0x00276709
		// (set) Token: 0x06005670 RID: 22128 RVA: 0x00278511 File Offset: 0x00276711
		[HideInInspector]
		public CombatantListItemController combatantListItemController { get; protected set; }

		// Token: 0x06005671 RID: 22129
		public abstract void InitializeForCombat(CombatantController combatantController, CombatantListItemController listItemController);

		// Token: 0x04003D7D RID: 15741
		public bool maintainAnimation;
	}
}
