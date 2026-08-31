using System;
using Shapes;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000802 RID: 2050
	public class CombatGridLinePlacementHelper : MonoBehaviour
	{
		// Token: 0x06004A52 RID: 19026 RVA: 0x001F28DC File Offset: 0x001F0ADC
		[ContextMenu("Generate Grid")]
		private void GenerateGrid()
		{
			if (this._linePrefab == null)
			{
				Debug.LogError("CombatGridLinePlacementHelper.GenerateGrid: Failed to generate grid! Line prefab is null.");
				return;
			}
			Draw.LineGeometry = LineGeometry.Billboard;
			Draw.ThicknessSpace = ThicknessSpace.Meters;
			Draw.Thickness = 0.01f;
			float num = 5000f;
			int num2 = 0;
			float num3 = 20000f;
			int num4 = (int)(num3 / num) + 1;
			for (int i = -(num4 >> 1); i <= num4 >> 1; i++)
			{
				Line component = global::UnityEngine.Object.Instantiate<GameObject>(this._linePrefab, Vector3.zero, Quaternion.identity, base.transform).GetComponent<Line>();
				component.Start = new Vector3(-(num3 / 2f), (float)num2, (float)i * num);
				component.End = new Vector3(num3 / 2f, (float)num2, (float)i * num);
				component.Geometry = LineGeometry.Billboard;
				component.ThicknessSpace = ThicknessSpace.Meters;
				component.Thickness = 1f;
				Line component2 = global::UnityEngine.Object.Instantiate<GameObject>(this._linePrefab, Vector3.zero, Quaternion.identity, base.transform).GetComponent<Line>();
				component2.Start = new Vector3((float)i * num, (float)num2, -(num3 / 2f));
				component2.End = new Vector3((float)i * num, (float)num2, num3 / 2f);
				component2.Geometry = LineGeometry.Billboard;
				component2.ThicknessSpace = ThicknessSpace.Meters;
				component2.Thickness = 1f;
			}
		}

		// Token: 0x04002B1F RID: 11039
		[SerializeField]
		private GameObject _linePrefab;
	}
}
