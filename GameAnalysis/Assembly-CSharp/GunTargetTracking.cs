using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
public class GunTargetTracking : MonoBehaviour
{
	// Token: 0x060000EA RID: 234 RVA: 0x00007E6D File Offset: 0x0000606D
	public void Awake()
	{
		this._initialBaseObjectRotation = this.baseObject.transform.localRotation;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00007E88 File Offset: 0x00006088
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = this._target.transform.position;
		if (Vector3.Angle(this.baseObject.transform.up, (position - base.transform.position).normalized) <= 180f)
		{
			Vector3 normalized = (base.transform.parent.rotation * this._initialBaseObjectRotation * Vector3.up).normalized;
			float num = Vector3.Dot(normalized, position - this.baseObject.transform.position);
			float num2 = Vector3.SignedAngle((position - normalized * num - this.baseObject.transform.position).normalized, base.transform.parent.rotation * this._initialBaseObjectRotation * Vector3.forward, Vector3.up);
			if (!Mathf.Approximately(num2, 0f))
			{
				Quaternion quaternion = this._initialBaseObjectRotation * Quaternion.AngleAxis(-num2, this._initialBaseObjectRotation * Vector3.up);
				if (Mathf.Abs(num2) > this.turretTrainingRate_degsec * deltaTime)
				{
					num2 = this.turretTrainingRate_degsec * deltaTime;
				}
				this.baseObject.transform.localRotation = Quaternion.RotateTowards(this.baseObject.transform.localRotation, quaternion, num2);
			}
			Vector3 right = this.weaponObject.transform.right;
			float num3 = Vector3.Dot(right, position - this.baseObject.transform.position);
			Vector3 normalized2 = (position - right * num3 - this.weaponObject.transform.position).normalized;
			float num4 = Vector3.SignedAngle(this.weaponObject.transform.forward, normalized2, this.weaponObject.transform.right);
			if (!Mathf.Approximately(num4, 0f))
			{
				Quaternion quaternion2 = this.weaponObject.transform.localRotation * Quaternion.Euler(num4, 0f, 0f);
				if (Mathf.Abs(num4) > this.turretTrainingRate_degsec * deltaTime)
				{
					num4 = this.turretTrainingRate_degsec * deltaTime;
				}
				this.weaponObject.transform.localRotation = Quaternion.RotateTowards(this.weaponObject.transform.localRotation, quaternion2, num4);
			}
			if (this.OnTarget())
			{
				this._target.GetComponent<Renderer>().material = this._green;
				return;
			}
			this._target.GetComponent<Renderer>().material = this._red;
		}
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000813C File Offset: 0x0000633C
	public bool OnTarget()
	{
		return Vector3.Angle(this.weaponObject.transform.forward, (this._target.transform.position - this.weaponObject.transform.position).normalized) < 1f;
	}

	// Token: 0x040000DF RID: 223
	public Material _red;

	// Token: 0x040000E0 RID: 224
	public Material _green;

	// Token: 0x040000E1 RID: 225
	public GameObject _target;

	// Token: 0x040000E2 RID: 226
	[Space(10f)]
	public GameObject baseObject;

	// Token: 0x040000E3 RID: 227
	public GameObject weaponObject;

	// Token: 0x040000E4 RID: 228
	public GameObject firePoint;

	// Token: 0x040000E5 RID: 229
	private float turretTrainingRate_degsec = 135f;

	// Token: 0x040000E6 RID: 230
	private Quaternion _initialBaseObjectRotation;
}
