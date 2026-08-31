using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E3 RID: 2531
	public class AccelerationConstraints
	{
		// Token: 0x1700107B RID: 4219
		// (get) Token: 0x06005FA4 RID: 24484 RVA: 0x002D4A24 File Offset: 0x002D2C24
		public float LinearAcceleration
		{
			get
			{
				return this._linearAcceleration * this._effectivenessModifier;
			}
		}

		// Token: 0x1700107C RID: 4220
		// (get) Token: 0x06005FA5 RID: 24485 RVA: 0x002D4A33 File Offset: 0x002D2C33
		public float CruiseLinearAcceleration
		{
			get
			{
				return this._cruiseLinearAcceleration * this._effectivenessModifier;
			}
		}

		// Token: 0x1700107D RID: 4221
		// (get) Token: 0x06005FA6 RID: 24486 RVA: 0x002D4A42 File Offset: 0x002D2C42
		public float AngularAcceleration
		{
			get
			{
				return this._angularAcceleration * this._effectivenessModifier;
			}
		}

		// Token: 0x1700107E RID: 4222
		// (get) Token: 0x06005FA7 RID: 24487 RVA: 0x002D4A51 File Offset: 0x002D2C51
		public float MaxAngularVelocity
		{
			get
			{
				return this._maxAngularVelocity * this._effectivenessModifier;
			}
		}

		// Token: 0x1700107F RID: 4223
		// (get) Token: 0x06005FA8 RID: 24488 RVA: 0x002D4A60 File Offset: 0x002D2C60
		// (set) Token: 0x06005FA9 RID: 24489 RVA: 0x002D4A68 File Offset: 0x002D2C68
		public float EffectivenessModifier
		{
			get
			{
				return this._effectivenessModifier;
			}
			set
			{
				this._effectivenessModifier = Mathf.Clamp01(value);
			}
		}

		// Token: 0x06005FAA RID: 24490 RVA: 0x002D4A76 File Offset: 0x002D2C76
		public AccelerationConstraints(float linearAcceleration, float cruiseAcceleration, float angularAcceleration, float maxAngularVelocity)
		{
			this._linearAcceleration = linearAcceleration;
			this._cruiseLinearAcceleration = cruiseAcceleration;
			this._angularAcceleration = angularAcceleration;
			this._maxAngularVelocity = maxAngularVelocity;
		}

		// Token: 0x06005FAB RID: 24491 RVA: 0x002D4AA6 File Offset: 0x002D2CA6
		public void UpdateAccelerationConstraits(float linearAcceleration, float cruiseAcceleration, float angularAcceleration, float maxAngularVelocity)
		{
			this._linearAcceleration = linearAcceleration;
			this._cruiseLinearAcceleration = cruiseAcceleration;
			this._angularAcceleration = angularAcceleration;
			this._maxAngularVelocity = maxAngularVelocity;
		}

		// Token: 0x040043F5 RID: 17397
		private float _linearAcceleration;

		// Token: 0x040043F6 RID: 17398
		private float _cruiseLinearAcceleration;

		// Token: 0x040043F7 RID: 17399
		private float _angularAcceleration;

		// Token: 0x040043F8 RID: 17400
		private float _maxAngularVelocity;

		// Token: 0x040043F9 RID: 17401
		private float _effectivenessModifier = 1f;
	}
}
