using System;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class RangeSphereController : MonoBehaviour
{
	// Token: 0x0600153A RID: 5434 RVA: 0x00068325 File Offset: 0x00066525
	private void Start()
	{
		if (this._Camera == null)
		{
			this._Camera = Camera.main.transform;
		}
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x00068348 File Offset: 0x00066548
	private void LateUpdate()
	{
		if (this._material != null && this._Camera != null)
		{
			this._material.SetVector("_CameraDirection", this._Camera.forward);
			this._material.SetVector("_CameraPosition", this._Camera.position);
			this._material.SetVector("_ObjectPosition", base.transform.position);
			this._material.SetFloat("_ScaleOffset", base.transform.localScale.x);
		}
	}

	// Token: 0x040012AB RID: 4779
	[SerializeField]
	private Material _material;

	// Token: 0x040012AC RID: 4780
	[SerializeField]
	private Transform _Camera;
}
