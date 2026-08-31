using System;
using UnityEngine;

// Token: 0x0200042A RID: 1066
public class ShipModelViewer : MonoBehaviour
{
	// Token: 0x06001637 RID: 5687 RVA: 0x00071188 File Offset: 0x0006F388
	private void Update()
	{
		if (this.isDragging && this.shipT != null)
		{
			this.lastMousePos = TIInputManager.lastMousePos;
			if (Input.mousePosition != this.lastMousePos)
			{
				Vector3 vector = new Vector3(0f, 0f, 0f);
				if (Input.mousePosition.x > this.lastMousePos.x)
				{
					vector.y = this.rotateSpeed * Mathf.Abs(this.lastMousePos.x - Input.mousePosition.x);
				}
				if (Input.mousePosition.x < this.lastMousePos.x)
				{
					vector.y = -this.rotateSpeed * Mathf.Abs(this.lastMousePos.x - Input.mousePosition.x);
				}
				if (Input.mousePosition.y > this.lastMousePos.y)
				{
					vector.z = this.rotateSpeed * Mathf.Abs(this.lastMousePos.y - Input.mousePosition.y);
				}
				if (Input.mousePosition.y < this.lastMousePos.y)
				{
					vector.z = -this.rotateSpeed * Mathf.Abs(this.lastMousePos.y - Input.mousePosition.y);
				}
				Vector3 vector2 = this.shipT.localRotation.eulerAngles + vector;
				this.shipT.localRotation = Quaternion.Euler(vector2);
			}
		}
	}

	// Token: 0x04001458 RID: 5208
	public bool isDragging;

	// Token: 0x04001459 RID: 5209
	public Transform shipT;

	// Token: 0x0400145A RID: 5210
	private Vector3 lastMousePos;

	// Token: 0x0400145B RID: 5211
	private float rotateSpeed = 0.2f;
}
