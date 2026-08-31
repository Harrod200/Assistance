using System;
using UnityEngine;

// Token: 0x0200040D RID: 1037
public class faceCamera : MonoBehaviour
{
	// Token: 0x06001540 RID: 5440 RVA: 0x000684B1 File Offset: 0x000666B1
	private void Start()
	{
	}

	// Token: 0x06001541 RID: 5441 RVA: 0x000684B3 File Offset: 0x000666B3
	private void Update()
	{
		base.transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}
}
