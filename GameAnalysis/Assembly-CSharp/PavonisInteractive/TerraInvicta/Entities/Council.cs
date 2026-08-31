using System;
using System.Collections.Generic;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Entities
{
	// Token: 0x02000964 RID: 2404
	public class Council
	{
		// Token: 0x17000F86 RID: 3974
		// (get) Token: 0x06005BA3 RID: 23459 RVA: 0x002BF2EF File Offset: 0x002BD4EF
		public TIFactionState state { get; }

		// Token: 0x17000F87 RID: 3975
		// (get) Token: 0x06005BA4 RID: 23460 RVA: 0x002BF2F7 File Offset: 0x002BD4F7
		public Player player { get; }

		// Token: 0x17000F88 RID: 3976
		// (get) Token: 0x06005BA5 RID: 23461 RVA: 0x002BF300 File Offset: 0x002BD500
		public IReadOnlyCollection<Councilor> councilors
		{
			get
			{
				if (this.state.councilors.Count != this._councilors.Count)
				{
					this.ReloadCouncilors();
				}
				foreach (Councilor councilor in this._councilors)
				{
					if (!this.state.councilors.Contains(councilor.state))
					{
						this.ReloadCouncilors();
						break;
					}
				}
				return this._councilors;
			}
		}

		// Token: 0x06005BA6 RID: 23462 RVA: 0x002BF398 File Offset: 0x002BD598
		public Council(TIFactionState councilState, Councilor.Factory councilorFactory)
		{
			this.state = councilState;
			this.councilorFactory = councilorFactory;
			this.ReloadCouncilors();
		}

		// Token: 0x06005BA7 RID: 23463 RVA: 0x002BF3B4 File Offset: 0x002BD5B4
		private void ReloadCouncilors()
		{
			List<Councilor> councilors = new List<Councilor>();
			this._councilors = councilors;
			this.state.councilors.ForEach(delegate(TICouncilorState c)
			{
				councilors.Add(this.councilorFactory.Create(c));
			});
		}

		// Token: 0x0400419B RID: 16795
		private List<Councilor> _councilors;

		// Token: 0x0400419C RID: 16796
		private readonly Councilor.Factory councilorFactory;

		// Token: 0x0200132E RID: 4910
		public class Factory : Factory<TIFactionState, Council>
		{
		}
	}
}
