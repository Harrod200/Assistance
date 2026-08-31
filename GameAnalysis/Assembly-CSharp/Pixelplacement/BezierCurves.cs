using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x02000516 RID: 1302
	public static class BezierCurves
	{
		// Token: 0x0600201D RID: 8221 RVA: 0x000A6D1C File Offset: 0x000A4F1C
		public static Vector3 GetPoint(Vector3 startPosition, Vector3 controlPoint, Vector3 endPosition, float percentage)
		{
			percentage = Mathf.Clamp01(percentage);
			float num = 1f - percentage;
			return num * num * startPosition + 2f * num * percentage * controlPoint + percentage * percentage * endPosition;
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x000A6D64 File Offset: 0x000A4F64
		public static Vector3 GetFirstDerivative(Vector3 startPoint, Vector3 controlPoint, Vector3 endPosition, float percentage)
		{
			percentage = Mathf.Clamp01(percentage);
			return 2f * (1f - percentage) * (controlPoint - startPoint) + 2f * percentage * (endPosition - controlPoint);
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x000A6DA0 File Offset: 0x000A4FA0
		public static Vector3 GetPoint(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage, bool evenDistribution, int distributionSteps)
		{
			if (!evenDistribution)
			{
				return BezierCurves.Locate(startPosition, endPosition, startTangent, endTangent, percentage);
			}
			int num = distributionSteps + 1;
			float[] array = new float[num];
			Vector3 vector = BezierCurves.Locate(startPosition, endPosition, startTangent, endTangent, 0f);
			float num2 = 0f;
			for (int i = 1; i < num; i++)
			{
				Vector3 vector2 = BezierCurves.Locate(startPosition, endPosition, startTangent, endTangent, (float)i / (float)num);
				num2 += Vector3.Distance(vector, vector2);
				array[i] = num2;
				vector = vector2;
			}
			float num3 = percentage * array[distributionSteps];
			int j = 0;
			int num4 = distributionSteps;
			int num5 = 0;
			while (j < num4)
			{
				num5 = j + (((num4 - j) / 2) | 0);
				if (array[num5] < num3)
				{
					j = num5 + 1;
				}
				else
				{
					num4 = num5;
				}
			}
			if (array[num5] > num3)
			{
				num5--;
			}
			float num6 = array[num5];
			if (num6 == num3)
			{
				return BezierCurves.Locate(startPosition, endPosition, startTangent, endTangent, (float)(num5 / distributionSteps));
			}
			return BezierCurves.Locate(startPosition, endPosition, startTangent, endTangent, ((float)num5 + (num3 - num6) / (array[num5 + 1] - num6)) / (float)distributionSteps);
		}

		// Token: 0x06002020 RID: 8224 RVA: 0x000A6E9C File Offset: 0x000A509C
		public static Vector3 GetFirstDerivative(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage)
		{
			percentage = Mathf.Clamp01(percentage);
			float num = 1f - percentage;
			return 3f * num * num * (startTangent - startPosition) + 6f * num * percentage * (endTangent - startTangent) + 3f * percentage * percentage * (endPosition - endTangent);
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000A6F08 File Offset: 0x000A5108
		private static Vector3 Locate(Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, float percentage)
		{
			percentage = Mathf.Clamp01(percentage);
			float num = 1f - percentage;
			return num * num * num * startPosition + 3f * num * num * percentage * startTangent + 3f * num * percentage * percentage * endTangent + percentage * percentage * percentage * endPosition;
		}
	}
}
