using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.Systems
{
	// Token: 0x02000A0B RID: 2571
	public class GameTimeCondition
	{
		// Token: 0x060063E6 RID: 25574 RVA: 0x002F325E File Offset: 0x002F145E
		public static GameTimeCondition Daily0000(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily0000, now);
		}

		// Token: 0x060063E7 RID: 25575 RVA: 0x002F3267 File Offset: 0x002F1467
		public static GameTimeCondition Daily0300(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily0300, now);
		}

		// Token: 0x060063E8 RID: 25576 RVA: 0x002F3270 File Offset: 0x002F1470
		public static GameTimeCondition Daily0600(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily0600, now);
		}

		// Token: 0x060063E9 RID: 25577 RVA: 0x002F3279 File Offset: 0x002F1479
		public static GameTimeCondition Daily0900(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily0900, now);
		}

		// Token: 0x060063EA RID: 25578 RVA: 0x002F3282 File Offset: 0x002F1482
		public static GameTimeCondition Daily1030(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily1030, now);
		}

		// Token: 0x060063EB RID: 25579 RVA: 0x002F328B File Offset: 0x002F148B
		public static GameTimeCondition Daily1200(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily1200, now);
		}

		// Token: 0x060063EC RID: 25580 RVA: 0x002F3294 File Offset: 0x002F1494
		public static GameTimeCondition Daily1500(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily1500, now);
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x002F329D File Offset: 0x002F149D
		public static GameTimeCondition Daily1800(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily1800, now);
		}

		// Token: 0x060063EE RID: 25582 RVA: 0x002F32A6 File Offset: 0x002F14A6
		public static GameTimeCondition Daily2100(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily2100, now);
		}

		// Token: 0x060063EF RID: 25583 RVA: 0x002F32AF File Offset: 0x002F14AF
		public static GameTimeCondition Daily2300(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Daily2300, now);
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x002F32B9 File Offset: 0x002F14B9
		public static GameTimeCondition Monthly(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.Monthly, now);
		}

		// Token: 0x060063F1 RID: 25585 RVA: 0x002F32C3 File Offset: 0x002F14C3
		public static GameTimeCondition MidMonthly(DateTime now)
		{
			return new GameTimeCondition(GameTimeCondition.Condition.MidMonthly, now);
		}

		// Token: 0x060063F2 RID: 25586 RVA: 0x002F32D0 File Offset: 0x002F14D0
		private GameTimeCondition(GameTimeCondition.Condition condition, DateTime now)
		{
			this.condition = condition;
			switch (condition)
			{
			case GameTimeCondition.Condition.Daily0000:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily0300:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 3, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily0600:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily0900:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily1030:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 10, 30, 0);
				break;
			case GameTimeCondition.Condition.Daily1200:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily1500:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily1800:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily2100:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0);
				break;
			case GameTimeCondition.Condition.Daily2300:
				this.nextUpdate = new DateTime(now.Year, now.Month, now.Day, 23, 0, 0);
				break;
			case GameTimeCondition.Condition.Monthly:
				this.nextUpdate = new DateTime(now.Year, now.Month, 1);
				break;
			case GameTimeCondition.Condition.MidMonthly:
				this.nextUpdate = new DateTime(now.Year, now.Month, 15);
				break;
			}
			this.Satisfied(now);
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x002F34F8 File Offset: 0x002F16F8
		public bool Satisfied(DateTime now)
		{
			if (!(now > this.nextUpdate))
			{
				return false;
			}
			GameTimeCondition.Condition condition = this.condition;
			if (condition <= GameTimeCondition.Condition.Daily2300)
			{
				this.nextUpdate = this.nextUpdate.AddDays(1.0);
				return true;
			}
			if (condition - GameTimeCondition.Condition.Monthly > 1)
			{
				throw new Exception("unhandled SimTime Condition");
			}
			this.nextUpdate = this.nextUpdate.AddMonths(1);
			return true;
		}

		// Token: 0x040046AE RID: 18094
		private GameTimeCondition.Condition condition;

		// Token: 0x040046AF RID: 18095
		private DateTime nextUpdate;

		// Token: 0x040046B0 RID: 18096
		public const int MIDMONTHDAY = 15;

		// Token: 0x020013C1 RID: 5057
		private enum Condition
		{
			// Token: 0x040072B1 RID: 29361
			Daily0000,
			// Token: 0x040072B2 RID: 29362
			Daily0300,
			// Token: 0x040072B3 RID: 29363
			Daily0600,
			// Token: 0x040072B4 RID: 29364
			Daily0900,
			// Token: 0x040072B5 RID: 29365
			Daily1030,
			// Token: 0x040072B6 RID: 29366
			Daily1200,
			// Token: 0x040072B7 RID: 29367
			Daily1500,
			// Token: 0x040072B8 RID: 29368
			Daily1800,
			// Token: 0x040072B9 RID: 29369
			Daily2100,
			// Token: 0x040072BA RID: 29370
			Daily2300,
			// Token: 0x040072BB RID: 29371
			Monthly,
			// Token: 0x040072BC RID: 29372
			MidMonthly
		}
	}
}
