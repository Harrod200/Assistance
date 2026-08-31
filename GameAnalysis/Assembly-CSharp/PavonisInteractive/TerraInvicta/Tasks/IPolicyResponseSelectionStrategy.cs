using System;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200094C RID: 2380
	public interface IPolicyResponseSelectionStrategy
	{
		// Token: 0x06005ADA RID: 23258
		bool SelectPolicyReply(TINationState promptingNation, TINationState respondingNation, TIPolicyOptionWithConfirm policy);

		// Token: 0x06005ADB RID: 23259
		bool SelectPolicyReply(TINationState promptingNation, TINationState respondingNation, TIWarState war, TIPolicyOptionWithConfirm policy);

		// Token: 0x06005ADC RID: 23260
		bool SelectPolicyReply(TINationState promptingNation, TINationState respondingNation, TIPolicyOptionWithConfirm policy, TIRegionState region);
	}
}
