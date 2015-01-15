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
				return MainWindow.Модель.ПериодГрафика
					.Count(dateTime => !Усиление || MainWindow.Модель.Усиления.Contains(dateTime));
			}
		}

		public int КоличествоВыходных
		{
			get
			{
				return MainWindow.Модель.ПериодГрафика
					.Where(time => time.Date.DayOfWeek == DayOfWeek.Saturday || time.Date.DayOfWeek == DayOfWeek.Sunday)
					.Count(dateTime => !Усиление || MainWindow.Модель.Усиления.Contains(dateTime));
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

		public override string ToString()
		{
			return string.Format("{0} ({1} часов{2})", Название, Длительность, Усиление ? " только в Усиление" : "");
		}
	}
}