using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer
{
	[Serializable]
	public class Наряд
	{
		public string Название { get; set; }
		public int Длительность { get; set; }

		public IEnumerable<DateTime> ДниНаряда
		{
			get
			{
				return App.Модель.ПериодГрафика
					.Where(date => Дни == WeekDays.Все
					               || (Дни == WeekDays.Выходные && App.Модель.Праздники.Contains(date))
					               || (Дни == WeekDays.Будние && !App.Модель.Праздники.Contains(date)));
			}
		}

		public int Количество
		{
			get
			{
				var count = ДниНаряда.Count(time => !Усиление || App.Модель.Усиления.Contains(time));


				if (!App.Модель.ИсключитьБлокированные) return count;
				var blocked = App.Модель.ДатыГрафика
					.Where(date => Дни == WeekDays.Все
					               || (Дни == WeekDays.Выходные && App.Модель.Праздники.Contains(date.Date))
					               || (Дни == WeekDays.Будние && !App.Модель.Праздники.Contains(date.Date)))
					.SelectMany(gr => gr.Блокировки)
					.Count(наряд => наряд == this);
				count -= blocked;
				return count;
			}
		}

		public int РаспределеноКоличество
		{
			get
			{
				var cnt = App.Модель.ДатыГрафика
					.SelectMany(gr => gr.Смены)
					.Count(nar => nar.Key == this && App.Модель.Подразделения.Contains(nar.Value));
				return cnt;
			}
		}

		public int КоличествоВыходных
		{
			get
			{
				//var nars = ДниНаряда.Where(time => App.Модель.Праздники.Contains(time));
				//var count =	nars.Count(time => !Усиление || App.Модель.Усиления.Contains(time));
				//if (!App.Модель.ИсключитьБлокированные) return count;
				var i = 0;
				foreach (var data in App.Модель.ДатыГрафика.Where(time => App.Модель.Праздники.Contains(time.Date)))
				{
					if (App.Модель.ИсключитьБлокированные)
					{
						if (data.Блокировки.Contains(this))
							continue;
					}
					if (data.Смены.ContainsKey(this))
					{
						i++;
					}
				}

				//var cnt = App.Модель.ДатыГрафика
				//	.Where(time => App.Модель.Праздники.Contains(time.Date))
				//	.SelectMany(gr => gr.Подразделения)
				//	.Count(nar => nar.Nar == this && (!App.Модель.ИсключитьБлокированные || !nar.Locked));
				return i;


				//var blocked = App.Модель.ДатыГрафика
				//	.Where(date => (Дни == WeekDays.Выходные && App.Модель.Праздники.Contains(date.Date)))
				//	.Where(time => App.Модель.Праздники.Contains(time.Date))
				//	.SelectMany(gr => gr.Блокировки)
				//	.Count(наряд => наряд == this);
				//count -= blocked;
				//return count;
			}
		}

		public int РаспределеноКоличествоВыходных
		{
			get
			{
				var cnt = App.Модель.ДатыГрафика
					.Where(time => App.Модель.Праздники.Contains(time.Date))
					.SelectMany(gr => gr.Смены)
					.Count(nar => nar.Key == this && App.Модель.Подразделения.Contains(nar.Value));
				return cnt;
			}
		}

		public int Всего
		{
			get { return Количество*Длительность; }
		}

		public int Выходных
		{
			get { return КоличествоВыходных*Длительность; }
		}

		public bool Усиление { get; set; }
		public WeekDays Дни { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1} часов{2})", Название, Длительность, Усиление ? " только в Усиление" : "");
		}
	}

	public enum WeekDays
	{
		Все,
		Будние,
		Выходные
	}
}