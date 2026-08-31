using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009B3 RID: 2483
	public abstract class BaseScenario : IScenario
	{
		// Token: 0x17001020 RID: 4128
		// (get) Token: 0x06005DB9 RID: 23993 RVA: 0x002CA0A8 File Offset: 0x002C82A8
		public virtual string scenarioTemplateName
		{
			get
			{
				return base.GetType().Name;
			}
		}

		// Token: 0x17001021 RID: 4129
		// (get) Token: 0x06005DBA RID: 23994 RVA: 0x002CA0B5 File Offset: 0x002C82B5
		// (set) Token: 0x06005DBB RID: 23995 RVA: 0x002CA0BD File Offset: 0x002C82BD
		public virtual TIMetaTemplate scenarioTemplate { get; protected set; }

		// Token: 0x17001022 RID: 4130
		// (get) Token: 0x06005DBC RID: 23996 RVA: 0x002CA0C6 File Offset: 0x002C82C6
		// (set) Token: 0x06005DBD RID: 23997 RVA: 0x002CA0CE File Offset: 0x002C82CE
		public TIFactionTemplate activePlayerFaction { get; protected set; }

		// Token: 0x06005DBE RID: 23998 RVA: 0x002CA0D8 File Offset: 0x002C82D8
		public bool OnStartScene()
		{
			if (string.IsNullOrEmpty(this.scenarioTemplateName))
			{
				Debug.LogError("No ScenarioTemplate provided to load.");
				return false;
			}
			if (this.scenarioTemplate == null)
			{
				this.scenarioTemplate = TemplateManager.Find<TIMetaTemplate>(this.scenarioTemplateName, false);
				if (this.scenarioTemplate == null)
				{
					Debug.LogError("No ScenarioTemplate found for " + this.scenarioTemplateName);
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005DBF RID: 23999 RVA: 0x002CA138 File Offset: 0x002C8338
		public virtual bool Initialize()
		{
			this.OnStartScene();
			TIMetaTemplate.LoadMetaTemplates(this.scenarioTemplate.templateNames);
			return true;
		}

		// Token: 0x06005DC0 RID: 24000 RVA: 0x002CA152 File Offset: 0x002C8352
		public void SetActivePlayerFaction(TIFactionTemplate faction)
		{
			this.activePlayerFaction = faction;
		}
	}
}
