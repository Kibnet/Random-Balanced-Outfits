using System;
using System.ComponentModel;
using System.Linq;
using Randomizer.Annotations;

namespace Randomizer
{
	[Serializable]
	public class Наряд
	{
		public string Название { get; set; }

		public int Длительность { get; set; }


		public int Количество
		{
			get
			{
				var count = App.Модель.ПериодГрафика
					.Count(time => !Усиление || App.Модель.Усиления.Contains(time));

				if (!App.Модель.ИсключитьБлокированные) return count;
				var blocked = App.Модель.ДатыГрафика.SelectMany(gr => gr.Блокировки)
					.Count(наряд => наряд == this);
				count -= blocked;
				return count;
			}
		}

		public int КоличествоВыходных
		{
			get
			{
				var count = App.Модель.ПериодГрафика
					.Where(time => App.Модель.Праздники.Contains(time))
					.Count(time => !Усиление || App.Модель.Усиления.Contains(time));
				if (!App.Модель.ИсключитьБлокированные) return count;

				var blocked = App.Модель.ДатыГрафика
					.Where(time => App.Модель.Праздники.Contains(time.Date))
					.SelectMany(gr => gr.Блокировки)
					.Count(наряд => наряд == this);
				count -= blocked;
				return count;
			}
		}

		public int Всего
		{
			get { return Количество*Длительность; }
		}

		public int Выходных
		{
			get { return КоличествоВыходных * Длительность; }
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
		Все,Будние,Выходные
	}
}