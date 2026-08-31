using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000371 RID: 881
public class TINavigableTemplate : TINaturalSpaceObjectTemplate
{
	// Token: 0x170001D8 RID: 472
	// (get) Token: 0x06000FE5 RID: 4069 RVA: 0x00052AB2 File Offset: 0x00050CB2
	public override float ModelScale
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x170001D9 RID: 473
	// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x00052AB9 File Offset: 0x00050CB9
	public override string ModelResource
	{
		get
		{
			return "planets/EmptyContainer";
		}
	}

	// Token: 0x170001DA RID: 474
	// (get) Token: 0x06000FE7 RID: 4071 RVA: 0x00052AC0 File Offset: 0x00050CC0
	private TISpaceBodyState relatedObjectState
	{
		get
		{
			if (this._relatedObjectState == null)
			{
				this._relatedObjectState = GameStateManager.FindByTemplate<TISpaceBodyState>(this.relatedObject, true);
			}
			return this._relatedObjectState;
		}
	}

	// Token: 0x06000FE8 RID: 4072 RVA: 0x00052AE8 File Offset: 0x00050CE8
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TILagrangePointState>();
		}
		return tigameState;
	}

	// Token: 0x170001DB RID: 475
	// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x00052B0C File Offset: 0x00050D0C
	public override double SemiMajorAxis_m
	{
		get
		{
			switch (this.lagrangeValue)
			{
			case LagrangeValue.L1:
				return this.relatedObjectState.semiMajorAxis_m - this.relatedObjectState.hillRadius_m;
			case LagrangeValue.L2:
				return this.relatedObjectState.semiMajorAxis_m + this.relatedObjectState.hillRadius_m;
			}
			return this.relatedObjectState.semiMajorAxis_m;
		}
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x06000FEA RID: 4074 RVA: 0x00052B79 File Offset: 0x00050D79
	public override double Eccentricity
	{
		get
		{
			return this.relatedObjectState.ecc;
		}
	}

	// Token: 0x170001DD RID: 477
	// (get) Token: 0x06000FEB RID: 4075 RVA: 0x00052B86 File Offset: 0x00050D86
	public override double Inclination_Rad
	{
		get
		{
			return this.relatedObjectState.inclination_Rad;
		}
	}

	// Token: 0x170001DE RID: 478
	// (get) Token: 0x06000FEC RID: 4076 RVA: 0x00052B93 File Offset: 0x00050D93
	public override double LongitudeAscendingNode_Rad
	{
		get
		{
			return this.relatedObjectState.longAscendingNode_Rad;
		}
	}

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x06000FED RID: 4077 RVA: 0x00052BA0 File Offset: 0x00050DA0
	public override double ArgumentPeriapsis_Rad
	{
		get
		{
			double num = this.relatedObjectState.argPeriapsis_Rad;
			switch (this.lagrangeValue)
			{
			case LagrangeValue.L3:
				num += 3.141592653589793;
				break;
			case LagrangeValue.L4:
				num += 1.0471975511965976;
				break;
			case LagrangeValue.L5:
				num -= 1.0471975511965976;
				break;
			}
			return Mathd.ClampRadiansTwoPI(num);
		}
	}

	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x06000FEE RID: 4078 RVA: 0x00052C04 File Offset: 0x00050E04
	public override double MeanAnomalyAtEpoch_Rad
	{
		get
		{
			return this.relatedObjectState.meanAnomalyAtEpoch_Rad;
		}
	}

	// Token: 0x170001E1 RID: 481
	// (get) Token: 0x06000FEF RID: 4079 RVA: 0x00052C11 File Offset: 0x00050E11
	public override double Epoch_floatJYears
	{
		get
		{
			return this.relatedObjectState.epoch_JYears;
		}
	}

	// Token: 0x0400103D RID: 4157
	public string relatedObject;

	// Token: 0x0400103E RID: 4158
	public LagrangeValue lagrangeValue;

	// Token: 0x0400103F RID: 4159
	public TINavigablePosition positionCalculator;

	// Token: 0x04001040 RID: 4160
	private TISpaceBodyState _relatedObjectState;
}
