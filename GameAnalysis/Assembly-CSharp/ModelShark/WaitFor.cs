using System;
using System.Collections;

namespace ModelShark
{
	// Token: 0x020004B6 RID: 1206
	public static class WaitFor
	{
		// Token: 0x06001B11 RID: 6929 RVA: 0x00093188 File Offset: 0x00091388
		public static IEnumerator Frames(int frameCount)
		{
			while (frameCount > 0)
			{
				int num = frameCount;
				frameCount = num - 1;
				yield return null;
			}
			yield break;
		}
	}
}
