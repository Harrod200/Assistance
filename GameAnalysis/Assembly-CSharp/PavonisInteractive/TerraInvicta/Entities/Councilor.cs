using System;
using System.Collections.Generic;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Entities
{
	// Token: 0x02000965 RID: 2405
	public class Councilor
	{
		// Token: 0x17000F89 RID: 3977
		// (get) Token: 0x06005BA8 RID: 23464 RVA: 0x002BF401 File Offset: 0x002BD601
		public TICouncilorState state { get; }

		// Token: 0x06005BA9 RID: 23465 RVA: 0x002BF409 File Offset: 0x002BD609
		public Councilor(TICouncilorState councilorState)
		{
			this.state = councilorState;
		}

		// Token: 0x17000F8A RID: 3978
		// (get) Token: 0x06005BAA RID: 23466 RVA: 0x002BF418 File Offset: 0x002BD618
		public bool HasMission
		{
			get
			{
				return this.state.HasMission;
			}
		}

		// Token: 0x06005BAB RID: 23467 RVA: 0x002BF425 File Offset: 0x002BD625
		public IReadOnlyList<TIMissionTemplate> GetPossibleMissionList(bool filterForCouncilorConditions = false)
		{
			return this.state.GetPossibleMissionList(filterForCouncilorConditions, false, true, null, false);
		}

		// Token: 0x02001330 RID: 4912
		public class Factory : Factory<TICouncilorState, Councilor>
		{
		}
	}
}
