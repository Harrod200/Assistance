using System;
using System.Text;
using AOT;
using Steamworks;
using UnityEngine;

// Token: 0x0200041D RID: 1053
[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	// Token: 0x17000330 RID: 816
	// (get) Token: 0x060015FD RID: 5629 RVA: 0x0006FECE File Offset: 0x0006E0CE
	protected static SteamManager Instance
	{
		get
		{
			if (SteamManager.s_instance == null)
			{
				return new GameObject("SteamManager").AddComponent<SteamManager>();
			}
			return SteamManager.s_instance;
		}
	}

	// Token: 0x17000331 RID: 817
	// (get) Token: 0x060015FE RID: 5630 RVA: 0x0006FEF2 File Offset: 0x0006E0F2
	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x0006FEFE File Offset: 0x0006E0FE
	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x0006FF06 File Offset: 0x0006E106
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void InitOnPlayMode()
	{
		SteamManager.s_EverInitialized = false;
		SteamManager.s_instance = null;
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x0006FF14 File Offset: 0x0006E114
	protected virtual void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			global::UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		if (SteamManager.s_EverInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session!");
		}
		global::UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		if (Application.isEditor)
		{
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		Debug.Log("Steam successfully initialized");
		SteamManager.s_EverInitialized = true;
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x0006FFB8 File Offset: 0x0006E1B8
	protected virtual void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x00070006 File Offset: 0x0006E206
	protected virtual void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.Shutdown();
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x0007002A File Offset: 0x0006E22A
	protected virtual void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
	}

	// Token: 0x040013F7 RID: 5111
	protected static bool s_EverInitialized;

	// Token: 0x040013F8 RID: 5112
	protected static SteamManager s_instance;

	// Token: 0x040013F9 RID: 5113
	protected bool m_bInitialized;

	// Token: 0x040013FA RID: 5114
	protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
}
