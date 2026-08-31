using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.SimpleZip
{
	// Token: 0x020004AA RID: 1194
	public class Example : MonoBehaviour
	{
		// Token: 0x06001ADA RID: 6874 RVA: 0x00091680 File Offset: 0x0008F880
		public void Start()
		{
			try
			{
				string text = "El perro de San Roque no tiene rabo porque Ramón Rodríguez se lo ha robado.";
				text = string.Concat(new string[] { text, text, text, text, text });
				string text2 = Zip.CompressToString(text);
				string text3 = Zip.Decompress(text2);
				this.Text.text = string.Format("Plain text: {0}\n\nCompressed: {1}\n\nDecompressed: {2}", text, text2, text3);
			}
			catch (Exception ex)
			{
				this.Text.text = ex.ToString();
			}
		}

		// Token: 0x040016E0 RID: 5856
		public Text Text;
	}
}
