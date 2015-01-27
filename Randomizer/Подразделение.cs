using System;
using System.Collections.ObjectModel;
using System.Linq;
using Syncfusion.Windows.Shared;

namespace Randomizer
{
	[Serializable]
	public class Подразделение : NotificationObject
	{
		public Подразделение()
		{
			Наряды = new ObservableCollection<Наряд>();
		}

		public ObservableCollection<Наряд> Наряды { get; set; }
		public string Название { get; set; }
		public int Люди { get; set; }
		public double Процент { get; set; }
		public int Распред4Ч { get; set; }
		public int Распред12Ч { get; set; }
		public int Распред24Ч { get; set; }
		public int Часы { get; set; }
		public int ВыходныеЧасы { get; set; }


		public double Загруженность
		{
			get { return Math.Round((double)Часы / Люди * 100) / 100; }
		}



		public double ЗагруженностьВыходных
		{
			get { return Math.Round((double)ВыходныеЧасы / Люди * 100) / 100; }
		}

		public double ОтклонениеЗагруженности { get; set; }
		public double ОтклонениеЗагруженностиВыходных { get; set; }

		public string СписокНарядов
		{
			get
			{
				return Наряды != null
					? Наряды.Aggregate("", (current, наряд) => string.Format("{0}{1}; ", current, наряд.Название))
					: "";
			}
		}

		public override string ToString()
		{
			return Название;
		}

		public void Refresh()
		{
			Пересчитать();
			RaisePropertyChanged(() => Название);
			RaisePropertyChanged(() => Люди);
			RaisePropertyChanged(() => Наряды);
			RaisePropertyChanged(() => Процент);
			RaisePropertyChanged(() => Распред4Ч);
			RaisePropertyChanged(() => Распред12Ч);
			RaisePropertyChanged(() => Распред24Ч);
			RaisePropertyChanged(() => СписокНарядов);
			RaisePropertyChanged(() => Часы);
			RaisePropertyChanged(() => ВыходныеЧасы);
			RaisePropertyChanged(() => Загруженность);
			RaisePropertyChanged(() => ЗагруженностьВыходных);
			RaisePropertyChanged(() => ОтклонениеЗагруженности);
			RaisePropertyChanged(() => ОтклонениеЗагруженностиВыходных);
		}

		public void Пересчитать()
		{

			Процент = (double)Люди / App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);

			var dates = App.Модель.ДатыГрафика.ToArray();

			Распред4Ч = dates
					.SelectMany(графика => графика.Смены)
					.Count(pair => pair.Key.Длительность == 4 && pair.Value == this);

			Распред12Ч = dates
					.SelectMany(графика => графика.Смены)
					.Count(pair => pair.Key.Длительность == 12 && pair.Value == this);

			Распред24Ч = dates
					.SelectMany(графика => графика.Смены)
					.Count(pair => pair.Key.Длительность == 24 && pair.Value == this);

			Часы = dates
					.SelectMany(графика => графика.Смены)
					.Where(pair => pair.Value == this)
					.Sum(pair => pair.Key.Длительность);

			ВыходныеЧасы = dates
					.Where(графика => графика.Holyday)
					.SelectMany(графика => графика.Смены)
					.Where(pair => pair.Value == this)
					.Sum(pair => pair.Key.Длительность);

			ОтклонениеЗагруженности = App.Модель.ОбщаяНагрузка > 0
					? (((double)Часы / Люди) - App.Модель.ОбщаяНагрузка) / App.Модель.ОбщаяНагрузка
					: 0;

			ОтклонениеЗагруженностиВыходных = App.Модель.ОбщаяНагрузкаВыходных > 0
					? (((double)ВыходныеЧасы / Люди) - App.Модель.ОбщаяНагрузкаВыходных) / App.Модель.ОбщаяНагрузкаВыходных
					: 0;
		}
	}
}