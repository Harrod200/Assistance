using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Test
{
	// Token: 0x0200092E RID: 2350
	public class GSMSerializeTest : MonoBehaviour
	{
		// Token: 0x060059BE RID: 22974 RVA: 0x002928D4 File Offset: 0x00290AD4
		public void ATestSelfReference()
		{
			GSMSerializeTest.SelfState selfState = GameStateManager.CreateNewGameState<GSMSerializeTest.SelfState>();
			selfState.self = selfState;
			selfState.selfID = selfState.ID;
			this.SaveAndLoad();
			GSMSerializeTest.SelfState selfState2 = GameStateManager.FindGameState<GSMSerializeTest.SelfState>(selfState.ID, false);
			this.AssertNotNull(selfState2);
			this.AssertNotEqual(selfState, selfState2);
			this.AssertEqual(selfState.ID, selfState2.ID);
			this.AssertEqual(selfState2.selfID, selfState2.ID);
			this.AssertEqual(selfState2.self, selfState2);
		}

		// Token: 0x060059BF RID: 22975 RVA: 0x00292950 File Offset: 0x00290B50
		public void ATestOtherReference()
		{
			GSMSerializeTest.RefState refState = GameStateManager.CreateNewGameState<GSMSerializeTest.RefState>();
			GSMSerializeTest.SimpleState simpleState = GameStateManager.CreateNewGameState<GSMSerializeTest.SimpleState>();
			refState.state = simpleState;
			this.SaveAndLoad();
			GSMSerializeTest.RefState refState2 = GameStateManager.FindGameState<GSMSerializeTest.RefState>(refState.ID, false);
			GSMSerializeTest.SimpleState simpleState2 = GameStateManager.FindGameState<GSMSerializeTest.SimpleState>(simpleState.ID, false);
			this.AssertNotNull(refState2);
			this.AssertNotNull(simpleState2);
			this.AssertNotEqual(refState, refState2);
			this.AssertNotEqual(simpleState, simpleState2);
			this.AssertEqual(refState.ID, refState2.ID);
			this.AssertEqual(simpleState.ID, simpleState2.ID);
			this.AssertEqual(refState2.state, simpleState2);
		}

		// Token: 0x060059C0 RID: 22976 RVA: 0x002929E0 File Offset: 0x00290BE0
		public void ATestListReferences()
		{
			GSMSerializeTest.ListState listState = GameStateManager.CreateNewGameState<GSMSerializeTest.ListState>();
			GSMSerializeTest.SimpleState simpleState = GameStateManager.CreateNewGameState<GSMSerializeTest.SimpleState>();
			listState.states = new List<GSMSerializeTest.SimpleState>();
			listState.states.Add(simpleState);
			this.SaveAndLoad();
			GSMSerializeTest.ListState listState2 = GameStateManager.FindGameState<GSMSerializeTest.ListState>(listState.ID, false);
			GSMSerializeTest.SimpleState simpleState2 = GameStateManager.FindGameState<GSMSerializeTest.SimpleState>(simpleState.ID, false);
			this.AssertNotNull(listState2);
			this.AssertNotNull(simpleState2);
			this.AssertNotEqual(listState, listState2);
			this.AssertNotEqual(simpleState, simpleState2);
			this.AssertEqual(listState.ID, listState2.ID);
			this.AssertEqual(simpleState.ID, simpleState2.ID);
			this.AssertEqual(simpleState.ID, listState2.states[0].ID);
			this.AssertEqual(listState2.states[0], simpleState2);
		}

		// Token: 0x060059C1 RID: 22977 RVA: 0x00292AA4 File Offset: 0x00290CA4
		public void TestDictReferences()
		{
			GSMSerializeTest.DictionaryState dictionaryState = GameStateManager.CreateNewGameState<GSMSerializeTest.DictionaryState>();
			GSMSerializeTest.SimpleState simpleState = GameStateManager.CreateNewGameState<GSMSerializeTest.SimpleState>();
			GSMSerializeTest.SimpleState simpleState2 = GameStateManager.CreateNewGameState<GSMSerializeTest.SimpleState>();
			dictionaryState.stateMap = new Dictionary<GSMSerializeTest.SimpleState, GSMSerializeTest.SimpleState>();
			dictionaryState.stateMap[simpleState] = simpleState2;
			this.SaveAndLoad();
			GSMSerializeTest.DictionaryState dictionaryState2 = GameStateManager.FindGameState<GSMSerializeTest.DictionaryState>(dictionaryState.ID, false);
			GSMSerializeTest.SimpleState simpleState3 = GameStateManager.FindGameState<GSMSerializeTest.SimpleState>(simpleState.ID, false);
			GSMSerializeTest.SimpleState simpleState4 = GameStateManager.FindGameState<GSMSerializeTest.SimpleState>(simpleState2.ID, false);
			this.AssertNotNull(dictionaryState2);
			this.AssertNotNull(simpleState3);
			this.AssertNotNull(simpleState4);
			this.AssertNotEqual(dictionaryState, dictionaryState2);
			this.AssertNotEqual(simpleState, simpleState3);
			this.AssertNotEqual(simpleState2, simpleState4);
			this.AssertEqual(dictionaryState.ID, dictionaryState2.ID);
			this.AssertEqual(simpleState.ID, simpleState3.ID);
			this.AssertEqual(simpleState2.ID, simpleState4.ID);
			foreach (KeyValuePair<GSMSerializeTest.SimpleState, GSMSerializeTest.SimpleState> keyValuePair in dictionaryState2.stateMap)
			{
				this.AssertEqual(simpleState.ID, keyValuePair.Key.ID);
				this.AssertEqual(simpleState2.ID, keyValuePair.Value.ID);
				this.AssertEqual(simpleState3, keyValuePair.Key);
				this.AssertEqual(simpleState4, keyValuePair.Value);
			}
		}

		// Token: 0x060059C2 RID: 22978 RVA: 0x00292C04 File Offset: 0x00290E04
		private void SaveAndLoad()
		{
			string saveFilePath = TIUtilities.GetSaveFilePath(this.savePath);
			GameStateManager.SaveAllGameStates(saveFilePath, false);
			GameStateManager.ClearAllGameStates();
			GameStateManager.LoadAllGameStates(saveFilePath);
		}

		// Token: 0x060059C3 RID: 22979 RVA: 0x00292C24 File Offset: 0x00290E24
		private void Report()
		{
			Debug.Log(this.testCount.ToString() + " tests: " + this.failCount.ToString() + " failed");
		}

		// Token: 0x060059C4 RID: 22980 RVA: 0x00292C50 File Offset: 0x00290E50
		private void AssertNotNull(object o)
		{
			this.Assert(() => o != null, "Object is null");
		}

		// Token: 0x060059C5 RID: 22981 RVA: 0x00292C84 File Offset: 0x00290E84
		private void AssertEqual(GameStateID a, GameStateID b)
		{
			Func<bool> func = () => a == b;
			string text = "Objects are not equal: ";
			GameStateID gameStateID = a;
			string text2 = gameStateID.ToString();
			string text3 = " != ";
			gameStateID = b;
			this.Assert(func, text + text2 + text3 + gameStateID.ToString());
		}

		// Token: 0x060059C6 RID: 22982 RVA: 0x00292CF0 File Offset: 0x00290EF0
		private void AssertEqual(object a, object b)
		{
			Func<bool> func = () => a == b;
			string text = "Objects are not equal: ";
			object a2 = a;
			string text2 = ((a2 != null) ? a2.ToString() : null);
			string text3 = " != ";
			object b2 = b;
			this.Assert(func, text + text2 + text3 + ((b2 != null) ? b2.ToString() : null));
		}

		// Token: 0x060059C7 RID: 22983 RVA: 0x00292D58 File Offset: 0x00290F58
		private void AssertNotEqual(object a, object b)
		{
			Func<bool> func = () => a != b;
			string text = "Objects are equal: ";
			object a2 = a;
			string text2 = ((a2 != null) ? a2.ToString() : null);
			string text3 = " == ";
			object b2 = b;
			this.Assert(func, text + text2 + text3 + ((b2 != null) ? b2.ToString() : null));
		}

		// Token: 0x060059C8 RID: 22984 RVA: 0x00292DC0 File Offset: 0x00290FC0
		private void Assert(Func<bool> f, string failMessage = "Assertion Failed")
		{
			this.testCount++;
			bool flag;
			try
			{
				flag = f();
			}
			catch (Exception)
			{
				flag = false;
			}
			if (!flag)
			{
				Debug.LogError(failMessage);
				this.failCount++;
				return;
			}
		}

		// Token: 0x060059C9 RID: 22985 RVA: 0x00292E14 File Offset: 0x00291014
		private void Awake()
		{
			this.tests = new List<MethodInfo>();
			foreach (MethodInfo methodInfo in base.GetType().GetMethods())
			{
				if (methodInfo.Name.StartsWith("Test"))
				{
					this.tests.Add(methodInfo);
				}
			}
			Debug.Log("Loaded " + this.tests.Count.ToString() + " tests");
			Debug.Log("Press F1 to run tests");
		}

		// Token: 0x060059CA RID: 22986 RVA: 0x00292E9C File Offset: 0x0029109C
		private void Update()
		{
			if (Input.GetKeyUp(KeyCode.F1))
			{
				foreach (MethodInfo methodInfo in this.tests)
				{
					try
					{
						GameStateManager.ClearAllGameStates();
						methodInfo.Invoke(this, null);
					}
					catch (Exception ex)
					{
						this.testFailures++;
						throw ex;
					}
				}
				this.Report();
			}
		}

		// Token: 0x040040D6 RID: 16598
		public string savePath = "F:\\test";

		// Token: 0x040040D7 RID: 16599
		private List<MethodInfo> tests;

		// Token: 0x040040D8 RID: 16600
		private int testCount;

		// Token: 0x040040D9 RID: 16601
		private int failCount;

		// Token: 0x040040DA RID: 16602
		private int testFailures;

		// Token: 0x02001209 RID: 4617
		public class SimpleState : TIGameState
		{
			// Token: 0x040068EA RID: 26858
			public int a = 1;
		}

		// Token: 0x0200120A RID: 4618
		public class SelfState : TIGameState
		{
			// Token: 0x040068EB RID: 26859
			public GameStateID selfID;

			// Token: 0x040068EC RID: 26860
			public GSMSerializeTest.SelfState self;
		}

		// Token: 0x0200120B RID: 4619
		public class RefState : TIGameState
		{
			// Token: 0x040068ED RID: 26861
			public GSMSerializeTest.SimpleState state;
		}

		// Token: 0x0200120C RID: 4620
		public class ListState : TIGameState
		{
			// Token: 0x040068EE RID: 26862
			public List<GSMSerializeTest.SimpleState> states;
		}

		// Token: 0x0200120D RID: 4621
		public class DictionaryState : TIGameState
		{
			// Token: 0x040068EF RID: 26863
			public Dictionary<GSMSerializeTest.SimpleState, GSMSerializeTest.SimpleState> stateMap;
		}
	}
}
