using System;
using UnityEngine;

// Token: 0x02000420 RID: 1056
[RequireComponent(typeof(CharacterController))]
public class SimpleCharacterMotor : MonoBehaviour
{
	// Token: 0x0600160C RID: 5644 RVA: 0x000702FC File Offset: 0x0006E4FC
	private void Awake()
	{
		this.controller = base.GetComponent<CharacterController>();
		Cursor.lockState = this.cursorLockMode;
		Cursor.visible = this.cursorVisible;
		this.targetRotation = (this.targetPivotRotation = Quaternion.identity);
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x0007033F File Offset: 0x0006E53F
	private void Update()
	{
		this.UpdateTranslation();
		this.UpdateLookRotation();
	}

	// Token: 0x0600160E RID: 5646 RVA: 0x00070350 File Offset: 0x0006E550
	private void UpdateLookRotation()
	{
		float num = Input.GetAxis("Mouse Y");
		float axis = Input.GetAxis("Mouse X");
		num *= (float)(this.invertY ? (-1) : 1);
		this.targetRotation = base.transform.localRotation * Quaternion.AngleAxis(axis * this.lookSpeed * Time.deltaTime, Vector3.up);
		this.targetPivotRotation = this.cameraPivot.localRotation * Quaternion.AngleAxis(num * this.lookSpeed * Time.deltaTime, Vector3.right);
		base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, this.targetRotation, Time.deltaTime * 15f);
		this.cameraPivot.localRotation = Quaternion.Slerp(this.cameraPivot.localRotation, this.targetPivotRotation, Time.deltaTime * 15f);
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x00070438 File Offset: 0x0006E638
	private void UpdateTranslation()
	{
		if (this.controller.isGrounded)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool key = Input.GetKey(KeyCode.LeftShift);
			Vector3 vector = new Vector3(axis, 0f, axis2);
			this.speed = (key ? this.runSpeed : this.walkSpeed);
			this.movement = base.transform.TransformDirection(vector * this.speed);
		}
		else
		{
			this.movement.y = this.movement.y - this.gravity * Time.deltaTime;
		}
		this.finalMovement = Vector3.Lerp(this.finalMovement, this.movement, Time.deltaTime * this.movementAcceleration);
		this.controller.Move(this.finalMovement * Time.deltaTime);
	}

	// Token: 0x0400140C RID: 5132
	public CursorLockMode cursorLockMode = CursorLockMode.Locked;

	// Token: 0x0400140D RID: 5133
	public bool cursorVisible;

	// Token: 0x0400140E RID: 5134
	[Header("Movement")]
	public float walkSpeed = 2f;

	// Token: 0x0400140F RID: 5135
	public float runSpeed = 4f;

	// Token: 0x04001410 RID: 5136
	public float gravity = 9.8f;

	// Token: 0x04001411 RID: 5137
	[Space]
	[Header("Look")]
	public Transform cameraPivot;

	// Token: 0x04001412 RID: 5138
	public float lookSpeed = 45f;

	// Token: 0x04001413 RID: 5139
	public bool invertY = true;

	// Token: 0x04001414 RID: 5140
	[Space]
	[Header("Smoothing")]
	public float movementAcceleration = 1f;

	// Token: 0x04001415 RID: 5141
	private CharacterController controller;

	// Token: 0x04001416 RID: 5142
	private Vector3 movement;

	// Token: 0x04001417 RID: 5143
	private Vector3 finalMovement;

	// Token: 0x04001418 RID: 5144
	private float speed;

	// Token: 0x04001419 RID: 5145
	private Quaternion targetRotation;

	// Token: 0x0400141A RID: 5146
	private Quaternion targetPivotRotation;
}
