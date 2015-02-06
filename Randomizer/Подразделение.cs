using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Syncfusion.Windows.Shared;

namespace Randomizer
{
	[Serializable]
	public class Подразделение : NotificationObject
	{
		public Подразделение()
		{
			Наряды = new ObservableCollection<Наряд>();
			Распред = new SortedDictionary<int, int>();
		}

		public ObservableCollection<Наряд> Наряды { get; set; }
		public string Название { get; set; }
		public int Люди { get; set; }
		public double Процент { get; set; }
		public SortedDictionary<int, int> Распред { get; set; } 
		public string РаспредЧ { get; set; }
		//public int Распред12Ч { get; set; }
		//public int Распред24Ч { get; set; }
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

		public Dictionary<string, object> Fields
		{
			get
			{
				var dic = new Dictionary<string, object>();
				dic["подразделение"] = Название;
				dic["количество человек"] = Люди;
				dic["% от общего"] = Процент.ToString("P");
				dic["часы на подразделение"] = Часы;
				dic["часы в нерабочие дни"] = ВыходныеЧасы;
				var durat = App.Модель.Наряды.Select(наряд => наряд.Длительность).Distinct().ToList();
				durat.Sort();
				Распред = new SortedDictionary<int, int>();

				var dates = App.Модель.ДатыГрафика.ToArray();
				foreach (var source in dates.SelectMany(графика => графика.Смены).Where(pair => pair.Value == this))
				{
					if (Распред.ContainsKey(source.Key.Длительность))
					{
						Распред[source.Key.Длительность]++;
					}
					else
					{
						Распред[source.Key.Длительность] = 1;
					}
				}

				foreach (var i in durat)
				{
					if (Распред.ContainsKey(i))
					{
						dic["наряды по " + i + "часа"] = Распред[i];
					}
					else
					{
						dic["наряды по " + i + "часа"] = 0;
					}
				}
				//foreach (KeyValuePair<int, int> pair in Распред)
				//{
				//	dic["Распред "+pair.Key+"ч, шт"] = pair.Value;
				//}
				dic["часы на человека"] = Загруженность;
				dic["часы на человека в нерабочие дни"] = ЗагруженностьВыходных;
				dic["погрешность"] = ОтклонениеЗагруженности.ToString("P");
				dic["погрешность в нерабочие дни"] = ОтклонениеЗагруженностиВыходных.ToString("P");
				dic["доступные наряды"] = СписокНарядов;
				return dic;
			}
		}

		protected string GetName<T>(Expression<Func<T>> propertyExpression)
		{
			return (PropertySupport.ExtractPropertyName<T>(propertyExpression));
		}


		public void Refresh()
		{
			Пересчитать();
			RaisePropertyChanged(() => Название);
			RaisePropertyChanged(() => Люди);
			RaisePropertyChanged(() => Наряды);
			RaisePropertyChanged(() => Процент);
			RaisePropertyChanged(() => Распред);
			RaisePropertyChanged(() => РаспредЧ);
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

			//Распред4Ч = dates
			//		.SelectMany(графика => графика.Смены)
			//		.Count(pair => pair.Key.Длительность == 4 && pair.Value == this);

			//Распред12Ч = dates
			//		.SelectMany(графика => графика.Смены)
			//		.Count(pair => pair.Key.Длительность == 12 && pair.Value == this);

			//Распред24Ч = dates
			//		.SelectMany(графика => графика.Смены)
			//		.Count(pair => pair.Key.Длительность == 24 && pair.Value == this);

			Распред = new SortedDictionary<int, int>();

			foreach (var source in dates.SelectMany(графика => графика.Смены).Where(pair => pair.Value == this))
			{
				if (Распред.ContainsKey(source.Key.Длительность))
				{
					Распред[source.Key.Длительность]++;
				}
				else
				{
					Распред[source.Key.Длительность] = 1;
				}
			}

			РаспредЧ = "";
			foreach (KeyValuePair<int, int> pair in Распред)
			{
				РаспредЧ += pair.Key + "ч=" + pair.Value + "н; ";
			}
			РаспредЧ = РаспредЧ.Trim();

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