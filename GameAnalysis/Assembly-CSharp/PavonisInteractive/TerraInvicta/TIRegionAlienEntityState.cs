using System;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076F RID: 1903
	public abstract class TIRegionAlienEntityState : TIRegionEntityState
	{
		// Token: 0x170009F9 RID: 2553
		// (get) Token: 0x060039C5 RID: 14789 RVA: 0x00155BCE File Offset: 0x00153DCE
		public override string descriptor
		{
			get
			{
				return Loc.T("TIRegionAlienEntityState.BasicDescriptor");
			}
		}

		// Token: 0x170009FA RID: 2554
		// (get) Token: 0x060039C6 RID: 14790 RVA: 0x00155BDA File Offset: 0x00153DDA
		public override string description
		{
			get
			{
				return Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString());
			}
		}

		// Token: 0x060039C7 RID: 14791 RVA: 0x00155C00 File Offset: 0x00153E00
		public virtual bool VisibleToFaction(TIFactionState faction)
		{
			return this.Extant() && faction.GetIntel(this) > 0f;
		}

		// Token: 0x060039C8 RID: 14792 RVA: 0x00155C1A File Offset: 0x00153E1A
		public override string GetDisplayName(TIFactionState faction)
		{
			return Loc.T("TIRegionAlienEntityState.displayNameWithLocation", new object[]
			{
				this.displayName,
				base.region.displayName
			});
		}

		// Token: 0x170009FB RID: 2555
		// (get) Token: 0x060039C9 RID: 14793 RVA: 0x00155C43 File Offset: 0x00153E43
		public override bool isRegionAlienEntity
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009FC RID: 2556
		// (get) Token: 0x060039CA RID: 14794 RVA: 0x00155C46 File Offset: 0x00153E46
		public override TIFactionState ref_faction
		{
			get
			{
				return GameStateManager.AlienFaction();
			}
		}

		// Token: 0x170009FD RID: 2557
		// (get) Token: 0x060039CB RID: 14795 RVA: 0x00155C4D File Offset: 0x00153E4D
		public override TIRegionAlienEntityState ref_regionAlienEntity
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060039CC RID: 14796 RVA: 0x00155C50 File Offset: 0x00153E50
		public override bool Initialize()
		{
			this.displayName = Loc.T(new StringBuilder(base.GetType().Name).Append(".displayName").ToString());
			return base.Initialize();
		}
	}
}
