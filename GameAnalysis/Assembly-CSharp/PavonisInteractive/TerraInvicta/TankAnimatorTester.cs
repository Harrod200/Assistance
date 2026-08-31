using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200082D RID: 2093
	public class TankAnimatorTester : MonoBehaviour
	{
		// Token: 0x06004B0A RID: 19210 RVA: 0x001F519F File Offset: 0x001F339F
		public void GetAllTankAnimatorsInScene()
		{
			this.tankAnimators = new List<Animator>();
			this.tankAnimators.AddRange(global::UnityEngine.Object.FindObjectsOfType<Animator>());
		}

		// Token: 0x06004B0B RID: 19211 RVA: 0x001F51BC File Offset: 0x001F33BC
		public void ResetAllTriggers()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].ResetTrigger("Idle");
				this.tankAnimators[i].ResetTrigger("Move");
				this.tankAnimators[i].ResetTrigger("Turn Left");
				this.tankAnimators[i].ResetTrigger("Turn Right");
				this.tankAnimators[i].ResetTrigger("Destroyed");
				this.tankAnimators[i].ResetTrigger("Damaged");
				this.tankAnimators[i].ResetTrigger("Attack 1");
			}
		}

		// Token: 0x06004B0C RID: 19212 RVA: 0x001F5280 File Offset: 0x001F3480
		public void Idle()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Idle");
			}
		}

		// Token: 0x06004B0D RID: 19213 RVA: 0x001F52BC File Offset: 0x001F34BC
		public void Move()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Move");
			}
		}

		// Token: 0x06004B0E RID: 19214 RVA: 0x001F52F8 File Offset: 0x001F34F8
		public void TurnLeft()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Turn Left");
			}
		}

		// Token: 0x06004B0F RID: 19215 RVA: 0x001F5334 File Offset: 0x001F3534
		public void TurnRight()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Turn Right");
			}
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x001F5370 File Offset: 0x001F3570
		public void Destroyed()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Destroyed");
			}
		}

		// Token: 0x06004B11 RID: 19217 RVA: 0x001F53AC File Offset: 0x001F35AC
		public void Damaged()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Damaged");
			}
		}

		// Token: 0x06004B12 RID: 19218 RVA: 0x001F53E8 File Offset: 0x001F35E8
		public void Attack()
		{
			for (int i = 0; i < this.tankAnimators.Count; i++)
			{
				this.tankAnimators[i].SetTrigger("Attack 1");
			}
		}

		// Token: 0x04002BAF RID: 11183
		[SerializeField]
		private List<Animator> tankAnimators;

		// Token: 0x04002BB0 RID: 11184
		private const string IDLE = "Idle";

		// Token: 0x04002BB1 RID: 11185
		private const string MOVE = "Move";

		// Token: 0x04002BB2 RID: 11186
		private const string TURN_LEFT = "Turn Left";

		// Token: 0x04002BB3 RID: 11187
		private const string TURN_RIGHT = "Turn Right";

		// Token: 0x04002BB4 RID: 11188
		private const string DESTROYED = "Destroyed";

		// Token: 0x04002BB5 RID: 11189
		private const string DAMAGED = "Damaged";

		// Token: 0x04002BB6 RID: 11190
		private const string ATTACK_1 = "Attack 1";
	}
}
