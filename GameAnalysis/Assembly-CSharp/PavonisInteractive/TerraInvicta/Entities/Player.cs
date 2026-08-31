using System;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Entities
{
	// Token: 0x02000966 RID: 2406
	public class Player : MonoBehaviour, IPlayerActionRunner
	{
		// Token: 0x17000F8B RID: 3979
		// (get) Token: 0x06005BAC RID: 23468 RVA: 0x002BF437 File Offset: 0x002BD637
		// (set) Token: 0x06005BAD RID: 23469 RVA: 0x002BF43F File Offset: 0x002BD63F
		public TIPlayerState state { get; private set; }

		// Token: 0x06005BAE RID: 23470 RVA: 0x002BF448 File Offset: 0x002BD648
		[Inject]
		public void Construct(TIPlayerState playerState)
		{
			this.state = playerState;
		}

		// Token: 0x06005BAF RID: 23471 RVA: 0x002BF451 File Offset: 0x002BD651
		public void StartAction(PlayerAction action)
		{
			action.Execute();
		}

		// Token: 0x02001331 RID: 4913
		public class Factory : Factory<TIPlayerState, Player>
		{
		}
	}
}
