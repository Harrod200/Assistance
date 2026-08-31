using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007D2 RID: 2002
	public class TransferResult
	{
		// Token: 0x060047F2 RID: 18418 RVA: 0x001DC1EF File Offset: 0x001DA3EF
		public TransferResult(TransferResult.Outcome result, double value = 0.0, double value2 = 0.0)
		{
			this.Result = result;
			this.Value = value;
			this.Value2 = value2;
		}

		// Token: 0x060047F3 RID: 18419 RVA: 0x001DC20C File Offset: 0x001DA40C
		public static TransferResult Best(TransferResult a, TransferResult b)
		{
			if (b == null)
			{
				return a;
			}
			if (a == null)
			{
				return b;
			}
			if (a.Result == TransferResult.Outcome.Success)
			{
				return a;
			}
			if (b.Result == TransferResult.Outcome.Success)
			{
				return b;
			}
			double num;
			bool flag = a.TryGetMinimumDVneeded_mps(out num);
			double num2;
			bool flag2 = b.TryGetMinimumDVneeded_mps(out num2);
			if (flag && flag2)
			{
				if (num < num2)
				{
					return a;
				}
				return b;
			}
			else
			{
				if (flag)
				{
					return a;
				}
				if (flag2)
				{
					return b;
				}
				double num3;
				bool flag3 = a.TryGetMinimumAccelerationNeeded(out num3, 1.0);
				double num4;
				bool flag4 = b.TryGetMinimumAccelerationNeeded(out num4, 1.0);
				if (flag3 && flag4)
				{
					if (num3 < num4)
					{
						return a;
					}
					return b;
				}
				else
				{
					if (flag3)
					{
						return a;
					}
					if (flag4)
					{
						return b;
					}
					if (a.Result == TransferResult.Outcome.Fail_CodePathNotImplemented)
					{
						return b;
					}
					return a;
				}
			}
		}

		// Token: 0x060047F4 RID: 18420 RVA: 0x001DC2B0 File Offset: 0x001DA4B0
		public bool WasBug()
		{
			return this.Result == TransferResult.Outcome.Fail_CodePathNotImplemented || this.Result == TransferResult.Outcome.Fail_ArrivalBeforeLaunch || this.Result == TransferResult.Outcome.Fail_BurnNaN;
		}

		// Token: 0x060047F5 RID: 18421 RVA: 0x001DC2D1 File Offset: 0x001DA4D1
		public bool TryGetMinimumDVneeded_mps(out double minimumDV_mps)
		{
			if (this.Result == TransferResult.Outcome.Fail_InsufficientDV)
			{
				minimumDV_mps = this.Value;
				return true;
			}
			minimumDV_mps = 0.0;
			return false;
		}

		// Token: 0x060047F6 RID: 18422 RVA: 0x001DC2F4 File Offset: 0x001DA4F4
		public bool TryGetMinimumDVneeded_kps(out double minimumDV_kps)
		{
			double num;
			bool flag = this.TryGetMinimumDVneeded_mps(out num);
			minimumDV_kps = num / 1000.0;
			return flag;
		}

		// Token: 0x060047F7 RID: 18423 RVA: 0x001DC318 File Offset: 0x001DA518
		public bool TryGetMinimumAccelerationNeeded(out double minimumAcceleration_mps2, double fleetAcceleration_mps2)
		{
			if (this.Result == TransferResult.Outcome.Fail_BurnLongerThanTransfer)
			{
				double value = this.Value;
				double value2 = this.Value2;
				double num = value / value2;
				minimumAcceleration_mps2 = fleetAcceleration_mps2 * num;
				return true;
			}
			if (this.Result == TransferResult.Outcome.Fail_LaunchInPast)
			{
				double value3 = this.Value;
				double value4 = this.Value2;
				double num2 = 1.0 - value3 / value4;
				minimumAcceleration_mps2 = fleetAcceleration_mps2 / num2;
				return true;
			}
			if (this.Result == TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit)
			{
				double value5 = this.Value;
				double value6 = this.Value2;
				double num3 = value5 * 2.0 / value6;
				minimumAcceleration_mps2 = fleetAcceleration_mps2 * num3;
				return true;
			}
			if (this.Result == TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust)
			{
				double value7 = this.Value;
				double value8 = this.Value2;
				minimumAcceleration_mps2 = value7 / (2.0 * value8 * value8);
				return true;
			}
			if (this.Result == TransferResult.Outcome.Fail_InsufficientAcceleration)
			{
				minimumAcceleration_mps2 = this.Value;
				return true;
			}
			if (this.Result == TransferResult.Outcome.Fail_CoastPhaseEndsBeforeItStarts)
			{
				double value9 = this.Value;
				double num4 = this.Value2 / value9;
				minimumAcceleration_mps2 = fleetAcceleration_mps2 * num4;
				return true;
			}
			minimumAcceleration_mps2 = 0.0;
			return false;
		}

		// Token: 0x060047F8 RID: 18424 RVA: 0x001DC418 File Offset: 0x001DA618
		public override string ToString()
		{
			TransferResult.Outcome result = this.Result;
			if (result <= TransferResult.Outcome.Fail_ExceedsMaxDuration)
			{
				if (result == TransferResult.Outcome.Success)
				{
					return Loc.T("UI.TransferResult.Success");
				}
				if (result == TransferResult.Outcome.Fail_InsufficientDV)
				{
					return Loc.T("UI.TransferResult.Fail_InsufficientDV", new object[] { (this.Value / 1000.0).ToString("N1") });
				}
				if (result == TransferResult.Outcome.Fail_ExceedsMaxDuration)
				{
					return Loc.T("UI.TransferResult.Fail_ExceedsMaxDuration", new object[] { (this.Value / 60.0 / 60.0 / 24.0 / 365.0).ToString("N1") });
				}
			}
			else
			{
				if (result == TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit)
				{
					return Loc.T("UI.TransferResult.Fail_BurnLongerThanHalfOrbit");
				}
				if (result == TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust)
				{
					return Loc.T("UI.TransferResult.Fail_AttemptedFleetInterceptInMicrothrust");
				}
				if (result == TransferResult.Outcome.Fail_AttemptedFleetInterceptAfterArrivalAtAsset)
				{
					return Loc.T("UI.TransferResult.Fail_AttemptedFleetInterceptAfterArrivalAtAsset", new object[] { (this.Value / 60.0 / 60.0 / 24.0 / 30.0).ToString("N1") });
				}
			}
			return Loc.T("UI.TransferResult.Fail_Generic", new object[]
			{
				this.Result.ToString(),
				this.Value.ToString("N2"),
				this.Value2.ToString("N2")
			});
		}

		// Token: 0x040029A6 RID: 10662
		public TransferResult.Outcome Result;

		// Token: 0x040029A7 RID: 10663
		public double Value;

		// Token: 0x040029A8 RID: 10664
		public double Value2;

		// Token: 0x02000F94 RID: 3988
		public enum Outcome
		{
			// Token: 0x04005EBD RID: 24253
			Success,
			// Token: 0x04005EBE RID: 24254
			Fail_InsufficientDV,
			// Token: 0x04005EBF RID: 24255
			Fail_ArrivalBeforeLaunch,
			// Token: 0x04005EC0 RID: 24256
			Fail_LaunchInPast,
			// Token: 0x04005EC1 RID: 24257
			Fail_CoastPhaseEndsBeforeItStarts,
			// Token: 0x04005EC2 RID: 24258
			Fail_Parabolic,
			// Token: 0x04005EC3 RID: 24259
			Fail_Hyperbolic,
			// Token: 0x04005EC4 RID: 24260
			Fail_HyperbolicMicrothrust,
			// Token: 0x04005EC5 RID: 24261
			Fail_InsufficientAcceleration,
			// Token: 0x04005EC6 RID: 24262
			Fail_OrbitPeriod,
			// Token: 0x04005EC7 RID: 24263
			Fail_ExceedsMaxDuration,
			// Token: 0x04005EC8 RID: 24264
			Fail_BurnLongerThanTransfer,
			// Token: 0x04005EC9 RID: 24265
			Fail_BurnLongerThanHalfOrbit,
			// Token: 0x04005ECA RID: 24266
			Fail_BurnNaN,
			// Token: 0x04005ECB RID: 24267
			Fail_WouldCollideWithBody,
			// Token: 0x04005ECC RID: 24268
			Fail_WouldExceedHillRadius,
			// Token: 0x04005ECD RID: 24269
			Fail_AttemptedFleetInterceptInMicrothrust,
			// Token: 0x04005ECE RID: 24270
			Fail_AttemptedFleetInterceptAfterArrivalAtAsset,
			// Token: 0x04005ECF RID: 24271
			Fail_AttemptedFleetInterceptThatWouldCauseTargetingLoop,
			// Token: 0x04005ED0 RID: 24272
			Fail_CodePathNotImplemented
		}
	}
}
