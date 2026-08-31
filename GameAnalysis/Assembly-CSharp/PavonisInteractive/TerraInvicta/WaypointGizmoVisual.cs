using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070C RID: 1804
	public class WaypointGizmoVisual : MonoBehaviour
	{
		// Token: 0x06002AE3 RID: 10979 RVA: 0x000E8A49 File Offset: 0x000E6C49
		public void SetGizmoHighlight(bool value)
		{
			this._isHighlighted = value;
			if (this._isHighlighted)
			{
				this.gizmoMesh.material = this.hoverMaterial;
				return;
			}
			this.gizmoMesh.material = this.baseMaterial;
		}

		// Token: 0x06002AE4 RID: 10980 RVA: 0x000E8A7D File Offset: 0x000E6C7D
		public void SetGizmoInvalid(bool value)
		{
			if (value)
			{
				this.gizmoMesh.material = this.invalidMaterial;
				return;
			}
			this.gizmoMesh.material = this.baseMaterial;
		}

		// Token: 0x040020CE RID: 8398
		public MeshRenderer gizmoMesh;

		// Token: 0x040020CF RID: 8399
		public Material baseMaterial;

		// Token: 0x040020D0 RID: 8400
		public Material hoverMaterial;

		// Token: 0x040020D1 RID: 8401
		public Material invalidMaterial;

		// Token: 0x040020D2 RID: 8402
		private bool _isHighlighted;
	}
}
