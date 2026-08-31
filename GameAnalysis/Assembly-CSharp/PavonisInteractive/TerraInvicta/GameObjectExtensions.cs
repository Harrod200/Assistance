using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F7 RID: 2039
	public static class GameObjectExtensions
	{
		// Token: 0x060049FD RID: 18941 RVA: 0x001F103B File Offset: 0x001EF23B
		public static T GetComponent<T>(this GameObject gameObject) where T : MonoBehaviour
		{
			return gameObject.GetComponent<T>();
		}

		// Token: 0x060049FE RID: 18942 RVA: 0x001F1044 File Offset: 0x001EF244
		public static void GetComponentsInChildren<T>(this GameObject gameObject, bool includeInactive, int childDepth, ref List<T> components)
		{
			foreach (object obj in gameObject.transform)
			{
				Transform transform = (Transform)obj;
				T t;
				if ((transform.gameObject.activeSelf || includeInactive) && transform.TryGetComponent<T>(out t))
				{
					components.Add(t);
				}
				if (childDepth > 0)
				{
					gameObject.GetComponentsInChildren<T>(includeInactive, childDepth - 1, ref components);
				}
			}
		}

		// Token: 0x060049FF RID: 18943 RVA: 0x001F10C8 File Offset: 0x001EF2C8
		public static T GetComponentOnChild<T>(this GameObject gameObject, string childName) where T : Component
		{
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].name == childName)
				{
					return componentsInChildren[i].gameObject.GetComponent<T>();
				}
			}
			return default(T);
		}

		// Token: 0x06004A00 RID: 18944 RVA: 0x001F1112 File Offset: 0x001EF312
		public static bool Has<T>(this GameObject gameObject) where T : MonoBehaviour
		{
			return gameObject.GetComponent<T>() != null;
		}

		// Token: 0x06004A01 RID: 18945 RVA: 0x001F1128 File Offset: 0x001EF328
		public static T Add<T>(this GameObject gameObject) where T : MonoBehaviour
		{
			TIGameObjectEntity component = gameObject.GetComponent<TIGameObjectEntity>();
			if (component == null)
			{
				throw new Exception("Cannot add ComponentData without TIGameObjectEntity");
			}
			T t = gameObject.AddComponent<T>();
			component.enabled = false;
			component.enabled = true;
			return t;
		}

		// Token: 0x06004A02 RID: 18946 RVA: 0x001F1164 File Offset: 0x001EF364
		public static T GetOrAdd<T>(this GameObject gameObject) where T : MonoBehaviour
		{
			if (!gameObject.Has<T>())
			{
				return gameObject.Add<T>();
			}
			return gameObject.GetComponent<T>();
		}

		// Token: 0x06004A03 RID: 18947 RVA: 0x001F117C File Offset: 0x001EF37C
		public static void Remove<T>(this GameObject gameObject, bool destroyImmediately = false) where T : MonoBehaviour
		{
			TIGameObjectEntity component = gameObject.GetComponent<TIGameObjectEntity>();
			if (component == null)
			{
				throw new Exception("Cannot remove ComponentData without TIGameObjectEntity");
			}
			if (destroyImmediately)
			{
				global::UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<T>());
			}
			else
			{
				global::UnityEngine.Object.Destroy(gameObject.GetComponent<T>());
			}
			component.enabled = false;
			component.enabled = true;
		}

		// Token: 0x06004A04 RID: 18948 RVA: 0x001F11D5 File Offset: 0x001EF3D5
		public static T GetComponentInParent<T>(this GameObject gameObject, bool includeInactive) where T : Component
		{
			return gameObject.transform.GetComponentInParent<T>(includeInactive);
		}

		// Token: 0x06004A05 RID: 18949 RVA: 0x001F11E3 File Offset: 0x001EF3E3
		public static T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
		{
			return component.transform.GetComponentInParent<T>(includeInactive);
		}

		// Token: 0x06004A06 RID: 18950 RVA: 0x001F11F4 File Offset: 0x001EF3F4
		public static T GetComponentInParent<T>(this Transform transform, bool includeInactive) where T : Component
		{
			if (!includeInactive)
			{
				return transform.GetComponentInParent<T>();
			}
			T component = transform.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			if (transform.parent == null)
			{
				return default(T);
			}
			return transform.parent.GetComponentInParent<T>(includeInactive);
		}

		// Token: 0x06004A07 RID: 18951 RVA: 0x001F1248 File Offset: 0x001EF448
		public static void SortChildren(this Transform transform, Func<Transform, IComparable> Evaluate, bool smallestToLargest = true, Func<Transform, bool> Predicate = null)
		{
			if (Predicate == null)
			{
				Predicate = (Transform child) => true;
			}
			List<Transform> list = transform.GetChildren().Where<Transform>(Predicate).Sorted<Transform, IComparable>(Evaluate);
			if (smallestToLargest)
			{
				list.Reverse();
			}
			foreach (Transform transform2 in list)
			{
				transform2.SetAsFirstSibling();
			}
		}

		// Token: 0x06004A08 RID: 18952 RVA: 0x001F12D4 File Offset: 0x001EF4D4
		public static void SortChildren<T>(this Transform transform, Func<T, IComparable> Evaluate, bool smallestToLargest = true) where T : MonoBehaviour
		{
			transform.SortChildren((Transform child) => Evaluate(child.GetComponent<T>()), smallestToLargest, (Transform child) => child.HasComponent<T>());
		}

		// Token: 0x06004A09 RID: 18953 RVA: 0x001F1320 File Offset: 0x001EF520
		public static IEnumerable<Transform> Children(this Transform transform)
		{
			return from child in transform.GetComponentsInChildren<Transform>()
				where child != transform
				select child;
		}

		// Token: 0x06004A0A RID: 18954 RVA: 0x001F1356 File Offset: 0x001EF556
		public static int ActiveChildCount(this Transform transform)
		{
			return (from child in transform.Children()
				where child.gameObject.activeSelf
				select child).Count<Transform>();
		}

		// Token: 0x06004A0B RID: 18955 RVA: 0x001F1387 File Offset: 0x001EF587
		public static bool HasComponent<T>(this GameObject gameObject)
		{
			return gameObject.GetComponent<T>() != null;
		}

		// Token: 0x06004A0C RID: 18956 RVA: 0x001F1397 File Offset: 0x001EF597
		public static bool HasComponent<T>(this Component component)
		{
			return component.GetComponent<T>() != null;
		}
	}
}
