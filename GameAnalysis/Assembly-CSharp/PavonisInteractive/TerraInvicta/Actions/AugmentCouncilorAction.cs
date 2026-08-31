using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4F RID: 2639
	public class AugmentCouncilorAction : PlayerAction
	{
		// Token: 0x060064EB RID: 25835 RVA: 0x002FA7F3 File Offset: 0x002F89F3
		public AugmentCouncilorAction(TICouncilorState councilor, CouncilorAugmentationOption augmentationOption)
		{
			this.councilorID = councilor.ID;
			this.augmentationOption = augmentationOption;
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x002FA80E File Offset: 0x002F8A0E
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).ApplyAugmentation(this.augmentationOption);
		}

		// Token: 0x04004708 RID: 18184
		private GameStateID councilorID;

		// Token: 0x04004709 RID: 18185
		private CouncilorAugmentationOption augmentationOption;
	}
}
