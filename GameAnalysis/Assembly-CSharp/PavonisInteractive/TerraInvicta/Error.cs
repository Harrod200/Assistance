using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000909 RID: 2313
	public static class Error
	{
		// Token: 0x0600585F RID: 22623 RVA: 0x00289134 File Offset: 0x00287334
		public static void Log(string message, params object[] args)
		{
			Debug.LogErrorFormat(message, args);
		}

		// Token: 0x06005860 RID: 22624 RVA: 0x0028913D File Offset: 0x0028733D
		public static void LogException(Exception e)
		{
			Error.Log("{0}: {1}", new object[]
			{
				e.GetType(),
				e.Message
			});
			Error.Log(e.StackTrace, Array.Empty<object>());
		}

		// Token: 0x06005861 RID: 22625 RVA: 0x00289171 File Offset: 0x00287371
		public static bool Is(bool condition, string message, params object[] args)
		{
			if (condition)
			{
				Error.Log(message, args);
			}
			return condition;
		}

		// Token: 0x06005862 RID: 22626 RVA: 0x0028917E File Offset: 0x0028737E
		public static bool Is(bool condition)
		{
			return Error.Is(condition, "Check Failed", Array.Empty<object>());
		}

		// Token: 0x06005863 RID: 22627 RVA: 0x00289190 File Offset: 0x00287390
		public static bool IsEqual<T>(T left, T right, string message, params object[] args)
		{
			return Error.Is(left.Equals(right), message, args);
		}

		// Token: 0x06005864 RID: 22628 RVA: 0x002891AC File Offset: 0x002873AC
		public static bool IsEqual<T>(T left, T right)
		{
			return Error.Is(left.Equals(right), "Check Failed: Expected not equal [{0}] {1} == {2}", new object[]
			{
				typeof(T).Name,
				left,
				right
			});
		}

		// Token: 0x06005865 RID: 22629 RVA: 0x00289200 File Offset: 0x00287400
		public static bool IsNotEqual<T>(T left, T right, string message, params object[] args)
		{
			return Error.Is(!left.Equals(right), message, args);
		}

		// Token: 0x06005866 RID: 22630 RVA: 0x00289220 File Offset: 0x00287420
		public static bool IsNotEqual<T>(T left, T right)
		{
			return Error.Is(!left.Equals(right), "Check Failed: Expected equal [{0}] {0} != {1}", new object[]
			{
				typeof(T).Name,
				left,
				right
			});
		}

		// Token: 0x06005867 RID: 22631 RVA: 0x00289277 File Offset: 0x00287477
		public static bool IsNull<T>(T obj, string message, params object[] args)
		{
			return Error.Is(obj == null, message, args);
		}

		// Token: 0x06005868 RID: 22632 RVA: 0x00289289 File Offset: 0x00287489
		public static bool IsNull<T>(T obj)
		{
			return Error.Is(obj == null, "Check Failed: {0} is null", new object[] { typeof(T).Name });
		}

		// Token: 0x06005869 RID: 22633 RVA: 0x002892B6 File Offset: 0x002874B6
		public static bool IsNotNull<T>(T obj, string message, params object[] args)
		{
			return Error.Is(obj != null, message, args);
		}

		// Token: 0x0600586A RID: 22634 RVA: 0x002892C8 File Offset: 0x002874C8
		public static bool IsNotNull<T>(T obj)
		{
			return Error.Is(obj != null, "Check Failed: {0} is not null", new object[] { typeof(T).Name });
		}

		// Token: 0x0600586B RID: 22635 RVA: 0x002892F5 File Offset: 0x002874F5
		public static bool IsNot<T>(object obj, string message, params object[] args)
		{
			return Error.Is(typeof(T) != obj.GetType(), message, args);
		}

		// Token: 0x0600586C RID: 22636 RVA: 0x00289313 File Offset: 0x00287513
		public static bool IsNot<T>(object obj)
		{
			return Error.Is(typeof(T) != obj.GetType(), "Check Failed: var has type {0} instead of {1}", new object[]
			{
				typeof(T),
				obj.GetType()
			});
		}

		// Token: 0x0600586D RID: 22637 RVA: 0x00289350 File Offset: 0x00287550
		public static bool IsOutOfRange(double value, double low, double high, string message, params object[] args)
		{
			return Error.Is(value < low || value > high, message, args);
		}

		// Token: 0x0600586E RID: 22638 RVA: 0x00289365 File Offset: 0x00287565
		public static bool IsOutOfRange(double value, double low, double high)
		{
			return Error.Is(value < low || value > high, "Check Failed: {0} is out of range {1}..{2}", new object[] { value, low, high });
		}

		// Token: 0x0600586F RID: 22639 RVA: 0x0028939D File Offset: 0x0028759D
		public static bool IsNotEqualCount(ICollection left, ICollection right, string message, params object[] args)
		{
			return Error.IsNotEqual<int>(left.Count, right.Count, message, args);
		}

		// Token: 0x06005870 RID: 22640 RVA: 0x002893B2 File Offset: 0x002875B2
		public static bool IsNotEqualCount(ICollection left, ICollection right)
		{
			return Error.IsNotEqual<int>(left.Count, right.Count);
		}

		// Token: 0x06005871 RID: 22641 RVA: 0x002893C5 File Offset: 0x002875C5
		public static bool IsEmpty<T>(T collection, string message, params object[] args) where T : ICollection
		{
			return Error.Is(collection.Count == 0, message, args);
		}

		// Token: 0x06005872 RID: 22642 RVA: 0x002893DE File Offset: 0x002875DE
		public static bool IsEmpty<T>(T collection) where T : ICollection
		{
			return Error.Is(collection.Count == 0, "Check Failed: {0} is empty", new object[] { typeof(T).Name });
		}

		// Token: 0x06005873 RID: 22643 RVA: 0x00289412 File Offset: 0x00287612
		public static bool NotContain<T>(ICollection<T> collection, T obj, string message, params object[] args)
		{
			return Error.Is(!collection.Contains(obj), message, args);
		}

		// Token: 0x06005874 RID: 22644 RVA: 0x00289425 File Offset: 0x00287625
		public static bool NotContain<T>(ICollection<T> collection, T obj)
		{
			return Error.Is(!collection.Contains(obj), "Check Failed: collection does not contain {0}", new object[] { obj });
		}

		// Token: 0x06005875 RID: 22645 RVA: 0x0028944C File Offset: 0x0028764C
		public static bool OnException(Action action, string message, params object[] args)
		{
			bool flag;
			try
			{
				action();
				flag = false;
			}
			catch (Exception ex)
			{
				Error.Log(message, args);
				Error.LogException(ex);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06005876 RID: 22646 RVA: 0x00289484 File Offset: 0x00287684
		public static bool OnException(Action action)
		{
			bool flag;
			try
			{
				action();
				flag = false;
			}
			catch (Exception ex)
			{
				Error.LogException(ex);
				flag = true;
			}
			return flag;
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x002894B8 File Offset: 0x002876B8
		public static bool IsInvalid(object obj)
		{
			return obj is IValidatable && Error.IsInvalid((IValidatable)obj);
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x002894CF File Offset: 0x002876CF
		public static bool IsInvalid(IValidatable obj)
		{
			return Error.Is(!obj.Valid());
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x002894DF File Offset: 0x002876DF
		public static bool IsInvalidGameState<T>(T state) where T : TIGameState
		{
			return Error.IsNull<T>(state) || Error.IsNot<T>(state) || Error.IsInvalid(state);
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x00289503 File Offset: 0x00287703
		public static bool IsDirectoryMissing(string path, string message, params object[] args)
		{
			return Error.Is(!Directory.Exists(path), message, args);
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x00289515 File Offset: 0x00287715
		public static bool IsDirectoryMissing(string path)
		{
			return Error.Is(!Directory.Exists(path), "Check Failed: {0} not found", new object[] { path });
		}

		// Token: 0x0600587C RID: 22652 RVA: 0x00289534 File Offset: 0x00287734
		public static bool IsFileMissing(string path, string message, params object[] args)
		{
			return Error.Is(!File.Exists(path), message, args);
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x00289546 File Offset: 0x00287746
		public static bool IsFileMissing(string path)
		{
			return Error.Is(!File.Exists(path), "Check Failed: {0} not found", new object[] { path });
		}

		// Token: 0x0400404A RID: 16458
		private const string failedMessage = "Check Failed";

		// Token: 0x0400404B RID: 16459
		private const string typeMessage = "Check Failed: var has type {0}";

		// Token: 0x0400404C RID: 16460
		private const string notTypeMessage = "Check Failed: var has type {0} instead of {1}";

		// Token: 0x0400404D RID: 16461
		private const string nullMessage = "Check Failed: {0} is null";

		// Token: 0x0400404E RID: 16462
		private const string notNullMessage = "Check Failed: {0} is not null";

		// Token: 0x0400404F RID: 16463
		private const string equalMessage = "Check Failed: Expected not equal [{0}] {1} == {2}";

		// Token: 0x04004050 RID: 16464
		private const string notEqualMessage = "Check Failed: Expected equal [{0}] {0} != {1}";

		// Token: 0x04004051 RID: 16465
		private const string emptyCollectionMessage = "Check Failed: {0} is empty";

		// Token: 0x04004052 RID: 16466
		private const string notContainsMessage = "Check Failed: collection does not contain {0}";

		// Token: 0x04004053 RID: 16467
		private const string outOfRangeMessage = "Check Failed: {0} is out of range {1}..{2}";

		// Token: 0x04004054 RID: 16468
		private const string exceptionMessage = "{0}: {1}";

		// Token: 0x04004055 RID: 16469
		private const string notFoundMessage = "Check Failed: {0} not found";
	}
}
