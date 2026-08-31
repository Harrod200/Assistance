using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Token: 0x02000402 RID: 1026
[CreateAssetMenu(fileName = "TestSettings", menuName = "Installers/TestSettings")]
public class TestSettings : ScriptableObjectInstaller<TestSettings>
{
	// Token: 0x0600150B RID: 5387 RVA: 0x000668F4 File Offset: 0x00064AF4
	public override void InstallBindings()
	{
		base.Container.BindInstance<TestSettings.TemplateSettings>(this.templateSettings);
		base.Container.BindInstance<TestSettings.EntitySettings>(this.entitySettings);
		base.Container.BindInstance<TestSettings.DebugSettings>(this.debugSettings);
	}

	// Token: 0x0400129B RID: 4763
	public TestSettings.TemplateSettings templateSettings;

	// Token: 0x0400129C RID: 4764
	public TestSettings.EntitySettings entitySettings;

	// Token: 0x0400129D RID: 4765
	public TestSettings.DebugSettings debugSettings;

	// Token: 0x02000BFA RID: 3066
	[Serializable]
	public class TemplateSettings
	{
		// Token: 0x04004CEF RID: 19695
		public List<string> templates;
	}

	// Token: 0x02000BFB RID: 3067
	[Serializable]
	public class EntitySettings
	{
		// Token: 0x04004CF0 RID: 19696
		public GameObject entityPrefab;

		// Token: 0x04004CF1 RID: 19697
		public GameObject gameTimePrefab;
	}

	// Token: 0x02000BFC RID: 3068
	[Serializable]
	public class DebugSettings
	{
		// Token: 0x04004CF2 RID: 19698
		public bool ShowCameraDebug;
	}
}
