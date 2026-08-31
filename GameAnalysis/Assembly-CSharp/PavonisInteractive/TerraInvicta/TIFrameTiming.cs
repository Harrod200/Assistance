using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F1 RID: 1777
	public class TIFrameTiming : MonoBehaviour
	{
		// Token: 0x060029CF RID: 10703 RVA: 0x000E2A4C File Offset: 0x000E0C4C
		private void Start()
		{
			for (int i = 0; i < this.frametimeHistoryLength; i++)
			{
				this.frametimeHistory.Enqueue(0f);
			}
		}

		// Token: 0x060029D0 RID: 10704 RVA: 0x000E2A7C File Offset: 0x000E0C7C
		private void Update()
		{
			TIMutableFrameCounter.FrameCount = Time.frameCount;
			this.previousStartOfUpdateSeconds = this.startOfUpdateSeconds;
			this.startOfUpdateSeconds = this.stopwatch.GetElapsedSeconds();
			this.frametimeHistory.Enqueue(TIFrameTiming.lastFrametime);
			this.frametimeHistory.Dequeue();
		}

		// Token: 0x170005C3 RID: 1475
		// (get) Token: 0x060029D1 RID: 10705 RVA: 0x000E2ACC File Offset: 0x000E0CCC
		private static TIFrameTiming singleton
		{
			get
			{
				if (TIFrameTiming.singleton_ == null)
				{
					TIFrameTiming.singleton_ = new GameObject("FirstSingleton").AddComponent<TIFrameTiming>();
					TIFrameTiming.singleton.stopwatch.Start();
				}
				return TIFrameTiming.singleton_;
			}
		}

		// Token: 0x170005C4 RID: 1476
		// (get) Token: 0x060029D2 RID: 10706 RVA: 0x000E2B03 File Offset: 0x000E0D03
		public static float lastFrametime
		{
			get
			{
				return (float)(TIFrameTiming.singleton.startOfUpdateSeconds - TIFrameTiming.singleton.previousStartOfUpdateSeconds);
			}
		}

		// Token: 0x170005C5 RID: 1477
		// (get) Token: 0x060029D3 RID: 10707 RVA: 0x000E2B1B File Offset: 0x000E0D1B
		public static double secondsSinceStartOfUpdate
		{
			get
			{
				return TIFrameTiming.singleton.stopwatch.GetElapsedSeconds() - TIFrameTiming.singleton.startOfUpdateSeconds;
			}
		}

		// Token: 0x060029D4 RID: 10708 RVA: 0x000E2B38 File Offset: 0x000E0D38
		public static float GetAverageFrametime(int targetFrameCount)
		{
			float num = 0f;
			int num2 = 0;
			foreach (float num3 in TIFrameTiming.singleton.frametimeHistory)
			{
				num += num3;
				if (++num2 >= targetFrameCount)
				{
					break;
				}
			}
			return num / (float)num2;
		}

		// Token: 0x04002037 RID: 8247
		private Stopwatch stopwatch = new Stopwatch();

		// Token: 0x04002038 RID: 8248
		private double previousStartOfUpdateSeconds;

		// Token: 0x04002039 RID: 8249
		private double startOfUpdateSeconds;

		// Token: 0x0400203A RID: 8250
		private Queue<float> frametimeHistory = new Queue<float>();

		// Token: 0x0400203B RID: 8251
		private int frametimeHistoryLength = 240;

		// Token: 0x0400203C RID: 8252
		private static TIFrameTiming singleton_;
	}
}
