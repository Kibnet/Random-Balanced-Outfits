using System;
using System.Collections.Generic;
using System.Linq;
using Syncfusion.Windows.Shared;

namespace Randomizer
{
	[Serializable]
	public class Наряд:NotificationObject
	{
		public string Название { get; set; }
		public int Длительность { get; set; }
		public bool Усиление { get; set; }
		public WeekDays Дни { get; set; }

		public void Refresh()
		{
			Пересчитать();
			RaisePropertyChanged(() => Количество);
			RaisePropertyChanged(() => Выходных);
			RaisePropertyChanged(() => Длительность);
			RaisePropertyChanged(() => Дни);
			//RaisePropertyChanged(() => ДниНаряда);
			RaisePropertyChanged(() => КоличествоВыходных);
			RaisePropertyChanged(() => Название);
			RaisePropertyChanged(() => РаспределеноКоличество);
			RaisePropertyChanged(() => РаспределеноКоличествоВыходных);
			RaisePropertyChanged(() => Усиление);
			RaisePropertyChanged(() => Всего);
		}

		public void Пересчитать()
		{
			//ДниНаряда= App.Модель.ПериодГрафика
			//		.Where(date => Дни == WeekDays.Все
			//					   || (Дни == WeekDays.Выходные && App.Модель.Праздники.Contains(date))
			//					   || (Дни == WeekDays.Будние && !App.Модель.Праздники.Contains(date)));

			//Количество = ДниНаряда.Count(time => !Усиление || App.Модель.Усиления.Contains(time));

			var dates = App.Модель.ДатыГрафика.ToArray();
			Количество = dates
				.Count(data => (!App.Модель.ИсключитьБлокированные || !data.Блокировки.Contains(this)) && data.Смены.ContainsKey(this));


			if (App.Модель.ИсключитьБлокированные)
			{
				var blocked = dates
					.Where(date => Дни == WeekDays.Все
					               || (Дни == WeekDays.Выходные && App.Модель.Праздники.Contains(date.Date))
					               || (Дни == WeekDays.Будние && !App.Модель.Праздники.Contains(date.Date)))
					.SelectMany(gr => gr.Блокировки)
					.Count(наряд => наряд == this);
				Количество -= blocked;
			}

			РаспределеноКоличество = dates
					.SelectMany(time => time.Смены)
					.Count(nar => nar.Key == this && App.Модель.Подразделения.Contains(nar.Value));

			КоличествоВыходных = dates
				.Where(time => App.Модель.Праздники.Contains(time.Date))
				.Count(data => (!App.Модель.ИсключитьБлокированные || !data.Блокировки.Contains(this)) && data.Смены.ContainsKey(this));
			
			РаспределеноКоличествоВыходных = dates
					.Where(time => App.Модель.Праздники.Contains(time.Date))
					.SelectMany(time => time.Смены)
					.Count(nar => nar.Key == this && App.Модель.Подразделения.Contains(nar.Value));
		}

		//public IEnumerable<DateTime> ДниНаряда { get; set; }

		public int Количество { get; set; }

		public int РаспределеноКоличество { get; set; }

		public int КоличествоВыходных { get; set; }

		public int РаспределеноКоличествоВыходных { get; set; }

		public int Всего
		{
			get { return Количество*Длительность; }
		}

		public int Выходных
		{
			get { return КоличествоВыходных*Длительность; }
		}

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