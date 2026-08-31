using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200071D RID: 1821
	public class GameStateNotFound : Exception
	{
		// Token: 0x06002BF2 RID: 11250 RVA: 0x000F08F5 File Offset: 0x000EEAF5
		public GameStateNotFound(GameStateID ID, Type type)
			: base(string.Format("Could not find game state: {0}({1}) [{2}]", type.Name, ID, GameStateManager.FindType(ID).Name))
		{
		}

		// Token: 0x04002171 RID: 8561
		private const string ExceptionMessage = "Could not find game state: {0}({1}) [{2}]";
	}
}
