using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007DF RID: 2015
	public static class AchievementEvents
	{
		// Token: 0x04002A00 RID: 10752
		public const string resistWin = "resistWin";

		// Token: 0x04002A01 RID: 10753
		public const string destroyWin = "destroyWin";

		// Token: 0x04002A02 RID: 10754
		public const string escapeWin = "escapeWin";

		// Token: 0x04002A03 RID: 10755
		public const string exploitWin = "exploitWin";

		// Token: 0x04002A04 RID: 10756
		public const string cooperateWin = "cooperateWin";

		// Token: 0x04002A05 RID: 10757
		public const string appeaseWin = "appeaseWin";

		// Token: 0x04002A06 RID: 10758
		public const string submitWin = "submitWin";

		// Token: 0x04002A07 RID: 10759
		public const string normalWin = "normalWin";

		// Token: 0x04002A08 RID: 10760
		public const string veteranWin = "veteranWin";

		// Token: 0x04002A09 RID: 10761
		public const string brutalWin = "brutalWin";

		// Token: 0x04002A0A RID: 10762
		public const string allFactionWin = "allFactionWin";

		// Token: 0x04002A0B RID: 10763
		public const string winPheonix = "winPheonix";

		// Token: 0x04002A0C RID: 10764
		public const string buildTitan = "buildTitan";

		// Token: 0x04002A0D RID: 10765
		public const string buildFirstShip = "buildFirstShip";

		// Token: 0x04002A0E RID: 10766
		public const string buildShipHighDV = "buildShipHighDV";

		// Token: 0x04002A0F RID: 10767
		public const string buildShipHighAccel = "buildShipHighAccel";

		// Token: 0x04002A10 RID: 10768
		public const string startCampaign = "startCampaign";

		// Token: 0x04002A11 RID: 10769
		public const string winCombatNoLosses = "winCombatNoLosses";

		// Token: 0x04002A12 RID: 10770
		public const string loseCombatNoSurvivors = "loseCombatNoSurvivors";

		// Token: 0x04002A13 RID: 10771
		public const string destroyMothership = "destroyMothership";

		// Token: 0x04002A14 RID: 10772
		public const string recruitFullCouncil = "recruitFullCouncil";

		// Token: 0x04002A15 RID: 10773
		public const string turnCouncilor = "turnCouncilor";

		// Token: 0x04002A16 RID: 10774
		public const string upgradeShipClass = "upgradeShipClass";

		// Token: 0x04002A17 RID: 10775
		public const string refitShip = "refitShip";

		// Token: 0x04002A18 RID: 10776
		public const string stationStolen = "stationStolen";

		// Token: 0x04002A19 RID: 10777
		public const string shipStolen = "shipStolen";

		// Token: 0x04002A1A RID: 10778
		public const string stealStation = "stealStation";

		// Token: 0x04002A1B RID: 10779
		public const string stealShip = "stealShip";

		// Token: 0x04002A1C RID: 10780
		public const string controlStrongFleet = "controlStrongFleet";

		// Token: 0x04002A1D RID: 10781
		public const string controlBigFleet = "controlBigFleet";

		// Token: 0x04002A1E RID: 10782
		public const string controlFullCouncilTurned = "controlFullCouncilTurned";

		// Token: 0x04002A1F RID: 10783
		public const string investigateCrash = "investigateCrash";

		// Token: 0x04002A20 RID: 10784
		public const string buildHabSpace = "buildHabSpace";

		// Token: 0x04002A21 RID: 10785
		public const string buildHabBase = "buildHabBase";

		// Token: 0x04002A22 RID: 10786
		public const string winCombat = "winCombat";

		// Token: 0x04002A23 RID: 10787
		public const string colonizeLuna = "colonizeLuna";

		// Token: 0x04002A24 RID: 10788
		public const string colonizeMercury = "colonizeMercury";

		// Token: 0x04002A25 RID: 10789
		public const string colonizeVenus = "colonizeVenus";

		// Token: 0x04002A26 RID: 10790
		public const string colonizeLPoint = "colonizeLPoint";

		// Token: 0x04002A27 RID: 10791
		public const string colonizeMars = "colonizeMars";

		// Token: 0x04002A28 RID: 10792
		public const string colonizeCeres = "colonizeCeres";

		// Token: 0x04002A29 RID: 10793
		public const string colonizeJupiter = "colonizeJupiter";

		// Token: 0x04002A2A RID: 10794
		public const string colonizeIo = "colonizeIo";

		// Token: 0x04002A2B RID: 10795
		public const string colonizeEuropa = "colonizeEuropa";

		// Token: 0x04002A2C RID: 10796
		public const string colonizeSaturn = "colonizeSaturn";

		// Token: 0x04002A2D RID: 10797
		public const string colonizeMimas = "colonizeMimas";

		// Token: 0x04002A2E RID: 10798
		public const string colonizeUranus = "colonizeUranus";

		// Token: 0x04002A2F RID: 10799
		public const string colonizeMiranda = "colonizeMiranda";

		// Token: 0x04002A30 RID: 10800
		public const string colonizeNeptune = "colonizeNeptune";

		// Token: 0x04002A31 RID: 10801
		public const string colonizePluto = "colonizePluto";

		// Token: 0x04002A32 RID: 10802
		public const string colonizeHaumea = "colonizeHaumea";

		// Token: 0x04002A33 RID: 10803
		public const string controlManyHabs = "controlManyHabs";

		// Token: 0x04002A34 RID: 10804
		public const string colonizeAllMajorPlanets = "colonizeAllMajorPlanets";

		// Token: 0x04002A35 RID: 10805
		public const string controlLaunchFacility = "controlLaunchFacility";

		// Token: 0x04002A36 RID: 10806
		public const string controlNation = "controlNation";

		// Token: 0x04002A37 RID: 10807
		public const string controlManyNations = "controlManyNations";

		// Token: 0x04002A38 RID: 10808
		public const string councilorIntoGround = "councilorIntoGround";

		// Token: 0x04002A39 RID: 10809
		public const string declareWar = "declareWar";

		// Token: 0x04002A3A RID: 10810
		public const string researchAllTechs = "researchAllTechs";

		// Token: 0x04002A3B RID: 10811
		public const string destroyMegafauna = "destroyMegafauna";

		// Token: 0x04002A3C RID: 10812
		public const string killCouncilor = "killCouncilor";

		// Token: 0x04002A3D RID: 10813
		public const string destroyAlienArmy = "destroyAlienArmy";

		// Token: 0x04002A3E RID: 10814
		public const string controlMegafauna = "controlMegafauna";

		// Token: 0x04002A3F RID: 10815
		public const string coup = "coup";

		// Token: 0x04002A40 RID: 10816
		public const string regimeChange = "regimeChange";

		// Token: 0x04002A41 RID: 10817
		public const string failEasyMission = "failEasyMission";

		// Token: 0x04002A42 RID: 10818
		public const string captureCouncilor = "captureCouncilor";

		// Token: 0x04002A43 RID: 10819
		public const string captureAlien = "captureAlien";

		// Token: 0x04002A44 RID: 10820
		public const string researchAlienMovements = "researchAlienMovements";

		// Token: 0x04002A45 RID: 10821
		public const string killAlien = "killAlien";

		// Token: 0x04002A46 RID: 10822
		public const string investigateCouncilor = "investigateCouncilor";

		// Token: 0x04002A47 RID: 10823
		public const string assaultHabMarines = "assaultHabMarines";

		// Token: 0x04002A48 RID: 10824
		public const string stackedCouncilor = "stackedCouncilor";

		// Token: 0x04002A49 RID: 10825
		public const string escapeAlienFleet = "escapeAlienFleet";

		// Token: 0x04002A4A RID: 10826
		public const string sabotageResearch = "sabotageResearch";

		// Token: 0x04002A4B RID: 10827
		public const string stealTechnology = "stealTechnology";

		// Token: 0x04002A4C RID: 10828
		public const string firstTransfer = "firstTransfer";

		// Token: 0x04002A4D RID: 10829
		public const string bombardment = "bombardment";

		// Token: 0x04002A4E RID: 10830
		public const string fireNukeBarrage = "fireNukeBarrage";

		// Token: 0x04002A4F RID: 10831
		public const string seaLevelRise = "seaLevelRise";

		// Token: 0x04002A50 RID: 10832
		public const string nuclearWinter = "nuclearWinter";

		// Token: 0x04002A51 RID: 10833
		public const string completeNukeProgram = "completeNukeProgram";

		// Token: 0x04002A52 RID: 10834
		public const string councilorDeathNatural = "councilorDeathNatural";

		// Token: 0x04002A53 RID: 10835
		public const string discoverVictory = "discoverVictory";

		// Token: 0x04002A54 RID: 10836
		public const string researchPherocytes = "researchPherocytes";

		// Token: 0x04002A55 RID: 10837
		public const string researchExotics = "researchExotics";

		// Token: 0x04002A56 RID: 10838
		public const string killCouncilorSpace = "killCouncilorSpace";

		// Token: 0x04002A57 RID: 10839
		public const string completeTrade = "completeTrade";

		// Token: 0x04002A58 RID: 10840
		public const string purgeExecutive = "purgeExecutive";

		// Token: 0x04002A59 RID: 10841
		public const string augment = "augment";

		// Token: 0x04002A5A RID: 10842
		public const string lowAtrocityWin = "lowAtrocityWin";

		// Token: 0x04002A5B RID: 10843
		public const string cometBase = "cometBase";

		// Token: 0x04002A5C RID: 10844
		public const string temperatureAnomaly = "temperatureAnomaly";

		// Token: 0x04002A5D RID: 10845
		public const string exofighterWin = "exofighterWin";

		// Token: 0x04002A5E RID: 10846
		public const string winBattleOutmatched = "winBattleOutmatched";

		// Token: 0x04002A5F RID: 10847
		public const string spacebodyPopulation = "spacebodyPopulation";

		// Token: 0x04002A60 RID: 10848
		public const string officersOnShip = "officersOnShip";

		// Token: 0x04002A61 RID: 10849
		public const string admiral = "admiral";

		// Token: 0x04002A62 RID: 10850
		public const string unrestBreakaway = "unrestBreakaway";

		// Token: 0x04002A63 RID: 10851
		public const string ramming = "ramming";

		// Token: 0x04002A64 RID: 10852
		public static bool retrievedMicrosoftAchievements = false;

		// Token: 0x04002A65 RID: 10853
		public static List<string> passedXBLAchievements = new List<string>();

		// Token: 0x02000F99 RID: 3993
		public enum Achievements
		{
			// Token: 0x04005EE0 RID: 24288
			resistWin = 1,
			// Token: 0x04005EE1 RID: 24289
			destroyWin,
			// Token: 0x04005EE2 RID: 24290
			escapeWin,
			// Token: 0x04005EE3 RID: 24291
			exploitWin,
			// Token: 0x04005EE4 RID: 24292
			cooperateWin,
			// Token: 0x04005EE5 RID: 24293
			appeaseWin,
			// Token: 0x04005EE6 RID: 24294
			submitWin,
			// Token: 0x04005EE7 RID: 24295
			normalWin,
			// Token: 0x04005EE8 RID: 24296
			veteranWin,
			// Token: 0x04005EE9 RID: 24297
			brutalWin,
			// Token: 0x04005EEA RID: 24298
			allFactionWin,
			// Token: 0x04005EEB RID: 24299
			winPheonix,
			// Token: 0x04005EEC RID: 24300
			buildTitan,
			// Token: 0x04005EED RID: 24301
			buildFirstShip,
			// Token: 0x04005EEE RID: 24302
			buildShipHighDV,
			// Token: 0x04005EEF RID: 24303
			buildShipHighAccel,
			// Token: 0x04005EF0 RID: 24304
			startCampaign,
			// Token: 0x04005EF1 RID: 24305
			winCombatNoLosses,
			// Token: 0x04005EF2 RID: 24306
			loseCombatNoSurvivors,
			// Token: 0x04005EF3 RID: 24307
			destroyMothership,
			// Token: 0x04005EF4 RID: 24308
			recruitFullCouncil,
			// Token: 0x04005EF5 RID: 24309
			turnCouncilor,
			// Token: 0x04005EF6 RID: 24310
			upgradeShipClass,
			// Token: 0x04005EF7 RID: 24311
			refitShip,
			// Token: 0x04005EF8 RID: 24312
			stationStolen,
			// Token: 0x04005EF9 RID: 24313
			shipStolen,
			// Token: 0x04005EFA RID: 24314
			stealStation,
			// Token: 0x04005EFB RID: 24315
			stealShip,
			// Token: 0x04005EFC RID: 24316
			controlStrongFleet,
			// Token: 0x04005EFD RID: 24317
			controlBigFleet,
			// Token: 0x04005EFE RID: 24318
			controlFullCouncilTurned,
			// Token: 0x04005EFF RID: 24319
			investigateCrash,
			// Token: 0x04005F00 RID: 24320
			buildHabSpace,
			// Token: 0x04005F01 RID: 24321
			buildHabBase,
			// Token: 0x04005F02 RID: 24322
			winCombat,
			// Token: 0x04005F03 RID: 24323
			colonizeLuna,
			// Token: 0x04005F04 RID: 24324
			colonizeMercury,
			// Token: 0x04005F05 RID: 24325
			colonizeVenus,
			// Token: 0x04005F06 RID: 24326
			colonizeLPoint,
			// Token: 0x04005F07 RID: 24327
			colonizeMars,
			// Token: 0x04005F08 RID: 24328
			colonizeCeres,
			// Token: 0x04005F09 RID: 24329
			colonizeJupiter,
			// Token: 0x04005F0A RID: 24330
			colonizeIo,
			// Token: 0x04005F0B RID: 24331
			colonizeEuropa,
			// Token: 0x04005F0C RID: 24332
			colonizeSaturn,
			// Token: 0x04005F0D RID: 24333
			colonizeMimas,
			// Token: 0x04005F0E RID: 24334
			colonizeUranus,
			// Token: 0x04005F0F RID: 24335
			colonizeMiranda,
			// Token: 0x04005F10 RID: 24336
			colonizeNeptune,
			// Token: 0x04005F11 RID: 24337
			colonizePluto,
			// Token: 0x04005F12 RID: 24338
			colonizeHaumea,
			// Token: 0x04005F13 RID: 24339
			controlManyHabs,
			// Token: 0x04005F14 RID: 24340
			colonizeAllMajorPlanets,
			// Token: 0x04005F15 RID: 24341
			controlLaunchFacility,
			// Token: 0x04005F16 RID: 24342
			controlNation,
			// Token: 0x04005F17 RID: 24343
			controlManyNations,
			// Token: 0x04005F18 RID: 24344
			councilorIntoGround,
			// Token: 0x04005F19 RID: 24345
			declareWar,
			// Token: 0x04005F1A RID: 24346
			researchAllTechs,
			// Token: 0x04005F1B RID: 24347
			destroyMegafauna,
			// Token: 0x04005F1C RID: 24348
			killCouncilor,
			// Token: 0x04005F1D RID: 24349
			destroyAlienArmy,
			// Token: 0x04005F1E RID: 24350
			controlMegafauna,
			// Token: 0x04005F1F RID: 24351
			coup,
			// Token: 0x04005F20 RID: 24352
			regimeChange,
			// Token: 0x04005F21 RID: 24353
			failEasyMission,
			// Token: 0x04005F22 RID: 24354
			captureCouncilor,
			// Token: 0x04005F23 RID: 24355
			captureAlien,
			// Token: 0x04005F24 RID: 24356
			researchAlienMovements,
			// Token: 0x04005F25 RID: 24357
			killAlien,
			// Token: 0x04005F26 RID: 24358
			investigateCouncilor,
			// Token: 0x04005F27 RID: 24359
			assaultHabMarines,
			// Token: 0x04005F28 RID: 24360
			stackedCouncilor,
			// Token: 0x04005F29 RID: 24361
			escapeAlienFleet,
			// Token: 0x04005F2A RID: 24362
			sabotageResearch,
			// Token: 0x04005F2B RID: 24363
			stealTechnology,
			// Token: 0x04005F2C RID: 24364
			firstTransfer,
			// Token: 0x04005F2D RID: 24365
			bombardment,
			// Token: 0x04005F2E RID: 24366
			fireNukeBarrage,
			// Token: 0x04005F2F RID: 24367
			seaLevelRise,
			// Token: 0x04005F30 RID: 24368
			nuclearWinter,
			// Token: 0x04005F31 RID: 24369
			completeNukeProgram,
			// Token: 0x04005F32 RID: 24370
			councilorDeathNatural,
			// Token: 0x04005F33 RID: 24371
			discoverVictory,
			// Token: 0x04005F34 RID: 24372
			researchPherocytes,
			// Token: 0x04005F35 RID: 24373
			researchExotics,
			// Token: 0x04005F36 RID: 24374
			killCouncilorSpace,
			// Token: 0x04005F37 RID: 24375
			completeTrade,
			// Token: 0x04005F38 RID: 24376
			purgeExecutive,
			// Token: 0x04005F39 RID: 24377
			augment,
			// Token: 0x04005F3A RID: 24378
			lowAtrocityWin,
			// Token: 0x04005F3B RID: 24379
			cometBase,
			// Token: 0x04005F3C RID: 24380
			temperatureAnomaly,
			// Token: 0x04005F3D RID: 24381
			exofighterWin,
			// Token: 0x04005F3E RID: 24382
			winBattleOutmatched,
			// Token: 0x04005F3F RID: 24383
			spacebodyPopulation,
			// Token: 0x04005F40 RID: 24384
			officersOnShip,
			// Token: 0x04005F41 RID: 24385
			admiral,
			// Token: 0x04005F42 RID: 24386
			unrestBreakaway,
			// Token: 0x04005F43 RID: 24387
			ramming
		}
	}
}
