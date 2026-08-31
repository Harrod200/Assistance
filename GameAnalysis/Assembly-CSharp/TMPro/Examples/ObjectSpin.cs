using System;
using UnityEngine;

namespace TMPro.Examples
{
	// Token: 0x02000502 RID: 1282
	public class ObjectSpin : MonoBehaviour
	{
		// Token: 0x06001FBB RID: 8123 RVA: 0x000A4D14 File Offset: 0x000A2F14
		private void Awake()
		{
			this.m_transform = base.transform;
			this.m_initial_Rotation = this.m_transform.rotation.eulerAngles;
			this.m_initial_Position = this.m_transform.position;
			Light component = base.GetComponent<Light>();
			this.m_lightColor = ((component != null) ? component.color : Color.black);
		}

		// Token: 0x06001FBC RID: 8124 RVA: 0x000A4D80 File Offset: 0x000A2F80
		private void Update()
		{
			if (this.Motion == ObjectSpin.MotionType.Rotation)
			{
				this.m_transform.Rotate(0f, this.SpinSpeed * Time.deltaTime, 0f);
				return;
			}
			if (this.Motion == ObjectSpin.MotionType.BackAndForth)
			{
				this.m_time += this.SpinSpeed * Time.deltaTime;
				this.m_transform.rotation = Quaternion.Euler(this.m_initial_Rotation.x, Mathf.Sin(this.m_time) * (float)this.RotationRange + this.m_initial_Rotation.y, this.m_initial_Rotation.z);
				return;
			}
			this.m_time += this.SpinSpeed * Time.deltaTime;
			float num = 15f * Mathf.Cos(this.m_time * 0.95f);
			float num2 = 10f;
			float num3 = 0f;
			this.m_transform.position = this.m_initial_Position + new Vector3(num, num3, num2);
			this.m_prevPOS = this.m_transform.position;
			this.frames++;
		}

		// Token: 0x04001868 RID: 6248
		public float SpinSpeed = 5f;

		// Token: 0x04001869 RID: 6249
		public int RotationRange = 15;

		// Token: 0x0400186A RID: 6250
		private Transform m_transform;

		// Token: 0x0400186B RID: 6251
		private float m_time;

		// Token: 0x0400186C RID: 6252
		private Vector3 m_prevPOS;

		// Token: 0x0400186D RID: 6253
		private Vector3 m_initial_Rotation;

		// Token: 0x0400186E RID: 6254
		private Vector3 m_initial_Position;

		// Token: 0x0400186F RID: 6255
		private Color32 m_lightColor;

		// Token: 0x04001870 RID: 6256
		private int frames;

		// Token: 0x04001871 RID: 6257
		public ObjectSpin.MotionType Motion;

		// Token: 0x02000C77 RID: 3191
		public enum MotionType
		{
			// Token: 0x04004E76 RID: 20086
			Rotation,
			// Token: 0x04004E77 RID: 20087
			BackAndForth,
			// Token: 0x04004E78 RID: 20088
			Translation
		}
	}
}
