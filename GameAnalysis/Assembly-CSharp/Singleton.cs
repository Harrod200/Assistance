using System;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001777 RID: 6007 RVA: 0x00079D90 File Offset: 0x00077F90
	public static T Instance
	{
		get
		{
			T t;
			if (Singleton<T>.m_ShuttingDown)
			{
				string text = "[Singleton] Instance '";
				Type typeFromHandle = typeof(T);
				Debug.LogWarning(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null) + "' already destroyed. Returning null.");
				t = default(T);
				return t;
			}
			object @lock = Singleton<T>.m_Lock;
			lock (@lock)
			{
				if (Singleton<T>.m_Instance == null)
				{
					Singleton<T>.m_Instance = (T)((object)global::UnityEngine.Object.FindObjectOfType(typeof(T)));
					if (Singleton<T>.m_Instance == null)
					{
						GameObject gameObject = new GameObject();
						Singleton<T>.m_Instance = gameObject.AddComponent<T>();
						gameObject.name = typeof(T).ToString() + " (Singleton)";
						global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
					}
				}
				t = Singleton<T>.m_Instance;
			}
			return t;
		}
	}

	// Token: 0x06001778 RID: 6008 RVA: 0x00079E7C File Offset: 0x0007807C
	private void OnApplicationQuit()
	{
		Singleton<T>.m_ShuttingDown = true;
	}

	// Token: 0x06001779 RID: 6009 RVA: 0x00079E84 File Offset: 0x00078084
	private void OnDestroy()
	{
		Singleton<T>.m_ShuttingDown = true;
	}

	// Token: 0x040015BD RID: 5565
	private static bool m_ShuttingDown = false;

	// Token: 0x040015BE RID: 5566
	private static object m_Lock = new object();

	// Token: 0x040015BF RID: 5567
	private static T m_Instance;
}
