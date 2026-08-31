using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B4 RID: 1460
	[CreateAssetMenu(menuName = "TerraInvicta/Sounds/VariableSFXGroup")]
	[Serializable]
	public class VariableSFXGroup : ScriptableObject
	{
		// Token: 0x04001D6E RID: 7534
		public VariableAudioFX.VariableSFX[] variableSFX;
	}
}
