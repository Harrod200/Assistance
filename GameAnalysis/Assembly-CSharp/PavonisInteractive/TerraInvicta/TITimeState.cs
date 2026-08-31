using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007DB RID: 2011
	public class TITimeState : TIGameState
	{
		// Token: 0x17000E78 RID: 3704
		// (get) Token: 0x0600489F RID: 18591 RVA: 0x001DDDCA File Offset: 0x001DBFCA
		public TIStartTimeTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIStartTimeTemplate>();
			}
		}

		// Token: 0x17000E79 RID: 3705
		// (get) Token: 0x060048A0 RID: 18592 RVA: 0x001DDDD2 File Offset: 0x001DBFD2
		// (set) Token: 0x060048A1 RID: 18593 RVA: 0x001DDDDA File Offset: 0x001DBFDA
		public int daysInCampaign { get; private set; }

		// Token: 0x17000E7A RID: 3706
		// (get) Token: 0x060048A2 RID: 18594 RVA: 0x001DDDE3 File Offset: 0x001DBFE3
		// (set) Token: 0x060048A3 RID: 18595 RVA: 0x001DDDEB File Offset: 0x001DBFEB
		public int currentQuarterSinceStart { get; private set; }

		// Token: 0x17000E7B RID: 3707
		// (get) Token: 0x060048A4 RID: 18596 RVA: 0x001DDDF4 File Offset: 0x001DBFF4
		// (set) Token: 0x060048A5 RID: 18597 RVA: 0x001DDDFC File Offset: 0x001DBFFC
		public string masterMetaTemplateName { get; private set; }

		// Token: 0x17000E7C RID: 3708
		// (get) Token: 0x060048A6 RID: 18598 RVA: 0x001DDE05 File Offset: 0x001DC005
		// (set) Token: 0x060048A7 RID: 18599 RVA: 0x001DDE0D File Offset: 0x001DC00D
		public string scenarioMetaTemplateName { get; private set; }

		// Token: 0x17000E7D RID: 3709
		// (get) Token: 0x060048A8 RID: 18600 RVA: 0x001DDE16 File Offset: 0x001DC016
		public TIMetaTemplate masterMetaTemplate
		{
			get
			{
				return TemplateManager.Find<TIMetaTemplate>(this.masterMetaTemplateName, false);
			}
		}

		// Token: 0x17000E7E RID: 3710
		// (get) Token: 0x060048A9 RID: 18601 RVA: 0x001DDE24 File Offset: 0x001DC024
		public TIMetaTemplate scenarioMetaTemplate
		{
			get
			{
				return TemplateManager.Find<TIMetaTemplate>(this.scenarioMetaTemplateName, false);
			}
		}

		// Token: 0x060048AA RID: 18602 RVA: 0x001DDE34 File Offset: 0x001DC034
		public override void InitWithTemplate(TIDataTemplate template)
		{
			TIStartTimeTemplate tistartTimeTemplate = template as TIStartTimeTemplate;
			if (tistartTimeTemplate == null)
			{
				return;
			}
			this.templateName = tistartTimeTemplate.dataName;
			this.currentDateTime = new TIDateTime();
			this.currentDateTime.SetTime(tistartTimeTemplate.year, tistartTimeTemplate.month, tistartTimeTemplate.day, tistartTimeTemplate.hour, tistartTimeTemplate.minute, tistartTimeTemplate.second, 0);
			base.InitWithTemplate(template);
		}

		// Token: 0x060048AB RID: 18603 RVA: 0x001DDE9A File Offset: 0x001DC09A
		public void SetMasterMetaTemplate(string templateName, string scenarioTemplateName)
		{
			this.masterMetaTemplateName = templateName;
			this.scenarioMetaTemplateName = scenarioTemplateName;
		}

		// Token: 0x060048AC RID: 18604 RVA: 0x001DDEAA File Offset: 0x001DC0AA
		public void UpdateCurrentDateTime(double seconds)
		{
			this.currentDateTime.AddSeconds(seconds);
		}

		// Token: 0x060048AD RID: 18605 RVA: 0x001DDEB8 File Offset: 0x001DC0B8
		public void SetCurrentDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond)
		{
			this.currentDateTime.SetTime(year, month, day, hour, minute, second, millisecond);
		}

		// Token: 0x060048AE RID: 18606 RVA: 0x001DDED0 File Offset: 0x001DC0D0
		public TIDateTime Time_Now()
		{
			return new TIDateTime(this.currentDateTime);
		}

		// Token: 0x060048AF RID: 18607 RVA: 0x001DDEE0 File Offset: 0x001DC0E0
		public static TIDateTime Now()
		{
			TITimeState titimeState = GameStateManager.Time();
			if (titimeState == null)
			{
				return null;
			}
			return new TIDateTime(titimeState.currentDateTime);
		}

		// Token: 0x060048B0 RID: 18608 RVA: 0x001DDF09 File Offset: 0x001DC109
		public DateTime Time_SystemNow()
		{
			return new TIDateTime(this.currentDateTime).ExportTime();
		}

		// Token: 0x060048B1 RID: 18609 RVA: 0x001DDF1B File Offset: 0x001DC11B
		public static DateTime SystemNow()
		{
			return new TIDateTime(GameStateManager.Time().currentDateTime).ExportTime();
		}

		// Token: 0x060048B2 RID: 18610 RVA: 0x001DDF31 File Offset: 0x001DC131
		public void AddDayToCampaign()
		{
			this.daysInCampaign++;
		}

		// Token: 0x060048B3 RID: 18611 RVA: 0x001DDF41 File Offset: 0x001DC141
		public void AddQuarterToCampaign()
		{
			this.currentQuarterSinceStart++;
		}

		// Token: 0x060048B4 RID: 18612 RVA: 0x001DDF51 File Offset: 0x001DC151
		public static int CampaignDuration_days()
		{
			return GameStateManager.Time().daysInCampaign;
		}

		// Token: 0x060048B5 RID: 18613 RVA: 0x001DDF5D File Offset: 0x001DC15D
		public static int CurrentQuarter()
		{
			return GameStateManager.Time().currentQuarterSinceStart;
		}

		// Token: 0x060048B6 RID: 18614 RVA: 0x001DDF69 File Offset: 0x001DC169
		public static int CampaignDuration_CompleteMonths()
		{
			return (int)Math.Truncate((double)((float)GameStateManager.Time().daysInCampaign / 30.436874f));
		}

		// Token: 0x060048B7 RID: 18615 RVA: 0x001DDF83 File Offset: 0x001DC183
		public static float CampaignDuration_months_Exact()
		{
			return (float)GameStateManager.Time().daysInCampaign / 30.436874f;
		}

		// Token: 0x060048B8 RID: 18616 RVA: 0x001DDF96 File Offset: 0x001DC196
		public static int CampaignDuration_CompleteYears()
		{
			return (int)Math.Truncate((double)((float)GameStateManager.Time().daysInCampaign / 365.2422f));
		}

		// Token: 0x060048B9 RID: 18617 RVA: 0x001DDFB0 File Offset: 0x001DC1B0
		public static float CampaignDuration_years_Exact()
		{
			return (float)GameStateManager.Time().daysInCampaign / 365.2422f;
		}

		// Token: 0x040029D7 RID: 10711
		[SerializeField]
		private TIDateTime currentDateTime;
	}
}
