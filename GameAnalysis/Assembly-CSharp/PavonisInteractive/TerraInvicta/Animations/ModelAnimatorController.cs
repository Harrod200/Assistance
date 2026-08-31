using System;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Animations
{
	// Token: 0x02000AAD RID: 2733
	public class ModelAnimatorController : MonoBehaviour
	{
		// Token: 0x1700111C RID: 4380
		// (get) Token: 0x060065B5 RID: 26037 RVA: 0x002FDC00 File Offset: 0x002FBE00
		// (set) Token: 0x060065B6 RID: 26038 RVA: 0x002FDC08 File Offset: 0x002FBE08
		public TerrestrialUnitModel unitModel { get; private set; }

		// Token: 0x1700111D RID: 4381
		// (get) Token: 0x060065B7 RID: 26039 RVA: 0x002FDC11 File Offset: 0x002FBE11
		private bool canAnimate
		{
			get
			{
				return this.unitModel != null && this.unitModel.Animator != null;
			}
		}

		// Token: 0x1700111E RID: 4382
		// (get) Token: 0x060065B8 RID: 26040 RVA: 0x002FDC34 File Offset: 0x002FBE34
		private string loadedModelName
		{
			get
			{
				if (!(this.unitModel != null))
				{
					return string.Empty;
				}
				return this.unitModel.name;
			}
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x002FDC58 File Offset: 0x002FBE58
		private void OnEnable()
		{
			bool flag;
			if (this.canAnimate)
			{
				flag = this.unitModel.Animator.parameters.Any<AnimatorControllerParameter>((AnimatorControllerParameter x) => x.name == "Idle");
			}
			else
			{
				flag = false;
			}
			this.canIdle = flag;
			bool flag2;
			if (this.canAnimate)
			{
				flag2 = this.unitModel.Animator.parameters.Any<AnimatorControllerParameter>((AnimatorControllerParameter x) => x.name == "Damaged");
			}
			else
			{
				flag2 = false;
			}
			this.canDamaged = flag2;
			this.PlayAnimationState(this.currentState);
		}

		// Token: 0x1700111F RID: 4383
		// (get) Token: 0x060065BA RID: 26042 RVA: 0x002FDCFB File Offset: 0x002FBEFB
		public ModelAnimatorController.AnimationState GetAnimationState
		{
			get
			{
				return this.currentState;
			}
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x002FDD04 File Offset: 0x002FBF04
		public void PlayAnimationState(ModelAnimatorController.AnimationState state)
		{
			switch (state)
			{
			case ModelAnimatorController.AnimationState.Idle:
				this.PlayIdle(true);
				return;
			case ModelAnimatorController.AnimationState.Move:
				this.PlayMove(true);
				return;
			case ModelAnimatorController.AnimationState.TurnLeft:
				this.PlayTurnLeft(true);
				return;
			case ModelAnimatorController.AnimationState.TurnRight:
				this.PlayTurnRight(true);
				return;
			case ModelAnimatorController.AnimationState.Destroyed:
				this.PlayDestroyed(true);
				return;
			case ModelAnimatorController.AnimationState.Damaged:
				this.PlayDamaged(true);
				return;
			case ModelAnimatorController.AnimationState.Attack:
				this.PlayAttack(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x002FDD6C File Offset: 0x002FBF6C
		public void UpdateAnimatorController(GameObject prefab)
		{
			if (this.loadedModelName != prefab.name)
			{
				if (this.unitModel != null)
				{
					global::UnityEngine.Object.Destroy(this.unitModel.gameObject);
				}
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(prefab, base.transform);
				this.unitModel = gameObject.GetComponent<TerrestrialUnitModel>();
				if (this.unitModel == null)
				{
					this.unitModel = gameObject.AddComponent<TerrestrialUnitModel>();
				}
			}
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x002FDDDD File Offset: 0x002FBFDD
		public void PlayIdle(bool force)
		{
			if (this.canIdle && base.gameObject.activeInHierarchy && (this.currentState > ModelAnimatorController.AnimationState.Idle || force))
			{
				this.unitModel.Animator.SetTrigger("Idle");
			}
			this.currentState = ModelAnimatorController.AnimationState.Idle;
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x002FDE20 File Offset: 0x002FC020
		public void PlayMove(bool force)
		{
			if (this.canAnimate && base.gameObject.activeInHierarchy && (this.currentState != ModelAnimatorController.AnimationState.Move || force))
			{
				this.unitModel.Animator.SetTrigger("Move");
			}
			this.currentState = ModelAnimatorController.AnimationState.Move;
		}

		// Token: 0x060065BF RID: 26047 RVA: 0x002FDE70 File Offset: 0x002FC070
		public void PlayDamaged(bool force)
		{
			if (this.canDamaged && base.gameObject.activeInHierarchy && (this.currentState != ModelAnimatorController.AnimationState.Damaged || force))
			{
				this.unitModel.Animator.SetTrigger("Damaged");
			}
			this.currentState = ModelAnimatorController.AnimationState.Damaged;
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x002FDEC0 File Offset: 0x002FC0C0
		public void PlayDestroyed(bool force)
		{
			if (this.canAnimate && base.gameObject.activeInHierarchy && (this.currentState != ModelAnimatorController.AnimationState.Destroyed || force))
			{
				this.unitModel.Animator.SetTrigger("Destroyed");
			}
			this.currentState = ModelAnimatorController.AnimationState.Destroyed;
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x002FDF0E File Offset: 0x002FC10E
		public void PlayTurnLeft(bool force)
		{
			if (this.canAnimate && base.gameObject.activeInHierarchy)
			{
				this.unitModel.Animator.SetTrigger("Turn Left");
			}
			this.currentState = ModelAnimatorController.AnimationState.TurnLeft;
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x002FDF41 File Offset: 0x002FC141
		public void PlayTurnRight(bool force)
		{
			if (this.canAnimate && base.gameObject.activeInHierarchy)
			{
				this.unitModel.Animator.SetTrigger("Turn Right");
			}
			this.currentState = ModelAnimatorController.AnimationState.TurnRight;
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x002FDF74 File Offset: 0x002FC174
		public void PlayAttack(bool force)
		{
			if (this.canAnimate && base.gameObject.activeInHierarchy && (this.currentState != ModelAnimatorController.AnimationState.Attack || force))
			{
				int integer = this.unitModel.Animator.GetInteger("AttackTypeCount");
				int num = global::UnityEngine.Random.Range(0, integer);
				switch (num)
				{
				case 0:
					this.unitModel.Animator.SetTrigger("Attack 1");
					break;
				case 1:
					this.unitModel.Animator.SetTrigger("Attack 2");
					break;
				case 2:
					this.unitModel.Animator.SetTrigger("Attack 3");
					break;
				default:
					Log.Warn("ModelAnimatorController.PlayAttack: No attack animation is implemented for index {0}", new object[] { num });
					break;
				}
			}
			this.currentState = ModelAnimatorController.AnimationState.Attack;
		}

		// Token: 0x04004804 RID: 18436
		private const string IDLE = "Idle";

		// Token: 0x04004805 RID: 18437
		private const string MOVE = "Move";

		// Token: 0x04004806 RID: 18438
		private const string TURN_LEFT = "Turn Left";

		// Token: 0x04004807 RID: 18439
		private const string TURN_RIGHT = "Turn Right";

		// Token: 0x04004808 RID: 18440
		private const string DESTROYED = "Destroyed";

		// Token: 0x04004809 RID: 18441
		private const string DAMAGED = "Damaged";

		// Token: 0x0400480A RID: 18442
		private const string ATTACK_1 = "Attack 1";

		// Token: 0x0400480B RID: 18443
		private const string ATTACK_2 = "Attack 2";

		// Token: 0x0400480C RID: 18444
		private const string ATTACK_3 = "Attack 3";

		// Token: 0x0400480D RID: 18445
		private const string ATTACK_TYPE_COUNT = "AttackTypeCount";

		// Token: 0x0400480F RID: 18447
		private bool canIdle;

		// Token: 0x04004810 RID: 18448
		private bool canDamaged;

		// Token: 0x04004811 RID: 18449
		private ModelAnimatorController.AnimationState currentState;

		// Token: 0x020013DA RID: 5082
		public enum AnimationState
		{
			// Token: 0x04007310 RID: 29456
			Idle,
			// Token: 0x04007311 RID: 29457
			Move,
			// Token: 0x04007312 RID: 29458
			TurnLeft,
			// Token: 0x04007313 RID: 29459
			TurnRight,
			// Token: 0x04007314 RID: 29460
			Destroyed,
			// Token: 0x04007315 RID: 29461
			Damaged,
			// Token: 0x04007316 RID: 29462
			Attack
		}
	}
}
