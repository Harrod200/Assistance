using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D8 RID: 2008
	public class TIPlayerState : TIGameState
	{
		// Token: 0x17000E71 RID: 3697
		// (get) Token: 0x06004887 RID: 18567 RVA: 0x001DD412 File Offset: 0x001DB612
		public TIPlayerTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIPlayerTemplate>();
			}
		}

		// Token: 0x17000E72 RID: 3698
		// (get) Token: 0x06004888 RID: 18568 RVA: 0x001DD41A File Offset: 0x001DB61A
		// (set) Token: 0x06004889 RID: 18569 RVA: 0x001DD422 File Offset: 0x001DB622
		public bool isAI { get; private set; }

		// Token: 0x0600488A RID: 18570 RVA: 0x001DD42C File Offset: 0x001DB62C
		public override void InitWithTemplate(TIDataTemplate template)
		{
			TIPlayerTemplate tiplayerTemplate = template as TIPlayerTemplate;
			if (tiplayerTemplate == null)
			{
				return;
			}
			this.templateName = tiplayerTemplate.dataName;
			this.name = template.dataName;
			base.InitWithTemplate(template);
		}

		// Token: 0x0600488B RID: 18571 RVA: 0x001DD464 File Offset: 0x001DB664
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (this.template != null)
			{
				bool flag = false;
				foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
				{
					if (tifactionState.templateName == this.template.council)
					{
						this.faction = tifactionState;
						tifactionState.player = this;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Debug.LogWarning("Could not find council " + this.template.council + " when loading player " + this.templateName);
				}
			}
		}

		// Token: 0x0600488C RID: 18572 RVA: 0x001DD508 File Offset: 0x001DB708
		public override void PostGlobalGameStateCreateInit_2()
		{
			base.PostGlobalGameStateCreateInit_2();
			this.bugReportMessage = "";
		}

		// Token: 0x0600488D RID: 18573 RVA: 0x001DD51B File Offset: 0x001DB71B
		public void AssignAIStatus(bool isAI)
		{
			this.isAI = isAI;
		}

		// Token: 0x040029B7 RID: 10679
		public TIFactionState faction;

		// Token: 0x040029B8 RID: 10680
		public string name;

		// Token: 0x040029BA RID: 10682
		public string bugReportMessage = "";
	}
}
