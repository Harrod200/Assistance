using System;
using Shapes;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000801 RID: 2049
	public class CombatGrid : MonoBehaviour
	{
		// Token: 0x06004A4A RID: 19018 RVA: 0x001F271E File Offset: 0x001F091E
		private void Awake()
		{
			this.mainCamera = Camera.main;
			this.gridLines = base.transform.GetComponentsInChildren<Line>();
		}

		// Token: 0x06004A4B RID: 19019 RVA: 0x001F273C File Offset: 0x001F093C
		private void Start()
		{
			this.plane = new Plane(Vector3.up, Vector3.zero);
			this.gridCollider = base.GetComponent<Collider>();
		}

		// Token: 0x06004A4C RID: 19020 RVA: 0x001F2760 File Offset: 0x001F0960
		private void Update()
		{
			Plane plane = new Plane(this.mainCamera.transform.forward, this.mainCamera.transform.position);
			for (int i = 0; i < this.gridLines.Length; i++)
			{
				float num = plane.GetDistanceToPoint(plane.ClosestPointOnPlane(this.mainCamera.transform.position));
				num = Mathf.Clamp(num, 1f, 20f);
				this.gridLines[i].Thickness = num * 0.8f * 1.5f;
			}
		}

		// Token: 0x17000E90 RID: 3728
		// (get) Token: 0x06004A4D RID: 19021 RVA: 0x001F27F4 File Offset: 0x001F09F4
		public Vector3 cursorPosition
		{
			get
			{
				Vector3 vector = Vector3.zero;
				Ray ray = this.mainCamera.ScreenPointToRay(Input.mousePosition);
				this.gridCollider.enabled = true;
				RaycastHit raycastHit;
				if (this.gridCollider.Raycast(ray, out raycastHit, float.PositiveInfinity))
				{
					vector = raycastHit.point;
				}
				this.gridCollider.enabled = false;
				return vector;
			}
		}

		// Token: 0x06004A4E RID: 19022 RVA: 0x001F2850 File Offset: 0x001F0A50
		public Vector3 CursorPositionRelativeToPlaneAt(Vector3 position, Vector3 normal, Vector3 mousePixelCoord)
		{
			this.plane.SetNormalAndPosition(normal, position);
			Utilities.DebugDrawPlane(position, normal, Color.magenta, 10f);
			Ray ray = this.mainCamera.ScreenPointToRay(mousePixelCoord);
			float num;
			if (this.plane.Raycast(ray, out num))
			{
				return ray.GetPoint(num);
			}
			return Vector3.zero;
		}

		// Token: 0x06004A4F RID: 19023 RVA: 0x001F28A8 File Offset: 0x001F0AA8
		public void GetDistanceToPointOfIntersection(Ray ray, out float distance)
		{
			this.plane.Raycast(ray, out distance);
		}

		// Token: 0x06004A50 RID: 19024 RVA: 0x001F28B8 File Offset: 0x001F0AB8
		public void ToggleGrid()
		{
			base.gameObject.SetActive(!base.gameObject.activeSelf);
		}

		// Token: 0x04002B1B RID: 11035
		private Collider gridCollider;

		// Token: 0x04002B1C RID: 11036
		private Plane plane;

		// Token: 0x04002B1D RID: 11037
		private Camera mainCamera;

		// Token: 0x04002B1E RID: 11038
		private Line[] gridLines;
	}
}
