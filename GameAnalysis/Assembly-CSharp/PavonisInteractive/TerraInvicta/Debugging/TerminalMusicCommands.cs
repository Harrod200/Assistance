using System;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000916 RID: 2326
	public class TerminalMusicCommands
	{
		// Token: 0x060058E5 RID: 22757 RVA: 0x0028C309 File Offset: 0x0028A509
		public TerminalMusicCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058E6 RID: 22758 RVA: 0x0028C320 File Offset: 0x0028A520
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("musicinfo", new CommandHandler(this.MusicInfo), "Show current music info");
			this.terminalController.RegisterCommand("setmusicintensity", new CommandHandler(this.SetIntensity), "Set intensity of music [0-1] eg; 'setmusicintensity .5'");
			this.terminalController.RegisterCommand("setmusicvolume", new CommandHandler(this.SetVolume), "Set volume of music [0-1] eg; 'setmusicvolume .5'");
			this.terminalController.RegisterCommand("playsoundevent", new CommandHandler(this.PlaySoundEvent), "plays an fmod sound event, specify the event path");
		}

		// Token: 0x060058E7 RID: 22759 RVA: 0x0028C3B4 File Offset: 0x0028A5B4
		public void MusicInfo(string[] args)
		{
			this.terminalController.Output("Intensity: " + AudioManager.GetIntensity().ToString());
		}

		// Token: 0x060058E8 RID: 22760 RVA: 0x0028C3E4 File Offset: 0x0028A5E4
		public void SetIntensity(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Missing intensity value");
				return;
			}
			float num;
			if (float.TryParse(args[0], out num))
			{
				num = Mathf.Clamp01(num);
				AudioManager.SetIntensity(num);
				return;
			}
			this.terminalController.OutputError("set music intensity error: couldn't parse intensity value " + args[0]);
		}

		// Token: 0x060058E9 RID: 22761 RVA: 0x0028C43C File Offset: 0x0028A63C
		public void SetVolume(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Missing volume value");
				return;
			}
			float num;
			if (float.TryParse(args[0], out num))
			{
				num = Mathf.Clamp01(num);
				BusManager.SetVolume(BusManager.Music, num);
				return;
			}
			this.terminalController.OutputError("set music volume error: couldn't parse volume value " + args[0]);
		}

		// Token: 0x060058EA RID: 22762 RVA: 0x0028C497 File Offset: 0x0028A697
		public void PlaySoundEvent(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Missing sound event path");
				return;
			}
			AudioManager.PlayOneShot(args[0], false, false);
		}

		// Token: 0x04004073 RID: 16499
		private TerminalController terminalController;
	}
}
