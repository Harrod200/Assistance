using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x02000520 RID: 1312
	public static class CoreMath
	{
		// Token: 0x0600205A RID: 8282 RVA: 0x000A824D File Offset: 0x000A644D
		public static float LinearInterpolate(float from, float to, float percentage)
		{
			return (to - from) * percentage + from;
		}

		// Token: 0x0600205B RID: 8283 RVA: 0x000A8256 File Offset: 0x000A6456
		public static Vector2 LinearInterpolate(Vector2 from, Vector2 to, float percentage)
		{
			return new Vector2(CoreMath.LinearInterpolate(from.x, to.x, percentage), CoreMath.LinearInterpolate(from.y, to.y, percentage));
		}

		// Token: 0x0600205C RID: 8284 RVA: 0x000A8281 File Offset: 0x000A6481
		public static Vector3 LinearInterpolate(Vector3 from, Vector3 to, float percentage)
		{
			return new Vector3(CoreMath.LinearInterpolate(from.x, to.x, percentage), CoreMath.LinearInterpolate(from.y, to.y, percentage), CoreMath.LinearInterpolate(from.z, to.z, percentage));
		}

		// Token: 0x0600205D RID: 8285 RVA: 0x000A82C0 File Offset: 0x000A64C0
		public static Vector4 LinearInterpolate(Vector4 from, Vector4 to, float percentage)
		{
			return new Vector4(CoreMath.LinearInterpolate(from.x, to.x, percentage), CoreMath.LinearInterpolate(from.y, to.y, percentage), CoreMath.LinearInterpolate(from.z, to.z, percentage), CoreMath.LinearInterpolate(from.w, to.w, percentage));
		}

		// Token: 0x0600205E RID: 8286 RVA: 0x000A831C File Offset: 0x000A651C
		public static Rect LinearInterpolate(Rect from, Rect to, float percentage)
		{
			return new Rect(CoreMath.LinearInterpolate(from.x, to.x, percentage), CoreMath.LinearInterpolate(from.y, to.y, percentage), CoreMath.LinearInterpolate(from.width, to.width, percentage), CoreMath.LinearInterpolate(from.height, to.height, percentage));
		}

		// Token: 0x0600205F RID: 8287 RVA: 0x000A8380 File Offset: 0x000A6580
		public static Color LinearInterpolate(Color from, Color to, float percentage)
		{
			return new Color(CoreMath.LinearInterpolate(from.r, to.r, percentage), CoreMath.LinearInterpolate(from.g, to.g, percentage), CoreMath.LinearInterpolate(from.b, to.b, percentage), CoreMath.LinearInterpolate(from.a, to.a, percentage));
		}

		// Token: 0x06002060 RID: 8288 RVA: 0x000A83DC File Offset: 0x000A65DC
		public static float EvaluateCurve(AnimationCurve curve, float percentage)
		{
			return curve.Evaluate(curve[curve.length - 1].time * percentage);
		}
	}
}
