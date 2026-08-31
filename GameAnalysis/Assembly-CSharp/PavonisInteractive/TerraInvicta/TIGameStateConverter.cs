using System;
using System.Collections.Generic;
using FullSerializer;
using FullSerializer.Internal;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D7 RID: 2007
	public class TIGameStateConverter : fsReflectedConverter
	{
		// Token: 0x0600487E RID: 18558 RVA: 0x001DD266 File Offset: 0x001DB466
		public static void Reset()
		{
			TIGameStateConverter.gamestates.Clear();
			TIGameStateConverter.gameStateDepth = 0;
		}

		// Token: 0x0600487F RID: 18559 RVA: 0x001DD278 File Offset: 0x001DB478
		public override object CreateInstance(fsData data, Type storageType)
		{
			return Activator.CreateInstance(storageType);
		}

		// Token: 0x06004880 RID: 18560 RVA: 0x001DD280 File Offset: 0x001DB480
		public override bool RequestCycleSupport(Type storageType)
		{
			return false;
		}

		// Token: 0x06004881 RID: 18561 RVA: 0x001DD283 File Offset: 0x001DB483
		public override bool CanProcess(Type type)
		{
			return typeof(TIGameState).IsAssignableFrom(type);
		}

		// Token: 0x06004882 RID: 18562 RVA: 0x001DD298 File Offset: 0x001DB498
		public fsResult DeserializeGameStateFromID(fsData data, ref object instance)
		{
			fsResult fsResult = fsResult.Success;
			object obj = null;
			fsResult fsResult2 = this.Serializer.TryDeserialize(data, typeof(GameStateID), ref obj);
			fsResult fsResult3;
			fsResult = (fsResult3 = fsResult + fsResult2);
			if (fsResult3.Failed)
			{
				return fsResult;
			}
			GameStateID gameStateID = (GameStateID)obj;
			TIGameState tigameState;
			if (!TIGameStateConverter.gamestates.TryGetValue(gameStateID, out tigameState))
			{
				TIGameState tigameState2 = (TIGameState)instance;
				tigameState2.ID = gameStateID;
				TIGameStateConverter.gamestates[gameStateID] = tigameState2;
			}
			else
			{
				instance = tigameState;
			}
			return fsResult;
		}

		// Token: 0x06004883 RID: 18563 RVA: 0x001DD318 File Offset: 0x001DB518
		public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
		{
			Dictionary<string, fsData> asDictionary = data.AsDictionary;
			fsData fsData;
			if (!asDictionary.TryGetValue("ID", out fsData) || !fsData.IsDictionary)
			{
				if (!asDictionary.ContainsKey("value"))
				{
					return fsResult.Fail("Invalid GameState reference. No GameStateID found");
				}
				fsData = data;
			}
			fsResult fsResult = this.DeserializeGameStateFromID(fsData, ref instance);
			if (data == fsData)
			{
				return fsResult;
			}
			return fsResult + base.TryDeserialize(data, ref instance, storageType);
		}

		// Token: 0x06004884 RID: 18564 RVA: 0x001DD384 File Offset: 0x001DB584
		public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
		{
			fsResult fsResult;
			try
			{
				TIGameStateConverter.gameStateDepth++;
				if (TIGameStateConverter.gameStateDepth > 1)
				{
					fsResult = this.Serializer.TrySerialize(typeof(GameStateID), ((TIGameState)instance).ID, out serialized);
				}
				else
				{
					fsResult = base.TrySerialize(instance, out serialized, storageType);
				}
			}
			finally
			{
				TIGameStateConverter.gameStateDepth--;
			}
			return fsResult;
		}

		// Token: 0x040029B5 RID: 10677
		private static Dictionary<GameStateID, TIGameState> gamestates = new Dictionary<GameStateID, TIGameState>();

		// Token: 0x040029B6 RID: 10678
		private static int gameStateDepth = 0;
	}
}
