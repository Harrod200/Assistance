using System;
using PavonisInteractive.TerraInvicta.Entities;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006EC RID: 1772
	public class PlayerManager : MonoBehaviour
	{
		// Token: 0x170005AF RID: 1455
		// (get) Token: 0x06002939 RID: 10553 RVA: 0x000DBCB3 File Offset: 0x000D9EB3
		private GameObjectDictionary<GameStateID> container
		{
			get
			{
				if (this._container == null)
				{
					this._container = new GameObjectDictionary<GameStateID>("Player Container");
				}
				return this._container;
			}
		}

		// Token: 0x0600293A RID: 10554 RVA: 0x000DBCD3 File Offset: 0x000D9ED3
		[Inject]
		public void Construct(Player.Factory playerFactory)
		{
			this.factory = playerFactory;
		}

		// Token: 0x0600293B RID: 10555 RVA: 0x000DBCDC File Offset: 0x000D9EDC
		public GameObject FindPlayer(GameStateID playerID)
		{
			return this.container[playerID];
		}

		// Token: 0x0600293C RID: 10556 RVA: 0x000DBCEC File Offset: 0x000D9EEC
		public Player FindPlayerComponent(TIFactionState faction)
		{
			GameObject gameObject = this.FindPlayer(faction.player.ID);
			if (gameObject != null)
			{
				return gameObject.GetComponent<Player>();
			}
			return null;
		}

		// Token: 0x0600293D RID: 10557 RVA: 0x000DBD1C File Offset: 0x000D9F1C
		public bool TryFindPlayer(GameStateID ID, out GameObject gameObject)
		{
			return this.container.TryFind(ID, out gameObject);
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x000DBD2C File Offset: 0x000D9F2C
		public void RemovePlayer(GameStateID playerID)
		{
			if (!this.container.Remove(playerID, true))
			{
				Debug.LogWarning("Attempting to remove player for PlayerID: " + ((int)playerID).ToString() + " does not exist.");
				return;
			}
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x000DBD6B File Offset: 0x000D9F6B
		public void Initialize()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> Load Players", delegate
			{
				this.container.Clear(true);
				foreach (TIPlayerState tiplayerState in GameStateManager.IterateByClass<TIPlayerState>(false))
				{
					TIFactionState faction = tiplayerState.faction;
					Player player = this.factory.Create(tiplayerState);
					player.gameObject.name = tiplayerState.name;
					if (!this.container.Add(tiplayerState.ID, player.gameObject, false, false))
					{
						Debug.LogWarning("Attempting to add player for PlayerID: " + ((int)tiplayerState.ID).ToString() + " that has already been created.");
					}
				}
			}, true, true);
		}

		// Token: 0x04001F92 RID: 8082
		private Player.Factory factory;

		// Token: 0x04001F93 RID: 8083
		private GameObjectDictionary<GameStateID> _container;
	}
}
