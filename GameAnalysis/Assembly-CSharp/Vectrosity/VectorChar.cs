using System;
using UnityEngine;

namespace Vectrosity
{
	// Token: 0x0200049B RID: 1179
	public class VectorChar
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x00081C1C File Offset: 0x0007FE1C
		public static Vector2[][] data
		{
			get
			{
				if (VectorChar.points == null)
				{
					VectorChar.points = new Vector2[256][];
					VectorChar.points[33] = new Vector2[]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.75f)
					};
					VectorChar.points[34] = new Vector2[]
					{
						new Vector2(0.15f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, 0f)
					};
					VectorChar.points[35] = new Vector2[]
					{
						new Vector2(0.2f, 0f),
						new Vector2(0.2f, -1f),
						new Vector2(0f, -0.33f),
						new Vector2(0.6f, -0.33f),
						new Vector2(0.4f, 0f),
						new Vector2(0.4f, -1f),
						new Vector2(0f, -0.66f),
						new Vector2(0.6f, -0.66f)
					};
					VectorChar.points[37] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -0.25f),
						new Vector2(0.15f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0f, -0.25f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0f, 0f),
						new Vector2(0.15f, 0f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f)
					};
					VectorChar.points[38] = new Vector2[]
					{
						new Vector2(0.2f, -0.5f),
						new Vector2(0.2f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.2f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.7f),
						new Vector2(0.2f, 0f),
						new Vector2(0.5f, 0f),
						new Vector2(0.5f, -0.5f),
						new Vector2(0.5f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.5f, -0.5f)
					};
					VectorChar.points[39] = new Vector2[]
					{
						new Vector2(0.3f, -0.25f),
						new Vector2(0.45f, 0f)
					};
					VectorChar.points[40] = new Vector2[]
					{
						new Vector2(0.45f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.15f, -0.75f),
						new Vector2(0.45f, -1f),
						new Vector2(0.15f, -0.75f)
					};
					VectorChar.points[41] = new Vector2[]
					{
						new Vector2(0.15f, 0f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, -0.25f),
						new Vector2(0.45f, -0.75f),
						new Vector2(0.15f, -1f),
						new Vector2(0.45f, -0.75f)
					};
					VectorChar.points[42] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.5f, -0.1f),
						new Vector2(0.1f, -0.9f),
						new Vector2(0.5f, -0.9f),
						new Vector2(0.1f, -0.1f)
					};
					VectorChar.points[43] = new Vector2[]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, -0.9f),
						new Vector2(0.3f, -0.1f)
					};
					VectorChar.points[44] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0.15f, -0.75f)
					};
					VectorChar.points[45] = new Vector2[]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[46] = new Vector2[]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[47] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[48] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[49] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[50] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[51] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[52] = new Vector2[]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[53] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[54] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[55] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f)
					};
					VectorChar.points[56] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[57] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[58] = new Vector2[]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.3f),
						new Vector2(0f, -0.4f)
					};
					VectorChar.points[59] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0.15f, -0.75f),
						new Vector2(0.1f, -0.3f),
						new Vector2(0.1f, -0.4f)
					};
					VectorChar.points[60] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[61] = new Vector2[]
					{
						new Vector2(0.6f, -0.25f),
						new Vector2(0f, -0.25f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f)
					};
					VectorChar.points[62] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[63] = new Vector2[]
					{
						new Vector2(0f, -0.9f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[65] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.3f),
						new Vector2(0.6f, -0.3f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, 0f),
						new Vector2(0f, -0.3f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.3f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[66] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.347f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0.6f, -0.347f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.653f),
						new Vector2(0.448f, -0.5f),
						new Vector2(0.6f, -0.653f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.447f, -1f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0f, -1f),
						new Vector2(0.447f, -1f)
					};
					VectorChar.points[67] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[68] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.447f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.447f, 0f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.6f, -0.155f),
						new Vector2(0.6f, -0.845f),
						new Vector2(0.447f, -1f),
						new Vector2(0.447f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[69] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f)
					};
					VectorChar.points[70] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.5f)
					};
					VectorChar.points[71] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[72] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[73] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[74] = new Vector2[]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.725f)
					};
					VectorChar.points[75] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[76] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[77] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[78] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f)
					};
					VectorChar.points[79] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f)
					};
					VectorChar.points[80] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[81] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					VectorChar.points[82] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.15f, -0.5f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[83] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[84] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[85] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[86] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.3f, -1f),
						new Vector2(0.6f, 0f)
					};
					VectorChar.points[87] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[88] = new Vector2[]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[89] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.6f, 0f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					VectorChar.points[90] = new Vector2[]
					{
						new Vector2(0.6f, 0f),
						new Vector2(0f, 0f),
						new Vector2(0.6f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[91] = new Vector2[]
					{
						new Vector2(0.4f, 0f),
						new Vector2(0.1f, 0f),
						new Vector2(0.1f, -1f),
						new Vector2(0.4f, -1f),
						new Vector2(0.1f, -1f),
						new Vector2(0.1f, 0f)
					};
					VectorChar.points[92] = new Vector2[]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, 0f)
					};
					VectorChar.points[93] = new Vector2[]
					{
						new Vector2(0.2f, 0f),
						new Vector2(0.5f, 0f),
						new Vector2(0.2f, -1f),
						new Vector2(0.5f, -1f),
						new Vector2(0.5f, 0f),
						new Vector2(0.5f, -1f)
					};
					VectorChar.points[94] = new Vector2[]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, 0f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[95] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0.8f, -1f)
					};
					VectorChar.points[96] = new Vector2[]
					{
						new Vector2(0.5f, -0.3f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[97] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[98] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[99] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[100] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, 0f)
					};
					VectorChar.points[101] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[102] = new Vector2[]
					{
						new Vector2(0.15f, -1f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.45f, 0f),
						new Vector2(0.3f, 0f),
						new Vector2(0.15f, -0.25f),
						new Vector2(0.3f, 0f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.15f, -0.5f)
					};
					VectorChar.points[103] = new Vector2[]
					{
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[104] = new Vector2[]
					{
						new Vector2(0f, 0f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[105] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0.3f, -0.25f),
						new Vector2(0.3f, -0.15f)
					};
					VectorChar.points[106] = new Vector2[]
					{
						new Vector2(0.3f, -0.25f),
						new Vector2(0.3f, -0.15f),
						new Vector2(0.3f, -1.25f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0.3f, -1.25f)
					};
					VectorChar.points[107] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, 0f),
						new Vector2(0f, -0.75f),
						new Vector2(0.3f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -1f)
					};
					VectorChar.points[108] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, 0f)
					};
					VectorChar.points[109] = new Vector2[]
					{
						new Vector2(0.45f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.5f)
					};
					VectorChar.points[110] = new Vector2[]
					{
						new Vector2(0.45f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[111] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[112] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -1.25f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[113] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1.25f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[114] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[115] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -0.75f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0f, -0.75f),
						new Vector2(0.6f, -0.75f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[116] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0.3f, -0.25f),
						new Vector2(0.45f, -0.5f),
						new Vector2(0.15f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.45f, -1f)
					};
					VectorChar.points[117] = new Vector2[]
					{
						new Vector2(0f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
					VectorChar.points[118] = new Vector2[]
					{
						new Vector2(0.3f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[119] = new Vector2[]
					{
						new Vector2(0.15f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0.3f, -0.75f),
						new Vector2(0.15f, -1f),
						new Vector2(0.3f, -0.75f),
						new Vector2(0.45f, -1f),
						new Vector2(0.45f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[120] = new Vector2[]
					{
						new Vector2(0.6f, -1f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f)
					};
					VectorChar.points[121] = new Vector2[]
					{
						new Vector2(0f, -1.25f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.3f, -0.875f),
						new Vector2(0f, -0.5f)
					};
					VectorChar.points[122] = new Vector2[]
					{
						new Vector2(0.6f, -0.5f),
						new Vector2(0f, -0.5f),
						new Vector2(0f, -1f),
						new Vector2(0.6f, -0.5f),
						new Vector2(0.6f, -1f),
						new Vector2(0f, -1f)
					};
				}
				return VectorChar.points;
			}
		}

		// Token: 0x04001642 RID: 5698
		public const int numberOfCharacters = 256;

		// Token: 0x04001643 RID: 5699
		private static Vector2[][] points;
	}
}
