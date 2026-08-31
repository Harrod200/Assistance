using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F6 RID: 1782
	public class SaveStructure
	{
		// Token: 0x06002A42 RID: 10818 RVA: 0x000E5480 File Offset: 0x000E3680
		public static SaveStructure Load(string filepath)
		{
			if (!File.Exists(filepath))
			{
				Log.Warn("Attempting to load non-existant save file at " + filepath, Array.Empty<object>());
				GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
				return null;
			}
			string text;
			if (filepath.Contains(".gz"))
			{
				using (FileStream fileStream = File.Open(filepath, FileMode.Open))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
					{
						using (StreamReader streamReader = new StreamReader(gzipStream))
						{
							text = streamReader.ReadToEnd();
							goto IL_0081;
						}
					}
				}
			}
			text = File.ReadAllText(filepath);
			IL_0081:
			if (!text.Contains("\"exists\": true"))
			{
				text = text.Replace("\"archived\": false,", "\"archived\": false,\"exists\": true,");
			}
			return StringSerializationAPI.Deserialize(typeof(SaveStructure), text) as SaveStructure;
		}

		// Token: 0x04002089 RID: 8329
		public GameStateID currentID;

		// Token: 0x0400208A RID: 8330
		public Dictionary<Type, Dictionary<GameStateID, TIGameState>> gamestates;
	}
}
