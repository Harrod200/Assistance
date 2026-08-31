using System;
using UnityEngine;

namespace AssetBundles
{
	// Token: 0x020004EE RID: 1262
	public class Utility
	{
		// Token: 0x06001E35 RID: 7733 RVA: 0x0009E533 File Offset: 0x0009C733
		public static string GetPlatformName()
		{
			return Utility.GetPlatformForAssetBundles(Application.platform);
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x0009E53F File Offset: 0x0009C73F
		private static string GetPlatformForAssetBundles(RuntimePlatform platform)
		{
			if (platform == RuntimePlatform.OSXPlayer)
			{
				return "StandaloneOSX";
			}
			if (platform == RuntimePlatform.WindowsPlayer)
			{
				return "StandaloneWindows64";
			}
			if (platform != RuntimePlatform.LinuxPlayer)
			{
				throw new Exception(string.Format("Unsupported RuntimePlatform {0}", platform));
			}
			return "StandaloneLinux64";
		}

		// Token: 0x040017F1 RID: 6129
		public const string AssetBundlesOutputPath = "AssetBundles";
	}
}
