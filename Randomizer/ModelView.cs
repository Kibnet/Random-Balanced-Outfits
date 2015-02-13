using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows;
using Syncfusion.Windows.Shared;
using Brushes = System.Windows.Media.Brushes;

namespace Randomizer
{
	public class ModelView : NotificationObject
	{
		private НастройкиГенератора настройки;
		private bool _isBusy;
		private string _status;

		public string SettingsName = "Настройки.bin";

		public ModelView()
		{
			Deserialize(SettingsName);
			IsBusy = false;
			foreach (var pod in Подразделения)
			{
				pod.Marked = false;
			}
		}

		public ModelView(string filename)
		{
			Deserialize(filename);
			foreach (var pod in Подразделения)
			{
				pod.Marked = false;
			}
		}

		public void Deserialize(string filename)
		{
			настройки = НастройкиГенератора.Deserialize(filename);
		}

		public ObservableCollection<Наряд> Наряды
		{
			get
			{
				return настройки.Наряды;
			}
			set
			{
				if (Equals(value, настройки.Наряды))
				{
					return;
				}
				настройки.Наряды = value;
				RaisePropertyChanged(() => Наряды);
			}
		}

		public ObservableCollection<Подразделение> Подразделения
		{
			get { return настройки.Подразделения; }
			set
			{
				if (Equals(value, настройки.Подразделения))
				{
					return;
				}
				настройки.Подразделения = value;
				RaisePropertyChanged(() => Подразделения);
			}
		}

		public ObservableCollection<DateTime> Усиления
		{
			get { return настройки.Усиления; }
			set
			{
				if (Equals(value, настройки.Усиления))
				{
					return;
				}
				настройки.Усиления = value;
				RaisePropertyChanged(() => Усиления);
			}
		}

		public ObservableCollection<DateTime> ПериодГрафика
		{
			get { return настройки.ПериодГрафика; }
			set
			{
				if (Equals(value, настройки.ПериодГрафика))
				{
					return;
				}
				настройки.ПериодГрафика = value;
				RaisePropertyChanged(() => ПериодГрафика);
			}
		}

		public ObservableCollection<DateTime> Праздники
		{
			get { return настройки.Праздники; }
			set
			{
				if (Equals(value, настройки.Праздники))
				{
					return;
				}
				настройки.Праздники = value;
				RaisePropertyChanged(() => Праздники);
			}
		}

		public ObservableCollection<ДатаГрафика> ДатыГрафика
		{
			get { return настройки.ДатыГрафика; }
			set
			{
				if (Equals(value, настройки.ДатыГрафика))
				{
					return;
				}
				настройки.ДатыГрафика = value;
				RaisePropertyChanged(() => ДатыГрафика);
			}
		}

		public string ПутьСохранения
		{
			get { return настройки.ПутьСохранения; }
			set
			{
				if (value == настройки.ПутьСохранения)
				{
					return;
				}
				настройки.ПутьСохранения = value;
				RaisePropertyChanged(() => ПутьСохранения);
			}
		}

		public double ОбщаяНагрузка
		{
			get
			{
				return App.Модель.ДлительностьНарядов /
							  (double)App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);
			}
		}

		public double ОбщаяНагрузкаВыходных
		{
			get
			{
				return App.Модель.ДлительностьВыходныхНарядов /
							   (double)App.Модель.Подразделения.Sum(подразделение => подразделение.Люди);
			}
		}

		public string Распределено
		{
			get
			{
				return string.Format("Распределено {0} часов из {1}. Общая нагрузка {2} ч/чел., на выходных {3} ч/чел.",
					ДлительностьРаспределеныхНарядов, ДлительностьНарядов, ОбщаяНагрузка.ToString("F2"), ОбщаяНагрузкаВыходных.ToString("F2"));
			}
		}

		public int ДлительностьРаспределеныхНарядов
		{
			get { return Подразделения.Sum(подразделение => подразделение.Часы); }
		}

		public string ВсегоНарядов
		{
			get
			{
				RaisePropertyChanged(() => Подразделения);
				RaisePropertyChanged(() => Наряды);
				RaisePropertyChanged(() => ДлительностьНарядов);
				RaisePropertyChanged(() => ДлительностьВыходныхНарядов);
				RaisePropertyChanged(() => КоэффициэнтВыходных);
				RaisePropertyChanged(() => КоличествоНарядов);
				RaisePropertyChanged(() => КоличествоВыходныхНарядов);
				return string.Format("Всего {3} шт. ({0} часов) из них {4} шт. ({1} часов) выходных ({2}%)",
					ДлительностьНарядов,
					ДлительностьВыходныхНарядов,
					(КоэффициэнтВыходных * 100).ToString("F1"),
					КоличествоНарядов,
					КоличествоВыходныхНарядов);
			}
		}

		public double КоэффициэнтВыходных
		{
			get
			{
				var hproc = 0d;
				if (ДлительностьНарядов != 0)
				{
					hproc = ДлительностьВыходныхНарядов / (double)ДлительностьНарядов;
				}
				return hproc;
			}
		}

		public int ДлительностьНарядов
		{
			get { return Наряды.Sum(наряд => наряд.Всего); }
		}

		public int КоличествоНарядов
		{
			get { return Наряды.Sum(наряд => наряд.Количество); }
		}

		public int ДлительностьВыходныхНарядов
		{
			get { return Наряды.Sum(наряд => наряд.Выходных); }
		}

		public int КоличествоВыходныхНарядов
		{
			get { return Наряды.Sum(наряд => наряд.КоличествоВыходных); }
		}

		public DataTable GenerateTable
		{
			get
			{
				var table = new DataTable();
				try
				{
					table.Columns.Add("Дата", typeof(string));
					foreach (var naryad in Наряды)
					{
						table.Columns.Add(naryad.Название);
					}
					foreach (var eventDate in ДатыГрафика)
					{
						var row = table.NewRow();
						var obj = new List<object>
						{
							eventDate.Date.ToString("yyyy.MM.dd ddd")
						};
						foreach (var naryad in Наряды)
						{
							var add = "---";
							if (eventDate.Смены.ContainsKey(naryad))
							{
								add = eventDate.Смены[naryad] == null ? "" : eventDate.Смены[naryad].Название;
							}
							obj.Add(add);
						}
						row.ItemArray = obj.ToArray();
						table.Rows.Add(row);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(string.Format("{0}\n{1}", ex.Message, ex.StackTrace), "Исключение");
				}
				return table;
			}
		}

		public bool ИсключитьБлокированные
		{
			get { return настройки.ИсключитьБлокированные; }
			set
			{
				if (value.Equals(настройки.ИсключитьБлокированные)) return;
				настройки.ИсключитьБлокированные = value;
				RaisePropertyChanged(() => ИсключитьБлокированные);
			}
		}

		public double Zoom
		{
			get { return настройки.Zoom; }
			set
			{
				if (value.Equals(настройки.Zoom))
				{
					return;
				}
				настройки.Zoom = value;
				RaisePropertyChanged(() => Zoom);
			}
		}

		public string ИтогоДаты
		{
			get
			{
				return string.Format("Всего {0} {3}, из них {1} выходных {4} и {2} {5} усиления",
					ПериодГрафика.Count, Праздники.Count, Усиления.Count, Day(ПериодГрафика.Count), Day(Праздники.Count), Day(Усиления.Count));
			}
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				_isBusy = value;
				RaisePropertyChanged(() => IsBusy);
			}
		}

		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				RaisePropertyChanged(() => Status);
			}
		}

		public string Day(int day)
		{
			switch (day)
			{
				case 11:
				case 12:
				case 13:
				case 14:
					return "дней";
				default:
					{
						var ost = day % 10;
						switch (ost)
						{
							case 1:
								return "день";
							case 2:
							case 3:
							case 4:
								return "дня";
							default:
								return "дней";
						}
					}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void ClearEvents()
		{
			//Формирование всех дат и доступных нарядов в них
			var dates = ПериодГрафика.Select(date => new ДатаГрафика
			{
				Date = date,
				Смены =
					Наряды.Where(nar => !nar.Усиление || Усиления.Contains(date))
						.Where(nar => nar.Дни == WeekDays.Все
									  || (nar.Дни == WeekDays.Выходные && Праздники.Contains(date))
									  || (nar.Дни == WeekDays.Будние && !Праздники.Contains(date)))
						.ToDictionary<Наряд, Наряд, Подразделение>(nar => nar, nar => null)
			}).ToList();
			//Заполнение заблокированных нарядов
			foreach (var date in dates)
			{
				var find = ДатыГрафика.FirstOrDefault(gr => gr.Date == date.Date);
				if (find == null) continue;
				foreach (var nar in find.Блокировки)
				{
					date.Блокировки.Add(nar);
					if (!find.Смены.ContainsKey(nar)) continue;
					if (nar.Усиление && !Усиления.Contains(date.Date)) continue;
					if (Подразделения.Contains(find.Смены[nar]))
					{
						date.Смены[nar] = find.Смены[nar];
					}
				}
			}

			//Перезапись старого графика новым
			ДатыГрафика = new ObservableCollection<ДатаГрафика>(dates);
			ПересчитатьПодразделения();
		}

		public void GenerateEvents()
		{
			ObservableCollection<ДатаГрафика> best = null;
			double bestres = 0;
			for (int i = 0; i < 100; i++)
			{

				ClearEvents();
				var rand = new Random();

				//Формирование конкретных нарядов на выходные
				var holypairs = new List<KeyValuePair<ДатаГрафика, Наряд>>();
				foreach (var day in ДатыГрафика)
				{
					var day1 = day;
					foreach (var smen in day.Смены
						.Where(pair => !day1.Блокировки.Contains(pair.Key) && pair.Value == null && day1.Holyday)
						.Select(pair => pair.Key))
					{
						holypairs.Insert(rand.Next(holypairs.Count), (new KeyValuePair<ДатаГрафика, Наряд>(day, smen)));
					}
				}

				//Сортировка нарядов по длительности в порядке убывания
				holypairs.Sort((pair, valuePair) => valuePair.Value.Длительность.CompareTo(pair.Value.Длительность));

				РаспределитьНаряды(holypairs);

				//Формирование конкретных нарядов
				var pairs = new List<KeyValuePair<ДатаГрафика, Наряд>>();
				foreach (var day in ДатыГрафика)
				{
					var day1 = day;
					foreach (var smen in day.Смены
						.Where(pair => !day1.Блокировки.Contains(pair.Key) && pair.Value == null)
						.Select(pair => pair.Key))
					{
						pairs.Insert(rand.Next(pairs.Count), (new KeyValuePair<ДатаГрафика, Наряд>(day, smen)));
					}
				}

				//Сортировка нарядов по длительности в порядке убывания
				pairs.Sort((pair, valuePair) => valuePair.Value.Длительность.CompareTo(pair.Value.Длительность));

				РаспределитьНаряды(pairs);

				//Выборка лучшего случая
				if (best == null)
				{
					best = ДатыГрафика;
					if (Подразделения.Count == 0)
					{
						break;
					}
					bestres = Math.Abs(Подразделения.Max(dist => dist.ОтклонениеЗагруженности)) +
							  Math.Abs(Подразделения.Min(dist => dist.ОтклонениеЗагруженности));
				}
				else
				{
					var res = Math.Abs(Подразделения.Max(dist => dist.ОтклонениеЗагруженности)) +
							  Math.Abs(Подразделения.Min(dist => dist.ОтклонениеЗагруженности));
					if (res < bestres)
					{
						best = ДатыГрафика;
						ПересчитатьПодразделения();
						bestres = res;
					}
				}
			}
			ДатыГрафика = best;
			Раскидать();
		}

		/// <summary>
		///     Раскидывание подразделений по разным дням
		///     Чтобы в один день не было в наряде несколько человек с одного подразделения
		/// </summary>
		public void Раскидать()
		{
			//Счётчик сделаных замен
			var count = 0;

			do
			{
				count = 0;
				var collisions = new Dictionary<ДатаГрафика, Dictionary<Наряд, Подразделение>>();

				//Ищем повторы в днях
				foreach (var day in ДатыГрафика)
				{
					//Берём все заполненные наряды
					var nars = day.Смены.Where(pair => pair.Value != null).ToList();
					//Сортируем по имени подразделения, чтобы легко найти дубликаты
					nars.Sort((p1, p2) => String.Compare(p1.Value.Название, p2.Value.Название, StringComparison.Ordinal));
					var last = new KeyValuePair<Наряд, Подразделение>();
					foreach (var nar in nars)
					{
						//Если два подряд одинаковые
						if (nar.Value == last.Value && last.Value != null)
						{
							if (!collisions.ContainsKey(day))
							{
								collisions[day] = new Dictionary<Наряд, Подразделение>();
							}
							//Добавляем в словарь повторов
							collisions[day][last.Key] = last.Value;
							collisions[day][nar.Key] = nar.Value;
						}
						last = nar;
					}
				}


				//Пробегаем по повторам
				foreach (var collision in collisions)
				{
					//Выделяем подобные дни
					var days = ДатыГрафика.Where(day => day.Holyday == collision.Key.Holyday).ToList();
					//Выделяем подразделения с повторами
					foreach (var district in collision.Value.Select(vp => vp.Value).Distinct().ToList())
					{
						var finded = false;
						//Выделяем наряды в которые в этот день идёт конкретное подразделение
						var naryads = collision.Value.Where(vp => vp.Value == district).Select(vp => vp.Key).ToList();
						//Выделяем дни в которые данное подразделение ещё не стоит в наряде
						foreach (var vday in days.Where(d => !d.Смены.ContainsValue(district)).ToList())
						{
							//Проверяем каждый день до первой замены
							if (finded) break;
							foreach (var nar in naryads)
							{
								//Ищем наряд в который уже идёт какое-нибудь подразделение
								var podob = vday.Смены.Where(vp => vp.Value != null
									//Такой же длительности
																   && vp.Key.Длительность == nar.Длительность
									//И подразделение которое в него идёт не идёт в выбранный день
																   && !collision.Key.Смены.ContainsValue(vp.Value)
									//И подразделение которое в него идёт может пойти в выбранный наряд
																   && vp.Value.Наряды.Contains(nar)).ToList();

								if (podob.Count <= 0) continue;
								finded = true;
								count++;
								//Меняем наряды местами
								collision.Key.Смены[nar] = podob[0].Value;
								vday.Смены[podob[0].Key] = district;
								break;
							}
						}
					}
				}
			} while (count > 0); //Пока есть что заменять
		}

		/// <summary>
		///     Распределение нарядов по подразделениям
		/// </summary>
		/// <param name="pairs">Список пар даты и наряда</param>
		private void РаспределитьНаряды(List<KeyValuePair<ДатаГрафика, Наряд>> pairs)
		{
			foreach (var datpair in pairs)
			{
				var dict = new Dictionary<Подразделение, double>();
				var datpair1 = datpair;
				foreach (var d in Подразделения.Where(nar => nar.Наряды.Contains(datpair1.Value)))
				{
					//Для каждого подразделения которое может пойти в этот наряд
					var krat = (double)datpair1.Value.Длительность;
					//Вычисляется нагрузка которая будет если оно пойдёт
					var load = (d.Часы + krat) / d.Люди;
					dict[d] = load;
				}
				//Выбирается подразделение с самой низкой вычисленной нагрузкой
				if (dict.Count == 0)
				{
					return;
				}
				var min = dict.Min(pair => pair.Value);
				var first = dict.First(pair => pair.Value <= min).Key;
				//Это подразделение берёт наряд
				datpair1.Key.Смены[datpair1.Value] = first;
				first.Пересчитать();
			}
		}

		public void RefreshTable()
		{
			RaisePropertyChanged(() => ПериодГрафика);
			RaisePropertyChanged(() => Усиления);
			RaisePropertyChanged(() => Праздники);
			RaisePropertyChanged(() => Наряды);
			RaisePropertyChanged(() => Подразделения);
			RaisePropertyChanged(() => ДатыГрафика);
			//RaisePropertyChanged(() => GenerateTable);
		}

		public List<Подразделение> ColorizeList = new List<Подразделение>();

		public void ColorizeGrafic()
		{
			foreach (var date in ДатыГрафика)
			{
				date.Refresh();
			}
		}

		public void ОбновитьНаряды()
		{
			foreach (var наряд in Наряды)
			{
				наряд.Refresh();
			}
		}

		public void ОбновитьПодразделения()
		{
			foreach (var подразделение in Подразделения)
			{
				подразделение.Refresh();
			}
		}

		public void ОбновитьДаты()
		{
			foreach (var датаГрафика in ДатыГрафика)
			{
				датаГрафика.Refresh();
			}
		}

		public void ПересчитатьПодразделения()
		{
			foreach (var подразделение in Подразделения)
			{
				подразделение.Пересчитать();
			}
		}

		public void Serialize(string filename)
		{
			настройки.Serialize(filename);
		}

	}
}