using System;
using UnityEngine;

// Token: 0x0200041F RID: 1055
public class ProximityActivate : MonoBehaviour
{
	// Token: 0x06001608 RID: 5640 RVA: 0x00070068 File Offset: 0x0006E268
	private void Start()
	{
		this.originRotation = base.transform.rotation;
		this.alpha = (float)(this.activeState ? 1 : (-1));
		if (this.activator == null)
		{
			this.activator = Camera.main.transform;
		}
		this.infoIcon.SetActive(this.infoPanel != null);
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000700D0 File Offset: 0x0006E2D0
	private bool IsTargetNear()
	{
		if ((this.distanceActivator.position - this.activator.position).sqrMagnitude < this.distance * this.distance)
		{
			if (this.lookAtActivator != null)
			{
				Vector3 vector = this.lookAtActivator.position - this.activator.position;
				if (Vector3.Dot(this.activator.forward, vector.normalized) > 0.95f)
				{
					return true;
				}
			}
			Vector3 vector2 = this.target.transform.position - this.activator.position;
			if (Vector3.Dot(this.activator.forward, vector2.normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000701A0 File Offset: 0x0006E3A0
	private void Update()
	{
		if (!this.activeState)
		{
			if (this.IsTargetNear())
			{
				this.alpha = 1f;
				this.activeState = true;
			}
		}
		else if (!this.IsTargetNear())
		{
			this.alpha = -1f;
			this.activeState = false;
			this.enableInfoPanel = false;
		}
		this.target.alpha = Mathf.Clamp01(this.target.alpha + this.alpha * Time.deltaTime);
		if (this.infoPanel != null)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				this.enableInfoPanel = !this.enableInfoPanel;
			}
			this.infoPanel.alpha = Mathf.Lerp(this.infoPanel.alpha, Mathf.Clamp01(this.enableInfoPanel ? this.alpha : 0f), Time.deltaTime * 10f);
		}
		if (this.lookAtCamera)
		{
			if (this.activeState)
			{
				this.targetRotation = Quaternion.LookRotation(this.activator.position - base.transform.position);
			}
			else
			{
				this.targetRotation = this.originRotation;
			}
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.targetRotation, Time.deltaTime);
		}
	}

	// Token: 0x040013FF RID: 5119
	public Transform distanceActivator;

	// Token: 0x04001400 RID: 5120
	public Transform lookAtActivator;

	// Token: 0x04001401 RID: 5121
	public float distance;

	// Token: 0x04001402 RID: 5122
	public Transform activator;

	// Token: 0x04001403 RID: 5123
	public bool activeState;

	// Token: 0x04001404 RID: 5124
	public CanvasGroup target;

	// Token: 0x04001405 RID: 5125
	public bool lookAtCamera = true;

	// Token: 0x04001406 RID: 5126
	public bool enableInfoPanel;

	// Token: 0x04001407 RID: 5127
	public GameObject infoIcon;

	// Token: 0x04001408 RID: 5128
	private float alpha;

	// Token: 0x04001409 RID: 5129
	public CanvasGroup infoPanel;

	// Token: 0x0400140A RID: 5130
	private Quaternion originRotation;

	// Token: 0x0400140B RID: 5131
	private Quaternion targetRotation;
}
