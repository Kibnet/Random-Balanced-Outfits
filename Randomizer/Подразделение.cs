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

		public string Процент
		{
			get
			{
				return ((Люди*100.0f/App.Модель.Подразделения.Sum(подразделение => подразделение.Люди))).ToString("F1") + "%";
			}
		}

		public int Распред12Ч
		{
			get
			{
				return App.Модель.ДатыГрафика
					.SelectMany(графика => графика.Смены)
					.Count(pair => pair.Key.Длительность == 12 && pair.Value == this);
			}
		}

		public int Распред24Ч
		{
			get
			{
				return App.Модель.ДатыГрафика
					.SelectMany(графика => графика.Смены)
					.Count(pair => pair.Key.Длительность == 24 && pair.Value == this);
			}
		}

		public int Часы
		{
			get
			{
				return App.Модель.ДатыГрафика
					.SelectMany(графика => графика.Смены)
					.Where(pair => pair.Value == this)
					.Sum(pair => pair.Key.Длительность);
			}
		}

		public int ВыходныеЧасы
		{
			get
			{
				return App.Модель.ДатыГрафика
					.Where(графика => графика.Holyday)
					.SelectMany(графика => графика.Смены)
					.Where(pair => pair.Value == this)
					.Sum(pair => pair.Key.Длительность);
			}
		}

		public double Загруженность
		{
			get { return Math.Round(((double) Часы/Люди)*100)/100; }
		}

		public double ЗагруженностьВыходных
		{
			get { return Math.Round(((double) ВыходныеЧасы/Люди)*100)/100; }
		}

		public double ОтклонениеЗагруженности
		{
			get
			{
				if (App.Модель.ОбщаяНагрузка>0)
				{

					return (Загруженность - App.Модель.ОбщаяНагрузка) / App.Модель.ОбщаяНагрузка;
				}
				else
				{
					return 0;
				}
			}
		}
		public double ОтклонениеЗагруженностиВыходных
		{
			get
			{
				if (App.Модель.ОбщаяНагрузкаВыходных > 0)
				{

					return (ЗагруженностьВыходных - App.Модель.ОбщаяНагрузкаВыходных) / App.Модель.ОбщаяНагрузкаВыходных;
				}
				else
				{
					return 0;
				}
			}
		}

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
			RaisePropertyChanged(() => Название);
			RaisePropertyChanged(() => Люди);
			RaisePropertyChanged(() => Наряды);
			RaisePropertyChanged(() => Процент);
			RaisePropertyChanged(() => Распред12Ч);
			RaisePropertyChanged(() => Распред24Ч);
			RaisePropertyChanged(() => СписокНарядов);
			RaisePropertyChanged(() => Часы);
			RaisePropertyChanged(() => ВыходныеЧасы);
			RaisePropertyChanged(() => Загруженность);
			RaisePropertyChanged(() => Загруженность);
			RaisePropertyChanged(() => ОтклонениеЗагруженности);
			RaisePropertyChanged(() => ОтклонениеЗагруженностиВыходных);
		}
	}
}