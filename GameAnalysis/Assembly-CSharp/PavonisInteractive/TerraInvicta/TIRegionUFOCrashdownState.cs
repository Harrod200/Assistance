using System;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000778 RID: 1912
	public class TIRegionUFOCrashdownState : TIRegionAlienEntityState
	{
		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06003B02 RID: 15106 RVA: 0x0015C692 File Offset: 0x0015A892
		// (set) Token: 0x06003B03 RID: 15107 RVA: 0x0015C69A File Offset: 0x0015A89A
		public bool crashdownPresent { get; private set; }

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06003B04 RID: 15108 RVA: 0x0015C6A3 File Offset: 0x0015A8A3
		// (set) Token: 0x06003B05 RID: 15109 RVA: 0x0015C6AB File Offset: 0x0015A8AB
		public TIDateTime crashdownTime { get; private set; }

		// Token: 0x06003B06 RID: 15110 RVA: 0x0015C6B4 File Offset: 0x0015A8B4
		public override bool Extant()
		{
			return this.crashdownPresent;
		}

		// Token: 0x06003B07 RID: 15111 RVA: 0x0015C6BC File Offset: 0x0015A8BC
		public override string GetIconResourcePath(TIFactionState faction)
		{
			return TemplateManager.global.pathGeoscapeCrashdown;
		}

		// Token: 0x06003B08 RID: 15112 RVA: 0x0015C6C8 File Offset: 0x0015A8C8
		public override string GetIllustrationPath(TIFactionState faction)
		{
			return TemplateManager.global.illus_crashedUFO;
		}

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06003B09 RID: 15113 RVA: 0x0015C6D4 File Offset: 0x0015A8D4
		public override TIRegionUFOCrashdownState ref_UFOCrashdown
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06003B0A RID: 15114 RVA: 0x0015C6D7 File Offset: 0x0015A8D7
		public override bool isRegionUFOCrashdown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003B0B RID: 15115 RVA: 0x0015C6DA File Offset: 0x0015A8DA
		public override void PostGlobalGameStateCreateInit_2()
		{
			base.PostGlobalGameStateCreateInit_2();
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
		}

		// Token: 0x06003B0C RID: 15116 RVA: 0x0015C6F2 File Offset: 0x0015A8F2
		public void InitWithRegionState(TIRegionState region)
		{
			if (!this.gameStateSubjectCreated)
			{
				this.templateName = region.template.dataName;
				base.region = region;
				this.gameStateSubjectCreated = true;
			}
		}

		// Token: 0x06003B0D RID: 15117 RVA: 0x0015C71B File Offset: 0x0015A91B
		public void SetAsInitialCrashdownRegion()
		{
			this.crashdownPresent = true;
		}

		// Token: 0x06003B0E RID: 15118 RVA: 0x0015C724 File Offset: 0x0015A924
		public void TriggerCrashdown(bool firstCrashdown)
		{
			this.crashdownPresent = true;
			this.crashdownTime = new TIDateTime(this.gameTime.currentTime);
			foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
			{
				tifactionState.SetIntel(this, 1f, null, false);
			}
			if (AIEvaluators.ShouldAliensGoLoud())
			{
				base.region.xenoforming.ChangeXenoformingLevel(0.001f);
			}
			GameControl.eventManager.TriggerEvent(new AlienCrashdownInRegion(base.region), null, new object[] { base.region });
			TINotificationQueueState.LogAlienCrashdown(base.region, firstCrashdown);
		}

		// Token: 0x06003B0F RID: 15119 RVA: 0x0015C7E0 File Offset: 0x0015A9E0
		public void ExpireCrashdownForFaction(TIFactionState faction)
		{
			faction.ExpireIntel(this, true);
		}

		// Token: 0x06003B10 RID: 15120 RVA: 0x0015C7EC File Offset: 0x0015A9EC
		public void ExpireUFOCrashdownForAll()
		{
			this.crashdownPresent = false;
			foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
			{
				this.ExpireCrashdownForFaction(tifactionState);
			}
			GameControl.eventManager.TriggerEvent(new AlienRegionEntityUpdated(this, base.region), null, new object[] { base.region });
		}

		// Token: 0x040025B3 RID: 9651
		private GameTimeManager gameTime;
	}
}
