using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x0200002E RID: 46
public static class Utilities
{
	// Token: 0x0600019B RID: 411 RVA: 0x0000DC60 File Offset: 0x0000BE60
	public static string GetStackTrace()
	{
		StackFrame stackFrame = new StackFrame(1, true);
		StackTrace stackTrace = new StackTrace(stackFrame);
		return stackFrame.GetMethod().Name + ", " + stackTrace.ToString();
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000DC95 File Offset: 0x0000BE95
	public static float SinEase(float input)
	{
		return (Mathf.Sin(3.1415927f * (input - 0.5f)) + 1f) / 2f;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000DCB5 File Offset: 0x0000BEB5
	public static int CountBits(uint bits)
	{
		bits -= (bits >> 1) & 1431655765U;
		bits = (bits & 858993459U) + ((bits >> 2) & 858993459U);
		bits = ((bits + (bits >> 4)) & 252645135U) * 16843009U >> 24;
		return (int)bits;
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000DCEE File Offset: 0x0000BEEE
	public static int CountBits(ulong bits)
	{
		return Utilities.CountBits((uint)(bits & (ulong)(-1))) + Utilities.CountBits((uint)(bits >> 32));
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000DD08 File Offset: 0x0000BF08
	public static string Capitalize(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			return str.First<char>().ToString().ToUpper(CultureInfo.CurrentCulture) + str.Substring(1);
		}
		return str;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000DD43 File Offset: 0x0000BF43
	public static string PlayerCountryCode()
	{
		return RegionInfo.CurrentRegion.ThreeLetterISORegionName;
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x0000DD4F File Offset: 0x0000BF4F
	public static int IndexOf<T>(this IEnumerable<T> enumerable, T element)
	{
		return enumerable.ToList<T>().IndexOf(element);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000DD60 File Offset: 0x0000BF60
	public static T MinBy_IComparable<T, U>(this IEnumerable<T> enumerable, Func<T, U> Evaluate) where U : IComparable
	{
		if (enumerable.Count<T>() == 0)
		{
			return default(T);
		}
		return enumerable.Sorted<T, U>(Evaluate).First<T>();
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000DD8C File Offset: 0x0000BF8C
	public static T MaxBy_IComparable<T, U>(this IEnumerable<T> enumerable, Func<T, U> Evaluate) where U : IComparable
	{
		if (enumerable.Count<T>() == 0)
		{
			return default(T);
		}
		return enumerable.Sorted<T, U>(Evaluate).Last<T>();
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000DDB7 File Offset: 0x0000BFB7
	public static IEnumerable<Transform> GetChildren(this Transform transform)
	{
		return transform.GetComponentsInChildren<Transform>();
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000DDC0 File Offset: 0x0000BFC0
	public static double VariableTruncate(double value, int decimalPlaces)
	{
		decimalPlaces = Mathf.Clamp(decimalPlaces, 0, 28);
		if (decimalPlaces == 0)
		{
			return Math.Truncate(value);
		}
		double num = Mathd.Pow(10.0, (double)decimalPlaces);
		return Math.Truncate(num * value) / num;
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000DE00 File Offset: 0x0000C000
	public static float VariableTruncate(float value, int decimalPlaces)
	{
		decimalPlaces = Mathf.Clamp(decimalPlaces, 0, 9);
		if (decimalPlaces == 0)
		{
			return (float)Math.Truncate((double)value);
		}
		float num = Mathf.Pow(10f, (float)decimalPlaces);
		return (float)Math.Truncate((double)(num * value)) / num;
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000DE3D File Offset: 0x0000C03D
	public static int Round(this float value)
	{
		return (int)Math.Round((double)value);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000DE47 File Offset: 0x0000C047
	public static int RoundUp(this float value)
	{
		return (int)Math.Ceiling((double)value);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000DE51 File Offset: 0x0000C051
	public static int RoundDown(this float value)
	{
		return (int)value;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0000DE55 File Offset: 0x0000C055
	public static bool Between(double value, double lower, double upper, bool inclusiveLower, bool inclusiveUpper)
	{
		if (inclusiveLower)
		{
			if (inclusiveUpper)
			{
				return value >= lower && value <= upper;
			}
			return value >= lower && value < upper;
		}
		else
		{
			if (inclusiveUpper)
			{
				return value > lower && value <= upper;
			}
			return value > lower && value < upper;
		}
	}

	// Token: 0x060001AB RID: 427 RVA: 0x0000DE93 File Offset: 0x0000C093
	public static Vector3 XZY(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.z, vector.y);
	}

	// Token: 0x060001AC RID: 428 RVA: 0x0000DEAC File Offset: 0x0000C0AC
	public static IEnumerable<int> Range(this int integer)
	{
		if (integer < 0)
		{
			throw new ArgumentException();
		}
		IEnumerable<int> enumerable = Enumerable.Empty<int>();
		for (int i = 0; i < integer; i++)
		{
			enumerable = enumerable.Append(i);
		}
		return enumerable;
	}

	// Token: 0x060001AD RID: 429 RVA: 0x0000DEE0 File Offset: 0x0000C0E0
	public static string ToCommaSeparatedString<T>(this IEnumerable<T> elements, Func<T, string> ToString = null)
	{
		if (!elements.Any<T>())
		{
			return "";
		}
		if (ToString == null)
		{
			ToString = (T x) => x.ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		foreach (T t in elements)
		{
			if (!flag)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(ToString(t));
			flag = false;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0000DF80 File Offset: 0x0000C180
	public static string ToSeparatedString<T>(this IEnumerable<T> elements, Func<T, string> ToString = null)
	{
		if (!elements.Any<T>())
		{
			return "";
		}
		if (ToString == null)
		{
			ToString = (T x) => x.ToString();
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (T t in elements)
		{
			stringBuilder.Append(ToString(t));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0000E010 File Offset: 0x0000C210
	public static IEnumerable<T> LinkedList<T>(T element, Func<T, T> GetNextElement)
	{
		IEnumerable<T> enumerable = Enumerable.Empty<T>();
		while (element != null)
		{
			enumerable = enumerable.Append(element);
			element = GetNextElement(element);
		}
		return enumerable;
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0000E03F File Offset: 0x0000C23F
	public static IEnumerable<U> SelectSansNulls<T, U>(this IEnumerable<T> collection, Func<T, U> Selector)
	{
		return from x in collection.Select<T, U>(Selector)
			where x != null
			select x;
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000E06C File Offset: 0x0000C26C
	public static double GetElapsedFractionalMilliseconds(this Stopwatch stopwatch)
	{
		return 1000.0 * stopwatch.GetElapsedSeconds();
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000E07E File Offset: 0x0000C27E
	public static double GetElapsedSeconds(this Stopwatch stopwatch)
	{
		return (double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000E090 File Offset: 0x0000C290
	public static bool CanParseAsInt(this string string_)
	{
		int num;
		return int.TryParse(string_, out num);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000E0A8 File Offset: 0x0000C2A8
	public static bool CanParseAsFloat(this string string_)
	{
		float num;
		return float.TryParse(string_, out num);
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000E0C0 File Offset: 0x0000C2C0
	public static bool CanParseAsDouble(this string string_)
	{
		double num;
		return double.TryParse(string_, out num);
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000E0D5 File Offset: 0x0000C2D5
	public static IEnumerable<string> SplitLines(this string string_)
	{
		return string_.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000E0EC File Offset: 0x0000C2EC
	private static IEnumerator EnableOrDisableAsynchronously(MonoBehaviour[] monoBehaviours, bool enable, float secondsPerFrame)
	{
		Stopwatch stopwatch = new Stopwatch();
		foreach (MonoBehaviour behaviour in monoBehaviours)
		{
			stopwatch.Start();
			behaviour.enabled = enable;
			stopwatch.Stop();
			if (stopwatch.GetElapsedSeconds() > (double)secondsPerFrame)
			{
				stopwatch.Reset();
				yield return null;
			}
		}
		MonoBehaviour[] array = null;
		yield break;
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000E109 File Offset: 0x0000C309
	public static void EnableAsynchronously(this MonoBehaviour[] monoBehaviours, float secondsPerFrame = 0.001f)
	{
		CoroutineDummy.Singleton.StartCoroutine(Utilities.EnableOrDisableAsynchronously(monoBehaviours, true, secondsPerFrame));
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000E11E File Offset: 0x0000C31E
	public static void EnableAsynchronously(this IEnumerable<MonoBehaviour> monoBehaviours, float secondsPerFrame = 0.001f)
	{
		monoBehaviours.ToArray<MonoBehaviour>().EnableAsynchronously(secondsPerFrame);
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000E12C File Offset: 0x0000C32C
	public static void DisableAsynchronously(this MonoBehaviour[] monoBehaviours, float secondsPerFrame = 0.001f)
	{
		CoroutineDummy.Singleton.StartCoroutine(Utilities.EnableOrDisableAsynchronously(monoBehaviours, false, secondsPerFrame));
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000E141 File Offset: 0x0000C341
	public static void DisableAsynchronously(this IEnumerable<MonoBehaviour> monoBehaviours, float secondsPerFrame = 0.001f)
	{
		monoBehaviours.ToArray<MonoBehaviour>().DisableAsynchronously(secondsPerFrame);
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x060001BC RID: 444 RVA: 0x0000E150 File Offset: 0x0000C350
	private static List<float> SmoothLerpSamples
	{
		get
		{
			if (Utilities.smoothLerpSamples != null)
			{
				return Utilities.smoothLerpSamples;
			}
			Utilities.smoothLerpSamples = new List<float>();
			float num = 0f;
			while ((double)num < 0.99)
			{
				Utilities.smoothLerpSamples.Add(num);
				num = Mathf.Lerp(num, 1f, 0.0033333334f);
			}
			Utilities.smoothLerpSamples.Add(num);
			for (int i = 0; i < Utilities.smoothLerpSamples.Count; i++)
			{
				List<float> list = Utilities.smoothLerpSamples;
				int num2 = i;
				list[num2] /= num;
			}
			List<float> list2 = new List<float>(Utilities.smoothLerpSamples);
			List<float> list3 = new List<float>(Utilities.smoothLerpSamples);
			list3.Reverse();
			for (int j = 0; j < list2.Count; j++)
			{
				list2[j] = 0.5f + list2[j] / 2f;
				list3[j] = (1f - list3[j]) / 2f;
			}
			List<float> list4 = list3.Union<float>(list2).ToList<float>();
			Utilities.smoothLerpSamples.Clear();
			Utilities.smoothLerpSamples.Add(0f);
			for (int k = 1; k < 8; k++)
			{
				Utilities.smoothLerpSamples.Add(list4[(int)((double)(list4.Count * k) / 8.0)]);
			}
			Utilities.smoothLerpSamples.Add(1f);
			return Utilities.smoothLerpSamples;
		}
	}

	// Token: 0x060001BD RID: 445 RVA: 0x0000E2C0 File Offset: 0x0000C4C0
	public static float GetSmoothLerpFactor(float factor)
	{
		float num = (float)Utilities.SmoothLerpSamples.Count * factor;
		return Mathf.Lerp(Utilities.SmoothLerpSamples[(int)num], Utilities.SmoothLerpSamples[(int)num + 1], num - (float)((int)num));
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000E300 File Offset: 0x0000C500
	public static float Median(IEnumerable<float> values, bool medoid = false)
	{
		values = values.OrderBy<float, float>((float x) => x);
		int num = values.Count<float>();
		if (num > 1)
		{
			int num2 = num / 2;
			if (num2 % 2 == 0 && !medoid)
			{
				return (values.ElementAt<float>(num2 - 1) + values.ElementAt<float>(num2 + 1)) / 2f;
			}
			return values.ElementAt<float>(num2);
		}
		else
		{
			if (num == 1)
			{
				return values.First<float>();
			}
			return 0f;
		}
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000E380 File Offset: 0x0000C580
	public static float RoundToStep(float value, float stepAmount, Utilities.RoundType type = Utilities.RoundType.Nearest)
	{
		float num = 1f / stepAmount;
		float num2 = value * num;
		switch (type)
		{
		case Utilities.RoundType.Nearest:
			num2 = Mathf.Round(num2);
			break;
		case Utilities.RoundType.Up:
			num2 = Mathf.Ceil(num2);
			break;
		case Utilities.RoundType.Down:
			num2 = Mathf.Floor(num2);
			break;
		default:
			throw new ArgumentException(string.Format("Unknown type: {0}", type), "type");
		}
		return num2 / num;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000E3E6 File Offset: 0x0000C5E6
	public static bool CompareColor32(Color32 color1, Color32 color2)
	{
		return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b;
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000E414 File Offset: 0x0000C614
	public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
		if (!directoryInfo.Exists)
		{
			throw new DirectoryNotFoundException("Source directory not found: " + directoryInfo.FullName);
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		Directory.CreateDirectory(destinationDir);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles())
		{
			string text = Path.Combine(destinationDir, fileInfo.Name);
			fileInfo.CopyTo(text, true);
		}
		if (recursive)
		{
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string text2 = Path.Combine(destinationDir, directoryInfo2.Name);
				Utilities.CopyDirectory(directoryInfo2.FullName, text2, true);
			}
		}
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000E4C4 File Offset: 0x0000C6C4
	public static bool IsFileInUse(FileInfo file)
	{
		try
		{
			using (file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
			{
			}
		}
		catch (Exception)
		{
			return true;
		}
		return false;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000E510 File Offset: 0x0000C710
	public static bool CanDeleteDirectory(DirectoryInfo directory)
	{
		bool flag = true;
		FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
		for (int i = 0; i < files.Length; i++)
		{
			if (Utilities.IsFileInUse(files[i]))
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000E54C File Offset: 0x0000C74C
	public static void DebugDrawPlane(Vector3 position, Vector3 normal, Color color, float scalar = 1f)
	{
		Vector3 vector;
		if (normal.normalized != Vector3.forward)
		{
			vector = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
		}
		else
		{
			vector = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
		}
		Vector3 vector2 = position + vector * scalar;
		Vector3 vector3 = position - vector * scalar;
		vector = Quaternion.AngleAxis(90f, normal) * vector;
		Vector3 vector4 = position + vector * scalar;
		Vector3 vector5 = position - vector * scalar;
		global::UnityEngine.Debug.DrawLine(vector2, vector3, color);
		global::UnityEngine.Debug.DrawLine(vector4, vector5, color);
		global::UnityEngine.Debug.DrawLine(vector2, vector4, color);
		global::UnityEngine.Debug.DrawLine(vector4, vector3, color);
		global::UnityEngine.Debug.DrawLine(vector3, vector5, color);
		global::UnityEngine.Debug.DrawLine(vector5, vector2, color);
		global::UnityEngine.Debug.DrawRay(position, normal, Color.red);
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000E63C File Offset: 0x0000C83C
	public static void DebugDrawPoint(Vector3 position, float lineLength, Color color, float duration = 0f)
	{
		global::UnityEngine.Debug.DrawLine(position + new Vector3(lineLength, lineLength), position + new Vector3(-lineLength, -lineLength), color, duration);
		global::UnityEngine.Debug.DrawLine(position + new Vector3(lineLength, -lineLength), position + new Vector3(-lineLength, lineLength), color, duration);
		global::UnityEngine.Debug.DrawLine(position + new Vector3(0f, 0f, lineLength), position + new Vector3(0f, 0f, -lineLength), color, duration);
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000E6C4 File Offset: 0x0000C8C4
	public static void DebugDrawCone(Transform transform, Vector3 direction, int numberOfLines, float angle, float lineLength, Color color, float duration = 0f)
	{
		int num = 360 / numberOfLines;
		float num2 = lineLength / Mathf.Cos(angle * 0.017453292f);
		for (int i = 0; i < numberOfLines; i++)
		{
			Vector3 vector = Quaternion.AngleAxis((float)(i * num), direction) * Quaternion.AngleAxis(angle, transform.right) * direction.normalized * num2;
			global::UnityEngine.Debug.DrawRay(transform.position, vector, color, duration);
		}
		global::UnityEngine.Debug.DrawRay(transform.position, direction.normalized * lineLength, color, duration);
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000E750 File Offset: 0x0000C950
	public static void DebugDrawCircle(Vector3 position, Quaternion rotation, float radius, Color color, int segments = 8, float duration = 0f)
	{
		if (radius <= 0f || segments <= 0)
		{
			return;
		}
		float num = 360f / (float)segments;
		num *= 0.017453292f;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		for (int i = 0; i < segments; i++)
		{
			vector.x = Mathf.Cos(num * (float)i);
			vector.y = Mathf.Sin(num * (float)i);
			vector.z = 0f;
			vector2.x = Mathf.Cos(num * (float)(i + 1));
			vector2.y = Mathf.Sin(num * (float)(i + 1));
			vector2.z = 0f;
			vector *= radius;
			vector2 *= radius;
			vector = rotation * vector;
			vector2 = rotation * vector2;
			vector += position;
			vector2 += position;
			global::UnityEngine.Debug.DrawLine(vector, vector2, color, duration);
		}
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000E834 File Offset: 0x0000CA34
	public static void DebugDrawSphere(Vector3 position, Quaternion orientation, float radius, Color color, int segments = 4, float duration = 0f)
	{
		if (segments < 2)
		{
			segments = 2;
		}
		int num = segments * 2;
		float num2 = 180f / (float)segments;
		for (int i = 0; i < segments; i++)
		{
			Utilities.DebugDrawCircle(position, orientation * Quaternion.Euler(0f, num2 * (float)i, 0f), radius, color, num, duration);
		}
		Vector3 vector = Vector3.zero;
		float num3 = 3.1415927f / (float)segments;
		for (int j = 1; j < segments; j++)
		{
			float num4 = num3 * (float)j;
			vector = orientation * Vector3.up * Mathf.Cos(num4) * radius;
			float num5 = Mathf.Sin(num4) * radius;
			Utilities.DebugDrawCircle(position + vector, orientation * Quaternion.Euler(90f, 0f, 0f), num5, color, num, 0f);
		}
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000E91C File Offset: 0x0000CB1C
	public static void DebugDrawBox(Bounds bounds, Color color, float duration = 0f)
	{
		global::UnityEngine.Debug.DrawLine(bounds.min, new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), color, duration);
		global::UnityEngine.Debug.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), color, duration);
		global::UnityEngine.Debug.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z), new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z), new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z), new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.min.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(bounds.max, new Vector3(bounds.min.x, bounds.max.y, bounds.max.z), color, duration);
		global::UnityEngine.Debug.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.max.y, bounds.min.z), color, duration);
	}

	// Token: 0x040001E0 RID: 480
	public const string templateFolder = "/Templates";

	// Token: 0x040001E1 RID: 481
	public const string namelistFolder = "/Namelists";

	// Token: 0x040001E2 RID: 482
	public const string locFolder = "/Localization";

	// Token: 0x040001E3 RID: 483
	public const string modFolder = "Mods/Enabled";

	// Token: 0x040001E4 RID: 484
	public const string modFolderDisabled = "Mods/Disabled";

	// Token: 0x040001E5 RID: 485
	public const string modFolderWithSlash = "Mods/Enabled/";

	// Token: 0x040001E6 RID: 486
	public const string modFolderDisabledWithSlash = "Mods/Disabled/";

	// Token: 0x040001E7 RID: 487
	public const string dlcFolder = "DLC_Content";

	// Token: 0x040001E8 RID: 488
	public const string dlcFolderWithSlash = "DLC_Content/";

	// Token: 0x040001E9 RID: 489
	private static List<float> smoothLerpSamples;

	// Token: 0x02000AC8 RID: 2760
	public enum RoundType
	{
		// Token: 0x04004890 RID: 18576
		Nearest,
		// Token: 0x04004891 RID: 18577
		Up,
		// Token: 0x04004892 RID: 18578
		Down
	}
}
